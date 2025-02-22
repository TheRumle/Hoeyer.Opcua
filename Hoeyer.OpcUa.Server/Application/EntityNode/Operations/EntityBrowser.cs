using System.Collections.Generic;
using FluentResults;
using Hoeyer.OpcUa.Entity;
using Opc.Ua;

namespace Hoeyer.OpcUa.Server.Application.EntityNode.Operations;

internal class EntityBrowser(IEntityNode entityNode, EntityHandleManager handleManager)
    : IEntityBrowser
{
    public EntityBrowser(IEntityNode node) : this(node, new EntityHandleManager(node))
    {
    }

    /// <inheritdoc />
    public IEnumerable<Result<ReferenceDescription>> Browse(
        BrowseResultMask resultMask,
        INodeBrowser browser)
    {
        for (var reference = browser.Next(); reference != null; reference = browser.Next())
        {
            var description = CreateDefaultReferenceDescription(reference, resultMask);
            var target = GetTarget(reference);
            if (target == null)
            {
                description.Unfiltered = true;
                yield return description;
                continue;
            }

            description.SetTargetAttributes(resultMask,
                target.NodeClass,
                target.BrowseName,
                target.DisplayName,
                target.TypeDefinitionId);

            yield return description;
        }
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

    private BaseInstanceState? GetTarget(IReference reference)
    {
        if (handleManager.IsManagedEntityHandle(reference.TargetId)) return entityNode.Entity;

        if (handleManager.IsManagedFolderHandle(reference.TargetId)) return entityNode.Folder;
        if (handleManager.IsManagedPropertyHandle(reference, out var property)) return property.Value;
        return null;
    }
}