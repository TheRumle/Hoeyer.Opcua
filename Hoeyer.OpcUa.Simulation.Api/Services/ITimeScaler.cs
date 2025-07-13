using System;

namespace Hoeyer.OpcUa.Simulation.Api.Services;

public interface ITimeScaler
{
    public TimeSpan Scale(TimeSpan timeSpan);
    public TimeSpan RestoreOriginal(TimeSpan timeSpan);
}