namespace Hoeyer.OpcUa.Server.SourceGeneration.OpcUaTypes;

public record struct OpcUaProperty(string Name, string CSharpType, string OpcNativeTypeId, string ValueRank);