using Opc.Ua;

namespace Hoeyer.OpcUa.Client.Application.Reading;

public record struct ReadResult(Node Node, ServiceResult OperationDescription); 