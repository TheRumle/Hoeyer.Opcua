using Hoeyer.OpcUa.Server.Api.RequestResponse;
using Opc.Ua;

namespace Hoeyer.OpcUa.Server.Api;

public interface IPropertyReader
{
    EntityValueReadResponse ReadProperty(ReadValueId readId, PropertyState node);
}