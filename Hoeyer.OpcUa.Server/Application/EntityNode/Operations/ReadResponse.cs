using Opc.Ua;

namespace Hoeyer.OpcUa.Server.Application.EntityNode;

public record struct ReadResponse(DataValue Response, ReadValueId Request);