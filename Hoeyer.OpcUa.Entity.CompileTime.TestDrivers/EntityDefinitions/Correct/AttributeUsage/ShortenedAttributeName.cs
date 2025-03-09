using Hoeyer.OpcUa.Core.Entity;
using Hoeyer.OpcUa.Core.Entity.Node;

namespace Hoeyer.OpcUa.Entity.CompileTime.Testing.EntityDefinitions.Correct.AttributeUsage;

[OpcUaEntity]
public class ShortenedAttributeName
{
    public string MyString { get; set; } = "";
}