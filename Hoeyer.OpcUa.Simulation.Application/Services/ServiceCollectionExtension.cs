using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using Hoeyer.Common.Architecture;
using Hoeyer.Common.Extensions.Types;
using Hoeyer.Common.Messaging.Api;
using Hoeyer.Common.Messaging.Subscriptions;
using Hoeyer.Common.Messaging.Subscriptions.ChannelBased;
using Hoeyer.Common.Reflection;
using Hoeyer.OpcUa.Core.Api;
using Hoeyer.OpcUa.Core.Configuration;
using Hoeyer.OpcUa.Simulation.Api;
using Hoeyer.OpcUa.Simulation.Api.Configuration;
using Hoeyer.OpcUa.Simulation.Api.Configuration.Exceptions;
using Hoeyer.OpcUa.Simulation.Api.Execution;
using Hoeyer.OpcUa.Simulation.Api.PostProcessing;
using Hoeyer.OpcUa.Simulation.Api.Services;
using Hoeyer.OpcUa.Simulation.Execution;
using Hoeyer.OpcUa.Simulation.PostProcessing;
using Microsoft.Extensions.DependencyInjection;

namespace Hoeyer.OpcUa.Simulation.Services;

public static class ServiceCollectionExtension
{
    public static OnGoingOpcEntityServiceRegistrationWithSimulation WithOpcUaSimulationServices(
        this OnGoingOpcEntityServiceRegistration registration)
    {
        return WithOpcUaSimulationServices(registration, (c) => { });
    }

    public static OnGoingOpcEntityServiceRegistrationWithSimulation WithOpcUaSimulationServices(
        this IServiceCollection registration, Action<SimulationServicesConfig> configure,
        Assembly? assemblyContainingSimulators = null) =>
        WithOpcUaSimulationServices(new OnGoingOpcEntityServiceRegistration(registration), configure,
            assemblyContainingSimulators);


