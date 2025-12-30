using System;
using Opc.Ua;

namespace Hoeyer.OpcUa.Core;

[AttributeUsage(AttributeTargets.Property)]
public abstract class LimitAlarmAttribute<T> : OpcAlarmAttribute
    where T : IComparable<T>
{
    protected LimitAlarmAttribute(T lowLow,
        T low,
        T high,
        T highHigh,
        string browseName,
        AlarmSeverity severity) : base(AlarmType.Limit, browseName, severity)
    {
        LowLow = lowLow;
        Low = low;
        High = high;
        HighHigh = highHigh;
        if (LowLow.CompareTo(low) > 0)
        {
            throw new ArgumentOutOfRangeException(nameof(lowLow) + " was greater that " + nameof(low));
        }

        if (High.CompareTo(HighHigh) > 0)
        {
            throw new ArgumentOutOfRangeException(nameof(lowLow) + " was greater that " + nameof(low));
        }
    }

    public T LowLow { get; }
    public T Low { get; }
    public T High { get; }
    public T HighHigh { get; }

    public Func<T, LimitAlarmStates> ToMap() => alarm =>
    {
        if (alarm.CompareTo(Low) <= 0)
        {
            return alarm.CompareTo(LowLow) <= 0
                ? LimitAlarmStates.LowLow
                : LimitAlarmStates.Low;
        }

        if (alarm.CompareTo(High) >= 0)
        {
            return alarm.CompareTo(HighHigh) >= 0
                ? LimitAlarmStates.HighHigh
                : LimitAlarmStates.High;
        }

        return LimitAlarmStates.Inactive;
    };
}