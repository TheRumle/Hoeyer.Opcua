using System;

namespace Hoeyer.Machines.Proxy;

public sealed class MachineClientConnectionException : Exception {
    public MachineClientConnectionException(Exception e, String msg) :base(msg, e)
    {
    }

    public MachineClientConnectionException(string msg) :base(msg) {
    }
}