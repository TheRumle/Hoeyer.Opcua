using Hoeyer.OpcUa.Test.Simulation;

namespace Hoeyer.OpcUa.Test;

public static class AssertionExtensions
{
    public static async Task AssertThatService<T>(this IEnumerable<ISpecifiedTestSession<T>> sessions,
        Func<T, Task> assertion)
    {
        using var multiAssert = Assert.Multiple();
        foreach (var session in sessions)
        {
            await assertion.Invoke(session.TestedService);
        }
    }

    public static async Task AssertThatSimulation<T>(this IEnumerable<ISpecifiedTestSession<T>> sessions,
        Func<ISpecifiedTestSession<T>, Task> assertion)
    {
        using var multiAssert = Assert.Multiple();
        foreach (var session in sessions)
        {
            await assertion.Invoke(session);
        }
    }
}