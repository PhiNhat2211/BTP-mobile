using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HessianComm.Objects
{
    public class User
    {
        public string usrId { get; set; }

        public string usrPasswd { get; set; }

        public string usrNm { get; set; }

        public ArrayList usrGrp { get; set; }

        public string mchnTp { get; set; }
        // public string usrSts { get; set; }
        // public bool isUsrLogin { get; set; }
        // public string religion { get; set; }
        // public string mobile { get; set; }

        public User()
        {
            this.usrGrp = null;
        }
    }
}
