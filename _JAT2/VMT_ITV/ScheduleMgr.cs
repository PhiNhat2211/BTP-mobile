using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Threading;
using System.Collections;


namespace VMT_ITV
{
    class ScheduleData
    {
        private XmlNode m_xmlNode;
        private TimeSpan m_ts;

        public XmlNode XmlNode
        {
            set { m_xmlNode = value; }
            get { return m_xmlNode; }
        }

        public TimeSpan TS
        {
            set { m_ts = value; }
            get { return m_ts; }
        }

        public ScheduleData()
        {
            m_xmlNode = null;
            m_ts = TimeSpan.Zero;
        }

        ~ScheduleData() { }
    }

    class ScheduleMgr
    {   
        //------------------------------------
        //- Event
        public delegate void SendEventHandler(XmlNode xmlNode);
        public event SendEventHandler SendEvent;

        //------------------------------------
        //- 
        private Thread m_nThread = null;
        private bool m_bAbortThread;
        private XmlDocument m_xmlDoc = null;
        private int m_nDelayTime = 2; // 2 sec
        private bool m_bRepeat;
        private string m_strXmlPath = "";

        public ScheduleMgr()
        {
            m_bAbortThread = false;

            // Initalize Kinetic Media XML Document
            m_xmlDoc = new XmlDocument();
        }

        ~ScheduleMgr() { }        

        public bool isRunning()
        {
            if( m_nThread != null)
                return true;
            else
                return false;
        }

        public void setRepeat(bool value)
        {
            m_bRepeat = value;
        }

        public void setXmlPath(string strPath)
        {
            m_strXmlPath = strPath;
        }

        //----------------------------------------------------------
        //--
        public void start()
        {
            if (m_nThread == null)
            {
                m_bAbortThread = false;
                m_nThread = new Thread(new ThreadStart(this.ScheduleThread));
                m_nThread.Name = "ScheduleThread";
                m_nThread.Start();
            }
        }

        public void stop()
        {
            m_bAbortThread = true;
            m_nThread = null;
        }

        public void end()
        {

        }

        //----------------------------------------------------------
        //-
        // private List<ScheduleData> scheduleDataList;
        private Dictionary<long, List<ScheduleData>> scheduleDataListDic;
        // private TimeSpan m_totalDuration;
        private DateTime m_startDT = DateTime.MinValue;// 첫 InterfaceMessage 의 DateTime - 2sec 
        private void readScheduleXml(string strXmlPath, bool bFirst)
        {
            if (System.IO.File.Exists(strXmlPath) == false)
                return;

            m_xmlDoc.Load(strXmlPath);

            // Get SceneObject's
            XmlNodeList nl = m_xmlDoc.SelectNodes("//InterfaceMessage");

            // scheduleDataList = new List<ScheduleData>();
            // scheduleDataList.Clear();
            foreach (XmlNode nd in nl)
            {   
                List<ScheduleData> scheduleDataList = new List<ScheduleData>();

                // string strSceneObject = nd.OuterXml;
                string strIMDate = nd.Attributes["date"].InnerText;
                DateTime dInterFaceDate;
                try
                {   
                    dInterFaceDate = Convert.ToDateTime(strIMDate);
                }
                catch(Exception ex)
                {
                    string strError = ex.Message;
                    continue;
                }

                TimeSpan executeTs = TimeSpan.Zero;
                if (scheduleDataListDic.Count == 0) // 첫 InterfaceMessage
                {
                    m_startDT = dInterFaceDate.AddSeconds(-m_nDelayTime); // 2 sec
                }

                executeTs = dInterFaceDate.Subtract(m_startDT);

                //executeTs = new TimeSpan(
                //    dInterFaceDate.Hour - m_scheduleTs.Hours,
                //    dInterFaceDate.Minute - m_scheduleTs.Minutes,
                //    dInterFaceDate.Second - m_scheduleTs.Seconds,
                //    dInterFaceDate.Millisecond - m_scheduleTs.Milliseconds);


                if (!scheduleDataListDic.TryGetValue((long)executeTs.TotalMilliseconds, out scheduleDataList))
                {   
                    scheduleDataList = new List<ScheduleData>();
                    scheduleDataListDic.Add((long)executeTs.TotalMilliseconds, scheduleDataList);
                }

                ScheduleData scheduleData = new ScheduleData();
                scheduleData.TS = executeTs;
                scheduleData.XmlNode = nd;

                scheduleDataList.Add(scheduleData);
            }
        }

        // private bool isTestMode = true;

        private TimeSpan _SearchSpan = new TimeSpan(0, 0, 1);
        private TimeSpan _AddNSubSpan = new TimeSpan(0, 0, 0, 0, 500);
        private TimeSpan _CorrectionSpan = TimeSpan.Zero;
        private void ScheduleThread()
        {
            _CorrectionSpan = TimeSpan.Zero;

            scheduleDataListDic = new Dictionary<long, List<ScheduleData>>();
            scheduleDataListDic.Clear();
            
            //DateTime scheduleDate = DateTime.Now;
            //m_scheduleTs = new TimeSpan(
            //        scheduleDate.Hour,
            //        scheduleDate.Minute,
            //        scheduleDate.Second + 3); // 3 Sec

            readScheduleXml(m_strXmlPath, true);

            DateTime excuteTime = DateTime.Now;
            while (!m_bAbortThread)
            {   
                DateTime currentTime = DateTime.Now;

                TimeSpan playTs = currentTime.Subtract(excuteTime) + _CorrectionSpan;

                // Schedule Data Correction
                if (scheduleDataListDic.Count > 1)
                {
                    long tempKey = scheduleDataListDic.Keys.First();
                    if (playTs.Add(_SearchSpan).TotalMilliseconds < tempKey)
                    {
                        //System.Diagnostics.Trace.WriteLine(((long)playTs.TotalMilliseconds).ToString() + " + "
                        //    + ((long)_SearchSpan.TotalMilliseconds).ToString() + " < "
                        //    + tempKey.ToString());

                        _CorrectionSpan = _CorrectionSpan.Add(_AddNSubSpan);

                        //System.Diagnostics.Trace.WriteLine(((long)_CorrectionSpan.TotalMilliseconds).ToString());

                        //IEnumerator enumerator = scheduleDataListDic.Keys.GetEnumerator();
                        //enumerator.MoveNext();
                        //long nextKey = (long)enumerator.Current;
                        //System.Diagnostics.Trace.WriteLine(nextKey.ToString());
                    }
                    else if (playTs.TotalMilliseconds > tempKey)
                    {
                        _CorrectionSpan = _CorrectionSpan.Subtract(_AddNSubSpan);
                    }
                }

                List<ScheduleData> scheduleDataList;
                if (scheduleDataListDic.TryGetValue((long)playTs.TotalMilliseconds, out scheduleDataList))
                {
                    scheduleDataListDic.Remove((long)playTs.TotalMilliseconds);
                    foreach (ScheduleData scheduleData in scheduleDataList)
                    {
                        // Send Event
                        if (SendEvent != null)
                        {
                            SendEvent(scheduleData.XmlNode);
                        }
                    }
                    Thread.Sleep(5); // 0.005 Sec
                }

                if (scheduleDataListDic.Count == 0)
                    stop();
            }

            if (m_bRepeat == true)
            {
                //Mainform.sMainform.Invoke(new Action(
                //    delegate()
                //    {
                //        Mainform.sMainform.Btn_ScheduleStart_Click(null, null);
                //    }
                //));
            }
        }

    }
}
