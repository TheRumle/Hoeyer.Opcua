namespace Hoeyer.Machines.OpcUa.ResourceLoading;

internal record LoadableType(string TypeDefinition, string FileName)
{
    public string TypeDefinition = TypeDefinition;
    public string FileName = FileName;
}