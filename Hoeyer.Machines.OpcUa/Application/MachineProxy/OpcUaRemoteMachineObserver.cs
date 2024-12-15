using System;
using System.Threading;
using System.Threading.Tasks;
using Hoeyer.Common.Extensions.Functional;
using Hoeyer.Machines.Observation;
using Hoeyer.Machines.Proxy;
using Microsoft.Extensions.Logging;

namespace Hoeyer.Machines.OpcUa.Proxy;

public sealed class OpcUaRemoteMachineObserver<TMachineState>(
        IOpcUaNodeStateReader<TMachineState> opcUaNodeStateReader,
        ISessionManager sessionManager,
        Machine<TMachineState> machine,
        ILogger<OpcUaRemoteMachineObserver<TMachineState>> logger)
    : IRemoteMachineObserver<TMachineState>
{
    private readonly StateChangeBehaviour<ConnectionState> _stateChanger = new(ConnectionState.PreInitialized);
    /// <inheritdoc />
    public async Task<TMachineState> ReadEntityAsync(CancellationToken token)
    {
        
        var a = await sessionManager.ConnectAndThen(opcUaNodeStateReader.ReadOpcUaEntityAsync, token);
        a.Tap(machine.ChangeState,
            e => logger.LogError(
                "Invalid data OpcUa server. Could not assign the information fetcetd to entity of type {TYPE}",
                typeof(TMachineState).Name));
        
        
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public override string ToString()
    {
        return $"{typeof(TMachineState).Name} connection status: Moved from {_stateChanger.LastState} to {_stateChanger.CurrentState} on {_stateChanger.EnteredStateOn}";
    }
    
}
