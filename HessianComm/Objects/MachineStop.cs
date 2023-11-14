using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HessianComm.Objects
{
    public class MachineStop
    {
        public Machine mchn { get; set; }

        public User user { get; set; }

        public string reasonCd { get; set; }

        public string reasonNm { get; set; }

        public bool isStop { get; set; }

        public string approvalCd { get; set; }
    }
}
