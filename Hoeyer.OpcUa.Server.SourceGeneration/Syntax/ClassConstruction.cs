using System.Collections.Generic;
using System.Linq;
using Hoeyer.OpcUa.Server.SourceGeneration.Generation.IncrementalProvider;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Hoeyer.OpcUa.Server.SourceGeneration.Syntax;

public static class ClassConstruction
{
    /// <summary>
    ///     Creates a class declaration syntax and its constructor. The construtor does not take any argument/>
    /// </summary>
    /// <param name="typeContext">The context containing the Entity and semantic model</param>
    /// <param name="className">The name of the generated class</param>
    /// <param name="baseClassName">The name of the base class</param>
    /// <typeparam name="T">The target type</typeparam>
    /// <returns>The class and its constructor</returns>
    public static (ClassDeclarationSyntax classDeclarationSyntax, ConstructorDeclarationSyntax constructor)
        CreateClassInheritingFromEntityGeneric<T>(
            this TypeContext<T> typeContext,
            string className,
            string baseClassName) where T : TypeDeclarationSyntax
    {
        var entityName = typeContext.Node.Identifier;

        var super = SyntaxFactory.GenericName(SyntaxFactory.Identifier(baseClassName))
            .WithTypeArgumentList(SyntaxFactory.TypeArgumentList(
                SyntaxFactory.SingletonSeparatedList<TypeSyntax>(
                    SyntaxFactory.IdentifierName(entityName))));

        var publicSealedPartial = SyntaxFactory.TokenList(
            SyntaxFactory.Token(SyntaxKind.PublicKeyword),
            SyntaxFactory.Token(SyntaxKind.SealedKeyword));

        var constructor = SyntaxFactory.ConstructorDeclaration(className)
            .WithModifiers(SyntaxFactory.TokenList(SyntaxFactory.Token(SyntaxKind.PublicKeyword)));

        return (SyntaxFactory.ClassDeclaration(className)
            .WithModifiers(publicSealedPartial)
            .WithBaseList(SyntaxFactory.BaseList([SyntaxFactory.SimpleBaseType(super)]))
            .WithMembers([constructor]), constructor);
    }

    public static ClassDeclarationSyntax CreateClassImplementingFromEntityGeneric<T>(this TypeContext<T> typeContext,
        SyntaxToken className,
        string @interface,
        MemberDeclarationSyntax[] members
    ) where T : TypeDeclarationSyntax
    {
        var entityName = typeContext.Node.Identifier;
        //represents ": mySuperClass<TEntity>" where T entity is the from the type context

        var super = SyntaxFactory.GenericName(SyntaxFactory.Identifier(@interface))
            .WithTypeArgumentList(SyntaxFactory.TypeArgumentList(
                SyntaxFactory.SingletonSeparatedList<TypeSyntax>(
                    SyntaxFactory.IdentifierName(entityName))));


        var publicSealedPartial = SyntaxFactory.TokenList(
            SyntaxFactory.Token(SyntaxKind.PublicKeyword),
            SyntaxFactory.Token(SyntaxKind.SealedKeyword));


        return SyntaxFactory.ClassDeclaration(className)
            .WithModifiers(publicSealedPartial)
            .WithBaseList(SyntaxFactory.BaseList([SyntaxFactory.SimpleBaseType(super)]))
            .WithMembers([..members]);
    }


    public static PropertyDeclarationSyntax PropertyWithPublicGetter(SyntaxKind typeKind, string propertyName,
        string? literalValue = null)
    {
        var value = SyntaxFactory.PropertyDeclaration(
                SyntaxFactory.PredefinedType(SyntaxFactory.Token(typeKind)),
                propertyName)
            .AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword))
            .WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken))
            .WithAccessorList(SyntaxFactory.AccessorList(
                SyntaxFactory.List(new[]
                {
                    SyntaxFactory.AccessorDeclaration(SyntaxKind.GetAccessorDeclaration)
                        .WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken))
                })));

        if (literalValue != null)
        {
            return value.WithInitializer(SyntaxFactory.EqualsValueClause(
                SyntaxFactory.LiteralExpression(
                    SyntaxKind.StringLiteralExpression, SyntaxFactory.Literal(literalValue))));
        }

        return value;
    }


    public static MethodDeclarationSyntax PublicMethodWithArgs(
        string methodName,
        TypeSyntax methodType,
        (string parameterName, TypeSyntax type)[] inputArgs,
        StatementSyntax[] body)
    {
        var parameterSyntaxes = inputArgs.Select(e => SyntaxFactory
                .Parameter(SyntaxFactory.Identifier(e.parameterName))
                .WithType(e.type))
            .ToSeparatedList();


        var parameters = SyntaxFactory.ParameterList(parameterSyntaxes);
        var block = SyntaxFactory.Block(body);

        return SyntaxFactory.MethodDeclaration(methodType, SyntaxFactory.Identifier(methodName))
            .AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword))
            .WithParameterList(parameters)
            .WithBody(block);
    }

    public static ObjectCreationExpressionSyntax ObjectInstantiationWithPropertyAssignments(
        IdentifierNameSyntax identifier,
        IEnumerable<ArgumentSyntax> ctorArgs,
        IEnumerable<AssignmentExpressionSyntax> assignments)
    {
        var initializationExpr = SyntaxFactory.InitializerExpression(SyntaxKind.ObjectInitializerExpression)
            .WithExpressions(SyntaxFactory.SeparatedList<ExpressionSyntax>(assignments));


        return SyntaxFactory.ObjectCreationExpression(identifier)
            .WithArgumentList(SyntaxFactory.ArgumentList(ctorArgs.ToSeparatedList()))
            .WithInitializer(initializationExpr);
    }

    public static ObjectCreationExpressionSyntax NewObjectWithCtor(
        string objectname,
        IEnumerable<ArgumentSyntax> ctorArgs)
    {
        var id = SyntaxFactory.IdentifierName(objectname);
        return SyntaxFactory.ObjectCreationExpression(id)
            .WithArgumentList(SyntaxFactory.ArgumentList(ctorArgs.ToSeparatedList()));
    }

    public static ObjectCreationExpressionSyntax NewObjectWithCtor(
        string objectname,
        IEnumerable<string> ctorArgs)
    {
        return NewObjectWithCtor(objectname,
            ctorArgs.Select(e => SyntaxFactory.Argument(SyntaxFactory.IdentifierName(e))).ToList());
    }


    public static AssignmentExpressionSyntax AssignToNewObject(
        string variable,
        string objectType,
        params string[] ctorArgs)
    {
        return SyntaxFactory.AssignmentExpression(
            SyntaxKind.SimpleAssignmentExpression,
            SyntaxFactory.IdentifierName(variable),
            NewObjectWithCtor(
                objectType, ctorArgs
            ));
    }
}