using System;
using System.Net;
using System.Text;
using System.Threading;
using System.Net.Sockets;
using System.Collections.Generic;
using System.IO;

namespace WPSocketComm.Socket_TCP
{
    public class socketClientTCP
    {
        TcpClient tcpClient;
        Thread tcpClientRcvThread;
        bool onRecieve = true;

        //public event TcpClientConnectedEventHandle tcpConnectedEvt;
        public event TcpClientDisConnectedEventHandle tcpDiconnectedEvt;
        public event TcpClientRecievedMsgEventHandle tcpCltRcvEvt;

        public delegate void TcpClientConnectedEventHandle();
        public delegate void TcpClientDisConnectedEventHandle();
        public delegate void TcpClientRecievedMsgEventHandle(byte[] rcvMsg);

        public socketClientTCP()
        {
        }

        public bool isSocketClinetConnect()
        {
            if (tcpClient == null)
                return false;

            return tcpClient.Connected;
        }

        public bool socketClientInit(string ipaddress, string iPort)
        {
            try
            {
                IPEndPoint srvEndip =
                    new IPEndPoint(IPAddress.Parse(ipaddress), int.Parse(iPort));

                tcpClient = new TcpClient();
                tcpClient.Connect(srvEndip);
                return true;
            }
            catch (SocketException sEx)
            {   
                string errMsg = sEx.Message;
                return false;
            }
            catch (Exception ex)
            {
                string errMsg = ex.Message;
                return false;
            }
        }

        public bool tcpClientStop()
        {
            try
            {
                if (tcpClientRcvThread != null)
                {
                    tcpClientRcvThread.Abort();
                    tcpClient.Close();
                }
                return true;
            }
            catch (Exception) 
            {
                return false;
            }
        }

        public void recieveThread()
        {
            tcpClientRcvThread = new Thread(new ThreadStart(tcpClientThread));
            tcpClientRcvThread.IsBackground = true;
            tcpClientRcvThread.Start();
        }

        private void tcpClientThread()
        {
            try
            {
                onRecieve = true;
                NetworkStream rcvStream = tcpClient.GetStream();

                while (onRecieve)
                {
                    if (rcvStream.CanRead)
                    {
                        byte[] rcvBuffer = new byte[1024];
                        //StringBuilder rcvCompleteMsg = new StringBuilder();
                        MemoryStream ms = new MemoryStream();
                        int numberOfBytesRead = 0;
                        //Incoming message may be larger than the buffer size. 
                        do
                        {
                            numberOfBytesRead = rcvStream.Read(rcvBuffer, 0, rcvBuffer.Length);
                            ms.Write(rcvBuffer, 0, numberOfBytesRead);
                            //rcvCompleteMsg.AppendFormat("{0}", Encoding.ASCII.GetString(rcvBuffer, 0, numberOfBytesRead));

                            //property is not a reliable way to detect the end of the response
                            Thread.Sleep(1);
                        }
                        while (rcvStream.DataAvailable);

                        Byte[] rcvCompleteMsg = ms.ToArray();
                        tcpCltRcvEvt(rcvCompleteMsg);
                    }
                    else
                    {
                        // cannot read from this NetworkStream.
                        onRecieve = false;
                    }
                }
                tcpClientStop();
            }
            catch (Exception ex)
            {
                string errMsg = ex.Message;
                onRecieve = false;
                tcpClientStop();
                if (tcpDiconnectedEvt != null)
                {
                    tcpDiconnectedEvt();
                }
            }
        }

        public Int32 tcpClientSndMsg(byte[] sndMsg)
        {
            if (tcpClient != null && tcpClient.Connected != false)
            {
                NetworkStream sndStream = tcpClient.GetStream();
                sndStream.Write(sndMsg, 0, sndMsg.Length);
                return sndMsg.Length;
            }
            else 
            {
                //System.Windows.Forms.MessageBox.Show("tcpClient is Null");
                return 0;
            }
        }
    }
}
