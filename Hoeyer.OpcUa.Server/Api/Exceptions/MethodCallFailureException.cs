using System;

namespace Hoeyer.OpcUa.Server.Api.Exceptions;

public class MethodCallFailureException(Exception inner) : Exception(inner.Message)
{
}