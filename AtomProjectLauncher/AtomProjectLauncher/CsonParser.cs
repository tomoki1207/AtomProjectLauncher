using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Codeplex.Data;

namespace AtomProjectLauncher
{
    public class CsonParser
    {
        /// <summary>
        /// インデント数を取得する正規表現
        /// </summary>
        private static readonly Regex IndentRegex = new Regex(
            string.Format(@"^({0}+)",
            Properties.Settings.Default.INDENT_TYPE == "tab" ? "\t" : " "));

        /// <summary>
        /// インデント長
        /// </summary>
        private static readonly int INDENT_LENGTH = Properties.Settings.Default.INDENT_LENGTH;

        /// <summary>
        /// CsonファイルをJSONオブジェクトにパースする
        /// </summary>
        /// <param name="filePath">ファイルパス</param>
        /// <returns>JSONオブジェクト</returns>
        /// <exception cref="System.ArgumentException"></exception>
        /// <exception cref="System.ArgumentNullException"></exception>
        /// <exception cref="System.IO.PathTooLongException"></exception>
        /// <exception cref="System.IO.DirectoryNotFoundException"></exception>
        /// <exception cref="System.IO.IOException"></exception>
        /// <exception cref="System.UnauthorizedAccessException"></exception>
        /// <exception cref="System.IO.FileNotFoundException"></exception>
        /// <exception cref="System.NotSupportedException"></exception>
        /// <exception cref="System.Security.SecurityException:"></exception>
        public static dynamic Parse(string filePath)
        {
            return Parse(File.ReadAllLines(filePath));
        }

        /// <summary>
        /// Cson文字列リストをdynamicのJSONオブジェクトにパースする
        /// </summary>
        /// <param name="cson">Cson文字列リスト</param>
        /// <returns>JSONオブジェクト</returns>
        public static dynamic Parse(IEnumerable<string> cson)
        {
            return DynamicJson.Parse(CsonToJson(cson));
        }

        // Cson -> Json
        private static string CsonToJson(IEnumerable<string> cson)
        {
            var csonList = cson.ToList();
            csonList.Add(string.Empty);
            var json = new StringBuilder();

            int aboveIndent = -1;
            bool inArray = false;
            foreach (var line in csonList)
            {
                var m = IndentRegex.Match(line);
                var indent = m.Length / INDENT_LENGTH;


                if (inArray)
                {
                    json.Append(aboveIndent == indent ? "," : string.Empty);
                }
                else
                {
                    json.Append(
                        aboveIndent < indent ? "{"
                            : indent < aboveIndent ? new string('}', aboveIndent - indent) + ","
                            : aboveIndent > 0 ? "," : string.Empty);
                }

                while (true)
                {
                    var target = line.Replace('\'', '\"');
                    var separator = target.IndexOf(':');
                    if(separator < 0)
                    {
                        json.Append(target);
                        break;
                    }

                    var key = target.Substring(0, separator);
                    var val = target.Substring(separator + 1);

                    if (key.Count(s => s == '"') %2 == 1 || val.Count(s => s == '"') % 2 == 1)
                    {
                        json.Append(target);
                        break;
                    }

                    json.Append(@"""").Append(key.Trim().Replace(@"""", string.Empty)).Append(@"""")
                        .Append(":")
                        .Append(val);
                    break;
                }

                aboveIndent = indent;
                if (line.IndexOf('[') >= 0)
                {
                    inArray = true;
                }
                if (line.IndexOf(']') >= 0)
                {
                    inArray = false;
                }
            }

            var jsonString = json.ToString();
            return jsonString.Substring(0, jsonString.Length - 1) + "}";
        }
    }
}
