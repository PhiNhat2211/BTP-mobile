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
    /// Interaction logic for JobTypeFilterView.xaml
    /// </summary>
    public partial class JobTypeFilterView : UserControl
    {
        public JobTypeFilterView()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            // Init UI Skin
            InitSkinImage();
        }

        private void InitSkinImage()
        {
        }
    }
}
