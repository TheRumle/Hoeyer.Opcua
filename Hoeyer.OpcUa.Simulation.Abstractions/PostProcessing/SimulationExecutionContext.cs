using Hoeyer.Common.Messaging.Api;

namespace Hoeyer.OpcUa.Simulation.Abstractions.PostProcessing;

public sealed record SimulationExecutionContext(IMessageSubscription Subscription)
{
    public IMessageSubscription Subscription { get; } = Subscription;
}