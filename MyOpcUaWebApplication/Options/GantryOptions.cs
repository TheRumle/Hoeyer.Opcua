namespace MyOpcUaWebApplication.Options;

public class GantryOptions
{
    public const string APPCONFIG_SECTION = "Gantry";
    
    public string Name { get; init; } 
    public string Id { get; init; }
    public float Speed { get; init; }
}