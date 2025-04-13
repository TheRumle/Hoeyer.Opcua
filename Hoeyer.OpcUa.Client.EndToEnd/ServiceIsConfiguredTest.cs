using Hoeyer.OpcUa.Client.Application.Browsing;
using Hoeyer.OpcUa.Client.EndToEnd.Generators;
using Hoeyer.OpcUa.Core.Entity;
using Hoeyer.OpcUa.Core.Entity.Node;
using Hoeyer.OpcUa.Core.Services;
using Hoeyer.OpcUa.Server.Entity;

namespace Hoeyer.OpcUa.Client.EndToEnd;


public sealed class ServiceIsConfiguredTest
{
    [Test]
    [ServiceImplementationFixture<IEntityBrowser>]
    public async Task CanGetEntityBrowser(ApplicationFixture<IEntityBrowser> fixture)
    {
       await Assert.That(await fixture.GetFixture()).IsNotDefault();
    }
    
    [Test]
    [ServiceImplementationFixture<IEntityTranslator>]
    public async Task CanGetEntityTranslator(ApplicationFixture<IEntityTranslator> fixture) => await AssertServiceNotNull(fixture);

    [Test]
    [ServiceImplementationFixture<IEntityNodeStructureFactory>]
    public async Task CanGetEntityNodeStructureFactory(ApplicationFixture<IEntityNodeStructureFactory> fixture) => await AssertServiceNotNull(fixture);
    
    [Test]
    [ClassDataSource<ApplicationFixture>]
    public async Task CanGetEntityInitializers(ApplicationFixture fixture)
    {
        var initializers = fixture.GetService<IEnumerable<IEntityInitializer>>();
        await Assert.That(initializers).IsNotDefault();
        await Assert.That(initializers).IsNotEmpty();
    }

    private static async Task AssertServiceNotNull<T>(ApplicationFixture<T> fixture)
    {
        await Assert.That(await fixture.GetFixture()).IsNotDefault();
    }
    
}