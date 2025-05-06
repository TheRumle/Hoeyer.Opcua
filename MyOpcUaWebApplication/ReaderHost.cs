using Hoeyer.Common.Extensions.Types;
using Hoeyer.OpcUa.Client.Api.Browsing;
using Hoeyer.OpcUa.Client.MachineProxy;
using Hoeyer.OpcUa.Server.Api;

namespace MyOpcUaWebApplication;

public class ReaderHost(IEntityBrowser<Gantry> client, IEntitySessionFactory factory, EntityServerStartedMarker marker, ILogger<ReaderHost> logger) : BackgroundService
{
    /// <inheritdoc />
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var result = await client.BrowseEntityNode(stoppingToken);

            foreach (var (propertyName, propertyState) in result.PropertyByBrowseName)
            {
                Console.WriteLine(propertyName + " has the value " + propertyState.Value);
            }

            await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
        }
    }

}
