using System;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Hoeyer.Common.Extensions.Types;
using Hoeyer.Common.Reflection;
using Hoeyer.OpcUa.Core.Configuration;
using Hoeyer.OpcUa.Server.Api.NodeManagement;
using Hoeyer.OpcUa.Server.Simulation.Api;
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


        ParallelQuery<(Type Implementor, Type? Interface, Type ArgType)> translatorInfoTuple = typeReferences
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
        Type? functionSimulatorInterface = typeof(IFunctionSimulationConfigurator<int, int>).GetGenericTypeDefinition();
        ParallelQuery<SimulationPatternTypeDetails> functionSimulators = typeReferences
            .Select(type => (implementor: type,
                simulatorInterface: type.GetImplementedVersionOfGeneric(functionSimulatorInterface)))
            .Where(configurator => configurator.simulatorInterface is not null)
            .Select(t => CreateConfiguratorInfoTuple(t.implementor, t.simulatorInterface!));


        foreach ((Type implementor, Type instantiatedSimulatorInterface, Type methodArgType, MethodInfo? method,
                     Type entity) in functionSimulators)
        {
            //There was not a matching method on the interface - likely due to generation failure
            if (method is null)
            {
                continue;
                //throw exception   
            }

            //When working with an IActionSimulator the return type for the client method that is being simulated must be Task (aka remote void)
            //If it is not, then IMethodSimulator should be implemented instead
            if (method.ReturnType.GetGenericTypeDefinition() != typeof(Task<>))
            {
                //aggregate some exceptions
            }

            Type simulationRetType = method.ReturnType.GetGenericArguments()[0];
            serviceCollection.AddSingleton(instantiatedSimulatorInterface, implementor);

            Type actionConfigurator = typeof(FunctionSimulationSetup<int, int, int>).GetGenericTypeDefinition()
                .MakeGenericType(entity, methodArgType, simulationRetType);
            Type configurator = typeof(IPreinitializedNodeConfigurator<int>).GetGenericTypeDefinition()
                .MakeGenericType(entity);
            serviceCollection.AddSingleton(configurator, actionConfigurator);

            Type wantedExecutor = typeof(FunctionSimulationExecutor<int, int, int>).GetGenericTypeDefinition()
                .MakeGenericType(methodArgType);
            Type executorService = typeof(IActionSimulationExecutor<int>).GetGenericTypeDefinition()
                .MakeGenericType(methodArgType);
            serviceCollection.AddSingleton(executorService, wantedExecutor);
        }
    }

    private static void AddActionSimulation(ParallelQuery<Type> typeReferences, IServiceCollection serviceCollection)
    {
        Type? actionSimulatorInterface = typeof(IActionSimulationConfigurator<int, int>).GetGenericTypeDefinition();
        ParallelQuery<SimulationPatternTypeDetails> actionSimulators = typeReferences
            .Select(type => (implementor: type,
                simulatorInterface: type.GetImplementedVersionOfGeneric(actionSimulatorInterface)))
            .Where(configurator => configurator.simulatorInterface is not null)
            .Select(t => CreateConfiguratorInfoTuple(t.implementor, t.simulatorInterface!));

        foreach ((Type implementor, Type instantiatedSimulatorInterface, Type methodArgType, MethodInfo? method,
                     Type entity) in actionSimulators)
        {
            //There was not a matching method on the interface - likely due to generation failure
            if (method is null)
            {
                continue;
                //throw exception   
            }

            //When working with an IActionSimulator the return type for the client method that is being simulated must be Task (aka remote void)
            //If it is not, then IMethodSimulator should be implemented instead
            if (method.ReturnType != typeof(Task))
            {
                //aggregate some exceptions
            }

            serviceCollection.AddSingleton(instantiatedSimulatorInterface, implementor);

            Type actionConfigurator = typeof(ActionsSimulationSetup<int, int>).GetGenericTypeDefinition()
                .MakeGenericType(entity, methodArgType);
            Type configurator = typeof(IPreinitializedNodeConfigurator<int>).GetGenericTypeDefinition()
                .MakeGenericType(entity);
            serviceCollection.AddSingleton(configurator, actionConfigurator);


            Type wantedExecutor = typeof(ActionSimulationExecutor<int>).GetGenericTypeDefinition()
                .MakeGenericType(methodArgType);
            Type executorService = typeof(IActionSimulationExecutor<int>).GetGenericTypeDefinition()
                .MakeGenericType(methodArgType);
            serviceCollection.AddSingleton(executorService, wantedExecutor);
        }
    }

    internal static SimulationPatternTypeDetails CreateConfiguratorInfoTuple(Type implementor, Type simulatorInterface)
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

    internal record struct SimulationPatternTypeDetails(
        Type Implementor,
        Type InstantiatedSimulatorInterface,
        Type MethodArgType,
        MethodInfo? MethodBeingSimulated,
        Type Entity);
}