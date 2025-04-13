using Hoeyer.OpcUa.Client.Application;
using Hoeyer.OpcUa.Client.Application.Browsing;

namespace Hoeyer.OpcUa.Client.Services;

public interface IClientServicesContainer
{
    public IEntityBrowser Browser { get; }
    public IEntityWriter Writer { get; }
}