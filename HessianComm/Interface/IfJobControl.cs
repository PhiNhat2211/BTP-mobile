using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using hessiancsharp.Class;

namespace HessianComm.Interface
{
    public interface IfJobControl
    {
        List<object> getJobOrderList(Hashtable hashJobOrder);

        List<object> getJobOrderByContainer(string searchString);

        List<object> getJobOrderListByKeys(HessianList keys);

        List<object> getMachineJobByKeys(HessianList keys);

        object setConfirmJobByScanner(HessianList hashConfirmJob);

        object setJobDone(Hashtable hashJobDone);     // Siemens 호출 API와 분리

        object setJobStatus(HessianList hashJobStatus);   // ret value 확인 필요        

        object setPickedContainer(HessianList hashTasks);

        object setDetwinJob(HessianList hashDetwinJob);             // Detwin job

        object setManualActivation(Hashtable hashTask);

        object setJobReOnChassis(HessianList hessianList);
    }

    public interface IfVmtWorkOrderControl      // Data Downsizing
    {
        List<object> getJobOrderList(HessianList machine);  // (string machineID, string machineType);

        List<object> getJobOrderListByKeys(HessianList keys);

        List<object> getMachineJobByKeys(HessianList keys);

        List<object> getMachineJobByKeys_Sync(HessianList keys);

        List<object> getJobOrderByContainer(string searchString);

        object checkYcDeTwin(HessianList list);

        object setBestPick(HessianList list);

        object validate4LoadingSwapping(HessianList list);

        object setPickedContainer(HessianList hashTasks);

        object setJobDone(Hashtable hashJobDone);

        object setReallocation(HessianList list);

        object chassisOrderComplete(Hashtable hash);

        object validChassisInfos(Hashtable hash);

        object itvLinkChassis(Hashtable hash);

        object itvUnLinkChassis(Hashtable hash);

        object setGateCancelJob(HessianList hash);

        List<object> getSwapList(HessianList machine);

        object setEmptySwap(HessianList machine);

        //Set Job Done For QC
        object setQCJobReleaseByYt(HessianList taskList);

        object releaseYtFromJob(HessianList list);
    }

    public interface IfVmtPLCControl      // Data Downsizing
    {
        object setPLCAutoFlg(HessianList list);  // (string machineID, string machineType);

        object setPLCMsg(HessianList list);
    }

    public interface VmtPLCControl      // Data Downsizing
    {
        object setPLCAutoFlg(HessianList list);  // (string machineID, string machineType);

        object setPLCMsg(HessianList list);

        object checkPLCData(HessianList list);

        object checkPLCTwistLock(HessianList list);

        object cancelPLC(Hashtable hash);
        
        object processPLC(Hashtable hash);

        object releasePLCLock(HessianList list);

        object initPLCMessage(HessianList list);
    }

    public interface VmtSwapService      // Data Downsizing
    {
        List<object> getSwapList(HessianList machine);
        object setEmptySwap(HessianList machine);
    }
    public interface IfVmtEmptySwapControl
    {
        object getEmptySwappingTargetList(HessianList list);
        object doEmptySwap(HessianList list);
    }
    public interface IfVmtSwapControl
    {
        List<object> getSwapList(HessianList machine);
        object setEmptySwap(HessianList machine);
    }
}
