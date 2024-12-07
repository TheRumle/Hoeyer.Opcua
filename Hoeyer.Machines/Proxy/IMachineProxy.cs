using System.Threading.Tasks;

namespace Hoeyer.Machines.Proxy;

public interface IMachineProxy<T>
{
    public Task Disconnect();
    public Task Connect();
    /// <summary>
    /// Connects to and configures the client to the machine.
    /// </summary>
    /// <returns>A tasks which, if completed successfully, connects to a machine and configures itself accordingly.</returns>
    public Task Setup();
    public Task<T> ReadMachineStateAsync();

}
