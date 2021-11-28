using ClosedXML.Excel;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace MithrilCube.Services
{
    /// <summary>
    /// Excel関係の処理
    /// </summary>
    public interface IExcelService
    {
        public string GetCell(string filePath, string sheetName = null, string cellAddress = null);
    }

    public class ExcelService : IExcelService
    {
        public string GetCell(string filePath, string sheetName = null, string cellAddress = null)
        {
            using var stream = new FileStream(filePath, FileMode.Open);
            using var wb = new XLWorkbook(stream);
            var address = string.IsNullOrWhiteSpace(cellAddress) ? "A1" : cellAddress;

            // シート名の指定がある場合
            if (!string.IsNullOrWhiteSpace(sheetName))
            {
                // 指定したシートを取得する
                if (wb.TryGetWorksheet(sheetName, out IXLWorksheet ws))
                {
                    return ws.Cell(address).Value.ToString();
                }
            }

            // シート名の指定が無い場合、最初のシートから取得する
            foreach (var ws in wb.Worksheets)
            {
                return ws.Cell(address).Value.ToString();
            }

            return null;
        }

        // RazorHelperからパクったので、重複しないよう注意

        /// <summary>
        /// Excelファイルを読み込み、シート名をキーとした辞書にする
        /// xlsxのみ対応
        /// </summary>
        /// <param name="directry">ディレクトリ</param>
        /// <param name="filename">拡張子付きのファイル名</param>
        /// <param name="isRequiredTitle">1行目に何もない列を無視する</param>
        /// <returns>シート名をキーとした辞書、行と列の2次元string</returns>
        public static Dictionary<string, List<List<string>>> ReadExcel(Stream stream, bool isRequiredTitle = false)
        {
            // ファイルの読み込み
            var xlsx = new Dictionary<string, List<List<string>>>();
            using (var wb = new XLWorkbook(stream))
            {
                foreach (var ws in wb.Worksheets)
                {
                    // ワークシート
                    List<List<string>> sheet = new List<List<string>>();
                    // TODO:何も書いてないシートがあると落ちる
                    for (int i = 1; i <= ws.LastCellUsed().Address.RowNumber; i++)
                    {
                        List<string> raw = new List<string>();
                        for (int j = 1; j <= ws.LastCellUsed().Address.ColumnNumber; j++)
                        {
                            // 1行目に何もない列を無視する
                            if (!isRequiredTitle || !string.IsNullOrWhiteSpace(ws.Cell(1, j).Value.ToString()))
                            {
                                raw.Add(ws.Cell(i, j).Value.ToString());
                            }
                        }
                        sheet.Add(raw);
                    }

                    // シート名と一緒に登録
                    xlsx.Add(ws.Name, sheet);
                }
            }

            return xlsx;
        }

        /// <summary>
        /// Excelを読み込む
        /// </summary>
        /// <param name="filePath">ファイルパス</param>
        /// <returns></returns>
        private Dictionary<string, List<List<string>>> ReadExcel(string filePath, bool isRequiredTitle = false)
        {
            using var stream = new FileStream(filePath, FileMode.Open);
            return ReadExcel(stream, isRequiredTitle);
        }
    }
}
