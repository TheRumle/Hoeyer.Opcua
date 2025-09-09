using System.Collections;

namespace Hoeyer.OpcUa.EntityModelling.Subtypes;

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