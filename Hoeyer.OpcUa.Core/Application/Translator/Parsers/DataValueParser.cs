﻿using System;
using Opc.Ua;

namespace Hoeyer.OpcUa.Core.Application.Translator.Parsers;

public abstract class DataValueParser<TTarget> : IValueParser<DataValue?, TTarget>
{
    protected readonly Func<TTarget> Identity;

    protected DataValueParser(Func<TTarget> identity)
    {
        Identity = identity;
    }

    public TTarget Parse(DataValue? dataValue)
    {
        if (dataValue == null)
        {
            return Identity();
        }

        var val = dataValue.GetValue(typeof(TTarget));
        if (val != null)
        {
            return (TTarget)val;
        }

        if (TryGetTargetOrIdentity(dataValue.Value, out var l))
        {
            return l;
        }

        if (TryGetTargetOrIdentity(dataValue.WrappedValue, out l))
        {
            return l;
        }

        if (TryGetTargetOrIdentity(dataValue.WrappedValue.Value, out l))
        {
            return l;
        }

        var wrappedTypeInfo = dataValue.WrappedValue.TypeInfo;
        if (wrappedTypeInfo is null)
        {
            return Identity.Invoke();
        }

        if (TryGetTargetOrIdentity(OpcUaTypes.ToType(wrappedTypeInfo.BuiltInType), out l))
        {
            return l;
        }

        return Identity.Invoke();
    }

    protected abstract bool TryGetTargetOrIdentity(object value, out TTarget target);
}