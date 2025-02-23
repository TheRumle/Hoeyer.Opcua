using Opc.Ua;

namespace Hoeyer.OpcUa.Server.Application.EntityNode.Operations;

public record struct ReadResponse(DataValue Response, ReadValueId Request);
public record struct BrowseResult(ReferenceDescription Response, ReadValueId Request);
