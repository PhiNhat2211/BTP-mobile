using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using VMT_Data_JAT2;

namespace VMT_Data_JAT2.Objects
{
    public class RMG
    {
        public class RMG_Member
        {
            private static readonly RMG_Member _theOnly = null;

            public static RMG_Member Singleton
            {
                get { return _theOnly; }
            }

            static RMG_Member()
            {
                _theOnly = new RMG_Member();
            }

            private String _TargetJobKey = "";
            private String _TargetContSwap = "";
            private String _TargetVirtualContNo = "";

            public String TargetJobKey
            {
                get { return _TargetJobKey; }
                set
                {
                    if (!(_TargetJobKey.Equals(value)))
                    {
                        _TargetJobKey = value;
                        Notify_TargetJobKey();
                    }
                }
            }

            public String TargetContSwap
            {
                get { return _TargetContSwap; }
                set
                {
                    if (!(_TargetContSwap.Equals(value)))
                    {
                        _TargetContSwap = value;
                        Notify_TargetContSwap();
                    }
                }
            }

            public String TargetVirtualContNo
            {
                get { return _TargetVirtualContNo; }
                set
                {
                    if (!(_TargetVirtualContNo.Equals(value)))
                    {
                        _TargetVirtualContNo = value;
                        Notify_TargetVirtualContNo();
                    }
                }
            }

            private VMT_Data_JAT2.Objects.Common.VD_Common_JobOrder _TargetJobOrder = null;
            public VMT_Data_JAT2.Objects.Common.VD_Common_JobOrder TargetJobOrder
            {
                get { return _TargetJobOrder; }
                set
                {
                    if (_TargetJobOrder == null)
                        _TargetJobOrder = value;
                    else if (!(_TargetJobOrder.jobKey.Equals(value.jobKey)))
                    {
                        _TargetJobOrder = value;
                    }
                }
            }

            private void SetTargetJobOrder()
            {

            }

            private RMG.VD_RMG_PDS_PickDrop_Payload _TwistLockStatus = null;
            public RMG.VD_RMG_PDS_PickDrop_Payload TwistLockStatus
            {
                get { return _TwistLockStatus; }
                set
                {
                    //if (_TwistLockStatus != value)
                    {
                        _TwistLockStatus = value;
                        Notify_TwistLockStatus();
                    }
                }
            }

            #region INotifyPropertyChanged implementation
            public event PropertyChangedEventHandler PropertyChanged_TargetJobKey;
            public event PropertyChangedEventHandler PropertyChanged_TargetContSwap;
            public event PropertyChangedEventHandler PropertyChanged_TargetVirtualContNo;
            public event PropertyChangedEventHandler PropertyChanged_TwistLockStatus;

            protected void Notify_TargetJobKey()
            {
                if (this.PropertyChanged_TargetJobKey != null)
                {
                    PropertyChanged_TargetJobKey(this, new PropertyChangedEventArgs(TargetJobKey));
                }
            }

            protected void Notify_TargetContSwap()
            {
                if (this.PropertyChanged_TargetContSwap != null)
                {
                    PropertyChanged_TargetContSwap(this, new PropertyChangedEventArgs(TargetContSwap));
                }
            }

            protected void Notify_TargetVirtualContNo()
            {
                if (this.PropertyChanged_TargetVirtualContNo != null)
                {
                    PropertyChanged_TargetVirtualContNo(this, new PropertyChangedEventArgs(TargetVirtualContNo));
                }
            }

            protected void Notify_TwistLockStatus()
            {
                if (this.PropertyChanged_TwistLockStatus != null)
                {
                    PropertyChanged_TwistLockStatus(this, new PropertyChangedEventArgs(TwistLockStatus.m_cTwistLockUnlock.ToString()));
                }
            }

            #endregion INotifyPropertyChanged implementation
        }

