using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HessianComm.Objects
{
    public class Inventory
    {
        public Container cntr { get; set; }

        public Location loc { get; set; }

        public string inOut { get; set; }

        public string reason { get; set; }
    }
}
