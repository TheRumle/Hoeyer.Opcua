using Hoeyer.OpcUa.Client.Application;
using Hoeyer.OpcUa.Client.Application.MachineProxy;
using Hoeyer.OpcUa.Server;
using Hoeyer.OpcUa.Server.Configuration;
using Microsoft.Extensions.Options;
using MyOpcUaWebApplication.Configuration.BackgroundService;
using Opc.Ua;

namespace MyOpcUaWebApplication.Background;

public class GantryScanner (
    OpcUaEntityServerFactory serverFactory,
    IOptions<OpcUaServerOptions> serverOptions,
    IOptions<OpcUaApplicationOptions> rootOptions,
    IOptions<GantryScannerOptions> gantryOptions,
    OpcUaEntityReader<Gantry> reader,
    SessionFactory factory ) : BackgroundService
{
    private PeriodicTimer _scanTimer = new(TimeSpan.FromMilliseconds(gantryOptions.Value.IntervalMs));
    


    /// <inheritdoc />
    protected override async Task ExecuteAsync(CancellationToken cancel)
    {
        while (await _scanTimer.WaitForNextTickAsync(cancel))
        {
            try
            {
                var driver = serverFactory.CreateServer();
                var header = new RequestHeader
                {
                    AuthenticationToken = null,
                    Timestamp = default,
                    RequestHandle = 0,
                    ReturnDiagnostics = 0,
                    AuditEntryId = null,
                    TimeoutHint = 0,
                    AdditionalHeader = null
                };

                var f = new AddNodesItemCollection(new List<AddNodesItem>()
                {
                    new AddNodesItem()
                    {
                        BrowseName = "ast",
                        NodeClass = NodeClass.Object
                    }
                });
                await driver.StartAsync();
                driver.EntityServer.AddNodes(header, f, out var q, out var b);
                var session = await factory.CreateSessionAsync();
                var node = await reader.ReadOpcUaEntityAsync(session);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
           
        }
    }
}

