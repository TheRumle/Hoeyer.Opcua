using TUnit.Core.Interfaces;

namespace Playground.Application.EndToEndTest;

public sealed class MultiAssertExecutor : ITestExecutor
{
    public ValueTask ExecuteTest(TestContext context, Func<ValueTask> action)
    {
        using (Assert.Multiple()) return action();
    }
}