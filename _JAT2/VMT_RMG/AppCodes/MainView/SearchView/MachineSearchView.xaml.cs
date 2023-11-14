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

//20190108
using Common.Interface;

namespace VMT_RMG
{
    /// <summary>
    /// Interaction logic for MachineSearchView.xaml
    /// </summary>
    public partial class MachineSearchView : UserControl
    {
        //private readonly int _showMachineMaxCount = 4;
        //private int _currentFirstIndex = 0;

        private List<MachineSearchControl> _machineControlItems = null;

        //private string _currentITVNumber = string.Empty;
        //private Boolean _isAvailableForDetwin = false;
        private Boolean _validate4LoadingSwap = false;
        public string ChssPsn
        {
            get
            {
                return this.Btn_Position_Fore.IsChecked == true ? "F" :
                    (this.Btn_Position_After.IsChecked == true ? "A" :
                    (this.Btn_Position_Center.IsChecked == true ? "C" : String.Empty));
            }
        }

        private VMT_Data_JAT2.Objects.Common.VD_Common_JobOrder _currentJob = null;
        public VMT_Data_JAT2.Objects.Common.VD_Common_JobOrder CurrentJob
        {
            set 
            { 
                this._currentJob = value;
                ///////////////////////////////////////////////////////////////////////////////////
                // 2017/02/17 : [JAT2] RMG VMT - New Requirement
                // Container Position (FWD / AFT) Update by RMG Operator, discussing for RMG VMT
                var job = this._currentJob;
                if (job == null)                
                    job = PresentationMgr.Singleton.JOB_Get(VMT_Data_JAT2.Objects.RMG.RMG_Member.Singleton.TargetJobKey);

                if (job == null || job.type == null)
                {
                    this._validate4LoadingSwap = false;

                    this.Btn_Position_Fore.IsChecked = false;
                    this.Btn_Position_After.IsChecked = false;
                    this.Btn_Position_Center.IsChecked = false;
                } else
                {
                    //this._isAvailableForDetwin = job != null &&
                    //    job.cntr.cntrIso.StartsWith("2") && string.IsNullOrEmpty(job.type.ycTwinKey) &&
                    //    (job.type.jobTp.Equals("LD") || job.type.jobTp.Equals("MO"));
                    //this._currentITVNumber = job.partnerMchn.mchnId;
                    //this.Btn_Position_After.IsChecked = this.Btn_Position_Center.IsChecked =
                    //    !(this.Btn_Position_Fore.IsChecked = job.type.jobFlagInfo == "F" ? true : false);

                    this._validate4LoadingSwap = //this.Grid_PositionOnChassis.IsEnabled =                         
                        job != null && job.type.jobStatus.Equals("P") &&
                        (job.type.jobTp.Equals("LD") || job.type.jobTp.Equals("MO"));

                    this.Btn_Position_Fore.IsChecked = job.type.jobFlagInfo.Equals("F");
                    this.Btn_Position_After.IsChecked = job.type.jobFlagInfo.Equals("A");
                    this.Btn_Position_Center.IsChecked = job.type.jobFlagInfo.Equals("C");

                    //if (this._validate4LoadingSwap)
                    //{
                    if (job.cntr.cntrIso.StartsWith("2"))
                    {
                        //if (string.IsNullOrEmpty(job.type.ycTwinKey))
                        //    this.Btn_Position_Fore.IsEnabled = this.Btn_Position_After.IsEnabled = this.Btn_Position_Center.IsEnabled = true;
                        //else
                        //{
                        this.Btn_Position_Fore.IsEnabled = this.Btn_Position_After.IsEnabled = true;
                        this.Btn_Position_Center.IsEnabled = false;
                        //}
                    }
                    else //if (job.cntr.cntrIso.StartsWith("4"))                    
                    {
                        this.Btn_Position_Fore.IsEnabled = this.Btn_Position_After.IsEnabled = false;
                        this.Btn_Position_Center.IsEnabled = true;
                        this.Btn_Position_Center.IsChecked = true;
                    }
                    //}
                    //else
                    //    this.Btn_Position_Fore.IsEnabled = this.Btn_Position_After.IsEnabled = this.Btn_Position_Center.IsEnabled = false;

                }

                this.EnablePositionOnChassis(this._validate4LoadingSwap);
                ///////////////////////////////////////////////////////////////////////////////////
            }
        }

