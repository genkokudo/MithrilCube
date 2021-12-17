using ClosedXML.Excel;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace MithrilCube.Services
{
    /// <summary>
    /// Excel関係の処理
    /// </summary>
    public interface IExcelService
    {
        public string GetCell(string filePath, string sheetName = null, string cellAddress = null);

        /// <summary>
        /// Excelファイルを読み込み、シート名をキーとした辞書にする
        /// xlsxのみ対応
        /// </summary>
        /// <param name="filePath">ファイルパス</param>
        /// <param name="isRequiredTitle">1行目に何もない列を無視する</param>
        /// <returns>シート名をキーとした辞書、行と列の2次元string、ファイルが存在しなければnull</returns>
        public Dictionary<string, List<List<string>>> ReadExcel(string filePath, bool isRequiredTitle = false);

        /// <summary>
        /// Excelファイル作成
        /// </summary>
        /// <param name="filePath">ファイルパス</param>
        /// <returns></returns>
        public XLWorkbook CreateExcelFile(string filePath);

        /// <summary>
        /// シート内の指定した列の番号を取得する
        /// </summary>
        /// <param name="sheet"></param>
        /// <returns>なかったら-1</returns>
        public int GetIndex(List<List<string>> sheet, string name);
    }

    public class ExcelService : IExcelService
    {
        public string GetCell(string filePath, string sheetName = null, string cellAddress = null)
        {
            if (!File.Exists(filePath))
            {
                return null;
            }
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

        public Dictionary<string, List<List<string>>> ReadExcel(string filePath, bool isRequiredTitle = false)
        {
            if (!File.Exists(filePath))
            {
                return null;
            }
            // ファイルの読み込み
            var xlsx = new Dictionary<string, List<List<string>>>();
            using (var wb = new XLWorkbook(filePath))
            {
                foreach (var ws in wb.Worksheets)
                {
                    // ワークシート
                    List<List<string>> sheet = new List<List<string>>();
                    if (ws.LastCellUsed() != null)
                    {
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
                    }

                    // シート名と一緒に登録
                    xlsx.Add(ws.Name, sheet);
                }
            }

            return xlsx;
        }

        public XLWorkbook CreateExcelFile(string filePath)
        {
            // ブック作成
            var wb = new XLWorkbook();
            // シート作成
            wb.AddWorksheet("Sheet1");
            // 保存
            wb.SaveAs(filePath);
            return wb;
        }

        public int GetIndex(List<List<string>> sheet, string name)
        {
            var result = -1;

            if (sheet.Count > 2)
            {
                for (int i = 0; i < sheet[0].Count; i++)
                {
                    if (sheet[0][i] == name)
                    {
                        return i;
                    }
                }
            }
            return result;
        }

    }
}