        public class RMG_User : UserInfo
        {
            public static void GetMachineType(ref String MchnTyp, ref int size)
            {
                String strIniFile = GetIniDirectory() + @"MachineInfo.ini";

                Ini.IniFile ini = new Ini.IniFile(strIniFile);
                MchnTyp = ini.IniReadValue("MACHINE", "TYPE", "TC");
            }

            public static void GetMachineID(ref String MchnID, ref int size)
            {
                String strIniFile = GetIniDirectory() + @"MachineInfo.ini";

                Ini.IniFile ini = new Ini.IniFile(strIniFile);
                MchnID = ini.IniReadValue("MACHINE", "ID", "R06");
            }
            public static void GetJobTypeSortOrder(ref Dictionary<string, int> JobTypeSortOrder)
            {
                String strIniFile = GetIniDirectory() + @"MachineInfo.ini";

                Ini.IniFile ini = new Ini.IniFile(strIniFile);
                String AH = ini.IniReadValue("JOBTYPESORTORDER", "AH");
                String LD = ini.IniReadValue("JOBTYPESORTORDER", "LD");
                String DS = ini.IniReadValue("JOBTYPESORTORDER", "DS");
                String LC = ini.IniReadValue("JOBTYPESORTORDER", "LC");
                String GI = ini.IniReadValue("JOBTYPESORTORDER", "GI");
                String GO = ini.IniReadValue("JOBTYPESORTORDER", "GO");
                String GC = ini.IniReadValue("JOBTYPESORTORDER", "GC");
                String MO = ini.IniReadValue("JOBTYPESORTORDER", "MO");
                String MI = ini.IniReadValue("JOBTYPESORTORDER", "MI");
                String RH = ini.IniReadValue("JOBTYPESORTORDER", "RH");

                int intParseValue = 0;
                int intExcValue = 11;

                JobTypeSortOrder.Add("AH", Int32.TryParse(AH, out intParseValue) ? intParseValue : intExcValue);
                JobTypeSortOrder.Add("LD", Int32.TryParse(LD, out intParseValue) ? intParseValue : intExcValue);
                JobTypeSortOrder.Add("DS", Int32.TryParse(DS, out intParseValue) ? intParseValue : intExcValue);
                JobTypeSortOrder.Add("LC", Int32.TryParse(LC, out intParseValue) ? intParseValue : intExcValue);
                JobTypeSortOrder.Add("GI", Int32.TryParse(GI, out intParseValue) ? intParseValue : intExcValue);
                JobTypeSortOrder.Add("GO", Int32.TryParse(GO, out intParseValue) ? intParseValue : intExcValue);
                JobTypeSortOrder.Add("GC", Int32.TryParse(GC, out intParseValue) ? intParseValue : intExcValue);
                JobTypeSortOrder.Add("MO", Int32.TryParse(MO, out intParseValue) ? intParseValue : intExcValue);
                JobTypeSortOrder.Add("MI", Int32.TryParse(MI, out intParseValue) ? intParseValue : intExcValue);
                JobTypeSortOrder.Add("RH", Int32.TryParse(RH, out intParseValue) ? intParseValue : intExcValue);
            }
        }

        [Serializable]
        public class VD_RMG_PDS_Periodic_Payload
        {
            public UInt32 m_dwTime;                            // 시간
            public Double m_dLatitude;                         // 위도
            public Double m_dLongitude;                        // 경도

            public float m_fHeadingDegree;               // 헤딩 정보
            public int m_cTrolleyPosition;               // 6자리 mm Unit 
            public int m_cHoistPosition;                 // 6자리 mm Unit 

            public int m_cGantryMoveOnOff;                // Gantry Move 0 = Not Moving, 1 = Moving  
            public int m_cDriveDirectionDegree;           // 0 degree = forward, 180 degree = backward
            public int m_cAntiCollisionDetectionSignal;   // 0 = Nothing, 1= stop RMG cause Anti-Collision Device alarm

            public int m_cRFIDStatus;                     // RFID 센서 상태 정보
            public int m_cFuelGage;                       // 연료게이지 센서 상태 정보
            public int m_cTirePressureCheck;              // 타이어 압력센서 상태 정보 

