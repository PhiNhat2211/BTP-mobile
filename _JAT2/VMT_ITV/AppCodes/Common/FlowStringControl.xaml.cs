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
using System.Windows.Threading;
using System.Timers;
using VMT_Data_JAT2;
using VMT_Data_JAT2.Objects;

namespace VMT_ITV
{
	/// <summary>
	/// Interaction logic for FlowStringControl.xaml
	/// </summary>
    public partial class FlowStringControl : UserControl
    {
        private Thickness Empty_Thickness = new Thickness(0, 10, 0, 0);
        private Thickness Normal_Thickness = new Thickness(10, 10, 10, 0);

        private DispatcherTimer _floatingText = new DispatcherTimer();

        private void StartFloatingText()
        {
            //--------------------------------------------------
            //- Create Random Sequence for Tme Raising order.
            _floatingText.Interval = TimeSpan.FromMilliseconds(10);
            _floatingText.IsEnabled = true;

            _floatingText.Tick += _FloatingTextTick;

        }

        private void StopFloatingText()
        {
            _floatingText.Stop();
            _floatingText.Tick -= _FloatingTextTick;
        }

        void _FloatingTextTick(object sender, EventArgs e)
        {
            try
            {
                Thickness mg = new Thickness();

                mg = this.Stack_Front.Margin;
                mg.Left -= 1;

                if (Math.Abs(this.Stack_Front.ActualWidth) < Math.Abs(mg.Left))
                    mg.Left = 0;

                this.Stack_Front.Margin = mg;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                _floatingText.Stop();
                _floatingText.Tick -= _FloatingTextTick;
            }

        }

        private MainWindow mMainWindow = null;
        public void Init(MainWindow mainWin)
        {
            mMainWindow = mainWin;
        }

        private List<TextBlock> LDSeqMachineTextBlockList = new List<TextBlock>();
        private List<TextBlock> TendemMachineTextBlockList = new List<TextBlock>();
        private List<TextBlock> KPITextBlockList = new List<TextBlock>();
        private List<TextBlock> MessageTextBlockList = new List<TextBlock>();

        public FlowStringControl()
        {   
            this.InitializeComponent();

            ///////////////////////////////////////////
            //------------------Initialize
            // Add LDSeqMachineTextBlockList
            // TB_Front_LDSeq.Text = "STS LD Sequence :";
            TB_Front_LDSeq.Text = "";
            LDSeqMachineTextBlockList.Clear();
            LDSeqMachineTextBlockList.Add(TB_Front_LDSeq_1);
            LDSeqMachineTextBlockList.Add(TB_Front_LDSeq_2);
            LDSeqMachineTextBlockList.Add(TB_Front_LDSeq_3);
            LDSeqMachineTextBlockList.Add(TB_Front_LDSeq_4);
            LDSeqMachineTextBlockList.Add(TB_Front_LDSeq_5);

            // Add TendemMachineTextBlockList
            TB_Front_Tandem.Text = "Tandem Machine :";
            TendemMachineTextBlockList.Clear();
            TendemMachineTextBlockList.Add(TB_Front_Tandem_1);

            // Add KPITextBlockList
            KPITextBlockList.Clear();

            // Add MessageTextBlockList
            MessageTextBlockList.Clear();
            ///////////////////////////////////////////


            ///////////////////////////////////////////
            //------------------SizeChanged Event
            // Add LDSeqMachineTextBlockList
            TB_Front_LDSeq_1.SizeChanged += new SizeChangedEventHandler(TB_Front_SizeChanged);
            TB_Front_LDSeq_2.SizeChanged += new SizeChangedEventHandler(TB_Front_SizeChanged);
            TB_Front_LDSeq_3.SizeChanged += new SizeChangedEventHandler(TB_Front_SizeChanged);
            TB_Front_LDSeq_4.SizeChanged += new SizeChangedEventHandler(TB_Front_SizeChanged);
            TB_Front_LDSeq_5.SizeChanged += new SizeChangedEventHandler(TB_Front_SizeChanged);

            TB_Front_Tandem_1.SizeChanged += new SizeChangedEventHandler(TB_Front_SizeChanged);
            ///////////////////////////////////////////


            // Stack_2 UI Visible Changed (It measns text should be floating(auto-scroll) )
            this.Stack_Rear.IsVisibleChanged += new DependencyPropertyChangedEventHandler(Stack_Rear_IsVisibleChanged);
        }

