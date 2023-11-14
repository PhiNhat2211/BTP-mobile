using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VMT_Data_JAT2;

namespace VMT_Data_JAT2.Objects
{
    public class SIEMENS
    {
        public const int MethodNo_jobAcceptedReport = 1;        

        public class Packet
        {            
            public Header header;
            public Body body = null;

            public Packet()
            {
                header = new Header();
            }

            public void Dispose()
            {
                this.header.Dispose();
                this.header = null;

                this.body.Dispose();
                this.body = null;
            }

            public void Parse(Byte[] data)
            {
                Byte[] btHeader = new Byte[80];
                Array.Clear(btHeader, 0, btHeader.Length);
                Array.Copy(data, btHeader, btHeader.Length);
                header.Parse(btHeader);                

                Byte[] btBody = new Byte[data.Length - btHeader.Length];
                Array.Clear(btBody, 0, btBody.Length);
                if (header.MethodNo == MethodNo_jobAcceptedReport)
                {
                    body = new BodyRmgStatus();
                    Array.Copy(data, btHeader.Length, btBody, 0, btBody.Length);
                    (body as BodyRmgStatus).Parse(btBody);
                }

                Array.Clear(btHeader, 0, btHeader.Length);
                btHeader = null;
                Array.Clear(btBody, 0, btBody.Length);
                btBody = null;
            }

            public class Header
            {
                public int totalLength = 0;
                public Byte[] Sync = new Byte[4]{0, 0, 0, 0};
                public int InstanceNo = 0;
                public int ClassNo = 0;
                public int MethodNo = 0;
                public String Sender = String.Empty;
                public String Receiver = String.Empty;
                public int replyFlag = 0;
                public int SequenceNo = 0;
                public String DateAndTime = String.Empty;

                public void Dispose()
                {
                    Array.Clear(this.Sync, 0, this.Sync.Length);
                    this.Sync = null;
                }

                public void Parse(Byte[] data)
                {                    
                    int idx = 0;
                    Byte[] temp = null;                    
                    
                    temp = data.Skip(0).Take(4).ToArray();
                    this.totalLength = BitConverter.ToInt32(temp, 0);
                    idx += 4;

                    if (data.Length < idx + 4)
                        return;                    
                    Array.Copy(data, idx, Sync, 0, 4);
                    idx += 4;

                    if (data.Length < idx + 12)
                        return;                    
                    temp = data.Skip(idx).Take(12).ToArray();
                    Sender = System.Text.Encoding.ASCII.GetString(temp);
                    idx += 12;

                    if (data.Length < idx + 12)
                        return;                    
                    temp = data.Skip(idx).Take(12).ToArray();
                    Receiver = System.Text.Encoding.ASCII.GetString(temp);                    
                    idx += 12;

                    if (data.Length < idx + 4)
                        return;                    
                    temp = data.Skip(idx).Take(4).ToArray();
                    this.InstanceNo = BitConverter.ToInt32(temp, 0);
                    idx += 4;

                    if (data.Length < idx + 4)
                        return;                    
                    temp = data.Skip(idx).Take(4).ToArray();
                    this.ClassNo = BitConverter.ToInt32(temp, 0);
                    idx += 4;

                    if (data.Length < idx + 4)
                        return;                    
                    temp = data.Skip(idx).Take(4).ToArray();
                    this.MethodNo = BitConverter.ToInt32(temp, 0);
                    idx += 4;

                    if (data.Length < idx + 4)
                        return;                    
                    temp = data.Skip(idx).Take(4).ToArray();
                    this.replyFlag = BitConverter.ToInt32(temp, 0);
                    idx += 4;

                    if (data.Length < idx + 4)
                        return;                    
                    temp = data.Skip(idx).Take(4).ToArray();
                    this.SequenceNo = BitConverter.ToInt32(temp, 0);
                    idx += 4;
                    
                    if (data.Length < idx + 28)
                        return;                    
                    temp = data.Skip(idx).Take(28).ToArray();
                    DateAndTime = BitConverter.ToString(temp, 0, 28);                    
                }
            }

            public class Body
            {
                public Object exData = null;

                public virtual void Dispose()
                {
                }
            }

            public class BodyRmgStatus : Body
            {
                public int twistLock = 0;
                public int weight = 0;
                public int spreaderSize = 0;
                public String actblock = String.Empty;
                public String actbay = String.Empty;
                public String actrow = String.Empty;
                public String acttier = String.Empty;
                public int trolleypos = 0;        
                public int actlane = 0;             // 1, 2 : LANE NUM, 3 : YARD
                public int hoistpos = 0;

