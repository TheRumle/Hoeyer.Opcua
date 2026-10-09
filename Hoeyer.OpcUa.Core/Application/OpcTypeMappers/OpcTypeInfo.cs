using Opc.Ua;

namespace Hoeyer.OpcUa.Core.Application.OpcTypeMappers;

public record struct OpcTypeInfo(BuiltInType BuiltInType, int Rank, NodeId TypeId);