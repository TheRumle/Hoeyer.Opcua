using System;
using System.Threading;
using System.Threading.Tasks;
using Hoeyer.OpcUa.Observation;
using Hoeyer.OpcUa.Proxy;
using Opc.Ua;
using Opc.Ua.Client;

namespace Hoeyer.OpcUa.Client.Application.MachineProxy;

internal sealed class SessionManager : ISessionManager
{
    private readonly SessionFactory _factory;
    public readonly StateChangeBehaviour<ConnectionState> StateChanger = new(ConnectionState.PreInitialized);
    private Session _session;
    private bool isSetupUp = false;
    
    public SessionManager(SessionFactory factory)
    {
        _factory = factory;
    }



    public bool IsConnected => _session.Connected && StateChanger.LastState == ConnectionState.Connected; 

    public async Task Setup()
    {
        try
        { 
            StateChanger.ChangeState(ConnectionState.Initializing);
            await _factory.Configuration.Validate(ApplicationType.Client);
            _session = await _factory.CreateSessionAsync();
            StateChanger.ChangeState(ConnectionState.Running);
            isSetupUp = true;
        }
        catch (Exception)
        {
            StateChanger.ChangeState(ConnectionState.FailedInitializing);
            throw;
        }
    }
    
    public async Task<T> ConnectAndThen<T>(Func<Session, Task<T>> todo, CancellationToken token)
    {
        try
        {
            if (!isSetupUp) await Setup();
            if (!_session.Connected) await _session.ReconnectAsync(token);
            var result = await todo.Invoke(_session);
            return result;
        }
        catch (Exception)
        {
            StateChanger.ChangeState(ConnectionState.FailedConnect);
            throw;
        }

    }

    /// <inheritdoc />
    public void Dispose()
    {
        try
        {
            StateChanger.ChangeState(ConnectionState.Disconnecting);
            _session.Dispose();
            StateChanger.ChangeState(ConnectionState.Disconnected);
        }
        catch (Exception)
        {
            StateChanger.ChangeState(ConnectionState.FailedDisconnect);
            throw;
        }
    }
}