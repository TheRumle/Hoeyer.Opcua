using System;
using System.Text;

namespace Hoeyer.OpcUa.Client.SourceGeneration.Generation;

public sealed class SourceCodeWriter : IDisposable
{
    private readonly StringBuilder _stringBuilder = new();
    private int _tabLevel;

    public void Dispose()
    {
        _stringBuilder.Clear();
    }


    public SourceCodeWriter Write(string value)
    {
        _stringBuilder.Append(value);
        return this;
    }

    public SourceCodeWriter WriteTabs()
    {
        for (var i = 0; i < _tabLevel; i++)
        {
            _stringBuilder.Append('\t');
        }

        return this;
    }

    public SourceCodeWriter WriteLine(string value)
    {
        if (string.IsNullOrEmpty(value)) return this;

        if (value[0] is '}' or ']') _tabLevel--;

        WriteTabs();

        _stringBuilder.AppendLine(value);

        if (value.Equals("{") || value.Equals("[")) _tabLevel++;

        return this;
    }

    public SourceCodeWriter WriteLine()
    {
        _stringBuilder.AppendLine();
        return this;
    }


    public override string ToString() => _stringBuilder.ToString();
}