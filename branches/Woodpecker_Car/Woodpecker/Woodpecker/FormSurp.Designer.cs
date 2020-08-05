namespace Woodpecker
{
    partial class FormSurp
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
            this.ImportDB = new System.Windows.Forms.CheckBox();
            this.CloseBtn = new System.Windows.Forms.Button();
            this.IPtextBox = new System.Windows.Forms.TextBox();
            this.PorttextBox = new System.Windows.Forms.TextBox();
            this.pictureHW = new System.Windows.Forms.PictureBox();
            this.button_python = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.pictureHW)).BeginInit();
            this.SuspendLayout();
            // 
            // ImportDB
            // 
            this.ImportDB.AutoSize = true;
            this.ImportDB.Location = new System.Drawing.Point(11, 624);
            this.ImportDB.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.ImportDB.Name = "ImportDB";
            this.ImportDB.Size = new System.Drawing.Size(119, 16);
            this.ImportDB.TabIndex = 58;
            this.ImportDB.Text = "Import Data analysis";
            this.ImportDB.UseVisualStyleBackColor = true;
            this.ImportDB.CheckedChanged += new System.EventHandler(this.ImportDB_CheckedChanged);
            // 
            // CloseBtn
            // 
            this.CloseBtn.Location = new System.Drawing.Point(380, 624);
            this.CloseBtn.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.CloseBtn.Name = "CloseBtn";
            this.CloseBtn.Size = new System.Drawing.Size(80, 25);
            this.CloseBtn.TabIndex = 59;
            this.CloseBtn.Text = "Close";
            this.CloseBtn.UseVisualStyleBackColor = true;
            this.CloseBtn.Click += new System.EventHandler(this.CloseBtn_Click);
            // 
            // IPtextBox
            // 
            this.IPtextBox.Location = new System.Drawing.Point(11, 587);
            this.IPtextBox.Margin = new System.Windows.Forms.Padding(2);
            this.IPtextBox.Name = "IPtextBox";
            this.IPtextBox.Size = new System.Drawing.Size(106, 22);
            this.IPtextBox.TabIndex = 60;
            this.IPtextBox.Text = "192.168.X.X";
            // 
            // PorttextBox
            // 
            this.PorttextBox.Location = new System.Drawing.Point(134, 587);
            this.PorttextBox.Margin = new System.Windows.Forms.Padding(2);
            this.PorttextBox.Name = "PorttextBox";
            this.PorttextBox.Size = new System.Drawing.Size(71, 22);
            this.PorttextBox.TabIndex = 61;
            this.PorttextBox.Text = "8000";
            // 
            // pictureHW
            // 
            this.pictureHW.Image = global::Woodpecker.Properties.Resources.Surprise;
            this.pictureHW.Location = new System.Drawing.Point(14, 14);
            this.pictureHW.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.pictureHW.Name = "pictureHW";
            this.pictureHW.Size = new System.Drawing.Size(477, 480);
            this.pictureHW.TabIndex = 0;
            this.pictureHW.TabStop = false;
            // 
            // button_python
            // 
            this.button_python.Location = new System.Drawing.Point(378, 536);
            this.button_python.Name = "button_python";
            this.button_python.Size = new System.Drawing.Size(81, 23);
            this.button_python.TabIndex = 62;
            this.button_python.Text = "python";
            this.button_python.UseVisualStyleBackColor = true;
            this.button_python.Click += new System.EventHandler(this.button_python_Click);
            // 
            // FormSurp
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(506, 668);
            this.Controls.Add(this.button_python);
            this.Controls.Add(this.PorttextBox);
            this.Controls.Add(this.IPtextBox);
            this.Controls.Add(this.CloseBtn);
            this.Controls.Add(this.ImportDB);
            this.Controls.Add(this.pictureHW);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Name = "FormSurp";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "FormSurp";
            this.Load += new System.EventHandler(this.Setting_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureHW)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureHW;
        private System.Windows.Forms.CheckBox ImportDB;
        private System.Windows.Forms.Button CloseBtn;
        private System.Windows.Forms.TextBox IPtextBox;
        private System.Windows.Forms.TextBox PorttextBox;
        private System.Windows.Forms.Button button_python;
    }
}