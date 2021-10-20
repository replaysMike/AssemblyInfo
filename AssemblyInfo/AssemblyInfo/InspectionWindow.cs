using System;
using System.Windows.Forms;

namespace AssemblyInfo
{
    public partial class InspectionWindow : Form
    {
        private const bool DefaultWordWrap = false;

        public InspectionWindow(string title, string inspectionText)
        {
            InitializeComponent();
            Text = $"Inspection - {title}";
            txtInspection.Text = inspectionText;
            chkWordWrap.Checked = txtInspection.WordWrap = DefaultWordWrap;
            chkWordWrap.CheckedChanged += ChkWordWrap_CheckedChanged;
            btnCopy.Click += BtnCopy_Click;
            ResizeWindow();
        }

        private void BtnCopy_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(txtInspection.Text);
        }

        private void ChkWordWrap_CheckedChanged(object sender, EventArgs e)
        {
            txtInspection.WordWrap = chkWordWrap.Checked;
        }

        private void InspectionWindow_Resize(object sender, EventArgs e)
        {
            ResizeWindow();
        }

        private void ResizeWindow()
        {
            if (txtInspection != null)
            {
                txtInspection.Width = Width - 40;
                txtInspection.Height = Height - 80;
            }
        }
    }
}
