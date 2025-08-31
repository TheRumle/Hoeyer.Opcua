using Hoeyer.OpcUa.Core;
using Hoeyer.OpcUa.TestEntities.Models;

namespace Hoeyer.OpcUa.TestEntities.Methods;

[OpcUaEntityMethods<Gantry>]
public interface IGantryMethods
{
    Task ChangePosition(Position position);
    Task<int> PlaceContainer(Position position);
    Task PickUpContainer(Position position);
    Task GetContainerId(Position position);
    public Task NoReturnValue(int q);
    public Task<Guid> GetCurrentContainerId();
}