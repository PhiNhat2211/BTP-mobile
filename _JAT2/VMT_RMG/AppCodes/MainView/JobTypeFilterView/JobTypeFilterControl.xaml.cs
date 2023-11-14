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
    /// Interaction logic for JobTypeFilterControl.xaml
    /// </summary>
    public partial class JobTypeFilterControl : UserControl
    {
        public JobTypeFilterControl()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            // Init UI Skin
            InitSkinImage();

            // Init Event Handler
            this.CheckBox_Type_All.Click += new RoutedEventHandler(CheckBox_Type_All_Click);
            this.CheckBox_Type_GI.Click += new RoutedEventHandler(CheckBox_Type_GI_Click);
            this.CheckBox_Type_GO.Click += new RoutedEventHandler(CheckBox_Type_GO_Click);
            this.CheckBox_Type_MI.Click += new RoutedEventHandler(CheckBox_Type_MI_Click);
            this.CheckBox_Type_MO.Click += new RoutedEventHandler(CheckBox_Type_MO_Click);
            this.CheckBox_Type_DS.Click += new RoutedEventHandler(CheckBox_Type_DS_Click);
            this.CheckBox_Type_LD.Click += new RoutedEventHandler(CheckBox_Type_LD_Click);
            this.CheckBox_Type_RH.Click += new RoutedEventHandler(CheckBox_Type_RH_Click);
            this.CheckBox_Type_AH.Click += new RoutedEventHandler(CheckBox_Type_AH_Click);

            Btn_Cancel.Content = PresentationMgr.Singleton.LanguageSer.GetResourceRMG("MS0030", LanguageService.MESSAGE_GROUP);
            JobFilter.Text = PresentationMgr.Singleton.LanguageSer.GetResourceRMG("LA0102", LanguageService.LABEL_CUSTOMIZE);
        }

        private void Grid_SeparateType_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            // TODO: Add event handler implementation here.
            if (PresentationMgr.MainView.UC_JobTypeFilterView.Visibility == System.Windows.Visibility.Visible)
                PresentationMgr.MainView.UC_JobTypeFilterView.Visibility = System.Windows.Visibility.Hidden;
        }

        private void CheckBox_Type_AH_Click(object sender, RoutedEventArgs e)
        {
            //throw new NotImplementedException();
        }

        private void CheckBox_Type_RH_Click(object sender, RoutedEventArgs e)
        {
            //throw new NotImplementedException();
        }

        private void CheckBox_Type_LD_Click(object sender, RoutedEventArgs e)
        {
            //throw new NotImplementedException();
        }

        private void CheckBox_Type_DS_Click(object sender, RoutedEventArgs e)
        {
            //throw new NotImplementedException();
        }

        private void CheckBox_Type_MO_Click(object sender, RoutedEventArgs e)
        {
            //throw new NotImplementedException();
        }

        private void CheckBox_Type_MI_Click(object sender, RoutedEventArgs e)
        {
            //throw new NotImplementedException();
        }

        private void CheckBox_Type_GO_Click(object sender, RoutedEventArgs e)
        {
            //throw new NotImplementedException();
        }

        private void CheckBox_Type_GI_Click(object sender, RoutedEventArgs e)
        {
            //throw new NotImplementedException();
        }

        private void CheckBox_Type_All_Click(object sender, RoutedEventArgs e)
        {
            //throw new NotImplementedException();
        }

        private void InitSkinImage()
        {
            PresentationMgr.SetSkinCheckBox(this.CheckBox_Type_All,
                new BitmapImage(new Uri(@"/VMT_RTG;component/Images/Common/JobList/JobList_btn_type_press.png", UriKind.Relative)),
                new BitmapImage(new Uri(@"/VMT_RTG;component/Images/Common/JobList/JobList_btn_type_default.png", UriKind.Relative))
                );

            PresentationMgr.SetSkinCheckBox(this.CheckBox_Type_GI,
                new BitmapImage(new Uri(@"/VMT_RTG;component/Images/Common/JobList/JobList_btn_type_press.png", UriKind.Relative)),
                new BitmapImage(new Uri(@"/VMT_RTG;component/Images/Common/JobList/JobList_btn_type_default.png", UriKind.Relative))
                );

            PresentationMgr.SetSkinCheckBox(this.CheckBox_Type_GO,
                new BitmapImage(new Uri(@"/VMT_RTG;component/Images/Common/JobList/JobList_btn_type_press.png", UriKind.Relative)),
                new BitmapImage(new Uri(@"/VMT_RTG;component/Images/Common/JobList/JobList_btn_type_default.png", UriKind.Relative))
                );

            PresentationMgr.SetSkinCheckBox(this.CheckBox_Type_MI,
                new BitmapImage(new Uri(@"/VMT_RTG;component/Images/Common/JobList/JobList_btn_type_press.png", UriKind.Relative)),
                new BitmapImage(new Uri(@"/VMT_RTG;component/Images/Common/JobList/JobList_btn_type_default.png", UriKind.Relative))
                );

            PresentationMgr.SetSkinCheckBox(this.CheckBox_Type_MO,
                new BitmapImage(new Uri(@"/VMT_RTG;component/Images/Common/JobList/JobList_btn_type_press.png", UriKind.Relative)),
                new BitmapImage(new Uri(@"/VMT_RTG;component/Images/Common/JobList/JobList_btn_type_default.png", UriKind.Relative))
                );

            PresentationMgr.SetSkinCheckBox(this.CheckBox_Type_DS,
                new BitmapImage(new Uri(@"/VMT_RTG;component/Images/Common/JobList/JobList_btn_type_press.png", UriKind.Relative)),
                new BitmapImage(new Uri(@"/VMT_RTG;component/Images/Common/JobList/JobList_btn_type_default.png", UriKind.Relative))
                );

            PresentationMgr.SetSkinCheckBox(this.CheckBox_Type_LD,
                new BitmapImage(new Uri(@"/VMT_RTG;component/Images/Common/JobList/JobList_btn_type_press.png", UriKind.Relative)),
                new BitmapImage(new Uri(@"/VMT_RTG;component/Images/Common/JobList/JobList_btn_type_default.png", UriKind.Relative))
                );

            PresentationMgr.SetSkinCheckBox(this.CheckBox_Type_RH,
                new BitmapImage(new Uri(@"/VMT_RTG;component/Images/Common/JobList/JobList_btn_type_press.png", UriKind.Relative)),
                new BitmapImage(new Uri(@"/VMT_RTG;component/Images/Common/JobList/JobList_btn_type_default.png", UriKind.Relative))
                );

            PresentationMgr.SetSkinCheckBox(this.CheckBox_Type_AH,
                new BitmapImage(new Uri(@"/VMT_RTG;component/Images/Common/JobList/JobList_btn_type_press.png", UriKind.Relative)),
                new BitmapImage(new Uri(@"/VMT_RTG;component/Images/Common/JobList/JobList_btn_type_default.png", UriKind.Relative))
                );

            PresentationMgr.SetSkinButton(this.Btn_Cancel,
                new BitmapImage(new Uri(@"/VMT_RTG;component/Images/Common/JobList/JobList_btn_down_default.png", UriKind.Relative)),
                new BitmapImage(new Uri(@"/VMT_RTG;component/Images/Common/JobList/JobList_btn_down_press.png", UriKind.Relative)),
                new BitmapImage(new Uri(@"/VMT_RTG;component/Images/Common/JobList/JobList_btn_down_disable.png", UriKind.Relative))
                );
        }

        
    }
}
