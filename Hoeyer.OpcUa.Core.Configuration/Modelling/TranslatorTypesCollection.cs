using System.Collections.Frozen;
using Hoeyer.Common.Architecture;
using Hoeyer.Common.Extensions.Types;
using Hoeyer.OpcUa.Core.Api;
using Hoeyer.OpcUa.Core.Api.NodeStructure;
using Microsoft.Extensions.DependencyInjection;

namespace Hoeyer.OpcUa.Core.Configuration.Modelling;

public sealed class TranslatorTypesCollection
{
    private static readonly Type NonGenericTranslatorInterface = typeof(IEntityTranslator<>);

    public TranslatorTypesCollection(
        [FromKeyedServices(ServiceKeys.MODELLING)]
        IEnumerable<AssemblyMarker> markers,
        EntityTypesCollection collection)
    {
        Translators = markers.SelectMany(e => e.TypesInAssembly)
            .Where(type => type is { IsAbstract: false, IsInterface: false, IsClass: true }
                           && type.IsImplementationOfGeneric(NonGenericTranslatorInterface))
            .Select(type => (
                Service: type.GetInterfaces().First(@interface =>
                    @interface.IsClosedGenericOf(NonGenericTranslatorInterface)),
                Impl: type
            ))
            .ToFrozenSet();

        TranslatorByEntity =
            Translators.ToFrozenDictionary(e => e.Service.GetGenericArguments()[0], e => (e.Service, e.Impl));

        var entitiesWithoutTranslatorImpl = collection.ModelledEntities.Except(TranslatorByEntity.Keys);
        var errors = entitiesWithoutTranslatorImpl.Select(entity =>
            new ModellingMismatchException(
                $"The entity {entity.FullName} does not have any implementation of {NonGenericTranslatorInterface.FullName} in the modelling landscape. Did you forget to add source generation to the assembly markers?")
        ).ToList();

        if (errors.Count > 0)
        {
            throw new AggregateException(errors);
        }
    }

    public FrozenDictionary<Type, (Type Service, Type Impl)> TranslatorByEntity { get; set; }
    public FrozenSet<(Type Service, Type Impl)> Translators { get; }
}