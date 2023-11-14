using System;
using System.Net;
using System.Text;
using System.Threading;
using System.Net.Sockets;
using System.Collections.Generic;

namespace WPSocketComm.Socket_TCP
{
    public class socketServerTCP
    {
        TcpListener tcpLisner;
        TcpClient acceptClient;
        NetworkStream rcvStream;
        Thread threadStartListner;
        bool listnerFlag = false;
        bool acceptFalg = false;

        public event TcpSrvMsgEventHandler tcpMsgEvt;
        public event TcpSrvRcvMsgEventHandler tcpRcvMsgEvt;
        //public event TcpConnectedEventHandler tcpConnected;
        public event TcpDisconnectedEventHandler tcpDisconnected;
        public delegate void TcpSrvMsgEventHandler(object sender, string rcvMsg, bool sMsgFlag);
        public delegate void TcpSrvRcvMsgEventHandler(object sender, byte[] rcvMsg);
        public delegate void TcpConnectedEventHandler(object sender);
        public delegate void TcpDisconnectedEventHandler(object sender);

        public bool IsHTTP_Get { get; set; } 

        public socketServerTCP()
        {
            IsHTTP_Get = false;
        }

        public bool socketServerInit(int iPort, bool IsHttpGet = false)
        {
            try
            {
                IsHTTP_Get = IsHttpGet;
                tcpLisner = new TcpListener(new IPEndPoint(IPAddress.Any, iPort));
                tcpLisner.Start();

                return true;
            }
            catch (Exception ex)
            {
                string errMsg = ex.Message;
                //MessageBox.Show(ex.Message);
                return false;
            }
        }

        public bool tcsSrvLsnStop()
        {
            try
            {
                if( rcvStream != null )
                    rcvStream.Close();
                tcpLisner.Stop();
                threadStartListner.Abort();
                tcpDisconnected(this);
                return true;
            }
            catch (Exception ex)
            {
                string errMsg = ex.Message;
                return false;
            }
        }

        public void startLisner()
        {
            try
            {
                threadStartListner = new Thread(new ThreadStart(tcpListnning));
                threadStartListner.IsBackground = true;
                threadStartListner.Start();
            }
            catch(Exception)
            {
                return;
            }
        }

        private void tcpListnning()
        {
            try
            {
                //tcpConnected(this);
                listnerFlag = true;
                while (listnerFlag)
                {
                    acceptClient = tcpLisner.AcceptTcpClient();
                    string acceptMsg = ((IPEndPoint)acceptClient.Client.RemoteEndPoint).Address.ToString();
                    if (tcpMsgEvt != null)
                    {
                        tcpMsgEvt(this, acceptMsg, true);
                    }

                    acceptFalg = true;
                    while (acceptFalg)
                    {
                        try
                        {
                            rcvStream = acceptClient.GetStream();
                            byte[] rcvMsg = new byte[1024];
                            int tSize = rcvStream.Read(rcvMsg, 0, rcvMsg.Length);
                            if (tSize > 0)
                            {
                                if (tcpRcvMsgEvt != null)
                                {
                                    Array.Resize<byte>(ref rcvMsg, tSize);
                                    tcpRcvMsgEvt(this, rcvMsg);
                                }

                                if (IsHTTP_Get)
                                {
                                    acceptFalg = false;
                                    acceptClient.Close();
                                }
                            }
                            else
                            {
                                acceptFalg = false;
                            }
                        }
                        catch (Exception)
                        {
                            rcvStream.Close();
                            acceptFalg = false;
                        }
                    }

                    if (tcpMsgEvt != null)
                    {
                        tcpMsgEvt(this, acceptMsg, false);
                    }
                }
            }
            catch (Exception)
            {
                listnerFlag = false;
                tcsSrvLsnStop();
            }
        }

        public void tcpSendMsg(byte[] sndMsg)
        {
            try
            {
                if (acceptClient.Connected)
                {
                    NetworkStream sndStream = acceptClient.GetStream();
                    sndStream.Write(sndMsg, 0, sndMsg.Length);
                }
            }
            catch (Exception)
            {
            }
        }
    }
}
