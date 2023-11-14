using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using WPSocketComm.Socket_TCP;
using System.Diagnostics;
using System.Runtime.Serialization.Json;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.InteropServices;

namespace TCPComm.Interface
{
    public class IfTCPControl
    {
        private socketClientTCP _tcpSocket;
        public TCPAPI.TCPCommCallback _sendCallback;
        public TCPAPI.TCPCommCallback _receiveCallback;
        private String _ip;
        private int _port;

        public IfTCPControl(String ip, int port, TCPAPI.TCPCommCallback sendCallback, TCPAPI.TCPCommCallback receiveCallback)
        {
            _ip = ip;
            _port = port;
            _sendCallback = sendCallback;
            _receiveCallback = receiveCallback;
        }

        ~IfTCPControl() { }

        void _tcpSocket_tcpConnectedEvt()
        {
            _sendCallback(TCPCommType.TCPSocketConnected, null);
        }

        void _tcpSocket_tcpDiconnectedEvt()
        {
            _sendCallback(TCPCommType.TCPSocketDisconnected, null);
        }

        const int PACKET_MAX = 1024 * 1024;
        byte[] bsRcvBuffer = new byte[PACKET_MAX];
        byte[] bsPacketBuffer = new byte[PACKET_MAX];
        int nRcvBufferEnd = 0;
                

        private void socketTCP_tcpCltRcvEvt(byte[] rcvMsg)
        {
            // Accumulate Received Byte Stream
            Buffer.BlockCopy(rcvMsg, 0, bsRcvBuffer, nRcvBufferEnd, rcvMsg.Length);
            nRcvBufferEnd += rcvMsg.Length;
                        
            int nExtractRet = 0;
            
            do
            {
                if (nRcvBufferEnd == 0)
                    break;

                int nPacketSize = 0;

                nExtractRet = ExtractPacketData(bsRcvBuffer, nRcvBufferEnd, bsPacketBuffer, ref nPacketSize);


                if (nExtractRet < 0)	// Invalid Packet (No Packet Header Signiture)
                {
                    Buffer.SetByte(bsRcvBuffer, 0, 0x00);
                    nRcvBufferEnd = 0;
                    nPacketSize = 0;
                    break;
                }
                else if (nExtractRet == 0)	// Packet is Partial (Continue to Receive)
                {
                    break;
                }
                else if (nExtractRet > 0)	// Packet is in the buffer
                {
                    // Trim Extracted Packet Payload from the Recieved Data Buffer
                    nRcvBufferEnd -= nPacketSize;
                    if (nRcvBufferEnd > 0)
                    {
                        //Buffer.SetByte(bsRcvBuffer,PACKET_MAX, 0x00);
                        Buffer.BlockCopy(bsRcvBuffer, nPacketSize, bsRcvBuffer, 0, nRcvBufferEnd);
                    }
                    else
                    {
                        Buffer.SetByte(bsRcvBuffer, 0, 0x00);
                        nRcvBufferEnd = 0;
                        nPacketSize = 0;
                        break;
                    }
                }

            } while (nExtractRet > 0);
        }

        private int ExtractPacketData(byte[] bsRcvBuffer, int nRcvBufferEnd, byte[] bsPacketBuffer, ref int pExtactedSize)
        {
	        int nRemain = -1;
	        int nReadSize = 0;

            EEPacket packet = new EEPacket();

            nRemain = packet.InitPacket(bsRcvBuffer, nRcvBufferEnd, ref nReadSize);
            pExtactedSize = nReadSize;


            if (nRemain > 0)
            {
                _receiveCallback((TCPCommType)packet.wProtocolType, packet);
            }
            else
            {
                // Error : Packet Parsing
                // Console.WriteLine(rcvMsg);
                _receiveCallback(TCPCommType.TCPParsingFail, packet.ByteStream);
            }

	        return nRemain;
        }

        #region Method

        public Boolean tcpSocketOpen()
        {
            //-----------------------------------------------------
            //- Initialize TCP Signal Socket // Client
            if (_tcpSocket != null)
                _tcpSocket.tcpClientStop();

            _tcpSocket = new socketClientTCP();
            Boolean bConnect = _tcpSocket.socketClientInit(_ip, _port.ToString());
            if (bConnect)
            {
                _tcpSocket.tcpConnectedEvt += new socketClientTCP.TcpClientConnectedEventHandle(_tcpSocket_tcpConnectedEvt);
                _tcpSocket.tcpDiconnectedEvt += new socketClientTCP.TcpClientDisConnectedEventHandle(_tcpSocket_tcpDiconnectedEvt);
                _tcpSocket.tcpCltRcvEvt += new socketClientTCP.TcpClientRecievedMsgEventHandle(socketTCP_tcpCltRcvEvt);
                _tcpSocket.recieveThread();
            }

            return bConnect;
        }

