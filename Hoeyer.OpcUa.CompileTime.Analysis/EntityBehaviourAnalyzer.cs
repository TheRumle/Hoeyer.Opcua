using System.Collections.Generic;
using System.Linq;
using Hoeyer.OpcUa.CompileTime.Analysis.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Hoeyer.OpcUa.CompileTime.Analysis;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class AgentBehaviourAnalyzer()
    : ConcurrentAnalyzer([
        Rules.MustBeSupportedOpcUaType, Rules.ReturnTypeMustBeTask, Rules.OpcUaAgentBehaviourMemberNotSupported,
        Rules.MustBeOpcAgentArgument, Rules.MethodNameMustBeUnique
    ])
{
    /// <inheritdoc />
    protected override void InitializeAnalyzer(AnalysisContext context)
    {
        context.RegisterSyntaxNodeAction(AnalyzeSyntax, SyntaxKind.InterfaceDeclaration);
    }

    private static void AnalyzeSyntax(SyntaxNodeAnalysisContext context)
    {
        if (context.Node is not InterfaceDeclarationSyntax interfaceSyntax ||
            !interfaceSyntax.IsAnnotatedAsOpcUaAgentBehaviour(context.SemanticModel))
        {
            return;
        }

        var errors = GetMemberNotSupported(interfaceSyntax)
            .Concat(GetGenericArgNotAgent(interfaceSyntax, context.SemanticModel))
            .Concat(GetMemberTypesNotSupported(interfaceSyntax, context.SemanticModel))
            .Concat(GetUniqueMethodNameViolations(interfaceSyntax));

        foreach (var diagnostic in errors)
        {
            context.ReportDiagnostic(diagnostic);
        }
    }

    public static IEnumerable<Diagnostic> GetUniqueMethodNameViolations(InterfaceDeclarationSyntax interfaceSyntax)
    {
        return interfaceSyntax.Members
            .OfType<MethodDeclarationSyntax>()
            .GroupBy(method => method.Identifier.Text)
            .Where(methodGroup => methodGroup.Count() > 1)
            .SelectMany(methodGroup => methodGroup
                .Skip(1)
                .Select(method => Diagnostic.Create(Rules.MethodNameMustBeUnique, method.GetLocation())));
    }

    private static IEnumerable<Diagnostic> GetMemberTypesNotSupported(InterfaceDeclarationSyntax interfaceSyntax,
        SemanticModel model)
    {
        var createDiagnostic = (TypeSyntax t) =>
        {
            var typeString = model.GetTypeInfo(t).Type;
            return Diagnostic.Create(Rules.MustBeSupportedOpcUaType, t.GetLocation(), typeString);
        };

        var methods = interfaceSyntax.Members.OfType<MethodDeclarationSyntax>().ToArray();

        var returnTypeViolations = methods
            .Where(method => !SupportedTypes.IsSupportedTask(method.ReturnType, model))
            .Select(method => Diagnostic.Create(Rules.ReturnTypeMustBeTask, method.ReturnType.GetLocation()));

        var argumentViolations = methods.SelectMany(method => method.ParameterList.Parameters
                .Select(param => param.Type!)
                .Where(methodParam => !SupportedTypes.IsSupported(methodParam!, model)))
            .Select(methodParam => createDiagnostic.Invoke(methodParam));

        return returnTypeViolations.Concat(argumentViolations);
    }

    private static IEnumerable<Diagnostic> GetMemberNotSupported(InterfaceDeclarationSyntax interfaceSyntax)
    {
        return interfaceSyntax
            .Members
            .Where(member => member is not MethodDeclarationSyntax)
            .Select(e => Diagnostic.Create(Rules.OpcUaAgentMemberNotSupported, e.GetLocation()));
    }

    private static IEnumerable<Diagnostic> GetGenericArgNotAgent(TypeDeclarationSyntax interfaceSyntax,
        SemanticModel model)
    {
        AttributeData? attribute = interfaceSyntax.GetOpcUaAgentBehaviourAttribute(model);
        ITypeSymbol? targetType = attribute?.AttributeClass?.TypeArguments.FirstOrDefault();
        if (targetType == null) yield break;
        if (!targetType.IsAnnotatedAsOpcUaAgent())
        {
            yield return Diagnostic.Create(Rules.MustBeOpcAgentArgument, interfaceSyntax.Identifier.GetLocation());
        }
    }
}