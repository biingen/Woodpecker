namespace AutoTest
{
    partial class FormSchedule
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
            this.RecVideo = new System.Windows.Forms.CheckBox();
            this.SchLooplabel5 = new System.Windows.Forms.Label();
            this.SchLooplabel4 = new System.Windows.Forms.Label();
            this.SchLooplabel3 = new System.Windows.Forms.Label();
            this.SchLooplabel2 = new System.Windows.Forms.Label();
            this.SchLooplabel1 = new System.Windows.Forms.Label();
            this.LoopBox5 = new System.Windows.Forms.TextBox();
            this.LoopBox4 = new System.Windows.Forms.TextBox();
            this.LoopBox3 = new System.Windows.Forms.TextBox();
            this.LoopBox2 = new System.Windows.Forms.TextBox();
            this.LoopBox1 = new System.Windows.Forms.TextBox();
            this.SchBox5 = new System.Windows.Forms.TextBox();
            this.LoadSchBtn5 = new System.Windows.Forms.Button();
            this.SchBox4 = new System.Windows.Forms.TextBox();
            this.LoadSchBtn4 = new System.Windows.Forms.Button();
            this.SchBox3 = new System.Windows.Forms.TextBox();
            this.LoadSchBtn3 = new System.Windows.Forms.Button();
            this.SaveSchBtn = new System.Windows.Forms.Button();
            this.SchBox2 = new System.Windows.Forms.TextBox();
            this.SchBox1 = new System.Windows.Forms.TextBox();
            this.LoadSchBtn2 = new System.Windows.Forms.Button();
            this.LoadSchBtn1 = new System.Windows.Forms.Button();
            this.SchOpen1 = new System.Windows.Forms.OpenFileDialog();
            this.SchOpen2 = new System.Windows.Forms.OpenFileDialog();
            this.SchOpen3 = new System.Windows.Forms.OpenFileDialog();
            this.SchOpen4 = new System.Windows.Forms.OpenFileDialog();
            this.SchOpen5 = new System.Windows.Forms.OpenFileDialog();
            this.SavedLabel = new System.Windows.Forms.Label();
            this.SchCheckBox2 = new System.Windows.Forms.CheckBox();
            this.SchCheckBox3 = new System.Windows.Forms.CheckBox();
            this.SchCheckBox4 = new System.Windows.Forms.CheckBox();
            this.SchCheckBox5 = new System.Windows.Forms.CheckBox();
            this.CompareCheckBox = new System.Windows.Forms.CheckBox();
            this.saveFileDialog = new System.Windows.Forms.SaveFileDialog();
            this.DifferenceBox = new System.Windows.Forms.ComboBox();
            this.checkBoxTimer1 = new System.Windows.Forms.CheckBox();
            this.dateTimePickerSch1 = new System.Windows.Forms.DateTimePicker();
            this.checkBoxTimer2 = new System.Windows.Forms.CheckBox();
            this.dateTimePickerSch2 = new System.Windows.Forms.DateTimePicker();
            this.checkBoxTimer3 = new System.Windows.Forms.CheckBox();
            this.dateTimePickerSch3 = new System.Windows.Forms.DateTimePicker();
            this.checkBoxTimer4 = new System.Windows.Forms.CheckBox();
            this.checkBoxTimer5 = new System.Windows.Forms.CheckBox();
            this.dateTimePickerSch4 = new System.Windows.Forms.DateTimePicker();
            this.dateTimePickerSch5 = new System.Windows.Forms.DateTimePicker();
            this.SuspendLayout();
            // 
            // RecVideo
            // 
            this.RecVideo.AutoSize = true;
            this.RecVideo.Font = new System.Drawing.Font("MS Reference Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.RecVideo.ForeColor = System.Drawing.Color.DeepSkyBlue;
            this.RecVideo.Location = new System.Drawing.Point(13, 544);
            this.RecVideo.Name = "RecVideo";
            this.RecVideo.Size = new System.Drawing.Size(301, 20);
            this.RecVideo.TabIndex = 81;
            this.RecVideo.Text = "video record after each schedule finished";
            this.RecVideo.UseVisualStyleBackColor = true;
            this.RecVideo.CheckedChanged += new System.EventHandler(this.RecVideo_CheckedChanged);
            // 
            // SchLooplabel5
            // 
            this.SchLooplabel5.AutoSize = true;
            this.SchLooplabel5.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.SchLooplabel5.Font = new System.Drawing.Font("MS Reference Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.SchLooplabel5.ForeColor = System.Drawing.Color.Crimson;
            this.SchLooplabel5.Location = new System.Drawing.Point(315, 451);
            this.SchLooplabel5.Name = "SchLooplabel5";
            this.SchLooplabel5.Size = new System.Drawing.Size(39, 16);
            this.SchLooplabel5.TabIndex = 80;
            this.SchLooplabel5.Text = "Loop";
            // 
            // SchLooplabel4
            // 
            this.SchLooplabel4.AutoSize = true;
            this.SchLooplabel4.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.SchLooplabel4.Font = new System.Drawing.Font("MS Reference Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.SchLooplabel4.ForeColor = System.Drawing.Color.Crimson;
            this.SchLooplabel4.Location = new System.Drawing.Point(315, 356);
            this.SchLooplabel4.Name = "SchLooplabel4";
            this.SchLooplabel4.Size = new System.Drawing.Size(39, 16);
            this.SchLooplabel4.TabIndex = 79;
            this.SchLooplabel4.Text = "Loop";
            // 
            // SchLooplabel3
            // 
            this.SchLooplabel3.AutoSize = true;
            this.SchLooplabel3.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.SchLooplabel3.Font = new System.Drawing.Font("MS Reference Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.SchLooplabel3.ForeColor = System.Drawing.Color.Crimson;
            this.SchLooplabel3.Location = new System.Drawing.Point(315, 261);
            this.SchLooplabel3.Name = "SchLooplabel3";
            this.SchLooplabel3.Size = new System.Drawing.Size(39, 16);
            this.SchLooplabel3.TabIndex = 78;
            this.SchLooplabel3.Text = "Loop";
            // 
            // SchLooplabel2
            // 
            this.SchLooplabel2.AutoSize = true;
            this.SchLooplabel2.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.SchLooplabel2.Font = new System.Drawing.Font("MS Reference Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.SchLooplabel2.ForeColor = System.Drawing.Color.Crimson;
            this.SchLooplabel2.Location = new System.Drawing.Point(315, 166);
            this.SchLooplabel2.Name = "SchLooplabel2";
            this.SchLooplabel2.Size = new System.Drawing.Size(39, 16);
            this.SchLooplabel2.TabIndex = 77;
            this.SchLooplabel2.Text = "Loop";
            // 
            // SchLooplabel1
            // 
            this.SchLooplabel1.AutoSize = true;
            this.SchLooplabel1.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.SchLooplabel1.Font = new System.Drawing.Font("MS Reference Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.SchLooplabel1.ForeColor = System.Drawing.Color.Crimson;
            this.SchLooplabel1.Location = new System.Drawing.Point(315, 71);
            this.SchLooplabel1.Name = "SchLooplabel1";
            this.SchLooplabel1.Size = new System.Drawing.Size(39, 16);
            this.SchLooplabel1.TabIndex = 76;
            this.SchLooplabel1.Text = "Loop";
            // 
            // LoopBox5
            // 
            this.LoopBox5.Font = new System.Drawing.Font("MS Reference Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LoopBox5.ForeColor = System.Drawing.Color.Crimson;
            this.LoopBox5.Location = new System.Drawing.Point(360, 445);
            this.LoopBox5.MaxLength = 6;
            this.LoopBox5.Name = "LoopBox5";
            this.LoopBox5.Size = new System.Drawing.Size(65, 22);
            this.LoopBox5.TabIndex = 75;
            this.LoopBox5.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // LoopBox4
            // 
            this.LoopBox4.Font = new System.Drawing.Font("MS Reference Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LoopBox4.ForeColor = System.Drawing.Color.Crimson;
            this.LoopBox4.Location = new System.Drawing.Point(360, 350);
            this.LoopBox4.MaxLength = 6;
            this.LoopBox4.Name = "LoopBox4";
            this.LoopBox4.Size = new System.Drawing.Size(65, 22);
            this.LoopBox4.TabIndex = 74;
            this.LoopBox4.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // LoopBox3
            // 
            this.LoopBox3.Font = new System.Drawing.Font("MS Reference Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LoopBox3.ForeColor = System.Drawing.Color.Crimson;
            this.LoopBox3.Location = new System.Drawing.Point(360, 255);
            this.LoopBox3.MaxLength = 6;
            this.LoopBox3.Name = "LoopBox3";
            this.LoopBox3.Size = new System.Drawing.Size(65, 22);
            this.LoopBox3.TabIndex = 73;
            this.LoopBox3.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // LoopBox2
            // 
            this.LoopBox2.Font = new System.Drawing.Font("MS Reference Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LoopBox2.ForeColor = System.Drawing.Color.Crimson;
            this.LoopBox2.Location = new System.Drawing.Point(360, 160);
            this.LoopBox2.MaxLength = 6;
            this.LoopBox2.Name = "LoopBox2";
            this.LoopBox2.Size = new System.Drawing.Size(65, 22);
            this.LoopBox2.TabIndex = 72;
            this.LoopBox2.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // LoopBox1
            // 
            this.LoopBox1.Font = new System.Drawing.Font("MS Reference Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LoopBox1.ForeColor = System.Drawing.Color.Crimson;
            this.LoopBox1.Location = new System.Drawing.Point(360, 65);
            this.LoopBox1.MaxLength = 6;
            this.LoopBox1.Name = "LoopBox1";
            this.LoopBox1.Size = new System.Drawing.Size(65, 22);
            this.LoopBox1.TabIndex = 71;
            this.LoopBox1.Tag = "";
            this.LoopBox1.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // SchBox5
            // 
            this.SchBox5.Font = new System.Drawing.Font("MS Reference Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.SchBox5.Location = new System.Drawing.Point(35, 424);
            this.SchBox5.Name = "SchBox5";
            this.SchBox5.Size = new System.Drawing.Size(390, 22);
            this.SchBox5.TabIndex = 70;
            this.SchBox5.Text = "--------------------";
            // 
            // LoadSchBtn5
            // 
            this.LoadSchBtn5.Font = new System.Drawing.Font("MS Reference Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LoadSchBtn5.Location = new System.Drawing.Point(35, 395);
            this.LoadSchBtn5.Name = "LoadSchBtn5";
            this.LoadSchBtn5.Size = new System.Drawing.Size(85, 23);
            this.LoadSchBtn5.TabIndex = 69;
            this.LoadSchBtn5.Text = "Schedule5";
            this.LoadSchBtn5.UseVisualStyleBackColor = true;
            this.LoadSchBtn5.Click += new System.EventHandler(this.LoadSchBtn5_Click);
            // 
            // SchBox4
            // 
            this.SchBox4.Font = new System.Drawing.Font("MS Reference Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.SchBox4.Location = new System.Drawing.Point(35, 329);
            this.SchBox4.Name = "SchBox4";
            this.SchBox4.Size = new System.Drawing.Size(390, 22);
            this.SchBox4.TabIndex = 68;
            this.SchBox4.Text = "--------------------";
            // 
            // LoadSchBtn4
            // 
            this.LoadSchBtn4.Font = new System.Drawing.Font("MS Reference Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LoadSchBtn4.Location = new System.Drawing.Point(35, 300);
            this.LoadSchBtn4.Name = "LoadSchBtn4";
            this.LoadSchBtn4.Size = new System.Drawing.Size(85, 23);
            this.LoadSchBtn4.TabIndex = 67;
            this.LoadSchBtn4.Text = "Schedule4";
            this.LoadSchBtn4.UseVisualStyleBackColor = true;
            this.LoadSchBtn4.Click += new System.EventHandler(this.LoadSchBtn4_Click);
            // 
            // SchBox3
            // 
            this.SchBox3.Font = new System.Drawing.Font("MS Reference Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.SchBox3.Location = new System.Drawing.Point(35, 234);
            this.SchBox3.Name = "SchBox3";
            this.SchBox3.Size = new System.Drawing.Size(390, 22);
            this.SchBox3.TabIndex = 66;
            this.SchBox3.Text = "--------------------";
            // 
            // LoadSchBtn3
            // 
            this.LoadSchBtn3.Font = new System.Drawing.Font("MS Reference Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LoadSchBtn3.Location = new System.Drawing.Point(35, 205);
            this.LoadSchBtn3.Name = "LoadSchBtn3";
            this.LoadSchBtn3.Size = new System.Drawing.Size(85, 23);
            this.LoadSchBtn3.TabIndex = 65;
            this.LoadSchBtn3.Text = "Schedule3";
            this.LoadSchBtn3.UseVisualStyleBackColor = true;
            this.LoadSchBtn3.Click += new System.EventHandler(this.LoadSchBtn3_Click);
            // 
            // SaveSchBtn
            // 
            this.SaveSchBtn.Font = new System.Drawing.Font("MS Reference Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.SaveSchBtn.Location = new System.Drawing.Point(350, 592);
            this.SaveSchBtn.Name = "SaveSchBtn";
            this.SaveSchBtn.Size = new System.Drawing.Size(85, 30);
            this.SaveSchBtn.TabIndex = 64;
            this.SaveSchBtn.Text = "SAVE";
            this.SaveSchBtn.UseVisualStyleBackColor = true;
            this.SaveSchBtn.Click += new System.EventHandler(this.SaveSchBtn_Click);
            // 
            // SchBox2
            // 
            this.SchBox2.Font = new System.Drawing.Font("MS Reference Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.SchBox2.Location = new System.Drawing.Point(35, 139);
            this.SchBox2.Name = "SchBox2";
            this.SchBox2.Size = new System.Drawing.Size(390, 22);
            this.SchBox2.TabIndex = 63;
            this.SchBox2.Text = "--------------------";
            // 
            // SchBox1
            // 
            this.SchBox1.Font = new System.Drawing.Font("MS Reference Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.SchBox1.Location = new System.Drawing.Point(35, 44);
            this.SchBox1.Name = "SchBox1";
            this.SchBox1.Size = new System.Drawing.Size(390, 22);
            this.SchBox1.TabIndex = 62;
            this.SchBox1.Text = "--------------------";
            // 
            // LoadSchBtn2
            // 
            this.LoadSchBtn2.Font = new System.Drawing.Font("MS Reference Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LoadSchBtn2.Location = new System.Drawing.Point(35, 110);
            this.LoadSchBtn2.Name = "LoadSchBtn2";
            this.LoadSchBtn2.Size = new System.Drawing.Size(85, 23);
            this.LoadSchBtn2.TabIndex = 61;
            this.LoadSchBtn2.Text = "Schedule2";
            this.LoadSchBtn2.UseVisualStyleBackColor = true;
            this.LoadSchBtn2.Click += new System.EventHandler(this.LoadSchBtn2_Click);
            // 
            // LoadSchBtn1
            // 
            this.LoadSchBtn1.Font = new System.Drawing.Font("MS Reference Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LoadSchBtn1.Location = new System.Drawing.Point(35, 15);
            this.LoadSchBtn1.Name = "LoadSchBtn1";
            this.LoadSchBtn1.Size = new System.Drawing.Size(85, 23);
            this.LoadSchBtn1.TabIndex = 60;
            this.LoadSchBtn1.Text = "Schedule1";
            this.LoadSchBtn1.UseVisualStyleBackColor = true;
            this.LoadSchBtn1.Click += new System.EventHandler(this.LoadSchBtn1_Click);
            // 
            // SchOpen1
            // 
            this.SchOpen1.FileName = "SchOpen1";
            // 
            // SchOpen2
            // 
            this.SchOpen2.FileName = "SchOpen2";
            // 
            // SchOpen3
            // 
            this.SchOpen3.FileName = "SchOpen3";
            // 
            // SchOpen4
            // 
            this.SchOpen4.FileName = "SchOpen4";
            // 
            // SchOpen5
            // 
            this.SchOpen5.FileName = "SchOpen5";
            // 
            // SavedLabel
            // 
            this.SavedLabel.AutoSize = true;
            this.SavedLabel.Font = new System.Drawing.Font("MS Reference Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.SavedLabel.ForeColor = System.Drawing.Color.Red;
            this.SavedLabel.Location = new System.Drawing.Point(11, 590);
            this.SavedLabel.Name = "SavedLabel";
            this.SavedLabel.Size = new System.Drawing.Size(0, 24);
            this.SavedLabel.TabIndex = 82;
            // 
            // SchCheckBox2
            // 
            this.SchCheckBox2.AutoSize = true;
            this.SchCheckBox2.Location = new System.Drawing.Point(12, 115);
            this.SchCheckBox2.Name = "SchCheckBox2";
            this.SchCheckBox2.Size = new System.Drawing.Size(15, 14);
            this.SchCheckBox2.TabIndex = 87;
            this.SchCheckBox2.UseVisualStyleBackColor = true;
            this.SchCheckBox2.CheckedChanged += new System.EventHandler(this.SchCheckBox2_CheckedChanged);
            // 
            // SchCheckBox3
            // 
            this.SchCheckBox3.AutoSize = true;
            this.SchCheckBox3.Location = new System.Drawing.Point(12, 210);
            this.SchCheckBox3.Name = "SchCheckBox3";
            this.SchCheckBox3.Size = new System.Drawing.Size(15, 14);
            this.SchCheckBox3.TabIndex = 88;
            this.SchCheckBox3.UseVisualStyleBackColor = true;
            this.SchCheckBox3.CheckedChanged += new System.EventHandler(this.SchCheckBox3_CheckedChanged);
            // 
            // SchCheckBox4
            // 
            this.SchCheckBox4.AutoSize = true;
            this.SchCheckBox4.Location = new System.Drawing.Point(12, 305);
            this.SchCheckBox4.Name = "SchCheckBox4";
            this.SchCheckBox4.Size = new System.Drawing.Size(15, 14);
            this.SchCheckBox4.TabIndex = 89;
            this.SchCheckBox4.UseVisualStyleBackColor = true;
            this.SchCheckBox4.CheckedChanged += new System.EventHandler(this.SchCheckBox4_CheckedChanged);
            // 
            // SchCheckBox5
            // 
            this.SchCheckBox5.AutoSize = true;
            this.SchCheckBox5.Location = new System.Drawing.Point(12, 400);
            this.SchCheckBox5.Name = "SchCheckBox5";
            this.SchCheckBox5.Size = new System.Drawing.Size(15, 14);
            this.SchCheckBox5.TabIndex = 90;
            this.SchCheckBox5.UseVisualStyleBackColor = true;
            this.SchCheckBox5.CheckedChanged += new System.EventHandler(this.SchCheckBox5_CheckedChanged);
            // 
            // CompareCheckBox
            // 
            this.CompareCheckBox.AutoSize = true;
            this.CompareCheckBox.Location = new System.Drawing.Point(13, 518);
            this.CompareCheckBox.Name = "CompareCheckBox";
            this.CompareCheckBox.Size = new System.Drawing.Size(78, 17);
            this.CompareCheckBox.TabIndex = 91;
            this.CompareCheckBox.Text = "Similarity：";
            this.CompareCheckBox.UseVisualStyleBackColor = true;
            this.CompareCheckBox.CheckedChanged += new System.EventHandler(this.CompareBox_CheckedChanged);
            // 
            // DifferenceBox
            // 
            this.DifferenceBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.DifferenceBox.Font = new System.Drawing.Font("MS Reference Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.DifferenceBox.FormattingEnabled = true;
            this.DifferenceBox.Items.AddRange(new object[] {
            "100%",
            "90%",
            "80%",
            "70%",
            "60%",
            "50%",
            "40%",
            "30%",
            "20%",
            "10%",
            "0%"});
            this.DifferenceBox.Location = new System.Drawing.Point(99, 514);
            this.DifferenceBox.Name = "DifferenceBox";
            this.DifferenceBox.Size = new System.Drawing.Size(64, 24);
            this.DifferenceBox.TabIndex = 94;
            this.DifferenceBox.SelectedIndexChanged += new System.EventHandler(this.DifferenceBox_SelectedIndexChanged);
            // 
            // checkBoxTimer1
            // 
            this.checkBoxTimer1.AutoSize = true;
            this.checkBoxTimer1.BackColor = System.Drawing.Color.Transparent;
            this.checkBoxTimer1.Font = new System.Drawing.Font("MS Reference Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.checkBoxTimer1.ForeColor = System.Drawing.Color.OrangeRed;
            this.checkBoxTimer1.Location = new System.Drawing.Point(264, 22);
            this.checkBoxTimer1.Name = "checkBoxTimer1";
            this.checkBoxTimer1.Size = new System.Drawing.Size(15, 14);
            this.checkBoxTimer1.TabIndex = 94;
            this.checkBoxTimer1.UseVisualStyleBackColor = false;
            this.checkBoxTimer1.CheckedChanged += new System.EventHandler(this.checkBoxTimer1_CheckedChanged);
            // 
            // dateTimePickerSch1
            // 
            this.dateTimePickerSch1.CustomFormat = "yyyy-MM-dd  HH:mm";
            this.dateTimePickerSch1.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dateTimePickerSch1.Location = new System.Drawing.Point(285, 18);
            this.dateTimePickerSch1.MinDate = new System.DateTime(2014, 12, 1, 0, 0, 0, 0);
            this.dateTimePickerSch1.Name = "dateTimePickerSch1";
            this.dateTimePickerSch1.Size = new System.Drawing.Size(140, 20);
            this.dateTimePickerSch1.TabIndex = 108;
            this.dateTimePickerSch1.Value = new System.DateTime(2014, 12, 8, 0, 0, 0, 0);
            // 
            // checkBoxTimer2
            // 
            this.checkBoxTimer2.AutoSize = true;
            this.checkBoxTimer2.BackColor = System.Drawing.Color.Transparent;
            this.checkBoxTimer2.Font = new System.Drawing.Font("MS Reference Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.checkBoxTimer2.ForeColor = System.Drawing.Color.OrangeRed;
            this.checkBoxTimer2.Location = new System.Drawing.Point(264, 115);
            this.checkBoxTimer2.Name = "checkBoxTimer2";
            this.checkBoxTimer2.Size = new System.Drawing.Size(15, 14);
            this.checkBoxTimer2.TabIndex = 109;
            this.checkBoxTimer2.UseVisualStyleBackColor = false;
            this.checkBoxTimer2.CheckedChanged += new System.EventHandler(this.checkBoxTimer2_CheckedChanged);
            // 
            // dateTimePickerSch2
            // 
            this.dateTimePickerSch2.CustomFormat = "yyyy-MM-dd  HH:mm";
            this.dateTimePickerSch2.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dateTimePickerSch2.Location = new System.Drawing.Point(285, 111);
            this.dateTimePickerSch2.MinDate = new System.DateTime(2014, 12, 1, 0, 0, 0, 0);
            this.dateTimePickerSch2.Name = "dateTimePickerSch2";
            this.dateTimePickerSch2.Size = new System.Drawing.Size(140, 20);
            this.dateTimePickerSch2.TabIndex = 110;
            this.dateTimePickerSch2.Value = new System.DateTime(2014, 12, 8, 0, 0, 0, 0);
            // 
            // checkBoxTimer3
            // 
            this.checkBoxTimer3.AutoSize = true;
            this.checkBoxTimer3.BackColor = System.Drawing.Color.Transparent;
            this.checkBoxTimer3.Font = new System.Drawing.Font("MS Reference Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.checkBoxTimer3.ForeColor = System.Drawing.Color.OrangeRed;
            this.checkBoxTimer3.Location = new System.Drawing.Point(264, 210);
            this.checkBoxTimer3.Name = "checkBoxTimer3";
            this.checkBoxTimer3.Size = new System.Drawing.Size(15, 14);
            this.checkBoxTimer3.TabIndex = 111;
            this.checkBoxTimer3.UseVisualStyleBackColor = false;
            this.checkBoxTimer3.CheckedChanged += new System.EventHandler(this.checkBoxTimer3_CheckedChanged);
            // 
            // dateTimePickerSch3
            // 
            this.dateTimePickerSch3.CustomFormat = "yyyy-MM-dd  HH:mm";
            this.dateTimePickerSch3.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dateTimePickerSch3.Location = new System.Drawing.Point(285, 206);
            this.dateTimePickerSch3.MinDate = new System.DateTime(2014, 12, 1, 0, 0, 0, 0);
            this.dateTimePickerSch3.Name = "dateTimePickerSch3";
            this.dateTimePickerSch3.Size = new System.Drawing.Size(140, 20);
            this.dateTimePickerSch3.TabIndex = 112;
            this.dateTimePickerSch3.Value = new System.DateTime(2014, 12, 8, 0, 0, 0, 0);
            // 
            // checkBoxTimer4
            // 
            this.checkBoxTimer4.AutoSize = true;
            this.checkBoxTimer4.BackColor = System.Drawing.Color.Transparent;
            this.checkBoxTimer4.Font = new System.Drawing.Font("MS Reference Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.checkBoxTimer4.ForeColor = System.Drawing.Color.OrangeRed;
            this.checkBoxTimer4.Location = new System.Drawing.Point(264, 305);
            this.checkBoxTimer4.Name = "checkBoxTimer4";
            this.checkBoxTimer4.Size = new System.Drawing.Size(15, 14);
            this.checkBoxTimer4.TabIndex = 113;
            this.checkBoxTimer4.UseVisualStyleBackColor = false;
            this.checkBoxTimer4.CheckedChanged += new System.EventHandler(this.checkBoxTimer4_CheckedChanged);
            // 
            // checkBoxTimer5
            // 
            this.checkBoxTimer5.AutoSize = true;
            this.checkBoxTimer5.BackColor = System.Drawing.Color.Transparent;
            this.checkBoxTimer5.Font = new System.Drawing.Font("MS Reference Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.checkBoxTimer5.ForeColor = System.Drawing.Color.OrangeRed;
            this.checkBoxTimer5.Location = new System.Drawing.Point(264, 400);
            this.checkBoxTimer5.Name = "checkBoxTimer5";
            this.checkBoxTimer5.Size = new System.Drawing.Size(15, 14);
            this.checkBoxTimer5.TabIndex = 114;
            this.checkBoxTimer5.UseVisualStyleBackColor = false;
            this.checkBoxTimer5.CheckedChanged += new System.EventHandler(this.checkBoxTimer5_CheckedChanged);
            // 
            // dateTimePickerSch4
            // 
            this.dateTimePickerSch4.CustomFormat = "yyyy-MM-dd  HH:mm";
            this.dateTimePickerSch4.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dateTimePickerSch4.Location = new System.Drawing.Point(285, 301);
            this.dateTimePickerSch4.MinDate = new System.DateTime(2014, 12, 1, 0, 0, 0, 0);
            this.dateTimePickerSch4.Name = "dateTimePickerSch4";
            this.dateTimePickerSch4.Size = new System.Drawing.Size(140, 20);
            this.dateTimePickerSch4.TabIndex = 115;
            this.dateTimePickerSch4.Value = new System.DateTime(2014, 12, 8, 0, 0, 0, 0);
            // 
            // dateTimePickerSch5
            // 
            this.dateTimePickerSch5.CustomFormat = "yyyy-MM-dd  HH:mm";
            this.dateTimePickerSch5.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dateTimePickerSch5.Location = new System.Drawing.Point(285, 396);
            this.dateTimePickerSch5.MinDate = new System.DateTime(2014, 12, 1, 0, 0, 0, 0);
            this.dateTimePickerSch5.Name = "dateTimePickerSch5";
            this.dateTimePickerSch5.Size = new System.Drawing.Size(140, 20);
            this.dateTimePickerSch5.TabIndex = 116;
            this.dateTimePickerSch5.Value = new System.DateTime(2014, 12, 8, 0, 0, 0, 0);
            // 
            // FormSchedule
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.AutoValidate = System.Windows.Forms.AutoValidate.Disable;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(455, 665);
            this.Controls.Add(this.dateTimePickerSch5);
            this.Controls.Add(this.dateTimePickerSch4);
            this.Controls.Add(this.checkBoxTimer5);
            this.Controls.Add(this.checkBoxTimer4);
            this.Controls.Add(this.dateTimePickerSch3);
            this.Controls.Add(this.checkBoxTimer3);
            this.Controls.Add(this.dateTimePickerSch2);
            this.Controls.Add(this.checkBoxTimer2);
            this.Controls.Add(this.dateTimePickerSch1);
            this.Controls.Add(this.checkBoxTimer1);
            this.Controls.Add(this.DifferenceBox);
            this.Controls.Add(this.CompareCheckBox);
            this.Controls.Add(this.SchCheckBox5);
            this.Controls.Add(this.SchCheckBox4);
            this.Controls.Add(this.SchCheckBox3);
            this.Controls.Add(this.SchCheckBox2);
            this.Controls.Add(this.SavedLabel);
            this.Controls.Add(this.RecVideo);
            this.Controls.Add(this.SchLooplabel5);
            this.Controls.Add(this.SchLooplabel4);
            this.Controls.Add(this.SchLooplabel3);
            this.Controls.Add(this.SchLooplabel2);
            this.Controls.Add(this.SchLooplabel1);
            this.Controls.Add(this.LoopBox5);
            this.Controls.Add(this.LoopBox4);
            this.Controls.Add(this.LoopBox3);
            this.Controls.Add(this.LoopBox2);
            this.Controls.Add(this.LoopBox1);
            this.Controls.Add(this.SchBox5);
            this.Controls.Add(this.LoadSchBtn5);
            this.Controls.Add(this.SchBox4);
            this.Controls.Add(this.LoadSchBtn4);
            this.Controls.Add(this.SchBox3);
            this.Controls.Add(this.LoadSchBtn3);
            this.Controls.Add(this.SaveSchBtn);
            this.Controls.Add(this.SchBox2);
            this.Controls.Add(this.SchBox1);
            this.Controls.Add(this.LoadSchBtn2);
            this.Controls.Add(this.LoadSchBtn1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Location = new System.Drawing.Point(200, 0);
            this.Name = "FormSchedule";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "FormSchedule";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox RecVideo;
        private System.Windows.Forms.Label SchLooplabel5;
        private System.Windows.Forms.Label SchLooplabel4;
        private System.Windows.Forms.Label SchLooplabel3;
        private System.Windows.Forms.Label SchLooplabel2;
        private System.Windows.Forms.Label SchLooplabel1;
        private System.Windows.Forms.TextBox LoopBox5;
        private System.Windows.Forms.TextBox LoopBox4;
        private System.Windows.Forms.TextBox LoopBox3;
        private System.Windows.Forms.TextBox LoopBox2;
        private System.Windows.Forms.TextBox LoopBox1;
        private System.Windows.Forms.TextBox SchBox5;
        private System.Windows.Forms.Button LoadSchBtn5;
        private System.Windows.Forms.TextBox SchBox4;
        private System.Windows.Forms.Button LoadSchBtn4;
        private System.Windows.Forms.TextBox SchBox3;
        private System.Windows.Forms.Button LoadSchBtn3;
        private System.Windows.Forms.Button SaveSchBtn;
        private System.Windows.Forms.TextBox SchBox2;
        private System.Windows.Forms.TextBox SchBox1;
        private System.Windows.Forms.Button LoadSchBtn2;
        private System.Windows.Forms.Button LoadSchBtn1;
        private System.Windows.Forms.OpenFileDialog SchOpen1;
        private System.Windows.Forms.OpenFileDialog SchOpen2;
        private System.Windows.Forms.OpenFileDialog SchOpen3;
        private System.Windows.Forms.OpenFileDialog SchOpen4;
        private System.Windows.Forms.OpenFileDialog SchOpen5;
        private System.Windows.Forms.Label SavedLabel;
        private System.Windows.Forms.CheckBox SchCheckBox2;
        private System.Windows.Forms.CheckBox SchCheckBox3;
        private System.Windows.Forms.CheckBox SchCheckBox4;
        private System.Windows.Forms.CheckBox SchCheckBox5;
        private System.Windows.Forms.CheckBox CompareCheckBox;
        private System.Windows.Forms.SaveFileDialog saveFileDialog;
        internal System.Windows.Forms.ComboBox DifferenceBox;
        private System.Windows.Forms.CheckBox checkBoxTimer1;
        private System.Windows.Forms.DateTimePicker dateTimePickerSch1;
        private System.Windows.Forms.CheckBox checkBoxTimer2;
        private System.Windows.Forms.DateTimePicker dateTimePickerSch2;
        private System.Windows.Forms.CheckBox checkBoxTimer3;
        private System.Windows.Forms.DateTimePicker dateTimePickerSch3;
        private System.Windows.Forms.CheckBox checkBoxTimer4;
        private System.Windows.Forms.CheckBox checkBoxTimer5;
        private System.Windows.Forms.DateTimePicker dateTimePickerSch4;
        private System.Windows.Forms.DateTimePicker dateTimePickerSch5;
    }
}