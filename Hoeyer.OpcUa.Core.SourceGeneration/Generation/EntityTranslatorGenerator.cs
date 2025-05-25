using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Hoeyer.OpcUa.Core.SourceGeneration.Constants;
using Hoeyer.OpcUa.Core.SourceGeneration.Generation.IncrementalProvider;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Hoeyer.OpcUa.Core.SourceGeneration.Generation;

[Generator]
public class EntityTranslatorGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var decoratedRecordsProvider = context
            .GetTypeContextForOpcEntities()
            .Select(async (typeContext, cancellationToken) =>
                await CreateCompilationUnit(typeContext, cancellationToken))
            .Select((e, _) => e.Result);

        context.RegisterImplementationSourceOutput(decoratedRecordsProvider.Collect(),
            (productionContext, compilations) =>
            {
                foreach (var c in compilations) c.AddToContext(productionContext);
            });
    }

    private static async Task<GeneratedClass> CreateCompilationUnit(TypeContext typeContext,
        CancellationToken cancellationToken)
    {
        var classString = GetClassDefinitionString(typeContext.Node.Identifier.Text,
            typeContext.Node.Members.OfType<PropertyDeclarationSyntax>().ToList(), typeContext.SemanticModel);

        var classDcl = (await CSharpSyntaxTree
                .ParseText(classString)
                .GetRootAsync(cancellationToken))
            .DescendantNodes()
            .OfType<ClassDeclarationSyntax>()
            .FirstOrDefault()!;


        var compilation = await typeContext.CreateCompilationUnitFor(classDcl, Locations.Utilities,
            cancellationToken);

        return new GeneratedClass(compilation, classDcl, typeContext.Node);
    }

    private static string GetClassDefinitionString(string entityName, List<PropertyDeclarationSyntax> properties,
        SemanticModel model)
    {
        var assignments = string.Join("\n\n", properties.Select(p => GetNodeAssignmentStatements(p, model)));
        var translateStatements = string.Join("\n\n", GetTranslationMethodCalls(properties, model));

        var fieldAssignments =
            string.Join("\n\n", properties.Select(e => $"{e.Identifier.Text} = {e.Identifier.Text},"));

        return $$"""
                 [OpcUaEntityServiceAttribute(typeof(IEntityTranslator<>))]
                 public sealed class {{entityName}}Translator :IEntityTranslator<{{entityName}}>
                 {
                     public void AssignToNode({{entityName}} state, IEntityNode node)
                     {
                        {{assignments}}
                     }
                     
                     public {{entityName}} Translate(IEntityNode state)
                     {
                         {{translateStatements}}
                         
                         return new {{entityName}}()
                         {
                           {{fieldAssignments}}
                         };
                     }
                 }
                 """;
    }

    private static string GetNodeAssignmentStatements(PropertyDeclarationSyntax property, SemanticModel model)
    {
        var propName = property.Identifier.Text;

        var toArrayExpr = model.GetTypeInfo(property.Type)!.Type is INamedTypeSymbol { Arity: 1, IsGenericType: true }
            ? $".{nameof(Enumerable.ToList)}()"
            : "";

        return $$"""node.PropertyByBrowseName["{{propName}}"].Value = state.{{propName}}{{toArrayExpr}};""";
    }

    private static string TranslateCollection(PropertyDeclarationSyntax property, SemanticModel model)
    {
        var namedTypeSymbol = (model.GetTypeInfo(property.Type)!.Type as INamedTypeSymbol)!;
        var propName = property.Identifier.Text;
        var typeSyntax = property.Type;
        var collectionType = namedTypeSymbol.TypeArguments.First();
        var translationMethod =
            $"DataTypeToTranslator.TranslateToCollection<{typeSyntax.ToString()}, {collectionType}>(state, \"{propName}\")";

        return $"{typeSyntax.ToString()} {property.Identifier.Text} = {translationMethod};";
    }

    private static string TranslateSingletonValue(PropertyDeclarationSyntax property)
    {
        var propName = property.Identifier.Text;
        var typeSyntax = property.Type;
        return $"""
                {typeSyntax.ToString()} {property.Identifier.Text} = DataTypeToTranslator.TranslateToSingle<{typeSyntax.ToString()}>(state, "{propName}");
                """;
    }

    private static IEnumerable<string> GetTranslationMethodCalls(
        List<PropertyDeclarationSyntax> properties,
        SemanticModel model)
    {
        var collectionProperties = properties.Where(property =>
        {
            var typeInfo = model.GetTypeInfo(property.Type).Type;
            return typeInfo is INamedTypeSymbol
            {
                Arity: 1, IsGenericType: true
            };
        }).ToList();

        var singletonAssignments = properties.Except(collectionProperties).Select(TranslateSingletonValue);
        var collectionAssignments = collectionProperties.Select(property => TranslateCollection(property, model));
        return [..singletonAssignments, ..collectionAssignments];
    }
}