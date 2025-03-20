using System;
using Opc.Ua;

namespace Hoeyer.OpcUa.Core.Entity.State.Loaders;

public class DataValueLoader<T> : IDataValueLoader<T>
{
    protected DataValue WithValue<TValue>(TValue value)
    {
        return new DataValue()
        {
            StatusCode = StatusCodes.Good,
            Value = value,
            ServerTimestamp = DateTime.Now,
        };
    }

    /// <inheritdoc />
    public virtual DataValue Parse(T value)
    {
        return WithValue(value);
    }
}