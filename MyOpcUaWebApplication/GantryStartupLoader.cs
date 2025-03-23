using Hoeyer.OpcUa.Core.Entity;

namespace MyOpcUaWebApplication;

public class GantryStartupLoader : IEntityStartupLoader<Gantry>
{
    /// <inheritdoc />
    public Gantry LoadStartUpState()
    {
        return new Gantry
        {
            Speeds = new List<int>
            {
                21, 32, 12, 3
            },
            messages = new HashSet<int>
            {
                2131, 11, 22
            },
            message = "OPCUA SAYS HELLO"
        };
    }
}