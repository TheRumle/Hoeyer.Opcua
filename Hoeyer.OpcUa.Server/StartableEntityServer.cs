using System;
using System.Threading.Tasks;
using Hoeyer.OpcUa.Core.Configuration;
using Hoeyer.OpcUa.Server.Api;
using Opc.Ua;
using Opc.Ua.Configuration;

namespace Hoeyer.OpcUa.Server;

internal sealed class StartableAgentServer(
    ApplicationInstance applicationInstance,
    OpcAgentServer agentServer,
    AgentServerStartedMarker marker)
    : IStartableAgentServer, IStartedAgentServer
{
    private readonly ApplicationInstance _applicationInstance =
        applicationInstance ?? throw new ArgumentNullException(nameof(applicationInstance));

    private readonly OpcAgentServer _agentServer =
        agentServer ?? throw new ArgumentNullException(nameof(agentServer));

    private bool _disposed;

    public async Task<IStartedAgentServer> StartAsync()
    {
        if (_disposed)
        {
            throw new ObjectDisposedException(nameof(StartableAgentServer));
        }

        if (marker.IsServerStarted) return this;
        try
        {
            await _applicationInstance.Start(_agentServer);
            marker.MarkCompleted();
        }
        catch (ServiceResultException e)
        {
            Console.WriteLine(e);
            throw;
        }

        return this;
    }

    /// <inheritdoc />
    public IOpcUaAgentServerInfo ServerInfo => _agentServer.ServerInfo;

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    private void Dispose(bool disposing)
    {
        if (_disposed)
        {
            return;
        }

        if (disposing)
        {
            _agentServer.Stop();
            _applicationInstance.Stop();
            _agentServer.Dispose();
        }

        _disposed = true;
    }

    ~StartableAgentServer()
    {
        Dispose(false);
    }
}