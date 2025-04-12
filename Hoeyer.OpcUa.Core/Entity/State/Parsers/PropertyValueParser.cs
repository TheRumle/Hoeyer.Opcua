using Opc.Ua;

namespace Hoeyer.OpcUa.Core.Entity.State.Parsers;

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
            T pure => pure,
            _ => default
        };

        return res;
    }
}