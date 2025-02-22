using System;
using Hoeyer.OpcUa.Configuration;
using Opc.Ua;

namespace Hoeyer.OpcUa.Server.ServiceConfiguration.Extensions;

public static class OpcUaEntityServerConfigurationExtensions
{
    public static OpcUaEntityServerSetup WithAdditionalServerConfiguration(IOpcUaEntityServerConfiguration setup, Action<ServerConfiguration> additionalConfiguration )
    {
        return new OpcUaEntityServerSetup(setup, additionalConfiguration);
    }

}