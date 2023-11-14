using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.IO;
using System.Xml;
using VMT_Data_JAT2;
//using ExternalAPI;

namespace VMT_RMG
{
    class InterfaceMessageLoader
    {
        static private InterfaceMessageLoader m_instance = null;

        static public InterfaceMessageLoader instance()
        {
            if (m_instance == null)
            {
                m_instance = new InterfaceMessageLoader();
            }

            return m_instance;
        }

        public bool isLoaded()
        {
            if (m_RoadXmlDoc != null)
                return true;
            else
                return false;
        }

        private string m_strXmlPath = "";

        private XmlDocument m_RoadXmlDoc = null;
        private XmlNodeList m_InterfaceMessageList = null;
        private int currentMessageIndex;

        private XmlDocument m_SaveXmlDoc = null;
        private XmlNode m_SaveXmlRootND = null;
        private bool m_bInit = false;

        public InterfaceMessageLoader()
        {
            m_RoadXmlDoc = new XmlDocument();
            m_SaveXmlDoc = new XmlDocument();

            currentMessageIndex = 0;
        }

        ~InterfaceMessageLoader() { }

        public void LoadInterfaceMessageXml(string strXmlPath)
        {
            m_RoadXmlDoc.Load(strXmlPath);
            
            m_InterfaceMessageList = m_RoadXmlDoc.SelectNodes("//InterfaceMessageList/InterfaceMessage");

            // init
            currentMessageIndex = 0;
        }

        public void InitSaveInterfaceMessageXml()
        {
            string sRootPath = App.GetAppDirectory();
            string sDirPath = sRootPath + @"{0}\Log";
            if (Directory.Exists(sDirPath) == false)
            {
                Directory.CreateDirectory(sDirPath);
            }

            m_strXmlPath = @sDirPath + @"\RMG_InterfaceMessag_LOG_"
                + System.DateTime.Now.Year + "." + System.DateTime.Now.Month + "." + System.DateTime.Now.Day
                + "-" + System.DateTime.Now.Hour + ".xml";

            
            // Save the document to a file and auto-indent the output.
            XmlTextWriter xmlWriter = new XmlTextWriter(m_strXmlPath, null);
            xmlWriter.Formatting = Formatting.Indented;
            m_SaveXmlDoc.Save(xmlWriter);
            xmlWriter.Flush();
            xmlWriter.Close();

            
            XmlDeclaration xmldecl = m_SaveXmlDoc.CreateXmlDeclaration("1.0", "UTF-8", null);
            m_SaveXmlDoc.AppendChild(xmldecl);

            m_SaveXmlRootND = m_SaveXmlDoc.CreateNode(XmlNodeType.Element, "InterfaceMessageList", "");

            if (m_SaveXmlRootND != null)
            {
                m_SaveXmlDoc.AppendChild(m_SaveXmlRootND);

                XmlNode interfaceMessageStartNode = m_SaveXmlDoc.CreateNode(XmlNodeType.Element, "InterfaceMessage", "");
                interfaceMessageStartNode.InnerText = "InterfaceMessage Start";
                m_SaveXmlRootND.AppendChild(interfaceMessageStartNode);
            }
            // m_SaveXmlDoc.InnerText = "InnerText";
            m_SaveXmlDoc.Save(m_strXmlPath);
        }

        //-------------------------------------
        //- InterfaceMessage Public Method
        private ScheduleMgr scheduleMgr = null;
        public void InterfaceMessageSchedule_Load(String strXmlPath)
        {
            scheduleMgr = new ScheduleMgr();
            scheduleMgr.SendEvent += new ScheduleMgr.SendEventHandler(scheduleMgr_SendEvent);
            // scheduleMgr.SendOpenEvent += new ScheduleMgr.SendOpenEventHandler(scheduleMgr_SendOpenEvent);
            // scheduleMgr.SendPlayEvent += new ScheduleMgr.SendPlayEventHandler(scheduleMgr_SendPlayEvent);

            if (scheduleMgr != null && !scheduleMgr.isRunning())
            {
                scheduleMgr.setRepeat(false);
                scheduleMgr.setXmlPath(strXmlPath);
            }
        }

