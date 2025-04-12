using System;
using System.Linq.Expressions;
using Hoeyer.OpcUa.Client.Application.Browsing;
using Opc.Ua;

namespace Hoeyer.OpcUa.Client.Services;

public static class DefaultMatcherFactory
{
    public static object CreateMatcher(Type entityType)
    {
        // (ReferenceDescription referenceDescription) =>
        var referenceParam = Expression.Parameter(typeof(ReferenceDescription), "referenceDescription");

        // referenceDescription.BrowseName
        var browseNameProperty = Expression.Property(referenceParam, nameof(ReferenceDescription.BrowseName));

        // referenceDescription.BrowseName.Name
        var nameProperty = Expression.Property(browseNameProperty, nameof(QualifiedName.Name));

        // entityType.Name
        var typeName = Expression.Constant(entityType.Name);

        // entityType.Name.Equals(referenceDescription.BrowseName.Name)
        var equalsCall = Expression.Call(typeName, typeof(string).GetMethod("Equals", new[] { typeof(string) }), nameProperty);

        // Create the delegate type EntityDescriptionMatcher<TEntity>
        var delegateType = typeof(EntityDescriptionMatcher<>).MakeGenericType(entityType);

        // Create and return the lambda expression
        var lambda = Expression.Lambda(delegateType, equalsCall, referenceParam);
        return lambda.Compile();
    }
}