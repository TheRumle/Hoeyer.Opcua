using Hoeyer.Common.Extensions.Collection;
using Hoeyer.OpcUa.Core.Api;
using Hoeyer.OpcUa.Core.Api.NodeStructure;
using Hoeyer.OpcUa.Core.Configuration.Modelling;
using Hoeyer.OpcUa.Core.Test.Fixtures;
using Microsoft.Extensions.DependencyInjection;

namespace Hoeyer.OpcUa.Core.Test.Application;

public class CoreServicesOfType<T>(
    Type wantedGenericService,
    IServiceCollection collection) : DataSourceGeneratorAttribute<GenericImplementation<T>> where T : class
{
    private readonly IServiceProvider _serviceProvider = collection.BuildServiceProvider();

    public CoreServicesOfType(Type wantedGenericService) : this(
        wantedGenericService,
        new OpcUaCoreServicesFixtureAttribute().ServiceCollection)
    {
    }

    private EntityTypesCollection Entities => _serviceProvider.GetService<EntityTypesCollection>()!;

    protected override IEnumerable<Func<GenericImplementation<T>>> GenerateDataSources(
        DataGeneratorMetadata dataGeneratorMetadata)
    {
        if (!wantedGenericService.IsGenericTypeDefinition)
        {
            throw new NotSupportedException(nameof(wantedGenericService) + "must be open generic version of T");
        }

        return Entities.ModelledEntities.Select(entity =>
        {
            var service = _serviceProvider.GetService(wantedGenericService.MakeGenericType(entity)) as T;
            var browseNameCollection = _serviceProvider
                .GetService(typeof(IBrowseNameCollection<>)
                    .MakeGenericType(entity)) as IBrowseNameCollection;

            var behaviour = _serviceProvider
                .GetService(typeof(IBehaviourTypeModel<>).MakeGenericType(entity)) as IBehaviourTypeModel;
            return new GenericImplementation<T>(service!, browseNameCollection!, behaviour!);
        }).SelectFunc();
    }
}