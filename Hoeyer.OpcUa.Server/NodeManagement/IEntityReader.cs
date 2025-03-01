using System;
using System.Collections.Generic;
using Hoeyer.OpcUa.Server.Application;
using Opc.Ua;

namespace Hoeyer.OpcUa.Server.NodeManagement;


public sealed class EntityValueReadResponse : RequestResponse<ReadValueId, (DataValue DataValue, StatusCode StatusCode)>
{
    public EntityValueReadResponse(ReadValueId request, string error)
        : base(request, error)
    {
        
    }

    public EntityValueReadResponse(ReadValueId request, Func<(DataValue, StatusCode)> valueGet) : base(request, valueGet)
    {}  
    
    public StatusCode StatusCode => Response == default ? StatusCodes.BadNodeClassInvalid : Response.StatusCode;
    public string StatusMessage => StatusCode.LookupSymbolicId(StatusCode.Code);
    public string AttributeName => Attributes.GetBrowseName(Request.AttributeId);
}

public interface IEntityReader
{
    public IEnumerable<EntityValueReadResponse> ReadProperties(IEnumerable<ReadValueId> valuesToRead);
    
}