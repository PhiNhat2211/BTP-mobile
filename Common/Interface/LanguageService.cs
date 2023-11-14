using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Xml.Linq;
using Common.Interface;

namespace Common.Interface
{
    public class LanguageService : IResourceService
    {
        public const string MESSAGE_GROUP = "MessageGroup";
        public const string MESSAGE_SERVER_GROUP = "MessageServerGroup";
        public const string MESSAGE_JOBDONE_GROUP = "MessageJobDone";
        public const string MESSAGE_JOBSETERROR_GROUP = "MessageJobError";
        public const string MESSAGE_CHANGEPOSITIONERROR_GROUP = "MessageChangePositionError";
        public const string MESSAGE_BESTPICK_GROUP = "MessageBestPickError";
        public const string MESSAGE_LOGIN_GROUP = "SetLogInMachine";
        public const string MESSAGE_LOGOUT_GROUP = "SetLogoutMachine";
        public const string MESSAGE_GETLOADINGSWAP = "GetLoadingSwapMessage";
        //public const string LABEL_GROUP = "LabelGroup";
        public const string LABEL_JOBTYPE = "JobType";
        public const string LABEL_LOGIN = "Login";
        public const string LABEL_MAINWINDOW = "MainWindow";
        public const string LABEL_SETTING = "Setting";
        public const string LABEL_BREAKTIME = "BreakTime";
        public const string LABEL_CHANGEDRIVER = "ChangeDriver";
        public const string LABEL_AVAILABLE = "Available";
        public const string LABEL_POPUP = "Popup";
        public const string LABEL_CONTAINERDETAIL = "ContainerDetail";
        public const string LABEL_KEYPAD = "KeyPad";
        public const string LABEL_SEARCH = "Search";
        public const string LABEL_JOBLIST = "JobList";
        public const string LABEL_CHSSCHG = "ChassisChange";
        public const string LABEL_MACHINESEARCHVIEW = "MachineSearchView";
        public const string LABEL_VIRTUALBLOCK = "VirtualBlock";
        public const string LABEL_DRIVERWORKING = "DriverWorking";
        public const string LABEL_OTHERMACHINEJOB = "OtherMachineJob";
        public const string LABEL_PRESENTATIONMGR = "PresentationMgr";
        public const string LABEL_SELECTIONVIEW = "SelectionView";
        public const string LABEL_CALIBRATIONINFO = "CalibrationInfo";
        public const string DUAL_LANGUAGE = "DualLanguage";
        public const string LABEL_CUSTOMIZE = "Customize";
        public const string LABEL_POPUPOUT = "PopupOut";
        public const string LABEL_YTSWAP = "YtSwap";
        public const string LABEL_EMPTYSWAP = "EmptySwap";
        public const string LABEL_DISPLAYOPTION = "DisplayOption";

        private ResourceXmlReader resourceXmlReader;

        private bool _isLoaded = false;

        public bool IsLoaded { get { return _isLoaded; } set { _isLoaded = value; } }

        public LanguageService()
        {
            _isLoaded = false;
            resourceXmlReader = new ResourceXmlReader();
            _isLoaded = true;
        }
        public int Count()
        {
            return resourceXmlReader.Resources.Count();
        }
        
        /*public string GetResource(string resourceKey, string messageGroup, string resourceName)
        {
            try
            {
                
                if (resourceXmlReader.Resources.ContainsKey(resourceName))
                {
                    if (resourceXmlReader.Resources[resourceName].ContainsKey(messageGroup))
                    {
                        if (resourceXmlReader.Resources[resourceName][messageGroup].ContainsKey(resourceKey))
                        {
                            return resourceXmlReader.Resources[resourceName][messageGroup][resourceKey];
                        }

                    }
                }
                return string.Empty;
            }
            catch
            {
                return string.Empty;
            }

        }*/

        public string GetResourceITV(string resourceKey, string messageGroup)
        {
            return GetResource(resourceKey, messageGroup, "UserLanguageITV");
        }
        public string GetResourceRMG(string resourceKey, string messageGroup)
        {
            return GetResource(resourceKey, messageGroup, "UserLanguageRTG");
        }
        public string GetResourceECH(string resourceKey, string messageGroup)
        {
            return GetResource(resourceKey, messageGroup, "UserLanguageECH");
        }
        public string GetResourceSC(string resourceKey, string messageGroup)
        {
            return GetResource(resourceKey, messageGroup, "UserLanguageSC");
        }
        private string GetResource(string resourceKey, string messageGroup, string keywork)
        {
            try
            {
                if (resourceXmlReader.Resources.ContainsKey(keywork))
                {
                    if (resourceXmlReader.Resources[keywork].ContainsKey(messageGroup))
                    {
                        if (resourceXmlReader.Resources[keywork][messageGroup].ContainsKey(resourceKey))
                        {
                            return resourceXmlReader.Resources[keywork][messageGroup][resourceKey];
                        }

                    }
                }
                return string.Empty;
            }
            catch
            {
                return string.Empty;
            }
        }

