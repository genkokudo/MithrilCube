using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
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

        // Directory.Deleteの第2引数をtrueにすると同じことができるので削除

        ///// <summary>
        ///// 指定したディレクトリの中身を全て削除する
        ///// </summary>
        ///// <param name="directory">ディレクトリ</param>
        //public void DeleteDirectory(string directory);

        /// <summary>
        /// 名前を指定して、アセンブリに埋め込まれているリソースを出力する
        /// </summary>
        /// <param name="assambly">呼び出し元のアセンブリ情報</param>
        /// <param name="resourceName">AssemblyのGetManifestResourceNamesで取得したリソース名</param>
        /// <param name="basePath">出力先ディレクトリ</param>
        public void CopyResource(Assembly assambly, string resourceName, string basePath = "./");

        /// <summary>
        /// アセンブリに埋め込まれているリソースを全て出力する
        /// フォルダ構成も埋め込んだ時の構成の通りにする
        /// </summary>
        /// <param name="assambly">呼び出し元のアセンブリ情報</param>
        /// <param name="basePath">出力先ディレクトリ</param>
        public void CopyResources(Assembly assambly, string basePath = "./");

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

        //public void DeleteDirectory(string directory)
        //{
        //    // 再帰処理
        //    DeleteDirectorySub(directory, true);
        //}

        //// 外から呼び出すときはisTop=trueとする
        //private void DeleteDirectorySub(string directory, bool isTop)
        //{
        //    if (!Directory.Exists(directory))
        //    {
        //        return;
        //    }

        //    //ディレクトリ以外の全ファイルを削除
        //    string[] filePaths = Directory.GetFiles(directory);
        //    foreach (string filePath in filePaths)
        //    {
        //        File.SetAttributes(filePath, FileAttributes.Normal);
        //        File.Delete(filePath);
        //    }

        //    //ディレクトリの中のディレクトリも再帰的に削除
        //    string[] directoryPaths = Directory.GetDirectories(directory);
        //    foreach (string directoryPath in directoryPaths)
        //    {
        //        DeleteDirectorySub(directoryPath, false);
        //    }

        //    if (!isTop)
        //    {
        //        //中が空になったらディレクトリ自身も削除
        //        Directory.Delete(directory, false);
        //    }
        //}

        public void CopyResources(Assembly assambly, string basePath = "./")
        {
            // アセンブリに埋め込まれているすべてのリソースをコピー
            foreach (var resourceName in assambly.GetManifestResourceNames())
            {
                // これは除外する
                if (resourceName == $"{assambly.GetName().Name}.g.resources")   // "DigitalMegaFlareOffline.Modules.Common.g.resources"
                {
                    continue;
                }

                // アセンブリに埋め込まれているリソースを出力する
                CopyResource(assambly, resourceName, basePath);
            }
        }

        public void CopyResource(Assembly assambly, string resourceName, string basePath = "./")
        {
            // アセンブリに埋め込まれているリソースのStreamを取得する
            using (var resourceStream = assambly.GetManifestResourceStream(resourceName)) // "DigitalMegaFlareOffline.Modules.Common.Sample.Excel.Sample1.xlsx"
            {
                var assamblyLength = assambly.GetName().Name.Length + 1;
                var resourcePath = resourceName.Substring(assamblyLength, resourceName.Length - assamblyLength);        // Sample.Excel.Sample1.xlsx

                // Sample/Excel/Sample1.xlsx にする
                var splited = resourcePath.Split('.');
                var fileName = $"{splited[splited.Length - 2]}.{splited[splited.Length - 1]}";
                var filePath = string.Empty;
                for (int i = 0; i < splited.Length - 2; i++)
                {
                    filePath += splited[i];
                    filePath += "/";
                }

                // ディレクトリを作成する
                SafeCreateDirectory(Path.Combine(basePath, filePath));

                // 出力ストリーム
                var fileFullPath = Path.Combine(basePath, filePath, fileName);
                var outStream = new FileStream(fileFullPath, FileMode.Create);    // ファイルが存在していたら強制上書き

                // リソースの内容を出力ストリームに書きだす
                int b;
                while ((b = resourceStream.ReadByte()) != -1)
                {
                    outStream.WriteByte((byte)b);
                }
                outStream.Close();
            }
        }

    }
}
