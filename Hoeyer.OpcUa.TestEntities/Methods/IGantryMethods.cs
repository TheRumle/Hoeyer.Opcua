using Hoeyer.OpcUa.Core;

namespace Hoeyer.opcUa.TestEntities.Methods;

[OpcUaEntityMethods<Gantry>]
public interface IGantryMethods
{
    public Task A(int q);
    public Task<int> LetsGoInt(int a, float b, List<int> i);
    public Task<int> MoreMethods(int a, float b, float c, List<int> i);
}