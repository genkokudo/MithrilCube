using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace MithrilCube.Data
{
    /// <summary>
    /// 簡易ツリー構造のジェネリッククラス
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class TreeNode<T>
    {
        protected IList<TreeNode<T>> _children = null;
        protected T _value;
        protected TreeNode<T> _parent = null;

        /// <summary>
        /// データ実体
        /// </summary>
        public virtual T Value { get { return _value; } set { _value = value; } }

        /// <summary>
        /// 親への参照プロパティ
        /// </summary>
        public virtual TreeNode<T> Parent { get { return _parent; } set { _parent = value; } }

        /// <summary>
        /// 子ノードのリストプロパティ
        /// nullの場合は新しくリストを作成するので、nullは返ってこない
        /// </summary>
        public virtual IList<TreeNode<T>> Children
        {
            get
            {
                if (_children == null)
                    _children = new List<TreeNode<T>>();
                return _children;
            }
            set { _children = value; }
        }

        public TreeNode(T data)
        {
            Value = data;
        }

        #region 追加削除メソッド
        /// <summary>
        /// 子ノードを追加する。
        /// nullの場合新しく子ノードを作成する
        /// </summary>
        /// <param name="child">追加したいノード</param>
        /// <returns>追加後のオブジェクト</returns>
        public virtual TreeNode<T> AddChild(TreeNode<T> child)
        {
            Children.Add(child);
            child.Parent = this;

            return this;
        }

        /// <summary>
        /// 子ノードを削除する。
        /// </summary>
        /// <param name="child">削除したいノード</param>
        /// <returns>削除後のオブジェクト</returns>
        public virtual TreeNode<T> RemoveChild(TreeNode<T> child)
        {
            Children.Remove(child);
            return this;
        }

        /// <summary>
        /// 子ノードを削除する。
        /// </summary>
        /// <param name="child">削除したいノード</param>
        /// <returns>削除の可否</returns>
        public virtual bool TryRemoveChild(TreeNode<T> child)
        {
            return Children.Remove(child);
        }

        /// <summary>
        /// 子ノードを全て削除する。
        /// </summary>
        /// <returns>子ノードを全削除後のオブジェクト</returns>
        public virtual TreeNode<T> ClearChildren()
        {
            Children.Clear();
            return this;
        }

        /// <summary>
        /// 自身のノードを親ツリーから削除する。
        /// </summary>
        /// <returns>親のオブジェクト</returns>
        public virtual TreeNode<T> RemoveOwn()
        {
            return Parent.RemoveChild(this);
        }

        /// <summary>
        /// 自身のノードを親ツリーから削除する。
        /// </summary>
        /// <returns>削除の可否</returns>
        public virtual bool TryRemoveOwn()
        {
            return Parent.TryRemoveChild(this);
        }
        #endregion

        #region リストに変換するメソッド
        /// <summary>
        /// 左方優先で順に辿ってリストにする
        /// </summary>
        /// <param name="parent"></param>
        public void ToList(List<T> parent)
        {
            parent.Add(Value);

            if (_children != null)
            {
                foreach (var item in _children)
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
        public List<TreeNode<T>> DepthList()
        {
            var result = new List<TreeNode<T>>();

            // 底
            if (_children != null)
            {
                foreach (var item in _children)
                {
                    var list = item.DepthList();
                    result.AddRange(list);
                }
            }

            result.Add(this);

            return result;
        }
        #endregion

        #region その他便利メソッド
        /// <summary>
        /// それぞれのデータの親が分かっている場合
        /// 木構造データを作成します
        /// 子が持つ親は1つまで
        /// もう木構造データ作るの大変だからこういう補助メソッド作り込もうね
        /// </summary>
        /// <param name="root">根</param>
        /// <param name="dictionary">データリスト</param>
        /// <param name="parentList">Key名に対する親Key名を格納したデータ</param>
        /// <returns></returns>
        public static TreeNode<T> MakeTree(TreeNode<T> root, Dictionary<string, T> dictionary, Dictionary<string, string> parentList)
        {
            // ノードを一通り作る
            var nodeList = new Dictionary<string, TreeNode<T>>();
            foreach (var item in dictionary)
            {
                // 登録
                var node = new TreeNode<T>(item.Value);
                nodeList.Add(item.Key, node);
            }

            foreach (var node in nodeList)
            {
                if (parentList.ContainsKey(node.Key))
                {
                    // Parentがあればその親に登録
                    nodeList[parentList[node.Key]].AddChild(node.Value);
                }
                else
                {
                    // Parentがなければroot直下に追加
                    root.AddChild(node.Value);
                }
            }

            return root;
        }
        #endregion
    }

}
