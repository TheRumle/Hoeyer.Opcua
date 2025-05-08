using System.Runtime.InteropServices;
using Opc.Ua;

namespace Hoeyer.OpcUa.Core.Application.Translator.Parsers;

public sealed class PropertyValueParser<T> : IValueParser<PropertyState, T?>
{
    private readonly DataValueParser<T?> _dataValueParser = new DefaultDataValueParser<T?>(default);

    /// <inheritdoc />
    public T? Parse(PropertyState dataValue)
    {
        var val = dataValue.Value;
        var res = val switch
        {
            DataValue dv => _dataValueParser.Parse(dv),
            Variant v => (T)v.Value,
            T pure => pure,
            _ => default
        };

        return res;
    }
}