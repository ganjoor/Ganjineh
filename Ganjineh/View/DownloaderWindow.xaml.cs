using Ganjineh.Data;
using Ganjineh.Model;
using HandyControl.Controls;
using HandyControl.Tools;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Ganjineh
{
    /// <summary>
    /// Interaction logic for DownloaderWindow.xaml
    /// </summary>
    public partial class DownloaderWindow
    {
        private const string ContributorAPI = "https://api.ganjoor.net/api/artifacts/tagbundle/contributor"; // دریافت لیست نویسندگان
        private const string ContributorTagAPI = "https://api.ganjoor.net/api/artifacts/tagged/contributor/{0}"; // دریافت لیست کتب نویسندگان
        private const string GetImagesAndInfoAPI = "https://api.ganjoor.net/api/artifacts/{0}"; // دریافت اطلاعات کتاب شامل عکس و توضیحات
        

        public ObservableCollection<CheckTreeSource> TreeRoot { get; set; } // کالکشن مربوط به لیست نویسندگان و آثار آنها
        public ObservableCollection<CheckTreeSource> SelectedItems = new ObservableCollection<CheckTreeSource>(); // کالکشن مربوط به آیتم های انتخاب شده
        private readonly Queue<DownloadModel> Books = new Queue<DownloadModel>(); // کالکشن مربوط به لیست ایتم های موردنظر جهت دانلود

        private int SelectedItemsCount = 0; // تعداد کل آیتم ها
        private string TempFileName = string.Empty; // متغیر موقت جهت نگه داری ادرس فایلی که بصورت ناقص دانلود شده است


        public DownloaderWindow()
        {
            InitializeComponent();
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //هنگام اجرای برنامه به جهت حجم بالای اطلاعات نیاز هست که پنل بارگزاری نمایش داده شود
            isbusy.IsBusy = true;
            chkAll.IsEnabled = false;

            txtBrowse.Text = GlobalData.Config.Location;

            //دریافت اطلاعات جیسون مربوط به نویسندگان و آثار آنها
            string responseContributor = await GetTaskAsync(ContributorAPI);
            ContributorModel.RootObject resultContributor = await Task.Run(() => JsonConvert.DeserializeObject<ContributorModel.RootObject>(responseContributor));
            TreeRoot = new ObservableCollection<CheckTreeSource>();
            foreach (ContributorModel.Value item in resultContributor.values)
            {
                CheckTreeSource contributorItem = new CheckTreeSource() { Text = item.name, IsChecked = false, CTag = item.friendlyUrl };
                string responseContributorTag = await GetTaskAsync(string.Format(ContributorTagAPI, item.friendlyUrl));
                List<ContributorTagModel.RootObject> resultContributorTag = await Task.Run(() => JsonConvert.DeserializeObject<List<ContributorTagModel.RootObject>>(responseContributorTag));
                foreach (ContributorTagModel.RootObject itemTag in resultContributorTag)
                {
                    CheckTreeSource contributorTagItem = new CheckTreeSource() { Text = itemTag.name, IsChecked = false, CTag = itemTag.friendlyUrl };
                    contributorItem.Add(contributorTagItem);
                }
                TreeRoot.Add(contributorItem);
            }

            // بعد از پایان بارگزاری اطلاعات پنل بارگزاری باید بسته شود
            isbusy.IsBusy = false;
            chkAll.IsEnabled = btnDownload.IsEnabled = true;

            DataContext = this;
        }

        /// <summary>
        /// ارسال متد گت بصورت ناهمزمان
        /// </summary>
        /// <param name="API"></param>
        /// <returns></returns>
        private async Task<string> GetTaskAsync(string API)
        {
            using (WebClient client = new WebClient())
            {
                client.Encoding = Encoding.UTF8;
                return await client.DownloadStringTaskAsync(API);
            }
        }

        private void btnBrowse_Click(object sender, RoutedEventArgs e)
        {
            // ذخیره مسیر پیشفرض برنامه جهت دانلود فایل ها
            using (System.Windows.Forms.FolderBrowserDialog dialog = new System.Windows.Forms.FolderBrowserDialog())
            {
                System.Windows.Forms.DialogResult result = dialog.ShowDialog();
                if (result == System.Windows.Forms.DialogResult.OK)
                {
                    GlobalData.Config.Location = txtBrowse.Text = dialog.SelectedPath + @"\";
                    GlobalData.Save();
                }
            }
        }

        public class DownloadModel
        {
            public string JsonUrl { get; set; }
            public string FriendlyId { get; set; }
            public string BookName { get; set; }
            public string ContributorName { get; set; }
        }

        private void GetSelectedItems()
        {
            SelectedItems.Clear();
            SelectedItemsCount = 0;

            foreach (object item in chkTree.Items)
            {
                foreach (CheckTreeSource subitem in ((CheckTreeSource)item).Children)
                {
                    if (subitem.IsChecked == true)
                    {
                        SelectedItemsCount++;
                        SelectedItems.Add(new CheckTreeSource { Text = subitem.Text, IsChecked = subitem.IsChecked, CTag = subitem.CTag, Children = subitem.Children, Parent = subitem.Parent });
                    }
                    else if (subitem.IsChecked == false)
                    {
                        List<CheckTreeSource> toRemove = SelectedItems.Where(x => x.Text == subitem.Text).ToList();
                        foreach (CheckTreeSource remitem in toRemove)
                        {
                            SelectedItemsCount--;
                            SelectedItems.Remove(remitem);
                        }
                    }
                }
            }
        }

        private void btnDownload_Checked(object sender, RoutedEventArgs e)
        {
            btnDownload.Content = "انصراف";
            btnDownload.Style = ResourceHelper.GetResource<Style>("ToggleButtonDanger") as Style;

            GetSelectedItems();
            chkAll.IsEnabled = chkTree.IsEnabled = false;

            // تمام ایتم های انتخاب شده را در کالکشن ذخیره میکنیم
            // همینطور چک میکنیم که گزینه تکراری وجود نداشته باشه
            foreach (CheckTreeSource item in SelectedItems)
            {
                if (item.Children == null)
                {
                    if (Books != null || Books.Any(p => p.BookName == item.Text || p.ContributorName == item.Parent.Text || p.FriendlyId == item.CTag) == false)
                    {
                        Books.Enqueue(new DownloadModel { JsonUrl = string.Format(GetImagesAndInfoAPI, item.CTag), BookName = item.Text, ContributorName = item.Parent.Text, FriendlyId = item.CTag });
                    }
                }
                else
                {
                    foreach (CheckTreeSource subItem in item.Children)
                    {
                        if (Books != null || Books.Any(p => p.BookName == item.Text || p.ContributorName == item.Parent.Text || p.FriendlyId == item.CTag) == false)
                        {
                            Books.Enqueue(new DownloadModel { JsonUrl = string.Format(GetImagesAndInfoAPI, subItem.CTag), BookName = subItem.Text, ContributorName = item.Text, FriendlyId = subItem.CTag });
                        }
                    }
                }
            }
            if(!Books.Any())
            {
                Growl.ErrorGlobal("مورد دریافت نشده‌ای انتخاب نشده.");
                return;
            }
            DownloadFile();
        }

        /// <summary>
        /// حذف کاراکتر های غیرمجاز و طول بیشتر از 260 کاراکتر در نام گذاری فایل و پوشه
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public string ReplaceInvalidChars(string filename)
        {
            string removedInvalidChars = string.Join("_", filename.Split(System.IO.Path.GetInvalidFileNameChars()));
            if (removedInvalidChars.Length > 259)
            {
                removedInvalidChars = removedInvalidChars.Substring(0, 259);
            }
            return removedInvalidChars.Trim();
        }


        private void btnDownload_Unchecked(object sender, RoutedEventArgs e)
        {
            btnDownload.Content = "دانلود آثار انتخاب شده";
            btnDownload.Style = ResourceHelper.GetResource<Style>("ToggleButtonPrimary") as Style;
            // اگر دانلود کنسل شد کالکشن ها باید خالی شوند
            Books.Clear();
            client?.CancelAsync();
            prgStatus.Value = prg.Value = 0;
            chkAll.IsEnabled = chkTree.IsEnabled = true;
            
        }

        private void chkAll_Checked(object sender, RoutedEventArgs e)
        {
            foreach (object item in chkTree.Items)
            {
                ((CheckTreeSource)item).IsChecked = chkAll.IsChecked.Value;
                foreach (CheckTreeSource subItem in ((CheckTreeSource)item).Children)
                {
                    subItem.IsChecked = chkAll.IsChecked.Value;
                }
            }
        }

        private void CheckBox_Click(object sender, RoutedEventArgs e)
        {
            CheckBox checkBox = (CheckBox)sender;
            CheckTreeSource source = (CheckTreeSource)checkBox.DataContext;

            source.UpdateChildStatus();
            source.UpdateParentStatus();
        }

        #region Downloader
        private WebClient client = new WebClient();

        private int currentIndex, currentImageIndex = 0;
        private async void DownloadFile()
        {
            if (btnDownload.IsChecked.Value)
            {
                if (Books.Any())
                {
                    try
                    {
                        IProgress<int> progress = new Progress<int>(i => { prgStatus.Value = i; });

                        //حذف لینک موردنظر از لیست
                        DownloadModel url = Books.Dequeue();

                        // دریافت نام کتاب و نویسنده بصورت اصلاح شده
                        string cleanedSubItem = ReplaceInvalidChars(url.BookName);
                        string ContributorName = ReplaceInvalidChars(url.ContributorName);

                        string JSON_PATH = GlobalData.Config.Location + $@"\{ContributorName}\{cleanedSubItem}\Info.json"; // مسیر ذخیره فایل توضیحات کتاب
                        string PATH = GlobalData.Config.Location + $@"\{ContributorName}\{cleanedSubItem}"; // مسیر ذخیره فایل ها که درون پوشه نویسنده/کتاب می باشد

                        // اگر پوشه های موردنظر نبود، ایجاد شود
                        if (!Directory.Exists(PATH))
                        {
                            Directory.CreateDirectory(PATH);
                        }
                        // دریافت اطلاعات جیسون مربوط به کتاب
                        string responseContributor = await GetTaskAsync(string.Format(GetImagesAndInfoAPI, url.FriendlyId));
                        ArtifactModel.RootObject resultContributor = JsonConvert.DeserializeObject<ArtifactModel.RootObject>(responseContributor);

                        // اگر فایل جیسون نبود، ایجاد شود
                        if (!File.Exists(JSON_PATH))
                        {
                            ArtifactModel.RootObject root = new ArtifactModel.RootObject
                            {
                                artifactTags = resultContributor.artifactTags,
                                contents = resultContributor.contents,
                                dateTime = resultContributor.dateTime,
                                description = resultContributor.description,
                                descriptionInEnglish = resultContributor.descriptionInEnglish,
                                friendlyUrl = resultContributor.friendlyUrl,
                                id = resultContributor.id,
                                itemCount = resultContributor.itemCount,
                                lastModified = resultContributor.lastModified,
                                name = resultContributor.name,
                                nameInEnglish = resultContributor.nameInEnglish,
                                rTagSums = resultContributor.rTagSums,
                                status = resultContributor.status,
                                tags = resultContributor.tags,
                                coverImage = resultContributor.coverImage,
                                coverImageId = resultContributor.coverImageId,
                                coverItemIndex = resultContributor.coverItemIndex
                            };

                            // ذخیره فایل جیسون
                            string json = JsonConvert.SerializeObject(root);
                            File.WriteAllText(JSON_PATH, json);
                        }

                        // گزارش پیشرفت برای بیشتر از 1 مجموعه
                        if (Books.Count > 0)
                        {
                            currentIndex++;
                            progress.Report((int)((double)(currentIndex) / SelectedItemsCount * 100)); // گزارش پیشرفت کار
                            txtCurrentIndex.Text = $"مجموعه دانلود شده {currentIndex} از {SelectedItemsCount}";
                            txtCurrentBook.Text = $"درحال دانلود کتاب {url.BookName} از {url.ContributorName}";
                        }
                       

                        // دریافت تصاویر مربوط به کتاب
                        foreach (ArtifactModel.Item item in resultContributor.items)
                        {
                            if (btnDownload.IsChecked.Value)
                            {
                                // گزارش پیشرفت تنها برای 1 مجموعه انتخاب شده
                                if (Books.Count == 0)
                                {
                                    progress.Report((int)((double)(currentImageIndex) / resultContributor.items.Count * 100)); // گزارش پیشرفت کار
                                    txtCurrentIndex.Text = string.Empty;
                                    txtCurrentBook.Text = $"درحال دانلود کتاب {url.BookName} از {url.ContributorName}";
                                }

                                txtCurrentImageIndex.Text = $"تصویر دانلود شده {currentImageIndex++} از {resultContributor.items.Count}";

                                // و اگر فایل قبلا دانلود نشده بود - موجود نباشد
                                if (!File.Exists(PATH + $@"\{item.images[0].originalFileName}"))
                                {
                                    TempFileName = PATH + $@"\{item.images[0].originalFileName}";
                                    using (client = new WebClient())
                                    {
                                        client.DownloadProgressChanged += Client_DownloadProgressChanged;
                                        client.DownloadFileCompleted += Client_DownloadFileCompleted;
                                        string imageUrl = item.images[0].externalNormalSizeImageUrl.Replace("/norm/", $"/{((ComboBoxItem)cmbSize.SelectedItem).Tag}/");
                                        try
                                        {
                                            await client.DownloadFileTaskAsync(imageUrl, PATH + $@"\{item.images[0].originalFileName}");
                                        }
                                        catch
                                        {
                                            await client.DownloadFileTaskAsync(item.images[0].externalNormalSizeImageUrl, PATH + $@"\{item.images[0].originalFileName}");
                                        }
                                        
                                    }
                                }
                            }
                        }
                        txtCurrentImageIndex.Text = $"تصویر دانلود شده {currentImageIndex++} از {resultContributor.items.Count}";

                        currentImageIndex = 0;

                        DownloadFile();
                        return;
                    }
                    catch (Exception exp)
                    {
                        Growl.ErrorGlobal($"این خطا رخ داد: {exp}");
                        prgStatus.Value = prg.Value = 0;
                        chkTree.IsEnabled = true;
                        client.Dispose();
                        currentIndex = currentImageIndex = 0;
                        btnDownload.IsChecked = false;
                        return;
                    }
                }
                else
                {
                    Growl.InfoGlobal("دانلود تصاویر تمام شد");
                    prgStatus.Value = prg.Value = 0;
                    chkTree.IsEnabled = true;
                    client.Dispose();
                    currentIndex = currentImageIndex = 0;
                    btnDownload.IsChecked = false;
                }
            }

            // End of the download
        }
        private void Client_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            //نمایش درصد پیشرفت کار
            double bytesIn = double.Parse(e.BytesReceived.ToString());
            double totalBytes = double.Parse(e.TotalBytesToReceive.ToString());
            double percentage = bytesIn / totalBytes * 100;
            prg.Value = int.Parse(Math.Truncate(percentage).ToString());
        }
        private void Client_DownloadFileCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            // اگر دانلود کنسل شد
            if (e.Cancelled == true)
            {
                client?.CancelAsync();
                Growl.ErrorGlobal("دانلود تصاویر کنسل شد");
                prgStatus.Value = prg.Value = 0;
                chkTree.IsEnabled = true;
                if (File.Exists(TempFileName))
                {
                    File.Delete(TempFileName);
                }
                client.Dispose();
                currentIndex = currentImageIndex = 0;
                txtCurrentImageIndex.Text = txtCurrentBook.Text = txtCurrentIndex.Text = string.Empty;
            }
        }

        #endregion
    }
}
