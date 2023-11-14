using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Collections;


namespace VMT_Data_JAT2.Objects
{
    public class UserInfo
    {
        public static Boolean IsUseHessian = true;
        public static Boolean IsUseTCP = false;
        public static Boolean IsUseUDP = false;

        public enum MchnTp
        {
            MchnTp_Unknown,
            MchnTp_ITV,
            MchnTp_RTG,
            MchnTp_ECH,
            MchnTp_RS,
            MchnTp_RMG,
        }

        public static String gMchnID = "";
        public static String gMchnTp = "";
        public static String gUserID = "";
        public static String gUserPW = "";
        public static String gUserNm = "";
        public static Dictionary<string, int> gJobTypeSortOrder = new Dictionary<string, int>();

        public static MchnTp GetMchnTp()
        {
            MchnTp returnType = MchnTp.MchnTp_Unknown;

            if (gMchnTp.Equals("YT") || gMchnTp.Equals("ITV") || gMchnTp.Equals("SC"))
                returnType = MchnTp.MchnTp_ITV;
            else if (gMchnTp.Equals("TC") || gMchnTp.Equals("RMG"))
                returnType = MchnTp.MchnTp_RMG;
            else if (gMchnTp.Equals("EH") || gMchnTp.Equals("TH"))
                returnType = MchnTp.MchnTp_ECH;
            else if (gMchnTp.Equals("RS"))
                returnType = MchnTp.MchnTp_RS;
            else
                returnType = MchnTp.MchnTp_Unknown;

            return returnType;
        }

        public static void GetMachineType(ref String MchnTyp, ref int size)
        {
            String strIniFile = GetIniDirectory() + @"MachineInfo.ini";

            Ini.IniFile ini = new Ini.IniFile(strIniFile);
            MchnTyp = ini.IniReadValue("MACHINE", "TYPE", "YT");
        }

        public static void GetMachineID(ref String MchnID, ref int size)
        {
            String strIniFile = GetIniDirectory() + @"MachineInfo.ini";

            Ini.IniFile ini = new Ini.IniFile(strIniFile);
            MchnID = ini.IniReadValue("MACHINE", "ID", "T001");
        }

        public static string GetAppDirectory()
        {
            string path = null;

            path = AppDomain.CurrentDomain.BaseDirectory;

            return path;
        }

        public static string GetIniDirectory()
        {
            String path = null;

            DirectoryInfo dInfo = Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory);
            String strParentPath = dInfo.Parent.FullName;
            String strCommonPath = strParentPath + @"\Common";

            if (!Directory.Exists(strCommonPath))
                Directory.CreateDirectory(strCommonPath);

            path = strCommonPath + @"\";

