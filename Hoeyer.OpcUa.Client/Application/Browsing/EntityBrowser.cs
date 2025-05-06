using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Hoeyer.Common.Extensions.Async;
using Hoeyer.Common.Extensions.LoggingExtensions;
using Hoeyer.Common.Extensions.Types;
using Hoeyer.OpcUa.Client.Api.Browsing;
using Hoeyer.OpcUa.Client.Api.Browsing.Reading;
using Hoeyer.OpcUa.Client.Api.Reading;
using Hoeyer.OpcUa.Client.Extensions;
using Hoeyer.OpcUa.Client.MachineProxy;
using Hoeyer.OpcUa.Core;
using Hoeyer.OpcUa.Core.Api;
using Microsoft.Extensions.Logging;
using Opc.Ua;
using Opc.Ua.Client;

namespace Hoeyer.OpcUa.Client.Application.Browsing;

/// <summary>
/// A client for browsing an entity - traverses the node tree from root using the <paramref name="traversalStrategy"/> and until a match is found using <paramref name="identityMatcher"/> - the match is deemed to be the entity node.
/// If the node has already been found previously, the tree is not traversed again.
/// BrowseById specific Object node representing an assigned entity. Holds BrowseById reference to the ObjectNode (once the first browse has been done successfully)
/// </summary>
/// <param name="logger">A logger to log browse exceptions and diagnostics</param>
/// <param name="traversalStrategy">A strategy for traversing the node tree</param>
/// <param name="identityMatcher">True if the provided <see cref="ReferenceDescription"/> is a description for the entity - if null, equality between the name of the <typeparamref name="TEntity"/> and the browse name of nodes is used.</param>
/// <typeparam name="TEntity">The entity the EntityBrowser is assigned to</typeparam>
[OpcUaEntityService(typeof(IEntityBrowser<>))]
public sealed class EntityBrowser<TEntity>(
    ILogger<EntityBrowser<TEntity>> logger,
    INodeTreeTraverser traversalStrategy,
    IEntitySessionFactory sessionFactory,
    INodeReader reader,
    IEntityNodeStructureFactory<TEntity> nodeStructureFactory,
    EntityDescriptionMatcher<TEntity>? identityMatcher = null) : IEntityBrowser<TEntity>
{
    private Node? _entityRoot;
    private static readonly string EntityName = typeof(TEntity).Name;
    private readonly EntityDescriptionMatcher<TEntity> _identityMatcher 
        = identityMatcher ?? (n => EntityName.Equals(n.BrowseName.Name));

    private readonly Lazy<ISession> _session = new(() => sessionFactory.CreateSession(typeof(TEntity).Name + "Browser"));
    private ISession Session => _session.Value; 
    
    public (IEntityNode? node, DateTime timeLoaded)? LastState { get; private set; }

    private async Task<Node> FindEntityRoot(CancellationToken cancellationToken = default)
    {
        return await logger.LogWithScopeAsync(new
        {
            Session = Session.ToLoggingObject(),
            Entity = EntityName,
        }, async () =>
        {
            var r = await traversalStrategy
                .TraverseUntil(Session,
                    ObjectIds.RootFolder,
                    predicate: _identityMatcher.Invoke,
                    token: cancellationToken);

            return await reader.ReadNodeAsync(Session, r.NodeId, cancellationToken);
        });

    }

    /// <inheritdoc />
    public async Task<IEntityNode> BrowseEntityNode(CancellationToken cancellationToken = default)
    {
        var values = await ReadEntity(cancellationToken);
        return await ParseToEntity(Session, cancellationToken, values);
    }

    private async Task<IEntityNode> ParseToEntity(ISession session, CancellationToken cancellationToken, ReadResult values)
    {
        var variables = await reader
            .ReadNodesAsync(session, values.SuccesfulReads.Select(value => value!.NodeId),
                ct: cancellationToken)
            .ThenAsync(e => e.SuccesfulReads)
            .ThenAsync(nodes => nodes.Where(v => v switch
            {
                VariableNode => true,
                _ => false
            }))
            .ThenAsync( e => e.Select(node => node as VariableNode!).ToList());

        var (readValues, _) = await session.ReadValuesAsync(variables.Select(e=>e.NodeId).ToList(), cancellationToken);
        for (int i = 0; i < readValues.Count; i++)
        {
            variables[i]!.Value = readValues[i].WrappedValue;
        }

        var index = _entityRoot!.NodeId.NamespaceIndex;
        var structure = nodeStructureFactory.Create(index);
        foreach (var state in structure.PropertyStates)
        {
            
            var match = variables
                .FirstOrDefault(n => n != null && n.NodeId.Equals(state.NodeId));
            if (match != null)
            {
                state.Value = match.Value;
            }
        }
        LastState = new ValueTuple<IEntityNode, DateTime>(structure, DateTime.Now);
        return structure;
    }

    private async Task<ReadResult> ReadEntity(CancellationToken cancellationToken)
    {
        _entityRoot ??= await FindEntityRoot(cancellationToken);
        var descendants = await traversalStrategy.TraverseFrom(_entityRoot.NodeId, Session, cancellationToken).Collect();
        var values = await reader.ReadNodesAsync(Session, descendants.Select(e=>e.NodeId), ct: cancellationToken);
        return values;
    }
}