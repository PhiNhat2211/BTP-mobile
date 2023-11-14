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
using System.Reflection;

namespace VMT_RMG
{
	/// <summary>
	/// Interaction logic for Keypad.xaml
	/// </summary>
	public partial class Keypad : UserControl
	{
        enum INPUTBOX_TYPE
        {
            TEXTBOX = 0,
            PASSWORDBOX
        };

        private TextBox mTextBox;
        private PasswordBox mPasswordBox;
        private INPUTBOX_TYPE mBoxType;

        public delegate void KeypadDoneCallback();
        public KeypadDoneCallback DoneCallback {get; set;}        

		public Keypad()
		{
			this.InitializeComponent();

            DoneCallback = null;
		}

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            // Init UI Skin
            InitSkinImage();
        }

        private void InitSkinImage()
        {
            //-----------------------------------------------------------------
            //- Button Defualt
            List<Button> btnNumberList = new List<Button>();
            btnNumberList.Add(this.Button_keypad_1);
            btnNumberList.Add(this.Button_keypad_2);
            btnNumberList.Add(this.Button_keypad_3);
            btnNumberList.Add(this.Button_keypad_4);
            btnNumberList.Add(this.Button_keypad_5);
            btnNumberList.Add(this.Button_keypad_6);
            btnNumberList.Add(this.Button_keypad_7);
            btnNumberList.Add(this.Button_keypad_8);
            btnNumberList.Add(this.Button_keypad_9);
            btnNumberList.Add(this.Button_keypad_0);
            btnNumberList.Add(this.Button_keypad_dot);
            foreach (Button btn in btnNumberList)
            {
                PresentationMgr.SetSkinButton(btn,
                    new BitmapImage(new Uri(@"/VMT_RTG;component/Images/Common/Keypad/keyboard_btn_number_default.png", UriKind.Relative)),
                    new BitmapImage(new Uri(@"/VMT_RTG;component/Images/Common/Keypad/keyboard_btn_number_press.png", UriKind.Relative)),
                    new BitmapImage(new Uri(@"/VMT_RTG;component/Images/Common/Keypad/keyboard_btn_number_default.png", UriKind.Relative))
                    );
            }

            List<Button> btnTextList = new List<Button>();
            btnTextList.Add(this.Button_keypad_Q);
            btnTextList.Add(this.Button_keypad_W);
            btnTextList.Add(this.Button_keypad_E);
            btnTextList.Add(this.Button_keypad_R);
            btnTextList.Add(this.Button_keypad_T);
            btnTextList.Add(this.Button_keypad_Y);
            btnTextList.Add(this.Button_keypad_U);
            btnTextList.Add(this.Button_keypad_I);
            btnTextList.Add(this.Button_keypad_O);
            btnTextList.Add(this.Button_keypad_P);
            btnTextList.Add(this.Button_keypad_A);
            btnTextList.Add(this.Button_keypad_S);
            btnTextList.Add(this.Button_keypad_D);
            btnTextList.Add(this.Button_keypad_F);
            btnTextList.Add(this.Button_keypad_G);
            btnTextList.Add(this.Button_keypad_H);
            btnTextList.Add(this.Button_keypad_J);
            btnTextList.Add(this.Button_keypad_K);
            btnTextList.Add(this.Button_keypad_L);
            btnTextList.Add(this.Button_keypad_Z);
            btnTextList.Add(this.Button_keypad_X);
            btnTextList.Add(this.Button_keypad_C);
            btnTextList.Add(this.Button_keypad_V);
            btnTextList.Add(this.Button_keypad_B);
            btnTextList.Add(this.Button_keypad_N);
            btnTextList.Add(this.Button_keypad_M);

            btnTextList.Add(this.Button_keypad_Truck_T);
            btnTextList.Add(this.Button_keypad_Truck_TH);
            foreach (Button btn in btnTextList)
            {
                PresentationMgr.SetSkinButton(btn,
                    new BitmapImage(new Uri(@"/VMT_RTG;component/Images/Common/Keypad/keyboard_btn_number_default.png", UriKind.Relative)),
                    new BitmapImage(new Uri(@"/VMT_RTG;component/Images/Common/Keypad/keyboard_btn_number_press.png", UriKind.Relative)),
                    new BitmapImage(new Uri(@"/VMT_RTG;component/Images/Common/Keypad/keyboard_btn_number_default.png", UriKind.Relative))
                    );
            }
            List<Button> btnFullList = new List<Button>();
            btnFullList.Add(this.Button_FullPad_1);
            btnFullList.Add(this.Button_FullPad_2);
            btnFullList.Add(this.Button_FullPad_3);
            btnFullList.Add(this.Button_FullPad_4);
            btnFullList.Add(this.Button_FullPad_5);
            btnFullList.Add(this.Button_FullPad_6);
            btnFullList.Add(this.Button_FullPad_7);
            btnFullList.Add(this.Button_FullPad_8);
            btnFullList.Add(this.Button_FullPad_9);
            btnFullList.Add(this.Button_FullPad_0);

            btnFullList.Add(this.Button_FullPad_Q);
            btnFullList.Add(this.Button_FullPad_W);
            btnFullList.Add(this.Button_FullPad_E);
            btnFullList.Add(this.Button_FullPad_R);
            btnFullList.Add(this.Button_FullPad_T);
            btnFullList.Add(this.Button_FullPad_Y);
            btnFullList.Add(this.Button_FullPad_U);
            btnFullList.Add(this.Button_FullPad_I);
            btnFullList.Add(this.Button_FullPad_O);
            btnFullList.Add(this.Button_FullPad_P);
            btnFullList.Add(this.Button_FullPad_A);
            btnFullList.Add(this.Button_FullPad_S);
            btnFullList.Add(this.Button_FullPad_D);
            btnFullList.Add(this.Button_FullPad_F);
            btnFullList.Add(this.Button_FullPad_G);
            btnFullList.Add(this.Button_FullPad_H);
            btnFullList.Add(this.Button_FullPad_J);
            btnFullList.Add(this.Button_FullPad_K);
            btnFullList.Add(this.Button_FullPad_L);
            btnFullList.Add(this.Button_FullPad_Z);
            btnFullList.Add(this.Button_FullPad_X);
            btnFullList.Add(this.Button_FullPad_C);
            btnFullList.Add(this.Button_FullPad_V);
            btnFullList.Add(this.Button_FullPad_B);
            btnFullList.Add(this.Button_FullPad_N);
            btnFullList.Add(this.Button_FullPad_M);

            foreach (Button btn in btnFullList)
            {
                PresentationMgr.SetSkinButton(btn,
                    new BitmapImage(new Uri(@"/VMT_RTG;component/Images/Common/Keypad/keyboard_btn_number_default.png", UriKind.Relative)),
                    new BitmapImage(new Uri(@"/VMT_RTG;component/Images/Common/Keypad/keyboard_btn_number_press.png", UriKind.Relative)),
                    new BitmapImage(new Uri(@"/VMT_RTG;component/Images/Common/Keypad/keyboard_btn_number_default.png", UriKind.Relative))
                    );
            }
            //-----------------------------------------------------------------
            //- Button Text Input Change
            {
                PresentationMgr.SetSkinButton(this.Button_keypad_abc,
                    new BitmapImage(new Uri(@"/VMT_RTG;component/Images/Common/Keypad/keyboard_btn_abcOR123_default.png", UriKind.Relative)),
                    new BitmapImage(new Uri(@"/VMT_RTG;component/Images/Common/Keypad/keyboard_btn_abcOR123_press.png", UriKind.Relative)),
                    new BitmapImage(new Uri(@"/VMT_RTG;component/Images/Common/Keypad/keyboard_btn_abcOR123_default.png", UriKind.Relative))
                    );
                PresentationMgr.SetSkinButton(this.Button_keypad_123,
                    new BitmapImage(new Uri(@"/VMT_RTG;component/Images/Common/Keypad/keyboard_btn_abcOR123_default.png", UriKind.Relative)),
                    new BitmapImage(new Uri(@"/VMT_RTG;component/Images/Common/Keypad/keyboard_btn_abcOR123_press.png", UriKind.Relative)),
                    new BitmapImage(new Uri(@"/VMT_RTG;component/Images/Common/Keypad/keyboard_btn_abcOR123_default.png", UriKind.Relative))
                    );
                PresentationMgr.SetSkinButton(this.Button_FullPad_123,
                   new BitmapImage(new Uri(@"/VMT_RTG;component/Images/Common/Keypad/keyboard_btn_abcOR123_default.png", UriKind.Relative)),
                   new BitmapImage(new Uri(@"/VMT_RTG;component/Images/Common/Keypad/keyboard_btn_abcOR123_press.png", UriKind.Relative)),
                   new BitmapImage(new Uri(@"/VMT_RTG;component/Images/Common/Keypad/keyboard_btn_abcOR123_default.png", UriKind.Relative))
                   );
            }

            //-----------------------------------------------------------------
            //- Button Delete
            {
                PresentationMgr.SetSkinButton(this.Button_keypad_del_1,
                    new BitmapImage(new Uri(@"/VMT_RTG;component/Images/Common/Keypad/keyboard_btn_del_default.png", UriKind.Relative)),
                    new BitmapImage(new Uri(@"/VMT_RTG;component/Images/Common/Keypad/keyboard_btn_del_press.png", UriKind.Relative)),
                    new BitmapImage(new Uri(@"/VMT_RTG;component/Images/Common/Keypad/keyboard_btn_del_default.png", UriKind.Relative))
                    );
                PresentationMgr.SetSkinButton(this.Button_keypad_del_2,
                    new BitmapImage(new Uri(@"/VMT_RTG;component/Images/Common/Keypad/keyboard_btn_del_default.png", UriKind.Relative)),
                    new BitmapImage(new Uri(@"/VMT_RTG;component/Images/Common/Keypad/keyboard_btn_del_press.png", UriKind.Relative)),
                    new BitmapImage(new Uri(@"/VMT_RTG;component/Images/Common/Keypad/keyboard_btn_del_default.png", UriKind.Relative))
                    );
                PresentationMgr.SetSkinButton(this.Button_FullPad_del_2,
                   new BitmapImage(new Uri(@"/VMT_RTG;component/Images/Common/Keypad/keyboard_btn_del_default.png", UriKind.Relative)),
                   new BitmapImage(new Uri(@"/VMT_RTG;component/Images/Common/Keypad/keyboard_btn_del_press.png", UriKind.Relative)),
                   new BitmapImage(new Uri(@"/VMT_RTG;component/Images/Common/Keypad/keyboard_btn_del_default.png", UriKind.Relative))
                   );
            }

            //-----------------------------------------------------------------
            //- Button Done
            {
                PresentationMgr.SetSkinButton(this.Button_keypad_done_1,
                    new BitmapImage(new Uri(@"/VMT_RTG;component/Images/Common/Keypad/keyboard_btn_done_default.png", UriKind.Relative)),
                    new BitmapImage(new Uri(@"/VMT_RTG;component/Images/Common/Keypad/keyboard_btn_done_press.png", UriKind.Relative)),
                    new BitmapImage(new Uri(@"/VMT_RTG;component/Images/Common/Keypad/keyboard_btn_done_default.png", UriKind.Relative))
                    );
                PresentationMgr.SetSkinButton(this.Button_keypad_done_2,
                    new BitmapImage(new Uri(@"/VMT_RTG;component/Images/Common/Keypad/keyboard_btn_done_default.png", UriKind.Relative)),
                    new BitmapImage(new Uri(@"/VMT_RTG;component/Images/Common/Keypad/keyboard_btn_done_press.png", UriKind.Relative)),
                    new BitmapImage(new Uri(@"/VMT_RTG;component/Images/Common/Keypad/keyboard_btn_done_default.png", UriKind.Relative))
                    );
                PresentationMgr.SetSkinButton(this.Button_FullPad_done_2,
                   new BitmapImage(new Uri(@"/VMT_RTG;component/Images/Common/Keypad/keyboard_btn_done_default.png", UriKind.Relative)),
                   new BitmapImage(new Uri(@"/VMT_RTG;component/Images/Common/Keypad/keyboard_btn_done_press.png", UriKind.Relative)),
                   new BitmapImage(new Uri(@"/VMT_RTG;component/Images/Common/Keypad/keyboard_btn_done_default.png", UriKind.Relative))
                   );
            }
        }        

