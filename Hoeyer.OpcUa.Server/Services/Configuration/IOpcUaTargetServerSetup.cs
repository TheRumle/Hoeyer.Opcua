using System;
using System.Collections.Generic;
using Opc.Ua;

namespace Hoeyer.OpcUa.Server.Services.Configuration;

public interface IOpcUaTargetServerSetup
{
    public Action<ServerConfiguration> AdditionalConfiguration { get; }

    public ISet<Uri> Endpoints { get; }

    public Uri OpcUri { get; }
    public string ServerId { get; }
    public string ApplicationName { get; }
    public Uri Host { get; }
    public Uri ApplicationNamespace { get; }
}