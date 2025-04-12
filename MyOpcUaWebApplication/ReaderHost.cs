using FluentResults.Extensions;
using Hoeyer.Common.Extensions.Types;
using Hoeyer.OpcUa.Client.Application.Browsing;
using Hoeyer.OpcUa.Client.Extensions;
using Hoeyer.OpcUa.Client.MachineProxy;
using Hoeyer.OpcUa.Core.Extensions;
using Hoeyer.OpcUa.Server.Core;
using Opc.Ua;

namespace MyOpcUaWebApplication;

public class ReaderHost(IEntityBrowser<Gantry> client, IEntitySessionFactory factory, EntityServerStartedMarker marker, ILogger<ReaderHost> logger) : BackgroundService
{
    /// <inheritdoc />
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await marker.ServerRunning();
        var session = await factory.CreateSessionAsync("Gantry browser");
        var node = await client
            .BrowseEntityNode(session, stoppingToken)
            .AsResultTask(e => e.Children.Select(child => child.NodeId.AsNodeId(session.NamespaceUris)).ToList())
            .Bind(ids => session.ReadValuesAsync(ids, ct: stoppingToken).AsResultTask());
            

        if (node.IsFailed) logger.LogError("Errors: {@Errs}", node.Errors.ToLoggingObject());
        else
        {
            
            logger.LogInformation("Result: {@R}", node.Value.Item1.Select(e=>e.Value).ToList());
        }
    }

}
