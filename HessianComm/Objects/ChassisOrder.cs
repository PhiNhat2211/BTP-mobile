using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HessianComm.Objects
{
    public class ChassisOrder
    {
        public string ordSeq { get; set; }

        public string chssNo { get; set; }

        public string crntPsnIdxNo1 { get; set; }

        public string crntPsnIdxNo2 { get; set; }

        public string crntPsnIdxNo3 { get; set; }

        public string crntPsnIdxNo4 { get; set; }

        public string plnPsnIdxNo1 { get; set; }

        public string plnPsnIdxNo2 { get; set; }

        public string plnPsnIdxNo3 { get; set; }

        public string plnPsnIdxNo4 { get; set; }

        public string itvCd { get; set; }

        public string ordStsCd { get; set; }
        
        public string cntrNo { get; set; }

        public Boolean isArrival { get; set; }

        // public string point { get; set; }
    }
}
