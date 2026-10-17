using Hoeyer.OpcUa.Client.Abstractions.Browsing;
using Hoeyer.OpcUa.Client.Abstractions.Calling;
using Hoeyer.OpcUa.Client.Abstractions.Calling.Exceptions;
using Hoeyer.OpcUa.Client.Abstractions.Connection;
using Hoeyer.OpcUa.Core.Abstractions;
using Hoeyer.OpcUa.Core.Application.OpcTypeMappers;
using Microsoft.Extensions.Logging;
using Opc.Ua;
using Opc.Ua.Client;

namespace Hoeyer.OpcUa.Client.Application.Calling;

public class MethodCaller<TEntity>(
    VariantConverter converter,
    ILogger<MethodCaller<TEntity>> logger,
    IEntityBrowser<TEntity> browser,
    IEntitySessionFactory factory,
    IBrowseNameCollection<TEntity> browseNameCollection)
    : IMethodCaller<TEntity>
{
    public async Task CallMethod(string methodName,
        CancellationToken token = default,
        params Variant[] args) =>
        await CallNode(browseNameCollection.MethodNames[methodName], args, token);

    /// <inheritdoc />
    public async Task<T> CallMethod<T>(string methodName, CancellationToken token = default, params Variant[] args)
    {
        var res = await CallNode(methodName, args, token);
        return converter.ConvertToType<T>( res[0]);
    }


    private async Task<IList<Variant>> CallNode(string methodName, Variant[] args, CancellationToken token = default)
    {
        try
        {
            return await PerformCall(methodName, args, token);
        }
        catch (ServiceResultException e) when (e.StatusCode == StatusCodes.BadNotImplemented)
        {
            logger.LogError(e, "The method '{method}' node is not found on server: {message}", methodName, e.Message);
            throw EntityMethodCallException.NotImplementedOnServer(methodName, e, args);
        }
        catch (ServiceResultException e) when (e.StatusCode == StatusCodes.BadInternalError)
        {
            logger.LogError(e, "An internal server error occurred when calling '{method}': {message}", methodName,
                e.Message);
            throw EntityMethodCallException.InternalServerError(methodName, e);
        }
        catch (Exception e)
        {
            logger.LogError(e, "An unexpected error occurred when calling method '{method}': {message}", methodName,
                e.Message);
            throw;
        }
    }

    private async Task<IList<Variant>> PerformCall(
        string methodName,
        Variant[] args,
        CancellationToken token)
    {
        var entityNode = await browser.GetNodeStructure(token);
        NodeId methodToCall = entityNode.Methods[methodName];
        if (methodToCall == default(NodeId))
        {
            throw EntityMethodCallException.NoCallMethodModelled(
                entityNode.EntityName,
                methodName);
        }

        var session = await factory.GetSessionForAsync<TEntity>(token);

        var response = await session.Session.CallAsync(
            entityNode.NodeId,
            methodToCall,
            token,
            args);

        if (response == default(ArrayOf<Variant>))
        {
            throw EntityMethodCallException.NullResponse(
                entityNode.EntityName,
                methodName);
        }

        return response.ToList();
    }
}