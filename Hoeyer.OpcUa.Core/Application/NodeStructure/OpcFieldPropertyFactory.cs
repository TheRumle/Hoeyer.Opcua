using System.Reflection;
using Hoeyer.OpcUa.Core.Abstractions.NodeStructure;
using Hoeyer.OpcUa.Core.Application.OpcTypeMappers;
using Opc.Ua;

namespace Hoeyer.OpcUa.Core.Application.NodeStructure;

public abstract class PropertyStateFactory : IPropertyStateFactory
{
    private readonly record struct PropertyStateCreationContext(
        OpcTypeInfo TypeInfo,
        BaseInstanceState Parent,
        NodeId NodeId,
        string BrowseName,
        int Rank);

    public PropertyState ConstructPropertyState(
        string browseName,
        PropertyInfo propertyInfo,
        BaseInstanceState parent)
    {
        var typeInfo = OpcTypeInfoUtils.ConstructOpcTypeInfo(propertyInfo.PropertyType);
        var nodeId = new NodeId(parent.BrowseName.Name! + "." + browseName, parent.NodeId.NamespaceIndex);
        var context = new PropertyStateCreationContext(typeInfo, parent, nodeId, browseName, typeInfo.Rank);
        return typeInfo.BuiltInType switch
        {
            BuiltInType.Boolean => Create<bool, VariantBuilder>(context),
            BuiltInType.SByte => Create<sbyte, VariantBuilder>(context),
            BuiltInType.Byte => Create<byte, VariantBuilder>(context),
            BuiltInType.Int16 => Create<short, VariantBuilder>(context),
            BuiltInType.UInt16 => Create<ushort, VariantBuilder>(context),
            BuiltInType.Int32 => Create<int, VariantBuilder>(context),
            BuiltInType.UInt32 => Create<uint, VariantBuilder>(context),
            BuiltInType.Int64 => Create<long, VariantBuilder>(context),
            BuiltInType.UInt64 => Create<ulong, VariantBuilder>(context),
            BuiltInType.Float => Create<float, VariantBuilder>(context),
            BuiltInType.Double => Create<double, VariantBuilder>(context),
            BuiltInType.String => Create<string, VariantBuilder>(context),
            BuiltInType.DateTime => Create<DateTimeUtc, VariantBuilder>(context),
            BuiltInType.Guid => Create<Guid, UuidConverter>(context),
            BuiltInType.ByteString => Create<ByteString, VariantBuilder>(context),
            BuiltInType.NodeId => Create<NodeId, VariantBuilder>(context),
            BuiltInType.StatusCode => Create<StatusCode, VariantBuilder>(context),
            BuiltInType.DataValue => Create<DataValue, VariantBuilder>(context),
            BuiltInType.Number => Create<double, VariantBuilder>(context),
            BuiltInType.Integer => Create<int, VariantBuilder>(context),
            BuiltInType.UInteger => Create<uint, VariantBuilder>(context),
            BuiltInType.Enumeration => CreateEnum(context, propertyInfo),
            _ => ThrowNotSupported(typeInfo)
        };
    }

    private PropertyState CreateEnum(PropertyStateCreationContext context, PropertyInfo propertyInfo)
    {
        var inner = HandleEnumeration(context.Parent, context.BrowseName, propertyInfo);
        return WithDefaultValues(
            context.TypeInfo,
            context.NodeId,
            context.BrowseName,
            inner);
    }

    protected abstract PropertyState HandleEnumeration(
        BaseInstanceState parent,
        string browseName,
        PropertyInfo propertyInfo);

    private static PropertyState Create<T, TBuilder>(
        PropertyStateCreationContext context
    ) where TBuilder : struct, IVariantBuilder<T> =>
        WithDefaultValues(context.TypeInfo, context.NodeId, context.BrowseName,
            PropertyState<T>.With<TBuilder>(context.Parent));


    private static PropertyState ThrowNotSupported(OpcTypeInfo typeInfo) =>
        throw new InvalidOperationException(
            $"The {nameof(BuiltInType)} {Enum.GetName(typeof(BuiltInType), typeInfo.BuiltInType)}" +
            $"(value = {typeInfo.BuiltInType}) is not supported as property value"
        );

    public static PropertyState WithDefaultValues(
        OpcTypeInfo typeInfo,
        NodeId nodeId,
        string browseName,
        PropertyState state
    )
    {
        var name = new QualifiedName(browseName);
        state.NodeId = nodeId;
        state.BrowseName = name;
        state.DataType = typeInfo.TypeId;
        state.ValueRank = typeInfo.Rank;
        state.TypeDefinitionId = VariableTypeIds.PropertyType;
        state.SymbolicName = browseName;
        state.AccessLevel = AccessLevels.CurrentReadOrWrite;
        state.UserAccessLevel = AccessLevels.CurrentReadOrWrite;
        state.MinimumSamplingInterval = MinimumSamplingIntervals.Indeterminate;
        state.ReferenceTypeId = ReferenceTypeIds.HasProperty;
        state.DisplayName = new LocalizedText(browseName);
        return state;
    }
}