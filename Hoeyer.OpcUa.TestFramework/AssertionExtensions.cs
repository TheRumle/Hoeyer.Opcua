using Hoeyer.OpcUa.Test.Api;

namespace Hoeyer.OpcUa.Test;

public static class AssertionExtensions
{
    public static async Task AssertAllServices<T>(this SimulationTestServiceList<T> sessions,
        Func<T, Task> assertion)
    {
        using var multiAssert = Assert.Multiple();
        foreach (var session in sessions)
        {
            await assertion.Invoke(session.TestedService);
        }
    }
}