using TUnit.Core.Interfaces;

namespace Hoeyer.OpcUa.Fixtures.Common.TUnit.Configuration;

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