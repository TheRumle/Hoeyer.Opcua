using System.Collections.Generic;
using System.Linq;

namespace Hoeyer.Common.Extensions.Collection;

public static class SetExtensions
{
    public static bool IntersectsAny<T>(this ISet<T> first, ISet<T> second) => first.Any(second.Contains);
    public static bool AreDisjunct<T>(this ISet<T> first, ISet<T> second) => !first.Any(second.Contains);
    public static bool IsEmpty<T>(this IEnumerable<T> first) => !first.Any();

    public static bool AreSymmetric<T>(this ISet<T> first, ISet<T> second) =>
        first.Except(second).IsEmpty() && second.Except(first).IsEmpty();
}