        public MachineSearchView()
        {
            InitializeComponent();

            // Register this UI Component to PresentationMgr
            PresentationMgr.Singleton.AddUIComponent(this.ToString(), this);

            PresentationMgr.Singleton.PropertyChanged_UITheme += new System.ComponentModel.PropertyChangedEventHandler(Singleton_PropertyChanged_UITheme);

            this._machineControlItems = new List<MachineSearchControl>();

            this.UC_KeyPad.ShowTruckKey(true);
        }

        ~MachineSearchView()
        {
            PresentationMgr.Singleton.PropertyChanged_UITheme -= new System.ComponentModel.PropertyChangedEventHandler(Singleton_PropertyChanged_UITheme);
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            // Init UI Skin
            InitSkinImage();

            this.UC_KeyPad.Button_keypad_done_1.Visibility = Visibility.Hidden;
            this.UC_KeyPad.Button_keypad_done_2.Visibility = Visibility.Hidden;
            this.UC_KeyPad.ShowKeyPad(this.TextBox_Search);

            // Init Event Handler
            this.Btn_Up.Click += new RoutedEventHandler(Btn_Up_Click);
            this.Btn_Down.Click += new RoutedEventHandler(Btn_Down_Click);

            this.Btn_Search.Click += new RoutedEventHandler(Btn_Search_Click);
            this.Btn_Cancel.Click += new RoutedEventHandler(Btn_Cancel_Click);

            this.IsVisibleChanged += new DependencyPropertyChangedEventHandler(MachineSearchView_IsVisibleChanged);

            this.ListBox_Machine.MouseLeftButtonUp += new MouseButtonEventHandler(ListBox_Machine_MouseLeftButtonUp);

            this.ListBox_Machine.Children.Clear();
            this.TextBox_Search.Text = "";

            this.Btn_Position_Fore.Checked += new RoutedEventHandler(Btn_Position_Fore_Checked);
            this.Btn_Position_After.Checked += new RoutedEventHandler(Btn_Position_After_Checked);
            this.Btn_Position_Center.Checked += new RoutedEventHandler(Btn_Position_Center_Checked);
            //this.UC_KeyPad.DoneCallback += new Keypad.KeypadDoneCallback(this.KeypadDone);
            this.Btn_Done.Click += new RoutedEventHandler(Btn_Done_Click);

            TextBlock_Machine.Text = PresentationMgr.Singleton.LanguageSer.GetResourceRMG("LA0074", LanguageService.LABEL_JOBLIST);
            Btn_Done.Content = PresentationMgr.Singleton.LanguageSer.GetResourceRMG("LA0039", LanguageService.LABEL_POPUP);
            Btn_Cancel.Content = PresentationMgr.Singleton.LanguageSer.GetResourceRMG("LA0016", LanguageService.LABEL_SETTING);
            Btn_Search.Content = PresentationMgr.Singleton.LanguageSer.GetResourceRMG("LA0019", LanguageService.LABEL_MAINWINDOW);
        }

        void MachineSearchView_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            //throw new NotImplementedException();
            if (this.Visibility != System.Windows.Visibility.Visible)
                return;

            this.TextBox_Search.Text = String.Empty;
            this.Dispatcher.BeginInvoke(new Action(delegate
            {
                this.TextBox_Search.Focus();
            }));
        }

        private SolidColorBrush _enabledTextBrush = new SolidColorBrush(Color.FromArgb(0xFF, 0x46, 0x48, 0x48));
        private SolidColorBrush _disabledTextBrush = new SolidColorBrush(Color.FromArgb(0xFF, 0xFF, 0xFF, 0xFF));
        public void EnablePositionOnChassis(Boolean enable)
        {
            //this.Btn_Position_Fore.IsEnabled = this.Btn_Position_After.IsEnabled = this.Btn_Position_Center.IsEnabled = enable;
            //if (enable)
            //{
                if (this.Btn_Position_Fore.IsChecked == true)
                {
                    this.Btn_Position_Fore.Foreground = _disabledTextBrush;
                    this.Btn_Position_After.Foreground = this.Btn_Position_After.IsEnabled ? _enabledTextBrush : _disabledTextBrush;
                    this.Btn_Position_Center.Foreground = this.Btn_Position_Center.IsEnabled ? _enabledTextBrush : _disabledTextBrush;
                }
                else if (this.Btn_Position_After.IsChecked == true)
                {
                    this.Btn_Position_Fore.Foreground = this.Btn_Position_Fore.IsEnabled ? _enabledTextBrush : _disabledTextBrush;
                    this.Btn_Position_After.Foreground = _disabledTextBrush;
                    this.Btn_Position_Center.Foreground = this.Btn_Position_Center.IsEnabled ? _enabledTextBrush : _disabledTextBrush;
                }
                else if (this.Btn_Position_Center.IsChecked == true)
                {
                    this.Btn_Position_Fore.Foreground = this.Btn_Position_Fore.IsEnabled ? _enabledTextBrush : _disabledTextBrush;
                    this.Btn_Position_After.Foreground = this.Btn_Position_After.IsEnabled ? _enabledTextBrush : _disabledTextBrush;
                    this.Btn_Position_Center.Foreground = _disabledTextBrush;
                }
            //}
            //else            
            //    this.Btn_Position_Fore.Foreground = this.Btn_Position_After.Foreground = this.Btn_Position_Center.Foreground = _disabledTextBrush;            
        }

