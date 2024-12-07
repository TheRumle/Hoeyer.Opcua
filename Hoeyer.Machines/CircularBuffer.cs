using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Hoeyer.Machines;

internal class CircularBuffer<T> : ICollection<T>
{
    private T[] _buffer;
    private int _head;
    private int _tail;
    private int _count;

    public CircularBuffer(int capacity)
    {
        if (capacity <= 0)
            throw new ArgumentException("Capacity must be greater than zero.", nameof(capacity));
        
        Capacity = capacity;
        _buffer = new T[capacity];
        _head = 0;
        _tail = 0;
        _count = 0;
    }

    public int Capacity { get; }

    /// <inheritdoc />
    public bool Remove(T item)
    {
        if (IsEmpty) return false;
        var index = IndexOf(item);
        if (index == -1) return false;
        ShiftElementAfter(index);
        return true;
    }

    private void ShiftElementAfter(int index)
    {
        for (var i = index; i != _tail; i = (i + 1) % Capacity) 
            _buffer[i] = _buffer[(i + 1) % Capacity];

        _tail = (_tail - 1 + Capacity) % Capacity;
        _count--;
    }

    private int IndexOf(T item)
    {
        for (var i = 0; i < _count; i++)
        {
            var currentIndex = (_head + i) % Capacity;
            if (EqualityComparer<T>.Default.Equals(_buffer[currentIndex], item))
                return currentIndex;
        }

        return -1;
    }

    public int Count => _count;

    public bool IsReadOnly => false;

    public bool IsFull => _count == Capacity;

    public bool IsEmpty => _count == 0;

    public void Add(T item)
    {
        _buffer[_tail] = item;
        _tail = (_tail + 1) % Capacity;
        if (IsFull)
        {
            _head = (_head + 1) % Capacity; // Overwrite oldest
        }
        else
        {
            _count++;
        }
    }
    
    public T Peek()
    {
        if (IsEmpty)
            throw new InvalidOperationException("Buffer is empty.");
        
        T item = _buffer[_head];
        _head = (_head + 1) % Capacity;
        _count--;
        return item;
    }

    public void Clear()
    {
        _head = 0;
        _tail = 0;
        _count = 0;
        Array.Clear(_buffer, 0, Capacity);
    }

    public bool Contains(T item)
    {
        EqualityComparer<T> comparer = EqualityComparer<T>.Default;
        return this.Any(element => comparer.Equals(element, item));
    }

    public void CopyTo(T[] array, int arrayIndex)
    {
        if (array == null) throw new ArgumentNullException(nameof(array));

        if (arrayIndex < 0 || arrayIndex + _count > array.Length)
            throw new ArgumentOutOfRangeException(nameof(arrayIndex));

        foreach (var item in this)
        {
            array[arrayIndex++] = item;
        }
    }

    public IEnumerator<T> GetEnumerator()
    {
        for (int i = 0; i < _count; i++)
        {
            yield return _buffer[(_head + i) % Capacity];
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
    
    

}