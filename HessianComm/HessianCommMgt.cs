using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using HessianComm.Interface;
using HessianComm.Objects;
using HessianCSharp.client;
using HessianCSharp.Class;

namespace HessianComm
{
    public enum HessianCommMgtType
    {
        getSwapList,
        setEmptySwap,
        ExceptionOccured,
    }

    public partial class HessianMgtException : Exception
    {
        public HessianMgtException()
        {
        }

        public HessianMgtException(string message)
            : base(message)
        {
        }

        public HessianMgtException(string message, Exception inner)
            : base(message, inner)
        {
        }

        public HessianCommMgtType hessianCommMgtType { get; set; }
    }

    public class HessianMgtAPI
    {   
        #region variables
        private const string _hessian_VmtSwapService = @"/HESSIAN/HESSIAN_IfVmtSwapService";

        private static HessianMgtAPI _instance = new HessianMgtAPI();

        private string _ip = @"116.127.223.206";
        private int _port = 7110;        

        private VmtSwapService _vmtSwapService = null;

        private CHessianProxyFactory factory = new CHessianProxyFactory();

        private HessianMgtExecuteThread _thread = null;
        #endregion

        // Exception Throw object for External Module
        public delegate void ExceptionDelegate(Exception ex);

        public delegate void HessianCommCallback(HessianCommMgtType type, object obj);

        public static List<ExceptionDelegate> ExceptionDelegatorList = new List<ExceptionDelegate>();

        public static void AddExceptionDelegator(ExceptionDelegate ed)
        {
            ExceptionDelegatorList.Add(ed);
        }

        public static void RemoveExceptionDelegator(ExceptionDelegate ed)
        {
            ExceptionDelegatorList.Remove(ed);
        }

        #region properties

        public VmtSwapService VmtSwapService
        {
            get
            {
                if (this._vmtSwapService == null)
                {
                    this._vmtSwapService = (VmtSwapService)this.factory.Create<VmtSwapService>(typeof(VmtSwapService), "http://" + this._ip + ":" + this._port + _hessian_VmtSwapService);
                }

                return this._vmtSwapService;
            }
        }

        #endregion

        private HessianMgtAPI()
        {
            this._thread = new HessianMgtExecuteThread(this);
        }

        public static void Init(string ip, int port, HessianCommCallback callback)
        {
            _instance._ip = ip;
            _instance._port = port;
            _instance._thread.Callback = callback;
        }

        public static void Start()
        {
            _instance._thread.Start();
        }

        public static void End()
        {
            _instance._thread.End();
        }

        public static void getSwapList(HessianList list)
        {
            _instance._thread.Do(HessianCommMgtType.getSwapList, list);
        }

        public static void setEmptySwap(HessianList list)
        {
            _instance._thread.Do(HessianCommMgtType.setEmptySwap, list);
        }
        //////////////////////////////////////////////////
    }
}
