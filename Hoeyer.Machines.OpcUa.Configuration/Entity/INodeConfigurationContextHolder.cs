using Hoeyer.Machines.OpcUa.Configuration.Entity.Context;

namespace Hoeyer.Machines.OpcUa.Configuration.Entity;

internal interface INodeConfigurationContextHolder
{
    internal OpcUaEntityConfiguration Context {get; set; }

}