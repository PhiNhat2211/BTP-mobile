using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using Common.Util;
using HessianCSharp.Class;

namespace HessianComm
{
    public class HessianMgtExecuteThread
    {
        // Thread Queue
        private class QueueData
        {
            public HessianCommMgtType type { get; private set; }

            public object obj { get; set; }

            public QueueData(HessianCommMgtType type, object obj)
            {
                this.type = type;
                this.obj = obj;
            }
        }

        private Queue<QueueData> _queue = new Queue<QueueData>();
        private AutoResetEvent _queueEvent = new AutoResetEvent(false);
        private AutoResetEvent _quitEvent = new AutoResetEvent(false);
        // WaitHandle[] _events = new WaitHandle[] { new AutoResetEvent(false), new AutoResetEvent(false) };
        private WaitHandle[] _events = null;
        private Thread _thread = null;
        private HessianMgtAPI _parent = null;

        private Timer _timerKeepAlive = null;
        private Timer _timerKeepAliveStandAlone = null;
        private Timer _timerDGPSAlive = null;
        private Timer _timerPinningStation = null;
        private Timer _timerJobOrderList = null;
        private Timer _timerMachineNotice = null;
        private Timer _timerPrecedingYtList = null;
        private Timer _timerCheckMachineLogout = null;
        private Timer _timerCheckMachineStop = null;

        private int _IntervalJobOrderListTimer = 0;
        private int _IntervalKeepAliveTimer = 0;
        private int _IntervalGetMachineStopTimer = 0;

        private HessianMgtAPI.HessianCommCallback _callback = null;

        public HessianMgtAPI.HessianCommCallback Callback
        {
            set { this._callback = value; }
        }        

        public HessianMgtExecuteThread(HessianMgtAPI parent)
        {
            this._parent = parent;
            this._thread = new Thread(this.ThreadFunc);
            this._events = new WaitHandle[] { this._quitEvent, this._queueEvent };
        }

        public void Start()
        {
            this._thread.Start();            
        }

        public void End()
        {
            this._quitEvent.Set();            
        }

        public void Do(HessianCommMgtType type, object obj)
        {
            lock (((ICollection)this._queue).SyncRoot)
            {
                bool bEnqueue = true;
               
                for (int i = 0; i < this._queue.Count; i++)
                {
                    if (this._queue.ElementAt(i).type == type)
                    {
                        this._queue.ElementAt(i).obj = obj;
                        bEnqueue = false;
                        break;
                    }
                }
                

                if (bEnqueue)
                {
                    this._queue.Enqueue(new QueueData(type, obj));
                }
            }

            this._queueEvent.Set(); // __queuetimer
        }

        private void HessianToLog(Hashtable hashTable)
        {
            Dictionary<string, string> keyPairs = new Dictionary<string, string>();
            foreach (KeyValuePair<string, string> items in hashTable)
            {
                keyPairs.Add(items.Key, items.Value);
                // Logging.Instance.WriteInformation(string.Format("Key: {0} \t Value: {1}", items.Key, items.Value));
                // Logger.Log("Do Execute : " + data.type.ToString());
            }
        }

