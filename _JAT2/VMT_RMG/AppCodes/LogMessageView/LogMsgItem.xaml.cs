using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace VMT_RMG
{
    /// <summary>
    /// Interaction logic for LogMsgItem.xaml
    /// </summary>
    public partial class LogMsgItem : UserControl
    {
        private Boolean _bExpanding = false;
        private String _StrLogMessage;
        private double _Padding = 2;

        public String StrLogMessage
        {
            set
            {
                this.TexBox_Date.Text = DateTime.Now.ToString("[HH:mm:ss:fff]");
                this.TexBox_Log.Text = value;
                _StrLogMessage = value;

                if( TexBoxLogSize().Height > this.TexBox_Log.MinHeight)
                    this.TexBox_DisplayExpanding.Text = "+";
                else
                    this.TexBox_DisplayExpanding.Text = "";
            }
            get
            {
                return _StrLogMessage;
            }
        }

        public LogMsgItem()
        {
            InitializeComponent();
        }

        private Size TexBoxLogSize()
        {
            FormattedText formattedText = new FormattedText(
                                        this.TexBox_Log.Text,
                                        System.Globalization.CultureInfo.CurrentUICulture,
                                        FlowDirection.LeftToRight,
                                        new Typeface(this.TexBox_Log.FontFamily, this.TexBox_Log.FontStyle, this.TexBox_Log.FontWeight, this.TexBox_Log.FontStretch),
                                        this.TexBox_Log.FontSize,
                                        Brushes.Black);

            return new Size(formattedText.Width, formattedText.Height);
        }

        public void Expading(Boolean bExpanding)
        {
            _bExpanding = bExpanding;

            if (bExpanding == false)
            {
                this.TexBox_Log.TextWrapping = TextWrapping.NoWrap;
                this.TexBox_Log.Height = this.TexBox_Log.MinHeight;
            }
            else
            {
                this.TexBox_Log.TextWrapping = TextWrapping.Wrap;
                this.TexBox_Log.Height = TexBoxLogSize().Height + _Padding;
            }
        }

        private void UserControl_GotFocus(object sender, System.Windows.RoutedEventArgs e)
        {	
            Expading(true);
        }

        private void UserControl_LostFocus(object sender, System.Windows.RoutedEventArgs e)
        {
            Expading(false);
        }
    }
}
