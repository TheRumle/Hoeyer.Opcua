using System;
using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Opc.Ua;

namespace Hoeyer.OpcUa.Server.Generation.Configuration;

public static class OpcTypeDecider
{

    private static Dictionary<SyntaxKind, uint> SUPPORTED_DATATYPES = new(){
        {SyntaxKind.BoolKeyword, DataTypes.Boolean},
        {SyntaxKind.ByteKeyword, DataTypes.Byte},
        {SyntaxKind.ShortKeyword, DataTypes.Int16},
        {SyntaxKind.UShortKeyword, DataTypes.UInt16},
        {SyntaxKind.IntKeyword, DataTypes.Int32},
        {SyntaxKind.UIntKeyword, DataTypes.UInt32},
        {SyntaxKind.LongKeyword, DataTypes.Int64},
        {SyntaxKind.ULongKeyword, DataTypes.UInt64},
        {SyntaxKind.FloatKeyword, DataTypes.Float},
        {SyntaxKind.DoubleKeyword, DataTypes.Double},
        {SyntaxKind.StringKeyword, DataTypes.String}
    };

    public static (string type, uint opcUaTypeId) TypeString(TypeSyntax type, SemanticModel semanticModel)
    {
        if (SUPPORTED_DATATYPES.ContainsKey(type.Kind()))
        {
            return (type.ToString(), SUPPORTED_DATATYPES[type.Kind()]);
        }

        if (type is IdentifierNameSyntax or QualifiedNameSyntax)
        {
            var identifierName = type as NameSyntax;
            var typeSymbol = semanticModel.GetTypeInfo(identifierName!).Type;
            if (typeSymbol != null)
            {
                if (typeSymbol.SpecialType == SpecialType.System_DateTime)
                    return (typeSymbol.Name, DataTypes.DataValue);
                if (typeSymbol.ToDisplayString() == "System.Guid" || typeSymbol.Name == "Guid") 
                    return (typeSymbol.Name, DataTypes.Guid);
            }
        }
        
        throw new NotSupportedException($"The datatype {type.ToFullString()} is not supported.");

    }
}