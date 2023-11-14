using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace TCPComm
{

    public enum EEPACKETSTATE { packet_zero, packet_continue, packet_complete, packet_invalid }

    public class EEPacket
    {
        public const byte EEHEADER_btTag=0x7e;
        public const int EEHEADER_wMark = 16692; // 0x4134

        public byte btTag;
        public int wMark;
        public byte btVerify;
        public int iDataSize;
        public int wProtocolType;
        public int wErrorCode;
        public int wReplyFlag;
        public int wReserved;

        private byte[] _byteStream;
        private IntPtr _ptrStream;


        public byte[] ByteStream
        {
            get
            {
                return _byteStream;
            }
            set
            {
                _byteStream = value;
            }

        }

        public IntPtr PtrStrem
        {
            get
            {
                return _ptrStream;
            }
            set
            {
                _ptrStream = value;
            }

        }


        public EEPacket()
        {
            this.Reset();
        }

        ~EEPacket()
        {
            this.Reset();
        }

        public void Reset()
        {
            btTag = 0;
            wMark = 0;
            btVerify = 0;
            iDataSize = 0;
            wProtocolType = 0;
            wErrorCode = 0;
            wReplyFlag = 0;
            wReserved = 0;

            // Free the data stream memory.
            if (_ptrStream != IntPtr.Zero)
            {
                Marshal.FreeHGlobal(_ptrStream);
                _ptrStream = IntPtr.Zero;
            }

            // Resize the data stream array 
            Array.Resize<byte>(ref _byteStream, 0);

        }

        public int InitPacket(byte[] btStream, int size, ref int nRead)
        {
            // Return Value ( -1:Invalid Packet, 0:Insufficent Packet Size, 1;Parsing OK )
            int nRet = -1;
            nRead = 0;

            if (size <= 0 || size <16 )
            {
                return -1;
            }

            // Check Packet Start Tag ('~')
            btTag = btStream[0];
            if (btTag == EEHEADER_btTag)
                nRead += 1;
            else
                return -1;

            // Check Packet Mark
            wMark += btStream[1];
            wMark += btStream[2] << 8;
            if (wMark == EEHEADER_wMark)
                nRead += 2;
            else
                return -1;

            // Check Packet Version
            btVerify = btStream[3];
            nRead += 1;
            //if (btVerify != 0x32)
            //    return bResult;

            // Set Packet Size
            iDataSize += btStream[4];
            iDataSize += btStream[5] << 8;
            iDataSize += btStream[6] << 16;
            iDataSize += btStream[7] << 24;
            nRead += 4;

            // Set Protocol Type
            wProtocolType += btStream[8];
            wProtocolType += btStream[9] << 8;
            nRead += 2;

            // Set Error Code
            wErrorCode += btStream[10];
            wErrorCode += btStream[11];
            nRead += 2;

            // Set Reply Flag
            wReplyFlag += btStream[12];
            wReplyFlag += btStream[13] << 8;
            nRead += 2;

            // Set Reserved Field
            wReserved += btStream[14];
            wReserved += btStream[15] << 8;
            nRead += 2;

            if (iDataSize > size)
            {
                // Insufficent Packet Size
                nRet = 0;
                return nRet;
            }
            else
                nRet = 1;
                

            //-------------------------------------
            //- Allocate Payload Data
            try
            {
                if (iDataSize > 16 && iDataSize < 1048510 * 50) // Bigger than Header Size and Less than 50MByte
                {
                    int dataSize = iDataSize - 16;
                    
                    // Allocat Data to ByteStream Array
                    Array.Resize<byte>(ref _byteStream, dataSize);
                    Buffer.BlockCopy(btStream, 16, _byteStream, 0, dataSize);
                    nRead += dataSize;

                    // Allocate Data to unmanaged Stream memory 
                    _ptrStream = Marshal.AllocHGlobal(dataSize); // Payload Data = iDataSize - HeaderSize(16)
                    Marshal.Copy(_byteStream, 0, _ptrStream, dataSize);
                }
            }
            catch(Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message);
                // Free the unmanaged memory.
                Marshal.FreeHGlobal(_ptrStream);
                _ptrStream = IntPtr.Zero;
                nRet = -1;
            }

            return nRet;
        }

        public int EnvelopPacket(int cmdType,  byte[] btData, int size)
        {   
            int packetSize = size+16;

            Array.Resize<byte>(ref _byteStream, packetSize); // EE Header Size is 16

            // Set Packet Start Tag ('~')
            ByteStream[0] = EEHEADER_btTag;

            // Set Packet Mark   (0x4134)
            byte[] intBytes = BitConverter.GetBytes((short)EEHEADER_wMark);
            //if (BitConverter.IsLittleEndian)
            //    Array.Reverse(intBytes);
            ByteStream[1] = intBytes[0];
            ByteStream[2] = intBytes[1];

            // Check Packet Version
            ByteStream[3] = 0x32;

            // Set Packet Size
            intBytes = BitConverter.GetBytes((int)packetSize);
            //if (BitConverter.IsLittleEndian)
            //    Array.Reverse(intBytes);
            ByteStream[4] = intBytes[0];
            ByteStream[5] = intBytes[1];
            ByteStream[6] = intBytes[2];
            ByteStream[7] = intBytes[3];

            // Set Protocol Type
            intBytes = BitConverter.GetBytes((short)cmdType);
            //if (BitConverter.IsLittleEndian)
            //    Array.Reverse(intBytes);
            ByteStream[8] = intBytes[0];
            ByteStream[9] = intBytes[1];

            // Set Error Code
            ByteStream[10] = 0x00;
            ByteStream[11] = 0x00;

            // Set Reply Flag
            ByteStream[12] = 0x00;
            ByteStream[13] = 0x00;

            // Set Reserved Field
            ByteStream[14] = 0x00;
            ByteStream[15] = 0x00;

            //-------------------------------------
            //- Set Data Payload
            Buffer.BlockCopy(btData, 0, ByteStream, 16, size);

            return packetSize;
        }

        public void ReleasePacket()
        {
            this.Reset();
        }

    }
}