        public void ShowKeyPad(TextBox tbox)
        {
            if (MainWindow.SERVICE_COMPANY.Equals("BTP"))
            {
                mBoxType = INPUTBOX_TYPE.TEXTBOX;
                mTextBox = tbox;

                this.Visibility = System.Windows.Visibility.Visible;
            }
            ShowDotButton(tbox.Name);
        }

        public void ShowKeyPad(PasswordBox tbox)
        {
            if (MainWindow.SERVICE_COMPANY.Equals("BTP"))
            {
                mBoxType = INPUTBOX_TYPE.PASSWORDBOX;
                mPasswordBox = tbox;

                this.Visibility = System.Windows.Visibility.Visible;
            }
            ShowDotButton(tbox.Name);
        }

        public void HideKeyPad()
        {
            this.Visibility = System.Windows.Visibility.Hidden;
        }

        private void ShowDotButton(String txtID)
        {
            //if (PresentationMgr.Singleton.CurrentUIMode == PresentationMgr.UIMode.LogInView) //20200824 add DOT button only for Login View
            if (txtID.ToUpper().Equals("TEXTBOX_IDNUMBER") || txtID.ToUpper().Equals("PASSWORDBOX_PASSWORD"))
            {
                Button_keypad_dot.Visibility = Visibility.Visible;
            } else
            {
                Button_keypad_dot.Visibility = Visibility.Hidden;
            }
        }

