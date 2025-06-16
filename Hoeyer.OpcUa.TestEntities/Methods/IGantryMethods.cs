using Hoeyer.OpcUa.Core;

namespace Hoeyer.OpcUa.TestEntities.Methods;

[OpcUaEntityMethods<Gantry>]
public interface IGantryMethods
{
    Task ChangePosition(Position position);
    Task<int> PlaceContainer(Position position);
    Task PickUpContainer(Position position);
    Task<Guid> GetContainerId();
    Task<DateTime> GetDate();
    Task<List<DateTime>> GetDates();
}