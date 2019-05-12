using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using jini;
using System.IO;

namespace AutoTest
{
    public partial class FormSchedule : Form
    {
        Form1 Form1 = new Form1();

        string sPath = Application.StartupPath + "\\Config.ini";
        string MailPath = Application.StartupPath + "\\Mail.ini";

        public FormSchedule()
        {
            InitializeComponent();

            Form1.StartBtn.Enabled = false;
            Form1.WriteBtn.Enabled = false;

            SchBox1.Text = ini12.INIRead(sPath, "Schedule1", "Path", "");
            SchBox2.Text = ini12.INIRead(sPath, "Schedule2", "Path", "");
            SchBox3.Text = ini12.INIRead(sPath, "Schedule3", "Path", "");
            SchBox4.Text = ini12.INIRead(sPath, "Schedule4", "Path", "");
            SchBox5.Text = ini12.INIRead(sPath, "Schedule5", "Path", "");

            LoopBox1.Text = ini12.INIRead(sPath, "Schedule1 Loop", "value", "");
            LoopBox2.Text = ini12.INIRead(sPath, "Schedule2 Loop", "value", "");
            LoopBox3.Text = ini12.INIRead(sPath, "Schedule3 Loop", "value", "");
            LoopBox4.Text = ini12.INIRead(sPath, "Schedule4 Loop", "value", "");
            LoopBox5.Text = ini12.INIRead(sPath, "Schedule5 Loop", "value", "");

            string differentnum = ini12.INIRead(sPath, "ComparePIC", "DifferentNum", "");
            if (differentnum == "100")
            {
                DifferenceBox.Text = "0%";
            }
            else if (differentnum == "90")
            {
                DifferenceBox.Text = "10%";
            }
            else if (differentnum == "80")
            {
                DifferenceBox.Text = "20%";
            }
            else if (differentnum == "70")
            {
                DifferenceBox.Text = "30%";
            }
            else if (differentnum == "60")
            {
                DifferenceBox.Text = "40%";
            }
            else if (differentnum == "50")
            {
                DifferenceBox.Text = "50%";
            }
            else if (differentnum == "40")
            {
                DifferenceBox.Text = "60%";
            }
            else if (differentnum == "30")
            {
                DifferenceBox.Text = "70%";
            }
            else if (differentnum == "20")
            {
                DifferenceBox.Text = "80%";
            }
            else if (differentnum == "10")
            {
                DifferenceBox.Text = "90%";
            }
            else
            {
                DifferenceBox.Text = "100%";
            }

            if (int.Parse(ini12.INIRead(sPath, "RecVideo", "value", "")) == 1)
                RecVideo.Checked = true;
            else
                RecVideo.Checked = false;

            if (int.Parse(ini12.INIRead(sPath, "Schedule2 Exist", "value", "")) == 1)
                SchCheckBox2.Checked = true;
            else
            {
                SchCheckBox2.Checked = false;
                LoadSchBtn2.Enabled = false;
                SchBox2.Enabled = false;
                LoopBox2.Enabled = false;
                checkBoxTimer2.Enabled = false;
            }

            if (int.Parse(ini12.INIRead(sPath, "Schedule3 Exist", "value", "")) == 1)
                SchCheckBox3.Checked = true;
            else
            {
                SchCheckBox3.Checked = false;
                LoadSchBtn3.Enabled = false;
                SchBox3.Enabled = false;
                LoopBox3.Enabled = false;
                checkBoxTimer3.Enabled = false;
            }

            if (int.Parse(ini12.INIRead(sPath, "Schedule4 Exist", "value", "")) == 1)
                SchCheckBox4.Checked = true;
            else
            {
                SchCheckBox4.Checked = false;
                LoadSchBtn4.Enabled = false;
                SchBox4.Enabled = false;
                LoopBox4.Enabled = false;
                checkBoxTimer4.Enabled = false;
            }

            if (int.Parse(ini12.INIRead(sPath, "Schedule5 Exist", "value", "")) == 1)
                SchCheckBox5.Checked = true;
            else
            {
                SchCheckBox5.Checked = false;
                LoadSchBtn5.Enabled = false;
                SchBox5.Enabled = false;
                LoopBox5.Enabled = false;
                checkBoxTimer5.Enabled = false;
            }

            if (int.Parse(ini12.INIRead(sPath, "ComparePIC", "value", "")) == 1)
            {
                CompareCheckBox.Checked = true;
                DifferenceBox.Enabled = true;
            }
            else
            {
                CompareCheckBox.Checked = false;
                DifferenceBox.Enabled = false;
            }

            #region Timer
            dateTimePickerSch1.Text = ini12.INIRead(sPath, "Schedule1", "Timer", "");       //Schedule1 Timer
            if (int.Parse(ini12.INIRead(sPath, "Schedule1", "On Time Start", "")) == 1)
            {
                checkBoxTimer1.Checked = true;
                dateTimePickerSch1.Enabled = true;
            }
            else
            {
                checkBoxTimer1.Checked = false;
                dateTimePickerSch1.Enabled = false;
            }
            dateTimePickerSch2.Text = ini12.INIRead(sPath, "Schedule2", "Timer", "");       //Schedule2 Timer
            if (int.Parse(ini12.INIRead(sPath, "Schedule2", "On Time Start", "")) == 1)
            {
                checkBoxTimer2.Checked = true;
                dateTimePickerSch2.Enabled = true;
            }
            else
            {
                checkBoxTimer2.Checked = false;
                dateTimePickerSch2.Enabled = false;
            }
            dateTimePickerSch3.Text = ini12.INIRead(sPath, "Schedule3", "Timer", "");       //Schedule3 Timer
            if (int.Parse(ini12.INIRead(sPath, "Schedule3", "On Time Start", "")) == 1)
            {
                checkBoxTimer3.Checked = true;
                dateTimePickerSch3.Enabled = true;
            }
            else
            {
                checkBoxTimer3.Checked = false;
                dateTimePickerSch3.Enabled = false;
            }
            dateTimePickerSch4.Text = ini12.INIRead(sPath, "Schedule4", "Timer", "");       //Schedule4 Timer
            if (int.Parse(ini12.INIRead(sPath, "Schedule4", "On Time Start", "")) == 1)
            {
                checkBoxTimer4.Checked = true;
                dateTimePickerSch4.Enabled = true;
            }
            else
            {
                checkBoxTimer4.Checked = false;
                dateTimePickerSch4.Enabled = false;
            }
            dateTimePickerSch5.Text = ini12.INIRead(sPath, "Schedule5", "Timer", "");       //Schedule5 Timer
            if (int.Parse(ini12.INIRead(sPath, "Schedule5", "On Time Start", "")) == 1)
            {
                checkBoxTimer5.Checked = true;
                dateTimePickerSch5.Enabled = true;
            }
            else
            {
                checkBoxTimer5.Checked = false;
                dateTimePickerSch5.Enabled = false;
            }
            #endregion
        }

