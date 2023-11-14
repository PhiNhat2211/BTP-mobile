using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using hessiancsharp.Class;

namespace HessianComm.Interface
{
    public interface IfVmtMachineControl
    {
        object getMachineList();

        object setMachineStatusChanged(Hashtable hashMachine);

        object setMachinePassed(Hashtable hashYtTask);

        object setMachineArrival(HessianList list);

        object setMachineReady(HessianList list);

        object setItvDone(HessianList list);

        object changeChassisNo(HessianList list);

        object getChssUsingData(String chssNo);

        object getMachineStop(Hashtable hashMachine);

        object setMachineStop(Hashtable hashMachineStop);

        object getMachineNotice(Hashtable hashMachine);

        object setMachineNotice(Hashtable hashMachine);

        object getPrecedingYtList(Hashtable hashYtTask);

        // object getMachineList(Hashtable hashMachine);
        object getMachineListByType(Hashtable hashMachine);

        object getChangedMachineLocation(HessianList list);

        object getMachineListOfPool(Hashtable hashMachine);

        object doSwap4Manual(HessianList hashSwap);

        object doChangePosition(Hashtable machine, Hashtable loc);

        // Get Driver Job History
        object getDriverJobHistory(HessianList list);

        object setParkLocation(HessianList list);

        object getParkLocation(HessianList list);

        object getMachineStatusChanged(HessianList list);
    }
}
