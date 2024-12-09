using Hoeyer.Machines.OpcUa.Configuration.Entity.Context;
using Opc.Ua;

namespace MyOpcUaWebApplication.Sample;

public interface INodeParser<out T>
{
    T? Parse( PropertyConfiguration builder, Node nodeId );
}