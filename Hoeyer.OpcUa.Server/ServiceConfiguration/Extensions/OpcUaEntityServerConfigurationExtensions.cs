using System;
using Hoeyer.OpcUa.Configuration;
using Opc.Ua;

namespace Hoeyer.OpcUa.Server.ServiceConfiguration.Extensions;

public static class OpcUaEntityServerConfigurationExtensions
{
    public static OpcUaEntityServerConfigurationSetup WithAdditionalServerConfiguration(OpcUaEntityServerConfiguration setup, Action<ServerConfiguration> additionalConfiguration )
    {
        return new OpcUaEntityServerConfigurationSetup(setup, additionalConfiguration);
    }

}