using System.Collections.Generic;
using System.Linq;
using Hoeyer.OpcUa.Server.Configuration;
using Hoeyer.OpcUa.Server.Entity;
using Opc.Ua;
using Opc.Ua.Configuration;

namespace Hoeyer.OpcUa.Server.Application;

/// <summary>
/// Given number of <see cref="IEntityObjectStateCreator"/> and an <see cref="IApplicationConfigurationFactory"/>, will construct instances of <see cref="ApplicationInstance"/>, load the <see cref="ApplicationConfiguration"/> provided by the factory into the instance. The <see cref="IEntityObjectStateCreator"/> are used to create OpcUa nodes for all classes marked with the <see cref="OpcUaEntityAttribute"/>. 
/// </summary>
/// <param name="entityObjectCreators">A factory creating node managers for the different entities.</param>
/// <param name="configurationFactory">A factory that can create an application the server will use.</param>
/// <returns>A <see cref="StartableEntityServer"/>which encapsulates the Application instance running the server</returns>
public sealed class OpcUaEntityServerFactory(
    IApplicationConfigurationFactory configurationFactory,
    IEnumerable<IEntityObjectStateCreator> entityObjectCreators)
{
    public StartableEntityServer CreateServer()
    {
        var configuration = configurationFactory.CreateServerConfiguration("EntityServer", []);
        var application = new ApplicationInstance
        {
            ApplicationConfiguration = configuration,
            ApplicationName = configurationFactory.ApplicationName,
            ApplicationType = ApplicationType.Server
        };
        application.LoadApplicationConfiguration(false);
        
        var server = new OpcEntityServer(entityObjectCreators, configuration.ApplicationUri);
        return new StartableEntityServer(application, server);
    }
}