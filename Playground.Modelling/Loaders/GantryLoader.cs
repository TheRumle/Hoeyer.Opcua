using Hoeyer.OpcUa.EntityModelling.Models;
using Hoeyer.OpcUa.Server.Api;

namespace Hoeyer.OpcUa.EntityModelling.Loaders;

public class GantryLoader : IEntityLoader<Gantry>
{
    /// <inheritdoc />
    public ValueTask<Gantry> LoadCurrentState()
    {
        return new ValueTask<Gantry>(new Gantry
        {
            Occupied = false,
            HeldContainer = Guid.Empty,
            Position = Position.OnTheMoon,
            IntValue = 231,
            StringValue = "ntserioa",
            ListValue = ["neiodcxdv", "mne"]
        });
    }
}