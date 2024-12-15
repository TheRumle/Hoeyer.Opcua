using Hoeyer.Machines.Proxy;
using Microsoft.Extensions.Options;
using MyOpcUaWebApplication.Configuration.BackgroundService;

namespace MyOpcUaWebApplication.BackGroundService;

public class GantryScanner(
    IOptions<GantryScannerOptions> gantryOptions,
    IRemoteMachineObserver<Gantry> gantry,
    ILogger<GantryScanner> logger ) : BackGroundService, IDisposable
{

    private PeriodicTimer? _scanTimer = null;
    
    /// <inheritdoc />
    public void Dispose()
    {
        // TODO release managed resources here
    }

}

