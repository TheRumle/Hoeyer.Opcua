namespace Hoeyer.OpcUa.Server.Abstractions.Exceptions;

public class MethodCallFailureException(Exception inner) : Exception(inner.Message)
{
}