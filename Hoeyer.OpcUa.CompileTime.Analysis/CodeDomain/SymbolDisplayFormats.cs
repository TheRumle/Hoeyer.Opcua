using Microsoft.CodeAnalysis;

namespace Hoeyer.OpcUa.CompileTime.Analysis.CodeDomain;

public static class SymbolDisplayFormats
{
    public static readonly SymbolDisplayFormat FullyQualifiedNonGenericWithGlobalPrefix = new(
        SymbolDisplayGlobalNamespaceStyle.Included,
        SymbolDisplayTypeQualificationStyle.NameAndContainingTypesAndNamespaces,
        SymbolDisplayGenericsOptions.None,
        SymbolDisplayMemberOptions.IncludeContainingType,
        SymbolDisplayDelegateStyle.NameAndSignature,
        SymbolDisplayExtensionMethodStyle.Default,
        SymbolDisplayParameterOptions.IncludeType,
        SymbolDisplayPropertyStyle.NameOnly,
        SymbolDisplayLocalOptions.IncludeType, 
        SymbolDisplayKindOptions.None, 
        SymbolDisplayMiscellaneousOptions.IncludeNullableReferenceTypeModifier | SymbolDisplayMiscellaneousOptions.UseSpecialTypes
    );
    
}