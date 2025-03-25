using Hoeyer.OpcUa.Core;
using Hoeyer.OpcUa.Core.Configuration;
using Hoeyer.OpcUa.Server;
using Hoeyer.OpcUa.Server.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using TestConfiguration;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureLogging(logging => { logging.AddConsole(); })
    .ConfigureServices((context, services) =>
    {
        var portArg = context.Configuration["Port"] ?? throw new ArgumentException("Port is not defined!");
        if (!int.TryParse(portArg, out var port))
        {
            throw new ArgumentException($"Invalid port number provided: {portArg}");
        }

        services
            .AddTestAddOpcUaServerConfiguration(port)
            .WithEntityServices()
            .WithOpcUaServer();

        services.AddHostedService<OpcUaServerBackgroundService>();
    })
    .Build();

await host.RunAsync();