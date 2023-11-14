using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace VMT_ITV
{
    /// <summary>
    /// Interaction logic for LogWin.xaml
    /// </summary>
    public partial class LogWindow : Window
    {
        private Color Color_Black = Color.FromArgb(255, 0, 0, 0); // Black
        private Color Color_Red = Color.FromArgb(255, 255, 0, 0); // Red
        private Color Color_Green = Color.FromArgb(255, 0, 255, 0); // Green
        private Color Color_Blue = Color.FromArgb(255, 0, 0, 255); // Blue
        private Color Color_Yellow = Color.FromArgb(255, 255, 255, 0); // Yellow
        private Color Color_White = Color.FromArgb(255, 255, 255, 255); // White

        private Boolean _bPause = false;
        private String logStrOriginal = String.Empty;

        public LogWindow()
        {
            this.InitializeComponent();

            // Insert code required on object creation below this point.
        }

        public void WriteLog(string log, bool newline = true)
        {
            this.Dispatcher.Invoke(new Action(
                delegate ()
                {
                    if (this.TextBlock_Log == null)
                        return;
                    if (!_bPause)
                    {
                        if (newline)
                            log += "\r\n";

                        // Add Time Value
                        string now = DateTime.Now.ToString("HH:mm:ss.fff", System.Globalization.DateTimeFormatInfo.InvariantInfo);
                        log = now + " " + log;

                        string strLog = log;
                        strLog += logStrOriginal;

                        if (strLog.Length > 20000)
                            strLog = strLog.Substring(0, 20000);

                        logStrOriginal = strLog;
                        this.TextBlock_Log.Text = strLog;
                    }
                }
                ));
        }
        private void Button_Copy_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            Clipboard.SetText(TextBlock_Log.Text);
        }
        private void Button_Pause_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            _bPause = !_bPause;

            if (_bPause == true)
            {
                SolidColorBrush brush = new SolidColorBrush(Color_Red);
                this.Button_Pause.Foreground = brush;
            }
            else
            {
                SolidColorBrush brush = new SolidColorBrush(Color_Black);
                this.Button_Pause.Foreground = brush;
            }
        }
        private void Button_Clear_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            logStrOriginal = String.Empty;
            this.TextBlock_Log.Text = String.Empty;
        }
        private void TextBox_Search_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key != Key.Enter)
                return;

            Button_Search_Click(null, null);
        }

        private void Button_Search_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            String searchStr = this.TextBox_Search.Text;
            if (!String.IsNullOrEmpty(searchStr) && logStrOriginal.ToLower().Contains(searchStr.ToLower()))
            {
                if (!_bPause) // Not Paused
                {
                    Button_Pause_Click(null, null);
                }
                String[] stringArr = Regex.Split(logStrOriginal, "\r\n|\r|\n");
                TextBlock_Log.Text = String.Empty;

                foreach (String lineStr in stringArr)
                {
                    Run run = new Run(lineStr + Environment.NewLine);
                    if (lineStr.ToLower().Contains(searchStr.ToLower()))
                    {
                        run.Background = new SolidColorBrush(Color_Yellow);
                        TextBlock_Log.Inlines.Add(run);
                    }
                }
            }
            else
            {
                TextBlock_Log.Text = logStrOriginal;
            }
        }
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            // Hide Log Window
            this.Hide();
        }


    }
}