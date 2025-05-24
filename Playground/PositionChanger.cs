using Hoeyer.OpcUa.Server.Api;

namespace Playground;

public class PositionChanger(EntityServerStartedMarker marker, IGantryMethods methods) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await marker.ServerRunning();
        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(TimeSpan.FromSeconds(1), stoppingToken);
            Console.WriteLine("Calling the method GetRandomPosition resulted in " +
                              await methods.GetRandomPosition("q", 2f, []));
        }
    }
}