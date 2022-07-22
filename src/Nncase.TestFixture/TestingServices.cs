using System;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Autofac;
using Autofac.Extras.CommonServiceLocator;
using CommonServiceLocator;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
// using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Nncase.Transform;

namespace Nncase.TestFixture;

public interface ITestingProvider
{
    /// <summary>
    /// get the nncase `tests_ouput` path
    /// <remarks>
    /// you can set the subPath for get the `xxx/tests_output/subPath`
    /// </remarks>
    /// </summary>
    /// <param name="subDir">sub directory.</param>
    /// <returns> full path string. </returns>
    public string GetDumpDirPath(string subDir);
}

internal sealed class TestingProvider : ITestingProvider
{
    private readonly IDumpDirPathProvider _dumpDirPathProvider;

    public TestingProvider(IDumpDirPathProvider dumpDirPathProvider)
    {
        _dumpDirPathProvider = dumpDirPathProvider;
    }

    /// <inheritdoc/>
    public string GetDumpDirPath(string subDir) => _dumpDirPathProvider.GetDumpDirPath(subDir);
}

public static class Testing
{
    private static ITestingProvider? _provider;

    private static ITestingProvider Provider => _provider ?? throw new InvalidOperationException("Testing services provider must be set.");

    /// <summary>
    /// Configure testing services.
    /// </summary>
    /// <param name="provider">Service provider.</param>
    public static void Configure(ITestingProvider provider)
    {
        _provider = provider;
    }

    /// <summary>
    /// the fixed rand generator, maybe need impl by each module.
    /// </summary>
    public static readonly Random RandGenerator = new System.Random(123);

    /// <summary>
    /// get the nncase `tests_ouput` path
    /// <remarks>
    /// you can set the subPath for get the `xxx/tests_output/subPath`
    /// </remarks>
    /// </summary>
    /// <param name="subDir">sub directory.</param>
    /// <returns> full path string. </returns>
    public static string GetDumpDirPath(string subDir = "") => Provider.GetDumpDirPath(subDir);

    /// <summary>
    /// give the unittest class name, then return the dumpdir path
    /// <see cref="GetDumpDirPath(string)"/>
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public static string GetDumpDirPath(System.Type type)
    {
        var namespace_name = type.Namespace!.Split(".")[^1];
        if (!namespace_name.EndsWith("Test") || !type.Name.StartsWith("UnitTest"))
        {
            throw new System.ArgumentOutOfRangeException($"We Need NameSpace is `xxxTest`, Class is `UnitTestxxx`, But given namespace is {namespace_name}, class is {type.Name}");
        }
        return GetDumpDirPath(Path.Combine(namespace_name, type.Name));
    }

    /// <summary>
    /// Get the caller file path.
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public static string GetCallerFilePath([CallerFilePath] string path = "")
    {
        return path;
    }


    /// <summary>
    /// fixup the seq rand tensor into gived range.
    /// </summary>
    /// <param name="range"></param>
    /// <param name="symmetric"></param>
    /// <returns></returns>
    public static ValueRange<float> FixupRange(ValueRange<float> range, bool symmetric = false)
    {
        if (symmetric)
        {
            var r = Math.Max(Math.Max(Math.Abs(range.Min), Math.Abs(range.Max)), 0.01f);
            return new() { Min = -r, Max = r };
        }
        else
        {
            if (range.Min < -1e3f)
                range.Min = -1e3f;
            if (range.Max > 1e3f)
                range.Max = 1e3f;
            var r = range.Max - range.Min;
            if (r == 0)
                r = 0.1f;
            else if (r < 0.01f)
                r = 0.01f;
            range.Max = range.Min + r;

            if (range.Max < 0)
                range.Max = 0;
            if (range.Min > 0)
                range.Min = 0;
        }

        return range;
    }

    /// <summary>
    /// create the rand value by gived datatype.
    /// </summary>
    /// <param name="dataType"></param>
    /// <param name="shape"></param>
    /// <returns></returns>
    public static Tensor Rand(DataType dataType, params int[] shape)
    {
        return (Tensor)typeof(Testing).GetMethod("Rand", new[] { typeof(int[]) })!.MakeGenericMethod(dataType.CLRType).Invoke(null, new object[] { shape })!;
    }

    /// <summary>
    /// create the rand value by gived datatype.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="shape"></param>
    /// <returns></returns>
    public static Tensor<T> Rand<T>(params int[] shape)
        where T : unmanaged, IEquatable<T>
    {
        return Tensor.FromBytes<T>(Enumerable.Range(0, (int)TensorUtilities.GetProduct(shape)).Select(i =>
        {
            var bytes = new byte[Marshal.SizeOf(typeof(T))];
            RandGenerator.NextBytes(bytes);
            return bytes;
        }).SelectMany(i => i).ToArray(), shape);
    }

    /// <summary>
    /// create the seq value by gived datatype.
    /// </summary>
    /// <param name="dataType"></param>
    /// <param name="shape"></param>
    /// <returns></returns>
    public static Tensor Seq(DataType dataType, params int[] shape)
    {
        return (Tensor)typeof(Testing).GetMethod("Seq", new[] { typeof(int[]) })!.MakeGenericMethod(dataType.CLRType).Invoke(null, new object[] { shape })!;
    }

    /// <summary>
    /// create the seq value by gived datatype.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="shape"></param>
    /// <returns></returns>
    public static Tensor<T> Seq<T>(params int[] shape)
        where T : unmanaged, IEquatable<T>
    {
        return Tensor.FromArray(Enumerable.Range(0, (int)TensorUtilities.GetProduct(shape)).ToArray())
            .Cast<T>(CastMode.Default).Reshape(shape);
    }

    /// <summary>
    /// NOTE 映射一个sequence到新的range.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="t"></param>
    /// <param name="range"></param>
    /// <returns></returns>
    public static Tensor<T> ReArangeSeq<T>(Tensor<T> t, ValueRange<float> range)
      where T : unmanaged, System.IEquatable<T>
    {
        var scale = (range.Max - range.Min) / t.Length;
        return Tensor.FromArray(t.Cast<float>(CastMode.Default).Select(i => i * scale + range.Min).ToArray())
                .Cast<T>()
                .Reshape(t.Shape);
    }

    /// <summary>
    /// check all value close.
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <param name="tol"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public static int AllClose(Tensor a, Tensor b, float tol = .003f)
    {
        if (a.Shape != b.Shape)
            throw new InvalidOperationException();
        if (a.ElementType != b.ElementType)
            throw new InvalidOperationException();
        int err_count = 0;
        // int offset = 0;
        foreach (var p in a.Cast<float>().Zip(b.Cast<float>()))
        {
            if (Math.Abs(p.Item1 - p.Item2) > tol)
            {
                err_count++;
            }
        }
        return err_count;
    }
}


public class UnitTestFixtrue
{
    protected RunPassOptions passOptions;

    public UnitTestFixtrue()
    {
        string DumpDirPath = Testing.GetDumpDirPath(this.GetType());
        passOptions = new RunPassOptions(CompilerServices.GetTarget(CompilerServices.CompileOptions.Target), CompilerServices.CompileOptions.DumpLevel, DumpDirPath);
    }

}