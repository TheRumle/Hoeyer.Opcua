using System;
using System.Collections.Generic;
using System.Linq;
using FluentResults;
using Opc.Ua;

namespace Hoeyer.OpcUa.Core.Entity.State.Parsers;

public sealed class CollectionDataValueParser<TCollection, TValue>(Func<TCollection> identity)
    : DataValueParser<TCollection>(identity) where TCollection : ICollection<TValue>
{
    private readonly Type _type = typeof(TCollection);

    protected override bool TryGetTargetOrIdentity(object value, out TCollection target)
    {
        if (value.GetType().IsAssignableFrom(_type))
        {
            target = (TCollection)value;
            return true;
        }
        if (value is TValue tVal)
        {
            var i = Identity.Invoke();
            i.Add(tVal);
            target = i;
            return true;
        }

        target = default!;
        return false;
    }
}

public sealed class PropertyValueCollectionParser<T> : IValueParser<PropertyState,T[]?>
{
    private readonly DataValueParser<T[]> dataValueParser = new DefaultDataValueParser<T[]>(default);
    
    public T[]? Parse(PropertyState dataValue)
    {
        var val = dataValue.Value;
        return val switch
        {
            T singleton => [singleton],
            IEnumerable<T> dv => dv.ToArray(),
            DataValue dv => dataValueParser.Parse(dv),
            _ => null
        };
    }
}

public sealed class PropertyValueParser<T> : IValueParser<PropertyState,T?>
{
    private readonly DataValueParser<T?> _dataValueParser = new DefaultDataValueParser<T?>(default);
    /// <inheritdoc />
    public T? Parse(PropertyState dataValue)
    {
        var val = dataValue.Value;
        var res = val switch
        {
            DataValue dv => _dataValueParser.Parse(dv),
            T pure => pure,
            _ => default
        };

        return res;
    }
}