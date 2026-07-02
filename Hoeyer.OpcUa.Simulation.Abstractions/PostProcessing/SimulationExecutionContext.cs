using Hoeyer.Common.Messaging.Api;

namespace Hoeyer.OpcUa.Simulation.Api.PostProcessing;

public sealed record SimulationExecutionContext(IMessageSubscription Subscription)
{
    public IMessageSubscription Subscription { get; } = Subscription;
}