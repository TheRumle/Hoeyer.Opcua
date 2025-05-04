using System.Collections.Generic;
using System.Linq;
using Opc.Ua;

namespace Hoeyer.OpcUa.Core.Application.Parsers;

public sealed class PropertyValueCollectionParser<T> : IValueParser<PropertyState, T[]?>
{
    private readonly DataValueParser<T[]?> dataValueParser = new DefaultDataValueParser<T[]?>(null);

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