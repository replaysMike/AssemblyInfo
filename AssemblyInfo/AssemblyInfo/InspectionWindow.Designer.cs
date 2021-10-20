namespace AssemblyInfo
{
    partial class InspectionWindow
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(InspectionWindow));
            this.txtInspection = new System.Windows.Forms.RichTextBox();
            this.chkWordWrap = new System.Windows.Forms.CheckBox();
            this.btnCopy = new System.Windows.Forms.Button();
            this.SuspendLayout();

            // chkWordWrap
            this.chkWordWrap.Location = new System.Drawing.Point(12, 8);
            this.chkWordWrap.Name = "chkWordWrap";
            this.chkWordWrap.Size = new System.Drawing.Size(150, 16);
            this.chkWordWrap.TabIndex = 0;
            this.chkWordWrap.Text = "Word Wrap";
            this.chkWordWrap.Checked = false;

            // chkWordWrap
            this.btnCopy.Location = new System.Drawing.Point(533 - 90, 5);
            this.btnCopy.Name = "btnCopy";
            this.btnCopy.Size = new System.Drawing.Size(80, 22);
            this.btnCopy.TabIndex = 1;
            this.btnCopy.Text = "&Copy";
            // 
            // txtInspection
            // 
            this.txtInspection.Location = new System.Drawing.Point(12, 28);
            this.txtInspection.Name = "txtInspection";
            this.txtInspection.ReadOnly = true;
            this.txtInspection.Size = new System.Drawing.Size(509, 290);
            this.txtInspection.TabIndex = 2;
            this.txtInspection.Text = "";
            // 
            // InspectionWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(533, 334);
            this.Controls.Add(this.chkWordWrap);
            this.Controls.Add(this.btnCopy);
            this.Controls.Add(this.txtInspection);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "InspectionWindow";
            this.Text = "Inspection";
            this.Resize += new System.EventHandler(this.InspectionWindow_Resize);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.RichTextBox txtInspection;
        private System.Windows.Forms.CheckBox chkWordWrap;
        private System.Windows.Forms.Button btnCopy;
    }
}