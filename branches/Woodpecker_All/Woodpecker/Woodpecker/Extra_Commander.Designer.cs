namespace Woodpecker
{
    partial class Extra_Commander
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
            this.textBox_command = new System.Windows.Forms.TextBox();
            this.button_send = new System.Windows.Forms.Button();
            this.button_schedule = new System.Windows.Forms.Button();
            this.SchOpen = new System.Windows.Forms.OpenFileDialog();
            this.button_setting = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // textBox_command
            // 
            this.textBox_command.Location = new System.Drawing.Point(12, 12);
            this.textBox_command.Name = "textBox_command";
            this.textBox_command.Size = new System.Drawing.Size(259, 25);
            this.textBox_command.TabIndex = 0;
            // 
            // button_send
            // 
            this.button_send.Location = new System.Drawing.Point(13, 53);
            this.button_send.Name = "button_send";
            this.button_send.Size = new System.Drawing.Size(79, 26);
            this.button_send.TabIndex = 1;
            this.button_send.Text = "Send";
            this.button_send.UseVisualStyleBackColor = true;
            this.button_send.Click += new System.EventHandler(this.button_send_Click);
            // 
            // button_schedule
            // 
            this.button_schedule.Location = new System.Drawing.Point(277, 12);
            this.button_schedule.Name = "button_schedule";
            this.button_schedule.Size = new System.Drawing.Size(84, 26);
            this.button_schedule.TabIndex = 2;
            this.button_schedule.Text = "Browser";
            this.button_schedule.UseVisualStyleBackColor = true;
            this.button_schedule.Click += new System.EventHandler(this.button_schedule_Click);
            // 
            // SchOpen
            // 
            this.SchOpen.FileName = "SchOpen";
            // 
            // button_setting
            // 
            this.button_setting.Location = new System.Drawing.Point(277, 53);
            this.button_setting.Name = "button_setting";
            this.button_setting.Size = new System.Drawing.Size(84, 26);
            this.button_setting.TabIndex = 3;
            this.button_setting.Text = "Setting";
            this.button_setting.UseVisualStyleBackColor = true;
            this.button_setting.Click += new System.EventHandler(this.button_setting_Click);
            // 
            // Extra_Commander
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(368, 93);
            this.Controls.Add(this.button_setting);
            this.Controls.Add(this.button_schedule);
            this.Controls.Add(this.button_send);
            this.Controls.Add(this.textBox_command);
            this.Name = "Extra_Commander";
            this.Text = "Extra_Commander";
            this.Load += new System.EventHandler(this.Extra_Commander_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBox_command;
        private System.Windows.Forms.Button button_send;
        private System.Windows.Forms.Button button_schedule;
        private System.Windows.Forms.OpenFileDialog SchOpen;
        private System.Windows.Forms.Button button_setting;
    }
}