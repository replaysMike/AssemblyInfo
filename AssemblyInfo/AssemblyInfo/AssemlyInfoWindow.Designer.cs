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
            this.components = new System.ComponentModel.Container();
            this.listInfo = new System.Windows.Forms.ListView();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.SuspendLayout();
            // 
            // listInfo
            // 
            this.listInfo.FullRowSelect = true;
            this.listInfo.HideSelection = false;
            this.listInfo.Location = new System.Drawing.Point(12, 12);
            this.listInfo.Name = "listInfo";
            this.listInfo.Size = new System.Drawing.Size(406, 380);
            this.listInfo.TabIndex = 0;
            this.listInfo.UseCompatibleStateImageBehavior = false;
            this.listInfo.View = System.Windows.Forms.View.Details;
            // 
            // toolTip1
            // 
            this.toolTip1.AutomaticDelay = 250;
            this.toolTip1.AutoPopDelay = 2500;
            this.toolTip1.InitialDelay = 250;
            this.toolTip1.ReshowDelay = 0;
            this.toolTip1.ShowAlways = true;
            // 
            // AssemlyInfoWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(430, 404);
            this.Controls.Add(this.listInfo);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AssemlyInfoWindow";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Assembly Info";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListView listInfo;
        private System.Windows.Forms.ToolTip toolTip1;
    }
}

