namespace Hoeyer.OpcUa.Test.TUnit;

public static class ObjectLoading
{
    public static T GetOrCreateInstance<T>(Type testClassType, Func<T> instanceFactory) where T : class =>
        SharedDataSources.GetOrCreate(
            SharedType.PerTestSession, testClassType, null, instanceFactory);
}