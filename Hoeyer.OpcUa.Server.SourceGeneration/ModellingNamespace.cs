using Hoeyer.OpcUa.Core.Entity;
using Hoeyer.OpcUa.Core.Entity.Node;

namespace Hoeyer.OpcUa.Server.SourceGeneration;

public static class ModellingNamespace
{
    public const string ENTITY_ATTRIBUTE_NAMESPACE = "Hoeyer.OpcUa.Core.Entity";
    public const string ENTITY_NODE_NAMESPACE = "Hoeyer.OpcUa.Core.Entity.Node";
    public const string ENTITY_ATTRIBUTE_FULLNAME = ENTITY_ATTRIBUTE_NAMESPACE + "." + nameof(OpcUaEntityAttribute);
}