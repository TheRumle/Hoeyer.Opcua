using System.Collections.Generic;
using System.Linq;
using Opc.Ua;

namespace Hoeyer.OpcUa.Core.Application.Translator.Loaders;

public sealed class DataValueCollectionLoader<TTarget, TCollection> : DataValueLoader<TCollection>
    where TCollection : IEnumerable<TTarget>
{
    public override DataValue Parse(TCollection value) => WithValue(value.ToList());
}