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
using VMT_Data_JAT2;
//using ExternalAPI;

namespace VMT_RMG
{
    /// <summary>
    /// Interaction logic for JobListItem.xaml
    /// </summary>
    public partial class SwapListItem : UserControl
    {
        public Boolean IsFresh { get; set; }
        private Boolean _Selected = false;
        private List<TextBlock> _TextBlockList = new List<TextBlock>();

        private SolidColorBrush _itvPowInBrush = new SolidColorBrush(Color.FromArgb(0xFF, 0xFF, 0x20, 0x00));//SolidColorBrush(Color.FromArgb(0xFF, 0xE9, 0x20, 0x20));
        private SolidColorBrush _otrPowInBrush = new SolidColorBrush(Color.FromArgb(0xFF, 0x00, 0x40, 0xFF));//SolidColorBrush(Color.FromArgb(0xFF, 0x20, 0x20, 0xE9));

        public VMT_Data_JAT2.Objects.Common.VmtSwap swapItem = null;

        public Boolean Selected
        {
            get { return _Selected; }
            set
            {
                _Selected = value;
                RefreshJobListItem();
            }
        }

        public SwapListItem()
        {
            this.InitializeComponent();

            PresentationMgr.Singleton.PropertyChanged_UITheme += new System.ComponentModel.PropertyChangedEventHandler(Singleton_PropertyChanged_UITheme);

            this.IsFresh = false;

            AppendTextBlockList();
        }

        ~SwapListItem()
        {
            PresentationMgr.Singleton.PropertyChanged_UITheme -= new System.ComponentModel.PropertyChangedEventHandler(Singleton_PropertyChanged_UITheme);
        }

        private void Singleton_PropertyChanged_UITheme(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (PresentationMgr.Singleton.CurrentUITheme == PresentationMgr.UITheme.UITheme_Day)
            {
                RefreshJobListItem();
            }
            else if (PresentationMgr.Singleton.CurrentUITheme == PresentationMgr.UITheme.UITheme_Night)
            {
                RefreshJobListItem();
            }
        }

        private void AppendTextBlockList()
        {
            _TextBlockList.Clear();

            _TextBlockList.Add(contNo);
            _TextBlockList.Add(opr);
            _TextBlockList.Add(cntrIso);
            _TextBlockList.Add(swapPos);
            _TextBlockList.Add(cntrTp);
        }

        private void RefreshJobListItem()
        {
            //VMT_Data_JAT2.Objects.Common.VD_Common_JobOrder jobOrder = PresentationMgr.Singleton.JOB_Get(_jobKey);

            //if (jobOrder == null) khoa
            //    return;

            if (_Selected){
                this.LayoutRoot.Background = UIThemeMgr.LayoutRootSelectedBackground;
                //this.LayoutRoot.Background = new SolidColorBrush(Color.FromArgb(255, 240, 165, 15));
                VMT_Data_JAT2.Objects.RMG.RMG_Member.Singleton.TargetContSwap = this.contNo.Text;
            }
            else
            {
                this.LayoutRoot.Background = UIThemeMgr.LayoutRootNormalBackground;
                ResourceDictionary rec = Application.Current.Resources.MergedDictionaries[0];
                var strRec = rec["Gird_Background_8"];

                if (strRec is SolidColorBrush)
                    this.LayoutRoot.Background = strRec as SolidColorBrush;
            }

            foreach (TextBlock tBlock in _TextBlockList)
            {
                if (_Selected)
                    //tBlock.Foreground = new SolidColorBrush(Color.FromArgb(255, 255, 255, 255));
                    tBlock.Foreground = UIThemeMgr.TextBlockSelectedForeground;
                else
                {
                    ResourceDictionary rec = Application.Current.Resources.MergedDictionaries[0];
                    var strRec = rec["TextBox_Foreground_3"];

                    if (strRec is SolidColorBrush)
                        tBlock.Foreground = strRec as SolidColorBrush;
                    tBlock.Foreground = UIThemeMgr.TextBlockNormalForeground;
                }
            }

        }

        public void SetJobInfo(VMT_Data_JAT2.Objects.Common.VmtSwap swapJob, Int32 itemIndex)
        {
            swapItem = swapJob;
            //if (swapJob.regoNo!=null && swapJob.regoNo != "")
            //if (swapJob.cntrNo != null && swapJob.cntrNo != "")
                this.contNo.Text = swapJob.cntrNo;
            //if (swapJob.cntrIso != null && swapJob.cntrIso != "")
                this.cntrIso.Text = swapJob.cntrIso;
            //if (swapJob.opr != null && swapJob.opr != "")
                this.opr.Text = swapJob.opr;
            //if (swapJob.swapPos != null && swapJob.swapPos != "")
                this.swapPos.Text = swapJob.swapPos;
            //if (swapJob.cntrTp != null && swapJob.cntrTp != "")
                this.cntrTp.Text = swapJob.cntrTp;
        }

    }
}