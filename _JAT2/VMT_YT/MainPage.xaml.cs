using HessianComm;
using VMT_Data_JAT2;

namespace VMT_YT
{
    public partial class MainPage : ContentPage
    {
        int count = 0;

        public MainPage()
        {
            InitializeComponent();
            InitAppCallbackFunctions();
        }
        private void InitAppCallbackFunctions()
        {
            VMT_DataMgr_Common_Callback.SetCallback_NotifyKeepAlive(new VMT_DataMgr_Common_Callback.Callback_NotifyKeepAlive(NotifyKeepAlive));
        }
        public void NotifyKeepAlive(string value)
        {
            Dispatcher.Dispatch(() => Lbl_KeepAliveData.Text = value);
        }
        private void Btn_Call_Clicked(object sender, EventArgs e)
        {
            if (VMT_DataMgr.CreateVMTClient())
            {
                VMT_DataMgr_Common.StartPollingPriority_Ask(HessianCommType.KeepAlive);
                VMT_DataMgr_Common.StartPolling_Ask(HessianCommType.GetMachineJobByKeys_New);
            }
        }
    }
}