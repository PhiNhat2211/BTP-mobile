using System;
using System.Windows.Controls;

namespace VMT_ITV
{
	/// <summary>
	/// Interaction logic for MachineInfo.xaml
	/// </summary>
	public partial class WarningIconSetPopup : UserControl
	{
        //----------------------------------------------
        //- Default Machine Configuration
        public WarningIconSetPopup()
		{
			this.InitializeComponent();

            OnInit();
		}

        public void OnInit()
        {
            TextBox_Speed.Text = "20";
            CheckBox_ConeChecker.IsChecked = false;
            RadioButton_High.IsChecked = true;
        }

        private void Button_Close_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            this.Visibility = System.Windows.Visibility.Hidden;
        }

        private void Button_OK_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            Int32 speed;
            Int32.TryParse(TextBox_Speed.Text, out speed);

            Int32 bCone;
            if (CheckBox_ConeChecker.IsChecked == true)
                bCone = 1;
            else
                bCone = 0;

            Int32 bFuel; // default 80
            if (RadioButton_High.IsChecked == true)
            {
                bFuel = 100; // High
            }
            else if (RadioButton_Low.IsChecked == true)
            {
                bFuel = 20; // Low
            }
            else
            {
                bFuel = 0;
            }

            PresentationMgr.MainView.ProcessBySpeedKmCallback(speed);
            PresentationMgr.MainView.ProcessByEngineTempCallback(bCone);
            PresentationMgr.MainView.ProcessByFuelGageCallback(bFuel);
        }
	}
}