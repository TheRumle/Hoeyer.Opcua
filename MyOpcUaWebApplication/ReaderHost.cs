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
        await marker.ServerRunning();
        var session = await factory.CreateSessionAsync("Gantry browser");
        await client
            .BrowseEntityNode(stoppingToken)
            .ThenAsync(e => e.PropertyStates.Select(child => child.NodeId).ToList())
            .ThenAsync(ids => session.ReadValuesAsync(ids, ct: stoppingToken));
    }

}
