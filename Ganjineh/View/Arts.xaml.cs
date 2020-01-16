using Ganjineh.Data;
using HandyControl.Controls;
using HandyControl.Data;
using HandyControl.Tools.Extension;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Threading;

namespace Ganjineh
{
    /// <summary>
    /// Interaction logic for Arts.xaml
    /// </summary>
    public partial class Arts : UserControl
    {
        public class WriterData
        {
            public string Name { get; set; }
            public string Tag { get; set; }
        }

        public class ImageData
        {
            public string Name { get; set; }
            public string ImageSource { get; set; }
        }

        public ObservableCollection<WriterData> WriterNames { get; } = new ObservableCollection<WriterData>(); // کالکشن مربوط به نام نویسندگان

        public ObservableCollection<WriterData> WriterBooks { get; } = new ObservableCollection<WriterData>(); // کالکشن مربوط به نام کتاب ها

        public ObservableCollection<ImageData> Images { get; } = new ObservableCollection<ImageData>(); // کالکشن مربوط به تصاویر

        public Arts()
        {
            InitializeComponent();
            GetWriterNames();
            DataContext = this;
        }

        private void GetWriterNames()
        {
            try
            {
                WriterNames.Clear();
                busyIndicatorWriter.IsBusy = true;
                foreach (string dir in System.IO.Directory.EnumerateDirectories(GlobalData.Config.Location))
                {
                    WriterNames.Add(new WriterData
                    {
                        Name = dir.Replace(Path.GetDirectoryName(dir) + Path.DirectorySeparatorChar, ""),
                        Tag = dir
                    });
                }
                busyIndicatorWriter.IsBusy = false;
            }
            catch (DirectoryNotFoundException ex)
            {
                busyIndicatorWriter.IsBusy = false;
                Growl.ErrorGlobal("پوشه مورد نظر وجود ندارد \n" + ex.Message);
            }
        }

        private void GetWriterBooks(string WriterName)
        {
            try
            {
                WriterBooks.Clear();
                busyIndicatorBooks.IsBusy = true;
                foreach (string dir in System.IO.Directory.EnumerateDirectories(GlobalData.Config.Location + $"{WriterName}"))
                {
                    WriterBooks.Add(new WriterData
                    {
                        Name = dir.Replace(Path.GetDirectoryName(dir) + Path.DirectorySeparatorChar, ""),
                        Tag = dir
                    });
                }
                busyIndicatorBooks.IsBusy = false;
            }
            catch (DirectoryNotFoundException ex)
            {
                busyIndicatorBooks.IsBusy = false;
                Growl.ErrorGlobal("پوشه مورد نظر وجود ندارد \n" + ex.Message);
            }
        }

        private void WriterNames_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            GetWriterBooks((lstWriter.SelectedItem as WriterData).Name);
        }

        private bool isCanceled = false;
        private void WriterBooks_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            WriterData SelectedItem = lstBooks.SelectedItem as WriterData;
            if (SelectedItem != null)
            {
                lstBooks.IsEnabled = lstWriter.IsEnabled = false;
                Images.Clear();
                isCanceled = false;
                int index = 0;
                IProgress<int> progress = new Progress<int>(i => { prg.Value = i; });


                string[] extensions = { ".jpg", ".jpeg", ".png", ".bmp" };
                System.Collections.Generic.IEnumerable<string> files = Directory.EnumerateFiles(SelectedItem.Tag, "*.*").Where(s => extensions.Any(ext => ext == Path.GetExtension(s)));
                foreach (string path in files)
                {
                    if (!isCanceled)
                    {
                        index += 1;
                        progress.Report((int)((double)(index) / files.Count() * 100)); // گزارش پیشرفت کار
                        Dispatcher.Invoke(() =>
                        {
                            Images.Add(new ImageData { ImageSource = path, Name = Path.GetFileNameWithoutExtension(path) });
                        }, DispatcherPriority.Background);
                    }
                }
                Button_Click(null, null);
            }
        }
        private void SearchWriterNames_OnSearchStarted(object sender, FunctionEventArgs<string> e)
        {
            if (e.Info == null)
            {
                return;
            }

            foreach (WriterData item in lstWriter.Items)
            {
                ListBoxItem listBoxItem = lstWriter.ItemContainerGenerator.ContainerFromItem(item) as ListBoxItem;
                listBoxItem?.Show(item.Name.ToLower().Contains(e.Info.ToLower()));
            }
        }

        private void SearchWriterBooks_OnSearchStarted(object sender, FunctionEventArgs<string> e)
        {
            if (e.Info == null)
            {
                return;
            }

            foreach (WriterData item in lstBooks.Items)
            {
                ListBoxItem listBoxItem = lstBooks.ItemContainerGenerator.ContainerFromItem(item) as ListBoxItem;
                listBoxItem?.Show(item.Name.ToLower().Contains(e.Info.ToLower()));
            }
        }

        private void Button_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            isCanceled = true;
            lstBooks.IsEnabled = lstWriter.IsEnabled = true;
        }

        private void ButtonSlide_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            ImageViewer.Items = Images;
            new ImageViewer().ShowDialog();
        }
    }
}
