using Hoeyer.OpcUa.Core.Configuration.Health;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Playground.Application.host.healthcheck;

public sealed class ServerStartedHealthCheckAdapter(
    IServerStartedHealthCheck serverStarted,
    ILogger<ServerStartedHealthCheckAdapter> healthCheckLogger
) : IHealthCheck
{
    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        using var logScope = healthCheckLogger.BeginScope("Checking health for context {}",
            new
            {
                context.Registration.Timeout,
                context.Registration.Name,
                Tags = string.Join(", ", context.Registration.Tags)
            });
        healthCheckLogger.LogInformation("Beginning health check");

        var timeout = Task.Delay(context.Registration.Timeout, cancellationToken);
        await Task.WhenAny(serverStarted.ServerRunning(), timeout);
        var result = serverStarted.IsServerStarted
            ? HealthCheckResult.Healthy("Server is started and running.")
            : HealthCheckResult.Unhealthy("Server not yet started (timeout).");
        healthCheckLogger.LogInformation(result.Description);
        return result;
    }
}