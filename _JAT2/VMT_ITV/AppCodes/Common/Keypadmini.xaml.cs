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
using System.Windows.Shapes;

namespace VMT_ITV
{
    /// <summary>
    /// Interaction logic for Keypadmini.xaml
    /// </summary>
    public partial class Keypadmini : UserControl
    {
        enum INPUTBOX_TYPE
        {
            TEXTBOX = 0,
            PASSWORDBOX
        };

        private MainWindow mMainWindow;
        private TextBox mTextBox;
        private PasswordBox mPasswordBox;
        private INPUTBOX_TYPE mBoxType;

        public Keypadmini()
        {
            this.InitializeComponent();
        }

        public void Init(MainWindow mainWin)
        {
            mMainWindow = mainWin;
        }

        public void ShowKeyPad(TextBox tbox)
        {
            if (MainWindow.SERVICE_COMPANY.Equals("BTP"))
            {
                mBoxType = INPUTBOX_TYPE.TEXTBOX;
                mTextBox = tbox;

                this.Visibility = System.Windows.Visibility.Visible;
            }
        }

        public void ShowKeyPad(PasswordBox tbox)
        {
            if (MainWindow.SERVICE_COMPANY.Equals("BTP"))
            {
                mBoxType = INPUTBOX_TYPE.PASSWORDBOX;
                mPasswordBox = tbox;

                this.Visibility = System.Windows.Visibility.Visible;
            }
        }

        public void HideKeyPad()
        {
            this.Visibility = Visibility.Hidden;
        }

        private void Grid_keypad_digital_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {

            Grid grid = (Grid)sender;
            List<Image> imageList = new List<Image>();

            PresentationMgr.FindChildByType<Image>((DependencyObject)grid, imageList);
            foreach (Image img in imageList)
            {
                img.Source = PresentationMgr.AppWin.getImageByDayOrNight(@"g_keyboard_btn_02_default.png");

            }

        }

        private void Grid_keypad_digital_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {

            Grid grid = (Grid)sender;
            List<Image> imageList = new List<Image>();

            PresentationMgr.FindChildByType<Image>((DependencyObject)grid, imageList);
            foreach (Image img in imageList)
            {
                img.Source = PresentationMgr.AppWin.getImageByDayOrNight(@"g_keyboard_btn_02_press.png");

            }
        }

        private void Grid_keypad_digital_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {

            Grid grid = (Grid)sender;
            List<Image> imageList = new List<Image>();

            PresentationMgr.FindChildByType<Image>((DependencyObject)grid, imageList);
            foreach (Image img in imageList)
            {
                img.Source = PresentationMgr.AppWin.getImageByDayOrNight(@"g_keyboard_btn_02_default.png");

            }
            int digi = Convert.ToInt32(grid.Uid) - 10000;

            if (mBoxType == INPUTBOX_TYPE.TEXTBOX)
            {
                if (mTextBox.Text.Length < mTextBox.MaxLength || mTextBox.MaxLength == 0)
                    mTextBox.Text += digi.ToString();
                mTextBox.Select(mTextBox.Text.Length, 0);
                //   mTextBox.Cursor = mTextBox.Text.Length;
            }
            else
            {
                if (mPasswordBox.Password.Length < mPasswordBox.MaxLength || mPasswordBox.MaxLength == 0)
                    mPasswordBox.Password += digi.ToString();

                //mPasswordBox.GetType().GetMethod("Select", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(mPasswordBox, new object[] { mPasswordBox.Password.Length, 0 });
            }

        }

        private void Grid_keypad_del_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {

            Grid grid = (Grid)sender;
            List<Image> imageList = new List<Image>();

            PresentationMgr.FindChildByType<Image>((DependencyObject)grid, imageList);
            foreach (Image img in imageList)
            {
                img.Source = PresentationMgr.AppWin.getImageByDayOrNight(@"g_keyboard_btn_del_default.png");

            }
        }

