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
using VMT_RMG;

namespace VMT_RMG_800by600
{
    /// <summary>
    /// Interaction logic for MachineSearchView.xaml
    /// </summary>
    public partial class MachineSearchView : UserControl
    {
        //private readonly int _showMachineMaxCount = 4;
        //private int _currentFirstIndex = 0;

        private List<MachineSearchControl> _machineControlItems = null;
        private VMT_Data_JAT2.Objects.Common.VD_Common_JobOrder _currentJob = null;
        public VMT_Data_JAT2.Objects.Common.VD_Common_JobOrder CurrentJob
        {
            set { this._currentJob = value; }
        }

        public MachineSearchView()
        {
            InitializeComponent();

            // Register this UI Component to PresentationMgr
            PresentationMgr.Singleton.AddUIComponent(this.ToString(), this);

            PresentationMgr.Singleton.PropertyChanged_UITheme += new System.ComponentModel.PropertyChangedEventHandler(Singleton_PropertyChanged_UITheme);

            this._machineControlItems = new List<MachineSearchControl>();
        }

        ~MachineSearchView()
        {
            PresentationMgr.Singleton.PropertyChanged_UITheme -= new System.ComponentModel.PropertyChangedEventHandler(Singleton_PropertyChanged_UITheme);
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            // Init UI Skin
            InitSkinImage();

            this.UC_KeyPad.ShowKeyPad(this.TextBox_Search);

            // Init Event Handler
            this.Btn_Up.Click += new RoutedEventHandler(Btn_Up_Click);
            this.Btn_Down.Click += new RoutedEventHandler(Btn_Down_Click);

            this.Btn_Search.Click += new RoutedEventHandler(Btn_Search_Click);
            this.Btn_Cancel.Click += new RoutedEventHandler(Btn_Cancel_Click);

            this.ListBox_Machine.MouseLeftButtonUp += new MouseButtonEventHandler(ListBox_Machine_MouseLeftButtonUp);

            this.ListBox_Machine.Children.Clear();
            this.TextBox_Search.Text = "";

            this.UC_KeyPad.DoneCallback += new Keypad.KeypadDoneCallback(this.KeypadDone);
        }

        private void KeypadDone()
        {
            String machineID = String.Empty;
            if (GetSelectedMachine(ref machineID) && !String.IsNullOrEmpty(machineID))
            {
                this.Visibility = System.Windows.Visibility.Hidden;
                if (PresentationMgr.MainView.UC_JobList.IsRefreshReserved == true)
                {
                    PresentationMgr.MainView.UC_JobList.CheckBox_Refresh.IsChecked = true;
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

                VMT_Data_JAT2.Objects.Common.VD_Common_DoSwap4Manual_Send send = new VMT_Data_JAT2.Objects.Common.VD_Common_DoSwap4Manual_Send();
                send.cntrNo = this._currentJob.cntr.cntrNo;
                //send.cntrPoint = jobOrder.cntr.cntrNo;
                send.partnerMachineID = this._currentJob.partnerMchn.mchnId;
                send.jobid = this._currentJob.jobKey;
                send.externalId = this._currentJob.workingMchn.mchnId;
                send.newYTNo = machineID;

                InterfaceMessageLoader.instance().WriteInterfaceMessage<VMT_Data_JAT2.Objects.Common.VD_Common_DoSwap4Manual_Send>(String.Format("[{0}] {1}", System.Reflection.MethodBase.GetCurrentMethod().Name
                , "DoSwap4Manual_Ask"), send);

                VMT_Data_JAT2.VMT_DataMgr_Common.DoSwap4Manual_Ask(send);                
                PresentationMgr.AppWin.ShowProgressBar(0);
            }
        }

        private Boolean GetSelectedMachine(ref String machineID)
        {
            foreach (var uiElement in this.ListBox_Machine.Children)
            {
                if (uiElement is MachineSearchControl)
                {
                    MachineSearchControl control = (MachineSearchControl)uiElement;
                    if (control.Selected)
                    {
                        machineID = control.TextBlock_Machine.Text;
                        //machineID = Convert.ToString(control.CheckBox_Machine.Content);
                        return true;
                    }
                }
            }
            return false;
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
                    PresentationMgr.MainView.UC_JobList.CheckBox_Refresh.IsChecked = true;
                    if (!PresentationMgr.Singleton.IsEnabledInventoryThread())
                        PresentationMgr.Singleton.ThreadTimerStart(false);
                }
            }
        }

