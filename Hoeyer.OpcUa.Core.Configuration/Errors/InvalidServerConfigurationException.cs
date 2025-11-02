namespace Hoeyer.OpcUa.Core.Configuration.Errors;

public class InvalidServerConfigurationException(string reason) : Exception(reason);