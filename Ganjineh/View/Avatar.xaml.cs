using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Ganjineh
{
    /// <summary>
    /// Interaction logic for Avatar.xaml
    /// </summary>
    public partial class Avatar : UserControl
    {
        public Avatar()
        {
            InitializeComponent();
        }
        public static readonly DependencyProperty SourceProperty = DependencyProperty.Register(
           "Source", typeof(ImageSource), typeof(Avatar), new PropertyMetadata(default(ImageSource)));

        public ImageSource Source
        {
            get => (ImageSource)GetValue(SourceProperty);
            set => SetValue(SourceProperty, value);
        }

        public static readonly DependencyProperty UserNameProperty = DependencyProperty.Register(
            "DisplayName", typeof(string), typeof(Avatar), new PropertyMetadata(default(string)));

        public string DisplayName
        {
            get => (string)GetValue(UserNameProperty);
            set => SetValue(UserNameProperty, value);
        }

        public static readonly DependencyProperty LinkProperty = DependencyProperty.Register(
            "Link", typeof(string), typeof(Avatar), new PropertyMetadata(default(string)));

        public string Link
        {
            get => (string)GetValue(LinkProperty);
            set => SetValue(LinkProperty, value);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ObservableCollection<Arts.ImageData> datas = new ObservableCollection<Arts.ImageData>
            {
                new Arts.ImageData { ImageSource = Source, Name = DisplayName, Location = Link }
            };
            ImageViewer.Items = datas;
            new ImageViewer().ShowDialog();
        }
    }
}
