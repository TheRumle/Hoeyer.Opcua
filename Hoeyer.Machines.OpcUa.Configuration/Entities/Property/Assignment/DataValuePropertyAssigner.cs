using System;
using System.Collections.Generic;
using System.Linq;
using FluentResults;
using Hoeyer.Common;
using Hoeyer.Common.Extensions.Functional;
using Hoeyer.Machines.OpcUa.Configuration.Entities.Configuration;
using Hoeyer.Machines.Proxy;
using Opc.Ua;

namespace Hoeyer.Machines.OpcUa.Configuration.Entities.Property.Assignment;

public sealed class DataValuePropertyAssigner<TEntity> : IEntityPropertyAssigner<TEntity,
    PossiblePropertyMatch>
{
    /// <summary>
    /// Will try to read all node values into the properties. If any conversion of DataValue fails, then no assignment occurs.
    /// </summary>
    /// <param name="entity">The entity to assign values to.</param>
    /// <param name="source">Tuples representing the property to assign value to and the DataValue read from OpcUaServer.</param>
    /// <returns></returns>
    public Result<TEntity> TryAssignToEntity(Func<TEntity> instanceFactory, PossiblePropertyMatch source)
    {
        var entity = instanceFactory.Invoke();
        if (entity is null)
            return Result.Fail($"{instanceFactory} creating {typeof(TEntity).Name} returned a null value.");
        
        return source.Matches
            .Select(e => TryAssignToProperty(e.property, e.dataValue, entity))
            .Merge()
            .ToResult(entity);
    }
    private static Result TryAssignToProperty<T>(PropertyConfiguration conf, DataValue dataValue, T gantry)
    {
        return conf.OpcUaNodeType switch
        {
            BuiltInType.Boolean => TryGetAssignment<bool>(conf, dataValue, gantry),
            BuiltInType.Byte => TryGetAssignment<byte>(conf, dataValue, gantry),
            BuiltInType.Int16 => TryGetAssignment<short>(conf, dataValue, gantry),
            BuiltInType.UInt16 => TryGetAssignment<ushort>(conf, dataValue, gantry),
            BuiltInType.Int32 => TryGetAssignment<int>(conf, dataValue, gantry),
            BuiltInType.UInt32 => TryGetAssignment<uint>(conf, dataValue, gantry),
            BuiltInType.Integer => TryGetAssignment<int>(conf, dataValue, gantry),
            BuiltInType.UInteger => TryGetAssignment<uint>(conf, dataValue, gantry),
            BuiltInType.Int64 => TryGetAssignment<long>(conf, dataValue, gantry),
            BuiltInType.UInt64 => TryGetAssignment<ulong>(conf, dataValue, gantry),
            BuiltInType.Float => TryGetAssignment<float>(conf, dataValue, gantry),
            BuiltInType.Double => TryGetAssignment<double>(conf, dataValue, gantry),
            BuiltInType.String => TryGetAssignment<string>(conf, dataValue, gantry),
            BuiltInType.DateTime => TryGetAssignment<DateTime>(conf, dataValue, gantry),
            BuiltInType.Guid => TryGetAssignment<Guid>(conf, dataValue, gantry),
            BuiltInType.Null => throw new NotSupportedException($"Null is currently not a supported type of datavalue."),
            BuiltInType.SByte => throw new NotSupportedException($"SByte is currently not a supported type of datavalue."),
            BuiltInType.ByteString => throw new NotSupportedException($"ByteString is currently not a supported type of datavalue."),
            BuiltInType.XmlElement => throw new NotSupportedException($"XmlElement is currently not a supported type of datavalue."),
            BuiltInType.NodeId => throw new NotSupportedException($"NodeId is currently not a supported type of datavalue."),
            BuiltInType.ExpandedNodeId => throw new NotSupportedException($"ExpandedNodeId is currently not a supported type of datavalue."),
            BuiltInType.StatusCode => throw new NotSupportedException($"StatusCode is currently not a supported type of datavalue."),
            BuiltInType.QualifiedName => throw new NotSupportedException($"QualifiedName is currently not a supported type of datavalue."),
            BuiltInType.LocalizedText => throw new NotSupportedException($"LocalizedText is currently not a supported type of datavalue."),
            BuiltInType.ExtensionObject => throw new NotSupportedException($"ExtensionObject is currently not a supported type of datavalue."),
            BuiltInType.DataValue => throw new NotSupportedException($"DataValue is currently not a supported type of datavalue."),
            BuiltInType.Variant => throw new NotSupportedException($"Variant is currently not a supported type of datavalue."),
            BuiltInType.DiagnosticInfo => throw new NotSupportedException($"DiagnosticInfo is currently not a supported type of datavalue."),
            BuiltInType.Number => throw new NotSupportedException($"Number is currently not a supported type of datavalue."),
            BuiltInType.Enumeration => throw new NotSupportedException($"Enumeration is currently not a supported type of datavalue."),
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