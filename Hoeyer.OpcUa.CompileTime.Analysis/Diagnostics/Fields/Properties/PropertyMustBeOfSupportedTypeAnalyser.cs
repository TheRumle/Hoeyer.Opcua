using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Hoeyer.OpcUa.CompileTime.Analysis.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Hoeyer.OpcUa.CompileTime.Analysis.Diagnostics.Fields.Properties;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class PropertyMustBeOfSupportedTypeAnalyser : DiagnosticAnalyzer
{
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
        ImmutableArray.Create(OpcUaDiagnostics.MustBeSupportedTypeDescriptor);

    private static readonly ImmutableHashSet<SyntaxKind> SUPPORTED_SIMPLE_TYPES = ImmutableHashSet.CreateRange([
        SyntaxKind.BoolKeyword,
        SyntaxKind.ByteKeyword,
        SyntaxKind.ShortKeyword,
        SyntaxKind.UShortKeyword,
        SyntaxKind.IntKeyword,
        SyntaxKind.UIntKeyword,
        SyntaxKind.LongKeyword,
        SyntaxKind.ULongKeyword,
        SyntaxKind.FloatKeyword,
        SyntaxKind.DoubleKeyword,
        SyntaxKind.StringKeyword
    ]);
    
    private static readonly ImmutableHashSet<SpecialType> SUPPORTED_SPECIAL_TYPES = ImmutableHashSet.CreateRange([
        SpecialType.System_Boolean,
        SpecialType.System_Byte,
        SpecialType.System_Int16,
        SpecialType.System_UInt16,
        SpecialType.System_Int32,
        SpecialType.System_UInt32,
        SpecialType.System_Int64,
        SpecialType.System_UInt64,
        SpecialType.System_Single,
        SpecialType.System_Double,
        SpecialType.System_String,
        SpecialType.System_DateTime
    ]);
    
    private static readonly ImmutableHashSet<string> SUPPORTED_ENUMERABLE_NAMES = ImmutableHashSet.CreateRange
    ([
        "IList<>",
        "ICollection<>",
        "IEnumerable<>",
        "List<>",
        "ISet<>",
        "HashSet<>",
        "SortedSet<>",
        "SortedList<>"
    ]);
    
    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();
        context.RegisterSyntaxNodeAction(AnalyzeSyntax, SyntaxKind.ClassDeclaration);
        context.RegisterSyntaxNodeAction(AnalyzeSyntax, SyntaxKind.RecordDeclaration);
    }
    
    private static void AnalyzeSyntax(SyntaxNodeAnalysisContext context)
    {
        if (context.Node is not TypeDeclarationSyntax typeSyntax ||
            !typeSyntax.IsAnnotatedAsOpcUaEntity(context.SemanticModel))
            return;

        var properties = typeSyntax.Members
            .OfType<PropertyDeclarationSyntax>()
            .Where(property => !IsSupported(property, context.SemanticModel));

        foreach (var p in properties) context.ReportDiagnostic(OpcUaDiagnostics.MustBeSupportedType(p));
    }

    private static bool IsSupported(PropertyDeclarationSyntax property, SemanticModel semanticModel)
    {
        var typeSyntax = property.Type;
        var syntaxKind = typeSyntax.Kind();
        if (SUPPORTED_SIMPLE_TYPES.Contains(syntaxKind)) return true;

        var typeInfo = semanticModel.GetTypeInfo(property.Type).Type;
        if (typeInfo == null) return false;
        if (SUPPORTED_SPECIAL_TYPES.Contains(typeInfo.SpecialType)) return true;
        return IsSupportedEnumerable(typeInfo);
    }

    private static bool IsSupportedEnumerable(ITypeSymbol typeInfo)
    {
        if (typeInfo is INamedTypeSymbol { IsGenericType: true, TypeArguments.Length: 1 } namedTypeSymbol)
        {
            //Get the generic argument of IEnumerable<T> - that argument must be supported
            var genericParam = namedTypeSymbol.TypeArguments[0];
            var collectionTypeName = GetCollectionNameSpan(namedTypeSymbol).ToString();
            
            var isSupportedGenericArgument = SUPPORTED_SPECIAL_TYPES.Contains(genericParam.SpecialType);
            //get the XXXX<T> - XXXX must be supported
            var isSupportedEnumerable = SUPPORTED_ENUMERABLE_NAMES.Contains(collectionTypeName);
            if (isSupportedGenericArgument && isSupportedEnumerable) return true;
        }

        return false;
    }

    private static ReadOnlySpan<char> GetCollectionNameSpan(INamedTypeSymbol namedTypeSymbol)
    {
        var span = namedTypeSymbol.ConstructUnboundGenericType().ToString().AsSpan();
        var lastDot = span.LastIndexOf('.') + 1; //even if no . then it returns index 0! : )

        var startIndex = span.Slice(lastDot).IndexOf('<');
        var endIndex = span.IndexOf('>');
        var upToBracket = span.Slice(lastDot, startIndex + 1);
        var skippedTypeArgument = span.Slice(endIndex);

        var collectionTypeName = new ReadOnlySpan<char>([..upToBracket, ..skippedTypeArgument]);
        return collectionTypeName;
    }
}

