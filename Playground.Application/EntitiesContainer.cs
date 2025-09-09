using Hoeyer.OpcUa.Core.Services.OpcUaServices;

namespace Playground.Application;

public class EntitiesContainer
{
    public IEnumerable<Type> Entities { get; set; } = OpcUaEntityTypes.Entities;
}