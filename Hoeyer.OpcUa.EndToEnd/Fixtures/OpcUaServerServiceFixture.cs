﻿using Hoeyer.OpcUa.Server.Services;

namespace Hoeyer.OpcUa.EndToEndTest.Fixtures;

public class OpcUaServerServiceFixture : OpcUaCoreServicesFixture
{
    public OpcUaServerServiceFixture()
    {
        OnGoingOpcEntityServiceRegistration.WithOpcUaServer();
    }
}