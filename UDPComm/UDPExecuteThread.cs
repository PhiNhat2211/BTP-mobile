using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Collections;

namespace UDPComm
{
    public class UDPExecuteThread
    {
        // Thread Queue
        private class QueueData
        {
            public UDPCommType type { get; private set; }
            public Object obj { get; private set; }

            public QueueData(UDPCommType type, Object obj)
            {
                this.type = type;
                this.obj = obj;
            }
        }

        Queue<QueueData> _queue = new Queue<QueueData>();
        AutoResetEvent _queueEvent = new AutoResetEvent(false);
        AutoResetEvent _quitEvent = new AutoResetEvent(false);
        // WaitHandle[] _events = new WaitHandle[] { new AutoResetEvent(false), new AutoResetEvent(false) };
        WaitHandle[] _events = null;

        private UDPAPI.UDPCommCallback _sendCallback = null;
        public UDPAPI.UDPCommCallback sendCallback { set { _sendCallback = value; } }

        private UDPAPI.UDPCommCallback _receiveCallback = null;
        public UDPAPI.UDPCommCallback receiveCallback { set { _receiveCallback = value; } }

        private Thread _thread = null;

        private UDPAPI _parent = null;

        public UDPExecuteThread(UDPAPI parent)
        {
            _parent = parent;
            _thread = new Thread(this.ThreadFunc);
            _events = new WaitHandle[] { _quitEvent, _queueEvent };
        }

        public void Start()
        {
            _thread.Start();
        }

        public void End()
        {
            _quitEvent.Set();
        }

        public void StartPolling(UDPCommType type, Object obj, int timeSpan)
        {
            switch (type)
            {
                //case UDPCommType.Timer:
                //    {
                //        if (_timer == null)
                //            _timer = new Timer(TimerCallback);

                //        _timer.Change(0, timeSpan);
                //    }
                //    break;
                /*
                ....
                             
                             
                             
                */
            }
        }

        public void StopPolling(UDPCommType type)
        {
            switch (type)
            {
                //case UDPCommType.Timer:
                //    if (_timer != null)
                //    {
                //        _timer.Dispose();
                //        _timer = null;
                //    }
                //    break;
                /*
                ....
                             
                             
                             
                */
            }
        }

        public void Do(UDPCommType type, Object obj)
        {
            lock (((ICollection)_queue).SyncRoot)
            {
                bool bEnqueue = true;

                //if (type == UDPCommType.VMT_TU_RTG_PDS_PickDropConfirm ||
                //    type == UDPCommType.VMT_TU_RSEH_PDS_PickDropConfirm)
                //{ }
                //else
                //{
                    for (int i = 0; i < _queue.Count; i++)
                    {
                        if (_queue.ElementAt(i).type == type)
                        {
                            bEnqueue = false;
                            break;
                        }
                    }
                //}

                if (bEnqueue)
                {
                    //if (obj != null && obj is EEParentClass)
                    //    (obj as EEParentClass).RelocateByteArray();

                    _queue.Enqueue(new QueueData(type, obj));
                }
            }

            _queueEvent.Set(); // __queuetimer
        }

        private void ThreadFunc(Object obj)
        {
            try
            {
                int ret = 0;
                QueueData data = null;
                while (true)
                {
                    ret = WaitHandle.WaitAny(_events, -1); // 0.2 sec handle get waiting

                    if (ret == 0)    // QuitEvent
                    {
                        break;
                    }
                    else if (ret == 1) // QueueEvent
                    {

                        lock (((ICollection)_queue).SyncRoot)
                        {
                            if (_queue.Count == 0)
                                continue;

                            data = _queue.Dequeue();

                            if (_queue.Count != 0)
                                _queueEvent.Set();
                        }

                        try
                        {
                            switch (data.type)
                            {
                                case UDPCommType.UDPSocketOpen:
                                    _sendCallback(data.type, _parent.UDPControl.udpSocketOpen());
                                    break;
                                case UDPCommType.Send_CFG_RST:
                                    case UDPCommType.Send_PUBX_05:
                                    case UDPCommType.Send_PUBX_06:
                                    case UDPCommType.Send_HighBackward:
                                    case UDPCommType.Send_HighForward:
                                    case UDPCommType.Send_CFG_SAVE:
                                    case UDPCommType.Request_CFG_EKF:
                                    _sendCallback(data.type, _parent.UDPControl.SendPacket(data.type, data.obj));
                                    break;
                            }
                        }
                        catch (Exception ex)
                        {
                            UDPException hex = new UDPException(ex.Message, ex.InnerException);
                            hex.udpCommType = data.type;
                            
                            foreach (UDPAPI.ExceptionDelegate ed in UDPAPI.ExceptionDelegatorList)
                            {
                                ed(hex);
                            }

                            if (UDPAPI.ExceptionDelegatorList.Count == 0)
                                throw ex;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                foreach (UDPAPI.ExceptionDelegate ed in UDPAPI.ExceptionDelegatorList)
                {
                    ed(ex);
                }

                if (UDPAPI.ExceptionDelegatorList.Count == 0)
                    throw ex;
            }
        }

    }
}
