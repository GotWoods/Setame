using Marten.Events.Aggregation;
using Marten.Events.Projections;
using Marten.Internal.Sessions;
using Setame.Data.Data;
using Setame.Data.Models;

namespace Setame.Data.Projections
{
    public class ApplicationSettings
    {
        public Guid Id { get; set; }
        public VariableGrid Settings { get; set; } = new();
    }

    public class ApplicationSettingProjection : CustomProjection<ApplicationSettings, Guid> 
    {
        private readonly IApplicationRepository _applicationRepository;
        private readonly IEnvironmentSetRepository _environmentSetRepository;

        public ApplicationSettingProjection(IApplicationRepository applicationRepository, IEnvironmentSetRepository environmentSetRepository)
        {
            AggregateByStream();

            _applicationRepository = applicationRepository;
            _environmentSetRepository = environmentSetRepository;
            IncludeType<IApplicationEvent>();
            IncludeType<IEnvironmentSetEvent>();
        }

        public override async ValueTask ApplyChangesAsync(DocumentSessionBase session, EventSlice<ApplicationSettings, Guid> slice, CancellationToken cancellation,
            ProjectionLifecycle lifecycle = ProjectionLifecycle.Inline)
        {

            var aggregate = slice.Aggregate;

            var firstEvent = slice.Events().FirstOrDefault();

            if (firstEvent is not IApplicationEvent && firstEvent is not IEnvironmentSetEvent)
            {
                return;
            }

            if (aggregate == null)
            {
                aggregate = new ApplicationSettings() { Id = slice.Id };
            }


            var application = await _applicationRepository.GetById(slice.Id);
            var environmentSet = await _environmentSetRepository.GetById(application.EnvironmentSetId);
            
            aggregate.Settings = new VariableGrid();
            //apply the environment set
            foreach (var environment in environmentSet.Environments)
            {
                foreach (var setting in environment.Settings)
                {
                    aggregate.Settings[setting.Key, environment.Name] = setting.Value;
                }
            }
            
            //apply app globals to each environment
            foreach (var appStateApplicationDefault in application.ApplicationDefaults)
            {
                foreach (var environment in environmentSet.Environments)
                {
                    if (!string.IsNullOrWhiteSpace(appStateApplicationDefault.Value))
                        aggregate.Settings[appStateApplicationDefault.Name, environment.Name] = appStateApplicationDefault.Value;
                }
            }
            
            foreach (var environment in application.EnvironmentSettings)
            {
                foreach (var setting in environment.Settings)
                {
                    if (!string.IsNullOrWhiteSpace(setting.Value))
                        aggregate.Settings[setting.Name, environment.Name] = setting.Value;
                }
            }

            session.Store(aggregate);
        }
        

     
    }
}
