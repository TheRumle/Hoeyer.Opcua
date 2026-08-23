using System.Diagnostics.CodeAnalysis;
using Bogus;
using Hoeyer.OpcUa.Server.Abstractions;

namespace Hoeyer.OpcUa.IntegrationTest.Alarms;

public abstract class
    GeneratingLoader<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties)] T> : IEntityLoader<T>
    where T : class
{
    [UnconditionalSuppressMessage("AOT",
        "IL3050:Calling members annotated with 'RequiresDynamicCodeAttribute' may break functionality when AOT compiling.",
        Justification = "<Pending>")]
    private readonly Faker<T> _faker = BogusTestEntityGenerator.CreateFaker<T>();

    public ValueTask<T> LoadCurrentState() =>
        ValueTask.FromResult(_faker.Generate());
}