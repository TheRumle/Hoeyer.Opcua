using System.Collections.Generic;
using System.Linq;
using Hoeyer.Common.Extensions;
using Opc.Ua;
using Opc.Ua.Server;

namespace Hoeyer.OpcUa.Server.Entity.Api.RequestResponse;

/// <summary>
///     The result of the browsing of an Entity or one of its properties.
/// </summary>
public sealed record EntityBrowseResponse
{
    /// <summary>
    ///     The result of the browsing of an Entity or one of its properties.
    /// </summary>
    /// <param name="continuationPoint">
    ///     A <see cref="ContinuationPoint" /> describing where browsing halted, indicated by its
    ///     indexes. Null if browsing is finished.
    /// </param>
    /// <param name="relatedEntities">The descriptions of the entity properties</param>
    public EntityBrowseResponse(ContinuationPoint? continuationPoint, IList<ReferenceDescription> relatedEntities)
    {
        ContinuationPoint = continuationPoint;
        RelatedEntities = relatedEntities;
    }

    /// <summary>
    ///     A <see cref="ContinuationPoint" /> describing where browsing halted, indicated by its indexes. Null if browsing is
    ///     finished.
    /// </summary>
    public ContinuationPoint? ContinuationPoint { get; }

    /// <summary>
    ///     The descriptions of the entity properties
    /// </summary>
    public IList<ReferenceDescription> RelatedEntities { get; }

    /// <inheritdoc />
    public override string ToString()
    {
        if (RelatedEntities == null!)
        {
            return "<null>";
        }

        return RelatedEntities.Select(e => $"{e.BrowseName?.ToString()} ({e?.NodeId})").SeparateBy(", ");
    }

    public void Deconstruct(out ContinuationPoint? ContinuationPoint, out IList<ReferenceDescription> RelatedEntities)
    {
        ContinuationPoint = this.ContinuationPoint;
        RelatedEntities = this.RelatedEntities;
    }
}