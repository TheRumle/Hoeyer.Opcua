using Hoeyer.OpcUa.Core.Entity;

namespace MyOpcUaWebApplication;

public class GantryLoader : IEntityLoader<Gantry>
{
    /// <inheritdoc />
    public ValueTask<Gantry> LoadCurrentState()
    {
        return ValueTask.FromResult(new Gantry
        {
            Speeds = new List<int>
            {
                21,
                32,
                12,
                3
            },
            messages = new List<string>
            {
                "What a load",
                "What a load",
                " of nonsense"
            },
            message = "OPCUA SAYS HELLO",
            Moving = false,
            Position = Position.OverThere,
        });
    }
}