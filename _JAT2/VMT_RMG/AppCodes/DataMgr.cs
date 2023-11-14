using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Windows;
using System.Threading;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Diagnostics;
using System.IO;
//using ExternalAPI;
using System.Runtime.InteropServices;

using VMT_Data_JAT2;
using VMT_Data_JAT2.Objects;

namespace VMT_RMG
{
    public class Range
    {
        public double Min;
        public double Max;

        public double Size
        {
            get
            {
                return (Max - Min);
            }
        }

        public Range()
        {
            Min = 0;
            Max = 0;
        }

        public Range(double min, double max)
        {
            Min = min;
            Max = max;
        }
    }

    public class DataMgr : UIElement
    {
        #region [Singleton Pattern Implementation]
        private static readonly DataMgr _theOnly = null;

        public static DataMgr Singleton
        {
            get { return _theOnly; }
        }

        static DataMgr()
        {
            _theOnly = new DataMgr();
        }

        private DataMgr()
        {
        }
        #endregion [Singleton Pattern Implementation]


        private List<VMT_Data_JAT2.Objects.Common.VD_Common_JobOrder> _List_JobOrder;
        private Dictionary<String, BlockJobControl> _Dic_BlockJobControl;
        private List<RMG.VD_RMG_ManualReadyITV_Receive> _List_Ready_ITV;
        private List<RMG.VD_RMG_PDS_RFID_Payload> _List_RFID_OTR;
        public VMT_Data_JAT2.Objects.Common.VD_Common_SimpleBlockBayInfo_Receive _SimpleBlockBayInfo;
        public VMT_Data_JAT2.Objects.Common.VD_Common_SimpleBlockBayInfo_Receive _SimpleBlockBayInfoKeep;

        private VMT_Data_JAT2.Objects.RMG.VD_RMG_InventoryInfo_Receive _InventoryInfo;
        private VMT_Data_JAT2.Objects.RMG.VD_RMG_InventoryInfo_Receive _InventoryInfo1;
        private VMT_Data_JAT2.Objects.RMG.VD_RMG_InventoryInfo_Receive _InventoryInfo2;

        private RMG.VD_RMG_PartnerMachineList _List_MachineofPool;

        // key = block, value = noWorkArea(area/tunnel) List (fromto expression location )
        private Dictionary<String, VMT_Data_JAT2.Objects.RMG.VD_RMG_NoWorkArea_Receive> _Dic_NoWorkAreaInfo;
        // key = block, value = noWorkTier(tier) List (target location)
        private Dictionary<String, VMT_Data_JAT2.Objects.RMG.VD_RMG_NoWorkArea_Receive> _Dic_NoWorkTierInfo;

        private Object _inventorySyncObject = new Object();
        private Object _blockbayInfoSyncObject = new Object();

        public List<VMT_Data_JAT2.Objects.Common.VD_Common_JobOrder> List_JobOrder
        {
            get { return _List_JobOrder; }
        }

        public List<VMT_Data_JAT2.Objects.Common.VmtSwap> List_swap = null;

        public List<VMT_Data_JAT2.Objects.Common.VD_Common_Def_Inventory> List_ContVirtual = new List<VMT_Data_JAT2.Objects.Common.VD_Common_Def_Inventory>();

        public List<String> List_JobKey
        {
            get
            {
                IEnumerable<String> retValue = Enumerable.Empty<String>();

                lock (_List_JobOrder)
                {
                    retValue = from _JobOrder in _List_JobOrder
                               select _JobOrder.jobKey;
                }

                return retValue.ToList();
            }
        }

        public Dictionary<String, BlockJobControl> Dic_BlockJobControl
        {
            get { return _Dic_BlockJobControl; }
        }

        public Dictionary<String, VMT_Data_JAT2.Objects.RMG.VD_RMG_NoWorkArea_Receive> Dic_NoWorkAreaInfo
        {
            get { return _Dic_NoWorkAreaInfo; }
        }

        public Dictionary<String, VMT_Data_JAT2.Objects.RMG.VD_RMG_NoWorkArea_Receive> Dic_NoWorkTierInfo
        {
            get { return _Dic_NoWorkTierInfo; }
        }

        public VMT_Data_JAT2.Objects.RMG.VD_RMG_InventoryInfo_Receive InventoryInfo
        {
            get { return _InventoryInfo; }
        }

        public List<RMG.VD_RMG_ManualReadyITV_Receive> List_Ready_ITV
        {
            get { return _List_Ready_ITV; }
        }

