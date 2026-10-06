using Hoeyer.OpcUa.Fixtures.Server.ServerFixture;

namespace Hoeyer.OpcUa.Fixtures.Server.Attributes;

public sealed class ServerFixtureDependentTestAttribute()
    : SkipAttribute(NoFrameworkAdapterException.ErrorMessage)
{
    private static readonly Task EnvironmentCheckTask = CheckEnvironmentAsync();
    private Exception? _skipReason;

    private static Task CheckEnvironmentAsync()
    {
        try
        {
            //create one, but never initialize environment.
            var integrationEnv = ServerFixtureAdapter.CreateOrGetCached(nameof(ServerFixtureDependentTestAttribute));
            if (integrationEnv == null!)
            {
                return Task.FromException(new NoFrameworkAdapterException());
            }

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