        void TB_Front_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            TextBlock tb = (TextBlock)sender;
            if (tb.Text.Equals(""))
            {
                tb.Margin = Empty_Thickness;
            }
            else
            {
                tb.Margin = Normal_Thickness;
            }

            UpdateTitleTextBlock();
        }

        void UpdateTitleTextBlock()
        {
            // LDSeqMachineTextBlockList
            bool isVisible = false;
            foreach (TextBlock LDSeqMachineTextBlock in LDSeqMachineTextBlockList)
            {
                if (LDSeqMachineTextBlock.Text.Equals("") == false)
                {
                    isVisible = true; break;
                }
            }

            if (isVisible)
            {
                TB_Front_LDSeq.Visibility = System.Windows.Visibility.Visible;
                TB_Front_LDSeq.Margin = Normal_Thickness;
            }
            else
            {
                TB_Front_LDSeq.Visibility = System.Windows.Visibility.Collapsed;
                TB_Front_LDSeq.Margin = Empty_Thickness;
            }

            // TendemMachineTextBlockList
            isVisible = false;
            foreach (TextBlock TendemMachineTextBlock in TendemMachineTextBlockList)
            {
                if (TendemMachineTextBlock.Text.Equals("") == false)
                {
                    isVisible = true; break;
                }
            }

            if (isVisible)
            {
                TB_Front_Tandem.Visibility = System.Windows.Visibility.Visible;
                TB_Front_Tandem.Margin = Normal_Thickness;
            }
            else
            {
                TB_Front_Tandem.Visibility = System.Windows.Visibility.Collapsed;
                TB_Front_Tandem.Margin = Empty_Thickness;
            }
        }

        void Stack_Rear_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (this.Stack_Rear.IsVisible)
                StartFloatingText();
            else
            {
                StopFloatingText();

                // Set Stack 01 Left Margine to Zero
                Thickness mg = new Thickness();
                mg = this.Stack_Front.Margin;
                mg.Left = 0;
                this.Stack_Front.Margin = mg;
            }
        }

        private void Stack_FloatingText_SizeChanged(object sender, System.Windows.SizeChangedEventArgs e)
        {  
            if (this.Stack_Front.ActualWidth > this.Grid_FlowText.ActualWidth)
                this.Stack_Rear.Visibility = System.Windows.Visibility.Visible;
            else
                this.Stack_Rear.Visibility = System.Windows.Visibility.Hidden;
        }

        public void ClearLDSeqMachine()
        {
            foreach(TextBlock LDSeqMachineTextBlock in LDSeqMachineTextBlockList)
            {
                LDSeqMachineTextBlock.Text = "";
            }
        }

        public void AddLDSeqMachine(List<ITV.VD_ITV_PlanSeq> strMchnIDList)
        {
            for (int i = 0; i < strMchnIDList.Count; i++)
            {
                LDSeqMachineTextBlockList[i].Text = strMchnIDList[i].MchnID;

                if (LDSeqMachineTextBlockList[i].Text.Equals(ITV.ITV_User.gMchnID))
                {
                    LDSeqMachineTextBlockList[i].Foreground = new SolidColorBrush(Colors.Red); 
                }
                else
                {
                    LDSeqMachineTextBlockList[i].Foreground = PresentationMgr.brush_text_black;
                }
            }
        }

        public void SetTandemMachine(string strValue)
        {
            for (int i = 0; i < TendemMachineTextBlockList.Count; i++)
            {
                TendemMachineTextBlockList[i].Text = strValue;
            }
        }

        public void SetKPI(string strValue)
        {
            for (int i = 0; i < KPITextBlockList.Count; i++)
            {
                KPITextBlockList[i].Text = strValue;
            }
        }

        public void SetMessage(string strValue)
        {
            for (int i = 0; i < MessageTextBlockList.Count; i++)
            {
                MessageTextBlockList[i].Text = strValue;
            }
        }
    }  
}