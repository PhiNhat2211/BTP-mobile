using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows.Controls;

namespace VMT_RMG_800by600
{
    public class IntToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string strValue = value.ToString();
            return strValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is string)
            {
                int intValue = -1;
                if (int.TryParse((string)value, out intValue))
                {
                    return intValue;
                }
                
            }

            return -1;
        }
    }


    public class IndexLookupConverter : IMultiValueConverter
    {
        public object Convert(object[] value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            JobListItem lbi = value[0] as JobListItem;
            ListBox lb = value[1] as ListBox;

            int idx = lb.Items.IndexOf(lbi) + 1;

            return idx.ToString();
        }

        public object[] ConvertBack(object value, Type[] targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return null;
        }
    }

}
