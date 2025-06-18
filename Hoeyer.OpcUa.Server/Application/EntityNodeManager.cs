using System;
using System.Collections.Generic;
using System.Linq;
using Hoeyer.OpcUa.Core.Extensions.Logging;
using Hoeyer.OpcUa.Server.Api.NodeManagement;
using Microsoft.Extensions.Logging;
using Opc.Ua;
using Opc.Ua.Server;

namespace Hoeyer.OpcUa.Server.Application;

internal sealed class EntityNodeManager<T>(
    IManagedEntityNode<T> managedEntity,
    IServerInternal server,
    ILogger logger)
    : CustomNodeManager(server, managedEntity.Namespace), IEntityNodeManager<T>
{
    public IManagedEntityNode ManagedEntity { get; } = managedEntity;

    public override void CreateAddressSpace(IDictionary<NodeId, IList<IReference>> externalReferences)
    {
        using IDisposable? scope = logger.BeginScope(ManagedEntity.ToLoggingObject());
        logger.Log(LogLevel.Information, "Creating address space");
        try
        {
            InitializeNode(externalReferences);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Failed to create address space for entity");
        }
    }

    /// <inheritdoc />
    public override void Browse(OperationContext context, ref ContinuationPoint continuationPoint,
        IList<ReferenceDescription> references)
    {
        using (IDisposable? scope = logger.BeginScope(new
               {
                   context.AuditEntryId,
                   context.SessionId,
                   context.RequestId,
                   User = new
                   {
                       context.UserIdentity.DisplayName,
                       Roles = string.Join(", ",
                           context.UserIdentity.GrantedRoleIds.Select(e => e.Identifier.ToString()))
                   }
               }))
        {
            logger.LogInformation("Browsing node");
            base.Browse(context, ref continuationPoint, references);
        }
    }

    private void InitializeNode(IDictionary<NodeId, IList<IReference>> externalReferences)
    {
        lock (Lock)
        {
            BaseObjectState node = ManagedEntity.BaseObject;
            AddPredefinedNode(SystemContext, ManagedEntity.BaseObject);
            if (!externalReferences.TryGetValue(ObjectIds.ObjectsFolder, out IList<IReference>? references))
            {
                references ??= new List<IReference>();
                externalReferences[ObjectIds.ObjectsFolder] = references;
            }

            references.Add(new NodeStateReference(ReferenceTypeIds.Organizes, false, node.NodeId));

            foreach (MethodState? method in ManagedEntity.Methods)
            {
                method.OnCallMethod += (context, method, arguments, results) =>
                {
                    logger.LogInformation("Session {@Session} invoked method {@Method}",
                        context.SessionId,
                        method.ToLoggingObject(arguments));

                    return ServiceResult.Good;
                };
            }
        }
    }
}