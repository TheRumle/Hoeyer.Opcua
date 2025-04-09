using System.Collections.Generic;
using Opc.Ua;

namespace Hoeyer.OpcUa.Client.Application.Browsing;

public record struct EntityBrowseResult(Node Node, IEnumerable<ReferenceDescription> Children);