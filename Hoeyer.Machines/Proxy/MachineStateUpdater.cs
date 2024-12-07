using System.Threading.Tasks;
using Hoeyer.Machines.Machine;

namespace Hoeyer.Machines.Proxy;

public sealed class MachineStateUpdater<T>(IMachineProxy<T> machineProxy, Machine<T> machine)
{
    public async Task ReadAndUpdateMachineState()
    {
        var state = await machineProxy.ReadMachineStateAsync();
        machine.ChangeState(state);
    }
}