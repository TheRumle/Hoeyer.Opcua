using System.Collections.Generic;
using Opc.Ua;
using Opc.Ua.Server;

namespace Hoeyer.OpcUa.Server.Application.EntityNode;

public interface IEntityBrowser
{
    /// <inheritdoc />
    IEnumerable<ReferenceDescription> Browse(ContinuationPoint continuationPoint,
        IEntityNodeHandle nodeToBrowse);
}