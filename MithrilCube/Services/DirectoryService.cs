using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace MithrilCube.Services
{
    /// <summary>
    /// ディレクトリ操作関係でよく使うメソッドをまとめたもの
    /// </summary>
    public interface IDirectoryService
    {
        /// <summary>
        /// 指定したパスにディレクトリが存在しない場合
        /// すべてのディレクトリとサブディレクトリを作成します
        /// </summary>
        /// <param name="directory"></param>
        /// <returns></returns>
        public DirectoryInfo SafeCreateDirectory(string directory);

        /// <summary>
        /// 指定フォルダ以下のすべてのフォルダを探索し、
        /// 指定拡張子のファイル名をリストに順次追加していく
        /// </summary>
        /// <param name="folderPath">探索するフォルダ</param>
        /// <param name="fileFullPathList">ファイル名のフルパスリスト</param>
        /// <param name="extensions">検索する拡張子群{ ".cs", ".exe"}みたいな感じ</param>
        public void FolderInsiteSearch(string folderPath, List<string> fileFullPathList, string[] extensions);

        /// <summary>
        /// 指定したディレクトリとその中身を全て削除する
        /// </summary>
        /// <param name="directory">ディレクトリ</param>
        public void DeleteDirectory(string directory);

        ///// <summary>
        ///// ファイルを対象のディレクトリにコピーします
        ///// ディレクトリが無ければ作成します
        ///// </summary>
        ///// <param name="sourceFile"></param>
        ///// <param name="destDirectory"></param>
        ///// <param name="isOverWrite"></param>
        //public void FileCopyWithCreateDirectory(string sourceFile, string destDirectory, bool isOverWrite);
    }

    /// <summary>
    /// ディレクトリ操作関係でよく使うメソッドをまとめたもの
    /// </summary>
    public class DirectoryService : IDirectoryService
    {
        public DirectoryInfo SafeCreateDirectory(string directory)
        {
            if (!directory.EndsWith("\\") && !directory.EndsWith("/"))
            {
                directory += "/";
            }
            if (Directory.Exists(directory))
            {
                return null;
            }
            return Directory.CreateDirectory(directory);
        }

        public void FolderInsiteSearch(string folderPath, List<string> filenameList, string[] extensions)
        {
            //現在のフォルダ内の指定拡張子のファイル名をリストに追加
            foreach (var fileName in Directory.EnumerateFiles(folderPath))
                foreach (var endId in extensions)
                    if (fileName.EndsWith(endId))
                        filenameList.Add(fileName);
            //現在のフォルダ内のすべてのフォルダパスを取得
            var dirNames = Directory.EnumerateDirectories(folderPath);
            //フォルダがないならば再帰探索終了し、あるなら各フォルダに対して探索実行
            if (dirNames.Count() == 0)
                return;
            else
                foreach (var dirName in dirNames)
                    FolderInsiteSearch(dirName, filenameList, extensions);
        }

        public void DeleteDirectory(string directory)
        {
            // 再帰処理
            DeleteDirectory(directory, true);
        }

        // 外から呼び出すときはisTop=trueとする
        private void DeleteDirectory(string directory, bool isTop)
        {
            if (!Directory.Exists(directory))
            {
                return;
            }

            //ディレクトリ以外の全ファイルを削除
            string[] filePaths = Directory.GetFiles(directory);
            foreach (string filePath in filePaths)
            {
                File.SetAttributes(filePath, FileAttributes.Normal);
                File.Delete(filePath);
            }

            //ディレクトリの中のディレクトリも再帰的に削除
            string[] directoryPaths = Directory.GetDirectories(directory);
            foreach (string directoryPath in directoryPaths)
            {
                DeleteDirectory(directoryPath, false);
            }

            if (!isTop)
            {
                //中が空になったらディレクトリ自身も削除
                Directory.Delete(directory, false);
            }
        }

        ///// <summary>
        ///// 指定したディレクトリの中身を全て削除する
        ///// （更に中身のあるフォルダがある場合の動作を確認してないので没）
        ///// </summary>
        //public void DeleteFiles(string directory)
        //{
        //    DirectoryInfo target = new DirectoryInfo(directory);
        //    foreach (FileInfo file in target.GetFiles())
        //    {
        //        file.Delete();
        //    }
        //}
    }
}
