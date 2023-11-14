using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UDPComm.Interface;

namespace UDPComm
{
    public enum UDPCommType
    {
        UDPSocketOpen,

        Send_CFG_RST,
        Send_PUBX_05,
        Send_PUBX_06,
        Send_HighBackward,
        Send_HighForward,
        Send_CFG_SAVE,
        Request_CFG_EKF,

        SendChassisInfo,

        Recieve_DgpsSignal,
        Recieve_PdsSignal,
        Response_CFG_EKF,
    }

    public class UDPException : Exception
    {
        public UDPException()
        {
        }

        public UDPException(String message)
            : base(message)
        {
        }

        public UDPException(String message, Exception inner)
            : base(message, inner)
        {
        }
        public UDPCommType udpCommType { get; set; }
    }

    public class UDPAPI
    {
        public delegate void ExceptionDelegate(Exception ex);
        public static List<ExceptionDelegate> ExceptionDelegatorList = new List<ExceptionDelegate>();

        public static void AddExceptionDelegator(ExceptionDelegate ed)
        {
            ExceptionDelegatorList.Add(ed);
        }

        public static void RemoveExceptionDelegator(ExceptionDelegate ed)
        {
            ExceptionDelegatorList.Remove(ed);
        }

        public delegate void UDPCommCallback(UDPCommType type, Object obj);

        #region variables
        private static UDPAPI _instance = new UDPAPI();

        private Int32 _clientPort = 30000;
        private Int32 _serverPort = 40000;
        private UDPCommCallback _sendCallback = null;
        private UDPCommCallback _receiveCallback = null;

        private IfUDPControl _udpControl = null;

        private UDPExecuteThread _thread = null;
        #endregion variables

        #region properties
        public IfUDPControl UDPControl
        {
            get
            {
                if (_udpControl == null)
                {
                    _udpControl = new IfUDPControl(_clientPort, _serverPort, _instance._sendCallback, _instance._receiveCallback);
                }

                return _udpControl;
            }
        }
        #endregion properties

        private UDPAPI()
        {
            _thread = new UDPExecuteThread(this);
        }

        public static void Init(Int32 clientPort, Int32 serverPort, UDPCommCallback sendCallback, UDPCommCallback receiveCallback)
        {
            _instance._clientPort = clientPort;
            _instance._serverPort = serverPort;
            _instance._sendCallback = sendCallback;
            _instance._receiveCallback = receiveCallback;
            _instance._thread.sendCallback = sendCallback;
            _instance._thread.receiveCallback = receiveCallback;
        }

        public static void Start()
        {
            _instance._thread.Start();
        }

        public static void End()
        {
            _instance._thread.End();
        }

        public static void StartPolling(UDPCommType type, Object obj, int timeSpan)
        {
            _instance._thread.StartPolling(type, obj, timeSpan);
        }

        public static void StopPolling(UDPCommType type)
        {
            _instance._thread.StopPolling(type);
        }

        public static void UDPSocketOpen()
        {
            _instance._thread.Do(UDPCommType.UDPSocketOpen, null);
        }

        public static void Send_CFG_RST()
        {
            _instance._thread.Do(UDPCommType.Send_CFG_RST, null);
        }

        public static void Send_PUBX_05()
        {
            _instance._thread.Do(UDPCommType.Send_PUBX_05, null);
        }

        public static void Send_PUBX_06()
        {
            _instance._thread.Do(UDPCommType.Send_PUBX_06, null);
        }

        public static void Send_HighBackward()
        {
            _instance._thread.Do(UDPCommType.Send_HighBackward, null);
        }

        public static void Send_HighForward()
        {
            _instance._thread.Do(UDPCommType.Send_HighForward, null);
        }

        public static void Send_CFG_SAVE()
        {
            _instance._thread.Do(UDPCommType.Send_CFG_SAVE, null);
        }

        public static void Request_CFG_EKF()
        {
            _instance._thread.Do(UDPCommType.Request_CFG_EKF, null);
        }
    }
}