        private void scheduleMgr_SendEvent(XmlNode xmlNode)
        {
            ExcuteInterfaceMessage_ByXmlNode(xmlNode);
            // throw new NotImplementedException();
        }

        public void InterfaceMessageSchedule_Play()
        {
            if (scheduleMgr != null)
                scheduleMgr.start();
        }

        public void InterfaceMessageSchedule_Pause()
        {

        }

        public void InterfaceMessageSchedule_Resume()
        {

        }

        public void InterfaceMessageSchedule_Stop()
        {
            if (scheduleMgr != null && scheduleMgr.isRunning())
            {
                scheduleMgr.setRepeat(false);
                scheduleMgr.stop();
            }
        }

        public void ExcuteInterfaceMessage()
        {

            try
            {
                if (m_InterfaceMessageList.Count > currentMessageIndex)
                {
                    XmlNode interfaceMessage = m_InterfaceMessageList.Item(currentMessageIndex);
                    ExcuteInterfaceMessage_ByXmlNode(interfaceMessage);
                }
            }
            catch (Exception ex)
            {
                string strError = ex.Message;
            }
        }

        public void ExcuteInterfaceMessage_ByXmlNode(XmlNode interfaceMessage)
        {
            String strParameterName = getInnerText(interfaceMessage.Attributes["parameter"]);
            String strFunctionName = getInnerText(interfaceMessage.Attributes["function"]);

            if (strFunctionName.Equals("ProcessByJobOrderCallback") &&
                strParameterName.Equals("EEv2JobOrder")) // JobOrder
            {
                Exec_ProcessByJobOrderCallback(interfaceMessage);
            }
            else if (strFunctionName.Equals("ProcessByJobDeleteCallback") &&
                strParameterName.Equals("EEv2JobOrder")) // JobDelete
            {
                Exec_ProcessByJobDeleteCallback(interfaceMessage);
            }
            else if (strFunctionName.Equals("ProcessByJobDeleteAllCallback") &&
                strParameterName.Equals("int")) // JobDeleteAll
            {
                Exec_ProcessByJobDeleteAllCallback(interfaceMessage);
            }
            /* // Add Interface Name
            else if (strFunctionName.Equals("ProcessByJobOrderITVCallback") &&
                     strParameterName.Equals("EEv2JobOrderForITV"))
            {

            }
            */
            else
            {
                throw new Exception("not define");
            }
        }

        public void NextInterfaceMessage()
        {
            currentMessageIndex++;

            if (currentMessageIndex >= m_InterfaceMessageList.Count)
            {
                currentMessageIndex = m_InterfaceMessageList.Count - 1;
            }

            ExcuteInterfaceMessage();
        }

        public void PrevInterfaceMessage()
        {
            currentMessageIndex--;

            if (currentMessageIndex < 0)
                currentMessageIndex = 0;

            ExcuteInterfaceMessage();
        }

        #region [Write InterFace Message Function]

        public void WriteInterfaceMessage<T>(string strFunctionName, T obj)
        {
            if (App.MESSAGE_CAPTURE_MODE != true)
                return;
            
            try
            {
                if (m_bInit == false)
                {
                    m_bInit = true;
                    InitSaveInterfaceMessageXml();
                }

                // Export Packet Object Class to XML Document
                if (m_SaveXmlDoc != null &&
                    obj != null)
                {
                    //Util.ExportMessageObjToXml<T>(m_SaveXmlDoc, strFunctionName, obj);
                    //m_SaveXmlDoc.Save(m_strXmlPath);
                    VMT_Data_JAT2.Util.ExportMessageObjToXml_Refactoring<T>(m_strXmlPath, strFunctionName, obj);
                }
                else
                {
                    throw new Exception("Invalid Data Type");
                }
            }
            catch (Exception ex)
            {
                string strError = ex.Message;
            }
        }

        #endregion [Write InterFace Message Function]

