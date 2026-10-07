using Hoeyer.OpcUa.Server.Abstractions;

namespace Hoeyer.OpcUa.IntegrationTest.Fixtures.TestEntities;

public sealed class TestEntityLoader : IEntityLoader<DifferentFieldsEntity>
{
    public ValueTask<DifferentFieldsEntity> LoadCurrentState() => new(DifferentFieldsEntity.CreateRandom());
}