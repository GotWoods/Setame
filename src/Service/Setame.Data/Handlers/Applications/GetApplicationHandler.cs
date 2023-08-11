using MediatR;
using Setame.Data.Data;
using Setame.Data.Models;

namespace Setame.Data.Handlers.Applications;

public record GetApplication(Guid ApplicationId) : IRequest<Application>;

public class GetApplicationHandler : IRequestHandler<GetApplication, Application>
{
    
    private readonly IApplicationRepository _applicationRepository;

    public GetApplicationHandler(IApplicationRepository applicationRepository)
    {

        _applicationRepository = applicationRepository;
    }

    public async Task<Application> Handle(GetApplication request, CancellationToken cancellationToken)
    {
        return await _applicationRepository.GetById(request.ApplicationId);
    }
}

