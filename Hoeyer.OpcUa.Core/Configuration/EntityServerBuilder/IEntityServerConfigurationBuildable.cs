﻿namespace Hoeyer.OpcUa.Core.Configuration.EntityServerBuilder;

public interface IEntityServerConfigurationBuildable
{
    IOpcUaEntityServerInfo Build();
}