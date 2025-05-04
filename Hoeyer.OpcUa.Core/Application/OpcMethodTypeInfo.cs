using System;
using System.Linq;
using Hoeyer.OpcUa.Core.Extensions.Reflection;
using Opc.Ua;

namespace Hoeyer.OpcUa.Core.Application;

internal sealed record OpcMethodTypeInfo : IOpcTypeInfo
{
    public OpcMethodTypeInfo(string methodName, (Type type, string name)[] arguments, Type returnType,
        BaseObjectState parent)
    {
        var opcMethodName = parent.BrowseName.Name + "." + methodName;
        var method = new MethodState(parent)
        {
            NodeId = new NodeId(opcMethodName, parent.NodeId.NamespaceIndex),
            SymbolicName = opcMethodName,
            BrowseName = opcMethodName,
            DisplayName= opcMethodName,
            AccessRestrictions = AccessRestrictionType.None,
            Executable = true,
            UserExecutable = true,
        };

        var methodArguments = arguments.Select(e =>
        {
            var (id, rank) = TypeExtensions.GetOpcTypeInfo(e.type);
            return new Argument
            {
                Name = e.name,
                DataType = id,
                ValueRank = rank
            };
        }).ToArray();

        method.InputArguments = new PropertyState<Argument[]>(method)
        {
            NodeId = new NodeId(methodName+"InputArgs", parent.NodeId.NamespaceIndex),
            BrowseName = BrowseNames.InputArguments,
            TypeDefinitionId = VariableTypeIds.PropertyType,
            DataType = DataTypeIds.Argument,
            ValueRank = ValueRanks.OneDimension,
            Value = methodArguments
        };

        var (typeId, valueRank) = returnType.GetOpcTypeInfo();
        var outputArgument = new PropertyState<Argument[]>(method)
        {
            NodeId = new NodeId(methodName+"Result", parent.NodeId.NamespaceIndex),
            BrowseName = BrowseNames.OutputArguments,
            TypeDefinitionId = VariableTypeIds.PropertyType,
            DataType = DataTypeIds.Argument,
            ValueRank = ValueRanks.Scalar,
            Value = [new Argument
            {
                Name = "Result",
                DataType = typeId,
                ValueRank = valueRank
            }]
        };

        method.OutputArguments = outputArgument;
        InstanceState = method;
    }

    public BaseInstanceState InstanceState { get; }
}