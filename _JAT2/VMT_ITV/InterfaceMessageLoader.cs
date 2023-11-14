using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.IO;
using System.Xml;
using VMT_Data_JAT2;

namespace VMT_ITV
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

        private String m_strXmlPath = "";

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

        public void LoadInterfaceMessageXml(String strXmlPath)
        {
            m_RoadXmlDoc.Load(strXmlPath);
            
            m_InterfaceMessageList = m_RoadXmlDoc.SelectNodes("//InterfaceMessageList/InterfaceMessage");

            // init
            currentMessageIndex = 0;
        }

        public void InitSaveInterfaceMessageXml()
        {
            String sRootPath = AppCfgMgr.GetAppDirectory();
            String sDirPath = sRootPath + @"{0}\Log";
            if (Directory.Exists(sDirPath) == false)
            {
                Directory.CreateDirectory(sDirPath);
            }

            m_strXmlPath = @sDirPath + @"\ITV_InterfaceMessag_LOG_"
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
                m_SaveXmlDoc.AppendChild(m_SaveXmlRootND);
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
                String strError = ex.Message;
            }
        }

        public void ExcuteInterfaceMessage_ByXmlNode(XmlNode interfaceMessage)
        {
            String strParameterName = getInnerText(interfaceMessage.Attributes["parameter"]);
            String strFunctionName = getInnerText(interfaceMessage.Attributes["function"]);

            if (strFunctionName.Equals("ProcessByJobOrderITVCallback") &&
                strParameterName.Equals("EEv2JobOrderForITV")) // JobOrder
            {
                Exec_ProcessByJobOrderITVCallback(interfaceMessage);
            }
            else if (strFunctionName.Equals("ProcessByJobDoneCallback") &&
                 strParameterName.Equals("EEv2JobOrder")) // JobDone
            {
                Exec_ProcessByJobDoneCallback(interfaceMessage);
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

        public void WriteInterfaceMessage<T>(String strFunctionName, T obj)
        {
            if (MainWindow.MESSAGE_CAPTURE_MODE != true)
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
                    Util.ExportMessageObjToXml<T>(m_SaveXmlDoc, strFunctionName, obj);
                    m_SaveXmlDoc.Save(m_strXmlPath);
                }
                else
                {
                    throw new Exception("Invalid Data Type");
                }
            }
            catch (Exception ex)
            {
                String strError = ex.Message;
            }
        }

        #endregion [Write InterFace Message Function]

        #region [Execute InterFace Message Function]
        //-------------------------------------
        //- InterfaceMessage Detail Function
        private void Exec_ProcessByJobOrderITVCallback(XmlNode interfaceMessage)
        {
            try
            {
                //-------------------------------------
                //- FirstJob
                XmlNode FirstJobND = interfaceMessage.SelectSingleNode("firstJob");

                XmlNode First_WorkingMchnND = FirstJobND.SelectSingleNode("workingMchn");
                String First_working_mchnId = getInnerText(First_WorkingMchnND.SelectSingleNode("mchnId"));
                String First_working_mchnTp = getInnerText(First_WorkingMchnND.SelectSingleNode("mchnTp"));
                String First_working_mchnSts = getInnerText(First_WorkingMchnND.SelectSingleNode("mchnSts"));
                String First_working_vrtlFlg = getInnerText(First_WorkingMchnND.SelectSingleNode("vrtlFlg"));

                XmlNode First_PartnerMchnND = FirstJobND.SelectSingleNode("partnerMchn");
                String First_partner_mchnId = getInnerText(First_PartnerMchnND.SelectSingleNode("mchnId"));
                String First_partner_mchnTp = getInnerText(First_PartnerMchnND.SelectSingleNode("mchnTp"));
                String First_partner_mchnSts = getInnerText(First_PartnerMchnND.SelectSingleNode("mchnSts"));
                String First_partner_vrtlFlg = getInnerText(First_PartnerMchnND.SelectSingleNode("vrtlFlg"));

                XmlNode First_CntrND = FirstJobND.SelectSingleNode("cntr");
                String First_cntrNo = getInnerText(First_CntrND.SelectSingleNode("cntrNo"));
                String First_cntrIso = getInnerText(First_CntrND.SelectSingleNode("cntrIso"));
                String First_cntrTp = getInnerText(First_CntrND.SelectSingleNode("cntrTp"));
                String First_cls = getInnerText(First_CntrND.SelectSingleNode("cls"));
                String First_opr = getInnerText(First_CntrND.SelectSingleNode("opr"));
                String First_cntrCgoTp = getInnerText(First_CntrND.SelectSingleNode("cntrCgoTp"));
                String First_fullMty = getInnerText(First_CntrND.SelectSingleNode("fullMty"));

                XmlNode First_LocND = FirstJobND.SelectSingleNode("loc");
                String First_locTp = getInnerText(First_LocND.SelectSingleNode("locTp"));
                String First_blck = getInnerText(First_LocND.SelectSingleNode("blck"));
                String First_bay = getInnerText(First_LocND.SelectSingleNode("bay"));
                String First_row = getInnerText(First_LocND.SelectSingleNode("row"));
                String First_tier = getInnerText(First_LocND.SelectSingleNode("tier"));
                String First_lane = getInnerText(First_LocND.SelectSingleNode("lane"));
                String First_location = getInnerText(First_LocND.SelectSingleNode("location"));

                XmlNode First_TypeND = FirstJobND.SelectSingleNode("type");
                String First_jobTp = getInnerText(First_TypeND.SelectSingleNode("jobTp"));
                String First_jobStatus = getInnerText(First_TypeND.SelectSingleNode("jobStatus"));
                String First_vslCd = getInnerText(First_TypeND.SelectSingleNode("vslCd"));
                String First_voyNo = getInnerText(First_TypeND.SelectSingleNode("voyNo"));
                String First_planSeq = getInnerText(First_TypeND.SelectSingleNode("planSeq"));
                String First_twinTandemFlg = getInnerText(First_TypeND.SelectSingleNode("twinTandemFlg"));
                String First_twinTandumKey = getInnerText(First_TypeND.SelectSingleNode("twinTandumKey"));
                String First_tandemJoinYT = getInnerText(First_TypeND.SelectSingleNode("tandemJoinYT"));
                String First_jobFlagInfo = getInnerText(First_TypeND.SelectSingleNode("jobFlagInfo"));

                String First_firstJobStatus = getInnerText(interfaceMessage.SelectSingleNode("firstJobStatus"));
                String First_firstJobETA = getInnerText(interfaceMessage.SelectSingleNode("firstJobETA"));

                //-------------------------------------
                //- SecondJob
                XmlNode SecondJobND = interfaceMessage.SelectSingleNode("secondJob");

                XmlNode Second_WorkingMchnND = SecondJobND.SelectSingleNode("workingMchn");
                String Second_working_mchnId = getInnerText(Second_WorkingMchnND.SelectSingleNode("mchnId"));
                String Second_working_mchnTp = getInnerText(Second_WorkingMchnND.SelectSingleNode("mchnTp"));
                String Second_working_mchnSts = getInnerText(Second_WorkingMchnND.SelectSingleNode("mchnSts"));
                String Second_working_vrtlFlg = getInnerText(Second_WorkingMchnND.SelectSingleNode("vrtlFlg"));

                XmlNode Second_PartnerMchnND = SecondJobND.SelectSingleNode("partnerMchn");
                String Second_partner_mchnId = getInnerText(Second_PartnerMchnND.SelectSingleNode("mchnId"));
                String Second_partner_mchnTp = getInnerText(Second_PartnerMchnND.SelectSingleNode("mchnTp"));
                String Second_partner_mchnSts = getInnerText(Second_PartnerMchnND.SelectSingleNode("mchnSts"));
                String Second_partner_vrtlFlg = getInnerText(Second_PartnerMchnND.SelectSingleNode("vrtlFlg"));

                XmlNode Second_CntrND = SecondJobND.SelectSingleNode("cntr");
                String Second_cntrNo = getInnerText(Second_CntrND.SelectSingleNode("cntrNo"));
                String Second_cntrIso = getInnerText(Second_CntrND.SelectSingleNode("cntrIso"));
                String Second_cntrTp = getInnerText(Second_CntrND.SelectSingleNode("cntrTp"));
                String Second_cls = getInnerText(Second_CntrND.SelectSingleNode("cls"));
                String Second_opr = getInnerText(Second_CntrND.SelectSingleNode("opr"));
                String Second_cntrCgoTp = getInnerText(Second_CntrND.SelectSingleNode("cntrCgoTp"));
                String Second_fullMty = getInnerText(Second_CntrND.SelectSingleNode("fullMty"));

                XmlNode Second_LocND = SecondJobND.SelectSingleNode("loc");
                String Second_locTp = getInnerText(Second_LocND.SelectSingleNode("locTp"));
                String Second_blck = getInnerText(Second_LocND.SelectSingleNode("blck"));
                String Second_bay = getInnerText(Second_LocND.SelectSingleNode("bay"));
                String Second_row = getInnerText(Second_LocND.SelectSingleNode("row"));
                String Second_tier = getInnerText(Second_LocND.SelectSingleNode("tier"));
                String Second_lane = getInnerText(Second_LocND.SelectSingleNode("lane"));
                String Second_location = getInnerText(Second_LocND.SelectSingleNode("location"));

                XmlNode Second_TypeND = SecondJobND.SelectSingleNode("type");
                String Second_jobTp = getInnerText(Second_TypeND.SelectSingleNode("jobTp"));
                String Second_jobStatus = getInnerText(Second_TypeND.SelectSingleNode("jobStatus"));
                String Second_vslCd = getInnerText(Second_TypeND.SelectSingleNode("vslCd"));
                String Second_voyNo = getInnerText(Second_TypeND.SelectSingleNode("voyNo"));
                String Second_planSeq = getInnerText(Second_TypeND.SelectSingleNode("planSeq"));
                String Second_twinTandemFlg = getInnerText(Second_TypeND.SelectSingleNode("twinTandemFlg"));
                String Second_twinTandumKey = getInnerText(Second_TypeND.SelectSingleNode("twinTandumKey"));
                String Second_tandemJoinYT = getInnerText(Second_TypeND.SelectSingleNode("tandemJoinYT"));
                String Second_jobFlagInfo = getInnerText(Second_TypeND.SelectSingleNode("jobFlagInfo"));

                String Second_secondJobStatus = getInnerText(interfaceMessage.SelectSingleNode("secondJobStatus"));
                String Second_secondJobETA = getInnerText(interfaceMessage.SelectSingleNode("secondJobETA"));

                String kpiInfo1 = getInnerText(interfaceMessage.SelectSingleNode("kpiInfo1"));
                String kpiInfo2 = getInnerText(interfaceMessage.SelectSingleNode("kpiInfo2"));
                String kpiInfo3 = getInnerText(interfaceMessage.SelectSingleNode("kpiInfo3"));
                String kpiInfo4 = getInnerText(interfaceMessage.SelectSingleNode("kpiInfo4"));

                //-------------------------------------
                //- Send Callback
                VMT_Data_JAT2.Objects.ITV.VD_ITV_JobOrderList standAlone_job = new VMT_Data_JAT2.Objects.ITV.VD_ITV_JobOrderList();
                //
                standAlone_job.FirstJob.workingMchn.mchnId = First_working_mchnId;
                standAlone_job.FirstJob.workingMchn.mchnTp = First_working_mchnTp;
                standAlone_job.FirstJob.workingMchn.mchnSts = First_working_mchnSts;
                standAlone_job.FirstJob.workingMchn.vrtlFlg = First_working_vrtlFlg;

                standAlone_job.FirstJob.partnerMchn.mchnId = First_partner_mchnId;
                standAlone_job.FirstJob.partnerMchn.mchnTp = First_partner_mchnTp;
                standAlone_job.FirstJob.partnerMchn.mchnSts = First_partner_mchnSts;
                standAlone_job.FirstJob.partnerMchn.vrtlFlg = First_partner_vrtlFlg;

                standAlone_job.FirstJob.cntr.cntrNo = First_cntrNo;
                standAlone_job.FirstJob.cntr.cntrIso = First_cntrIso;
                standAlone_job.FirstJob.cntr.cntrTp = First_cntrTp;
                standAlone_job.FirstJob.cntr.cls = First_cls;
                standAlone_job.FirstJob.cntr.opr = First_opr;
                standAlone_job.FirstJob.cntr.cntrCgoTp = First_cntrCgoTp;
                standAlone_job.FirstJob.cntr.fullMty = First_fullMty;

                standAlone_job.FirstJob.locWorking.locTp = First_locTp;
                standAlone_job.FirstJob.locWorking.blck = First_blck;
                standAlone_job.FirstJob.locWorking.bay = First_bay;
                standAlone_job.FirstJob.locWorking.row = First_row;
                standAlone_job.FirstJob.locWorking.tier = First_tier;
                standAlone_job.FirstJob.locWorking.lane = First_lane;
                standAlone_job.FirstJob.locWorking.location = First_location;

                standAlone_job.FirstJob.type.jobTp = First_jobTp;
                standAlone_job.FirstJob.type.jobStatus = First_jobStatus;
                standAlone_job.FirstJob.type.vslCd = First_vslCd;
                standAlone_job.FirstJob.type.voyNo = First_voyNo;
                standAlone_job.FirstJob.type.planSeq = First_planSeq;
                standAlone_job.FirstJob.type.twinTandemFlg = First_twinTandemFlg;
                standAlone_job.FirstJob.type.twinTandumKey = First_twinTandumKey;
                standAlone_job.FirstJob.type.tandemJoinYT = First_tandemJoinYT;
                standAlone_job.FirstJob.type.jobFlagInfo = First_jobFlagInfo;

                //standAlone_job.FirstJobStatus = First_firstJobStatus;
                //standAlone_job.firstJobETA = Convert.ToInt32(First_firstJobETA);

                //
                standAlone_job.SecondJob.workingMchn.mchnId = Second_working_mchnId;
                standAlone_job.SecondJob.workingMchn.mchnTp = Second_working_mchnTp;
                standAlone_job.SecondJob.workingMchn.mchnSts = Second_working_mchnSts;
                standAlone_job.SecondJob.workingMchn.vrtlFlg = Second_working_vrtlFlg;

                standAlone_job.SecondJob.partnerMchn.mchnId = Second_partner_mchnId;
                standAlone_job.SecondJob.partnerMchn.mchnTp = Second_partner_mchnTp;
                standAlone_job.SecondJob.partnerMchn.mchnSts = Second_partner_mchnSts;
                standAlone_job.SecondJob.partnerMchn.vrtlFlg = Second_partner_vrtlFlg;

                standAlone_job.SecondJob.cntr.cntrNo = Second_cntrNo;
                standAlone_job.SecondJob.cntr.cntrIso = Second_cntrIso;
                standAlone_job.SecondJob.cntr.cntrTp = Second_cntrTp;
                standAlone_job.SecondJob.cntr.cls = Second_cls;
                standAlone_job.SecondJob.cntr.opr = Second_opr;
                standAlone_job.SecondJob.cntr.cntrCgoTp = Second_cntrCgoTp;
                standAlone_job.SecondJob.cntr.fullMty = Second_fullMty;

                standAlone_job.SecondJob.locWorking.locTp = Second_locTp;
                standAlone_job.SecondJob.locWorking.blck = Second_blck;
                standAlone_job.SecondJob.locWorking.bay = Second_bay;
                standAlone_job.SecondJob.locWorking.row = Second_row;
                standAlone_job.SecondJob.locWorking.tier = Second_tier;
                standAlone_job.SecondJob.locWorking.lane = Second_lane;
                standAlone_job.SecondJob.locWorking.location = Second_location;

                standAlone_job.SecondJob.type.jobTp = Second_jobTp;
                standAlone_job.SecondJob.type.jobStatus = Second_jobStatus;
                standAlone_job.SecondJob.type.vslCd = Second_vslCd;
                standAlone_job.SecondJob.type.voyNo = Second_voyNo;
                standAlone_job.SecondJob.type.planSeq = Second_planSeq;
                standAlone_job.SecondJob.type.twinTandemFlg = Second_twinTandemFlg;
                standAlone_job.SecondJob.type.twinTandumKey = Second_twinTandumKey;
                standAlone_job.SecondJob.type.tandemJoinYT = Second_tandemJoinYT;
                standAlone_job.SecondJob.type.jobFlagInfo = Second_jobFlagInfo;

                //standAlone_job.SecondJobStatus = Second_secondJobStatus;
                //standAlone_job.secondJobETA = Convert.ToInt32(Second_secondJobETA);

                //standAlone_job.kpiInfo1 = Convert.ToInt32(kpiInfo1);
                //standAlone_job.kpiInfo2 = Convert.ToInt32(kpiInfo2);
                //standAlone_job.kpiInfo3 = Convert.ToInt32(kpiInfo3);
                //standAlone_job.kpiInfo4 = Convert.ToInt32(kpiInfo4);
                
                 PresentationMgr.MainView.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal,
                        new Action(delegate
                            {
                                PresentationMgr.MainView.ProcessByJobOrderITVCallback(standAlone_job);
                            }));

            }
            catch (Exception ex)
            {
                String strError = ex.Message;
            }
        }

        private void Exec_ProcessByJobDoneCallback(XmlNode interfaceMessage)
        {
            try
            {
                String jobKey = interfaceMessage.SelectSingleNode("jobKey").InnerText;

                //-------------------------------------
                //- Send Callback
                VMT_Data_JAT2.Objects.Common.VD_Common_JobDone standAlone_job = new VMT_Data_JAT2.Objects.Common.VD_Common_JobDone();

                //
                standAlone_job.jobKey = jobKey;

                PresentationMgr.MainView.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal,
                        new Action(delegate
                        {
                            PresentationMgr.MainView.ProcessByJobDoneCallback(standAlone_job);
                        }));
            }
            catch (Exception ex)
            {
                String strError = ex.Message;
            }
        }

        private void Exec_ProcessByJobDeleteCallback(XmlNode interfaceMessage)
        {
            try
            {
                String jobKey = interfaceMessage.SelectSingleNode("jobKey").InnerText;

                //-------------------------------------
                //- Send Callback
                VMT_Data_JAT2.Objects.Common.VD_Common_JobKey standAlone_job = new VMT_Data_JAT2.Objects.Common.VD_Common_JobKey();

                standAlone_job.jobKey = jobKey;

                PresentationMgr.MainView.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal,
                        new Action(delegate
                        {
                            PresentationMgr.MainView.ProcessByJobDeleteCallback(standAlone_job);
                        }));
            }
            catch (Exception ex)
            {
                String strError = ex.Message;
            }
        }

        private void Exec_ProcessByJobDeleteAllCallback(XmlNode interfaceMessage)
        {
            try
            {
                String strValue = interfaceMessage.InnerText;
                int value;
                if (Int32.TryParse(strValue, out value))
                {
                    PresentationMgr.MainView.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal,
                            new Action(delegate
                            {
                                PresentationMgr.MainView.ProcessByJobDeleteAllCallback(value);
                            }));
                }
            }
            catch (Exception ex)
            {
                String strError = ex.Message;
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
