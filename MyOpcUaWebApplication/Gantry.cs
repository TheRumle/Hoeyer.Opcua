using Hoeyer.Machines.OpcUa.Domain;

namespace MyOpcUaWebApplication;

[OpcUaEntity]
public record Gantry
{
    public Gantry()
    {
        
    }

    public string Name { get; init; }
    public int Id { get; init; }
    public int Speed { get; init; }

    public void Deconstruct(out string Name, out int Id, out int Speed)
    {
        Name = this.Name;
        Id = this.Id;
        Speed = this.Speed;
    }
}