namespace Hoeyer.Machines.Observation;

public class MachineSubscriptionFactory<T>(Machine<T> machine)
{
    public void SubscribeTo(Machine<T> machine)
    {
        machine.Subscribe();
    }
    
}