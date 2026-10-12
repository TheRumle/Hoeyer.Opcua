using Hoeyer.OpcUa.Core.SourceGeneration.Constants;
using Hoeyer.OpcUa.Core.SourceGeneration.Generation.IncrementalProvider;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Hoeyer.OpcUa.Core.SourceGeneration.Generation;

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
                    productionContext.AddSource(symbol.Name + "Translator.g.cs", code.ToString());
                }
            });
    }

    private static (SourceCodeWriter writer, INamedTypeSymbol symbol) CreateSourceCode(TypeContext typeContext)
    {
        var symbol = typeContext.SemanticModel.GetDeclaredSymbol(typeContext.Node)!;
        var entityName = symbol.ToDisplayString(DisplayFormats.FullyQualifiedGenericWithGlobalPrefix);
        var className = entityName + "PropertyStateFactory";
        
        IPropertySymbol[] properties = typeContext.Node.Members.OfType<PropertyDeclarationSyntax>()
            .Select(prop => typeContext.SemanticModel.GetDeclaredSymbol(prop))
            .Where(p => p is not null)
            .Where(p => p!.Type.TypeKind == TypeKind.Enum)
            .ToArray()!;
        
        var x = $$"""
                  public sealed class {{className}}(
                      IEntityTypeModel<{{entityName}}> model
                  ) : FieldPropertyStateFactory, IPropertyStateFactory<{{entityName}}>
                  {
                      protected override PropertyState HandleEnumeration(
                          BaseInstanceState parent,
                          string browseName,
                          PropertyInfo propertyInfo
                      )
                      {
                          if (model.BrowseNameToEnumProperty.TryGetValue(browseName, out var type))
                          {
                              //PropName
                              {{string.Join("\n", CreatePropertyCode(properties))}}
                          }

                          throw new InvalidOperationException("There was enum property with browsename " + browseName);
                      }
                  }
                  """;
        
        throw new NotImplementedException();
    }

    private static IEnumerable<string> CreatePropertyCode(IPropertySymbol[] properties)
    {
        return properties.Select(prop =>
        {
            var typeName = prop.Type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
            return $$"""
                     if (type == typeof({{typeName}}))
                     {
                         //PropName
                         return PropertyState<{{typeName}}}>
                             .With<EnumVariantConverter<{{typeName}}>>(parent);
                     }
                     """;
        });
    }
}