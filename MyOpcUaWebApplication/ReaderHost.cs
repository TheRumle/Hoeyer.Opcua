using Hoeyer.OpcUa.Client.Application;
using Hoeyer.OpcUa.Client.Application.MachineProxy;
using Opc.Ua;

namespace MyOpcUaWebApplication;

public class ReaderHost(OpcEntityClient<Gantry> client, SessionFactory factory) : BackgroundService
{
    private readonly PeriodicTimer _scanTimer = new(TimeSpan.FromSeconds(5));

    /// <inheritdoc />
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (await _scanTimer.WaitForNextTickAsync(stoppingToken))
            try
            {
                var session = await factory.CreateSessionAsync();
                var node = await client.ReadOpcUaEntityAsync(session);
                return;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return;
            }
    }

}
