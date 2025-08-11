using System.Linq;
using Hoeyer.OpcUa.Core.SourceGeneration.Constants;
using Hoeyer.OpcUa.Core.SourceGeneration.Generation.IncrementalProvider;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Hoeyer.OpcUa.Core.SourceGeneration.Generation;

[Generator]
public class AgentTranslatorGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var decoratedRecordsProvider = context
            .GetTypeContextForOpcEntities()
            .Select(static (typeContext, cancellationToken) => CreateSourceCode(typeContext))
            .Select((e, _) => e);

        context.RegisterImplementationSourceOutput(decoratedRecordsProvider.Collect(),
            (productionContext, compilations) =>
            {
                foreach ((SourceCodeWriter code, INamedTypeSymbol symbol) in compilations)
                {
                    productionContext.AddSource(symbol.Name + "Translator.g.cs", code.ToString());
                }
            });
    }

    private static (SourceCodeWriter writer, INamedTypeSymbol symbol) CreateSourceCode(TypeContext typeContext)
    {
        INamedTypeSymbol symbol = typeContext.SemanticModel.GetDeclaredSymbol(typeContext.Node)!;
        var agentName = symbol.ToDisplayString(DisplayFormats.FullyQualifiedGenericWithGlobalPrefix);

        var writer = new SourceCodeWriter();
        writer.WriteLine("namespace " + WellKnown.CoreServiceName + ".Generated {");
        writer.Write("[")
            .Write(WellKnown.FullyQualifiedAttribute.OpcUaAgentServiceAttribute.WithGlobalPrefix)
            .Write("(typeof(").Write(WellKnown.FullyQualifiedInterface.AgentTranslatorInterfaceOf().WithGlobalPrefix)
            .Write("))").Write("]");

        writer.WriteLine("public sealed class " + symbol.Name + "Translator" + " : " + WellKnown.FullyQualifiedInterface
            .AgentTranslatorInterfaceOf(agentName).WithGlobalPrefix);
        writer.WriteLine("{");

        writer.WriteLine("public void AssignToNode( " + agentName + " state, " +
                         WellKnown.FullyQualifiedInterface.IAgent.WithGlobalPrefix + " node)");

        writer.WriteLine("{");
        WriteAssignments(writer, typeContext.Node, typeContext.SemanticModel);
        writer.WriteLine("}");

        writer.WriteLine("public void AssignToStructure(" + agentName + " state, ");
        writer.WriteLine("global::System.Action<string, object> assignmentAction");
        writer.WriteLine(")");
        writer.WriteLine("{");
        WriteStateAssignment(writer, typeContext.Node, typeContext.SemanticModel);
        writer.WriteLine("}");


        writer.WriteLine("public " + agentName + " Translate( " +
                         WellKnown.FullyQualifiedInterface.IAgent.WithGlobalPrefix + " state)");
        writer.WriteLine("{");

        WriteTranslations(writer, typeContext.Node, typeContext.SemanticModel);

        writer.WriteLine("return new " + agentName + "()");
        writer.WriteLine("{");
        foreach (var propertyName in typeContext.Node.Members.OfType<PropertyDeclarationSyntax>()
                     .Select(prop => prop.Identifier.Text))
        {
            writer.WriteLine(propertyName).Write(" = ").Write(propertyName).Write(",");
        }

        writer.WriteLine("};"); //return object creation end 
        writer.WriteLine("}");

        //Copy method begin
        WriteCopyMethod(typeContext, writer, agentName);
        writer.WriteLine("}");
        writer.WriteLine("}"); //class end

        return (writer, symbol);
    }

    private static void WriteCopyMethod(TypeContext typeContext, SourceCodeWriter writer, string agentName)
    {
        var propertySymbols = typeContext.Node.Members.OfType<PropertyDeclarationSyntax>()
            .Select(e => (symbol: typeContext.SemanticModel.GetDeclaredSymbol(e)!, text: e.Identifier.Text));


        writer.WriteLine("public " + agentName + " Copy(" + agentName + " agent)");
        writer.WriteLine("{");
        writer.WriteLine("return new " + agentName + "()");
        writer.WriteLine("{");
        foreach (var (symbol, propertyName) in propertySymbols)
        {
            var listInterface = GetListInterface(symbol, typeContext.SemanticModel);
            if (listInterface != null)
            {
                var type = symbol.Type.ToDisplayString(DisplayFormats.FullyQualifiedGenericWithGlobalPrefix);
                writer.Write(propertyName).Write(" = new " + type + " ( ").Write("agent." + propertyName)
                    .Write(".ToList())").Write(",\n");
            }
            else
            {
                writer.Write(propertyName).Write(" = ").Write("agent." + propertyName).Write(",\n");
            }
        }

        writer.WriteLine("};");
        writer.WriteLine("}"); //copy method end
    }

    private static void WriteTranslations(SourceCodeWriter writer, TypeDeclarationSyntax symbol,
        SemanticModel semanticModel)
    {
        foreach (PropertyDeclarationSyntax? member in symbol.Members.OfType<PropertyDeclarationSyntax>())
        {
            var name = member.Identifier.Text;
            IPropertySymbol propertySymbol = semanticModel.GetDeclaredSymbol(member)!;
            var type = propertySymbol.Type.ToDisplayString(DisplayFormats.FullyQualifiedGenericWithGlobalPrefix);


            INamedTypeSymbol? listInterface = GetListInterface(propertySymbol, semanticModel);
            writer.WriteLine("var " + name + " = " +
                             WellKnown.FullyQualifiedInterface.DataTypeTranslator.WithGlobalPrefix + ".");
            if (listInterface is not null)
            {
                var genericArg = listInterface.TypeArguments.First()!.Name;
                writer.Write("TranslateToCollection<").Write(type)
                    .Write(", ").Write(genericArg).Write(">(").Write("state, ").Write("\"").Write(name).Write("\");");
            }
            else
            {
                writer.Write("TranslateToSingle<").Write(type).Write(">").Write("(state, ").Write("\"").Write(name)
                    .Write("\");");
            }

            writer.WriteLine();
        }
    }


    private static void WriteAssignments(SourceCodeWriter writer, TypeDeclarationSyntax syntax, SemanticModel model)
    {
        foreach (PropertyDeclarationSyntax? property in syntax.Members.OfType<PropertyDeclarationSyntax>())
        {
            var propName = property.Identifier.Text;
            INamedTypeSymbol? listInterface = GetListInterface(model.GetDeclaredSymbol(property)!, model);

            var toList = listInterface is not null
                ? $".{nameof(Enumerable.ToArray)}()"
                : "";

            writer.WriteLine("node.PropertyByBrowseName[\"" + propName + "\"].Value = state." + propName + toList +
                             ";");
        }
    }

    private static void WriteStateAssignment(SourceCodeWriter writer,
        TypeDeclarationSyntax syntax, SemanticModel model)
    {
        foreach (PropertyDeclarationSyntax? property in syntax.Members.OfType<PropertyDeclarationSyntax>())
        {
            var propName = property.Identifier.Text;
            INamedTypeSymbol? listInterface = GetListInterface(model.GetDeclaredSymbol(property)!, model);

            var toList = listInterface is not null
                ? $".{nameof(Enumerable.ToArray)}()"
                : "";

            writer.WriteLine("assignmentAction(\"" + propName + "\", state." + propName + toList + ");");
        }
    }


    private static INamedTypeSymbol? GetListInterface(IPropertySymbol typeSymbol, SemanticModel model)
    {
        INamedTypeSymbol? ilistType = model.Compilation.GetTypeByMetadataName("System.Collections.Generic.IList`1");
        return typeSymbol.Type.AllInterfaces
            .FirstOrDefault(i => i.OriginalDefinition.Equals(ilistType, SymbolEqualityComparer.Default));
    }
}