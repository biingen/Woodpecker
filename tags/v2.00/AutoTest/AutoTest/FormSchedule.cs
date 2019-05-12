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

            if (int.Parse(ini12.INIRead(sPath, "RecVideo", "value", "")) == 1)
                RecVideo.Checked = true;
            else
                RecVideo.Checked = false;

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
            }

            if (int.Parse(ini12.INIRead(sPath, "Schedule3 Exist", "value", "")) == 1)
                SchCheckBox3.Checked = true;
            else
            {
                SchCheckBox3.Checked = false;
                LoadSchBtn3.Enabled = false;
                SchBox3.Enabled = false;
                LoopBox3.Enabled = false;
            }

            if (int.Parse(ini12.INIRead(sPath, "Schedule4 Exist", "value", "")) == 1)
                SchCheckBox4.Checked = true;
            else
            {
                SchCheckBox4.Checked = false;
                LoadSchBtn4.Enabled = false;
                SchBox4.Enabled = false;
                LoopBox4.Enabled = false;
            }

            if (int.Parse(ini12.INIRead(sPath, "Schedule5 Exist", "value", "")) == 1)
                SchCheckBox5.Checked = true;
            else
            {
                SchCheckBox5.Checked = false;
                LoadSchBtn5.Enabled = false;
                SchBox5.Enabled = false;
                LoopBox5.Enabled = false;
            }
        }



        private void LoadSchBtn1_Click(object sender, EventArgs e)      // Load Schedule1 Path
        {
            SchOpen1.Filter = "CSV files (*.csv)|*.CSV";
            SchOpen1.ShowDialog();
            if (SchOpen1.FileName == "SchOpen1")
            {
                SchBox1.Text = SchBox1.Text;
            }
            else
            {
                SchBox1.Text = SchOpen1.FileName;
            }
        }
        private void LoadSchBtn2_Click(object sender, EventArgs e)      // Load Schedule2 Path
        {
            SchOpen2.Filter = "CSV files (*.csv)|*.CSV";
            SchOpen2.ShowDialog();
            if (SchOpen2.FileName == "SchOpen2")
            {
                SchBox2.Text = SchBox2.Text;
            }
            else
            {
                SchBox2.Text = SchOpen2.FileName;
            }
        }
        private void LoadSchBtn3_Click(object sender, EventArgs e)      // Load Schedule3 Path
        {
            SchOpen3.Filter = "CSV files (*.csv)|*.CSV";
            SchOpen3.ShowDialog();
            if (SchOpen3.FileName == "SchOpen3")
            {
                SchBox3.Text = SchBox3.Text;
            }
            else
            {
                SchBox3.Text = SchOpen3.FileName;
            }
        }
        private void LoadSchBtn4_Click(object sender, EventArgs e)      // Load Schedule4 Path
        {
            SchOpen4.Filter = "CSV files (*.csv)|*.CSV";
            SchOpen4.ShowDialog();
            if (SchOpen4.FileName == "SchOpen4")
            {
                SchBox4.Text = SchBox4.Text;
            }
            else
            {
                SchBox4.Text = SchOpen4.FileName;
            }
        }
        private void LoadSchBtn5_Click(object sender, EventArgs e)      // Load Schedule5 Path
        {
            SchOpen5.Filter = "CSV files (*.csv)|*.CSV";
            SchOpen5.ShowDialog();
            if (SchOpen5.FileName == "SchOpen5")
            {
                SchBox5.Text = SchBox5.Text;
            }
            else
            {
                SchBox5.Text = SchOpen5.FileName;
            }
        }

        private void RecVideo_CheckedChanged(object sender, EventArgs e)       //勾選此CheckBox打開測試後錄影功能
        {
            if (RecVideo.Checked == true)
            {
                // 設定INI檔路徑
                String sPath;
                sPath = Application.StartupPath + "\\Config.ini";

                // 寫入Import DadaBase啟動
                ini12.INIWrite(sPath, "RecVideo", "value", "1");
            }
            else
            {
                // 設定INI檔路徑
                String sPath;
                sPath = Application.StartupPath + "\\Config.ini";

                // 寫入Import DadaBase關閉
                ini12.INIWrite(sPath, "RecVideo", "value", "0");
            }
        }

        public void SaveSchBtn_Click(object sender, EventArgs e)
        {
            if (System.IO.File.Exists(SchBox1.Text.Trim()))
            {
                ini12.INIWrite(sPath, "Schedule1", "Path", SchBox1.Text.Trim());
                ini12.INIWrite(MailPath, "TestCase", "TestCase1", Path.GetFileNameWithoutExtension(SchBox1.Text.Trim()));
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

            if (System.IO.File.Exists(SchBox2.Text.Trim()) && SchCheckBox2.Checked == true)
            {
                Console.WriteLine("fuck");
                ini12.INIWrite(sPath, "Schedule2", "Path", SchBox2.Text.Trim());
                ini12.INIWrite(MailPath, "TestCase", "TestCase2", Path.GetFileNameWithoutExtension(SchBox2.Text.Trim()));
                ini12.INIWrite(sPath, "Schedule2 Exist", "value", "1");
                Global.Schedule_Num2_Exist = 1;
                if (string.IsNullOrEmpty(LoopBox2.Text) || LoopBox2.Text == "0")
                {
                    SavedLabel.Text = "Schedule2 Loop error !";
                    LoopBox2.BackColor = System.Drawing.Color.Yellow;
                    Global.FormSchedule = false;
                }
                else
                {
                    LoopBox2.BackColor = default(Color);
                    ini12.INIWrite(sPath, "Schedule2 Loop", "value", LoopBox2.Text.Trim());
                }
            }

            if (System.IO.File.Exists(SchBox3.Text.Trim()) && SchCheckBox3.Checked == true)
            {
                ini12.INIWrite(sPath, "Schedule3", "Path", SchBox3.Text.Trim());
                ini12.INIWrite(MailPath, "TestCase", "TestCase3", Path.GetFileNameWithoutExtension(SchBox3.Text.Trim()));
                ini12.INIWrite(sPath, "Schedule3 Exist", "value", "1");
                Global.Schedule_Num3_Exist = 1;
                if (string.IsNullOrEmpty(LoopBox3.Text) || LoopBox3.Text == "0")
                {
                    SavedLabel.Text = "Schedule3 Loop error !";
                    LoopBox3.BackColor = System.Drawing.Color.Yellow;
                    Global.FormSchedule = false;
                }
                else
                {
                    LoopBox3.BackColor = default(Color);
                    ini12.INIWrite(sPath, "Schedule3 Loop", "value", LoopBox3.Text.Trim());
                }
            }

            if (System.IO.File.Exists(SchBox4.Text.Trim()) && SchCheckBox4.Checked == true)
            {
                ini12.INIWrite(sPath, "Schedule4", "Path", SchBox4.Text.Trim());
                ini12.INIWrite(MailPath, "TestCase", "TestCase4", Path.GetFileNameWithoutExtension(SchBox4.Text.Trim()));
                ini12.INIWrite(sPath, "Schedule4 Exist", "value", "1");
                Global.Schedule_Num4_Exist = 1;
                if (string.IsNullOrEmpty(LoopBox4.Text) || LoopBox4.Text == "0")
                {
                    SavedLabel.Text = "Schedule4 Loop error !";
                    LoopBox4.BackColor = System.Drawing.Color.Yellow;
                    Global.FormSchedule = false;
                }
                else
                {
                    LoopBox4.BackColor = default(Color);
                    ini12.INIWrite(sPath, "Schedule4 Loop", "value", LoopBox4.Text.Trim());
                }
            }

            if (System.IO.File.Exists(SchBox5.Text.Trim()) && SchCheckBox5.Checked == true)
            {
                ini12.INIWrite(sPath, "Schedule5", "Path", SchBox5.Text.Trim());
                ini12.INIWrite(MailPath, "TestCase", "TestCase5", Path.GetFileNameWithoutExtension(SchBox5.Text.Trim()));
                ini12.INIWrite(sPath, "Schedule5 Exist", "value", "1");
                Global.Schedule_Num5_Exist = 1;
                if (string.IsNullOrEmpty(LoopBox5.Text) || LoopBox5.Text == "0")
                {
                    SavedLabel.Text = "Schedule5 Loop error !";
                    LoopBox5.BackColor = System.Drawing.Color.Yellow;
                    Global.FormSchedule = false;
                }
                else
                {
                    LoopBox5.BackColor = default(Color);
                    ini12.INIWrite(sPath, "Schedule5 Loop", "value", LoopBox5.Text.Trim());
                }
            }

            if (LoopBox1.BackColor != System.Drawing.Color.Yellow &&
                    LoopBox2.BackColor != System.Drawing.Color.Yellow &&
                    LoopBox3.BackColor != System.Drawing.Color.Yellow &&
                    LoopBox4.BackColor != System.Drawing.Color.Yellow &&
                    LoopBox5.BackColor != System.Drawing.Color.Yellow &&
                    SchBox1.BackColor != System.Drawing.Color.Yellow)
            {
                SavedLabel.Text = "Save Successfully !";
                Global.FormSchedule = true;
            }

            for (int i = 1; i < 6; i++)
                Global.Total_Loop += int.Parse(ini12.INIRead(sPath, "Schedule" + i + " Loop", "value", ""));
        }

        private void SchCheckBox2_CheckedChanged(object sender, EventArgs e)
        {
            if (SchCheckBox2.Checked == false)
            {
                LoadSchBtn2.Enabled = false;
                SchBox2.Enabled = false;
                LoopBox2.Enabled = false;

                ini12.INIWrite(sPath, "Schedule2 Exist", "value", "0");
                Global.Schedule_Num2_Exist = 0;

                ini12.INIWrite(MailPath, "TestCase", "TestCase2", "");
            }
            else
            {
                LoadSchBtn2.Enabled = true;
                SchBox2.Enabled = true;
                LoopBox2.Enabled = true;
            }
        }

        private void SchCheckBox3_CheckedChanged(object sender, EventArgs e)
        {
            if (SchCheckBox3.Checked == false)
            {
                LoadSchBtn3.Enabled = false;
                SchBox3.Enabled = false;
                LoopBox3.Enabled = false;

                ini12.INIWrite(sPath, "Schedule3 Exist", "value", "0");
                Global.Schedule_Num3_Exist = 0;

                ini12.INIWrite(MailPath, "TestCase", "TestCase3", "");
            }
            else
            {
                LoadSchBtn3.Enabled = true;
                SchBox3.Enabled = true;
                LoopBox3.Enabled = true;
            }
        }

        private void SchCheckBox4_CheckedChanged(object sender, EventArgs e)
        {
            if (SchCheckBox4.Checked == false)
            {
                LoadSchBtn4.Enabled = false;
                SchBox4.Enabled = false;
                LoopBox4.Enabled = false;

                ini12.INIWrite(sPath, "Schedule4 Exist", "value", "0");
                Global.Schedule_Num4_Exist = 0;

                ini12.INIWrite(MailPath, "TestCase", "TestCase4", "");
            }
            else
            {
                LoadSchBtn4.Enabled = true;
                SchBox4.Enabled = true;
                LoopBox4.Enabled = true;
            }
        }

        private void SchCheckBox5_CheckedChanged(object sender, EventArgs e)
        {
            if (SchCheckBox5.Checked == false)
            {
                LoadSchBtn5.Enabled = false;
                SchBox5.Enabled = false;
                LoopBox5.Enabled = false;

                ini12.INIWrite(sPath, "Schedule5 Exist", "value", "0");
                Global.Schedule_Num5_Exist = 0;

                ini12.INIWrite(MailPath, "TestCase", "TestCase5", "");
            }
            else
            {
                LoadSchBtn5.Enabled = true;
                SchBox5.Enabled = true;
                LoopBox5.Enabled = true;
            }
        }
    }
}