        private void Grid_keypad_del_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {

            Grid grid = (Grid)sender;
            List<Image> imageList = new List<Image>();

            PresentationMgr.FindChildByType<Image>((DependencyObject)grid, imageList);
            foreach (Image img in imageList)
            {
                img.Source = PresentationMgr.AppWin.getImageByDayOrNight(@"g_keyboard_btn_del_press.png");

            }
        }

        private void Grid_keypad_del_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {

            Grid grid = (Grid)sender;
            List<Image> imageList = new List<Image>();

            PresentationMgr.FindChildByType<Image>((DependencyObject)grid, imageList);
            foreach (Image img in imageList)
            {
                img.Source = PresentationMgr.AppWin.getImageByDayOrNight(@"g_keyboard_btn_del_default.png");

            }
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
                mPasswordBox.Password = text;
        }

        private void Grid_keypad_done_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {

            Grid grid = (Grid)sender;
            List<Image> imageList = new List<Image>();

            PresentationMgr.FindChildByType<Image>((DependencyObject)grid, imageList);
            foreach (Image img in imageList)
            {
                img.Source = PresentationMgr.AppWin.getImageByDayOrNight(@"g_keyboard_btn_done_default.png");

            }
        }

        private void Grid_keypad_done_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {

            Grid grid = (Grid)sender;
            List<Image> imageList = new List<Image>();

            PresentationMgr.FindChildByType<Image>((DependencyObject)grid, imageList);
            foreach (Image img in imageList)
            {
                img.Source = PresentationMgr.AppWin.getImageByDayOrNight(@"g_keyboard_btn_done_press.png");

            }
        }

        private void Grid_keypad_done_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {

            Grid grid = (Grid)sender;
            List<Image> imageList = new List<Image>();

            PresentationMgr.FindChildByType<Image>((DependencyObject)grid, imageList);
            foreach (Image img in imageList)
            {
                img.Source = PresentationMgr.AppWin.getImageByDayOrNight(@"g_keyboard_btn_done_default.png");

            }
            HideKeyPad();
        }

        private void TextBlock_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {

            TextBlock tblock = (TextBlock)sender;
            Grid grid = (Grid)tblock.Parent;
            List<Image> imageList = new List<Image>();

            PresentationMgr.FindChildByType<Image>((DependencyObject)grid, imageList);
            foreach (Image img in imageList)
            {
                img.Source = PresentationMgr.AppWin.getImageByDayOrNight(@"g_keyboard_btn_02_press.png");

            }
        }

        private void TextBlock_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {

            TextBlock tblock = (TextBlock)sender;
            Grid grid = (Grid)tblock.Parent;
            List<Image> imageList = new List<Image>();

            PresentationMgr.FindChildByType<Image>((DependencyObject)grid, imageList);
            foreach (Image img in imageList)
            {
                img.Source = PresentationMgr.AppWin.getImageByDayOrNight(@"g_keyboard_btn_02_default.png");

            }
        }

        private void TextBlock_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {

            TextBlock tblock = (TextBlock)sender;
            Grid grid = (Grid)tblock.Parent;
            List<Image> imageList = new List<Image>();

            PresentationMgr.FindChildByType<Image>((DependencyObject)grid, imageList);
            foreach (Image img in imageList)
            {
                img.Source = PresentationMgr.AppWin.getImageByDayOrNight(@"g_keyboard_btn_02_default.png");

            }

            if (mBoxType == INPUTBOX_TYPE.TEXTBOX)
            {
                if (mTextBox.Text.Length < mTextBox.MaxLength || mTextBox.MaxLength == 0)
                    mTextBox.Text += tblock.Text;
                mTextBox.Select(mTextBox.Text.Length, 0);
                //   mTextBox.Cursor = mTextBox.Text.Length;
            }
            else
            {
                if (mPasswordBox.Password.Length < mPasswordBox.MaxLength || mPasswordBox.MaxLength == 0)
                    mPasswordBox.Password += tblock.Text;

                //mPasswordBox.GetType().GetMethod("Select", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(mPasswordBox, new object[] { mPasswordBox.Password.Length, 0 });
            }
        }
    }
}
