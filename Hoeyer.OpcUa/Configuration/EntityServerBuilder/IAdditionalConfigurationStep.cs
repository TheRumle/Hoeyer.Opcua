using System;
using Opc.Ua;

namespace Hoeyer.OpcUa.Configuration.EntityServerBuilder;

public interface IAdditionalConfigurationStep : IEntityServerConfigurationBuildable
{
    public IEntityServerConfigurationBuildable WithAdditionalConfiguration(Action<ServerConfiguration> additionalConfigurations);
}