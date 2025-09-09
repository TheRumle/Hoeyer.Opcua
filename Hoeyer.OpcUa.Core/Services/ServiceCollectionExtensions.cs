using System;
using System.Collections.Frozen;
using System.Linq;
using Hoeyer.Common.Extensions.Types;
using Hoeyer.OpcUa.Core.Api;
using Hoeyer.OpcUa.Core.Application.NodeStructureFactory;
using Hoeyer.OpcUa.Core.Configuration;
using Hoeyer.OpcUa.Core.Configuration.EntityServerBuilder;
using Hoeyer.OpcUa.Core.Services.OpcUaServices;
using Microsoft.Extensions.DependencyInjection;

namespace Hoeyer.OpcUa.Core.Services;

public static class ServiceCollectionExtensions
{
    private static readonly Type NonGenericTranslatorInterface = typeof(IEntityTranslator<>);

    public static readonly FrozenSet<(Type Service, Type Impl)> TranslatorImplementations =
        OpcUaEntityTypes.TypesFromReferencingAssemblies
            .Where(type => type is { IsInterface: false, IsAbstract: false, IsGenericTypeDefinition: false })
            .Select(ConstructServiceImplTuple)
            .Where(e => e.Services is not null)
            .ToFrozenSet();

    public static FrozenSet<(Type Service, Type Impl)> TranslatorImplementationsUsingMarker(this Type marker) =>
        OpcUaEntityTypes.TypesFromReferencingAssembliesUsingMarker(marker)
            .Where(type => type is { IsInterface: false, IsAbstract: false, IsGenericTypeDefinition: false })
            .Select(ConstructServiceImplTuple)
            .Where(e => e.Services is not null)
            .ToFrozenSet();

    private static (Type Services, Type Impl) ConstructServiceImplTuple(Type type)
    {
        try
        {
            return (
                Services: type.GetInterfaces()
                    .FirstOrDefault(@interface => @interface.IsGenericImplementationOf(NonGenericTranslatorInterface)),
                Impl: type);
        }
        catch (Exception e)
        {
            throw new InvalidOperationException("constructing tuple for type " + type.FullName + " threw exception");
        }
    }


    public static OnGoingOpcEntityServiceRegistration AddOpcUaServerConfiguration(this IServiceCollection services,
        Func<IEntityServerConfigurationBuilder, IOpcUaEntityServerInfo> configurationBuilder)
    {
        var entityServerConfiguration = configurationBuilder.Invoke(EntityServerConfigurationBuilder.Create());
        services.AddSingleton(entityServerConfiguration);
        return new OnGoingOpcEntityServiceRegistration(services);
    }

    public static IServiceCollection WithEntityServices(this IServiceCollection services) =>
        services.WithEntityServices(typeof(ServiceCollectionExtensions));

    public static IServiceCollection WithEntityServices(this IServiceCollection services, Type marker)
    {
        foreach (var (service, impl) in TranslatorImplementationsUsingMarker(marker))
        {
            services.AddServiceAndImplSingleton(service, impl);
        }

        services.AddSingleton(typeof(IEntityNodeStructureFactory<>), typeof(ReflectionBasedEntityStructureFactory<>));
        return services;
    }

    public static OnGoingOpcEntityServiceRegistration WithEntityServices(
        this OnGoingOpcEntityServiceRegistration registration)
    {
        registration.Collection.WithEntityServices();
        return registration;
    }

    public static void AddEntityServiceAsNonGenericSingleton<TInterfaceTarget>(this IServiceCollection collection,
        Type genericInterface) =>
        AddEntityServiceAsNonGeneric<TInterfaceTarget>(collection, genericInterface, ServiceLifetime.Singleton);

    public static void AddEntityServiceAsNonGenericTransient<TInterfaceTarget>(this IServiceCollection collection,
        Type genericInterface) =>
        AddEntityServiceAsNonGeneric<TInterfaceTarget>(collection, genericInterface, ServiceLifetime.Transient);

