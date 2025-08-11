using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using Hoeyer.Common.Extensions.Types;
using Hoeyer.Common.Messaging.Api;
using Hoeyer.Common.Messaging.Subscriptions;
using Hoeyer.Common.Messaging.Subscriptions.ChannelBased;
using Hoeyer.Common.Reflection;
using Hoeyer.OpcUa.Core.Configuration;
using Hoeyer.OpcUa.Simulation.Api;
using Hoeyer.OpcUa.Simulation.Api.Configuration;
using Hoeyer.OpcUa.Simulation.Api.Configuration.Exceptions;
using Hoeyer.OpcUa.Simulation.Api.Execution;
using Hoeyer.OpcUa.Simulation.Api.PostProcessing;
using Hoeyer.OpcUa.Simulation.Api.Services;
using Hoeyer.OpcUa.Simulation.Configuration;
using Hoeyer.OpcUa.Simulation.Execution;
using Hoeyer.OpcUa.Simulation.PostProcessing;
using Microsoft.Extensions.DependencyInjection;

namespace Hoeyer.OpcUa.Simulation.Services;

public static class ServiceCollectionExtension
{
    public static OnGoingOpcAgentServiceRegistrationWithSimulation WithOpcUaSimulationServices(
        this OnGoingOpcAgentServiceRegistration registration)
    {
        return WithOpcUaSimulationServices(registration, (c) => { });
    }

    public static OnGoingOpcAgentServiceRegistrationWithSimulation WithOpcUaSimulationServices(
        this IServiceCollection registration, Action<SimulationServicesConfig> configure) =>
        WithOpcUaSimulationServices(new OnGoingOpcAgentServiceRegistration(registration), configure);


    public static OnGoingOpcAgentServiceRegistrationWithSimulation WithOpcUaSimulationServices(
        this OnGoingOpcAgentServiceRegistration registration, Action<SimulationServicesConfig> configure)
    {
        var originalCollection = registration.Collection;
        var collection = new ServiceCollection();
        var simulationServices = new SimulationServicesContainer(collection);
        simulationServices.AddRange(originalCollection);
        originalCollection
            .AddTransient<IOpcMethodArgumentsAttributeUsageValidator, OpcMethodArgumentsAttributeUsageValidator>();
        simulationServices
            .AddTransient<IOpcMethodArgumentsAttributeUsageValidator, OpcMethodArgumentsAttributeUsageValidator>();
        simulationServices.AddTransient<ITimeScaler, Identity>();


        //Used as marker
        var typeReferences = typeof(SimulationApiAssemblyMarker)
            .GetTypesFromConsumingAssemblies()
            .AsParallel()
            .ToImmutableHashSet();

        var (actionSimulators, functionSimulators) = AddCoreServices(typeReferences, simulationServices);
        ValidateServices(actionSimulators.Union(functionSimulators));

        var config = new SimulationServicesConfig(
            registration.Collection,
            simulationServices,
            actionSimulators,
            functionSimulators);

        configure.Invoke(config);
        return new OnGoingOpcAgentServiceRegistrationWithSimulation(registration.Collection, config);
    }

    private static void ValidateServices(IEnumerable<SimulationPatternTypeDetails> actionSimulators)
    {
        var duplicateDefinitions = actionSimulators
            .GroupBy(e => e.InstantiatedSimulatorInterface).Where(g => g.Count() > 1).Select(group =>
            {
                var message =
                    $"Multiple implementations of '{group.Key.GetFriendlyTypeName()}' were found. The framework allows only for one implementation per simulation. Remove one of the simulations: [\n\t{string.Join(",\n\t", group.Select(e => e.Implementor.GetFriendlyTypeName()))}\n]";
                return new DuplicateSimulatorConfigurationException(message);
            }).ToList();

        if (duplicateDefinitions.Any())
        {
            throw new SimulationConfigurationException(duplicateDefinitions);
        }
    }

