using Hoeyer.OpcUa.Core.Api;
using Hoeyer.OpcUa.Core.Entity;

namespace MyOpcUaWebApplication;

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
            Speeds = [2,1]
        });
    }
}