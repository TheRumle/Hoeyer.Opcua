using Hoeyer.Machines.Proxy;
using Microsoft.Extensions.Options;
using MyOpcUaWebApplication.Configuration.BackgroundService;

namespace MyOpcUaWebApplication.Background;

public class GantryScanner (
    IOptions<GantryScannerOptions> gantryOptions,
    IRemoteMachineObserver<Gantry> gantry,
    ILogger<GantryScanner> logger ) : BackgroundService
{

    private PeriodicTimer? _scanTimer = null;
    


    /// <inheritdoc />
    protected override async Task ExecuteAsync(CancellationToken cancel)
    {
        logger.LogInformation("Scanning Gantry will begin in 10 seconds");
        _scanTimer = new(TimeSpan.FromMilliseconds(gantryOptions.Value.IntervalMs));
        
        while (!cancel.IsCancellationRequested && await _scanTimer.WaitForNextTickAsync(cancel))
        {
            var result = await gantry.ReadEntityAsync(cancel);
            logger.LogInformation("Gantry read: {Result}", result.Value);
        }
    }
}

