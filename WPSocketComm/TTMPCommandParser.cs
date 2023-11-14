using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WPSocketComm.Network
{
    public class CTInfo
    {
        public CTInfo()
        {
        }

        public string Group { get; set; }
        public string ContentsName { get; set; }
        public string ContentsID { get; set; }
        public int Row;
        public int Column;
        public string Description { get; set; }
        public string Author { get; set; }
        public long FrameCount;
        public string ContentsType { get; set; }
        public int ItemIndex { get; set; }

        public string Offset { get; set; }
        public string Duration { get; set; }
    }

    public class TTMPCommand
    {
        public ENMessageType MessageType
        {
            get { return GetMessageType(); }
        }

        public string Message
        {
            get { return _Message; }
        }

        public int ParamCount
        {
            get { return _ParamDic.Count; }
        }

        public enum ENMessageType 
        {
            MT_Unknown, MT_Command, MT_Acknoledge 
        };

        private string _Message;
        private Dictionary<string, string> _ParamDic = new Dictionary<string, string>();
        
        public bool Parse(string args)
        {

            if (args.IndexOf("ttcmd") < 0 && args.IndexOf("ttack") < 0)
                return false;

            // Extract Command and Value 
            string value;

            //--------------------------------------
            //- Parameter exist
            if (args.IndexOf(" ") < 0)
            {
                _Message = args.Substring(0, args.IndexOf("\r\n"));
                value = string.Empty;
            }
            //--------------------------------------
            //- Parameter none exist
            else
            {
                _Message = args.Substring(0, args.IndexOf(" "));
                value = args.Substring(args.IndexOf(" ") + 1, args.Length - (args.IndexOf(" ") + 1));
                value = value.Replace(TTMPCmdData.ENDLINE, string.Empty);
            }

            // Set Parameters
            if (!String.IsNullOrEmpty(value))
            {
                string[] eventParams = value.Split('|');


                if ((eventParams.Length >= 1) && (eventParams.Length <= 100))
                {
                    for (int cntLoop = 0; cntLoop < eventParams.Length; cntLoop++)
                    {
                        string[] strParam = eventParams[cntLoop].Split('=');

                        if (strParam.Length > 1)
                        {
                            string arg = string.Empty;
                            for (int i = 1; i < strParam.Length; i++)
                            {
                                arg += strParam[i];
                                if (i > 1 && i < strParam.Length - 1)
                                    arg += "=";
                            }

                            _ParamDic.Add(strParam[0], arg);
                        }
                        else
                            _ParamDic.Add(strParam[0], string.Empty);
                    }
                }
            }

            return true;
        }

        public ENMessageType GetMessageType()
        {
            if (_Message.ToLower() == "ttcmd")
                return ENMessageType.MT_Command;
            else if (_Message.ToLower() == "ttack")
                return ENMessageType.MT_Acknoledge;
            else
                return ENMessageType.MT_Unknown;

        }

        public string GetValue(string key)
        {
            if (!_ParamDic.ContainsKey(key))
                return string.Empty;

            return _ParamDic[key];
        }

        public static bool ParsingContentsInfoFromID(CTInfo ctinfo, string contentsID)
        {           
            string[] strTokens = contentsID.Split('_');

            if (strTokens.Length < 5)
                return false;

            int lastIndex = strTokens.Length - 1;

            //-------------------------------------
            //- Set ContentsID
            ctinfo.ContentsID = contentsID;

            //--------------------------------------
            //- Set Group
            ctinfo.Group = strTokens[0];

            //--------------------------------------
            //- Set Framecount
            string fcnt = strTokens[lastIndex];
            fcnt = fcnt.TrimStart('[');
            fcnt = fcnt.TrimEnd(']');
            long.TryParse(fcnt, out ctinfo.FrameCount);

            //-------------------------------------
            //- Set DisplayMatrix
            string matrix = strTokens[lastIndex - 1];
            matrix = matrix.ToLower();
            string[] rowColumn = matrix.Split('x');
            int.TryParse(rowColumn[0], out ctinfo.Row);
            int.TryParse(rowColumn[1], out ctinfo.Column);

            //-------------------------------------
            //- Set ContentsType
            string ctype = strTokens[lastIndex - 2];
            ctinfo.ContentsType = ctype;

            //------------------------------------
            //- Set Title
            string title = string.Empty;
            for (int j = 1; j < strTokens.Length - 3; j++)
            {
                if (title.Length > 0)
                    title += "_";
                title += strTokens[j];
            }
            ctinfo.ContentsName = title;


            return true;
        }
    }
}
