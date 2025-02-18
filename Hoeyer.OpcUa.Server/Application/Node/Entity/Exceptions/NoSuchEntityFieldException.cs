using Hoeyer.OpcUa.Entity;
using Opc.Ua;

namespace Hoeyer.OpcUa.Server.Application.Node.Entity.Exceptions;

public class NoSuchEntityFieldException : EntityNodeManagementException
{
    public NoSuchEntityFieldException(IEntityNode entity, ExpandedNodeId referenceId) 
        : base($"The entity {entity.Entity.DisplayName} does not hold a reference with id {referenceId}.")
    {
        
    }
}