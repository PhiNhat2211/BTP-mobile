using System;
using System.Linq;
using System.Text;
using System.Xml;
using System.IO;
using System.Data;
using System.Collections;
using System.Text.RegularExpressions;
using System.Collections.Generic;

namespace VMT_RMG
{

    public class AppCfgMgr
    {
        public static readonly string DefaultAppCfgFileName = "VMT_RMG.cfg.xml";

        public static readonly string AppSystemID = "VMT_RMG";   

                
        class ConfigDataSet
        {
            public string XPath { get; set; }
            public string Value { get; set; }

            public ConfigDataSet(string xPath, string value)
            {
                XPath = xPath;
                Value = value;
            }
        }

        // Application Configuration file Path
        public string FilePath { get; set; }

        // XML document object
        private XmlDocument xmlDoc = null;
        // Default Application Configuration DataSet
        private Hashtable defCfgDataSet = null;


        private static readonly AppCfgMgr _theOnly = null;

        public static AppCfgMgr Singleton
        {
            get { return _theOnly; }
        }

        static AppCfgMgr()
        {
            _theOnly = new AppCfgMgr();
        }

        private AppCfgMgr()
        {
            xmlDoc = new XmlDocument();
            defCfgDataSet = new Hashtable();

            // Initalize Application Default Configuration Hashtable
            InitAppDefCfgData();
        }
        
        private void InitAppDefCfgData()
        {
            //[Application Options]
            defCfgDataSet.Add("IsTestMode", new ConfigDataSet("AppCfgDoc/Options/IsTestMode", "0"));
            defCfgDataSet.Add("IsTestWriteMode", new ConfigDataSet("AppCfgDoc/Options/IsTestWriteMode", "0"));
            defCfgDataSet.Add("IsStandAlone", new ConfigDataSet("AppCfgDoc/Options/IsStandAlone", "0"));
            defCfgDataSet.Add("IsMessageCapture", new ConfigDataSet("AppCfgDoc/Options/IsMessageCapture", "0"));
            defCfgDataSet.Add("ShowAppUI", new ConfigDataSet("AppCfgDoc/Options/ShowAppUI", "1"));
            defCfgDataSet.Add("IsStartUp", new ConfigDataSet("AppCfgDoc/Options/IsStartUp", "1")); //Jan 6 2020

            defCfgDataSet.Add("MaxSizeRollBackups", new ConfigDataSet("AppCfgDoc/Options/MaxSizeRollBackups", "7"));
            defCfgDataSet.Add("MaximumFileSize", new ConfigDataSet("AppCfgDoc/Options/MaximumFileSize", "20MB"));

            //[Tos Hessian Server]
            defCfgDataSet.Add("HessianServerIP", new ConfigDataSet("AppCfgDoc/HessianServer/HessianServerIP", "172.20.130.156"));
            defCfgDataSet.Add("HessianServerPort", new ConfigDataSet("AppCfgDoc/HessianServer/HessianServerPort", "7110"));
            defCfgDataSet.Add("HessianServerMgtIP", new ConfigDataSet("AppCfgDoc/HessianServer/HessianServerMgtIP", "116.127.223.206"));
            defCfgDataSet.Add("HessianServerMgtPort", new ConfigDataSet("AppCfgDoc/HessianServer/HessianServerMgtPort", "7110"));

            //[Siemens Interface]
            defCfgDataSet.Add("SiemensInterfacePort", new ConfigDataSet("AppCfgDoc/SiemensInterface/SiemensInterfacePort", "58001"));
            defCfgDataSet.Add("SiemensConnectionCheckPeriod", new ConfigDataSet("AppCfgDoc/SiemensInterface/SiemensConnectionCheckPeriod", "10"));
        }

        public void LoadFile(string xmlPath=null)
        {
            if( !String.IsNullOrEmpty(xmlPath) )
                FilePath = xmlPath;
            if (String.IsNullOrEmpty(FilePath))
                throw new Exception("XML file path not assgined!");

            try {
                ValidateCfgFile(FilePath);
                xmlDoc.Load(FilePath);
            }
            catch (Exception ex) {
                System.Windows.MessageBox.Show(PresentationMgr.AppWin, ex.Message);
            }
        }

