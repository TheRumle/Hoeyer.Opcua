using Hoeyer.OpcUa.Test.Api;
using Hoeyer.OpcUa.Test.Api.Attributes;

namespace Hoeyer.OpcUa.TestFramework.Test;

[PerClassSimulationFixture]
public class PerClassSimulationFixtureAttributeTest(ISimulationTestSession thirdSession)
{
    [PerClassSimulationFixture] public required SimulationTestSession FirstSession { get; set; }

    [PerClassSimulationFixture] public required SimulationTestSession SecondSession { get; set; }

    public IEnumerable<Func<(ISimulationTestSession first, ISimulationTestSession second)>> GetSessions()
    {
        yield return () => (FirstSession, SecondSession);
        yield return () => (SecondSession, thirdSession);
        yield return () => (thirdSession, FirstSession);
    }

    [Test]
    [DisplayName("No matter how sessions is injected as PerClass, it is the same instance")]
    [InstanceMethodDataSource(nameof(GetSessions))]
    public async Task AllSessionsAreTheSame(
        ISimulationTestSession first,
        ISimulationTestSession second
    ) => await Assert.That(first).IsSameReferenceAs(second);
}