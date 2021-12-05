using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;

// TreeNodeと実装が重複しているが、継承はできない
// 型引数を使ってもObservableCollectionとIListの暗黙変換ができないため
namespace MithrilCube.Prism
{
    // 実際はこのように使うが、XAMLはジェネリクス型をDataTypeとして参照できないので、TreeSourceを継承したModelクラスを作る。
    // xmlns:dmfm="clr-namespace:DigitalMegaFlareOffline.Modules.Common.Models;"
    //<HierarchicalDataTemplate DataType = "{x:Type dmfm:TreeSource}" ItemsSource="{Binding Children}">
    //    <TextBlock Text = "{Binding Value}" />    // Valueは<T>によって指定を変えること
    //</ HierarchicalDataTemplate >

    // このように空っぽのクラスを作成すればHierarchicalDataTemplateで使用できる。
    //public class FileTree : TreeSource<string>
    //{
    //}

    /// <summary>
    /// WPF&Prism用の木構造データで、TreeViewにバインドできる
    /// </summary>
    public class TreeSource<T> : INotifyPropertyChanged
    {
        private bool _isExpanded = false;
        private T _value;
        private TreeSource<T> _parent = null;
        private ObservableCollection<TreeSource<T>> _children = null;

        public TreeSource(T value)
        {
            _value = value;
        }

        /// <summary>
        /// TreeViewのツリーを開いているか
        /// デフォルトはfalseにする
        /// </summary>
        public bool IsExpanded
        {
            get { return _isExpanded; }
            set { _isExpanded = value; OnPropertyChanged("IsExpanded"); }
        }

        /// <summary>
        /// 値
        /// </summary>
        public T Value
        {
            get { return _value; }
            set { _value = value; OnPropertyChanged("Value"); }
        }

        /// <summary>
        /// 親要素：画面表示には関係しない
        /// </summary>
        public TreeSource<T> Parent
        {
            get { return _parent; }
            set { _parent = value; OnPropertyChanged("Parent"); }
        }

        /// <summary>
        /// 子要素
        /// </summary>
        public ObservableCollection<TreeSource<T>> Children
        {
            get { return _children; }
            set { _children = value; OnPropertyChanged("Childen"); }
        }

        #region INotifyPropertyChangedの実装
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string name)
        {
            if (null == PropertyChanged) return;
            PropertyChanged(this, new PropertyChangedEventArgs(name));
        }
        #endregion

        #region 追加削除などのメソッド
        /// <summary>
        /// 子ノードを追加する。
        /// </summary>
        public virtual TreeSource<T> AddChild(TreeSource<T> child)
        {
            if (null == Children) Children = new ObservableCollection<TreeSource<T>>();
            child.Parent = this;
            Children.Add(child);

            return this;
        }

        /// <summary>
        /// 子ノードを削除する。
        /// </summary>
        /// <param name="child">削除したいノード</param>
        /// <returns>削除後のオブジェクト</returns>
        public virtual TreeSource<T> RemoveChild(TreeSource<T> child)
        {
            Children.Remove(child);
            return this;
        }

        /// <summary>
        /// 子ノードを削除する。
        /// </summary>
        /// <param name="child">削除したいノード</param>
        /// <returns>削除の可否</returns>
        public virtual bool TryRemoveChild(TreeSource<T> child)
        {
            return Children.Remove(child);
        }

        /// <summary>
        /// 子ノードを全て削除する。
        /// </summary>
        /// <returns>子ノードを全削除後のオブジェクト</returns>
        public virtual TreeSource<T> ClearChildren()
        {
            Children.Clear();
            return this;
        }

        /// <summary>
        /// 自身のノードを親ツリーから削除する。
        /// </summary>
        /// <returns>親のオブジェクト</returns>
        public virtual TreeSource<T> RemoveOwn()
        {
            var parent = Parent;
            parent.RemoveChild(this);
            return parent;
        }

        /// <summary>
        /// 自身のノードを親ツリーから削除する。
        /// </summary>
        /// <returns>削除の可否</returns>
        public virtual bool TryRemoveOwn()
        {
            var parent = Parent;
            return parent.TryRemoveChild(this);
        }
        #endregion

        #region Listに変換するメソッド


        /// <summary>
        /// 左方優先で順に辿ってリストにする
        /// </summary>
        /// <param name="parent"></param>
        public void ToList(List<T> parent)
        {
            parent.Add(Value);

            if (Children != null)
            {
                foreach (var item in Children)
                {
                    item.ToList(parent);
                }
            }
        }

        /// <summary>
        /// 底優先探索の順を返す
        /// ツリーを昇る時に追加するので、深さ優先ではない
        /// </summary>
        /// <returns></returns>
        public List<TreeSource<T>> DepthList()
        {
            var result = new List<TreeSource<T>>();

            // 底
            if (Children != null)
            {
                foreach (var item in Children)
                {
                    var list = item.DepthList();
                    result.AddRange(list);
                }
            }

            result.Add(this);

            return result;
        }
        #endregion
    }
}
