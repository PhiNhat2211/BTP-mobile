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
using System.Windows.Shapes;

namespace VMT_RMG_800by600
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

        public LogWindow()
        {
            this.InitializeComponent();

            // Insert code required on object creation below this point.
        }

        public void WriteLog(string log, bool newline = true)
        {
            if (this.ListBox_Log == null)
                return;

            this.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal,
                new Action(delegate
                {
                    while(this.ListBox_Log.Items.Count > 100)
                    {
                        this.ListBox_Log.Items.RemoveAt(0);
                    }

                    SolidColorBrush brush = new SolidColorBrush(Color_White);
                    String strSearch = this.TextBox_Search.Text;
                    if (!String.IsNullOrEmpty(strSearch) &&
                        log.Contains(strSearch))
                    {
                        brush = new SolidColorBrush(Color_Red);
                    }

                    LogMsgItem lItem = new LogMsgItem();
                    lItem.StrLogMessage = log;
                    lItem.TexBox_Log.Background = brush;
                    this.ListBox_Log.Items.Add(lItem);

                    if (_bPause == false)
                    {
                        this.ListBox_Log.Items.MoveCurrentToLast();
                        this.ListBox_Log.ScrollIntoView(this.ListBox_Log.Items.CurrentItem);
                    }
                }));
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
            this.ListBox_Log.Items.Clear();
        }

        private void TextBox_Search_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {	
            //if (Keyboard.Modifiers == ModifierKeys.Shift)
            if (e.Key != Key.Enter)
                return;

            Button_Search_Click(null, null);
        }

        private void Button_Search_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            String strSearch = this.TextBox_Search.Text;

            String[] searchPatterns = strSearch.Split('&');
            foreach (LogMsgItem lItem in this.ListBox_Log.Items)
            {
                Boolean bMatch = true;
                foreach (String searchPattern in searchPatterns)
                {
                    if (String.IsNullOrEmpty(searchPattern) &&
                        searchPatterns.Length == 1) // Clear
                    {
                            bMatch = false;
                            break;
                    }

                    if (!lItem.TexBox_Log.Text.Contains(searchPattern))
                    {
                        bMatch = false;
                        break;
                    }
                }

                if (bMatch == true)
                {
                    SolidColorBrush brush = new SolidColorBrush(Color_Red);
                    lItem.TexBox_Log.Background = brush;
                }
                else
                {
                    SolidColorBrush brush = new SolidColorBrush(Color_White);
                    lItem.TexBox_Log.Background = brush;
                }
            }
        }
    }
}