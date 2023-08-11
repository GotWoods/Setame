using Marten;
using MediatR;
using Setame.Data.Data;
using Setame.Data.Projections;

namespace Setame.Data.Handlers.Applications
{
    public record GetActiveApplications : IRequest<List<ActiveApplication>>;

    public class GetActiveApplicationsHandler : IRequestHandler<GetActiveApplications, List<ActiveApplication>>
    {
        private readonly IApplicationRepository _applicationRepository;
        

        public GetActiveApplicationsHandler(IApplicationRepository applicationRepository)
        {
            _applicationRepository = applicationRepository;
            
        }

        public Task<List<ActiveApplication>> Handle(GetActiveApplications request, CancellationToken cancellationToken)
        {
            var allActivateApplications = _applicationRepository.GetAllActive();
            return Task.FromResult(allActivateApplications);
        }
    }
}
