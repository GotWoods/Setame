using Marten;
using MediatR;
using Setame.Data.Projections;

namespace Setame.Data.Handlers.Applications
{
    public record GetActiveApplications : IRequest<List<ActiveApplication>>;
    internal class GetActiveApplicationsHandler : IRequestHandler<GetActiveApplications, List<ActiveApplication>>
    {
        private readonly IQuerySession _querySession;

        public GetActiveApplicationsHandler(IQuerySession querySession)
        {
            _querySession = querySession;
        }

        public Task<List<ActiveApplication>> Handle(GetActiveApplications request, CancellationToken cancellationToken)
        {
            var allActivateApplications = _querySession.Query<ActiveApplication>().ToList();
            return Task.FromResult(allActivateApplications);
        }
    }
}
