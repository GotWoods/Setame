using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Marten.Events.Aggregation;
using Microsoft.EntityFrameworkCore.Infrastructure.Internal;

namespace ConfigMan.Data.Models.Projections
{
    public class EnvironmentSetSummary
    {
        public Dictionary<Guid, string> Environments { get; set; } = new();
    }

    public class EnvironmentSetSummaryProjection : SingleStreamProjection<EnvironmentSetSummary>
    {
        public static EnvironmentSetSummary Create()
        {
            return new EnvironmentSetSummary();
            //session.Events.QueryRawEventDataOnly<EnvironmentSetCreated>() could get all the Ids

        }

        public EnvironmentSetSummaryProjection()
        {
            //VersionIdentity<EnvironmentSet>(e=>e.);
        }

        public void Apply(EnvironmentSetCreated e, EnvironmentSetSummary current)
        {
            current.Environments.Add(e.Id, e.Name);
        }

        public void Apply(EnvironmentSetRenamed e, EnvironmentSetSummary current)
        {
            //current.Environments[e.Id] = e.NewName;
        }
    }
}
