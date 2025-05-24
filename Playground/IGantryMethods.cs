using Hoeyer.OpcUa.Core;

namespace Playground;

[OpcUaEntityMethods<Gantry>]
public interface IGantryMethods
{
    Task None(int q, int b, List<int> dict);

    Task<int> Int(int q, int b, List<int> dict);
    Task<string> String(int q, int b, List<int> dict);
    Task<Position> GetRandomPosition(string q, float b, List<int> dict);
}