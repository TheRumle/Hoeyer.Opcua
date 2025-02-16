using Hoeyer.OpcUa.Entity;
using Opc.Ua;

namespace MyOpcUaWebApplication;

internal record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
    
    public EntityNode CreateEntityOpcUaNode(FolderState root, ushort applicationNamespaceIndex)
    {
        
        BaseObjectState GantryNode = new BaseObjectState(root)
        {
            BrowseName =  new QualifiedName("Gantry", applicationNamespaceIndex),
            NodeId = new NodeId(Guid.NewGuid(), applicationNamespaceIndex),
            DisplayName = "Gantry",
        };
        GantryNode.AccessRestrictions = AccessRestrictionType.None;
       
        root.AddChild(GantryNode);
           
        //Assign properties
        PropertyState speed = GantryNode.AddProperty<int>("speed", DataTypes.Int32, ValueRanks.OneDimension);
        speed.AccessLevel = AccessLevels.CurrentRead | AccessLevels.CurrentReadOrWrite;
        speed.NodeId = new NodeId(Guid.NewGuid(), applicationNamespaceIndex);
           
        return new EntityNode(root, GantryNode, new List<PropertyState>()
        {
            speed
        });
    }
}