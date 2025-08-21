using System.Collections.Generic;
using System.Linq;
using Hoeyer.OpcUa.Client.SourceGeneration.Constants;
using Hoeyer.OpcUa.Client.SourceGeneration.Extensions;
using Hoeyer.OpcUa.Client.SourceGeneration.Generation;
using Hoeyer.OpcUa.Client.SourceGeneration.Generation.IncrementalProvider;
using Hoeyer.OpcUa.Client.SourceGeneration.Models;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Hoeyer.OpcUa.Client.SourceGeneration;

[Generator]
public sealed class RemoteMethodCallerGenerator : IIncrementalGenerator
{
    /// <inheritdoc />
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var decoratedRecordsProvider = context
            .GetEntityMethodInterfaces()
            .Select(static (ctx, ct) => CreateRemoteMethodCallerModel(ctx.interfaceNode, ctx.model))
            .Where(static model => model.HasValue);


        UnloadedIncrementalValuesProvider<(string Identifier, string SourceCode)> methodCallerImpls =
            decoratedRecordsProvider.Select(static (model, _) => CreateClassImplementation(model!.Value));

        context.RegisterSourceOutput(methodCallerImpls.Collect(),
            (sourceContext, dataModels) => { GenerateSourceCode(sourceContext, dataModels); });
    }


    private static void GenerateSourceCode(SourceProductionContext sourceContext,
        IEnumerable<(string identifier, string code)> dataModels)
    {
        foreach (var (identifier, code) in dataModels)
        {
            sourceContext.AddSource(identifier + ".g.cs", code);
        }
    }


    private static RemoteMethodCallerModel? CreateRemoteMethodCallerModel(
        InterfaceDeclarationSyntax interfaceDeclaration, SemanticModel model)
    {
        //for instance Task MyMethod(int a, int b, TypeFromNamespaceQ value) --> System.Threading.Task MyMethod(System.Int32 a, System.Int32 b, Q.TypeFromNamespaceQ value)
        INamedTypeSymbol? entityParameter = GetEntityFromGenericArgument(interfaceDeclaration, model);
        INamedTypeSymbol? declaredSymbol = model.GetDeclaredSymbol(interfaceDeclaration);
        if (entityParameter is null || declaredSymbol is null) return null;
        List<MethodDeclarationSyntax> methods = interfaceDeclaration.Members.OfType<MethodDeclarationSyntax>().ToList();

        var rewriter = new FullyQualifyTypeNamesRewriter(model);
        List<MemberDeclarationSyntax> rewrittenMethods = methods
            .Select(e => (MemberDeclarationSyntax)rewriter.Visit(e))
            .ToList();


        return new RemoteMethodCallerModel(declaredSymbol, entityParameter,
            interfaceDeclaration.WithMembers(List(rewrittenMethods)));
    }


    private static (string Identifier, string SourceCode) CreateClassImplementation(RemoteMethodCallerModel model)
    {
        InterfaceDeclarationSyntax interfaceDeclaration = model.GlobalizedMethodCaller;
        var classIdentifier = (interfaceDeclaration.Identifier.Text.StartsWith("I")
            ? interfaceDeclaration.Identifier.Text.Substring(1)
            : interfaceDeclaration.Identifier.Text) + "RemoteCaller";

        var builder = new SourceCodeWriter();
        var @namespace = model.InterfaceSymbol.GetFullNamespace();
        builder.WriteLine("namespace " + @namespace + ".Generated");
        builder.WriteLine("{");
        builder.WriteLine("public sealed class " + classIdentifier);
        builder.Write(
            $"({WellKnown.FullyQualifiedInterface.MethodCallerType.WithGlobalPrefix}<{model.RelatedEntityName.WithGlobalPrefix}> caller)");
        builder.WriteLine(" : " + model.InterfaceName.WithGlobalPrefix);
        builder.WriteLine("{");

        //All members are tasks, always
        foreach (IMethodSymbol? method in model.InterfaceSymbol.GetMembers().OfType<IMethodSymbol>())
        {
            var returnType = method.ReturnType.ToDisplayString(DisplayFormats.FullyQualifiedGenericWithGlobalPrefix);
            var genericArgs = method.ReturnType is INamedTypeSymbol { Arity: 1 } named
                ? $"<{named.TypeArguments[0].ToDisplayString(DisplayFormats.FullyQualifiedGenericWithGlobalPrefix)}>"
                : "";

            List<string> paramStrings = method.Parameters.Select(e =>
                e.ToDisplayString(DisplayFormats.FullyQualifiedGenericWithGlobalPrefix)).ToList();
            var typeArgs = string.Join(",", paramStrings);
            var paramNames = string.Join(", ", method.Parameters.Select(e => e.Name));
            builder.WriteLine($"public {returnType} {method.Name}({typeArgs})");
            builder.WriteLine("{");
            builder.Write($"return caller.CallMethod{genericArgs}(").Write($"nameof({method.Name}),");
            builder.Write(" global::System.Threading.CancellationToken.None");
            if (paramStrings.Count > 0) builder.Write(", " + paramNames);

            builder.Write(");");
            builder.WriteLine();
            builder.WriteLine("}");
        }

        builder.WriteLine("}");
        builder.WriteLine("}");
        return (Identifier: classIdentifier, SourceCode: builder.ToString());
    }

    private static INamedTypeSymbol? GetEntityFromGenericArgument(InterfaceDeclarationSyntax interfaceDeclaration,
        SemanticModel model)
    {
        IEnumerable<AttributeSyntax> attributes =
            interfaceDeclaration.AttributeLists.SelectMany(attrList => attrList.Attributes);
        return attributes
            .Select(attr => attr.GetEntityFromGenericArgument(model))
            .FirstOrDefault(a => a is not null);
    }
}