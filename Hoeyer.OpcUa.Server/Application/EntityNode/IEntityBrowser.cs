using System.Collections.Generic;
using FluentResults;
using Opc.Ua;
using Opc.Ua.Server;

namespace Hoeyer.OpcUa.Server.Application.EntityNode;

public interface IEntityBrowser
{
    /// <inheritdoc />
    IEnumerable<Result<ReferenceDescription>> Browse(
        BrowseResultMask resultMask,
        INodeBrowser browser);
}