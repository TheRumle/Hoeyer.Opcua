using Hoeyer.OpcUa.Server.Api;
using Hoeyer.OpcUa.TestEntities.Methods;

namespace Hoeyer.OpcUa.TestEntities.Loaders;

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
            AList = ["stnrei", "tsneriaotnsreiotnrsaeitsra"],
            IntValue = 231,
            StringValue = "ntserioa",
            AAginList = ["neiodcxdv", "mne"]
        });
    }
}