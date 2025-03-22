using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Hoeyer.OpcUa.Core.Observation;
using Hoeyer.OpcUa.Server.SourceGeneration.Constants;
using Hoeyer.OpcUa.Server.SourceGeneration.Generation.IncrementalProvider;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Hoeyer.OpcUa.Server.SourceGeneration.Generation;

[Generator]
public sealed class EntityContainerGenerator : IIncrementalGenerator
{
    
    private static readonly string STATECONTAINER = nameof(StateContainer<int>).Split('`')[0];
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var decoratedRecordsProvider = context
            .GetTypeContextForOpcEntities<ClassDeclarationSyntax>()
            .Select(async (typeContext,cancel) => await CreateSourceCodeAsync(typeContext, cancel));

        context.RegisterImplementationSourceOutput(decoratedRecordsProvider.Collect(), AddContainerSourceCode);
    }
    
    private static void AddContainerSourceCode(SourceProductionContext context, ImmutableArray<Task<GeneratedClass<ClassDeclarationSyntax>>> generatedUnits)
    {
        //Not yet supported and will not be it for a while
    }

    private static async Task<GeneratedClass<T>> CreateSourceCodeAsync<T>(TypeContext<T> context, CancellationToken cancellationToken) where T : TypeDeclarationSyntax
    {
        var classDeclaration = GetClassDeclaration(context, cancellationToken);
        var unit = await context.CreateCompilationUnitFor(classDeclaration, Locations.Utilities , cancellationToken: cancellationToken);
        
 
        
        
        return new GeneratedClass<T>(unit, classDeclaration, context.Node);
    }

    private static ClassDeclarationSyntax GetClassDeclaration<T>(TypeContext<T> typeContext, CancellationToken cancellationToken)
        where T : TypeDeclarationSyntax
    {
        var entityName = typeContext.Node.Identifier;
        var className = typeContext.Node.Identifier + "Container";
        
        var containerName = SyntaxFactory.GenericName(SyntaxFactory.Identifier(STATECONTAINER))
            .WithTypeArgumentList(SyntaxFactory.TypeArgumentList(
                SyntaxFactory.SingletonSeparatedList<TypeSyntax>(
                    SyntaxFactory.IdentifierName(entityName))));

        cancellationToken.ThrowIfCancellationRequested();

        var constructorParams = SyntaxFactory.ParameterList(
            SyntaxFactory.SingletonSeparatedList(
                SyntaxFactory.Parameter(SyntaxFactory.Identifier("entity"))
                    .WithType(SyntaxFactory.IdentifierName(entityName))));
        
        var constructor = GetConstructor(className, constructorParams);
        
        var notifyingSettersMethods =  typeContext.Node
            .Members
            .OfType<PropertyDeclarationSyntax>()
            .Select(CreateNotifyingSetter);

        var publicSealedPartial = SyntaxFactory.TokenList(
            SyntaxFactory.Token(SyntaxKind.PublicKeyword),
            SyntaxFactory.Token(SyntaxKind.SealedKeyword));

        var classDeclaration = SyntaxFactory.ClassDeclaration(className)
            .WithModifiers(publicSealedPartial)
            .WithBaseList(SyntaxFactory.BaseList([SyntaxFactory.SimpleBaseType(containerName)]))
            .WithMembers(SyntaxFactory.List<MemberDeclarationSyntax>(
                [constructor, ..notifyingSettersMethods]));

        return classDeclaration;
    }

    private static ConstructorDeclarationSyntax GetConstructor(string className, ParameterListSyntax constructorParams)
    {
        var baseArgs = SyntaxFactory.ArgumentList([SyntaxFactory.Argument(SyntaxFactory.IdentifierName("entity"))]);
        
        return SyntaxFactory.ConstructorDeclaration(className)
            .WithModifiers(SyntaxFactory.TokenList(SyntaxFactory.Token(SyntaxKind.PublicKeyword)))
            .WithParameterList(constructorParams)
            .WithInitializer(SyntaxFactory.ConstructorInitializer(
                SyntaxKind.BaseConstructorInitializer,
                baseArgs))
            .WithBody(SyntaxFactory.Block());
    }

    private static MethodDeclarationSyntax CreateNotifyingSetter(PropertyDeclarationSyntax property)
    {
        var identifier = property.Identifier.ToString();
        var lowerIdentifier = char.ToLowerInvariant(identifier.First()) + identifier.Substring(1);

        var returnType = SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.VoidKeyword));
        var setMethodName = SyntaxFactory.Identifier($"Set{identifier}");
        var parameters = SyntaxFactory.ParameterList(
            SyntaxFactory.SingletonSeparatedList(
                SyntaxFactory
                    .Parameter(SyntaxFactory.Identifier($"{lowerIdentifier}"))
                    .WithType(property.Type)));
        
        return SyntaxFactory.MethodDeclaration(returnType, setMethodName)
            .WithModifiers(SyntaxFactory.TokenList(SyntaxFactory.Token(SyntaxKind.PublicKeyword)))
            .WithParameterList(parameters)
            .WithBody(SyntaxFactory.Block(
                AssignmentExpression(identifier, lowerIdentifier),
                StateChangeCall()
            ));
    }

    private static ExpressionStatementSyntax StateChangeCall()
    {
        var methodToCall = SyntaxFactory.IdentifierName(nameof(StateContainer<int>.ChangeState));
        var args = SyntaxFactory.ArgumentList(
            SyntaxFactory.SingletonSeparatedList(
                SyntaxFactory.Argument(SyntaxFactory.IdentifierName(nameof(StateContainer<int>.State)))
                )
            );
        
        return SyntaxFactory.ExpressionStatement(SyntaxFactory.InvocationExpression(methodToCall)
            .WithArgumentList(args));
    }

    private static ExpressionStatementSyntax AssignmentExpression(string left, string right)
    {
        return SyntaxFactory.ExpressionStatement(
            SyntaxFactory.AssignmentExpression(
                SyntaxKind.SimpleAssignmentExpression,
                SyntaxFactory.MemberAccessExpression(
                    SyntaxKind.SimpleMemberAccessExpression,
                    SyntaxFactory.IdentifierName(nameof(StateContainer<int>.State)),
                    SyntaxFactory.IdentifierName(left)),
                SyntaxFactory.IdentifierName(right)));
    }
   
}