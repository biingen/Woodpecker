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
            this.pictureBox_shot.Location = new System.Drawing.Point(16, 15);
            this.pictureBox_shot.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.pictureBox_shot.Name = "pictureBox_shot";
            this.pictureBox_shot.Size = new System.Drawing.Size(133, 62);
            this.pictureBox_shot.TabIndex = 0;
            this.pictureBox_shot.TabStop = false;
            // 
            // pictureBox_save
            // 
            this.pictureBox_save.Location = new System.Drawing.Point(229, 15);
            this.pictureBox_save.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.pictureBox_save.Name = "pictureBox_save";
            this.pictureBox_save.Size = new System.Drawing.Size(133, 62);
            this.pictureBox_save.TabIndex = 1;
            this.pictureBox_save.TabStop = false;
            // 
            // label_Command
            // 
            this.label_Command.AutoSize = true;
            this.label_Command.Location = new System.Drawing.Point(16, 100);
            this.label_Command.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label_Command.Name = "label_Command";
            this.label_Command.Size = new System.Drawing.Size(99, 15);
            this.label_Command.TabIndex = 2;
            this.label_Command.Text = "label_Command";
            // 
            // label_Remark
            // 
            this.label_Remark.AutoSize = true;
            this.label_Remark.Location = new System.Drawing.Point(227, 100);
            this.label_Remark.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label_Remark.Name = "label_Remark";
            this.label_Remark.Size = new System.Drawing.Size(85, 15);
            this.label_Remark.TabIndex = 3;
            this.label_Remark.Text = "label_Remark";
            // 
            // Extra_Commander
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(379, 326);
            this.Controls.Add(this.label_Remark);
            this.Controls.Add(this.label_Command);
            this.Controls.Add(this.pictureBox_save);
            this.Controls.Add(this.pictureBox_shot);
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.Name = "Extra_Commander";
            this.Text = "Commander";
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