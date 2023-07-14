﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConfigMan.Data.Models;
using ConfigMan.Data.Projections;
using Marten;
using MediatR;

namespace ConfigMan.Data.Handlers.EnvironmentSets;

public record GetActiveEnvironmentSets : IRequest<List<EnvironmentSet>>;
internal class GetActiveEnvironmentSetsHandler : IRequestHandler<GetActiveEnvironmentSets, List<EnvironmentSet>>
{
    private readonly IQuerySession _querySession;

    public GetActiveEnvironmentSetsHandler(IQuerySession querySession)
    {
        _querySession = querySession;
    }

    public async Task<List<EnvironmentSet>> Handle(GetActiveEnvironmentSets request, CancellationToken cancellationToken)
    {
        var summary = await _querySession.Query<ActiveEnvironmentSet>().ToListAsync(token: cancellationToken);
        var items = new List<EnvironmentSet>();

        foreach (var activeEnvironmentSet in summary)
        {
            var aggregateStreamAsync = await _querySession.Events.AggregateStreamAsync<EnvironmentSet>(activeEnvironmentSet.Id, token: cancellationToken);
            if (aggregateStreamAsync != null)
                items.Add(aggregateStreamAsync);
        }

        var sorted = items.OrderBy(x => x.Name);
        return sorted.ToList();
    }
}