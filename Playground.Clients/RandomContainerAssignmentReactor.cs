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
        {
            try
            {
                var id = Guid.NewGuid();
                logger.LogInformation("Assigning {Id} to Gantry", id);
                await Task.Delay(TimeSpan.FromSeconds(2), stoppingToken);
                await methods.AssignContainer(id);
            }
            catch (Exception e)
            {
                logger.LogError("Exception occurred: {Message}", e.Message);
            }
        }
    }
}