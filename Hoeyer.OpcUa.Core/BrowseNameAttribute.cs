namespace Hoeyer.OpcUa.Core;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Class | AttributeTargets.Enum)]
public sealed class BrowseNameAttribute : Attribute
{
    public BrowseNameAttribute(string browseName)
    {
        if (string.IsNullOrEmpty(browseName))
        {
            throw new ArgumentOutOfRangeException(nameof(browseName), "The browse name must not be null or empty.");
        }

        BrowseName = browseName;
    }

    public string BrowseName { get; }
}