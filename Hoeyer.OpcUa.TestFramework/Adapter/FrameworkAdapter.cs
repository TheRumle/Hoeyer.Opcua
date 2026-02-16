using Hoeyer.Common.Reflection;

namespace Hoeyer.OpcUa.Test.Adapter;

public static class FrameworkAdapter
{
    private static ITestFrameworkAdapter? _adapterInstance = ScanForAdapter();

    public static ITestFrameworkAdapter AdapterInstance =>
        _adapterInstance ?? throw new NoFrameworkAdapterException(
            $"No adapter instance configured. Did you remember to use {nameof(AdaptionMethods.AddAdaptedTest)}?");

    private static ITestFrameworkAdapter? ScanForAdapter()
    {
        var type = AssemblyVisitor.Singleton
            .TypesFromVisitedAssemblies()
            .FirstOrDefault(t =>
                typeof(ITestFrameworkAdapter).IsAssignableFrom(t) &&
                !t.IsAbstract &&
                !t.IsInterface);

        if (type == null)
        {
            return null;
        }

        var constructor = type.GetConstructor(Type.EmptyTypes);
        if (constructor == null)
        {
            return null;
        }

        return (ITestFrameworkAdapter?)Activator.CreateInstance(type);
    }


    public static void Assign(ITestFrameworkAdapter adapter) => _adapterInstance = adapter;
}