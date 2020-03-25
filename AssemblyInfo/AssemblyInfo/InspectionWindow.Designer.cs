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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(InspectionWindow));
            this.txtInspection = new System.Windows.Forms.RichTextBox();
            this.SuspendLayout();
            // 
            // txtInspection
            // 
            this.txtInspection.Location = new System.Drawing.Point(12, 12);
            this.txtInspection.Name = "txtInspection";
            this.txtInspection.ReadOnly = true;
            this.txtInspection.Size = new System.Drawing.Size(509, 310);
            this.txtInspection.TabIndex = 0;
            this.txtInspection.Text = "";
            // 
            // InspectionWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(533, 334);
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
    }
}