namespace AutoTest
{
    partial class Setting
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
            this.button_Save = new System.Windows.Forms.Button();
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.folderBrowserDialog2 = new System.Windows.Forms.FolderBrowserDialog();
            this.openFileDialog2 = new System.Windows.Forms.OpenFileDialog();
            this.GroupBox_Rs232 = new System.Windows.Forms.GroupBox();
            this.checkBox_SerialPort3 = new System.Windows.Forms.CheckBox();
            this.comboBox_SerialPort3_PortName_Value = new System.Windows.Forms.ComboBox();
            this.comboBox_SerialPort3_BaudRate_Value = new System.Windows.Forms.ComboBox();
            this.label_SerialPort3_PortName = new System.Windows.Forms.Label();
            this.label_SerialPort3_BaudRate = new System.Windows.Forms.Label();
            this.checkBox_SerialPort2 = new System.Windows.Forms.CheckBox();
            this.checkBox_SerialPort1 = new System.Windows.Forms.CheckBox();
            this.comboBox_SerialPort1_BaudRate_Value = new System.Windows.Forms.ComboBox();
            this.comboBox_SerialPort1_PortName_Value = new System.Windows.Forms.ComboBox();
            this.label_SerialPort1_PortName = new System.Windows.Forms.Label();
            this.comboBox_SerialPort2_PortName_Value = new System.Windows.Forms.ComboBox();
            this.comboBox_SerialPort2_BaudRate_Value = new System.Windows.Forms.ComboBox();
            this.label_SerialPort2_PortName = new System.Windows.Forms.Label();
            this.label_SerialPort2_BaudRate = new System.Windows.Forms.Label();
            this.label_SerialPort1_BaudRate = new System.Windows.Forms.Label();
            this.button_RcDbPath = new System.Windows.Forms.Button();
            this.textBox_RcDbPath = new System.Windows.Forms.TextBox();
            this.textBox_LogPath = new System.Windows.Forms.TextBox();
            this.button_ImagePath = new System.Windows.Forms.Button();
            this.textBox_ImagePath = new System.Windows.Forms.TextBox();
            this.groupBox_Camera = new System.Windows.Forms.GroupBox();
            this.label_CameraDevice = new System.Windows.Forms.Label();
            this.comboBox_CameraDevice = new System.Windows.Forms.ComboBox();
            this.label_CameraAudio = new System.Windows.Forms.Label();
            this.comboBox_CameraAudio = new System.Windows.Forms.ComboBox();
            this.groupBox_RcDB = new System.Windows.Forms.GroupBox();
            this.label_TvBrands = new System.Windows.Forms.Label();
            this.comboBox_TvBrands = new System.Windows.Forms.ComboBox();
            this.label_SelectRedrat = new System.Windows.Forms.Label();
            this.comboBox__SelectRedrat = new System.Windows.Forms.ComboBox();
            this.button_LogPath = new System.Windows.Forms.Button();
            this.label_ErrorMessage = new System.Windows.Forms.Label();
            this.button_GeneratorPath = new System.Windows.Forms.Button();
            this.textBox_GeneratorPath = new System.Windows.Forms.TextBox();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.button_DosPath = new System.Windows.Forms.Button();
            this.textBox_DosPath = new System.Windows.Forms.TextBox();
            this.folderBrowserDialog3 = new System.Windows.Forms.FolderBrowserDialog();
            this.pictureBox_SerialPort2 = new System.Windows.Forms.PictureBox();
            this.pictureBox_SerialPort1 = new System.Windows.Forms.PictureBox();
            this.pictureBox_DosPath = new System.Windows.Forms.PictureBox();
            this.pictureBox_GeneratorPath = new System.Windows.Forms.PictureBox();
            this.pictureBox_RcDbPath = new System.Windows.Forms.PictureBox();
            this.pictureBox_LogPath = new System.Windows.Forms.PictureBox();
            this.pictureBox_ImagePath = new System.Windows.Forms.PictureBox();
            this.pictureBox_SerialPort3 = new System.Windows.Forms.PictureBox();
            this.checkBox_Displayhex = new System.Windows.Forms.CheckBox();
            this.GroupBox_Rs232.SuspendLayout();
            this.groupBox_Camera.SuspendLayout();
            this.groupBox_RcDB.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_SerialPort2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_SerialPort1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_DosPath)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_GeneratorPath)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_RcDbPath)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_LogPath)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_ImagePath)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_SerialPort3)).BeginInit();
            this.SuspendLayout();
            // 
            // button_Save
            // 
            this.button_Save.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.button_Save.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.button_Save.Font = new System.Drawing.Font("Microsoft Sans Serif", 16.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.button_Save.Location = new System.Drawing.Point(506, 440);
            this.button_Save.Margin = new System.Windows.Forms.Padding(2, 2, 24, 2);
            this.button_Save.Name = "button_Save";
            this.button_Save.Size = new System.Drawing.Size(78, 36);
            this.button_Save.TabIndex = 30;
            this.button_Save.Text = "SAVE";
            this.button_Save.UseVisualStyleBackColor = true;
            this.button_Save.Visible = false;
            this.button_Save.Click += new System.EventHandler(this.OkBtn_Click);
            // 
            // GroupBox_Rs232
            // 
            this.GroupBox_Rs232.BackColor = System.Drawing.Color.Transparent;
            this.GroupBox_Rs232.Controls.Add(this.checkBox_Displayhex);
            this.GroupBox_Rs232.Controls.Add(this.checkBox_SerialPort3);
            this.GroupBox_Rs232.Controls.Add(this.comboBox_SerialPort3_PortName_Value);
            this.GroupBox_Rs232.Controls.Add(this.comboBox_SerialPort3_BaudRate_Value);
            this.GroupBox_Rs232.Controls.Add(this.label_SerialPort3_PortName);
            this.GroupBox_Rs232.Controls.Add(this.label_SerialPort3_BaudRate);
            this.GroupBox_Rs232.Controls.Add(this.checkBox_SerialPort2);
            this.GroupBox_Rs232.Controls.Add(this.checkBox_SerialPort1);
            this.GroupBox_Rs232.Controls.Add(this.comboBox_SerialPort1_BaudRate_Value);
            this.GroupBox_Rs232.Controls.Add(this.comboBox_SerialPort1_PortName_Value);
            this.GroupBox_Rs232.Controls.Add(this.label_SerialPort1_PortName);
            this.GroupBox_Rs232.Controls.Add(this.comboBox_SerialPort2_PortName_Value);
            this.GroupBox_Rs232.Controls.Add(this.comboBox_SerialPort2_BaudRate_Value);
            this.GroupBox_Rs232.Controls.Add(this.label_SerialPort2_PortName);
            this.GroupBox_Rs232.Controls.Add(this.label_SerialPort2_BaudRate);
            this.GroupBox_Rs232.Controls.Add(this.label_SerialPort1_BaudRate);
            this.GroupBox_Rs232.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.GroupBox_Rs232.ForeColor = System.Drawing.Color.DarkOrange;
            this.GroupBox_Rs232.Location = new System.Drawing.Point(319, 182);
            this.GroupBox_Rs232.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.GroupBox_Rs232.Name = "GroupBox_Rs232";
            this.GroupBox_Rs232.Padding = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.GroupBox_Rs232.Size = new System.Drawing.Size(280, 245);
            this.GroupBox_Rs232.TabIndex = 37;
            this.GroupBox_Rs232.TabStop = false;
            this.GroupBox_Rs232.Text = "RS232";
            // 
            // checkBox_SerialPort3
            // 
            this.checkBox_SerialPort3.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.checkBox_SerialPort3.ForeColor = System.Drawing.Color.DeepSkyBlue;
            this.checkBox_SerialPort3.Location = new System.Drawing.Point(6, 167);
            this.checkBox_SerialPort3.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.checkBox_SerialPort3.Name = "checkBox_SerialPort3";
            this.checkBox_SerialPort3.Size = new System.Drawing.Size(222, 18);
            this.checkBox_SerialPort3.TabIndex = 63;
            this.checkBox_SerialPort3.Text = "SerialPort 3";
            this.checkBox_SerialPort3.UseVisualStyleBackColor = true;
            this.checkBox_SerialPort3.CheckedChanged += new System.EventHandler(this.checkBox_SerialPort3_CheckedChanged);
            // 
            // comboBox_SerialPort3_PortName_Value
            // 
            this.comboBox_SerialPort3_PortName_Value.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox_SerialPort3_PortName_Value.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.comboBox_SerialPort3_PortName_Value.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.comboBox_SerialPort3_PortName_Value.FormattingEnabled = true;
            this.comboBox_SerialPort3_PortName_Value.Items.AddRange(new object[] {
            "COM1",
            "COM2",
            "COM3",
            "COM4",
            "COM5",
            "COM6",
            "COM7",
            "COM8",
            "COM9",
            "COM10",
            "COM11",
            "COM12",
            "COM13",
            "COM14",
            "COM15",
            "COM16",
            "COM17",
            "COM18",
            "COM19",
            "COM20"});
            this.comboBox_SerialPort3_PortName_Value.Location = new System.Drawing.Point(126, 185);
            this.comboBox_SerialPort3_PortName_Value.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.comboBox_SerialPort3_PortName_Value.Name = "comboBox_SerialPort3_PortName_Value";
            this.comboBox_SerialPort3_PortName_Value.Size = new System.Drawing.Size(149, 23);
            this.comboBox_SerialPort3_PortName_Value.TabIndex = 62;
            this.comboBox_SerialPort3_PortName_Value.SelectedIndexChanged += new System.EventHandler(this.comboBox_SerialPort3_PortName_Value_SelectedIndexChanged);
            // 
            // comboBox_SerialPort3_BaudRate_Value
            // 
            this.comboBox_SerialPort3_BaudRate_Value.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox_SerialPort3_BaudRate_Value.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.comboBox_SerialPort3_BaudRate_Value.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.comboBox_SerialPort3_BaudRate_Value.FormattingEnabled = true;
            this.comboBox_SerialPort3_BaudRate_Value.Items.AddRange(new object[] {
            "110",
            "300",
            "600",
            "1200",
            "2400",
            "4800",
            "9600",
            "14400",
            "19200",
            "38400",
            "57600",
            "115200",
            "230400",
            "460800",
            "921600"});
            this.comboBox_SerialPort3_BaudRate_Value.Location = new System.Drawing.Point(126, 212);
            this.comboBox_SerialPort3_BaudRate_Value.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.comboBox_SerialPort3_BaudRate_Value.Name = "comboBox_SerialPort3_BaudRate_Value";
            this.comboBox_SerialPort3_BaudRate_Value.Size = new System.Drawing.Size(149, 23);
            this.comboBox_SerialPort3_BaudRate_Value.TabIndex = 60;
            this.comboBox_SerialPort3_BaudRate_Value.SelectedIndexChanged += new System.EventHandler(this.comboBox_SerialPort3_BaudRate_Value_SelectedIndexChanged);
            // 
            // label_SerialPort3_PortName
            // 
            this.label_SerialPort3_PortName.AutoSize = true;
            this.label_SerialPort3_PortName.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label_SerialPort3_PortName.ForeColor = System.Drawing.Color.Black;
            this.label_SerialPort3_PortName.Location = new System.Drawing.Point(55, 188);
            this.label_SerialPort3_PortName.Name = "label_SerialPort3_PortName";
            this.label_SerialPort3_PortName.Size = new System.Drawing.Size(63, 15);
            this.label_SerialPort3_PortName.TabIndex = 59;
            this.label_SerialPort3_PortName.Text = "PortName";
            // 
            // label_SerialPort3_BaudRate
            // 
            this.label_SerialPort3_BaudRate.AutoSize = true;
            this.label_SerialPort3_BaudRate.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label_SerialPort3_BaudRate.ForeColor = System.Drawing.Color.Black;
            this.label_SerialPort3_BaudRate.Location = new System.Drawing.Point(55, 214);
            this.label_SerialPort3_BaudRate.Name = "label_SerialPort3_BaudRate";
            this.label_SerialPort3_BaudRate.Size = new System.Drawing.Size(62, 15);
            this.label_SerialPort3_BaudRate.TabIndex = 61;
            this.label_SerialPort3_BaudRate.Text = "BaudRate";
            // 
            // checkBox_SerialPort2
            // 
            this.checkBox_SerialPort2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.checkBox_SerialPort2.ForeColor = System.Drawing.Color.DeepSkyBlue;
            this.checkBox_SerialPort2.Location = new System.Drawing.Point(6, 95);
            this.checkBox_SerialPort2.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.checkBox_SerialPort2.Name = "checkBox_SerialPort2";
            this.checkBox_SerialPort2.Size = new System.Drawing.Size(222, 18);
            this.checkBox_SerialPort2.TabIndex = 58;
            this.checkBox_SerialPort2.Text = "SerialPort 2 (SXP Command)";
            this.checkBox_SerialPort2.UseVisualStyleBackColor = true;
            this.checkBox_SerialPort2.CheckedChanged += new System.EventHandler(this.checkBox_SerialPort2_CheckedChanged);
            // 
            // checkBox_SerialPort1
            // 
            this.checkBox_SerialPort1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.checkBox_SerialPort1.ForeColor = System.Drawing.Color.DeepSkyBlue;
            this.checkBox_SerialPort1.Location = new System.Drawing.Point(6, 21);
            this.checkBox_SerialPort1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.checkBox_SerialPort1.Name = "checkBox_SerialPort1";
            this.checkBox_SerialPort1.Size = new System.Drawing.Size(258, 18);
            this.checkBox_SerialPort1.TabIndex = 57;
            this.checkBox_SerialPort1.Text = "SerialPort 1 (Astro and Quantum)";
            this.checkBox_SerialPort1.UseVisualStyleBackColor = true;
            this.checkBox_SerialPort1.CheckedChanged += new System.EventHandler(this.checkBox_SerialPort1_CheckedChanged);
            // 
            // comboBox_SerialPort1_BaudRate_Value
            // 
            this.comboBox_SerialPort1_BaudRate_Value.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox_SerialPort1_BaudRate_Value.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.comboBox_SerialPort1_BaudRate_Value.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.comboBox_SerialPort1_BaudRate_Value.FormattingEnabled = true;
            this.comboBox_SerialPort1_BaudRate_Value.Items.AddRange(new object[] {
            "110",
            "300",
            "600",
            "1200",
            "2400",
            "4800",
            "9600",
            "14400",
            "19200",
            "38400",
            "57600",
            "115200",
            "230400",
            "460800",
            "921600"});
            this.comboBox_SerialPort1_BaudRate_Value.Location = new System.Drawing.Point(126, 66);
            this.comboBox_SerialPort1_BaudRate_Value.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.comboBox_SerialPort1_BaudRate_Value.Name = "comboBox_SerialPort1_BaudRate_Value";
            this.comboBox_SerialPort1_BaudRate_Value.Size = new System.Drawing.Size(149, 23);
            this.comboBox_SerialPort1_BaudRate_Value.TabIndex = 56;
            this.comboBox_SerialPort1_BaudRate_Value.SelectedIndexChanged += new System.EventHandler(this.comboBox_SerialPort1_BaudRate_Value_SelectedIndexChanged);
            // 
            // comboBox_SerialPort1_PortName_Value
            // 
            this.comboBox_SerialPort1_PortName_Value.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox_SerialPort1_PortName_Value.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.comboBox_SerialPort1_PortName_Value.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.comboBox_SerialPort1_PortName_Value.FormattingEnabled = true;
            this.comboBox_SerialPort1_PortName_Value.Items.AddRange(new object[] {
            "COM1",
            "COM2",
            "COM3",
            "COM4",
            "COM5",
            "COM6",
            "COM7",
            "COM8",
            "COM9",
            "COM10",
            "COM11",
            "COM12",
            "COM13",
            "COM14",
            "COM15",
            "COM16",
            "COM17",
            "COM18",
            "COM19",
            "COM20"});
            this.comboBox_SerialPort1_PortName_Value.Location = new System.Drawing.Point(126, 39);
            this.comboBox_SerialPort1_PortName_Value.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.comboBox_SerialPort1_PortName_Value.Name = "comboBox_SerialPort1_PortName_Value";
            this.comboBox_SerialPort1_PortName_Value.Size = new System.Drawing.Size(149, 23);
            this.comboBox_SerialPort1_PortName_Value.TabIndex = 9;
            this.comboBox_SerialPort1_PortName_Value.SelectedIndexChanged += new System.EventHandler(this.comboBox_SerialPort1_PortName_Value_SelectedIndexChanged);
            // 
            // label_SerialPort1_PortName
            // 
            this.label_SerialPort1_PortName.AutoSize = true;
            this.label_SerialPort1_PortName.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label_SerialPort1_PortName.ForeColor = System.Drawing.Color.Black;
            this.label_SerialPort1_PortName.Location = new System.Drawing.Point(55, 42);
            this.label_SerialPort1_PortName.Name = "label_SerialPort1_PortName";
            this.label_SerialPort1_PortName.Size = new System.Drawing.Size(63, 15);
            this.label_SerialPort1_PortName.TabIndex = 8;
            this.label_SerialPort1_PortName.Text = "PortName";
            // 
            // comboBox_SerialPort2_PortName_Value
            // 
            this.comboBox_SerialPort2_PortName_Value.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox_SerialPort2_PortName_Value.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.comboBox_SerialPort2_PortName_Value.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.comboBox_SerialPort2_PortName_Value.FormattingEnabled = true;
            this.comboBox_SerialPort2_PortName_Value.Items.AddRange(new object[] {
            "COM1",
            "COM2",
            "COM3",
            "COM4",
            "COM5",
            "COM6",
            "COM7",
            "COM8",
            "COM9",
            "COM10",
            "COM11",
            "COM12",
            "COM13",
            "COM14",
            "COM15",
            "COM16",
            "COM17",
            "COM18",
            "COM19",
            "COM20"});
            this.comboBox_SerialPort2_PortName_Value.Location = new System.Drawing.Point(126, 113);
            this.comboBox_SerialPort2_PortName_Value.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.comboBox_SerialPort2_PortName_Value.Name = "comboBox_SerialPort2_PortName_Value";
            this.comboBox_SerialPort2_PortName_Value.Size = new System.Drawing.Size(149, 23);
            this.comboBox_SerialPort2_PortName_Value.TabIndex = 7;
            this.comboBox_SerialPort2_PortName_Value.SelectedIndexChanged += new System.EventHandler(this.comboBox_SerialPort2_PortName_Value_SelectedIndexChanged);
            // 
            // comboBox_SerialPort2_BaudRate_Value
            // 
            this.comboBox_SerialPort2_BaudRate_Value.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox_SerialPort2_BaudRate_Value.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.comboBox_SerialPort2_BaudRate_Value.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.comboBox_SerialPort2_BaudRate_Value.FormattingEnabled = true;
            this.comboBox_SerialPort2_BaudRate_Value.Items.AddRange(new object[] {
            "110",
            "300",
            "600",
            "1200",
            "2400",
            "4800",
            "9600",
            "14400",
            "19200",
            "38400",
            "57600",
            "115200",
            "230400",
            "460800",
            "921600"});
            this.comboBox_SerialPort2_BaudRate_Value.Location = new System.Drawing.Point(126, 140);
            this.comboBox_SerialPort2_BaudRate_Value.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.comboBox_SerialPort2_BaudRate_Value.Name = "comboBox_SerialPort2_BaudRate_Value";
            this.comboBox_SerialPort2_BaudRate_Value.Size = new System.Drawing.Size(149, 23);
            this.comboBox_SerialPort2_BaudRate_Value.TabIndex = 4;
            this.comboBox_SerialPort2_BaudRate_Value.SelectedIndexChanged += new System.EventHandler(this.comboBox_SerialPort2_BaudRate_Value_SelectedIndexChanged);
            // 
            // label_SerialPort2_PortName
            // 
            this.label_SerialPort2_PortName.AutoSize = true;
            this.label_SerialPort2_PortName.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label_SerialPort2_PortName.ForeColor = System.Drawing.Color.Black;
            this.label_SerialPort2_PortName.Location = new System.Drawing.Point(55, 116);
            this.label_SerialPort2_PortName.Name = "label_SerialPort2_PortName";
            this.label_SerialPort2_PortName.Size = new System.Drawing.Size(63, 15);
            this.label_SerialPort2_PortName.TabIndex = 3;
            this.label_SerialPort2_PortName.Text = "PortName";
            // 
            // label_SerialPort2_BaudRate
            // 
            this.label_SerialPort2_BaudRate.AutoSize = true;
            this.label_SerialPort2_BaudRate.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label_SerialPort2_BaudRate.ForeColor = System.Drawing.Color.Black;
            this.label_SerialPort2_BaudRate.Location = new System.Drawing.Point(55, 142);
            this.label_SerialPort2_BaudRate.Name = "label_SerialPort2_BaudRate";
            this.label_SerialPort2_BaudRate.Size = new System.Drawing.Size(62, 15);
            this.label_SerialPort2_BaudRate.TabIndex = 6;
            this.label_SerialPort2_BaudRate.Text = "BaudRate";
            // 
            // label_SerialPort1_BaudRate
            // 
            this.label_SerialPort1_BaudRate.AutoSize = true;
            this.label_SerialPort1_BaudRate.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label_SerialPort1_BaudRate.ForeColor = System.Drawing.Color.Black;
            this.label_SerialPort1_BaudRate.Location = new System.Drawing.Point(55, 68);
            this.label_SerialPort1_BaudRate.Name = "label_SerialPort1_BaudRate";
            this.label_SerialPort1_BaudRate.Size = new System.Drawing.Size(62, 15);
            this.label_SerialPort1_BaudRate.TabIndex = 2;
            this.label_SerialPort1_BaudRate.Text = "BaudRate";
            // 
            // button_RcDbPath
            // 
            this.button_RcDbPath.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.button_RcDbPath.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.button_RcDbPath.ForeColor = System.Drawing.Color.Black;
            this.button_RcDbPath.Location = new System.Drawing.Point(522, 76);
            this.button_RcDbPath.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.button_RcDbPath.Name = "button_RcDbPath";
            this.button_RcDbPath.Size = new System.Drawing.Size(106, 23);
            this.button_RcDbPath.TabIndex = 36;
            this.button_RcDbPath.Text = "RC DB Path";
            this.button_RcDbPath.UseVisualStyleBackColor = true;
            this.button_RcDbPath.Click += new System.EventHandler(this.button4_Click);
            // 
            // textBox_RcDbPath
            // 
            this.textBox_RcDbPath.AccessibleName = "";
            this.textBox_RcDbPath.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.textBox_RcDbPath.ForeColor = System.Drawing.Color.Black;
            this.textBox_RcDbPath.Location = new System.Drawing.Point(33, 76);
            this.textBox_RcDbPath.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.textBox_RcDbPath.Name = "textBox_RcDbPath";
            this.textBox_RcDbPath.Size = new System.Drawing.Size(454, 21);
            this.textBox_RcDbPath.TabIndex = 35;
            this.textBox_RcDbPath.TextChanged += new System.EventHandler(this.textBox_RcDbPath_TextChanged);
            // 
            // textBox_LogPath
            // 
            this.textBox_LogPath.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.textBox_LogPath.ForeColor = System.Drawing.Color.Black;
            this.textBox_LogPath.Location = new System.Drawing.Point(33, 43);
            this.textBox_LogPath.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.textBox_LogPath.Name = "textBox_LogPath";
            this.textBox_LogPath.Size = new System.Drawing.Size(454, 21);
            this.textBox_LogPath.TabIndex = 47;
            this.textBox_LogPath.TextChanged += new System.EventHandler(this.textBox_LogPath_TextChanged);
            // 
            // button_ImagePath
            // 
            this.button_ImagePath.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.button_ImagePath.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.button_ImagePath.ForeColor = System.Drawing.Color.Black;
            this.button_ImagePath.Location = new System.Drawing.Point(522, 11);
            this.button_ImagePath.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.button_ImagePath.Name = "button_ImagePath";
            this.button_ImagePath.Size = new System.Drawing.Size(106, 23);
            this.button_ImagePath.TabIndex = 29;
            this.button_ImagePath.Text = "Image Path";
            this.button_ImagePath.UseVisualStyleBackColor = true;
            this.button_ImagePath.Click += new System.EventHandler(this.button2_Click);
            // 
            // textBox_ImagePath
            // 
            this.textBox_ImagePath.BackColor = System.Drawing.Color.White;
            this.textBox_ImagePath.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.textBox_ImagePath.ForeColor = System.Drawing.Color.Black;
            this.textBox_ImagePath.Location = new System.Drawing.Point(33, 11);
            this.textBox_ImagePath.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.textBox_ImagePath.Name = "textBox_ImagePath";
            this.textBox_ImagePath.Size = new System.Drawing.Size(454, 21);
            this.textBox_ImagePath.TabIndex = 28;
            this.textBox_ImagePath.TextChanged += new System.EventHandler(this.textBox_ImagePath_TextChanged);
            // 
            // groupBox_Camera
            // 
            this.groupBox_Camera.BackColor = System.Drawing.Color.Transparent;
            this.groupBox_Camera.Controls.Add(this.label_CameraDevice);
            this.groupBox_Camera.Controls.Add(this.comboBox_CameraDevice);
            this.groupBox_Camera.Controls.Add(this.label_CameraAudio);
            this.groupBox_Camera.Controls.Add(this.comboBox_CameraAudio);
            this.groupBox_Camera.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.groupBox_Camera.ForeColor = System.Drawing.Color.DarkOrange;
            this.groupBox_Camera.Location = new System.Drawing.Point(33, 182);
            this.groupBox_Camera.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.groupBox_Camera.Name = "groupBox_Camera";
            this.groupBox_Camera.Padding = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.groupBox_Camera.Size = new System.Drawing.Size(280, 120);
            this.groupBox_Camera.TabIndex = 56;
            this.groupBox_Camera.TabStop = false;
            this.groupBox_Camera.Text = "CAMERA";
            // 
            // label_CameraDevice
            // 
            this.label_CameraDevice.AutoSize = true;
            this.label_CameraDevice.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label_CameraDevice.ForeColor = System.Drawing.Color.Black;
            this.label_CameraDevice.Location = new System.Drawing.Point(6, 18);
            this.label_CameraDevice.Name = "label_CameraDevice";
            this.label_CameraDevice.Size = new System.Drawing.Size(44, 15);
            this.label_CameraDevice.TabIndex = 42;
            this.label_CameraDevice.Text = "Device";
            // 
            // comboBox_CameraDevice
            // 
            this.comboBox_CameraDevice.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox_CameraDevice.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.comboBox_CameraDevice.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.comboBox_CameraDevice.FormattingEnabled = true;
            this.comboBox_CameraDevice.Location = new System.Drawing.Point(10, 36);
            this.comboBox_CameraDevice.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.comboBox_CameraDevice.Name = "comboBox_CameraDevice";
            this.comboBox_CameraDevice.Size = new System.Drawing.Size(265, 23);
            this.comboBox_CameraDevice.TabIndex = 43;
            this.comboBox_CameraDevice.DropDown += new System.EventHandler(this.AdjustWidthComboBox_DropDown);
            this.comboBox_CameraDevice.SelectedIndexChanged += new System.EventHandler(this.comboBox_CameraDevice_SelectedIndexChanged);
            // 
            // label_CameraAudio
            // 
            this.label_CameraAudio.AutoSize = true;
            this.label_CameraAudio.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label_CameraAudio.ForeColor = System.Drawing.Color.Black;
            this.label_CameraAudio.Location = new System.Drawing.Point(6, 68);
            this.label_CameraAudio.Name = "label_CameraAudio";
            this.label_CameraAudio.Size = new System.Drawing.Size(38, 15);
            this.label_CameraAudio.TabIndex = 49;
            this.label_CameraAudio.Text = "Audio";
            // 
            // comboBox_CameraAudio
            // 
            this.comboBox_CameraAudio.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox_CameraAudio.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.comboBox_CameraAudio.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.comboBox_CameraAudio.FormattingEnabled = true;
            this.comboBox_CameraAudio.Location = new System.Drawing.Point(10, 86);
            this.comboBox_CameraAudio.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.comboBox_CameraAudio.Name = "comboBox_CameraAudio";
            this.comboBox_CameraAudio.Size = new System.Drawing.Size(265, 23);
            this.comboBox_CameraAudio.TabIndex = 50;
            this.comboBox_CameraAudio.DropDown += new System.EventHandler(this.AdjustWidthComboBox_DropDown);
            this.comboBox_CameraAudio.SelectedIndexChanged += new System.EventHandler(this.comboBox_CameraAudio_SelectedIndexChanged);
            // 
            // groupBox_RcDB
            // 
            this.groupBox_RcDB.BackColor = System.Drawing.Color.Transparent;
            this.groupBox_RcDB.Controls.Add(this.label_TvBrands);
            this.groupBox_RcDB.Controls.Add(this.comboBox_TvBrands);
            this.groupBox_RcDB.Controls.Add(this.label_SelectRedrat);
            this.groupBox_RcDB.Controls.Add(this.comboBox__SelectRedrat);
            this.groupBox_RcDB.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.groupBox_RcDB.ForeColor = System.Drawing.Color.DarkOrange;
            this.groupBox_RcDB.Location = new System.Drawing.Point(33, 307);
            this.groupBox_RcDB.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.groupBox_RcDB.Name = "groupBox_RcDB";
            this.groupBox_RcDB.Padding = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.groupBox_RcDB.Size = new System.Drawing.Size(280, 120);
            this.groupBox_RcDB.TabIndex = 57;
            this.groupBox_RcDB.TabStop = false;
            this.groupBox_RcDB.Text = "RC DB";
            // 
            // label_TvBrands
            // 
            this.label_TvBrands.AutoSize = true;
            this.label_TvBrands.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label_TvBrands.ForeColor = System.Drawing.Color.Black;
            this.label_TvBrands.Location = new System.Drawing.Point(6, 18);
            this.label_TvBrands.Name = "label_TvBrands";
            this.label_TvBrands.Size = new System.Drawing.Size(63, 15);
            this.label_TvBrands.TabIndex = 40;
            this.label_TvBrands.Text = "TV Brands";
            // 
            // comboBox_TvBrands
            // 
            this.comboBox_TvBrands.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox_TvBrands.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.comboBox_TvBrands.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.comboBox_TvBrands.FormattingEnabled = true;
            this.comboBox_TvBrands.Location = new System.Drawing.Point(10, 36);
            this.comboBox_TvBrands.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.comboBox_TvBrands.Name = "comboBox_TvBrands";
            this.comboBox_TvBrands.Size = new System.Drawing.Size(265, 23);
            this.comboBox_TvBrands.TabIndex = 41;
            this.comboBox_TvBrands.DropDown += new System.EventHandler(this.AdjustWidthComboBox_DropDown);
            this.comboBox_TvBrands.SelectedIndexChanged += new System.EventHandler(this.comboBox_TvBrands_SelectedIndexChanged);
            // 
            // label_SelectRedrat
            // 
            this.label_SelectRedrat.AutoSize = true;
            this.label_SelectRedrat.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label_SelectRedrat.ForeColor = System.Drawing.Color.Black;
            this.label_SelectRedrat.Location = new System.Drawing.Point(6, 68);
            this.label_SelectRedrat.Name = "label_SelectRedrat";
            this.label_SelectRedrat.Size = new System.Drawing.Size(81, 15);
            this.label_SelectRedrat.TabIndex = 44;
            this.label_SelectRedrat.Text = "Select Redrat";
            // 
            // comboBox__SelectRedrat
            // 
            this.comboBox__SelectRedrat.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox__SelectRedrat.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.comboBox__SelectRedrat.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.comboBox__SelectRedrat.FormattingEnabled = true;
            this.comboBox__SelectRedrat.Location = new System.Drawing.Point(10, 86);
            this.comboBox__SelectRedrat.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.comboBox__SelectRedrat.Name = "comboBox__SelectRedrat";
            this.comboBox__SelectRedrat.Size = new System.Drawing.Size(265, 23);
            this.comboBox__SelectRedrat.TabIndex = 45;
            this.comboBox__SelectRedrat.DropDown += new System.EventHandler(this.AdjustWidthComboBox_DropDown);
            this.comboBox__SelectRedrat.SelectedIndexChanged += new System.EventHandler(this.comboBox__SelectRedrat_SelectedIndexChanged);
            // 
            // button_LogPath
            // 
            this.button_LogPath.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.button_LogPath.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.button_LogPath.ForeColor = System.Drawing.Color.Black;
            this.button_LogPath.Location = new System.Drawing.Point(522, 43);
            this.button_LogPath.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.button_LogPath.Name = "button_LogPath";
            this.button_LogPath.Size = new System.Drawing.Size(106, 23);
            this.button_LogPath.TabIndex = 48;
            this.button_LogPath.Text = "Log Path";
            this.button_LogPath.UseVisualStyleBackColor = true;
            this.button_LogPath.Click += new System.EventHandler(this.button5_Click);
            // 
            // label_ErrorMessage
            // 
            this.label_ErrorMessage.AutoSize = true;
            this.label_ErrorMessage.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.label_ErrorMessage.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label_ErrorMessage.ForeColor = System.Drawing.Color.Red;
            this.label_ErrorMessage.Location = new System.Drawing.Point(12, 446);
            this.label_ErrorMessage.Name = "label_ErrorMessage";
            this.label_ErrorMessage.Size = new System.Drawing.Size(83, 29);
            this.label_ErrorMessage.TabIndex = 83;
            this.label_ErrorMessage.Text = "~~~~~";
            this.label_ErrorMessage.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // button_GeneratorPath
            // 
            this.button_GeneratorPath.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.button_GeneratorPath.ForeColor = System.Drawing.Color.Black;
            this.button_GeneratorPath.Location = new System.Drawing.Point(522, 110);
            this.button_GeneratorPath.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.button_GeneratorPath.Name = "button_GeneratorPath";
            this.button_GeneratorPath.Size = new System.Drawing.Size(106, 23);
            this.button_GeneratorPath.TabIndex = 87;
            this.button_GeneratorPath.Text = "Generator Path";
            this.button_GeneratorPath.UseVisualStyleBackColor = true;
            this.button_GeneratorPath.Click += new System.EventHandler(this.button3_Click);
            // 
            // textBox_GeneratorPath
            // 
            this.textBox_GeneratorPath.AccessibleName = "";
            this.textBox_GeneratorPath.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.textBox_GeneratorPath.ForeColor = System.Drawing.Color.Black;
            this.textBox_GeneratorPath.Location = new System.Drawing.Point(33, 110);
            this.textBox_GeneratorPath.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.textBox_GeneratorPath.Name = "textBox_GeneratorPath";
            this.textBox_GeneratorPath.Size = new System.Drawing.Size(454, 21);
            this.textBox_GeneratorPath.TabIndex = 86;
            this.textBox_GeneratorPath.TextChanged += new System.EventHandler(this.textBox_GeneratorPath_TextChanged);
            // 
            // button_DosPath
            // 
            this.button_DosPath.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.button_DosPath.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.button_DosPath.ForeColor = System.Drawing.Color.Black;
            this.button_DosPath.Location = new System.Drawing.Point(522, 143);
            this.button_DosPath.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.button_DosPath.Name = "button_DosPath";
            this.button_DosPath.Size = new System.Drawing.Size(106, 23);
            this.button_DosPath.TabIndex = 89;
            this.button_DosPath.Text = "DOS Path";
            this.button_DosPath.UseVisualStyleBackColor = true;
            this.button_DosPath.Click += new System.EventHandler(this.buttonDosWorkingDirectory_Click);
            // 
            // textBox_DosPath
            // 
            this.textBox_DosPath.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.textBox_DosPath.ForeColor = System.Drawing.Color.Black;
            this.textBox_DosPath.Location = new System.Drawing.Point(33, 143);
            this.textBox_DosPath.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.textBox_DosPath.Name = "textBox_DosPath";
            this.textBox_DosPath.Size = new System.Drawing.Size(454, 21);
            this.textBox_DosPath.TabIndex = 88;
            this.textBox_DosPath.TextChanged += new System.EventHandler(this.textBox_DosPath_TextChanged);
            // 
            // pictureBox_SerialPort2
            // 
            this.pictureBox_SerialPort2.BackColor = System.Drawing.Color.Transparent;
            this.pictureBox_SerialPort2.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.pictureBox_SerialPort2.Location = new System.Drawing.Point(605, 295);
            this.pictureBox_SerialPort2.Margin = new System.Windows.Forms.Padding(2);
            this.pictureBox_SerialPort2.Name = "pictureBox_SerialPort2";
            this.pictureBox_SerialPort2.Size = new System.Drawing.Size(23, 23);
            this.pictureBox_SerialPort2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox_SerialPort2.TabIndex = 96;
            this.pictureBox_SerialPort2.TabStop = false;
            // 
            // pictureBox_SerialPort1
            // 
            this.pictureBox_SerialPort1.BackColor = System.Drawing.Color.Transparent;
            this.pictureBox_SerialPort1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.pictureBox_SerialPort1.Location = new System.Drawing.Point(605, 221);
            this.pictureBox_SerialPort1.Margin = new System.Windows.Forms.Padding(2);
            this.pictureBox_SerialPort1.Name = "pictureBox_SerialPort1";
            this.pictureBox_SerialPort1.Size = new System.Drawing.Size(23, 23);
            this.pictureBox_SerialPort1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox_SerialPort1.TabIndex = 95;
            this.pictureBox_SerialPort1.TabStop = false;
            // 
            // pictureBox_DosPath
            // 
            this.pictureBox_DosPath.BackColor = System.Drawing.Color.Transparent;
            this.pictureBox_DosPath.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.pictureBox_DosPath.Location = new System.Drawing.Point(493, 143);
            this.pictureBox_DosPath.Margin = new System.Windows.Forms.Padding(2);
            this.pictureBox_DosPath.Name = "pictureBox_DosPath";
            this.pictureBox_DosPath.Size = new System.Drawing.Size(23, 23);
            this.pictureBox_DosPath.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox_DosPath.TabIndex = 94;
            this.pictureBox_DosPath.TabStop = false;
            // 
            // pictureBox_GeneratorPath
            // 
            this.pictureBox_GeneratorPath.BackColor = System.Drawing.Color.Transparent;
            this.pictureBox_GeneratorPath.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.pictureBox_GeneratorPath.Location = new System.Drawing.Point(493, 110);
            this.pictureBox_GeneratorPath.Margin = new System.Windows.Forms.Padding(2);
            this.pictureBox_GeneratorPath.Name = "pictureBox_GeneratorPath";
            this.pictureBox_GeneratorPath.Size = new System.Drawing.Size(23, 23);
            this.pictureBox_GeneratorPath.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox_GeneratorPath.TabIndex = 93;
            this.pictureBox_GeneratorPath.TabStop = false;
            // 
            // pictureBox_RcDbPath
            // 
            this.pictureBox_RcDbPath.BackColor = System.Drawing.Color.Transparent;
            this.pictureBox_RcDbPath.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.pictureBox_RcDbPath.Location = new System.Drawing.Point(493, 76);
            this.pictureBox_RcDbPath.Margin = new System.Windows.Forms.Padding(2);
            this.pictureBox_RcDbPath.Name = "pictureBox_RcDbPath";
            this.pictureBox_RcDbPath.Size = new System.Drawing.Size(23, 23);
            this.pictureBox_RcDbPath.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox_RcDbPath.TabIndex = 92;
            this.pictureBox_RcDbPath.TabStop = false;
            // 
            // pictureBox_LogPath
            // 
            this.pictureBox_LogPath.BackColor = System.Drawing.Color.Transparent;
            this.pictureBox_LogPath.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.pictureBox_LogPath.Location = new System.Drawing.Point(493, 43);
            this.pictureBox_LogPath.Margin = new System.Windows.Forms.Padding(2);
            this.pictureBox_LogPath.Name = "pictureBox_LogPath";
            this.pictureBox_LogPath.Size = new System.Drawing.Size(23, 23);
            this.pictureBox_LogPath.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox_LogPath.TabIndex = 91;
            this.pictureBox_LogPath.TabStop = false;
            // 
            // pictureBox_ImagePath
            // 
            this.pictureBox_ImagePath.BackColor = System.Drawing.Color.Transparent;
            this.pictureBox_ImagePath.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.pictureBox_ImagePath.Location = new System.Drawing.Point(493, 11);
            this.pictureBox_ImagePath.Margin = new System.Windows.Forms.Padding(2);
            this.pictureBox_ImagePath.Name = "pictureBox_ImagePath";
            this.pictureBox_ImagePath.Size = new System.Drawing.Size(23, 23);
            this.pictureBox_ImagePath.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox_ImagePath.TabIndex = 90;
            this.pictureBox_ImagePath.TabStop = false;
            // 
            // pictureBox_SerialPort3
            // 
            this.pictureBox_SerialPort3.BackColor = System.Drawing.Color.Transparent;
            this.pictureBox_SerialPort3.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.pictureBox_SerialPort3.Location = new System.Drawing.Point(605, 367);
            this.pictureBox_SerialPort3.Margin = new System.Windows.Forms.Padding(2);
            this.pictureBox_SerialPort3.Name = "pictureBox_SerialPort3";
            this.pictureBox_SerialPort3.Size = new System.Drawing.Size(23, 23);
            this.pictureBox_SerialPort3.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox_SerialPort3.TabIndex = 97;
            this.pictureBox_SerialPort3.TabStop = false;
            // 
            // checkBox_Displayhex
            // 
            this.checkBox_Displayhex.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.checkBox_Displayhex.ForeColor = System.Drawing.Color.DeepSkyBlue;
            this.checkBox_Displayhex.Location = new System.Drawing.Point(188, 95);
            this.checkBox_Displayhex.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.checkBox_Displayhex.Name = "checkBox_Displayhex";
            this.checkBox_Displayhex.Size = new System.Drawing.Size(50, 18);
            this.checkBox_Displayhex.TabIndex = 64;
            this.checkBox_Displayhex.Text = "Hex";
            this.checkBox_Displayhex.UseVisualStyleBackColor = true;
            this.checkBox_Displayhex.CheckedChanged += new System.EventHandler(this.checkBox_Displayhex_CheckedChanged);
            // 
            // Setting
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.Color.Ivory;
            this.ClientSize = new System.Drawing.Size(658, 494);
            this.ControlBox = false;
            this.Controls.Add(this.pictureBox_SerialPort3);
            this.Controls.Add(this.pictureBox_SerialPort2);
            this.Controls.Add(this.pictureBox_SerialPort1);
            this.Controls.Add(this.pictureBox_DosPath);
            this.Controls.Add(this.pictureBox_GeneratorPath);
            this.Controls.Add(this.pictureBox_RcDbPath);
            this.Controls.Add(this.pictureBox_LogPath);
            this.Controls.Add(this.pictureBox_ImagePath);
            this.Controls.Add(this.button_DosPath);
            this.Controls.Add(this.textBox_DosPath);
            this.Controls.Add(this.button_GeneratorPath);
            this.Controls.Add(this.textBox_GeneratorPath);
            this.Controls.Add(this.label_ErrorMessage);
            this.Controls.Add(this.button_LogPath);
            this.Controls.Add(this.groupBox_RcDB);
            this.Controls.Add(this.button_Save);
            this.Controls.Add(this.groupBox_Camera);
            this.Controls.Add(this.button_ImagePath);
            this.Controls.Add(this.textBox_ImagePath);
            this.Controls.Add(this.GroupBox_Rs232);
            this.Controls.Add(this.textBox_LogPath);
            this.Controls.Add(this.button_RcDbPath);
            this.Controls.Add(this.textBox_RcDbPath);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Setting";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Setting";
            this.Load += new System.EventHandler(this.Setting_Load);
            this.GroupBox_Rs232.ResumeLayout(false);
            this.GroupBox_Rs232.PerformLayout();
            this.groupBox_Camera.ResumeLayout(false);
            this.groupBox_Camera.PerformLayout();
            this.groupBox_RcDB.ResumeLayout(false);
            this.groupBox_RcDB.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_SerialPort2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_SerialPort1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_DosPath)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_GeneratorPath)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_RcDbPath)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_LogPath)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_ImagePath)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_SerialPort3)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button_Save;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog2;
        private System.Windows.Forms.OpenFileDialog openFileDialog2;
        private System.Windows.Forms.CheckBox checkBox_SerialPort2;
        private System.Windows.Forms.CheckBox checkBox_SerialPort1;
        internal System.Windows.Forms.ComboBox comboBox_SerialPort1_BaudRate_Value;
        internal System.Windows.Forms.ComboBox comboBox_SerialPort1_PortName_Value;
        internal System.Windows.Forms.Label label_SerialPort1_PortName;
        internal System.Windows.Forms.ComboBox comboBox_SerialPort2_PortName_Value;
        private System.Windows.Forms.ComboBox comboBox_SerialPort2_BaudRate_Value;
        internal System.Windows.Forms.Label label_SerialPort2_PortName;
        internal System.Windows.Forms.Label label_SerialPort2_BaudRate;
        internal System.Windows.Forms.Label label_SerialPort1_BaudRate;
        private System.Windows.Forms.Button button_RcDbPath;
        private System.Windows.Forms.TextBox textBox_RcDbPath;
        private System.Windows.Forms.TextBox textBox_LogPath;
        private System.Windows.Forms.Button button_ImagePath;
        private System.Windows.Forms.TextBox textBox_ImagePath;
        private System.Windows.Forms.GroupBox groupBox_Camera;
        private System.Windows.Forms.Label label_CameraDevice;
        private System.Windows.Forms.ComboBox comboBox_CameraDevice;
        private System.Windows.Forms.Label label_CameraAudio;
        private System.Windows.Forms.ComboBox comboBox_CameraAudio;
        private System.Windows.Forms.GroupBox groupBox_RcDB;
        private System.Windows.Forms.Label label_TvBrands;
        private System.Windows.Forms.ComboBox comboBox_TvBrands;
        private System.Windows.Forms.Label label_SelectRedrat;
        private System.Windows.Forms.ComboBox comboBox__SelectRedrat;
        private System.Windows.Forms.Button button_LogPath;
        private System.Windows.Forms.Label label_ErrorMessage;
        private System.Windows.Forms.Button button_GeneratorPath;
        private System.Windows.Forms.TextBox textBox_GeneratorPath;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.GroupBox GroupBox_Rs232;
        private System.Windows.Forms.Button button_DosPath;
        private System.Windows.Forms.TextBox textBox_DosPath;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog3;
        private System.Windows.Forms.PictureBox pictureBox_ImagePath;
        private System.Windows.Forms.PictureBox pictureBox_LogPath;
        private System.Windows.Forms.PictureBox pictureBox_RcDbPath;
        private System.Windows.Forms.PictureBox pictureBox_GeneratorPath;
        private System.Windows.Forms.PictureBox pictureBox_DosPath;
        private System.Windows.Forms.PictureBox pictureBox_SerialPort1;
        private System.Windows.Forms.PictureBox pictureBox_SerialPort2;
        private System.Windows.Forms.CheckBox checkBox_SerialPort3;
        internal System.Windows.Forms.ComboBox comboBox_SerialPort3_PortName_Value;
        private System.Windows.Forms.ComboBox comboBox_SerialPort3_BaudRate_Value;
        internal System.Windows.Forms.Label label_SerialPort3_PortName;
        internal System.Windows.Forms.Label label_SerialPort3_BaudRate;
        private System.Windows.Forms.PictureBox pictureBox_SerialPort3;
        private System.Windows.Forms.CheckBox checkBox_Displayhex;
    }
}