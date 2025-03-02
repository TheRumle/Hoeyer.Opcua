using System;
using Opc.Ua;

namespace Hoeyer.OpcUa.Core.Configuration.EntityServerBuilder;

public interface IAdditionalConfigurationStep : IEntityServerConfigurationBuildable
{
    public IEntityServerConfigurationBuildable WithAdditionalConfiguration(
        Action<ServerConfiguration> additionalConfigurations);
}