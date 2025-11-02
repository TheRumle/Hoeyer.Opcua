using Hoeyer.OpcUa.Core.Configuration.ServerTarget;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Playground.Modelling.Methods;

namespace Playground.Clients;

public class RandomContainerAssignmentReactor(
    ILogger<RandomContainerAssignmentReactor> logger,
    IServerStartedHealthCheck marker,
    IGantryMethods methods) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await marker.ServerRunning();
        while (!stoppingToken.IsCancellationRequested)
        {
            var id = Guid.NewGuid();
            await Task.Delay(TimeSpan.FromSeconds(2), stoppingToken);
            logger.LogInformation("Assigning {Id} to Gantry", id);
            await methods.AssignContainer(id);
        }
    }
}