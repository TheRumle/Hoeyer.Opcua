using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentResults;
using FluentResults.Extensions;
using Hoeyer.Common.Extensions.LoggingExtensions;
using Hoeyer.Common.Extensions.Types;
using Hoeyer.OpcUa.Client.Application.Reading;
using Hoeyer.OpcUa.Client.Extensions;
using Hoeyer.OpcUa.Core.Entity.Node;
using Hoeyer.OpcUa.Core.Extensions;
using Microsoft.Extensions.Logging;
using Opc.Ua;
using Opc.Ua.Client;

namespace Hoeyer.OpcUa.Client.Application;

[SuppressMessage("Maintainability", "S3265", Justification = "OpcUa library does not do have a neat API, and this works fine.")]
public sealed class OpcEntityReader<TEntity>(ILogger logger)
{
    private static readonly string BrowseName = typeof(TEntity).Name;
    private const NodeClass CLASS_MASK = NodeClass.Object | NodeClass.Variable | NodeClass.DataType | NodeClass.View;


    public async Task<Result<TEntity>> ReadNode(Session session, CancellationToken ct = default)
    {
        var entityNode = await FindEntityRelatedNodes(session, ct);
        if (entityNode.IsFailed) return Result.Fail(entityNode.Errors);

        var q = await entityNode.Map(async node =>
        {
            return (await ReadEntityValues(session, ct, node))
                .Map(e =>
                {
                    DataValue? q = e.Left;
                    List<(DataValue first, ServiceResult second)>? f = e.Right;
                    return (q,f);
                });
        })
            .Bind(e=>e);

        return null;
    }
    
    
    

    private static async Task<Result<(DataValue Left, List<(DataValue first, ServiceResult second)> Right)>> 
        ReadEntityValues(
            Session session,
            CancellationToken ct,
            (Node Node, IEnumerable<(Node Node, ServiceResult OperationDescription)> Children) node)
    {
        var validReferences = node.Children
            .Where(readResult => StatusCode.IsGood(readResult.OperationDescription.StatusCode))
            .Select(readResult => readResult.Node.NodeId)
            .ToList();
            
        Task<DataValue> nodeValueTask = session.ReadValueAsync(node.Node.NodeId, ct);
        Task<(DataValueCollection, IList<ServiceResult>)> childValueTask = session.ReadValuesAsync(validReferences, ct);

        await Task.WhenAll(nodeValueTask, childValueTask);
        var nodeValue = nodeValueTask.TraverseToResult();
        var childValues = childValueTask.TraverseToResult().Map(e => e.Zip().ToList());
        if (nodeValue.IsFailed || childValues.IsFailed) return Result.Fail(nodeValue.Errors.Union(childValues.Errors));
        
        return nodeValue.MergeWith(childValues);
    }

    private async Task<Result<(Node Node, IEnumerable<(Node Node, ServiceResult OperationDescription)> Children)>> FindEntityRelatedNodes(Session session, CancellationToken ct)
    {
        return await logger
            .LogCaughtExceptionAs(LogLevel.Error)
            .WithScope("Reading entity")
            .WithErrorMessage("An unexpected error occured trying to read the entity from the server using session {@Session}",
                session.ToLoggingObject())
            .WhenExecutingAsync(async () =>
            {
                var entityId = await new BreadthFirstBrowse(session)
                    .SearchAsync(ObjectIds.RootFolder, reference => BrowseName.Equals(reference.BrowseName.Name), ct)
                    .Map(r => r.NodeId.ToNodeId(session.NamespaceUris));
                
                Result<Node> readResult = entityId
                    .Map(async entityNodeId => await session.ReadNodeAsync(entityNodeId, ct))
                    .TraverseToResult();
                
                var clientResponses = readResult
                    .Map(resultNode => 
                    {
                        var referencedIds = resultNode.ReferenceTable
                            .Select(referenceNode => referenceNode.TargetId.ToNodeId(session.NamespaceUris))
                            .ToList();

                        return session.ReadNodesAsync(referencedIds, CLASS_MASK, ct: ct)
                            .TraverseToResult()
                            .Map(value => value.Zip());
                    })
                    .Splash();

                return readResult.MergeWith(clientResponses);
            });
    }
}

public sealed class OpcEntityClient<TEntity>(ILogger<OpcEntityClient<TEntity>> logger) :
    IEntityClient<TEntity>
{
    private readonly OpcEntityReader<TEntity> _reader = new(logger);
    /// <inheritdoc />
    public async Task<Result<TEntity>> ReadOpcUaEntityAsync(
        Session session)
    {
        await _reader.ReadNode(session);
        return null;
    }

   

    static void WriteNodeValue(Session session, NodeId nodeId, object newValue)
    {
        WriteValue valueToWrite = new WriteValue
        {
            NodeId = nodeId,
            AttributeId = Attributes.Value,
            Value = new DataValue
            {
                Value = newValue,
                StatusCode = StatusCodes.Good,
                ServerTimestamp = DateTime.UtcNow,
                SourceTimestamp = DateTime.UtcNow
            }
        };

        WriteValueCollection valuesToWrite = new WriteValueCollection { valueToWrite };
        StatusCodeCollection results;
        DiagnosticInfoCollection diagnosticInfos;

        session.Write(null, valuesToWrite, out results, out diagnosticInfos);
        if (StatusCode.IsGood(results[0]))
        {
            Console.WriteLine("Successfully wrote value to node.");
        }
        else
        {
            Console.WriteLine($"Failed to write value: {results[0]}");
        }
    }


}