    private static (List<SimulationPatternTypeDetails> ActionsServices, List<SimulationPatternTypeDetails>
        FunctionServices)
        AddCoreServices(ImmutableHashSet<Type> typeReferences, SimulationServicesContainer simulationServicesContainer)
    {
        var actionSimulatorInterface = typeof(ISimulation<,>).GetGenericTypeDefinition();
        var actionSimulators = GetSimulatorConfiguratorsImplementing(typeReferences, actionSimulatorInterface).ToList();

        var functionSimulatorInterface = typeof(ISimulation<,,>).GetGenericTypeDefinition();
        var functionSimulators =
            GetSimulatorConfiguratorsImplementing(typeReferences, functionSimulatorInterface).ToList();

        AddActionServices(actionSimulators, simulationServicesContainer);
        AddFunctionServices(functionSimulators, simulationServicesContainer);
        return (actionSimulators, functionSimulators);
    }

    private static void AddFunctionServices(List<SimulationPatternTypeDetails> typeReferences,
        SimulationServicesContainer simulationServicesContainer)
    {
        foreach ((Type implementor, Type simulatorInterface, Type methodArgType, Type methodReturnType,
                     Type agent) in typeReferences)
        {
            List<IServiceCollection> args = [simulationServicesContainer];
            ExecuteLocalStaticGeneric(
                nameof(AddFunctionSimulationConfigurator),
                generics: [agent, methodArgType, methodReturnType],
                args: [implementor, args]);

            ExecuteLocalStaticGeneric(
                nameof(RegisterFunctionServices),
                generics: [agent, methodArgType, methodReturnType],
                args: args);
        }
    }


    private static void AddActionServices(List<SimulationPatternTypeDetails> typeReferences,
        SimulationServicesContainer simulationServicesContainer)
    {
        foreach (var (implementor, simulatorInterface, methodArgType, _, agent) in
                 typeReferences)
        {
            List<IServiceCollection> args = [simulationServicesContainer];
            ExecuteLocalStaticGeneric(
                nameof(AddActionSimulationConfigurator),
                generics: [agent, methodArgType],
                args: [implementor, args]);

            ExecuteLocalStaticGeneric(
                nameof(RegisterActionServices),
                generics: [agent, methodArgType],
                args: args);
        }
    }

    private static void AddActionSimulationConfigurator<TAgent, TArgs>(Type impl,
        params IEnumerable<IServiceCollection> collections)
    {
        foreach (var serviceCollection in collections)
        {
            serviceCollection.AddScoped<ISimulationBuilder<TAgent, TArgs>>
                (p => p.GetRequiredService<ISimulationBuilderFactory<TAgent, TArgs>>().CreateSimulationBuilder());

            serviceCollection
                .AddSingleton<ISimulationBuilderFactory<TAgent, TArgs>, SimulationBuilderFactory<TAgent, TArgs>>();
            serviceCollection.AddTransient(typeof(ISimulation<TAgent, TArgs>), impl);
            serviceCollection.AddTransient(impl, impl);
        }
    }

    private static void AddFunctionSimulationConfigurator<TAgent, TArgs, TReturn>(Type impl,
        params IEnumerable<IServiceCollection> collections)
    {
        foreach (var serviceCollection in collections)
        {
            serviceCollection.AddScoped<ISimulationBuilder<TAgent, TArgs, TReturn>>
            (p => p.GetRequiredService<ISimulationBuilderFactory<TAgent, TArgs, TReturn>>()
                .CreateSimulationBuilder());

            serviceCollection
                .AddSingleton<ISimulationBuilderFactory<TAgent, TArgs, TReturn>,
                    SimulationBuilderFactory<TAgent, TArgs, TReturn>>();
            serviceCollection.AddTransient(typeof(ISimulation<TAgent, TArgs, TReturn>), impl);
            serviceCollection.AddTransient(impl, impl);
        }
    }