    public static OnGoingOpcEntityServiceRegistrationWithSimulation WithOpcUaSimulationServices(
        this OnGoingOpcEntityServiceRegistration registration, Action<SimulationServicesConfig> configure,
        Assembly? assemblyContainingSimulators = null)
    {
        var originalCollection = registration.Collection;
        var collection = new ServiceCollection();
        var simulationServices = new SimulationServicesContainer(collection);
        simulationServices.AddRange(originalCollection);
        originalCollection
            .AddServiceAndImplTransient<IOpcMethodArgumentsAttributeUsageValidator,
                OpcMethodArgumentsAttributeUsageValidator>();
        simulationServices
            .AddServiceAndImplTransient<IOpcMethodArgumentsAttributeUsageValidator,
                OpcMethodArgumentsAttributeUsageValidator>();
        simulationServices.AddServiceAndImplTransient<ITimeScaler, IdentityTimeScaler>();

        var translators = originalCollection.Where(descriptor =>
            descriptor.ServiceType.IsGenericType
            && descriptor.ServiceType.GetGenericTypeDefinition() == typeof(IEntityTranslator<>));
        foreach (var original in translators)
        {
            simulationServices.Add(original);
        }

        var typeReferences = typeof(SimulationApiAssemblyMarker)
            .GetTypesFromConsumingAssemblies()
            .Union(assemblyContainingSimulators?.GetTypes() ?? [])
            .Union(typeof(ServiceCollectionExtension).GetTypesFromConsumingAssemblies())
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
        return new OnGoingOpcEntityServiceRegistrationWithSimulation(registration.Collection, config);
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
        AddCoreServices(ImmutableHashSet<Type> typeReferences,
            SimulationServicesContainer simulationServicesContainer)
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
                     Type entity) in typeReferences)
        {
            List<IServiceCollection> args = [simulationServicesContainer];
            ExecuteLocalStaticGeneric(
                nameof(AddFunctionSimulationConfigurator),
                generics: [entity, methodArgType, methodReturnType],
                args: [implementor, args]);

            ExecuteLocalStaticGeneric(
                nameof(RegisterFunctionServices),
                generics: [entity, methodArgType, methodReturnType],
                args: args);
        }
    }


    private static void AddActionServices(List<SimulationPatternTypeDetails> typeReferences,
        SimulationServicesContainer simulationServicesContainer)
    {
        foreach (var (implementor, simulatorInterface, methodArgType, _, entity) in
                 typeReferences)
        {
            List<IServiceCollection> args = [simulationServicesContainer];
            ExecuteLocalStaticGeneric(
                nameof(AddActionSimulationConfigurator),
                generics: [entity, methodArgType],
                args: [implementor, args]);

            ExecuteLocalStaticGeneric(
                nameof(RegisterActionServices),
                generics: [entity, methodArgType],
                args: args);
        }
    }

    private static void AddActionSimulationConfigurator<TEntity, TArgs>(Type impl,
        params IEnumerable<IServiceCollection> collections)
    {
        foreach (var serviceCollection in collections)
        {
            serviceCollection.AddTransient(typeof(ISimulation<TEntity, TArgs>), impl);
            serviceCollection.AddTransient(impl, impl);
        }
    }

    private static void AddFunctionSimulationConfigurator<TEntity, TArgs, TReturn>(Type impl,
        params IEnumerable<IServiceCollection> collections)
    {
        foreach (var serviceCollection in collections)
        {
            serviceCollection.AddTransient(typeof(ISimulation<TEntity, TArgs, TReturn>), impl);
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


    private static void RegisterFunctionServices<TEntity, TArgs, TReturn>(
        params IEnumerable<IServiceCollection> simulationServices)
    {
        foreach (var simulationService in simulationServices)
        {
            AddSubscription<TEntity>(simulationService);
            simulationService
                .AddTransient<ISimulationStepValidator, ReturnValueOrderValidator<TEntity, TArgs, TReturn>>();
            simulationService
                .AddTransient<ISimulationExecutor<TEntity, TArgs>,
                    SimulationExecutor<TEntity, TArgs>>(); //for compositional design
            simulationService
                .AddTransient<ISimulationExecutor<TEntity, TArgs, TReturn>,
                    SimulationExecutor<TEntity, TArgs, TReturn>>();
            simulationService
                .AddSingleton<ISimulationProcessorPipeline<TEntity, TArgs, TReturn>,
                    SimulationProcessorPipeline<TEntity, TArgs, TReturn>>();
            simulationService
                .AddSingleton<ISimulationOrchestrator<TEntity, TArgs, TReturn>,
                    SimulationOrchestrator<TEntity, TArgs, TReturn>>();
        }
    }

    private static void RegisterActionServices<TEntity, TArgs>(
        params IEnumerable<IServiceCollection> simulationServices)
    {
        foreach (var simulationService in simulationServices)
        {
            AddSubscription<TEntity>(simulationService);
            simulationService.AddTransient<ISimulationExecutor<TEntity, TArgs>, SimulationExecutor<TEntity, TArgs>>();
            simulationService
                .AddSingleton<ISimulationProcessorPipeline<TEntity, TArgs>,
                    SimulationProcessorPipeline<TEntity, TArgs>>();
            simulationService
                .AddSingleton<ISimulationOrchestrator<TEntity, TArgs>, SimulationOrchestrator<TEntity, TArgs>>();
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

    private static void AddSubscription<TEntity>(IServiceCollection simulationService)
    {
        simulationService.AddSingleton<
            IMessageSubscriptionFactory<SimulationResult<TEntity>>,
            ChannelSubscriptionFactory<SimulationResult<TEntity>>
        >();
        simulationService.AddSingleton<
            ISubscriptionManager<SimulationResult<TEntity>>,
            SubscriptionManager<SimulationResult<TEntity>>
        >();
    }
}