        #region LoadSchBtn
        private void LoadSchBtn1_Click(object sender, EventArgs e)      // Load Schedule1 Path
        {
            SchOpen1.Filter = "CSV files (*.csv)|*.CSV";
            SchOpen1.ShowDialog();
            if (SchOpen1.FileName == "SchOpen1")
                SchBox1.Text = SchBox1.Text;
            else
                SchBox1.Text = SchOpen1.FileName;
        }
        private void LoadSchBtn2_Click(object sender, EventArgs e)      // Load Schedule2 Path
        {
            SchOpen2.Filter = "CSV files (*.csv)|*.CSV";
            SchOpen2.ShowDialog();
            if (SchOpen2.FileName == "SchOpen2")
                SchBox2.Text = SchBox2.Text;
            else
                SchBox2.Text = SchOpen2.FileName;
        }
        private void LoadSchBtn3_Click(object sender, EventArgs e)      // Load Schedule3 Path
        {
            SchOpen3.Filter = "CSV files (*.csv)|*.CSV";
            SchOpen3.ShowDialog();
            if (SchOpen3.FileName == "SchOpen3")
                SchBox3.Text = SchBox3.Text;
            else
                SchBox3.Text = SchOpen3.FileName;
        }
        private void LoadSchBtn4_Click(object sender, EventArgs e)      // Load Schedule4 Path
        {
            SchOpen4.Filter = "CSV files (*.csv)|*.CSV";
            SchOpen4.ShowDialog();
            if (SchOpen4.FileName == "SchOpen4")
                SchBox4.Text = SchBox4.Text;
            else
                SchBox4.Text = SchOpen4.FileName;
        }
        private void LoadSchBtn5_Click(object sender, EventArgs e)      // Load Schedule5 Path
        {
            SchOpen5.Filter = "CSV files (*.csv)|*.CSV";
            SchOpen5.ShowDialog();
            if (SchOpen5.FileName == "SchOpen5")
                SchBox5.Text = SchBox5.Text;
            else
                SchBox5.Text = SchOpen5.FileName;
        }
        #endregion

