using System;

namespace Hoeyer.OpcUa.Simulation.Abstractions.Services;

public interface ITimeScaler
{
    public TimeSpan Scale(TimeSpan timeSpan);
    public TimeSpan RestoreOriginal(TimeSpan timeSpan);
}