namespace AutoTest
{
    partial class FormMonkeyTest
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
            this.buttonRunAll = new System.Windows.Forms.Button();
            this.buttonLoadApps = new System.Windows.Forms.Button();
            this.comboxQcProName = new System.Windows.Forms.ComboBox();
            this.SaveBtn = new System.Windows.Forms.Button();
            this.checkBoxBasic = new System.Windows.Forms.CheckBox();
            this.checkBox2 = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // buttonRunAll
            // 
            this.buttonRunAll.Location = new System.Drawing.Point(50, 185);
            this.buttonRunAll.Name = "buttonRunAll";
            this.buttonRunAll.Size = new System.Drawing.Size(238, 37);
            this.buttonRunAll.TabIndex = 0;
            this.buttonRunAll.Text = "Monkey Test (All)";
            this.buttonRunAll.UseVisualStyleBackColor = true;
            this.buttonRunAll.Click += new System.EventHandler(this.buttonRunAll_Click);
            // 
            // buttonLoadApps
            // 
            this.buttonLoadApps.Location = new System.Drawing.Point(50, 244);
            this.buttonLoadApps.Name = "buttonLoadApps";
            this.buttonLoadApps.Size = new System.Drawing.Size(111, 34);
            this.buttonLoadApps.TabIndex = 1;
            this.buttonLoadApps.Text = "Load Apps";
            this.buttonLoadApps.UseVisualStyleBackColor = true;
            this.buttonLoadApps.Click += new System.EventHandler(this.buttonLoadApps_Click);
            // 
            // comboxQcProName
            // 
            this.comboxQcProName.FormattingEnabled = true;
            this.comboxQcProName.Location = new System.Drawing.Point(50, 284);
            this.comboxQcProName.Name = "comboxQcProName";
            this.comboxQcProName.Size = new System.Drawing.Size(287, 25);
            this.comboxQcProName.TabIndex = 2;
            // 
            // SaveBtn
            // 
            this.SaveBtn.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.SaveBtn.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.SaveBtn.Location = new System.Drawing.Point(350, 590);
            this.SaveBtn.Name = "SaveBtn";
            this.SaveBtn.Size = new System.Drawing.Size(85, 30);
            this.SaveBtn.TabIndex = 32;
            this.SaveBtn.Text = "SAVE";
            this.SaveBtn.UseVisualStyleBackColor = true;
            this.SaveBtn.Click += new System.EventHandler(this.SaveBtn_Click);
            // 
            // checkBoxBasic
            // 
            this.checkBoxBasic.AutoSize = true;
            this.checkBoxBasic.Location = new System.Drawing.Point(50, 104);
            this.checkBoxBasic.Name = "checkBoxBasic";
            this.checkBoxBasic.Size = new System.Drawing.Size(169, 21);
            this.checkBoxBasic.TabIndex = 33;
            this.checkBoxBasic.Text = "Basic Mokey Test Mode";
            this.checkBoxBasic.UseVisualStyleBackColor = true;
            // 
            // checkBox2
            // 
            this.checkBox2.AutoSize = true;
            this.checkBox2.Location = new System.Drawing.Point(50, 142);
            this.checkBox2.Name = "checkBox2";
            this.checkBox2.Size = new System.Drawing.Size(246, 21);
            this.checkBox2.TabIndex = 35;
            this.checkBox2.Text = "Select one of the specified categories";
            this.checkBox2.UseVisualStyleBackColor = true;
            // 
            // FormMonkeyTest
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.AutoScroll = true;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(455, 665);
            this.Controls.Add(this.checkBox2);
            this.Controls.Add(this.checkBoxBasic);
            this.Controls.Add(this.SaveBtn);
            this.Controls.Add(this.comboxQcProName);
            this.Controls.Add(this.buttonLoadApps);
            this.Controls.Add(this.buttonRunAll);
            this.Font = new System.Drawing.Font("微軟正黑體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "FormMonkeyTest";
            this.Text = "Form2";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonRunAll;
        private System.Windows.Forms.Button buttonLoadApps;
        private System.Windows.Forms.ComboBox comboxQcProName;
        private System.Windows.Forms.Button SaveBtn;
        private System.Windows.Forms.CheckBox checkBoxBasic;
        private System.Windows.Forms.CheckBox checkBox2;
    }
}