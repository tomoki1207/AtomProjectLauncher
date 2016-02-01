using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace AtomProjectLauncher
{
    public partial class AtomProjectLauncher : Form
    {
        private Image backgroundImage;

        public AtomProjectLauncher()
        {
            InitializeComponent();
            this.backgroundImage = Properties.Resources._128;
            this.LoadProjectsJson();
        }

        // 位置を調整するため手動で描画させる
        protected override void OnPaintBackground(PaintEventArgs e)
        {
            base.OnPaintBackground(e);
            var rc = new Rectangle(this.ClientSize.Width - backgroundImage.Width * 4 / 5,
                this.ClientSize.Height - backgroundImage.Height * 4 / 5,
                backgroundImage.Width, backgroundImage.Height);
            e.Graphics.DrawImage(backgroundImage, rc);
        }

        // projects.csonからプロジェクト情報を読み込む
        private void LoadProjectsJson()
        {
            var csonPath = !string.IsNullOrEmpty(Properties.Settings.Default.PROJECT_CSON_DIRECTORY) ? Properties.Settings.Default.PROJECT_CSON_DIRECTORY : @"C:\Users\%USERNAME%\.atom";
            var csonFile = Path.Combine(csonPath, "projects.cson");

            var projects = CsonParser.Parse(Environment.ExpandEnvironmentVariables(csonFile));

            var combo = new List<ProjectComboBoxItem>();
            foreach (var project in projects)
            {
                var paths = new List<string>();
                foreach (var path in project.Value.paths)
                {
                    paths.Add(path);
                }

                combo.Add(new ProjectComboBoxItem
                {
                    ProjectTitle = project.Value.title,
                    ProjectPath = paths
                });
            }

            this.comboProjectName.DataSource = combo;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Exit();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            var project = this.comboProjectName.SelectedItem as ProjectComboBoxItem;
            
            try
            {
                var process = new ProcessStartInfo();
                process.FileName = "cmd.exe";

                // project.cson内で適切にエスケープされているはずなのでノーチェック
                process.Arguments = "/c atom " + string.Join(" ", (project.ProjectPath));

                Process.Start(process);
                this.Exit();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // フォームの終了
        private void Exit()
        {
            this.Dispose();
            this.Close();
        }

    }
}
