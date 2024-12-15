using Hoeyer.Machines.OpcUa.Configuration.Entities.Configuration;
using Opc.Ua;

namespace MyOpcUaWebApplication.Sample;

public interface INodeParser<out T>
{
    T? Parse( PropertyConfiguration configuration, Node nodeId );
}