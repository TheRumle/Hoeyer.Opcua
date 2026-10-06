namespace Hoeyer.OpcUa.Fixtures.Server.FixtureKeys;

public static class TestKeys
{
    public const string PER_ASSEMBLY_KEY = "Assembly-scoped-environment";
    public static readonly string PerTestSessionKey = TestSessionContext.Current!.Id;
}