using DotNet.Testcontainers.Containers;

namespace Playground.Application.EndToEndTest.Environment.Adapter.TestContainer;

public sealed class DockerHealthChecker(IContainer container)
{
    public async Task<TestcontainersHealthStatus> HealthCheck()
    {
        var dockerHealth = container.Health;
        Console.WriteLine($"Docker health: {dockerHealth}");
        if (dockerHealth != TestcontainersHealthStatus.Healthy)
        {
            (string Stdout, string Stderr) logs = await container.GetLogsAsync();
            Console.WriteLine($" Container was not healthy. Error logs: {logs.Stderr}");
        }


        return dockerHealth;
    }

    public async Task<bool> IsHealthy() => await HealthCheck() == TestcontainersHealthStatus.Healthy;
}