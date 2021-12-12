using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

// ディレクトリを指定して、そのディレクトリとファイルの情報をツリー形式で取得したい
namespace MithrilCubeWpf.Prism
{
    // ツリー表示データ
    // XAMLはジェネリクス型が扱えないため、このように型を固定したクラスに再定義することが必要
    public class FileTree : TreeSource<FileData>
    {
        public FileTree(FileData value) : base(value)
        {
        }
    }

    /// <summary>
    /// ディレクトリとファイルの情報
    /// </summary>
    public class FileData
    {
        /// <summary>
        /// フルパス
        /// </summary>
        public string FullPath { get; set; }

        /// <summary>
        /// 表示名
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// ディレクトリならtrue
        /// ファイルならfalse
        /// </summary>
        public bool IsDirectory { get; set; }
    }
}
