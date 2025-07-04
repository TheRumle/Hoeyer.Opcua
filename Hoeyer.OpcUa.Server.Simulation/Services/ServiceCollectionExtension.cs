using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using Hoeyer.Common.Extensions.Types;
using Hoeyer.Common.Reflection;
using Hoeyer.OpcUa.Core.Configuration;
using Hoeyer.OpcUa.Server.Api.NodeManagement;
using Hoeyer.OpcUa.Server.Simulation.Api;
using Hoeyer.OpcUa.Server.Simulation.Builder;
using Hoeyer.OpcUa.Server.Simulation.ServerAdapter;
using Hoeyer.OpcUa.Server.Simulation.ServerAdapter.Action;
using Hoeyer.OpcUa.Server.Simulation.ServerAdapter.Function;
using Hoeyer.OpcUa.Server.Simulation.Services.Action;
using Hoeyer.OpcUa.Server.Simulation.Services.Function;
using Microsoft.Extensions.DependencyInjection;

namespace Hoeyer.OpcUa.Server.Simulation.Services;

public static class ServiceCollectionExtension
{
    public static OnGoingOpcEntityServiceRegistration WithOpcUaServerSimulation(
        this OnGoingOpcEntityServiceRegistration registration)
    {
        var serviceCollection = registration.Collection;
        serviceCollection.AddTransient<ITimeScaler, IdentityTimeScaler>();
        var typeReferences = typeof(IEntityMethodArgTranslator<>).GetTypesFromConsumingAssemblies().AsParallel()
            .ToImmutableHashSet();


        var translatorInfoTuple = typeReferences
            .Select(e => (Implementor: e,
                translatorInterface: e.GetImplementedVersionOfGeneric(typeof(IEntityMethodArgTranslator<>))))
            .Where(e => e.translatorInterface is not null)
            .AsParallel()
            .Select(e => (e.Implementor, Interface: e.translatorInterface,
                ArgType: e.translatorInterface!.GenericTypeArguments[0]));

        foreach (var (implementor, @interface, argType) in translatorInfoTuple)
        {
            serviceCollection.AddTransient(@interface!, implementor);
            serviceCollection.AddTransient(
                typeof(IMethodArgumentParser<>).MakeGenericType(argType),
                typeof(MethodArgumentParser<>).MakeGenericType(argType)
            );
        }

        var actionSimulatorInterface = typeof(IActionSimulationConfigurator<,>).GetGenericTypeDefinition();
        var actionSimulators = GetSimulatorDetails(typeReferences, actionSimulatorInterface).ToList();

        var functionSimulatorInterface = typeof(IFunctionSimulationConfigurator<,,>).GetGenericTypeDefinition();
        var functionSimulators = GetSimulatorDetails(typeReferences, functionSimulatorInterface).ToList();

        var duplicateDefinitions = actionSimulators.Union(functionSimulators)
            .GroupBy(e => e.InstantiatedSimulatorInterface).Where(g => g.Count() > 1).Select(group =>
            {
                var message =
                    $"Multiple implementations of '{group.Key.GetFriendlyTypeName()}' were found. The framework allows only for one implementation per method to simulate. Remove one of the simulation configurators: [\n\t{string.Join(",\n\t", group.Select(e => e.Implementor.GetFriendlyTypeName()))}\n]";
                return new DuplicateSimulatorConfigurationException(message);
            }).ToList();

        if (duplicateDefinitions.Any())
        {
            throw new SimulationConfigurationException(duplicateDefinitions);
        }

        serviceCollection
            .AddTransient<IOpcMethodArgumentsAttributeUsageValidator, OpcMethodArgumentsAttributeUsageValidator>();
        serviceCollection.AddTransient<ISimulationExecutorErrorHandler, SimulationExecutorErrorHandler>();

        AddActionSimulation(actionSimulators, serviceCollection);
        AddFunctionSimulation(functionSimulators, serviceCollection);

        return registration;
    }

    private static void AddFunctionSimulation(List<SimulationPatternTypeDetails> typeReferences,
        IServiceCollection serviceCollection)
    {
        foreach ((Type implementor, Type simulatorInterface, Type methodArgType, MethodInfo? method,
                     Type entity) in typeReferences)
        {
            serviceCollection.AddSingleton(simulatorInterface, implementor);

            Type returnType = method!.ReturnType.GetGenericArguments()[0];

            serviceCollection.AddTransient(
                typeof(IFunctionSimulationOrchestrator<>).MakeGenericType(returnType),
                typeof(FunctionSimulationOrchestrator<,,>).MakeGenericType(entity, methodArgType, returnType)
            );

            serviceCollection.AddTransient(
                typeof(IFunctionSimulationBuilderFactory<,,>).MakeGenericType(entity, methodArgType, returnType),
                typeof(FunctionSimulationBuilderFactory<,,>).MakeGenericType(entity, methodArgType, returnType)
            );


            Type simulationStepFactoryTypeService =
                typeof(ISimulationStepFactory<,>).MakeGenericType(entity, methodArgType);
            Type simulationStepFactoryType = typeof(SimulationStepFactory<,>).MakeGenericType(entity, methodArgType);
            serviceCollection.AddTransient(simulationStepFactoryTypeService, simulationStepFactoryType);


            Type functionConfigurator =
                typeof(FunctionSimulationSetup<,,>).MakeGenericType(entity, methodArgType, returnType);
            Type configurator = typeof(IPreinitializedNodeConfigurator<>).MakeGenericType(entity);

            Type executor = typeof(FunctionSimulationExecutor<,,>).MakeGenericType(entity, methodArgType, returnType);
            var executorInterface =
                typeof(IFunctionSimulationExecutor<,,>).MakeGenericType(entity, methodArgType, returnType);

            serviceCollection.AddSingleton(configurator, functionConfigurator);
            serviceCollection.AddSingleton(executorInterface, executor);
        }
    }

    private static void AddActionSimulation(List<SimulationPatternTypeDetails> typeReferences,
        IServiceCollection serviceCollection)
    {
        foreach (var (implementor, simulatorInterface, methodArgType, _, entity) in
                 typeReferences)
        {
            serviceCollection.AddSingleton(simulatorInterface, implementor);
            serviceCollection.AddTransient(
                typeof(IActionSimulationOrchestrator),
                typeof(ActionSimulationOrchestrator<,>).MakeGenericType(entity, methodArgType)
            );

            serviceCollection.AddTransient(
                typeof(IActionSimulationBuilderFactory<,>).MakeGenericType(entity, methodArgType),
                typeof(ActionSimulationBuilderFactory<,>).MakeGenericType(entity, methodArgType)
            );

            Type actionConfigurator = typeof(ActionSimulationSetup<,>).MakeGenericType(entity, methodArgType);
            Type configurator = typeof(IPreinitializedNodeConfigurator<>).MakeGenericType(entity);

            Type executor = typeof(ActionSimulationExecutor<,>).MakeGenericType(entity, methodArgType);
            Type executorInterface = typeof(IActionSimulationExecutor<,>).MakeGenericType(entity, methodArgType);

            serviceCollection.AddSingleton(configurator, actionConfigurator);
            serviceCollection.AddSingleton(executorInterface, executor);
        }
    }

    private static IEnumerable<SimulationPatternTypeDetails> GetSimulatorDetails(IEnumerable<Type> typeReferences,
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

    private record struct SimulationPatternTypeDetails(
        Type Implementor,
        Type InstantiatedSimulatorInterface,
        Type MethodArgType,
        MethodInfo MethodBeingSimulated,
        Type Entity);
}