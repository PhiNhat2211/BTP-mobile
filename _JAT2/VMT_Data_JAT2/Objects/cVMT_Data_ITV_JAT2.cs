using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace VMT_Data_JAT2.Objects
{
    public class ITV
    {
        public enum ITV_Container_Type
        {
            Blank,
            Active,
            Processing,
            Full
        }

        public class ITV_User : UserInfo
        {
            public static ArrayList gJobOrderList = new ArrayList();
            public static String gFirstJobStatus = "";
            public static String gSecondJobStatus = "";

            public static String jobKey = "";

            public static String gServerTime = "";

            new public static void GetMachineType(ref String MchnTyp, ref int size)
            {
                String strIniFile = GetIniDirectory() + @"MachineInfo.ini";

                Ini.IniFile ini = new Ini.IniFile(strIniFile);
                MchnTyp = ini.IniReadValue("MACHINE", "TYPE", "YT");
            }

            new public static void GetMachineID(ref String MchnID, ref int size)
            {
                String strIniFile = GetIniDirectory() + @"MachineInfo.ini";

                Ini.IniFile ini = new Ini.IniFile(strIniFile);
                MchnID = ini.IniReadValue("MACHINE", "ID", "T001");
            }
        }

        [Serializable]
        public class VD_ITV_PDS_Periodic_Payload
        {
            //----------------PayLoad 77
            public UInt32 m_dwTime;               // __time32_t 타입의 시간.
            public Double m_dLatitude;            // 위도
            public Double m_dLongitude;           // 경도        
            public float m_fHeadingDegree;        // 헤딩
            public float m_fSpeedOver;            // 속도
            public int m_BForwardBackward;        // 0x00 = forward, 0x01 = backward
            public int m_BShockSensor;            // 0x00 = Run, 0x01 = Break Down
            public int m_BAccelerator;            // 0x00 = Run, 0x01 = Break Down
            public int m_BChassisSensor;          // 0x00 = Run, 0x01 = Break Down
            public int m_BFuelGage;               // 0x00 = Run, 0x01 = Break Down
            public int m_BCollisionDetection;     // 0x00 = Run, 0x01 = Break Down
            public int m_BTirePressure;           // 0x00 = Run, 0x01 = Break Down
            public int m_BContainerCheck;         // 0x00 = Run, 0x01 = Break Down
            public int m_BConCheck;               // 0x00 = Run, 0x01 = Break Down
            public int m_BDGPS_INS;               // 0x00 = Run, 0x01 = Break Down
            public int m_BOdometerPulse;          // 0x00 = Run, 0x01 = Break Down
            public int m_BaseStation_Status;      // 0 = disconnect, 1 = connect, 2 = Error
            public int m_DGPS_Status;             // 0 = disconnect, 1 = connect, 2 = Error
            public int m_PDS_Status;              // 0 = disconnect, 1 = connect, 2 = Error  
            public int m_TOSClient_Status;        // 0 = disconnect, 1 = connect, 2 = Error
            public int m_EagleEyeEvent_Status;    // 0 = disconnect, 1 = connect, 2 = Error
            public int m_EagleEyePeriodic_Status; // 0 = disconnect, 1 = connect, 2 = Error  

            public VD_ITV_PDS_Periodic_Payload()
            {
                m_dwTime = 0;
                m_dLatitude = 0;
                m_dLongitude = 0;
                m_fHeadingDegree = 0;
                m_fSpeedOver = 0;
                m_BForwardBackward = 0;
                m_BShockSensor = 0;
                m_BAccelerator = 0;
                m_BChassisSensor = 0;
                m_BFuelGage = 0;
                m_BCollisionDetection = 0;
                m_BTirePressure = 0;
                m_BContainerCheck = 0;
                m_BConCheck = 0;
                m_BDGPS_INS = 0;
                m_BOdometerPulse = 0;
                m_BaseStation_Status = 0;
                m_DGPS_Status = 0;
                m_PDS_Status = 0;
                m_TOSClient_Status = 0;
                m_EagleEyeEvent_Status = 0;
                m_EagleEyePeriodic_Status = 0;
            }
        }

        [Serializable]
        public class VD_ITV_PDS_Event_Payload
        {
            //----------------PayLoad 77
            public UInt32 m_dwTime;					  // __time32_t 타입의 시간.

            public int m_sShockSensorX;         // -512 ~ 511
            public int m_sShockSensorY;         // -512 ~ 511
            public int m_sShockSensorZ;		  // -512 ~ 511
            public int m_sAcceleratorX;         // -512 ~ 511
            public int m_sAcceleratorY;         // -512 ~ 511 
            public int m_sAcceleratorZ;         // -512 ~ 511

            public int m_ucDeviceStatus;         // 0 = DeviceNoStart, 1 = Run, 3 = BreakDown
            public int m_BChassisSensor;         // 샤시 장착 
            public int m_ucFuelGage;            // 0~999 주유용량
            public int m_ucTireGage;            // 0~999 타이어용량
            public int m_BCollisionDetection;    // 충돌감지? (두자리라 99까지)
            public int m_BContainerCheck;        // 1 = 0n, 0 = 0ff = Container
            public int m_BConCheck;              // 1 = 0n, 0 = 0ff = Cone

            public VD_ITV_PDS_Event_Payload()
            {
                m_dwTime = 0;

                m_sShockSensorX = 0;
                m_sShockSensorY = 0;
                m_sShockSensorZ = 0;
                m_sAcceleratorX = 0;
                m_sAcceleratorY = 0;
                m_sAcceleratorZ = 0;

                m_ucDeviceStatus = 0;
                m_BChassisSensor = 0;
                m_ucFuelGage = 0;
                m_ucTireGage = 0;
                m_BCollisionDetection = 0;
                m_BContainerCheck = 0;
                m_BConCheck = 0;
            }
        }

        [Serializable]
        public class VD_ITV_ChassisAttachInfo_Send
        {
            public enum ChassisType : int
            {
                None = 0,
                Foot20,
                Foot40,
                GooseNeck,
                Special
            };

            public ChassisType nType;
            public String m_ChassisNumber;

            public VD_ITV_ChassisAttachInfo_Send()
            {
                nType = ChassisType.None;
                m_ChassisNumber = String.Empty;
            }
        }

        [Serializable]
        public class VD_ITV_ChassisAttachInfo_Receive
        {
            public enum ChassisType : int
            {
                None = 0,
                Foot20,
                Foot40,
                GooseNeck,
                Special
            };

            public ChassisType nType;
            public String m_ChassisNumber;

            public VD_ITV_ChassisAttachInfo_Receive()
            {
                nType = ChassisType.None;
                m_ChassisNumber = String.Empty;
            }
        }

        [Serializable]
        public class VD_ITV_NotifyBlockEnter_Receive
        {
            public String m_BlockName;
            public Boolean m_bEntrance; //TRUE: in, FALSE: out

            public VD_ITV_NotifyBlockEnter_Receive()
            {
                m_BlockName = String.Empty;
                m_bEntrance = false;
            }
        }

        [Serializable]
        public class VD_ITV_NotifyCPSAlign_Receive
        {
            public String PartnerMchnID;
            public int iAlignment;

            public VD_ITV_NotifyCPSAlign_Receive()
            {
                PartnerMchnID = String.Empty;
                iAlignment = 0;
            }
        }

        [Serializable]
        public class VD_ITV_SetManuaArrival_Send
        {
            public String WorkingMchnID;
            public String PartnerMchnID;

            public VD_ITV_SetManuaArrival_Send()
            {
                WorkingMchnID = String.Empty;
                PartnerMchnID = String.Empty;
            }
        }

        [Serializable]
        public class VD_ITV_SetManualArrival_Receive
        {
            public String WorkingMchnID;
            public String PartnerMchnID;
            public Boolean m_bPOWIN; // TRUE : in FALSE  : out

            public VD_ITV_SetManualArrival_Receive()
            {
                WorkingMchnID = String.Empty;
                PartnerMchnID = String.Empty;
                m_bPOWIN = false;
            }
        }

        [Serializable]
        public class VD_ITV_SetManualReady_Send
        {
            public String WorkingMchnID;
            public String PartnerMchnID;

            public VD_ITV_SetManualReady_Send()
            {
                WorkingMchnID = String.Empty;
                PartnerMchnID = String.Empty;                
            }
        }

        [Serializable]
        public class VD_ITV_SetManualReady_Receive
        {
            public String WorkingMchnID;
            public String PartnerMchnID;
            public int iReadyResult;

            public VD_ITV_SetManualReady_Receive()
            {
                WorkingMchnID = String.Empty;
                PartnerMchnID = String.Empty;
                iReadyResult = -1;
            }
        }

        [Serializable]
        public class VD_ITV_SetManualDone_Send
        {
            public String jobKey;

            public VD_ITV_SetManualDone_Send()
            {
                jobKey = String.Empty;
            }
        }

        [Serializable]
        public class VD_ITV_SetManualDone_Receive
        {
            public int iDoneResult;

            public VD_ITV_SetManualDone_Receive()
            {
                iDoneResult = -1;
            }
        }

        [Serializable]
        public class VD_ITV_PlanSeq
        {
            public String MchnID;
            public String MchnType;
            public int planSeq;

            public VD_ITV_PlanSeq()
            {
                MchnID = String.Empty;
                MchnType = String.Empty;
                planSeq = -1;
            }
        }

        [Serializable]
        public class VD_ITV_STS_LDPlan
        {
            public String StsID;
            public int nCount;
            public List<VD_ITV_PlanSeq> MchnPlan;  // max 5, but reserved by 10

            public VD_ITV_STS_LDPlan()
            {
                StsID = String.Empty;
                nCount = -1;
                MchnPlan = new List<VD_ITV_PlanSeq>();
            }
        }

        [Serializable]
        public class VD_ITV_JobOrderSub
        {
            public String szReadyArrivalFlag_0;
            public int nETA;
            public int iPlace; //0: None 1:Forward 2:After 3:Center

            public VD_ITV_JobOrderSub()
            {
                szReadyArrivalFlag_0 = String.Empty;
                nETA = -1;
                iPlace = 0;
            }
        }

        [Serializable]
        public class VD_ITV_Result
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

            public VD_ITV_Result()
            {
                resultTP = "PROCEED";
                resultObj = String.Empty;
            }
        }

        [Serializable]
        public class VD_Common_ChassisInventory
        {
            public String ordSeq;

            public String chssNo;

            public String crntPsnIdxNo1;

            public String crntPsnIdxNo2;

            public String crntPsnIdxNo3;

            public String crntPsnIdxNo4;

            public String plnPsnIdxNo1;

            public String plnPsnIdxNo2;

            public String plnPsnIdxNo3;

            public String plnPsnIdxNo4;

            public String ordCrntPsnIdxNo1;

            public String ordCrntPsnIdxNo2;

            public String ordCrntPsnIdxNo3;

            public String ordCrntPsnIdxNo4;

            public String itvCd;

            public String itvIdxNo;

            public String ordItvCd;

            public String ordStsCd;

            public String jobOdrSeqNo;

            public String crntPsnBlck;

            public String crntPsnBay;

            public String crntPsnRow;

            public String crntPsnTier;

            public String plnPsnBlck;

            public String plnPsnBay;

            public String plnPsnRow;

            public String plnPsnTier;


            public VD_Common_ChassisInventory()
            {
                ordSeq = String.Empty;
                chssNo = String.Empty;
                crntPsnIdxNo1 = String.Empty;
                crntPsnIdxNo2 = String.Empty;
                crntPsnIdxNo3 = String.Empty;
                crntPsnIdxNo4 = String.Empty;
                plnPsnIdxNo1 = String.Empty;
                plnPsnIdxNo2 = String.Empty;
                plnPsnIdxNo3 = String.Empty;
                plnPsnIdxNo4 = String.Empty;
                ordCrntPsnIdxNo1 = String.Empty;
                ordCrntPsnIdxNo2 = String.Empty;
                ordCrntPsnIdxNo3 = String.Empty;
                ordCrntPsnIdxNo4 = String.Empty;
                itvCd = String.Empty;
                itvIdxNo = String.Empty;
                ordItvCd = String.Empty;
                ordStsCd = String.Empty;
                jobOdrSeqNo = String.Empty;
                crntPsnBlck = String.Empty;
                crntPsnBay = String.Empty;
                crntPsnRow = String.Empty;
                crntPsnTier = String.Empty;
                plnPsnBlck = String.Empty;
                plnPsnBay = String.Empty;
                plnPsnRow = String.Empty;
                plnPsnTier = String.Empty;
            }
        }

        [Serializable]
        public class VD_ITV_JobOrderList
        {
            private int _iCount; // 갯수
            public List<Common.VD_Common_JobOrder> JobOrder;
            public List<VD_ITV_JobOrderSub> Sub;

            public VD_ITV_JobOrderList()
            {
                _iCount = -1;
                JobOrder = new List<Common.VD_Common_JobOrder>();
                Sub = new List<VD_ITV_JobOrderSub>();
            }

            public int Count
            {
                get
                {
                    _iCount = JobOrder.Count;
                    return _iCount;
                }
            }

            public Common.VD_Common_JobOrder FirstJob
            {
                get
                {
                    if (JobOrder.Count > 0 && JobOrder.ElementAt(0).type.jobFlagInfo != "A") //201090807
                        return JobOrder.ElementAt(0);
                    else if (JobOrder.Count > 1 && JobOrder.ElementAt(1).type.jobFlagInfo != "A")
                        return JobOrder.ElementAt(1);
                    else
                        return new Common.VD_Common_JobOrder();
                }
            }

            public VD_ITV_JobOrderSub FirstSub
            {
                get
                {
                    if (JobOrder.Count > 0 && JobOrder.ElementAt(0).type.jobFlagInfo != "A") //201090807
                        return Sub.ElementAt(0);
                    else if (JobOrder.Count > 1 && JobOrder.ElementAt(1).type.jobFlagInfo != "A")
                        return Sub.ElementAt(1);
                    else
                        return new VD_ITV_JobOrderSub();
                }
            }

            public String FirstJobStatus
            {
                set
                {
                    if (FirstJob != null)
                        FirstJob.type.jobStatus = value;
                }
                get
                {
                    if (FirstJob != null)
                        return FirstJob.type.jobStatus;
                    else
                        return "";
                }
            }

            public Common.VD_Common_JobOrder SecondJob
            {
                get
                {
                    if (JobOrder.Count > 0 && JobOrder.ElementAt(0).type.jobFlagInfo == "A") //20190807
                        return JobOrder.ElementAt(0);
                    else if (JobOrder.Count > 1 && JobOrder.ElementAt(1).type.jobFlagInfo == "A")
                        return JobOrder.ElementAt(1);
                    else
                        return new Common.VD_Common_JobOrder();
                }
            }

            public VD_ITV_JobOrderSub SecondSub
            {
                get
                {
                    if (JobOrder.Count > 0 && JobOrder.ElementAt(0).type.jobFlagInfo == "A") //20190807
                        return Sub.ElementAt(0);
                    else if (JobOrder.Count > 1 && JobOrder.ElementAt(1).type.jobFlagInfo == "A")
                        return Sub.ElementAt(1);
                    else
                        return new VD_ITV_JobOrderSub();
                }
            }

            public String SecondJobStatus
            {
                set
                {
                    if (SecondJob != null)
                        SecondJob.type.jobStatus = value;
                }
                get
                {
                    if (SecondJob != null)
                        return SecondJob.type.jobStatus;
                    else
                        return "";
                }
            }
        }
    }
}