            //----sRMG_PDS_TireFuel_Payload_Recv
            public int m_cFuelGageLiter;                  // 실제 연료량
            public int m_cTireRessurePSI;                 // 실제 타이어 압력

            //---- 접속여부..
            public int m_CPS_Status;                      // 0 = disconnect, 1 = connect, 2 = Error
            public int m_PDS_Status;                      // 0 = disconnect, 1 = connect, 2 = Error  
            public int m_TOSClient_Status;                // 0 = disconnect, 1 = connect, 2 = Error
            public int m_EagleEyeEvent_Status;            // 0 = disconnect, 1 = connect, 2 = Error
            public int m_EagleEyePeriodic_Status;         // 0 = disconnect, 1 = connect, 2 = Error

            public int m_cBlockInOut;                     //Block In = 1, Block Out = 0
            public int m_cBay;                            //Bay Number
            public int m_cRow;                            // 1 = A, 2 = B 3 = C.... 26 = Z 
            public int m_cTier;                           // 1,2,3,4,5,6,7

            public String m_strBlockName;                 //BlockName

            public VD_RMG_PDS_Periodic_Payload()
            {
                m_dwTime = 0;
                m_dLatitude = 0;
                m_dLongitude = 0;
                m_fHeadingDegree = 0;

                m_cTrolleyPosition = 0;
                m_cHoistPosition = 0;

                m_cGantryMoveOnOff = 0;
                m_cDriveDirectionDegree = 0;
                m_cAntiCollisionDetectionSignal = 0;

                m_cRFIDStatus = 0;
                m_cFuelGage = 0;
                m_cTirePressureCheck = 0;

                m_cFuelGageLiter = 0;
                m_cTireRessurePSI = 0;

                m_CPS_Status = 0;
                m_PDS_Status = 0;
                m_TOSClient_Status = 0;
                m_EagleEyeEvent_Status = 0;
                m_EagleEyePeriodic_Status = 0;

                m_cBlockInOut = 0;
                m_cBay = 0;

                m_strBlockName = String.Empty;
            }
        }

        [Serializable]
        public class VD_RMG_CPS_Alignment_Payload
        {
            public UInt32 m_dwTime;
            public Byte m_cAlignmentResult;  // '0'=Empty, '1' = Completed, '2'=Processing, '3' = Detected, '4'= Passed, 'E'=Error
            // (Completed : 완료, Processing : Alignment 진행 중, Detected : 차량진입발견, Passed = 발견된 차량이 지나간 경우)
            public Byte m_cForeAfter;        // '0'= Null, '1'=Fore., '2' = Aft, 'E'=error
            public Byte m_cDirection;        // '0'=Empty,(비어있음) , '1' = Normal Direction(정방향 접근), 

            public VD_RMG_CPS_Alignment_Payload()
            {
                m_dwTime = 0;
                m_cAlignmentResult = 0;
                m_cForeAfter = 0;
                m_cDirection = 0;
            }
        }

        [Serializable]
        public class VD_RMG_PDS_PickDrop_Payload
        {
            public UInt32 m_dwTime;                // 시간
            public Byte[] m_cBlock;             // Block Name '1A'

            public int m_cTrolleyPosition;   // 6자리 mm Unit
            public int m_cHoistPosition;     // 6자리 mm Unit

            // '0' = Null, '1' = 20ft, '2'=20ft_Twin,'3' = 40ft, '4' = 45ft,
            // '5'=48ft,   '8' = None containerized operation, '10'=Error
            public int m_cOperationType;
            public int m_cBay;                // 00, 01, ... 256 
            public int m_cRow;                // 1 = A, 2 = B 3 = C.... 26 = Z	
            public int m_cTier;               // 1, 2

            public int m_cTwistLockUnlock;    // 1 = Lock, 2 = Unlock, 3 = Error
            public int m_cContainerWeight;   // 컨테이너 무게Kg

