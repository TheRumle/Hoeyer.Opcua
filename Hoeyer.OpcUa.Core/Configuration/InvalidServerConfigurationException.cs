using System;

namespace Hoeyer.OpcUa.Core.Configuration;

public class InvalidServerConfigurationException(string reason) : Exception(reason);