                public override void Dispose()
                {
                    base.Dispose();
                }

                public void CopyExceptLock(BodyRmgStatus status)
                {
                    this.weight = status.weight;
                    this.spreaderSize = status.spreaderSize;
                    this.actblock = status.actblock;
                    this.actbay = status.actbay;
                    this.actrow = status.actrow;
                    this.acttier = status.acttier;
                    this.trolleypos = status.trolleypos;
                    this.actlane = status.actlane;
                    this.hoistpos = status.hoistpos;
                }

                public void CopyPosition(BodyRmgStatus status)
                {
                    this.actblock = status.actblock;
                    this.actbay = status.actbay;
                    this.actrow = status.actrow;
                    this.acttier = status.acttier;
                    this.trolleypos = status.trolleypos;                    
                    this.hoistpos = status.hoistpos;
                }

                public void Copy(BodyRmgStatus status)
                {
                    this.twistLock = status.twistLock;
                    this.CopyExceptLock(status);                    
                }

                public void Parse(Byte[] data)
                {                   
                    int idx = 0;
                    Byte[] temp = null; 
                    
                    temp = data.Skip(idx).Take(4).ToArray();
                    this.twistLock = BitConverter.ToInt32(temp, 0);
                    idx += 4;

                    if (data.Length < idx + 4)
                        return;                    
                    temp = data.Skip(idx).Take(4).ToArray();
                    this.weight = BitConverter.ToInt32(temp, 0);
                    idx += 4;

                    if (data.Length < idx + 4)
                        return;                    
                    temp = data.Skip(idx).Take(4).ToArray();
                    this.spreaderSize = BitConverter.ToInt32(temp, 0);
                    idx += 4;

                    if (data.Length < idx + 4)
                        return;
                    if (BitConverter.IsLittleEndian)
                        Array.Reverse(data, idx, 4);
                    temp = data.Skip(idx).Take(4).ToArray();
                    this.actblock = System.Text.Encoding.ASCII.GetString(temp);
                    this.actblock = this.actblock.Replace("\0", "");
                    this.actblock = this.actblock.Replace(" ", "");
                    idx += 4;

                    if (data.Length < idx + 4)
                        return;
                    if (BitConverter.IsLittleEndian)
                        Array.Reverse(data, idx, 4);
                    temp = data.Skip(idx).Take(4).ToArray();
                    var tempBay = System.Text.Encoding.ASCII.GetString(temp);                    
                    tempBay = tempBay.Replace("\0", "");
                    tempBay = tempBay.Replace(" ", "");
                    tempBay = Convert.ToString(Convert.ToUInt32(tempBay));
                    if (tempBay.Length == 1)
                        tempBay = "0" + tempBay;
                    this.actbay = tempBay;

                    idx += 4;

                    if (data.Length < idx + 4)
                        return;
                    if (BitConverter.IsLittleEndian)
                        Array.Reverse(data, idx, 4);
                    temp = data.Skip(idx).Take(4).ToArray();                    
                    this.actrow = System.Text.Encoding.ASCII.GetString(temp);
                    this.actrow = this.actrow.Replace("\0", "");
                    this.actrow = this.actrow.Replace(" ", "");
                    idx += 4;

                    if (data.Length < idx + 4)
                        return;
                    if (BitConverter.IsLittleEndian)
                        Array.Reverse(data, idx, 4);
                    temp = data.Skip(idx).Take(4).ToArray();                    
                    this.acttier = System.Text.Encoding.ASCII.GetString(temp);
                    this.acttier = this.acttier.Replace("\0", "");
                    this.acttier = this.acttier.Replace(" ", "");
                    idx += 4;

                    if (data.Length < idx + 4)
                        return;                    
                    temp = data.Skip(idx).Take(4).ToArray();
                    this.trolleypos = BitConverter.ToInt32(temp, 0);
                    idx += 4;

                    if (data.Length < idx + 4)
                        return;                    
                    temp = data.Skip(idx).Take(4).ToArray();
                    this.actlane = BitConverter.ToInt32(temp, 0);
                    idx += 4;

                    if (data.Length < idx + 4)
                        return;                    
                    temp = data.Skip(idx).Take(4).ToArray();
                    this.hoistpos = BitConverter.ToInt32(temp, 0);
                }
            }            
        }        
    }
}
