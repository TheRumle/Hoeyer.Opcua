using System;
using System.Collections.Generic;
using System.Linq;
using FluentResults;
using Hoeyer.Common.Extensions.Exceptions;
using Opc.Ua;

namespace Hoeyer.OpcUa.Configuration;

public interface IOpcUaEntityServerConfiguration
{
    string ServerId { get; }
    string ServerName { get; }
    Uri Host { get; }
    ISet<Uri> Endpoints { get; }

    /// <summary>
    /// For instance, http://samples.org/UA/MyApplication or something else uniqely identifying the overall resource,
    /// </summary>
    Uri ApplicationNamespace { get; }
}

internal record OpcUaEntityServerConfiguration : IOpcUaEntityServerConfiguration
{
    public OpcUaEntityServerConfiguration(string ServerId, string ServerName, Uri Host, ISet<Uri> Endpoints,
        Uri ApplicationNamespace)
    {
        var validation = ValidateSupportedPrototol([Host, ..Endpoints]);
        if (validation.IsFailed)
            throw validation.Errors.ToArgumentException();
        
        this.ServerId = ServerId;
        this.ServerName = ServerName;
        this.Host = Host;
        this.Endpoints = Endpoints;
        this.ApplicationNamespace = ApplicationNamespace;
    }

    public string ServerId { get; }
    public string ServerName { get; }
    public Uri Host { get; }
    public ISet<Uri> Endpoints { get; set; }

    /// <summary>
    /// For instance, http://samples.org/UA/MyApplication or something else uniqely identifying the overall resource,
    /// </summary>
    public Uri ApplicationNamespace { get; }
    
    private static Result ValidateSupportedPrototol(IEnumerable<Uri> addresses)
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