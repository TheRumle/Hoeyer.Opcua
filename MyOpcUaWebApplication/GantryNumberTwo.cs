﻿using Hoeyer.OpcUa.Core;

namespace MyOpcUaWebApplication;

[OpcUaEntity]
public sealed class GantryNumberTwo
{
    public Position Position { get; set; }
    public bool Moving { get; set; }
    public List<int> Speeds { get; set; }
    public string message { get; set; }
    public List<string> messages { get; set; }

    public delegate void ChangePosition(Position oldPosition, Position newPosition);

    public List<string> Names { get; set; } = ["rasmus", "christmas"];
}