        public void ReloadLanguage()
        {
            resourceXmlReader = new ResourceXmlReader();
        }
    }

    public class ResourceXmlReader
    {
        public readonly Dictionary<string, Dictionary<string, Dictionary<string, string>>> Resources = new Dictionary<string, Dictionary<string, Dictionary<string, string>>>();
        private string path = "";
        private string filePath = "";
        public ResourceXmlReader()
        {

            DirectoryInfo dInfo = Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory);
            string strParentPath = dInfo.Parent.FullName;
            string strCommonPath = strParentPath + @"\Common";

            if (!Directory.Exists(strCommonPath))
            {
                Directory.CreateDirectory(strCommonPath);
            }

            string strIniFile = strCommonPath + @"\MachineInfo.ini";
            var lines = File.ReadAllLines(strIniFile);
            foreach (var line in lines)
            {
                if (line.IndexOf("LanguagePath") > -1)
                {
                    OpenAndStoreResource(line.Replace("LanguagePath", "").Replace("=", ""));
                    break;
                }
            }

            /*
            //string currentLang=Thread.CurrentThread.CurrentCulture.ToString();
            string currentLang = "en-US";

            string p = AppDomain.CurrentDomain.BaseDirectory;
            FileInfo pInfo = new FileInfo(p);
            DirectoryInfo pInfoParent = pInfo.Directory.Parent.Parent;
            string pParent = pInfoParent.FullName;


            //path = pParent + @"\Languages\MessageEnglish.xml";
            path = p + @"\Languages\MessageEnglish.xml";
            OpenAndStoreResource(path, currentLang);
            //path = pParent + @"\Languages\MessageForeign.xml";
            path = p + @"\Languages\MessageForeign.xml";
            System.Globalization.CultureInfo cultureInfoForeign = new System.Globalization.CultureInfo("fr-FR");
            currentLang = cultureInfoForeign.ToString();
            OpenAndStoreResource(path, currentLang);
            */
        }

        private void OpenAndStoreResource(string resourcePath, string languageType)
        {
            try
            {
                
                string fileName = Path.GetFileName(resourcePath).Split('.')[0];
                XDocument doc = XDocument.Load(resourcePath);
                if (null != doc)
                {
                    Dictionary<string, Dictionary<string, string>> currentResourceGroup = new Dictionary<string, Dictionary<string, string>>();
                    var resources = doc.Descendants("ResourceGroup").ToList();

                    Dictionary<string, string> currentResource = null;

                    foreach (var item in resources)
                    {
                        currentResource = new Dictionary<string, string>();
                        var children = item.Descendants("Resource").ToList();
                        children.ForEach(o => currentResource.Add(o.Attribute("key").Value, o.Value.TrimStart().TrimEnd()));
                        if (!currentResourceGroup.ContainsKey(item.FirstAttribute.Value))
                        {
                            currentResourceGroup.Add(item.FirstAttribute.Value, currentResource);
                        }
                    }

                    if (!Resources.ContainsKey(languageType))
                    {
                        Resources.Add(languageType, currentResourceGroup);
                    }
                    else
                    {
                        Resources.Remove(languageType);
                        Resources.Add(languageType, currentResourceGroup);
                    }
                }
            }
            catch(Exception e)
            {

            }

        }

        private void OpenAndStoreResource(string resourcePath)
        {
            try
            {

                string fileName = Path.GetFileName(resourcePath).Split('.')[0];
                XDocument doc = XDocument.Load(resourcePath);
                if (null != doc)
                {   
                    Dictionary<string, string> currentResource = null;
                    var resources = doc.Descendants("ResourceItem").ToList();
                    foreach (var itemByType in resources)
                    {
                        Dictionary<string, Dictionary<string, string>> currentResourceGroup = new Dictionary<string, Dictionary<string, string>>();
                        var itemResources = itemByType.Descendants("ResourceGroup").ToList();
                        foreach (var item in itemResources)
                        {
                            currentResource = new Dictionary<string, string>();
                            var children = item.Descendants("Resource").ToList();
                            children.ForEach(o => currentResource.Add(o.Attribute("key").Value, o.Value.TrimStart().TrimEnd()));
                            if (!currentResourceGroup.ContainsKey(item.FirstAttribute.Value))
                            {
                                currentResourceGroup.Add(item.FirstAttribute.Value, currentResource);
                            }
                        }
                        string keywork = "UserLanguage" + itemByType.Attribute("id").Value;
                        if (!Resources.ContainsKey(keywork))
                        {
                            Resources.Add(keywork, currentResourceGroup);                       
                        }
                        else
                        {
                            Resources.Remove(keywork);
                            Resources.Add(keywork, currentResourceGroup);
                        }
                    }      
                }
            }
            catch (Exception e)
            {

            }

        }
    } 
}
