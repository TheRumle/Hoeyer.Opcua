namespace Hoeyer.OpcUa.CompileTime.OpcUaTypes;

public record struct OpcUaProperty(string Name, string CSharpType, string OpcNativeTypeId, string ValueRank);