        private void Btn_Position_Fore_Checked(object sender, RoutedEventArgs e)
        {
            this.Btn_Position_Fore.Foreground = _disabledTextBrush;
            this.Btn_Position_After.Foreground = this.Btn_Position_After.IsEnabled ? _enabledTextBrush : _disabledTextBrush;
            this.Btn_Position_Center.Foreground = this.Btn_Position_Center.IsEnabled ? _enabledTextBrush : _disabledTextBrush;
        }

        private void Btn_Position_After_Checked(object sender, RoutedEventArgs e)
        {
            this.Btn_Position_Fore.Foreground = this.Btn_Position_Fore.IsEnabled ? _enabledTextBrush : _disabledTextBrush;
            this.Btn_Position_After.Foreground = _disabledTextBrush;
            this.Btn_Position_Center.Foreground = this.Btn_Position_Center.IsEnabled ? _enabledTextBrush : _disabledTextBrush;
        }

        private void Btn_Position_Center_Checked(object sender, RoutedEventArgs e)
        {
            this.Btn_Position_Fore.Foreground = this.Btn_Position_Fore.IsEnabled ? _enabledTextBrush : _disabledTextBrush;
            this.Btn_Position_After.Foreground = this.Btn_Position_After.IsEnabled ? _enabledTextBrush : _disabledTextBrush;
            this.Btn_Position_Center.Foreground = _disabledTextBrush;
        }

        private void Btn_Done_Click(object sender, RoutedEventArgs e)
        {
            this.KeypadDone();
        }

        private void KeypadDone()
        {
            String machineID = GetSelectedMachine();
            if (!String.IsNullOrEmpty(machineID))
            {
                this.Visibility = System.Windows.Visibility.Hidden;
                if (PresentationMgr.MainView.UC_JobList.IsRefreshReserved == true)
                {
                    PresentationMgr.MainView.UC_InfomationView.UC_TargetJobInfo.CheckBox_Refresh.IsChecked = true;
                    if (!PresentationMgr.Singleton.IsEnabledInventoryThread())
                        PresentationMgr.Singleton.ThreadTimerStart(false);
                }

                if (this._currentJob == null)
                {
                    String jobKey = VMT_Data_JAT2.Objects.RMG.RMG_Member.Singleton.TargetJobKey;
                    if (String.IsNullOrEmpty(jobKey))
                        return;

                    this._currentJob = PresentationMgr.Singleton.JOB_Get(jobKey);
                    if (this._currentJob == null)
                        return;
                }

                if (machineID.Equals(this._currentJob.partnerMchn.mchnId))
                    return;

                //if (this._isAvailableForDetwin && 
                //    this.Btn_Position_Fore.IsEnabled && this.Btn_Position_After.IsEnabled)
                //{
                //    send.positionOnChassis = this.Btn_Position_Fore.IsChecked == true ? "F" : (this.Btn_Position_After.IsChecked == true ? "A" : String.Empty);
                //    send.chgPrgmId = "DTWN";
                //}
                //if (this._validate4LoadingSwap)
                //{
                //    InterfaceMessageLoader.instance().WriteInterfaceMessage<string>(String.Format("[{0}] {1}", System.Reflection.MethodBase.GetCurrentMethod().Name
                //        , "Validate4LoadingSwapping_Ask"), String.Format("cntrNo : {0}, itv : {1}, chssPsn : {2}", this._currentJob.cntr.cntrNo, machineID, this.ChssPsn));
                //    VMT_Data_JAT2.VMT_DataMgr_RMG.Validate4LoadingSwapping_Ask(this._currentJob.cntr.cntrNo, machineID, ChssPsn);
                //}
                //else
                //{
                VMT_Data_JAT2.Objects.Common.VD_Common_DoSwap4Manual_Send send = new VMT_Data_JAT2.Objects.Common.VD_Common_DoSwap4Manual_Send();
                send.cntrNo = this._currentJob.cntr.cntrNo;
                //send.cntrPoint = jobOrder.cntr.cntrNo;
                send.partnerMachineID = this._currentJob.partnerMchn.mchnId;
                send.jobid = this._currentJob.jobKey;
                send.positionOnChassis = this.ChssPsn;
                send.externalId = this._currentJob.workingMchn.mchnId;
                send.newYTNo = machineID;

                InterfaceMessageLoader.instance().WriteInterfaceMessage<VMT_Data_JAT2.Objects.Common.VD_Common_DoSwap4Manual_Send>(String.Format("[{0}] {1}", System.Reflection.MethodBase.GetCurrentMethod().Name
                    , "DoSwap4Manual_Ask"), send);
                VMT_Data_JAT2.VMT_DataMgr_Common.DoSwap4Manual_Ask(send);
                //}
                PresentationMgr.AppWin.ShowProgressBar(0);
            }
            else
            {
                if (PresentationMgr.MainView.jobDone)
                {
                    this.Visibility = System.Windows.Visibility.Hidden;
                    PresentationMgr.MainView.jobDone = false;
                    var target = PresentationMgr.Singleton.JOB_Get(VMT_Data_JAT2.Objects.RMG.RMG_Member.Singleton.TargetJobKey);
                    PresentationMgr.Singleton.SendJobDoneAsk(target);
                }
            }
        }

