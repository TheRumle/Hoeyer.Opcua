using Hoeyer.OpcUa.Core.Configuration.Application;
using Hoeyer.OpcUa.Core.Test.Fixtures;

namespace Hoeyer.OpcUa.Core.Test.Application;

[CoreServiceInjection]
public class ApplicationConfigurationTemplateFactoryTest(IApplicationConfigurationTemplateFactory templateFactory)
{
    [Test]
    [DisplayName("A configuration template can be created for the application")]
    public void CreateTemplate()
    {
        templateFactory.CreateTemplate();
    }
}