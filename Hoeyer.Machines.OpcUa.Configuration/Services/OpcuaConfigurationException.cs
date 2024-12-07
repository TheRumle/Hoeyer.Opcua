using System;

namespace Hoeyer.Machines.OpcUa.Configuration.Services;

public class OpcuaConfigurationException(string message) : Exception(message)
{
}