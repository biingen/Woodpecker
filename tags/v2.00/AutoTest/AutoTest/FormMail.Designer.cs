namespace AutoTest
{
    partial class FormMail
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
            this.SendMailBtn = new System.Windows.Forms.Button();
            this.ToLabel = new System.Windows.Forms.Label();
            this.FromLabel = new System.Windows.Forms.Label();
            this.ToTextBox = new System.Windows.Forms.TextBox();
            this.FromTextBox = new System.Windows.Forms.TextBox();
            this.ProjectTextBox = new System.Windows.Forms.TextBox();
            this.ProjectLabel = new System.Windows.Forms.Label();
            this.VersionLabel = new System.Windows.Forms.Label();
            this.StartLabel = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.TotalTextBox = new System.Windows.Forms.TextBox();
            this.TesterTextBox = new System.Windows.Forms.TextBox();
            this.VersionTextBox = new System.Windows.Forms.TextBox();
            this.SaveMailBtn = new System.Windows.Forms.Button();
            this.ModelLabel = new System.Windows.Forms.Label();
            this.ModelTextBox = new System.Windows.Forms.TextBox();
            this.Testcase1TextBox = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.Testcase2TextBox = new System.Windows.Forms.TextBox();
            this.Testcase3TextBox = new System.Windows.Forms.TextBox();
            this.Testcase4TextBox = new System.Windows.Forms.TextBox();
            this.Testcase5TextBox = new System.Windows.Forms.TextBox();
            this.SendMailcheckBox = new System.Windows.Forms.CheckBox();
            this.SavedLabel = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // SendMailBtn
            // 
            this.SendMailBtn.Font = new System.Drawing.Font("MS Reference Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.SendMailBtn.Location = new System.Drawing.Point(307, 586);
            this.SendMailBtn.Margin = new System.Windows.Forms.Padding(4);
            this.SendMailBtn.Name = "SendMailBtn";
            this.SendMailBtn.Size = new System.Drawing.Size(36, 32);
            this.SendMailBtn.TabIndex = 0;
            this.SendMailBtn.Text = "Send";
            this.SendMailBtn.UseVisualStyleBackColor = true;
            this.SendMailBtn.Visible = false;
            this.SendMailBtn.Click += new System.EventHandler(this.SendMailBtn_Click);
            // 
            // ToLabel
            // 
            this.ToLabel.AutoSize = true;
            this.ToLabel.Font = new System.Drawing.Font("MS Reference Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ToLabel.Location = new System.Drawing.Point(43, 127);
            this.ToLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.ToLabel.Name = "ToLabel";
            this.ToLabel.Size = new System.Drawing.Size(32, 16);
            this.ToLabel.TabIndex = 1;
            this.ToLabel.Text = "To :";
            // 
            // FromLabel
            // 
            this.FromLabel.AutoSize = true;
            this.FromLabel.Font = new System.Drawing.Font("MS Reference Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FromLabel.Location = new System.Drawing.Point(27, 79);
            this.FromLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.FromLabel.Name = "FromLabel";
            this.FromLabel.Size = new System.Drawing.Size(48, 16);
            this.FromLabel.TabIndex = 2;
            this.FromLabel.Text = "From :";
            // 
            // ToTextBox
            // 
            this.ToTextBox.Font = new System.Drawing.Font("MS Reference Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ToTextBox.Location = new System.Drawing.Point(83, 124);
            this.ToTextBox.Margin = new System.Windows.Forms.Padding(4);
            this.ToTextBox.Name = "ToTextBox";
            this.ToTextBox.Size = new System.Drawing.Size(329, 22);
            this.ToTextBox.TabIndex = 3;
            // 
            // FromTextBox
            // 
            this.FromTextBox.Font = new System.Drawing.Font("MS Reference Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FromTextBox.Location = new System.Drawing.Point(83, 76);
            this.FromTextBox.Margin = new System.Windows.Forms.Padding(4);
            this.FromTextBox.Name = "FromTextBox";
            this.FromTextBox.Size = new System.Drawing.Size(329, 22);
            this.FromTextBox.TabIndex = 4;
            // 
            // ProjectTextBox
            // 
            this.ProjectTextBox.Location = new System.Drawing.Point(141, 349);
            this.ProjectTextBox.Name = "ProjectTextBox";
            this.ProjectTextBox.Size = new System.Drawing.Size(208, 22);
            this.ProjectTextBox.TabIndex = 5;
            // 
            // ProjectLabel
            // 
            this.ProjectLabel.AutoSize = true;
            this.ProjectLabel.Font = new System.Drawing.Font("MS Reference Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ProjectLabel.ForeColor = System.Drawing.Color.DeepSkyBlue;
            this.ProjectLabel.Location = new System.Drawing.Point(27, 352);
            this.ProjectLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.ProjectLabel.Name = "ProjectLabel";
            this.ProjectLabel.Size = new System.Drawing.Size(100, 16);
            this.ProjectLabel.TabIndex = 6;
            this.ProjectLabel.Text = "Project name :";
            // 
            // VersionLabel
            // 
            this.VersionLabel.AutoSize = true;
            this.VersionLabel.Font = new System.Drawing.Font("MS Reference Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.VersionLabel.ForeColor = System.Drawing.Color.DeepSkyBlue;
            this.VersionLabel.Location = new System.Drawing.Point(27, 408);
            this.VersionLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.VersionLabel.Name = "VersionLabel";
            this.VersionLabel.Size = new System.Drawing.Size(64, 16);
            this.VersionLabel.TabIndex = 7;
            this.VersionLabel.Text = "Version :";
            // 
            // StartLabel
            // 
            this.StartLabel.AutoSize = true;
            this.StartLabel.Font = new System.Drawing.Font("MS Reference Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.StartLabel.ForeColor = System.Drawing.Color.DarkOrange;
            this.StartLabel.Location = new System.Drawing.Point(27, 491);
            this.StartLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.StartLabel.Name = "StartLabel";
            this.StartLabel.Size = new System.Drawing.Size(108, 16);
            this.StartLabel.TabIndex = 8;
            this.StartLabel.Text = "Total test time :";
            this.StartLabel.Visible = false;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("MS Reference Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.DarkOrange;
            this.label1.Location = new System.Drawing.Point(27, 463);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(57, 16);
            this.label1.TabIndex = 9;
            this.label1.Text = "Tester :";
            // 
            // TotalTextBox
            // 
            this.TotalTextBox.Location = new System.Drawing.Point(141, 488);
            this.TotalTextBox.Name = "TotalTextBox";
            this.TotalTextBox.Size = new System.Drawing.Size(208, 22);
            this.TotalTextBox.TabIndex = 10;
            this.TotalTextBox.Visible = false;
            // 
            // TesterTextBox
            // 
            this.TesterTextBox.Location = new System.Drawing.Point(141, 460);
            this.TesterTextBox.Name = "TesterTextBox";
            this.TesterTextBox.Size = new System.Drawing.Size(208, 22);
            this.TesterTextBox.TabIndex = 11;
            // 
            // VersionTextBox
            // 
            this.VersionTextBox.Location = new System.Drawing.Point(141, 405);
            this.VersionTextBox.Name = "VersionTextBox";
            this.VersionTextBox.Size = new System.Drawing.Size(208, 22);
            this.VersionTextBox.TabIndex = 12;
            // 
            // SaveMailBtn
            // 
            this.SaveMailBtn.Font = new System.Drawing.Font("MS Reference Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.SaveMailBtn.Location = new System.Drawing.Point(350, 585);
            this.SaveMailBtn.Name = "SaveMailBtn";
            this.SaveMailBtn.Size = new System.Drawing.Size(85, 30);
            this.SaveMailBtn.TabIndex = 65;
            this.SaveMailBtn.Text = "SAVE";
            this.SaveMailBtn.UseVisualStyleBackColor = true;
            this.SaveMailBtn.Click += new System.EventHandler(this.SaveSchBtn_Click);
            // 
            // ModelLabel
            // 
            this.ModelLabel.AutoSize = true;
            this.ModelLabel.Font = new System.Drawing.Font("MS Reference Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ModelLabel.ForeColor = System.Drawing.Color.DeepSkyBlue;
            this.ModelLabel.Location = new System.Drawing.Point(27, 380);
            this.ModelLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.ModelLabel.Name = "ModelLabel";
            this.ModelLabel.Size = new System.Drawing.Size(93, 16);
            this.ModelLabel.TabIndex = 67;
            this.ModelLabel.Text = "Model name :";
            // 
            // ModelTextBox
            // 
            this.ModelTextBox.Location = new System.Drawing.Point(141, 377);
            this.ModelTextBox.Name = "ModelTextBox";
            this.ModelTextBox.Size = new System.Drawing.Size(208, 22);
            this.ModelTextBox.TabIndex = 66;
            // 
            // Testcase1TextBox
            // 
            this.Testcase1TextBox.Location = new System.Drawing.Point(141, 182);
            this.Testcase1TextBox.Name = "Testcase1TextBox";
            this.Testcase1TextBox.Size = new System.Drawing.Size(208, 22);
            this.Testcase1TextBox.TabIndex = 69;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("MS Reference Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.ForeColor = System.Drawing.Color.Crimson;
            this.label2.Location = new System.Drawing.Point(27, 185);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(77, 16);
            this.label2.TabIndex = 68;
            this.label2.Text = "Test case :";
            // 
            // Testcase2TextBox
            // 
            this.Testcase2TextBox.Location = new System.Drawing.Point(141, 210);
            this.Testcase2TextBox.Name = "Testcase2TextBox";
            this.Testcase2TextBox.Size = new System.Drawing.Size(208, 22);
            this.Testcase2TextBox.TabIndex = 70;
            // 
            // Testcase3TextBox
            // 
            this.Testcase3TextBox.Location = new System.Drawing.Point(141, 238);
            this.Testcase3TextBox.Name = "Testcase3TextBox";
            this.Testcase3TextBox.Size = new System.Drawing.Size(208, 22);
            this.Testcase3TextBox.TabIndex = 71;
            // 
            // Testcase4TextBox
            // 
            this.Testcase4TextBox.Location = new System.Drawing.Point(141, 266);
            this.Testcase4TextBox.Name = "Testcase4TextBox";
            this.Testcase4TextBox.Size = new System.Drawing.Size(208, 22);
            this.Testcase4TextBox.TabIndex = 72;
            // 
            // Testcase5TextBox
            // 
            this.Testcase5TextBox.Location = new System.Drawing.Point(141, 294);
            this.Testcase5TextBox.Name = "Testcase5TextBox";
            this.Testcase5TextBox.Size = new System.Drawing.Size(208, 22);
            this.Testcase5TextBox.TabIndex = 73;
            // 
            // SendMailcheckBox
            // 
            this.SendMailcheckBox.AutoSize = true;
            this.SendMailcheckBox.Location = new System.Drawing.Point(30, 28);
            this.SendMailcheckBox.Name = "SendMailcheckBox";
            this.SendMailcheckBox.Size = new System.Drawing.Size(105, 20);
            this.SendMailcheckBox.TabIndex = 74;
            this.SendMailcheckBox.Text = "Mail function";
            this.SendMailcheckBox.UseVisualStyleBackColor = true;
            this.SendMailcheckBox.CheckedChanged += new System.EventHandler(this.SendMailcheckBox_CheckedChanged);
            // 
            // SavedLabel
            // 
            this.SavedLabel.AutoSize = true;
            this.SavedLabel.Font = new System.Drawing.Font("MS Reference Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.SavedLabel.ForeColor = System.Drawing.Color.Red;
            this.SavedLabel.Location = new System.Drawing.Point(10, 586);
            this.SavedLabel.Name = "SavedLabel";
            this.SavedLabel.Size = new System.Drawing.Size(0, 24);
            this.SavedLabel.TabIndex = 75;
            // 
            // FormMail
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(447, 631);
            this.Controls.Add(this.SavedLabel);
            this.Controls.Add(this.SendMailcheckBox);
            this.Controls.Add(this.Testcase5TextBox);
            this.Controls.Add(this.Testcase4TextBox);
            this.Controls.Add(this.Testcase3TextBox);
            this.Controls.Add(this.Testcase2TextBox);
            this.Controls.Add(this.Testcase1TextBox);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.ModelLabel);
            this.Controls.Add(this.ModelTextBox);
            this.Controls.Add(this.SaveMailBtn);
            this.Controls.Add(this.VersionTextBox);
            this.Controls.Add(this.TesterTextBox);
            this.Controls.Add(this.TotalTextBox);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.StartLabel);
            this.Controls.Add(this.VersionLabel);
            this.Controls.Add(this.ProjectLabel);
            this.Controls.Add(this.ProjectTextBox);
            this.Controls.Add(this.FromTextBox);
            this.Controls.Add(this.ToTextBox);
            this.Controls.Add(this.FromLabel);
            this.Controls.Add(this.ToLabel);
            this.Controls.Add(this.SendMailBtn);
            this.Font = new System.Drawing.Font("MS Reference Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "FormMail";
            this.Text = "Form2";
            this.Load += new System.EventHandler(this.FormMail_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label ToLabel;
        private System.Windows.Forms.Label FromLabel;
        private System.Windows.Forms.TextBox ToTextBox;
        private System.Windows.Forms.TextBox FromTextBox;
        public System.Windows.Forms.Button SendMailBtn;
        private System.Windows.Forms.TextBox ProjectTextBox;
        private System.Windows.Forms.Label ProjectLabel;
        private System.Windows.Forms.Label VersionLabel;
        private System.Windows.Forms.Label StartLabel;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox TotalTextBox;
        private System.Windows.Forms.TextBox TesterTextBox;
        private System.Windows.Forms.TextBox VersionTextBox;
        private System.Windows.Forms.Button SaveMailBtn;
        private System.Windows.Forms.Label ModelLabel;
        private System.Windows.Forms.TextBox ModelTextBox;
        private System.Windows.Forms.TextBox Testcase1TextBox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox Testcase2TextBox;
        private System.Windows.Forms.TextBox Testcase3TextBox;
        private System.Windows.Forms.TextBox Testcase4TextBox;
        private System.Windows.Forms.TextBox Testcase5TextBox;
        private System.Windows.Forms.CheckBox SendMailcheckBox;
        private System.Windows.Forms.Label SavedLabel;
    }
}