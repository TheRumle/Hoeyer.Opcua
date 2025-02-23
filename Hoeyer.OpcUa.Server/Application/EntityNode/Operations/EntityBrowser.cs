using System.Collections.Generic;
using FluentResults;
using Opc.Ua;
using Opc.Ua.Server;

namespace Hoeyer.OpcUa.Server.Application.EntityNode.Operations;

internal class EntityBrowser(ServerSystemContext context)
    : IEntityBrowser
{
    /// <inheritdoc />
    public IEnumerable<Result<ReferenceDescription>> Browse(ContinuationPoint continuationPoint, IEntityNodeHandle nodeToBrowse)
    {
        var browser = CreateBrowser(continuationPoint, nodeToBrowse.HandledNode);
        for (var reference = browser.Next(); reference != null; reference = browser.Next())
        {
            var description = CreateDefaultReferenceDescription(reference, continuationPoint.ResultMask);
            description.SetTargetAttributes(continuationPoint.ResultMask,
                nodeToBrowse.HandledNode.NodeClass,
                nodeToBrowse.HandledNode.BrowseName,
                nodeToBrowse.HandledNode.DisplayName,
                nodeToBrowse.HandledNode.TypeDefinitionId);
            
            continuationPoint.Index += 1;
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

    private static ReferenceDescription CreateDefaultReferenceDescription(IReference reference,
        BrowseResultMask resultMask)
    {
        var description = new ReferenceDescription
        {
            NodeId = reference.TargetId
        };
        description.SetReferenceType(resultMask, reference.ReferenceTypeId, reference.IsInverse);
        return description;
    }
}