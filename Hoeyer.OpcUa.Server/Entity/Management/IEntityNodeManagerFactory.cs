using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Hoeyer.OpcUa.Server.Api;
using Opc.Ua.Server;

namespace Hoeyer.OpcUa.Server.Entity.Management;

internal interface IEntityNodeManagerFactory
{
    Task<IEnumerable<IEntityNodeManager>> CreateEntityManagers(
        Func<string, (string @namespace, ushort index)> namespaceIndexFactory, IServerInternal server);
}