    [SuppressMessage("Design", "S3011",
        Justification =
            "This makes type safety easier to manage as compiletime information about generic arguments is present.")]
    private static void ExecuteLocalStaticGeneric(string methodName, Type[] generics, params object[] args)
    {
        var info = typeof(ServiceCollectionExtension).GetMethod(methodName,
            BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Public)!;

        info.MakeGenericMethod(generics).Invoke(null, args);
    }


    private static void RegisterFunctionServices<TAgent, TArgs, TReturn>(
        params IEnumerable<IServiceCollection> simulationServices)
    {
        foreach (var simulationService in simulationServices)
        {
            AddSubscription<TAgent>(simulationService);
            simulationService
                .AddTransient<ISimulationStepValidator, ReturnValueOrderValidator<TAgent, TArgs, TReturn>>();
            simulationService
                .AddTransient<ISimulationExecutor<TAgent, TArgs>,
                    SimulationExecutor<TAgent, TArgs>>(); //for compositional design
            simulationService
                .AddTransient<ISimulationExecutor<TAgent, TArgs, TReturn>,
                    SimulationExecutor<TAgent, TArgs, TReturn>>();
            simulationService
                .AddSingleton<ISimulationProcessorPipeline<TAgent, TArgs, TReturn>,
                    SimulationProcessorPipeline<TAgent, TArgs, TReturn>>();
            simulationService
                .AddSingleton<ISimulationOrchestrator<TAgent, TArgs, TReturn>,
                    SimulationOrchestrator<TAgent, TArgs, TReturn>>();
        }
    }

    private static void RegisterActionServices<TAgent, TArgs>(
        params IEnumerable<IServiceCollection> simulationServices)
    {
        foreach (var simulationService in simulationServices)
        {
            AddSubscription<TAgent>(simulationService);
            simulationService.AddTransient<ISimulationExecutor<TAgent, TArgs>, SimulationExecutor<TAgent, TArgs>>();
            simulationService
                .AddSingleton<ISimulationProcessorPipeline<TAgent, TArgs>,
                    SimulationProcessorPipeline<TAgent, TArgs>>();
            simulationService
                .AddSingleton<ISimulationOrchestrator<TAgent, TArgs>, SimulationOrchestrator<TAgent, TArgs>>();
        }
    }


    private static IEnumerable<SimulationPatternTypeDetails> GetSimulatorConfiguratorsImplementing(
        IEnumerable<Type> typeReferences,
        Type genericSimulatorInterface)
    {
        return typeReferences
            .Select(type => (implementor: type,
                simulatorInterfaces: type.GetAllImplementedVersionsOfGeneric(genericSimulatorInterface)))
            .Where(t => t.simulatorInterfaces.Any())
            .SelectMany(t => t.simulatorInterfaces
                .Select(@interface => CreateConfiguratorInfoTuple(t.implementor, @interface)));
    }


    private static SimulationPatternTypeDetails CreateConfiguratorInfoTuple(Type implementor, Type simulatorInterface)
    {
        Type? args = simulatorInterface!.GenericTypeArguments[1];
        IOpcMethodArgumentsAttribute? argumentAttribute =
            args.GetCustomAttributes().OfType<IOpcMethodArgumentsAttribute>().First();

        return new SimulationPatternTypeDetails(implementor,
            simulatorInterface,
            simulatorInterface!.GenericTypeArguments[1],
            argumentAttribute.Method,
            simulatorInterface!.GenericTypeArguments[0]);
    }

    private static void AddSubscription<TAgent>(IServiceCollection simulationService)
    {
        simulationService.AddSingleton<
            IMessageSubscriptionFactory<SimulationResult<TAgent>>,
            ChannelSubscriptionFactory<SimulationResult<TAgent>>
        >();
        simulationService.AddSingleton<
            ISubscriptionManager<SimulationResult<TAgent>>,
            SubscriptionManager<SimulationResult<TAgent>>
        >();
    }
}