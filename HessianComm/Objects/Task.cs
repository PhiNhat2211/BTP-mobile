using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HessianComm.Objects
{
    public class Task
    {
        public Machine workingMchn { get; set; }

        public Machine partnerMchn { get; set; }

        public Container cntr { get; set; }

        public Location loc { get; set; }        

        public string jobTp { get; set; }

        public string jobId { get; set; }

        public string externalId { get; set; }

        public string positionOnChassis { get; set; }

        public string chgPrgmId { get; set; }
    }
}
