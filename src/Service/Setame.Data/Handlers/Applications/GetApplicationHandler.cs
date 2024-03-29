﻿using Marten;
using MediatR;
using Setame.Data.Models;

namespace Setame.Data.Handlers.Applications;

public record GetApplication(Guid ApplicationId) : IRequest<Application>;

public class GetApplicationHandler : IRequestHandler<GetApplication, Application>
{
    private readonly IQuerySession _querySession;

    public GetApplicationHandler(IQuerySession querySession)
    {
        _querySession = querySession;
    }

    public async Task<Application> Handle(GetApplication request, CancellationToken cancellationToken)
    {
        return await _querySession.Events.AggregateStreamAsync<Application>(request.ApplicationId, token: cancellationToken) ?? throw new NullReferenceException("Application could not be found");
    }
}

