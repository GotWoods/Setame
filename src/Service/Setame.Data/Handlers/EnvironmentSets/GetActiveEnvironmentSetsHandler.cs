using MediatR;
using Setame.Data.Data;
using Setame.Data.Models;
using Setame.Data.Projections;

namespace Setame.Data.Handlers.EnvironmentSets;

public record GetActiveEnvironmentSets : IRequest<List<EnvironmentSet>>;

public class GetActiveEnvironmentSetsHandler : IRequestHandler<GetActiveEnvironmentSets, List<EnvironmentSet>>
{
    private readonly IEnvironmentSetRepository _environmentSetRepository;

    public GetActiveEnvironmentSetsHandler(IEnvironmentSetRepository environmentSetRepository)
    {
        
        _environmentSetRepository = environmentSetRepository;
    }

    public async Task<List<EnvironmentSet>> Handle(GetActiveEnvironmentSets request, CancellationToken cancellationToken)
    {
        var summary = await _environmentSetRepository.GetAllActiveEnvironmentSets();
        var items = new List<EnvironmentSet>();

        foreach (var activeEnvironmentSet in summary)
        {
            var aggregateStreamAsync = await _environmentSetRepository.GetById(activeEnvironmentSet.Id);
            items.Add(aggregateStreamAsync);
        }

        var sorted = items.OrderBy(x => x.Name);
        return sorted.ToList();
    }
}