        public List<RMG.VD_RMG_PDS_RFID_Payload> List_RFID_OTR
        {
            get { return _List_RFID_OTR; }
        }

        public VMT_Data_JAT2.Objects.Common.VD_Common_SimpleBlockBayInfo_Receive SimpleBlockBayInfo
        {
            get { return _SimpleBlockBayInfo; }
        }

        public RMG.VD_RMG_PartnerMachineList List_MachineofPool
        {
            get { return _List_MachineofPool; }
            set
            {
                _List_MachineofPool.Machine.Clear();
                foreach (var machine in value.Machine)
                {
                    var newMachine = new VMT_Data_JAT2.Objects.Common.VD_Common_Job_Machine();
                    newMachine.mchnId = machine.mchnId;
                    newMachine.mchnTp = machine.mchnTp;
                    newMachine.mchnSts = machine.mchnSts;
                    newMachine.vrtlFlg = machine.vrtlFlg;
                    _List_MachineofPool.Machine.Add(newMachine);
                }
            }

        }

        public void InitData()
        {
            _List_JobOrder = new List<VMT_Data_JAT2.Objects.Common.VD_Common_JobOrder>();
            _Dic_BlockJobControl = new Dictionary<String, BlockJobControl>();
            _List_Ready_ITV = new List<RMG.VD_RMG_ManualReadyITV_Receive>();
            _List_RFID_OTR = new List<RMG.VD_RMG_PDS_RFID_Payload>();
            _SimpleBlockBayInfo = new VMT_Data_JAT2.Objects.Common.VD_Common_SimpleBlockBayInfo_Receive();

            _InventoryInfo = new VMT_Data_JAT2.Objects.RMG.VD_RMG_InventoryInfo_Receive();
            _InventoryInfo1 = new VMT_Data_JAT2.Objects.RMG.VD_RMG_InventoryInfo_Receive();
            _InventoryInfo2 = new VMT_Data_JAT2.Objects.RMG.VD_RMG_InventoryInfo_Receive();

            _List_MachineofPool = new RMG.VD_RMG_PartnerMachineList();

            _Dic_NoWorkAreaInfo = new Dictionary<string, RMG.VD_RMG_NoWorkArea_Receive>();
            _Dic_NoWorkTierInfo = new Dictionary<string, RMG.VD_RMG_NoWorkArea_Receive>();
        }

        public void ReleaseData()
        {
        }

        //-----------------------------------------------------------------
        //- JobOrder Methods Section
        //-----------------------------------------------------------------
        public VMT_Data_JAT2.Objects.Common.VD_Common_JobOrder GetJobOrder(String jobKey)
        {
            IEnumerable<VMT_Data_JAT2.Objects.Common.VD_Common_JobOrder> retValue;
            lock (_List_JobOrder)
            {
                retValue = from _JobOrder in _List_JobOrder
                           where _JobOrder.jobKey.Equals(jobKey)
                           select _JobOrder;
            }

            int nCount = retValue.Count();

            if (nCount > 0)
                return retValue.First();
            else
                return null;
        }
        public VMT_Data_JAT2.Objects.Common.VD_Common_Def_Inventory GetVirtualContByCntrNo(String cntrNo)
        {
            IEnumerable<VMT_Data_JAT2.Objects.Common.VD_Common_Def_Inventory> retValue;
            lock (DataMgr.Singleton.List_ContVirtual)
            {
                retValue = from _JobOrder in List_ContVirtual
                           where _JobOrder.cntr.cntrNo.Equals(cntrNo)
                           select _JobOrder;
            }

            int nCount = retValue.Count();

            if (nCount > 0)
                return retValue.First();
            else
                return null;
        }


        public VMT_Data_JAT2.Objects.Common.VmtSwap GetJobByCntrNo(String cntrNo)
        {
            IEnumerable<VMT_Data_JAT2.Objects.Common.VmtSwap> retValue;
            lock (DataMgr.Singleton.List_swap)
            {
                retValue = from _JobOrder in List_swap
                           where _JobOrder.cntrNo.Equals(cntrNo)
                           select _JobOrder;
            }

            int nCount = retValue.Count();

            if (nCount > 0)
                return retValue.First();
            else
                return null;
        }

