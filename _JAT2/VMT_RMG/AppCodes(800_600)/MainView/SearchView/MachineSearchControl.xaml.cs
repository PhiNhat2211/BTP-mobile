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
    /// Interaction logic for MachineSearchControl.xaml
    /// </summary>
    public partial class MachineSearchControl : UserControl
    {
        private Boolean _Selected = false;        

        public Boolean Selected
        {
            get { return _Selected; }
            set
            {
                _Selected = value;
                RefreshContainerSearchControl();
            }
        }

        public MachineSearchControl()
        {
            InitializeComponent();

            PresentationMgr.Singleton.PropertyChanged_UITheme += new System.ComponentModel.PropertyChangedEventHandler(Singleton_PropertyChanged_UITheme);
        }

        ~MachineSearchControl()
        {
            PresentationMgr.Singleton.PropertyChanged_UITheme -= new System.ComponentModel.PropertyChangedEventHandler(Singleton_PropertyChanged_UITheme);
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            // Init UI Skin
            InitSkinImage();

            //this.TextBlock_Machine.Text = String.Empty;
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

        private void RefreshContainerSearchControl()
        {
            if (_Selected)
                this.LayoutRoot.Background = UIThemeMgr.LayoutRootSelectedBackground;   //new SolidColorBrush(Color.FromArgb(0xFF, 0xF0, 0xA5, 0x0F));
            else
            {
                //ResourceDictionary rec = Application.Current.Resources.MergedDictionaries[0];
                //var strRec = rec["Gird_Background_8"];

                //if (strRec is SolidColorBrush)
                //    this.LayoutRoot.Background = strRec as SolidColorBrush;

                this.LayoutRoot.Background = UIThemeMgr.LayoutRootNormalBackground; //new SolidColorBrush(Color.FromArgb(0xFF, 0xED, 0xF1, 0xF2));
            }

            if (_Selected)
                this.TextBlock_Machine.Foreground = UIThemeMgr.TextBlockSelectedForeground; //new SolidColorBrush(Color.FromArgb(0xFF, 0xFF, 0xFF, 0xFF));
            else
            {
                //ResourceDictionary rec = Application.Current.Resources.MergedDictionaries[0];
                //var strRec = rec["TextBox_Foreground_3"];

                //if (strRec is SolidColorBrush)
                //    this.TextBlock_Machine.Foreground = strRec as SolidColorBrush;

                this.TextBlock_Machine.Foreground = UIThemeMgr.TextBlockNormalForeground;   // new SolidColorBrush(Color.FromArgb(0xFF, 0x46, 0x48, 0x48));
            }
        }
    }
}
