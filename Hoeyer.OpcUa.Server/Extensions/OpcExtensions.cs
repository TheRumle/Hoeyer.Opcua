using Opc.Ua;

namespace Hoeyer.OpcUa.Server.Extensions;

public static class OpcExtensions
{
    public static DataValue ToDataValue(this BaseVariableState property)
    {
        return new DataValue(new Variant
        {
            Value = property.Value
        })
        {
            ServerTimestamp = property.Timestamp,
            StatusCode = property.StatusCode
        };
    }
}