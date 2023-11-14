using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Windows;
using System.Threading;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Diagnostics;
using System.IO;
using System.Windows.Controls.Primitives;

//20190108
using Common;
using Common.Interface;

namespace VMT_ITV
{

    #region [ Presentation Manger Custom Routed Event Handler Defintion ]

    // UPdateUIEvent Event Handler Argument Class
    public class UpdateUIEventArgs : RoutedEventArgs
    {
        public UpdateUIEventArgs() { }
        public UpdateUIEventArgs(RoutedEvent routedEvent)
        {
            RoutedEvent = routedEvent;
        }
        public UpdateUIEventArgs(RoutedEvent routedEvent, object source)
        {
            RoutedEvent = routedEvent;
            Source = source;
        }

        public object Param { get; set; }
    }

    // UPdateDataEvent Event Handler Argument Class
    public class UpdateDataEventArgs : RoutedEventArgs
    {
        public UpdateDataEventArgs() { }
        public UpdateDataEventArgs(RoutedEvent routedEvent)
        {
            RoutedEvent = routedEvent;
        }
        public UpdateDataEventArgs(RoutedEvent routedEvent, object source)
        {
            RoutedEvent = routedEvent;
            Source = source;
        }

        public object Param { get; set; }
    }

    // UpdateUIEvent Handler Delegate
    public delegate void UpdateUIEventHandler(object sender, UpdateUIEventArgs e);
    // UpdateDataEvent Handler Delegate
    public delegate void UpdateDataEventHandler(object sender, UpdateDataEventArgs e);

    #endregion

    public class PresentationMgr : UIElement
    {

        #region [ Custom Routed Evet Registration ]

        public static readonly RoutedEvent UpdateUIEvent =
            EventManager.RegisterRoutedEvent("UpateUI", RoutingStrategy.Bubble, typeof(UpdateUIEventHandler), typeof(PresentationMgr));
        public static readonly RoutedEvent UpdateDataEvent =
            EventManager.RegisterRoutedEvent("UpdateData", RoutingStrategy.Bubble, typeof(UpdateDataEventHandler), typeof(PresentationMgr));

        public event UpdateUIEventHandler UpdateUI
        {
            add { AddHandler(PresentationMgr.UpdateUIEvent, value); }
            remove { RemoveHandler(PresentationMgr.UpdateUIEvent, value); }
        }

        public event UpdateDataEventHandler UpdateData
        {
            add { AddHandler(PresentationMgr.UpdateDataEvent, value); }
            remove { RemoveHandler(PresentationMgr.UpdateDataEvent, value); }
        }

        #endregion

        //--------------------------------------------------------------
        //- Change Image Day <-> Night 
        //--------------------------------------------------------------
        public static SolidColorBrush brush_view_bg_color_day = new SolidColorBrush(Color.FromArgb(255, 239, 242, 243));
        public static SolidColorBrush brush_view_bg_color_night = new SolidColorBrush(Color.FromArgb(255, 21, 23, 21));

        public static SolidColorBrush brush_main_statusbar_bg_color_night = new SolidColorBrush(Color.FromArgb(255, 55, 57, 54));
        public static SolidColorBrush brush_main_statusbar_bg_color_day = new SolidColorBrush(Color.FromArgb(255, 195, 200, 203));

