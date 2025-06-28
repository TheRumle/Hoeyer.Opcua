using Hoeyer.OpcUa.Core;

namespace Hoeyer.OpcUa.TestEntities.Methods;

[OpcUaEntityMethods<Gantry>]
public interface IGantryMethods
{
    Task ChangePosition(Position position);
    Task PlaceContainer();
    Task PickUpContainer();
    Task<Guid> GetCurrentContainerId();
}