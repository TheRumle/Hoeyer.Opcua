using Hoeyer.OpcUa.Client.Application;
using Hoeyer.OpcUa.Client.Application.MachineProxy;
using Hoeyer.OpcUa.Server.Core;
using Microsoft.Extensions.Options;
using Opc.Ua;

namespace MyOpcUaWebApplication;

public class GantryScanner(
    OpcUaEntityServerFactory serverFactory,
    OpcEntityClient<Gantry> client,
    SessionFactory factory) : BackgroundService
{
    private readonly PeriodicTimer _scanTimer = new(TimeSpan.FromSeconds(100));


    /// <inheritdoc />
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (await _scanTimer.WaitForNextTickAsync(stoppingToken))
            try
            {
                var driver = serverFactory.CreateServer();
                var header = new RequestHeader
                {
                    AuthenticationToken = null,
                    Timestamp = DateTime.Now,
                    RequestHandle = 0,
                    ReturnDiagnostics = (int)DiagnosticsLevel.Basic,
                    AuditEntryId = null,
                    TimeoutHint = 0,
                    AdditionalHeader = null
                };

                await driver.StartAsync();
                var session = await factory.CreateSessionAsync();
                var node = await client.ReadOpcUaEntityAsync(session);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
    }
}