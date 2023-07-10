using ConfigMan.Data.Models;
using ConfigMan.Data.Projections;
using Marten;
using MediatR;

namespace ConfigMan.Data.Handlers.Applications
{
    public record GetActiveApplications : IRequest<List<Application>>;
    internal class GetActiveApplicationssHandler : IRequestHandler<GetActiveApplications, List<Application>>
    {
        private readonly IQuerySession _querySession;

        public GetActiveApplicationssHandler(IQuerySession querySession)
        {
            _querySession = querySession;
        }

        public async Task<List<Application>> Handle(GetActiveApplications request, CancellationToken cancellationToken)
        {
            var allActivateApplications = _querySession.Query<ActiveApplication>().ToList();
            var items = new List<Application>();
            foreach (var activeApplication in allActivateApplications)
            {
                var aggregateStreamAsync = await _querySession.Events.AggregateStreamAsync<Application>(activeApplication.Id);
                items.Add(aggregateStreamAsync);
            }
            return items;
        }
    }
}
