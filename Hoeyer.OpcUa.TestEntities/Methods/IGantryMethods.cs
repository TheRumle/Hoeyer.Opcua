using Hoeyer.OpcUa.Core;

namespace Hoeyer.OpcUa.TestEntities.Methods;

[OpcUaEntityMethods<Gantry>]
public interface IGantryMethods
{
    public Task IntegerInput(int q);
    public Task<int> MultiInputIntReturn(int a, float b, List<int> i);
    public Task<int> MoreMethods(int a, float b, float c, List<int> i);
}