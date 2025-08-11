using Hoeyer.Common.Messaging.Api;
using Hoeyer.OpcUa.Core.Api;
using Hoeyer.OpcUa.Server.Api.NodeManagement;
using Hoeyer.OpcUa.Simulation.Api.Execution;
using Hoeyer.OpcUa.Simulation.Api.PostProcessing;
using Opc.Ua;

namespace Hoeyer.OpcUa.Simulation.ServerAdapter;

/// <summary>
/// Notifies the OpcUa server that the held agent has changed state when a simulation state change occurs.
/// </summary>
/// <param name="translator"></param>
/// <typeparam name="TAgent"></typeparam>
internal class AgentStateChangedNotifier<TAgent>(
    IAgentTranslator<TAgent> translator) : INodeConfigurator<TAgent>,
    IStateChangeSimulationProcessor<TAgent>
{
    private ISystemContext Context { get; set; } = null!;
    public IManagedAgent ManagedNode { get; set; } = null!;
    public IMessageSubscription Subscription { get; set; } = null!;

    public void Configure(IManagedAgent managed, ISystemContext context)
    {
        Context = context;
        ManagedNode = managed;
    }

    public void Consume(IMessage<SimulationResult<TAgent>> message)
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