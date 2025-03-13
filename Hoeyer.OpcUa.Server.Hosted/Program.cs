using Microsoft.Extensions.DependencyInjection;
using Hoeyer.OpcUa.Server;
using Hoeyer.OpcUa.Server.ServiceConfiguration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using TestConfiguration;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureLogging(logging =>
    {
        logging.AddConsole();
    })
    .ConfigureServices((context, services) =>
    {
        string portArg = context.Configuration["Port"] ?? throw new ArgumentException("Port is not defined!"); 
        if (!int.TryParse(portArg, out int port))
        {
            throw new ArgumentException($"Invalid port number provided: {portArg}");
        }

        services
            .AddTestAddOpcUaServerConfiguration(port)
            .AddEntityOpcUaServer()
            .WithAutomaticEntityNodeCreation();

        services.AddHostedService<OpcUaServerBackgroundService>();
    })
    .Build();

await host.RunAsync();
