using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
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
    ILogger<MethodCaller<TEntity>> logger,
    IEntityBrowser<TEntity> browser,
    IEntitySessionFactory factory,
    IBrowseNameCollection<TEntity> browseNameCollection)
    : IMethodCaller<TEntity>
{
    public async Task CallMethod(string methodName,
        CancellationToken token = default,
        params object[] args) =>
        await CallNode(browseNameCollection.MethodNames[methodName], token, args);

    /// <inheritdoc />
    public async Task<T> CallMethod<T>(string methodName, CancellationToken token = default, params object[] args)
    {
        IList<object> res = await CallNode(methodName, token, args);
        var returnValue = res[0];
        return returnValue == null ? default! : (T)OpcToCSharpValueParser.ParseOpcValue(returnValue)!;
    }


    private async Task<IList<object>> CallNode(string methodName, CancellationToken token = default,
        params object[] args)
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

    private async Task<IList<object>> PerformCall(
        string methodName,
        object[] args,
        CancellationToken token)
    {
        var entityNode = await browser.GetNodeStructure(token);
        var methodToCall = entityNode.Methods[methodName]
                           ?? throw EntityMethodCallException.NoCallMethodModelled(
                               entityNode.EntityName,
                               methodName);

        var session = await factory.GetSessionForAsync<TEntity>(token);

        var callArgument = args.Length == 1
            ? args[0]
            : args;

        var response = await session.Session.CallAsync(
            entityNode.NodeId,
            methodToCall,
            token,
            callArgument);

        return response
               ?? throw EntityMethodCallException.NullResponse(
                   entityNode.EntityName,
                   methodName);
    }
}