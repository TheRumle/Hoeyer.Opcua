using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Hoeyer.Common.Extensions.Types;
using Hoeyer.OpcUa.Client.Api;
using Hoeyer.OpcUa.Client.Api.Browsing.Reading;
using Hoeyer.OpcUa.Client.Api.Reading;
using Opc.Ua;
using Opc.Ua.Client;

namespace Hoeyer.OpcUa.Client.Application.Reading;

internal sealed class NodeReader : INodeReader
{
    public Task<ReadResult> ReadNodesAsync(
        ISession session,
        IEnumerable<NodeId> ids,
        NodeClass filter = NodeClassFilters.Any,
        CancellationToken ct = default)
    {
        var idList = ids.ToList();
        var task =  CreateReadNodesTask(session, filter, ct, idList);
        task.ConfigureAwait(false);
        return task;
    }

    private static Task<ReadResult> CreateReadNodesTask(ISession session, NodeClass filter, CancellationToken ct, List<NodeId> idList)
    {
        return session
            .ReadNodesAsync(idList, filter, ct: ct)
            .ContinueWith(response =>
            {
                if (response.IsCompletedSuccessfully)
                {
                    return new ReadResult(response.Result.Zip());
                }

                var errors = response.Exception!.InnerExceptions.OfType<ServiceResultException>().FirstOrDefault();
                if (errors is not null) throw new NodeReadException(idList, errors);
                throw new NodeReadException(idList, response.Exception.Message);
            }, ct);
    }

    /// <inheritdoc />
    public async Task<Node> ReadNodeAsync(ISession session, NodeId nodeId, CancellationToken ct = default)
    {
        try
        {
            return await session.ReadNodeAsync(nodeId, ct);
        }
        catch (ServiceResultException  e)
        {
            throw new NodeReadException(nodeId, e);
        }
    }
}