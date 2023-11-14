using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace VMT_Data_JAT2
{
    public class ForeignInfo
    {
        public static string languageUser = string.Empty;
        public static string languagePathUser = string.Empty;
        public static string uiSizeUser = string.Empty;
        public static string externalTruck = "N";
        public static string ytAssigned = "N";
        public static string jobType = String.Empty;

        new public static void GetUserLanguageType(ref string language)
        {
            string strIniFile = GetIniDirectory() + @"MachineInfo.ini"; // NCT

            Ini.IniFile ini = new Ini.IniFile(strIniFile);
            string tmp = ini.IniReadValue("MACHINE", "LanguageType", "fr-FR");
            language = String.IsNullOrEmpty(tmp) ? "fr-FR" : tmp;
        }

        new public static void GetUserLanguagePath(ref string languagePath)
        {
            string strIniFile = GetIniDirectory() + @"MachineInfo.ini"; // NCT
            string defaultPath = AppDomain.CurrentDomain.BaseDirectory + @"\Languages\English.xml";

            Ini.IniFile ini = new Ini.IniFile(strIniFile);
            languagePath = ini.IniReadValue("MACHINE", "LanguagePath");

            if (languagePath.Equals(string.Empty))
            {
                languagePath = defaultPath;
            }

            if (!File.Exists(languagePath)) languagePath = defaultPath;
            //if (!File.Exists(languagePath)) languagePath = "";
            ini.IniWriteValue("MACHINE", "LanguagePath", languagePath);
        }

        new public static void GetUserUiSize(ref string uiSize)
        {
            string strIniFile = GetIniDirectory() + @"MachineInfo.ini"; // NCT

            Ini.IniFile ini = new Ini.IniFile(strIniFile);
            uiSize = ini.IniReadValue("MACHINE", "UISIZE");

            if (uiSize.Equals(string.Empty))
            {
                uiSize = "1024";
            }

            if (!File.Exists(uiSize)) uiSize = "1024";
            ini.IniWriteValue("MACHINE", "UISIZE", uiSize);
        }
        public static void GetExternalTruck(ref string externalTruck)
        {
            string strIniFile = GetIniDirectory() + @"MachineInfo.ini"; // NCT
            Ini.IniFile ini = new Ini.IniFile(strIniFile);
            externalTruck = ini.IniReadValue("MACHINE", "ExternalTruck");
            if (String.IsNullOrEmpty(externalTruck))
            {
                ini.IniWriteValue("MACHINE", "ExternalTruck", "N");
            }
        }
        public static void GetYTAssigned(ref string ytAssigned)
        {
            string strIniFile = GetIniDirectory() + @"MachineInfo.ini"; // NCT
            Ini.IniFile ini = new Ini.IniFile(strIniFile);
            ytAssigned = ini.IniReadValue("MACHINE", "ytAssigned");
            if (String.IsNullOrEmpty(ytAssigned))
            {
                ini.IniWriteValue("MACHINE", "ytAssigned", "N");
            }
        }
        public static void GetJobTypeSortOrder(ref string jobType)
        {
            string strIniFile = GetIniDirectory() + @"MachineInfo.ini"; // NCT

            Ini.IniFile ini = new Ini.IniFile(strIniFile);
            jobType = ini.IniReadValue("JOBTYPESORTORDER", "AH");
            String LD = ini.IniReadValue("JOBTYPESORTORDER", "LD");
            String DS = ini.IniReadValue("JOBTYPESORTORDER", "DS");
            String LC = ini.IniReadValue("JOBTYPESORTORDER", "LC");
            String GI = ini.IniReadValue("JOBTYPESORTORDER", "GI");
            String GO = ini.IniReadValue("JOBTYPESORTORDER", "GO");
            String GC = ini.IniReadValue("JOBTYPESORTORDER", "GC");
            String MO = ini.IniReadValue("JOBTYPESORTORDER", "MO");
            String MI = ini.IniReadValue("JOBTYPESORTORDER", "MI");
            String RH = ini.IniReadValue("JOBTYPESORTORDER", "RH");

            if (jobType.Equals(string.Empty) || LD.Equals(string.Empty) || DS.Equals(string.Empty) || LC.Equals(string.Empty) || GI.Equals(string.Empty)
                || GO.Equals(string.Empty) || GC.Equals(string.Empty) || MO.Equals(string.Empty) || MI.Equals(string.Empty) || RH.Equals(string.Empty))
            {
                ini.IniWriteValue("JOBTYPESORTORDER", "AH", "1");
                ini.IniWriteValue("JOBTYPESORTORDER", "LD", "2");
                ini.IniWriteValue("JOBTYPESORTORDER", "DS", "3");
                ini.IniWriteValue("JOBTYPESORTORDER", "LC", "4");
                ini.IniWriteValue("JOBTYPESORTORDER", "GI", "5");
                ini.IniWriteValue("JOBTYPESORTORDER", "GO", "6");
                ini.IniWriteValue("JOBTYPESORTORDER", "GC", "7");
                ini.IniWriteValue("JOBTYPESORTORDER", "MO", "8");
                ini.IniWriteValue("JOBTYPESORTORDER", "MI", "9");
                ini.IniWriteValue("JOBTYPESORTORDER", "RH", "10");
            }
        }
        public static string GetIniDirectory()
        {
            string path = null;

            DirectoryInfo dInfo = Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory);
            string strParentPath = dInfo.Parent.FullName;
            string strCommonPath = strParentPath + @"\Common";

            if (!Directory.Exists(strCommonPath))
            {
                Directory.CreateDirectory(strCommonPath);
            }

            path = strCommonPath + @"\";

            return path;
        }
    }
        
}
