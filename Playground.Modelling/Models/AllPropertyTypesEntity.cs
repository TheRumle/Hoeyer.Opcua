using Hoeyer.OpcUa.Core;

namespace Playground.Modelling.Models;

[OpcUaEntity]
public sealed record AllPropertyTypesEntity
{
    public enum EnumValue
    {
        start = 10,
        middle = 15,
        end = 20
    }


    public int Integer { get; set; }
    public long Long { get; set; }
    public Guid Guid { get; set; }
    public required string String { get; set; }
    public required List<int> IntList { get; set; }
    public double Double { get; set; }
    public float Float { get; set; }
    public bool Bool { get; set; }

    public EnumValue EnumVal { get; set; }


    public static AllPropertyTypesEntity CreateRandom()
    {
        Random _random = new();
        return new AllPropertyTypesEntity
        {
            Integer = _random.Next(),
            Long = _random.NextInt64(),
            String = Guid.NewGuid().ToString(),
            Guid = Guid.NewGuid(),
            IntList = Enumerable.Range(1, _random.Next(1, 10)).Select(e => _random.Next()).ToList(),
            Double = _random.NextDouble() * 1000,
            Float = (float)(_random.NextDouble() * 1000),
            Bool = _random.Next(2) == 1
        };
    }
}