using System;
using System.Threading;
using System.Threading.Tasks;
using Hoeyer.OpcUa.Client.Application;
using Hoeyer.OpcUa.Core.Application.Observation;
using Hoeyer.OpcUa.Core.Observation;
using Hoeyer.OpcUa.Core.Proxy;
using Microsoft.Extensions.Logging;

namespace Hoeyer.OpcUa.Client.MachineProxy;

public sealed class EntityProxy<TMachineState>(
    IEntityClient<TMachineState> entityClient,
    ISessionManager sessionManager,
    StateContainer<TMachineState> stateContainer,
    ILogger<EntityProxy<TMachineState>> logger)
    : IEntityObserver<TMachineState>
{
    private readonly StateChangeSubscriptionManager<ConnectionState> _stateChanger = new(ConnectionState.PreInitialized);

    /// <inheritdoc />
    public async Task<TMachineState> ReadEntityAsync(CancellationToken token)
    {
        var a = await sessionManager.ConnectAndThen(entityClient.ReadOpcUaEntityAsync, token);
        
        throw new NotImplementedException();
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