using System.Reflection;
using System.Windows.Controls;

namespace Ganjineh
{
    /// <summary>
    /// Interaction logic for About.xaml
    /// </summary>
    public partial class About : UserControl
    {
        public About()
        {
            InitializeComponent();
            txtVersion.Text = "نسخه " + Assembly.GetExecutingAssembly().GetName().Version.ToString();
        }
    }
}
