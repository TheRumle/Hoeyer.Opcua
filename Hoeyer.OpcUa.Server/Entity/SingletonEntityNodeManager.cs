using Opc.Ua;
using Opc.Ua.Server;

namespace Hoeyer.OpcUa.Server.Entity;

internal sealed class SingletonEntityNodeManager (IEntityObjectStateCreator entityObjectCreator, 
    IServerInternal server, ApplicationConfiguration configuration) : CustomNodeManager(server, configuration){
    
    //TODO Find out when the object should be created and establish how to keep track of it.
    
}