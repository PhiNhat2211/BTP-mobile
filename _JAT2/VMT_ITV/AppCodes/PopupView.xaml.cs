using System;
using System.Collections.Generic;
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
using System.Windows.Threading;

//20190108
using Common.Interface;

namespace VMT_ITV
{
	/// <summary>
	/// Interaction logic for uc_Popup.xaml
	/// </summary>
	public partial class uc_Popup : UserControl
	{
        private MainWindow mMainWindow;

        public delegate void Callback_Popup(int seleted);

        static private Callback_Popup callback_popup;

        private DispatcherTimer TimerStop = null;

        private String[] arrayMessageTemp;
        private int messCountTemp = 0;
        private List<String> listMessageFinal = new List<string>();
        private int messCountFinal = 0;
        private int messPage = 1;

        public uc_Popup()
		{
			this.InitializeComponent();
		}
        public void Init(MainWindow mainWin)
        {
            mMainWindow = mainWin;
        }

        public void StartAnimationTruck(int sec)
        {

            if (TimerStop == null)
            {
                TimerStop = new DispatcherTimer();
                TimerStop.Interval = new TimeSpan(0, 0, sec);
                TimerStop.Tick += new EventHandler(AnimationTimer_Handler);
                TimerStop.Start();
            }
        }

        public void StopAnimationTruck()
        {
            if (TimerStop != null)
                TimerStop.Stop();
            TimerStop = null;

            if (callback_popup != null)
                 callback_popup(0);
        }

