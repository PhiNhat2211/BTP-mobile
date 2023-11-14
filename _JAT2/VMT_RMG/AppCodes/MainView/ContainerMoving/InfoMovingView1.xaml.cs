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
using System.ComponentModel;
//using ExternalAPI;

namespace VMT_RMG
{
	/// <summary>
	/// Interaction logic for InfomationView.xaml
	/// </summary>
	public partial class InfoMovingView1 : UserControl
	{
        public InfoMovingView1()
		{
			this.InitializeComponent();

            // Register this UI Component to PresentationMgr
            PresentationMgr.Singleton.AddUIComponent(this.ToString(), this);

            PresentationMgr.Singleton.PropertyChanged_UITheme += new System.ComponentModel.PropertyChangedEventHandler(Singleton_PropertyChanged_UITheme);

            //VMT_Data_JAT2.Objects.RMG.RMG_Member.Singleton.PropertyChanged_TargetJobKey +=
            //    new PropertyChangedEventHandler(RMG_Member_PropertyChanged_TargetJobKey);
		}

        ~InfoMovingView1()
        {
            PresentationMgr.Singleton.PropertyChanged_UITheme -= new System.ComponentModel.PropertyChangedEventHandler(Singleton_PropertyChanged_UITheme);
        }

        private void Singleton_PropertyChanged_UITheme(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (PresentationMgr.Singleton.CurrentUITheme == PresentationMgr.UITheme.UITheme_Day)
            {

            }
            else if (PresentationMgr.Singleton.CurrentUITheme == PresentationMgr.UITheme.UITheme_Night)
            {

            }
        }

        public void RMG_Member_PropertyChanged_TargetJobKey(object sender, PropertyChangedEventArgs e)
        {   
            String jobKey = VMT_Data_JAT2.Objects.RMG.RMG_Member.Singleton.TargetJobKey;
            if (!String.IsNullOrEmpty(jobKey))
            {
                VMT_Data_JAT2.Objects.Common.VD_Common_JobOrder jobOrder = PresentationMgr.Singleton.JOB_Get(jobKey);
                if (jobOrder != null && jobOrder.type != null)
                {
                    var location = jobOrder.locWorking;
                    if (PresentationMgr.UseFromLocationForRehandling == true &&
                        (jobOrder.type.jobTp == "RH" || jobOrder.type.jobTp == "AH") &&
                        jobOrder.type.jobStatus != "P")
                        location = jobOrder.locFrom;
                    else if (jobOrder.type.jobTp == "LD" || jobOrder.type.jobTp == "GO" || jobOrder.type.jobTp == "MO")
                        location = string.IsNullOrEmpty(jobOrder.locFrom.location) ? jobOrder.locWorking : jobOrder.locFrom;

                    VMT_Data_JAT2.Marshalling.Geometry.sPosition pos = new VMT_Data_JAT2.Marshalling.Geometry.sPosition();
                    pos.m_cBlock = location.blck;
                    pos.m_cBay = location.bay;
                    pos.m_cBay = PresentationMgr.GetFrontOddBay(location.bay);
                    pos.m_cRow = location.row;
                    pos.m_cTier = location.tier;
                    PresentationMgr.Singleton.MovingPosition1 = pos;
                }
            }
            else
            {
                PresentationMgr.Singleton.MovingPosition1.m_cRow = PresentationMgr.Singleton.MovingPosition1.m_cTier = String.Empty;

                //if (!PresentationMgr.Singleton.IsEnabledInventoryThread())
                    PresentationMgr.Singleton.ThreadTimerStart(true);
            }

            this.UC_TargetJobInfo.RefreshTargetJobInfo();
        }
	}
}