    public static void AddEntityServiceAsNonGenericScoped<TInterfaceTarget>(this IServiceCollection collection,
        Type genericInterface) =>
        AddEntityServiceAsNonGeneric<TInterfaceTarget>(collection, genericInterface, ServiceLifetime.Scoped);


    public static void AddEntityServiceAsNonGeneric<TInterfaceTarget>(this IServiceCollection collection,
        Type genericInterface, ServiceLifetime descriptor)
    {
        var target = typeof(TInterfaceTarget);
        if (!genericInterface.IsGenericTypeDefinition || genericInterface.GetGenericArguments().Length != 1)
        {
            throw new ArgumentException(
                genericInterface.FullName + " must be a generic type definition with 1 type arg");
        }

        if (!typeof(TInterfaceTarget).IsAssignableFrom(genericInterface))
        {
            throw new ArgumentException(genericInterface.FullName + " must be assignable to " + target.FullName);
        }

        foreach (var entity in OpcUaEntityTypes.Entities)
        {
            switch (descriptor)
            {
                case ServiceLifetime.Singleton:
                    collection.AddSingleton(target, genericInterface.MakeGenericType(entity));
                    break;
                case ServiceLifetime.Scoped:
                    collection.AddScoped(target, genericInterface.MakeGenericType(entity));
                    break;
                case ServiceLifetime.Transient:
                    collection.AddTransient(target, genericInterface.MakeGenericType(entity));
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(descriptor), descriptor, null);
            }
        }
    }

    public static IServiceCollection AddServiceAndImplSingleton(this IServiceCollection collection, Type service,
        Type impl)
        => AddServiceAndImpl(collection, service, impl, ServiceLifetime.Singleton);

    public static IServiceCollection AddServiceAndImplTransient(this IServiceCollection collection, Type service,
        Type impl)
        => AddServiceAndImpl(collection, service, impl, ServiceLifetime.Transient);

    public static IServiceCollection AddServiceAndImplScoped(this IServiceCollection collection, Type service,
        Type impl)
        => AddServiceAndImpl(collection, service, impl, ServiceLifetime.Scoped);


    public static IServiceCollection AddServiceAndImplSingleton<T>(this IServiceCollection collection, Type impl)
        => AddServiceAndImpl(collection, typeof(T), impl, ServiceLifetime.Singleton);

    public static IServiceCollection AddServiceAndImplTransient<T>(this IServiceCollection collection, Type impl)
        => AddServiceAndImpl(collection, typeof(T), impl, ServiceLifetime.Transient);

    public static IServiceCollection AddServiceAndImplScoped<T>(this IServiceCollection collection, Type impl)
        => AddServiceAndImpl(collection, typeof(T), impl, ServiceLifetime.Scoped);

    public static IServiceCollection AddServiceAndImplSingleton<TService, TImpl>(this IServiceCollection collection)
        => AddServiceAndImpl(collection, typeof(TService), typeof(TImpl), ServiceLifetime.Singleton);

    public static IServiceCollection AddServiceAndImplTransient<TService, TImpl>(this IServiceCollection collection)
        => AddServiceAndImpl(collection, typeof(TService), typeof(TImpl), ServiceLifetime.Transient);

    public static IServiceCollection AddServiceAndImplScoped<TService, TImpl>(this IServiceCollection collection)
        => AddServiceAndImpl(collection, typeof(TService), typeof(TImpl), ServiceLifetime.Scoped);


    public static IServiceCollection AddServiceAndImpl(this IServiceCollection collection, Type service, Type impl,
        ServiceLifetime descriptor)
    {
        switch (descriptor)
        {
            case ServiceLifetime.Singleton:
                collection.AddSingleton(impl, impl);
                collection.AddSingleton(service, p => p.GetService(impl));
                break;
            case ServiceLifetime.Scoped:
                collection.AddScoped(service, impl);
                collection.AddScoped(impl, impl);
                break;
            case ServiceLifetime.Transient:
                collection.AddTransient(service, impl);
                collection.AddTransient(impl, impl);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(descriptor), descriptor, null);
        }

        return collection;
    }
}