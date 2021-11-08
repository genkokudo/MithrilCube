using System;
using System.Collections.Generic;
using System.Text;

namespace MinteaCore.HtmlToDom
{
    // TODO:前のプロジェクトから引っ張り出してきたけど使うか分からない。C#標準のDirectoryクラスとかにデータ構造があればそっちを使う。

    /// <summary>
    /// ディレクトリとファイルの情報
    /// </summary>
    public class DirectoryData
    {
        /// <summary>
        /// フルパス
        /// </summary>
        public string FullPath { get; set; }

        /// <summary>
        /// 名前
        /// リストボックス選択肢のnameになる
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// ディレクトリならtrue
        /// ファイルならfalse
        /// </summary>
        public bool IsDirectory { get; set; }

        /// <summary>
        /// リストボックスで選択したとき
        /// 下位の要素リストを取得するためのキー
        /// リストボックス選択肢のvalueになる
        /// 
        /// ファイルならFullPathと同じ
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// 所属する辞書のキー
        /// </summary>
        public string DictionaryKey { get; set; }

        public override string ToString()
        {
            return $"FullPath:{FullPath}\nDictionaryKey:{DictionaryKey}\nIsDirectory:{IsDirectory}\nName:{Name}\nValue:{Value}";
        }
    }

    #region privateGetDirectoryFileTree
    ///// <summary>
    ///// 引数以下のディレクトリの階層構造を取得します
    ///// 
    ///// ファイルとフォルダの区別は？
    ///// とりあえずフォルダなら最後にスラッシュつける
    ///// </summary>
    ///// <param name="parent"></param>
    ///// <returns></returns>
    //private static TreeNode<DirectoryData> GetDirectoryFileTree(DirectoryData parent, List<DirectoryData> list)
    //{
    //    var currentDir = new TreeNode<DirectoryData>(parent);

    //    IEnumerable<string> subFiles = Directory.GetFiles(parent.FullPath, "*", SearchOption.TopDirectoryOnly);
    //    IEnumerable<string> subFolders = Directory.GetDirectories(parent.FullPath, "*", SearchOption.TopDirectoryOnly);

    //    // ファイルの登録
    //    foreach (var file in subFiles)
    //    {
    //        var subFile = new DirectoryData
    //        {
    //            FullPath = file,
    //            Name = Path.GetFileName(file),
    //            IsDirectory = false,
    //            Value = file,
    //            DictionaryKey = parent.Value
    //        };
    //        list.Add(subFile);
    //        currentDir.AddChild(new TreeNode<DirectoryData>(subFile));
    //    }

    //    // ディレクトリの登録
    //    foreach (var folder in subFolders)
    //    {
    //        var subFolder = new DirectoryData
    //        {
    //            FullPath = folder,
    //            Name = Path.GetFileName(folder),
    //            IsDirectory = true,
    //            Value = $"{parent.Value}#{Path.GetFileName(folder)}".Trim('#'),
    //            DictionaryKey = parent.Value
    //        };
    //        list.Add(subFolder);
    //        var child = new TreeNode<DirectoryData>(subFolder);

    //        // 更に下の階層のディレクトリ
    //        GetDirectoryFileTree(subFolder, list);

    //        // このディレクトリに追加
    //        currentDir.AddChild(child);
    //    }

    //    return currentDir;
    //}

    //public static Dictionary<string, Dictionary<string, string>> GetDirectoryFileList(string path)
    //{
    //    var list = new List<DirectoryData>();
    //    GetDirectoryFileTree(new DirectoryData
    //    {
    //        FullPath = path,
    //        Name = "root",
    //        IsDirectory = true,
    //        Value = "",
    //        DictionaryKey = ""
    //    }, list);

    //    var result = new Dictionary<string, Dictionary<string, string>>();

    //    foreach (var item in list)
    //    {
    //        result.NewDictionaryIfNotExists(item.DictionaryKey);
    //        result[item.DictionaryKey].Add(item.Name, item.Value);
    //    }

    //    return result;
    //}
    #endregion

}
