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

namespace VMT_RMG_800by600
{
    /// <summary>
    /// Interaction logic for ContainerSearchControl.xaml
    /// </summary>
    public partial class ContainerSearchControl : UserControl
    {
        private Boolean _Selected = false;
        private List<TextBlock> _TextBlockList = new List<TextBlock>();
        
        public VMT_Data_JAT2.Objects.Common.VD_Common_JobOrder JobOrder = null;

        public Boolean Selected
        {
            get { return _Selected; }
            set
            {
                _Selected = value;
                RefreshContainerSearchControl();
            }
        }

        public ContainerSearchControl()
        {
            InitializeComponent();

            PresentationMgr.Singleton.PropertyChanged_UITheme += new System.ComponentModel.PropertyChangedEventHandler(Singleton_PropertyChanged_UITheme);

            AppendTextBlockList();

            this.JobOrder = null;
        }

        ~ContainerSearchControl()
        {
            PresentationMgr.Singleton.PropertyChanged_UITheme -= new System.ComponentModel.PropertyChangedEventHandler(Singleton_PropertyChanged_UITheme);
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            // Init UI Skin
            InitSkinImage();

            // Init Event Handler
        }

        private void Singleton_PropertyChanged_UITheme(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (PresentationMgr.Singleton.CurrentUITheme == PresentationMgr.UITheme.UITheme_Day)
            {
                RefreshContainerSearchControl();
            }
            else if (PresentationMgr.Singleton.CurrentUITheme == PresentationMgr.UITheme.UITheme_Night)
            {
                RefreshContainerSearchControl();
            }
        }

        private void InitSkinImage()
        {
            // this.Singleton_PropertyChanged_UITheme(null, null);
        }

        private void AppendTextBlockList()
        {
            _TextBlockList.Clear();

            _TextBlockList.Add(this.TextBlock_Container);
            _TextBlockList.Add(this.TextBlock_ISO);
            _TextBlockList.Add(this.TextBlock_TruckNo);
            _TextBlockList.Add(this.TextBlock_WaitJob);
            _TextBlockList.Add(this.TextBlock_Location);
            _TextBlockList.Add(this.TextBlock_PlanLocation);
        }

        private void RefreshContainerSearchControl()
        {
            if (_Selected)
                //this.LayoutRoot.Background = new SolidColorBrush(Color.FromArgb(255, 240, 165, 15));
                this.LayoutRoot.Background = UIThemeMgr.LayoutRootSelectedBackground;   //new SolidColorBrush(Color.FromArgb(0xFF, 0xF0, 0xA5, 0x0F));
            else
            {
                ResourceDictionary rec = Application.Current.Resources.MergedDictionaries[0];
                var strRec = rec["Gird_Background_8"];

                if (strRec is SolidColorBrush)
                    this.LayoutRoot.Background = strRec as SolidColorBrush;
            }

            foreach (TextBlock tBlock in _TextBlockList)
            {
                if (_Selected)
                    //tBlock.Foreground = new SolidColorBrush(Color.FromArgb(255, 255, 255, 255));
                    tBlock.Foreground = UIThemeMgr.TextBlockSelectedForeground; //new SolidColorBrush(Color.FromArgb(0xFF, 0xFF, 0xFF, 0xFF));
                else
                {
                    ResourceDictionary rec = Application.Current.Resources.MergedDictionaries[0];
                    var strRec = rec["TextBox_Foreground_3"];

                    if (strRec is SolidColorBrush)
                        tBlock.Foreground = strRec as SolidColorBrush;
                }
            }
        }

        public void ClearJobInfo()
        {
            this.TextBlock_Container.Text = String.Empty;
            this.TextBlock_ISO.Text = String.Empty;
            this.TextBlock_TruckNo.Text = String.Empty;
            this.TextBlock_WaitJob.Text = String.Empty;
            this.TextBlock_Location.Text = String.Empty;
            this.TextBlock_PlanLocation.Text = String.Empty;
            this.Selected = false;

            this.JobOrder = null;            
        }

        public void SetJobInfo(VMT_Data_JAT2.Objects.Common.VD_Common_JobOrder jobOrder)
        {
            this.TextBlock_Container.Text = jobOrder.cntr.cntrNo;
            this.TextBlock_ISO.Text = jobOrder.cntr.cntrIso;
            this.TextBlock_TruckNo.Text = jobOrder.partnerMchn.mchnId;
            this.TextBlock_WaitJob.Text = PresentationMgr.GetContainerStatus(jobOrder.type.jobTp, jobOrder.type.jobStatus);

            if (PresentationMgr.UseFromLocationForRehandling == true && 
                (jobOrder.type.jobTp == "RH" || jobOrder.type.jobTp == "AH" ||
                jobOrder.type.jobTp == "DS" || jobOrder.type.jobTp == "GI" || jobOrder.type.jobTp == "MI" ||
                jobOrder.type.jobTp == "LC" || jobOrder.type.jobTp == "GC"))
            {
                this.TextBlock_Location.Text = jobOrder.locFrom.location;
                this.TextBlock_PlanLocation.Text = jobOrder.locWorking.location;
            }
            else
            {
                this.TextBlock_Location.Text = jobOrder.locWorking.location;
                this.TextBlock_PlanLocation.Text = String.Empty;
            }            

            this.JobOrder = jobOrder;
        }
    }
}
