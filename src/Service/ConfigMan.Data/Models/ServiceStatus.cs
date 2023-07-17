using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConfigMan.Data.Models
{
    

    public record ApplicationInitialized();
    public class ServiceStatus
    {
        public static Guid ServiceId = Guid.Parse("F0CC4086-38BA-4499-98E3-8EF431A71A47"); //random guid that will always be the one doc

        public Guid Id { get; set; }

        public bool IsInitialized { get; internal set; }

        public void Apply(ApplicationInitialized e)
        {
            this.Id = ServiceId;
            this.IsInitialized = true;
        }
    }
}
