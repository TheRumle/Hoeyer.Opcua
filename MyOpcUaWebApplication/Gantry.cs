﻿using Hoeyer.OpcUa.Core;

namespace MyOpcUaWebApplication;

[OpcUaEntity]
public class Gantry
{
    public List<int> Speeds { get; set;  }
    public string message     { get; set; }
    public HashSet<int> messages     { get; set; }
}

