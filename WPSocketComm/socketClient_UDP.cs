using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Net.NetworkInformation;

namespace WPSocketComm.Socket_UDP
{
    public class socketClientUDP
    {
        //public event UdpSrvMsgEventHandler udpMsgEvt;
        public event UdpSrvRcvMsgEventHandler udpRcvMsgEvt;
        public delegate void UdpSrvMsgEventHandler(object sender, string rcvMsg, bool sMsgFlag);
        public delegate void UdpSrvRcvMsgEventHandler(object sender, byte[] rcvMsg);

        Thread threadStartListner;
        private byte[] data = new byte[1024];

        Socket _cSocket;
        private IPEndPoint _EPLocal;
        private EndPoint _local;


        public void Program()
        {
        }

        public bool socketClientInit(IPAddress addr, int port)
        {
            try
            {
                _cSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                _EPLocal = new IPEndPoint(addr, port);
                _cSocket.Bind(_EPLocal);
                _local = (EndPoint)_EPLocal;
            }
            catch (SocketException ex)
            {
                if (ex.SocketErrorCode == SocketError.AddressAlreadyInUse /* check this is the one you get */ )
                    throw ex;

                return false;
            }

            return true;
        }

        public bool CheckPortIsAvailable(int port)
        {
            bool isAvailable = true;

            // Evaluate current system tcp connections. This is the same information provided 
            // by the netstat command line application, just in .Net strongly-typed object 
            // form.  We will look through the list, and if our port we would like to use 
            // in our TcpClient is occupied, we will set isAvailable to false. 
            IPGlobalProperties ipGlobalProperties = IPGlobalProperties.GetIPGlobalProperties();
            IPEndPoint[] tcpConnInfoArray = ipGlobalProperties.GetActiveUdpListeners();

            foreach (IPEndPoint ep in tcpConnInfoArray)
            {
                if (ep.Port == port)
                {
                    isAvailable = false;
                    break;
                }
            }

            return isAvailable;
        }

        public void startListen()
        {
            threadStartListner = new Thread(new ThreadStart(udpListnning));
            threadStartListner.Priority = ThreadPriority.Highest;
            threadStartListner.IsBackground = true;
            threadStartListner.Start();
 
        }

        private void udpListnning()
        {
            try
            {
                byte[] rcvMsg = new byte[1024];
                int recv = 0;

                while (true)
                {
                    recv = _cSocket.ReceiveFrom(data, ref _local);

                    if (udpRcvMsgEvt != null)
                    {
                       
                        Array.Resize<byte>(ref rcvMsg, recv);
                        Array.Copy(data, rcvMsg, recv);
                        udpRcvMsgEvt(this, rcvMsg);
                    }

                    //String stringData = Encoding.UTF8.GetString(data, 0, recv);
                    //MessageBox.Show(stringData.ToString());
                }
            }
            catch (Exception e)
            {
                string exception = e.Message;
            }
        }

        public void Close()
        {
            threadStartListner.Abort();

            _cSocket.Close();
        }
    }
}
