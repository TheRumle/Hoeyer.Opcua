using Hoeyer.OpcUa.Core.SourceGeneration.Constants;
using Hoeyer.OpcUa.Core.SourceGeneration.Generation.IncrementalProvider;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Hoeyer.OpcUa.Core.SourceGeneration.Generation;

[Generator]
public class EntityPropertyStateFactoryGenerator : IIncrementalGenerator
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
                foreach (var (code, symbol) in compilations)
                {
                    productionContext.AddSource(symbol.Name + "PropertyStateFactory.g.cs", code);
                }
            });
    }

    private static (string writer, INamedTypeSymbol symbol) CreateSourceCode(TypeContext typeContext)
    {
        var symbol = typeContext.SemanticModel.GetDeclaredSymbol(typeContext.Node)!;
        var entityName = symbol.ToDisplayString(DisplayFormats.FullyQualifiedGenericWithGlobalPrefix);
        var className = symbol.Name + "PropertyStateFactory";
        
        IPropertySymbol[] properties = typeContext.Node.Members.OfType<PropertyDeclarationSyntax>()
            .Select(prop => typeContext.SemanticModel.GetDeclaredSymbol(prop))
            .Where(p => p is not null)
            .Where(p => p!.Type.TypeKind == TypeKind.Enum)
            .ToArray()!;
        
        var text = $$"""
                  using System.Reflection;
                  using Hoeyer.OpcUa.Core.Abstractions.NodeStructure;
                  using Hoeyer.OpcUa.Core.Application.NodeStructure;
                  using Hoeyer.OpcUa.Core.Application.OpcTypeMappers;
                  using Opc.Ua;
                  namespace {{WellKnown.CoreServiceName}}.Generated {
                      
                      
                      public sealed class {{className}}(
                          IEntityTypeModel<{{entityName}}> model
                      ) : {{WellKnown.PropertyFactoryName.AbstractFactory}}, {{WellKnown.PropertyFactoryName.PropertyStateFactoryOf(entityName)}}
                      {
                          protected override PropertyState HandleEnumeration(
                              BaseInstanceState parent,
                              string browseName,
                              PropertyInfo propertyInfo
                          )
                          {
                              if (model.BrowseNameToEnumProperty.TryGetValue(browseName, out var type))
                              {
                                  {{string.Join("\n", CreatePropertyCode(properties))}}
                              }
    
                              throw new InvalidOperationException("There was no enum property with browsename " + browseName + " for entity {{entityName}}");
                          }
                      }
                  }
                  """;

        return (text, symbol);
    }

    private static IEnumerable<string> CreatePropertyCode(IPropertySymbol[] properties)
    {
        return properties.Select(prop =>
        {
            var typeName = prop.Type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
            return $$"""
                     if (type == typeof({{typeName}}))
                     {
                         return PropertyState<{{typeName}}>.With<EnumVariantConverter<{{typeName}}>>(parent);
                     }
                     """;
        });
    }
}