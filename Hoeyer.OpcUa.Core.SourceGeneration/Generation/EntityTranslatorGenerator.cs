using Hoeyer.OpcUa.Core.SourceGeneration.Constants;
using Hoeyer.OpcUa.Core.SourceGeneration.Generation.IncrementalProvider;
using Hoeyer.OpcUa.Core.SourceGeneration.Models;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Hoeyer.OpcUa.Core.SourceGeneration.Generation;

[Generator]
public class EntityTranslatorGenerator : IIncrementalGenerator
{
    private static FullyQualifiedTypeName BrowseNameLookupType(string entityName) =>
        WellKnown.FullyQualifiedInterface.EntityBrowseNameCollection(entityName);

    private static readonly FullyQualifiedTypeName AggregatedConverter = WellKnown.VariantConverter.Aggregated;
    private const string BrowseNameLookup = "_browseNameLookup";
    private const string Converter = "_converter";

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var decoratedRecordsProvider = context
            .GetTypeContextForOpcEntities()
            .Select(static (typeContext, cancellationToken) => CreateSourceCode(typeContext))
            .Select((e, _) => e);

        context.RegisterImplementationSourceOutput(decoratedRecordsProvider.Collect(),
            (productionContext, compilations) =>
            {
                foreach (var (code, symbol) in compilations)
                {
                    productionContext.AddSource(symbol.Name + "Translator.g.cs", code.ToString());
                }
            });
    }

    private static (SourceCodeWriter writer, INamedTypeSymbol symbol) CreateSourceCode(TypeContext typeContext)
    {
        var symbol = typeContext.SemanticModel.GetDeclaredSymbol(typeContext.Node)!;
        var entityName = symbol.ToDisplayString(DisplayFormats.FullyQualifiedGenericWithGlobalPrefix);
        var className = symbol.Name + "Translator";

        IPropertySymbol[] properties = typeContext.Node.Members.OfType<PropertyDeclarationSyntax>()
            .Select(prop => typeContext.SemanticModel.GetDeclaredSymbol(prop))
            .Where(p => p is not null)
            .ToArray()!;

        var writer = new SourceCodeWriter();
        writer.WriteLine("namespace " + WellKnown.CoreServiceName + ".Generated {");

        writer.WriteLine("public sealed class " + className + " : " + WellKnown.FullyQualifiedInterface
            .EntityTranslatorInterfaceOf(entityName).WithGlobalPrefix);

        writer.WriteLine("{");
        writer.WriteLine("private readonly " + BrowseNameLookupType(entityName).WithGlobalPrefix +
                         $" {BrowseNameLookup};");
        writer.WriteLine("private readonly " + AggregatedConverter.WithGlobalPrefix + $" {Converter};");

        //Constructor
        WriteConstructor(writer, className, BrowseNameLookupType(entityName));


        writer.WriteLine("public void AssignToNode( " + entityName + " state, " +
                         WellKnown.FullyQualifiedInterface.IEntityNode.WithGlobalPrefix + " node)");

        writer.WriteLine("{");
        WriteAssignments(writer, properties, typeContext.SemanticModel);
        writer.WriteLine("}");

        writer.WriteLine("public void AssignToStructure(" + entityName + " state, ");
        writer.WriteLine($"{WellKnown.FullyQualifiedInterface.NodeStructure.WithGlobalPrefix} nodeStructure");
        writer.WriteLine(")");
        writer.WriteLine("{");
        WriteStateAssignment(writer, properties, typeContext.SemanticModel);
        writer.WriteLine("}");


        writer.WriteLine("public " + entityName + " Translate( " +
                         WellKnown.FullyQualifiedInterface.IEntityNode.WithGlobalPrefix + " state)");
        writer.WriteLine("{");

        WriteTranslations(writer, typeContext.Node, typeContext.SemanticModel);

        writer.WriteLine("return new " + entityName + "()");
        writer.WriteLine("{");
        foreach (var propertyName in typeContext.Node.Members.OfType<PropertyDeclarationSyntax>()
                     .Select(prop => prop.Identifier.Text))
        {
            writer.WriteLine(propertyName).Write(" = ").Write(propertyName).Write(",");
        }

        writer.WriteLine("};"); //return object creation end 
        writer.WriteLine("}");

        //Copy method begin
        WriteCopyMethod(typeContext, writer, entityName);
        writer.WriteLine("}");
        writer.WriteLine("}"); //class end

        return (writer, symbol);
    }

    private static void WriteConstructor(
        SourceCodeWriter writer,
        string className,
        FullyQualifiedTypeName browseNameLookupType)
    {
        writer.WriteLine("public " + className + "(");
        writer.WriteLine(browseNameLookupType.WithGlobalPrefix + " browseNameLookup,");
        writer.WriteLine(AggregatedConverter.WithGlobalPrefix + " converter");
        writer.Write(")");

        writer.WriteLine("{");
        writer.WriteLine($"this.{BrowseNameLookup} = browseNameLookup;");
        writer.WriteLine($"this.{Converter} = converter;");
        writer.WriteLine("}");
    }

    private static void WriteCopyMethod(TypeContext typeContext, SourceCodeWriter writer, string entityName)
    {
        var propertySymbols = typeContext.Node.Members.OfType<PropertyDeclarationSyntax>()
            .Select(e => (symbol: typeContext.SemanticModel.GetDeclaredSymbol(e)!, text: e.Identifier.Text));


        writer.WriteLine("public " + entityName + " Copy(" + entityName + " entity)");
        writer.WriteLine("{");
        writer.WriteLine("return new " + entityName + "()");
        writer.WriteLine("{");
        foreach (var (symbol, propertyName) in propertySymbols)
        {
            var listInterface = GetListInterface(symbol, typeContext.SemanticModel);
            if (listInterface != null)
            {
                var type = symbol.Type.ToDisplayString(DisplayFormats.FullyQualifiedGenericWithGlobalPrefix);
                writer.Write(propertyName).Write(" = new " + type + " ( ").Write("entity." + propertyName)
                    .Write(".ToList())").Write(",\n");
            }
            else
            {
                writer.Write(propertyName).Write(" = ").Write("entity." + propertyName).Write(",\n");
            }
        }

        writer.WriteLine("};");
        writer.WriteLine("}"); //copy method end
    }

    private static void WriteTranslations(SourceCodeWriter writer, TypeDeclarationSyntax symbol,
        SemanticModel semanticModel)
    {
        foreach (var member in symbol.Members.OfType<PropertyDeclarationSyntax>())
        {
            var name = member.Identifier.Text;
            var propertySymbol = semanticModel.GetDeclaredSymbol(member)!;
            var propertyTypeName =
                propertySymbol.Type.ToDisplayString(DisplayFormats.FullyQualifiedGenericWithGlobalPrefix);
            var getBrowseName = $"_browseNameLookup.PropertyToBrowseName[\"{name}\"]";
            var getPropertyValue = $"state.PropertyByBrowseName[{getBrowseName}].Value";

            var listInterface = GetListInterface(propertySymbol, semanticModel);
            if (listInterface is not null)
            {
                var genericArg = listInterface.TypeArguments.First().Name;
                writer.Write(
                    $"var {name} = {Converter}.ConvertToCollectionType<{genericArg}, List<{genericArg}>>({getPropertyValue});");
            }
            else
            {
                writer.Write($"var {name} = {Converter}.ConvertToType<{propertyTypeName}>({getPropertyValue});");
            }

            writer.WriteLine();
        }
    }


    private static void WriteAssignments(
        SourceCodeWriter writer,
        IPropertySymbol[] propertySymbols,
        SemanticModel model
    )
    {
        foreach (var property in propertySymbols)
        {
            string convert = "";
            var propName = property.Name;

            var genericListArg = GetListInterface(property, model)?.TypeArguments[0];
            if (genericListArg is not null)
            {
                convert = $"{Converter}.ConvertCollectionVariant(state." + propName + ")";
            }
            else
            {
                convert = $"{Converter}.ConvertToVariant(state." + propName + ")";
            }

            writer.WriteLine("node.PropertyByBrowseName[_browseNameLookup.PropertyToBrowseName[" + "\"" + propName +
                             "\"" +
                             "]].Value = " + convert + ";");
        }
    }

    private static void WriteStateAssignment(
        SourceCodeWriter writer,
        IPropertySymbol[] propertySymbols,
        SemanticModel model)
    {
        foreach (var property in propertySymbols)
        {
            var propName = property.Name;
            string convert = "";

            var listInterface = GetListInterface(property, model)?.TypeArguments[0];
            if (listInterface is not null)
            {
                convert = $"{Converter}.ConvertCollectionVariant(state." + propName + ")";
            }
            else
            {
                convert = $"{Converter}.ConvertToVariant(state." + propName + ")";
            }
            writer.WriteLine($"nodeStructure.Properties[\"{propName}\"].Value = {convert};");
        }
    }


    private static INamedTypeSymbol? GetListInterface(IPropertySymbol typeSymbol, SemanticModel model)
    {
        var ilistType = model.Compilation.GetTypeByMetadataName("System.Collections.Generic.IEnumerable`1");
        return typeSymbol.Type.AllInterfaces
            .FirstOrDefault(i => i.OriginalDefinition.Equals(ilistType, SymbolEqualityComparer.Default));
    }
}