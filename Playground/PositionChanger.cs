using Hoeyer.OpcUa.Server.Api;

namespace Playground;

public class PositionChanger(EntityServerStartedMarker marker, IGantryMethods methods) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await marker.ServerRunning();
        while (!stoppingToken.IsCancellationRequested)
        {
            await methods.Int(new Random().Next(), new Random().Next(),
                Enumerable.Range(0, 10).Select(_ => new Random().Next()).ToList());
            await Task.Delay(TimeSpan.FromSeconds(1), stoppingToken);
        }
    }
}