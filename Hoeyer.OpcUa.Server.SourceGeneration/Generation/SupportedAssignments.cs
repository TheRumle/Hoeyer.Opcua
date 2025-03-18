using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Hoeyer.OpcUa.CompileTime.Analysis;
using Hoeyer.OpcUa.CompileTime.Analysis.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Hoeyer.OpcUa.Server.SourceGeneration.Generation;

public static class SupportedAssignments
{
    public static class SimpleTypes
    {
        private const string DEFAULT_NUMERIC = "0";
        private const string DEFAULT_NUMERIC_DECIMAL = "0.0";

        private static readonly ImmutableDictionary<SpecialType, string> SpecialTypeToDefaultValue =
            new Dictionary<SpecialType, string>()
            {
                { SpecialType.System_Boolean, "false" },
                { SpecialType.System_Byte, DEFAULT_NUMERIC },
                { SpecialType.System_Int16, DEFAULT_NUMERIC },
                { SpecialType.System_UInt16, DEFAULT_NUMERIC },
                { SpecialType.System_Int32, DEFAULT_NUMERIC },
                { SpecialType.System_UInt32, DEFAULT_NUMERIC },
                { SpecialType.System_Int64, DEFAULT_NUMERIC },
                { SpecialType.System_UInt64, DEFAULT_NUMERIC },
                { SpecialType.System_Single, DEFAULT_NUMERIC_DECIMAL + "f" },
                { SpecialType.System_Double, DEFAULT_NUMERIC_DECIMAL },
                { SpecialType.System_String, string.Empty }
            }.ToImmutableDictionary();
        
        public static LiteralExpressionSyntax? GetNewDefaultCollectionForType(ITypeSymbol? typeSymbol)
        {
            if (typeSymbol == null) return null;
            if (SupportedTypes.Simple.Supports(typeSymbol.SpecialType))
            {
                var specialTypeString = SpecialTypeToDefaultValue[typeSymbol.SpecialType];
                var expression = SyntaxFactory.ParseExpression(specialTypeString);
                if (expression is not LiteralExpressionSyntax literal) throw new ArgumentException($"Unsupported literal type: {specialTypeString}");
                return literal;
            }

            return null;
        }
    }
    
    public static class CollectionTypes
    {
        private const string COLLECTION_DEFAULT_INSTANTIATION = "new System.Collections.Generic.List<>()";
        private const string SET_DEFAULT_INSTANTIATION = "new System.Collections.Generic.HashSet<>()";
        private static readonly ImmutableDictionary<string, string> GenericInstantiationExpressions =
            new Dictionary<string, string>
            {
                
                {"System.Collections.Generic.IList<>", COLLECTION_DEFAULT_INSTANTIATION},
                {"System.Collections.Generic.ICollection<>", COLLECTION_DEFAULT_INSTANTIATION},
                {"System.Collections.Generic.IEnumerable<>", COLLECTION_DEFAULT_INSTANTIATION},
                {"System.Collections.Generic.List<>", COLLECTION_DEFAULT_INSTANTIATION},
                {"System.Collections.Generic.ISet<>",SET_DEFAULT_INSTANTIATION},
                {"System.Collections.Generic.HashSet<>", SET_DEFAULT_INSTANTIATION},
                {"System.Collections.Generic.SortedSet<>", "new System.Collections.Generic.SortedSet<>()"},
                {"System.Collections.Generic.SortedList<>", "new System.Collections.Generic.SortedList<>()"},
            }.ToImmutableDictionary();

        
        private static  ImmutableHashSet<string> PossibleGenericCollectionsInstantiation()
        {
     
            return SupportedTypes.Simple
                .SpecialTypes
                .SelectMany(simpleType => GenericInstantiationExpressions
                    .Select(e => e.Value
                        .Replace("<>", $"<{simpleType}>")))
                .ToImmutableHashSet();
        }



        private static readonly ImmutableDictionary<string, ObjectCreationExpressionSyntax> PossibleGenericCollections =
            GetObjectCreationExpressions();

        public static ObjectCreationExpressionSyntax? GetNewDefaultCollectionForType(ITypeSymbol? typeSymbol)
        {
            switch (typeSymbol)
            {
                case null: return null;
                case INamedTypeSymbol { IsGenericType: true, Arity: 1 } namedType:
                {
                    var genericType = namedType.TypeArguments.First();
                    return PossibleGenericCollections[genericType.OriginalDefinition.ToFullyQualifiedTypeName()];
                }
                default: return null;
            }
        } 
        
        private static ImmutableDictionary<string, ObjectCreationExpressionSyntax> GetObjectCreationExpressions()
        {
            var allSupportedCollections = PossibleGenericCollectionsInstantiation();
            Dictionary<string, ObjectCreationExpressionSyntax> newInstanceCollection = new();
            foreach (var supportedCollection in allSupportedCollections)
            {
                if (SyntaxFactory.ParseExpression(supportedCollection) is not ObjectCreationExpressionSyntax expressionSyntax)
                    throw new ArgumentException($"Could not parse {supportedCollection} to {nameof(ObjectCreationExpressionSyntax)}" );
                
                newInstanceCollection[supportedCollection] = expressionSyntax;
            }

            return newInstanceCollection.ToImmutableDictionary();
        }
    }
}