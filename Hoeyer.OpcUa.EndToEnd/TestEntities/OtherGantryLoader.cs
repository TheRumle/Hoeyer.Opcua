using Hoeyer.OpcUa.Core.Api;


namespace Hoeyer.OpcUa.EndToEndTest.TestEntities;

public class OtherGantryLoader : IEntityLoader<OtherGantry>
{
    /// <inheritdoc />
    public ValueTask<OtherGantry> LoadCurrentState()
    {
        return new ValueTask<OtherGantry>(new OtherGantry
        {
            AList = new List<string>
            {
                "stnreisss"
            },
            IntValue = 231,
            StringValue = "This is another string value"
        });
    }
}