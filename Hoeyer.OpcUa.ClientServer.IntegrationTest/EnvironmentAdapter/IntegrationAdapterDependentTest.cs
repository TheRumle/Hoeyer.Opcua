namespace Hoeyer.OpcUa.IntegrationTest.EnvironmentAdapter;

public sealed class IntegrationAdapterDependentTest()
    : SkipAttribute(NoFrameworkAdapterException.ErrorMessage)
{
    private static readonly Task EnvironmentCheckTask = CheckEnvironmentAsync();
    private Exception? _skipReason = null;

    private static Task CheckEnvironmentAsync()
    {
        try
        {
            //create one, but never initialize environment.
            var integrationEnv = IntegrationTestAdapter.CreateOrGetCached(nameof(IntegrationAdapterDependentTest));
            if (integrationEnv == null!) return Task.FromException(new NoFrameworkAdapterException());
            return Task.CompletedTask;
        }
        catch (Exception e)
        {
            return Task.FromException(e);
        }
    }

    protected override string GetSkipReason(TestRegisteredContext context) =>
        _skipReason?.Message ?? base.GetSkipReason(context);

    public override async Task<bool> ShouldSkip(TestRegisteredContext context)
    {
        try
        {
            await EnvironmentCheckTask;
            return false;
        }
        catch (Exception e)
        {
            _skipReason = e;
            return true;
        }
    }
}