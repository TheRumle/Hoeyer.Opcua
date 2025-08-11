using Hoeyer.OpcUa.Server.Api;

namespace Playground.Models.Loaders;

public class GantryLoader : IEntityLoader<Gantry>
{
    /// <inheritdoc />
    public ValueTask<Gantry> LoadCurrentState()
    {
        return ValueTask.FromResult(new Gantry
        {
            message = "tsnerio",
            messages = ["stnrei", "snterio"],
            Moving = true,
            Position = Position.OnTheMoon,
            Speeds = [2, 1],
            BB = ["stneri,", "stnreio"]
        });
    }
}