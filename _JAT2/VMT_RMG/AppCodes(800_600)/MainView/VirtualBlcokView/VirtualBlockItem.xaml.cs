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
    /// VirtualBlockItem.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class VirtualBlockItem : UserControl
    {
        private Boolean _Selected = false;
        private List<TextBlock> _TextBlockList = new List<TextBlock>();
        public String CntrNo { get; set; }

        public Boolean Selected
        {
            get { return _Selected; }
            set {
                _Selected = value;
                RefreshContainerSearchControl();
            }
        }

        public VirtualBlockItem()
        {
            InitializeComponent();

            PresentationMgr.Singleton.PropertyChanged_UITheme += new System.ComponentModel.PropertyChangedEventHandler(Singleton_PropertyChanged_UITheme);

            AppendTextBlockList();

            this.CntrNo = String.Empty;
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

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            this.Singleton_PropertyChanged_UITheme(null, null);
        }

        private void AppendTextBlockList()
        {
            _TextBlockList.Clear();

            _TextBlockList.Add(this.TextBlock_Container);
            _TextBlockList.Add(this.TextBlock_ISO);            
        }

        private void RefreshContainerSearchControl()
        {
            if (_Selected)                
                this.LayoutRoot.Background = UIThemeMgr.LayoutRootSelectedBackground;
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
                    tBlock.Foreground = UIThemeMgr.TextBlockSelectedForeground;
                else
                {
                    ResourceDictionary rec = Application.Current.Resources.MergedDictionaries[0];
                    var strRec = rec["TextBox_Foreground_3"];

                    if (strRec is SolidColorBrush)
                        tBlock.Foreground = strRec as SolidColorBrush;
                }
            }
        }

        public void ClearCntrInfo()
        {
            this.CntrNo = this.TextBlock_Container.Text = String.Empty;
            this.TextBlock_ISO.Text = String.Empty;
            this._Selected = false;
        }

        public void SetCntrInfo(VMT_Data_JAT2.Objects.Common.VD_Common_Def_Container cntr)
        {
            this.ClearCntrInfo();

            this.CntrNo = this.TextBlock_Container.Text = cntr.cntrNo;
            this.TextBlock_ISO.Text = cntr.cntrIso;            
        }
    }
}
