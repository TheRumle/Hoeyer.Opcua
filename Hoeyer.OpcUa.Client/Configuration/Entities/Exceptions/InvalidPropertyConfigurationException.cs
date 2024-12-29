namespace Hoeyer.OpcUa.Client.Configuration.Entities.Exceptions;

public class InvalidPropertyConfigurationException() : OpcuaConfigurationException(
    """
    Invalid Property Selection. The selection of a property must be of the form "a => a.property"
    """);