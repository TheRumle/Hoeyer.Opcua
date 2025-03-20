using System;
using Hoeyer.OpcUa.Core.Utility;
using Opc.Ua;

namespace Hoeyer.OpcUa.Core.Entity.State.Parsers;

public abstract class ValueParser<TTarget>(Func<TTarget> identity) : IDataValueParser<TTarget>
{
    protected readonly Func<TTarget> Identity = identity;

    public TTarget Parse(DataValue dataValue)
    {
        var val = dataValue.GetValue(typeof(TTarget));
        if (val != null) return (TTarget)val;
        if (TryGetTargetOrIdentity(dataValue.Value, out var l)) return l;
        if (TryGetTargetOrIdentity(dataValue.WrappedValue, out l)) return l;
        if (TryGetTargetOrIdentity(dataValue.WrappedValue.Value, out l)) return l;
        if (TryGetTargetOrIdentity(dataValue.Value, out l)) return l;
        
        TypeInfo? wrappedTypeInfo = dataValue.WrappedValue.TypeInfo;
        if (wrappedTypeInfo is null) return Identity.Invoke();
        
        if (TryGetTargetOrIdentity(OpcUaTypes.ToType(wrappedTypeInfo.BuiltInType), out l)) return l;
        return Identity.Invoke();
    }

    protected abstract bool TryGetTargetOrIdentity(object value, out TTarget target);
}