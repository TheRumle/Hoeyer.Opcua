using System;

namespace Hoeyer.OpcUa.Server.Exceptions;

public sealed class ServerSetupException(string message) : Exception(message)
{
}