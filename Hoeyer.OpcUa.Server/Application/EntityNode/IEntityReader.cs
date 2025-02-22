﻿using System.Collections.Generic;
using FluentResults;
using Opc.Ua;

namespace Hoeyer.OpcUa.Server.Application.EntityNode;

public interface IEntityReader
{
    public IEnumerable<Result<ReadResponse>> Read(IEnumerable<ReadValueId> readables);
    public Result<ReadResponse> Read(ReadValueId toRead);

}