using Hoeyer.Common.Extensions.Async;
using Hoeyer.Common.Extensions.Types;
using Hoeyer.OpcUa.Client.Abstractions.Browsing;
using Hoeyer.OpcUa.Client.Abstractions.Browsing.Reading;
using Hoeyer.OpcUa.Client.Abstractions.Connection;
using Hoeyer.OpcUa.Core.Abstractions;
using Microsoft.Extensions.Logging;
using Opc.Ua;
using Opc.Ua.Client;

namespace Hoeyer.OpcUa.Client.Application.Browsing;

/// <summary>
///     A client for browsing an entity - traverses the node tree from root using the <paramref name="traversalStrategy" />
///     and until a match is found using the browse name of the entity.
///     If the node has already been found previously, the tree is not traversed again.
/// </summary>
/// <param name="logger">A logger to log browse exceptions and diagnostics</param>
/// <param name="traversalStrategy">A strategy for traversing the node tree</param>
/// <typeparam name="TEntity">The entity the EntityBrowser is assigned to</typeparam>
public sealed class EntityBrowser<TEntity>(
    IBrowseNameCollection<TEntity> browseNameCollection,
    ILogger<EntityBrowser<TEntity>> logger,
    INodeTreeTraverser traversalStrategy,
    IEntitySessionFactory sessionFactory,
    INodeReader reader,
    IEntityNodeStructureFactory<TEntity> nodeStructureFactory) : IEntityBrowser<TEntity>
{
    private readonly EntityDescriptionMatcher<TEntity> _identityMatcher
        = n => browseNameCollection.EntityName.Equals(n.BrowseName.Name);

    private Node? _entityRoot;

    public (IEntityNode node, DateTime timeLoaded)? LastState { get; private set; }

    /// <inheritdoc />
    public async Task<IEntityNode> BrowseEntityNode(CancellationToken cancellationToken = default)
    {
        try
        {
            var session = await GetSession();
            logger.LogInformation("Browsing entity");
            var values = await ReadEntity(session, cancellationToken);
            return await ParseToEntity(session, cancellationToken, values);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Failed to browse entity '{name}' due to error: {errMessage}",
                browseNameCollection.EntityName, e.Message);
            throw;
        }
    }

    public async ValueTask<EntityNodeStructure> GetNodeStructure(CancellationToken token = default) =>
        LastState.HasValue
            ? LastState.Value.node.ToStructureOnly()
            : await BrowseEntityNode(token).SelectAsync(e => e.ToStructureOnly());

    private async Task<ISession> GetSession()
    {
        var entitySession = await sessionFactory.GetSessionForAsync<TEntity>();
        return entitySession.Session;
    }

    private async Task<IEntityNode> ParseToEntity(ISession session, CancellationToken cancellationToken,
        ReadResult values)
    {
        logger.LogDebug("Parsing entity");
        var variables = await reader
            .ReadNodesAsync(session, values.SuccesfulReads.Select(value => value!.NodeId),
                ct: cancellationToken)
            .SelectAsync(result => result.SuccesfulReads.OfType<VariableNode>())
            .SelectAsync(nodes => nodes.ToList());

        var structure = AssignReadValues(variables);
        return structure;
    }

    private IEntityNode AssignReadValues(List<VariableNode> variables)
    {
        logger.LogDebug("Assigning read values");
        var index = _entityRoot!.NodeId.NamespaceIndex;
        var structure = nodeStructureFactory.Create(index);
        foreach (var variable in variables)
        {
            logger.LogTrace("Assigning {variableName} to {value}", variable.BrowseName.Name, variable.Value);
            var browseName = variable.BrowseName.Name;
            var match = structure.PropertyByBrowseName.TryGetValue(browseName, out var currentValue);
            if (match)
            {
                currentValue!.Value = variable.Value;
            }
        }

        LastState = new ValueTuple<IEntityNode, DateTime>(structure, DateTime.Now);
        return structure;
    }

    private async Task<ReadResult> ReadEntity(ISession session, CancellationToken cancellationToken)
    {
        logger.LogInformation("Reading entity from server");
        _entityRoot ??= await FindEntityRoot(session, cancellationToken);
        var descendants = await traversalStrategy
            .TraverseFrom(_entityRoot.NodeId, session, cancellationToken)
            .Collect();
        return await reader.ReadNodesAsync(session, descendants.Select(e => e.NodeId), ct: cancellationToken);
    }

    private async Task<Node> FindEntityRoot(ISession session, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Looking for entity root...");
        var r = await traversalStrategy
            .TraverseUntil(session,
                ObjectIds.RootFolder,
                _identityMatcher.Invoke,
                cancellationToken);
        logger.LogInformation("Entity root found at {NodeId}", r.NodeId.ToString());
        return await reader.ReadNodeAsync(session, r.NodeId, cancellationToken);
    }
}