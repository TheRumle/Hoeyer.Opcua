using System.Collections.Generic;
using System.Linq;
using Hoeyer.OpcUa.Server.Generation.Diagnostics;
using Hoeyer.OpcUa.Server.Generation.IncrementalProvider;
using Hoeyer.OpcUa.Server.Generation.SyntaxExtensions;
using Hoeyer.OpcUa.TypeUtilities;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Hoeyer.OpcUa.Server.Generation.OpcUa;


[Generator]
public class EntityOpcUaObjectStateGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        
        var decoratedRecordsProvider = context
            .GetTypeDeclarationsDecoratedWith<ClassDeclarationSyntax>(ModellingNamespace.ENTITY_ATTRIBUTE_FULLNAME)
            .Select((e, c) => new TypeContext<ClassDeclarationSyntax>(e.SemanticModel, e.Node));
        
        context.RegisterImplementationSourceOutput(
            decoratedRecordsProvider.Collect(),
            static (context, typeContexts) =>
        {
            foreach (var typeContext in typeContexts)
            {
                ExtractAssignmentTexts(typeContext, context);
            }
            
            
        });
    }

    private static void ExtractAssignmentTexts(TypeContext<ClassDeclarationSyntax> typeContext, SourceProductionContext context)
    {
        var entityName = typeContext.Node.Identifier.ToString();
        var nodeName = entityName + "Node";
        var className = $"{entityName}OpcUaNodeCreator";
        var sourceCode = CreateSourceCode(typeContext, context, entityName, nodeName, className);
        context.AddSource(className+".g.cs", sourceCode);
    }

    private static string CreateSourceCode(TypeContext<ClassDeclarationSyntax> typeContext, SourceProductionContext context, string entityName,
        string nodeName, string className)
    {
        return $$"""
using System;
using System.Linq;
using Hoeyer.OpcUa.Variables;
using Opc.Ua;

namespace Hoeyer.OpcUa.Server.Entity
{
    internal sealed class {{className}} : IEntityObjectStateCreator
    {
       public string EntityName { get; } = "{{entityName}}";
       public NodeState CreateEntityOpcUaNode(NodeState root, ushort dynamicNamespaceIndex)
       {
           BaseObjectState {{nodeName}} = new BaseObjectState(root)
           {
               BrowseName =  new QualifiedName("{{entityName}}", dynamicNamespaceIndex),
               NodeId = new NodeId(Guid.NewGuid(), dynamicNamespaceIndex),
               DisplayName = "{{entityName}}",
           };
       
           root.AddChild({{nodeName}});
           
           //Assign properties
           {{string.Join("\n", GetPropertyCreationStatements(typeContext, context, nodeName))}}
           
           return {{nodeName}};
       }
    }
}
""";
    }

    private static IEnumerable<string> GetPropertyCreationStatements(TypeContext<ClassDeclarationSyntax> typeContext,
        SourceProductionContext context, string nodeName)
    {
        var semanticModel = typeContext.SemanticModel;

        var obtainedProperties = typeContext
            .Node.Members.OfType<PropertyDeclarationSyntax>()
            .Select(property => (PropertyInfo: OpcTypeInfo.PropertyInfo(property, semanticModel),
                Errors: GetDiagnosticErrors(property, semanticModel))).ToList();

        foreach (var err in obtainedProperties.Where(e=>e.Errors.Any()).SelectMany(e=>e.Errors))
        {
            context.ReportDiagnostic(err);
        }
        
        foreach (var propertyInfo in obtainedProperties
                     .Where(e=>e.Errors.Count == 0)
                     .Select(e=>e.PropertyInfo))
        {
            var propertyName = char.ToLower(propertyInfo.Name.Trim()[0]) + propertyInfo.Name.Trim().Substring(1);
            yield return $"""
PropertyState {propertyName} = {nodeName}.AddProperty< {propertyInfo.Type}>("{propertyName}", {propertyInfo.OpcUaTypeId}, ValueRanks.Scalar);
           {propertyName}.NodeId = new NodeId(Guid.NewGuid(), dynamicNamespaceIndex);
""";
        }
        
    }

    private static List<Diagnostic> GetDiagnosticErrors(PropertyDeclarationSyntax property, SemanticModel semanticModel)
    {
        var diagnostics = new List<Diagnostic>();
        
        if (!property.HasPublicSetter()) diagnostics.Add(OpcUaDiagnostics.MustHavePublicSetter(property));
        if (!OpcTypeInfo.IsSupported(property, semanticModel)) diagnostics.Add(OpcUaDiagnostics.UnsupportedOpcUaType(property));
        
        return diagnostics;
    }
}