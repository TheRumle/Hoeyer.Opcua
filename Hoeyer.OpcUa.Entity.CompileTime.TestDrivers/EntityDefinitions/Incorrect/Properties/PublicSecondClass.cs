﻿using Hoeyer.OpcUa.Core.Entity;

namespace Hoeyer.OpcUa.Entity.CompileTime.Testing.EntityDefinitions.Incorrect.Properties;

[OpcUaEntity]
public class PublicSecondClass
{
    public string PublicSecond { get; private set; }
}