        public void ShowTruckKey(Boolean show)
        {
            this.Button_keypad_Truck_T.Visibility = this.Button_keypad_Truck_TH.Visibility =
                show ? Visibility.Hidden : Visibility.Hidden;
        }

        private void Button_keypad_digital_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (!(sender is Button))
                return;

            Button btn = sender as Button;
            int digi;
            Int32.TryParse(btn.Content.ToString(), out digi);

            if (mBoxType == INPUTBOX_TYPE.TEXTBOX)
            {
                if (mTextBox.Text.Length < mTextBox.MaxLength || mTextBox.MaxLength == 0)
                {
                    string tempValue = mTextBox.Text + digi.ToString();
                    mTextBox.Text += digi.ToString();
                    //Check change value on event Changed
                    if (tempValue != mTextBox.Text)
                    {
                        tempValue = mTextBox.Text;
                        mTextBox.Text = "";
                        mTextBox.Text = tempValue;
                    }
                }
                mTextBox.Select(mTextBox.Text.Length, 0);
            }
            else
            {
                if (mPasswordBox.Password.Length < mPasswordBox.MaxLength ||  mPasswordBox.MaxLength == 0)
                    mPasswordBox.Password += digi.ToString();

                mPasswordBox.GetType().GetMethod("Select", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(mPasswordBox, new object[] { mPasswordBox.Password.Length, 0 }); 
            }
		}

