using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Opc.Ua.Server;

namespace Hoeyer.OpcUa.Server.Api.Management;

internal interface IEntityNodeManagerFactory
{
    Task<IEnumerable<IEntityNodeManager>> CreateEntityManagers(
        Func<string, (string @namespace, ushort index)> namespaceIndexFactory, IServerInternal server);
}