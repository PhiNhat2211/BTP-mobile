using HessianCSharp.Class;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HessianComm
{   
    public interface IfVmtUserControl
    {
        List<string> getUserAccessRole(string usrId);

        object getUserAccessRole(Hashtable hashUser); // Connect

        // object getLoginInfo4Machine(Hashtable param); // Connect
        object setLogin4Machine(Hashtable param_Login); // DriverInfo, Login

        object changeDriverCheck(Hashtable hashLogin);

        object setLogout4Machine(Hashtable hashLogout);

        object getLogin4MachineList(Hashtable hashLogout);

        object changeDriver(HessianList list);

        object getMachineAccessAction(HessianList accessInfo);

        object setVMTMachineStatus(HessianList list);
    }
}