        public void SaveFile(string xmlPath=null)
        {
            if( !String.IsNullOrEmpty(xmlPath) )
                FilePath = xmlPath;
            if (String.IsNullOrEmpty(FilePath))
                throw new Exception("XML file path not assgined!");
            try
            {
                xmlDoc.Save(FilePath);
            }
            catch (Exception e)
            {
                System.Windows.MessageBox.Show(PresentationMgr.AppWin, e.Message);
            }
        }

        public string GetValueByKey(string keyName)
        {
            if (xmlDoc == null)
                throw new Exception("XML Document was not initailized!");

            string retValue = null;

            try {
                ConfigDataSet ds = defCfgDataSet[keyName] as ConfigDataSet;
                XmlNode node = xmlDoc.SelectSingleNode(ds.XPath);

                if(node != null)
                    retValue = xmlDoc.SelectSingleNode(ds.XPath).InnerText;
                else {
                    node = CreateXPathNode(ds.XPath);
                    node.InnerText = ds.Value;
                    retValue = ds.Value;
                }
            }
            catch (Exception ex) {
                System.Windows.MessageBox.Show(PresentationMgr.AppWin, ex.Message);
            }

            return retValue;  
        }

        public void SetValueByKey(string keyName, string value)
        {
            try
            {
                ConfigDataSet ds = defCfgDataSet[keyName] as ConfigDataSet;
                if (ds == null)
                {
                    System.Windows.MessageBox.Show(PresentationMgr.AppWin, "AppCfgMgr : Not defined Application KeyValue Assinged!");
                    return;
                }

                XmlNode node = xmlDoc.SelectSingleNode(ds.XPath);

                if (node == null)
                    node = CreateXPathNode(ds.XPath);

                // Set XML Node Value
                node.InnerText = value;
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(PresentationMgr.AppWin, ex.Message);
            }
        }

        public void RemoveValueByKey(string keyName)
        {
            try
            {
                XmlNode xNode = xmlDoc.SelectSingleNode("//" + keyName);
                XmlNode xParent = xNode.ParentNode;
                xParent.RemoveChild(xNode);
            }
            catch
            {
            }
        }

        private XmlNode CreateXPathNode(string xPath)
        {
            XmlNode xmlNode = null;
            bool bRoot = false;

            int ilast = xPath.LastIndexOf('/');
            if (ilast < 0)
                bRoot = true;

            string strParentNode = xPath.Substring(0, ilast);
            string strNodeName = xPath.Substring(ilast+1, xPath.Length-(ilast+1));

            XmlNode xmlParentNode = xmlDoc.SelectSingleNode(strParentNode);

            if (xmlParentNode == null)
            {
                if (bRoot == false)
                    xmlParentNode = CreateXPathNode(strParentNode);
                else
                    throw new Exception("Root Element cannot be created by CreateXPathNode function!");
            }

            if (strNodeName.IndexOf('@') < 0)
            {
                xmlNode = xmlDoc.CreateNode(XmlNodeType.Element, strNodeName, "");
                // Append created XML node
                xmlParentNode.AppendChild(xmlNode);
            }
            else
            {
                xmlNode = xmlDoc.CreateNode(XmlNodeType.Attribute, strNodeName.Trim('@'), "");
                xmlParentNode.Attributes.Append(xmlNode as XmlAttribute);
            }

            return xmlNode;
        }

        private bool ValidateCfgFile(string path, bool create=true)
        {
            bool ret = false;

            FileInfo fi = new FileInfo(path);

            if (fi.Exists)
                ret = true;
            else
            {
                if(create)
                {
                    XmlTextWriter writer = new XmlTextWriter(DefaultAppCfgFileName, Encoding.UTF8  );
                    writer.Formatting = Formatting.Indented;
                    writer.WriteStartDocument();
                    writer.WriteStartElement("AppCfgDoc");

                    writer.WriteEndElement();
                    writer.WriteEndDocument();
                    writer.Flush();
                    writer.Close();
                 }
            }

            return ret;
        }

        public static string GetAppDirectory()
        {
            string path = null;

            path = AppDomain.CurrentDomain.BaseDirectory;

            return path;
        }

        public static string GetApplicationPath(bool bFileName = false)
        {
            string path = null;


            path = AppDomain.CurrentDomain.BaseDirectory;

            if (bFileName)
                path += @"VMT_RMG.exe";

            return path;
        }

        public static string GetApplicationDBDir()
        {
            string path = null;

            path = AppDomain.CurrentDomain.BaseDirectory;
            path += @"DB\";

            return path;
        }

    }
}
