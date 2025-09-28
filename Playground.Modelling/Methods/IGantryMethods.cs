using Hoeyer.OpcUa.Core;
using Hoeyer.OpcUa.EntityModelling.Models;

namespace Hoeyer.OpcUa.EntityModelling.Methods;

[OpcUaEntityMethods<Gantry>]
public interface IGantryMethods
{
    [BrowseName("ChangeGantryPosition")]
    Task ChangePosition(Position position);

    Task<int> PlaceContainer(Position position);
    Task PickUpContainer(Position position);
    public Task AssignContainer(Guid guid);
}