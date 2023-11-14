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
	/// Interaction logic for PopupProgressView.xaml
	/// </summary>
	public partial class PopupProgressView : UserControl
	{
        MainWindow mMainWindow = null;

        private DispatcherTimer TimerAniTruck = null;
        private DispatcherTimer TimerAutoHide= null;

        private DispatcherTimer TimerProgressBar = null;

        public int messgeWhat = 0;
        private int ProgressBar_Current = 0;
        private String[] progressImages = {"g_img_loading_play01.png",
                                  "g_img_loading_play02.png",
                                  "g_img_loading_play03.png",
                                  "g_img_loading_play04.png",
                                  "g_img_loading_play05.png",
                                  "g_img_loading_play06.png",
                                  "g_img_loading_play07.png",
                                  "g_img_loading_play08.png",
                                  "g_img_loading_play09.png",
                                  "g_img_loading_play10.png",
                                  "g_img_loading_play11.png",
                                  "g_img_loading_play12.png"};

        private int progressImageIndex = 0;


		public PopupProgressView()
		{
			this.InitializeComponent();
		}
        public void Init(MainWindow mainWin)
        {
            mMainWindow = mainWin;
            TimerAniTruck = new DispatcherTimer();
            TimerAniTruck.Interval = new TimeSpan(0, 0, 0, 0, 200);
            TimerAniTruck.Tick += new EventHandler(AnimationTimer_Handler);

            TimerAutoHide = new DispatcherTimer();
            TimerAutoHide.Interval = new TimeSpan(0, 0, 0, 0, 60000);
            TimerAutoHide.Tick += new EventHandler(AutoHideTimer_Handler);

            TimerProgressBar = new DispatcherTimer();
            TimerProgressBar.Interval = new TimeSpan(0, 0, 0, 1,0);
            TimerProgressBar.Tick += new EventHandler(ProgressBar_Timer_Handler);
            
        }
        public void StartAnimation(int messge, string text="")
        {
            Image_progress.Visibility = System.Windows.Visibility.Visible;
            Grid_ProgressBar.Visibility = System.Windows.Visibility.Collapsed;

            if (!string.IsNullOrEmpty(text))
            {
                this.Grid_TextMessage.Visibility = System.Windows.Visibility.Visible;
                this.TextBlock_Message.Text = text;
            }
            else
                this.Grid_TextMessage.Visibility = System.Windows.Visibility.Hidden;

            messgeWhat = messge;
            TimerAniTruck.Start();
            TimerAutoHide.Start();

            ProgressBar_Text_Copy.Text = PresentationMgr.Singleton.LanguageSer.GetResourceITV("LA0141", LanguageService.LABEL_CUSTOMIZE);
        }
        public void StopAnimation(int message)
        {
            if (messgeWhat == message)
            {
                if (TimerAniTruck != null)
                    TimerAniTruck.Stop();
                progressImageIndex = 0;
                messgeWhat = 0;
                this.TextBlock_Message.Text = "";

                if (TimerAutoHide != null)
                    TimerAutoHide.Stop();
            }
        }
        public void StartProgressBar_Timer(int sec)
        {
            Image_progress.Visibility = System.Windows.Visibility.Collapsed;
            Grid_ProgressBar.Visibility = System.Windows.Visibility.Visible;
            ProgressBar_Timer.Maximum = sec;
            ProgressBar_Current = 0;
            TimerProgressBar.Start();
           

        }
        public void StopProgrssBar_Timer()
        {
            TimerProgressBar.Stop();
        }
        void AnimationTimer_Handler(object sender, EventArgs e)
        {
            if (progressImageIndex >= progressImages.Length)
                progressImageIndex = 0;

            Image_progress.Source = mMainWindow.getImageByDayOrNightForProgress(progressImages[progressImageIndex]);
            progressImageIndex++;
        }
        void AutoHideTimer_Handler(object sender, EventArgs e)
        {
            mMainWindow.HideProgressBar(this.messgeWhat);
        }

        void ProgressBar_Timer_Handler(object sender, EventArgs e)
        {
            ProgressBar_Current++;

            ProgressBar_Timer.Value = ProgressBar_Current;
            ProgressBar_Text.Text = ((int)(ProgressBar_Current / ProgressBar_Timer.Maximum * 100)) + "%";
            if (ProgressBar_Timer.Maximum <= ProgressBar_Current)
            {
                mMainWindow.HideProgressBarTimer();
            }

        }

        private void Image_progress_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
        	
            if(Image_progress.Visibility == System.Windows.Visibility.Visible)
                mMainWindow.HideProgressBar(this.messgeWhat);

        }


	}
}