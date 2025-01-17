﻿using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Globalization;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Nncase.SourceGenerator.Evaluator;


public enum InterfaceKind
{
    IEvaluator,
    ITypeInferencer,
}

/// <summary>
/// some method get from the interface kink info.
/// </summary>
public static class InterfaceKindExtensions
{
    public static (string return_type_name, string context_type_name) GetKindInfo(this InterfaceKind target_interface) => (target_interface.GetReturnType(), target_interface.GetContextType());

    public static string GetReturnType(this InterfaceKind target_interface) => target_interface switch
    {
        InterfaceKind.IEvaluator => "Nncase.IValue",
        InterfaceKind.ITypeInferencer => "Nncase.IR.IRType",
        _ => throw new NotImplementedException(),
    };

    public static string GetContextType(this InterfaceKind target_interface) => target_interface switch
    {
        InterfaceKind.IEvaluator => "IEvaluateContext",
        InterfaceKind.ITypeInferencer => "ITypeInferenceContext",
        _ => throw new NotImplementedException(),
    };

    public static string GetAttrName(this InterfaceKind target_interface) => target_interface switch
    {
        InterfaceKind.IEvaluator => "EvaluatorGeneratorAttribute",
        InterfaceKind.ITypeInferencer => "TypeInferGeneratorAttribute",
        _ => throw new NotImplementedException(),
    };

    /// <summary>
    /// check the return type , can process the interface type
    /// </summary>
    /// <param name="typeSymbol"></param>
    /// <param name="interfaceKind"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public static bool CheckReturnTypeRange(this ITypeSymbol typeSymbol, InterfaceKind interfaceKind) => interfaceKind switch
    {
        InterfaceKind.IEvaluator => typeSymbol switch
        {
            { Name: "Tensor" } or { BaseType: { Name: "Tensor" } } => true,
            { Name: "Const" } or { BaseType: { Name: "Const" } } => true,
            { Name: "IValue" } => true,
            _ => false,
        },
        InterfaceKind.ITypeInferencer => typeSymbol switch
        {
            { Name: "IRType" } => true,
            { BaseType: { Name: "IRType" } } => true,
            _ => false,
        },
        _ => throw new NotImplementedException($"CheckReturnTypeRange : {typeSymbol.Name} {interfaceKind}"),
    };

    public static string BuildReturnWrapper(this ITypeSymbol typeSymbol, InterfaceKind interfaceKind, string visitStatement) => interfaceKind switch
    {
        InterfaceKind.IEvaluator => typeSymbol switch
        {
            { Name: "Tensor" } or { BaseType: { Name: "Tensor" } } => $"Value.FromTensor({visitStatement})",
            { Name: "Const" } or { BaseType: { Name: "Const" } } => $"Value.FromConst({visitStatement})",
            { Name: "IValue" } => visitStatement,
            _ => throw new ArgumentOutOfRangeException($"Can't Return {typeSymbol.ToDisplayString()} For {interfaceKind}!"),
        },
        InterfaceKind.ITypeInferencer => visitStatement,
        _ => throw new NotImplementedException(),
    };
}

/// <summary>
/// the candidate will be generated for new instance
/// </summary>
internal class GenerateCandidate
{
    public INamedTypeSymbol Class;
    public INamedTypeSymbol Op;
    public IMethodSymbol Method;
    public InterfaceKind Target;


    public GenerateCandidate(INamedTypeSymbol classSymbol, INamedTypeSymbol opSymbol, IMethodSymbol method, InterfaceKind target_kind)
    {
        this.Class = classSymbol;
        this.Op = opSymbol;
        this.Method = method;
        this.Target = target_kind;
    }
}

[Generator]
internal class EvaluatorGenerator : IIncrementalGenerator
{

    public INamedTypeSymbol? ExprSymobl;
    public INamedTypeSymbol? TensorSymobl;
    public INamedTypeSymbol? ParameterInfoSymobl;
    public INamedTypeSymbol? IRTypeSymobl;
    public INamedTypeSymbol? IEvaluateContextSymobl;
    public INamedTypeSymbol? ITypeInferenceContext;

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        // Do a simple filter for enums
        IncrementalValuesProvider<GenerateCandidate> candidates = context.SyntaxProvider
            .CreateSyntaxProvider(
                predicate: static (node, _) => IsSyntaxTargetForGeneration(node), // select recored with base type named op
                transform: (ctx, _) => GetSemanticTargetForGeneration(ctx)) // sect the enum with the [EnumExtensions] attribute
            .SelectMany((s, _) => s)!; // filter out attributed enums that we don't care about

