using Hoeyer.OpcUa.Core.Configuration.Modelling;
using Microsoft.Extensions.DependencyInjection;

namespace Hoeyer.OpcUa.Core.Configuration;

public record OnGoingOpcEntityServiceRegistrationWithModels(
    IServiceCollection Collection,
    EntityTypesCollection EntityCollection
) : OnGoingOpcEntityServiceRegistration(Collection)
{
    public EntityTypesCollection EntityCollection { get; } = EntityCollection;
}