        private void Button_keypad_del_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (!(sender is Button))
                return;
            
            String text;
            if (mBoxType == INPUTBOX_TYPE.TEXTBOX)
                text = mTextBox.Text;
            else
                text = mPasswordBox.Password;

            if (text.Length > 0)
            {
                text = text.Remove(text.Length - 1);
            }

            if (mBoxType == INPUTBOX_TYPE.TEXTBOX)
            {
                mTextBox.Text = text;
                mTextBox.Select(mTextBox.Text.Length, 0);
            }
            else
            {
                mPasswordBox.Password = text;
                mPasswordBox.GetType().GetMethod("Select", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(mPasswordBox, new object[] { mPasswordBox.Password.Length, 0 });
            }
		}

        private void Button_keypad_done_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (!(sender is Button))
                return;
                        
            if (this.DoneCallback != null)
                this.DoneCallback();
		}
        
        private void Button_Text_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (!(sender is Button))
                return;

            Button btn = sender as Button;
            if (mBoxType == INPUTBOX_TYPE.TEXTBOX)
            {
                if (mTextBox.Text.Length < mTextBox.MaxLength || mTextBox.MaxLength == 0)
                {
                    string tempValue = mTextBox.Text + btn.Content;
                    mTextBox.Text += btn.Content;
                    //Check change value on event Changed
                    if (tempValue != mTextBox.Text)
                    {
                        tempValue = mTextBox.Text;
                        mTextBox.Text = "";
                        mTextBox.Text = tempValue;
                    }
                }
                mTextBox.Select(mTextBox.Text.Length, 0);
                //   mTextBox.Cursor = mTextBox.Text.Length;
            }
            else
            {
                if (mPasswordBox.Password.Length < mPasswordBox.MaxLength || mPasswordBox.MaxLength == 0)
                    mPasswordBox.Password += btn.Content;

                mPasswordBox.GetType().GetMethod("Select", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(mPasswordBox, new object[] { mPasswordBox.Password.Length, 0 });
            }
		}

        private void Button_keypad_text_abc_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (!(sender is Button))
                return;

            if (Grid_DigitalPad.Visibility == System.Windows.Visibility.Visible)
            {
                Grid_DigitalPad.Visibility = System.Windows.Visibility.Hidden;
                Grid_TextPad.Visibility = System.Windows.Visibility.Visible;
            }
            else
            {
                Grid_DigitalPad.Visibility = System.Windows.Visibility.Visible;
                Grid_TextPad.Visibility = System.Windows.Visibility.Hidden;
            }
		}
	}
}