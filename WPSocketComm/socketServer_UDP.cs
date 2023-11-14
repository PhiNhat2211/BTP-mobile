using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;

namespace WPSocketComm.Socket_UDP
{
    public class socketServerUDP
    {   
        Socket _sSocket;
        //IPEndPoint _EPLocal;
        IPEndPoint _EPRemote;
        EndPoint _remote;
        bool _bInit = false;

        public void Program()
        {
        }

        // param addr : Direct Broadcasting Address
        // param port : Boradcasting Server/Client Port
        public bool socketServerInit(IPAddress addr, int port)
        {
            _sSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            _sSocket.EnableBroadcast = true;
            _sSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Broadcast, 1);
            _EPRemote = new IPEndPoint(addr, port);
            _remote = (EndPoint)_EPRemote;

            _bInit = true;

            return true;
        }

        public void udpSendMsg(byte[] sndMsg)
        {
            if(_bInit)
                _sSocket.SendTo(sndMsg, _remote);
        }

        public void Close()
        {
            _sSocket.Close();

            _bInit = false;
        }
    }
}
