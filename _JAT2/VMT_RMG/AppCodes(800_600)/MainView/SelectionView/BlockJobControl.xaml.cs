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
//using ExternalAPI;

namespace VMT_RMG_800by600
{
	/// <summary>
	/// Interaction logic for BlockJobControl.xaml
	/// </summary>
	public partial class BlockJobControl : UserControl
	{
        // Control Color
        private SolidColorBrush brush_control_selected = new SolidColorBrush(Color.FromArgb(255, 32, 32, 234));

        private Boolean _isSelected = false;
        private Int32 _nDS, _nLD, _nMO, _nMI, _nGI, _nGO, _nETC;
        private Int32 _nCount;
        private String _strBlckName;

        public Boolean IsSelected
        {
            get { return _isSelected; }
            set
            {
                _isSelected = value;

                if (_isSelected == true)
                {
                    //this.Border_BG.BorderBrush = brush_control_selected;
                    String imgUri = @"/Images/MainView/SelectionView/BlockJobControl_Select.png";
                    this.BackgroundSample.Source = PresentationMgr.GetImageSource(imgUri);
                }
                else
                {
                    //this.Border_BG.BorderBrush = null;
                    String imgUri = @"/Images/MainView/SelectionView/BlockJobControl.png";
                    this.BackgroundSample.Source = PresentationMgr.GetImageSource(imgUri);
                }
            }
        }

        public Int32 nDS
        {
            get { return _nDS; }
            set
            {
                _nDS = value;
                this.TextBlock_DS.Text = _nDS.ToString();
            }
        }

        public Int32 nLD
        {
            get { return _nLD; }
            set
            {
                _nLD = value;
                this.TextBlock_LD.Text = _nLD.ToString();
            }
        }

        public Int32 nMO
        {
            get { return _nMO; }
            set
            {
                _nMO = value;
                this.TextBlock_MO.Text = _nMO.ToString();
            }
        }

        public Int32 nMI
        {
            get { return _nMI; }
            set
            {
                _nMI = value;
                this.TextBlock_MI.Text = _nMI.ToString();
            }
        }

        public Int32 nGI
        {
            get { return _nGI; }
            set
            {
                _nGI = value;
                this.TextBlock_GI.Text = _nGI.ToString();
            }
        }

        public Int32 nGO
        {
            get { return _nGO; }
            set
            {
                _nGO = value;
                this.TextBlock_GO.Text = _nGO.ToString();
            }
        }

        public Int32 nETC
        {
            get { return _nETC; }
            set
            {
                _nETC = value;
                this.TextBlock_ETC.Text = _nETC.ToString();
            }
        }

        public Int32 nCount
        {
            get { return this._ListJobOrder.Count; }
            set
            {
                _nCount = value;
                this.TextBlock_JobCount.Text = nCount.ToString();
            }
        }

        public String BlckName
        {
            get { return _strBlckName; }
            set
            {
                _strBlckName = value;
                this.TextBlock_BlockName.Text = _strBlckName;
            }
        }

        private List<VMT_Data_JAT2.Objects.Common.VD_Common_JobOrder> _ListJobOrder;
		public BlockJobControl()
		{
			this.InitializeComponent();

            //PresentationMgr.Singleton.PropertyChanged_UITheme += new System.ComponentModel.PropertyChangedEventHandler(Singleton_PropertyChanged_UITheme);

            this.LayoutRoot.PreviewMouseLeftButtonUp += new MouseButtonEventHandler(LayoutRoot_PreviewMouseLeftButtonUp);

            this.Grid_BlockName.PreviewMouseLeftButtonUp += new MouseButtonEventHandler(Grid_BlockName_PreviewMouseLeftButtonUp);

            //this.Grid_DS.PreviewMouseLeftButtonUp += new MouseButtonEventHandler(Grid_DS_PreviewMouseLeftButtonUp);
            //this.Grid_LD.PreviewMouseLeftButtonUp += new MouseButtonEventHandler(Grid_LD_PreviewMouseLeftButtonUp);

            //this.Grid_MO.PreviewMouseLeftButtonUp += new MouseButtonEventHandler(Grid_MO_PreviewMouseLeftButtonUp);
            //this.Grid_MI.PreviewMouseLeftButtonUp += new MouseButtonEventHandler(Grid_MI_PreviewMouseLeftButtonUp);

            //this.Grid_GI.PreviewMouseLeftButtonUp += new MouseButtonEventHandler(Grid_GI_PreviewMouseLeftButtonUp);
            //this.Grid_GO.PreviewMouseLeftButtonUp += new MouseButtonEventHandler(Grid_GO_PreviewMouseLeftButtonUp);

            //this.Grid_ETC.PreviewMouseLeftButtonUp += new MouseButtonEventHandler(Grid_ETC_PreviewMouseLeftButtonUp);


            _ListJobOrder = new List<VMT_Data_JAT2.Objects.Common.VD_Common_JobOrder>();
            nDS = nLD = nMO = nMI = nGI = nGO = nETC = 0;
            nCount = 0;
		}        

