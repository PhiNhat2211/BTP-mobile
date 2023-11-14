using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace TCPComm.EEStruct
{
    //########################################################################
    //
    //
    //                                                      Sensing Device
    //
    //
    //########################################################################

    //---------------------------------------------------------
    //  
    // Protocol : VMT_TU_ITV_DGPS_Periodic
    //                
    //---------------------------------------------------------
    // Notify   Tray -> UI
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto, Pack = 1)]
    [Serializable]
    public class sTrayUI_sITV_PDS_Periodic_Payload : EEParentClass
    {
        //----------------PayLoad 77
        public UInt32 m_dwTime;             // __time32_t 타입의 시간.
        public Double m_dLatitude;          // 위도
        public Double m_dLongitude;         // 경도        
        public float m_fHeadingDegree;      // 헤딩
        public float m_fSpeedOver;          // 속도
        public int m_BForwardBackward;     // 0x00 = forward, 0x01 = backward
        public int m_BShockSensor;         // 0x00 = Run, 0x01 = Break Down
        public int m_BAccelerator;         // 0x00 = Run, 0x01 = Break Down
        public int m_BChassisSensor;       // 0x00 = Run, 0x01 = Break Down
        public int m_BFuelGage;            // 0x00 = Run, 0x01 = Break Down
        public int m_BCollisionDetection;  // 0x00 = Run, 0x01 = Break Down
        public int m_BTirePressure;        // 0x00 = Run, 0x01 = Break Down
        public int m_BContainerCheck;      // 0x00 = Run, 0x01 = Break Down
        public int m_BConCheck;            // 0x00 = Run, 0x01 = Break Down
        public int m_BDGPS_INS;            // 0x00 = Run, 0x01 = Break Down
        public int m_BOdometerPulse;       // 0x00 = Run, 0x01 = Break Down

        public int m_BaseStation_Status;     // 0 = disconnect, 1 = connect, 2 = Error
        public int m_DGPS_Status;            // 0 = disconnect, 1 = connect, 2 = Error
        public int m_PDS_Status;             // 0 = disconnect, 1 = connect, 2 = Error  
        public int m_TOSClient_Status;       // 0 = disconnect, 1 = connect, 2 = Error
        public int m_EagleEyeEvent_Status;   // 0 = disconnect, 1 = connect, 2 = Error
        public int m_EagleEyePeriodic_Status;// 0 = disconnect, 1 = connect, 2 = Error  
    }

    //---------------------------------------------------------
    //  
    // Protocol : VMT_TU_ITV_PDS
    //                
    //---------------------------------------------------------
    // Notify   Tray -> UI
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto, Pack = 1)]
    [Serializable]
    public class sTrayUI_sITV_PDS_Event_Payload : EEParentClass
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
    }

    //---------------------------------------------------------
    //  
    // Protocol : VMT_TU_SetChassis_Attach
    //                
    //---------------------------------------------------------
    // Send  UI -> Tray
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto, Pack = 1)]
    [Serializable]
    public class sTrayUI_ChassisAttachInfo : EEParentClass
    {
        public enum ChassisType : int
        {
            None = 0,
            Foot20,
            Foot40,
            GooseNeck,
            Special
        };

        public int nType;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 30)]
        public String m_ChassisNumber;
    };
    // Ack    Tray -> UI
    // public class 없음

    //---------------------------------------------------------
    //  
    // Protocol : VMT_TU_NofityBlockEnterance
    //                
    //---------------------------------------------------------
    // Notify   Tray -> UI
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto, Pack = 1)]
    [Serializable]
    public class sTrayUI_BlockEnter : EEParentClass
    {
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = EEv2StringMax.EEV2_STRING_MAX_LOCATION_BLOCK)]
        public String m_BlockName;
        [MarshalAs(UnmanagedType.Bool)]
        public Boolean m_bEntrance; //TRUE: in, FALSE: out
    };

    //---------------------------------------------------------
    //  
    // Protocol : VMT_TU_NotifyCPSAlign
    //                
    //---------------------------------------------------------
    // Notify   Tray -> UI
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto, Pack = 1)]
    [Serializable]
    public class sTrayUI_NotifyCPSAlign : EEParentClass
    {
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = EEv2StringMax.EEV2_STRING_MAX_MACHINE_ID)]
        public String PartnerMchnID;
        public int iAlignment;
    };

    //---------------------------------------------------------
    //  
    // Protocol : VMT_TU_SetManualArrival
    //                
    //---------------------------------------------------------
    // Send    UI -> Tray
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto, Pack = 1)]
    [Serializable]
    public class sTrayUI_SetManuaArrivalRQ : EEParentClass
    {
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = EEv2StringMax.EEV2_STRING_MAX_MACHINE_ID)]
        public String WorkingMchnID;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = EEv2StringMax.EEV2_STRING_MAX_MACHINE_ID)]
        public String PartnerMchnID;
    }
    // Responce  Tray -> UI
    // public class없음.

    //---------------------------------------------------------
    //  
    // Protocol : VMT_TU_NotifyManualReady
    //                
    //---------------------------------------------------------
    // Notify , Responce  Tray -> UI
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto, Pack = 1)]
    [Serializable]
    public class sTrayUI_NotifyManualArrivalRP : EEParentClass
    {
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = EEv2StringMax.EEV2_STRING_MAX_MACHINE_ID)]
        public String WorkingMchnID;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = EEv2StringMax.EEV2_STRING_MAX_MACHINE_ID)]
        public String PartnerMchnID;
        [MarshalAs(UnmanagedType.Bool)]
        public Boolean m_bPOWIN; // TRUE : in FALSE  : out
    }

    //---------------------------------------------------------
    //  
    // Protocol : VMT_TU_SetManualReady
    //                
    //---------------------------------------------------------
    // Send    UI -> Tray
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto, Pack = 1)]
    [Serializable]
    public class sTrayUI_SetManualReadyRQ : EEParentClass
    {
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = EEv2StringMax.EEV2_STRING_MAX_MACHINE_ID)]
        public String WorkingMchnID;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = EEv2StringMax.EEV2_STRING_MAX_MACHINE_ID)]
        public String PartnerMchnID;
    };
    // Responce  Tray -> UI
    // public class없음.

    //---------------------------------------------------------
    //  
    // Protocol : VMT_TU_NotifyManualReady
    //                
    //---------------------------------------------------------
    // Notify  Tray -> UI
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto, Pack = 1)]
    [Serializable]
    public class sTrayUI_NotifyManualReady : EEParentClass
    {
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = EEv2StringMax.EEV2_STRING_MAX_MACHINE_ID)]
        public String WorkingMchnID;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = EEv2StringMax.EEV2_STRING_MAX_MACHINE_ID)]
        public String PartnerMchnID;
        public int iReadyResult;
        // Ready에 대한 결과값                                    
        //0 : 성공 
        // ============ 요청에 의한 결과 (Reason) ========== 
        // 48 : 작업이 없음
        // 50 : 서버의 참조 데이터가 유효하지 않음
        // 51 : 우선권을 가진 Ready가 있어 반려함
        // 52 : 그 외 포괄적 거부( Reject )
        // ============ 강제 ========================
        // 100 : ACH로 Ready강제 해제
    };

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto, Pack = 1)]
    [Serializable]
    public class sTrayUI_ITVJobOrderSub : EEParentClass
    {
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = EEv2StringMax.EEV2_STRING_MAX_ITV_JOB_STAT)]
        public String szReadyArrivalFlag_0;
        public int nETA;
        public int iPlace; //0: None 1:Forward 2:After 3:Center
    };

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto, Pack = 1)]
    [Serializable]
    public class EEv3JobOrderList_ITV : EEParentClass
    {
        public int iCount; // 갯수
        public EEv3JobOrder[] JobOrder;
        public sTrayUI_ITVJobOrderSub[] Sub;
    }
}