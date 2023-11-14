using System;
using System.IO;

namespace hessiancsharp.io
{  
    public class CDecimalDeserializer : AbstractDeserializer
    {
        public override object ReadMap(AbstractHessianInput abstractHessianInput)
        {
            string strInitValue = null;

            while (!abstractHessianInput.IsEnd())
            {
                string strKey = abstractHessianInput.ReadString();
                string strValue = abstractHessianInput.ReadString();

                if (strKey.Equals("value"))
                    strInitValue = strValue;
            }

            abstractHessianInput.ReadMapEnd();

            if (strInitValue == null)
                throw new IOException("No value found for decimal.");

            return Decimal.Parse(strInitValue);
        }
    }
}