        public VMT_Data_JAT2.Objects.Common.VD_Common_JobOrder GetJobOrderByCntrNo(String cntrNo)
        {
            IEnumerable<VMT_Data_JAT2.Objects.Common.VD_Common_JobOrder> retValue;
            lock (_List_JobOrder)
            {
                retValue = from _JobOrder in _List_JobOrder
                           where _JobOrder.cntr.cntrNo.Equals(cntrNo)
                           select _JobOrder;
            }

            int nCount = retValue.Count();

            if (nCount > 0)
                return retValue.First();
            else
                return null;
        }

        public VMT_Data_JAT2.Objects.Common.VD_Common_JobOrder GetJobOrderByMachine(String machineID)
        {
            IEnumerable<VMT_Data_JAT2.Objects.Common.VD_Common_JobOrder> retValue;
            lock (_List_JobOrder)
            {
                retValue = from _JobOrder in _List_JobOrder
                           where _JobOrder.partnerMchn.mchnId.Equals(machineID)
                           select _JobOrder;
            }

            int nCount = retValue.Count();

            if (nCount > 0)
                return retValue.First();
            else
                return null;
        }

        public Boolean IsContain(String jobKey)
        {
            if (this.GetJobOrder(jobKey) != null)
                return true;
            else
                return false;
        }

        public Boolean IsContainCntr(String cntrNo)
        {
            if (this.GetJobByCntrNo(cntrNo) != null)
                return true;
            else
                return false;
        }

        public Boolean IsContain(VMT_Data_JAT2.Objects.Common.VD_Common_JobOrder jobOrder)
        {
            return this.IsContain(jobOrder.jobKey);
        }

        public Boolean JobOrder_Add(VMT_Data_JAT2.Objects.Common.VD_Common_JobOrder jobOrder)
        {
            Boolean retValue = false;

            try
            {
                if (this.IsContain(jobOrder))
                    JobOrder_Del(jobOrder);

                lock (_List_JobOrder)
                {
                    _List_JobOrder.Add(jobOrder);
                }
                retValue = true;
            }
            catch (Exception ex)
            {
                String errMsg = ex.Message;
                retValue = false;
            }

            return retValue;
        }

        public Boolean JobOrder_Del(String jobKey)
        {
            Boolean retValue = false;

            try
            {
                if (!this.IsContain(jobKey))
                    return true;

                lock (_List_JobOrder)
                {
                    _List_JobOrder.Remove(this.GetJobOrder(jobKey));
                }
                retValue = true;
            }
            catch (Exception ex)
            {
                String errMsg = ex.Message;
                retValue = false;
            }

            return retValue;
        }

        public Boolean JobOrder_Del(VMT_Data_JAT2.Objects.Common.VD_Common_JobOrder jobOrder)
        {
            return JobOrder_Del(jobOrder.jobKey);
        }

        public Boolean JobOrder_Clear()
        {
            Boolean retValue = false;

            try
            {
                lock (_List_JobOrder)
                {
                    for (int i = 0; i < _List_JobOrder.Count; i++)
                    {
                        VMT_Data_JAT2.Objects.Common.VD_Common_JobOrder item = _List_JobOrder[i] as VMT_Data_JAT2.Objects.Common.VD_Common_JobOrder;
                        item.Clear();
                        item = null;
                    }

                    _List_JobOrder.Clear();
                }
                retValue = true;
            }
            catch (Exception ex)
            {
                String errMsg = ex.Message;
                retValue = false;
            }

            return retValue;
        }

        public List<String> SiblingJobKey(String jobKey)
        {
            VMT_Data_JAT2.Objects.Common.VD_Common_JobOrder jobOrder = this.GetJobOrder(jobKey);

            if (jobOrder == null)
                return new List<String>();

            List<String> retValue = new List<String>();

            foreach (VMT_Data_JAT2.Objects.Common.VD_Common_JobOrder job in SiblingJobOrder(jobOrder))
            {
                retValue.Add(job.jobKey);
            }

            return retValue;
        }

