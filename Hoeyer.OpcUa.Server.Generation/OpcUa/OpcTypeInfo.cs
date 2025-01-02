
using System;
using System.Collections.Generic;
using Hoeyer.OpcUa.Server.Generation.Diagnostics;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Opc.Ua;

namespace Hoeyer.OpcUa.Server.Generation.OpcUa;

public static class OpcTypeInfo
{

    private static readonly Dictionary<SpecialType, uint> SupportedDatatypes = new(){
        {SpecialType.System_Boolean, DataTypes.Boolean},
        {SpecialType.System_Byte, DataTypes.Byte},
        {SpecialType.System_Int16, DataTypes.Int16},
        {SpecialType.System_UInt16, DataTypes.UInt16},
        {SpecialType.System_Int32, DataTypes.Int32},
        {SpecialType.System_UInt32, DataTypes.UInt32},
        {SpecialType.System_Int64, DataTypes.Int64},
        {SpecialType.System_UInt64, DataTypes.UInt64},
        {SpecialType.System_Single, DataTypes.Float},
        {SpecialType.System_Double, DataTypes.Double},
        {SpecialType.System_String, DataTypes.String},
        {SpecialType.System_DateTime, DataTypes.DateTime},
        {SpecialType.System_Decimal, DataTypes.Decimal}
    };

    public static bool IsSupported(PropertyDeclarationSyntax property,  SemanticModel semanticModel)
    {
        var type = property.Type;
        var typeInfo = semanticModel.GetTypeInfo(type).Type;
        if (typeInfo == null || SymbolEqualityComparer.Default.Equals(typeInfo, null)) 
            return false;

        return SupportedDatatypes.TryGetValue(typeInfo.SpecialType, out var _);
    }
    
    public static OpcUaProperty PropertyInfo(PropertyDeclarationSyntax property, SemanticModel semanticModel)
    {
        if (!IsSupported(property, semanticModel)) throw new DiagnosticException(OpcUaDiagnostics.UnsupportedOpcUaType(property));
        
        return new OpcUaProperty(property.Identifier.ToFullString(), property.Type.ToFullString(), SupportedDatatypes[semanticModel.GetTypeInfo(property.Type).Type!.SpecialType]);
    }
}