        private void RecVideo_CheckedChanged(object sender, EventArgs e)       //勾選此CheckBox打開測試後錄影功能
        {
            if (RecVideo.Checked == true)
                ini12.INIWrite(sPath, "RecVideo", "value", "1");        // 寫入Import DadaBase啟動
            else
                ini12.INIWrite(sPath, "RecVideo", "value", "0");        // 寫入Import DadaBase關閉
        }

        public void SaveSchBtn_Click(object sender, EventArgs e)
        {
            if (System.IO.File.Exists(SchBox1.Text.Trim()))
            {
                ini12.INIWrite(sPath, "Schedule1", "Path", SchBox1.Text.Trim());
                ini12.INIWrite(MailPath, "Test Case", "TestCase1", Path.GetFileNameWithoutExtension(SchBox1.Text.Trim()));
                ini12.INIWrite(sPath, "Schedule1 Exist", "value", "1");
                LoopBox1.BackColor = default(Color);
                SchBox1.BackColor = default(Color);
                if (string.IsNullOrEmpty(LoopBox1.Text) || LoopBox1.Text == "0")
                {
                    SavedLabel.Text = "Schedule1 Loop error !";
                    LoopBox1.BackColor = System.Drawing.Color.Yellow;
                    Global.FormSchedule = false;
                }
                else
                    ini12.INIWrite(sPath, "Schedule1 Loop", "value", LoopBox1.Text.Trim());
            }
            else
            {
                SavedLabel.Text = "Schedule1 must exist !";
                SchBox1.BackColor = System.Drawing.Color.Yellow;
                Global.FormSchedule = false;
            }

            if (SchCheckBox2.Checked == true)
            {
                if (System.IO.File.Exists(SchBox2.Text.Trim()) == true)
                {
                    SchBox2.BackColor = default(Color);
                    ini12.INIWrite(sPath, "Schedule2", "Path", SchBox2.Text.Trim());
                    ini12.INIWrite(MailPath, "Test Case", "TestCase2", Path.GetFileNameWithoutExtension(SchBox2.Text.Trim()));
                    ini12.INIWrite(sPath, "Schedule2 Exist", "value", "1");
                    Global.Schedule_Num2_Exist = 1;

                }
                else
                {
                    SchBox2.BackColor = System.Drawing.Color.Yellow;
                    SavedLabel.Text = "Schedule2 csv file not exist !";
                    Global.FormSchedule = false;
                }

                if (string.IsNullOrEmpty(LoopBox2.Text) || LoopBox2.Text == "0")
                {
                    LoopBox2.BackColor = System.Drawing.Color.Yellow;
                    SavedLabel.Text = "Schedule2 Loop error !";
                    Global.FormSchedule = false;
                }
                else
                {
                    LoopBox2.BackColor = default(Color);
                    ini12.INIWrite(sPath, "Schedule2 Loop", "value", LoopBox2.Text.Trim());
                }
            }

            if (SchCheckBox3.Checked == true)
            {
                if (System.IO.File.Exists(SchBox3.Text.Trim()) == true)
                {
                    SchBox3.BackColor = default(Color);
                    ini12.INIWrite(sPath, "Schedule3", "Path", SchBox3.Text.Trim());
                    ini12.INIWrite(MailPath, "Test Case", "TestCase3", Path.GetFileNameWithoutExtension(SchBox3.Text.Trim()));
                    ini12.INIWrite(sPath, "Schedule3 Exist", "value", "1");
                    Global.Schedule_Num3_Exist = 1;
                }
                else
                {
                    SchBox3.BackColor = System.Drawing.Color.Yellow;
                    SavedLabel.Text = "Schedule3 csv file not exist !";
                    Global.FormSchedule = false;
                }

                if (string.IsNullOrEmpty(LoopBox3.Text) || LoopBox3.Text == "0")
                {
                    LoopBox3.BackColor = System.Drawing.Color.Yellow;
                    SavedLabel.Text = "Schedule3 Loop error !";
                    Global.FormSchedule = false;
                }
                else
                {
                    LoopBox3.BackColor = default(Color);
                    ini12.INIWrite(sPath, "Schedule3 Loop", "value", LoopBox3.Text.Trim());
                }
            }

            if (SchCheckBox4.Checked == true)
            {
                if (System.IO.File.Exists(SchBox4.Text.Trim()) == true)
                {
                    SchBox4.BackColor = default(Color);
                    ini12.INIWrite(sPath, "Schedule4", "Path", SchBox4.Text.Trim());
                    ini12.INIWrite(MailPath, "Test Case", "TestCase4", Path.GetFileNameWithoutExtension(SchBox4.Text.Trim()));
                    ini12.INIWrite(sPath, "Schedule4 Exist", "value", "1");
                    Global.Schedule_Num4_Exist = 1;
                }
                else
                {
                    SchBox4.BackColor = System.Drawing.Color.Yellow;
                    SavedLabel.Text = "Schedule4 csv file not exist !";
                    Global.FormSchedule = false;
                }

                if (string.IsNullOrEmpty(LoopBox4.Text) || LoopBox4.Text == "0")
                {
                    LoopBox4.BackColor = System.Drawing.Color.Yellow;
                    SavedLabel.Text = "Schedule4 Loop error !";
                    Global.FormSchedule = false;
                }
                else
                {
                    LoopBox4.BackColor = default(Color);
                    ini12.INIWrite(sPath, "Schedule4 Loop", "value", LoopBox4.Text.Trim());
                }
            }

            if (SchCheckBox5.Checked == true)
            {
                if (System.IO.File.Exists(SchBox5.Text.Trim()) == true)
                {
                    SchBox5.BackColor = default(Color);
                    ini12.INIWrite(sPath, "Schedule5", "Path", SchBox5.Text.Trim());
                    ini12.INIWrite(MailPath, "Test Case", "TestCase5", Path.GetFileNameWithoutExtension(SchBox5.Text.Trim()));
                    ini12.INIWrite(sPath, "Schedule5 Exist", "value", "1");
                    Global.Schedule_Num5_Exist = 1;
                }
                else
                {
                    SchBox5.BackColor = System.Drawing.Color.Yellow;
                    SavedLabel.Text = "Schedule5 csv file not exist !";
                    Global.FormSchedule = false;
                }

                if (string.IsNullOrEmpty(LoopBox5.Text) || LoopBox5.Text == "0")
                {
                    LoopBox5.BackColor = System.Drawing.Color.Yellow;
                    SavedLabel.Text = "Schedule5 Loop error !";
                    Global.FormSchedule = false;
                }
                else
                {
                    LoopBox5.BackColor = default(Color);
                    ini12.INIWrite(sPath, "Schedule5 Loop", "value", LoopBox5.Text.Trim());
                }
            }

            for (int i = 1; i < 6; i++)
                Global.Total_Loop += int.Parse(ini12.INIRead(sPath, "Schedule" + i + " Loop", "value", ""));

            ini12.INIWrite(sPath, "Schedule1", "Timer", dateTimePickerSch1.Text.Trim() + ":00");
            ini12.INIWrite(sPath, "Schedule2", "Timer", dateTimePickerSch2.Text.Trim() + ":00");
            ini12.INIWrite(sPath, "Schedule3", "Timer", dateTimePickerSch3.Text.Trim() + ":00");
            ini12.INIWrite(sPath, "Schedule4", "Timer", dateTimePickerSch4.Text.Trim() + ":00");
            ini12.INIWrite(sPath, "Schedule5", "Timer", dateTimePickerSch5.Text.Trim() + ":00");

            DateTime dt1 = Convert.ToDateTime(dateTimePickerSch1.Text);
            DateTime dt2 = Convert.ToDateTime(dateTimePickerSch2.Text);
            DateTime dt3 = Convert.ToDateTime(dateTimePickerSch3.Text);
            DateTime dt4 = Convert.ToDateTime(dateTimePickerSch4.Text);
            DateTime dt5 = Convert.ToDateTime(dateTimePickerSch5.Text);

            #region Schedule2偵錯
            if (ini12.INIRead(sPath, "Schedule2", "On Time Start", "") == "1")
            {
                if (ini12.INIRead(sPath, "Schedule1", "On Time Start", "") == "1" && DateTime.Compare(dt1, dt2) > 0)
                {
                    checkBoxTimer2.BackColor = System.Drawing.Color.DarkMagenta;
                    SavedLabel.Text = "Schedule2 Timer Error !";
                    Global.FormSchedule = false;
                }
                else
                    checkBoxTimer2.BackColor = default(Color);
            }
            else
                checkBoxTimer2.BackColor = default(Color);
            #endregion
            #region Schedule3偵錯
            if (ini12.INIRead(sPath, "Schedule3", "On Time Start", "") == "1")
            {
                if (ini12.INIRead(sPath, "Schedule1", "On Time Start", "") == "1" && DateTime.Compare(dt1, dt3) > 0)
                {
                    checkBoxTimer3.BackColor = System.Drawing.Color.DarkMagenta;
                    SavedLabel.Text = "Schedule3 Timer Error !";
                    Global.FormSchedule = false;
                }
                else if (ini12.INIRead(sPath, "Schedule2", "On Time Start", "") == "1" && DateTime.Compare(dt2, dt3) > 0)
                {
                    checkBoxTimer3.BackColor = System.Drawing.Color.DarkMagenta;
                    SavedLabel.Text = "Schedule3 Timer Error !";
                    Global.FormSchedule = false;
                }
                else
                    checkBoxTimer3.BackColor = default(Color);
            }
            else
                checkBoxTimer3.BackColor = default(Color);
            #endregion
            #region Schedule4偵錯
            if (ini12.INIRead(sPath, "Schedule4", "On Time Start", "") == "1")
            {
                if (ini12.INIRead(sPath, "Schedule1", "On Time Start", "") == "1" && DateTime.Compare(dt1, dt4) > 0)
                {
                    checkBoxTimer4.BackColor = System.Drawing.Color.DarkMagenta;
                    SavedLabel.Text = "Schedule4 Timer Error !";
                    Global.FormSchedule = false;
                }
                else if (ini12.INIRead(sPath, "Schedule2", "On Time Start", "") == "1" && DateTime.Compare(dt2, dt4) > 0)
                {
                    checkBoxTimer4.BackColor = System.Drawing.Color.DarkMagenta;
                    SavedLabel.Text = "Schedule4 Timer Error !";
                    Global.FormSchedule = false;
                }
                else if (ini12.INIRead(sPath, "Schedule3", "On Time Start", "") == "1" && DateTime.Compare(dt3, dt4) > 0)
                {
                    checkBoxTimer4.BackColor = System.Drawing.Color.DarkMagenta;
                    SavedLabel.Text = "Schedule4 Timer Error !";
                    Global.FormSchedule = false;
                }
                else
                    checkBoxTimer4.BackColor = default(Color);
            }
            else
                checkBoxTimer4.BackColor = default(Color);
            #endregion
            #region Schedule5偵錯
            if (ini12.INIRead(sPath, "Schedule5", "On Time Start", "") == "1")
            {
                if (ini12.INIRead(sPath, "Schedule1", "On Time Start", "") == "1" && DateTime.Compare(dt1, dt5) > 0)
                {
                    checkBoxTimer5.BackColor = System.Drawing.Color.DarkMagenta;
                    SavedLabel.Text = "Schedule5 Timer Error !";
                    Global.FormSchedule = false;
                }
                else if (ini12.INIRead(sPath, "Schedule2", "On Time Start", "") == "1" && DateTime.Compare(dt2, dt5) > 0)
                {
                    checkBoxTimer5.BackColor = System.Drawing.Color.DarkMagenta;
                    SavedLabel.Text = "Schedule5 Timer Error !";
                    Global.FormSchedule = false;
                }
                else if (ini12.INIRead(sPath, "Schedule3", "On Time Start", "") == "1" && DateTime.Compare(dt3, dt5) > 0)
                {
                    checkBoxTimer5.BackColor = System.Drawing.Color.DarkMagenta;
                    SavedLabel.Text = "Schedule5 Timer Error !";
                    Global.FormSchedule = false;
                }
                else if (ini12.INIRead(sPath, "Schedule4", "On Time Start", "") == "1" && DateTime.Compare(dt4, dt5) > 0)
                {
                    checkBoxTimer5.BackColor = System.Drawing.Color.DarkMagenta;
                    SavedLabel.Text = "Schedule5 Timer Error !";
                    Global.FormSchedule = false;
                }
                else
                    checkBoxTimer5.BackColor = default(Color);
            }
            else
                checkBoxTimer5.BackColor = default(Color);
            #endregion

            if (
                LoopBox1.BackColor != System.Drawing.Color.Yellow && 
                LoopBox2.BackColor != System.Drawing.Color.Yellow &&
                LoopBox3.BackColor != System.Drawing.Color.Yellow &&
                LoopBox4.BackColor != System.Drawing.Color.Yellow &&
                LoopBox5.BackColor != System.Drawing.Color.Yellow &&
                SchBox1.BackColor != System.Drawing.Color.Yellow &&
                SchBox2.BackColor != System.Drawing.Color.Yellow &&
                SchBox3.BackColor != System.Drawing.Color.Yellow &&
                SchBox4.BackColor != System.Drawing.Color.Yellow &&
                SchBox5.BackColor != System.Drawing.Color.Yellow &&
                checkBoxTimer2.BackColor != System.Drawing.Color.DarkMagenta &&
                checkBoxTimer3.BackColor != System.Drawing.Color.DarkMagenta &&
                checkBoxTimer4.BackColor != System.Drawing.Color.DarkMagenta &&
                checkBoxTimer5.BackColor != System.Drawing.Color.DarkMagenta
                )
            {
                SavedLabel.Text = "Save Successfully !";
                Global.FormSchedule = true;
            }
        }

