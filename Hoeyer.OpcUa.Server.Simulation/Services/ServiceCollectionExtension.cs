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
using Hoeyer.OpcUa.Server.Simulation.Services.Action;
using Hoeyer.OpcUa.Server.Simulation.Services.Function;
using Microsoft.Extensions.DependencyInjection;

namespace Hoeyer.OpcUa.Server.Simulation.Services;

public static class ServiceCollectionExtension
{
    public static OnGoingOpcEntityServiceRegistration WithOpcUaServerSimulation(
        this OnGoingOpcEntityServiceRegistration registration)
    {
        IServiceCollection serviceCollection = registration.Collection;
        serviceCollection.AddTransient<ITimeScaler, IdentityTimeScaler>();
        ImmutableHashSet<Type> typeReferences =
            typeof(IEntityMethodArgTranslator<>).GetTypesFromConsumingAssemblies().AsParallel().ToImmutableHashSet();


        var translatorInfoTuple = typeReferences
            .Select(e => (Implementor: e,
                translatorInterface: e.GetImplementedVersionOfGeneric(typeof(IEntityMethodArgTranslator<>))))
            .Where(e => e.translatorInterface is not null)
            .AsParallel()
            .Select(e => (e.Implementor, Interface: e.translatorInterface,
                ArgType: e.translatorInterface!.GenericTypeArguments[0]));

        foreach ((Type Implementor, Type? Interface, Type ArgType) translatorInfo in translatorInfoTuple)
        {
            serviceCollection.AddTransient(translatorInfo.Interface!, translatorInfo.Implementor);
        }

        Type actionSimulatorInterface = typeof(IActionSimulationConfigurator<,>).GetGenericTypeDefinition();
        List<SimulationPatternTypeDetails> actionSimulators =
            GetSimulatorDetails(typeReferences, actionSimulatorInterface).ToList();

        Type functionSimulatorInterface = typeof(IFunctionSimulationConfigurator<,,>).GetGenericTypeDefinition();
        List<SimulationPatternTypeDetails> functionSimulators =
            GetSimulatorDetails(typeReferences, functionSimulatorInterface).ToList();

        List<DuplicateSimulatorConfigurationException> duplicateDefinitions = actionSimulators.Union(functionSimulators)
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
            Type returnType = method!.ReturnType.GetGenericArguments()[0];

            Type simulationStepFactoryTypeService =
                typeof(ISimulationStepFactory<,>).MakeGenericType(entity, methodArgType);
            Type simulationStepFactoryType = typeof(SimulationStepFactory<,>).MakeGenericType(entity, methodArgType);
            serviceCollection.AddTransient(simulationStepFactoryTypeService, simulationStepFactoryType);

            serviceCollection.AddSingleton(simulatorInterface, implementor);

            Type functionConfigurator =
                typeof(FunctionSimulationSetup<,,>).MakeGenericType(entity, methodArgType, returnType);
            Type configurator = typeof(IPreinitializedNodeConfigurator<>).MakeGenericType(entity);

            Type executor = typeof(FunctionSimulationExecutor<,,>).MakeGenericType(entity, methodArgType, returnType);
            Type executorInterface = typeof(IFunctionSimulationExecutor<,>).MakeGenericType(methodArgType, returnType);

            serviceCollection.AddSingleton(configurator, functionConfigurator);
            serviceCollection.AddSingleton(executorInterface, executor);
        }
    }

    private static void AddActionSimulation(List<SimulationPatternTypeDetails> typeReferences,
        IServiceCollection serviceCollection)
    {
        foreach ((Type implementor, Type simulatorInterface, Type methodArgType, MethodInfo? _, Type entity) in
                 typeReferences)
        {
            serviceCollection.AddSingleton(simulatorInterface, implementor);

            Type actionConfigurator = typeof(ActionSimulationSetup<,>).MakeGenericType(entity, methodArgType);
            Type configurator = typeof(IPreinitializedNodeConfigurator<>).MakeGenericType(entity);

            Type executor = typeof(ActionSimulationExecutor<,>).MakeGenericType(entity, methodArgType);
            Type executorInterface = typeof(IActionSimulationExecutor<>).MakeGenericType(methodArgType);

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
        MethodInfo? MethodBeingSimulated,
        Type Entity);
}