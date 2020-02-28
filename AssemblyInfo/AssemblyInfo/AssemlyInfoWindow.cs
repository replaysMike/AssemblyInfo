using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Versioning;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Linq;
using System.Security.Cryptography;
using AssemblyInfo.Extensions;

namespace AssemblyInfo
{
    public partial class AssemlyInfoWindow : Form
    {
        private ContextMenu _contextMenu;
        private bool _toolTipVisible = false;
        public Options Options { get; }


        public AssemlyInfoWindow(Options options)
        {
            Options = options;
            InitializeComponent();
            BuildDetailsList();
            InspectAssembly(options.Filename);
        }

        private void BuildDetailsList()
        {
            _contextMenu = new ContextMenu();
            _contextMenu.MenuItems.Add(new MenuItem("Copy", MenuItemCopy_Click, Shortcut.CtrlC));
            listInfo.ContextMenu = _contextMenu;
            listInfo.ItemMouseHover += ListInfo_ItemMouseHover;
            //listInfo.MouseMove += ListInfo_MouseMove;
            toolTip1.Popup += ToolTip1_Popup;
            toolTip1.SetToolTip(listInfo, "Test");
            var columnHeaders = new ColumnHeader[] {
                new ColumnHeader { Name = "Property", Text = "", Width = 100 },
                new ColumnHeader { Name = "Value", Text = "", Width = listInfo.Width - 105 },
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
                new ListViewItem(new string[] { "PublicKeyToken", "" }),
                new ListViewItem(new string[] { "Framework", "" }),
                new ListViewItem(new string[] { "Copyright", "" }),
                new ListViewItem(new string[] { "Build", "" }),
                new ListViewItem(new string[] { "IsDebug", "" }),
                new ListViewItem(new string[] { "IsPatched", "" }),
                new ListViewItem(new string[] { "IsPreRelease", "" }),
                new ListViewItem(new string[] { "Language", "" }),
                new ListViewItem(new string[] { "OriginalFilename", "" }),
                new ListViewItem(new string[] { "FileSize", "" }),
                new ListViewItem(new string[] { "SHA", "" }),
                new ListViewItem(new string[] { "MD5", "" }),
            };
            listInfo.Items.AddRange(items);
        }

        private void ListInfo_ItemMouseHover(object sender, ListViewItemMouseHoverEventArgs e)
        {
            var item = e.Item;
            if (item != null && e.Item.SubItems.Count >= 2 && !string.IsNullOrEmpty(item.SubItems[1].Text))
            {
                Debug.WriteLine($"{item.SubItems[1].Text}");
                toolTip1.ToolTipTitle = item.SubItems[0].Text;
                toolTip1.SetToolTip(listInfo, item.SubItems[1].Text);
            }
        }

        private void ToolTip1_Popup(object sender, PopupEventArgs e)
        {
            _toolTipVisible = true;
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

        private void ListInfo_MouseMove(object sender, MouseEventArgs e)
        {
        }

        private void InspectAssembly(string filename)
        {
            var assembly = Assembly.LoadFrom(filename);
            var assemblyName = assembly.GetName();
            var version = assemblyName.Version;
            var fileVersion = FileVersionInfo.GetVersionInfo(assembly.Location);
            var targetFramework = assembly.CustomAttributes.FirstOrDefault(x => x.AttributeType == typeof(TargetFrameworkAttribute));
            var assemblyConfiguration = assembly.CustomAttributes.FirstOrDefault(x => x.AttributeType == typeof(AssemblyConfigurationAttribute));
            var fileInfo = new FileInfo(filename);

            listInfo.Items[0].SubItems[1].Text = $"{assembly.FullName}";
            listInfo.Items[1].SubItems[1].Text = $"{fileVersion.ProductName}";
            listInfo.Items[2].SubItems[1].Text = $"{fileVersion.FileDescription}";
            listInfo.Items[3].SubItems[1].Text = $"{fileVersion.Comments}";
            listInfo.Items[4].SubItems[1].Text = $"{fileVersion.CompanyName}";
            listInfo.Items[5].SubItems[1].Text = $"{version.ToString()}";
            listInfo.Items[6].SubItems[1].Text = $"{fileVersion.FileVersion}";
            listInfo.Items[7].SubItems[1].Text = $"{fileVersion.ProductVersion}";
            listInfo.Items[8].SubItems[1].Text = $"{string.Join("", assemblyName.GetPublicKeyToken().Select(b => b.ToString("x2")))}";
            listInfo.Items[9].SubItems[1].Text = targetFramework.ConstructorArguments?.FirstOrDefault().Value.ToString();
            listInfo.Items[10].SubItems[1].Text = $"{fileVersion.LegalCopyright}";
            listInfo.Items[11].SubItems[1].Text = $"{assemblyConfiguration.ConstructorArguments?.FirstOrDefault().Value.ToString()}";
            listInfo.Items[12].SubItems[1].Text = $"{fileVersion.IsDebug}";
            listInfo.Items[13].SubItems[1].Text = $"{fileVersion.IsPatched}";
            listInfo.Items[14].SubItems[1].Text = $"{fileVersion.IsPreRelease}";
            listInfo.Items[15].SubItems[1].Text = $"{fileVersion.Language}";
            listInfo.Items[16].SubItems[1].Text = $"{fileVersion.OriginalFilename}";
            listInfo.Items[17].SubItems[1].Text = $"{Util.BytesToString(fileInfo.Length)}";

            ComputeHashAsync(filename).ConfigureAwait(false);
        }

        private async Task ComputeHashAsync(string filename)
        {
            var bytes = File.ReadAllBytes(filename);
            using (var sha = SHA256.Create())
            {
                var shaHash = sha.ComputeHash(bytes);
                listInfo.Items[18].SubItems[1].Text = AssemblyInfo.Extensions.ByteConverter.ToHex(shaHash);
            }
            using (var md5 = MD5.Create())
            {
                var md5Hash = md5.ComputeHash(bytes);
                listInfo.Items[19].SubItems[1].Text = AssemblyInfo.Extensions.ByteConverter.ToHex(md5Hash);
            }
        }
    }
}
