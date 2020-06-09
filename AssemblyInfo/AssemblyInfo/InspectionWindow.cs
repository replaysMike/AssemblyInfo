using System;
using System.Windows.Forms;

namespace AssemblyInfo
{
    public partial class InspectionWindow : Form
    {
        public InspectionWindow(string title, string inspectionText)
        {
            InitializeComponent();
            Text = $"Inspection - {title}";
            txtInspection.Text = inspectionText;
        }

        private void InspectionWindow_Resize(object sender, EventArgs e)
        {
            txtInspection.Width = Width - 35;
            txtInspection.Height = Height - 60;
        }
    }
}
