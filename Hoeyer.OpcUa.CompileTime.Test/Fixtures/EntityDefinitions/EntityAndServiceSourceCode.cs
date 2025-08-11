namespace Hoeyer.OpcUa.CompileTime.Test.Fixtures.EntityDefinitions;

/// <summary>
/// Represent source code for both an Entity and an accompagnying service that depends on the source code definition of the Entity. 
/// </summary>
/// <param name="ServiceName">The name of the interface</param>
/// <param name="EntityName">The name of the entity</param>
/// <param name="CombinedSourceCode"></param>
public sealed record EntityAndServiceSourceCode(
    string ServiceName,
    string EntityName,
    string EntitySourceCode,
    string ServiceSourceCode)
{
    /// <inheritdoc />
    public override string ToString() => ServiceName + " and " + EntityName;
}