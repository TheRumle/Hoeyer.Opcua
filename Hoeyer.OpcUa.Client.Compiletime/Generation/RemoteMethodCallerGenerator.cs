using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Hoeyer.OpcUa.Client.SourceGeneration.Constants;
using Hoeyer.OpcUa.Client.SourceGeneration.Extensions;
using Hoeyer.OpcUa.Client.SourceGeneration.Models;
using Microsoft.CodeAnalysis;

namespace Hoeyer.OpcUa.Client.SourceGeneration.Generation;

[Generator]
public sealed class RemoteMethodCallerGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var methodCallerImpls = context.GetAllOrdinaryTypesFromAllAssemblies()
            .Select((symbol, c) => (
                Interface: symbol,
                InterfaceMethods: symbol.GetMembers().OfType<IMethodSymbol>(),
                EntitySymbol: symbol.GetEntityFromGenericAttributeArgument()))
            .Where(symbolTuple => symbolTuple.EntitySymbol is not null)
            .Select((e, c) => CreateClassImplementation(e.Interface, e.InterfaceMethods, e.EntitySymbol!, c));

        context.RegisterSourceOutput(methodCallerImpls.Collect(), (sourceContext, dataModels) =>
        {
            foreach (var (identifier, code) in dataModels)
            {
                sourceContext.AddSource(identifier + ".g.cs", code);
            }
        });
    }


    private static (string Identifier, string SourceCode) CreateClassImplementation(INamedTypeSymbol interfaceSymbol,
        IEnumerable<IMethodSymbol> interfaceMethods, INamedTypeSymbol entitySymbol, CancellationToken cancellationToken)
    {
        if (cancellationToken.IsCancellationRequested)
        {
            return ("", "");
        }

        var interfaceName = new FullyQualifiedTypeName(interfaceSymbol);
        var relatedEntityName = new FullyQualifiedTypeName(entitySymbol);
        var classIdentifier = (interfaceSymbol.Name.StartsWith("I")
            ? interfaceSymbol.Name.Substring(1)
            : interfaceSymbol.Name) + "RemoteCaller";

        var builder = new SourceCodeWriter();
        var @namespace = interfaceSymbol.GetFullNamespace();
        builder.WriteLine("namespace " + @namespace + ".Generated");
        builder.WriteLine("{");
        builder.WriteLine("public sealed class " + classIdentifier);
        builder.Write(
            $"({WellKnown.FullyQualifiedInterface.MethodCallerType.WithGlobalPrefix}<{relatedEntityName.WithGlobalPrefix}> caller)");
        builder.WriteLine(" : " + interfaceName.WithGlobalPrefix);
        builder.WriteLine("{");

        //All members are tasks, always
        foreach (var method in interfaceMethods)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                return ("", "");
            }

            var returnType = method.ReturnType.ToDisplayString(DisplayFormats.FullyQualifiedGenericWithGlobalPrefix);
            var genericArgs = method.ReturnType is INamedTypeSymbol { Arity: 1 } named
                ? $"<{named.TypeArguments[0].ToDisplayString(DisplayFormats.FullyQualifiedGenericWithGlobalPrefix)}>"
                : "";

            List<string> paramStrings = method.Parameters.Select(e =>
                e.ToDisplayString(DisplayFormats.FullyQualifiedGenericWithGlobalPrefix)).ToList();
            var typeArgs = string.Join(",", paramStrings);
            var paramNames = string.Join(", ", method.Parameters.Select(e => e.Name));
            builder.WriteLine($"public {returnType} {method.Name}({typeArgs})");
            builder.WriteLine("{");
            builder.Write($"return caller.CallMethod{genericArgs}(").Write($"nameof({method.Name}),");
            builder.Write(" global::System.Threading.CancellationToken.None");
            if (paramStrings.Count > 0) builder.Write(", " + paramNames);

            builder.Write(");");
            builder.WriteLine();
            builder.WriteLine("}");
        }

        builder.WriteLine("}");
        builder.WriteLine("}");
        return (Identifier: classIdentifier, SourceCode: builder.ToString());
    }
}