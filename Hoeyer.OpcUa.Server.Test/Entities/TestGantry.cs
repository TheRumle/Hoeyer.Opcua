﻿using Hoeyer.OpcUa.Core;

namespace Hoeyer.OpcUa.Server.Test.Entities;

[OpcUaEntity]
public class TestGantry
{
    public int Speed { get; set; }
    public bool IsMoving { get; set; }
}