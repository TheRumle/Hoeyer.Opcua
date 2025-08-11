using Hoeyer.OpcUa.Server.Api;

namespace Hoeyer.OpcUa.TestEntities.Loaders;

public class OtherGantryLoader : IAgentLoader<OtherGantry>
{
    /// <inheritdoc />
    public ValueTask<OtherGantry> LoadCurrentState()
    {
        return new ValueTask<OtherGantry>(new OtherGantry
        {
            OtherGantryAListOfSomeSort = Enumerable.Range(1, 10).Select(e => Guid.NewGuid()).ToList(),
            OtherGantryIntValue = 231,
            OtherGantryStringValue = "This is another string value"
        });
    }
}