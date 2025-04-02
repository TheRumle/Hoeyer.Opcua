using Hoeyer.OpcUa.Client.Application;
using Hoeyer.OpcUa.Client.Application.MachineProxy;
using Hoeyer.OpcUa.Core;
using Hoeyer.OpcUa.Server;
using MyOpcUaWebApplication;

var builder = WebApplication.CreateBuilder(args);


builder.Logging.AddConsole();
builder.Services.AddHostedService<ReaderHost>();

builder.Services.AddOpcUaServerConfiguration(conf => conf
        .WithServerId("MyServer")
        .WithServerName("My Server")
        .WithHttpsHost("localhost", 4840)
        .WithEndpoints(["opc.tcp://localhost:4840"])
        .Build())
    .WithEntityServices()
    .WithOpcUaServerAsBackgroundService()
    .Collection.AddTransient<SessionFactory>()
    .AddTransient<OpcEntityClient<Gantry>>();



var app = builder.Build();
var factory = app.Services.GetService<OpcEntityClient<Gantry>>()!;


app.UseHttpsRedirection();
await app.RunAsync();