using System.Globalization;

namespace Hoeyer.Common.Utilities.Providers;

public sealed class InvariantCultureInfoProvider : ICultureInfoProvider
{
    public CultureInfo CurrentCulture() => CultureInfo.InvariantCulture;
}