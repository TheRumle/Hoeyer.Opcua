namespace Hoeyer.OpcUa.Core.Configuration.Errors;

public class InvalidUaConfigurationException(string reason) : Exception(reason);