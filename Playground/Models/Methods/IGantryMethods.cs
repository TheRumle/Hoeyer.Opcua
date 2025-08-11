using Hoeyer.OpcUa.Core;

namespace Playground.Models.Methods;

[OpcUaAgentMethods<Gantry>]
public interface IGantryMethods
{
    Task ChangePosition(Position position);
    Task<int> PlaceContainer(Position position);
    Task PickUpContainer(Position position);
    Task GetContainerId(Position position);
    public Task NoReturnValue(int q);
    public Task<int> IntReturnValue();
}