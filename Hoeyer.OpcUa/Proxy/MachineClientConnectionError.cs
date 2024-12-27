using System;
using FluentResults;

namespace Hoeyer.OpcUa.Proxy;

public sealed class MachineClientConnectionError : Error {
    public MachineClientConnectionError(Exception e, String msg) :base(msg + "\n" + e.Message)
    {
    }

    public MachineClientConnectionError(string msg) :base(msg) {
    }

    public Exception ToException()
    {
        return new Exception(ToString());
    }
}