        public List<VMT_Data_JAT2.Objects.Common.VD_Common_JobOrder> SiblingJobOrder(VMT_Data_JAT2.Objects.Common.VD_Common_JobOrder jobOrder)
        {
            if (jobOrder == null)
                new List<VMT_Data_JAT2.Objects.Common.VD_Common_JobOrder>();

            IEnumerable<VMT_Data_JAT2.Objects.Common.VD_Common_JobOrder> siblingJobs;
            lock (_List_JobOrder)
            {
                siblingJobs = from job in _List_JobOrder
                              where job.locFrom.blck.Equals(jobOrder.locFrom.blck) &&
                                    job.locFrom.bay.Equals(jobOrder.locFrom.bay) &&
                                    job.locFrom.row.Equals(jobOrder.locFrom.row) &&
                                    job.type.jobTp.Equals("AH")
                              select job;
            }

            int nCount = siblingJobs.Count();

            IEnumerable<VMT_Data_JAT2.Objects.Common.VD_Common_JobOrder> orderValue;
            orderValue = siblingJobs.OrderBy<VMT_Data_JAT2.Objects.Common.VD_Common_JobOrder, Int32>
                    (job => Convert.ToInt32(job.locWorking.tier));

            return orderValue.ToList();
        }


        //-----------------------------------------------------------------
        //- BlockBay Inventory Methods Section
        //-----------------------------------------------------------------
        #region [ BlockBay Inventory Methods ]

        //public bool INV_IsInventory(String block, String bay)
        //{
        //    return _Dic_BlockBayInfo.ContainsKey(Util.Parse<Int32>(bay));
        //}

        //public RMG.VD_RMG_BlockBayInfo_Receive INV_GetInventory(String block, String bay)
        //{
        //    return _Dic_BlockBayInfo[Util.Parse<Int32>(bay)];
        //}

        //public void INV_Clear()
        //{
        //    _Dic_BlockBayInfo.Clear();
        //}

        public bool INV_hasInventory(String block, String bay)
        {
            if (_InventoryInfo == null || _InventoryInfo.DicBlock == null)
                return false;
            else if (String.IsNullOrEmpty(block) || bay == null)
                return false;
            else if (!_InventoryInfo.DicBlock.ContainsKey(block) || _InventoryInfo.DicBlock[block].DicBay == null)
                return false;
            else
                return _InventoryInfo.DicBlock[block].DicBay.ContainsKey(bay);
        }

        public bool INV_hasInventory1(String block)
        {
            if (_InventoryInfo == null || _InventoryInfo.DicBlock == null)
                return false;
            else if (String.IsNullOrEmpty(block))
                return false;
            else if (!_InventoryInfo.DicBlock.ContainsKey(block) || _InventoryInfo.DicBlock[block].DicBay == null)
                return false;
            else
                return true;
        }

        public bool INV_hasInventory2(String block, String bay)
        {
            if (_InventoryInfo2 == null || _InventoryInfo2.DicBlock == null)
                return false;
            else if (String.IsNullOrEmpty(block) || bay == null)
                return false;
            else if (!_InventoryInfo2.DicBlock.ContainsKey(block) || _InventoryInfo2.DicBlock[block].DicBay == null)
                return false;
            else
                return _InventoryInfo2.DicBlock[block].DicBay.ContainsKey(bay);
        }

        public List<VMT_Data_JAT2.Objects.Common.VD_Common_Def_Inventory> INV_GetInventory(String block, String bay)
        {
            lock (this._InventoryInfo)
            {
                if (INV_hasInventory(block, bay))
                    return _InventoryInfo.DicBlock[block].DicBay[bay].invenList;
                else
                    return null;
            }
        }

        public List<VMT_Data_JAT2.Objects.Common.VD_Common_Def_Inventory> INV_GetInventory1(String block, String bay)
        {
            lock (this._InventoryInfo)
            {
                if (INV_hasInventory1(block))
                {
                    List<VMT_Data_JAT2.Objects.Common.VD_Common_Def_Inventory> lst = new List<VMT_Data_JAT2.Objects.Common.VD_Common_Def_Inventory>();
                    foreach (var bayDic in _InventoryInfo.DicBlock[block].DicBay.Values)
                    {
                        lst.AddRange(bayDic.invenList);
                    }
                    
                    return lst;
                }
                else
                    return null;
            }
        }

        public List<VMT_Data_JAT2.Objects.Common.VD_Common_Def_Inventory> INV_GetInventory2(String block, String bay)
        {
            lock (this._InventoryInfo2)
            {
                if (INV_hasInventory2(block, bay))
                    return _InventoryInfo2.DicBlock[block].DicBay[bay].invenList;
                else
                    return null;
            }
        }

        public void INV_Clear()
        {
            _InventoryInfo.Dispose();
            _InventoryInfo = new RMG.VD_RMG_InventoryInfo_Receive();
        }

        #endregion  [BlockBay Inventory Methods ]

