using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows.Controls;
using VMT_Data_JAT2;
using VMT_Data_JAT2.Objects;

namespace VMT_ITV
{
    class FlowText
    {
        static private FlowText m_instance = null;

        static public FlowText instance()
        {
            if (m_instance == null)
            {
                m_instance = new FlowText();
            }

            return m_instance;
        }

        // Constructor / Destructor
        public FlowText()
        {   
            m_strSTSLDSeqMachine = "";
            m_strTandemMachine = "";
            m_strKPI = "";
            m_strMessage = "";
        }
        ~FlowText() { }

        private MainWindow mMainWindow = null;
        public void Init(MainWindow mainWin)
        {
            mMainWindow = mainWin;
        }

        // private member
        private string m_strSTSLDSeqMachine;
        private string m_strTandemMachine;
        private string m_strKPI;
        private string m_strMessage;
       

        // public member
        // set
        public void ClearLDSeqMachine()
        {
            mMainWindow.MainView.FlowString.ClearLDSeqMachine();
        }

        public void AddLDSeqMachine(List<ITV.VD_ITV_PlanSeq> strMchnIDList)
        {
            mMainWindow.MainView.FlowString.AddLDSeqMachine(strMchnIDList);
        }

        public void SetTandemMachine(string strValue)
        {
            mMainWindow.MainView.FlowString.SetTandemMachine(strValue);
            // m_strTandemMachine = strValue;
        }

        public void SetKPI(string strValue)
        {
            mMainWindow.MainView.FlowString.SetKPI(strValue);
            // m_strKPI = strValue;
        }

        public void SetMessage(string strValue)
        {
            mMainWindow.MainView.FlowString.SetMessage(strValue);
            // m_strMessage = strValue;
        }

        // get
        public string GetFlowText
        {
            get 
            { 
                string strRetValue = "";
                
                if (m_strSTSLDSeqMachine != null && !m_strSTSLDSeqMachine.Equals(""))
                    strRetValue += " " + "STS LD Seq Machine : " + m_strSTSLDSeqMachine;

                if (m_strTandemMachine != null && !m_strTandemMachine.Equals(""))
                    strRetValue += " " + "Tandem Partner : " + m_strTandemMachine;

                if (m_strKPI != null && !m_strKPI.Equals(""))
                    strRetValue += " " + "KPI : " + m_strKPI;

                if (m_strMessage != null && !m_strMessage.Equals(""))
                    strRetValue += " " + "Message : " + m_strMessage;

                return strRetValue;
            }
        }
    }
}
