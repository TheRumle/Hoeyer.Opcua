using System;
using System.Collections.Generic;
using System.Linq;
using FluentResults;
using Hoeyer.OpcUa.Client.Configuration.Entities;
using Hoeyer.OpcUa.Client.Configuration.Entities.Property.Errors;
using Opc.Ua;

namespace Hoeyer.OpcUa.Client.Application.MachineProxy;

public sealed class DataValuePropertyAssigner<TEntity>
{
    /// <summary>
    ///     Will try to read all node values into the properties. If any conversion of DataValue fails, then no assignment
    ///     occurs.
    /// </summary>
    /// <param name="instanceFactory">A factory that creates new instances which the values can be assigned to</param>
    /// <param name="source">Tuples representing the property to assign value to and the DataValue read from OpcUaServer.</param>
    /// <returns></returns>
    internal Result<TEntity> AssignValuesToInstance(Func<TEntity> instanceFactory,
        IEnumerable<PossiblePropertyDataMatch> source)
    {
        var entity = instanceFactory.Invoke();
        if (entity is null)
            return Result.Fail($"{instanceFactory} creating {typeof(TEntity).Name} returned a null value.");

        return source
            .Select(e => TryAssignToProperty(e.PropertyConfiguration, e.DataValue, entity))
            .Merge()
            .ToResult(entity);
    }

    private static Result TryAssignToProperty<T>(PropertyConfiguration conf, DataValue dataValue, T entity)
    {
        if (entity is null) return Result.Fail("The entity is null, and values cannot be assigned to its properties.");
        var type = conf.OpcUaNodeType;

        Func<Result> throwException = () =>
            throw new NotSupportedException(
                $"{Enum.GetName(type.GetType(), type)} is currently not supported type of datavalue.");
        return type switch
        {
            BuiltInType.Boolean => TryGetAssignment<bool>(conf, dataValue, entity),
            BuiltInType.Byte => TryGetAssignment<byte>(conf, dataValue, entity),
            BuiltInType.Int16 => TryGetAssignment<short>(conf, dataValue, entity),
            BuiltInType.UInt16 => TryGetAssignment<ushort>(conf, dataValue, entity),
            BuiltInType.Int32 or BuiltInType.Integer => TryGetAssignment<int>(conf, dataValue, entity),
            BuiltInType.UInt32 or BuiltInType.UInteger => TryGetAssignment<uint>(conf, dataValue, entity),
            BuiltInType.Int64 => TryGetAssignment<long>(conf, dataValue, entity),
            BuiltInType.UInt64 => TryGetAssignment<ulong>(conf, dataValue, entity),
            BuiltInType.Float => TryGetAssignment<float>(conf, dataValue, entity),
            BuiltInType.Double => TryGetAssignment<double>(conf, dataValue, entity),
            BuiltInType.String => TryGetAssignment<string>(conf, dataValue, entity),
            BuiltInType.DateTime => TryGetAssignment<DateTime>(conf, dataValue, entity),
            BuiltInType.Guid => TryGetAssignment<Guid>(conf, dataValue, entity),

            BuiltInType.Null or BuiltInType.SByte or BuiltInType.ByteString or
                BuiltInType.XmlElement or BuiltInType.NodeId or BuiltInType.ExpandedNodeId or
                BuiltInType.StatusCode or BuiltInType.QualifiedName or BuiltInType.LocalizedText or
                BuiltInType.ExtensionObject or BuiltInType.DataValue or BuiltInType.Variant or
                BuiltInType.DiagnosticInfo or BuiltInType.Number or BuiltInType.Enumeration => throwException.Invoke(),
            _ => throwException.Invoke()
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
            : Result.Fail(new EntityOpcUaStateDiscrepencyError(
                $"The declared OpcUa value for {propertyConfiguration.PropertyInfo.Name} is {propertyConfiguration.OpcUaNodeType} but the value of the data read from OpcUa server was of type {value.Value.GetType().Name}."));
    }
}