        public void Clear()
        {
            this.IsSelected = false;

            for (int i = 0; i < _ListJobOrder.Count; i++)
            {
                var item = _ListJobOrder[i] as VMT_Data_JAT2.Objects.Common.VD_Common_JobOrder;
                    item = null;
            }
            _ListJobOrder.Clear();
            nDS = nLD = nMO = nMI = nGI = nGO = nETC = 0;
            nCount = 0;
        }

        //private void Singleton_PropertyChanged_UITheme(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        //{
        //    if (PresentationMgr.Singleton.CurrentUITheme == PresentationMgr.UITheme.UITheme_Day)
        //    {

        //    }
        //    else if (PresentationMgr.Singleton.CurrentUITheme == PresentationMgr.UITheme.UITheme_Night)
        //    {

        //    }
        //}

        void LayoutRoot_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            //PresentationMgr.Singleton.JV_SetTaskBlock(this.BlckName);
            PresentationMgr.Singleton.SetBlockSelection(this.BlckName);            
        }

        void Grid_BlockName_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {   
            // 
        }

        void Grid_DS_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            // throw new NotImplementedException();
            if (nDS == 0)
                return;
        }

        void Grid_LD_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            // throw new NotImplementedException();
            if (nLD == 0)
                return;
        }

        void Grid_MO_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            // throw new NotImplementedException();
            if (nMO == 0)
                return;
        }

        void Grid_MI_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            // throw new NotImplementedException();
            if (nMI == 0)
                return;
        }

        void Grid_GI_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            // throw new NotImplementedException();
            if (nGI == 0)
                return;
        }

        void Grid_GO_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            // throw new NotImplementedException();
            if (nGO == 0)
                return;
        }

        void Grid_ETC_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            // throw new NotImplementedException();
            if (nETC == 0)
                return;
        }

        public void AddJobOrder(VMT_Data_JAT2.Objects.Common.VD_Common_JobOrder jo)
        {
            _ListJobOrder.Add(jo);
            UpdateUI();
        }

        public void RemoveJobOrder(VMT_Data_JAT2.Objects.Common.VD_Common_JobOrder jo)
        {
            _ListJobOrder.Remove(jo);
            UpdateUI();
        }

        public void UpdateUI()
        {
            nDS = nLD = nMO = nMI = nGI = nGO = nETC = 0;

            // Total Count
            nCount = this._ListJobOrder.Count;

            // Block Name Update
            String strBlckName = "";
            if (nCount > 0)
                strBlckName = this._ListJobOrder[0].locWorking.blck;
            BlckName = strBlckName;

            foreach (VMT_Data_JAT2.Objects.Common.VD_Common_JobOrder jo in _ListJobOrder)
            {
                if(jo.type.jobTp.Equals("DS"))
                {
                    nDS++;
                }
                else if (jo.type.jobTp.Equals("LD"))
                {
                    nLD++;
                }
                else if (jo.type.jobTp.Equals("MI"))
                {
                    nMI++;
                }
                else if (jo.type.jobTp.Equals("MO"))
                {
                    nMO++;
                }
                else if (jo.type.jobTp.Equals("GI"))
                {
                    nGI++;
                }
                else if (jo.type.jobTp.Equals("GO"))
                {
                    nGO++;
                }
                else
                {
                    nETC++;
                }
            }
        }
	}
}