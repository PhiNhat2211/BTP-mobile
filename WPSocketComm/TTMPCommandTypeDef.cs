using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WPSocketComm.Network
{       
    public class TTMPCmdData
    {
        public const string ENDLINE = "/ttend\r\n";
        public const string SPACE = " ";
        public const string EQUAL = "=";
        public const string DELIMITER = "|";

        public const string XMLSTREAM_START = "<$type:xml>";
        public const string XMLSTREAM_END = "</$type:xml>";

        //======================================================
        //= For CLT EE.Client(exe) Tray (Command)
        //--Receive TTCMD
        public const string TTCMD_DgpsSignal = "ttcmd_dgpssignal";
        public const string TTCMD_PdsSignal = "ttcmd_pdssignal";
        public const string TTCMD_Response_CFG_EKF = "ttcmd_responsecfgekf";

        public const string TTCMD_keepAlive = "ttcmd_keepalive";

        //--Send TTCMD
        public const string TTCMD_ResetDgps = "ttcmd_resetdgps";
        public const string TTCMD_SendPubx05 = "ttcmd_sendpubx05";
        public const string TTCMD_SendPubx06 = "ttcmd_sendpubx06";

        public const string TTCMD_SendHighBackward = "ttcmd_sendhighbackward";
        public const string TTCMD_SendHighForward = "ttcmd_sendhighforward";

        public const string TTCMD_Request_CFG_EKF = "ttcmd_requestcfgekf";        
        public const string TTCMD_SavedGpsCfg = "ttcmd_savedgpscfg";

        public const string TTCMD_SendChassisInfo = "ttcmd_sendchassisinfo";

        public const string TTCMD_MachineArrival = "ttcmd_machinearrival";
        public const string TTCMD_MachineReady = "ttcmd_machineready";

        //--Receive Ack
        public const string TTACK_Resetdgps = "ttack_resetdgps";
        public const string TTACK_SendPubx05 = "ttack_sendpubx05";
        public const string TTACK_SendPubx06 = "ttack_sendpubx06";

        public const string TTACK_SendHighBackward = "ttack_sendhighbackward";
        public const string TTACK_SendHighForward = "ttack_sendhighforward";

        public const string TTACK_Request_CFG_EKF = "ttack_request_cfg_ekf";
        public const string TTACK_SavedGpsCfg = "ttack_savedgpscfg";

        public const string TTACK_SendChassisInfo = "ttack_sendchassisinfo";

        public const string TTACK_MachineArrival = "ttack_machinearrival";
        public const string TTACK_MachineReady = "ttack_machineready";

        //======================================================
        //= For CLT EE.Client(exe) Tray (Parameter)
        //-- Common Param
        public const string PARAM_Uuid = "uuid";

        //--Receive Param
        public const string PARAM_Enable = "enable";
        //--ttcmd_dgpssignal--//
        public const string PARAM_Latitude= "latitude";
        public const string PARAM_Longitude= "longitude";        
        public const string PARAM_Pulse = "pulse";
        public const string PARAM_Gyro = "gyro";
        public const string PARAM_Temp = "temp";
        public const string PARAM_SpeedPulse = "speedpulse";
        public const string PARAM_GyroSF = "gyrosf";
        public const string PARAM_GyroBias = "gyrobias";
        public const string PARAM_Speed = "speed";

        //--ttcmd_pdssignal--//
        public const string PARAM_Fuel = "fuel";
        public const string PARAM_ConeChecker = "conechecker";

        //--ttcmd_responsecfgekf--//
        public const string PARAM_Response = "response";

        //--Send Param
        public const string PARAM_ChassisType = "chassistype";
        public const string PARAM_ChassisNumber = "chassisnumber";

        //======================================================
        //= Command constant Definition
        public const string TTCMD_ContentOpen = "ttcmd_contentopen";
        public const string TTCMD_ContentPlay = "ttcmd_contentplay";
        public const string TTCMD_ContentPause = "ttcmd_contentpause";
        public const string TTCMD_ContentStop = "ttcmd_contentstop";
        public const string TTCMD_ContentStatus = "ttcmd_contentstatus";

        public const string TTACK_ContentOpen = "ttack_contentopen";
        public const string TTACK_ContentPlay = "ttack_contentplay";
        public const string TTACK_ContentPause = "ttack_contentpause";
        public const string TTACK_ContentStop = "ttack_contentstop";
        public const string TTACK_ContentStatus = "ttack_contentstatus";

        public const string TTACK_Connect = "ttack_connect";
        public const string TTACK_Disconnect = "ttack_disconnect";
        public const string TTACK_Status = "ttack_status";        
        //======================================================
        //= Parameter constant Definition
        public const string PARAM_Source = "Source";
        public const string PARAM_ID = "ID";
        public const string PARAM_Position = "Position";
        
        public const string PARAM_ContentsName = "ContentsName";
        public const string PARAM_ContentsID = "ContentsID";
        public const string PARAM_PlayListID = "PlayListID";
        public const string PARAM_ConfigurationID = "ConfigurationID";
        public const string PARAM_ApplicationID = "ApplicationID";
        public const string PARAM_SystemName = "SystemName";
        public const string PARAM_SystemID = "SystemID";
        public const string PARAM_Invoke = "Invoke";
        public const string PARAM_Email = "Email";
        public const string PARAM_Password = "Password";
        public const string PARAM_EmployeeID = "EmployeeID";
        public const string PARAM_Name = "Name";
        public const string PARAM_Status = "Status";
        public const string PARAM_ReturnCode = "ReturnCode";
        public const string PARAM_PresentationMode = "PresentationMode";

        public const string PARAM_ContentsRepeat = "Repeat";
        public const string PARAM_DisplayMatrix = "DisplayMatrix";
        public const string PARAM_ClientAddress = "ClientAddress";
        public const string PARAM_ContentsServerAddress = "ContentServerAddress";
        public const string PARAM_Account = "Account";
        public const string PARAM_PortNum = "PortNum";
        public const string PARAM_ShowLog = "ShowLog";
        public const string PARAM_Transition = "Transition";
        public const string PARAM_StartPosition = "StartPosition";
        public const string PARAM_Duration = "Duration";
        public const string PARAM_BGColor = "BGColor";
        public const string PARAM_Volume = "Volume";
        public const string PARAM_Mute = "Mute";
        public const string PARAM_Group = "Group";
        public const string PARAM_Filter = "Filter";
        public const string PARAM_ShellCommand = "ShellCommand";
        public const string PARAM_Platform = "platform";
        public const string PARAM_Scene = "Scene";
        public const string PARAM_AutoPlay = "Autoplay";
        public const string PARAM_Power = "Power";
        public const string PARAM_Service = "Service";
        public const string PARAM_Option = "Option";
    }

    public class TTMPCmdType
    {
    }
}
