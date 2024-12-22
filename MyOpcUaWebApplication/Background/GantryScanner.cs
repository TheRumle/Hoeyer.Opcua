using Hoeyer.Machines.OpcUa.Client.Application;
using Hoeyer.Machines.OpcUa.Client.Application.MachineProxy;
using Hoeyer.Machines.Proxy;
using Microsoft.Extensions.Options;
using MyOpcUaWebApplication.Configuration.BackgroundService;
using Opc.Ua;

namespace MyOpcUaWebApplication.Background;

public class GantryScanner (
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
                EntityOpcuaServer server = new();
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
                var session = await factory.CreateSessionAsync();
                var node = await reader.ReadOpcUaEntityAsync(session);
                var f = new AddNodesItemCollection(new List<AddNodesItem>()
                {
                    new AddNodesItem()
                    {
                        BrowseName = "",
                        NodeAttributes = ,
                        NodeClass = ,
                        ParentNodeId = ,
                        ReferenceTypeId = ,
                        RequestedNewNodeId = ,
                        TypeDefinition = ,
                    }
                });
                server._server.AddNodes(header, , out var q, out var b);
                Console.WriteLine(node.Value);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
           
        }
    }
}

