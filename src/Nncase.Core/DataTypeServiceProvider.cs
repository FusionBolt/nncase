﻿// Copyright (c) Canaan Inc. All rights reserved.
// Licensed under the Apache license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;

namespace Nncase;

internal interface IDataTypeServiceProvider
{
    PrimType GetPrimTypeFromType(Type type);

    PrimType GetPrimTypeFromTypeCode(Runtime.TypeCode typeCode);

    ValueType GetValueTypeFromType(Type type);

    DataType GetDataTypeFromType(Type type);
    
    ISpanConverter GetConverter(Type fromType, Type toType);
}

internal class DataTypeServiceProvider : IDataTypeServiceProvider
{
    private readonly Dictionary<Type, PrimType> _primTypes = new();
    private readonly Dictionary<Runtime.TypeCode, PrimType> _typeCodeToPrimTypes = new();
    private readonly Dictionary<Type, ValueType> _valueTypes = new();
    private readonly IComponentContext _componentContext;

    public DataTypeServiceProvider(PrimType[] primTypes, ValueType[] valueTypes, IComponentContext componentContext)
    {
        _primTypes = primTypes.ToDictionary(x => x.CLRType);
        _typeCodeToPrimTypes = primTypes.Where(x => x.TypeCode < Runtime.TypeCode.ValueType).ToDictionary(x => x.TypeCode);
        _valueTypes = valueTypes.ToDictionary(x => x.CLRType);
        _componentContext = componentContext;
    }

    public ISpanConverter GetConverter(Type fromType, Type toType)
    {
        if (fromType.IsGenericType && fromType.GetGenericTypeDefinition() == typeof(Pointer<>))
        {
            var converter = _componentContext.Resolve(typeof(IPointerSpanConverter<>).MakeGenericType(toType));
            var wrapperType = typeof(PointerSpanConverter<,>).MakeGenericType(fromType.GenericTypeArguments[0], toType);
            return (ISpanConverter)Activator.CreateInstance(wrapperType, converter)!;
        }
        else
        {
            return (ISpanConverter)_componentContext.Resolve(typeof(ISpanConverter<,>).MakeGenericType(fromType, toType));
        }
    }

    public DataType GetDataTypeFromType(Type type)
    {
        if (_primTypes.TryGetValue(type, out var primType))
        {
            return primType;
        }
        else if (_valueTypes.TryGetValue(type, out var valueType))
        {
            return valueType;
        }

        throw new NotSupportedException($"Unsupported Type {type} in GetDataTypefromType");
    }
    
    public PrimType GetPrimTypeFromType(Type type)
    {
        return _primTypes[type];
    }

    public PrimType GetPrimTypeFromTypeCode(Runtime.TypeCode typeCode)
    {
        return _typeCodeToPrimTypes[typeCode];
    }

    public ValueType GetValueTypeFromType(Type type)
    {
        return _valueTypes[type];
    }

    private class PointerSpanConverter<TElem, TTo> : ISpanConverter<Pointer<TElem>, TTo>
        where TElem : unmanaged, IEquatable<TElem>
        where TTo : unmanaged, IEquatable<TTo>
    {
        private readonly IPointerSpanConverter<TTo> _spanConverter;

        public PointerSpanConverter(IPointerSpanConverter<TTo> spanConverter)
        {
            _spanConverter = spanConverter;
        }

        public void ConvertTo(ReadOnlySpan<Pointer<TElem>> source, Span<TTo> dest, CastMode castMode)
        {
            _spanConverter.ConvertTo(source, dest, castMode);
        }
    }
}
