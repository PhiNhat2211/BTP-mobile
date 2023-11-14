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
    /// Interaction logic for NavigatorItem.xaml
    /// </summary>
    public partial class NavigatorItem : UserControl
    {
        private Boolean _Exist = false;
        public Boolean Exist
        {
            get
            {
                return _Exist;
            }
            set
            {
                _Exist = value;
            }
        }

        public NavigatorItem()
        {
            InitializeComponent();            
            //PresentationMgr.Singleton.PropertyChanged_UITheme += new System.ComponentModel.PropertyChangedEventHandler(Singleton_PropertyChanged_UITheme);
        }

        //~NavigatorItem()
        //{
        //    PresentationMgr.Singleton.PropertyChanged_UITheme -= new System.ComponentModel.PropertyChangedEventHandler(Singleton_PropertyChanged_UITheme);
        //}

        //public void Singleton_PropertyChanged_UITheme(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        //{
        //    if (PresentationMgr.Singleton.CurrentUITheme == PresentationMgr.UITheme.UITheme_Day)
        //    {

        //    }
        //    else if (PresentationMgr.Singleton.CurrentUITheme == PresentationMgr.UITheme.UITheme_Night)
        //    {

        //    }
        //}

        public void SetInfo(Boolean bExist)
        {
            _Exist = bExist;

            this.Btn_NavigatorItem.Visibility = Visibility.Visible;

            if (bExist == true)
            {
                this.Btn_NavigatorItem.IsChecked = true;
            }
            else
            {
                this.Btn_NavigatorItem.IsChecked = false;
            }
        }

        public void SetSelection(Boolean bExist)
        {
            this.Btn_NavigatorItem_select.Visibility = bExist ? Visibility.Visible : Visibility.Hidden;            
        }
    }
}
