using System;
using System.Collections.Generic;

namespace Hoeyer.OpcUa.Core.Configuration;

public interface IOpcUaEntityServerInfo
{
    string ServerId { get; }
    string ApplicationName { get; }
    Uri Host { get; }
    ISet<Uri> Endpoints { get; }

    /// <summary>
    ///     For instance, http://samples.org/UA/MyApplication or something else uniqely identifying the overall resource,
    /// </summary>
    Uri ApplicationNamespace { get; }

    Uri OpcUri { get; }
}