        private void Singleton_PropertyChanged_UITheme(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (PresentationMgr.Singleton.CurrentUITheme == PresentationMgr.UITheme.UITheme_Day)
            {
                PresentationMgr.SetSkinButton(this.Btn_Up,
                    UIThemeMgr.Day.MachineSearchView_ButtonUpDefaultImage, UIThemeMgr.Day.MachineSearchView_ButtonUpPressImage, UIThemeMgr.Day.MachineSearchView_ButtonUpDisableImage);
                    //new BitmapImage(new Uri(@"/VMT_RMG;component/Images/MainView/SearchView/MachineSearchView_Up_Default.png", UriKind.Relative)),
                    //new BitmapImage(new Uri(@"/VMT_RMG;component/Images/MainView/SearchView/MachineSearchView_Up_Press.png", UriKind.Relative)),
                    //new BitmapImage(new Uri(@"/VMT_RMG;component/Images/MainView/SearchView/MachineSearchView_Up_Disable.png", UriKind.Relative))
                    //);

                PresentationMgr.SetSkinButton(this.Btn_Down,
                    UIThemeMgr.Day.MachineSearchView_ButtonDownDefaultImage, UIThemeMgr.Day.MachineSearchView_ButtonDownPressImage, UIThemeMgr.Day.MachineSearchView_ButtonDownDisableImage);
                    //new BitmapImage(new Uri(@"/VMT_RMG;component/Images/MainView/SearchView/MachineSearchView_Down_Default.png", UriKind.Relative)),
                    //new BitmapImage(new Uri(@"/VMT_RMG;component/Images/MainView/SearchView/MachineSearchView_Down_Press.png", UriKind.Relative)),
                    //new BitmapImage(new Uri(@"/VMT_RMG;component/Images/MainView/SearchView/MachineSearchView_Down_Disable.png", UriKind.Relative))
                    //);

                PresentationMgr.SetSkinButton(this.Btn_Search,
                    UIThemeMgr.Day.MachineSearchView_ButtonSearchDefaultImage, UIThemeMgr.Day.MachineSearchView_ButtonSearchPressImage, UIThemeMgr.Day.MachineSearchView_ButtonSearchDefaultImage);
                    //new BitmapImage(new Uri(@"/VMT_RMG;component/Images/MainView/SearchView/MachineSearchView_Search_Default.png", UriKind.Relative)),
                    //new BitmapImage(new Uri(@"/VMT_RMG;component/Images/MainView/SearchView/MachineSearchView_Search_Press.png", UriKind.Relative)),
                    //new BitmapImage(new Uri(@"/VMT_RMG;component/Images/MainView/SearchView/MachineSearchView_Search_Default.png", UriKind.Relative))
                    //);

                PresentationMgr.SetSkinButton(this.Btn_Cancel,
                    UIThemeMgr.Day.ButtonDefaultImage, UIThemeMgr.Day.ButtonPressImage, UIThemeMgr.Day.ButtonDefaultImage);
                    //new BitmapImage(new Uri(@"/VMT_RMG;component/Images/MainView/Button/Button_default.png", UriKind.Relative)),
                    //new BitmapImage(new Uri(@"/VMT_RMG;component/Images/MainView/Button/Button_press.png", UriKind.Relative)),
                    //new BitmapImage(new Uri(@"/VMT_RMG;component/Images/MainView/Button/Button_default.png", UriKind.Relative))
                    //);
            }
            else if (PresentationMgr.Singleton.CurrentUITheme == PresentationMgr.UITheme.UITheme_Night)
            {
                PresentationMgr.SetSkinButton(this.Btn_Up,
                    UIThemeMgr.Night.MachineSearchView_ButtonUpDefaultImage, UIThemeMgr.Night.MachineSearchView_ButtonUpPressImage, UIThemeMgr.Night.MachineSearchView_ButtonUpDisableImage);
                    //new BitmapImage(new Uri(@"/VMT_RMG;component/Images(Night)/MainView/SearchView/MachineSearchView_Up_Default.png", UriKind.Relative)),
                    //new BitmapImage(new Uri(@"/VMT_RMG;component/Images(Night)/MainView/SearchView/MachineSearchView_Up_Press.png", UriKind.Relative)),
                    //new BitmapImage(new Uri(@"/VMT_RMG;component/Images(Night)/MainView/SearchView/MachineSearchView_Up_Disable.png", UriKind.Relative))
                    //);

                PresentationMgr.SetSkinButton(this.Btn_Down,
                    UIThemeMgr.Night.MachineSearchView_ButtonDownDefaultImage, UIThemeMgr.Night.MachineSearchView_ButtonDownPressImage, UIThemeMgr.Night.MachineSearchView_ButtonDownDisableImage);
                    //new BitmapImage(new Uri(@"/VMT_RMG;component/Images(Night)/MainView/SearchView/MachineSearchView_Down_Default.png", UriKind.Relative)),
                    //new BitmapImage(new Uri(@"/VMT_RMG;component/Images(Night)/MainView/SearchView/MachineSearchView_Down_Press.png", UriKind.Relative)),
                    //new BitmapImage(new Uri(@"/VMT_RMG;component/Images(Night)/MainView/SearchView/MachineSearchView_Down_Disable.png", UriKind.Relative))
                    //);

                PresentationMgr.SetSkinButton(this.Btn_Search,
                    UIThemeMgr.Night.MachineSearchView_ButtonSearchDefaultImage, UIThemeMgr.Night.MachineSearchView_ButtonSearchPressImage, UIThemeMgr.Night.MachineSearchView_ButtonSearchDefaultImage);
                    //new BitmapImage(new Uri(@"/VMT_RMG;component/Images(Night)/MainView/SearchView/MachineSearchView_Search_Default.png", UriKind.Relative)),
                    //new BitmapImage(new Uri(@"/VMT_RMG;component/Images(Night)/MainView/SearchView/MachineSearchView_Search_Press.png", UriKind.Relative)),
                    //new BitmapImage(new Uri(@"/VMT_RMG;component/Images(Night)/MainView/SearchView/MachineSearchView_Search_Default.png", UriKind.Relative))
                    //);

                PresentationMgr.SetSkinButton(this.Btn_Cancel,
                    UIThemeMgr.Night.ButtonDefaultImage, UIThemeMgr.Night.ButtonPressImage, UIThemeMgr.Night.ButtonDefaultImage);
                    //new BitmapImage(new Uri(@"/VMT_RMG;component/Images/MainView/Button/Button_default.png", UriKind.Relative)),
                    //new BitmapImage(new Uri(@"/VMT_RMG;component/Images/MainView/Button/Button_press.png", UriKind.Relative)),
                    //new BitmapImage(new Uri(@"/VMT_RMG;component/Images/MainView/Button/Button_default.png", UriKind.Relative))
                    //);
            }
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
            }
        }

        public void SetMachineList()
        {
            this.ListBox_Machine.Children.Clear();

            Int32 controlCount = 0;
            foreach (VMT_Data_JAT2.Objects.Common.VD_Common_Job_Machine machine in DataMgr.Singleton.List_MachineofPool.Machine)
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
                control.Selected = false;
                
                this.ListBox_Machine.Children.Add(control);

                controlCount++;
            }

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
