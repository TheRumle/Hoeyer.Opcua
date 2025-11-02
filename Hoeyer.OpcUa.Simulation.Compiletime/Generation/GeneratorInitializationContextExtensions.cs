using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Microsoft.CodeAnalysis;

namespace Hoeyer.OpcUa.Simulation.SourceGeneration.Generation;

public static class GeneratorInitializationContextExtensions
{
    public static bool IsSystemNamespace(this INamespaceSymbol ns)
    {
        var name = ns.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
        return name.StartsWith("System", StringComparison.Ordinal) ||
               name.StartsWith("Microsoft", StringComparison.Ordinal);
    }

    public static IEnumerable<INamedTypeSymbol> VisitNamespace(
        this INamespaceSymbol ns,
        CancellationToken cancellationToken,
        HashSet<INamespaceOrTypeSymbol>? visited = null
    )
    {
        var alreadyVisited = visited ?? new HashSet<INamespaceOrTypeSymbol>(SymbolEqualityComparer.Default);

        if (ns.IsSystemNamespace() || alreadyVisited.Contains(ns) ||
            cancellationToken.IsCancellationRequested)
        {
            return [];
        }

        var types = ns.GetMembers().SelectMany(member => member switch
        {
            INamedTypeSymbol namedTypeSymbol => [namedTypeSymbol, ..GetNestedTypes(namedTypeSymbol)],
            INamespaceSymbol namespaceSymbol => namespaceSymbol.VisitNamespace(cancellationToken, alreadyVisited),
            var _ => []
        });

        alreadyVisited.Add(ns);
        return types;
    }

    private static IEnumerable<INamedTypeSymbol> GetNestedTypes(this INamedTypeSymbol type)
    {
        foreach (var nested in type.GetTypeMembers())
        {
            yield return nested;
            foreach (var deeper in nested.GetNestedTypes())
            {
                yield return deeper;
            }
        }
    }

    public static INamedTypeSymbol? GetEntityFromGenericAttributeArgument(this INamedTypeSymbol symbol)
    {
        return symbol
            .GetOpcMethodAttribute()?
            .AttributeClass?
            .TypeArguments
            .Where(type => type.IsAnnotatedAsOpcUaEntity())
            .OfType<INamedTypeSymbol>()
            .FirstOrDefault();
    }

    public static IncrementalValuesProvider<INamedTypeSymbol> GetAllOrdinaryTypesFromAllAssemblies(
        this IncrementalGeneratorInitializationContext context)
    {
        return context.CompilationProvider.SelectMany((compilation, c) =>
        {
            var fromCurrentAssembly = compilation.Assembly.GlobalNamespace.VisitNamespace(c);
            var topLevelTypes = compilation.Assembly.GetForwardedTypes();
            var fromReferencedAssemblies = compilation.References.SelectMany(reference =>
            {
                var asmSymbol = compilation.GetAssemblyOrModuleSymbol(reference) as IAssemblySymbol;
                if (asmSymbol == null || c.IsCancellationRequested)
                {
                    return [];
                }

                return asmSymbol.GlobalNamespace.VisitNamespace(c);
            });

            return fromCurrentAssembly
                .Union(topLevelTypes, SymbolEqualityComparer.Default)
                .Union(fromReferencedAssemblies, SymbolEqualityComparer.Default)
                .OfType<INamedTypeSymbol>()
                .Where(static type => !type.ContainingNamespace.IsSystemNamespace())
                .Where(static type => type is
                {
                    IsScriptClass: false,
                    IsImplicitClass: false,
                    IsAnonymousType: false,
                    IsStatic: false,
                    IsExtern: false,
                    IsFileLocal: false,
                    IsNativeIntegerType: false,
                    IsUnmanagedType: false,
                    IsNamespace: false,
                    EnumUnderlyingType: null
                } && !IsModuleType(type))
                .Where(static type => !IsCompilerGenerated(type))
                .Where(static type => !type.ContainingNamespace.ToDisplayString().StartsWith("Microsoft"))
                .Where(static type => !type.ContainingNamespace.ToDisplayString().StartsWith("Org.BouncyCastle"))
                .Where(static type => !type.ContainingNamespace.ToDisplayString().StartsWith("System."))
                .OrderBy(static type =>
                {
                    var ns = type.ContainingNamespace?.ToDisplayString() ?? "";

                    // 0 = exact or starts with Playground
                    // 1 = contains Playground deeper in hierarchy
                    // 2 = anything else
                    if (ns.Equals("Playground", StringComparison.Ordinal) ||
                        ns.StartsWith("Playground.", StringComparison.Ordinal))
                    {
                        return 0;
                    }

                    return 1;
                })
                .ThenBy(static type => type.ContainingNamespace?.ToDisplayString(), StringComparer.Ordinal)
                .ThenBy(static type => type.Name, StringComparer.Ordinal);
        });
    }

    private static bool IsModuleType(INamedTypeSymbol type) =>
        type.Name is "<PrivateImplementationDetails>" or "<Module>"
        && type.ContainingNamespace.IsGlobalNamespace;

    public static bool IsCompilerGenerated(this INamedTypeSymbol type)
    {
        // Attribute check (most reliable)
        if (type.GetAttributes().Any(a =>
                a.AttributeClass?.ToDisplayString() == "System.Runtime.CompilerServices.CompilerGeneratedAttribute"))
        {
            return true;
        }

        // Name pattern check (faster)
        if (type.Name.StartsWith("<>", StringComparison.Ordinal) ||
            type.Name.Contains("__DisplayClass") ||
            type.Name.Contains("__Generated"))
        {
            return true;
        }

        return false;
    }
}