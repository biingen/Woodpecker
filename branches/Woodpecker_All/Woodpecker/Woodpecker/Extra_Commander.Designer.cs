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
            this.pictureBox_shot = new System.Windows.Forms.PictureBox();
            this.pictureBox_save = new System.Windows.Forms.PictureBox();
            this.label_Command = new System.Windows.Forms.Label();
            this.label_Remark = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_shot)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_save)).BeginInit();
            this.SuspendLayout();
            // 
            // pictureBox_shot
            // 
            this.pictureBox_shot.Location = new System.Drawing.Point(12, 12);
            this.pictureBox_shot.Name = "pictureBox_shot";
            this.pictureBox_shot.Size = new System.Drawing.Size(100, 50);
            this.pictureBox_shot.TabIndex = 0;
            this.pictureBox_shot.TabStop = false;
            // 
            // pictureBox_save
            // 
            this.pictureBox_save.Location = new System.Drawing.Point(172, 12);
            this.pictureBox_save.Name = "pictureBox_save";
            this.pictureBox_save.Size = new System.Drawing.Size(100, 50);
            this.pictureBox_save.TabIndex = 1;
            this.pictureBox_save.TabStop = false;
            // 
            // label_Command
            // 
            this.label_Command.AutoSize = true;
            this.label_Command.Location = new System.Drawing.Point(12, 80);
            this.label_Command.Name = "label_Command";
            this.label_Command.Size = new System.Drawing.Size(82, 12);
            this.label_Command.TabIndex = 2;
            this.label_Command.Text = "label_Command";
            // 
            // label_Remark
            // 
            this.label_Remark.AutoSize = true;
            this.label_Remark.Location = new System.Drawing.Point(170, 80);
            this.label_Remark.Name = "label_Remark";
            this.label_Remark.Size = new System.Drawing.Size(70, 12);
            this.label_Remark.TabIndex = 3;
            this.label_Remark.Text = "label_Remark";
            // 
            // Extra_Commander
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 261);
            this.Controls.Add(this.label_Remark);
            this.Controls.Add(this.label_Command);
            this.Controls.Add(this.pictureBox_save);
            this.Controls.Add(this.pictureBox_shot);
            this.Name = "Extra_Commander";
            this.Text = "Commander";
            this.Load += new System.EventHandler(this.Extra_Commander_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_shot)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_save)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBox_shot;
        private System.Windows.Forms.PictureBox pictureBox_save;
        private System.Windows.Forms.Label label_Command;
        private System.Windows.Forms.Label label_Remark;
    }
}