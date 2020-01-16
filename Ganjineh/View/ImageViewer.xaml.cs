using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace Ganjineh
{
    /// <summary>
    /// Interaction logic for ImageViewer.xaml
    /// </summary>
    public partial class ImageViewer
    {
        public static ObservableCollection<Arts.ImageData> Items;
        public ImageViewer()
        {
            InitializeComponent();
        }

        private void GoToLoc_Click(object sender, RoutedEventArgs e)
        {
            Image item = img.Items[img.PageIndex] as Image;
            System.Diagnostics.Process.Start("explorer.exe", "/select, \"" + item.Source + "\"");
        }

        private async void main_Loaded(object sender, RoutedEventArgs e)
        {
            busyIndicator.IsBusy = true;
            foreach (Arts.ImageData item in Items)
            {
                await Dispatcher.InvokeAsync(() =>
                  {
                      Image slidImg = new Image();
                      using (FileStream imageStream = File.OpenRead(item.Location))
                      {
                          BitmapDecoder decoder = BitmapDecoder.Create(imageStream, BitmapCreateOptions.IgnoreColorProfile,
                              BitmapCacheOption.Default);
                          int width = decoder.Frames[0].PixelWidth;
                          slidImg.Source = new BitmapImage(new Uri(item.Location, UriKind.Absolute));
                          slidImg.Stretch = Stretch.Uniform;
                          slidImg.Width = width;
                      }
                      img.Items.Add(slidImg);
                  }, DispatcherPriority.Background);
            }
            busyIndicator.IsBusy = false;
        }
    }
}