            public VD_RMG_PDS_PickDrop_Payload()
            {
                m_dwTime = 0;
                m_cBlock = new Byte[0];

                m_cTrolleyPosition = 0;
                m_cHoistPosition = 0;

                m_cOperationType = 0;
                m_cBay = 0;
                m_cRow = 0;
                m_cTier = 0;

                m_cTwistLockUnlock = 0;
                m_cContainerWeight = 0;
            }
        }

        [Serializable]
        public class VD_RMG_PDS_RFID_Payload
        {
            public UInt32 m_dwTime;   // 시간 정보
            public Byte m_cAntennaID; // '1' = 1번, '2' = 2번, 'E' = error or Unknown (번호는 RFID Antenna ID를 따름)
            public Byte[] m_cTagID;   // EPC 64bit ASCII Code String 'NCT00001' => 0x4E0x430x540x300x300x300x300x31 
            // N   C   T   0   0   0   0    1
            public Byte m_cFlag;      // '1' = Begin, '2' = End

            public VD_RMG_PDS_RFID_Payload()
            {
                m_dwTime = 0;
                m_cAntennaID = 0;
                m_cTagID = new Byte[0];

                m_cFlag = 0;
            }
        }

        //------------------------------------------------------------------------------
        //- ITV Info
        [Serializable]
        public class VD_RMG_POWInfo_Receive
        {
            public String ITVMachineID;
            public String BlockName;
            public int iBay;
            public Boolean bPOW; //TRUE in, FALSE out

            public VD_RMG_POWInfo_Receive()
            {
                ITVMachineID = String.Empty;
                BlockName = String.Empty;
                iBay = -1;
                bPOW = false;
            }
        }

        [Serializable]
        public class VD_RMG_BlockEnteranceITV_Receive
        {
            public String ITVMachineID;
            public String BlockName;
            public Boolean bEnterance; //TRUE in   , FALSE out

            public VD_RMG_BlockEnteranceITV_Receive()
            {
                ITVMachineID = String.Empty;
                BlockName = String.Empty;
                bEnterance = false;
            }
        }

        [Serializable]
        public class VD_RMG_ManualReadyITV_Receive
        {
            public String ITVMachineID;

            public VD_RMG_ManualReadyITV_Receive()
            {
                ITVMachineID = String.Empty;
            }
        }

        [Serializable]
        public class VD_RMG_ManualReady_Send
        {
            public String jobKey;
            public String ITVMachineID;
            public Boolean bReadyOnOff;//( 1: On, 0 : Off)

            public VD_RMG_ManualReady_Send()
            {
                jobKey = String.Empty;
                ITVMachineID = String.Empty;
                bReadyOnOff = false;
            }
        }
        //------------------------------------------------------------------------------

        //------------------------------------------------------------------------------
        //- Inventory Correction
        [Serializable]
        public class VD_RMG_GetBlockInfo_Send
        {
            public String szBlockName;
            public Byte bayNo;

            public VD_RMG_GetBlockInfo_Send()
            {
                szBlockName = String.Empty;
                bayNo = 0;
            }
        }

        [Serializable]
        public class VD_RMG_Inventory_BayInfo : IDisposable
        {
            public String BayName;
            public List<Common.VD_Common_Def_Inventory> invenList;

            public VD_RMG_Inventory_BayInfo()
            {
                BayName = String.Empty;
                invenList = new List<Common.VD_Common_Def_Inventory>();
            }

            public void Clear()
            {
                if (this.invenList != null)
                {
                    foreach (var item in this.invenList)
                        item.Dispose();

                    this.invenList.Clear();                    
                }
            }

            public void Dispose()
            {
                this.Clear();
                this.invenList = null;                
            }
        }

        [Serializable]
        public class VD_RMG_Inventory_BlockInfo : IDisposable
        {
            public String BlcName;
            public Dictionary<String, VD_RMG_Inventory_BayInfo> DicBay;

