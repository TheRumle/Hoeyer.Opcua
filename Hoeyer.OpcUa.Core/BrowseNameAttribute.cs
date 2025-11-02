using System;

namespace Hoeyer.OpcUa.Core;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Class | AttributeTargets.Enum)]
public sealed class BrowseNameAttribute : Attribute
{
    public BrowseNameAttribute(string browseName)
    {
        if (string.IsNullOrEmpty(browseName))
        {
            throw new ArgumentOutOfRangeException(nameof(browseName) + " was null or empty.");
        }

        BrowseName = browseName;
    }

    public string BrowseName { get; }
}