        //-----------------------------------------------------------------
        //- Machine Methods Section
        //-----------------------------------------------------------------
        #region  [Machine Methods ]
        public List<VMT_Data_JAT2.Objects.Common.VD_Common_Job_Machine> GetMachineList(String jobKey)
        {
            IEnumerable<VMT_Data_JAT2.Objects.Common.VD_Common_Job_Machine> retValue;
            if (String.IsNullOrEmpty(jobKey))
            {
                lock (_List_JobOrder)
                {
                    retValue = from _JobOrder in _List_JobOrder
                               select _JobOrder.partnerMchn;
                }
            }
            else
            {
                if (!this.IsContain(jobKey))
                    return null;

                lock (_List_JobOrder)
                {
                    retValue = from _JobOrder in _List_JobOrder
                               where _JobOrder.jobKey.Equals(jobKey)
                               select _JobOrder.partnerMchn;
                }
            }

            int nCount = retValue.Count();

            if (nCount > 0)
                return retValue.ToList();
            else
                return null;
        }

        public List<String> GetReadyMachineJobKeys()
        {
            List<String> retValue = new List<String>();

            IEnumerable<String> readyITVValue = Enumerable.Empty<String>();
            foreach (RMG.VD_RMG_ManualReadyITV_Receive Ready_ITV in _List_Ready_ITV)
            {
                lock (_List_JobOrder)
                {
                    readyITVValue = from _JobOrder in _List_JobOrder
                                    where _JobOrder.partnerMchn.mchnId.Equals(Ready_ITV.ITVMachineID)
                                    select _JobOrder.jobKey;
                }
            }

            retValue.AddRange(readyITVValue.ToList());

            IEnumerable<String> readyOTRValue = Enumerable.Empty<String>();
            foreach (RMG.VD_RMG_PDS_RFID_Payload RFID_OTR in _List_RFID_OTR)
            {
                lock (_List_JobOrder)
                {
                    readyOTRValue = from _JobOrder in _List_JobOrder
                                    where _JobOrder.partnerMchn.mchnId.Equals(Encoding.UTF8.GetString(RFID_OTR.m_cTagID))
                                    select _JobOrder.jobKey;
                }
            }

            retValue.AddRange(readyOTRValue.ToList());

            return retValue;
        }

        #endregion  [Machine Methods ]
        
        public void AddInventoryInfo(VMT_Data_JAT2.Objects.RMG.VD_RMG_InventoryInfo_Receive value)
        {
            if (value != null)
            {
                lock (this._InventoryInfo)
                {
                    foreach (var blockInfo in value.DicBlock)
                    {
                        if (!this._InventoryInfo.DicBlock.ContainsKey(blockInfo.Key))
                        {
                            this._InventoryInfo.DicBlock.Add(blockInfo.Key,
                                Util.DeepCopy<VMT_Data_JAT2.Objects.RMG.VD_RMG_Inventory_BlockInfo>(blockInfo.Value));
                        }
                        else
                        {
                            foreach (var bayInfo in blockInfo.Value.DicBay)
                            {
                                if (this._InventoryInfo.DicBlock[blockInfo.Key].DicBay.ContainsKey(bayInfo.Key))
                                {
                                    this._InventoryInfo.DicBlock[blockInfo.Key].DicBay[bayInfo.Key].Dispose();
                                    this._InventoryInfo.DicBlock[blockInfo.Key].DicBay[bayInfo.Key] = Util.DeepCopy<VMT_Data_JAT2.Objects.RMG.VD_RMG_Inventory_BayInfo>(bayInfo.Value);
                                }
                                else
                                {
                                    this._InventoryInfo.DicBlock[blockInfo.Key].DicBay.Add(bayInfo.Key,
                                        Util.DeepCopy<VMT_Data_JAT2.Objects.RMG.VD_RMG_Inventory_BayInfo>(bayInfo.Value));
                                }
                            }
                        }
                    }
                }
            }
        }

