using Marten.Events.Aggregation;
using Marten.Events.Projections;
using Marten.Internal.Sessions;
using Org.BouncyCastle.Math.EC.Rfc7748;
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
        

        public ApplicationSettingProjection()
        {
            AggregateByStream();
            IncludeType<IApplicationEvent>();
            IncludeType<IEnvironmentSetEvent>();
        }

        public override async ValueTask ApplyChangesAsync(DocumentSessionBase session, EventSlice<ApplicationSettings, Guid> slice, CancellationToken cancellation, ProjectionLifecycle lifecycle = ProjectionLifecycle.Inline)
        {

            var applicationEvent = false;
            var environmentSetEvent = false;

            var firstEvent = slice.Events().FirstOrDefault()?.EventType;

            if (typeof(IApplicationEvent).IsAssignableFrom(firstEvent)) 
                applicationEvent = true;

            if (typeof(IEnvironmentSetEvent).IsAssignableFrom(firstEvent))
                environmentSetEvent = true;

            if (!applicationEvent && !environmentSetEvent)
            {
                return;
            }

          
            if (environmentSetEvent)
            {
                var association = session.Query<EnvironmentSetApplicationAssociation>().FirstOrDefault(x => x.Id == slice.Id);
                var environmentSet = await session.Events.AggregateStreamAsync<EnvironmentSet>(slice.Id, token: cancellation);
                foreach (var simpleApplication in association.Applications)
                {
                    var application = await session.Events.AggregateStreamAsync<Application>(simpleApplication.Id, token: cancellation);
                    await UpdateApplicationSettings(application, environmentSet, null, session, cancellation);
                }
            }
            else
            {
                var application = await session.Events.AggregateStreamAsync<Application>(slice.Id, token: cancellation);
                var environmentSet = await session.Events.AggregateStreamAsync<EnvironmentSet>(application.EnvironmentSetId, token: cancellation);

                await UpdateApplicationSettings(application, environmentSet, slice.Aggregate, session, cancellation);
            }
        }

        private static Task UpdateApplicationSettings( Application application, EnvironmentSet environmentSet, ApplicationSettings? aggregate, DocumentSessionBase session, CancellationToken cancellation)
        {
            EventSlice<ApplicationSettings, Guid> slice;

            if (aggregate == null)
            {
                aggregate = new ApplicationSettings() { Id = application.Id };
            }

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
            return Task.CompletedTask;
        }
    }
}
