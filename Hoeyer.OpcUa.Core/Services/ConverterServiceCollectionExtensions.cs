using Hoeyer.OpcUa.Core.Abstractions;
using Hoeyer.OpcUa.Core.Application.OpcTypeMappers;
using Microsoft.Extensions.DependencyInjection;
using Opc.Ua;

namespace Hoeyer.OpcUa.Core.Services;

public static class ConverterServiceCollectionExtensions
{
    public static IServiceCollection AddVariantBuilders(
        this IServiceCollection services,
        params ICollection<Type> assemblyMarkers)
    {
        var assemblyTypes = assemblyMarkers
            .SelectMany(e => e.Assembly.GetTypes())
            .Distinct()
            .OrderBy(e => e.FullName)
            .ToArray();

        foreach (var types in assemblyTypes
                     .Where(t => t is { IsGenericTypeDefinition: false, IsClass: false, IsAbstract: false })
                     .SelectMany(VariantBuilderInterfaces))
        {
            services.AddSingleton(types.service, types.serviceImpl);
        }

        services.AddSingleton<VariantConverterCollection>(provider => CreateConverterCollection(provider, assemblyTypes));
        services.AddSingleton<AggregateVariantConverter>();
        return services;
    }


    private static VariantConverterCollection CreateConverterCollection(
        IServiceProvider provider,
        Type[] assemblyTypes)
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

        var otherConverters = assemblyTypes
            .Where(t => t is { IsGenericTypeDefinition: false, IsClass: false, IsAbstract: false })
            .SelectMany(VariantBuilderInterfaces)
            .Select(types => (
                provider.GetRequiredService(types.serviceImpl),
                types.service)
            )
            .Select(VariantGenericArg)
            .Where(e => !defaultBuilders.Keys.Contains(e.variantArg)) //remove duplicate
            .ToDictionary(tuple => tuple.variantArg, tuple => tuple.implTypes);

        var result = defaultBuilders
            .Concat(enumConverters)
            .Concat(otherConverters)
            .ToDictionary(e => e.Key, e => e.Value);

        return new VariantConverterCollection(result);
    }


    private static (object implTypes, Type variantArg) VariantGenericArg((object implTypes, Type variantArg) arg) =>
        (arg.implTypes, arg.variantArg.GetGenericArguments()[0]);

    private static IEnumerable<(Type serviceImpl, Type service)> VariantBuilderInterfaces(Type serviceImpl)
    {
        return serviceImpl
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