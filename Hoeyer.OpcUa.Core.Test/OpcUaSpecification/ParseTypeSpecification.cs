using Opc.Ua;

namespace Hoeyer.OpcUa.Core.Test.OpcUaSpecification;

public sealed class ParseTypeSpecification
{
    [Test]
    [MethodDataSource<ParseTypeSpecification>(nameof(TypeToNodeIdSource))]
    public async Task HowTo_Parse_NativeTypeTo_TypeId(Type t, NodeId expectedNodeId)
    {
        NodeId x = TypeInfo.GetDataTypeId(t);
        var rank = TypeInfo.GetValueRank(t);
        await Assert.That(x).IsEquatableOrEqualTo(expectedNodeId);
        await Assert.That(rank).IsEqualTo(ValueRanks.Scalar);
    }

    [Test]
    [MethodDataSource<ParseTypeSpecification>(nameof(TypeToNodeIdSource))]
    public async Task HowTo_Parse_ListTypesWithNativeArgument_ToTypeId(Type t, NodeId expectedNodeId)
    {
        var collectionInterface = t.GetInterfaces().FirstOrDefault(i =>
            i.IsGenericType &&
            i.GetGenericTypeDefinition() == typeof(ICollection<>));

        if (collectionInterface is not null)
        {
            var gottenId = TypeInfo.GetDataTypeId(collectionInterface.GenericTypeArguments[0]);
            var valueRank = TypeInfo.GetValueRank(t);
            await Assert.That(gottenId).IsEquatableOrEqualTo(expectedNodeId);
            await Assert.That(valueRank).IsEqualTo(ValueRanks.OneDimension);
        }
    }

    public static IEnumerable<Func<(Type t, NodeId expectedNodeId)>> TypeToNodeIdSource()
    {
        return
        [
            () => (typeof(Test), DataTypes.Enumeration),
            () => (typeof(bool), DataTypeIds.Boolean),
            () => (typeof(byte), DataTypeIds.Byte),
            () => (typeof(short), DataTypeIds.Int16),
            () => (typeof(ushort), DataTypeIds.UInt16),
            () => (typeof(int), DataTypeIds.Int32),
            () => (typeof(uint), DataTypeIds.UInt32),
            () => (typeof(long), DataTypeIds.Int64),
            () => (typeof(ulong), DataTypeIds.UInt64),
            () => (typeof(float), DataTypeIds.Float),
            () => (typeof(double), DataTypeIds.Double),
            () => (typeof(string), DataTypeIds.String),
            () => (typeof(DateTime), DataTypeIds.DateTime)
        ];
    }

    public static IEnumerable<Func<(Type t, NodeId expectedNodeId)>> GenericListToNodeIdSource()
    {
        return
        [
            () => (typeof(List<Test>), DataTypes.Enumeration),
            () => (typeof(List<bool>), DataTypeIds.Boolean),
            () => (typeof(List<byte>), DataTypeIds.Byte),
            () => (typeof(List<short>), DataTypeIds.Int16),
            () => (typeof(List<ushort>), DataTypeIds.UInt16),
            () => (typeof(List<int>), DataTypeIds.Int32),
            () => (typeof(List<uint>), DataTypeIds.UInt32),
            () => (typeof(List<long>), DataTypeIds.Int64),
            () => (typeof(List<ulong>), DataTypeIds.UInt64),
            () => (typeof(List<float>), DataTypeIds.Float),
            () => (typeof(List<double>), DataTypeIds.Double),
            () => (typeof(List<string>), DataTypeIds.String),
            () => (typeof(List<DateTime>), DataTypeIds.DateTime)
        ];
    }

    private enum Test
    {
        DA,
        EN
    }
}