        #region [Execute InterFace Message Function]
        //-------------------------------------
        //- InterfaceMessage Detail Function
        private void Exec_ProcessByJobOrderCallback(XmlNode interfaceMessage)
        {
            try
            {
                XmlNode WorkingMchnND = interfaceMessage.SelectSingleNode("workingMchn");
                String working_mchnId = WorkingMchnND.SelectSingleNode("mchnId").InnerText;
                String working_mchnTp = WorkingMchnND.SelectSingleNode("mchnTp").InnerText;
                String working_mchnSts = WorkingMchnND.SelectSingleNode("mchnSts").InnerText;
                String working_vrtlFlg = WorkingMchnND.SelectSingleNode("vrtlFlg").InnerText;

                XmlNode PartnerMchnND = interfaceMessage.SelectSingleNode("partnerMchn");
                String partner_mchnId = PartnerMchnND.SelectSingleNode("mchnId").InnerText;
                String partner_mchnTp = PartnerMchnND.SelectSingleNode("mchnTp").InnerText;
                String partner_mchnSts = PartnerMchnND.SelectSingleNode("mchnSts").InnerText;
                String partner_vrtlFlg = PartnerMchnND.SelectSingleNode("vrtlFlg").InnerText;

                XmlNode CntrND = interfaceMessage.SelectSingleNode("cntr");
                String cntrNo = CntrND.SelectSingleNode("cntrNo").InnerText;
                String cntrIso = CntrND.SelectSingleNode("cntrIso").InnerText;
                String cntrTp = CntrND.SelectSingleNode("cntrTp").InnerText;
                String cls = CntrND.SelectSingleNode("cls").InnerText;
                String opr = CntrND.SelectSingleNode("opr").InnerText;
                String cntrCgoTp = CntrND.SelectSingleNode("cntrCgoTp").InnerText;
                String fullMty = CntrND.SelectSingleNode("fullMty").InnerText;

                XmlNode LocND = interfaceMessage.SelectSingleNode("loc");
                String locTp = LocND.SelectSingleNode("locTp").InnerText;
                String blck = LocND.SelectSingleNode("blck").InnerText;
                String bay = LocND.SelectSingleNode("bay").InnerText;
                String row = LocND.SelectSingleNode("row").InnerText;
                String tier = LocND.SelectSingleNode("tier").InnerText;
                String lane = LocND.SelectSingleNode("lane").InnerText;
                String location = LocND.SelectSingleNode("location").InnerText;

                XmlNode TypeND = interfaceMessage.SelectSingleNode("type");
                String jobTp = TypeND.SelectSingleNode("jobTp").InnerText;
                String jobStatus = TypeND.SelectSingleNode("jobStatus").InnerText;
                String vslCd = TypeND.SelectSingleNode("vslCd").InnerText;
                String voyNo = TypeND.SelectSingleNode("voyNo").InnerText;
                String planSeq = TypeND.SelectSingleNode("planSeq").InnerText;
                String twinTandemFlg = TypeND.SelectSingleNode("twinTandemFlg").InnerText;
                String twinTandumKey = TypeND.SelectSingleNode("twinTandumKey").InnerText;
                String tandemJoinYT = TypeND.SelectSingleNode("tandemJoinYT").InnerText;
                String jobFlagInfo = TypeND.SelectSingleNode("jobFlagInfo").InnerText;

                //-------------------------------------
                //- Send Callback                
                VMT_Data_JAT2.Objects.Common.VD_Common_JobOrder standAlone_job = new VMT_Data_JAT2.Objects.Common.VD_Common_JobOrder();
                
                standAlone_job.workingMchn.mchnId = working_mchnId;
                standAlone_job.workingMchn.mchnTp = working_mchnTp;
                standAlone_job.workingMchn.mchnSts = working_mchnSts;
                standAlone_job.workingMchn.vrtlFlg = working_vrtlFlg;

                standAlone_job.partnerMchn.mchnId = partner_mchnId;
                standAlone_job.partnerMchn.mchnTp = partner_mchnTp;
                standAlone_job.partnerMchn.mchnSts = partner_mchnSts;
                standAlone_job.partnerMchn.vrtlFlg = partner_vrtlFlg;

                standAlone_job.cntr.cntrNo = cntrNo;
                standAlone_job.cntr.cntrIso = cntrIso;
                standAlone_job.cntr.cntrTp = cntrTp;
                standAlone_job.cntr.cls = cls;
                standAlone_job.cntr.opr = opr;
                standAlone_job.cntr.cntrCgoTp = cntrCgoTp;
                standAlone_job.cntr.fullMty = fullMty;

                standAlone_job.locWorking.locTp = locTp;
                standAlone_job.locWorking.blck = blck;
                standAlone_job.locWorking.bay = bay;
                standAlone_job.locWorking.row = row;
                standAlone_job.locWorking.tier = tier;
                standAlone_job.locWorking.lane = lane;
                standAlone_job.locWorking.location = location;

                standAlone_job.type.jobTp = jobTp;
                standAlone_job.type.jobStatus = jobStatus;
                standAlone_job.type.vslCd = vslCd;
                standAlone_job.type.voyNo = voyNo;
                standAlone_job.type.planSeq = planSeq;
                standAlone_job.type.twinTandemFlg = twinTandemFlg;
                standAlone_job.type.twinTandumKey = twinTandumKey;
                standAlone_job.type.tandemJoinYT = tandemJoinYT;
                standAlone_job.type.jobFlagInfo = jobFlagInfo;

                ////PresentationMgr.Singleton.JOB_Add(standAlone_job);
                //if (App.CurrentResolution == App.ResolutionType.TYPE_800by600)
                //    VMT_RMG_800by600.PresentationMgr.Singleton.JOB_Add(standAlone_job);
                //else // if (App.CurrentResolution == App.ResolutionType.TYPE_1024by768)
                    VMT_RMG.PresentationMgr.Singleton.JOB_Add(standAlone_job);
            }
            catch (Exception ex)
            {
                string strError = ex.Message;
            }
        }

