﻿using System.Collections.Generic;
using FluentResults;
using Hoeyer.OpcUa.Server.Entity.Api.RequestResponse;
using Hoeyer.OpcUa.Server.Entity.Handle;
using Opc.Ua.Server;

namespace Hoeyer.OpcUa.Server.Entity.Api;

public interface IEntityBrowser
{
    /// <summary>
    /// </summary>
    /// <param name="continuationPoint">Where to continue to browse</param>
    /// <param name="nodeToBrowse">The node which must be browsed</param>
    /// <returns>The reference descriptions which describes the content of the browsed value</returns>
    public Result<EntityBrowseResponse> Browse(ContinuationPoint continuationPoint,
        IEntityNodeHandle nodeToBrowse);
}