        // Combine the selected enums with the `Compilation`
        //IncrementalValueProvider<(Compilation, ImmutableArray<GenerateCandidate>)> compilationAndEnums
        //= context.CompilationProvider.Combine(candidates.Collect());

        // Generate the source using the compilation and enums
        context.RegisterSourceOutput(candidates.Collect(), (spc, source) => Execute(spc, source));
    }


    /// <summary>
    /// check the calss have one more attr && one more base type && have Partial keyword
    /// </summary>
    /// <param name="node"></param>
    /// <returns></returns>
    static bool IsSyntaxTargetForGeneration(SyntaxNode node)
    {
        if (node is ClassDeclarationSyntax { BaseList: { } baselist } classDeclaration && classDeclaration.AttributeLists.Count > 0 && baselist.Types.Count > 0)
        {
            return classDeclaration.Modifiers.Any(tok => tok.IsKind(SyntaxKind.PartialKeyword));
        }
        return false;
    }

    IEnumerable<GenerateCandidate> GetSemanticTargetForGeneration(GeneratorSyntaxContext ctx)
    {
        ExprSymobl ??= ctx.SemanticModel.Compilation.GetTypeByMetadataName("Nncase.IR.Expr");
        TensorSymobl ??= ctx.SemanticModel.Compilation.GetTypeByMetadataName("Nncase.Tensor");
        IRTypeSymobl ??= ctx.SemanticModel.Compilation.GetTypeByMetadataName("Nncase.IR.IRType");
        IEvaluateContextSymobl ??= ctx.SemanticModel.Compilation.GetTypeByMetadataName("Nncase.Evaluator.IEvaluateContext");
        ITypeInferenceContext ??= ctx.SemanticModel.Compilation.GetTypeByMetadataName("Nncase.Evaluator.ITypeInferenceContext");
        ParameterInfoSymobl ??= ctx.SemanticModel.Compilation.GetTypeByMetadataName("Nncase.IR.ParameterInfo");

        var classSymbol = ctx.SemanticModel.GetDeclaredSymbol((ClassDeclarationSyntax)ctx.Node)!;
        var eval_candidate = ReceiveTargetInterface(classSymbol, InterfaceKind.IEvaluator);
        var typeinfer_candidate = ReceiveTargetInterface(classSymbol, InterfaceKind.ITypeInferencer);
        return new[] { eval_candidate, typeinfer_candidate }.OfType<GenerateCandidate>();
    }

    GenerateCandidate? ReceiveTargetInterface(INamedTypeSymbol classSymbol, InterfaceKind target_kind)
    {
        if (classSymbol!.GetAttributes().Any(attr => attr!.AttributeClass!.Name == target_kind.GetAttrName()))
        {
            // 1. find op symbol
            var interfaces = classSymbol.Interfaces.Where(i => i.TypeArguments.Count() == 1 && i.Name == target_kind.ToString()).ToArray();
            if (interfaces.Length != 1)
                return null;
            //Diagnostics.Add(Diagnostic.Create(RecriverUtil.ClassNotFromInterfaceError, Location.None, classSymbol.ToDisplayString(), target_kind));
            var OpSymbol = interfaces[0].TypeArguments.OfType<INamedTypeSymbol>().First();
            // 2. find the reference method!
            var methods = classSymbol.GetMembers()
                                   .OfType<IMethodSymbol>()
                                   .Where(m => m.Name == "Visit" && m.ReturnType.CheckReturnTypeRange(target_kind)).ToArray();
            if (methods.Length == 0)
                //Diagnostics.Add(Diagnostic.Create(RecriverUtil.ClassNoValidMethodError, Location.None, classSymbol.ToDisplayString()));
                return null;


            if (methods.Length > 1)
            {
                //Diagnostics.Add(Diagnostic.Create(RecriverUtil.ClassMoreMethodError, Location.None, classSymbol.ToDisplayString()));
                return null;
            }

            var method = methods[0];
            if (method.ReturnType.Name == target_kind.GetReturnType()
                            && method.Parameters.Count() == 2
                            && method.Parameters[0].Type.Name == target_kind.GetContextType()
                            && method.Parameters[1].Type.Name == OpSymbol.Name)
                return null;

            // 3. add to the Candidates
            return new(classSymbol, OpSymbol, method, target_kind);
            //Console.WriteLine($"EvaluatorGenerator Receive {classSymbol} For {target_kind}");
        }
        return null;
    }

    void Execute(SourceProductionContext context, ImmutableArray<GenerateCandidate> candidates)
    {
        var evalCandidates = candidates.Where(c => c.Target is InterfaceKind.IEvaluator);
        if (evalCandidates.Any())
        {
            var eval_compilationunit = BuildFile(context, evalCandidates);
            context.AddSource("Ops.Evaluator", SyntaxTree(eval_compilationunit, encoding: Encoding.UTF8).GetText());
        }

        var typeInferCandidates = candidates.Where(c => c.Target is InterfaceKind.ITypeInferencer);
        if (typeInferCandidates.Any())
        {
            var typeinfer_compilationunit = BuildFile(context, typeInferCandidates);
            context.AddSource("Ops.TypeInferencer", SyntaxTree(typeinfer_compilationunit, encoding: Encoding.UTF8).GetText());
        }
    }

    /// <summary>
    /// build the value convert expression like:
    /// var alpha = context.GetArgumentValueAsScalar<int>(celu, Celu.Alpha);
    /// </summary>
    /// <param name="cand"></param>
    /// <returns></returns>
    /// <exception cref="NotSupportedException"></exception>
    List<StatementSyntax> BuildStatements(GenerateCandidate cand)
    {
        var (return_type_name, context_type_name) = cand.Target.GetKindInfo();
        var statementSyntaxes = new List<StatementSyntax>();
        var allOpParams = new HashSet<string>(cand.Op.GetMembers().OfType<IFieldSymbol>().Where(f => SymbolEqualityComparer.Default.Equals(f.Type, ParameterInfoSymobl)).Select(f => f.Name));

        foreach (var Parameter in cand.Method.Parameters)
        {
            // if (!cand.Op.MemberNames.Any(name => name == Parameter.Name))
            //     Context.Value.ReportDiagnostic(Diagnostic.Create(RecriverUtil.MethodParamError, Location.None, cand.Class.Name, string.Join(", ", cand.Method.Parameters.Select(p => p.Name)), cand.Op.Name));
            var paramType = Parameter.Type;
            if ((cand.Target == InterfaceKind.IEvaluator && SymbolEqualityComparer.Default.Equals(paramType, IEvaluateContextSymobl))
              || (cand.Target == InterfaceKind.ITypeInferencer && SymbolEqualityComparer.Default.Equals(paramType, ITypeInferenceContext)))
            {
                if (Parameter.Name != "context")
                    statementSyntaxes.Add(ParseStatement($"var {Parameter.Name} = context;"));
                continue;
            }
            if (SymbolEqualityComparer.Default.Equals(paramType, cand.Op))
            {
                if (Parameter.Name != "target")
                    statementSyntaxes.Add(ParseStatement($"var {Parameter.Name} = target;"));
                continue;
            }
            string callMethod = cand.Target switch
            {
                InterfaceKind.IEvaluator => paramType switch
                {
                    INamedTypeSymbol { IsReferenceType: true } x when x.ToDisplayString() == "Nncase.IValue" => $"GetArgumentValue",
                    INamedTypeSymbol { IsGenericType: true, IsReferenceType: true } x when x.Name == "Tensor" => $"GetArgumentValueAsTensor<{x.TypeArguments[0].ToDisplayString()}>",
                    IArrayTypeSymbol { ElementType: { IsUnmanagedType: true, IsValueType: true } e } x => $"GetArgumentValueAsArray<{e.ToDisplayString()}>",
                    { IsReferenceType: true } x when SymbolEqualityComparer.Default.Equals(x, TensorSymobl) => $"GetArgumentValueAsTensor",
                    { IsReferenceType: true } x when x.ToDisplayString().EndsWith("OrtKISharp.Tensor") => "GetOrtArgumentValue",
                    { IsUnmanagedType: true, IsValueType: true } x => $"GetArgumentValueAsScalar<{paramType.ToDisplayString()}>",
                    _ => throw new NotSupportedException($"Convert {cand.Class.Name} Params {paramType.ToDisplayString()} For IEvaluator Impl!")
                },
                InterfaceKind.ITypeInferencer => paramType switch
                {
                    { IsReferenceType: true } x when x.IsInheritFrom(IRTypeSymobl) => $"CheckArgumentType<{x}>",
                    var x when SymbolEqualityComparer.Default.Equals(x, ExprSymobl) => $"GetArgument",
                    _ => throw new NotSupportedException($"Convert {cand.Class.Name} Params {paramType.ToDisplayString()} For ITypeInferencer Impl!")
                },
                _ => throw new NotSupportedException($"{paramType.ToDisplayString()} with {cand.Target}!")
            };

            statementSyntaxes.Add(ParseStatement($"var {Parameter.Name} = context.{callMethod}(target, {cand.Op.ToDisplayString()}.{Parameter.Name});"));

            if (allOpParams.Contains(Parameter.Name))
                allOpParams.Remove(Parameter.Name);
        }

        // when ITypeInferencer we need try check each input parameter.
        if (cand.Target == InterfaceKind.ITypeInferencer)
            allOpParams.ToList().ForEach(name =>
            {
                statementSyntaxes.Add(ParseStatement($"context.CheckArgumentType<Nncase.IR.IRType>(target, {cand.Op.ToDisplayString()}.{name});"));
            });

        var visitMethod = cand.Method.ReturnType.BuildReturnWrapper(cand.Target, $"Visit({string.Join(",", cand.Method.Parameters.Select(p => p.Name))})");
        statementSyntaxes.Add(ParseStatement($"return {visitMethod};"));
        return statementSyntaxes;
    }

    /// <summary>
    /// build the whole call method like:
    /// <code>
    /// public IValue Visit(IEvaluateContext context, Celu celu)
    /// {
    ///     var input = context.GetTorchArgumentValue(celu, Celu.Input);
    ///     var alpha = context.GetArgumentValueAsScalar<int>(celu, Celu.Alpha);
    ///     return Visit(context, celu, input, alpha);
    /// }
    /// </code>
    /// </summary>
    /// <param name="cand"></param>
    /// <param name="statements"></param>
    /// <returns></returns>
    MethodDeclarationSyntax BuildMethod(GenerateCandidate cand, IEnumerable<StatementSyntax> statements)
    {
        var (return_type_name, context_type_name) = cand.Target.GetKindInfo();
        var method = GeneratorUtil.MakeMethod(ParseTypeName(return_type_name).WithTrailingTrivia(ElasticSpace), "Visit")
            .AddModifiers(Token(SyntaxKind.PublicKeyword).WithTrailingTrivia(ElasticSpace))
            .AddParameterListParameters(
              Parameter(Identifier("context")).
                WithType(ParseTypeName(context_type_name).WithTrailingTrivia(ElasticSpace)),
              Parameter(Identifier("target")).
                WithType(ParseTypeName(cand.Op.ToDisplayString()).WithTrailingTrivia(ElasticSpace)))
            .WithBody(GeneratorUtil.MakeBlock(statements));
        return method;
    }

    CompilationUnitSyntax BuildFile(SourceProductionContext context, IEnumerable<GenerateCandidate> Candidates)
    {
        List<NamespaceDeclarationSyntax> namespaceDeclarations = new();
        var NamespaceCandidates = Candidates.GroupBy(keySelector: can => can.Class.ContainingNamespace, SymbolEqualityComparer.Default).ToDictionary(g => g.Key, g => g.ToList(), SymbolEqualityComparer.Default);
        foreach (var (Namespace, candidates) in NamespaceCandidates.Select(kv => (kv.Key, kv.Value)))
        {
            List<ClassDeclarationSyntax> classDeclarations = new();
            foreach (var cand in candidates)
            {
                // 1. generate the param preprocess
                var statementSyntaxes = BuildStatements(cand);
                // 2. generate the IEvaluator Interface Impl
                var methodDeclarations = BuildMethod(cand, statementSyntaxes.Select(s => s.WithLeadingTrivia(ElasticTab).WithTrailingTrivia(ElasticLineFeed)));
                // 3. generate the classes
                var cls_name = cand.Class.Name;
                var cls = GeneratorUtil.MakeClass(cls_name).
                    AddModifiers(
                      Token(SyntaxKind.PublicKeyword).WithTrailingTrivia(ElasticSpace),
                      Token(SyntaxKind.PartialKeyword).WithTrailingTrivia(ElasticSpace)).
                    AddBaseListTypes(cand.Class.Interfaces.Select(i => SimpleBaseType(ParseTypeName(i.ToDisplayString()))).ToArray()).
                    AddMembers(methodDeclarations);
                classDeclarations.Add(cls);
            }
            // 4. generate the namespaces
            namespaceDeclarations.Add(
                GeneratorUtil.MakeNameSpace(Namespace!.ToDisplayString()).
                    AddMembers(classDeclarations.ToArray()));
        }

        // 4. generate the file
        var compilationUnit = CompilationUnit().
                WithMembers(new(namespaceDeclarations)).
                WithLeadingTrivia(GeneratorUtil.MakeWarningTrivid(SyntaxKind.DisableKeyword)).
                WithTrailingTrivia(GeneratorUtil.MakeWarningTrivid(SyntaxKind.RestoreKeyword));
        return compilationUnit;
    }

}
