using Hoeyer.Common.Architecture;
using Hoeyer.OpcUa.Core.Api;
using Hoeyer.OpcUa.Core.Api.NodeStructure;
using Hoeyer.OpcUa.Core.Application.NodeStructure;
using Hoeyer.OpcUa.Core.Configuration.Modelling;
using Hoeyer.OpcUa.Core.Configuration.Options;
using Hoeyer.OpcUa.Core.Configuration.ServerTarget;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Hoeyer.OpcUa.Core.Configuration;

public static class ServiceCollectionExtensions
{
    public static OnGoingOpcEntityServiceRegistration AddOpcUa(this IServiceCollection services,
        Func<IEntityServerConfigurationBuilder, IOpcUaTargetServerInfo> configurationBuilder)
    {
        var entityServerConfiguration = configurationBuilder.Invoke(EntityServerConfigurationBuilder.Create());
        services.AddSingleton(entityServerConfiguration);
        return new OnGoingOpcEntityServiceRegistration(services);
    }

    public static OnGoingOpcEntityServiceRegistration AddOpcUaFromEnvironmentVariables(
        this IHostApplicationBuilder applicationBuilder, string sectionName = "OpcUa")
    {
        applicationBuilder.Configuration.AddEnvironmentVariables();
        return AddOpcUaFromOptions(applicationBuilder.Services, sectionName);
    }

    public static OnGoingOpcEntityServiceRegistration AddOpcUaFromOptions(
        this IHostApplicationBuilder builder,
        string sectionName = "OpcUa") => AddOpcUaFromOptions(builder.Services, sectionName);

    public static OnGoingOpcEntityServiceRegistration AddOpcUaFromOptions(
        this IServiceCollection services,
        string sectionName = "OpcUa")
    {
        services.AddOptions<OpcUaOptions>()
            .BindConfiguration(sectionName)
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddSingleton<IOpcUaTargetServerInfoFactory, OpcUaTargetServerFromOptions>();
        services.AddSingleton<IOpcUaTargetServerInfo>(p => p.GetRequiredService<IOpcUaTargetServerInfoFactory>().Get());
        return new OnGoingOpcEntityServiceRegistration(services);
    }

    public static OnGoingOpcEntityServiceRegistrationWithModels WithEntityModelsFrom(
        this OnGoingOpcEntityServiceRegistration registration,
        params IEnumerable<Type> assemblyMarkers)
    {
        registration.Collection.WithEntityModelsFrom(assemblyMarkers);
        return new OnGoingOpcEntityServiceRegistrationWithModels(
            registration.Collection,
            registration.Collection.BuildServiceProvider()
                .GetRequiredService<EntityTypesCollection>());
    }

    public static IServiceCollection WithEntityModelsFrom(this IServiceCollection services,
        params IEnumerable<Type> assemblyMarkers)
    {
        var markers = assemblyMarkers.ToList();
        AddEntityModels(services, markers);
        services.AddSingleton<TranslatorTypesCollection>();
        services.AddSingleton(typeof(IBrowseNameCollection<>), typeof(EntityTypeModel<>));
        services.AddSingleton(typeof(IEntityTypeModel<>), typeof(EntityTypeModel<>));
        services.AddSingleton(typeof(IBehaviourTypeModel<>), typeof(EntityTypeModel<>));
        services.AddServiceAndImplSingleton(typeof(IEntityNodeStructureFactory<>),
            typeof(ReflectionBasedEntityStructureFactory<>));
        services.AddServiceAndImplSingleton(typeof(IEntityNodeMethodAssigner<>), typeof(EntityNodeMethodAssigner<>));
        services.AddServiceAndImplSingleton(typeof(IEntityNodePropertyAssigner<>),
            typeof(EntityNodePropertyAssigner<>));
        services.AddServiceAndImplSingleton(typeof(IEntityNodeAlarmAssigner<>), typeof(EntityNodeAlarmAssigner<>));
        AddTranslators(services);
        return services;
    }

    public static IServiceCollection AddEntityModels(this IServiceCollection services, List<Type> markers)
    {
        services.AddKeyedSingleton(ServiceKeys.MODELLING, markers.Select(m => new AssemblyMarker(m)));
        services.AddSingleton<EntityTypesCollection>();
        return services;
    }

    private static void AddTranslators(IServiceCollection services)
    {
        var translatorCollection = services.BuildServiceProvider().GetRequiredService<TranslatorTypesCollection>();
        foreach (var translator in translatorCollection.Translators)
        {
            services.AddSingleton(translator.Service, translator.Impl);
        }
    }
}