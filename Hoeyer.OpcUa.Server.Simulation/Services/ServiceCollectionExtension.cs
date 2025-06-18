using System;
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
            typeof(IEntityMethodArgTranslator<>).GetTypesFromConsumingAssemblies().ToImmutableHashSet();


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

        AddActionSimulation(typeReferences.AsParallel(), serviceCollection);
        AddFunctionSimulation(typeReferences.AsParallel(), serviceCollection);

        return registration;
    }

    private static void AddFunctionSimulation(ParallelQuery<Type> typeReferences, IServiceCollection serviceCollection)
    {
        Type functionSimulatorInterface = typeof(IFunctionSimulationConfigurator<,,>).GetGenericTypeDefinition();

        ParallelQuery<SimulationPatternTypeDetails> functionSimulators =
            GetSimulatorDetails(typeReferences, functionSimulatorInterface);

        foreach ((Type implementor, Type simulatorInterface, Type methodArgType, MethodInfo? method, Type entity) in
                 functionSimulators)
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

    private static void AddActionSimulation(ParallelQuery<Type> typeReferences, IServiceCollection serviceCollection)
    {
        Type actionSimulatorInterface = typeof(IActionSimulationConfigurator<,>).GetGenericTypeDefinition();

        ParallelQuery<SimulationPatternTypeDetails> actionSimulators =
            GetSimulatorDetails(typeReferences, actionSimulatorInterface);

        foreach ((Type implementor, Type simulatorInterface, Type methodArgType, MethodInfo? _, Type entity) in
                 actionSimulators)
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

    private static ParallelQuery<SimulationPatternTypeDetails> GetSimulatorDetails(ParallelQuery<Type> typeReferences,
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