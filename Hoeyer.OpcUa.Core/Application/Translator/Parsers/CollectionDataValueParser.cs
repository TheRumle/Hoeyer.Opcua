using System;
using System.Collections.Generic;

namespace Hoeyer.OpcUa.Core.Application.Translator.Parsers;

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