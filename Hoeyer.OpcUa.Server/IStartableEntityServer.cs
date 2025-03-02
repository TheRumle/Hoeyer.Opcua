﻿using System.Threading.Tasks;
using Hoeyer.OpcUa.Core.Configuration;

namespace Hoeyer.OpcUa.Server;

public interface IStartableEntityServer
{
    IOpcUaEntityServerInfo ServerInfo { get; }
    Task<IStartedEntityServer> StartAsync();
}