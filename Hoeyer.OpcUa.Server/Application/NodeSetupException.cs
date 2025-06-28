using System;

namespace Hoeyer.OpcUa.Server.Application;

public sealed class NodeSetupException(string err) : Exception(err)
{
}