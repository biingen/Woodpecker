using System.Drawing;
namespace AutoTest
{
    partial class Form1
    {
        /// <summary>
        /// 設計工具所需的變數。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清除任何使用中的資源。
        /// </summary>
        /// <param name="disposing">如果應該處置 Managed 資源則為 true，否則為 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form 設計工具產生的程式碼

        /// <summary>
        /// 此為設計工具支援所需的方法 - 請勿使用程式碼編輯器修改這個方法的內容。
        ///
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.PowerBtn = new System.Windows.Forms.Button();
            this.SettingBtn = new System.Windows.Forms.Button();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.DataGridView1 = new System.Windows.Forms.DataGridView();
            this.Column1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column4 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column5 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column6 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column7 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column8 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column9 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column10 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.StartBtn = new System.Windows.Forms.Button();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.serialPort1 = new System.IO.Ports.SerialPort(this.components);
            this.serialPort2 = new System.IO.Ports.SerialPort(this.components);
            this.TimeLable = new System.Windows.Forms.Label();
            this.timer = new System.Windows.Forms.Timer(this.components);
            this.RedratLable = new System.Windows.Forms.Label();
            this.labelVersion = new System.Windows.Forms.Label();
            this.Com1Btn = new System.Windows.Forms.Button();
            this.Com2Btn = new System.Windows.Forms.Button();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.SchBtn1 = new System.Windows.Forms.Button();
            this.SchBtn2 = new System.Windows.Forms.Button();
            this.SchOpen1 = new System.Windows.Forms.OpenFileDialog();
            this.SchOpen2 = new System.Windows.Forms.OpenFileDialog();
            this.SchBtn3 = new System.Windows.Forms.Button();
            this.SchOpen3 = new System.Windows.Forms.OpenFileDialog();
            this.SchBtn4 = new System.Windows.Forms.Button();
            this.SchBtn5 = new System.Windows.Forms.Button();
            this.SchOpen4 = new System.Windows.Forms.OpenFileDialog();
            this.SchOpen5 = new System.Windows.Forms.OpenFileDialog();
            this.backgroundWorker = new System.ComponentModel.BackgroundWorker();
            this.DataBtn = new System.Windows.Forms.Button();
            this.saveFileDialog = new System.Windows.Forms.SaveFileDialog();
            this.notifyIcon = new System.Windows.Forms.NotifyIcon(this.components);
            this.panelVideo = new System.Windows.Forms.PictureBox();
            this.MiniPicBox = new System.Windows.Forms.PictureBox();
            this.ClosePicBox = new System.Windows.Forms.PictureBox();
            this.WriteBtn = new System.Windows.Forms.Button();
            this.pictureBox4 = new System.Windows.Forms.PictureBox();
            this.pictureBox3 = new System.Windows.Forms.PictureBox();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.pictureBox5 = new System.Windows.Forms.PictureBox();
            this.CamPreviewBtn = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.DataGridView1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.panelVideo)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.MiniPicBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ClosePicBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox5)).BeginInit();
            this.SuspendLayout();
            // 
            // PowerBtn
            // 
            this.PowerBtn.BackColor = System.Drawing.Color.Transparent;
            this.PowerBtn.Enabled = false;
            this.PowerBtn.Font = new System.Drawing.Font("MS Reference Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.PowerBtn.Location = new System.Drawing.Point(181, 35);
            this.PowerBtn.Name = "PowerBtn";
            this.PowerBtn.Size = new System.Drawing.Size(140, 30);
            this.PowerBtn.TabIndex = 26;
            this.PowerBtn.Text = "POWER ON/OFF";
            this.PowerBtn.UseVisualStyleBackColor = false;
            this.PowerBtn.Click += new System.EventHandler(this.PowerBtn_Click);
            // 
            // SettingBtn
            // 
            this.SettingBtn.BackColor = System.Drawing.Color.Transparent;
            this.SettingBtn.Font = new System.Drawing.Font("MS Reference Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.SettingBtn.Location = new System.Drawing.Point(98, 35);
            this.SettingBtn.Name = "SettingBtn";
            this.SettingBtn.Size = new System.Drawing.Size(85, 30);
            this.SettingBtn.TabIndex = 25;
            this.SettingBtn.Text = "SETTING";
            this.SettingBtn.UseVisualStyleBackColor = true;
            this.SettingBtn.Click += new System.EventHandler(this.SettingBtn_Click);
            // 
            // textBox1
            // 
            this.textBox1.BackColor = System.Drawing.Color.Black;
            this.textBox1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBox1.Font = new System.Drawing.Font("MS Reference Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox1.ForeColor = System.Drawing.Color.White;
            this.textBox1.Location = new System.Drawing.Point(525, 91);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBox1.Size = new System.Drawing.Size(513, 368);
            this.textBox1.TabIndex = 24;
            // 
            // DataGridView1
            // 
            this.DataGridView1.AllowUserToResizeColumns = false;
            this.DataGridView1.AllowUserToResizeRows = false;
            this.DataGridView1.BackgroundColor = System.Drawing.Color.White;
            this.DataGridView1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.DataGridView1.ClipboardCopyMode = System.Windows.Forms.DataGridViewClipboardCopyMode.EnableWithoutHeaderText;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.PaleTurquoise;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Berlin Sans FB", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.DataGridView1.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.DataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.DataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Column1,
            this.Column2,
            this.Column3,
            this.Column4,
            this.Column5,
            this.Column6,
            this.Column7,
            this.Column8,
            this.Column9,
            this.Column10});
            this.DataGridView1.GridColor = System.Drawing.Color.Wheat;
            this.DataGridView1.Location = new System.Drawing.Point(-1, 496);
            this.DataGridView1.Name = "DataGridView1";
            this.DataGridView1.RowHeadersWidth = 30;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("MS Reference Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.Color.DeepSkyBlue;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.Color.White;
            this.DataGridView1.RowsDefaultCellStyle = dataGridViewCellStyle2;
            this.DataGridView1.RowTemplate.Height = 24;
            this.DataGridView1.Size = new System.Drawing.Size(1062, 230);
            this.DataGridView1.TabIndex = 23;
            // 
            // Column1
            // 
            this.Column1.HeaderText = "RC Key";
            this.Column1.Name = "Column1";
            this.Column1.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.Column1.ToolTipText = "_cmd, _log, _astro, _quantum, _dektec";
            this.Column1.Width = 80;
            // 
            // Column2
            // 
            this.Column2.HeaderText = "Times";
            this.Column2.Name = "Column2";
            this.Column2.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.Column2.Width = 50;
            // 
            // Column3
            // 
            this.Column3.HeaderText = "RC Repeat";
            this.Column3.Name = "Column3";
            this.Column3.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.Column3.Width = 90;
            // 
            // Column4
            // 
            this.Column4.HeaderText = "Picture";
            this.Column4.Name = "Column4";
            this.Column4.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.Column4.ToolTipText = "_shot";
            this.Column4.Width = 70;
            // 
            // Column5
            // 
            this.Column5.HeaderText = "。Video Recording 。Dektec ";
            this.Column5.Name = "Column5";
            this.Column5.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.Column5.ToolTipText = "_start, _stop";
            this.Column5.Width = 125;
            // 
            // Column6
            // 
            this.Column6.HeaderText = "Log Capture";
            this.Column6.Name = "Column6";
            this.Column6.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.Column6.ToolTipText = "_saveCom1, _saveCom2, clearCom1, clearCom2";
            // 
            // Column7
            // 
            this.Column7.HeaderText = "。AC Power on/off     。Stream Name";
            this.Column7.Name = "Column7";
            this.Column7.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.Column7.ToolTipText = "_on, _off";
            this.Column7.Width = 145;
            // 
            // Column8
            // 
            this.Column8.HeaderText = "。Timing        。TV System";
            this.Column8.Name = "Column8";
            this.Column8.Width = 110;
            // 
            // Column9
            // 
            this.Column9.HeaderText = "。Quantum Color Space 。Frequency";
            this.Column9.Name = "Column9";
            this.Column9.Width = 160;
            // 
            // Column10
            // 
            this.Column10.HeaderText = "Sys Delay";
            this.Column10.Name = "Column10";
            this.Column10.Width = 80;
            // 
            // StartBtn
            // 
            this.StartBtn.BackColor = System.Drawing.Color.Transparent;
            this.StartBtn.Font = new System.Drawing.Font("MS Reference Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.StartBtn.Location = new System.Drawing.Point(15, 35);
            this.StartBtn.Name = "StartBtn";
            this.StartBtn.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.StartBtn.Size = new System.Drawing.Size(85, 30);
            this.StartBtn.TabIndex = 22;
            this.StartBtn.Text = "START";
            this.StartBtn.UseVisualStyleBackColor = false;
            this.StartBtn.Click += new System.EventHandler(this.StartBtn_Click);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.BackColor = System.Drawing.Color.Transparent;
            this.label6.Font = new System.Drawing.Font("MS Reference Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.ForeColor = System.Drawing.Color.Black;
            this.label6.Location = new System.Drawing.Point(577, 46);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(48, 16);
            this.label6.TabIndex = 36;
            this.label6.Text = "Power";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.BackColor = System.Drawing.Color.Transparent;
            this.label5.Font = new System.Drawing.Font("MS Reference Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.ForeColor = System.Drawing.Color.Black;
            this.label5.Location = new System.Drawing.Point(496, 46);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(57, 16);
            this.label5.TabIndex = 34;
            this.label5.Text = "Camera";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.BackColor = System.Drawing.Color.Transparent;
            this.label4.Font = new System.Drawing.Font("MS Reference Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.ForeColor = System.Drawing.Color.Black;
            this.label4.Location = new System.Drawing.Point(418, 46);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(54, 16);
            this.label4.TabIndex = 32;
            this.label4.Text = "RedRat";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.BackColor = System.Drawing.Color.Transparent;
            this.label2.Font = new System.Drawing.Font("MS Reference Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.ForeColor = System.Drawing.Color.Black;
            this.label2.Location = new System.Drawing.Point(804, 7);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(46, 19);
            this.label2.TabIndex = 30;
            this.label2.Text = "Loop";
            // 
            // label1
            // 
            this.label1.BackColor = System.Drawing.Color.Transparent;
            this.label1.Font = new System.Drawing.Font("MS Reference Sans Serif", 36F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.Tomato;
            this.label1.Location = new System.Drawing.Point(838, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(217, 51);
            this.label1.TabIndex = 29;
            this.label1.Text = "---------";
            // 
            // serialPort1
            // 
            this.serialPort1.DataReceived += new System.IO.Ports.SerialDataReceivedEventHandler(this.serialPort1_DataReceived);
            // 
            // serialPort2
            // 
            this.serialPort2.DataReceived += new System.IO.Ports.SerialDataReceivedEventHandler(this.serialPort2_DataReceived);
            // 
            // TimeLable
            // 
            this.TimeLable.AutoSize = true;
            this.TimeLable.BackColor = System.Drawing.Color.Transparent;
            this.TimeLable.Font = new System.Drawing.Font("MS Reference Sans Serif", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TimeLable.ForeColor = System.Drawing.Color.Violet;
            this.TimeLable.Location = new System.Drawing.Point(330, 743);
            this.TimeLable.Name = "TimeLable";
            this.TimeLable.Size = new System.Drawing.Size(378, 29);
            this.TimeLable.TabIndex = 44;
            this.TimeLable.Text = "xxx, xx xxx xxxx xx:xx:xx xxx";
            this.TimeLable.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // timer
            // 
            this.timer.Enabled = true;
            this.timer.Interval = 1000;
            this.timer.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // RedratLable
            // 
            this.RedratLable.AutoSize = true;
            this.RedratLable.BackColor = System.Drawing.Color.Transparent;
            this.RedratLable.Font = new System.Drawing.Font("MS Reference Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.RedratLable.ForeColor = System.Drawing.Color.Tomato;
            this.RedratLable.Location = new System.Drawing.Point(4, 466);
            this.RedratLable.Name = "RedratLable";
            this.RedratLable.Size = new System.Drawing.Size(226, 24);
            this.RedratLable.TabIndex = 45;
            this.RedratLable.Text = "------------------------";
            // 
            // labelVersion
            // 
            this.labelVersion.AutoSize = true;
            this.labelVersion.BackColor = System.Drawing.Color.Transparent;
            this.labelVersion.Font = new System.Drawing.Font("MS Reference Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelVersion.ForeColor = System.Drawing.Color.MidnightBlue;
            this.labelVersion.Location = new System.Drawing.Point(487, 784);
            this.labelVersion.Name = "labelVersion";
            this.labelVersion.Size = new System.Drawing.Size(79, 19);
            this.labelVersion.TabIndex = 46;
            this.labelVersion.Text = "Ver. 2.00";
            this.labelVersion.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.labelVersion.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.labelVersion_MouseClick);
            // 
            // Com1Btn
            // 
            this.Com1Btn.BackColor = System.Drawing.Color.Transparent;
            this.Com1Btn.Font = new System.Drawing.Font("MS Reference Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Com1Btn.Location = new System.Drawing.Point(525, 69);
            this.Com1Btn.Name = "Com1Btn";
            this.Com1Btn.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.Com1Btn.Size = new System.Drawing.Size(257, 22);
            this.Com1Btn.TabIndex = 48;
            this.Com1Btn.Text = "Serial Port 1";
            this.Com1Btn.UseVisualStyleBackColor = false;
            this.Com1Btn.Click += new System.EventHandler(this.Com1Btn_Click);
            // 
            // Com2Btn
            // 
            this.Com2Btn.BackColor = System.Drawing.Color.Transparent;
            this.Com2Btn.Font = new System.Drawing.Font("MS Reference Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Com2Btn.Location = new System.Drawing.Point(781, 69);
            this.Com2Btn.Name = "Com2Btn";
            this.Com2Btn.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.Com2Btn.Size = new System.Drawing.Size(257, 22);
            this.Com2Btn.TabIndex = 49;
            this.Com2Btn.Text = "Serial Port 2";
            this.Com2Btn.UseVisualStyleBackColor = false;
            this.Com2Btn.Click += new System.EventHandler(this.Com2Btn_Click);
            // 
            // textBox2
            // 
            this.textBox2.BackColor = System.Drawing.Color.Black;
            this.textBox2.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBox2.Font = new System.Drawing.Font("MS Reference Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox2.ForeColor = System.Drawing.Color.White;
            this.textBox2.Location = new System.Drawing.Point(525, 91);
            this.textBox2.Multiline = true;
            this.textBox2.Name = "textBox2";
            this.textBox2.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBox2.Size = new System.Drawing.Size(513, 368);
            this.textBox2.TabIndex = 50;
            // 
            // SchBtn1
            // 
            this.SchBtn1.Font = new System.Drawing.Font("MS Reference Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.SchBtn1.Location = new System.Drawing.Point(458, 471);
            this.SchBtn1.Name = "SchBtn1";
            this.SchBtn1.Size = new System.Drawing.Size(23, 23);
            this.SchBtn1.TabIndex = 51;
            this.SchBtn1.Text = "1";
            this.SchBtn1.UseVisualStyleBackColor = true;
            this.SchBtn1.Click += new System.EventHandler(this.SchBtn1_Click);
            // 
            // SchBtn2
            // 
            this.SchBtn2.Font = new System.Drawing.Font("MS Reference Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.SchBtn2.Location = new System.Drawing.Point(485, 471);
            this.SchBtn2.Name = "SchBtn2";
            this.SchBtn2.Size = new System.Drawing.Size(23, 23);
            this.SchBtn2.TabIndex = 52;
            this.SchBtn2.Text = "2";
            this.SchBtn2.UseVisualStyleBackColor = true;
            this.SchBtn2.Click += new System.EventHandler(this.SchBtn2_Click);
            // 
            // SchBtn3
            // 
            this.SchBtn3.Font = new System.Drawing.Font("MS Reference Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.SchBtn3.Location = new System.Drawing.Point(513, 471);
            this.SchBtn3.Name = "SchBtn3";
            this.SchBtn3.Size = new System.Drawing.Size(23, 23);
            this.SchBtn3.TabIndex = 55;
            this.SchBtn3.Text = "3";
            this.SchBtn3.UseVisualStyleBackColor = true;
            this.SchBtn3.Click += new System.EventHandler(this.SchBtn3_Click);
            // 
            // SchBtn4
            // 
            this.SchBtn4.Font = new System.Drawing.Font("MS Reference Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.SchBtn4.Location = new System.Drawing.Point(541, 471);
            this.SchBtn4.Name = "SchBtn4";
            this.SchBtn4.Size = new System.Drawing.Size(23, 23);
            this.SchBtn4.TabIndex = 56;
            this.SchBtn4.Text = "4";
            this.SchBtn4.UseVisualStyleBackColor = true;
            this.SchBtn4.Click += new System.EventHandler(this.SchBtn4_Click);
            // 
            // SchBtn5
            // 
            this.SchBtn5.Font = new System.Drawing.Font("MS Reference Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.SchBtn5.Location = new System.Drawing.Point(569, 471);
            this.SchBtn5.Name = "SchBtn5";
            this.SchBtn5.Size = new System.Drawing.Size(23, 23);
            this.SchBtn5.TabIndex = 57;
            this.SchBtn5.Text = "5";
            this.SchBtn5.UseVisualStyleBackColor = true;
            this.SchBtn5.Click += new System.EventHandler(this.SchBtn5_Click);
            // 
            // backgroundWorker
            // 
            this.backgroundWorker.WorkerReportsProgress = true;
            this.backgroundWorker.WorkerSupportsCancellation = true;
            // 
            // DataBtn
            // 
            this.DataBtn.Location = new System.Drawing.Point(4, 732);
            this.DataBtn.Name = "DataBtn";
            this.DataBtn.Size = new System.Drawing.Size(75, 69);
            this.DataBtn.TabIndex = 62;
            this.DataBtn.Text = "Data";
            this.DataBtn.UseVisualStyleBackColor = true;
            this.DataBtn.Visible = false;
            // 
            // notifyIcon
            // 
            this.notifyIcon.Text = "AutoTest";
            this.notifyIcon.Visible = true;
            // 
            // panelVideo
            // 
            this.panelVideo.BackColor = System.Drawing.Color.Black;
            this.panelVideo.BackgroundImage = global::AutoTest.Properties.Resources.TV_Screen;
            this.panelVideo.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.panelVideo.Location = new System.Drawing.Point(0, 69);
            this.panelVideo.Name = "panelVideo";
            this.panelVideo.Size = new System.Drawing.Size(525, 390);
            this.panelVideo.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.panelVideo.TabIndex = 28;
            this.panelVideo.TabStop = false;
            // 
            // MiniPicBox
            // 
            this.MiniPicBox.Image = global::AutoTest.Properties.Resources.Minimize;
            this.MiniPicBox.Location = new System.Drawing.Point(32, -2);
            this.MiniPicBox.Name = "MiniPicBox";
            this.MiniPicBox.Size = new System.Drawing.Size(25, 25);
            this.MiniPicBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.MiniPicBox.TabIndex = 61;
            this.MiniPicBox.TabStop = false;
            this.MiniPicBox.Click += new System.EventHandler(this.MiniPicBox_Click);
            // 
            // ClosePicBox
            // 
            this.ClosePicBox.Image = global::AutoTest.Properties.Resources.Close;
            this.ClosePicBox.Location = new System.Drawing.Point(4, -2);
            this.ClosePicBox.Name = "ClosePicBox";
            this.ClosePicBox.Size = new System.Drawing.Size(25, 25);
            this.ClosePicBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.ClosePicBox.TabIndex = 60;
            this.ClosePicBox.TabStop = false;
            this.ClosePicBox.Click += new System.EventHandler(this.ClosePicBox_Click);
            // 
            // WriteBtn
            // 
            this.WriteBtn.BackColor = System.Drawing.Color.Transparent;
            this.WriteBtn.BackgroundImage = global::AutoTest.Properties.Resources.Write_Scheduler;
            this.WriteBtn.Font = new System.Drawing.Font("MS Reference Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.WriteBtn.Location = new System.Drawing.Point(1003, 462);
            this.WriteBtn.Name = "WriteBtn";
            this.WriteBtn.Size = new System.Drawing.Size(32, 32);
            this.WriteBtn.TabIndex = 47;
            this.WriteBtn.UseVisualStyleBackColor = false;
            this.WriteBtn.Click += new System.EventHandler(this.WriteBtn_Click);
            // 
            // pictureBox4
            // 
            this.pictureBox4.Location = new System.Drawing.Point(1052, 69);
            this.pictureBox4.Name = "pictureBox4";
            this.pictureBox4.Size = new System.Drawing.Size(522, 390);
            this.pictureBox4.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox4.TabIndex = 38;
            this.pictureBox4.TabStop = false;
            // 
            // pictureBox3
            // 
            this.pictureBox3.BackColor = System.Drawing.Color.Transparent;
            this.pictureBox3.Location = new System.Drawing.Point(580, 13);
            this.pictureBox3.Name = "pictureBox3";
            this.pictureBox3.Size = new System.Drawing.Size(30, 30);
            this.pictureBox3.TabIndex = 35;
            this.pictureBox3.TabStop = false;
            this.pictureBox3.Click += new System.EventHandler(this.pictureBox3_Click);
            // 
            // pictureBox2
            // 
            this.pictureBox2.BackColor = System.Drawing.Color.Transparent;
            this.pictureBox2.Location = new System.Drawing.Point(509, 13);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(30, 30);
            this.pictureBox2.TabIndex = 33;
            this.pictureBox2.TabStop = false;
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackColor = System.Drawing.Color.Transparent;
            this.pictureBox1.Location = new System.Drawing.Point(437, 13);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(30, 30);
            this.pictureBox1.TabIndex = 31;
            this.pictureBox1.TabStop = false;
            // 
            // pictureBox5
            // 
            this.pictureBox5.Location = new System.Drawing.Point(523, 131);
            this.pictureBox5.Name = "pictureBox5";
            this.pictureBox5.Size = new System.Drawing.Size(522, 390);
            this.pictureBox5.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox5.TabIndex = 38;
            this.pictureBox5.TabStop = false;
            // 
            // CamPreviewBtn
            // 
            this.CamPreviewBtn.Font = new System.Drawing.Font("MS Reference Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.CamPreviewBtn.Location = new System.Drawing.Point(98, 35);
            this.CamPreviewBtn.Name = "CamPreviewBtn";
            this.CamPreviewBtn.Size = new System.Drawing.Size(223, 30);
            this.CamPreviewBtn.TabIndex = 63;
            this.CamPreviewBtn.Text = "CAMERA PREVIEW";
            this.CamPreviewBtn.UseVisualStyleBackColor = true;
            this.CamPreviewBtn.Click += new System.EventHandler(this.CamPreviewBtn_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Gainsboro;
            this.ClientSize = new System.Drawing.Size(1038, 806);
            this.Controls.Add(this.DataGridView1);
            this.Controls.Add(this.DataBtn);
            this.Controls.Add(this.SettingBtn);
            this.Controls.Add(this.Com2Btn);
            this.Controls.Add(this.Com1Btn);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.panelVideo);
            this.Controls.Add(this.MiniPicBox);
            this.Controls.Add(this.ClosePicBox);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.textBox2);
            this.Controls.Add(this.SchBtn5);
            this.Controls.Add(this.SchBtn4);
            this.Controls.Add(this.SchBtn3);
            this.Controls.Add(this.SchBtn2);
            this.Controls.Add(this.SchBtn1);
            this.Controls.Add(this.WriteBtn);
            this.Controls.Add(this.labelVersion);
            this.Controls.Add(this.RedratLable);
            this.Controls.Add(this.TimeLable);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.pictureBox4);
            this.Controls.Add(this.pictureBox3);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.pictureBox2);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.PowerBtn);
            this.Controls.Add(this.StartBtn);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.CamPreviewBtn);
            this.DoubleBuffered = true;
            this.Font = new System.Drawing.Font("Berlin Sans FB", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "Form1";
            this.Opacity = 0.94D;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "AutoTest";
            this.MinimumSizeChanged += new System.EventHandler(this.MiniPicBox_Click);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.gPanelTitleBack_MouseDown);
            ((System.ComponentModel.ISupportInitialize)(this.DataGridView1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.panelVideo)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.MiniPicBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ClosePicBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox5)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button PowerBtn;
        private System.Windows.Forms.Button SettingBtn;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.DataGridView DataGridView1;
        private System.Windows.Forms.PictureBox panelVideo;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.PictureBox pictureBox3;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.PictureBox pictureBox2;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.IO.Ports.SerialPort serialPort1;
        private System.Windows.Forms.PictureBox pictureBox4;
        private System.IO.Ports.SerialPort serialPort2;
        private USBClassLibrary.USBClass USBPort;
        private USBClassLibrary.USBClass.DeviceProperties USBDeviceProperties;
        bool MyUSBHubDeviceConnected;
        bool MyUSBRedratDeviceConnected;
        bool MyUSBCameraDeviceConnected;
        private System.Windows.Forms.Label TimeLable;
        private System.Windows.Forms.Timer timer;
        private System.Windows.Forms.Label RedratLable;
        private System.Windows.Forms.PictureBox pictureBox5;
        private System.Windows.Forms.Label labelVersion;
        private System.Windows.Forms.Button Com1Btn;
        private System.Windows.Forms.Button Com2Btn;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.OpenFileDialog SchOpen1;
        private System.Windows.Forms.OpenFileDialog SchOpen2;
        private System.Windows.Forms.OpenFileDialog SchOpen3;
        private System.Windows.Forms.OpenFileDialog SchOpen4;
        private System.Windows.Forms.OpenFileDialog SchOpen5;
        private System.Windows.Forms.PictureBox ClosePicBox;
        private System.Windows.Forms.PictureBox MiniPicBox;
        protected internal System.Windows.Forms.Button WriteBtn;
        protected internal System.Windows.Forms.Button StartBtn;
        protected internal System.Windows.Forms.Button SchBtn1;
        protected internal System.Windows.Forms.Button SchBtn2;
        protected internal System.Windows.Forms.Button SchBtn3;
        protected internal System.Windows.Forms.Button SchBtn4;
        protected internal System.Windows.Forms.Button SchBtn5;
        private System.ComponentModel.BackgroundWorker backgroundWorker;
        private System.Windows.Forms.Button DataBtn;
        private System.Windows.Forms.SaveFileDialog saveFileDialog;
        private System.Windows.Forms.NotifyIcon notifyIcon;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column1;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column2;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column3;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column4;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column5;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column6;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column7;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column8;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column9;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column10;
        private System.Windows.Forms.Button CamPreviewBtn;
    }
}

