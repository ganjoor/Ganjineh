using Ganjineh.Data;
using HandyControl.Data;
using System.Windows;
using System.Windows.Controls;

namespace Ganjineh
{
    /// <summary>
    /// Interaction logic for Settings.xaml
    /// </summary>
    public partial class Settings : UserControl
    {
        public Settings()
        {
            InitializeComponent();

            InitSettings();
        }

        /// <summary>
        /// دریافت تنظیمات پیشفرض
        /// </summary>
        private void InitSettings()
        {
            txtBrowse.Text = GlobalData.Config.Location;
        }

        /// <summary>
        /// ذخیره مسیر دانلود فایل ها
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            using (System.Windows.Forms.FolderBrowserDialog dialog = new System.Windows.Forms.FolderBrowserDialog())
            {
                System.Windows.Forms.DialogResult result = dialog.ShowDialog();
                if (result == System.Windows.Forms.DialogResult.OK)
                {
                    txtBrowse.Text = dialog.SelectedPath + @"\";
                    GlobalData.Config.Location = txtBrowse.Text;
                    GlobalData.Save();
                }
            }
        }

        /// <summary>
        /// تغییر پوسته برنامه
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonSkins_OnClick(object sender, RoutedEventArgs e)
        {
            if (e.OriginalSource is Button button && button.Tag is SkinType tag)
            {
                if (tag.Equals(GlobalData.Config.Skin))
                {
                    return;
                }

                GlobalData.Config.Skin = tag;
                GlobalData.Save();
                ((App)Application.Current).UpdateSkin(tag);
            }
        }
    }
}
