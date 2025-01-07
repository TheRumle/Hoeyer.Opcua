namespace Hoeyer.OpcUa.Entity.Generation.Test;

internal record struct GeneratorTestDriverOptions(bool AssertCorrectInputCode)
{


    public static GeneratorTestDriverOptions Default = new(false);
}