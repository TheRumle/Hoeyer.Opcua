using Hoeyer.OpcUa.Server.Application.RequestResponse;
using Opc.Ua;

namespace Hoeyer.OpcUa.Server.Entity.Api;

public interface IPropertyReader
{
    EntityValueReadResponse ReadProperty(ReadValueId readId, PropertyState node);
}