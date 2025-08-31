using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Hoeyer.OpcUa.Core.CompileTime;

internal record struct MemberTypeSupport(bool IsSupported, Location Location, params IEnumerable<string> TypesWithError)
{
    public static MemberTypeSupport Success(MemberDeclarationSyntax member) =>
        new MemberTypeSupport(true, member.GetLocation());

    public static MemberTypeSupport Failure(MemberDeclarationSyntax syntax, IEnumerable<TypeSyntax> types) =>
        new MemberTypeSupport(false, syntax.GetLocation(), types.Select(e => e.ToString()));

    public static MemberTypeSupport Failure(MemberDeclarationSyntax syntax, TypeSyntax types) =>
        new MemberTypeSupport(false, syntax.GetLocation(), types.ToString());


    public static MemberTypeSupport Failure(Location location, TypeSyntax types) =>
        new MemberTypeSupport(false, location, types.ToString());

    public static MemberTypeSupport Failure(MemberDeclarationSyntax syntax, List<ITypeSymbol> errors) =>
        new MemberTypeSupport(false, syntax.GetLocation(), string.Join(", ", errors.Select(t => t.ToString())));
}