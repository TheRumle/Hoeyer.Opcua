using MyOpcUaWebApplication.Configuration.OpcUa.Options;

namespace MyOpcUaWebApplication.Configuration.BackgroundService;

public class GantryScannerOptions
{
    public const string APPCONFIG_SECTION = GantryOptions.APPCONFIG_SECTION + ":ScanningOptions";
    public int IntervalMs { get; init; } 
}