            public VD_RMG_Inventory_BlockInfo()
            {
                BlcName = String.Empty;
                DicBay = new Dictionary<String, VD_RMG_Inventory_BayInfo>();
            }

            public void Dispose()
            {
                if (this.DicBay != null)
                {
                    foreach (var item in this.DicBay)
                        item.Value.Dispose();

                    this.DicBay.Clear();
                    this.DicBay = null;
                }
            }
        }

        [Serializable]
        public class VD_RMG_InventoryInfo_Receive : IDisposable
        {
            public enum Purpose_type
            {
                TYPE_FOREGROUND = 1,
                TYPE_FOREGROUND_Ex,
                TYPE_BACKGROUND,
                TYPE_BACKGROUND_Ex,
                TYPE_DATA,
                TYPE_DATA_Ex,
            }

            public Purpose_type enPurposeType = Purpose_type.TYPE_FOREGROUND;

            public Dictionary<String, VD_RMG_Inventory_BlockInfo> DicBlock;        // <block, bayList>

            public int Count
            {
                get
                {
                    if (DicBlock == null)
                        return 0;
                    else
                    {
                        int cnt = 0;
                        foreach (var blck in DicBlock.Values)
                        {
                            foreach (var bay in blck.DicBay.Values)
                            {
                                cnt += bay.invenList.Count;
                            }
                        }
                        return cnt;
                    }
                }
            }

            public VD_RMG_InventoryInfo_Receive()
            {
                DicBlock = new Dictionary<String, VD_RMG_Inventory_BlockInfo>();
            }

            public void Dispose()
            {
                if (this.DicBlock != null)
                {
                    foreach (var item in this.DicBlock)
                        item.Value.Dispose();

                    this.DicBlock.Clear();
                    this.DicBlock = null;
                }
            }
        }

        [Serializable]
        public class VD_RMG_BlockBayInfoSimple_Receive
        {
            public int bay; // 해당 베이 번호
            public int row; // row max
            public int tier; // tier max            
            public Byte[] BlockBay; // C=Container, N=Tunnel, T=No Work Tier , A=No Work Area, ""=empty 
            public Byte cntrCount;
            public List<Common.VD_Common_Def_InventorySimple> cntr;

            public VD_RMG_BlockBayInfoSimple_Receive()
            {
                bay = 0;
                row = 0;
                tier = 0;
                BlockBay = new Byte[0];
                cntrCount = 0;
                cntr = new List<Common.VD_Common_Def_InventorySimple>();
            }
        }

        [Serializable]
        public class VD_RMG_Correction_Send
        {
            public String cntrNo;
            public Common.VD_Common_Yard_Location fromLoc;
            public Common.VD_Common_Yard_Location toLoc;
            public String actionType;   // C=creation, D=delete, M=move 인벤토리정보(DB) 를 수정하는 형태

            public VD_RMG_Correction_Send()
            {
                cntrNo = String.Empty;
                fromLoc = new Common.VD_Common_Yard_Location();
                toLoc = new Common.VD_Common_Yard_Location();
                actionType = String.Empty;
            }
        }

        [Serializable]
        public class VD_RMG_SetCurrentJob_Send
        {
            public String jobKey;

            public VD_RMG_SetCurrentJob_Send()
            {
                jobKey = String.Empty;
            }
        }

        [Serializable]
        public class VD_RMG_SetCurrentJob_Receive
        {
            public String jobKey;

            public VD_RMG_SetCurrentJob_Receive()
            {
                jobKey = String.Empty;
            }
        }

        [Serializable]
        public class VD_RMG_HandleJobDone_Send
        {
            public String jobKey;
            public String WorkingMachineID;
            public String WorkingMachineTP;
            public String PartnerMachineID;
            public String PartnerMachineTP;
            public String cntrNo;
            public String qcLn;
            public String workingStatus;
            public Boolean isGcBtn;
            public Common.VD_Common_Job_Location Loc;
            public Common.VD_Commmon_Spreader sprd;
            public String positionOnChassis;

