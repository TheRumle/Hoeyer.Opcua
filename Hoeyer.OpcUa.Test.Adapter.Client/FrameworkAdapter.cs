using Hoeyer.Common.Reflection;
using Hoeyer.OpcUa.Test.Adapter.Client.Api;

namespace Hoeyer.OpcUa.Test.Adapter.Client;

public static class FrameworkAdapter
{
    public static readonly Lazy<ITestFrameworkAdapter> AdapterInstance = new(FindAndCreateAdapter);

    private static readonly Type AdapterType = AssemblyVisitor.Singleton
        .TypesFromVisitedAssemblies()
        .FirstOrDefault(t =>
            t is { IsAbstract: false, IsInterface: false } && t.IsAssignableTo(typeof(ITestFrameworkAdapter)))!;

    private static ITestFrameworkAdapter FindAndCreateAdapter()
    {
        if (AdapterType == null)
        {
            throw new ArgumentException(
                $"No test framework adapter implementing {nameof(ITestFrameworkAdapter)} could be found. Implement an adapter in a consuming assembly.");
        }

        var emptyCtor = AdapterType.GetConstructor(Type.EmptyTypes);
        if (emptyCtor == null)
        {
            throw new ArgumentException(
                $"{AdapterType.FullName} must have a public parameterless constructor");
        }

        return (emptyCtor.Invoke([]) as ITestFrameworkAdapter)!;
    }
}