        private void SchCheckBox2_CheckedChanged(object sender, EventArgs e)
        {
            if (SchCheckBox2.Checked == false)
            {
                SchBox2.BackColor = default(Color);
                LoopBox2.BackColor = default(Color);

                LoadSchBtn2.Enabled = false;
                SchBox2.Enabled = false;
                LoopBox2.Enabled = false;

                ini12.INIWrite(sPath, "Schedule2 Exist", "value", "0");
                Global.Schedule_Num2_Exist = 0;

                ini12.INIWrite(MailPath, "Test Case", "TestCase2", "");

                checkBoxTimer2.Checked = false;
                checkBoxTimer2.Enabled = false;
            }
            else
            {
                LoadSchBtn2.Enabled = true;
                SchBox2.Enabled = true;
                LoopBox2.Enabled = true;
                checkBoxTimer2.Enabled = true;
            }
        }

        private void SchCheckBox3_CheckedChanged(object sender, EventArgs e)
        {
            if (SchCheckBox3.Checked == false)
            {
                SchBox3.BackColor = default(Color);
                LoopBox3.BackColor = default(Color);

                LoadSchBtn3.Enabled = false;
                SchBox3.Enabled = false;
                LoopBox3.Enabled = false;

                ini12.INIWrite(sPath, "Schedule3 Exist", "value", "0");
                Global.Schedule_Num3_Exist = 0;

                ini12.INIWrite(MailPath, "Test Case", "TestCase3", "");

                checkBoxTimer3.Checked = false;
                checkBoxTimer3.Enabled = false;
            }
            else
            {
                LoadSchBtn3.Enabled = true;
                SchBox3.Enabled = true;
                LoopBox3.Enabled = true;
                checkBoxTimer3.Enabled = true;
            }
        }

