using System;

namespace Hoeyer.OpcUa.Server;

public class OpcUaEntityServerException(string failedToStartTheServer, Exception exception) : Exception(
    failedToStartTheServer,
    exception);