        public void AddInventoryInfo1(VMT_Data_JAT2.Objects.RMG.VD_RMG_InventoryInfo_Receive value)
        {
            if (value != null)
            {
                lock (this._InventoryInfo1)
                {
                    foreach (var blockInfo in value.DicBlock)
                    {
                        if (!this._InventoryInfo1.DicBlock.ContainsKey(blockInfo.Key))
                        {
                            this._InventoryInfo1.DicBlock.Add(blockInfo.Key,
                                Util.DeepCopy<VMT_Data_JAT2.Objects.RMG.VD_RMG_Inventory_BlockInfo>(blockInfo.Value));
                        }
                        else
                        {
                            foreach (var bayInfo in blockInfo.Value.DicBay)
                            {
                                if (this._InventoryInfo1.DicBlock[blockInfo.Key].DicBay.ContainsKey(bayInfo.Key))
                                {
                                    this._InventoryInfo1.DicBlock[blockInfo.Key].DicBay[bayInfo.Key].Dispose();
                                    this._InventoryInfo1.DicBlock[blockInfo.Key].DicBay[bayInfo.Key] = Util.DeepCopy<VMT_Data_JAT2.Objects.RMG.VD_RMG_Inventory_BayInfo>(bayInfo.Value);
                                }
                                else
                                {
                                    this._InventoryInfo1.DicBlock[blockInfo.Key].DicBay.Add(bayInfo.Key,
                                        Util.DeepCopy<VMT_Data_JAT2.Objects.RMG.VD_RMG_Inventory_BayInfo>(bayInfo.Value));
                                }
                            }
                        }
                    }
                }
            }
        }

        public void AddInventoryInfo2(VMT_Data_JAT2.Objects.RMG.VD_RMG_InventoryInfo_Receive value)
        {
            if (value != null)
            {
                lock (this._InventoryInfo2)
                {
                    foreach (var blockInfo in value.DicBlock)
                    {
                        if (!this._InventoryInfo2.DicBlock.ContainsKey(blockInfo.Key))
                        {
                            this._InventoryInfo2.DicBlock.Add(blockInfo.Key,
                                Util.DeepCopy<VMT_Data_JAT2.Objects.RMG.VD_RMG_Inventory_BlockInfo>(blockInfo.Value));
                        }
                        else
                        {
                            foreach (var bayInfo in blockInfo.Value.DicBay)
                            {
                                if (this._InventoryInfo2.DicBlock[blockInfo.Key].DicBay.ContainsKey(bayInfo.Key))
                                {
                                    this._InventoryInfo2.DicBlock[blockInfo.Key].DicBay[bayInfo.Key].Dispose();
                                    this._InventoryInfo2.DicBlock[blockInfo.Key].DicBay[bayInfo.Key] = Util.DeepCopy<VMT_Data_JAT2.Objects.RMG.VD_RMG_Inventory_BayInfo>(bayInfo.Value);
                                }
                                else
                                {
                                    this._InventoryInfo2.DicBlock[blockInfo.Key].DicBay.Add(bayInfo.Key,
                                        Util.DeepCopy<VMT_Data_JAT2.Objects.RMG.VD_RMG_Inventory_BayInfo>(bayInfo.Value));
                                }
                            }
                        }
                    }
                }
            }
        }

        private class InventoryLocation
        {
            public String Block { get; set; }
            public String Bay { get; set; }

            public InventoryLocation(String block, String bay)
            {
                this.Block = block;
                this.Bay = bay;
            }
        }

        private List<InventoryLocation> _inventoryClearList = new List<InventoryLocation>();
        private List<InventoryLocation> _inventoryClearList1 = new List<InventoryLocation>();
        private List<InventoryLocation> _inventoryClearList2 = new List<InventoryLocation>();
        public void Inventory_AddItemToClear(String block, String bay)
        {
            if (!String.IsNullOrEmpty(block) && bay != null)
            {
                lock (this._inventorySyncObject)//(this._inventoryClearList)
                {
                    if (_inventoryClearList.Count <= 0 || !_inventoryClearList.Any(location => location.Block.Equals(block) && location.Bay.Equals(bay)))
                        _inventoryClearList.Add(new InventoryLocation(block, bay));
                }
            }
        }

        public void Inventory_AddItemToClear1(String block, String bay)
        {
            if (!String.IsNullOrEmpty(block) && bay != null)
            {
                lock (this._inventorySyncObject)//(this._inventoryClearList)
                {
                    if (_inventoryClearList1.Count <= 0 || !_inventoryClearList1.Any(location => location.Block.Equals(block) && location.Bay.Equals(bay)))
                        _inventoryClearList1.Add(new InventoryLocation(block, bay));
                }
            }
        }

