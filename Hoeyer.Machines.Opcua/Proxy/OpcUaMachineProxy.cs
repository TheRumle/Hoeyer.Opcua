using Hoeyer.Machines.Proxy;
using Opc.Ua;
using Opc.Ua.Client;

namespace Hoeyer.Machines.OpcUa.Proxy;

public sealed class OpcUaMachineProxy<T> (IOpcUaNodeStateReader<T> opcUaNodeStateReader, SessionFactory factory) : IMachineProxy<T>
{
    
    private Session? _session = null!;
    public async Task Connect()
    {
        if (!SessionActive)
            _session = await factory.CreateActiveSession();
    }

    /// <inheritdoc />
    public async Task<T> ReadMachineStateAsync()
    {
        await Connect();
        return await opcUaNodeStateReader.ReadOpcUaNodeAsync(_session!);
    }

    public async Task Disconnect()
    {
        if (!SessionActive) 
            return;
        
        await _session!.CloseAsync();
        _session.Dispose();
    }

    public async Task Setup()
    {
        if (!SessionActive)
            _session = await factory.CreateActiveSession();
        await factory._config.Validate(ApplicationType.Client);
    }

    private bool SessionActive => _session != null && _session.Connected;
    
}