            return path;
        }
    }

    public class Common
    {
        public static String BlckVal = "";
        public enum Container_Type
        {
            Blank,
            Active,
            Processing,
            Full
        }

        #region [VMT_Data_Common Class]

        //------------------------------------------
        // EAHS_VMT_DRIVER_INFO
        //------------------------------------------  

        [Serializable]
        public class VD_Common_Swinfo_Send
        {
            public String m_VMTUI_Version;
            public String m_szMchnID;
            public String m_szMchnTp;

            public VD_Common_Swinfo_Send()
            {
                m_VMTUI_Version = String.Empty;
                m_szMchnID = String.Empty;
                m_szMchnTp = String.Empty;
            }
        }

        [Serializable]
        public class VD_Common_Swinfo_Receive
        {
            public int m_iResult;      // 0 접속불허,    1 접속허가.
            public int m_iLoginResult; //  0 login이 안된상태   1  login이된상태 
            public String m_VMTTray_Version;
            public String m_UserID;
            public String m_UserPW;
            public String m_GroupName;
            public String m_DriverName;
            public String m_szSite; // KAP, JAT3 

            public VD_Common_Swinfo_Receive()
            {
                m_iResult = -1;
                m_VMTTray_Version = String.Empty;
                m_UserID = String.Empty;
                m_UserPW = String.Empty;
                m_GroupName = String.Empty;
                m_DriverName = String.Empty;
                m_szSite = String.Empty;
            }
        }

        [Serializable]
        public class VD_Common_ConnectionStatus_Receive
        {
            public int m_iEagleEyeStatus; //0 이면 연결안됨,    1이면 연결됨
            public int m_iGPSStatus;      // 0 이면 연결안됨,    1이면 연결됨

            public VD_Common_ConnectionStatus_Receive()
            {
                m_iEagleEyeStatus = -1;
                m_iGPSStatus = -1;
            }
        }

        [Serializable]
        public class VD_Common_MachineNotice_Receive
        {
            public int m_iMessageType; //0: 없음 1: TOS  2: VT  3: EE.Server 4: Tray
            public String m_strMessage;
            public String m_strMessage2;

            public VD_Common_MachineNotice_Receive()
            {
                m_iMessageType = -1;
                m_strMessage = String.Empty;
                m_strMessage2 = String.Empty;
            }
        }

        //------------------------------------------------------------------------------
        //- Login
        [Serializable]
        public class VD_Common_GetUserAccesRole_Send
        {
            public String UserID;

            public VD_Common_GetUserAccesRole_Send()
            {
                UserID = String.Empty;
            }
        }

        [Serializable]
        public class VD_Common_GetUserAccesRole_Receive
        {
            public Boolean bIsOn; // TRUE 성공,FALSE 실패
            public String GroupListSeperator; // "Group1|Group2|Group3" 형태로 전달
            public String Notice;
            public String shift;
            public String usrNm;

            public VD_Common_GetUserAccesRole_Receive()
            {
                bIsOn = false;
                GroupListSeperator = String.Empty;
                Notice = String.Empty;
                shift = String.Empty;
                usrNm = String.Empty;
            }
        }

        [Serializable]
        public class VD_Common_SetLogin4Machine_Send
        {
            public String UserID;
            public String UserPW;
            public String GroupName;
            public String MchnID;
            public String MchnTp;
            public String chssNo;

            public VD_Common_SetLogin4Machine_Send()
            {
                UserID = String.Empty;
                UserPW = String.Empty;
                GroupName = String.Empty;
                MchnID = String.Empty;
                MchnTp = String.Empty;
                chssNo = String.Empty;
            }
        }

        [Serializable]
        public class VD_Common_SetLogin4Machine_Receive
        {
            public int iLogin; // 0 : 로그인 실패  1: 로그인성공  2: 이미로그인상태            
            public String UserName;
            public String Notice;
            public String chssNo;
            public VD_Common_Yard_Location loc;

            public VD_Common_SetLogin4Machine_Receive()
            {
                iLogin = -1;
                UserName = String.Empty;
                Notice = String.Empty;
                chssNo = String.Empty;
                loc = new VD_Common_Yard_Location();
            }
        }

        [Serializable]
        public class VD_Common_Config_Receive
        {
            public Boolean Arrival;
            public Boolean Ready;
            public Boolean Done;
            public Boolean ShowUnplugReeferOnly;
            public Boolean autoFlg;
            public String mchnId;
            public String loc;
            public String mchnTp;
            public String chssNo;
            public String jobItemColor;

            public VD_Common_Config_Receive()
            {
                Arrival = false;
                Ready = false;
                Done = false;
                ShowUnplugReeferOnly = false;
                mchnId = String.Empty;
                loc = String.Empty;
                mchnTp = String.Empty;
                chssNo = String.Empty;
                jobItemColor = String.Empty;
            }
        }

        [Serializable]
        public class VD_Common_SendMachineStatusChange_Send
        {
            public String m_MchnID;
            public String m_UserID;
            public Boolean m_bisON;  //True  ,   False
            public Boolean m_buseRemark;
            public String m_remark;

            public VD_Common_SendMachineStatusChange_Send()
            {
                m_MchnID = String.Empty;
                m_UserID = String.Empty;
                m_bisON = false;
                m_buseRemark = false;
                m_remark = String.Empty;
            }
        }

        [Serializable]
        public class VD_Common_SendMachineStatusChange_Receive
        {
            public int m_iResult;		      // 1:  성공 , 0 : 실패

            public VD_Common_SendMachineStatusChange_Receive()
            {
                // m_iResult = -1;

                //Main 화면을 표출하기 위해 -1 인 것을 무조건 성공으로 처리함
                m_iResult = 1;
            }
        }
        //------------------------------------------------------------------------------

        //------------------------------------------------------------------------------
        //- Common Sensing Device
        [Serializable]
        public class VD_Common_RequestCfgekf_Receive
        {
            public int m_iDirection;

            public VD_Common_RequestCfgekf_Receive()
            {
                m_iDirection = -1;
            }
        }

        //------------------------------------------------------------------------------

        //------------------------------------------------------------------------------
        //- Available
        [Serializable]
        public class VD_Common_Available
        {
            public String ReasonCd;
            public String ReasonNm;

            public VD_Common_Available()
            {
                ReasonCd = String.Empty;
                ReasonNm = String.Empty;
            }
        }

        [Serializable]
        public class VD_Common_MachineStopCodeList_Receive
        {
            public int m_iAvailableCount; //갯수
            public List<VD_Common_Available> m_pData; // Dynamic

            public VD_Common_MachineStopCodeList_Receive()
            {
                m_iAvailableCount = -1;
                m_pData = new List<VD_Common_Available>();
            }
        }

        [Serializable]
        public class VD_Common_MachineAccessAction_Receive
        {
            public bool showSetting, showCHGLOC, showViewINV, viewBLockList, enableItvSwap;

            public VD_Common_MachineAccessAction_Receive()
            {
                showSetting = false;
                showCHGLOC = false;
                showViewINV = false;
                viewBLockList = false;
                enableItvSwap = false;
            }
        }

        [Serializable]
        public class VD_Common_GetMachineStop_Receive
        {
            public VD_Common_Available Data;
            public int m_iBreak; //0 : No Break    1: Breaking
            public long StartTime;
            public long FinishTime;
            public String remark;

            public VD_Common_GetMachineStop_Receive()
            {
                Data = new VD_Common_Available();
                m_iBreak = -1;
                StartTime = 0;
                FinishTime = 0;
                remark = String.Empty;
            }
        }

        [Serializable]
        public class VD_Common_SetMachineStop_Send
        {
            public VD_Common_Available Data;
            public int m_iBreakStatus; //1 : Break 2: Release
            public String m_szMchnID;
            public String m_szMchnTp;
            public String m_UserID;
            public String m_DriverName;
            public long m_StartTime;
            public long m_FinishTime;

            public VD_Common_SetMachineStop_Send()
            {
                Data = new VD_Common_Available();
                m_iBreakStatus = -1;
                m_szMchnID = String.Empty;
                m_szMchnTp = String.Empty;
                m_UserID = String.Empty;
                m_DriverName = String.Empty;
                m_StartTime = -1;
                m_FinishTime = -1;
            }
        }

        [Serializable]
        public class VD_Common_SetMachineStop_Receive
        {
            public VD_Common_Available Data;
            public int m_iBreakStatus; //1 : Break 2: Release
            public String m_szMchnID;
            public String m_szMchnTp;
            public String m_UserID;
            public String m_DriverName;
            public int m_iResult; // 1:  성공 , 0 : 실패

            public VD_Common_SetMachineStop_Receive()
            {
                Data = new VD_Common_Available();
                m_iBreakStatus = -1;
                m_szMchnID = String.Empty;
                m_szMchnTp = String.Empty;
                m_UserID = String.Empty;
                m_DriverName = String.Empty;
                m_iResult = -1;
            }
        }

        [Serializable]
        public class VD_Common_MachineStopConfirm_Receive
        {
            public String ReasonCd;
            public Boolean IsApproved;
            public String MchnStopDt;
            public String MchnRequestDt;   // 2015-08-10 Request Time Add
            public Boolean Acknowledgement; // 2015-09-18 Acknowledgement Add
            public Int32 Message_Period_Sec;  // 2015-09-18 Acknowledgement Add

            public VD_Common_MachineStopConfirm_Receive()
            {
                ReasonCd = String.Empty;
                IsApproved = false;
                MchnStopDt = String.Empty;
                MchnRequestDt = String.Empty;
                Acknowledgement = false;
                Message_Period_Sec = -1;
            }
        }
        //------------------------------------------------------------------------------

        //------------------------------------------------------------------------------
        //- Alarm
        [Serializable]
        public class VD_Common_Alram_Receive
        {
            public enum enVmtAlram : int
            {
                ALRAM_Unknown = 0,
                ALRAM_OVER_SPEED = 1,
                ALRAM_FUEL_LIMIT,
                ALRAM_TIRE_PRESSURE,
                ALRAM_OVRE_TEMP,
                ALRAM_CROSS_BLOCK
            };
            public enVmtAlram enType;
            public int nVaildTime;  //Alram 유지타임(sec)
            public Double nValue;   //서버가 감지한 알람상황의 근거데이터 ALRAM_CROSS_BLOCK = 1.0(최대)
            //0.0에 가까울수록 CROSS_BLOCK 근원지와 가까운것 
            public String szDesc;//ex) SpeedOver, FuelLimit

            public VD_Common_Alram_Receive()
            {
                enType = enVmtAlram.ALRAM_Unknown;
                nVaildTime = -1;
                nValue = 0;
                szDesc = String.Empty;
            }
        }

        [Serializable]
        public class VD_Common_Exception_Receive
        {
            //BreakAlarm 추가
            //BreakAlarm_ITV,
            //BreakAlarm_STS,
            //BreakAlarm_RTG,
            //BreakAlarm_RSEH,
            //BreakAlarm_Operation,
            //BreakAlarm_ARMGC

            public int exceptionType;
            public int nVaildTime;  //Alram 유지타임(sec)
            public Double nValue;   //서버가 감지한 알람상황의 근거데이터 ALRAM_CROSS_BLOCK = 1.0(최대)
            //0.0에 가까울수록 CROSS_BLOCK 근원지와 가까운것 
            public String szDesc;//ex) SpeedOver, FuelLimit

            public VD_Common_Exception_Receive()
            {
                exceptionType = -1;
                nVaildTime = -1;
                nValue = 0;
                szDesc = String.Empty;
            }
        }
        //------------------------------------------------------------------------------

        //------------------------------------------------------------------------------
        //- Job  Common
        [Serializable]
        public class VD_Common_Job_Machine : IEquatable<VD_Common_Job_Machine>
        {
            //머신 아이디
            public String mchnId;
            /*
            "Machine Code
            e.g) TT501, RS202,QC103"
            */
            //머신 타입
            public String mchnTp;
            public String ycTp;
            /*
            "Machine Type
            e.g) RS : Reach Stacker 
                 TC : Transfer Crane (JAT3 AMGC)(NCT RTG)
                 TH : Top Handler
                 QC : Quay Crane (STS)
                 YT : Internal Terminal Truck( Yard Tractor)" 
            */
            //장비 상태
            public String mchnSts;
            public String vrtlFlg; //Virtual Crane Flag "Y",""
            public String aprchLn;

            public VD_Common_Job_Machine()
            {
                mchnId = String.Empty;
                mchnTp = String.Empty;
                ycTp = String.Empty;
                mchnSts = String.Empty;
                vrtlFlg = String.Empty;
                aprchLn = String.Empty;
            }

            public override bool Equals(Object obj)
            {
                var other = obj as VD_Common_Job_Machine;
                if (other == null) return false;

                return Equals(other);
            }

            public bool Equals(VD_Common_Job_Machine other)
            {
                if (other == null)
                {
                    return false;
                }

                if (ReferenceEquals(this, other))
                {
                    return true;
                }

                // You can also use a specific StringComparer instead of EqualityComparer<string>
                // Check out the specific implementations (StringComparer.CurrentCulture, e.a.).
                if (EqualityComparer<String>.Default.Equals(mchnId, other.mchnId) &&
                    EqualityComparer<String>.Default.Equals(mchnTp, other.mchnTp) &&
                    EqualityComparer<String>.Default.Equals(mchnSts, other.mchnSts) &&
                    EqualityComparer<String>.Default.Equals(vrtlFlg, other.vrtlFlg) &&
                    EqualityComparer<String>.Default.Equals(aprchLn, other.aprchLn)
                    )
                    return true;

                return false;
            }
        }

        [Serializable]
        public class VD_Common_Yard_Location
        {
            public String blck;
            /*
            "Block Name 
            e.g) 1A, 2A, 1B, …."
            */
            public String bay;
            /*
            "Bay Name
            e.g) 1, 2, 3, 4, …."
            */
            public String row;
            /*
            "Row Name
            e.g) A, B, C, D, …."
            */
            public String tier;
            /*
            "Tier Name
            e.g) 1, 2, 3, 4, …."
            */

            public VD_Common_Yard_Location()
            {
                blck = String.Empty;
                bay = String.Empty;
                row = String.Empty;
                tier = String.Empty;
            }
        }

        [Serializable]
        public class VD_Common_ChassisOrder
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

            public String itvCd;

            public String ordStsCd;

            public String cntrNo;

            //public Boolean isArrival;


            public VD_Common_ChassisOrder()
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
                itvCd = String.Empty;
                ordStsCd = String.Empty;
                cntrNo = String.Empty;
                //isArrival = false;
            }
        }

        [Serializable]
        public class VD_Common_VmtPrcMachineList
        {
            public String mchnId;

            public String cntrNo;

            public String point;

            public String foreAfter;

            public VD_Common_Job_Location loc;

            public String wrkSts;

            public String twinTandemCd;

            public String twinTandemKey;

            public String chassNo;

            public Boolean isExistFlg;

            public String blockLoc;


            public VD_Common_VmtPrcMachineList()
            {
                mchnId = String.Empty;
                cntrNo = String.Empty;
                point = String.Empty;
                foreAfter = String.Empty;
                loc = new VD_Common_Job_Location();
                wrkSts = String.Empty;
                twinTandemCd = String.Empty;
                twinTandemKey = String.Empty;
                chassNo = String.Empty;
                isExistFlg = false;
                blockLoc = String.Empty;
            }
        }

        [Serializable]
        public class VD_Common_Job_Location : IEquatable<VD_Common_Job_Location>
        {
            public String locTp;
            /*
            enum enType- 다 대문자임
            {
                "Vessel",
                "Yard",
                "Rail",
                "TP",
                "IP",
                "Lane"
            };
            */
            public String blck;
            /*
            "Block Name 
            e.g) 1A, 2A, 1B, …."
            */
            public String bay;
            /*
            "Bay Name
            e.g) 1, 2, 3, 4, …."
            */
            public String row;
            /*
            "Row Name
            e.g) A, B, C, D, …."
            */
            public String tier;
            /*
            "Tier Name
            e.g) 1, 2, 3, 4, …."
            */
            public String lane;
            //W : Water-side //L :Land-side
            public String location;
            /*
            "Location                             if Lane Type
            e.g) 1A-1-A-1, 1A-1-B-1, ….           E.g) 1, 2"
            */

            public VD_Common_Job_Location()
            {
                locTp = String.Empty;
                blck = String.Empty;
                bay = String.Empty;
                row = String.Empty;
                tier = String.Empty;
                location = String.Empty;
                lane = String.Empty;
            }

            public override bool Equals(Object obj)
            {
                var other = obj as VD_Common_Job_Location;
                if (other == null) return false;

                return Equals(other);
            }

            public bool Equals(VD_Common_Job_Location other)
            {
                if (other == null)
                {
                    return false;
                }

                if (ReferenceEquals(this, other))
                {
                    return true;
                }

                // You can also use a specific StringComparer instead of EqualityComparer<string>
                // Check out the specific implementations (StringComparer.CurrentCulture, e.a.).
                if (EqualityComparer<String>.Default.Equals(locTp, other.locTp) &&
                    EqualityComparer<String>.Default.Equals(blck, other.blck) &&
                    EqualityComparer<String>.Default.Equals(bay, other.bay) &&
                    EqualityComparer<String>.Default.Equals(row, other.row) &&
                    EqualityComparer<String>.Default.Equals(tier, other.tier) &&
                    EqualityComparer<String>.Default.Equals(location, other.location) &&
                    EqualityComparer<String>.Default.Equals(lane, other.lane)
                    )
                    return true;

                return false;
            }

        }

        [Serializable]
        public class VD_Common_Job_Container : IEquatable<VD_Common_Job_Container>
        {
            public String cntrNo;
            public String cntrIso;
            public String cntrLen;
            public String cntrTp;
            public String cls;//수출 수입
            public String opr; //e.g) HJS
            public String cntrCgoTp;
            public String cntrSpTp;
            public String fullMty;
            public String cntrWgt;
            public String cntrGrade;
            public String pod;
            public String cntrHgt;
            public String imdgCd;

            public VD_Common_Job_Container()
            {
                cntrNo = String.Empty;
                cntrIso = String.Empty;
                cntrLen = String.Empty;
                cntrTp = String.Empty;
                cls = String.Empty;
                opr = String.Empty;
                cntrCgoTp = String.Empty;
                fullMty = String.Empty;
                cntrWgt = String.Empty;
                cntrGrade = String.Empty;
                pod = String.Empty;
                cntrSpTp = String.Empty;
                cntrHgt = String.Empty;
                imdgCd = String.Empty;
            }

            public override bool Equals(Object obj)
            {
                var other = obj as VD_Common_Job_Container;
                if (other == null) return false;

                return Equals(other);
            }

            public bool Equals(VD_Common_Job_Container other)
            {
                if (other == null)
                {
                    return false;
                }

                if (ReferenceEquals(this, other))
                {
                    return true;
                }

                // You can also use a specific StringComparer instead of EqualityComparer<string>
                // Check out the specific implementations (StringComparer.CurrentCulture, e.a.).
                if (EqualityComparer<String>.Default.Equals(cntrNo, other.cntrNo) &&
                    EqualityComparer<String>.Default.Equals(cntrIso, other.cntrIso) &&
                    EqualityComparer<String>.Default.Equals(cntrLen, other.cntrLen) &&
                    EqualityComparer<String>.Default.Equals(cntrTp, other.cntrTp) &&
                    EqualityComparer<String>.Default.Equals(cls, other.cls) &&
                    EqualityComparer<String>.Default.Equals(opr, other.opr) &&
                    EqualityComparer<String>.Default.Equals(cntrCgoTp, other.cntrCgoTp) &&
                    EqualityComparer<String>.Default.Equals(fullMty, other.fullMty) &&
                    EqualityComparer<String>.Default.Equals(cntrWgt, other.cntrWgt) &&
                    EqualityComparer<String>.Default.Equals(pod, other.pod) &&
                    EqualityComparer<String>.Default.Equals(cntrSpTp, other.cntrSpTp)
                    )
                    return true;

                return false;
            }
        }

        [Serializable]
        public class VD_Common_Job_Type_Base
        {
            public String jobTp;
            /*
            DS : Discharge
            LD : Load
            MI : Movement In Yard
            MO : Movement Out
            RH : Rehandleing
            AH : Auto Rehandleing
            LC : Loading Cancel
            GI : Gate In
            GO : Gate Out
            GC : Gate Out Cancel"
            */
            public String jobStatus;//2
            //Active = "A", Inactive="Q", Processing = "P", Completed = "C"
            public String vslCd;//10
            //<vslCd>HJLN</vslCd> // DS,LOAD 때만 기입
            public String voyNo;//16
            public String planSeq;//16
            public String twinTandemFlg;
            public String twinTandumKey;
            public String tandemJoinYT;
            public String jobFlagInfo;
            public String conePlan;
            public String ycTwinKey;

            public VD_Common_Job_Type_Base()
            {
                jobTp = String.Empty;
                jobStatus = String.Empty;
                vslCd = String.Empty;
                voyNo = String.Empty;
                planSeq = String.Empty;
                twinTandemFlg = String.Empty;
                twinTandumKey = String.Empty;
                tandemJoinYT = String.Empty;
                jobFlagInfo = String.Empty;
                conePlan = String.Empty;
                ycTwinKey = String.Empty;
            }
        }

        [Serializable]
        public class VD_Common_Job_Type : VD_Common_Job_Type_Base, IEquatable<VD_Common_Job_Type>
        {
            public String etw;
            public String eventId;
            public String queue;
            public String waitingTime;
            public String qcId;
            public Boolean isArrivedItv;

            public VD_Common_Job_Type()
            {
                etw = String.Empty;
                eventId = String.Empty;
                queue = String.Empty;
                waitingTime = String.Empty;
                qcId = String.Empty;
                isArrivedItv = false;
            }

            public override bool Equals(Object obj)
            {
                var other = obj as VD_Common_Job_Type;
                if (other == null) return false;

                return Equals(other);
            }

            public bool Equals(VD_Common_Job_Type other)
            {
                if (other == null)
                {
                    return false;
                }

                if (ReferenceEquals(this, other))
                {
                    return true;
                }

                // You can also use a specific StringComparer instead of EqualityComparer<string>
                // Check out the specific implementations (StringComparer.CurrentCulture, e.a.).
                if (EqualityComparer<String>.Default.Equals(jobTp, other.jobTp) &&
                    EqualityComparer<String>.Default.Equals(jobStatus, other.jobStatus) &&
                    EqualityComparer<String>.Default.Equals(vslCd, other.vslCd) &&
                    EqualityComparer<String>.Default.Equals(voyNo, other.voyNo) &&
                    EqualityComparer<String>.Default.Equals(planSeq, other.planSeq) &&
                    EqualityComparer<String>.Default.Equals(twinTandemFlg, other.twinTandemFlg) &&
                    EqualityComparer<String>.Default.Equals(twinTandumKey, other.twinTandumKey) &&
                    EqualityComparer<String>.Default.Equals(tandemJoinYT, other.tandemJoinYT) &&
                    EqualityComparer<String>.Default.Equals(jobFlagInfo, other.jobFlagInfo) &&
                    EqualityComparer<String>.Default.Equals(conePlan, other.conePlan) &&
                    EqualityComparer<String>.Default.Equals(ycTwinKey, other.ycTwinKey) &&
                    EqualityComparer<String>.Default.Equals(etw, other.etw) &&
                    EqualityComparer<String>.Default.Equals(eventId, other.eventId) &&
                    EqualityComparer<String>.Default.Equals(queue, other.queue)
                    )
                    return true;

                return false;
            }
        }

        [Serializable]
        public class VD_Common_JobOrder : IEquatable<VD_Common_JobOrder>
        {
            public String jobKey;
            public Boolean isCmtJob;
            public String prvCntrNo;
            public String prvJobTp;
            public String prvLocation;
            public String qcLn;
            public String hold;
            public String tpLocTp;
            public String tpBlck;
            public String tpBay;
            public String tpRow;
            public String tpTier;
            public String tpLocation;
            public String qlnLocTp;
            public String qlnBlck;
            public String qlnBay;
            public String qlnRow;
            public String qlnTier;
            public String qlnLocation;
            public String workingStatus;
            public String korJobAct;
            public String commCode;
            public String regBr;
            public String chassisNo;
            public String ytAprchLn;
            public String rfidNo;
            public String rfidChk;
            public String vslHoldDeck;
            public String doorDir;
            public String podCd;
            public String ytJbSts;
            public String cancelYn;
            public String ycNo;
            public String autoType;
            public String jobTpKor;
            public String jobTpKorShort;
            public String foreAfterKor;
            public String doorKor;
            public Boolean isWstp;
            public Boolean isLink;
            public Boolean isSwap;
            public Boolean isPick;
            public Boolean isSetDown;
            public Boolean isarrival;
            public Boolean isGcBtn;
            public Boolean isOcpyAndAlct;
            public Boolean is1stPart;
            public VD_Common_Job_Machine workingMchn;
            public VD_Common_Job_Machine partnerMchn;
            public VD_Common_Job_Location locTo;        //RH 작업 시 to 위치
            public VD_Common_Job_Location locFrom;	    //RH 작업 시 from-to 구조일때 from위치
            public VD_Common_Job_Location locWorking;   //일할장소

            // 1by1 고려하여 추가된사항 v3로 정의 추가
            public VD_Common_Job_Container cntr;
            public VD_Common_ChassisOrder chassisOrder;
            public List<VD_Common_ChassisOrder> prvItvList;
            public List<VD_Common_VmtPrcMachineList> prcMchnList;
            public List<String> qcLaneList;
            public VD_Common_Job_Type type;
            public String taskId;
            public String jobCount;
            public String priorityJob;      // hot job
            public String pinChkFlg;
            public String spndFlg;
            public String batNo;

            // T2 요구사항 추가
            public VD_Common_Def_Reefer reefer;

            public String vbsDate;

            public VD_Common_JobOrder()
            {
                jobKey = String.Empty;
                workingMchn = new VD_Common_Job_Machine();
                partnerMchn = new VD_Common_Job_Machine();
                locTo = new VD_Common_Job_Location();
                locFrom = new VD_Common_Job_Location();
                locWorking = new VD_Common_Job_Location();
                cntr = new VD_Common_Job_Container();
                type = new VD_Common_Job_Type();
                taskId = String.Empty;
                jobCount = String.Empty;
                priorityJob = String.Empty;
                isCmtJob = false;
                prvCntrNo = String.Empty;
                prvLocation = String.Empty;
                prvJobTp = String.Empty;
                qcLn = String.Empty;
                hold = String.Empty;
                tpLocTp = String.Empty;
                tpBlck = String.Empty;
                tpBay = String.Empty;
                tpRow = String.Empty;
                tpTier = String.Empty;
                tpLocation = String.Empty;
                qlnLocTp = String.Empty;
                qlnBlck = String.Empty;
                qlnBay = String.Empty;
                qlnRow = String.Empty;
                qlnTier = String.Empty;
                qlnLocation = String.Empty;
                workingStatus = String.Empty;
                korJobAct = String.Empty;
                commCode = String.Empty;
                regBr = String.Empty;
                ytAprchLn = String.Empty;
                rfidNo = String.Empty;
                rfidChk = String.Empty;
                vslHoldDeck = String.Empty;
                doorDir = String.Empty;
                podCd = String.Empty;
                ytJbSts = String.Empty;
                cancelYn = String.Empty;
                ycNo = String.Empty;
                autoType = String.Empty;
                jobTpKor = String.Empty;
                jobTpKorShort = String.Empty;
                foreAfterKor = String.Empty;
                doorKor = String.Empty;
                isWstp = false;
                isLink = false;
                isSwap = false;
                isPick = false;
                isSetDown = false;
                is1stPart = false;
                prvItvList = new List<VD_Common_ChassisOrder>();
                prcMchnList = new List<VD_Common_VmtPrcMachineList>();
                qcLaneList = new List<String>();
                isOcpyAndAlct = false;
                isarrival = false;
                isGcBtn = false;
                reefer = new VD_Common_Def_Reefer();
                chassisOrder = new VD_Common_ChassisOrder();
                vbsDate = String.Empty;
                pinChkFlg = String.Empty;
                spndFlg = String.Empty;
                batNo = String.Empty;
            }

            public void Clear()
            {
                this.workingMchn = null;
                this.partnerMchn = null;
                this.locTo = null;
                this.locFrom = null;
                this.locWorking = null;
                this.cntr = null;
                this.type = null;
                this.reefer = null;
                this.isCmtJob = false;
                this.chassisOrder = null;
                this.prvCntrNo = null;
                this.prvLocation = null;
                this.prvJobTp = null;
                this.qcLn = null;
                this.hold = null;
                this.tpLocTp = null;
                this.tpBlck = null;
                this.tpBay = null;
                this.tpRow = null;
                this.tpTier = null;
                this.tpLocation = null;
                this.qlnLocTp = null;
                this.qlnBlck = null;
                this.qlnBay = null;
                this.qlnRow = null;
                this.qlnTier = null;
                this.qlnLocation = null;
                this.workingStatus = null;
                this.korJobAct = null;
                this.commCode = null;
                this.regBr = null;
                this.vslHoldDeck = null;
                this.pinChkFlg = null;
                this.spndFlg = null;
                this.batNo = null;
                this.doorDir = null;
                this.podCd = null;
                this.ytJbSts = null;
                this.cancelYn = null;
                this.isLink = false;
                this.isWstp = false;
                this.isarrival = false;
                this.isSwap = false;
                this.isPick = false;
                this.isSetDown = false;
                this.prvItvList = null;
                this.qcLaneList = null;
                this.prcMchnList = null;
                this.isOcpyAndAlct = false;
                this.isGcBtn = false;
            }

            public override bool Equals(Object obj)
            {
                var other = obj as VD_Common_JobOrder;
                if (other == null) return false;

                return Equals(other);
            }

            public bool Equals(VD_Common_JobOrder other)
            {
                if (other == null)
                {
                    return false;
                }

                if (ReferenceEquals(this, other))
                {
                    return true;
                }

                // You can also use a specific StringComparer instead of EqualityComparer<string>
                // Check out the specific implementations (StringComparer.CurrentCulture, e.a.).
                if (EqualityComparer<String>.Default.Equals(jobKey, other.jobKey) &&
                    EqualityComparer<VD_Common_Job_Machine>.Default.Equals(workingMchn, other.workingMchn) &&
                    EqualityComparer<VD_Common_Job_Machine>.Default.Equals(partnerMchn, other.partnerMchn) &&
                    EqualityComparer<VD_Common_Job_Location>.Default.Equals(locTo, other.locTo) &&
                    EqualityComparer<VD_Common_Job_Location>.Default.Equals(locFrom, other.locFrom) &&
                    EqualityComparer<VD_Common_Job_Location>.Default.Equals(locWorking, other.locWorking) &&
                    EqualityComparer<VD_Common_Job_Container>.Default.Equals(cntr, other.cntr) &&
                    EqualityComparer<VD_Common_Job_Type>.Default.Equals(type, other.type) &&
                    EqualityComparer<String>.Default.Equals(taskId, other.taskId) &&
                    EqualityComparer<String>.Default.Equals(priorityJob, other.priorityJob) &&
                    EqualityComparer<String>.Default.Equals(prvCntrNo, other.prvCntrNo) &&
                    EqualityComparer<String>.Default.Equals(prvLocation, other.prvLocation) &&
                    EqualityComparer<String>.Default.Equals(prvJobTp, other.prvJobTp) &&
                    EqualityComparer<String>.Default.Equals(qcLn, other.qcLn) &&
                    EqualityComparer<String>.Default.Equals(hold, other.hold) &&
                    EqualityComparer<String>.Default.Equals(tpLocTp, other.tpLocTp) &&
                    EqualityComparer<String>.Default.Equals(tpBlck, other.tpBlck) &&
                    EqualityComparer<String>.Default.Equals(tpBay, other.tpBay) &&
                    EqualityComparer<String>.Default.Equals(tpRow, other.tpRow) &&
                    EqualityComparer<String>.Default.Equals(tpTier, other.tpTier) &&
                    EqualityComparer<String>.Default.Equals(tpLocation, other.tpLocation) &&
                    EqualityComparer<String>.Default.Equals(qlnLocTp, other.qlnLocTp) &&
                    EqualityComparer<String>.Default.Equals(qlnBlck, other.qlnBlck) &&
                    EqualityComparer<String>.Default.Equals(qlnBay, other.qlnBay) &&
                    EqualityComparer<String>.Default.Equals(qlnRow, other.qlnRow) &&
                    EqualityComparer<String>.Default.Equals(qlnTier, other.qlnTier) &&
                    EqualityComparer<String>.Default.Equals(qlnLocation, other.qlnLocation) &&
                    EqualityComparer<String>.Default.Equals(workingStatus, other.workingStatus) &&
                    EqualityComparer<String>.Default.Equals(korJobAct, other.korJobAct) &&
                    EqualityComparer<String>.Default.Equals(commCode, other.commCode) &&
                    EqualityComparer<String>.Default.Equals(regBr, other.regBr) &&
                    EqualityComparer<String>.Default.Equals(podCd, other.podCd) &&
                    EqualityComparer<VD_Common_Def_Reefer>.Default.Equals(reefer, other.reefer) &&
                    EqualityComparer<List<VD_Common_ChassisOrder>>.Default.Equals(prvItvList, other.prvItvList) &&
                    EqualityComparer<List<VD_Common_VmtPrcMachineList>>.Default.Equals(prcMchnList, other.prcMchnList) &&
                    EqualityComparer<List<String>>.Default.Equals(qcLaneList, other.qcLaneList) &&
                    EqualityComparer<VD_Common_ChassisOrder>.Default.Equals(chassisOrder, other.chassisOrder)
                    )
                    return true;

                // To compare the members array, you could perhaps use the 
                // [SequenceEquals][2] method.  But, be aware that [] {"a", "b"} will not
                // be considerd equal as [] {"b", "a"}
                return false;
            }

            public bool EqualsJob(VD_Common_JobOrder other)
            {
                if (other == null)
                {
                    return false;
                }

                if (ReferenceEquals(this, other))
                {
                    return true;
                }

                // You can also use a specific StringComparer instead of EqualityComparer<string>
                // Check out the specific implementations (StringComparer.CurrentCulture, e.a.).
                if (EqualityComparer<String>.Default.Equals(jobKey, other.jobKey) &&
                    EqualityComparer<VD_Common_Job_Machine>.Default.Equals(workingMchn, other.workingMchn) &&
                    EqualityComparer<VD_Common_Job_Machine>.Default.Equals(partnerMchn, other.partnerMchn) &&
                    EqualityComparer<VD_Common_Job_Location>.Default.Equals(locTo, other.locTo) &&
                    EqualityComparer<VD_Common_Job_Location>.Default.Equals(locFrom, other.locFrom) &&
                    EqualityComparer<VD_Common_Job_Location>.Default.Equals(locWorking, other.locWorking) &&
                    EqualityComparer<VD_Common_Job_Container>.Default.Equals(cntr, other.cntr) &&
                    EqualityComparer<VD_Common_Job_Type>.Default.Equals(type, other.type) &&
                    EqualityComparer<String>.Default.Equals(type.waitingTime, other.type.waitingTime) &&
                    EqualityComparer<String>.Default.Equals(taskId, other.taskId) &&
                    EqualityComparer<String>.Default.Equals(priorityJob, other.priorityJob) &&
                    EqualityComparer<String>.Default.Equals(prvCntrNo, other.prvCntrNo) &&
                    EqualityComparer<String>.Default.Equals(prvLocation, other.prvLocation) &&
                    EqualityComparer<String>.Default.Equals(prvJobTp, other.prvJobTp) &&
                    EqualityComparer<String>.Default.Equals(qcLn, other.qcLn) &&
                    EqualityComparer<String>.Default.Equals(hold, other.hold) &&
                    EqualityComparer<String>.Default.Equals(tpLocTp, other.tpLocTp) &&
                    EqualityComparer<String>.Default.Equals(tpBlck, other.tpBlck) &&
                    EqualityComparer<String>.Default.Equals(tpBay, other.tpBay) &&
                    EqualityComparer<String>.Default.Equals(tpRow, other.tpRow) &&
                    EqualityComparer<String>.Default.Equals(tpTier, other.tpTier) &&
                    EqualityComparer<String>.Default.Equals(tpLocation, other.tpLocation) &&
                    EqualityComparer<String>.Default.Equals(qlnLocTp, other.qlnLocTp) &&
                    EqualityComparer<String>.Default.Equals(qlnBlck, other.qlnBlck) &&
                    EqualityComparer<String>.Default.Equals(qlnBay, other.qlnBay) &&
                    EqualityComparer<String>.Default.Equals(qlnRow, other.qlnRow) &&
                    EqualityComparer<String>.Default.Equals(qlnTier, other.qlnTier) &&
                    EqualityComparer<String>.Default.Equals(qlnLocation, other.qlnLocation) &&
                    EqualityComparer<String>.Default.Equals(workingStatus, other.workingStatus) &&
                    EqualityComparer<String>.Default.Equals(korJobAct, other.korJobAct) &&
                    EqualityComparer<String>.Default.Equals(commCode, other.commCode) &&
                    EqualityComparer<String>.Default.Equals(regBr, other.regBr) &&
                    EqualityComparer<String>.Default.Equals(podCd, other.podCd) &&
                    EqualityComparer<VD_Common_Def_Reefer>.Default.Equals(reefer, other.reefer)
                    )
                    return true;

                // To compare the members array, you could perhaps use the 
                // [SequenceEquals][2] method.  But, be aware that [] {"a", "b"} will not
                // be considerd equal as [] {"b", "a"}
                return false;
            }

            public static int CompareBackUp(VD_Common_JobOrder x, VD_Common_JobOrder y)
            {
                try
                {
                    String xSts = x.type.jobStatus;
                    String ySts = y.type.jobStatus;
                    if (xSts.Equals(ySts))     // xVbsDate = Priority = ycNo = ytNo > ETW > Container No 
                    {
                        String xVbsDate = x.vbsDate, yVbsDate = y.vbsDate;
                        if (!String.IsNullOrEmpty(xVbsDate) && String.IsNullOrEmpty(yVbsDate))
                            return -1;
                        else if (String.IsNullOrEmpty(xVbsDate) && !String.IsNullOrEmpty(yVbsDate))
                            return 1;
                        else if (!String.IsNullOrEmpty(xVbsDate) && !String.IsNullOrEmpty(yVbsDate))
                            return xVbsDate.CompareTo(yVbsDate);

                        String xPriority = x.priorityJob, yPriority = y.priorityJob;
                        if (!String.IsNullOrEmpty(xPriority) && String.IsNullOrEmpty(yPriority))
                            return -1;
                        else if (String.IsNullOrEmpty(xPriority) && !String.IsNullOrEmpty(yPriority))
                            return 1;
                        else if (!String.IsNullOrEmpty(xPriority) && !String.IsNullOrEmpty(yPriority))
                            return xPriority.CompareTo(yPriority);

                        String xYcNo = x.ycNo, yYcNo = y.ycNo;
                        if (!String.IsNullOrEmpty(xYcNo) && String.IsNullOrEmpty(yYcNo))
                            return -1;
                        else if (String.IsNullOrEmpty(xYcNo) && !String.IsNullOrEmpty(yYcNo))
                            return 1;
                        else if (!String.IsNullOrEmpty(xYcNo) && !String.IsNullOrEmpty(yYcNo))
                            return xYcNo.CompareTo(yYcNo);

                        String xYtNo = x.partnerMchn.mchnId, yYtNo = y.partnerMchn.mchnId;
                        Boolean xArrivedItv = x.type.isArrivedItv, yArrivedItv = y.type.isArrivedItv;
                        if (!String.IsNullOrEmpty(xYtNo) && String.IsNullOrEmpty(yYtNo))
                            return -1;
                        else if (String.IsNullOrEmpty(xYtNo) && !String.IsNullOrEmpty(yYtNo))
                            return 1;
                        else if (!String.IsNullOrEmpty(xYtNo) && !String.IsNullOrEmpty(yYtNo))
                        {
                            if (xArrivedItv && !yArrivedItv)
                                return -1;
                            else if (!xArrivedItv && yArrivedItv)
                                return 1;
                            else if (xArrivedItv && yArrivedItv)
                                return xYtNo.CompareTo(yYtNo);
                        }


                        //if ((xArrivedItv && yArrivedItv && xArrivedItv && yArrivedItv) || (String.IsNullOrEmpty(xPriority) && String.IsNullOrEmpty(yPriority)))
                        //{
                        //if (!"AH".Equals(x.type.jobTp) && !"AH".Equals(y.type.jobTp))
                        //{
                        //    String xWait = x.type.waitingTime;
                        //    String yWait = y.type.waitingTime;
                        //    if (String.IsNullOrEmpty(xWait) && String.IsNullOrEmpty(yWait))
                        //        return x.cntr.cntrNo.CompareTo(y.cntr.cntrNo);
                        //    else if (String.IsNullOrEmpty(xWait))
                        //        return 1;
                        //    else if (String.IsNullOrEmpty(yWait))
                        //        return -1;
                        //    else // if (!String.IsNullOrEmpty(xEtw) && !String.IsNullOrEmpty(yEtw))
                        //        return xWait.CompareTo(yWait);
                        //}
                        //else
                        //{
                        String xEtw = x.type.etw;
                        String yEtw = y.type.etw;
                        if (String.IsNullOrEmpty(xEtw) && String.IsNullOrEmpty(yEtw))
                            return x.cntr.cntrNo.CompareTo(y.cntr.cntrNo);
                        else if (String.IsNullOrEmpty(xEtw))
                            return 1;
                        else if (String.IsNullOrEmpty(yEtw))
                            return -1;
                        else // if (!String.IsNullOrEmpty(xEtw) && !String.IsNullOrEmpty(yEtw))
                            return xEtw.CompareTo(yEtw);
                        //}
                        //}
                    }
                    //-----------------------------------------------------------------
                    //- Compare Order ( P -> A -> Q -> B )
                    //-----------------------------------------------------------------

                    else
                    {
                        int xStsNo = xSts.Equals("P") ? 1 : xSts.Equals("A") ? 2 : xSts.Equals("Q") ? 3 : xSts.Equals("B") ? 4 : 5;
                        int yStsNo = ySts.Equals("P") ? 1 : ySts.Equals("A") ? 2 : ySts.Equals("Q") ? 3 : ySts.Equals("B") ? 4 : 5;

                        if (xStsNo < yStsNo) return -1;
                        else if (xStsNo > yStsNo) return 1;
                        else if (xStsNo == yStsNo && xStsNo < 5) return -1;
                        else return 0;

                        /*else if (xSts.Equals("P"))
                            return -1;
                        else if (ySts.Equals("P"))
                            return 1;
                        else
                        {
                            if (xSts.Equals("A"))
                                return -1;
                            else if (ySts.Equals("A"))
                                return 1;
                            else
                            {
                                if (xSts.Equals("Q"))
                                    return -1;
                                else if (ySts.Equals("Q"))
                                    return 1;
                                else
                                {
                                    if (xSts.Equals("B"))
                                        return -1;
                                    else if (ySts.Equals("B"))
                                        return 1;
                                    return 0;
                                }
                            }*/
                    }
                }
                catch
                {
                }
                return 0;
            }

            public static int Compare(VD_Common_JobOrder x, VD_Common_JobOrder y)
            {
                try
                {
                    //1.Processing Job
                    //2.VBS Date
                    //3.JobType
                    // - LD 1st->sort by taskId
                    // - DS/LC 2nd->sort by ETW
                    // - GI / GO / GC 3rd->sort by ETW
                    // - MI / MO 4th->sort by ETW
                    // - RH / AH top->sort by ETW among RH/ AH job

                    if (x.jobKey.Equals(y.jobKey))
                        return 0;
                    else
                    {
                        // Aug 12 2021 Move to top to declare 1 time
                        String xJobTp = x.type.jobTp.ToUpper();
                        String yJobTp = y.type.jobTp.ToUpper();
                        int xJobTpNo = RMG.RMG_User.gJobTypeSortOrder[xJobTp];
                        int yJobTpNo = RMG.RMG_User.gJobTypeSortOrder[yJobTp];

                        String xEtw = x.type.etw;
                        String yEtw = y.type.etw;

                        String xSts = x.type.jobStatus;
                        String ySts = y.type.jobStatus;
                        if (xSts.Equals("P"))
                        {
                            return -1;
                        }
                        else if (ySts.Equals("P"))
                        {
                            return 1;
                        }
                        else
                        {
                            // HOT JOB Aug 6 2021
                            String xPriorityJob = x.priorityJob;
                            String yPriorityJob = y.priorityJob;
                            if (!String.IsNullOrEmpty(xPriorityJob) && String.IsNullOrEmpty(yPriorityJob))
                                return -1;
                            else if (String.IsNullOrEmpty(xPriorityJob) && !String.IsNullOrEmpty(yPriorityJob))
                                return 1;
                            else if (!String.IsNullOrEmpty(xPriorityJob) && !String.IsNullOrEmpty(yPriorityJob) && !xPriorityJob.Equals(yPriorityJob))
                            {
                                // Aug 12 2021 Compare jobTp, etw when 2 priorityJob                              
                                if (xJobTpNo < yJobTpNo)
                                    return -1;
                                else if (xJobTpNo > yJobTpNo)
                                    return 1;
                                else
                                {
                                    if (!String.IsNullOrEmpty(xEtw) && String.IsNullOrEmpty(yEtw))
                                        return -1;
                                    else if (String.IsNullOrEmpty(xEtw) && !String.IsNullOrEmpty(yEtw))
                                        return 1;
                                    else if (!xEtw.Equals(yEtw))
                                        return xEtw.CompareTo(yEtw);
                                    else
                                        return -1;
                                }
                            }
                            else
                            {
                                ////VBSDATE
                                //String xVbsDate = x.vbsDate, yVbsDate = y.vbsDate;
                                //if (!String.IsNullOrEmpty(xVbsDate) && String.IsNullOrEmpty(yVbsDate))
                                //    return -1;
                                //else if (String.IsNullOrEmpty(xVbsDate) && !String.IsNullOrEmpty(yVbsDate))
                                //    return 1;
                                //else if (!xVbsDate.Equals(yVbsDate))
                                //    return xVbsDate.CompareTo(yVbsDate);
                                //else
                                {
                                    //LD MO ACTIVE JOB
                                    if (xSts.Equals("A") && !ySts.Equals("A"))
                                    {
                                        return -1;
                                    }
                                    else if (!xSts.Equals("A") && ySts.Equals("A"))
                                    {
                                        return 1;
                                    }
                                    else
                                    {
                                        //int xJobTpNo = xJobTp.Equals("AH") ? 0 :
                                        //         (xJobTp.Equals("LD") && !String.IsNullOrEmpty(x.partnerMchn.mchnId)) ? 1 :
                                        //          xJobTp.Equals("DS") ? 2 :
                                        //          xJobTp.Equals("GI") ? 3 :
                                        //          xJobTp.Equals("GO") ? 4 :
                                        //          xJobTp.Equals("GC") ? 5 :
                                        //          (xJobTp.Equals("MO") && !String.IsNullOrEmpty(x.partnerMchn.mchnId)) ? 6 :
                                        //          xJobTp.Equals("MI") ? 7 :
                                        //          xJobTp.Equals("RH") ? 8 :
                                        //          (xJobTp.Equals("LD") && String.IsNullOrEmpty(x.partnerMchn.mchnId)) ? 9 :
                                        //          (xJobTp.Equals("MO") && String.IsNullOrEmpty(x.partnerMchn.mchnId)) ? 10 : 11;

                                        //int yJobTpNo = yJobTp.Equals("AH") ? 0 :
                                        //               (yJobTp.Equals("LD") && !String.IsNullOrEmpty(y.partnerMchn.mchnId)) ? 1 :
                                        //                yJobTp.Equals("DS") ? 2 :
                                        //                yJobTp.Equals("GI") ? 3 :
                                        //                yJobTp.Equals("GO") ? 4 :
                                        //                yJobTp.Equals("GC") ? 5 :
                                        //                (yJobTp.Equals("MO") && !String.IsNullOrEmpty(y.partnerMchn.mchnId)) ? 6 :
                                        //                yJobTp.Equals("MI") ? 7 :
                                        //                yJobTp.Equals("RH") ? 8 :
                                        //                (yJobTp.Equals("LD") && String.IsNullOrEmpty(y.partnerMchn.mchnId)) ? 9 :
                                        //                (yJobTp.Equals("MO") && String.IsNullOrEmpty(y.partnerMchn.mchnId)) ? 10 : 11;

                                        // Compare JobTp
                                        if (xJobTpNo < yJobTpNo)
                                            return -1;
                                        else if (xJobTpNo > yJobTpNo)
                                            return 1;
                                        else if (xJobTpNo == yJobTpNo /*&& xJobTpNo != 1 && xJobTpNo != 9*/) //use ETW
                                        {
                                            if (String.IsNullOrEmpty(xEtw) && String.IsNullOrEmpty(yEtw))
                                                return -1;
                                            else if (String.IsNullOrEmpty(xEtw))
                                                return 1;
                                            else if (String.IsNullOrEmpty(yEtw))
                                                return -1;
                                            else if (!xEtw.Equals(yEtw))
                                                return xEtw.CompareTo(yEtw);
                                            else
                                            {
                                                String xJbKey = x.jobKey;
                                                String yJbKey = y.jobKey;
                                                if (String.IsNullOrEmpty(xJbKey) && String.IsNullOrEmpty(yJbKey))
                                                    return -1;
                                                else if (String.IsNullOrEmpty(xJbKey))
                                                    return 1;
                                                else if (String.IsNullOrEmpty(yJbKey))
                                                    return -1;
                                                else if (!xJbKey.Equals(yJbKey))
                                                    return xJbKey.CompareTo(yJbKey);
                                                else
                                                    return 0;
                                            }
                                        }
                                        // xJobTpNo & yJobTpNo 1, 9 in Config file is not LD, so don't need this case
                                        //else if (xJobTpNo == xJobTpNo && (xJobTpNo == 1 || xJobTpNo == 9)) //LD case use taskID to compare
                                        //{
                                        //    String xTaskId = x.taskId;
                                        //    String yTaskId = y.taskId;
                                        //    if (String.IsNullOrEmpty(xTaskId) && String.IsNullOrEmpty(yTaskId))
                                        //        return -1;
                                        //    else if (String.IsNullOrEmpty(xTaskId))
                                        //        return 1;
                                        //    else if (String.IsNullOrEmpty(yTaskId))
                                        //        return -1;
                                        //    else if (!xTaskId.Equals(yTaskId))
                                        //    {
                                        //        int xTaskIdInt = Convert.ToInt32(xTaskId);
                                        //        int yTaskIdInt = Convert.ToInt32(yTaskId);
                                        //        return xTaskIdInt.CompareTo(yTaskIdInt);
                                        //    }
                                        //    else
                                        //    {
                                        //        String xEtw = x.type.etw;
                                        //        String yEtw = y.type.etw;
                                        //        if (String.IsNullOrEmpty(xEtw) && String.IsNullOrEmpty(yEtw))
                                        //            return -1;
                                        //        else if (String.IsNullOrEmpty(xEtw))
                                        //            return 1;
                                        //        else if (String.IsNullOrEmpty(yEtw))
                                        //            return -1;
                                        //        else if (!xEtw.Equals(yEtw))
                                        //            return xEtw.CompareTo(yEtw);
                                        //        else
                                        //        {
                                        //            String xJbKey = x.jobKey;
                                        //            String yJbKey = y.jobKey;
                                        //            if (String.IsNullOrEmpty(xJbKey) && String.IsNullOrEmpty(yJbKey))
                                        //                return -1;
                                        //            else if (String.IsNullOrEmpty(xJbKey))
                                        //                return 1;
                                        //            else if (String.IsNullOrEmpty(yJbKey))
                                        //                return -1;
                                        //            else if (!xJbKey.Equals(yJbKey))
                                        //                return xJbKey.CompareTo(yJbKey);
                                        //            else
                                        //                return 0;
                                        //        }

                                        //    }
                                        //}
                                    }
                                }
                            }
                        }
                    }
                }
                catch
                {
                }
                return 0;
            }
        }
        //  ITV의 경우        
        //  int                        iCount; // 갯수
        //  VD_Common_JobOrder         JobOrder[N];
        //  sTrayUI_ITVJobOrderSub     Sub[N];

        //  RTG의 경우        
        //  int                        iCount; // 갯수
        //  VD_Common_JobOrder         JobOrder[N];

        [Serializable]
        public class VmtSwap : IEquatable<VmtSwap>
        {
            public String regoNo;
            public String cntrNo;
            public String cntrPnt;
            public String cntrTp;
            public String cntrIso;
            public String cntrCmdt;
            public String opr;
            public String swapPos;
            public String psnIdx1;
            public String psnIdx2;
            public String psnIdx3;
            public String psnIdx4;

            public VmtSwap()
            {
                regoNo = String.Empty;
                cntrNo = String.Empty;
                cntrPnt = String.Empty;
                cntrTp = String.Empty;
                cntrIso = String.Empty;
                cntrCmdt = String.Empty;
                opr = String.Empty;
                swapPos = String.Empty;
                psnIdx1 = String.Empty;
                psnIdx2 = String.Empty;
                psnIdx3 = String.Empty;
                psnIdx4 = String.Empty;
            }
            public void Clear()
            {
                this.regoNo = null;
                this.cntrNo = null;
                this.cntrPnt = null;
                this.cntrTp = null;
                this.cntrIso = null;
                this.cntrCmdt = null;
                this.opr = null;
                this.swapPos = null;
                this.psnIdx1 = null;
                this.psnIdx2 = null;
                this.psnIdx3 = null;
                this.psnIdx4 = null;
            }

            public override bool Equals(Object obj)
            {
                var other = obj as VmtSwap;
                if (other == null) return false;

                return Equals(other);
            }

            public bool Equals(VmtSwap other)
            {
                if (other == null)
                {
                    return false;
                }

                if (ReferenceEquals(this, other))
                {
                    return true;
                }

                // You can also use a specific StringComparer instead of EqualityComparer<string>
                // Check out the specific implementations (StringComparer.CurrentCulture, e.a.).
                if (EqualityComparer<String>.Default.Equals(regoNo, other.regoNo) &&
                    EqualityComparer<String>.Default.Equals(cntrNo, other.cntrNo) &&
                    EqualityComparer<String>.Default.Equals(cntrPnt, other.cntrPnt) &&
                    EqualityComparer<String>.Default.Equals(cntrTp, other.cntrTp) &&
                    EqualityComparer<String>.Default.Equals(cntrIso, other.cntrIso) &&
                    EqualityComparer<String>.Default.Equals(cntrCmdt, other.cntrCmdt) &&
                    EqualityComparer<String>.Default.Equals(opr, other.opr) &&
                    EqualityComparer<String>.Default.Equals(swapPos, other.swapPos) &&
                    EqualityComparer<String>.Default.Equals(psnIdx1, other.psnIdx1) &&
                    EqualityComparer<String>.Default.Equals(psnIdx2, other.psnIdx2) &&
                    EqualityComparer<String>.Default.Equals(psnIdx3, other.psnIdx3) &&
                    EqualityComparer<String>.Default.Equals(psnIdx4, other.psnIdx4)
                    )
                    return true;

                // To compare the members array, you could perhaps use the 
                // [SequenceEquals][2] method.  But, be aware that [] {"a", "b"} will not
                // be considerd equal as [] {"b", "a"}
                return false;
            }
        }

        [Serializable]
        public class VmtMachine
        {
            public String mchnId;
            public String mchnTp;
            public Boolean isLogOn;
            public Boolean isOn;
            public String mchnSts;
            public String rfidBlck;
            public String armgReadFlg;
            public String vrtlFlg;
            public String noticeMsg;
            public List<String> loginUsrLst;
            public List<String> hatchQcList;
            public String blck;
            public String bay;
            public String row;
            public String tier;
            public String locChgFlg;
            public String autoFlg;
            public String wgtSysStsCd;
            public String mchnMd;

            public VmtMachine()
            {
                mchnId = String.Empty;
                mchnTp = String.Empty;
                isLogOn = false;
                isOn = false;
                mchnSts = String.Empty;
                rfidBlck = String.Empty;
                armgReadFlg = String.Empty;
                vrtlFlg = String.Empty;
                noticeMsg = String.Empty;
                blck = String.Empty;
                bay = String.Empty;
                row = String.Empty;
                tier = String.Empty;
                locChgFlg = String.Empty;
                autoFlg = String.Empty;
                wgtSysStsCd = String.Empty;
                loginUsrLst = new List<string>();
                hatchQcList = new List<string>();
                mchnMd = "N"; //20200728 default N
            }
            public void Clear()
            {
                this.mchnId = null;
                this.mchnTp = null;
                this.isLogOn = false;
                this.isOn = false;
                this.mchnSts = null;
                this.rfidBlck = null;
                this.armgReadFlg = null;
                this.vrtlFlg = null;
                this.noticeMsg = null;
                this.loginUsrLst = null;
                this.hatchQcList = null;
                this.blck = null;
                this.bay = null;
                this.row = null;
                this.tier = null;
                this.locChgFlg = null;
                this.autoFlg = null;
                this.wgtSysStsCd = null;
                this.mchnMd = null;
            }

            public override bool Equals(Object obj)
            {
                var other = obj as VmtSwap;
                if (other == null) return false;

                return Equals(other);
            }
        }

        [Serializable]
        public class VD_Common_JobKey
        {
            public String jobKey;

            public VD_Common_JobKey()
            {
                jobKey = String.Empty;
            }
        }

        [Serializable]
        public class VD_Common_JobDone
        {
            public String jobKey;

            public VD_Common_JobDone()
            {
                jobKey = String.Empty;
            }
        }

        [Serializable]
        public class VD_Common_ManualJobDone_Send
        {
            public String jobKey;

            public VD_Common_ManualJobDone_Send()
            {
                jobKey = String.Empty;
            }
        }
        //------------------------------------------------------------------------------

        //------------------------------------------------------------------------------
        //- Container
        [Serializable]
        public class VD_Common_Def_Reefer : IEquatable<VD_Common_Def_Reefer>
        {
            public float reeferTemp;		//온도
            public float WorkTemp;
            public String plugCd;
            /*
            "status of Plug
            PIW : Wait for Plug-In
            PIM : Monitoring of Plug-In
            POW : Wait for Plug-Out
            POC : Completed of Plug-Out"
            */
            public String unit;
            /*
            "unit of temperature
            F : Fahrenheit
            C : Celsius "
            */

            public VD_Common_Def_Reefer()
            {
                reeferTemp = 0;
                plugCd = String.Empty;
                unit = String.Empty;
            }

            public bool Equals(VD_Common_Def_Reefer other)
            {
                if (other == null)
                {
                    return false;
                }

                if (ReferenceEquals(this, other))
                {
                    return true;
                }

                // You can also use a specific StringComparer instead of EqualityComparer<string>
                // Check out the specific implementations (StringComparer.CurrentCulture, e.a.).
                if (EqualityComparer<float>.Default.Equals(reeferTemp, other.reeferTemp) &&
                    EqualityComparer<float>.Default.Equals(WorkTemp, other.WorkTemp) &&
                    EqualityComparer<String>.Default.Equals(unit, other.unit) &&
                    EqualityComparer<String>.Default.Equals(plugCd, other.plugCd)
                    )
                    return true;

                // To compare the members array, you could perhaps use the 
                // [SequenceEquals][2] method.  But, be aware that [] {"a", "b"} will not
                // be considerd equal as [] {"b", "a"}
                return false;
            }
        }

        [Serializable]
        public class VD_Common_Def_Damage
        {
            public String dmgCd;
            /*
            "Damage Code
            e.g) AS :  ALL SNOW
                 PU : PUSHED
                 BM : Band Missing"
            */
            public String dmgInOut; //Inside / Outside
            /*
            "Inside / Outside
            e.g) I : Inside, O : Outside"
            */
            public String dmgPart;//Damage part
            /*
            "Damage part
            e.g) F : Fore, R : Rear, L : Left, R : Right, T : Top, B : BottomR)"
            */
            public String dmgRange;//Damage Range
            /*
            "Damage Range
            e.g) 10*10*10"
            */
            public String dmgDesc;//ISO Code

            public VD_Common_Def_Damage()
            {
                dmgCd = String.Empty;
                dmgInOut = String.Empty;
                dmgPart = String.Empty;
                dmgRange = String.Empty;
                dmgDesc = String.Empty;
            }
        }

        [Serializable]
        public class VD_Common_Def_Seal
        {
            public String sealNo;//IMDG
            /*
            "Seal No
            e.g.) 12311231231"

            */
            public String sealTp;
            /*
            "C : Custom
            O : Operator"
            */

            public VD_Common_Def_Seal()
            {
                sealNo = String.Empty;
                sealTp = String.Empty;
            }
        }

        [Serializable]
        public class VD_Common_Def_Imdg
        {
            public String imdg;//IMDG
            public String unNo;//UNNO
            public String fireCd;//Fire Code


            public VD_Common_Def_Imdg()
            {
                imdg = String.Empty;
                unNo = String.Empty;
                fireCd = String.Empty;
            }
        }

        [Serializable]
        public class VD_Common_Def_Container
        {
            public String cntrNo;
            public String cntrIso;
            public String cntrTp;
            /*
            "Container Type
            GE : General
            RF :  Reefer
            TK : Tank
            FR : Flat Rack
            OT : Open Top
            BK : Dry Bulk
            AS : Air Surface"
            */
            public String cntrLen;
            public String cntrHgt;
            public String opr; //e.g) HJS
            public String cntrWgt;
            public String cls;//수출 수입

            public String groupCode;
            public String rfTemp;
            public String rfPlug;
            public String imdgCd;
            public String bkgNo;
            public String stkDay;
            public String inVsl;
            public String outVsl;
            public String qcNo;
            public String qcEtw;
            /*
            "Class : Import/Export
            II: Import
            TI: Trashipment Import
            XX: Export
            TX : Transhipment Export
            TL: Transshipment
            S1: Shift 1Time
            S2: Shift 2Time
            YY: Storage Empty
            OT : Through Cargo"
            */
            public String cntrSpTp;
            public String cntrCgoTp;
            /*
            "Cargo Type
             G : General
             M: Empty
             MH : Empty Hazardous
             R : Reefer
             RH : Reefer Hazardous
             H : Hazardous"
            */
            public String fullMty;
            /*
            "F: Full
            M: Empty"
            */
            public String pod;
            public String nPod;
            public String pol;
            public String fPod;
            public String cntrGrade;
            public String doorDirect;
            /*
            Fore:F
            After:A
            */
            public Boolean isDmg;
            public Boolean isBundle;
            public Boolean isBundleMaster;
            public Boolean isHighCubic;
            public String rmk;
            public Boolean isSeal;
            public Boolean isHold;
            public Boolean chkHold;
            public Boolean chkDamage;

            public String overValue;    // OOG(Out of Gauge) : Over Slot Gauge(F/B/L/R/T) = 11/12/13/14/15

            public VD_Common_Def_Reefer reefer;
            public List<VD_Common_Def_Damage> dmgList;
            public List<VD_Common_Def_Seal> seaList;
            public List<VD_Common_Def_Imdg> imdgList;

            public VD_Common_Def_Container()
            {
                cntrNo = String.Empty;
                cntrIso = String.Empty;
                cntrTp = String.Empty;
                cntrLen = String.Empty;
                cntrHgt = String.Empty;
                opr = String.Empty;
                cntrWgt = String.Empty;
                cls = String.Empty;

                groupCode = String.Empty;
                rfTemp = String.Empty;
                rfPlug = String.Empty;
                imdgCd = String.Empty;
                bkgNo = String.Empty;
                stkDay = String.Empty;
                inVsl = String.Empty;
                outVsl = String.Empty;
                qcNo = String.Empty;
                qcEtw = String.Empty;

                cntrSpTp = String.Empty;
                cntrCgoTp = String.Empty;
                fullMty = String.Empty;
                pod = String.Empty;
                nPod = String.Empty;
                pol = String.Empty;
                fPod = String.Empty;
                cntrGrade = String.Empty;
                doorDirect = String.Empty;
                isDmg = false;
                isSeal = false;
                isHold = false;
                chkHold = false;
                overValue = String.Empty;

                reefer = new VD_Common_Def_Reefer();
                dmgList = new List<VD_Common_Def_Damage>();
                seaList = new List<VD_Common_Def_Seal>();
                imdgList = new List<VD_Common_Def_Imdg>();
            }

            public void Dispose()
            {
                reefer = null;
                for (int i = 0; i < dmgList.Count; i++)
                {
                    var item = dmgList[i] as VD_Common_Def_Damage;
                    item = null;
                }
                dmgList.Clear();
                dmgList = null;

                for (int i = 0; i < seaList.Count; i++)
                {
                    var item = seaList[i] as VD_Common_Def_Seal;
                    item = null;
                }
                seaList.Clear();
                seaList = null;

                for (int i = 0; i < imdgList.Count; i++)
                {
                    var item = imdgList[i] as VD_Common_Def_Imdg;
                    item = null;
                }
                imdgList.Clear();
                imdgList = null;
            }
        }
        //------------------------------------------------------------------------------

        //------------------------------------------------------------------------------
        //- Location
        [Serializable]
        public class VD_Common_Def_Location
        {
            public String locTp;
            /*
            enum enType- 다 대문자임
            {
                "Vessel",
                "Yard",
                "Rail",
                "TP",
                "IP",
                "Lane"
            };
            */
            public String blck;
            /*
            "Block Name 
            e.g) 1A, 2A, 1B, …."
            */
            public String bay;
            /*
            "Bay Name
            e.g) 1, 2, 3, 4, …."
            */
            public String row;
            /*
            "Row Name
            e.g) A, B, C, D, …."
            */
            public String tier;
            /*
            "Tier Name
            e.g) 1, 2, 3, 4, …."
            */
            public String lane;
            //W : Water-side //L :Land-side
            public String location;
            /*
            "Location                             if Lane Type
            e.g) 1A-1-A-1, 1A-1-B-1, ….           E.g) 1, 2"
            */

            public VD_Common_Def_Location()
            {
                locTp = String.Empty;
                blck = String.Empty;
                bay = String.Empty;
                row = String.Empty;
                tier = String.Empty;
                lane = String.Empty;
                location = String.Empty;
            }
        }

        [Serializable]
        public class VD_Common_Def_Inventory : IDisposable
        {
            public Common.VD_Common_Def_Container cntr;
            public Common.VD_Common_Def_Location loc;
            //1.4.1 구조
            public String inOut; //In/Out
            public String reason; //reason
            public Boolean isTOSData; //TOS가 관리한
            public Boolean mgrEE; //EagleEye가 관리한  
            public String InvenTp; // INV,NWT,NWA,TNL,VRT
            public Boolean IsCorrectionSelect;
            public String bColor;
            public String fColor;
            public String stkDay;
            public String vehicle;
            public String batNo;
            public Boolean qrntn;
            public Boolean qrntnRf;
            public String jobTp;
            public String pickSetChk;
            public String otrWaitTime;
            public String groupCode;
            public String etb;
            public String jobSts;
            public Boolean isTopOog;
            public Boolean isLeftOog;
            public Boolean isRightOog;
            public String jobTpKorShort;


            public VD_Common_Def_Inventory()
            {
                cntr = new VD_Common_Def_Container();
                loc = new VD_Common_Def_Location();
                inOut = String.Empty;
                reason = String.Empty;
                isTOSData = false;
                mgrEE = false;
                InvenTp = String.Empty;
                IsCorrectionSelect = false;
                bColor = "";
                fColor = "";
                stkDay = "";
                vehicle = "";
                batNo = String.Empty;
                qrntn = false;
                qrntnRf = false;
                jobTp = "";
                pickSetChk = "";
                otrWaitTime = "";
                groupCode = "";
                etb = "";
                jobSts = "";
                isTopOog = false;
                isLeftOog = false;
                isRightOog = false;
                jobTpKorShort = "";
            }

            public void Dispose()
            {
                if (cntr != null)
                    cntr.Dispose();
                cntr = null;
                loc = null;
            }
        }

        [Serializable]
        public class VD_Common_Def_InventorySimple
        {
            public String cntrNo;
            public String cntrIso;
            public String cntrTp;
            public VD_Common_Def_Location loc;   //해당컨테이너의 Location 정보
            public Boolean isDmg;
            public Boolean isHold;
            public String plugCd;
            public String fullMty;

            public VD_Common_Def_InventorySimple()
            {
                cntrNo = String.Empty;
                cntrIso = String.Empty;
                cntrTp = String.Empty;
                loc = new VD_Common_Def_Location();
                isDmg = false;
                isHold = false;
                plugCd = String.Empty;
                fullMty = String.Empty;
            }
        }

        [Serializable]
        public class VD_Commmon_Spreader
        {
            public String sprdMd;
            //    "SINGLE"
            //    "TWIN"
            //    "TANDEM"
            public String sprdTp;
            //    "SINGLE_SPREADER20"
            //    "SINGLE_SPREADER40"
            //    "SINGLE_SPREADER45"
            public String sprdSts;
            //   "LS_SPREADER_LOCKED"
            //   "LS_SPREADER_UNLOCKED"

            public VD_Commmon_Spreader()
            {
                sprdMd = String.Empty;
                sprdTp = String.Empty;
                sprdSts = String.Empty;
            }
        }
        //------------------------------------------------------------------------------

        [Serializable]
        public class VD_Common_SaveXML
        {
            public bool AutoSeach1;     // xml com port 첫번째 
            public int Port1;
            public bool AutoSeach2;    // xml com port 두번째
            public int Port2;

            public VD_Common_SaveXML()
            {
                AutoSeach1 = false;
                Port1 = 0;
                AutoSeach2 = false;
                Port2 = 0;
            }
        }

        [Serializable]
        public class VD_Common_ShutDown
        {
            public VD_Common_ShutDown()
            {
            }
        }

        [Serializable]
        public class VD_Common_GPS
        {
            public String Latitude;
            public String Longitude;

            public VD_Common_GPS()
            {
                Latitude = String.Empty;
                Longitude = String.Empty;
            }
        }

        [Serializable]
        public class VD_Common_Calibration
        {
            public String Pulse;
            public String Gyro;
            public float Temp;
            public Char SpeedPulse;
            public Char GyroSF;
            public Char GyroBias;

            public VD_Common_Calibration()
            {
                Pulse = String.Empty;
                Gyro = String.Empty;
                Temp = 0;
                SpeedPulse = '\0';
                GyroSF = '\0';
                GyroBias = '\0';
            }
        }

        [Serializable]
        public class VD_Common_SaveIni
        {
            public String MchnID;
            public String MchnType;
            public string LanguageType;
            public string LanguagePath;
            public string UISize;

            public VD_Common_SaveIni()
            {
                MchnID = String.Empty;
                MchnType = String.Empty;
                LanguageType = string.Empty;
                LanguagePath = string.Empty;
                UISize = string.Empty;
            }
        }

        //[Serializable]
        //public class VD_Common_SimpleBlockInfo
        //{
        //    public String BlcName;
        //    public Boolean IsVirtual;
        //}

        //[Serializable]
        //public class VD_Common_SimpleBlockInfoList_Receive
        //{
        //    public int m_iCount;
        //    public List<VD_Common_SimpleBlockInfo> m_pData;

        //    public VD_Common_SimpleBlockInfoList_Receive()
        //    {
        //        m_iCount = -1;
        //        m_pData = new List<VD_Common_SimpleBlockInfo>();
        //    }
        //}

        //[Serializable]
        //public class VD_Common_SimpleBayInfo
        //{
        //    public String BayName;

        //    public static int Compare(VD_Common_SimpleBayInfo x, VD_Common_SimpleBayInfo y)
        //    {
        //        Int32 numX = Convert.ToInt32(x.BayName);
        //        Int32 numY = Convert.ToInt32(y.BayName);

        //        return numX < numY ? -1 : numX > numY ? 1 : 0;                
        //    }
        //}

        //public class VD_Common_SimpleBlockBayInfo
        //{
        //    public String BayName;

        //    public static int Compare(VD_Common_SimpleBayInfo x, VD_Common_SimpleBayInfo y)
        //    {
        //        Int32 numX = Convert.ToInt32(x.BayName);
        //        Int32 numY = Convert.ToInt32(y.BayName);

        //        return numX < numY ? -1 : numX > numY ? 1 : 0;
        //    }
        //}

        //[Serializable]
        //public class VD_Common_SimpleBayInfoList_Receive
        //{
        //    public int m_iCount;
        //    public Dictionary<String, List<VD_Common_SimpleBayInfo>> m_pData;        // <block, bayList>

        //    public VD_Common_SimpleBayInfoList_Receive()
        //    {                
        //        m_iCount = -1;
        //        m_pData = new Dictionary<String, List<VD_Common_SimpleBayInfo>>();
        //    }
        //}

        [Serializable]
        public class VD_Common_SimpleRowInfo
        {
            public String rowNm;
            public String abbRowNo;
            public Boolean isUse;

            public VD_Common_SimpleRowInfo()
            {
                rowNm = String.Empty;
                abbRowNo = String.Empty;
                isUse = true;
            }
        }

        [Serializable]
        public class VD_Common_SimpleBayInfo : IDisposable
        {
            public String BayName;
            public String opr;
            public String cntrLen;
            public String cntrTp;
            public String inspCode;
            public String categoryName;
            public SortedDictionary<int, VD_Common_SimpleRowInfo> RowNameMap;

            public VD_Common_SimpleBayInfo()
            {
                BayName = String.Empty;
                opr = String.Empty;
                cntrLen = String.Empty;
                cntrTp = String.Empty;
                inspCode = String.Empty;
                categoryName = String.Empty;
                RowNameMap = new SortedDictionary<int, VD_Common_SimpleRowInfo>();
            }

            public void Dispose()
            {
                this.RowNameMap.Clear();
                this.RowNameMap = null;
            }

            public static int Compare(VD_Common_SimpleBayInfo x, VD_Common_SimpleBayInfo y)
            {
                // Aug 10 2022 improve bay sort order
                Boolean parseIntBayXBool = Int32.TryParse(x.BayName, out int bayIntX);
                Boolean parseIntBayYBool = Int32.TryParse(y.BayName, out int bayIntY);
                if (!parseIntBayXBool && parseIntBayYBool)
                {
                    return -1;
                }
                else if (parseIntBayXBool && !parseIntBayYBool)
                {
                    return 1;
                }
                else if (parseIntBayXBool && parseIntBayYBool)
                {
                    return bayIntX.CompareTo(bayIntY);
                }
                else // parseIntBayXBool FALSE && parseIntBayYBool FALSE
                {
                    return x.BayName.CompareTo(y.BayName);
                }
            }
        }

        [Serializable]
        public enum Row_Direction
        {
            TB, //: Top to Bottom
            BT, //: Bottom to Top
        }

        [Serializable]
        public class VD_Common_SimpleBlockInfo : IDisposable
        {
            public String BlcName;
            public String userCode;
            public String loginUser;
            public String goCont;
            public String giCont;
            public String moCont;
            public String miCont;
            public String dsCont;
            public String ldCont;
            public String etcCont;
            public String totalCont;

            public Boolean IsVirtual;
            public Boolean isBolBlck;
            public Int32 MaxTier;
            public Row_Direction Direction;

            public SortedDictionary<string, VD_Common_SimpleBayInfo> DicBay;

            public SortedDictionary<string, VD_Common_SimpleBayInfo> DicBayUnlink;

            public VD_Common_SimpleBlockInfo()
            {
                BlcName = String.Empty;
                userCode = String.Empty;
                loginUser = String.Empty;
                goCont = String.Empty;
                giCont = String.Empty;
                moCont = String.Empty;
                miCont = String.Empty;
                dsCont = String.Empty;
                ldCont = String.Empty;
                etcCont = String.Empty;
                totalCont = String.Empty;
                IsVirtual = false;
                isBolBlck = true;
                Direction = Row_Direction.BT;
                MaxTier = 8;
                DicBay = null;// new SortedDictionary<int, VD_Common_SimpleBayInfo>();
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

            public static int Compare(VD_Common_SimpleBlockInfo x, VD_Common_SimpleBlockInfo y)
            {
                if (x.IsVirtual == y.IsVirtual)
                    return x.BlcName.CompareTo(y.BlcName);
                else if (x.IsVirtual)
                    return -1;
                else //if (y.IsVirtual)
                    return 1;
            }
        }

        [Serializable]

        public class VD_Common_SimpleBlockBayInfo_Receive : IDisposable
        {
            //public SortedDictionary<String, VD_Common_SimpleBlockInfo> DicBlock;        // <block, bayList>
            public Dictionary<String, VD_Common_SimpleBlockInfo> DicBlock;        // <block, bayList>

            public int Count
            {
                get
                {
                    return DicBlock.Count;
                }
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

            public VD_Common_SimpleBlockBayInfo_Receive()
            {
                //DicBlock = new SortedDictionary<String, VD_Common_SimpleBlockInfo>();
                DicBlock = new Dictionary<String, VD_Common_SimpleBlockInfo>();
            }
        }

        [Serializable]
        public class VD_Common_Def_Vessel
        {
            public String vessel;
            public String voyage;
            public VD_Common_Def_Location vslLoc;

            public VD_Common_Def_Vessel()
            {
                vessel = String.Empty;
                voyage = String.Empty;
                vslLoc = new VD_Common_Def_Location();
            }
        }

        [Serializable]
        public class VD_Common_DoSwap4Manual_Send
        {
            public String cntrNo;
            //public String cntrPoint;
            public String partnerMachineID;
            public String jobid;
            public String externalId;
            public String newYTNo;

            public String positionOnChassis;
            public String chgPrgmId;

            public VD_Common_DoSwap4Manual_Send()
            {
                cntrNo = String.Empty;
                //cntrPoint = String.Empty;
                partnerMachineID = String.Empty;
                jobid = String.Empty;
                externalId = String.Empty;
                newYTNo = String.Empty;
                positionOnChassis = String.Empty;
                chgPrgmId = String.Empty;
            }
        }

        [Serializable]
        public class VD_Common_Def_NoWorkLocation : IDisposable
        {
            // NoWorkType : AREA, TUNNEL, TIER
            public String noWorkTp;

            // blck             e.g) 65A
            // bay (From-To)    e.g) 71-79
            // row (From-To)    e.g) A-F
            // tier (From-To)   e.g) 1-2
            public VD_Common_Def_Location loc;
            public String FromBay;
            public String ToBay;
            public String FromRow;
            public String ToRow;
            public String FromTier;
            public String ToTier;

            public VD_Common_Def_NoWorkLocation()
            {
                noWorkTp = String.Empty;
                loc = new VD_Common_Def_Location();

                FromBay = String.Empty;
                ToBay = String.Empty;
                FromRow = String.Empty;
                ToRow = String.Empty;
                FromTier = String.Empty;
                ToTier = String.Empty;
            }

            public void Dispose()
            {
                loc = null;
            }
        }

        #endregion [VMT_Data_Common Class]
    }
}