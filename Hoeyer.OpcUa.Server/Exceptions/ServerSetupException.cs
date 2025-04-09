using System;

namespace Hoeyer.OpcUa.Server.Exceptions;

public sealed class UnableToInitializeException(string message) : Exception(message)
{
}