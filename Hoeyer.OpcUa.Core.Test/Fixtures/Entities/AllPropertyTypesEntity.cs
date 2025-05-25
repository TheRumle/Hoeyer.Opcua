using System.Collections;

namespace Hoeyer.OpcUa.Core.Test.Fixtures.Entities;

public sealed class CustomIList : IList<int>
{
    private readonly List<int> _list = new();

    /// <inheritdoc />
    public IEnumerator<int> GetEnumerator() => _list.GetEnumerator();

    /// <inheritdoc />
    IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)_list).GetEnumerator();

    /// <inheritdoc />
    public void Add(int item)
    {
        _list.Add(item);
    }

    /// <inheritdoc />
    public void Clear()
    {
        _list.Clear();
    }

    /// <inheritdoc />
    public bool Contains(int item) => _list.Contains(item);

    /// <inheritdoc />
    public void CopyTo(int[] array, int arrayIndex)
    {
        _list.CopyTo(array, arrayIndex);
    }

    /// <inheritdoc />
    public bool Remove(int item) => _list.Remove(item);

    /// <inheritdoc />
    public int Count => _list.Count;

    /// <inheritdoc />
    public bool IsReadOnly => ((ICollection<int>)_list).IsReadOnly;

    /// <inheritdoc />
    public int IndexOf(int item) => _list.IndexOf(item);

    /// <inheritdoc />
    public void Insert(int index, int item)
    {
        _list.Insert(index, item);
    }

    /// <inheritdoc />
    public void RemoveAt(int index)
    {
        _list.RemoveAt(index);
    }

    /// <inheritdoc />
    public int this[int index]
    {
        get => _list[index];
        set => _list[index] = value;
    }
}

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