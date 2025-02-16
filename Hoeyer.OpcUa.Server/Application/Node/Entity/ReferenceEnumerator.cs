using System;
using System.Collections;
using System.Collections.Generic;
using Opc.Ua;
using Opc.Ua.Server;

namespace Hoeyer.OpcUa.Server.Application.Node.Entity;

internal class ReferenceEnumerator(INodeBrowser browser, ContinuationPoint continuationPoint, EntityHandleManager _handleManager)
    : IEnumerable<ReferenceDescription>
{
    private readonly INodeBrowser _browser = browser ?? throw new ArgumentNullException(nameof(browser));
    private readonly ContinuationPoint _continuationPoint = continuationPoint ?? throw new ArgumentNullException(nameof(continuationPoint));

    public IEnumerator<ReferenceDescription> GetEnumerator()
    {
        for (IReference reference = _browser.Next(); reference != null; reference = _browser.Next())
        {
            var description = GetReferenceDescription(reference, _continuationPoint);
            if (description == null) continue;
            yield return description;
        }
    }
    
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    
    private ReferenceDescription? GetReferenceDescription(IReference reference, ContinuationPoint continuationPoint)
    {
        ReferenceDescription description = new ReferenceDescription();
        description.NodeId = reference.TargetId;
        description.SetReferenceType(continuationPoint.ResultMask, reference.ReferenceTypeId, !reference.IsInverse);

        if (reference.TargetId.IsAbsolute)
        {
            return continuationPoint.NodeClassMask != 0 ? null : description;
        }

        var target = _handleManager.GetEntityHandle((NodeId)reference.TargetId);
        // the target may be a reference to a node in another node manager. In these cases
        // the target attributes must be fetched by the caller. The Unfiltered flag tells the
        // caller to do that.
        if (target == null)
        {
            description.Unfiltered = true;
            return description;
        }

        // apply node class filter.
        if (continuationPoint.NodeClassMask != 0 && ((continuationPoint.NodeClassMask & (uint)target.NodeClass) == 0))
        {
            return null;
        }

        description.SetTargetAttributes(continuationPoint.ResultMask,
            target.NodeClass,
            target.BrowseName,
            target.DisplayName,
            target.TypeDefinitionId);

        return description;
    }
}