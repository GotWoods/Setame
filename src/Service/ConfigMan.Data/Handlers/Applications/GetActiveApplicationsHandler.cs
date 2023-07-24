using ConfigMan.Data.Models;
using ConfigMan.Data.Projections;
using Marten;
using MediatR;

namespace ConfigMan.Data.Handlers.Applications
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
