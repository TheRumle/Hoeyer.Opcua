using System.Collections.Generic;
using System.Linq;
using Opc.Ua;

namespace Hoeyer.OpcUa.Core.Entity.State.Loaders;

public sealed class CollectionLoader<TTarget> : DataValueLoader<IEnumerable<TTarget>>
{
    public override DataValue Parse(IEnumerable<TTarget> value)
    {
        return WithValue(value.ToArray());
    }
}