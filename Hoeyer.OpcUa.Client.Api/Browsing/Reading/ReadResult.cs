﻿using System.Collections.Generic;
using System.Linq;
using Opc.Ua;

namespace Hoeyer.OpcUa.Client.Api.Browsing.Reading;

public sealed class ReadResult
{
    public readonly bool AllSuccess;

    public ReadResult(IEnumerable<(Node? node, ServiceResult result)> values)
    {
        List<(Node? node, ServiceResult result)> nodes = values.ToList();
        SuccesfulReads = nodes.Where(e => e.result.IsGood()).Select(e => e.node).ToList();
        FailedReads = nodes.Where(e => e.result.IsNotGood()).Select(e => e.node).ToList();
        AllSuccess = !FailedReads.Any();
    }

    public IReadOnlyList<Node?> FailedReads { get; set; }

    public IReadOnlyList<Node?> SuccesfulReads { get; set; }
}