        public Object SendByPassPacket(TCPCommType cmdType, Object obj)
        {
            Int32 nSize = 0;

            TCPSendPacket tcpPacket = null;
            if (obj is Byte[])
            {
                byte[] btDataArray = obj as Byte[];
                int arraySize = btDataArray.Length;
                int packetSize = btDataArray.Length;
                nSize = _tcpSocket.tcpClientSndMsg(btDataArray);

                tcpPacket = new TCPSendPacket();
                tcpPacket.PacketSize = packetSize;
                tcpPacket.SendSize = nSize;
                tcpPacket.CmdType = cmdType;
                tcpPacket.Obj = obj;
            }

            return tcpPacket;
        }

        public Object SendPacket(TCPCommType cmdType, Object obj)
        {
            Int32 nSize = 0;

            EEPacket sndPacket = new EEPacket();
            byte[] btDataArray = StructToByte(obj);
            int arraySize = btDataArray.Length;
            sndPacket.EnvelopPacket((int)cmdType, btDataArray, btDataArray.Length);
            int packetSize = sndPacket.ByteStream.Length;
            nSize = _tcpSocket.tcpClientSndMsg(sndPacket.ByteStream);

            TCPSendPacket tcpPacket = new TCPSendPacket();
            tcpPacket.PacketSize = packetSize;
            tcpPacket.SendSize = nSize;
            tcpPacket.CmdType = cmdType;
            tcpPacket.Obj = obj;

            return tcpPacket;
        }

        public Object keepAlive()
        {
            String strMsg = "";
            byte[] bStrByte = null;
            bStrByte = Encoding.UTF8.GetBytes(strMsg);
            _tcpSocket.tcpClientSndMsg(bStrByte);

            return null;
        }

        public Object SendTest(Object obj)
        {
            String strMsg = ClassToJsonString(obj);
            byte[] bStrByte = null;
            bStrByte = Encoding.UTF8.GetBytes(strMsg);
            _tcpSocket.tcpClientSndMsg(bStrByte);

            return null;
        }


        #endregion Method


        #region [Helper Functions]

        public byte[] ToByteArray(object source)
        {
            var formatter = new BinaryFormatter();
            using (var stream = new MemoryStream())
            {
                formatter.Serialize(stream, source);
                return stream.ToArray();
            }
        }

        public byte[] ObjectToByteArray(object src)
        {
            if (src == null)
                return null;

            BinaryFormatter bf = new BinaryFormatter();
            using (MemoryStream ms = new MemoryStream())
            {
                bf.Serialize(ms, src);
                return ms.ToArray();
            }
        }

        public byte[] StructToByte(object st)
        {
            if (st == null)
                return new Byte[0];

            int objsize = System.Runtime.InteropServices.Marshal.SizeOf(st);
            byte[] arr = new byte[objsize];

            IntPtr buff = System.Runtime.InteropServices.Marshal.AllocHGlobal(objsize);
            System.Runtime.InteropServices.Marshal.StructureToPtr(st, buff, false);
            System.Runtime.InteropServices.Marshal.Copy(buff, arr, 0, objsize);
            System.Runtime.InteropServices.Marshal.FreeHGlobal(buff);

            return arr;
        }

        public String ClassToJsonString<T>(T template)
        {
            if (template == null)
                return String.Empty;

            String retValue = String.Empty;

            var instance = Activator.CreateInstance<T>();
            DataContractJsonSerializer js = new DataContractJsonSerializer(instance.GetType());

            MemoryStream mStream = new MemoryStream();
            js.WriteObject(mStream, instance);
            mStream.Position = 0;

            StreamReader sReader = new StreamReader(mStream);
            retValue = sReader.ReadToEnd();
            Console.WriteLine(retValue);

            return retValue;
        }

        public T JsonStringToClass<T>(String str)
        {
            T retValue = default(T);

            var instance = Activator.CreateInstance<T>();
            using (MemoryStream ms = new MemoryStream(Encoding.Unicode.GetBytes(str)))
            {
                DataContractJsonSerializer serializer = new DataContractJsonSerializer(instance.GetType());
                retValue = (T)serializer.ReadObject(ms);
            }

            return retValue;
        }

        public Object JsonStringToObject(String str)
        {
            Object retValue = null;

            using (MemoryStream ms = new MemoryStream(Encoding.Unicode.GetBytes(str)))
            {
                DataContractJsonSerializer serializer = new DataContractJsonSerializer(retValue.GetType());
                retValue = serializer.ReadObject(ms);
            }

            return retValue;
        }

        #endregion [Helper Functions]
    }
}
