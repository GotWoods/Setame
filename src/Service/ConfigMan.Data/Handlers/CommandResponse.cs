using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JasperFx.CodeGeneration.Frames;

namespace ConfigMan.Data.Handlers
{
    public class CommandResponse
    {
        public bool WasSuccessful => !Errors.Any();
        public List<Errors> Errors { get; set; } = new();
        public long NewVersion { get; set; }
    }
}

