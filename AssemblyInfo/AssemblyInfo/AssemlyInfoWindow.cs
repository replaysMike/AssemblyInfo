using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace AssemblyInfo
{
    public partial class AssemlyInfoWindow : Form
    {
        private ContextMenu _contextMenu;
        private AssemblyData _assemblyData;
        public Options Options { get; }


        public AssemlyInfoWindow(Options options, AssemblyInspector inspector)
        {
            Options = options;
            InitializeComponent();
            BuildUIComponents();
            var filename = Options.Filename;
            if (string.IsNullOrEmpty(filename))
            {
                var result = openFileDialog1.ShowDialog(this);
                if (result == DialogResult.OK)
                {
                    filename = openFileDialog1.FileName;
                }
                else
                {
                    // close the application
                    Load += (s, e) => Close();
                    return;
                }
            }
            if (InspectAssembly(inspector, filename))
            {
                UpdateUI(_assemblyData);
            }
        }

        private bool InspectAssembly(AssemblyInspector inspector, string filename)
        {
            try
            {
                _assemblyData = inspector.Inspect(filename);
                if (_assemblyData == null)
                {
                    MessageBox.Show($"File '{Path.GetFileName(filename)}' is not a valid assembly.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Load += (s, e) => Close();
                    return false;
                }
            }
            catch (FileNotFoundException)
            {
                MessageBox.Show($"File '{Path.GetFileName(filename)}' not found!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Load += (s, e) => Close();
                return false;
            }
            return true;
        }

        private void BuildUIComponents()
        {
            _contextMenu = new ContextMenu();
            _contextMenu.MenuItems.Add(new MenuItem("Copy", MenuItemCopy_Click, Shortcut.CtrlC));
            listInfo.ContextMenu = _contextMenu;
            var columnHeaders = new ColumnHeader[] {
                new ColumnHeader { Name = "Property", Text = "", Width = 110 },
                new ColumnHeader { Name = "Value", Text = "", Width = listInfo.Width - 120 },
            };
            listInfo.Columns.AddRange(columnHeaders);

            var items = new ListViewItem[]
            {
                new ListViewItem(new string[] { "Name", "" }),
                new ListViewItem(new string[] { "ProductName", "" }),
                new ListViewItem(new string[] { "FileDescription", "" }),
                new ListViewItem(new string[] { "Description", "" }),
                new ListViewItem(new string[] { "Company", "" }),
                new ListViewItem(new string[] { "Version", "" }),
                new ListViewItem(new string[] { "FileVersion", "" }),
                new ListViewItem(new string[] { "ProductVersion", "" }),
                new ListViewItem(new string[] { "IsStronglyNamed", "" }),
                new ListViewItem(new string[] { "PublicKeyToken", "" }),
                new ListViewItem(new string[] { "Framework", "" }),
                new ListViewItem(new string[] { "FrameworkVersion", "" }),
                new ListViewItem(new string[] { "Copyright", "" }),
                new ListViewItem(new string[] { "Build", "" }),
                new ListViewItem(new string[] { "DebuggableModes", "" }),
                new ListViewItem(new string[] { "IsDebug", "" }),
                new ListViewItem(new string[] { "IsPatched", "" }),
                new ListViewItem(new string[] { "IsPreRelease", "" }),
                new ListViewItem(new string[] { "Language", "" }),
                new ListViewItem(new string[] { "OriginalFilename", "" }),
                new ListViewItem(new string[] { "FileSize", "" }),
                new ListViewItem(new string[] { "SHA256", "" }),
                new ListViewItem(new string[] { "SHA", "" }),
                new ListViewItem(new string[] { "MD5", "" }),
                new ListViewItem(new string[] { "Path", "" }),
                new ListViewItem(new string[] { "IsClsCompliant", "" }),
                new ListViewItem(new string[] { "InformationalVersion", "" }),
                new ListViewItem(new string[] { "Metadata", "" }),
            };
            listInfo.Items.AddRange(items);
        }

        private void MenuItemCopy_Click(object sender, EventArgs e)
        {
            var menuItem = sender as MenuItem;
            var parent = menuItem.Parent as ContextMenu;
            var control = parent.SourceControl as ListView;
            var itemList = new List<ListViewItem>();
            foreach (ListViewItem item in control.SelectedItems)
                itemList.Add(item);
            Clipboard.SetText(string.Join(Environment.NewLine, itemList.Select(x => $"{x.SubItems[0].Text}={x.SubItems[1].Text}")));
        }

        private void UpdateUI(AssemblyData assemblyData)
        {
            Text = $"Assembly Info - {assemblyData.Filename}";
            listInfo.Items[0].SubItems[1].Text = assemblyData.Name;
            listInfo.Items[1].SubItems[1].Text = assemblyData.ProductName;
            listInfo.Items[2].SubItems[1].Text = assemblyData.FileDescription;
            listInfo.Items[3].SubItems[1].Text = assemblyData.Description;
            listInfo.Items[4].SubItems[1].Text = assemblyData.Company;
            listInfo.Items[5].SubItems[1].Text = assemblyData.Version;
            listInfo.Items[6].SubItems[1].Text = assemblyData.FileVersion;
            listInfo.Items[7].SubItems[1].Text = assemblyData.ProductVersion;
            listInfo.Items[8].SubItems[1].Text = $"{assemblyData.IsStronglyNamed}";
            listInfo.Items[9].SubItems[1].Text = assemblyData.PublicKeyToken;
            listInfo.Items[10].SubItems[1].Text = assemblyData.Framework;
            listInfo.Items[11].SubItems[1].Text = assemblyData.FrameworkVersion;
            listInfo.Items[12].SubItems[1].Text = assemblyData.Copyright;
            listInfo.Items[13].SubItems[1].Text = assemblyData.Build;
            listInfo.Items[14].SubItems[1].Text = assemblyData.DebuggableModes;
            listInfo.Items[15].SubItems[1].Text = $"{assemblyData.IsDebug}";
            listInfo.Items[16].SubItems[1].Text = $"{assemblyData.IsPatched}";
            listInfo.Items[17].SubItems[1].Text = $"{assemblyData.IsPreRelease}";
            listInfo.Items[18].SubItems[1].Text = assemblyData.Language;
            listInfo.Items[19].SubItems[1].Text = assemblyData.OriginalFilename;
            listInfo.Items[20].SubItems[1].Text = assemblyData.FileSize;
            listInfo.Items[21].SubItems[1].Text = assemblyData.Sha256;
            listInfo.Items[22].SubItems[1].Text = assemblyData.Sha;
            listInfo.Items[23].SubItems[1].Text = assemblyData.Md5;
            listInfo.Items[24].SubItems[1].Text = Path.GetDirectoryName(assemblyData.FullPath);
            listInfo.Items[25].SubItems[1].Text = $"{assemblyData.IsClsCompliant}";
            listInfo.Items[26].SubItems[1].Text = assemblyData.InformationalVersion;
            listInfo.Items[27].SubItems[1].Text = assemblyData.Metadata;
        }

        private void AssemlyInfoWindow_Resize(object sender, EventArgs e)
        {
            listInfo.Width = Width - 40;
            listInfo.Height = Height - 60;
            listInfo.Columns[1].Width = listInfo.Width - 120;
        }
    }
}
