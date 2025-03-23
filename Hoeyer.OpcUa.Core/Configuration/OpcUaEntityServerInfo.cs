using System;
using System.Collections.Generic;
using System.Linq;
using FluentResults;
using Hoeyer.Common.Extensions.Exceptions;
using Opc.Ua;

namespace Hoeyer.OpcUa.Core.Configuration;

internal record OpcUaEntityServerInfo : IOpcUaEntityServerInfo
{
    public OpcUaEntityServerInfo(string ServerId, string ServerName, Uri Host, ISet<Uri> Endpoints,
        Uri ApplicationNamespace)
    {
        var validation = ValidateSupportedProtocol([Host, ..Endpoints]);
        if (validation.IsFailed)
            throw validation.Errors.ToArgumentException();

        this.ServerId = ServerId;
        ApplicationName = ServerName;
        this.Host = Host;
        this.Endpoints = Endpoints;
        this.ApplicationNamespace = ApplicationNamespace;
    }

    public string ServerId { get; }
    public string ApplicationName { get; }
    public Uri Host { get; }
    public ISet<Uri> Endpoints { get; set; }

    /// <summary>
    ///     For instance, http://samples.org/UA/MyApplication or something else uniqely identifying the overall resource,
    /// </summary>
    public Uri ApplicationNamespace { get; }

    private static Result ValidateSupportedProtocol(IEnumerable<Uri> addresses)
    {
        return addresses.Select(address => address.Scheme switch
        {
            Utils.UriSchemeHttps or Utils.UriSchemeOpcHttps => Result.Ok(),
            Utils.UriSchemeOpcTcp => Result.Ok(),
            Utils.UriSchemeOpcWss => Result.Ok(),
            "http" => Result.Fail("HTTP is not supported, use https instead"),
            _ => Result.Fail($"Protocol {address.Scheme} is not supported, use https instead")
        }).Merge();
    }
}