using TUnit.Core.Interfaces;

namespace Hoeyer.OpcUa.IntegrationTest.TUnitConfiguration;

public sealed class MultiAssertExecutor : ITestExecutor
{
    public ValueTask ExecuteTest(TestContext context, Func<ValueTask> action)
    {
        using (Assert.Multiple())
        {
            return action();
        }
    }
}