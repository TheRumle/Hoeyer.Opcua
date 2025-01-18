namespace MyOpcUaWebApplication.Configuration.OpcUa.Options;

public class GantryOptions
{
    public const string APPCONFIG_SECTION = "OpcUa:Gantry";

    public required string Name { get; init; }
    public required string Id { get; init; }
    public float Speed { get; init; }
}