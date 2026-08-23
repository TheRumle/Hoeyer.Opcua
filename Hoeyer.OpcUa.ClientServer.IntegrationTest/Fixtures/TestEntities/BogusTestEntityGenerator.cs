using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Bogus;

namespace Hoeyer.OpcUa.IntegrationTest.Alarms;

public static class BogusTestEntityGenerator
{
    [RequiresDynamicCode(
        "This test data generator uses reflection and runtime generic type construction.")]
    [UnconditionalSuppressMessage("AOT",
        "IL2072:Calling members annotated with 'RequiresDynamicCodeAttribute' may break functionality when AOT compiling.")]
    public static Faker<TType> CreateFaker<
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties)]
        TType>()
        where TType : class
    {
        var faker = new Faker<TType>();

        foreach (var property in GetWritableProperties<TType>())
        {
            faker.RuleFor(
                property.Name,
                (f, _) => GenerateValue(f, property.PropertyType));
        }

        return faker;
    }

    [RequiresDynamicCode("Calls System.Reflection.MethodInfo.MakeGenericMethod(params Type[])")]
    private static object? GenerateValue(
        Faker faker,
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.Interfaces)]
        Type type)
    {
        if (Nullable.GetUnderlyingType(type) is { } underlyingType)
        {
            type = underlyingType;
        }

        object? numberType = ExtractDefaultType(faker, type);
        if (numberType is not null)
        {
            return numberType;
        }

        var specialStructType = ExtractSpecialStructType(faker, type);
        if (specialStructType is not null)
        {
            return specialStructType;
        }

        return type switch
        {
            _ when type.IsEnum => GenerateEnumValue(faker, type),
            _ when TryGetCollectionElementType(type, out var elementType) =>
                GenerateCollection(faker, type, elementType),
            _ => throw new NotSupportedException($"Type {type} is not supported")
        };
    }

    private static object? ExtractSpecialStructType(Faker faker, Type type)
    {
        object? specialStructType = type switch
        {
            _ when type == typeof(Guid) => Guid.NewGuid(),
            _ when type == typeof(DateTime) => faker.Date.Past(),
            _ when type == typeof(DateTimeOffset) => new DateTimeOffset(faker.Date.Past()),
            _ when type == typeof(TimeSpan) => TimeSpan.FromSeconds(faker.Random.Int(0, 100000)),
            _ when type == typeof(DateOnly) => DateOnly.FromDateTime(faker.Date.Past()),
            _ when type == typeof(TimeOnly) => TimeOnly.FromDateTime(faker.Date.Recent()),
            _ => null
        };
        return specialStructType;
    }

    private static object? ExtractDefaultType(Faker faker, Type type)
    {
        object? numberType = type switch
        {
            _ when type == typeof(string) => faker.Lorem.Word(),
            _ when type == typeof(int) => faker.Random.Int(),
            _ when type == typeof(long) => faker.Random.Long(),
            _ when type == typeof(short) => faker.Random.Short(),
            _ when type == typeof(byte) => faker.Random.Byte(),
            _ when type == typeof(float) => faker.Random.Float(),
            _ when type == typeof(double) => faker.Random.Double(),
            _ when type == typeof(decimal) => faker.Random.Decimal(),
            _ when type == typeof(bool) => faker.Random.Bool(),
            _ when type == typeof(char) => faker.Random.Char(),
            _ => null
        };
        return numberType;
    }

    private static IEnumerable<PropertyInfo> GetWritableProperties<
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties)]
        TType>()
    {
        return typeof(TType)
            .GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(property => property.CanWrite);
    }

    [RequiresDynamicCode("Calls System.Enum.GetValues(Type)")]
    private static object GenerateEnumValue(Faker faker, Type type)
    {
        var values = Enum.GetValues(type);
        return values.GetValue(
            faker.Random.Int(0, values.Length - 1))!;
    }

    [RequiresDynamicCode("Calls System.Reflection.MethodInfo.MakeGenericMethod(params Type[])")]
    private static object GenerateCollection(
        Faker faker,
        Type collectionType,
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.Interfaces)]
        Type elementType)
    {
        var count = faker.Random.Int(1, 5);

        var elements = Enumerable
            .Range(0, count)
            .Select(_ => GenerateValue(faker, elementType))
            .ToList();

        if (collectionType.IsArray)
            return CreateArray(elementType, elements);

        return collectionType.GetGenericTypeDefinition() switch
        {
            var type when type == typeof(List<>)
                          || type == typeof(IList<>)
                          || type == typeof(IEnumerable<>)
                          || type == typeof(ICollection<>)
                => CreateList(elementType, elements),

            var type when type == typeof(HashSet<>)
                          || type == typeof(ISet<>)
                => CreateHashSet(elementType, elements),

            _ => throw new NotSupportedException(
                $"Collection type {collectionType} is not supported")
        };
    }

    [RequiresDynamicCode("Calls System.Array.CreateInstance(Type, Int32)")]
    private static Array CreateArray(
        Type elementType,
        IReadOnlyList<object?> elements)
    {
        var array = Array.CreateInstance(elementType, elements.Count);

        for (var i = 0; i < elements.Count; i++)
            array.SetValue(elements[i], i);

        return array;
    }

    [RequiresDynamicCode("Calls System.Type.MakeGenericType(params Type[])")]
    private static object CreateList(
        Type elementType,
        IReadOnlyList<object?> elements)
    {
        var listType = typeof(List<>).MakeGenericType(elementType);

        return Activator.CreateInstance(listType, elements)!;
    }

    [RequiresDynamicCode("Calls System.Type.MakeGenericType(params Type[])")]
    private static object CreateHashSet(
        Type elementType,
        IReadOnlyList<object?> elements)
    {
        var setType = typeof(HashSet<>).MakeGenericType(elementType);

        return Activator.CreateInstance(setType, elements)!;
    }

    private static bool TryGetCollectionElementType(
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.Interfaces)]
        Type type,
        out Type elementType)
    {
        if (type.IsArray)
        {
            elementType = type.GetElementType()!;
            return true;
        }

        var enumerable = type
            .GetInterfaces()
            .Append(type)
            .FirstOrDefault(interfaceType =>
                interfaceType.IsGenericType &&
                interfaceType.GetGenericTypeDefinition() == typeof(ICollection<>));

        if (enumerable is not null)
        {
            elementType = enumerable.GetGenericArguments()[0];
            return true;
        }

        elementType = null!;
        return false;
    }
}