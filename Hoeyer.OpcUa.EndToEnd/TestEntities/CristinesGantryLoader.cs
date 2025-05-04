using Hoeyer.OpcUa.Core.Api;


namespace Hoeyer.OpcUa.EndToEndTest.TestEntities;

public class CristinesGantryLoader : IEntityLoader<CristinesGantry>
{
    /// <inheritdoc />
    public ValueTask<CristinesGantry> LoadCurrentState()
    {
        return new ValueTask<CristinesGantry>(new CristinesGantry
        {
            AList = new List<string>
            {
                "stnreisss"
            },
            IntValue = 231,
            StringValue = "HEJSA CRssISTINE"
        });
    }
}