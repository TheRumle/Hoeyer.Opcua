using System.Reflection;
using Hoeyer.OpcUa.Core.Abstractions.NodeStructure;
using Hoeyer.OpcUa.Core.Application.OpcTypeMappers;
using Opc.Ua;

namespace Hoeyer.OpcUa.Core.Application.NodeStructure;

public sealed class MethodStateFactory : IMethodStateFactory
{
    public MethodState ConstructMethodState(
        BaseObjectState parent, string methodName,
        IEnumerable<ParameterInfo> arguments,
        Type? returnType
    )
    {
        var opcMethodName = methodName;
        var method = new MethodState(parent)
        {
            NodeId = new NodeId(opcMethodName, parent.NodeId.NamespaceIndex),
            SymbolicName = opcMethodName,
            BrowseName = new QualifiedName(opcMethodName),
            DisplayName = new LocalizedText(opcMethodName),
            AccessRestrictions = AccessRestrictionType.None,
            Executable = true,
            UserExecutable = true
        };

        AssignInputArguments(methodName,  arguments, parent, method);
        if (returnType is not null)
        {
            AssignReturnValueNode(methodName, returnType, parent, method);
        }

        return method;
    }

    private static void AssignInputArguments(string methodName,
        IEnumerable<ParameterInfo> arguments,
        BaseObjectState parent,
        MethodState method)
    {
        

        var property = PropertyState<ArrayOf<Argument>>.With<StructureBuilder<Argument>>(method);
        property.NodeId = new NodeId(methodName + "InputArgs", parent.NodeId.NamespaceIndex);
        property.BrowseName = new QualifiedName(BrowseNames.InputArguments);
        property.DisplayName = new LocalizedText(BrowseNames.InputArguments);
        property.TypeDefinitionId = VariableTypeIds.PropertyType;
        property.DataType = DataTypeIds.Argument;
        property.ValueRank = ValueRanks.OneDimension;
        
        Argument[] inputArguments = arguments.Select(arg =>
        {
            var data = OpcTypeInfoUtils.ConstructOpcTypeInfo(arg.ParameterType);

            return new Argument
            {
                Name = arg.Name,
                DataType = data.TypeId,
                ValueRank = data.Rank
            };
        }).ToArray();
        property.Value = new ArrayOf<Argument>(inputArguments);
        

        method.AddReference(ReferenceTypeIds.HasProperty, false, property.NodeId);
        method.InputArguments = property;
    }

    private static void AssignReturnValueNode(string methodName, Type returnType, BaseObjectState parent, MethodState method)
    {
        var (_, valueRank, typeId) = OpcTypeInfoUtils.ConstructOpcTypeInfo(returnType);

        Argument[] value =
        [
            new()
            {
                Name = "Result",
                DataType = typeId,
                ValueRank = valueRank
            }
        ];


        var property = PropertyState<ArrayOf<Argument>>.With<StructureBuilder<Argument>>(method);
        property.Value = new ArrayOf<Argument>(value);
        property.NodeId = new NodeId(
            methodName + "Result",
            parent.NodeId.NamespaceIndex);

        property.BrowseName = new QualifiedName(BrowseNames.OutputArguments);
        property.DisplayName = new LocalizedText(BrowseNames.OutputArguments);
        property.TypeDefinitionId = VariableTypeIds.PropertyType;
        property.DataType = DataTypeIds.Argument;
        property.ValueRank = ValueRanks.Scalar;
        method.OutputArguments = property;
        method.AddReference(ReferenceTypeIds.HasProperty, false, property.NodeId);
    }
}