using System;
using System.Reflection;

namespace Hoeyer.OpcUa.Core.Application.NodeStructure;

public static class BrowseNameExtensions
{
    public static string GetBrowseNameOrDefault(this Type type, string fallback) =>
        type.GetCustomAttribute<BrowseNameAttribute>().GetBrowseNameOrDefault(fallback);

    public static string GetBrowseNameOrDefault(this BrowseNameAttribute? attr, string fallback) =>
        attr?.BrowseName ?? fallback;

    public static string GetBrowseNameOrDefault(this MemberInfo prop, string fallback) =>
        prop.GetCustomAttribute<BrowseNameAttribute>().GetBrowseNameOrDefault(fallback);
}