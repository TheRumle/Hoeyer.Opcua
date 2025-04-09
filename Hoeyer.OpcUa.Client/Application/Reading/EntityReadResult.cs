using System.Collections.Generic;
using Opc.Ua;

namespace Hoeyer.OpcUa.Client.Application.Reading;

public record struct EntityReadResult(Node Node, IEnumerable<ReadResult> Children);