using Hoeyer.OpcUa.Core.Entity;

namespace Hoeyer.OpcUa.EndToEndTest.TestEntities;

public class GantryLoader : IEntityLoader<Gantry>
{
    /// <inheritdoc />
    public ValueTask<Gantry> LoadCurrentState()
    {
        return new ValueTask<Gantry>(new Gantry
        {
            AList = new List<string>
            {
                "stnrei"
            },
            IntValue = 231,
            StringValue = "ntserioa"
        });
    }
}