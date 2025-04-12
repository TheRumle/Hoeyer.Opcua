using System.Collections.Generic;
using System.Linq;
using Opc.Ua;
using Opc.Ua.Client;

namespace Hoeyer.OpcUa.Client.Application.Reading;

public record struct EntityReadResult(Node Node, IEnumerable<ReadResult> Children);