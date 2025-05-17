namespace Hoeyer.OpcUa.Client.Api.Calling.Exception;

public sealed class NoSuchEntityMethodException(string entity, string methodName)
    : System.Exception($"The entity {entity} does not have CallMethod method called {methodName}");