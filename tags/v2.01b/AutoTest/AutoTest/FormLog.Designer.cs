namespace AutoTest
{
    partial class FormLog
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
            this.Logsearch = new System.Windows.Forms.CheckBox();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.Camerarecord = new System.Windows.Forms.CheckBox();
            this.Sendmail = new System.Windows.Forms.CheckBox();
            this.savelog = new System.Windows.Forms.CheckBox();
            this.Showmessage = new System.Windows.Forms.CheckBox();
            this.Stoptest = new System.Windows.Forms.CheckBox();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // Logsearch
            // 
            this.Logsearch.AutoSize = true;
            this.Logsearch.Location = new System.Drawing.Point(12, 12);
            this.Logsearch.Name = "Logsearch";
            this.Logsearch.Size = new System.Drawing.Size(150, 20);
            this.Logsearch.TabIndex = 0;
            this.Logsearch.Text = "Log search function";
            this.Logsearch.UseVisualStyleBackColor = true;
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(36, 48);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(198, 22);
            this.textBox1.TabIndex = 1;
            // 
            // Camerarecord
            // 
            this.Camerarecord.AutoSize = true;
            this.Camerarecord.Location = new System.Drawing.Point(12, 341);
            this.Camerarecord.Name = "Camerarecord";
            this.Camerarecord.Size = new System.Drawing.Size(120, 20);
            this.Camerarecord.TabIndex = 7;
            this.Camerarecord.Text = "Camera record";
            this.Camerarecord.UseVisualStyleBackColor = true;
            // 
            // Sendmail
            // 
            this.Sendmail.AutoSize = true;
            this.Sendmail.Location = new System.Drawing.Point(149, 341);
            this.Sendmail.Name = "Sendmail";
            this.Sendmail.Size = new System.Drawing.Size(88, 20);
            this.Sendmail.TabIndex = 8;
            this.Sendmail.Text = "Send mail";
            this.Sendmail.UseVisualStyleBackColor = true;
            // 
            // savelog
            // 
            this.savelog.AutoSize = true;
            this.savelog.Location = new System.Drawing.Point(12, 393);
            this.savelog.Name = "savelog";
            this.savelog.Size = new System.Drawing.Size(81, 20);
            this.savelog.TabIndex = 9;
            this.savelog.Text = "Save log";
            this.savelog.UseVisualStyleBackColor = true;
            // 
            // Showmessage
            // 
            this.Showmessage.AutoSize = true;
            this.Showmessage.Location = new System.Drawing.Point(149, 367);
            this.Showmessage.Name = "Showmessage";
            this.Showmessage.Size = new System.Drawing.Size(123, 20);
            this.Showmessage.TabIndex = 10;
            this.Showmessage.Text = "Show message";
            this.Showmessage.UseVisualStyleBackColor = true;
            // 
            // Stoptest
            // 
            this.Stoptest.AutoSize = true;
            this.Stoptest.Location = new System.Drawing.Point(149, 393);
            this.Stoptest.Name = "Stoptest";
            this.Stoptest.Size = new System.Drawing.Size(85, 20);
            this.Stoptest.TabIndex = 11;
            this.Stoptest.Text = "Stop test";
            this.Stoptest.UseVisualStyleBackColor = true;
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Location = new System.Drawing.Point(12, 367);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(108, 20);
            this.checkBox1.TabIndex = 12;
            this.checkBox1.Text = "Camera shot";
            this.checkBox1.UseVisualStyleBackColor = true;
            // 
            // FormLog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(447, 631);
            this.Controls.Add(this.checkBox1);
            this.Controls.Add(this.Stoptest);
            this.Controls.Add(this.Showmessage);
            this.Controls.Add(this.savelog);
            this.Controls.Add(this.Sendmail);
            this.Controls.Add(this.Camerarecord);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.Logsearch);
            this.Font = new System.Drawing.Font("MS Reference Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "FormLog";
            this.Text = "FormLog";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox Logsearch;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.CheckBox Camerarecord;
        private System.Windows.Forms.CheckBox Sendmail;
        private System.Windows.Forms.CheckBox savelog;
        private System.Windows.Forms.CheckBox Showmessage;
        private System.Windows.Forms.CheckBox Stoptest;
        private System.Windows.Forms.CheckBox checkBox1;
    }
}