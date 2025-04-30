using System.Collections.Generic;
using Opc.Ua;

namespace Hoeyer.OpcUa.Client.Api.Browsing;

public record struct EntityReadResult(Node Node, IEnumerable<ReferenceDescription> Children);