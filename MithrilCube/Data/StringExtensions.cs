using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace MithrilCube.Data
{
    // TODO:ちゃんと拡張の形になってないので直すこと
    public class StringExtensions
    {
        #region ReplaceSpaces:複数スペースを1つのスペースにする
        /// <summary>
        /// 複数スペースを1つのスペースにする
        /// </summary>
        /// <param name="text">対象テキスト</param>
        /// <returns>複数スペースを1つのスペースにしたテキスト</returns>
        public static string ReplaceSpaces(string text)
        {
            var pattern = @"\s\s+";
            var regex = new Regex(pattern);
            return regex.Replace(text, " ");
        }
        #endregion
    }
}
