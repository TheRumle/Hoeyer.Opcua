namespace Hoeyer.OpcUa.Fixtures.Server.FixtureKeys;

public static class ServerFixtureKeyExtractor
{
    public static string ExtractFixtureKey(
        SharedType? shared,
        string? key,
        string classTypeName,
        string testName)
    {
        var perTestSessionKey = TestSessionContext.Current!.Id;
        return (shared, key) switch
        {
            (null, null) => perTestSessionKey,
            (SharedType.PerTestSession, var _) => perTestSessionKey,
            (SharedType.Keyed, var k) => k!,
            (SharedType.PerClass, var _) => classTypeName,
            (SharedType.PerAssembly, var _) => "Assembly-scoped-environment",
            (SharedType.None, var _) => testName,
            var _ => throw new ArgumentOutOfRangeException(nameof(shared), shared, "The fixture key cannot be computed")
        };
    }
}