using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Hoeyer.Common.Extensions.Types;
using Hoeyer.OpcUa.Client.Abstractions;
using Hoeyer.OpcUa.Client.Abstractions.Browsing.Reading;
using Microsoft.Extensions.Logging;
using Opc.Ua;
using Opc.Ua.Client;

namespace Hoeyer.OpcUa.Client.Application.Browsing.Reading;

internal sealed class NodeReader(ILogger<NodeReader> logger) : INodeReader
{
    public async Task<ReadResult> ReadNodesAsync(
        ISession session,
        IEnumerable<NodeId> ids,
        NodeClass filter = NodeClass.Unspecified,
        CancellationToken ct = default)
    {
        List<NodeId> idList = ids.ToList();

        logger.LogTrace("Reading nodes {nodes}", string.Join(", ", idList));
        var responseTuples = await session.ReadNodesAsync(idList, filter, ct: ct);
        var zipped = responseTuples
            .Zip().ToList();

        var variables = zipped
            .Where(readResponse => readResponse.second.IsGood())
            .Select(readResponse => readResponse.first)
            .OfType<VariableNode>()
            .ToList();

        var (readValues, responseCodes) =
            await session.ReadValuesAsync(variables.Select(e => e.NodeId).ToList(), ct);

        for (var i = 0; i < readValues.Count; i++)
        {
            if (responseCodes[i].IsGood())
            {
                logger.LogTrace("Assigning {variable} to {value}", variables[i].BrowseName.Name,
                    readValues[i].WrappedValue);
                variables[i]!.Value = readValues[i].WrappedValue;
            }
        }

        return new ReadResult(zipped!);
    }

    public async Task<Node> ReadNodeAsync(ISession session, NodeId nodeId, CancellationToken ct = default)
    {
        try
        {
            logger.LogTrace("Attempting to read node {node}", nodeId);
            return await session.ReadNodeAsync(nodeId, ct);
        }
        catch (ServiceResultException e)
        {
            logger.LogError(e, "Error while reading node {nodeId}", nodeId);
            throw new NodeReadException(nodeId, e);
        }
    }
}