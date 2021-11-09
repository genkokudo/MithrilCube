using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;

namespace MithrilCube.Data
{
    /// <summary>
    /// WPF用の木構造データで、TreeViewにバインドできる
    /// 動作を確認したらTreeNodeと混ぜる
    /// </summary>
    public class TreeSource : INotifyPropertyChanged    // TODO:これ必要？
    {
        private bool _IsExpanded = true;
        private string _Text = "";
        private TreeSource _Parent = null;
        private ObservableCollection<TreeSource> _Children = null;


        public bool IsExpanded
        {
            get { return _IsExpanded; }
            set { _IsExpanded = value; OnPropertyChanged("IsExpanded"); }
        }

        public string Text
        {
            get { return _Text; }
            set { _Text = value; OnPropertyChanged("Text"); }
        }

        public TreeSource Parent
        {
            get { return _Parent; }
            set { _Parent = value; OnPropertyChanged("Parent"); }
        }

        public ObservableCollection<TreeSource> Children
        {
            get { return _Children; }
            set { _Children = value; OnPropertyChanged("Childen"); }
        }



        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string name)
        {
            if (null == this.PropertyChanged) return;
            this.PropertyChanged(this, new PropertyChangedEventArgs(name));
        }

        public void Add(TreeSource child)
        {
            if (null == Children) Children = new ObservableCollection<TreeSource>();
            child.Parent = this;
            Children.Add(child);
        }
    }
}
