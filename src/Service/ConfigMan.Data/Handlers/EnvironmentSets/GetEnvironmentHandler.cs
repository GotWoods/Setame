using ConfigMan.Data.Data;
using ConfigMan.Data.Models;
using MediatR;

namespace ConfigMan.Data.Handlers.EnvironmentSets;

public record GetEnvironment(Guid EnvironmentSetId) : IRequest<EnvironmentSet>;
public class GetEnvironmentHandler : IRequestHandler<GetEnvironment, EnvironmentSet>
{
    private readonly IEnvironmentSetRepository _environmentSetRepository;

    public GetEnvironmentHandler(IEnvironmentSetRepository environmentSetRepository)
    {
        _environmentSetRepository = environmentSetRepository;
    }

    public async Task<EnvironmentSet> Handle(GetEnvironment request, CancellationToken cancellationToken)
    {
        return await _environmentSetRepository.GetById(request.EnvironmentSetId) ?? throw new NullReferenceException("The environment set could not be found");
    }
}