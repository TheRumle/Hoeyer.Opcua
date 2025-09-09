using Hoeyer.OpcUa.EntityModelling.Methods;
using Hoeyer.OpcUa.Server.Api;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Hoeyer.OpcUa.EntityModelling.Reactants;

public class RandomContainerAssignmentReactor(
    ILogger<RandomContainerAssignmentReactor> logger,
    IEntityServerStartedMarker marker,
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