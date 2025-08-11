using System;

namespace Hoeyer.OpcUa.Simulation.Api.Services;

public sealed class Identity : ITimeScaler
{
    public TimeSpan Scale(TimeSpan timeSpan) => timeSpan;
    public TimeSpan RestoreOriginal(TimeSpan timeSpan) => timeSpan;
}