using Hoeyer.OpcUa.Core.Abstractions;
using Hoeyer.OpcUa.Core.Application.OpcTypeMappers;
using Microsoft.Extensions.DependencyInjection;
using Opc.Ua;

namespace Hoeyer.OpcUa.Core.Services;

public static class ConverterServiceCollectionExtensions
{
    public static IServiceCollection AddVariantConverters(
        this IServiceCollection services,
        params ICollection<Type> assemblyMarkers)
    {
        var assemblyTypes = assemblyMarkers.SelectMany(e => e.Assembly.GetTypes()).ToArray();
        
        var variantBuilderImplementors = AddVariantBuilderImplementations(services, assemblyTypes);
        
        services.AddSingleton<VariantConverterCollection>(provider => CreateConverterCollection(provider, variantBuilderImplementors));
        services.AddSingleton<AggregateVariantConverter>();
        
        return services;
    }
    
    private static Type[] AddVariantBuilderImplementations(IServiceCollection services, IEnumerable<Type> assemblyTypes)
    {
        var implementations = assemblyTypes
            .Where(t => t is {IsGenericTypeDefinition: false, IsClass: false, IsAbstract: false } && t.GetInterfaces().Any(IsVariantBuilder))
            .ToArray();

        // Register every concrete implementation as a singleton.
        foreach (var implementation in implementations)
        {
            services.AddSingleton(implementation);
        }

        return implementations;
    }

    private static VariantConverterCollection CreateConverterCollection(
        IServiceProvider provider,
        Type[] variantBuilderImplementors)
    {
        var defaultBuilders = typeof(VariantBuilder)
            .GetInterfaces()
            .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IVariantBuilder<>))
            .ToDictionary(
                i => i.GetGenericArguments()[0],
                _ => (object)new VariantBuilder());

        var enumConverters = provider.GetRequiredService<IEnumerable<IBrowseNameCollection>>()
            .SelectMany(service => service.BrowseNameToEnumProperty.Values)
            .ToDictionary(propertyType => propertyType, CreateEnumVariantConverter);

        var otherConverters = variantBuilderImplementors
            .Select(provider.GetRequiredService)
            .SelectMany(VariantBuilderInterfaces)
            .Select(VariantGenericArg)
            .ToDictionary(tuple => tuple.service, tuple => tuple.serviceImpl);

        var result = defaultBuilders
            .Concat(enumConverters)
            .Concat(otherConverters)
            .ToDictionary(e => e.Key, e => e.Value);

        return new VariantConverterCollection(result);
    }


    private static (object serviceImpl, Type service) VariantGenericArg((object serviceImpl, Type service) arg) =>
        (arg.serviceImpl, arg.service.GetGenericArguments()[0]);

    private static IEnumerable<(object serviceImpl, Type service)> VariantBuilderInterfaces(object serviceImpl)
    {
        return serviceImpl.GetType()
            .GetInterfaces()
            .Where(IsVariantBuilder)
            .Select(service => (serviceImpl, service));
    }

    private static bool IsVariantBuilder(Type i) =>
        i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IVariantBuilder<>);

    private static object CreateEnumVariantConverter(Type enumType)
    {
        if (!enumType.IsEnum)
        {
            throw new ArgumentException(
                $"{enumType} is not an enumeration.",
                nameof(enumType));
        }

        Type converterType = typeof(EnumVariantConverter<>).MakeGenericType(enumType);
        return Activator.CreateInstance(converterType)!;
    }
}