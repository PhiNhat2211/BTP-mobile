using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace VMT_Data_JAT2.Objects
{
    public class SC
    {
        public enum SC_Container_Type
        {
            Blank,
            Active,
            Processing,
            Full
        }

        public class SC_User : UserInfo
        {
            public static ArrayList gJobOrderList = new ArrayList();
            public static String gFirstJobStatus = "";
            public static String gSecondJobStatus = "";

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
        public class VD_SC_PDS_Periodic_Payload
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

            public VD_SC_PDS_Periodic_Payload()
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
        public class VD_SC_PDS_Event_Payload
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

            public VD_SC_PDS_Event_Payload()
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
        public class VD_SC_ChassisAttachInfo_Send
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

            public VD_SC_ChassisAttachInfo_Send()
            {
                nType = ChassisType.None;
                m_ChassisNumber = String.Empty;
            }
        }

        [Serializable]
        public class VD_SC_ChassisAttachInfo_Receive
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

            public VD_SC_ChassisAttachInfo_Receive()
            {
                nType = ChassisType.None;
                m_ChassisNumber = String.Empty;
            }
        }

        [Serializable]
        public class VD_SC_NotifyBlockEnter_Receive
        {
            public String m_BlockName;
            public Boolean m_bEntrance; //TRUE: in, FALSE: out

            public VD_SC_NotifyBlockEnter_Receive()
            {
                m_BlockName = String.Empty;
                m_bEntrance = false;
            }
        }

        [Serializable]
        public class VD_SC_NotifyCPSAlign_Receive
        {
            public String PartnerMchnID;
            public int iAlignment;

            public VD_SC_NotifyCPSAlign_Receive()
            {
                PartnerMchnID = String.Empty;
                iAlignment = 0;
            }
        }

        [Serializable]
        public class VD_SC_SetManuaArrival_Send
        {
            public String WorkingMchnID;
            public String PartnerMchnID;

            public VD_SC_SetManuaArrival_Send()
            {
                WorkingMchnID = String.Empty;
                PartnerMchnID = String.Empty;
            }
        }

        [Serializable]
        public class VD_SC_SetManualArrival_Receive
        {
            public String WorkingMchnID;
            public String PartnerMchnID;
            public Boolean m_bPOWIN; // TRUE : in FALSE  : out

            public VD_SC_SetManualArrival_Receive()
            {
                WorkingMchnID = String.Empty;
                PartnerMchnID = String.Empty;
                m_bPOWIN = false;
            }
        }

        [Serializable]
        public class VD_SC_SetManualReady_Send
        {
            public String WorkingMchnID;
            public String PartnerMchnID;

            public VD_SC_SetManualReady_Send()
            {
                WorkingMchnID = String.Empty;
                PartnerMchnID = String.Empty;                
            }
        }

        [Serializable]
        public class VD_SC_SetManualReady_Receive
        {
            public String WorkingMchnID;
            public String PartnerMchnID;
            public int iReadyResult;

            public VD_SC_SetManualReady_Receive()
            {
                WorkingMchnID = String.Empty;
                PartnerMchnID = String.Empty;
                iReadyResult = -1;
            }
        }

        [Serializable]
        public class VD_SC_SetManualDone_Send
        {
            public String jobKey;

            public VD_SC_SetManualDone_Send()
            {
                jobKey = String.Empty;
            }
        }

        [Serializable]
        public class VD_SC_SetManualDone_Receive
        {
            public int iDoneResult;

            public VD_SC_SetManualDone_Receive()
            {
                iDoneResult = -1;
            }
        }

        [Serializable]
        public class VD_SC_PlanSeq
        {
            public String MchnID;
            public String MchnType;
            public int planSeq;

            public VD_SC_PlanSeq()
            {
                MchnID = String.Empty;
                MchnType = String.Empty;
                planSeq = -1;
            }
        }

        [Serializable]
        public class VD_SC_STS_LDPlan
        {
            public String StsID;
            public int nCount;
            public List<VD_SC_PlanSeq> MchnPlan;  // max 5, but reserved by 10

            public VD_SC_STS_LDPlan()
            {
                StsID = String.Empty;
                nCount = -1;
                MchnPlan = new List<VD_SC_PlanSeq>();
            }
        }

        [Serializable]
        public class VD_SC_JobOrderSub
        {
            public String szReadyArrivalFlag_0;
            public int nETA;
            public int iPlace; //0: None 1:Forward 2:After 3:Center

            public VD_SC_JobOrderSub()
            {
                szReadyArrivalFlag_0 = String.Empty;
                nETA = -1;
                iPlace = 0;
            }
        }

        [Serializable]
        public class VD_SC_JobOrderList
        {
            private int _iCount; // 갯수
            public List<Common.VD_Common_JobOrder> JobOrder;
            public List<VD_SC_JobOrderSub> Sub;

            public VD_SC_JobOrderList()
            {
                _iCount = -1;
                JobOrder = new List<Common.VD_Common_JobOrder>();
                Sub = new List<VD_SC_JobOrderSub>();
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
                    if (JobOrder.Count > 0)
                        return JobOrder.ElementAt(0);
                    else
                        return new Common.VD_Common_JobOrder();
                }
            }

            public VD_SC_JobOrderSub FirstSub
            {
                get
                {
                    if (Sub.Count > 0)
                        return Sub.ElementAt(0);
                    else
                        return new VD_SC_JobOrderSub();
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
                    if (JobOrder.Count > 1)
                        return JobOrder.ElementAt(1);
                    else
                        return new Common.VD_Common_JobOrder();
                }
            }

            public VD_SC_JobOrderSub SecondSub
            {
                get
                {
                    if (JobOrder.Count > 1)
                        return Sub.ElementAt(1);
                    else
                        return new VD_SC_JobOrderSub();
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
