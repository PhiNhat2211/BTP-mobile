using VMT_Data_JAT2;

namespace VMT_YT
{
    public partial class MainPage : ContentPage
    {
        int count = 0;

        public MainPage()
        {
            InitializeComponent();
        }

        private void OnCounterClicked(object sender, EventArgs e)
        {
            if (VMT_DataMgr.CreateVMTClient())
            {

            }
        }
    }
}