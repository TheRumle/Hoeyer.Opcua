using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Linq;
using Hoeyer.Common.Architecture;
using Hoeyer.Common.Extensions.Types;
using Hoeyer.OpcUa.Core;
using Hoeyer.OpcUa.Core.Api.NodeStructure;
using Microsoft.Extensions.DependencyInjection;

namespace Hoeyer.OpcUa.Client.Services;

internal class EntityBehaviourImplementationModel<T>
{
    public EntityBehaviourImplementationModel(
        [FromKeyedServices(ServiceKeys.CLIENT_SERVICES)]
        IEnumerable<AssemblyMarker> markers, IEntityTypeModel<T> model)
    {
        var wantedInterfaces = model.BehaviourInterfaces;
        MethodImplementors = markers
            .SelectMany(marker => marker.TypesInAssembly)
            .Where(type => type is { IsInterface: false, IsAbstract: false } && type.ImplementsAny(wantedInterfaces))
            .Select(type => (
                EntityBehaviourInterface: type.GetInterfaces().First(wantedInterfaces.Contains),
                EntityBehaviourImplementation: type
            )).ToFrozenSet();

        var hits = MethodImplementors.Select(e => e.EntityBehaviourInterface);
        var misses = model.BehaviourInterfaces.Except(hits);
        var errors = misses.Select(@interface => new ModellingMismatchException(
            $"The modelling landscape does not contain any implementation of {@interface.FullName}. Did you forget to add an assembly marker for assemblies containing interfaces annotated with [{typeof(OpcUaEntityMethodsAttribute<>)}]? Did you add source generation to those assemblies?")
        ).ToList();

        if (errors.Count > 0)
        {
            throw new AggregateException(errors);
        }
    }

    public FrozenSet<(Type EntityBehaviourInterface, Type EntityBehaviourImplementation)> MethodImplementors
    {
        get;
        set;
    }
}