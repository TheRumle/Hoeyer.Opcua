using System;
using System.Collections.Generic;
using Hoeyer.OpcUa.Core.Entity.Node;
using Hoeyer.OpcUa.Core.Entity.State;

namespace MyOpcUaWebApplication;

public sealed class GantryTranslator : IEntityTranslator<Gantry>
{
    public bool AssignToNode(Gantry state, IEntityNode node)
    {
        try
        {
            if (node.PropertyByBrowseName.ContainsKey("message"))
            {
                node.PropertyByBrowseName["message"].Value = state.message;
            }
            else
            {
                return default;
            }
            
            node.PropertyByBrowseName["speeds"].Value = state.Speeds;
        }
        catch (Exception)
        {
            return false;
        }

        return true;
    }
    
    /// <inheritdoc />
    public Gantry? Translate(IEntityNode state)
    {
        string message = DataTypeToTranslator.TranslateToSingle<string>(state, "message");
        if (message == default) return null;
        
        var speeds = DataTypeToTranslator.TranslateToCollection<List<int>, int>(state, "speeds");
        if (speeds == default) return null;
        
        return new Gantry()
        { 
            message = message,
            Speeds = speeds,
        };
    }
    
}