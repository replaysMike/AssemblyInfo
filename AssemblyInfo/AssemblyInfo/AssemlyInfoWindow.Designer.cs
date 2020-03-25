namespace AssemblyInfo
{
    partial class AssemlyInfoWindow
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AssemlyInfoWindow));
            this.listInfo = new System.Windows.Forms.ListView();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.SuspendLayout();
            // 
            // listInfo
            // 
            this.listInfo.FullRowSelect = true;
            this.listInfo.HideSelection = false;
            this.listInfo.Location = new System.Drawing.Point(12, 12);
            this.listInfo.Name = "listInfo";
            this.listInfo.ShowItemToolTips = true;
            this.listInfo.Size = new System.Drawing.Size(432, 507);
            this.listInfo.TabIndex = 0;
            this.listInfo.UseCompatibleStateImageBehavior = false;
            this.listInfo.View = System.Windows.Forms.View.Details;
            this.listInfo.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.listInfo_MouseDoubleClick);
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.DefaultExt = "*.dll;*.exe;*.msi";
            this.openFileDialog1.Filter = "Assemblies (*.dll;*.exe;*.msi;*.com)|*.dll;*.exe;*.msi;*.com|All files (*.*)|*.*";
            this.openFileDialog1.Title = "Choose assembly";
            // 
            // AssemlyInfoWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(456, 531);
            this.Controls.Add(this.listInfo);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AssemlyInfoWindow";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Assembly Info";
            this.Resize += new System.EventHandler(this.AssemlyInfoWindow_Resize);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListView listInfo;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
    }
}

