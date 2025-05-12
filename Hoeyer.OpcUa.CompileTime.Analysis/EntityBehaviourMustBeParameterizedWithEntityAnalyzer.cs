using System.Linq;
using Hoeyer.OpcUa.CompileTime.Analysis.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Hoeyer.OpcUa.CompileTime.Analysis;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class EntityBehaviourMustBeParameterizedWithEntityAnalyzer()
    : ConcurrentAnalyzer([])
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

        var unsupportedMemberTypes = interfaceSyntax.Members.Where(member => member is not MethodDeclarationSyntax);
        
        var supportedMemberTypes = interfaceSyntax.Members.Except(unsupportedMemberTypes);
        




    }

}