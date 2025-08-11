using System;
using System.Linq.Expressions;
using Hoeyer.OpcUa.Client.Api.Browsing;
using Opc.Ua;

namespace Hoeyer.OpcUa.Client.Services;

public static class DefaultMatcherFactory
{
    public static object CreateMatcher(Type agentType)
    {
        // (ReferenceDescription referenceDescription) =>
        ParameterExpression referenceParam = Expression.Parameter(typeof(ReferenceDescription), "referenceDescription");

        // referenceDescription.BrowseName
        MemberExpression browseNameProperty =
            Expression.Property(referenceParam, nameof(ReferenceDescription.BrowseName));

        // referenceDescription.BrowseName.Name
        MemberExpression nameProperty = Expression.Property(browseNameProperty, nameof(QualifiedName.Name));

        // agentType.Name
        ConstantExpression typeName = Expression.Constant(agentType.Name);

        // agentType.Name.Equals(referenceDescription.BrowseName.Name)
        MethodCallExpression equalsCall = Expression.Call(typeName,
            typeof(string).GetMethod("Equals", new[] { typeof(string) }),
            nameProperty);

        // Create the delegate type AgentDescriptionMatcher<TAgent>
        Type delegateType = typeof(AgentDescriptionMatcher<>).MakeGenericType(agentType);

        // Create and return the lambda expression
        LambdaExpression lambda = Expression.Lambda(delegateType, equalsCall, referenceParam);
        return lambda.Compile();
    }
}