namespace TSWTimetableConverter
{
    partial class Form1
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
            this.Convert_B = new System.Windows.Forms.Button();
            this.ToConvert_TB = new System.Windows.Forms.TextBox();
            this.Converted_TB = new System.Windows.Forms.TextBox();
            this.ConvertFile_B = new System.Windows.Forms.Button();
            this.FilePath_TB = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // Convert_B
            // 
            this.Convert_B.Location = new System.Drawing.Point(12, 12);
            this.Convert_B.Name = "Convert_B";
            this.Convert_B.Size = new System.Drawing.Size(75, 23);
            this.Convert_B.TabIndex = 0;
            this.Convert_B.Text = "Convert";
            this.Convert_B.UseVisualStyleBackColor = true;
            this.Convert_B.Click += new System.EventHandler(this.Convert_B_Click);
            // 
            // ToConvert_TB
            // 
            this.ToConvert_TB.Location = new System.Drawing.Point(95, 12);
            this.ToConvert_TB.MaxLength = 32767000;
            this.ToConvert_TB.Multiline = true;
            this.ToConvert_TB.Name = "ToConvert_TB";
            this.ToConvert_TB.Size = new System.Drawing.Size(684, 213);
            this.ToConvert_TB.TabIndex = 1;
            this.ToConvert_TB.Text = "Copy Unofficial Timetable Here";
            // 
            // Converted_TB
            // 
            this.Converted_TB.Location = new System.Drawing.Point(95, 231);
            this.Converted_TB.MaxLength = 32767000;
            this.Converted_TB.Multiline = true;
            this.Converted_TB.Name = "Converted_TB";
            this.Converted_TB.Size = new System.Drawing.Size(684, 399);
            this.Converted_TB.TabIndex = 2;
            // 
            // ConvertFile_B
            // 
            this.ConvertFile_B.Location = new System.Drawing.Point(12, 633);
            this.ConvertFile_B.Name = "ConvertFile_B";
            this.ConvertFile_B.Size = new System.Drawing.Size(75, 23);
            this.ConvertFile_B.TabIndex = 3;
            this.ConvertFile_B.Text = "ConvertFile";
            this.ConvertFile_B.UseVisualStyleBackColor = true;
            this.ConvertFile_B.Click += new System.EventHandler(this.ConvertFile_B_Click);
            // 
            // FilePath_TB
            // 
            this.FilePath_TB.Location = new System.Drawing.Point(95, 636);
            this.FilePath_TB.Name = "FilePath_TB";
            this.FilePath_TB.Size = new System.Drawing.Size(684, 20);
            this.FilePath_TB.TabIndex = 4;
            this.FilePath_TB.Text = "Path to Folder";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(784, 664);
            this.Controls.Add(this.FilePath_TB);
            this.Controls.Add(this.ConvertFile_B);
            this.Controls.Add(this.Converted_TB);
            this.Controls.Add(this.ToConvert_TB);
            this.Controls.Add(this.Convert_B);
            this.Name = "Form1";
            this.Text = "TSW Timetable Converter";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button Convert_B;
        private System.Windows.Forms.TextBox ToConvert_TB;
        private System.Windows.Forms.TextBox Converted_TB;
        private System.Windows.Forms.Button ConvertFile_B;
        private System.Windows.Forms.TextBox FilePath_TB;
    }
}

