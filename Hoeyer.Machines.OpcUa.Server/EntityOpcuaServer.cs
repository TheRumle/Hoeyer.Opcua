using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Opc.Ua;
using Opc.Ua.Server;

namespace Hoeyer.Machines.OpcUa.Server;

public sealed class EntityOpcuaServer : IDisposable
{
    public StandardServer Server;
    private readonly OpcUaLocalHostServerFactory _opcUaLocalHostServerFactory;
    private readonly EndpointFactory _endpointFactory;

    public EntityOpcuaServer(OpcUaServerOptions options, string applicationName)
    {
        _opcUaLocalHostServerFactory = new OpcUaLocalHostServerFactory(options.ServerName,
            options.Port.ToString(), applicationName);
        this._endpointFactory = new EndpointFactory(_opcUaLocalHostServerFactory.BaseAddress);
    }



    public Uri Start()
    {
        (Server, var configuration) = _opcUaLocalHostServerFactory.CreateServer();
        var endpoint = _endpointFactory.CreateEndpoint(configuration);
        try
        {

            Server.Start(configuration);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
        return endpoint.EndpointUrl;
    }

    public void Stop() => Server.Stop();

    /// <inheritdoc />
    public void Dispose()
    {
        Server.Stop();
        Server.Dispose();
    }
}


public record OpcUaApplicationOptions
{
    public string ApplicationName { get; set; }
}

public class MyOPCUAServer : StandardServer
{
    private readonly ApplicationConfiguration _configuration;
    private bool _started;


    public MyOPCUAServer(ApplicationConfiguration configuration)
    {
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
    }

    protected override MasterNodeManager CreateMasterNodeManager(IServerInternal server, ApplicationConfiguration configuration)
    {
        List<INodeManager> nodeManagers = new List<INodeManager>();


        return new MasterNodeManager(server, configuration, "myUri");
    }   


    /// <summary>
    /// Starts the server.
    /// </summary>
    public async Task StartAsync()
    {
        if (_started) return;

        try
        {
            // Start the server.
            Start(_configuration);
            _started = true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error starting OPC UA Server: {ex.Message}");
        }
    }

    public void Stop()
    {
        if (!_started) return;

        try
        {
            base.Stop();
            _started = false;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error stopping OPC UA Server: {ex.Message}");
        }
    }
}