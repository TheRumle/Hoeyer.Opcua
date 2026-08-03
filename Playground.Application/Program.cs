using Playground.Application;
using Playground.Clients;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddHostedService<PositionChangeReactor>();
builder.Services.AddHostedService<RandomContainerAssignmentReactor>();
builder.AddDefaultSimulationApplication(useEnvironmentVariables: false,
    configureOpcUaDefaults: (appConfig) => { appConfig.SecurityConfiguration.AutoAcceptUntrustedCertificates = true; },
    serverConfiguration: (provider, config) =>
    {
        var serverConfiguration = config.ServerConfiguration;
        serverConfiguration.MaxSessionCount = 1000;
        serverConfiguration.MaxSubscriptionCount = 1000;
        serverConfiguration.MaxBrowseContinuationPoints = 100;
        serverConfiguration.MaxQueryContinuationPoints = 1000;
        serverConfiguration.MaxHistoryContinuationPoints = 1000;
    });

var app = builder.Build();
app.MapHealthChecks("/health/server");
await app.RunAsync();