            public VD_RMG_HandleJobDone_Send()
            {
                jobKey = String.Empty;
                WorkingMachineID = String.Empty;
                WorkingMachineTP = String.Empty;
                PartnerMachineID = String.Empty;
                PartnerMachineTP = String.Empty;
                cntrNo = String.Empty;
                qcLn = String.Empty;
                workingStatus = String.Empty;
                Loc = new Common.VD_Common_Job_Location();
                sprd = new Common.VD_Commmon_Spreader();
                positionOnChassis = String.Empty;
                isGcBtn = false;
            }
        }

        [Serializable]
        public class VD_RMG_TargetJob_Receive
        {
            public String jobKey;

            public VD_RMG_TargetJob_Receive()
            {
                jobKey = String.Empty;
            }
        }

        [Serializable]
        public class VD_RMG_RMGMarrying_Send
        {
            public String WorkingMachineID;
            public String WorkingMachineTP;
            public String PartnerMachineID;
            public String PartnerMachineTP;
            public String cntrNo;
            public Common.VD_Common_Job_Location Loc;

            public VD_RMG_RMGMarrying_Send()
            {
                WorkingMachineID = String.Empty;
                WorkingMachineTP = String.Empty;
                PartnerMachineID = String.Empty;
                PartnerMachineTP = String.Empty;
                cntrNo = String.Empty;
                Loc = new Common.VD_Common_Job_Location();
            }
        }

        [Serializable]
        public class VD_RMG_RMGMarrying_Receive
        {
            public String WorkingMachineID;
            public String WorkingMachineTP;
            public String PartnerMachineID;
            public String PartnerMachineITP;
            public String cntrNo;
            public Common.VD_Common_Job_Location Loc;

            public VD_RMG_RMGMarrying_Receive()
            {
                WorkingMachineID = String.Empty;
                WorkingMachineTP = String.Empty;
                PartnerMachineID = String.Empty;
                PartnerMachineITP = String.Empty;
                cntrNo = String.Empty;
                Loc = new Common.VD_Common_Job_Location();
            }
        }

        [Serializable]
        public class VD_RMG_SwapResult_Receive
        {
            public UInt32 swapResult; //PROCEED : 1  SUCCESS : 2  FAIL : 3  RETURN TO YARD  : 0

            public VD_RMG_SwapResult_Receive()
            {
                swapResult = 0;
            }
        }

        [Serializable]
        public class VD_RMG_ReturnWarning_Receive
        {
            public String PartnerMachineID;
            public UInt32 returnWarning; //0: Off      1: Display Warning

            public VD_RMG_ReturnWarning_Receive()
            {
                PartnerMachineID = String.Empty;
                returnWarning = 0;
            }
        }

        [Serializable]
        public class VD_RMG_OTR_ManualBlockInOut_Send
        {
            public String m_JobKey;
            public Byte m_btInOut; // 1byte 1 이면 In (0 이면 Out : Default)
            //----------------------------- Tag정보.
            public VD_RMG_PDS_RFID_Payload m_OTR_RFIDTagInfo; // 없으면 0값으로

            public VD_RMG_OTR_ManualBlockInOut_Send()
            {
                m_JobKey = String.Empty;
                m_btInOut = 0;
                m_OTR_RFIDTagInfo = new VD_RMG_PDS_RFID_Payload();
            }
        }
        //------------------------------------------------------------------------------

        [Serializable]
        public class VD_RMG_JobOrderList
        {
            private int _iCount; // 갯수
            public List<Common.VD_Common_JobOrder> JobOrder;

            public VD_RMG_JobOrderList()
            {
                _iCount = -1;
                JobOrder = new List<Common.VD_Common_JobOrder>();
            }

            public void Clear()
            {
                for (int i=0; i< this.JobOrder.Count; i++)                
                {
                    var item = this.JobOrder[i] as Common.VD_Common_JobOrder;
                    item.Clear();
                    item = null;
                }
                this.JobOrder.Clear();
                this.JobOrder = null;
            }

