using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Hoeyer.OpcUa.Simulation.SourceGeneration.Constants;
using Hoeyer.OpcUa.Simulation.SourceGeneration.Extensions;
using Hoeyer.OpcUa.Simulation.SourceGeneration.Generation;
using Hoeyer.OpcUa.Simulation.SourceGeneration.Generation.IncrementalProvider;
using Hoeyer.OpcUa.Simulation.SourceGeneration.Models;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Hoeyer.OpcUa.Simulation.SourceGeneration;

[Generator]
public sealed class MethodArgumentsStructureGenerator : IIncrementalGenerator
{
    /// <inheritdoc />
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        UnloadedIncrementalValuesProvider<(string identifier, string code)> argumentStructures = context
            .GetEntityMethodInterfaces()
            .Select(static (ctx, ct) => CreateMethodArgumentStructures(ctx, ct))
            .Where(static e => e != null)
            .SelectMany(static (ctx, cs) => CreateSourceCode(ctx!.Value));

        context.RegisterSourceOutput(argumentStructures.Collect(), (sourceContext, methodModelsList) =>
        {
            foreach (var (identifier, code) in methodModelsList)
            {
                sourceContext.AddSource(identifier + ".g.cs", code);
            }
        });
    }

    private static MethodArgumentsStructureModel? CreateMethodArgumentStructures(
        (InterfaceDeclarationSyntax interfaceNode, SemanticModel model) ctx, CancellationToken _)
    {
        (InterfaceDeclarationSyntax? interfaceDeclaration, SemanticModel? model) = ctx;

        INamedTypeSymbol? entityParameter = interfaceDeclaration
            .AttributeLists
            .SelectMany(attrList => attrList.Attributes)
            .Select(attr => attr.GetEntityFromGenericArgument(model))
            .FirstOrDefault(a => a is not null);

        INamedTypeSymbol? interfaceSymbol = model.GetDeclaredSymbol(interfaceDeclaration);
        if (entityParameter is null || interfaceSymbol is null) return null;


        List<IMethodSymbol> methods = interfaceDeclaration
            .Members
            .OfType<MethodDeclarationSyntax>()
            .Select(method => model.GetDeclaredSymbol(method))
            .OfType<IMethodSymbol>()
            .ToList();

        return new MethodArgumentsStructureModel(entityParameter, interfaceSymbol, methods);
    }

    private static IEnumerable<(string identifier, string code)> CreateSourceCode(MethodArgumentsStructureModel model)
    {
        var results = new List<(string identifier, string code)>();
        var entity = model.Entity.ToDisplayString(DisplayFormats.FullyQualifiedGenericWithGlobalPrefix);
        var @interface = model.InterfaceSymbol.ToDisplayString(DisplayFormats.FullyQualifiedGenericWithGlobalPrefix);
        var @namespace = model.InterfaceSymbol.GetFullNamespace();

        foreach (IMethodSymbol methodSymbol in model.Methods)
        {
            var builder = new SourceCodeWriter();

            var className = methodSymbol.Name + "Args";
            List<(string Type, string Name)> paramList = methodSymbol
                .Parameters
                .Select(param => (
                    Type: param.Type.ToDisplayString(DisplayFormats.FullyQualifiedGenericWithGlobalPrefix),
                    Name: param.Name))
                .ToList();

            Func<string, string> firstLetterToUpper = (string s) => char.ToUpper(s[0]) + s.Substring(1);
            List<(string Type, string UpperName, string LowerName)> propertyReferences = paramList
                .Select(e => (e.Type,
                    UpperName: firstLetterToUpper.Invoke(e.Name),
                    LowerName: e.Name)).ToList();

            builder.WriteLine("namespace " + @namespace + ".Generated");
            builder.WriteLine("{");

            //method arg attribute
            var attrName = WellKnown.FullyQualifiedAttribute.OpcMethodArgumentsAttribute.WithGlobalPrefix;
            builder.WriteLine($"[{attrName}(typeof({entity}), typeof({@interface}), \"{methodSymbol.Name}\")]");
            builder.WriteLine($"public sealed record {className}");
            builder.WriteLine("{");
            foreach (var (type, upperName, _) in propertyReferences)
            {
                builder.WriteLine($"public {type} {upperName} {{ get; }}");
            }

            builder.WriteLine(
                $"public {className}({string.Join(", ", propertyReferences.Select(e => e.Type + " " + e.LowerName))})"); //ctor begin
            builder.WriteLine("{");
            foreach (var (_, upperName, lowerName) in propertyReferences) //property assignments
            {
                builder.WriteLine($"this.{upperName} = {lowerName};");
            }

            builder.WriteLine("}"); //ctor end
            builder.WriteLine("}"); //class end
            builder.WriteLine("}"); // namespace end

            results.Add((className, builder.ToString()));
        }

        return results;
    }
}