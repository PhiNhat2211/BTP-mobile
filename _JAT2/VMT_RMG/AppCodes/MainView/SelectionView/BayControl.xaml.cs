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

namespace VMT_RMG
{
    /// <summary>
    /// Interaction logic for BlockJobControl.xaml
    /// </summary>
    public partial class BayControl : UserControl
    {
        // Control Color
        private SolidColorBrush brush_control_selected = new SolidColorBrush(Color.FromArgb(255, 32, 32, 234));

        private Boolean _isSelected = false;
        private Int32 _nCount;
        private String _strBayName;
        private String _strOpr;
        private String _strCntrLenTp;
        private String _strInspCode;
        private String _strCategoryName;

        public Boolean IsSelected
        {
            get { return _isSelected; }
            set
            {
                _isSelected = value;

                if (_isSelected == true)
                {
                    //this.Border_BG.BorderBrush = brush_control_selected;
                    //String imgUri = @"/Images/MainView/SelectionView/BlockJobControl_Select.png";
                    //this.BackgroundSample.Source = PresentationMgr.GetImageSource(imgUri);
                }
                else
                {
                    //this.Border_BG.BorderBrush = null;
                    //String imgUri = @"/Images/MainView/SelectionView/BlockJobControl.png";
                    //this.BackgroundSample.Source = PresentationMgr.GetImageSource(imgUri);
                }
            }
        }



        public String BayName
        {
            get { return _strBayName; }
            set
            {
                _strBayName = value;
                this.TextBlock_BayName.Text = _strBayName;
            }
        }

        public String CategoryName
        {
            get { return _strCategoryName; }
            set
            {
                _strCategoryName = value;
                this.TextBlock_CategoryName.Text = _strCategoryName;
            }
        }

        public String Opr
        {
            get { return _strOpr; }
            set
            {
                _strOpr = value;
                //this.TextBlock_OPR.Text = _strOpr;
            }
        }

        public String CntrLenTp
        {
            get { return _strCntrLenTp; }
            set
            {
                _strCntrLenTp = value;
                //this.TextBlock_CntrLenTp.Text = _strCntrLenTp;
            }
        }

        public String InspCode
        {
            get { return _strInspCode; }
            set
            {
                _strInspCode = value;
                //this.TextBlock_InspCode.Text = _strInspCode;
            }
        }

        private List<VMT_Data_JAT2.Objects.Common.VD_Common_JobOrder> _ListJobOrder;
        public BayControl()
        {
            this.InitializeComponent();

            this.LayoutRoot.PreviewMouseLeftButtonUp += new MouseButtonEventHandler(LayoutRoot_PreviewMouseLeftButtonUp);


            _ListJobOrder = new List<VMT_Data_JAT2.Objects.Common.VD_Common_JobOrder>();
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
        }


        void LayoutRoot_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            PresentationMgr.Singleton.SetBlockBaySelection(this.BayName);
        }
    }
}