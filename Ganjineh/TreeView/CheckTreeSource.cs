using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Ganjineh
{
    public class CheckTreeSource : INotifyPropertyChanged
    {
        private string _CTag = "";
        private bool? _IsChecked = false;
        private string _Text = "";
        private CheckTreeSource _Parent = null;
        private ObservableCollection<CheckTreeSource> _Children = null;

        public bool? IsChecked
        {
            get => _IsChecked;
            set { _IsChecked = value; OnPropertyChanged("IsChecked"); }
        }

        public string Text
        {
            get => _Text;
            set { _Text = value; OnPropertyChanged("Text"); }
        }

        public string CTag
        {
            get => _CTag;
            set { _CTag = value; OnPropertyChanged("CTag"); }
        }

        public CheckTreeSource Parent
        {
            get => _Parent;
            set { _Parent = value; OnPropertyChanged("Parent"); }
        }

        public ObservableCollection<CheckTreeSource> Children
        {
            get => _Children;
            set { _Children = value; OnPropertyChanged("Childen"); }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string name)
        {
            if (null == PropertyChanged)
            {
                return;
            }

            PropertyChanged(this, new PropertyChangedEventArgs(name));
        }

        public void Add(CheckTreeSource child)
        {
            if (null == Children)
            {
                Children = new ObservableCollection<CheckTreeSource>();
            }

            child.Parent = this;
            Children.Add(child);
        }

        public void UpdateParentStatus()
        {
            if (null != Parent)
            {
                int isCheckedNull = 0;
                int isCheckedOn = 0;
                int isCheckedOff = 0;
                if (null != Parent.Children)
                {
                    foreach (CheckTreeSource item in Parent.Children)
                    {
                        if (null == item.IsChecked)
                        {
                            isCheckedNull += 1;
                        }

                        if (true == item.IsChecked)
                        {
                            isCheckedOn += 1;
                        }

                        if (false == item.IsChecked)
                        {
                            isCheckedOff += 1;
                        }
                    }
                }
                if ((0 < isCheckedNull) || (0 < isCheckedOn) || (0 < isCheckedOff))
                {
                    if (0 < isCheckedNull)
                    {
                        Parent.IsChecked = null;
                    }
                    else if ((0 < isCheckedOn) && (0 < isCheckedOff))
                    {
                        Parent.IsChecked = null;
                    }
                    else if (0 < isCheckedOn)
                    {
                        Parent.IsChecked = true;
                    }
                    else
                    {
                        Parent.IsChecked = false;
                    }
                }
                Parent.UpdateParentStatus();
            }
        }

        public void UpdateChildStatus()
        {
            if (null != IsChecked)
            {
                if (null != Children)
                {
                    foreach (CheckTreeSource item in Children)
                    {
                        item.IsChecked = IsChecked;
                        item.UpdateChildStatus();
                    }
                }
            }
        }
    }
}
