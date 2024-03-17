using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using 卓汇数据追溯系统.Models;

namespace 卓汇数据追溯系统
{
    /// <summary>
    /// SearchTextBox.xaml 的交互逻辑
    /// </summary>
    public partial class SearchTextBox : UserControl
    {
        public SearchTextBox()
        {
            InitializeComponent();
            model = new ComboViewModel();
            searchModel = new SearchModel();
            this.DataContext = this;

        }
        public SearchModel searchModel { set; get; }
        public ComboViewModel model { set; get; }
        public Action<SearchModel, string> ComparedData{set;get;}
        public Action ItemCheckChanged { get =>model.ItemCheckChanged; set { model.ItemCheckChanged = value; } }
        public void AddItem(string itemText, object data = null)
        {
            model.AddItem(itemText, data);
        }
        public void Clear()
        {
            
            model.Clear();
        }
        public void SetItemChecked(string itemText, bool isChecked)
        {
            model.SetItemChecked(itemText, isChecked);
        }
        public List<string> SelectedItemTexts
        {
            get
            {
                List<string> ret = new List<string>();
                foreach (var item in model.Items)
                {
                    if (item.ItemChecked)
                        ret.Add(item.ItemText);
                }
                return ret;
            }
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
           var a= model.Items.Where(t => t.ItemText != (string)((sender as CheckBox).Content));
            foreach (var item in a)
            {
                item.ItemChecked = false;
            }
            ComparedData?.Invoke(searchModel, (string)((sender as CheckBox).Content));
        }
    }
    public class ComboViewModel : ViewModelBase
    {
      
        public Action ItemCheckChanged { get; set; }
        public void AddItem(string itemText, object data = null)
        {
            if (_Items == null) return;
            _Items.Add(new CheckItem(itemText, data));
        }
        public void Clear()
        {
            if (_Items == null) return;
            _Items.Clear();
        }
        public void SetItemChecked(string itemText, bool isChecked)
        {
            if (_Items == null) return;
            foreach (var item in _Items)
            {
                if (item.ItemText == itemText)
                    item.ItemChecked = isChecked;
            }
        }
        public DelegateCommand ClearChecked
        {
            get
            {
                return new DelegateCommand((obj) =>
                {
                    if (_Items == null) return;
                    foreach (var item in _Items)
                        item.ItemChecked = false;
                });
            }
        }
        private ObservableCollection<CheckItem> _Items;
        public ObservableCollection<CheckItem> Items
        {
            get
            {
                if (_Items == null)
                {
                    _Items = new ObservableCollection<CheckItem>();
                    _Items.CollectionChanged += Items_CollectionChanged;
                }
                return _Items;
            }
        }

        private void Items_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.OldItems != null)
            {
                foreach (CheckItem item in e.OldItems)
                {
                    item.PropertyChanged -= ItemPropertyChanged;
                }
            }

            if (e.NewItems != null)
            {
                foreach (CheckItem item in e.NewItems)
                {
                    item.PropertyChanged += ItemPropertyChanged;
                }
            }
        }
        private void ItemPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "ItemChecked")
            {
                CheckItem item = sender as CheckItem;

                if (item != null)
                {
                    IEnumerable<CheckItem> checkedItems = Items.Where(b => b.ItemChecked == true);

                    StringBuilder builder = new StringBuilder();

                    foreach (CheckItem i in checkedItems)
                    {
                        builder.Append(i.ItemText);
                    }

                    _selectedText = builder == null ? string.Empty : builder.ToString();
                    RaisePropertyChanged("SelectedText");

                    ItemCheckChanged?.Invoke();
                }
            }
        }
        private string _selectedText;

       

        public string SelectedText
        {
            get => _selectedText;
            set { }
        }
    }
    public class ViewModelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
    public class DelegateCommand : ICommand
    {
        public DelegateCommand(Action<object> action)
        {
            _action = action;
        }
        public event EventHandler CanExecuteChanged;
        private Action<object> _action;
        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            _action?.Invoke(parameter);
        }
    }
    public class CheckItem : ViewModelBase
    {
        public CheckItem(string text, object data = null)
        {
            ItemText = text;
            Data = data;
        }
        public string ItemText { get; private set; }
        private bool _checked;
        public bool ItemChecked
        {
            get => _checked;
            set
            {
                _checked = value;
                RaisePropertyChanged();
                RaisePropertyChanged("Color");
            }
        }
        public string Color
        {
            get
            {
                return ItemChecked ? "Red" : "Black";
            }
        }
        public object Data { get; set; } =null;
    }
}
