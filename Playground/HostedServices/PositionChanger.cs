using Hoeyer.OpcUa.Server.Api;
using Playground.Models;
using Playground.Models.Methods;

namespace Playground.HostedServices;

public class PositionChanger(EntityServerStartedMarker marker, IGantryMethods methods, ILogger<PositionChanger> logger)
    : BackgroundService
{
    public async Task ChangePosition(Position p)
    {
        await marker.ServerRunning();
        logger.LogInformation("Attempting to change position to {Name}", p);
        await methods.ChangePosition(p);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
    }
}