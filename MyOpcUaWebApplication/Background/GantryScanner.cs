using Hoeyer.Machines.OpcUa.Client.Application;
using Hoeyer.Machines.OpcUa.Client.Application.MachineProxy;
using Hoeyer.Machines.OpcUa.Server;
using Hoeyer.Machines.Proxy;
using Microsoft.Extensions.Options;
using MyOpcUaWebApplication.Configuration.BackgroundService;
using Opc.Ua;

namespace MyOpcUaWebApplication.Background;

public class GantryScanner (
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
                using EntityOpcuaServer server = new(serverOptions.Value, rootOptions.Value.ApplicationName);
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
                server.Start();
                server.Server.AddNodes(header, f, out var q, out var b);
                var session = await factory.CreateSessionAsync();
                var node = await reader.ReadOpcUaEntityAsync(session);
                Console.WriteLine(node.Value);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
           
        }
    }
}