        private void Exec_ProcessByJobDeleteCallback(XmlNode interfaceMessage)
        {
            try
            {
                XmlNode WorkingMchnND = interfaceMessage.SelectSingleNode("workingMchn");
                String working_mchnId = WorkingMchnND.SelectSingleNode("mchnId").InnerText;
                String working_mchnTp = WorkingMchnND.SelectSingleNode("mchnTp").InnerText;
                String working_mchnSts = WorkingMchnND.SelectSingleNode("mchnSts").InnerText;
                String working_vrtlFlg = WorkingMchnND.SelectSingleNode("vrtlFlg").InnerText;

                XmlNode PartnerMchnND = interfaceMessage.SelectSingleNode("partnerMchn");
                String partner_mchnId = PartnerMchnND.SelectSingleNode("mchnId").InnerText;
                String partner_mchnTp = PartnerMchnND.SelectSingleNode("mchnTp").InnerText;
                String partner_mchnSts = PartnerMchnND.SelectSingleNode("mchnSts").InnerText;
                String partner_vrtlFlg = PartnerMchnND.SelectSingleNode("vrtlFlg").InnerText;

                XmlNode CntrND = interfaceMessage.SelectSingleNode("cntr");
                String cntrNo = CntrND.SelectSingleNode("cntrNo").InnerText;
                String cntrIso = CntrND.SelectSingleNode("cntrIso").InnerText;
                String cntrTp = CntrND.SelectSingleNode("cntrTp").InnerText;
                String cls = CntrND.SelectSingleNode("cls").InnerText;
                String opr = CntrND.SelectSingleNode("opr").InnerText;
                String cntrCgoTp = CntrND.SelectSingleNode("cntrCgoTp").InnerText;
                String fullMty = CntrND.SelectSingleNode("fullMty").InnerText;

                XmlNode LocND = interfaceMessage.SelectSingleNode("loc");
                String locTp = LocND.SelectSingleNode("locTp").InnerText;
                String blck = LocND.SelectSingleNode("blck").InnerText;
                String bay = LocND.SelectSingleNode("bay").InnerText;
                String row = LocND.SelectSingleNode("row").InnerText;
                String tier = LocND.SelectSingleNode("tier").InnerText;
                String lane = LocND.SelectSingleNode("lane").InnerText;
                String location = LocND.SelectSingleNode("location").InnerText;

                XmlNode TypeND = interfaceMessage.SelectSingleNode("type");
                String jobTp = TypeND.SelectSingleNode("jobTp").InnerText;
                String jobStatus = TypeND.SelectSingleNode("jobStatus").InnerText;
                String vslCd = TypeND.SelectSingleNode("vslCd").InnerText;
                String voyNo = TypeND.SelectSingleNode("voyNo").InnerText;
                String planSeq = TypeND.SelectSingleNode("planSeq").InnerText;
                String twinTandemFlg = TypeND.SelectSingleNode("twinTandemFlg").InnerText;
                String twinTandumKey = TypeND.SelectSingleNode("twinTandumKey").InnerText;
                String tandemJoinYT = TypeND.SelectSingleNode("tandemJoinYT").InnerText;
                String jobFlagInfo = TypeND.SelectSingleNode("jobFlagInfo").InnerText;

                //-------------------------------------
                //- Send Callback
                VMT_Data_JAT2.Objects.Common.VD_Common_JobOrder standAlone_job = new VMT_Data_JAT2.Objects.Common.VD_Common_JobOrder();

                //
                standAlone_job.workingMchn.mchnId = working_mchnId;
                standAlone_job.workingMchn.mchnTp = working_mchnTp;
                standAlone_job.workingMchn.mchnSts = working_mchnSts;
                standAlone_job.workingMchn.vrtlFlg = working_vrtlFlg;

                standAlone_job.partnerMchn.mchnId = partner_mchnId;
                standAlone_job.partnerMchn.mchnTp = partner_mchnTp;
                standAlone_job.partnerMchn.mchnSts = partner_mchnSts;
                standAlone_job.partnerMchn.vrtlFlg = partner_vrtlFlg;

                standAlone_job.cntr.cntrNo = cntrNo;
                standAlone_job.cntr.cntrIso = cntrIso;
                standAlone_job.cntr.cntrTp = cntrTp;
                standAlone_job.cntr.cls = cls;
                standAlone_job.cntr.opr = opr;
                standAlone_job.cntr.cntrCgoTp = cntrCgoTp;
                standAlone_job.cntr.fullMty = fullMty;

                standAlone_job.locWorking.locTp = locTp;
                standAlone_job.locWorking.blck = blck;
                standAlone_job.locWorking.bay = bay;
                standAlone_job.locWorking.row = row;
                standAlone_job.locWorking.tier = tier;
                standAlone_job.locWorking.lane = lane;
                standAlone_job.locWorking.location = location;

                standAlone_job.type.jobTp = jobTp;
                standAlone_job.type.jobStatus = jobStatus;
                standAlone_job.type.vslCd = vslCd;
                standAlone_job.type.voyNo = voyNo;
                standAlone_job.type.planSeq = planSeq;
                standAlone_job.type.twinTandemFlg = twinTandemFlg;
                standAlone_job.type.twinTandumKey = twinTandumKey;
                standAlone_job.type.tandemJoinYT = tandemJoinYT;
                standAlone_job.type.jobFlagInfo = jobFlagInfo;


                ////PresentationMgr.Singleton.JOB_Remove(standAlone_job);
                //if (App.CurrentResolution == App.ResolutionType.TYPE_800by600)
                //    VMT_RMG_800by600.PresentationMgr.Singleton.JOB_Remove(standAlone_job);
                //else // if (App.CurrentResolution == App.ResolutionType.TYPE_1024by768)
                    VMT_RMG.PresentationMgr.Singleton.JOB_Remove(standAlone_job);                
            }
            catch (Exception ex)
            {
                string strError = ex.Message;
            }
        }

        private void Exec_ProcessByJobDeleteAllCallback(XmlNode interfaceMessage)
        {
            try
            {
                string strValue = interfaceMessage.InnerText;
                int value;
                if (Int32.TryParse(strValue, out value))
                {

                    ////PresentationMgr.Singleton.JOB_Clear();
                    //if (App.CurrentResolution == App.ResolutionType.TYPE_800by600)
                    //    VMT_RMG_800by600.PresentationMgr.Singleton.JOB_Clear();
                    //else // if (App.CurrentResolution == App.ResolutionType.TYPE_1024by768)
                        VMT_RMG.PresentationMgr.Singleton.JOB_Clear();

                }
            }
            catch (Exception ex)
            {
                string strError = ex.Message;
            }
        }

        #endregion [Execute InterFace Message Function]

        private String getInnerText(XmlNode xmlNode)
        {
            String retValue = "";

            if (xmlNode != null)
            {
                retValue = xmlNode.InnerText;
            }

            return retValue;
        }
    }
}
