using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HessianComm.Objects
{
    public class VmtWorkOrder
    {
        public string jobKey { get; set; }

        public string cntrNo { get; set; }

        public string mchnId { get; set; }

        public string mchnTp { get; set; }

        public Boolean isMvmtIn { get; set; }        

        public string locTp { get; set; }

        public string blck { get; set; }

        public string bay { get; set; }

        public string row { get; set; }

        public string tier { get; set; }

        public string qcLn { get; set; }

        public string userId { get; set; }

        public string workingStatus { get; set; }

        public string ytNo { get; set; }
        public string positionOnChassis { get; set; }
        public string qcNo { get; set; }
        public string jobTp { get; set; }
        public Boolean isGcBtn { get; set; }
    }

    public class VmtWorkOrder4QCJobDone
    {
        public string cntrNo { get; set; }

        public string ytNo { get; set; }

        public string qcNo { get; set; }

        public string jobTp { get; set; }
        
        public string userId { get; set; }
    }
}
