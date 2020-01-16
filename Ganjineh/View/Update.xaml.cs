using HandyControl.Controls;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;

namespace Ganjineh
{
    /// <summary>
    /// Interaction logic for Update.xaml
    /// </summary>
    public partial class Update : UserControl
    {
        public Update()
        {
            InitializeComponent();
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                UpdateHelper.GithubReleaseModel ver = UpdateHelper.CheckForUpdateGithubRelease("ghost1372", "Ganjineh");
                lblCreatedAt.Text = ver.CreatedAt.ToString();
                lblPublishedAt.Text = ver.PublishedAt.ToString();
                lblDownloadUrl.CommandParameter = lblDownloadUrl.Content = ver.Asset[0].browser_download_url;
                lblCurrentVersion.Text = Assembly.GetExecutingAssembly().GetName().Version.ToString();
                lblVersion.Text = ver.TagName.Replace("v", "");
                txtChangelog.Text = ver.Changelog;
                if (ver.IsExistNewVersion)
                {
                    Growl.InfoGlobal("نسخه جدید پیدا شد");
                }
                else
                {
                    Growl.ErrorGlobal("شما از آخرین نسخه استفاده می کنید");
                }
            }
            catch (System.Exception)
            {

                Growl.ErrorGlobal("نسخه جدیدی پیدا نشد");
            }
        }
    }
}
