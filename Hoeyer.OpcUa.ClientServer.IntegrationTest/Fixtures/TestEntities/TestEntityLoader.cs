using Hoeyer.OpcUa.Server.Abstractions;

namespace Hoeyer.OpcUa.IntegrationTest.Fixtures.TestEntities;

public sealed class TestEntityLoader : IEntityLoader<TestEntity>
{
    public ValueTask<TestEntity> LoadCurrentState() => new(TestEntity.CreateRandom());
}