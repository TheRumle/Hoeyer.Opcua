using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Hoeyer.OpcUa.Server.Generation.Configuration;


[Generator]
public class EntityOpcUaObjectStateGenerator : IIncrementalGenerator
{
    private const string ATTRIBUTE_META_NAME = "Hoeyer.OpcUa.OpcUaEntityAttribute";
    private static bool done = false;
    

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
      
        
        
        IncrementalValuesProvider<GeneratorAttributeSyntaxContext> decoratedRecordsProvider = context.SyntaxProvider.ForAttributeWithMetadataName(ATTRIBUTE_META_NAME,
            static (decoratedClass, cancellationToken) => true,
            static (attributeSyntaxContext, cancellationToken) => attributeSyntaxContext);

        context.RegisterImplementationSourceOutput(
            decoratedRecordsProvider.Collect(),
            static (context, a) =>
        {
          if (!done) context.AddSource("MySource.g.cs", "public class MyClass{}");
          done = true;
        });
    }
    

    private static UnloadedIncrementalValuesProvider<TypeContext<T>> GetValueProviderForDecorated<T>(IncrementalGeneratorInitializationContext context, string attributeMetaName)
        where T : BaseTypeDeclarationSyntax
    {
        var valueProvider = context.SyntaxProvider.ForAttributeWithMetadataName(attributeMetaName,
            predicate: (decoratedClass, cancellationToken) => decoratedClass is T,
            transform: (attributeSyntaxContext, cancellationToken) =>
                new TypeContext<T>(attributeSyntaxContext.SemanticModel, (T)attributeSyntaxContext.TargetNode));
        return new UnloadedIncrementalValuesProvider<TypeContext<T>>(valueProvider);

    }
    internal record struct UnloadedIncrementalValuesProvider<T>(IncrementalValuesProvider<T> source)
    {
        public IncrementalValuesProvider<T> Source { get; private set; } = source;

        public UnloadedIncrementalValuesProvider<T> Where(Func<T, bool> predicate) =>
            new(source.Where(predicate));
    
        public UnloadedIncrementalValuesProvider<TOut> Select<TOut>(Func<T, CancellationToken, TOut> selector) where TOut : BaseTypeDeclarationSyntax => new(source.Select(selector));

        public UnloadedIncrementalValuesProvider<TOut> SelectMany<TOut>(Func<T, CancellationToken, IEnumerable<TOut>> selector) where TOut : BaseTypeDeclarationSyntax => new(source.SelectMany(selector));
    }
}