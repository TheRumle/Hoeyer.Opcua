using System;
using System.Collections.Generic;
using System.Linq;
using Hoeyer.Common.Extensions;
using Hoeyer.OpcUa.Entity;
using Hoeyer.OpcUa.Server.Application.Node.Entity.Exceptions;
using Microsoft.Extensions.Logging;
using Opc.Ua;
using Opc.Ua.Server;

namespace Hoeyer.OpcUa.Server.Application.Node.Entity;

internal class EntityBrowser(EntityNode entityNode, EntityHandleManager handleManager, ILogger? logger = null)
    
{
    private readonly BaseObjectState _entity = entityNode.Entity;

    public EntityBrowser(EntityNode entityNode, ILogger? logger = null):this(entityNode, new EntityHandleManager(entityNode, logger), logger)
    {}
    
    /// <inheritdoc />
    public IEnumerable<ReferenceDescription> Browse(
        ContinuationPoint continuationPoint,
        SystemContext systemContext)
    {
        if (continuationPoint == null) throw new ArgumentNullException(nameof(continuationPoint));

        var errorMessages = LogAndGetErrors(continuationPoint).ToList();
        if (errorMessages.Any()) throw new InvalidBrowseException(errorMessages.NewlineSeparated());
        
        var browser = continuationPoint.Data as INodeBrowser
                      ?? _entity.CreateBrowser(systemContext,
                          continuationPoint.View,
                          continuationPoint.ReferenceTypeId,
                          continuationPoint.IncludeSubtypes,
                          continuationPoint.BrowseDirection,
                          null,
                          null,
                          false);
        
        return new ReferenceEnumerator(browser, continuationPoint, handleManager);
    }

    private IEnumerable<string> LogAndGetErrors(ContinuationPoint continuationPoint)
    {
        if (!ViewDescription.IsDefault(continuationPoint.View))
        {
            var m = "The continuation point is not a default view, and no other views are supported.";
            logger?.LogError(m);
            yield return m;
        }

        if (!handleManager.IsEntityHandle(continuationPoint.NodeToBrowse))
        {
            logger?.LogError(
                "The entity manager cannot browse node with handle {@Handle} as the handle does not associate with a managed entity.",
                continuationPoint.NodeToBrowse);
            yield return
                $"The entity manager cannot browse node with handle {continuationPoint.NodeToBrowse} as the handle does not associate with a managed entity.";
        }
    }
}