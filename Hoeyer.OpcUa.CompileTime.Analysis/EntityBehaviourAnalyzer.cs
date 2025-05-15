using System.Collections.Generic;
using System.Linq;
using Hoeyer.OpcUa.CompileTime.Analysis.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Hoeyer.OpcUa.CompileTime.Analysis;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class EntityBehaviourAnalyzer()
    : ConcurrentAnalyzer([Rules.OpcUaEntityBehaviourMemberNotSupported])
{
    /// <inheritdoc />
    protected override void InitializeAnalyzer(AnalysisContext context)
    {
        context.RegisterSyntaxNodeAction(AnalyzeSyntax, SyntaxKind.InterfaceDeclaration);
    }

    private static void AnalyzeSyntax(SyntaxNodeAnalysisContext context)
    {
        if (context.Node is not InterfaceDeclarationSyntax interfaceSyntax ||
            !interfaceSyntax.IsAnnotatedAsOpcUaEntityBehaviour(context.SemanticModel))
        {
            return;
        }
        

        
        var errors = GetMemberNotSupported(interfaceSyntax)
            .Concat(AssessAnnotationArguments(interfaceSyntax, context.SemanticModel));
        
        foreach (var diagnostic in errors.AsParallel())
        {
            context.ReportDiagnostic(diagnostic);
        }
    }

    private static IEnumerable<Diagnostic> GetMemberNotSupported(InterfaceDeclarationSyntax interfaceSyntax)
    {
        return interfaceSyntax
            .Members
            .Where(member => member is not MethodDeclarationSyntax)
            .Select(e => Diagnostic.Create(Rules.OpcUaEntityMemberNotSupported, e.GetLocation()));
    }

    private static IEnumerable<Diagnostic> AssessAnnotationArguments(TypeDeclarationSyntax interfaceSyntax, SemanticModel model)
    {
        AttributeData? attribute = interfaceSyntax.GetOpcUaEntityBehaviourAttribute(model);
        if (attribute == null) yield break;
        ITypeSymbol? targetType = attribute.AttributeClass?.TypeArguments.FirstOrDefault();
        if (targetType == null) yield break;
        if (!targetType.IsAnnotatedAsOpcUaEntity(model))
        {
            yield return Diagnostic.Create(Rules.MustBeOpcEntityArgument, interfaceSyntax.Identifier.GetLocation());
        }
    }
}