using Hoeyer.OpcUa.Core.Entity.State;
using Hoeyer.OpcUa.Core.Observation;

namespace MyOpcUaWebApplication;

public sealed class GantryObserver : IStateChangeSubscriber<Gantry>
{
    private readonly ILogger<GantryObserver> _logger;
    private readonly StateChangeSubscription<Gantry> sub;

    public GantryObserver(ILogger<GantryObserver> logger, SubscriptionEngine<Gantry> subscriptionEngine)
    {
        _logger = logger;
        sub = subscriptionEngine.SubscribeToMachine(this);
    }

    /// <inheritdoc />
    public void OnStateChange(StateChange<Gantry> stateChange)
    {
        _logger.LogInformation(
            "State of Gantry changed at {EnteredOn}. It was {PreviousState} and it is now {ReachedState}",
            stateChange.EnteredStateOn, stateChange.PreviousState, stateChange.ReachedState);
    }
}