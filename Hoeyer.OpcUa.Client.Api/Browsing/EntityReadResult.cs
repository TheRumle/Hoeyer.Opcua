using System.Collections.Generic;
using Opc.Ua;

namespace Hoeyer.OpcUa.Client.Api.Browsing;

public record struct AgentReadResult(Node Node, IEnumerable<ReferenceDescription> Children);