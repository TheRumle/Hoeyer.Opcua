using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Reflection;
using Assembly = System.Reflection.Assembly;

namespace Hoeyer.Common.Reflection;

public class AssemblyVisitor
{
    private Type[]? _types;
    public static AssemblyVisitor Singleton { get; } = new();

    private ConcurrentDictionary<Assembly, Type[]> Visited { get; } = new();
    private ConcurrentQueue<Assembly> Queue { get; } = new();

    public Type[] TypesFromVisitedAssemblies()
    {
        _types ??= VisitAllAssemblies();
        return _types;
    }

    private Type[] VisitAllAssemblies()
    {
        var assemblies = AppDomain.CurrentDomain.GetAssemblies();
        foreach (var assembly in assemblies)
        {
            Queue.Enqueue(assembly);
        }

        while (Queue.TryDequeue(out var assembly))
        {
            if (Visited.ContainsKey(assembly))
            {
                continue;
            }

            Visited[assembly] = GetTypesSafely(assembly);
            EnqueueNeighbours(assembly);
        }

        return Visited.Values.SelectMany(e => e).ToArray();
    }

    private static Type[] GetTypesSafely(Assembly assembly)
    {
        try
        {
            return assembly.GetTypes();
        }
        catch (ReflectionTypeLoadException ex)
        {
            return ex.Types.Where(t => t != null).ToArray()!;
        }
        catch
        {
            return [];
        }
    }

    private void EnqueueNeighbours(Assembly assembly)
    {
        try
        {
            var ass = Assembly.Load(assembly.GetName());
            Queue.Enqueue(ass);
        }
        catch
        {
            // ignored
        }
    }
}