        public void Inventory_AddItemToClear2(String block, String bay)
        {
            if (!String.IsNullOrEmpty(block) && bay != null)
            {
                lock (this._inventorySyncObject)//(this._inventoryClearList)
                {
                    if (_inventoryClearList2.Count <= 0 || !_inventoryClearList2.Any(location => location.Block.Equals(block) && location.Bay.Equals(bay)))
                        _inventoryClearList2.Add(new InventoryLocation(block, bay));
                }
            }
        }

        public void Inventory_ClearCurrentItems()
        {
            lock (this._inventorySyncObject)//(this._InventoryInfo)
            {
                foreach (var item in this._inventoryClearList)
                {
                    if (!String.IsNullOrEmpty(item.Block) && item.Bay != null &&
                        this._InventoryInfo.DicBlock.ContainsKey(item.Block) &&
                        this._InventoryInfo.DicBlock[item.Block].DicBay.ContainsKey(item.Bay))
                        this._InventoryInfo.DicBlock[item.Block].DicBay[item.Bay].Clear();
                }

                this._inventoryClearList.Clear();
            }
            //lock (this._inventoryClearList)
            //{
            //    _inventoryClearList.Clear();
            //}
        }

        public void Inventory_ClearCurrentItems1()
        {
            lock (this._inventorySyncObject)//(this._InventoryInfo)
            {
                foreach (var item in this._inventoryClearList1)
                {
                    if (!String.IsNullOrEmpty(item.Block) && item.Bay != null &&
                        this._InventoryInfo1.DicBlock.ContainsKey(item.Block) &&
                        this._InventoryInfo1.DicBlock[item.Block].DicBay.ContainsKey(item.Bay))
                        this._InventoryInfo1.DicBlock[item.Block].DicBay[item.Bay].Clear();
                }

                this._inventoryClearList1.Clear();
            }
            //lock (this._inventoryClearList)
            //{
            //    _inventoryClearList.Clear();
            //}
        }

        public void Inventory_ClearCurrentItems2()
        {
            lock (this._inventorySyncObject)//(this._InventoryInfo)
            {
                foreach (var item in this._inventoryClearList2)
                {
                    if (!String.IsNullOrEmpty(item.Block) && item.Bay != null &&
                        this._InventoryInfo2.DicBlock.ContainsKey(item.Block) &&
                        this._InventoryInfo2.DicBlock[item.Block].DicBay.ContainsKey(item.Bay))
                        this._InventoryInfo2.DicBlock[item.Block].DicBay[item.Bay].Clear();
                }

                this._inventoryClearList2.Clear();
            }
            //lock (this._inventoryClearList)
            //{
            //    _inventoryClearList.Clear();
            //}
        }

        public void AddBlockBayInfo(VMT_Data_JAT2.Objects.Common.VD_Common_SimpleBlockBayInfo_Receive value)
        {
            if (value != null)
            {
                lock (this._blockbayInfoSyncObject)
                {
                    foreach (var info in value.DicBlock)
                    {
                        if (this._SimpleBlockBayInfo.DicBlock.ContainsKey(info.Key))
                        {
                            this._SimpleBlockBayInfo.DicBlock[info.Key].Dispose();
                            info.Value.isBolBlck = this._SimpleBlockBayInfo.DicBlock[info.Key].isBolBlck;
                            this._SimpleBlockBayInfo.DicBlock[info.Key] = Util.DeepCopy<VMT_Data_JAT2.Objects.Common.VD_Common_SimpleBlockInfo>(info.Value);

                            //20201020 keep blockbayinfo data
                            if (PresentationMgr.Singleton.showViewINV && !PresentationMgr.Singleton.viewBLockList && DataMgr.Singleton._SimpleBlockBayInfoKeep != null && this._SimpleBlockBayInfoKeep.DicBlock.ContainsKey(info.Key)) //20201020 keep blockbayinfo data
                            {
                                if (this._SimpleBlockBayInfoKeep.DicBlock[info.Key].DicBay == null)
                                {
                                    this._SimpleBlockBayInfoKeep.DicBlock[info.Key].Dispose();
                                    this._SimpleBlockBayInfoKeep.DicBlock[info.Key] = Util.DeepCopy<VMT_Data_JAT2.Objects.Common.VD_Common_SimpleBlockInfo>(info.Value);
                                }
                            }
                        }
                        else
                        {
                            this._SimpleBlockBayInfo.DicBlock.Add(info.Key,
                                Util.DeepCopy<VMT_Data_JAT2.Objects.Common.VD_Common_SimpleBlockInfo>(info.Value));
                        }
                    }
                }
            }
        }
    }
}
