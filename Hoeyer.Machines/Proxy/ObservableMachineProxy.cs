using System;
using System.Threading.Tasks;
using Hoeyer.Machines.Machine;

namespace Hoeyer.Machines.Proxy;

public abstract class ObservableMachineProxy<T>(IMachineProxy<T> proxy) : IMachineProxy<T>
{
    private bool _isConnected;
    private readonly StateChangeBehaviour<ConnectionState> _stateChanger = new(ConnectionState.PreInitialized);
    public ConnectionState ConnectionStatus => _stateChanger.CurrentState;
    protected abstract String MachineName { get; }

    public async Task Setup()
    {
        try
        {
            if (!_isConnected) await Connect();
            _stateChanger.ChangeState(ConnectionState.Initializing);
            await proxy.Setup();
            _stateChanger.ChangeState(ConnectionState.Running);
            await Disconnect();
        }
        catch (MachineClientConnectionException e)
        {
            _stateChanger.ChangeState(ConnectionState.FailedInitializing);
            throw;
        }
    }

    /// <inheritdoc />
    public Task<T> ReadMachineStateAsync() => proxy.ReadMachineStateAsync();

    public async Task Connect()
    {
        try
        {
            _stateChanger.ChangeState(ConnectionState.Connecting);
            await proxy.Connect();
            _stateChanger.ChangeState(ConnectionState.Connected);
            _isConnected = true;
        }
        catch (MachineClientConnectionException e)
        {
            _stateChanger.ChangeState(ConnectionState.FailedConnect);
            _isConnected = false;
            throw;
        }
    }



    public async Task Disconnect()
    {
        try
        {
            _stateChanger.ChangeState(ConnectionState.Disconnecting);
            await proxy.Disconnect();
            _stateChanger.ChangeState(ConnectionState.Disconnected);
            _isConnected = false;
        }
        catch (MachineClientConnectionException _)
        {
            _stateChanger.ChangeState(ConnectionState.FailedDisconnect);
            _isConnected = true;
            throw;
        }
    }
    
    /// <inheritdoc />
    public override string ToString()
    {
        return $"{MachineName} connection status: Moved from {_stateChanger.LastState} to {_stateChanger.CurrentState} on {_stateChanger.EnteredStateOn}";
    }
}