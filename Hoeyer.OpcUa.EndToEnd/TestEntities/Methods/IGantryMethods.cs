using Hoeyer.OpcUa.Core;

namespace Hoeyer.OpcUa.EndToEndTest.TestEntities.Methods;

[OpcUaEntityMethods<Gantry>]
public interface IGantryMethods
{
    public Task A(int q);
    public Task<int> LetsGoInt(int a, float b, List<int> i);
}