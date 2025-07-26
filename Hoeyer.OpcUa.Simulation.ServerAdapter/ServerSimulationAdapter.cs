using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using Hoeyer.Common.Architecture;
using Hoeyer.Common.Extensions.Types;
using Hoeyer.Common.Reflection;
using Hoeyer.OpcUa.Server.Api.NodeManagement;
using Hoeyer.OpcUa.Simulation.Api.PostProcessing;
using Hoeyer.OpcUa.Simulation.ServerAdapter.Api;
using Hoeyer.OpcUa.Simulation.ServerAdapter.Application;
using Hoeyer.OpcUa.Simulation.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Hoeyer.OpcUa.Simulation.ServerAdapter;

public sealed class ServerSimulationAdapter : ILayerAdapter<SimulationServicesConfig>
{
    public void Adapt(SimulationServicesConfig adaptionSource, IServiceCollection adaptionTarget)
    {
        var simulationServiceContainer = adaptionSource.SimulationServices;

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

        foreach (var (entityType, methodArgs) in adaptionSource.ActionSimulationPatterns)
        {
            ExecuteLocalGenericRegistration(nameof(AddActionAdapterServices), [entityType, methodArgs],
                simulationServiceContainer);
        }

        foreach (var (entityType, methodArgs, returnType) in adaptionSource.FunctionSimulationPatterns)
        {
            ExecuteLocalGenericRegistration(nameof(AddFunctionAdapterServices), [entityType, methodArgs, returnType],
                simulationServiceContainer);
        }

        var entities = adaptionSource
            .ActionSimulationPatterns.Select(e => e.entityType)
            .Union(adaptionSource.FunctionSimulationPatterns.Select(e => e.entityType));
        foreach (var entity in entities)
        {
            ExecuteLocalGenericRegistration(nameof(RegisterPerEntityServices), [entity], simulationServiceContainer);
            ExecuteLocalGenericRegistration(nameof(AdaptToServerLayer), [entity], simulationServiceContainer,
                adaptionTarget);
        }
    }

    [SuppressMessage("Design", "S3011",
        Justification =
            "This makes type safety easier to manage as compiletime information about generic arguments is present.")]
    private static void ExecuteLocalGenericRegistration(string methodName, Type[] generics, params object[] arguments)
    {
        var info = typeof(ServerSimulationAdapter).GetMethod(methodName,
            BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Public)!;
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
        simulationServicesContainer.AddTransient<
            IAdaptionContextTranslator<(IList<object>, IManagedEntityNode), TEntity, TMethodArgs>,
            AdaptionContextTranslator<TEntity, TMethodArgs>
        >();

        simulationServicesContainer.AddSingleton<
            INodeConfigurator<TEntity>,
            ActionSimulationAdapter<TEntity, TMethodArgs>
        >();
    }

    private static void AddFunctionAdapterServices<TEntity, TMethodArgs, TReturnType>(
        SimulationServicesContainer simulationServicesContainer)
    {
        simulationServicesContainer.AddTransient<
            IAdaptionContextTranslator<(IList<object>, IManagedEntityNode), TEntity, TMethodArgs, TReturnType>,
            AdaptionContextTranslator<TEntity, TMethodArgs, TReturnType>
        >();


        simulationServicesContainer.AddSingleton<
            INodeConfigurator<TEntity>,
            FunctionSimulationAdapter<TEntity, TMethodArgs, TReturnType>
        >();
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