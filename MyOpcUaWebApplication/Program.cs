using Hoeyer.OpcUa.Client.Services;
using Hoeyer.OpcUa.Core.Configuration;
using Hoeyer.OpcUa.Server;
using Hoeyer.OpcUa.Server.ServiceConfiguration;

var builder = WebApplication.CreateBuilder(args);


builder.Logging.AddConsole();

builder.Services.AddOpcUaServerConfiguration(conf => conf
        .WithServerId("MyServer")
        .WithServerName("My Server")
        .WithHttpsHost("localhost", 4840)
        .WithEndpoints(["opc.tcp://localhost:4840"])
        .Build())
    .AddEntityOpcUaServer()
    .WithAutomaticEntityNodeCreation()
    .AddOpcUaClientServices();

var app = builder.Build();

var factory = app.Services.GetService<OpcUaEntityServerFactory>()!;
var server = factory.CreateServer();

await server.StartAsync();

app.UseHttpsRedirection();
await app.RunAsync();