        private void SchCheckBox4_CheckedChanged(object sender, EventArgs e)
        {
            if (SchCheckBox4.Checked == false)
            {
                SchBox4.BackColor = default(Color);
                LoopBox4.BackColor = default(Color);

                LoadSchBtn4.Enabled = false;
                SchBox4.Enabled = false;
                LoopBox4.Enabled = false;

                ini12.INIWrite(sPath, "Schedule4 Exist", "value", "0");
                Global.Schedule_Num4_Exist = 0;

                ini12.INIWrite(MailPath, "Test Case", "TestCase4", "");

                checkBoxTimer4.Checked = false;
                checkBoxTimer4.Enabled = false;
            }
            else
            {
                LoadSchBtn4.Enabled = true;
                SchBox4.Enabled = true;
                LoopBox4.Enabled = true;
                checkBoxTimer4.Enabled = true;
            }
        }

        private void SchCheckBox5_CheckedChanged(object sender, EventArgs e)
        {
            if (SchCheckBox5.Checked == false)
            {
                SchBox5.BackColor = default(Color);
                LoopBox5.BackColor = default(Color);

                LoadSchBtn5.Enabled = false;
                SchBox5.Enabled = false;
                LoopBox5.Enabled = false;

                ini12.INIWrite(sPath, "Schedule5 Exist", "value", "0");
                Global.Schedule_Num5_Exist = 0;

                ini12.INIWrite(MailPath, "Test Case", "TestCase5", "");

                checkBoxTimer5.Checked = false;
                checkBoxTimer5.Enabled = false;
            }
            else
            {
                LoadSchBtn5.Enabled = true;
                SchBox5.Enabled = true;
                LoopBox5.Enabled = true;
                checkBoxTimer5.Enabled = true;
            }
        }

