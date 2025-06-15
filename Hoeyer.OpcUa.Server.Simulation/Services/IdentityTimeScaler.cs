using System;
using Hoeyer.OpcUa.Server.Simulation.Api;

namespace Hoeyer.OpcUa.Server.Simulation.Services;

public sealed class IdentityTimeScaler : ITimeScaler
{
    public TimeSpan ScaleDown(TimeSpan timeSpan) => timeSpan;
    public TimeSpan RestoreOriginal(TimeSpan timeSpan) => timeSpan;
}