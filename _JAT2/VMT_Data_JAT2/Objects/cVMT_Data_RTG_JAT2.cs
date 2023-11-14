using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using VMT_Data_JAT2;

namespace VMT_Data_JAT2.Objects
{
    public class RTG
    {
        public class RTG_Member
        {
            private static readonly RTG_Member _theOnly = null;

            public static RTG_Member Singleton
            {
                get { return _theOnly; }
            }

            static RTG_Member()
            {
                _theOnly = new RTG_Member();
            }

            private String _TargetJobKey = "";

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

            private RTG.VD_RTG_PDS_PickDrop_Payload _TwistLockStatus = null;
            public RTG.VD_RTG_PDS_PickDrop_Payload TwistLockStatus
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
            public event PropertyChangedEventHandler PropertyChanged_TwistLockStatus;

            protected void Notify_TargetJobKey()
            {
                if (this.PropertyChanged_TargetJobKey != null)
                {
                    PropertyChanged_TargetJobKey(this, new PropertyChangedEventArgs(TargetJobKey));
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

        public class RTG_UserInfo : UserInfo
        {
            //public static String gTargetJobKey = "";

            new public static void GetMachineType(ref String MchnTyp, ref int size)
            {
                String strIniFile = GetIniDirectory() + @"MachineInfo.ini";

                Ini.IniFile ini = new Ini.IniFile(strIniFile);
                MchnTyp = ini.IniReadValue("MACHINE", "TYPE", "TC");
            }

            new public static void GetMachineID(ref String MchnID, ref int size)
            {
                String strIniFile = GetIniDirectory() + @"MachineInfo.ini";

                Ini.IniFile ini = new Ini.IniFile(strIniFile);
                MchnID = ini.IniReadValue("MACHINE", "ID", "RTG01");
            }
        }

        [Serializable]
        public class VD_RTG_PDS_Periodic_Payload
        {
            public UInt32 m_dwTime;                            // 시간
            public Double m_dLatitude;                         // 위도
            public Double m_dLongitude;                        // 경도

            public float m_fHeadingDegree;               // 헤딩 정보
            public int m_cTrolleyPosition;               // 6자리 mm Unit 
            public int m_cHoistPosition;                 // 6자리 mm Unit 

            public int m_cGantryMoveOnOff;                // Gantry Move 0 = Not Moving, 1 = Moving  
            public int m_cDriveDirectionDegree;           // 0 degree = forward, 180 degree = backward
            public int m_cAntiCollisionDetectionSignal;   // 0 = Nothing, 1= stop RTG cause Anti-Collision Device alarm

            public int m_cRFIDStatus;                     // RFID 센서 상태 정보
            public int m_cFuelGage;                       // 연료게이지 센서 상태 정보
            public int m_cTirePressureCheck;              // 타이어 압력센서 상태 정보 

            //----sRTG_PDS_TireFuel_Payload_Recv
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

            public VD_RTG_PDS_Periodic_Payload()
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
        public class VD_RTG_CPS_Alignment_Payload
        {
            public UInt32 m_dwTime;
            public Byte m_cAlignmentResult;  // '0'=Empty, '1' = Completed, '2'=Processing, '3' = Detected, '4'= Passed, 'E'=Error
            // (Completed : 완료, Processing : Alignment 진행 중, Detected : 차량진입발견, Passed = 발견된 차량이 지나간 경우)
            public Byte m_cForeAfter;        // '0'= Null, '1'=Fore., '2' = Aft, 'E'=error
            public Byte m_cDirection;        // '0'=Empty,(비어있음) , '1' = Normal Direction(정방향 접근), 

            public VD_RTG_CPS_Alignment_Payload()
            {
                m_dwTime = 0;
                m_cAlignmentResult = 0;
                m_cForeAfter = 0;
                m_cDirection = 0;
            }
        }

        [Serializable]
        public class VD_RTG_PDS_PickDrop_Payload
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

            public VD_RTG_PDS_PickDrop_Payload()
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
        public class VD_RTG_PDS_RFID_Payload
        {
            public UInt32 m_dwTime;   // 시간 정보
            public Byte m_cAntennaID; // '1' = 1번, '2' = 2번, 'E' = error or Unknown (번호는 RFID Antenna ID를 따름)
            public Byte[] m_cTagID;   // EPC 64bit ASCII Code String 'NCT00001' => 0x4E0x430x540x300x300x300x300x31 
            // N   C   T   0   0   0   0    1
            public Byte m_cFlag;      // '1' = Begin, '2' = End

            public VD_RTG_PDS_RFID_Payload()
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
        public class VD_RTG_POWInfo_Receive
        {
            public String ITVMachineID;
            public String BlockName;
            public int iBay;
            public Boolean bPOW; //TRUE in, FALSE out

            public VD_RTG_POWInfo_Receive()
            {
                ITVMachineID = String.Empty;
                BlockName = String.Empty;
                iBay = -1;
                bPOW = false;
            }
        }

        [Serializable]
        public class VD_RTG_BlockEnteranceITV_Receive
        {
            public String ITVMachineID;
            public String BlockName;
            public Boolean bEnterance; //TRUE in   , FALSE out

            public VD_RTG_BlockEnteranceITV_Receive()
            {
                ITVMachineID = String.Empty;
                BlockName = String.Empty;
                bEnterance = false;
            }
        }

        [Serializable]
        public class VD_RTG_ManualReadyITV_Receive
        {
            public String ITVMachineID;

            public VD_RTG_ManualReadyITV_Receive()
            {
                ITVMachineID = String.Empty;
            }
        }

        [Serializable]
        public class VD_RTG_ManualReady_Send
        {
            public String jobKey;
            public String ITVMachineID;
            public Boolean bReadyOnOff;//( 1: On, 0 : Off)

            public VD_RTG_ManualReady_Send()
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
        public class VD_RTG_GetBlockInfo_Send
        {
            public String szBlockName;
            public Byte bayNo;

            public VD_RTG_GetBlockInfo_Send()
            {
                szBlockName = String.Empty;
                bayNo = 0;
            }
        }

        [Serializable]
        public class VD_RTG_BlockBayInfo_Receive
        {
            public int bay; // 해당 베이 번호
            public int row; // row max
            public int tier; // tier max            
            public Byte[] BlockBay; // C=Container, T=Tunnel, N=No Work Tier , A=No Work Area, ""=empty 
            public Byte cntrCount;
            public List<Common.VD_Common_Def_Inventory> cntr;

            public VD_RTG_BlockBayInfo_Receive()
            {
                bay = 0;
                row = 0;
                tier = 0;
                BlockBay = new Byte[0];
                cntrCount = 0;
                cntr = new List<Common.VD_Common_Def_Inventory>();
            }
        }

        [Serializable]
        public class VD_RTG_BlockBayInfoSimple_Receive
        {
            public int bay; // 해당 베이 번호
            public int row; // row max
            public int tier; // tier max            
            public Byte[] BlockBay; // C=Container, N=Tunnel, T=No Work Tier , A=No Work Area, ""=empty 
            public Byte cntrCount;
            public List<Common.VD_Common_Def_InventorySimple> cntr;

            public VD_RTG_BlockBayInfoSimple_Receive()
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
        public class VD_RTG_Correction_Send
        {
            public String cntrNo;
            public Common.VD_Common_Yard_Location fromLoc;
            public Common.VD_Common_Yard_Location toLoc;
            public String actionType;   // C=creation, D=delete, M=move 인벤토리정보(DB) 를 수정하는 형태

            public VD_RTG_Correction_Send()
            {
                cntrNo = String.Empty;
                fromLoc = new Common.VD_Common_Yard_Location();
                toLoc = new Common.VD_Common_Yard_Location();
                actionType = String.Empty;
            }
        }

        [Serializable]
        public class VD_RTG_SetCurrentJob_Send
        {
            public String jobKey;

            public VD_RTG_SetCurrentJob_Send()
            {
                jobKey = String.Empty;
            }
        }

        [Serializable]
        public class VD_RTG_SetCurrentJob_Receive
        {
            public String jobKey;

            public VD_RTG_SetCurrentJob_Receive()
            {
                jobKey = String.Empty;
            }
        }

        [Serializable]
        public class VD_RTG_HandleJobDone_Send
        {
            public String jobKey;
            public String WorkingMachineID;
            public String WorkingMachineTP;
            public String PartnerMachineID;
            public String PartnerMachineTP;
            public String cntrNo;
            public Common.VD_Common_Job_Location Loc;
            public Common.VD_Commmon_Spreader sprd;

            public VD_RTG_HandleJobDone_Send()
            {
                jobKey = String.Empty;
                WorkingMachineID = String.Empty;
                WorkingMachineTP = String.Empty;
                PartnerMachineID = String.Empty;
                PartnerMachineTP = String.Empty;
                cntrNo = String.Empty;
                Loc = new Common.VD_Common_Job_Location();
                sprd = new Common.VD_Commmon_Spreader();
            }
        }

        [Serializable]
        public class VD_RTG_TargetJob_Receive
        {
            public String jobKey;

            public VD_RTG_TargetJob_Receive()
            {
                jobKey = String.Empty;
            }
        }

        [Serializable]
        public class VD_RTG_RTGMarrying_Send
        {
            public String WorkingMachineID;
            public String WorkingMachineTP;
            public String PartnerMachineID;
            public String PartnerMachineTP;
            public String cntrNo;
            public Common.VD_Common_Job_Location Loc;

            public VD_RTG_RTGMarrying_Send()
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
        public class VD_RTG_RTGMarrying_Receive
        {
            public String WorkingMachineID;
            public String WorkingMachineTP;
            public String PartnerMachineID;
            public String PartnerMachineITP;
            public String cntrNo;
            public Common.VD_Common_Job_Location Loc;

            public VD_RTG_RTGMarrying_Receive()
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
        public class EmptySwapListSend
        {
            public String jobKey;
            public String orgCntrNo;
            public String newCntrNo;
            public String regoNo;

            public EmptySwapListSend()
            {
                jobKey = String.Empty;
                orgCntrNo = String.Empty;
                newCntrNo = String.Empty;
                regoNo = String.Empty;
            }
        }

        [Serializable]
        public class VD_RTG_SwapResult_Receive
        {
            public UInt32 swapResult; //PROCEED : 1  SUCCESS : 2  FAIL : 3  RETURN TO YARD  : 0

            public VD_RTG_SwapResult_Receive()
            {
                swapResult = 0;
            }
        }

        [Serializable]
        public class VD_RTG_ReturnWarning_Receive
        {
            public String PartnerMachineID;
            public UInt32 returnWarning; //0: Off      1: Display Warning

            public VD_RTG_ReturnWarning_Receive()
            {
                PartnerMachineID = String.Empty;
                returnWarning = 0;
            }
        }
        //------------------------------------------------------------------------------

        [Serializable]
        public class VD_RTG_JobOrderList
        {
            private int _iCount; // 갯수
            public List<Common.VD_Common_JobOrder> JobOrder;

            public VD_RTG_JobOrderList()
            {
                _iCount = -1;
                JobOrder = new List<Common.VD_Common_JobOrder>();
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
    }
}
