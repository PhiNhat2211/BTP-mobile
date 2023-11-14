using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace VMT_RMG
{
    internal partial class SplashForm : Form
    {
        private static SplashForm _frmSplash = null;        

        public static void ShowSplash()
        {
            if (SplashForm._frmSplash != null)
                return;

            var thread = new Thread(new ThreadStart(SplashForm.ShowForm));
            thread.IsBackground = true;
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
        }

        public static void HideSplash()
        {
            try
            {
                if (SplashForm._frmSplash != null)
                {
                    SplashForm._frmSplash.BeginInvoke(new Action(() =>
                        SplashForm._frmSplash.Close()));
                }
            }
            catch
            {                
            }
        }

        static private void ShowForm()
        {
            try
            {
                SplashForm._frmSplash = new SplashForm();
                SplashForm._frmSplash.FormClosed +=
                    (s, e) =>
                    {
                        System.Windows.Threading.Dispatcher.CurrentDispatcher.BeginInvokeShutdown(
                            System.Windows.Threading.DispatcherPriority.Background);
                        SplashForm._frmSplash = null;
                    };
                SplashForm._frmSplash.Show();
                System.Windows.Threading.Dispatcher.Run();

                //Application.Run(SplashForm._frmSplash);
            }
            catch (Exception e)
            {                
            }
        }

        internal SplashForm()
        {
            this.InitializeComponent();
        }

        protected override void OnLoad(EventArgs e)
        {            
            base.OnLoad(e);

            this.label.Text = "VMT Loading...";
        }

        //protected override void WndProc(ref Message m)
        //{
        //    const int WM_NCHITTEST = 0x84;
        //    const int HTCLIENT = 0x1;
        //    const int HTCAPTION = 0x2;

        //    base.WndProc(ref m);

        //    if (m.Msg == WM_NCHITTEST && (int)m.Result == HTCLIENT)
        //    {
        //        m.Result = (IntPtr)HTCAPTION;
        //    }
        //}
    }
}