        // Text Color
        public static SolidColorBrush brush_text_white = new SolidColorBrush(Color.FromArgb(255, 255, 255, 255));
        public static SolidColorBrush brush_text_black = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));
        public static SolidColorBrush brush_text_gray = new SolidColorBrush(Color.FromArgb(255, 100, 100, 100));

        public static SolidColorBrush brush_text_gray_day = new SolidColorBrush(Color.FromArgb(255, 71, 71, 71));
        public static SolidColorBrush brush_text_gray_night = new SolidColorBrush(Color.FromArgb(255, 187, 187, 187));

        // Default Application Configuration DataSet
        private Hashtable uiComponentTable = null;

        private static readonly PresentationMgr _theOnly = null;

        private LanguageService languageSer = new LanguageService();

        public LanguageService LanguageSer
        {
            get { return this.languageSer; }
        }

        public static MainWindow AppWin
        {
            get { return Application.Current.MainWindow as MainWindow; }
        }

        public static MainView MainView
        {
            get
            {
                return PresentationMgr.Singleton.GetUIComponent("VMT_ITV.MainView") as MainView;
            }
        }

        public static PresentationMgr Singleton
        {
            get { return _theOnly; }
        }

        static PresentationMgr()
        {
            _theOnly = new PresentationMgr();
        }

        private PresentationMgr()
        {
            uiComponentTable = new Hashtable();

            // Register Custom Routed Event

        }

        public String CurrentBlock = "";

        public String CurrentBay = "";

        // Add UIContent Object to UIComponent Dictionary
        public void AddUIComponent(string keyName, Object uiObj)
        {
            if (!uiComponentTable.ContainsKey(keyName))
                uiComponentTable.Add(keyName, uiObj);
        }

        // Get UIComponent Object by Keyname
        public Object GetUIComponent(string keyName)
        {
            return uiComponentTable[keyName];
        }

        // Remove UIComponent Object by Keyname
        public void RemoveUIComponent(string keyName)
        {
            uiComponentTable.Remove(keyName);
        }


        //-----------------------------------------------------------------
        //- Application Common Methods Section
        //-----------------------------------------------------------------
        #region [ Application Common Methods ]

        public static void APP_CloseApp()
        {
            PresentationMgr.AppWin.SaveAppCfg();

            App.Current.Shutdown();
            Environment.Exit(0);
        }

        public static void App_SystemRestart()
        {
            System.Diagnostics.Process.Start("ShutDown", "/r /f /t 1"); //restart
        }

        #endregion


        public static T FindChild<T>(DependencyObject depObj, string childName)
            where T : DependencyObject
        {
            // Confirm obj is valid. 
            if (depObj == null) return null;

            // success case
            if (depObj is T && ((FrameworkElement)depObj).Name == childName)
                return depObj as T;

            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(depObj, i);

                //DFS
                T obj = FindChild<T>(child, childName);

                if (obj != null)
                    return obj;
            }

            return null;
        }

        public static int FindChildByType<T>(DependencyObject depObj, List<T> objList)
            where T : DependencyObject
        {
            // Confirm obj is valid. 
            if (depObj == null) return 0;

            // success case
            if (depObj is T)
            {
                objList.Add(depObj as T);
                return objList.Count;
            }

            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(depObj, i);

                //DFS
                int findCount = FindChildByType<T>(child, objList);
            }

            return objList.Count;
        }


        // Update Day&Night Mode
        public void ChangeDayMode(bool bMode)
        {
            LogInView uiLogInView = GetUIComponent("VMT_ITV.LogInView") as LogInView;
            MainView uiMainView = GetUIComponent("VMT_ITV.MainView") as MainView;
            Indicator uiIndicator = GetUIComponent("VMT_ITV.Indicator") as Indicator;


            //--------------------------------------------------------------
            //- Change Image Day <-> Night

            List<Image> imageList = new List<Image>();

            FindChildByType<Image>((DependencyObject)uiLogInView, imageList);
            FindChildByType<Image>((DependencyObject)uiMainView, imageList);
            FindChildByType<Image>((DependencyObject)uiIndicator, imageList);

            if (bMode)      // Day Mode
            {
                foreach (Image img in imageList)
                {
                    String srcUri = "";

                    if (img.Source is BitmapFrame)
                        srcUri = ((BitmapFrame)img.Source).Decoder.ToString();
                    else if (img.Source is BitmapImage)
                        srcUri = ((BitmapImage)img.Source).UriSource.ToString();

                    int idxName = srcUri.LastIndexOf('/') + 1;
                    String nightName = srcUri.Substring(idxName, srcUri.Length - idxName);

                    if (srcUri.IndexOf("(night)_") < 0)
                        continue;

                    String dayName = nightName.Replace("(night)_", "");
                    srcUri = srcUri.Replace(@"/night/", @"/day/");
                    srcUri = srcUri.Replace(nightName, dayName);
                    srcUri = srcUri.Replace(@"pack://application:,,,", @"");

                    try
                    {
                        BitmapImage bitmap = new BitmapImage(new Uri(srcUri, UriKind.Relative));
                        img.Source = bitmap;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }

            }
            else            // Night Mode
            {
                foreach (Image img in imageList)
                {
                    String srcUri = "";

                    if (img.Source is BitmapFrame)
                        srcUri = ((BitmapFrame)img.Source).Decoder.ToString();
                    else if (img.Source is BitmapImage)
                        srcUri = ((BitmapImage)img.Source).UriSource.ToString();

                    int idxName = srcUri.LastIndexOf('/') + 1;
                    String dayName = srcUri.Substring(idxName, srcUri.Length - idxName);

                    if (srcUri.Length <= 0)
                        continue;

                    if (srcUri.IndexOf("(night)") >= 0)
                        continue;

                    String nightName = "(night)_" + dayName;
                    srcUri = srcUri.Replace(@"/day/", @"/night/");
                    srcUri = srcUri.Replace(dayName, nightName);

                    // Make Absolute Uri to detect Uri Exception
                    if (srcUri.IndexOf("pack://application:,,,") < 0)
                        srcUri = "pack://application:,,," + srcUri;

                    try
                    {
                        BitmapImage bitmap = new BitmapImage(new Uri(srcUri));
                        img.Source = bitmap;
                    }
                    catch (Exception ex)
                    {
                        //String strMsg = String.Format("{0}\r\n,{1}", ex.Message, srcUri);
                        //MessageBox.Show(strMsg);
                    }


                }
            }




            if (bMode)
            {
                //----------------------------------------------------------
                //- LogIn
                uiLogInView.Border_LogInView.Background = brush_view_bg_color_day;

                uiLogInView.Label_IDNumber.Foreground = brush_text_gray_day;
                uiLogInView.Label_Password.Foreground = brush_text_gray_day;
                uiLogInView.Label_Name.Foreground = brush_text_gray_day;
                uiLogInView.Label_Team.Foreground = brush_text_gray_day;
                uiLogInView.Label_ScreenMode.Foreground = brush_text_gray_day;

                uiLogInView.Label_Btn_LogIn.Foreground = brush_text_white;
                uiLogInView.Label_Btn_Reset.Foreground = brush_text_white;

                //----------------------------------------------------------
                //- Chassis Attached
                uiLogInView.Border_ChassisAttached.Background = brush_view_bg_color_day;
                uiLogInView.Boarder_AfterChassisAttach.Background = brush_view_bg_color_day;
                uiLogInView.Label_Btn_Yes.Foreground = brush_text_black;
                uiLogInView.Label_Btn_No.Foreground = brush_text_black;
                uiLogInView.Label_Btn_Continue.Foreground = brush_text_black;

                //----------------------------------------------------------
                //- Chassis Selection
                uiLogInView.Border_ChassisSelection.Background = brush_view_bg_color_day;
                uiLogInView.Lebel_SelectYourChassis.Foreground = brush_text_black;
                uiLogInView.Label_Btn_Cancel.Foreground = brush_text_black;
                uiLogInView.Label_Btn_OK.Foreground = brush_text_black;
                uiLogInView.TextBlock_Name.Foreground = brush_text_black;
                //----------------------------------------------------------
                //- Main View
                //uiMainView.Border_Main.Background = brush_view_bg_color_day;
                //uiMainView.TextBlock_speed.Foreground = brush_text_black;
                //uiMainView.Border_Statusbar.Background = brush_main_statusbar_bg_color_day;
                //uiMainView.Border_break.Background = brush_view_bg_color_day;
                //uiMainView.Border_Available.Background = brush_view_bg_color_day;


                uiMainView.Grid_Fore.Background = brush_view_bg_color_day;
                uiMainView.Grid_After.Background = brush_view_bg_color_day;
                uiMainView.Grid_Bottom.Background = brush_view_bg_color_day;
                uiMainView.Grid_Button.Background = brush_view_bg_color_day;

                uiMainView.lbl_positionF.Foreground = brush_text_black;
                uiMainView.lbl_positionA.Foreground = brush_text_black;
                uiMainView.lbl_Hatch_Job.Foreground = brush_text_black;


                // uiMainView.TextBox_KPI.Foreground = brush_text_black;
                uiMainView.TextBlock_ID.Foreground = brush_text_black;

                //uiMainView.TextBlock_JobCode_20F.Foreground = brush_text_black;
                //uiMainView.TextBlock_JobCode_40F_center.Foreground = brush_text_black;
                //uiMainView.TextBlock_JobCode_40F_double_back.Foreground = brush_text_black;
                //uiMainView.TextBlock_JobCode_40F_double_front.Foreground = brush_text_black;
                //uiMainView.TextBlock_JobCode_40F_normal.Foreground = brush_text_black;

                //uiMainView.TextBlock_Location_40F_center.Foreground = brush_text_black;
                //uiMainView.TextBlock_Location_40F_double_back.Foreground = brush_text_black;
                //uiMainView.TextBlock_Location_40F_double_front.Foreground = brush_text_black;
                //uiMainView.TextBlock_Location_40F_normal.Foreground = brush_text_black;

                //foreach(UIElement uiElement in uiMainView.Wrap_Available.Children)
                //{
                //    if (uiElement is AvailableItem)
                //    {
                //        ((AvailableItem)uiElement).TextBlock_AvailableItem.Foreground = brush_text_black;
                //    }
                //}

                //uiMainView.TextBlock_ScreenMode_Cancel.Foreground = brush_text_black;
                //uiMainView.TextBlock_ScreenMode_OK.Foreground = brush_text_black;

                //uiMainView.TextBlock_break_Cancel.Foreground = brush_text_black;
                //uiMainView.TextBlock_break_Complete.Foreground = brush_text_black;

                //uiMainView.TextBlock_Start_Time.Foreground = brush_text_black;
                //uiMainView.TextBlock_End_Time.Foreground = brush_text_black;
                //----------------------------------------------------------
                //- Indecator View
                uiIndicator.Label_time.Foreground = brush_text_black;
                if (uiLogInView.Visibility == System.Windows.Visibility.Visible)
                {
                    uiIndicator.TextBox_Version.Foreground = brush_text_white;
                    uiIndicator.TextBox_Job.Foreground = brush_text_white;
                }
                else
                {
                    //uiIndicator.TextBox_Version.Foreground = brush_text_black;
                    uiIndicator.TextBox_Job.Foreground = brush_text_black;
                }
            }
            else
            {
                //----------------------------------------------------------
                //- LogInView
                uiLogInView.Border_LogInView.Background = brush_view_bg_color_night;

                uiLogInView.Label_IDNumber.Foreground = brush_text_gray_night;
                uiLogInView.Label_Password.Foreground = brush_text_gray_night;
                uiLogInView.Label_Name.Foreground = brush_text_gray_night;
                uiLogInView.Label_Team.Foreground = brush_text_gray_night;
                uiLogInView.Label_ScreenMode.Foreground = brush_text_gray_night;

                uiLogInView.Label_Btn_LogIn.Foreground = brush_text_gray;
                uiLogInView.Label_Btn_Reset.Foreground = brush_text_gray;

                //----------------------------------------------------------
                //- Chassis Attached
                uiLogInView.Border_ChassisAttached.Background = brush_view_bg_color_night;
                uiLogInView.Boarder_AfterChassisAttach.Background = brush_view_bg_color_night;

                uiLogInView.Label_Btn_Yes.Foreground = brush_text_white;
                uiLogInView.Label_Btn_No.Foreground = brush_text_white;
                uiLogInView.Label_Btn_Continue.Foreground = brush_text_white;
                uiLogInView.TextBlock_Name.Foreground = brush_text_white;
                //----------------------------------------------------------
                //- Chassis Selection
                uiLogInView.Border_ChassisSelection.Background = brush_view_bg_color_night;
                uiLogInView.Lebel_SelectYourChassis.Foreground = brush_text_white;
                uiLogInView.Label_Btn_Cancel.Foreground = brush_text_white;
                uiLogInView.Label_Btn_OK.Foreground = brush_text_white;
                //uiMainView.Border_break.Background = brush_view_bg_color_night;
                ////----------------------------------------------------------
                ////- Main View
                //uiMainView.Border_Main.Background = brush_view_bg_color_night;
                //uiMainView.TextBlock_speed.Foreground = brush_text_white;
                //uiMainView.Border_Statusbar.Background = brush_main_statusbar_bg_color_night;
                //uiMainView.Border_Available.Background = brush_view_bg_color_night;


                uiMainView.Grid_Fore.Background = brush_view_bg_color_night;
                uiMainView.Grid_After.Background = brush_view_bg_color_night;
                uiMainView.Grid_Bottom.Background = brush_view_bg_color_night;
                uiMainView.Grid_Button.Background = brush_view_bg_color_night;

                uiMainView.lbl_positionF.Foreground = brush_text_white;
                uiMainView.lbl_positionA.Foreground = brush_text_white;
                uiMainView.lbl_Hatch_Job.Foreground = brush_text_white;


                // uiMainView.TextBox_KPI.Foreground = brush_text_white;
                uiMainView.TextBlock_ID.Foreground = brush_text_white;

                //uiMainView.TextBlock_JobCode_20F.Foreground = brush_text_white;
                //uiMainView.TextBlock_JobCode_40F_center.Foreground = brush_text_white;
                //uiMainView.TextBlock_JobCode_40F_double_back.Foreground = brush_text_white;
                //uiMainView.TextBlock_JobCode_40F_double_front.Foreground = brush_text_white;
                //uiMainView.TextBlock_JobCode_40F_normal.Foreground = brush_text_white;

                //uiMainView.TextBlock_Location_40F_center.Foreground = brush_text_white;
                //uiMainView.TextBlock_Location_40F_double_back.Foreground = brush_text_white;
                //uiMainView.TextBlock_Location_40F_double_front.Foreground = brush_text_white;
                //uiMainView.TextBlock_Location_40F_normal.Foreground = brush_text_white;

                //foreach (UIElement uiElement in uiMainView.Wrap_Available.Children)
                //{
                //    if (uiElement is AvailableItem)
                //    {
                //        ((AvailableItem)uiElement).TextBlock_AvailableItem.Foreground = brush_text_black;
                //    }
                //}

                //uiMainView.TextBlock_ScreenMode_Cancel.Foreground = brush_text_white;
                //uiMainView.TextBlock_ScreenMode_OK.Foreground = brush_text_white;

                //uiMainView.TextBlock_Start_Time.Foreground = brush_text_white;
                //uiMainView.TextBlock_End_Time.Foreground = brush_text_white;
                //uiMainView.TextBlock_break_Cancel.Foreground = brush_text_white;
                //uiMainView.TextBlock_break_Complete.Foreground = brush_text_white;

                uiIndicator.Label_time.Foreground = brush_text_white;
                uiIndicator.TextBox_Version.Foreground = brush_text_white;
                uiIndicator.TextBox_Job.Foreground = brush_text_white;
            }
        }

        //public void ChangDayOrNight()
        //{
        //    //  DependencyObject foundChild = null;
        //    DependencyObject reference = this.Grid_Indicator_Job;
        //    if (reference != null)
        //    {
        //        int childrenCount = VisualTreeHelper.GetChildrenCount(reference);
        //        for (int i = 0; i < childrenCount; i++)
        //        {
        //            var child = VisualTreeHelper.GetChild(reference, i);
        //            // If the child is not of the request child type child
        //            if (child is Image)
        //            {
        //                // recursively drill down the tree
        //                Image tempImage = (Image)child;

        //                String tempString = ((BitmapFrame)tempImage.Source).Decoder.ToString();

        //                //   tempImage.Source = 
        //                //   foundChild = FindChild(child, childName, childType);
        //            }

        //        }
        //    }

        //}


        public static void ThreadStartingSplash()
        {
            //PleaseWaitDialog waitDlg = new PleaseWaitDialog();
            //waitDlg.Topmost = true;
            //waitDlg.ShowInTaskbar = false;
            ////IntPtr ownerWindowHandle = new WindowInteropHelper(m_splash).Handle;
            ////// Set the owned WPF window’s owner with the non-WPF owner window
            ////WindowInteropHelper helper = new WindowInteropHelper(m_splash);
            ////helper.Owner = ownerWindowHandle;

            //MainFrame mf = PresentationMgr.Singleton.GetUIComponent("MPlayer.MainFrame") as MainFrame;
            //if ( mf != null && mf.IsVisible)
            //{
            //    waitDlg.WindowStartupLocation = WindowStartupLocation.Manual;
            //    double left = mf.normalWinLeft + (mf.normalWinWidth - waitDlg.ActualWidth) / 2;
            //    double top = mf.normalWinTop + (mf.normalWinHeight - waitDlg.ActualHeight) / 2;
            //    waitDlg.Left = left; 
            //    waitDlg.Top = top - 100; 
            //}

            //waitDlg.ShowDialog();
            //Thread.Sleep(1000);
        }

        public static void ExecuteProcess(string fileName, string arg, bool bShow = false)
        {
            //-----------------------------------------
            //- Run ContentPresenter with ContentName

            Process cp = new Process();

            if (string.IsNullOrEmpty(fileName))
                return;

            String filePath = AppCfgMgr.GetAppDirectory() + fileName;

            FileInfo fi = new FileInfo(filePath);

            if (!fi.Exists)
                return;

            string dir = fi.DirectoryName;

            cp.StartInfo.WorkingDirectory = dir;
            cp.StartInfo.FileName = filePath;
            cp.StartInfo.Arguments = arg;

            if (!bShow)
                cp.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;

            try
            {
                cp.Start();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        public static void App_RestartApp()
        {
            System.Windows.Forms.Application.Restart();
        }

        public static void FileTouchEvent_RestartApp()
        {
            //--------------------------------------------------------------
            //- Save Touch Event for CLTAgent to restart application
            Microsoft.Win32.RegistryKey keyDir = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(MainWindow.KeyCLTVMT_ITV);
            String CLTVMT_ITVPath = "";
            string moduleDir = AppCfgMgr.GetAppDirectory();
            moduleDir.Replace("\"", "");

            if (keyDir != null)
            {
                CLTVMT_ITVPath = (String)keyDir.GetValue("InstallDir", moduleDir);
                CLTVMT_ITVPath = CLTVMT_ITVPath.Replace("\"", "");
            }

            if (String.IsNullOrEmpty(CLTVMT_ITVPath))
                CLTVMT_ITVPath = moduleDir;

            CLTVMT_ITVPath += @"\VMT_ITV.exe";

            String touchCmd = "EXECUTE" + "|" + CLTVMT_ITVPath;

            String TouchFile = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            TouchFile += @"\CLTConnect";
            System.IO.Directory.CreateDirectory(TouchFile);
            TouchFile += @"\CLTAgent.touch";

            // Write the string to a file.
            System.IO.StreamWriter file = new System.IO.StreamWriter(TouchFile);
            file.WriteLine(touchCmd);

            file.Close();
        }

        public static void FileTouchEvent_RunUpdate()
        {
            //--------------------------------------------------------------
            //- Save Touch Event for CLTAgent to run live update
            String touchCmd = "RUNLIVEUPDATE";

            String TouchFile = Environment.GetFolderPath(Environment.SpecialFolder.Personal);

            TouchFile += @"\CLTConnect";
            System.IO.Directory.CreateDirectory(TouchFile);
            TouchFile += @"\CLTAgent.touch";

            // Write the string to a file.
            System.IO.StreamWriter file = new System.IO.StreamWriter(TouchFile);
            file.WriteLine(touchCmd);

            file.Close();
        }

        public class SingleShot
        {
            public delegate void SingleShotCallBack();
            private SingleShotCallBack m_callBackFunction;

            public SingleShot(int intervalMilliseconds, SingleShotCallBack CallBackFunction)
            {
                System.Timers.Timer m_timer = new System.Timers.Timer(intervalMilliseconds);
                m_callBackFunction = CallBackFunction;
                m_timer.Elapsed += new System.Timers.ElapsedEventHandler(timerElapsedFunc);
                // m_timer.Interval = intervalMilliseconds;
                m_timer.AutoReset = false;
                m_timer.Start();
            }

            ~SingleShot() { }

            public void timerElapsedFunc(object sender, System.Timers.ElapsedEventArgs e)
            {
                // m_timer.Stop();
                // m_timer.Dispose();            
                m_callBackFunction();
            }
        }

        public static void SetSkinToggleButton(ToggleButton btnObj, BitmapImage bmpDef, BitmapImage bmpPress, BitmapImage bmpDisable = null)
        {
            Image img = null;

            img = btnObj.Template.FindName("Image_Default", btnObj) as Image;
            if (img != null) img.Source = bmpDef;

            img = btnObj.Template.FindName("Image_Press", btnObj) as Image;
            if (img != null) img.Source = bmpPress;

            img = btnObj.Template.FindName("Image_Disable", btnObj) as Image;
            if (img != null) img.Source = bmpDisable;
        }

        public static void SetSkinButton(Button btnObj, BitmapImage bmpDef, BitmapImage bmpPress, BitmapImage bmpDisable = null)
        {
            Image img = null;

            img = btnObj.Template.FindName("Image_Default", btnObj) as Image;
            if (img != null) img.Source = bmpDef;

            img = btnObj.Template.FindName("Image_Press", btnObj) as Image;
            if (img != null) img.Source = bmpPress;

            img = btnObj.Template.FindName("Image_Disable", btnObj) as Image;
            if (img != null) img.Source = bmpDisable;

        }

        public static void SetSkinRadioButton(RadioButton btnObj, BitmapImage bmpCheck, BitmapImage bmpUncheck, BitmapImage bmpDisable = null)
        {
            Image img = null;

            img = btnObj.Template.FindName("Image_Check", btnObj) as Image;
            if (img != null) img.Source = bmpCheck;

            img = btnObj.Template.FindName("Image_Uncheck", btnObj) as Image;
            if (img != null) img.Source = bmpUncheck;

            img = btnObj.Template.FindName("Image_Disable", btnObj) as Image;
            if (img != null) img.Source = bmpDisable;

        }

        public static string GetMatchedAprchLn(string aprchLn)
        {
            switch (aprchLn)
            {
                case "L1" :
                    return "L2";
                case "L2":
                    return "L1";
                case "W1":
                    return "L3";
                case "W2":
                    return "L4";
                default :
                    return aprchLn;
            }
        }
    }
}