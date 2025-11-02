using Hoeyer.OpcUa.Core;
using Playground.Modelling.Models;

namespace Playground.Modelling.Methods;

[OpcUaEntityMethods<Gantry>]
public interface IGantryMethods
{
    Task ChangePosition(Position position);

    Task<int> PlaceContainer(Position position);
    Task PickUpContainer(Position position);
    public Task AssignContainer(Guid guid);
}