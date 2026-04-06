using Hoeyer.Common.Reflection;

namespace Hoeyer.OpcUa.Test.Adapter;

internal static class AssignFrameworkAdapter
{
    [Before(TestDiscovery)]
    public static void ScanForAdapter()
    {
        var type = AssemblyVisitor.Singleton
            .TypesFromVisitedAssemblies()
            .FirstOrDefault(t =>
                typeof(ITestFrameworkAdapter).IsAssignableFrom(t) &&
                !t.IsAbstract &&
                !t.IsInterface);

        if (type == null)
        {
            return;
        }

        var constructor = type.GetConstructor(Type.EmptyTypes);
        if (constructor == null)
        {
            return;
        }

        var adapter = (ITestFrameworkAdapter?)Activator.CreateInstance(type)!;
        FrameworkAdapter.Assign(adapter);
    }
}