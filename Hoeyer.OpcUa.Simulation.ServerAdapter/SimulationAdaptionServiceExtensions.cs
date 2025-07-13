using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using Hoeyer.Common.Extensions.Types;
using Hoeyer.Common.Reflection;
using Hoeyer.OpcUa.Core.Api;
using Hoeyer.OpcUa.Server.Api.NodeManagement;
using Hoeyer.OpcUa.Simulation.Api;
using Hoeyer.OpcUa.Simulation.Api.Configuration;
using Hoeyer.OpcUa.Simulation.Api.Execution;
using Hoeyer.OpcUa.Simulation.Api.PostProcessing;
using Hoeyer.OpcUa.Simulation.ServerAdapter.Api;
using Hoeyer.OpcUa.Simulation.ServerAdapter.Application;
using Hoeyer.OpcUa.Simulation.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Hoeyer.OpcUa.Simulation.ServerAdapter;

public static class SimulationAdaptionServiceExtensions
{
    [SuppressMessage("Design", "S3011",
        Justification =
            "This makes type safety easier to manage as compiletime information about generic arguments is present.")]
    private static void ExecuteLocalGenericRegistration(string methodName, Type[] generics, params object[] arguments)
    {
        var info = typeof(SimulationAdaptionServiceExtensions)
            .GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Public)!;
        info.MakeGenericMethod(generics).Invoke(null, arguments);
    }

    private static void RegisterPerEntityServices<TEntity>(SimulationServicesContainer simulationServicesContainer)
    {
        simulationServicesContainer
            .AddSingleton<
                EntityStateChangedNotifier<TEntity>,
                EntityStateChangedNotifier<TEntity>
            >();

        simulationServicesContainer.AddSingleton<IStateChangeSimulationProcessor<TEntity>>(p =>
            p.GetRequiredService<EntityStateChangedNotifier<TEntity>>());
        simulationServicesContainer.AddSingleton<INodeConfigurator<TEntity>>(p =>
            p.GetRequiredService<EntityStateChangedNotifier<TEntity>>());
    }

    private static void AddActionAdapterServices<TEntity, TMethodArgs>(
        SimulationServicesContainer simulationServicesContainer)
    {
        simulationServicesContainer
            .AddSingleton<INodeConfigurator<TEntity>, ActionSimulationAdapter<TEntity, TMethodArgs>>(coreServices =>
            {
                var logger = coreServices.GetRequiredService<ILogger<ActionSimulationAdapter<TEntity, TMethodArgs>>>();
                var entityTranslator = coreServices.GetRequiredService<IEntityTranslator<TEntity>>();

                var orchestrator = coreServices.GetRequiredService<ISimulationOrchestrator<TEntity, TMethodArgs>>();
                var argsParser = coreServices.GetRequiredService<IMethodArgumentParser<TMethodArgs>>();
                var simulator = coreServices.GetRequiredService<ISimulation<TEntity, TMethodArgs>>();
                var errorHandler = coreServices.GetRequiredService<ISimulationExecutorErrorHandler>();
                var validator = coreServices.GetRequiredService<IOpcMethodArgumentsAttributeUsageValidator>();

                return new ActionSimulationAdapter<TEntity, TMethodArgs>(
                    logger: logger,
                    translator: entityTranslator,
                    argsParser: argsParser,
                    simulator: simulator,
                    orchestrator: orchestrator,
                    errorHandler: errorHandler,
                    argsTypeAnnotationValidator: validator);
            });
    }

    private static void AddFunctionAdapterServices<TEntity, TMethodArgs, TReturnType>(
        SimulationServicesContainer simulationServicesContainer)
    {
        simulationServicesContainer
            .AddSingleton<INodeConfigurator<TEntity>,
                FunctionSimulationAdapter<TEntity, TMethodArgs, TReturnType>>(coreServices =>
            {
                var logger = coreServices
                    .GetRequiredService<ILogger<FunctionSimulationAdapter<TEntity, TMethodArgs, TReturnType>>>();
                var entityTranslator = coreServices.GetRequiredService<IEntityTranslator<TEntity>>();

                var orchestrator = coreServices
                    .GetRequiredService<ISimulationOrchestrator<TEntity, TMethodArgs, TReturnType>>();
                var argsParser = coreServices.GetRequiredService<IMethodArgumentParser<TMethodArgs>>();
                var simulator = coreServices.GetRequiredService<ISimulation<TEntity, TMethodArgs, TReturnType>>();
                var errorHandler = coreServices.GetRequiredService<ISimulationExecutorErrorHandler>();
                var validator = coreServices.GetRequiredService<IOpcMethodArgumentsAttributeUsageValidator>();

                return new FunctionSimulationAdapter<TEntity, TMethodArgs, TReturnType>(
                    logger: logger,
                    translator: entityTranslator,
                    argsParser: argsParser,
                    simulator: simulator,
                    orchestrator: orchestrator,
                    errorHandler: errorHandler,
                    argsTypeAnnotationValidator: validator);
            });
    }

    public static SimulationAdapterConfig AdaptToServerRuntime(this SimulationAdapterConfig config)
    {
        var simulationServiceContainer = config.SimulationServicesContainer;
        var serviceCollection = config.Services;

        var argTranslators = typeof(IEntityMethodArgTranslator<>)
            .GetTypesFromConsumingAssemblies()
            .Select(e => (Implementor: e,
                translatorInterface: e.GetImplementedVersionOfGeneric(typeof(IEntityMethodArgTranslator<>))))
            .Where(e => e.translatorInterface is not null)
            .Select(e => (ArgTranslatorImplementor: e.Implementor, ArgTranslatorInterface: e.translatorInterface,
                ArgContainer: e.translatorInterface!.GenericTypeArguments[0]))
            .AsParallel();

        simulationServiceContainer.AddScoped<ISimulationExecutorErrorHandler, SimulationExecutorErrorHandler>();
        foreach (var (argTranslatorImplementor, argTranslatorInterface, argContainer) in argTranslators)
        {
            simulationServiceContainer.AddScoped(argTranslatorInterface!, argTranslatorImplementor);
            simulationServiceContainer.AddScoped(argTranslatorImplementor, argTranslatorImplementor);
            simulationServiceContainer.AddScoped(typeof(IMethodArgumentParser<>).MakeGenericType(argContainer),
                typeof(MethodArgumentParser<>).MakeGenericType(argContainer));
        }

        foreach (var (entityType, methodArgs) in config.ActionSimulationPatterns)
        {
            ExecuteLocalGenericRegistration(nameof(AddActionAdapterServices), [entityType, methodArgs],
                simulationServiceContainer);
        }

        foreach (var (entityType, methodArgs, returnType) in config.FunctionSimulationPatterns)
        {
            ExecuteLocalGenericRegistration(nameof(AddFunctionAdapterServices), [entityType, methodArgs, returnType],
                simulationServiceContainer);
        }

        var entities = config
            .ActionSimulationPatterns.Select(e => e.entityType)
            .Union(config.FunctionSimulationPatterns.Select(e => e.entityType));
        foreach (var entity in entities)
        {
            ExecuteLocalGenericRegistration(nameof(RegisterPerEntityServices), [entity], simulationServiceContainer);
            ExecuteLocalGenericRegistration(nameof(AdaptToServerLayer), [entity], simulationServiceContainer,
                serviceCollection);
        }

        return config;
    }

    private static void AdaptToServerLayer<TEntity>(SimulationServicesContainer simulationServicesContainer,
        IServiceCollection serviceCollection)
    {
        var adapters = simulationServicesContainer.BuildServiceProvider().GetServices<INodeConfigurator<TEntity>>();
        foreach (var adapter in adapters)
        {
            serviceCollection.AddSingleton(adapter);
        }
    }
}