using System.Globalization;

namespace Hoeyer.Common.Utilities.Providers;

public interface ICultureInfoProvider
{
    CultureInfo CurrentCulture();
}

public sealed class InvariantCultureInfoProvider : ICultureInfoProvider
{
    public CultureInfo CurrentCulture() => CultureInfo.InvariantCulture;
}