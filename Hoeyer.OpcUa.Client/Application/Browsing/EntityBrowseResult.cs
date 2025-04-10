using System.Collections.Generic;
using System.Linq;
using Hoeyer.OpcUa.Core.Application.RequestResponse;
using Opc.Ua;

namespace Hoeyer.OpcUa.Client.Application.Browsing;

public record struct EntityBrowseResult(Node Node, IEnumerable<ReferenceDescription> Children);