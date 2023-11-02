using Ganjineh.Data;
using Ganjineh.Model;
using HandyControl.Controls;
using HandyControl.Data;
using HandyControl.Tools.Extension;
using Newtonsoft.Json;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

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
            public string Location { get; set; }
            public ImageSource ImageSource { get; set; }
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
                foreach (string dir in System.IO.Directory.EnumerateDirectories(Path.Combine(GlobalData.Config.Location, $"{WriterName}")))
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

        private async void WriterBooks_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            LoadJsonInfo();
            await LoadFolder();
        }

        private bool isCanceled = false;

        private void LoadJsonInfo()
        {
            WriterData SelectedItem = lstBooks.SelectedItem as WriterData;
            if (SelectedItem != null)
            {
                string PATH = SelectedItem.Tag + @"\Info.json";
                if (File.Exists(PATH))
                {
                    ArtifactModel.RootObject m = JsonConvert.DeserializeObject<ArtifactModel.RootObject>(File.ReadAllText(PATH));
                    txtDetail.Text = m.description;

                    foreach (ArtifactModel.ArtifactTag item in m.artifactTags)
                    {
                        TextBlock txtSubject = new TextBlock();
                        StackPanel HorizontalStack = new StackPanel();
                        StackPanel VerticalValueStack = new StackPanel();

                        HorizontalStack.Orientation = Orientation.Horizontal;
                        HorizontalStack.Margin = new System.Windows.Thickness(5, 0, 5, 0);
                        txtSubject.Text = item.name;
                        txtSubject.Margin = new System.Windows.Thickness(10);
                        txtSubject.VerticalAlignment = System.Windows.VerticalAlignment.Center;
                        HorizontalStack.Children.Add(txtSubject);


                        foreach (ArtifactModel.Value subitem in item.values)
                        {
                            if (subitem.valueSupplement.Contains("http"))
                            {
                                Button btn = new Button
                                {
                                    Content = subitem.value,
                                    Margin = new System.Windows.Thickness(10, 10, 10, 15)
                                };
                                btn.Click += (s, e) => System.Diagnostics.Process.Start(subitem.valueSupplement);
                                VerticalValueStack.Children.Add(btn);
                            }
                            else
                            {
                                TextBlock txtValue = new TextBlock
                                {
                                    Text = subitem.value,
                                    Margin = new System.Windows.Thickness(10)
                                };

                                VerticalValueStack.Children.Add(txtValue);
                            }

                        }
                        HorizontalStack.Children.Add(VerticalValueStack);

                        stck.Children.Add(HorizontalStack);
                    }
                }
            }

        }

        public async Task LoadFolder()
        {
            Images.Clear();
            isCanceled = false;
            int index = 0;
            lstBooks.IsEnabled = lstWriter.IsEnabled = false;
            IProgress<int> progress = new Progress<int>(i => { prg.Value = i; });
            WriterData SelectedItem = lstBooks.SelectedItem as WriterData;
            if (SelectedItem != null)
            {
                string[] extensions = { ".jpg", ".jpeg", ".png", ".bmp" };
                System.Collections.Generic.IEnumerable<string> files = Directory.EnumerateFiles(SelectedItem.Tag, "*.*").Where(s => extensions.Any(ext => ext == Path.GetExtension(s)));
                foreach (string path in files)
                {
                    if (!isCanceled)
                    {
                        index += 1;
                        progress.Report((int)((double)(index) / files.Count() * 100)); // گزارش پیشرفت کار
                        Images.Add(new ImageData
                        {
                            Name = Path.GetFileNameWithoutExtension(path),
                            Location = path,
                            ImageSource = await LoadImage(path)
                        });
                    }
                }
                Button_Click(null, null);
            }

        }
        public Task<BitmapImage> LoadImage(string path)
        {
            return Task.Run(() =>
            {
                BitmapImage bitmap = new BitmapImage();

                using (FileStream stream = new FileStream(path, FileMode.Open, FileAccess.Read))
                {
                    bitmap.BeginInit();
                    bitmap.DecodePixelHeight = 350;
                    bitmap.CacheOption = BitmapCacheOption.OnLoad;
                    bitmap.StreamSource = stream;
                    bitmap.EndInit();
                    bitmap.Freeze();
                }

                return bitmap;
            });
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
