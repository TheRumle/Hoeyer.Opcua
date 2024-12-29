using Hoeyer.OpcUa.Server.Configuration;
using Hoeyer.OpcUa.Server.Entity;
using Opc.Ua;
using Opc.Ua.Configuration;

namespace Hoeyer.OpcUa.Server.Application;

/// <summary>
/// Given an <see cref="IEntityNodeManagerFactory"/> and <see cref="IApplicationConfigurationFactory"/>, will construct instances of <see cref="ApplicationInstance"/>, load the <see cref="ApplicationConfiguration"/> provided by the factory into the instance. This class exposes a method used to create tuples containing an <see cref="OpcEntityServer"/> instance and an <see cref="ApplicationInstance"/> which can be used to start it.
/// </summary>
/// <param name="entityNodeManagerFactory">A factory creating node managers for the different entities.</param>
/// <param name="configurationFactory">A factory that can create an application the server will use.</param>
public sealed class OpcUaEntityServerFactory(
    IEntityNodeManagerFactory entityNodeManagerFactory, IApplicationConfigurationFactory configurationFactory)
{
    public OpcEntityServerDriver CreateServer()
    {
        var configuration = configurationFactory.CreateServerConfiguration("EntityServer", []);
        var application = new ApplicationInstance
        {
            ApplicationConfiguration = configuration,
            ApplicationName = configurationFactory.ApplicationName,
            ApplicationType = ApplicationType.Server
        };
        application.LoadApplicationConfiguration(false);
        var server = new OpcEntityServer(entityNodeManagerFactory, configuration.ApplicationUri);
        return new OpcEntityServerDriver(application, server);
    }
}