        private void ComparePIC_CheckedChanged(object sender, EventArgs e)
        {
            if (CompareCheckBox.Checked == true)
            {
                // 設定INI檔路徑
                String sPath;
                sPath = Application.StartupPath + "\\Config.ini";

                // 寫入Import DadaBase啟動
                ini12.INIWrite(sPath, "ComparePIC", "value", "1");
            }
            else
            {
                // 設定INI檔路徑
                String sPath;
                sPath = Application.StartupPath + "\\Config.ini";

                // 寫入Import DadaBase關閉
                ini12.INIWrite(sPath, "ComparePIC", "value", "0");
            }
        }

        private void CompareBox_CheckedChanged(object sender, EventArgs e)
        {
            if (CompareCheckBox.Checked == true)
            {
                // 設定INI檔路徑
                String sPath;
                sPath = Application.StartupPath + "\\Config.ini";

                // 寫入Import DadaBase啟動
                ini12.INIWrite(sPath, "ComparePIC", "value", "1");

                DifferenceBox.Enabled = true;
            }
            else
            {
                // 設定INI檔路徑
                String sPath;
                sPath = Application.StartupPath + "\\Config.ini";

                // 寫入Import DadaBase關閉
                ini12.INIWrite(sPath, "ComparePIC", "value", "0");

                DifferenceBox.Enabled = false;
            }
        }

