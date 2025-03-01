using System.Collections.Generic;
using FluentResults;
using Opc.Ua;
using Opc.Ua.Server;

namespace Hoeyer.OpcUa.Server.NodeManagement;

public interface IEntityBrowser
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="continuationPoint">Where to continue to browse</param>
    /// <param name="nodeToBrowse">The node which must be browsed</param>
    /// <returns>The reference descriptions which describes the content of the browsed value</returns>
    public Result<IEnumerable<ReferenceDescription>> Browse(ContinuationPoint continuationPoint,
        IEntityNodeHandle nodeToBrowse);

}