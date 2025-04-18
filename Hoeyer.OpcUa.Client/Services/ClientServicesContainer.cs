﻿using Hoeyer.OpcUa.Client.Application;
using Hoeyer.OpcUa.Client.Application.Browsing;

namespace Hoeyer.OpcUa.Client.Services;

public interface IClientServicesContainer
{
    public IEntityBrowser Browser { get; }
    public IEntityWriter Writer { get; }
}

internal sealed record ClientServicesContainer<T>(IEntityBrowser<T> EntityBrowser, IEntityWriter<T> EntityWriter) : IClientServicesContainer
{
    public IEntityBrowser<T> EntityBrowser { get; } = EntityBrowser;
    public IEntityBrowser Browser => EntityBrowser;
    public IEntityWriter<T> EntityWriter { get; } = EntityWriter;
    public IEntityWriter Writer => EntityWriter;
}