            public int Count
            {
                get
                {
                    _iCount = JobOrder.Count;
                    return _iCount;
                }
            }
        }

        [Serializable]
        public class VD_RMG_SwapList
        {
            private int _iCount; // 갯수
            public List<Common.VmtSwap> vmtSwap;

            public VD_RMG_SwapList()
            {
                _iCount = -1;
                vmtSwap = new List<Common.VmtSwap>();
            }

            public void Clear()
            {
                for (int i = 0; i < this.vmtSwap.Count; i++)
                {
                    var item = this.vmtSwap[i] as Common.VmtSwap;
                    item.Clear();
                    item = null;
                }
                this.vmtSwap.Clear();
                this.vmtSwap = null;
            }

            public int Count
            {
                get
                {
                    _iCount = vmtSwap.Count;
                    return _iCount;
                }
            }
        }

        [Serializable]
        public class VD_RMG_PartnerMachineList
        {
            private int _iCount; // 갯수
            public List<Common.VD_Common_Job_Machine> Machine;

            public VD_RMG_PartnerMachineList()
            {
                _iCount = -1;
                Machine = new List<Common.VD_Common_Job_Machine>();
            }

            public int Count
            {
                get
                {
                    _iCount = Machine.Count;
                    return _iCount;
                }
            }

            public void Clear()
            {
                if (this.Machine != null)                
                    this.Machine.Clear();                
            }
        }

        [Serializable]
        public class VD_RMG_ContainerInfo_Receive
        {
            public Common.VD_Common_Def_Container cntr;
            public Common.VD_Common_Def_Location fmLoc;
            public Common.VD_Common_Def_Location toLoc;
            public Common.VD_Common_Def_Vessel inVsl;
            public Common.VD_Common_Def_Vessel outVsl;
            public String rmk;

            public VD_RMG_ContainerInfo_Receive()
            {
                cntr = new Common.VD_Common_Def_Container();
                fmLoc = new Common.VD_Common_Def_Location();
                toLoc = new Common.VD_Common_Def_Location();
                inVsl = new Common.VD_Common_Def_Vessel();
                outVsl = new Common.VD_Common_Def_Vessel();
                rmk = String.Empty;
            }
        }
         
       

        [Serializable]
        public class VD_RMG_NoWorkArea_Receive : IDisposable
        {
            public List<Common.VD_Common_Def_NoWorkLocation> NoWorkArea;
            
            public VD_RMG_NoWorkArea_Receive()
            {
                this.NoWorkArea = new List<Common.VD_Common_Def_NoWorkLocation>();
            }

            public void Dispose()
            {
                for (int i=0; i<this.NoWorkArea.Count; ++i)
                {
                    this.NoWorkArea[i].Dispose();
                    this.NoWorkArea[i] = null;
                }
                this.NoWorkArea.Clear();
                this.NoWorkArea = null;
            }
        }

        [Serializable]
        public class VD_RMG_JobSet_Receive
        {
            //ResultType {  
            //   SUCCESS,   // AH 생성 성공
            //    FAIL      // AH 생성 실패
            //    PROCEED   // 현재 작업 진행
            //}
            public String resultTP;

            //Error Code
            //IF resultTp = FAIL
            //  F1: There is no usable location.
            //  F2: There is no container within the block.
            //  F3: Selected job does not belong to the machine. 
            //  F4: There is no slot to be loaded within the same bay.
            //  F6: Check the Plug of the Reefer container.
            //  F7: The error is not defined.
            //  F8: There is no container information.
            //  F9: There is no container within the general block.

            public String resultObj;

            public VD_RMG_JobSet_Receive()
            {
                resultTP = "PROCEED";
                resultObj = String.Empty;
            }
        }

        [Serializable]
        public class VD_RMG_JobDone_Receive
        {
            //ResultType {
            //   SUCCESS,
            //    FAIL
            //}
            public String resultTP;

