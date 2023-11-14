using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common.Util
{
    public class SingleShot
    {
        public delegate void SingleShotCallBack();

        private SingleShotCallBack m_callBackFunction;

        public SingleShot(int intervalMilliseconds, SingleShotCallBack CallBackFunction)
        {
            System.Timers.Timer m_timer = new System.Timers.Timer(intervalMilliseconds);
            this.m_callBackFunction = CallBackFunction;
            m_timer.Elapsed += new System.Timers.ElapsedEventHandler(this.TimerElapsedFunc);            
            m_timer.AutoReset = false;
            m_timer.Start();
        }

        ~SingleShot() 
        { 
        }

        public void TimerElapsedFunc(object sender, System.Timers.ElapsedEventArgs e)
        {
            this.m_callBackFunction();
        }
    }

    public class TCPCommandSingleShot
    {
        public delegate void TCPCommandSingleShotCallback(int tpcCommType, object value);

        private TCPCommandSingleShotCallback _callBackFunction;
        private int _tpcCommType;
        private object _value;

        public TCPCommandSingleShot(TCPCommandSingleShotCallback CallBackFunction, int tpcCommType, object value, int intervalMilliseconds)
        {
            System.Timers.Timer m_timer = new System.Timers.Timer(intervalMilliseconds);
            this._callBackFunction = CallBackFunction;
            this._tpcCommType = tpcCommType;
            this._value = value;
            m_timer.Elapsed += new System.Timers.ElapsedEventHandler(this.TimerElapsedFunc);
            m_timer.AutoReset = false;
            m_timer.Start();
        }

        ~TCPCommandSingleShot() 
        {
        }

        public void TimerElapsedFunc(object sender, System.Timers.ElapsedEventArgs e)
        {
            this._callBackFunction(this._tpcCommType, this._value);
        }
    }
}
