using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using FluentResults;
using Opc.Ua;
using Opc.Ua.Server;

namespace Hoeyer.OpcUa.Server.Application.EntityNode.Operations;

internal class EntityBrowser(ServerSystemContext context)
    : IEntityBrowser
{
    /// <inheritdoc />
    public IEnumerable<ReferenceDescription> Browse(ContinuationPoint continuationPoint, IEntityNodeHandle nodeToBrowse)
    {
        INodeBrowser browser = CreateBrowser(continuationPoint, nodeToBrowse.HandledNode);
        return BrowseAll(continuationPoint, nodeToBrowse, browser)
            .Skip(continuationPoint.Index);
    }
    private static IEnumerable<ReferenceDescription> BrowseAll(ContinuationPoint continuationPoint, IEntityNodeHandle nodeToBrowse,
        INodeBrowser browser)
    {
        var i = 1;
        var resultMask = continuationPoint.ResultMask;
        for (var reference = browser.Next(); reference != null; reference = browser.Next())
        {
            Console.WriteLine(i);
            Console.WriteLine(reference.TargetId);
            Console.WriteLine(nodeToBrowse);
            i += 1;
            var description = CreateDefaultReferenceDescription(nodeToBrowse);
            description.SetReferenceType(resultMask, nodeToBrowse.HandledNode.ReferenceTypeId, !reference.IsInverse);
            description.SetTargetAttributes(resultMask, nodeToBrowse.HandledNode.NodeClass, nodeToBrowse.HandledNode.BrowseName, nodeToBrowse.HandledNode.DisplayName, nodeToBrowse.HandledNode.TypeDefinitionId);
            Console.WriteLine(JsonSerializer.Serialize(description));
            yield return description;
        }
    }

    private INodeBrowser CreateBrowser(ContinuationPoint continuationPoint, NodeState nodeToBrowse)
    {
        var contextCopy = context.Copy();
        return continuationPoint.Data as INodeBrowser ?? nodeToBrowse.CreateBrowser(contextCopy,
            continuationPoint.View,
            continuationPoint.ReferenceTypeId,
            continuationPoint.IncludeSubtypes,
            continuationPoint.BrowseDirection,
            null,
            null,
            false);
    }

    private static ReferenceDescription CreateDefaultReferenceDescription(IEntityNodeHandle nodeToBrowse)
    {
        var description = new ReferenceDescription
        {
            NodeId = nodeToBrowse.HandledNode.NodeId
        };

        return description;
    }
}