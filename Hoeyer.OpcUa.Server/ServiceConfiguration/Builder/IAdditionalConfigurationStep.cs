using System;
using Opc.Ua;

namespace Hoeyer.OpcUa.Server.ServiceConfiguration.Builder;

public interface IAdditionalConfigurationStep : IEntityServerConfigurationBuildable
{
    public IEntityServerConfigurationBuildable WithAdditionalConfiguration(Action<ServerConfiguration> additionalConfigurations);
}