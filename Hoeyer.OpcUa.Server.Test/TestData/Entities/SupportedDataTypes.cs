using Hoeyer.OpcUa.Core.Entity;

namespace Hoeyer.OpcUa.Server.Test.TestData.Entities;

[OpcUaEntity]
public class SupportedDataTypes
{
    public IList<int> TestIList { get; set; } = [];
    public ICollection<int> TestICollection { get; set; } = [];
    public IEnumerable<int> TestIEnumerable { get; set; } = [];
    public List<int> TestList { get; set; } = [];
    public ISet<int> TestISet { get; set; } = new HashSet<int>();
    public HashSet<int> TestHashSet { get; set; } = [];
    public SortedSet<int> TestSortedSet { get; set; } = new();

    public bool TestBool { get; set; }
    public byte TestByte { get; set; } 
    public short TestShort { get; set; }
    public ushort TestUShort { get; set; }
    public int TestInt { get; set; }
    public uint TestUInt { get; set; }
    public long TestLong { get; set; }
    public ulong TestULong { get; set; }
    public float TestFloat { get; set; }
    public double TestDouble { get; set; }
    public string TestString { get; set; } = "Test";
    public decimal TestDecimal { get; set; }
}