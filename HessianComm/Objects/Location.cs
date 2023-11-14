using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HessianComm.Objects
{    
    public class Location
    {
        /*
        LocationType
            "UNDEFINED",
            "VESSEL",
            "YARD",
            "RAIL",
            "TPW",
            "TPL",
            "IP",
            "AP",
            "LANE"
        */
        public LocationType locTp { get; set; }

        public string blck { get; set; }

        public string bay { get; set; }

        public string row { get; set; }

        public string tier { get; set; }

        /*
        LANE(STS) : Lane Number
        YARD(RTG,RMGC) : W (Water-Side), L (Land-Side)
        */
        public string location { get; set; }
    }
}
