using System.Collections.Immutable;
using System.Text;
using Hoeyer.OpcUa.Client.SourceGeneration.Constants;
using Hoeyer.OpcUa.Client.SourceGeneration.Extensions;
using Hoeyer.OpcUa.Client.SourceGeneration.Models;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Hoeyer.OpcUa.Client.SourceGeneration.Generation;

[Generator]
public sealed class RemoteMethodCallerGenerator : IIncrementalGenerator
{
    private const string CONVERTER_FIELD = "converter";

    private record struct InterfaceInformation(
        SemanticModel SemanticModel,
        INamedTypeSymbol Interface,
        IEnumerable<IMethodSymbol> InterfaceMethods,
        INamedTypeSymbol EntitySymbol
    );

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var methodCallerImpls = context.SyntaxProvider.ForAttributeWithMetadataName(
                "Hoeyer.OpcUa.Core.OpcUaEntityMethodsAttribute`1",
                static (targetNode, _) => targetNode is InterfaceDeclarationSyntax,
                static (attributeSyntaxContext, cancellationToken) => (
                    Interface: attributeSyntaxContext.TargetSymbol as INamedTypeSymbol,
                    attributeSyntaxContext.SemanticModel)
            )
            .Where(static e => e.Interface is not null)
            .Select(static (tuple, cancel) =>
            {
                if (cancel.IsCancellationRequested) return default;
                var (@interface, semanticModel) = tuple;
                var interfaceMethods = @interface!.GetMembers().OfType<IMethodSymbol>();
                var entityArg = @interface.GetEntityFromGenericAttributeArgument();

                return new InterfaceInformation(
                    semanticModel,
                    @interface,
                    interfaceMethods,
                    entityArg!
                );
            })
            .Where(static symbolTuple => symbolTuple.EntitySymbol is not null)
            .Select(CreateClassImplementation);

        context.RegisterSourceOutput(methodCallerImpls.Collect(), (sourceContext, dataModels) =>
        {
            foreach (var (identifier, code) in dataModels)
            {
                sourceContext.AddSource(identifier + ".g.cs", code);
            }
        });
    }

    private static (string Identifier, string SourceCode) CreateClassImplementation(
        InterfaceInformation information,
        CancellationToken cancellationToken)
    {
        if (cancellationToken.IsCancellationRequested)
        {
            return ("", "");
        }

        var (semanticModel, interfaceSymbol, interfaceMethods, entitySymbol) = information;

        var interfaceName = new FullyQualifiedTypeName(interfaceSymbol);
        var relatedEntityName = new FullyQualifiedTypeName(entitySymbol);
        var classIdentifier = (interfaceSymbol.Name.StartsWith("I")
            ? interfaceSymbol.Name.Substring(1)
            : interfaceSymbol.Name) + "RemoteCaller";

        var builder = new SourceCodeWriter();
        var @namespace = interfaceSymbol.GetFullNamespace();
        builder.WriteLine("namespace " + @namespace + ".Generated");
        builder.WriteLine("{");
        WriteClassHeader(builder, classIdentifier, relatedEntityName, interfaceName);
        builder.WriteLine("{");

        var methodBodies = interfaceMethods.TakeWhile(f => !cancellationToken.IsCancellationRequested)
            .Select(symbol => WriteMethodCall(symbol, semanticModel));
        
        builder.Write(string.Join("\n", methodBodies));
        
        builder.WriteLine("}");
        builder.WriteLine("}");
        return (Identifier: classIdentifier, SourceCode: builder.ToString());
    }

    private static string WriteMethodCall(IMethodSymbol method, SemanticModel model)
    {
        StringBuilder builder = new();
        var returnType = method.ReturnType.ToDisplayString(DisplayFormats.FullyQualifiedGenericWithGlobalPrefix);
        var genericArgs = method.ReturnType is INamedTypeSymbol { Arity: 1 } named
            ? $"<{named.TypeArguments[0].ToDisplayString(DisplayFormats.FullyQualifiedGenericWithGlobalPrefix)}>"
            : "";

        var parameters = method.Parameters
            .Select(e => e.ToDisplayString(DisplayFormats.FullyQualifiedGenericWithGlobalPrefix))
            .ToList();

        var variantConvertedParams = ConvertedParams(method.Parameters, model).ToArray();

        builder.Append($$"""
                             public {{returnType}} {{method.Name}}({{string.Join(", ", parameters)}})
                             {
                                 return caller.CallMethod{{genericArgs}}
                                 (
                                     nameof({{method.Name}}),
                                     global::System.Threading.CancellationToken.None{{(variantConvertedParams.Length > 0 ? "," : "")}}
                                     {{string.Join(",\n                             ", variantConvertedParams)}}
                                 );
                             }
                         """);

        return builder.ToString();
    }

    private static void WriteClassHeader(SourceCodeWriter builder, string classIdentifier,
        FullyQualifiedTypeName relatedEntityName, FullyQualifiedTypeName interfaceName)
    {
        builder.WriteLine("public sealed class " + classIdentifier);
        builder.Write(
            $"""
             (
             {WellKnown.VariantConverter.Aggregated.WithGlobalPrefix} {CONVERTER_FIELD},
             {WellKnown.FullyQualifiedInterface.MethodCallerType.WithGlobalPrefix}<{relatedEntityName.WithGlobalPrefix}> caller
             )
             """);
        builder.WriteLine(" : " + interfaceName.WithGlobalPrefix);
    }

    private static IEnumerable<string> ConvertedParams(
        ImmutableArray<IParameterSymbol> parameterSymbols,
        SemanticModel model
    )
    {
        foreach (var parameterSymbol in parameterSymbols)
        {
            var listInterface = GetListInterface(parameterSymbol.Type, model);
            if (listInterface is not null)
            {
                yield return $"{CONVERTER_FIELD}.ConvertCollectionVariant(" + parameterSymbol.Name + ")";
            }
            else
            {
                yield return $"{CONVERTER_FIELD}.ConvertToVariant(" + parameterSymbol.Name + ")";
            }
        }
    }

    private static INamedTypeSymbol? GetListInterface(ITypeSymbol typeSymbol, SemanticModel model)
    {
        var ilistType = model.Compilation.GetTypeByMetadataName("System.Collections.Generic.IList`1");
        return typeSymbol.AllInterfaces
            .FirstOrDefault(i => i.OriginalDefinition.Equals(ilistType, SymbolEqualityComparer.Default));
    }
}