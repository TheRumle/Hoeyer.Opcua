using Hoeyer.Machines.Proxy;

namespace Hoeyer.Machines.OpcUa.Proxy;

public abstract class ObservableOpcUaMachineProxy<T>(OpcUaMachineProxy<T> proxy) : ObservableMachineProxy<T>(proxy)
{
    /// <inheritdoc />
    protected override string MachineName { get; } = typeof(T).Name;
}