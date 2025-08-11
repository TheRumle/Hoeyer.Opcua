using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Hoeyer.Common.Extensions.Async;
using Hoeyer.Common.Extensions.LoggingExtensions;
using Hoeyer.Common.Extensions.Types;
using Hoeyer.OpcUa.Client.Api.Browsing;
using Hoeyer.OpcUa.Client.Api.Browsing.Reading;
using Hoeyer.OpcUa.Client.Api.Connection;
using Hoeyer.OpcUa.Client.Extensions;
using Hoeyer.OpcUa.Core;
using Hoeyer.OpcUa.Core.Api;
using Microsoft.Extensions.Logging;
using Opc.Ua;
using Opc.Ua.Client;

namespace Hoeyer.OpcUa.Client.Application.Browsing;

/// <summary>
///     A client for browsing an agent - traverses the node tree from root using the <paramref name="traversalStrategy" />
///     and until a match is found using <paramref name="idagentMatcher" /> - the match is deemed to be the agent node.
///     If the node has already been found previously, the tree is not traversed again.
/// </summary>
/// <param name="logger">A logger to log browse exceptions and diagnostics</param>
/// <param name="traversalStrategy">A strategy for traversing the node tree</param>
/// <param name="idagentMatcher">True if the provided <see cref="ReferenceDescription" /> is a description for the agent</param>
/// <typeparam name="TAgent">The agent the AgentBrowser is assigned to</typeparam>
[OpcUaAgentService(typeof(IAgentBrowser<>))]
public sealed class AgentBrowser<TAgent>(
    ILogger<AgentBrowser<TAgent>> logger,
    INodeTreeTraverser traversalStrategy,
    IAgentSessionFactory sessionFactory,
    INodeReader reader,
    IAgentStructureFactory<TAgent> nodeStructureFactory,
    AgentDescriptionMatcher<TAgent>? idagentMatcher = null) : IAgentBrowser<TAgent>
{
    private static readonly string AgentName = typeof(TAgent).Name;

    private readonly AgentDescriptionMatcher<TAgent> _idagentMatcher
        = idagentMatcher ?? (n => AgentName.Equals(n.BrowseName.Name));

    private readonly Lazy<ISession>
        _session = new(() => sessionFactory.GetSessionFor(typeof(TAgent).Name + "Browser").Session);

    private Node? _agentRoot;
    private ISession Session => _session.Value;

    public (IAgent node, DateTime timeLoaded)? LastState { get; private set; }

    /// <inheritdoc />
    public async Task<IAgent> BrowseAgent(CancellationToken cancellationToken = default)
    {
        ReadResult values = await ReadAgent(cancellationToken);
        return await ParseToAgent(Session, cancellationToken, values);
    }

    public async ValueTask<AgentStructure> GetNodeStructure(CancellationToken token = default) =>
        LastState.HasValue
            ? LastState.Value.node.ToStructureOnly()
            : await BrowseAgent(token).ThenAsync(e => e.ToStructureOnly());

    private async Task<IAgent> ParseToAgent(ISession session, CancellationToken cancellationToken,
        ReadResult values)
    {
        List<VariableNode> variables = await reader
            .ReadNodesAsync(session, values.SuccesfulReads.Select(value => value!.NodeId),
                ct: cancellationToken)
            .ThenAsync(result => result.SuccesfulReads.OfType<VariableNode>())
            .ThenAsync(nodes => nodes.ToList());

        var structure = AssignReadValues(variables);
        return structure;
    }

    private IAgent AssignReadValues(List<VariableNode> variables)
    {
        var index = _agentRoot!.NodeId.NamespaceIndex;
        var structure = nodeStructureFactory.Create(index);
        foreach (PropertyState? state in structure.PropertyStates)
        {
            VariableNode? match = variables.FirstOrDefault(n => n != null && n.NodeId.Equals(state.NodeId));
            if (match != null)
            {
                state.Value = match.Value;
            }
        }

        LastState = new ValueTuple<IAgent, DateTime>(structure, DateTime.Now);

        return structure;
    }

    private async Task<ReadResult> ReadAgent(CancellationToken cancellationToken)
    {
        return await logger.LogWithScopeAsync(new
        {
            Session = Session.ToLoggingObject(),
            Agent = AgentName
        }, async () =>
        {
            _agentRoot ??= await FindAgentRoot(cancellationToken);
            IEnumerable<ReferenceWithId> descendants = await traversalStrategy
                .TraverseFrom(_agentRoot.NodeId, Session, cancellationToken)
                .Collect();
            return await reader.ReadNodesAsync(Session, descendants.Select(e => e.NodeId), ct: cancellationToken);
        });
    }

    private async Task<Node> FindAgentRoot(CancellationToken cancellationToken = default)
    {
        ReferenceWithId r = await traversalStrategy
            .TraverseUntil(Session,
                ObjectIds.RootFolder,
                _idagentMatcher.Invoke,
                cancellationToken);

        return await reader.ReadNodeAsync(Session, r.NodeId, cancellationToken);
    }
}