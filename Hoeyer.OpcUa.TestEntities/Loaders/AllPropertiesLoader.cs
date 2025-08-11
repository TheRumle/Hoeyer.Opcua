using Hoeyer.OpcUa.Server.Api;

namespace Hoeyer.OpcUa.TestEntities.Loaders;

public class AllPropertiesLoader : IAgentLoader<AllPropertyTypesAgent>
{
    /// <inheritdoc />
    public ValueTask<AllPropertyTypesAgent> LoadCurrentState() =>
        new(new AllPropertyTypesAgent
        {
            Bool = true,
            Double = 2.123,
            Float = 2.231321f,
            Guid = Guid.CreateVersion7(),
            Integer = 2123,
            IntList = [213, 31, 1, 2],
            Long = 15L,
            String = "Hello there"
        });
}