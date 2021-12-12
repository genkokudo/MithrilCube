using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace MithrilCubeWpf.Prism.Services
{
    /// <summary>
    /// WPF用
    /// ディレクトリ関係の処理
    /// </summary>
    public interface IWpfDirectoryService
    {
        // ※[+]をクリックする度に都度取得するという考えもあるけど、今回は一括で取得。
        /// <summary>
        /// ディレクトリ構造をツリー形式で取得する
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public FileTree GetDirectoryFileTree(string path);
    }

    /// <summary>
    /// WPF用
    /// ディレクトリ関係の処理
    /// </summary>
    public class WpfDirectoryService : IWpfDirectoryService
    {
        public FileTree GetDirectoryFileTree(string path)
        {
            var root = new FileTree(new FileData { FullPath = path, IsDirectory = true, Name = Path.GetFileName(path) });  // rootを作る

            // 再帰で子要素を取得
            GetDirectoryFileTree(root);

            return root;
        }

        /// <summary>
        /// 再帰的にディレクトリ以下の階層構造を取得します
        /// </summary>
        private void GetDirectoryFileTree(TreeSource<FileData> parent)
        {
            var currentDirPath = parent.Value.FullPath;

            // ファイルとディレクトリを取得
            IEnumerable<string> subFiles = Directory.GetFiles(currentDirPath, "*", SearchOption.TopDirectoryOnly);
            IEnumerable<string> subDirectories = Directory.GetDirectories(currentDirPath, "*", SearchOption.TopDirectoryOnly);

            // ファイルの登録
            foreach (var file in subFiles)
            {
                var subFile = new FileData
                {
                    FullPath = file,
                    Name = Path.GetFileName(file),
                    IsDirectory = false
                };
                parent.AddChild(new TreeSource<FileData>(subFile));
            }

            // ディレクトリの登録
            foreach (var folder in subDirectories)
            {
                var subFolder = new FileData
                {
                    FullPath = folder,
                    Name = Path.GetFileName(folder),
                    IsDirectory = true
                };
                var child = new TreeSource<FileData>(subFolder);

                // 更に下の階層のディレクトリを登録していく
                GetDirectoryFileTree(child);                    // ※ここにif文を付ければ、このフォルダだけ取得するといったメソッドが作れるはず

                // このディレクトリに追加
                parent.AddChild(child);
            }
        }
    }
}