        void AnimationTimer_Handler(object sender, EventArgs e)
        {
            StopAnimationTruck();
            HidePopup();
        }
        private void splitMessage()
        {
            listMessageFinal.Clear();
            foreach (String mess in arrayMessageTemp)
            {
                String messTrim = mess.Trim();
                if (messTrim.Length > 20)
                {
                    String mess1 = messTrim.Substring(0, 20);
                    String mess2 = messTrim.Substring(20);
                    if (mess1.Contains(" "))
                    {
                        listMessageFinal.Add(mess1.Substring(0, mess1.LastIndexOf(" ")));
                        listMessageFinal.Add(mess1.Substring(mess1.LastIndexOf(" ")) + mess2 + ".");
                    }
                    else
                    {
                        listMessageFinal.Add(mess1);
                        listMessageFinal.Add(mess2 + ".");
                    }                   
                }
                else if (messTrim.Length > 1)
                {
                    listMessageFinal.Add(messTrim + ".");
                }
            }
            messCountFinal = listMessageFinal.Count;
            SetMessageGrid();
        }
        public void SetMessageGrid()
        {
            this.TextBlock_popup_message.Text = String.Empty;
            this.Tbl_MessPage.Text = messPage + "/" + (messCountFinal / 3 + (messCountFinal % 3 > 0 ? 1 : 0));
            for (int i = 3 * messPage - 2; i <= 3 * messPage; i++)
            {
                if (i <= messCountFinal)
                {
                    this.TextBlock_popup_message.Text += listMessageFinal[i - 1];
                    if (i <= 3 * messPage - 1)
                        this.TextBlock_popup_message.Text += "\r\n";
                }
            }
        }
        public void ShowPopup( int type, String title, String message, String LeftButton, String CenterButton, String RightButton, Callback_Popup callback, int sec, int fontSize = 36)
        {
            if (mMainWindow.MainView.popupShowed == false)
            {
                TextBlock_popup_title.Text = title;
                TextBlock_popup_message.FontSize = fontSize;

                message = message.Trim('\n');
                arrayMessageTemp = message.Split('.');
                messCountTemp = arrayMessageTemp.Length;
                if (messCountTemp >= 4)
                {
                    Grid_MessScroll.Visibility = Visibility.Visible;
                    messPage = 1;
                    splitMessage();
                }
                else
                {
                    Grid_MessScroll.Visibility = Visibility.Hidden;
                    TextBlock_popup_message.Text = message;
                    listMessageFinal.Clear();
                    messCountFinal = 0;
                }
                Grid_OneButton.Visibility = System.Windows.Visibility.Hidden;
                Grid_TwoButton.Visibility = System.Windows.Visibility.Hidden;
                Grid_ThreeButton.Visibility = System.Windows.Visibility.Hidden;
                if (type == 0)
                {

                }
                else if (type == 1) // one button
                {
                    Grid_OneButton.Visibility = System.Windows.Visibility.Visible;
                    TextBlock_popup_Onebutton.Text = CenterButton;
                }
                else if (type == 2) // two button
                {
                    Grid_TwoButton.Visibility = System.Windows.Visibility.Visible;
                    TextBlock_TwoButton_Left.Text = LeftButton;
                    TextBlock_TwoButton_Right.Text = RightButton;
                }
                else
                {
                    Grid_ThreeButton.Visibility = System.Windows.Visibility.Visible;
                    TextBlock_ThreeButton_Left.Text = LeftButton;
                    TextBlock_ThreeButton_Center.Text = CenterButton;
                    TextBlock_ThreeButton_Right.Text = RightButton;
                }
                this.Visibility = System.Windows.Visibility.Visible;
                callback_popup = callback;

                if (sec > 0)
                {
                    StartAnimationTruck(sec);
                }
            }         
        }
        private void Btn_MessUp_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (messPage > 1 && messCountFinal > 0)
            {
                messPage -= 1;
                SetMessageGrid();
            }
        }
        private void Btn_MessDown_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (messPage < (messCountFinal / 3 + (messCountFinal % 3 > 0 ? 1 : 0)))
            {
                messPage += 1;
                SetMessageGrid();
            }
        }
        public void HidePopup()
        {
            this.Visibility = System.Windows.Visibility.Collapsed;
        }

        private void Grid_OneButton_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
        	
            Image_OK_OneButton.Source = mMainWindow.getImageByDayOrNightForPopup(@"popup_btn_02-1_default.png");
        }

        private void Grid_OneButton_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
        	
            Image_OK_OneButton.Source = mMainWindow.getImageByDayOrNightForPopup(@"popup_btn_02-1_press.png");
        }

        private void Grid_OneButton_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
        	
            Image_OK_OneButton.Source = mMainWindow.getImageByDayOrNightForPopup(@"popup_btn_02-1_default.png");
            mMainWindow.HidePopup();
            if (callback_popup != null)
                callback_popup(1);
        }

        private void Grid_TwoButton_Left_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
        	
            Image_TwoButton_left.Source = mMainWindow.getImageByDayOrNightForPopup(@"popup_btn_02-1_default.png");
        }

        private void Grid_TwoButton_Left_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
        	
            Image_TwoButton_left.Source = mMainWindow.getImageByDayOrNightForPopup(@"popup_btn_02-1_press.png");
        }

        private void Grid_TwoButton_Left_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
        	
            Image_TwoButton_left.Source = mMainWindow.getImageByDayOrNightForPopup(@"popup_btn_02-1_default.png");
            mMainWindow.HidePopup();
            if (callback_popup != null)
                callback_popup(0);
        }

        private void Grid_TwoButton_Right_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
        	
            Image_TwoButton_Right.Source = mMainWindow.getImageByDayOrNightForPopup(@"popup_btn_02-1_default.png");
        }

        private void Grid_TwoButton_Right_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
        	
            Image_TwoButton_Right.Source = mMainWindow.getImageByDayOrNightForPopup(@"popup_btn_02-1_press.png");
        }

        private void Grid_TwoButton_Right_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
        	
            Image_TwoButton_Right.Source = mMainWindow.getImageByDayOrNightForPopup(@"popup_btn_02-1_default.png");
            mMainWindow.HidePopup();
            if (callback_popup != null)
                callback_popup(2);
        }

        private void Grid_ThreeButton_Left_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
        	
            Image_ThreeButton_Left.Source = mMainWindow.getImageByDayOrNightForPopup(@"popup_btn_03-1_default.png");
        }

        private void Grid_ThreeButton_Left_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
        	
            Image_ThreeButton_Left.Source = mMainWindow.getImageByDayOrNightForPopup(@"popup_btn_03-1_press.png");
        }

        private void Grid_ThreeButton_Left_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
        	
            Image_ThreeButton_Left.Source = mMainWindow.getImageByDayOrNightForPopup(@"popup_btn_03-1_default.png");
            mMainWindow.HidePopup();
            if (callback_popup != null)
                callback_popup(0);
        }

        private void Grid_ThreeButton_Center_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
        	
            Image_ThreeButton_Center.Source = mMainWindow.getImageByDayOrNightForPopup(@"popup_btn_03-2_default.png");
        }

        private void Grid_ThreeButton_Center_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
        	
            Image_ThreeButton_Center.Source = mMainWindow.getImageByDayOrNightForPopup(@"popup_btn_03-2_press.png");
        }

        private void Grid_ThreeButton_Center_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
        	
            Image_ThreeButton_Center.Source = mMainWindow.getImageByDayOrNightForPopup(@"popup_btn_03-2_default.png");
            mMainWindow.HidePopup();
            if (callback_popup != null)
                callback_popup(1);
        }

        private void Grid_ThreeButton_Right_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
        	
            Image_ThreeButton_Right.Source = mMainWindow.getImageByDayOrNightForPopup(@"popup_btn_03-3_default.png");
        }

        private void Grid_ThreeButton_Right_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
        	
            Image_ThreeButton_Right.Source = mMainWindow.getImageByDayOrNightForPopup(@"popup_btn_03-3_press.png");
        }

        private void Grid_ThreeButton_Right_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
        	
            Image_ThreeButton_Right.Source = mMainWindow.getImageByDayOrNightForPopup(@"popup_btn_03-3_default.png");
            mMainWindow.HidePopup();
            if (callback_popup != null)
                callback_popup(2);
        }
      //  public void Show
	}
}