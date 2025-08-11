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

        ExtractInputArgumentTranslation(simulationServiceContainer);

        foreach (var (agentType, methodArgs) in adaptionSource.ActionSimulationPatterns)
        {
            ExecuteLocalGenericRegistration(nameof(AdaptActionSimulationToTarget), [agentType, methodArgs],
                simulationServiceContainer, adaptionTarget);
        }

        foreach (var (agentType, methodArgs, returnType) in adaptionSource.FunctionSimulationPatterns)
        {
            ExecuteLocalGenericRegistration(nameof(AdaptFunctionSimulationToTarget),
                [agentType, methodArgs, returnType], simulationServiceContainer, adaptionTarget);
        }
    }

    private static void ExtractInputArgumentTranslation(SimulationServicesContainer simulationServiceContainer)
    {
        var argTranslators = typeof(IAgentMethodArgTranslator<>)
            .GetTypesFromConsumingAssemblies()
            .Select(e => (Implementor: e,
                translatorInterface: e.GetImplementedVersionOfGeneric(typeof(IAgentMethodArgTranslator<>))))
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
        var info = typeof(ServerSimulationAdapter).GetMethod(methodName,
            BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Public)!;
        info.MakeGenericMethod(generics).Invoke(null, arguments);
    }

    private static void BuildSingletonAdapterInstance<TAgent>(
        SimulationServicesContainer simulationServicesContainer, IServiceCollection targetCollection)
    {
        simulationServicesContainer.AddSingleton<AgentStateChangedNotifier<TAgent>>();
        simulationServicesContainer.AddSingleton<IStateChangeSimulationProcessor<TAgent>>(p =>
            p.GetRequiredService<AgentStateChangedNotifier<TAgent>>());
        simulationServicesContainer.AddSingleton<INodeConfigurator<TAgent>>(p =>
            p.GetRequiredService<AgentStateChangedNotifier<TAgent>>());
        var adapters = simulationServicesContainer.BuildServiceProvider().GetServices<INodeConfigurator<TAgent>>();
        foreach (var adapter in adapters)
        {
            targetCollection.AddSingleton(adapter);
        }
    }

    private static void AdaptFunctionSimulationToTarget<TAgent, TMethodArgs, TReturnType>(
        SimulationServicesContainer source, IServiceCollection target)
    {
        source.AddTransient<
            IAdaptionContextTranslator<(IList<object>, IManagedAgent), TAgent, TMethodArgs, TReturnType>,
            AdaptionContextTranslator<TAgent, TMethodArgs, TReturnType>
        >();


        source.AddSingleton<
            INodeConfigurator<TAgent>,
            FunctionSimulationAdapter<TAgent, TMethodArgs, TReturnType>
        >();

        BuildSingletonAdapterInstance<TAgent>(source, target);
    }


    private static void AdaptActionSimulationToTarget<TAgent, TMethodArgs>(
        SimulationServicesContainer simulationServicesContainer, IServiceCollection targetCollection)
    {
        simulationServicesContainer.AddTransient<
            IAdaptionContextTranslator<(IList<object>, IManagedAgent), TAgent, TMethodArgs>,
            AdaptionContextTranslator<TAgent, TMethodArgs>
        >();

        simulationServicesContainer.AddSingleton<
            INodeConfigurator<TAgent>,
            ActionSimulationAdapter<TAgent, TMethodArgs>
        >();

        BuildSingletonAdapterInstance<TAgent>(simulationServicesContainer, targetCollection);
    }
}