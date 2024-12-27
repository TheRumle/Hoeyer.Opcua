using Hoeyer.OpcUa.Server.Entity;
using Opc.Ua;
using Opc.Ua.Configuration;
using Opc.Ua.Server;

namespace Hoeyer.OpcUa.Server;

/// <summary>
/// Given an <see cref="IEntityNodeManagerFactory"/> and <see cref="IApplicationConfigurationFactory"/>, will construct instances of <see cref="ApplicationInstance"/>, load the <see cref="ApplicationConfiguration"/> provided by the factory into the instance. This class exposes a method used to create tuples containing an <see cref="OpcEntityServer"/> instance and an <see cref="ApplicationInstance"/> which can be used to start it.
/// </summary>
/// <param name="entityNodeManagerFactory">A factory creating node managers for the different entities.</param>
/// <param name="configurationFactory">A factory that can create an application the server will use.</param>
public sealed class OpcUaEntityServerFactory(IEntityNodeManagerFactory entityNodeManagerFactory, IApplicationConfigurationFactory configurationFactory)
{
    public OpcEntityServerDriver CreateServer()
    {
        ApplicationInstance _application = new ApplicationInstance
        {
            ApplicationName = configurationFactory.ApplicationName,
            ApplicationType = ApplicationType.Server
        };
        _application.ApplicationConfiguration = configurationFactory.CreateServerConfiguration(_application.ApplicationConfiguration.ApplicationUri);
        _application.LoadApplicationConfiguration(false);
        var server = new OpcEntityServer(entityNodeManagerFactory, _application.ApplicationConfiguration.ApplicationUri);
        return new OpcEntityServerDriver(_application, server);
    }
}