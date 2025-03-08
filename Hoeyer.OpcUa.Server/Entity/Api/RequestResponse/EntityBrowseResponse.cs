using System.Collections.Generic;
using Opc.Ua;
using Opc.Ua.Server;

namespace Hoeyer.OpcUa.Server.Entity.Api.RequestResponse;

/// <summary>
/// The result of the browsing of an Entity or one of its properties.
/// </summary>
/// <param name="ContinuationPoint">A <see cref="ContinuationPoint"/> describing where browsing halted, indicated by its indexes. Null if browsing is finished.</param>
/// <param name="RelatedEntities">The descriptions of the entity properties</param>
public sealed record EntityBrowseResponse(ContinuationPoint? ContinuationPoint, IList<ReferenceDescription> RelatedEntities)
{
    /// <summary>
    /// A <see cref="ContinuationPoint"/> describing where browsing halted, indicated by its indexes. Null if browsing is finished.
    /// </summary>
    public ContinuationPoint? ContinuationPoint { get; } = ContinuationPoint;
    /// <summary>
    /// The descriptions of the entity properties
    /// </summary>
    public IList<ReferenceDescription> RelatedEntities { get; } = RelatedEntities;
}