        public string GetSelectedMachine()
        {
            foreach (var uiElement in this.ListBox_Machine.Children)
            {
                if (uiElement is MachineSearchControl)
                {
                    MachineSearchControl control = (MachineSearchControl)uiElement;
                    if (control.Selected)                    
                        return control.TextBlock_Machine.Text;                                            
                }
            }
            return string.Empty;
        }

        private void Scroll_SearchMachine_ScrollChanged(object sender, System.Windows.Controls.ScrollChangedEventArgs e)
        {
            if (!(sender is ScrollViewer))
                return;

            ScrollViewer scroller = sender as ScrollViewer;

            if (scroller.ContentVerticalOffset == 0)
            {
                this.Btn_Up.IsEnabled = false;
            }
            else
            {
                this.Btn_Up.IsEnabled = true;
            }

            if (scroller.ScrollableHeight == scroller.ContentVerticalOffset)
            {
                this.Btn_Down.IsEnabled = false;
            }
            else
            {
                this.Btn_Down.IsEnabled = true;
            }
        }

        private double scrollMoveOffSet = 54; // MachineSearchControl DesignHeight

        private void Btn_Up_Click(object sender, RoutedEventArgs e)
        {
            if (!(sender is Button))
                return;

            double currentOffset = this.Scroll_SearchMachine.ContentVerticalOffset;

            if (currentOffset - scrollMoveOffSet > 0)
                this.Scroll_SearchMachine.ScrollToVerticalOffset(currentOffset - scrollMoveOffSet);
            else
                this.Scroll_SearchMachine.ScrollToVerticalOffset(0);

            //if (_currentFirstIndex > 0)
            //{
            //    _currentFirstIndex--;
            //    this.SetMachineList();
            //}
        }

        private void Btn_Down_Click(object sender, RoutedEventArgs e)
        {
            if (!(sender is Button))
                return;

            double currentOffset = this.Scroll_SearchMachine.ContentVerticalOffset;
            if (currentOffset + scrollMoveOffSet < this.Scroll_SearchMachine.ScrollableHeight)
                this.Scroll_SearchMachine.ScrollToVerticalOffset(currentOffset + scrollMoveOffSet);
            else
                this.Scroll_SearchMachine.ScrollToVerticalOffset(this.Scroll_SearchMachine.ScrollableHeight);

            //if (_currentFirstIndex < DataMgr.Singleton.List_MachineofPool.Count - 1)
            //{
            //    _currentFirstIndex++;
            //    this.SetMachineList();
            //}
        }

