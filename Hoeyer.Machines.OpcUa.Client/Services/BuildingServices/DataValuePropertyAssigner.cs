using System;
using System.Collections.Generic;
using System.Linq;
using FluentResults;
using Hoeyer.Machines.OpcUa.Client.Infrastructure.Configuration.Entities;
using Hoeyer.Machines.OpcUa.Client.Infrastructure.Configuration.Entities.Property.Errors;
using Opc.Ua;

namespace Hoeyer.Machines.OpcUa.Client.Services.BuildingServices;

public sealed class DataValuePropertyAssigner<TEntity> 
{
    
    /// <summary>   
    /// Will try to read all node values into the properties. If any conversion of DataValue fails, then no assignment occurs.
    /// </summary>
    /// <param name="instanceFactory">A factory that creates new instances which the values can be assigned to</param>
    /// <param name="source">Tuples representing the property to assign value to and the DataValue read from OpcUaServer.</param>
    /// <returns></returns>
    internal Result<TEntity> AssignValuesToInstance(Func<TEntity> instanceFactory, IEnumerable<PossiblePropertyDataMatch> source)
    {
        var entity = instanceFactory.Invoke();
        if (entity is null)
            return Result.Fail($"{instanceFactory} creating {typeof(TEntity).Name} returned a null value.");
        
        return source
            .Select(e => TryAssignToProperty(e.Property, e.DataValue, entity))
            .Merge()
            .ToResult(entity);
    }
    private static Result TryAssignToProperty<T>(PropertyConfiguration conf, DataValue dataValue, T entity)
    {
        if (entity is null) return Result.Fail("The entity is null, and values cannot be assigned to its properties.");
        return conf.OpcUaNodeType switch
        {
            BuiltInType.Boolean => TryGetAssignment<bool>(conf, dataValue, entity),
            BuiltInType.Byte => TryGetAssignment<byte>(conf, dataValue, entity),
            BuiltInType.Int16 => TryGetAssignment<short>(conf, dataValue, entity),
            BuiltInType.UInt16 => TryGetAssignment<ushort>(conf, dataValue, entity),
            BuiltInType.Int32 => TryGetAssignment<int>(conf, dataValue, entity),
            BuiltInType.UInt32 => TryGetAssignment<uint>(conf, dataValue, entity),
            BuiltInType.Integer => TryGetAssignment<int>(conf, dataValue, entity),
            BuiltInType.UInteger => TryGetAssignment<uint>(conf, dataValue, entity),
            BuiltInType.Int64 => TryGetAssignment<long>(conf, dataValue, entity),
            BuiltInType.UInt64 => TryGetAssignment<ulong>(conf, dataValue, entity),
            BuiltInType.Float => TryGetAssignment<float>(conf, dataValue, entity),
            BuiltInType.Double => TryGetAssignment<double>(conf, dataValue, entity),
            BuiltInType.String => TryGetAssignment<string>(conf, dataValue, entity),
            BuiltInType.DateTime => TryGetAssignment<DateTime>(conf, dataValue, entity),
            BuiltInType.Guid => TryGetAssignment<Guid>(conf, dataValue, entity),
            BuiltInType.Null => throw new NotSupportedException("Null is currently not a supported type of datavalue."),
            BuiltInType.SByte => throw new NotSupportedException("SByte is currently not a supported type of datavalue."),
            BuiltInType.ByteString => throw new NotSupportedException("ByteString is currently not a supported type of datavalue."),
            BuiltInType.XmlElement => throw new NotSupportedException("XmlElement is currently not a supported type of datavalue."),
            BuiltInType.NodeId => throw new NotSupportedException("NodeId is currently not a supported type of datavalue."),
            BuiltInType.ExpandedNodeId => throw new NotSupportedException("ExpandedNodeId is currently not a supported type of datavalue."),
            BuiltInType.StatusCode => throw new NotSupportedException("StatusCode is currently not a supported type of datavalue."),
            BuiltInType.QualifiedName => throw new NotSupportedException("QualifiedName is currently not a supported type of datavalue."),
            BuiltInType.LocalizedText => throw new NotSupportedException("LocalizedText is currently not a supported type of datavalue."),
            BuiltInType.ExtensionObject => throw new NotSupportedException("ExtensionObject is currently not a supported type of datavalue."),
            BuiltInType.DataValue => throw new NotSupportedException("DataValue is currently not a supported type of datavalue."),
            BuiltInType.Variant => throw new NotSupportedException("Variant is currently not a supported type of datavalue."),
            BuiltInType.DiagnosticInfo => throw new NotSupportedException("DiagnosticInfo is currently not a supported type of datavalue."),
            BuiltInType.Number => throw new NotSupportedException("Number is currently not a supported type of datavalue."),
            BuiltInType.Enumeration => throw new NotSupportedException("Enumeration is currently not a supported type of datavalue."),
            _ => throw new NotSupportedException($"{conf.OpcUaNodeType} is currently not supported.")
        };
    }

    private static Result TryGetAssignment<T>(PropertyConfiguration conf, DataValue dataValue, object entity)
    {
        return CanBeConvertedTo<T>(dataValue, conf).Bind(e =>
        {
            conf.PropertyInfo.SetValue(entity, e);
            return Result.Ok();
        })!;
    }

    private static Result<T> CanBeConvertedTo<T>(DataValue value, PropertyConfiguration propertyConfiguration)
    {
        return value.Value is T result && propertyConfiguration.PropertyInfo.PropertyType.IsInstanceOfType(value.Value)
            ? Result.Ok(result)
            : Result.Fail(new EntityOpcUaStateDiscrepencyError($"The declared OpcUa value for {propertyConfiguration.PropertyInfo.Name} is {propertyConfiguration.OpcUaNodeType} but the value of the data read from OpcUa server was of type {value.Value.GetType().Name}."));
    }
}