using Ganjineh.Data;
using HandyControl.Controls;
using HandyControl.Data;
using System.Windows;
using System.Windows.Controls;
namespace Ganjineh
{
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        #region Change Skin
        private void ButtonConfig_OnClick(object sender, RoutedEventArgs e)
        {
            PopupConfig.IsOpen = true;
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
                PopupConfig.IsOpen = false;
                if (tag.Equals(GlobalData.Config.Skin))
                {
                    return;
                }

                GlobalData.Config.Skin = tag;
                GlobalData.Save();
                ((App)Application.Current).UpdateSkin(tag);
            }
        }
        #endregion

        /// <summary>
        /// تغییر ویو های برنامه
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SideMenuItem_Selected(object sender, RoutedEventArgs e)
        {
            SideMenuItem selectedItem = sender as SideMenuItem;
            switch (selectedItem.Tag)
            {
                case "art":
                    contentView.Content = new Arts();
                    break;
                case "setting":
                    contentView.Content = new Settings();
                    break;
                case "dev":
                    contentView.Content = new About();
                    break;
                case "update":
                    contentView.Content = new Update();
                    break;
                case "download":
                    new DownloaderWindow().ShowDialog();
                    break;
            }
        }
    }
}
