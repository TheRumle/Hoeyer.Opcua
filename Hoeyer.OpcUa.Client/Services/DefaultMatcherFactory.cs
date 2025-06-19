using System;
using System.Linq.Expressions;
using Hoeyer.OpcUa.Client.Api.Browsing;
using Opc.Ua;

namespace Hoeyer.OpcUa.Client.Services;

public static class DefaultMatcherFactory
{
    public static object CreateMatcher(Type entityType)
    {
        // (ReferenceDescription referenceDescription) =>
        ParameterExpression referenceParam = Expression.Parameter(typeof(ReferenceDescription), "referenceDescription");

        // referenceDescription.BrowseName
        MemberExpression browseNameProperty =
            Expression.Property(referenceParam, nameof(ReferenceDescription.BrowseName));

        // referenceDescription.BrowseName.Name
        MemberExpression nameProperty = Expression.Property(browseNameProperty, nameof(QualifiedName.Name));

        // entityType.Name
        ConstantExpression typeName = Expression.Constant(entityType.Name);

        // entityType.Name.Equals(referenceDescription.BrowseName.Name)
        MethodCallExpression equalsCall = Expression.Call(typeName,
            typeof(string).GetMethod("Equals", new[] { typeof(string) }),
            nameProperty);

        // Create the delegate type EntityDescriptionMatcher<TEntity>
        Type delegateType = typeof(EntityDescriptionMatcher<>).MakeGenericType(entityType);

        // Create and return the lambda expression
        LambdaExpression lambda = Expression.Lambda(delegateType, equalsCall, referenceParam);
        return lambda.Compile();
    }
}