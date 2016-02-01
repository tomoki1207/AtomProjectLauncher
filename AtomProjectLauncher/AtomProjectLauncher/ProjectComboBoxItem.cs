
using System.Collections.Generic;

namespace AtomProjectLauncher
{
    public class ProjectComboBoxItem
    {
        /// <summary>
        /// プロジェクト名
        /// </summary>
        public string ProjectTitle { get; set; }

        /// <summary>
        /// プロジェクトフォルダ
        /// </summary>
        public IList<string> ProjectPath { get; set; }

        // コンボボックスで表示される文字列
        public override string ToString()
        {
            return ProjectTitle ?? string.Empty;
        }
    }
}
