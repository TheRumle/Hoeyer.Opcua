using System.Threading;
using System.Threading.Tasks;
using FluentResults;
using Hoeyer.Common.Extensions.Functional;
using Hoeyer.Machines.Observation;
using Hoeyer.Machines.OpcUa.Domain;
using Hoeyer.Machines.Proxy;
using Hoeyer.Machines.StateSnapshot;
using Microsoft.Extensions.Logging;

namespace Hoeyer.Machines.OpcUa.Application.MachineProxy;

    public sealed class OpcUaRemoteMachineProxy<TMachineState>(
        IOpcUaNodeConnectionHolder<TMachineState> opcUaNodeConnectionHolder,
        ISessionManager sessionManager,
        Machine<TMachineState> machine,
        ILogger<OpcUaRemoteMachineProxy<TMachineState>> logger)
    : IRemoteMachineObserver<TMachineState>
{
    private readonly StateChangeBehaviour<ConnectionState> _stateChanger = new(ConnectionState.PreInitialized);
    
    /// <inheritdoc />
    public async Task<Result<TMachineState>> ReadEntityAsync(CancellationToken token)
    {
        
        var a = await sessionManager.ConnectAndThen(opcUaNodeConnectionHolder.ReadOpcUaEntityAsync, token);
        return a.Tap(machine.ChangeState,
            errors => logger.LogError(
                "Invalid data OpcUa server. Could not assign the information fetched to entity of type {TYPE}. The error was {ERROR}",
                typeof(TMachineState).Name, string.Join(",", errors)));
    }

    /// <inheritdoc />
    public StateChangeSubscription<ConnectionState> Subscribe(IStateChangeSubscriber<ConnectionState> subscriber)
    {
        return this._stateChanger.Subscribe(subscriber);
    }


    /// <inheritdoc />
    public override string ToString()
    {
        return $"{typeof(TMachineState).Name} connection status: Moved from {_stateChanger.LastState} to {_stateChanger.CurrentState} on {_stateChanger.EnteredStateOn}";
    }
    
}