            //Error Code
            //e.g) S1 : "Completed"
            //F1 : "Current Location is Null!!"
            //F2 : "Yard Location is not equal a JobOrder Location!!"
            //F3 : "There are another container on the current container!!"
            //F4 : "There are terminal in container job on the current truck!!"
            //F5 : "The status of reefer container is POW or ROW!!"
            //F6 : "The situation of container is not OYS or OYG!!"
            //F7 : "Check the Hold Information!!"
            //U1 : "Unknown error"
            public String resultObj;

            public VD_RMG_JobDone_Receive()
            {
                resultTP = "SUCCESS";
                resultObj = "S1";
            }
        }

        [Serializable]
        public class VD_RMG_VmtResult
        {
            //public enum resultTp
            //{
            //    SUCCESS = 1,
            //    FAIL = -1,
            //    PROCEEED = 0,
            //}
            public String resultTp;

            //Error Code
            //e.g) S1 : "Completed"
            //F1 : The job information is not exists.
            //F2 : The Container is not in yard.
            //F3 : The Swap Container is not exists a pre-advice.
            //F4 : The Swap Container's booking information is wrong.
            //F5 : The Container is Holded.
            //F6 : The On-chassis Container Cancel is failed.
            //F7 : GC  data exists.
            //F8 : GC insert failed.
            //F9 : The swap task fail.
            //F10 : The Swap Container not in yard.
            //F11 : The Swap is failed.
            //U1 : Unknow error.
            public String resultObj;

            public String resultMsg;

        }

        [Serializable]
        public class VD_RMG_VmtDomain
        {
            public string mchnId;

            public string cntrNo;

            public string rsnDesc;

            public string twistLockStatus;

            public string errorCode;

            public string containerWeight;

            public string currentCell;

            public string currentBlock;

            public string currentBay;

            public string currentRow;

            public string currentTier;

            public string msgSeq;

            public string wrkCd;

            public string jbFlg;

            public VD_RMG_VmtDomain()
            {
                mchnId = String.Empty;
                cntrNo = String.Empty;
                rsnDesc = String.Empty;
                twistLockStatus = String.Empty;
                errorCode = String.Empty;
                containerWeight = String.Empty;
                currentCell = String.Empty;
                currentBlock = String.Empty;
                currentBay = String.Empty;
                currentRow = String.Empty;
                currentTier = String.Empty;
                msgSeq = String.Empty;
                wrkCd = String.Empty;
                jbFlg = String.Empty;
            }

        }

        [Serializable]
        public class VD_RMG_VmtEmptySwapOutVO
        {
            public List<VD_RMG_VmtEmptySwapVO> reservedList;

            public List<VD_RMG_VmtEmptySwapVO> swappingList;

            public VD_RMG_VmtEmptySwapOutVO()
            {
                reservedList = new List<VD_RMG_VmtEmptySwapVO>();
                swappingList = new List<VD_RMG_VmtEmptySwapVO>();
            }
        }

        [Serializable]
        public class VD_RMG_VmtEmptySwapVO
        {
            public string cntrNo;

            public string blck;

            public string bay;

            public string row;

            public string tier;

            public VD_RMG_VmtEmptySwapVO()
            {
                cntrNo = String.Empty;
                blck = String.Empty;
                bay = String.Empty;
                row = String.Empty;
                tier = String.Empty;
            }
        }

        [Serializable]
        public class VD_RMG_MachineList_Receive : IDisposable
        {            
            public List<HessianComm.Objects.Machine> MachineList;

            public VD_RMG_MachineList_Receive()
            {                
                MachineList = new List<HessianComm.Objects.Machine>();
            }

            public void Dispose()
            {
                foreach (var machine in MachineList)
                    machine.Dispose();
                MachineList.Clear();
                MachineList = null;
            }

            public int Count
            {
                get
                {
                    if (MachineList != null)
                        return MachineList.Count;
                    else
                        return -1;
                }
            }
        }
    }
}

