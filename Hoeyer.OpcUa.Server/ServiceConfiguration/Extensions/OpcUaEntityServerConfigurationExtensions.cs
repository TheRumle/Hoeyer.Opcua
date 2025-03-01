using System;
using Hoeyer.OpcUa.Core.Configuration;
using Opc.Ua;

namespace Hoeyer.OpcUa.Server.ServiceConfiguration.Extensions;

public static class OpcUaEntityServerConfigurationExtensions
{
    public static OpcUaEntityServerSetup WithAdditionalServerConfiguration(IOpcUaEntityServerInfo setup,
        Action<ServerConfiguration> additionalConfiguration)
    {
        return new OpcUaEntityServerSetup(setup, additionalConfiguration);
    }
}