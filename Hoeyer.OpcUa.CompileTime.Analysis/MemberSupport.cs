using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Hoeyer.OpcUa.CompileTime.Analysis;

internal record struct MemberSupport(bool IsSupported, MemberDeclarationSyntax DeclarationSyntax, params IEnumerable<string> TypesWithError)
{
    public static MemberSupport Success(MemberDeclarationSyntax member) =>
        new MemberSupport(true, member);
    
    public static MemberSupport Failure(MemberDeclarationSyntax syntax, IEnumerable<TypeSyntax> types) =>
        new MemberSupport(false, syntax, types.Select(e=>e.ToString()));
    
    public static MemberSupport Failure(MemberDeclarationSyntax syntax, TypeSyntax types) =>
        new MemberSupport(false, syntax, types.ToString());

    public static MemberSupport Failure(MemberDeclarationSyntax eventSyntax, List<ITypeSymbol> errors) => new MemberSupport(false, eventSyntax, string.Join(", ", errors.Select(t => t.ToString())));
}