        // private DateTime totalDateTime = DateTime.Now;
        private void ThreadFunc(object obj)
        {
            try
            {
                int ret = 0;
                QueueData data = null;
                while (true)
                {
                    ret = WaitHandle.WaitAny(this._events, -1); // 0.2 sec handle get waiting
                                        
                    if (ret == 0)
                    {
                        // QuitEvent
                        break;
                    }
                    else if (ret == 1)
                    {
                        // QueueEvent

                        lock (((ICollection)this._queue).SyncRoot)
                        {
                            if (this._queue.Count == 0)
                            {
                                continue;
                            }

                            data = this._queue.Dequeue();

                            if (this._queue.Count != 0)
                            {
                                this._queueEvent.Set();
                            }
                        }

                        DateTime typeDateTime = DateTime.Now;
                        TimeSpan tSpan = DateTime.Now - typeDateTime;

                        try
                        {
                            // Logger.Log("Do Execute : " + data.type.ToString());

                            System.Diagnostics.Trace.WriteLine("[VMT Timestamp]");
                            System.Diagnostics.Trace.WriteLine("[VMT Timestamp]" + DateTime.Now.ToString("[HH:mm:ss:fff]") + data.type.ToString() + "(+)");

                            Common.Util.Logger.Log("[VMT Timestamp]");
                            Common.Util.Logger.Log("[VMT Timestamp]" + DateTime.Now.ToString("[HH:mm:ss:fff]") + data.type.ToString() + "(+)");

                            switch (data.type)
                            {
                                case HessianCommMgtType.getSwapList:
                                    this._callback(HessianCommMgtType.getSwapList, this._parent.VmtSwapService.getSwapList((HessianList)data.obj));
                                    break;
                                case HessianCommMgtType.setEmptySwap:
                                    this._callback(HessianCommMgtType.setEmptySwap, this._parent.VmtSwapService.setEmptySwap((HessianList)data.obj));
                                    break;
                                /////////////////////////
                            }

                            System.Diagnostics.Trace.WriteLine("[VMT Timestamp]" + DateTime.Now.ToString("[HH:mm:ss:fff]") + data.type.ToString() + "(-)");
                            Common.Util.Logger.Log("[VMT Timestamp]" + DateTime.Now.ToString("[HH:mm:ss:fff]") + data.type.ToString() + "(-)");

                            tSpan = DateTime.Now - typeDateTime;

                            if (tSpan.TotalMilliseconds >= 1000.0)
                            {
                                System.Diagnostics.Trace.WriteLine("[VMT Timestamp]" + "Excute DateTime TotalMilliseconds : " + tSpan.TotalMilliseconds.ToString() + " Response Slow");
                                Common.Util.Logger.Log("[VMT Timestamp]" + "Excute DateTime TotalMilliseconds : " + tSpan.TotalMilliseconds.ToString() + " Response Slow");
                            }
                            else
                            {
                                System.Diagnostics.Trace.WriteLine("[VMT Timestamp]" + "Excute DateTime TotalMilliseconds : " + tSpan.TotalMilliseconds.ToString());
                                Common.Util.Logger.Log("[VMT Timestamp]" + "Excute DateTime TotalMilliseconds : " + tSpan.TotalMilliseconds.ToString());
                            }

                            data.obj = null;
                        }
                        catch (Exception ex)
                        {
                            this._callback(HessianCommMgtType.ExceptionOccured, ex.Message);

                            HessianMgtException hex = new HessianMgtException(ex.Message, ex.InnerException);
                            hex.hessianCommMgtType = data.type;

                            Logger.Log("HessianException Type : " + hex.hessianCommMgtType.ToString());
                            Logger.Log("HessianException Message : " + hex.Message);

                            // tSpan = DateTime.Now - typeDateTime;
                            // Common.Util.Logger.Log("[VMT Timestamp]" + "Excute DateTime TotalMilliseconds : " + tSpan.TotalMilliseconds.ToString());

                            if (ex.InnerException is WebException)
                            {
                                WebException wE = ex.InnerException as WebException;

                                switch (wE.Status)
                                {
                                    case WebExceptionStatus.ConnectFailure:
                                    case WebExceptionStatus.Timeout:
                                        {
                                            Logger.Log("HessianException Status : " + wE.Status.ToString());
                                        }
                                        break;
                                    default:
                                        {
                                            Logger.Log("HessianException Status : " + wE.Status.ToString());
                                        }
                                        break;
                                }
                            }

                            foreach (HessianAPI.ExceptionDelegate ed in HessianAPI.ExceptionDelegatorList)
                            {
                                ed(hex);
                            }

                            if (HessianAPI.ExceptionDelegatorList.Count == 0)
                            {
                                throw ex;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log("Exception Message : " + ex.Message);

                foreach (HessianAPI.ExceptionDelegate ed in HessianAPI.ExceptionDelegatorList)
                {
                    ed(ex);
                }

                if (HessianAPI.ExceptionDelegatorList.Count == 0)
                {
                    throw ex;
                }
            }
        }
    }
}
