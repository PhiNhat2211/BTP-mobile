using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using hessiancsharp.Class;

namespace HessianComm.Interface
{   
    public interface IfContainerControl
    {
        object getInventoryList(Hashtable hashInventoryList);

        object getInventory(Hashtable hashInventory);

        object getContainerInfo(Hashtable cntr);

        object getInventoryList4Multi(ArrayList locationList);

        object isValidLocation(Hashtable location);
    }

    public interface IfVmtContainerControl      // Data Downsizing
    {
        object getInventoryList4Multi(ArrayList locationList);

        object getInventoryList4Multi_Sync(ArrayList locationList);
        
        //object getContainerInfo(Hashtable cntr);
        object getContainerInfo(string cntr);
    }
}
