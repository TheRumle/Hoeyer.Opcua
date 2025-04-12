using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Hoeyer.Common.Extensions.LoggingExtensions;
using Hoeyer.Common.Extensions.Types;
using Hoeyer.OpcUa.Client.Application.Reading;
using Hoeyer.OpcUa.Client.Extensions;
using Hoeyer.OpcUa.Core.Extensions;
using Hoeyer.OpcUa.Core.Extensions.Logging;
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
[ClientService]
public sealed class EntityBrowser<TEntity>(
    ILogger<EntityBrowser<TEntity>> logger,
    INodeTreeTraverser traversalStrategy,
    INodeReader reader,
    EntityDescriptionMatcher<TEntity>? identityMatcher = null) : IEntityBrowser<TEntity>
{
    private Node? _entityNode;
    private static readonly string EntityName = typeof(TEntity).Name;
    private readonly EntityDescriptionMatcher<TEntity> _identityMatcher 
        = identityMatcher ?? (n => EntityName.Equals(n.BrowseName.Name));


    public async Task<EntityReadResult> BrowseEntityNode(
        ISession session,
        NodeId treeRoot,
        CancellationToken cancellationToken = default)
    {
        return await logger.LogCaughtExceptionAs(LogLevel.Error, exception => new EntityBrowseException(exception.Message))
            .WithScope("Browsing entity")
            .WithErrorMessage("An error occured while browsing  ")
            .WhenExecutingAsync(async () =>
            {
                using var scope = logger.BeginScope(new
                {
                    Session = session.ToLoggingObject(),
                    Entity = EntityName,
                    TreeRoot = treeRoot,
                });
                
                Node node = _entityNode ?? await FindEntityNode(session, treeRoot, (p) => _identityMatcher.Invoke(p), cancellationToken);
                
                _entityNode = node;
                IEnumerable<ReferenceDescription> children = await Browse(session, cancellationToken: cancellationToken)
                    .ThenAsync(e => e.SelectMany(child => child.References));
                return new EntityReadResult(node, children);
            });
    }

    private async Task<Node> FindEntityNode(ISession session,
        NodeId root,
        Predicate<ReferenceDescription> entityReferenceMatcher,
        CancellationToken cancellationToken)
    {
        return await logger.LogCaughtExceptionAs(LogLevel.Error, exception => new EntityBrowseException(exception.Message))
            .WithScope("Locating entity node")
            .WithErrorMessage("Failed to locate entity node")
            .WhenExecutingAsync(async () =>
            {
                return await traversalStrategy
                    .TraverseUntil(session,
                        root,
                        predicate: entityReferenceMatcher,
                        token: cancellationToken)
                    .ThenAsync(referenceDescription => referenceDescription.NodeId.AsNodeId(session.NamespaceUris))
                    .ThenAsync(nodeId => reader.ReadNodeAsync(session, nodeId, cancellationToken));
            });
    }

    private Task<IEnumerable<BrowseResult>> Browse(ISession session,
        NodeClass filter = NodeClassFilters.EntityData,
        CancellationToken cancellationToken = default)
    {
        return session.BrowseAsync(
                null,
                null,
                250u,
                new BrowseDescriptionCollection
                {
                    new BrowseDescription
                    {
                        BrowseDirection = BrowseDirection.Forward,
                        NodeId = _entityNode!.NodeId,
                        ResultMask = (uint)BrowseResultMask.All,
                        NodeClassMask = (uint)filter,
                    }
                },
                cancellationToken)
            .ThenAsync(response =>
            {
                if (!response.DiagnosticInfos.Any()) return;
                logger.LogInformation("Browsing diagnostics for {@Node}: {@Diagnostics}",
                    _entityNode.ToLoggingObject(),
                    response.DiagnosticInfos.ToLoggingObject());
            })
            .ThenAsync(e => e.Results.Select(result => result));
    }
}