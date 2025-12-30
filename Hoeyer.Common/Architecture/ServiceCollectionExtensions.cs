using System;
using Microsoft.Extensions.DependencyInjection;

namespace Hoeyer.Common.Architecture;

public static class ServiceCollectionExtensions
{
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
                collection.AddSingleton(service, impl);
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