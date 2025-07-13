using System;

namespace Hoeyer.OpcUa.Simulation.Api.Services;

public sealed class IdentityTimeScaler : ITimeScaler
{
    public TimeSpan Scale(TimeSpan timeSpan) => timeSpan;
    public TimeSpan RestoreOriginal(TimeSpan timeSpan) => timeSpan;
}