        void Btn_Search_Click(object sender, RoutedEventArgs e)
        {
            //throw new NotImplementedException();
            String searchText = this.TextBox_Search.Text.ToUpper();

            this.ListBox_Machine.Children.Clear();

            List<String> machineIDList = new List<String>();
            foreach (var machine in DataMgr.Singleton.List_MachineofPool.Machine)
            {
                if (String.IsNullOrEmpty(searchText) || machine.mchnId.Contains(searchText))
                    machineIDList.Add(machine.mchnId);
            }

            Int32 controlCount = 0;
            foreach (var machineID in machineIDList)
            {
                MachineSearchControl control = null;
                if (this._machineControlItems.Count <= controlCount)
                {
                    control = new MachineSearchControl();
                    _machineControlItems.Add(control);
                }
                else
                    control = _machineControlItems[controlCount];
                control.TextBlock_Machine.Text = machineID;
                control.Selected = false;

                this.ListBox_Machine.Children.Add(control);
                controlCount++;
            }
        }

        private void Btn_Cancel_Click(object sender, RoutedEventArgs e)
        {
            //throw new NotImplementedException();

            if (this.Visibility == System.Windows.Visibility.Visible)
            {
                this.Visibility = System.Windows.Visibility.Hidden;

                if (PresentationMgr.MainView.UC_JobList.IsRefreshReserved == true)
                {
                    PresentationMgr.MainView.UC_InfomationView.UC_TargetJobInfo.CheckBox_Refresh.IsChecked = true;
                    if (!PresentationMgr.Singleton.IsEnabledInventoryThread())
                        PresentationMgr.Singleton.ThreadTimerStart(false);
                }
                if (PresentationMgr.MainView.jobDone)
                {
                    PresentationMgr.MainView.jobDone = false;
                    var target = PresentationMgr.Singleton.JOB_Get(VMT_Data_JAT2.Objects.RMG.RMG_Member.Singleton.TargetJobKey);
                    PresentationMgr.Singleton.SendJobDoneAsk(target);
                }
            }
        }

        private void Singleton_PropertyChanged_UITheme(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (PresentationMgr.Singleton.CurrentUITheme == PresentationMgr.UITheme.UITheme_Day)
            {
                PresentationMgr.SetSkinButton(this.Btn_Up,
                    UIThemeMgr.Day.MachineSearchView_ButtonUpDefaultImage, UIThemeMgr.Day.MachineSearchView_ButtonUpPressImage, UIThemeMgr.Day.MachineSearchView_ButtonUpDisableImage);               

                PresentationMgr.SetSkinButton(this.Btn_Down,
                    UIThemeMgr.Day.MachineSearchView_ButtonDownDefaultImage, UIThemeMgr.Day.MachineSearchView_ButtonDownPressImage, UIThemeMgr.Day.MachineSearchView_ButtonDownDisableImage);                

                //PresentationMgr.SetSkinButton(this.Btn_Search,
                //    UIThemeMgr.Day.MachineSearchView_ButtonSearchDefaultImage, UIThemeMgr.Day.MachineSearchView_ButtonSearchPressImage, UIThemeMgr.Day.MachineSearchView_ButtonSearchDefaultImage);                                
            }
            else if (PresentationMgr.Singleton.CurrentUITheme == PresentationMgr.UITheme.UITheme_Night)
            {
                PresentationMgr.SetSkinButton(this.Btn_Up,
                    UIThemeMgr.Night.MachineSearchView_ButtonUpDefaultImage, UIThemeMgr.Night.MachineSearchView_ButtonUpPressImage, UIThemeMgr.Night.MachineSearchView_ButtonUpDisableImage);                

                PresentationMgr.SetSkinButton(this.Btn_Down,
                    UIThemeMgr.Night.MachineSearchView_ButtonDownDefaultImage, UIThemeMgr.Night.MachineSearchView_ButtonDownPressImage, UIThemeMgr.Night.MachineSearchView_ButtonDownDisableImage);                

                //PresentationMgr.SetSkinButton(this.Btn_Search,
                //    UIThemeMgr.Night.MachineSearchView_ButtonSearchDefaultImage, UIThemeMgr.Night.MachineSearchView_ButtonSearchPressImage, UIThemeMgr.Night.MachineSearchView_ButtonSearchDefaultImage);
            }

            PresentationMgr.SetSkinButton(this.Btn_Cancel,
                    UIThemeMgr.Day.ButtonDefaultImage, UIThemeMgr.Day.ButtonPressImage, UIThemeMgr.Day.ButtonDefaultImage);

            PresentationMgr.SetSkinButton(this.Btn_Done,
                UIThemeMgr.Day.ButtonDefaultImage, UIThemeMgr.Day.ButtonPressImage, UIThemeMgr.Day.ButtonDefaultImage);

            PresentationMgr.SetSkinButton(this.Btn_Search,
                UIThemeMgr.Day.ButtonDefaultImage, UIThemeMgr.Day.ButtonPressImage, UIThemeMgr.Day.ButtonDefaultImage);

            PresentationMgr.SetSkinRadioButton(this.Btn_Position_Fore,
                UIThemeMgr.Day.ButtonDefaultImage_1, UIThemeMgr.Day.ButtonDefaultImage, UIThemeMgr.Day.ButtonDisableImage);

            PresentationMgr.SetSkinRadioButton(this.Btn_Position_After,
                UIThemeMgr.Day.ButtonDefaultImage_1, UIThemeMgr.Day.ButtonDefaultImage, UIThemeMgr.Day.ButtonDisableImage);

            PresentationMgr.SetSkinRadioButton(this.Btn_Position_Center,
                UIThemeMgr.Day.ButtonDefaultImage_1, UIThemeMgr.Day.ButtonDefaultImage, UIThemeMgr.Day.ButtonDisableImage);
        }

