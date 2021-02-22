using System.Drawing;

namespace OPTT
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.button_Setting = new System.Windows.Forms.Button();
            this.textBox_serial = new System.Windows.Forms.TextBox();
            this.label_Power = new System.Windows.Forms.Label();
            this.label_Camera = new System.Windows.Forms.Label();
            this.label_RedRat = new System.Windows.Forms.Label();
            this.labellabel_LoopTimes_Value = new System.Windows.Forms.Label();
            this.label_LoopNumber_Value = new System.Windows.Forms.Label();
            this.PortA = new System.IO.Ports.SerialPort(this.components);
            this.PortB = new System.IO.Ports.SerialPort(this.components);
            this.TimeLabel = new System.Windows.Forms.Label();
            this.timer_system = new System.Windows.Forms.Timer(this.components);
            this.label_FwVersion = new System.Windows.Forms.Label();
            this.button_SerialPort = new System.Windows.Forms.Button();
            this.button_Schedule1 = new System.Windows.Forms.Button();
            this.button_Schedule2 = new System.Windows.Forms.Button();
            this.SchOpen1 = new System.Windows.Forms.OpenFileDialog();
            this.SchOpen2 = new System.Windows.Forms.OpenFileDialog();
            this.button_Schedule3 = new System.Windows.Forms.Button();
            this.SchOpen3 = new System.Windows.Forms.OpenFileDialog();
            this.button_Schedule4 = new System.Windows.Forms.Button();
            this.button_Schedule5 = new System.Windows.Forms.Button();
            this.SchOpen4 = new System.Windows.Forms.OpenFileDialog();
            this.SchOpen5 = new System.Windows.Forms.OpenFileDialog();
            this.DataBtn = new System.Windows.Forms.Button();
            this.saveFileDialog = new System.Windows.Forms.SaveFileDialog();
            this.button_VirtualRC = new System.Windows.Forms.Button();
            this.TimeLabel2 = new System.Windows.Forms.Label();
            this.labelSch1Timer = new System.Windows.Forms.Label();
            this.labelSch2Timer = new System.Windows.Forms.Label();
            this.labelSch5Timer = new System.Windows.Forms.Label();
            this.labelSch4Timer = new System.Windows.Forms.Label();
            this.labelSch3Timer = new System.Windows.Forms.Label();
            this.button_Pause = new System.Windows.Forms.Button();
            this.label_ScheduleTime_Value = new System.Windows.Forms.Label();
            this.timer_countdown = new System.Windows.Forms.Timer(this.components);
            this.panel1 = new System.Windows.Forms.Panel();
            this.SegLabel4 = new System.Windows.Forms.Label();
            this.SegLabel3 = new System.Windows.Forms.Label();
            this.SegLabel2 = new System.Windows.Forms.Label();
            this.SegLabel1 = new System.Windows.Forms.Label();
            this.button_TimeLine = new System.Windows.Forms.Button();
            this.button_SaveSchedule = new System.Windows.Forms.Button();
            this.button_Schedule = new System.Windows.Forms.Button();
            this.button_Camera = new System.Windows.Forms.Button();
            this.label_LoopTimes = new System.Windows.Forms.Label();
            this.label_LoopNumber = new System.Windows.Forms.Label();
            this.label_ScheduleTime = new System.Windows.Forms.Label();
            this.label_AutoBox = new System.Windows.Forms.Label();
            this.serialPortWood = new System.IO.Ports.SerialPort(this.components);
            this.button_Input = new System.Windows.Forms.Button();
            this.button_Output = new System.Windows.Forms.Button();
            this.label_Command = new System.Windows.Forms.Label();
            this.labelGPIO_Input = new System.Windows.Forms.Label();
            this.label_TestTime = new System.Windows.Forms.Label();
            this.label_TestTime_Value = new System.Windows.Forms.Label();
            this.button_InsertRow = new System.Windows.Forms.Button();
            this.panel_VirtualRC = new System.Windows.Forms.Panel();
            this.label_BoxVersion = new System.Windows.Forms.Label();
            this.label_Remark = new System.Windows.Forms.Label();
            this.button_AcUsb = new System.Windows.Forms.Button();
            this.panel_AcUsb = new System.Windows.Forms.Panel();
            this.groupBox_Usb = new System.Windows.Forms.GroupBox();
            this.label_Usb2 = new System.Windows.Forms.Label();
            this.label_Usb1 = new System.Windows.Forms.Label();
            this.pictureBox_Usb1 = new System.Windows.Forms.PictureBox();
            this.pictureBox_Usb2 = new System.Windows.Forms.PictureBox();
            this.groupBox_Ac = new System.Windows.Forms.GroupBox();
            this.label_Ac2 = new System.Windows.Forms.Label();
            this.label_Ac1 = new System.Windows.Forms.Label();
            this.pictureBox_Ac2 = new System.Windows.Forms.PictureBox();
            this.pictureBox_Ac1 = new System.Windows.Forms.PictureBox();
            this.comboBox_CameraDevice = new System.Windows.Forms.ComboBox();
            this.PortC = new System.IO.Ports.SerialPort(this.components);
            this.button_Network = new System.Windows.Forms.Button();
            this.label_ext_board = new System.Windows.Forms.Label();
            this.timer_canbus = new System.Windows.Forms.Timer(this.components);
            this.label_canbus = new System.Windows.Forms.Label();
            this.button_Analysis = new System.Windows.Forms.Button();
            this.timer_kline = new System.Windows.Forms.Timer(this.components);
            this.button_Start = new System.Windows.Forms.Button();
            this.pictureBox_canbus = new System.Windows.Forms.PictureBox();
            this.pictureBox_ext_board = new System.Windows.Forms.PictureBox();
            this.pictureBox_BlueRat = new System.Windows.Forms.PictureBox();
            this.MiniPicBox = new System.Windows.Forms.PictureBox();
            this.ClosePicBox = new System.Windows.Forms.PictureBox();
            this.pictureBox4 = new System.Windows.Forms.PictureBox();
            this.pictureBox_AcPower = new System.Windows.Forms.PictureBox();
            this.pictureBox_Camera = new System.Windows.Forms.PictureBox();
            this.pictureBox_RedRat = new System.Windows.Forms.PictureBox();
            this.panelVideo = new System.Windows.Forms.PictureBox();
            this.pictureBox5 = new System.Windows.Forms.PictureBox();
            this.button_savelog = new System.Windows.Forms.Button();
            this.PortD = new System.IO.Ports.SerialPort(this.components);
            this.PortE = new System.IO.Ports.SerialPort(this.components);
            this.comboBox_savelog = new System.Windows.Forms.ComboBox();
            this.timer_ca310 = new System.Windows.Forms.Timer(this.components);
            this.label_Minolta = new System.Windows.Forms.Label();
            this.pictureBox_Minolta = new System.Windows.Forms.PictureBox();
            this.timer_Chamber = new System.Windows.Forms.Timer(this.components);
            this.timer_duringShot = new System.Windows.Forms.Timer(this.components);
            this.DataGridView_Schedule = new OPTT.SafeDataGridView();
            this.Column1 = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.Column2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column4 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column5 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column6 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column7 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column8 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column9 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column10 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.panel1.SuspendLayout();
            this.panel_VirtualRC.SuspendLayout();
            this.panel_AcUsb.SuspendLayout();
            this.groupBox_Usb.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_Usb1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_Usb2)).BeginInit();
            this.groupBox_Ac.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_Ac2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_Ac1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_canbus)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_ext_board)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_BlueRat)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.MiniPicBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ClosePicBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_AcPower)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_Camera)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_RedRat)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.panelVideo)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox5)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_Minolta)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.DataGridView_Schedule)).BeginInit();
            this.SuspendLayout();
            // 
            // button_Setting
            // 
            this.button_Setting.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.button_Setting.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(103)))), ((int)(((byte)(179)))));
            this.button_Setting.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button_Setting.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button_Setting.ForeColor = System.Drawing.Color.White;
            this.button_Setting.Location = new System.Drawing.Point(849, 22);
            this.button_Setting.Margin = new System.Windows.Forms.Padding(2);
            this.button_Setting.Name = "button_Setting";
            this.button_Setting.Size = new System.Drawing.Size(103, 30);
            this.button_Setting.TabIndex = 25;
            this.button_Setting.Text = "SETTINGS";
            this.button_Setting.UseVisualStyleBackColor = false;
            this.button_Setting.Click += new System.EventHandler(this.SettingBtn_Click);
            // 
            // textBox_serial
            // 
            this.textBox_serial.BackColor = System.Drawing.Color.Black;
            this.textBox_serial.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBox_serial.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox_serial.ForeColor = System.Drawing.Color.White;
            this.textBox_serial.Location = new System.Drawing.Point(4, 63);
            this.textBox_serial.Margin = new System.Windows.Forms.Padding(2);
            this.textBox_serial.Multiline = true;
            this.textBox_serial.Name = "textBox_serial";
            this.textBox_serial.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBox_serial.Size = new System.Drawing.Size(950, 400);
            this.textBox_serial.TabIndex = 24;
            // 
            // label_Power
            // 
            this.label_Power.AutoSize = true;
            this.label_Power.BackColor = System.Drawing.Color.Transparent;
            this.label_Power.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label_Power.ForeColor = System.Drawing.Color.Gainsboro;
            this.label_Power.Location = new System.Drawing.Point(175, 29);
            this.label_Power.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label_Power.Name = "label_Power";
            this.label_Power.Size = new System.Drawing.Size(47, 17);
            this.label_Power.TabIndex = 36;
            this.label_Power.Text = "Power";
            // 
            // label_Camera
            // 
            this.label_Camera.AutoSize = true;
            this.label_Camera.BackColor = System.Drawing.Color.Transparent;
            this.label_Camera.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label_Camera.ForeColor = System.Drawing.Color.Gainsboro;
            this.label_Camera.Location = new System.Drawing.Point(94, 29);
            this.label_Camera.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label_Camera.Name = "label_Camera";
            this.label_Camera.Size = new System.Drawing.Size(57, 17);
            this.label_Camera.TabIndex = 34;
            this.label_Camera.Text = "Camera";
            // 
            // label_RedRat
            // 
            this.label_RedRat.AutoSize = true;
            this.label_RedRat.BackColor = System.Drawing.Color.Transparent;
            this.label_RedRat.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label_RedRat.ForeColor = System.Drawing.Color.Gainsboro;
            this.label_RedRat.Location = new System.Drawing.Point(437, 29);
            this.label_RedRat.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label_RedRat.Name = "label_RedRat";
            this.label_RedRat.Size = new System.Drawing.Size(56, 17);
            this.label_RedRat.TabIndex = 32;
            this.label_RedRat.Text = "RedRat";
            this.label_RedRat.Visible = false;
            // 
            // labellabel_LoopTimes_Value
            // 
            this.labellabel_LoopTimes_Value.AutoSize = true;
            this.labellabel_LoopTimes_Value.BackColor = System.Drawing.Color.Transparent;
            this.labellabel_LoopTimes_Value.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labellabel_LoopTimes_Value.ForeColor = System.Drawing.Color.White;
            this.labellabel_LoopTimes_Value.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.labellabel_LoopTimes_Value.Location = new System.Drawing.Point(158, 27);
            this.labellabel_LoopTimes_Value.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.labellabel_LoopTimes_Value.Name = "labellabel_LoopTimes_Value";
            this.labellabel_LoopTimes_Value.Size = new System.Drawing.Size(59, 20);
            this.labellabel_LoopTimes_Value.TabIndex = 30;
            this.labellabel_LoopTimes_Value.Text = "99999";
            this.labellabel_LoopTimes_Value.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label_LoopNumber_Value
            // 
            this.label_LoopNumber_Value.AutoSize = true;
            this.label_LoopNumber_Value.BackColor = System.Drawing.Color.Transparent;
            this.label_LoopNumber_Value.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label_LoopNumber_Value.ForeColor = System.Drawing.Color.White;
            this.label_LoopNumber_Value.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.label_LoopNumber_Value.Location = new System.Drawing.Point(158, 78);
            this.label_LoopNumber_Value.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label_LoopNumber_Value.Name = "label_LoopNumber_Value";
            this.label_LoopNumber_Value.Size = new System.Drawing.Size(19, 20);
            this.label_LoopNumber_Value.TabIndex = 29;
            this.label_LoopNumber_Value.Text = "0";
            this.label_LoopNumber_Value.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // TimeLabel
            // 
            this.TimeLabel.AutoSize = true;
            this.TimeLabel.BackColor = System.Drawing.Color.DarkOrange;
            this.TimeLabel.Font = new System.Drawing.Font("Microsoft JhengHei", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.TimeLabel.ForeColor = System.Drawing.Color.White;
            this.TimeLabel.Location = new System.Drawing.Point(318, 690);
            this.TimeLabel.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.TimeLabel.Name = "TimeLabel";
            this.TimeLabel.Size = new System.Drawing.Size(184, 18);
            this.TimeLabel.TabIndex = 44;
            this.TimeLabel.Text = "xxx, xx xxx xxxx xx:xx:xx xxx";
            this.TimeLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // timer_system
            // 
            this.timer_system.Enabled = true;
            this.timer_system.Interval = 1;
            this.timer_system.Tick += new System.EventHandler(this.timer_Tick);
            // 
            // label_FwVersion
            // 
            this.label_FwVersion.AutoSize = true;
            this.label_FwVersion.BackColor = System.Drawing.Color.Transparent;
            this.label_FwVersion.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label_FwVersion.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(207)))), ((int)(((byte)(102)))), ((int)(((byte)(121)))));
            this.label_FwVersion.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.label_FwVersion.Location = new System.Drawing.Point(2, 99);
            this.label_FwVersion.Margin = new System.Windows.Forms.Padding(2);
            this.label_FwVersion.Name = "label_FwVersion";
            this.label_FwVersion.Size = new System.Drawing.Size(82, 17);
            this.label_FwVersion.TabIndex = 46;
            this.label_FwVersion.Text = "Ver. 0.0.0.0";
            // 
            // button_SerialPort
            // 
            this.button_SerialPort.BackColor = System.Drawing.SystemColors.Control;
            this.button_SerialPort.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.button_SerialPort.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button_SerialPort.ForeColor = System.Drawing.Color.Black;
            this.button_SerialPort.Location = new System.Drawing.Point(965, 159);
            this.button_SerialPort.Margin = new System.Windows.Forms.Padding(2);
            this.button_SerialPort.Name = "button_SerialPort";
            this.button_SerialPort.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.button_SerialPort.Size = new System.Drawing.Size(110, 30);
            this.button_SerialPort.TabIndex = 48;
            this.button_SerialPort.Text = "Serial Port";
            this.button_SerialPort.UseVisualStyleBackColor = false;
            this.button_SerialPort.Visible = false;
            this.button_SerialPort.Click += new System.EventHandler(this.Com1Btn_Click);
            // 
            // button_Schedule1
            // 
            this.button_Schedule1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(103)))), ((int)(((byte)(179)))));
            this.button_Schedule1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button_Schedule1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button_Schedule1.ForeColor = System.Drawing.Color.White;
            this.button_Schedule1.Location = new System.Drawing.Point(954, 438);
            this.button_Schedule1.Margin = new System.Windows.Forms.Padding(2);
            this.button_Schedule1.Name = "button_Schedule1";
            this.button_Schedule1.Size = new System.Drawing.Size(24, 24);
            this.button_Schedule1.TabIndex = 51;
            this.button_Schedule1.Text = "1";
            this.button_Schedule1.UseVisualStyleBackColor = false;
            this.button_Schedule1.Click += new System.EventHandler(this.SchBtn1_Click);
            // 
            // button_Schedule2
            // 
            this.button_Schedule2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(103)))), ((int)(((byte)(179)))));
            this.button_Schedule2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button_Schedule2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button_Schedule2.ForeColor = System.Drawing.Color.White;
            this.button_Schedule2.Location = new System.Drawing.Point(954, 414);
            this.button_Schedule2.Margin = new System.Windows.Forms.Padding(2);
            this.button_Schedule2.Name = "button_Schedule2";
            this.button_Schedule2.Size = new System.Drawing.Size(24, 24);
            this.button_Schedule2.TabIndex = 52;
            this.button_Schedule2.Text = "2";
            this.button_Schedule2.UseVisualStyleBackColor = false;
            this.button_Schedule2.Click += new System.EventHandler(this.SchBtn2_Click);
            // 
            // button_Schedule3
            // 
            this.button_Schedule3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(103)))), ((int)(((byte)(179)))));
            this.button_Schedule3.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button_Schedule3.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button_Schedule3.ForeColor = System.Drawing.Color.White;
            this.button_Schedule3.Location = new System.Drawing.Point(954, 390);
            this.button_Schedule3.Margin = new System.Windows.Forms.Padding(2);
            this.button_Schedule3.Name = "button_Schedule3";
            this.button_Schedule3.Size = new System.Drawing.Size(24, 24);
            this.button_Schedule3.TabIndex = 55;
            this.button_Schedule3.Text = "3";
            this.button_Schedule3.UseVisualStyleBackColor = false;
            this.button_Schedule3.Click += new System.EventHandler(this.SchBtn3_Click);
            // 
            // button_Schedule4
            // 
            this.button_Schedule4.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(103)))), ((int)(((byte)(179)))));
            this.button_Schedule4.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button_Schedule4.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button_Schedule4.ForeColor = System.Drawing.Color.White;
            this.button_Schedule4.Location = new System.Drawing.Point(954, 366);
            this.button_Schedule4.Margin = new System.Windows.Forms.Padding(2);
            this.button_Schedule4.Name = "button_Schedule4";
            this.button_Schedule4.Size = new System.Drawing.Size(24, 24);
            this.button_Schedule4.TabIndex = 56;
            this.button_Schedule4.Text = "4";
            this.button_Schedule4.UseVisualStyleBackColor = false;
            this.button_Schedule4.Click += new System.EventHandler(this.SchBtn4_Click);
            // 
            // button_Schedule5
            // 
            this.button_Schedule5.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(103)))), ((int)(((byte)(179)))));
            this.button_Schedule5.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button_Schedule5.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button_Schedule5.ForeColor = System.Drawing.Color.White;
            this.button_Schedule5.Location = new System.Drawing.Point(954, 342);
            this.button_Schedule5.Margin = new System.Windows.Forms.Padding(2);
            this.button_Schedule5.Name = "button_Schedule5";
            this.button_Schedule5.Size = new System.Drawing.Size(24, 24);
            this.button_Schedule5.TabIndex = 57;
            this.button_Schedule5.Text = "5";
            this.button_Schedule5.UseVisualStyleBackColor = false;
            this.button_Schedule5.Click += new System.EventHandler(this.SchBtn5_Click);
            // 
            // DataBtn
            // 
            this.DataBtn.Location = new System.Drawing.Point(226, 690);
            this.DataBtn.Margin = new System.Windows.Forms.Padding(2);
            this.DataBtn.Name = "DataBtn";
            this.DataBtn.Size = new System.Drawing.Size(51, 22);
            this.DataBtn.TabIndex = 62;
            this.DataBtn.Text = "Data";
            this.DataBtn.UseVisualStyleBackColor = true;
            this.DataBtn.Visible = false;
            this.DataBtn.Click += new System.EventHandler(this.DataBtn_Click);
            // 
            // button_VirtualRC
            // 
            this.button_VirtualRC.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.button_VirtualRC.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(103)))), ((int)(((byte)(179)))));
            this.button_VirtualRC.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.button_VirtualRC.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button_VirtualRC.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button_VirtualRC.ForeColor = System.Drawing.Color.White;
            this.button_VirtualRC.Location = new System.Drawing.Point(2, 54);
            this.button_VirtualRC.Margin = new System.Windows.Forms.Padding(2);
            this.button_VirtualRC.Name = "button_VirtualRC";
            this.button_VirtualRC.Size = new System.Drawing.Size(100, 30);
            this.button_VirtualRC.TabIndex = 63;
            this.button_VirtualRC.Text = "Virtual RC";
            this.button_VirtualRC.UseVisualStyleBackColor = false;
            this.button_VirtualRC.Visible = false;
            // 
            // TimeLabel2
            // 
            this.TimeLabel2.BackColor = System.Drawing.Color.Transparent;
            this.TimeLabel2.Font = new System.Drawing.Font("Consolas", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TimeLabel2.ForeColor = System.Drawing.Color.LightSeaGreen;
            this.TimeLabel2.Location = new System.Drawing.Point(10, 679);
            this.TimeLabel2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.TimeLabel2.Name = "TimeLabel2";
            this.TimeLabel2.Size = new System.Drawing.Size(246, 41);
            this.TimeLabel2.TabIndex = 64;
            this.TimeLabel2.Text = "yyyy-MM-dd  HH:mm:ss";
            this.TimeLabel2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // labelSch1Timer
            // 
            this.labelSch1Timer.AccessibleRole = System.Windows.Forms.AccessibleRole.ScrollBar;
            this.labelSch1Timer.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.labelSch1Timer.AutoSize = true;
            this.labelSch1Timer.BackColor = System.Drawing.Color.Transparent;
            this.labelSch1Timer.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.labelSch1Timer.ForeColor = System.Drawing.Color.Gainsboro;
            this.labelSch1Timer.Location = new System.Drawing.Point(10, 14);
            this.labelSch1Timer.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.labelSch1Timer.Name = "labelSch1Timer";
            this.labelSch1Timer.Size = new System.Drawing.Size(146, 17);
            this.labelSch1Timer.TabIndex = 65;
            this.labelSch1Timer.Text = "2014-12-11  12:23:00";
            // 
            // labelSch2Timer
            // 
            this.labelSch2Timer.AccessibleRole = System.Windows.Forms.AccessibleRole.ScrollBar;
            this.labelSch2Timer.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.labelSch2Timer.AutoSize = true;
            this.labelSch2Timer.BackColor = System.Drawing.Color.Transparent;
            this.labelSch2Timer.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.labelSch2Timer.ForeColor = System.Drawing.Color.Gainsboro;
            this.labelSch2Timer.Location = new System.Drawing.Point(10, 62);
            this.labelSch2Timer.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.labelSch2Timer.Name = "labelSch2Timer";
            this.labelSch2Timer.Size = new System.Drawing.Size(146, 17);
            this.labelSch2Timer.TabIndex = 66;
            this.labelSch2Timer.Text = "2014-12-11  12:23:00";
            // 
            // labelSch5Timer
            // 
            this.labelSch5Timer.AccessibleRole = System.Windows.Forms.AccessibleRole.ScrollBar;
            this.labelSch5Timer.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.labelSch5Timer.AutoSize = true;
            this.labelSch5Timer.BackColor = System.Drawing.Color.Transparent;
            this.labelSch5Timer.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.labelSch5Timer.ForeColor = System.Drawing.Color.Gainsboro;
            this.labelSch5Timer.Location = new System.Drawing.Point(10, 202);
            this.labelSch5Timer.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.labelSch5Timer.Name = "labelSch5Timer";
            this.labelSch5Timer.Size = new System.Drawing.Size(146, 17);
            this.labelSch5Timer.TabIndex = 67;
            this.labelSch5Timer.Text = "2014-12-11  12:23:00";
            // 
            // labelSch4Timer
            // 
            this.labelSch4Timer.AccessibleRole = System.Windows.Forms.AccessibleRole.ScrollBar;
            this.labelSch4Timer.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.labelSch4Timer.AutoSize = true;
            this.labelSch4Timer.BackColor = System.Drawing.Color.Transparent;
            this.labelSch4Timer.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.labelSch4Timer.ForeColor = System.Drawing.Color.Gainsboro;
            this.labelSch4Timer.Location = new System.Drawing.Point(10, 155);
            this.labelSch4Timer.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.labelSch4Timer.Name = "labelSch4Timer";
            this.labelSch4Timer.Size = new System.Drawing.Size(146, 17);
            this.labelSch4Timer.TabIndex = 68;
            this.labelSch4Timer.Text = "2014-12-11  12:23:00";
            // 
            // labelSch3Timer
            // 
            this.labelSch3Timer.AccessibleRole = System.Windows.Forms.AccessibleRole.ScrollBar;
            this.labelSch3Timer.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.labelSch3Timer.AutoSize = true;
            this.labelSch3Timer.BackColor = System.Drawing.Color.Transparent;
            this.labelSch3Timer.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.labelSch3Timer.ForeColor = System.Drawing.Color.Gainsboro;
            this.labelSch3Timer.Location = new System.Drawing.Point(10, 109);
            this.labelSch3Timer.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.labelSch3Timer.Name = "labelSch3Timer";
            this.labelSch3Timer.Size = new System.Drawing.Size(146, 17);
            this.labelSch3Timer.TabIndex = 69;
            this.labelSch3Timer.Text = "2014-12-11  12:23:00";
            // 
            // button_Pause
            // 
            this.button_Pause.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(103)))), ((int)(((byte)(179)))));
            this.button_Pause.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button_Pause.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button_Pause.ForeColor = System.Drawing.Color.White;
            this.button_Pause.Location = new System.Drawing.Point(738, 22);
            this.button_Pause.Margin = new System.Windows.Forms.Padding(2);
            this.button_Pause.Name = "button_Pause";
            this.button_Pause.Size = new System.Drawing.Size(103, 30);
            this.button_Pause.TabIndex = 73;
            this.button_Pause.Text = "PAUSE";
            this.button_Pause.UseVisualStyleBackColor = false;
            this.button_Pause.Click += new System.EventHandler(this.PauseButton_Click);
            // 
            // label_ScheduleTime_Value
            // 
            this.label_ScheduleTime_Value.AutoSize = true;
            this.label_ScheduleTime_Value.BackColor = System.Drawing.Color.Transparent;
            this.label_ScheduleTime_Value.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label_ScheduleTime_Value.ForeColor = System.Drawing.Color.White;
            this.label_ScheduleTime_Value.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.label_ScheduleTime_Value.Location = new System.Drawing.Point(279, 27);
            this.label_ScheduleTime_Value.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label_ScheduleTime_Value.Name = "label_ScheduleTime_Value";
            this.label_ScheduleTime_Value.Size = new System.Drawing.Size(145, 20);
            this.label_ScheduleTime_Value.TabIndex = 75;
            this.label_ScheduleTime_Value.Text = "0d 0h 0m 0s 0ms";
            this.label_ScheduleTime_Value.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // timer_countdown
            // 
            this.timer_countdown.Interval = 1;
            this.timer_countdown.Tick += new System.EventHandler(this.timer_countdown_Tick);
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.Transparent;
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.SegLabel4);
            this.panel1.Controls.Add(this.SegLabel3);
            this.panel1.Controls.Add(this.SegLabel2);
            this.panel1.Controls.Add(this.SegLabel1);
            this.panel1.Controls.Add(this.labelSch1Timer);
            this.panel1.Controls.Add(this.labelSch3Timer);
            this.panel1.Controls.Add(this.labelSch5Timer);
            this.panel1.Controls.Add(this.labelSch4Timer);
            this.panel1.Controls.Add(this.labelSch2Timer);
            this.panel1.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.panel1.Location = new System.Drawing.Point(778, 227);
            this.panel1.Margin = new System.Windows.Forms.Padding(2);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(176, 236);
            this.panel1.TabIndex = 76;
            // 
            // SegLabel4
            // 
            this.SegLabel4.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.SegLabel4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.SegLabel4.ForeColor = System.Drawing.Color.Gainsboro;
            this.SegLabel4.Location = new System.Drawing.Point(6, 182);
            this.SegLabel4.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.SegLabel4.Name = "SegLabel4";
            this.SegLabel4.Size = new System.Drawing.Size(162, 12);
            this.SegLabel4.TabIndex = 73;
            this.SegLabel4.Text = "---------------------------------------";
            // 
            // SegLabel3
            // 
            this.SegLabel3.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.SegLabel3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.SegLabel3.ForeColor = System.Drawing.Color.Gainsboro;
            this.SegLabel3.Location = new System.Drawing.Point(6, 134);
            this.SegLabel3.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.SegLabel3.Name = "SegLabel3";
            this.SegLabel3.Size = new System.Drawing.Size(162, 12);
            this.SegLabel3.TabIndex = 72;
            this.SegLabel3.Text = "---------------------------------------";
            // 
            // SegLabel2
            // 
            this.SegLabel2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.SegLabel2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.SegLabel2.ForeColor = System.Drawing.Color.Gainsboro;
            this.SegLabel2.Location = new System.Drawing.Point(6, 88);
            this.SegLabel2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.SegLabel2.Name = "SegLabel2";
            this.SegLabel2.Size = new System.Drawing.Size(162, 12);
            this.SegLabel2.TabIndex = 71;
            this.SegLabel2.Text = "---------------------------------------";
            // 
            // SegLabel1
            // 
            this.SegLabel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.SegLabel1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.SegLabel1.ForeColor = System.Drawing.Color.Gainsboro;
            this.SegLabel1.Location = new System.Drawing.Point(6, 41);
            this.SegLabel1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.SegLabel1.Name = "SegLabel1";
            this.SegLabel1.Size = new System.Drawing.Size(162, 12);
            this.SegLabel1.TabIndex = 70;
            this.SegLabel1.Text = "---------------------------------------";
            // 
            // button_TimeLine
            // 
            this.button_TimeLine.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.button_TimeLine.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(103)))), ((int)(((byte)(179)))));
            this.button_TimeLine.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.button_TimeLine.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button_TimeLine.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.button_TimeLine.ForeColor = System.Drawing.Color.White;
            this.button_TimeLine.Location = new System.Drawing.Point(638, 477);
            this.button_TimeLine.Margin = new System.Windows.Forms.Padding(2);
            this.button_TimeLine.Name = "button_TimeLine";
            this.button_TimeLine.Size = new System.Drawing.Size(92, 30);
            this.button_TimeLine.TabIndex = 77;
            this.button_TimeLine.Text = "TIMELINE";
            this.button_TimeLine.UseVisualStyleBackColor = false;
            this.button_TimeLine.Visible = false;
            this.button_TimeLine.Click += new System.EventHandler(this.TimerPanelbutton_Click);
            // 
            // button_SaveSchedule
            // 
            this.button_SaveSchedule.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.button_SaveSchedule.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(103)))), ((int)(((byte)(179)))));
            this.button_SaveSchedule.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.button_SaveSchedule.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button_SaveSchedule.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button_SaveSchedule.ForeColor = System.Drawing.Color.White;
            this.button_SaveSchedule.Location = new System.Drawing.Point(879, 477);
            this.button_SaveSchedule.Margin = new System.Windows.Forms.Padding(2);
            this.button_SaveSchedule.Name = "button_SaveSchedule";
            this.button_SaveSchedule.Size = new System.Drawing.Size(73, 30);
            this.button_SaveSchedule.TabIndex = 47;
            this.button_SaveSchedule.Text = "SAVE SCHDULE";
            this.button_SaveSchedule.UseVisualStyleBackColor = false;
            this.button_SaveSchedule.Click += new System.EventHandler(this.WriteBtn_Click);
            // 
            // button_Schedule
            // 
            this.button_Schedule.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(103)))), ((int)(((byte)(179)))));
            this.button_Schedule.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button_Schedule.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button_Schedule.ForeColor = System.Drawing.Color.White;
            this.button_Schedule.Location = new System.Drawing.Point(965, 63);
            this.button_Schedule.Margin = new System.Windows.Forms.Padding(2);
            this.button_Schedule.Name = "button_Schedule";
            this.button_Schedule.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.button_Schedule.Size = new System.Drawing.Size(110, 30);
            this.button_Schedule.TabIndex = 78;
            this.button_Schedule.Text = "Schedule";
            this.button_Schedule.UseVisualStyleBackColor = false;
            this.button_Schedule.Click += new System.EventHandler(this.Button_TabScheduler_Click);
            // 
            // button_Camera
            // 
            this.button_Camera.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(103)))), ((int)(((byte)(179)))));
            this.button_Camera.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button_Camera.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button_Camera.ForeColor = System.Drawing.Color.White;
            this.button_Camera.Location = new System.Drawing.Point(965, 111);
            this.button_Camera.Margin = new System.Windows.Forms.Padding(2);
            this.button_Camera.Name = "button_Camera";
            this.button_Camera.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.button_Camera.Size = new System.Drawing.Size(110, 30);
            this.button_Camera.TabIndex = 79;
            this.button_Camera.Text = "Camera";
            this.button_Camera.UseVisualStyleBackColor = false;
            this.button_Camera.Click += new System.EventHandler(this.Button_TabCamera_Click);
            // 
            // label_LoopTimes
            // 
            this.label_LoopTimes.AutoSize = true;
            this.label_LoopTimes.BackColor = System.Drawing.Color.Transparent;
            this.label_LoopTimes.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label_LoopTimes.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(3)))), ((int)(((byte)(218)))), ((int)(((byte)(198)))));
            this.label_LoopTimes.Location = new System.Drawing.Point(158, 9);
            this.label_LoopTimes.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label_LoopTimes.Name = "label_LoopTimes";
            this.label_LoopTimes.Size = new System.Drawing.Size(88, 16);
            this.label_LoopTimes.TabIndex = 80;
            this.label_LoopTimes.Text = "LOOP TIMES";
            // 
            // label_LoopNumber
            // 
            this.label_LoopNumber.AutoSize = true;
            this.label_LoopNumber.BackColor = System.Drawing.Color.Transparent;
            this.label_LoopNumber.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label_LoopNumber.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(3)))), ((int)(((byte)(218)))), ((int)(((byte)(198)))));
            this.label_LoopNumber.Location = new System.Drawing.Point(158, 60);
            this.label_LoopNumber.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label_LoopNumber.Name = "label_LoopNumber";
            this.label_LoopNumber.Size = new System.Drawing.Size(106, 16);
            this.label_LoopNumber.TabIndex = 81;
            this.label_LoopNumber.Text = "LOOP NUMBER";
            // 
            // label_ScheduleTime
            // 
            this.label_ScheduleTime.AutoSize = true;
            this.label_ScheduleTime.BackColor = System.Drawing.Color.Transparent;
            this.label_ScheduleTime.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label_ScheduleTime.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(3)))), ((int)(((byte)(218)))), ((int)(((byte)(198)))));
            this.label_ScheduleTime.Location = new System.Drawing.Point(279, 9);
            this.label_ScheduleTime.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label_ScheduleTime.Name = "label_ScheduleTime";
            this.label_ScheduleTime.Size = new System.Drawing.Size(119, 16);
            this.label_ScheduleTime.TabIndex = 82;
            this.label_ScheduleTime.Text = "EST. FINISH TIME";
            // 
            // label_AutoBox
            // 
            this.label_AutoBox.AutoSize = true;
            this.label_AutoBox.BackColor = System.Drawing.Color.Transparent;
            this.label_AutoBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label_AutoBox.ForeColor = System.Drawing.Color.Gainsboro;
            this.label_AutoBox.Location = new System.Drawing.Point(18, 29);
            this.label_AutoBox.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label_AutoBox.Name = "label_AutoBox";
            this.label_AutoBox.Size = new System.Drawing.Size(53, 17);
            this.label_AutoBox.TabIndex = 83;
            this.label_AutoBox.Text = "AutoKit";
            // 
            // button_Input
            // 
            this.button_Input.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.button_Input.BackColor = System.Drawing.Color.Transparent;
            this.button_Input.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.button_Input.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.button_Input.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button_Input.Location = new System.Drawing.Point(991, 526);
            this.button_Input.Margin = new System.Windows.Forms.Padding(2);
            this.button_Input.Name = "button_Input";
            this.button_Input.Size = new System.Drawing.Size(80, 25);
            this.button_Input.TabIndex = 85;
            this.button_Input.Text = "INPUT";
            this.button_Input.UseVisualStyleBackColor = false;
            this.button_Input.Visible = false;
            this.button_Input.Click += new System.EventHandler(this.button_Input_Click);
            // 
            // button_Output
            // 
            this.button_Output.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.button_Output.BackColor = System.Drawing.Color.Transparent;
            this.button_Output.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.button_Output.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.button_Output.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button_Output.Location = new System.Drawing.Point(994, 562);
            this.button_Output.Margin = new System.Windows.Forms.Padding(2);
            this.button_Output.Name = "button_Output";
            this.button_Output.Size = new System.Drawing.Size(80, 25);
            this.button_Output.TabIndex = 86;
            this.button_Output.Text = "OUTPUT";
            this.button_Output.UseVisualStyleBackColor = false;
            this.button_Output.Visible = false;
            this.button_Output.Click += new System.EventHandler(this.button_Output_Click);
            // 
            // label_Command
            // 
            this.label_Command.BackColor = System.Drawing.Color.Transparent;
            this.label_Command.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label_Command.ForeColor = System.Drawing.Color.Tomato;
            this.label_Command.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.label_Command.Location = new System.Drawing.Point(490, 506);
            this.label_Command.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label_Command.Name = "label_Command";
            this.label_Command.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.label_Command.Size = new System.Drawing.Size(461, 39);
            this.label_Command.TabIndex = 45;
            this.label_Command.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // labelGPIO_Input
            // 
            this.labelGPIO_Input.BackColor = System.Drawing.Color.Transparent;
            this.labelGPIO_Input.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.labelGPIO_Input.ForeColor = System.Drawing.Color.MediumSlateBlue;
            this.labelGPIO_Input.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.labelGPIO_Input.Location = new System.Drawing.Point(505, 477);
            this.labelGPIO_Input.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.labelGPIO_Input.Name = "labelGPIO_Input";
            this.labelGPIO_Input.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.labelGPIO_Input.Size = new System.Drawing.Size(127, 25);
            this.labelGPIO_Input.TabIndex = 87;
            this.labelGPIO_Input.Text = "---------------";
            this.labelGPIO_Input.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.labelGPIO_Input.Visible = false;
            // 
            // label_TestTime
            // 
            this.label_TestTime.AutoSize = true;
            this.label_TestTime.BackColor = System.Drawing.Color.Transparent;
            this.label_TestTime.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label_TestTime.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(3)))), ((int)(((byte)(218)))), ((int)(((byte)(198)))));
            this.label_TestTime.Location = new System.Drawing.Point(279, 60);
            this.label_TestTime.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label_TestTime.Name = "label_TestTime";
            this.label_TestTime.Size = new System.Drawing.Size(79, 16);
            this.label_TestTime.TabIndex = 90;
            this.label_TestTime.Text = "TEST TIME";
            // 
            // label_TestTime_Value
            // 
            this.label_TestTime_Value.AutoSize = true;
            this.label_TestTime_Value.BackColor = System.Drawing.Color.Transparent;
            this.label_TestTime_Value.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label_TestTime_Value.ForeColor = System.Drawing.Color.White;
            this.label_TestTime_Value.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.label_TestTime_Value.Location = new System.Drawing.Point(279, 78);
            this.label_TestTime_Value.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label_TestTime_Value.Name = "label_TestTime_Value";
            this.label_TestTime_Value.Size = new System.Drawing.Size(145, 20);
            this.label_TestTime_Value.TabIndex = 89;
            this.label_TestTime_Value.Text = "0d 0h 0m 0s 0ms";
            this.label_TestTime_Value.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // button_InsertRow
            // 
            this.button_InsertRow.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.button_InsertRow.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(103)))), ((int)(((byte)(179)))));
            this.button_InsertRow.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.button_InsertRow.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button_InsertRow.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button_InsertRow.ForeColor = System.Drawing.Color.White;
            this.button_InsertRow.Location = new System.Drawing.Point(740, 477);
            this.button_InsertRow.Margin = new System.Windows.Forms.Padding(2);
            this.button_InsertRow.Name = "button_InsertRow";
            this.button_InsertRow.Size = new System.Drawing.Size(131, 30);
            this.button_InsertRow.TabIndex = 91;
            this.button_InsertRow.Text = "INSERT A ROW";
            this.button_InsertRow.UseVisualStyleBackColor = false;
            this.button_InsertRow.Click += new System.EventHandler(this.button_insert_a_row_Click);
            // 
            // panel_VirtualRC
            // 
            this.panel_VirtualRC.AutoScroll = true;
            this.panel_VirtualRC.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.panel_VirtualRC.BackColor = System.Drawing.Color.Transparent;
            this.panel_VirtualRC.Controls.Add(this.label_BoxVersion);
            this.panel_VirtualRC.Controls.Add(this.label_Remark);
            this.panel_VirtualRC.Controls.Add(this.label_FwVersion);
            this.panel_VirtualRC.Controls.Add(this.label_ScheduleTime_Value);
            this.panel_VirtualRC.Controls.Add(this.button_AcUsb);
            this.panel_VirtualRC.Controls.Add(this.labellabel_LoopTimes_Value);
            this.panel_VirtualRC.Controls.Add(this.label_LoopNumber_Value);
            this.panel_VirtualRC.Controls.Add(this.label_LoopTimes);
            this.panel_VirtualRC.Controls.Add(this.label_LoopNumber);
            this.panel_VirtualRC.Controls.Add(this.label_TestTime);
            this.panel_VirtualRC.Controls.Add(this.label_ScheduleTime);
            this.panel_VirtualRC.Controls.Add(this.label_TestTime_Value);
            this.panel_VirtualRC.Controls.Add(this.button_VirtualRC);
            this.panel_VirtualRC.Location = new System.Drawing.Point(4, 476);
            this.panel_VirtualRC.Margin = new System.Windows.Forms.Padding(2);
            this.panel_VirtualRC.Name = "panel_VirtualRC";
            this.panel_VirtualRC.Size = new System.Drawing.Size(950, 125);
            this.panel_VirtualRC.TabIndex = 92;
            // 
            // label_BoxVersion
            // 
            this.label_BoxVersion.AutoSize = true;
            this.label_BoxVersion.BackColor = System.Drawing.Color.Transparent;
            this.label_BoxVersion.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label_BoxVersion.ForeColor = System.Drawing.Color.SteelBlue;
            this.label_BoxVersion.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.label_BoxVersion.Location = new System.Drawing.Point(86, 99);
            this.label_BoxVersion.Margin = new System.Windows.Forms.Padding(2);
            this.label_BoxVersion.Name = "label_BoxVersion";
            this.label_BoxVersion.Size = new System.Drawing.Size(44, 17);
            this.label_BoxVersion.TabIndex = 100;
            this.label_BoxVersion.Text = "_0.00";
            this.label_BoxVersion.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label_Remark
            // 
            this.label_Remark.BackColor = System.Drawing.Color.Transparent;
            this.label_Remark.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label_Remark.ForeColor = System.Drawing.Color.Tomato;
            this.label_Remark.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.label_Remark.Location = new System.Drawing.Point(519, 60);
            this.label_Remark.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label_Remark.Name = "label_Remark";
            this.label_Remark.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.label_Remark.Size = new System.Drawing.Size(428, 39);
            this.label_Remark.TabIndex = 99;
            this.label_Remark.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // button_AcUsb
            // 
            this.button_AcUsb.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.button_AcUsb.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(103)))), ((int)(((byte)(179)))));
            this.button_AcUsb.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.button_AcUsb.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button_AcUsb.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button_AcUsb.ForeColor = System.Drawing.Color.White;
            this.button_AcUsb.Location = new System.Drawing.Point(2, 6);
            this.button_AcUsb.Margin = new System.Windows.Forms.Padding(2);
            this.button_AcUsb.Name = "button_AcUsb";
            this.button_AcUsb.Size = new System.Drawing.Size(100, 30);
            this.button_AcUsb.TabIndex = 94;
            this.button_AcUsb.Text = "AC / USB SWITCH";
            this.button_AcUsb.UseVisualStyleBackColor = false;
            this.button_AcUsb.Click += new System.EventHandler(this.button_AcUsb_Click);
            // 
            // panel_AcUsb
            // 
            this.panel_AcUsb.BackColor = System.Drawing.Color.Transparent;
            this.panel_AcUsb.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel_AcUsb.Controls.Add(this.groupBox_Usb);
            this.panel_AcUsb.Controls.Add(this.groupBox_Ac);
            this.panel_AcUsb.Location = new System.Drawing.Point(4, 303);
            this.panel_AcUsb.Margin = new System.Windows.Forms.Padding(2);
            this.panel_AcUsb.Name = "panel_AcUsb";
            this.panel_AcUsb.Size = new System.Drawing.Size(200, 160);
            this.panel_AcUsb.TabIndex = 93;
            // 
            // groupBox_Usb
            // 
            this.groupBox_Usb.Controls.Add(this.label_Usb2);
            this.groupBox_Usb.Controls.Add(this.label_Usb1);
            this.groupBox_Usb.Controls.Add(this.pictureBox_Usb1);
            this.groupBox_Usb.Controls.Add(this.pictureBox_Usb2);
            this.groupBox_Usb.ForeColor = System.Drawing.Color.Gainsboro;
            this.groupBox_Usb.Location = new System.Drawing.Point(12, 84);
            this.groupBox_Usb.Margin = new System.Windows.Forms.Padding(2);
            this.groupBox_Usb.Name = "groupBox_Usb";
            this.groupBox_Usb.Padding = new System.Windows.Forms.Padding(2);
            this.groupBox_Usb.Size = new System.Drawing.Size(170, 60);
            this.groupBox_Usb.TabIndex = 1;
            this.groupBox_Usb.TabStop = false;
            this.groupBox_Usb.Text = "USB";
            // 
            // label_Usb2
            // 
            this.label_Usb2.BackColor = System.Drawing.Color.Transparent;
            this.label_Usb2.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label_Usb2.ForeColor = System.Drawing.Color.Gainsboro;
            this.label_Usb2.Location = new System.Drawing.Point(85, 18);
            this.label_Usb2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label_Usb2.Name = "label_Usb2";
            this.label_Usb2.Size = new System.Drawing.Size(32, 32);
            this.label_Usb2.TabIndex = 100;
            this.label_Usb2.Text = "2";
            this.label_Usb2.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // label_Usb1
            // 
            this.label_Usb1.BackColor = System.Drawing.Color.Transparent;
            this.label_Usb1.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label_Usb1.ForeColor = System.Drawing.Color.Gainsboro;
            this.label_Usb1.Location = new System.Drawing.Point(16, 18);
            this.label_Usb1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label_Usb1.Name = "label_Usb1";
            this.label_Usb1.Size = new System.Drawing.Size(32, 32);
            this.label_Usb1.TabIndex = 99;
            this.label_Usb1.Text = "1";
            this.label_Usb1.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // pictureBox_Usb1
            // 
            this.pictureBox_Usb1.Image = global::OPTT.Properties.Resources.Switch_to_PC;
            this.pictureBox_Usb1.Location = new System.Drawing.Point(52, 18);
            this.pictureBox_Usb1.Margin = new System.Windows.Forms.Padding(2);
            this.pictureBox_Usb1.Name = "pictureBox_Usb1";
            this.pictureBox_Usb1.Size = new System.Drawing.Size(32, 32);
            this.pictureBox_Usb1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox_Usb1.TabIndex = 97;
            this.pictureBox_Usb1.TabStop = false;
            this.pictureBox_Usb1.Click += new System.EventHandler(this.pictureBox_Usb1_Click);
            // 
            // pictureBox_Usb2
            // 
            this.pictureBox_Usb2.Image = global::OPTT.Properties.Resources.Switch_to_PC;
            this.pictureBox_Usb2.Location = new System.Drawing.Point(121, 18);
            this.pictureBox_Usb2.Margin = new System.Windows.Forms.Padding(2);
            this.pictureBox_Usb2.Name = "pictureBox_Usb2";
            this.pictureBox_Usb2.Size = new System.Drawing.Size(32, 32);
            this.pictureBox_Usb2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox_Usb2.TabIndex = 98;
            this.pictureBox_Usb2.TabStop = false;
            this.pictureBox_Usb2.Click += new System.EventHandler(this.pictureBox_Usb2_Click);
            // 
            // groupBox_Ac
            // 
            this.groupBox_Ac.Controls.Add(this.label_Ac2);
            this.groupBox_Ac.Controls.Add(this.label_Ac1);
            this.groupBox_Ac.Controls.Add(this.pictureBox_Ac2);
            this.groupBox_Ac.Controls.Add(this.pictureBox_Ac1);
            this.groupBox_Ac.ForeColor = System.Drawing.Color.Gainsboro;
            this.groupBox_Ac.Location = new System.Drawing.Point(12, 20);
            this.groupBox_Ac.Margin = new System.Windows.Forms.Padding(2);
            this.groupBox_Ac.Name = "groupBox_Ac";
            this.groupBox_Ac.Padding = new System.Windows.Forms.Padding(2);
            this.groupBox_Ac.Size = new System.Drawing.Size(170, 60);
            this.groupBox_Ac.TabIndex = 0;
            this.groupBox_Ac.TabStop = false;
            this.groupBox_Ac.Text = "AC";
            // 
            // label_Ac2
            // 
            this.label_Ac2.BackColor = System.Drawing.Color.Transparent;
            this.label_Ac2.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label_Ac2.ForeColor = System.Drawing.Color.Gainsboro;
            this.label_Ac2.Location = new System.Drawing.Point(85, 18);
            this.label_Ac2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label_Ac2.Name = "label_Ac2";
            this.label_Ac2.Size = new System.Drawing.Size(32, 32);
            this.label_Ac2.TabIndex = 96;
            this.label_Ac2.Text = "2";
            this.label_Ac2.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // label_Ac1
            // 
            this.label_Ac1.BackColor = System.Drawing.Color.Transparent;
            this.label_Ac1.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label_Ac1.ForeColor = System.Drawing.Color.Gainsboro;
            this.label_Ac1.Location = new System.Drawing.Point(16, 18);
            this.label_Ac1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label_Ac1.Name = "label_Ac1";
            this.label_Ac1.Size = new System.Drawing.Size(32, 32);
            this.label_Ac1.TabIndex = 95;
            this.label_Ac1.Text = "1";
            this.label_Ac1.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // pictureBox_Ac2
            // 
            this.pictureBox_Ac2.Image = global::OPTT.Properties.Resources.Switch_On_AC;
            this.pictureBox_Ac2.Location = new System.Drawing.Point(121, 18);
            this.pictureBox_Ac2.Margin = new System.Windows.Forms.Padding(2);
            this.pictureBox_Ac2.Name = "pictureBox_Ac2";
            this.pictureBox_Ac2.Size = new System.Drawing.Size(32, 32);
            this.pictureBox_Ac2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox_Ac2.TabIndex = 1;
            this.pictureBox_Ac2.TabStop = false;
            this.pictureBox_Ac2.Click += new System.EventHandler(this.pictureBox_Ac2_Click);
            // 
            // pictureBox_Ac1
            // 
            this.pictureBox_Ac1.Image = global::OPTT.Properties.Resources.Switch_On_AC;
            this.pictureBox_Ac1.Location = new System.Drawing.Point(52, 18);
            this.pictureBox_Ac1.Margin = new System.Windows.Forms.Padding(2);
            this.pictureBox_Ac1.Name = "pictureBox_Ac1";
            this.pictureBox_Ac1.Size = new System.Drawing.Size(32, 32);
            this.pictureBox_Ac1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox_Ac1.TabIndex = 0;
            this.pictureBox_Ac1.TabStop = false;
            this.pictureBox_Ac1.Click += new System.EventHandler(this.pictureBox_Ac1_Click);
            // 
            // comboBox_CameraDevice
            // 
            this.comboBox_CameraDevice.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox_CameraDevice.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.comboBox_CameraDevice.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.comboBox_CameraDevice.FormattingEnabled = true;
            this.comboBox_CameraDevice.Location = new System.Drawing.Point(703, 63);
            this.comboBox_CameraDevice.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.comboBox_CameraDevice.Name = "comboBox_CameraDevice";
            this.comboBox_CameraDevice.Size = new System.Drawing.Size(250, 23);
            this.comboBox_CameraDevice.TabIndex = 95;
            this.comboBox_CameraDevice.SelectedIndexChanged += new System.EventHandler(this.comboBox_CameraDevice_SelectedIndexChanged);
            // 
            // button_Network
            // 
            this.button_Network.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.button_Network.BackColor = System.Drawing.SystemColors.Control;
            this.button_Network.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.button_Network.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.button_Network.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button_Network.ForeColor = System.Drawing.Color.White;
            this.button_Network.Location = new System.Drawing.Point(994, 21);
            this.button_Network.Margin = new System.Windows.Forms.Padding(2);
            this.button_Network.Name = "button_Network";
            this.button_Network.Size = new System.Drawing.Size(80, 25);
            this.button_Network.TabIndex = 98;
            this.button_Network.Text = "NETWORK";
            this.button_Network.UseVisualStyleBackColor = false;
            this.button_Network.Visible = false;
            this.button_Network.Click += new System.EventHandler(this.button_Network_Click);
            // 
            // label_ext_board
            // 
            this.label_ext_board.AutoSize = true;
            this.label_ext_board.BackColor = System.Drawing.Color.Transparent;
            this.label_ext_board.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label_ext_board.ForeColor = System.Drawing.Color.Gainsboro;
            this.label_ext_board.Location = new System.Drawing.Point(247, 29);
            this.label_ext_board.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label_ext_board.Name = "label_ext_board";
            this.label_ext_board.Size = new System.Drawing.Size(72, 17);
            this.label_ext_board.TabIndex = 100;
            this.label_ext_board.Text = "Ext_board";
            // 
            // timer_canbus
            // 
            this.timer_canbus.Interval = 250;
            this.timer_canbus.Tick += new System.EventHandler(this.timer_canbus_Tick);
            // 
            // label_canbus
            // 
            this.label_canbus.AutoSize = true;
            this.label_canbus.BackColor = System.Drawing.Color.Transparent;
            this.label_canbus.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label_canbus.ForeColor = System.Drawing.Color.Gainsboro;
            this.label_canbus.Location = new System.Drawing.Point(346, 29);
            this.label_canbus.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label_canbus.Name = "label_canbus";
            this.label_canbus.Size = new System.Drawing.Size(67, 17);
            this.label_canbus.TabIndex = 102;
            this.label_canbus.Text = "CAN_bus";
            // 
            // button_Analysis
            // 
            this.button_Analysis.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(103)))), ((int)(((byte)(179)))));
            this.button_Analysis.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button_Analysis.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button_Analysis.ForeColor = System.Drawing.Color.White;
            this.button_Analysis.Location = new System.Drawing.Point(986, 432);
            this.button_Analysis.Name = "button_Analysis";
            this.button_Analysis.Size = new System.Drawing.Size(86, 30);
            this.button_Analysis.TabIndex = 109;
            this.button_Analysis.Text = "Log Analysis";
            this.button_Analysis.UseVisualStyleBackColor = false;
            this.button_Analysis.Visible = false;
            // 
            // timer_kline
            // 
            this.timer_kline.Interval = 250;
            this.timer_kline.Tick += new System.EventHandler(this.Timer_kline_Tick);
            // 
            // button_Start
            // 
            this.button_Start.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.button_Start.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(103)))), ((int)(((byte)(179)))));
            this.button_Start.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button_Start.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button_Start.ForeColor = System.Drawing.Color.White;
            this.button_Start.Location = new System.Drawing.Point(629, 22);
            this.button_Start.Margin = new System.Windows.Forms.Padding(2);
            this.button_Start.Name = "button_Start";
            this.button_Start.Size = new System.Drawing.Size(103, 30);
            this.button_Start.TabIndex = 111;
            this.button_Start.Text = "START";
            this.button_Start.UseVisualStyleBackColor = false;
            this.button_Start.Click += new System.EventHandler(this.StartBtn_Click);
            // 
            // pictureBox_canbus
            // 
            this.pictureBox_canbus.BackColor = System.Drawing.Color.Transparent;
            this.pictureBox_canbus.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.pictureBox_canbus.Location = new System.Drawing.Point(330, 30);
            this.pictureBox_canbus.Margin = new System.Windows.Forms.Padding(2);
            this.pictureBox_canbus.Name = "pictureBox_canbus";
            this.pictureBox_canbus.Size = new System.Drawing.Size(13, 13);
            this.pictureBox_canbus.TabIndex = 101;
            this.pictureBox_canbus.TabStop = false;
            // 
            // pictureBox_ext_board
            // 
            this.pictureBox_ext_board.BackColor = System.Drawing.Color.Transparent;
            this.pictureBox_ext_board.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.pictureBox_ext_board.Location = new System.Drawing.Point(233, 30);
            this.pictureBox_ext_board.Margin = new System.Windows.Forms.Padding(2);
            this.pictureBox_ext_board.Name = "pictureBox_ext_board";
            this.pictureBox_ext_board.Size = new System.Drawing.Size(13, 13);
            this.pictureBox_ext_board.TabIndex = 99;
            this.pictureBox_ext_board.TabStop = false;
            // 
            // pictureBox_BlueRat
            // 
            this.pictureBox_BlueRat.BackColor = System.Drawing.Color.Transparent;
            this.pictureBox_BlueRat.Location = new System.Drawing.Point(5, 30);
            this.pictureBox_BlueRat.Margin = new System.Windows.Forms.Padding(2);
            this.pictureBox_BlueRat.Name = "pictureBox_BlueRat";
            this.pictureBox_BlueRat.Size = new System.Drawing.Size(13, 13);
            this.pictureBox_BlueRat.TabIndex = 84;
            this.pictureBox_BlueRat.TabStop = false;
            // 
            // MiniPicBox
            // 
            this.MiniPicBox.Image = global::OPTT.Properties.Resources.mini1;
            this.MiniPicBox.Location = new System.Drawing.Point(1404, -5);
            this.MiniPicBox.Margin = new System.Windows.Forms.Padding(2);
            this.MiniPicBox.Name = "MiniPicBox";
            this.MiniPicBox.Size = new System.Drawing.Size(34, 26);
            this.MiniPicBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.MiniPicBox.TabIndex = 61;
            this.MiniPicBox.TabStop = false;
            this.MiniPicBox.Visible = false;
            this.MiniPicBox.Click += new System.EventHandler(this.MiniPicBox_Click);
            this.MiniPicBox.MouseEnter += new System.EventHandler(this.MiniPicBox_Enter);
            this.MiniPicBox.MouseLeave += new System.EventHandler(this.MiniPicBox_Leave);
            // 
            // ClosePicBox
            // 
            this.ClosePicBox.Image = global::OPTT.Properties.Resources.close1;
            this.ClosePicBox.Location = new System.Drawing.Point(1404, -5);
            this.ClosePicBox.Margin = new System.Windows.Forms.Padding(2);
            this.ClosePicBox.Name = "ClosePicBox";
            this.ClosePicBox.Size = new System.Drawing.Size(34, 26);
            this.ClosePicBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.ClosePicBox.TabIndex = 60;
            this.ClosePicBox.TabStop = false;
            this.ClosePicBox.Visible = false;
            this.ClosePicBox.Click += new System.EventHandler(this.ClosePicBox_Click);
            this.ClosePicBox.MouseEnter += new System.EventHandler(this.ClosePicBox_Enter);
            this.ClosePicBox.MouseLeave += new System.EventHandler(this.ClosePicBox_Leave);
            // 
            // pictureBox4
            // 
            this.pictureBox4.Location = new System.Drawing.Point(1404, 21);
            this.pictureBox4.Margin = new System.Windows.Forms.Padding(2);
            this.pictureBox4.Name = "pictureBox4";
            this.pictureBox4.Size = new System.Drawing.Size(585, 435);
            this.pictureBox4.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox4.TabIndex = 38;
            this.pictureBox4.TabStop = false;
            // 
            // pictureBox_AcPower
            // 
            this.pictureBox_AcPower.BackColor = System.Drawing.Color.Transparent;
            this.pictureBox_AcPower.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.pictureBox_AcPower.Location = new System.Drawing.Point(161, 30);
            this.pictureBox_AcPower.Margin = new System.Windows.Forms.Padding(2);
            this.pictureBox_AcPower.Name = "pictureBox_AcPower";
            this.pictureBox_AcPower.Size = new System.Drawing.Size(13, 13);
            this.pictureBox_AcPower.TabIndex = 35;
            this.pictureBox_AcPower.TabStop = false;
            // 
            // pictureBox_Camera
            // 
            this.pictureBox_Camera.BackColor = System.Drawing.Color.Transparent;
            this.pictureBox_Camera.Location = new System.Drawing.Point(79, 30);
            this.pictureBox_Camera.Margin = new System.Windows.Forms.Padding(2);
            this.pictureBox_Camera.Name = "pictureBox_Camera";
            this.pictureBox_Camera.Size = new System.Drawing.Size(13, 13);
            this.pictureBox_Camera.TabIndex = 33;
            this.pictureBox_Camera.TabStop = false;
            // 
            // pictureBox_RedRat
            // 
            this.pictureBox_RedRat.BackColor = System.Drawing.Color.Transparent;
            this.pictureBox_RedRat.Location = new System.Drawing.Point(422, 30);
            this.pictureBox_RedRat.Margin = new System.Windows.Forms.Padding(2);
            this.pictureBox_RedRat.Name = "pictureBox_RedRat";
            this.pictureBox_RedRat.Size = new System.Drawing.Size(13, 13);
            this.pictureBox_RedRat.TabIndex = 31;
            this.pictureBox_RedRat.TabStop = false;
            this.pictureBox_RedRat.Visible = false;
            // 
            // panelVideo
            // 
            this.panelVideo.BackColor = System.Drawing.Color.Black;
            this.panelVideo.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.panelVideo.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelVideo.Image = global::OPTT.Properties.Resources.TV_Screen;
            this.panelVideo.Location = new System.Drawing.Point(4, 63);
            this.panelVideo.Margin = new System.Windows.Forms.Padding(2);
            this.panelVideo.Name = "panelVideo";
            this.panelVideo.Size = new System.Drawing.Size(950, 400);
            this.panelVideo.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.panelVideo.TabIndex = 28;
            this.panelVideo.TabStop = false;
            // 
            // pictureBox5
            // 
            this.pictureBox5.Location = new System.Drawing.Point(523, 115);
            this.pictureBox5.Name = "pictureBox5";
            this.pictureBox5.Size = new System.Drawing.Size(522, 390);
            this.pictureBox5.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox5.TabIndex = 38;
            this.pictureBox5.TabStop = false;
            // 
            // button_savelog
            // 
            this.button_savelog.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(103)))), ((int)(((byte)(179)))));
            this.button_savelog.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button_savelog.ForeColor = System.Drawing.Color.White;
            this.button_savelog.Location = new System.Drawing.Point(986, 395);
            this.button_savelog.Name = "button_savelog";
            this.button_savelog.Size = new System.Drawing.Size(86, 31);
            this.button_savelog.TabIndex = 112;
            this.button_savelog.Text = "Save Log";
            this.button_savelog.UseVisualStyleBackColor = false;
            this.button_savelog.Click += new System.EventHandler(this.button_savelog_Click);
            // 
            // comboBox_savelog
            // 
            this.comboBox_savelog.BackColor = System.Drawing.SystemColors.Window;
            this.comboBox_savelog.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox_savelog.FormattingEnabled = true;
            this.comboBox_savelog.Location = new System.Drawing.Point(986, 366);
            this.comboBox_savelog.Name = "comboBox_savelog";
            this.comboBox_savelog.Size = new System.Drawing.Size(86, 24);
            this.comboBox_savelog.TabIndex = 111;
            // 
            // timer_ca310
            // 
            this.timer_ca310.Interval = 250;
            // 
            // label_Minolta
            // 
            this.label_Minolta.AutoSize = true;
            this.label_Minolta.BackColor = System.Drawing.Color.Transparent;
            this.label_Minolta.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label_Minolta.ForeColor = System.Drawing.Color.Gainsboro;
            this.label_Minolta.Location = new System.Drawing.Point(524, 29);
            this.label_Minolta.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label_Minolta.Name = "label_Minolta";
            this.label_Minolta.Size = new System.Drawing.Size(53, 17);
            this.label_Minolta.TabIndex = 114;
            this.label_Minolta.Text = "Minolta";
            this.label_Minolta.Visible = false;
            // 
            // pictureBox_Minolta
            // 
            this.pictureBox_Minolta.BackColor = System.Drawing.Color.Transparent;
            this.pictureBox_Minolta.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.pictureBox_Minolta.Location = new System.Drawing.Point(508, 29);
            this.pictureBox_Minolta.Margin = new System.Windows.Forms.Padding(2);
            this.pictureBox_Minolta.Name = "pictureBox_Minolta";
            this.pictureBox_Minolta.Size = new System.Drawing.Size(13, 13);
            this.pictureBox_Minolta.TabIndex = 113;
            this.pictureBox_Minolta.TabStop = false;
            this.pictureBox_Minolta.Visible = false;
            // 
            // timer_Chamber
            // 
            this.timer_Chamber.Interval = 10000;
            // 
            // timer_duringShot
            // 
            this.timer_duringShot.Tick += new System.EventHandler(this.timer_duringShot_Tick);
            // 
            // DataGridView_Schedule
            // 
            this.DataGridView_Schedule.AllowUserToResizeColumns = false;
            this.DataGridView_Schedule.AllowUserToResizeRows = false;
            this.DataGridView_Schedule.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.DataGridView_Schedule.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.DataGridView_Schedule.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.DisplayedCells;
            this.DataGridView_Schedule.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(51)))), ((int)(((byte)(51)))));
            this.DataGridView_Schedule.ClipboardCopyMode = System.Windows.Forms.DataGridViewClipboardCopyMode.EnableWithoutHeaderText;
            this.DataGridView_Schedule.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(51)))), ((int)(((byte)(51)))));
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.Color.LightGray;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.DataGridView_Schedule.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.DataGridView_Schedule.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.DataGridView_Schedule.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
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
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(51)))), ((int)(((byte)(51)))));
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.Color.LightGray;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.DataGridView_Schedule.DefaultCellStyle = dataGridViewCellStyle2;
            this.DataGridView_Schedule.EnableHeadersVisualStyles = false;
            this.DataGridView_Schedule.GridColor = System.Drawing.SystemColors.Control;
            this.DataGridView_Schedule.Location = new System.Drawing.Point(4, 63);
            this.DataGridView_Schedule.Margin = new System.Windows.Forms.Padding(2);
            this.DataGridView_Schedule.Name = "DataGridView_Schedule";
            this.DataGridView_Schedule.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            this.DataGridView_Schedule.RowHeadersWidth = 20;
            this.DataGridView_Schedule.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.Color.LightGray;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.Color.Black;
            this.DataGridView_Schedule.RowsDefaultCellStyle = dataGridViewCellStyle3;
            this.DataGridView_Schedule.RowTemplate.Height = 24;
            this.DataGridView_Schedule.Size = new System.Drawing.Size(950, 400);
            this.DataGridView_Schedule.TabIndex = 23;
            this.DataGridView_Schedule.CellMouseClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.DataGridView_Schedule_CellMouseClick);
            this.DataGridView_Schedule.DataError += new System.Windows.Forms.DataGridViewDataErrorEventHandler(this.DataGridView1_DataError);
            this.DataGridView_Schedule.EditingControlShowing += new System.Windows.Forms.DataGridViewEditingControlShowingEventHandler(this.DataGridView1_EditingControlShowing);
            this.DataGridView_Schedule.RowHeaderMouseDoubleClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.DataGridView_Schedule_RowHeaderMouseDoubleClick);
            // 
            // Column1
            // 
            this.Column1.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.Column1.FillWeight = 120F;
            this.Column1.HeaderText = "Command";
            this.Column1.MaxDropDownItems = 20;
            this.Column1.MinimumWidth = 120;
            this.Column1.Name = "Column1";
            this.Column1.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.Column1.ToolTipText = "_cmd, _log1, _log2, _astro, _quantum, _dektec";
            // 
            // Column2
            // 
            this.Column2.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.Column2.FillWeight = 80F;
            this.Column2.HeaderText = ">Times >Keyword#";
            this.Column2.MaxInputLength = 32;
            this.Column2.MinimumWidth = 80;
            this.Column2.Name = "Column2";
            this.Column2.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.Column2.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // Column3
            // 
            this.Column3.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.Column3.FillWeight = 50F;
            this.Column3.HeaderText = "Interval";
            this.Column3.MinimumWidth = 50;
            this.Column3.Name = "Column3";
            this.Column3.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.Column3.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // Column4
            // 
            this.Column4.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.Column4.FillWeight = 70F;
            this.Column4.HeaderText = ">COM  >Pin";
            this.Column4.MinimumWidth = 70;
            this.Column4.Name = "Column4";
            this.Column4.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.Column4.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.Column4.Width = 72;
            // 
            // Column5
            // 
            this.Column5.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.Column5.HeaderText = "Function";
            this.Column5.MinimumWidth = 100;
            this.Column5.Name = "Column5";
            this.Column5.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.Column5.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // Column6
            // 
            this.Column6.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.Column6.HeaderText = "Sub-function";
            this.Column6.MinimumWidth = 100;
            this.Column6.Name = "Column6";
            this.Column6.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.Column6.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // Column7
            // 
            this.Column7.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.Column7.FillWeight = 150F;
            this.Column7.HeaderText = ">SerialPort                   >I/O cmd";
            this.Column7.MinimumWidth = 150;
            this.Column7.Name = "Column7";
            this.Column7.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.Column7.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // Column8
            // 
            this.Column8.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.Column8.FillWeight = 80F;
            this.Column8.HeaderText = "AC/USB Switch";
            this.Column8.MinimumWidth = 80;
            this.Column8.Name = "Column8";
            this.Column8.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.Column8.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // Column9
            // 
            this.Column9.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.Column9.FillWeight = 50F;
            this.Column9.HeaderText = "Wait";
            this.Column9.MinimumWidth = 50;
            this.Column9.Name = "Column9";
            this.Column9.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.Column9.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // Column10
            // 
            this.Column10.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.Column10.HeaderText = "Remark";
            this.Column10.MinimumWidth = 100;
            this.Column10.Name = "Column10";
            this.Column10.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.Column10.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(51)))), ((int)(((byte)(51)))));
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.ClientSize = new System.Drawing.Size(1079, 610);
            this.Controls.Add(this.label_Minolta);
            this.Controls.Add(this.pictureBox_Minolta);
            this.Controls.Add(this.button_savelog);
            this.Controls.Add(this.comboBox_savelog);
            this.Controls.Add(this.button_Start);
            this.Controls.Add(this.button_Analysis);
            this.Controls.Add(this.label_canbus);
            this.Controls.Add(this.pictureBox_canbus);
            this.Controls.Add(this.label_ext_board);
            this.Controls.Add(this.pictureBox_ext_board);
            this.Controls.Add(this.button_Network);
            this.Controls.Add(this.button_InsertRow);
            this.Controls.Add(this.labelGPIO_Input);
            this.Controls.Add(this.button_Output);
            this.Controls.Add(this.button_Input);
            this.Controls.Add(this.button_Schedule);
            this.Controls.Add(this.button_Schedule1);
            this.Controls.Add(this.pictureBox_BlueRat);
            this.Controls.Add(this.button_Camera);
            this.Controls.Add(this.label_AutoBox);
            this.Controls.Add(this.button_Schedule2);
            this.Controls.Add(this.TimeLabel);
            this.Controls.Add(this.button_Schedule3);
            this.Controls.Add(this.label_Camera);
            this.Controls.Add(this.button_Schedule4);
            this.Controls.Add(this.label_RedRat);
            this.Controls.Add(this.button_Schedule5);
            this.Controls.Add(this.button_TimeLine);
            this.Controls.Add(this.button_SaveSchedule);
            this.Controls.Add(this.DataBtn);
            this.Controls.Add(this.button_SerialPort);
            this.Controls.Add(this.MiniPicBox);
            this.Controls.Add(this.ClosePicBox);
            this.Controls.Add(this.label_Command);
            this.Controls.Add(this.label_Power);
            this.Controls.Add(this.pictureBox4);
            this.Controls.Add(this.pictureBox_AcPower);
            this.Controls.Add(this.pictureBox_Camera);
            this.Controls.Add(this.pictureBox_RedRat);
            this.Controls.Add(this.TimeLabel2);
            this.Controls.Add(this.button_Pause);
            this.Controls.Add(this.button_Setting);
            this.Controls.Add(this.panel_VirtualRC);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.panel_AcUsb);
            this.Controls.Add(this.DataGridView_Schedule);
            this.Controls.Add(this.textBox_serial);
            this.Controls.Add(this.comboBox_CameraDevice);
            this.Controls.Add(this.panelVideo);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(2);
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(1097, 655);
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "OPTT For Car";
            this.TopMost = true;
            this.MinimumSizeChanged += new System.EventHandler(this.MiniPicBox_Click);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Form1_FormClosed);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.Shown += new System.EventHandler(this.Form1_Shown);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.GPanelTitleBack_MouseDown);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel_VirtualRC.ResumeLayout(false);
            this.panel_VirtualRC.PerformLayout();
            this.panel_AcUsb.ResumeLayout(false);
            this.groupBox_Usb.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_Usb1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_Usb2)).EndInit();
            this.groupBox_Ac.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_Ac2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_Ac1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_canbus)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_ext_board)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_BlueRat)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.MiniPicBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ClosePicBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_AcPower)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_Camera)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_RedRat)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.panelVideo)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox5)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_Minolta)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.DataGridView_Schedule)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button_Setting;
        private System.Windows.Forms.TextBox textBox_serial;
        private System.Windows.Forms.PictureBox panelVideo;
        private System.Windows.Forms.Label label_Power;
        private System.Windows.Forms.PictureBox pictureBox_AcPower;
        private System.Windows.Forms.Label label_Camera;
        private System.Windows.Forms.PictureBox pictureBox_Camera;
        private System.Windows.Forms.Label label_RedRat;
        private System.Windows.Forms.PictureBox pictureBox_RedRat;
        private System.Windows.Forms.Label labellabel_LoopTimes_Value;
        private System.Windows.Forms.Label label_LoopNumber_Value;
        private System.IO.Ports.SerialPort PortA;
        private System.Windows.Forms.PictureBox pictureBox4;
        private System.IO.Ports.SerialPort PortB;
        private USBClassLibrary.USBClass USBPort;
        private USBClassLibrary.USBClass.DeviceProperties USBDeviceProperties;
        //bool MyUSBBoxDeviceConnected;
        bool MyUSBRedratDeviceConnected;
        bool MyUSBCameraDeviceConnected;
        private System.Windows.Forms.Timer timer_system;
        private System.Windows.Forms.PictureBox pictureBox5;
        private System.Windows.Forms.Label label_FwVersion;
        private System.Windows.Forms.Button button_SerialPort;
        private System.Windows.Forms.OpenFileDialog SchOpen1;
        private System.Windows.Forms.OpenFileDialog SchOpen2;
        private System.Windows.Forms.OpenFileDialog SchOpen3;
        private System.Windows.Forms.OpenFileDialog SchOpen4;
        private System.Windows.Forms.OpenFileDialog SchOpen5;
        private System.Windows.Forms.PictureBox ClosePicBox;
        private System.Windows.Forms.PictureBox MiniPicBox;
        protected internal System.Windows.Forms.Button button_SaveSchedule;
        protected internal System.Windows.Forms.Button button_Schedule1;
        protected internal System.Windows.Forms.Button button_Schedule2;
        protected internal System.Windows.Forms.Button button_Schedule3;
        protected internal System.Windows.Forms.Button button_Schedule4;
        protected internal System.Windows.Forms.Button button_Schedule5;
        private System.Windows.Forms.Button DataBtn;
        private System.Windows.Forms.SaveFileDialog saveFileDialog;
        private System.Windows.Forms.Button button_VirtualRC;
        private System.Windows.Forms.Label TimeLabel2;
        private System.Windows.Forms.Label labelSch1Timer;
        private System.Windows.Forms.Label labelSch2Timer;
        private System.Windows.Forms.Label labelSch5Timer;
        private System.Windows.Forms.Label labelSch4Timer;
        private System.Windows.Forms.Label labelSch3Timer;
        private System.Windows.Forms.Label label_ScheduleTime_Value;
        public System.Windows.Forms.Timer timer_countdown;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label SegLabel4;
        private System.Windows.Forms.Label SegLabel3;
        private System.Windows.Forms.Label SegLabel2;
        private System.Windows.Forms.Label SegLabel1;
        private System.Windows.Forms.Button button_TimeLine;
        private System.Windows.Forms.Label TimeLabel;
        private System.Windows.Forms.Button button_Schedule;
        private System.Windows.Forms.Button button_Camera;
        private System.Windows.Forms.Label label_LoopTimes;
        private System.Windows.Forms.Label label_LoopNumber;
        private System.Windows.Forms.Label label_ScheduleTime;
        private System.Windows.Forms.Label label_AutoBox;
        private System.Windows.Forms.PictureBox pictureBox_BlueRat;
        private System.IO.Ports.SerialPort serialPortWood;
        private System.Windows.Forms.Button button_Input;
        private System.Windows.Forms.Button button_Output;
        private System.Windows.Forms.Label label_Command;
        private System.Windows.Forms.Label labelGPIO_Input;
        private System.Windows.Forms.Label label_TestTime;
        private System.Windows.Forms.Label label_TestTime_Value;
        protected internal System.Windows.Forms.Button button_InsertRow;
        private System.Windows.Forms.Panel panel_VirtualRC;
        private System.Windows.Forms.Panel panel_AcUsb;
        private System.Windows.Forms.GroupBox groupBox_Ac;
        private System.Windows.Forms.GroupBox groupBox_Usb;
        private System.Windows.Forms.PictureBox pictureBox_Ac1;
        private System.Windows.Forms.Button button_AcUsb;
        private System.Windows.Forms.PictureBox pictureBox_Ac2;
        private System.Windows.Forms.Label label_Usb2;
        private System.Windows.Forms.Label label_Usb1;
        private System.Windows.Forms.PictureBox pictureBox_Usb1;
        private System.Windows.Forms.PictureBox pictureBox_Usb2;
        private System.Windows.Forms.Label label_Ac2;
        private System.Windows.Forms.Label label_Ac1;
        private System.IO.Ports.SerialPort PortC;
        private System.Windows.Forms.Button button_Network;
        private System.Windows.Forms.Label label_Remark;
        public SafeDataGridView DataGridView_Schedule;
        private System.Windows.Forms.Label label_ext_board;
        private System.Windows.Forms.PictureBox pictureBox_ext_board;
        private System.Windows.Forms.Timer timer_canbus;
        private System.Windows.Forms.Label label_canbus;
        private System.Windows.Forms.PictureBox pictureBox_canbus;
        private System.Windows.Forms.Button button_Analysis;
        private System.Windows.Forms.Timer timer_kline;
        private System.Windows.Forms.Button button_savelog;
        private System.IO.Ports.SerialPort PortD;
        private System.IO.Ports.SerialPort PortE;
        private System.Windows.Forms.ComboBox comboBox_savelog;
        private System.Windows.Forms.Label label_BoxVersion;
        protected internal System.Windows.Forms.Button button_Start;
        private System.Windows.Forms.DataGridViewComboBoxColumn Column1;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column2;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column3;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column4;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column5;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column6;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column7;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column8;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column9;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column10;
        protected internal System.Windows.Forms.Button button_Pause;
        private System.Windows.Forms.Timer timer_Chamber;
        private System.Windows.Forms.Timer timer_ca310;
        private System.Windows.Forms.Label label_Minolta;
        private System.Windows.Forms.PictureBox pictureBox_Minolta;
        protected internal System.Windows.Forms.ComboBox comboBox_CameraDevice;
        private System.Windows.Forms.Timer timer_duringShot;
    }
}

