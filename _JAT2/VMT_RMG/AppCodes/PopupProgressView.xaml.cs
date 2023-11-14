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

namespace VMT_RMG
{
	/// <summary>
	/// Interaction logic for PopupProgressView.xaml
	/// </summary>
	public partial class PopupProgressView : UserControl
	{
        MainWindow mMainWindow = null;

        private DispatcherTimer TimerAniTruck = null;
        private DispatcherTimer TimerAutoHide= null;

        public int messgeWhat = 0;

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
            //TimerAutoHide.Interval = new TimeSpan(0, 0, 0, 0, 60000);
            TimerAutoHide.Interval = new TimeSpan(0, 0, 0, 0, 30000);
            TimerAutoHide.Tick += new EventHandler(AutoHideTimer_Handler);
            
        }

        public void ShowProgressText(Boolean isShow, String text="")
        {            
            this.ProgressBar_Text.Text = text;
            this.ProgressBar_Text.Visibility = isShow ? Visibility.Visible : Visibility.Hidden;
        }

        public void StartAnimation(int messge)
        {
            messgeWhat = messge;
            TimerAniTruck.Start();
            TimerAutoHide.Start();
        }
        public void StopAnimation(int message)
        {
            if (messgeWhat == message)
            {
                if (TimerAniTruck != null)
                    TimerAniTruck.Stop();
                progressImageIndex = 0;
                messgeWhat = 0;

                if (TimerAutoHide != null)
                    TimerAutoHide.Stop();
            }
        }

        void AnimationTimer_Handler(object sender, EventArgs e)
        {
            if (progressImageIndex >= progressImages.Length)
                progressImageIndex = 0;

            Image_progress.Source = PresentationMgr.GetImageSource(@"/Images/PopupProgress/" + progressImages[progressImageIndex]);//mMainWindow.getImageByDayOrNightForProgress(progressImages[progressImageIndex]);
            progressImageIndex++;
        }

        void AutoHideTimer_Handler(object sender, EventArgs e)
        {
            mMainWindow.HideProgressBar(this.messgeWhat);
        }

        private void Image_progress_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {        	
#if DEBUG
            mMainWindow.HideProgressBar(this.messgeWhat);
#endif
        }


	}
}