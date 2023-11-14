using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WPSocketComm.Socket_UDP;
using System.Net;
using WPSocketComm.Network;

namespace UDPComm.Interface
{
    public class IfUDPControl
    {
        private socketClientUDP _udpClientSocket;
        private socketServerUDP _udpServerSocket;

        private Int32 _clientPort;
        private Int32 _serverPort;
        public UDPAPI.UDPCommCallback _sendCallback;
        public UDPAPI.UDPCommCallback _receiveCallback;

        public IfUDPControl(Int32 clientPort, Int32 serverPort, UDPAPI.UDPCommCallback sendCallback, UDPAPI.UDPCommCallback receiveCallback)
        {
            _clientPort = clientPort;
            _serverPort = serverPort;
            _sendCallback = sendCallback;
            _receiveCallback = receiveCallback;
        }

        public Boolean udpSocketOpen()
        {
            //-----------------------------------------------------
            //- Initialize UDP Broadcast Signal Socket // Client
            if (_udpClientSocket != null)
                _udpClientSocket.Close();

            _udpClientSocket = new socketClientUDP();
            Boolean bConnect = _udpClientSocket.socketClientInit(IPAddress.Any, _clientPort);
            if (bConnect)
            {
                _udpClientSocket.udpRcvMsgEvt += new socketClientUDP.UdpSrvRcvMsgEventHandler(_udpClientSocket_udpRcvMsgEvt);
                _udpClientSocket.startListen();
            }

            //-----------------------------------------------------
            //- Initialize UDP Broadcast Signal Socket // Server
            _udpServerSocket = new socketServerUDP();
            bConnect = _udpServerSocket.socketServerInit(IPAddress.Parse("127.0.0.255"), _serverPort);

            return bConnect;
        }

        private void _udpClientSocket_udpRcvMsgEvt(object sender, byte[] rcvMsg)
        {
            System.Text.Encoding enc = System.Text.Encoding.ASCII;
            string rsvString = enc.GetString(rcvMsg);

            // Do recieved command
            String[] cmdArray = rsvString.Split('\n');
            for (int i = 0; i < cmdArray.Length; i++)
            {
                String cmd = cmdArray[i];

                if (String.IsNullOrEmpty(cmd))
                    continue;

                cmd += "\n";
                //sEventMgr.DoEvent(cmd); // TODO

                DoEvent(cmd);

                System.Threading.Thread.Sleep(30);
            }
        }

        private void DoEvent(String cmd)
        {
            TTMPCommand ttmpCmd = new TTMPCommand();

            if (!ttmpCmd.Parse(cmd))
            {
                // Error
            }

            switch (ttmpCmd.Message)
            {
                case TTMPCmdData.TTCMD_DgpsSignal:
                    _receiveCallback(UDPCommType.Recieve_DgpsSignal, ttmpCmd);
                    break;
                case TTMPCmdData.TTCMD_PdsSignal:
                    _receiveCallback(UDPCommType.Recieve_PdsSignal, ttmpCmd);
                    break;
                case TTMPCmdData.TTCMD_Response_CFG_EKF:
                    _receiveCallback(UDPCommType.Response_CFG_EKF, ttmpCmd);
                    break;
                default:
                    break;
            }
        }

        private String GetOpenString(String strTTCMD, Dictionary<String, String> dic = null)
        {
            String strAck = strTTCMD;
            strAck += TTMPCmdData.SPACE;

            if (dic != null &&
                dic.Count != 0)
            {
                foreach (KeyValuePair<String, String> pair in dic)
                {
                    strAck += pair.Key + TTMPCmdData.EQUAL + pair.Value;
                    strAck += TTMPCmdData.DELIMITER;
                }

                strAck = strAck.Substring(0, strAck.LastIndexOf(TTMPCmdData.DELIMITER));
                strAck += TTMPCmdData.ENDLINE;
            }
            else
            {
                strAck += TTMPCmdData.ENDLINE;
            }

            return strAck;
        }

        public Object SendPacket(UDPCommType cmdType, Object dic = null)
        {
            String strMsg = String.Empty;
            byte[] bStrByte = null;

            switch (cmdType)
            {
                case UDPCommType.Send_CFG_RST:
                    {
                        strMsg = GetOpenString(TTMPCmdData.TTCMD_ResetDgps);
                    }
                    break;
                case UDPCommType.Send_PUBX_05:
                    {
                        strMsg = GetOpenString(TTMPCmdData.TTCMD_SendPubx05);
                    }
                    break;
                case UDPCommType.Send_PUBX_06:
                    {
                        strMsg = GetOpenString(TTMPCmdData.TTCMD_SendPubx06);
                    }
                    break;
                case UDPCommType.Send_HighBackward:
                    {
                        strMsg = GetOpenString(TTMPCmdData.TTCMD_SendHighBackward);
                    }
                    break;
                case UDPCommType.Send_HighForward:
                    {
                        strMsg = GetOpenString(TTMPCmdData.TTCMD_SendHighForward);
                    }
                    break;
                case UDPCommType.Request_CFG_EKF:
                    {
                        strMsg = GetOpenString(TTMPCmdData.TTCMD_Request_CFG_EKF);
                    }
                    break;
                case UDPCommType.Send_CFG_SAVE:
                    {
                        strMsg = GetOpenString(TTMPCmdData.TTCMD_SavedGpsCfg);
                    }
                    break;
                case UDPCommType.SendChassisInfo:
                    {
                        strMsg = GetOpenString(TTMPCmdData.TTCMD_SendChassisInfo, dic as Dictionary<String, String>);
                    }
                    break;
                default:
                    break;
            }

            bStrByte = Encoding.UTF8.GetBytes(strMsg);
            _udpServerSocket.udpSendMsg(bStrByte);

            return strMsg;
        }
    }


}
