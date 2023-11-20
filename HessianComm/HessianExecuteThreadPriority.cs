using Common.Util;
using HessianComm.Interface;
using HessianCSharp.client;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using static HessianComm.HessianExecuteThread;

namespace HessianComm
{
    public class HessianExecuteThreadPriority
    {
        private Queue<QueueData> _queue = new Queue<QueueData>();
        private AutoResetEvent _queueEvent = new AutoResetEvent(false);
        private AutoResetEvent _quitEvent = new AutoResetEvent(false);
        private WaitHandle[] _events = null;
        private Thread _threadPriority = null;
        private HessianAPI _parent = null;
        private Timer _timerKeepAlive = null;
        private int _IntervalKeepAliveTimer = 0;
        private HessianAPI.HessianCommCallback _callback = null;
        public HessianAPI.HessianCommCallback Callback
        {
            set { this._callback = value; }
        }

        public HessianExecuteThreadPriority(HessianAPI parent)
        {
            this._parent = parent;
            this._threadPriority = new Thread(this.ThreadFuncPriority);
            this._events = new WaitHandle[] { this._quitEvent, this._queueEvent };
        }
        public void Start()
        {
            this._threadPriority.Start();
        }
        public void End()
        {
            this._quitEvent.Set();
        }
        public void KeepAliveTimerCallback(object state)
        {
            this.Do(HessianCommType.KeepAlive, null);
        }
        public void StartPolling(HessianCommType type, object obj, int timeSpan)
        {
            switch (type)
            {
                case HessianCommType.KeepAlive:
                    {
                        if (this._timerKeepAlive == null)
                        {
                            this._timerKeepAlive = new Timer(this.KeepAliveTimerCallback);
                        }

                        this._timerKeepAlive.Change(0, timeSpan);
                        this._IntervalKeepAliveTimer = timeSpan;
                    }
                    break;              
            }
        }

        public void StopPolling(HessianCommType type)
        {
            switch (type)
            {
                case HessianCommType.KeepAlive:
                    if (this._timerKeepAlive != null)
                    {
                        this._timerKeepAlive.Dispose();
                        this._timerKeepAlive = null;
                    }
                    break;              
            }
        }
        public void Do(HessianCommType type, object obj)
        {
            lock (((ICollection)this._queue).SyncRoot)
            {
                bool bEnqueue = true;

                if (type == HessianCommType.KeepAlive &&
                    this._timerKeepAlive != null)
                {
                    this._timerKeepAlive.Change(this._IntervalKeepAliveTimer, this._IntervalKeepAliveTimer);
                }

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
        private void ThreadFuncPriority(object obj)
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
                      
                        try
                        {
                            // Aug 09 2023 HandleLog
                            this._callback(data.type, obj, true);

                            switch (data.type)
                            {
                                case HessianCommType.KeepAlive:
                                    this._callback(HessianCommType.KeepAlive, this._parent.SystemControl.keepAlive());
                                    break;
                                
                            }
                            data.obj = null;
                        }
                        catch (Exception ex)
                        {
                            this._callback(HessianCommType.ExceptionOccured, ex.Message + "@" + data.type);

                            HessianException hex = new HessianException(ex.Message, ex.InnerException);
                            hex.hessianCommType = data.type;

                            Logger.Log("HessianException Type : " + hex.hessianCommType.ToString());
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
