using System;
using System.Threading;
using System.Threading.Tasks;
using Opc.Ua.Client;

namespace Hoeyer.OpcUa.Client.Application.MachineProxy;

public interface ISessionManager : IDisposable
{
    bool IsConnected { get; }
    Task Setup();
    Task<T> ConnectAndThen<T>(Func<Session, Task<T>> todo, CancellationToken token);
}