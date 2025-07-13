using System;

namespace Hoeyer.OpcUa.Simulation.Api.Services;

public sealed class TimeScaler : ITimeScaler
{
    private readonly double _scale;

    public TimeScaler(double scale)
    {
        if (scale <= 0) throw new ArgumentOutOfRangeException(nameof(scale) + " must be greater than zero");
        _scale = scale;
    }

    public TimeSpan Scale(TimeSpan timeSpan) => timeSpan * _scale;

    public TimeSpan RestoreOriginal(TimeSpan timeSpan) => TimeSpan.FromTicks((long)(timeSpan.Ticks / _scale));
}