        private void InitSkinImage()
        {
            this.Singleton_PropertyChanged_UITheme(null, null);
        }

        private void ListBox_Machine_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (!(e.Source is MachineSearchControl))
                return;

            foreach (var cSearchControl in ListBox_Machine.Children)
            {
                if (cSearchControl is MachineSearchControl)
                {
                    (cSearchControl as MachineSearchControl).Selected = false;
                }
            }

            MachineSearchControl mSearchControl = e.Source as MachineSearchControl;
            mSearchControl.Selected = true;

            if (!String.IsNullOrEmpty(mSearchControl.TextBlock_Machine.Text))
            {
                this.TextBox_Search.Text = mSearchControl.TextBlock_Machine.Text;

                // 2017/02/17 : [JAT2] RMG VMT - New Requirement
                // Container Position (FWD / AFT) Update by RMG Operator, discussing for RMG VMT
                //if (this._isAvailableForDetwin)
                //{
                //    PresentationMgr.AppWin.ShowProgressBar(0);
                //    VMT_Data_JAT2.VMT_DataMgr_RMG.CheckYcDeTwin(this._currentITVNumber, mSearchControl.TextBlock_Machine.Text);                    
                //}
            }
        }
        
        public void SetMachineList()
        {
            this.ListBox_Machine.Children.Clear();
            //this.EnablePositionOnChassis(false);            

            var itvPowInList = PresentationMgr.Singleton.GetPowInMachineLIst(false);
            var machineList = DataMgr.Singleton.List_MachineofPool.Machine.ToList();
            machineList.Sort(
                (x, y) =>
                {
                    if (itvPowInList.Contains(x.mchnId) && !itvPowInList.Contains(y.mchnId))
                        return -1;
                    else if (!itvPowInList.Contains(x.mchnId) && itvPowInList.Contains(y.mchnId))
                        return 1;
                    else
                        return 0;
                });

            Int32 controlCount = 0;
            foreach (var machine in machineList)
            {
                MachineSearchControl control = null;
                if (this._machineControlItems.Count <= controlCount)
                {
                    control = new MachineSearchControl();
                    this._machineControlItems.Add(control);
                }
                else
                    control = this._machineControlItems[controlCount];
                control.TextBlock_Machine.Text = machine.mchnId;
                control.MachineTp = machine.mchnTp;
                control.Selected = false;

                if (itvPowInList.Contains(machine.mchnId))                
                    control.Rect_PowIn.Visibility = Visibility.Visible;                                    
                else                
                    control.Rect_PowIn.Visibility = Visibility.Hidden;

                this.ListBox_Machine.Children.Add(control);

                controlCount++;
            }
            
            machineList.Clear();
            itvPowInList.Clear();

            this.Scroll_SearchMachine.ScrollToVerticalOffset(0);
            //for (int i = 0; i < this._showMachineMaxCount; i++)
            //{
            //    int idx = i + _currentFirstIndex;
            //    if (idx >= DataMgr.Singleton.List_MachineofPool.Machine.Count)
            //        break;

            //    var machine = DataMgr.Singleton.List_MachineofPool.Machine[idx];
            //    var control = new MachineSearchControl();
            //    control.TextBlock_Machine.Text = machine.mchnId;
            //    //control.CheckBox_Machine.Content = machine.mchnId;
            //    this.ListBox_Machine.Children.Add(control);
            //}
        }
    }
}
