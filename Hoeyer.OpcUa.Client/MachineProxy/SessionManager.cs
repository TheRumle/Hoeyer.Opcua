using System;
using System.Threading;
using System.Threading.Tasks;
using Hoeyer.OpcUa.Core.Application.Observation;
using Hoeyer.OpcUa.Core.Proxy;
using Opc.Ua;
using Opc.Ua.Client;

namespace Hoeyer.OpcUa.Client.MachineProxy;

internal sealed class SessionManager : ISessionManager
{
    private readonly IEntitySessionFactory _factory;
    public readonly StateChangeSubscriptionManager<ConnectionState> StateChanger = new(ConnectionState.PreInitialized);
    private ISession _session;
    private bool isSetupUp;

    public SessionManager(IEntitySessionFactory factory)
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
            _session = await _factory.CreateSessionAsync("sure thing sugar");
            StateChanger.ChangeState(ConnectionState.Running);
            isSetupUp = true;
        }
        catch (Exception)
        {
            StateChanger.ChangeState(ConnectionState.FailedInitializing);
            throw;
        }
    }

    public async Task<T> ConnectAndThen<T>(Func<ISession, Task<T>> todo, CancellationToken token)
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