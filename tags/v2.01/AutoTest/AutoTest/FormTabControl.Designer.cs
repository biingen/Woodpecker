namespace AutoTest
{
    partial class FormTabControl
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormTabControl));
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem3 = new System.Windows.Forms.ToolStripMenuItem();
            this.tabControl = new System.Windows.Forms.TabControl();
            this.MainSettingBtn = new System.Windows.Forms.Button();
            this.ScheduleSettingBtn = new System.Windows.Forms.Button();
            this.ClosePicBox = new System.Windows.Forms.PictureBox();
            this.MailSettingBtn = new System.Windows.Forms.Button();
            this.LogSettingBtn = new System.Windows.Forms.Button();
            this.toolStripMenuItem4 = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ClosePicBox)).BeginInit();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem1,
            this.toolStripMenuItem2,
            this.toolStripMenuItem3,
            this.toolStripMenuItem4});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Padding = new System.Windows.Forms.Padding(7, 2, 0, 2);
            this.menuStrip1.Size = new System.Drawing.Size(625, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            this.menuStrip1.Visible = false;
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.ShowShortcutKeys = false;
            this.toolStripMenuItem1.Size = new System.Drawing.Size(134, 20);
            this.toolStripMenuItem1.Text = "toolStripMenuItem1";
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.ShowShortcutKeys = false;
            this.toolStripMenuItem2.Size = new System.Drawing.Size(134, 20);
            this.toolStripMenuItem2.Text = "toolStripMenuItem2";
            // 
            // toolStripMenuItem3
            // 
            this.toolStripMenuItem3.Name = "toolStripMenuItem3";
            this.toolStripMenuItem3.ShowShortcutKeys = false;
            this.toolStripMenuItem3.Size = new System.Drawing.Size(134, 20);
            this.toolStripMenuItem3.Text = "toolStripMenuItem3";
            // 
            // tabControl
            // 
            this.tabControl.Dock = System.Windows.Forms.DockStyle.Right;
            this.tabControl.ItemSize = new System.Drawing.Size(25, 25);
            this.tabControl.Location = new System.Drawing.Point(170, 0);
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(455, 665);
            this.tabControl.TabIndex = 1;
            // 
            // MainSettingBtn
            // 
            this.MainSettingBtn.Font = new System.Drawing.Font("MS Reference Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.MainSettingBtn.Location = new System.Drawing.Point(-3, 108);
            this.MainSettingBtn.Name = "MainSettingBtn";
            this.MainSettingBtn.Size = new System.Drawing.Size(180, 40);
            this.MainSettingBtn.TabIndex = 2;
            this.MainSettingBtn.Text = "   Main Setting";
            this.MainSettingBtn.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.MainSettingBtn.UseVisualStyleBackColor = true;
            this.MainSettingBtn.Click += new System.EventHandler(this.MainSettingBtn_Click);
            // 
            // ScheduleSettingBtn
            // 
            this.ScheduleSettingBtn.Font = new System.Drawing.Font("MS Reference Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ScheduleSettingBtn.Location = new System.Drawing.Point(-3, 147);
            this.ScheduleSettingBtn.Name = "ScheduleSettingBtn";
            this.ScheduleSettingBtn.Size = new System.Drawing.Size(180, 40);
            this.ScheduleSettingBtn.TabIndex = 3;
            this.ScheduleSettingBtn.Text = "   Multi Schedule";
            this.ScheduleSettingBtn.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.ScheduleSettingBtn.UseVisualStyleBackColor = true;
            this.ScheduleSettingBtn.Click += new System.EventHandler(this.ScheduleBtn_Click);
            // 
            // ClosePicBox
            // 
            this.ClosePicBox.BackColor = System.Drawing.Color.Transparent;
            this.ClosePicBox.Image = global::AutoTest.Properties.Resources.Close;
            this.ClosePicBox.Location = new System.Drawing.Point(4, -2);
            this.ClosePicBox.Name = "ClosePicBox";
            this.ClosePicBox.Size = new System.Drawing.Size(25, 25);
            this.ClosePicBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.ClosePicBox.TabIndex = 61;
            this.ClosePicBox.TabStop = false;
            this.ClosePicBox.Click += new System.EventHandler(this.ClosePicBox_Click);
            // 
            // MailSettingBtn
            // 
            this.MailSettingBtn.Font = new System.Drawing.Font("MS Reference Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.MailSettingBtn.Location = new System.Drawing.Point(-3, 186);
            this.MailSettingBtn.Name = "MailSettingBtn";
            this.MailSettingBtn.Size = new System.Drawing.Size(180, 40);
            this.MailSettingBtn.TabIndex = 62;
            this.MailSettingBtn.Text = "   Mail Setting";
            this.MailSettingBtn.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.MailSettingBtn.UseVisualStyleBackColor = true;
            this.MailSettingBtn.Click += new System.EventHandler(this.MailSettingBtn_Click);
            // 
            // LogSettingBtn
            // 
            this.LogSettingBtn.Font = new System.Drawing.Font("MS Reference Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LogSettingBtn.Location = new System.Drawing.Point(-3, 225);
            this.LogSettingBtn.Name = "LogSettingBtn";
            this.LogSettingBtn.Size = new System.Drawing.Size(180, 40);
            this.LogSettingBtn.TabIndex = 63;
            this.LogSettingBtn.Text = "   Log Setting";
            this.LogSettingBtn.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.LogSettingBtn.UseVisualStyleBackColor = true;
            this.LogSettingBtn.Visible = false;
            this.LogSettingBtn.Click += new System.EventHandler(this.LogSettingBtn_Click);
            // 
            // toolStripMenuItem4
            // 
            this.toolStripMenuItem4.Name = "toolStripMenuItem4";
            this.toolStripMenuItem4.Size = new System.Drawing.Size(134, 20);
            this.toolStripMenuItem4.Text = "toolStripMenuItem4";
            // 
            // FormTabControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.LightGray;
            this.ClientSize = new System.Drawing.Size(625, 665);
            this.Controls.Add(this.tabControl);
            this.Controls.Add(this.MailSettingBtn);
            this.Controls.Add(this.ClosePicBox);
            this.Controls.Add(this.ScheduleSettingBtn);
            this.Controls.Add(this.MainSettingBtn);
            this.Controls.Add(this.menuStrip1);
            this.Controls.Add(this.LogSettingBtn);
            this.Font = new System.Drawing.Font("MS Reference Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "FormTabControl";
            this.Opacity = 0.97D;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Setting";
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.gPanelTitleBack_MouseDown);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ClosePicBox)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem2;
        private System.Windows.Forms.TabControl tabControl;
        private System.Windows.Forms.Button MainSettingBtn;
        private System.Windows.Forms.Button ScheduleSettingBtn;
        private System.Windows.Forms.PictureBox ClosePicBox;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem3;
        private System.Windows.Forms.Button MailSettingBtn;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem4;
        private System.Windows.Forms.Button LogSettingBtn;
    }
}