        #region checkBoxTimer
        private void checkBoxTimer1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxTimer1.Checked == true)
            {
                ini12.INIWrite(sPath, "Schedule1", "On Time Start", "1");
                dateTimePickerSch1.Enabled = true;
            }
            else
            {
                ini12.INIWrite(sPath, "Schedule1", "On Time Start", "0");
                dateTimePickerSch1.Enabled = false;
            }
        }
        private void checkBoxTimer2_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxTimer2.Checked == true)
            {
                ini12.INIWrite(sPath, "Schedule2", "On Time Start", "1");
                dateTimePickerSch2.Enabled = true;
            }
            else
            {
                ini12.INIWrite(sPath, "Schedule2", "On Time Start", "0");
                dateTimePickerSch2.Enabled = false;
            }
        }
        private void checkBoxTimer3_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxTimer3.Checked == true)
            {
                ini12.INIWrite(sPath, "Schedule3", "On Time Start", "1");
                dateTimePickerSch3.Enabled = true;
            }
            else
            {
                ini12.INIWrite(sPath, "Schedule3", "On Time Start", "0");
                dateTimePickerSch3.Enabled = false;
            }
        }
        private void checkBoxTimer4_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxTimer4.Checked == true)
            {
                ini12.INIWrite(sPath, "Schedule4", "On Time Start", "1");
                dateTimePickerSch4.Enabled = true;
            }
            else
            {
                ini12.INIWrite(sPath, "Schedule4", "On Time Start", "0");
                dateTimePickerSch4.Enabled = false;
            }
        }
        private void checkBoxTimer5_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxTimer5.Checked == true)
            {
                ini12.INIWrite(sPath, "Schedule5", "On Time Start", "1");
                dateTimePickerSch5.Enabled = true;
            }
            else
            {
                ini12.INIWrite(sPath, "Schedule5", "On Time Start", "0");
                dateTimePickerSch5.Enabled = false;
            }
        }
        #endregion

        private void DifferenceBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            String sPath;
            sPath = Application.StartupPath + "\\Config.ini";

            if (DifferenceBox.Text == "0%")
            {
                ini12.INIWrite(sPath, "ComparePIC", "DifferentNum", "100");
            }
            else if (DifferenceBox.Text == "10%")
            {
                ini12.INIWrite(sPath, "ComparePIC", "DifferentNum", "90");
            }
            else if (DifferenceBox.Text == "20%")
            {
                ini12.INIWrite(sPath, "ComparePIC", "DifferentNum", "80");
            }
            else if (DifferenceBox.Text == "30%")
            {
                ini12.INIWrite(sPath, "ComparePIC", "DifferentNum", "70");
            }
            else if (DifferenceBox.Text == "40%")
            {
                ini12.INIWrite(sPath, "ComparePIC", "DifferentNum", "60");
            }
            else if (DifferenceBox.Text == "50%")
            {
                ini12.INIWrite(sPath, "ComparePIC", "DifferentNum", "50");
            }
            else if (DifferenceBox.Text == "60%")
            {
                ini12.INIWrite(sPath, "ComparePIC", "DifferentNum", "40");
            }
            else if (DifferenceBox.Text == "70%")
            {
                ini12.INIWrite(sPath, "ComparePIC", "DifferentNum", "30");
            }
            else if (DifferenceBox.Text == "80%")
            {
                ini12.INIWrite(sPath, "ComparePIC", "DifferentNum", "20");
            }
            else if (DifferenceBox.Text == "90%")
            {
                ini12.INIWrite(sPath, "ComparePIC", "DifferentNum", "10");
            }
            else
            {
                ini12.INIWrite(sPath, "ComparePIC", "DifferentNum", "0");
            }
        }
    }
}
