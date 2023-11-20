using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using HessianCSharp.Class;

namespace HessianComm.Interface
{
    public interface IfDefineControl
    {
        object getMachineStopCodeList(string machineType);

        object getBlockMapListForYt(string blck);

        object getBlockMapList(string blck);

        object getBlockList();      // block / bay info 추가

        object getNoWorkArea(Hashtable hashLoc);

        object getNoWorkTier(Hashtable hashLoc);        
    }

    public interface IfVmtDefineControl
    {
        object getMachineStopCodeList(string machineType);

        object getBlockMapListForYt(string blck);

        object getBlockMapList(string blck);

        object getBlockList();      // block / bay info 추가

        object getBlockListForBlockMap();      // block Map info 추가

        object getMaxRow();

        object getNoWorkArea(Hashtable hashLoc);

        object getNoWorkTier(Hashtable hashLoc);
        
        object getVmtAutoStartConfig();

        object getBlockListForYardSector(HessianList blockInfo);

        String getConfigValue(String configId);
    }
}