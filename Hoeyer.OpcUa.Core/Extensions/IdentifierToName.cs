using Opc.Ua;

namespace Hoeyer.OpcUa.Core.Extensions;

public static class IdentifierToName
{
    public static string AttributeName(this uint attributeId)
    {
        return Attributes.GetBrowseName(attributeId);
    }

    public static string StatusCodeName(this StatusCode s)
    {
        return StatusCodes.GetBrowseName(s.Code);
    }
}