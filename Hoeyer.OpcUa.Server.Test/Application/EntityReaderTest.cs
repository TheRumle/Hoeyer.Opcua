using Hoeyer.OpcUa.Core.Entity;
using Hoeyer.OpcUa.Server.Application;
using Hoeyer.OpcUa.Server.Entity.Api;
using Hoeyer.OpcUa.Server.Test.Fixtures.Application;
using Hoeyer.OpcUa.Server.Test.Fixtures.Generators;

namespace Hoeyer.OpcUa.Server.Test.Application;

[EntityFixtureGenerator]
public class EntityReaderTest(EntityReaderFixture fixture)
{
    private IEntityReader _reader = new EntityReader(fixture.Node, new PropertyReader());

    public async Task a()
    {
        //TODO what to read
        ///_reader.ReadProperties(fixture.);
    }

}