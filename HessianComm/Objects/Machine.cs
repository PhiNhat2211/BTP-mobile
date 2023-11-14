using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HessianComm.Objects
{    
    public class Machine
    {
        public string mchnId { get; set; } /* "Machine Code
                                              e.g) TT501, RS202,QC103" */
        public string mchnTp { get; set; }
        // public bool isTwistLock { get; set; }
        // public bool isTwistLockLS { get; set; }
        // public bool isTwistLockWS { get; set; }
        public string externalId { get; set; }
        public string chssNo { get; set; }

        public bool isLogOn { get; set; }

        public bool isOn { get; set; } /* "Status (On/Off)
                                              e.g) True: On, False: Off" */
        // public bool isLightOn { get; set; }
        // public bool isActive { get; set; }
        // public bool isAbortJob { get; set; }
        public string mchnSts { get; set; }

        public string vrtlFlg { get; set; }
        // public int xCoordinate { get; set; }
        // public int yCoordinate { get; set; }
        // public string heading { get; set; }
        // public string curNode { get; set; }

        public Spreader sprd { get; set; }
        // public int sprdTpWs { get; set; }
        // public int sprdTpLs { get; set; }
        // public int sprdMd { get; set; }
        // public int distbTp { get; set; }
        // public int autoSts { get; set; }
        // public int oprMd { get; set; }
        // public int waitTp { get; set; }
        // public string alignmentFlg { get; set; }

        //// Field descriptor #40 Lcom/clt/tos/external/model/machine/LandingInfo;
        // public LandingInfo landing { get; set; }

        public int movedDistance { get; set; }

        public int moveTime { get; set; }

        //// Field descriptor #16 Z
        // public bool isAccpt { get; set; }

        // public bool is1stPart { get; set; }

        public string noticeMsg { get; set; }

        public List<string> loginUsrLst = null;

        public bool isTwistLock { get; set; }

        public string autoFlg { get; set; }

        public bool useRemark { get; set; } /* "true: Use Remark
                                                  false: Not Use Remark" */
        public string remark { get; set; } /* "e.g.)
                                               : useRemark = true
                                               remark = 'samplesamplesample, …..'"*/
        public void Dispose()
        {
            if (this.loginUsrLst != null)
            {
                this.loginUsrLst.Clear();
                this.loginUsrLst = null;
            }
        }
    }
}
