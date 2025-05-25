using System;
using Opc.Ua;

namespace Hoeyer.OpcUa.Core.Application.Translator.Loaders;

public class DataValueLoader<T> : IDataValueLoader<T>
{
    /// <inheritdoc />
    public virtual DataValue Parse(T value) => WithValue(value);

    protected DataValue WithValue<TValue>(TValue value) =>
        new()
        {
            StatusCode = StatusCodes.Good,
            Value = value,
            ServerTimestamp = DateTime.Now
        };
}