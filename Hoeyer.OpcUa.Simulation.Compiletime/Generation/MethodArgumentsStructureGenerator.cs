using Hoeyer.OpcUa.Simulation.SourceGeneration.Constants;
using Microsoft.CodeAnalysis;

namespace Hoeyer.OpcUa.Simulation.SourceGeneration.Generation;

[Generator]
public sealed class MethodArgumentsStructureGenerator : IIncrementalGenerator
{
    private const string CONVERTER = "_converter";

    /// <inheritdoc />
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        //find all interfaces annotated as EntityMethod
        var sourceCodeContainers = context
            .GetEntityMethodSyntaxes()
            .Select(CreateSourceCode);

        context.RegisterSourceOutput(sourceCodeContainers.Collect(), (sourceContext, methodModelsList) =>
        {
            foreach (var (className, code) in methodModelsList.SelectMany(e => e))
            {
                sourceContext.AddSource(className + ".g.cs", code);
            }
        });
    }

    private static (string className, string definition)[] CreateSourceCode(
        TypeContext symbol,
        CancellationToken cancellationToken)
    {
        var interfaceSymbol = (INamedTypeSymbol)symbol.SemanticModel
            .GetDeclaredSymbol(symbol.Node, cancellationToken)!;

        var entity = interfaceSymbol
            .GetOpcMethodAttribute()!
            .AttributeClass!
            .TypeArguments[0];

        var methods = interfaceSymbol.GetMembers()
            .OfType<IMethodSymbol>()
            .ToArray();

        var records = methods
            .AsParallel()
            .WithCancellation(cancellationToken)
            .Select(method => CreateArgumentRecord(interfaceSymbol, method, entity));

        var translators = methods
            .AsParallel()
            .WithCancellation(cancellationToken)
            .Select(method => CreateTranslator(
                interfaceSymbol,
                method,
                symbol.SemanticModel));

        return [.. records, .. translators];
    }

    private static (string className, string definition)  CreateTranslator(
        INamedTypeSymbol interfaceSymbol,
        IMethodSymbol method,
        SemanticModel model)
    {
        var argumentRecord = CreateArgRecordName(method);
        var @namespace = interfaceSymbol.GetFullNamespace();
        var className = $"{argumentRecord}Translator";

        var classDefinition = $$"""
                 namespace {{@namespace}}.Generated
                 {
                     using Opc.Ua;
                     public sealed class {{className}} : {{WellKnown.FullyQualifiedInterface.IObjectArgsToTypedArgs(argumentRecord).WithGlobalPrefix}}
                     {
                       private readonly {{WellKnown.VariantConverter.Aggregated.WithGlobalPrefix}} {{CONVERTER}};
                       
                       //constructor
                       public {{className}}(
                          {{WellKnown.VariantConverter.Aggregated.WithGlobalPrefix}} {{CONVERTER}}
                       ){
                          this.{{CONVERTER}} = {{CONVERTER}};
                       }
                     
                       public {{argumentRecord}}? Map(IList<Variant> args){
                            return new {{argumentRecord}}({{ArgRecordCreation(method, model)}});
                       }
                       
                       public Variant[] Map({{argumentRecord}} argRecord)
                       {
                           return new Variant[]
                           {
                               {{ArgVariantCreation(method)}}
                           };
                       }
                     }
                 }
                 """;

        return (className, classDefinition);
    }

    private static string ArgRecordCreation(IMethodSymbol method, SemanticModel model)
    {
        var convertionStatements = method.Parameters.Select((parameter, index) =>
        {
            var listInterface = GetListInterface(parameter, model);
            var argumentValue = $"args[{index}]";
            if (listInterface is not null)
            {
                var genericArg = listInterface.TypeArguments.First().Name;
                return $"{CONVERTER}.ConvertToCollectionType<{genericArg}, List<{genericArg}>>({argumentValue})";
            }

            var parameterType = parameter.Type.ToDisplayString(DisplayFormats.FullyQualifiedGenericWithGlobalPrefix);
            return $"{CONVERTER}.ConvertToType<{parameterType}>({argumentValue})";
        }).ToList();

        return String.Join(", ", convertionStatements);
    }
    
    private static string ArgVariantCreation(IMethodSymbol method)
    {
        var convertionStatements = method.Parameters.Select((parameter, index) 
            => $"{CONVERTER}.ConvertToVariant(argRecord.{FirstLetterToUpper(parameter.Name)})").ToList();
        return string.Join(", ", convertionStatements);
    }


    private static INamedTypeSymbol? GetListInterface(IParameterSymbol typeSymbol, SemanticModel model)
    {
        var ilistType = model.Compilation.GetTypeByMetadataName("System.Collections.Generic.IList`1");
        return typeSymbol.Type.AllInterfaces
            .FirstOrDefault(i => i.OriginalDefinition.Equals(ilistType, SymbolEqualityComparer.Default));
    }

    private static (string className, string definition) CreateArgumentRecord(
        INamedTypeSymbol interfaceSymbol,
        IMethodSymbol methodSymbol,
        ITypeSymbol entityType
    )
    {
        var @interface = interfaceSymbol.ToDisplayString(DisplayFormats.FullyQualifiedGenericWithGlobalPrefix);
        var @namespace = interfaceSymbol.GetFullNamespace();
        var entity = entityType.ToFullyQualifiedTypeName();

        var paramList = methodSymbol
            .Parameters
            .Select((param) => (
                Type: param.Type.ToDisplayString(DisplayFormats.FullyQualifiedGenericWithGlobalPrefix),
                UpperName: FirstLetterToUpper(param.Name)))
            .ToList();

        var className = CreateArgRecordName(methodSymbol);
        var ctorArgs = string.Join(", ", paramList.Select(e => $"{e.Type} {e.UpperName}"));
        var methodArgsRecordDcl = $$"""
                                    namespace {{@namespace}}.Generated{
                                       [{{WellKnown.FullyQualifiedAttribute.OpcMethodArgumentsAttribute.WithGlobalPrefix}}
                                       <{{entity}}, {{@interface}}>("{{methodSymbol.Name}}")]
                                       public sealed record {{className}}({{ctorArgs}}) 
                                           : {{WellKnown.FullyQualifiedInterface.IArgsContainer.WithGlobalPrefix}}{}
                                    }
                                    """;

        return (className, methodArgsRecordDcl);
    }

    private static string CreateArgRecordName(IMethodSymbol methodSymbol)
    {
        var className = methodSymbol.Name + "Args";
        return className;
    }


    private static string FirstLetterToUpper(string s) => char.ToUpper(s[0]) + s.Substring(1);
}