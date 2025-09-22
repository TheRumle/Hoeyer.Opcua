using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using Hoeyer.Common.Architecture;
using Hoeyer.Common.Extensions.Types;
using Hoeyer.Common.Reflection;
using Hoeyer.Common.Utilities.Threading;
using Hoeyer.OpcUa.Core.Api;
using Hoeyer.OpcUa.Server.Api.NodeManagement;
using Hoeyer.OpcUa.Simulation.Api.PostProcessing;
using Hoeyer.OpcUa.Simulation.ServerAdapter.Api;
using Hoeyer.OpcUa.Simulation.ServerAdapter.Application;
using Hoeyer.OpcUa.Simulation.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Hoeyer.OpcUa.Simulation.ServerAdapter;

public sealed class LockedEntityState<TState>(
    ILocked<IEntityNode> node,
    IEntityTranslator<TState> translator) : ILocked<TState>
{
    private readonly object _lock = new();

    public void Examine(Action<TState> effect)
    {
        node.Examine(e => effect(translator.Translate(e)));
    }

    public void ChangeState(Action<TState> stateChanges)
    {
        lock (_lock)
        {
            node.ChangeState(e =>
            {
                var state = translator.Translate(e);
                stateChanges(state);
                translator.AssignToNode(state, e);
            });
        }
    }

    public TOut Select<TOut>(Func<TState, TOut> computation)
    {
        lock (_lock)
        {
            return node.Select(e => computation(translator.Translate(e)));
        }
    }
}

public sealed class OpcUaServerAdapter : ILayerAdapter<SimulationServicesConfig>
{
    public void Adapt(SimulationServicesConfig adaptionSource, IServiceCollection adaptionTarget)
    {
        var simulationServiceContainer = adaptionSource.SimulationServices;

        ExtractInputArgumentTranslation(simulationServiceContainer);

        foreach (var (entityType, methodArgs) in adaptionSource.ActionSimulationPatterns)
        {
            ExecuteLocalGenericRegistration(nameof(AdaptActionSimulationToTarget), [entityType, methodArgs],
                simulationServiceContainer, adaptionTarget);
        }

        foreach (var (entityType, methodArgs, returnType) in adaptionSource.FunctionSimulationPatterns)
        {
            ExecuteLocalGenericRegistration(nameof(AdaptFunctionSimulationToTarget),
                [entityType, methodArgs, returnType], simulationServiceContainer, adaptionTarget);
        }
    }

    private static void ExtractInputArgumentTranslation(SimulationServicesContainer simulationServiceContainer)
    {
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
    }

    [SuppressMessage("Design", "S3011",
        Justification =
            "This makes type safety easier to manage as compiletime information about generic arguments is present.")]
    private static void ExecuteLocalGenericRegistration(string methodName, Type[] generics, params object[] arguments)
    {
        var info = typeof(OpcUaServerAdapter).GetMethod(methodName,
            BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Public)!;
        info.MakeGenericMethod(generics).Invoke(null, arguments);
    }

    private static void BuildSingletonAdapterInstance<TEntity>(
        SimulationServicesContainer servicesContainer, IServiceCollection targetCollection)
    {
        servicesContainer.AddSingleton<ISimulationExecutorErrorHandler, SimulationExecutorErrorHandler>();
        servicesContainer.AddSingleton<EntityStateChangedNotifier<TEntity>>();
        servicesContainer.AddSingleton<IStateChangeSimulationProcessor<TEntity>>(p =>
            p.GetRequiredService<EntityStateChangedNotifier<TEntity>>());
        servicesContainer.AddSingleton<INodeConfigurator<TEntity>>(p =>
            p.GetRequiredService<EntityStateChangedNotifier<TEntity>>());
        var adapters = servicesContainer.BuildServiceProvider().GetServices<INodeConfigurator<TEntity>>();
        foreach (var adapter in adapters)
        {
            targetCollection.AddSingleton(adapter);
        }
    }

    private static void AdaptFunctionSimulationToTarget<TEntity, TMethodArgs, TReturnType>(
        SimulationServicesContainer source, IServiceCollection target)
    {
        source.AddServiceAndImplTransient<
            IAdaptionContextTranslator<TEntity, TMethodArgs, TReturnType>,
            AdaptionContextTranslator<TEntity, TMethodArgs, TReturnType>
        >();

        source.AddServiceAndImplSingleton<
            INodeConfigurator<TEntity>,
            SimulationNodeConfigurator<TEntity, TMethodArgs, TReturnType>
        >();

        BuildSingletonAdapterInstance<TEntity>(source, target);
    }


    private static void AdaptActionSimulationToTarget<TEntity, TMethodArgs>(
        SimulationServicesContainer simulationServicesContainer, IServiceCollection targetCollection)
    {
        simulationServicesContainer.AddServiceAndImplTransient<
            IAdaptionContextTranslator<TEntity, TMethodArgs>,
            AdaptionContextTranslator<TEntity, TMethodArgs>
        >();

        simulationServicesContainer.AddServiceAndImplSingleton<
            INodeConfigurator<TEntity>,
            SimulationNodeConfigurator<TEntity, TMethodArgs>
        >();

        BuildSingletonAdapterInstance<TEntity>(simulationServicesContainer, targetCollection);
    }
}