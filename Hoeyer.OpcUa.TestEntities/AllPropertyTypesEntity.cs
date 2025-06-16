using Hoeyer.OpcUa.Core;
using Hoeyer.OpcUa.TestEntities.Subtypes;

namespace Hoeyer.OpcUa.TestEntities;

[OpcUaEntity]
public sealed record AllPropertyTypesEntity
{
    public enum EnumValue
    {
        start = 10,
        middle = 15,
        end = 20
    }


    public CustomIList CustomIListMember { get; set; }
    public int Integer { get; set; }
    public long Long { get; set; }
    public string String { get; set; }
    public Guid Guid { get; set; }
    public List<int> IntList { get; set; }
    public double Double { get; set; }
    public float Float { get; set; }
    public bool Bool { get; set; }


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
            Bool = _random.Next(2) == 1,
            CustomIListMember = new CustomIList
            {
                231, 22, 30
            }
        };
    }
}