using Hoeyer.OpcUa.Server.Api;

namespace Hoeyer.opcUa.TestEntities.Loaders;

public class GantryLoader : IEntityLoader<Gantry>
{
    /// <inheritdoc />
    public ValueTask<Gantry> LoadCurrentState()
    {
        return new ValueTask<Gantry>(new Gantry
        {
            AList = ["stnrei", "tsneriaotnsreiotnrsaeitsra"],
            IntValue = 231,
            StringValue = "ntserioa",
            AAginList = ["neiodcxdv", "mne"]
        });
    }
}