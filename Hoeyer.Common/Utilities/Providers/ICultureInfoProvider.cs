using System.Globalization;

namespace Hoeyer.Common.Utilities.Providers;

public interface ICultureInfoProvider
{
    CultureInfo CurrentCulture();
}