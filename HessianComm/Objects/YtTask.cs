using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HessianComm.Objects
{
    public class YtTask
    {
        public string jobId { get; set; }

        public Machine workingMchn { get; set; }

        public Machine partnerMchn { get; set; }

        public Location loc { get; set; }

        public string moveType { get; set; }

        public string externalId { get; set; }
    }
}
