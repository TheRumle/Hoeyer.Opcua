using Hoeyer.OpcUa.Core.Entity;

namespace MyOpcUaWebApplication;

public class GantryLoader : IEntityLoader<Gantry>
{
    /// <inheritdoc />
    public ValueTask<Gantry> LoadCurrentState()
    {
        return ValueTask.FromResult(new Gantry
        {
         
        });
    }
}