using System;

namespace Hoeyer.OpcUa.Server.Simulation.Api;

public interface ITimeScaler
{
    public TimeSpan ScaleDown(TimeSpan timeSpan);
    public TimeSpan RestoreOriginal(TimeSpan timeSpan);
}