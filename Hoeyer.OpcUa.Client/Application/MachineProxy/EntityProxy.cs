using System.Threading;
using System.Threading.Tasks;
using FluentResults;
using Hoeyer.OpcUa.Core.Observation;
using Hoeyer.OpcUa.Core.Proxy;
using Microsoft.Extensions.Logging;

namespace Hoeyer.OpcUa.Client.Application.MachineProxy;

public sealed class EntityProxy<TMachineState>(
    IEntityClient<TMachineState> entityClient,
    ISessionManager sessionManager,
    StateContainer<TMachineState> stateContainer,
    ILogger<EntityProxy<TMachineState>> logger)
    : IEntityObserver<TMachineState>
{
    private readonly StateChangeBehaviour<ConnectionState> _stateChanger = new(ConnectionState.PreInitialized);

    /// <inheritdoc />
    public async Task<Result<TMachineState>> ReadEntityAsync(CancellationToken token)
    {
        var a = await sessionManager.ConnectAndThen(entityClient.ReadOpcUaEntityAsync, token);
        
        return null!;
    }

    /// <inheritdoc />
    public StateChangeSubscription<ConnectionState> Subscribe(IStateChangeSubscriber<ConnectionState> subscriber)
    {
        return _stateChanger.Subscribe(subscriber);
    }


    /// <inheritdoc />
    public override string ToString()
    {
        return
            $"{typeof(TMachineState).Name} connection status: Moved from {_stateChanger.LastState} to {_stateChanger.CurrentState} on {_stateChanger.EnteredStateOn}";
    }
}