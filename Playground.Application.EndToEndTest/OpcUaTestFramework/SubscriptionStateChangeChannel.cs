using Hoeyer.OpcUa.Client.Api.Monitoring;
using Hoeyer.OpcUa.Test;
using Hoeyer.OpcUa.Test.Api;
using Hoeyer.OpcUa.Test.Simulation;

namespace Playground.Application.EndToEndTest.OpcUaTestFramework;

[ClassDataSource<AdaptedSimulationServiceContext<ISubscribedStateChangeMonitor>>(Key = FixtureKeys.ReadOnlyFixture,
    Shared = SharedType.Keyed)]
public sealed class SubscriptionStateChangeChannel(List<ISpecifiedTestSession<ISubscribedStateChangeMonitor>> sessions)
{
    [Test]
    [DisplayName("Given a SubscribedStateChangeMonitor, the subscription is not paused or cancelled")]
    public async Task WhenProvidingSubscribedSession_SubscriptionIsOnByDefault()
    {
        await sessions.AssertThatService(async e =>
            await Assert.That(e.Subscription.IsCancelled || e.Subscription.IsPaused).IsFalse());
    }
}