using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Hoeyer.Common.Extensions.Types;
using Hoeyer.OpcUa.Client.Api;
using Hoeyer.OpcUa.Client.Api.Browsing.Reading;
using Opc.Ua;
using Opc.Ua.Client;

namespace Hoeyer.OpcUa.Client.Application.Browsing.Reading;

internal sealed class NodeReader : INodeReader
{
    public Task<ReadResult> ReadNodesAsync(
        ISession session,
        IEnumerable<NodeId> ids,
        NodeClass filter = NodeClassFilters.Any,
        CancellationToken ct = default)
    {
        List<NodeId> idList = ids.ToList();
        Task<ReadResult> task = CreateReadNodesTask(session, filter, ct, idList);
        task.ConfigureAwait(false);
        return task;
    }

    /// <inheritdoc />
    public async Task<Node> ReadNodeAsync(ISession session, NodeId nodeId, CancellationToken ct = default)
    {
        try
        {
            return await session.ReadNodeAsync(nodeId, ct);
        }
        catch (ServiceResultException e)
        {
            throw new NodeReadException(nodeId, e);
        }
    }

    private static Task<ReadResult> CreateReadNodesTask(ISession session, NodeClass filter, CancellationToken ct,
        List<NodeId> idList)
    {
        return session
            .ReadNodesAsync(idList, filter, ct: ct)
            .ThenAsync(async responseTuples =>
            {
                List<(Node first, ServiceResult second)> zipped = responseTuples.Zip().ToList();
                List<VariableNode> variables = zipped
                    .Where(readResponse => readResponse.second.IsGood())
                    .Select(readResponse => readResponse.first)
                    .OfType<VariableNode>()
                    .ToList();

                (DataValueCollection? readValues, IList<ServiceResult>? responseCodes) =
                    await session.ReadValuesAsync(variables.Select(e => e.NodeId).ToList(), ct);

                for (var i = 0; i < readValues.Count; i++)
                {
                    if (responseCodes[i].IsGood())
                    {
                        variables[i]!.Value = readValues[i].WrappedValue;
                    }
                }

                return new ReadResult(zipped!);
            }, ct);
    }
}