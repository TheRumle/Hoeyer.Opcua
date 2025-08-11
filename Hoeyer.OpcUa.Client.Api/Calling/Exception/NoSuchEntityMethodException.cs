namespace Hoeyer.OpcUa.Client.Api.Calling.Exception;

public sealed class NoSuchAgentMethodException(string agent, string methodName)
    : System.Exception($"The agent {agent} does not have CallMethod method called {methodName}");