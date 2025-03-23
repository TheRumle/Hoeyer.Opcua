using System.Collections.Generic;
using System.Linq;
using Opc.Ua;

namespace Hoeyer.OpcUa.Core.Entity.State.Loaders;

public sealed class DataValueCollectionLoader<TTarget, TCollection> : DataValueLoader<TCollection>
    where TCollection : IEnumerable<TTarget>
{
    public override DataValue Parse(TCollection value)
    {
        return WithValue(value.ToArray());
    }
}