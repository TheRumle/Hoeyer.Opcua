using Hoeyer.OpcUa.Core.Entity;

namespace Hoeyer.OpcUa.Server.SourceGeneration;

public static class ModellingNamespace
{
    public const string ENTITY_ATTRIBUTE_NAMESPACE = "Hoeyer.OpcUa.Core.Entity";
    public const string ENTITY_ATTRIBUTE_FULLNAME = ENTITY_ATTRIBUTE_NAMESPACE + "." + nameof(OpcUaEntityAttribute);
}