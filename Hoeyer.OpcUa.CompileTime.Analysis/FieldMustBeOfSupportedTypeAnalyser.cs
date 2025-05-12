using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Hoeyer.OpcUa.CompileTime.Analysis.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Hoeyer.OpcUa.CompileTime.Analysis;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class FieldMustBeOfSupportedTypeAnalyser() : ConcurrentAnalyzer([Rules.MustBeSupportedOpcUaType])
{
    protected override void InitializeAnalyzer(AnalysisContext context)
    {
        context.RegisterSyntaxNodeAction(AnalyzeSyntax, SyntaxKind.ClassDeclaration);
        context.RegisterSyntaxNodeAction(AnalyzeSyntax, SyntaxKind.RecordDeclaration);
    }

    private static void AnalyzeSyntax(SyntaxNodeAnalysisContext context)
    {
        if (context.Node is not TypeDeclarationSyntax typeSyntax ||
            !typeSyntax.IsAnnotatedAsOpcUaEntity(context.SemanticModel))
        {
            return;
        }

        var model = context.SemanticModel;
        
        var analysisResult = typeSyntax.Members
            .Select( member => member switch
            {
                FieldDeclarationSyntax field => SupportedTypes.IsSupported(field.Declaration.Type, model)
                    ? MemberSupport.Success(member)
                    : MemberSupport.Failure(member, field.Declaration.Type),

                PropertyDeclarationSyntax property => SupportedTypes.IsSupported(property.Type, model)
                    ? MemberSupport.Success(member)
                    : MemberSupport.Failure(member, property.Type),
            
                var other => MemberSupport.Failure(other!, new List<TypeSyntax>()) 
            }).ToImmutableHashSet();
        

        foreach (var (_, member, types) in analysisResult.Where(e => !e.IsSupported))
            context.ReportDiagnostic(Diagnostic.Create(
                Rules.MustBeSupportedOpcUaType, 
                member.GetLocation(),
                string.Join(", ", types.Select(t => t.ToString())))
            );
    }

    private static MemberSupport ExamineEventSupport(EventFieldDeclarationSyntax eventSyntax, SemanticModel model,  Compilation compilation)
    {
        return new TypeAnalyzer<EventFieldDeclarationSyntax, MemberSupport>(
                model,
                e => e.Declaration.Type,
                MemberSupport.Failure(eventSyntax, eventSyntax.Declaration.Type))
            .Map(eventSyntax, (typeSymbol) =>
            {

                Predicate<INamedTypeSymbol> isActionOrFunc = (typeSymbol) =>
                {
                    var arity = typeSymbol.Arity;

                    var funcMetadataName = $"System.Func`{arity}";
                    var actionMetadataName = $"System.Action`{arity}";

                    var funcType = compilation.GetTypeByMetadataName(funcMetadataName);
                    var actionType = compilation.GetTypeByMetadataName(actionMetadataName);

                    var originalDef = typeSymbol.OriginalDefinition;

                    return SymbolEqualityComparer.Default.Equals(originalDef, funcType)
                           || SymbolEqualityComparer.Default.Equals(originalDef, actionType);
                };


                if (!isActionOrFunc(typeSymbol)) return MemberSupport.Failure(eventSyntax, eventSyntax.Declaration.Type);

                var errors = typeSymbol.TypeArguments.Where(SupportedTypes.NotSupported).ToList();
                if (errors.Any())
                {
                    return MemberSupport.Failure(eventSyntax, errors);
                }

                return MemberSupport.Success(eventSyntax);
            });
    }
    
    public class TypeAnalyzer<T, TOut>(
        SemanticModel model,
        Func<T, TypeSyntax> typeSelect,
        TOut fallBack)
    {
        public TOut Map(T syntax, Func<INamedTypeSymbol, TOut> map)
        {
            var typeInfo = model.GetTypeInfo(typeSelect.Invoke(syntax));
            var typeSymbol = typeInfo.Type as INamedTypeSymbol;
            if (typeSymbol == null) return fallBack;
            return map.Invoke(typeSymbol);
        }
    }

    private static MemberSupport ExamineEventSupport(EventDeclarationSyntax eventSyntax, SemanticModel model,
        Compilation contextCompilation)
    {
        return new TypeAnalyzer<EventDeclarationSyntax, MemberSupport>(model,
                syntax => syntax.Type,
                MemberSupport.Failure(eventSyntax, eventSyntax.Type))
            .Map(eventSyntax,(typeSymbol) =>
            {
                var isFuncOrAction = typeSymbol.ConstructedFrom?.ToDisplayString() switch
                {
                    "System.Func" => true,
                    "System.Action" => true,
                    _ => false
                };
                if (!isFuncOrAction) return MemberSupport.Failure(eventSyntax, eventSyntax.Type);

                var errors = typeSymbol.TypeArguments.Where(SupportedTypes.NotSupported).ToList();
                if (errors.Any())
                {
                    return MemberSupport.Failure(eventSyntax, errors);
                }

                return MemberSupport.Success(eventSyntax);
            });
    }

    private static MemberSupport ExamineDelegateSupport(DelegateDeclarationSyntax delegateDecl, SemanticModel model)
    {
        var analysis = delegateDecl.ParameterList.Parameters
            .Select(p => p.Type)
            .Where(t => t != null)
            .Select(t => (Type: t!, IsSupported: SupportedTypes.IsSupported(t!, model)))
            .ToList();

        if (analysis.Any(t => !t.IsSupported))
        { 
            MemberSupport.Failure(delegateDecl, analysis.Where(t => !t.IsSupported).Select(e=>e.Type));
        }
        return MemberSupport.Success(delegateDecl);
    }

    private static IEnumerable<TypeSyntax> GetDelegateArgumentTypes(TypeSyntax typeSyntax, SemanticModel model)
    {
        var typeInfo = model.GetTypeInfo(typeSyntax);
        var symbol = typeInfo.Type as INamedTypeSymbol;

        if (symbol == null || symbol.DelegateInvokeMethod == null) return [];
        var syntaxList = new List<TypeSyntax>();
        foreach (var param in symbol.DelegateInvokeMethod.Parameters)
        {
            var syntaxRef = param.DeclaringSyntaxReferences.FirstOrDefault();
            if (syntaxRef?.GetSyntax() is ParameterSyntax paramSyntax && paramSyntax.Type is TypeSyntax ts)
            {
                syntaxList.Add(ts);
            }
            else
            {
                // Fallback: get type syntax via Symbol (not precise, but works for basic types)
                var typeNode = SyntaxFactory.ParseTypeName(param.Type.ToDisplayString());
                syntaxList.Add(typeNode);
            }
        }

        return syntaxList;
    }

    private static bool IsFuncSymbol(INamedTypeSymbol typeSymbol, Compilation compilation)
    {
        var arity = typeSymbol.Arity;

        var funcMetadataName = $"System.Func`{arity}";
        var actionMetadataName = $"System.Action`{arity}";

        var funcType = compilation.GetTypeByMetadataName(funcMetadataName);
        var actionType = compilation.GetTypeByMetadataName(actionMetadataName);

        var originalDef = typeSymbol.OriginalDefinition;

        return SymbolEqualityComparer.Default.Equals(originalDef, funcType)
               || SymbolEqualityComparer.Default.Equals(originalDef, actionType);
    }
}