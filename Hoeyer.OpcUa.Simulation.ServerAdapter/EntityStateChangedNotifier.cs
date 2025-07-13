using Hoeyer.Common.Messaging.Api;
using Hoeyer.OpcUa.Core.Api;
using Hoeyer.OpcUa.Server.Api.NodeManagement;
using Hoeyer.OpcUa.Simulation.Api.Execution;
using Hoeyer.OpcUa.Simulation.Api.PostProcessing;
using Microsoft.Extensions.Logging;
using Opc.Ua;

namespace Hoeyer.OpcUa.Simulation.ServerAdapter;

/// <summary>
/// Notifies the OpcUa server that the held entity has changed state when a simulation state change occurs.
/// </summary>
/// <param name="translator"></param>
/// <typeparam name="TEntity"></typeparam>
internal class EntityStateChangedNotifier<TEntity>(
    IEntityTranslator<TEntity> translator,
    ILogger<EntityStateChangedNotifier<TEntity>> logger) : INodeConfigurator<TEntity>,
    IStateChangeSimulationProcessor<TEntity>
{
    private ISystemContext Context { get; set; } = null!;
    public IManagedEntityNode ManagedNode { get; set; } = null!;
    public IMessageSubscription Subscription { get; set; } = null!;

    public void Configure(IManagedEntityNode managed, ISystemContext context)
    {
        Context = context;
        ManagedNode = managed;
    }

    public void Consume(IMessage<SimulationResult<TEntity>> message)
    {
        var (_, _, newState, _) = message.Payload;
        ManagedNode.ChangeState(node =>
        {
            node.BaseObject.UpdateChangeMasks(NodeStateChangeMasks.Children | NodeStateChangeMasks.Value);
            translator.AssignToNode(newState, node);
            node.BaseObject.ClearChangeMasks(Context, true);
        });
    }

    public void AssignContext(SimulationExecutionContext context)
    {
        Subscription = context.Subscription;
    }
}