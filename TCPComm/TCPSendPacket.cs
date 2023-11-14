using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TCPComm
{
    public class TCPSendPacket
    {
        private TCPCommType _cmdType;
        private Object _obj;
        private Int32 _packetSize;
        private Int32 _sendSize;

        public TCPSendPacket()
        {
            Obj = null;
            PacketSize = 0;
            SendSize = 0;
        }

        ~TCPSendPacket() { }

        public TCPCommType CmdType
        {
            set { _cmdType = value; }
            get { return _cmdType; }
        }

        public Object Obj
        {
            set { _obj = value; }
            get { return _obj; }
        }

        public Int32 PacketSize
        {
            set { _packetSize = value; }
            get { return _packetSize; }
        }

        public Int32 SendSize
        {
            set { _sendSize = value; }
            get { return _sendSize; }
        }

    }
}
