using Hoeyer.OpcUa.Core.Entity;

namespace Hoeyer.OpcUa.Entity.CompileTime.Testing.EntityDefinitions.Correct.AttributeUsage;

[OpcUaEntity]
public class ShortenedAttributeName
{
    public string MyString { get; set; } = "";
}