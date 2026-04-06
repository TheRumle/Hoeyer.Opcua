using Hoeyer.OpcUa.Client.Api.Monitoring;
using Hoeyer.OpcUa.Test.Api;
using Hoeyer.OpcUa.Test.Api.Attributes;
using Hoeyer.OpcUa.Test.Simulation;

namespace Playground.Application.EndToEndTest.Core;

[ReadonlySimulationFixture<ISubscribedStateChangeMonitor>]
public sealed class SubscriptionStateChangeChannel(SimulationServiceContext<ISubscribedStateChangeMonitor> session)
{
    public List<ISimulationTestContext<ISubscribedStateChangeMonitor>> GetInstances() => session.GetSimulationSession();

    [Test]
    [DisplayName("Given a SubscribedStateChangeMonitor, the subscription is not paused or cancelled")]
    [InstanceMethodDataSource(nameof(GetInstances))]
    public async Task WhenProvidingSubscribedSession_SubscriptionIsOnByDefault(ISubscribedStateChangeMonitor service)
    {
        await Assert.That(service.Subscription.IsCancelled || service.Subscription.IsPaused).IsFalse();
    }
}