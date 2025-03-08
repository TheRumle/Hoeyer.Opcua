using Hoeyer.OpcUa.Server.Entity.Api.RequestResponse;
using Opc.Ua;

namespace Hoeyer.OpcUa.Server.Entity.Api;

public interface IPropertyReader
{
    EntityValueReadResponse ReadProperty(ReadValueId readId, PropertyState node);
}