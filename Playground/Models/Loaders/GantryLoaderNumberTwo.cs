using Hoeyer.OpcUa.Server.Api;

namespace Playground.Models.Loaders;

public class GantryLoaderNumberTwo : IEntityLoader<GantryNumberTwo>
{
    /// <inheritdoc />
    public ValueTask<GantryNumberTwo> LoadCurrentState()
    {
        return ValueTask.FromResult(new GantryNumberTwo
        {
            message = "I hate to be second",
            messages = ["Oh boy do I hate it", "I don't know why I write it down"],
            Moving = true,
            Position = Position.OverThere,
            Speeds = [2231, 1]
        });
    }
}