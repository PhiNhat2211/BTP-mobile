using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Reflection;

namespace TCPComm.EEStruct
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto, Pack = 1)]
    [Serializable]
    public class EEParentClass
    {
        public EEParentClass RelocateByteArray()
        {
            if (this == null)
                return null;

            FieldInfo[] fieldInfo = this.GetType().GetFields();
            foreach (FieldInfo f in fieldInfo)
            {
                if (f.FieldType == typeof(System.Byte[]) &&
                    f.GetValue(this) != null && f.GetValue(this) is Byte[])
                {
                    Object[] attributes = f.GetCustomAttributes(typeof(MarshalAsAttribute), false);
                    MarshalAsAttribute marshal = (MarshalAsAttribute)attributes[0];
                    int nCount = marshal.SizeConst;

                    Byte[] byteArray = f.GetValue(this) as Byte[];
                    Array.Resize(ref byteArray, nCount);
                    f.SetValue(this, byteArray);
                }
                else if (f.FieldType.BaseType == typeof(EEParentClass) && // Class 안에 Class 존재할때
                    f.GetValue(this) != null && f.GetValue(this) is EEParentClass)
                {
                    (f.GetValue(this) as EEParentClass).RelocateByteArray(); // recursive
                }
            }
            return this;
        }
    }
}
