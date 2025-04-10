using System;
using System.Threading;
using System.Threading.Tasks;
using Opc.Ua.Client;

namespace Hoeyer.OpcUa.Client.MachineProxy;

public interface ISessionManager : IDisposable
{
    bool IsConnected { get; }
    Task Setup();
    Task<T> ConnectAndThen<T>(Func<ISession, Task<T>> todo, CancellationToken token);
}