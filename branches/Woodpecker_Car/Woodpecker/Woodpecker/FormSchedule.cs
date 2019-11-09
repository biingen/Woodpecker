using jini;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace Woodpecker
{
    public partial class FormSchedule : Form
    {
        string MainSettingPath = Application.StartupPath + "\\Config.ini";
        string MailPath = Application.StartupPath + "\\Mail.ini";

        public FormSchedule()
        {
            InitializeComponent();
        }

        private void setStyle()
        {
            // Button design
            List<Button> buttonsList = new List<Button> { button_Schedule1, button_Schedule2, button_Schedule3, button_Schedule4, button_Schedule5};

            foreach (Button buttonsAll in buttonsList)
            {
                if (buttonsAll.Enabled == true)
                {
                    buttonsAll.FlatAppearance.BorderColor = Color.FromArgb(45, 103, 179);
                    buttonsAll.FlatAppearance.BorderSize = 1;
                    buttonsAll.BackColor = System.Drawing.Color.FromArgb(45, 103, 179);
                }
                else
                {
                    buttonsAll.FlatAppearance.BorderColor = Color.FromArgb(220, 220, 220);
                    buttonsAll.FlatAppearance.BorderSize = 1;
                    buttonsAll.BackColor = System.Drawing.Color.FromArgb(220, 220, 220);
                }
            }
        }

        private void FormSchedule_Load(object sender, EventArgs e)
        {
            textBox_Schedule1.Text = ini12.INIRead(MainSettingPath, "Schedule1", "Path", "");
            textBox_Schedule1Loop.Text = ini12.INIRead(MainSettingPath, "Schedule1", "Loop", "");

            if (textBox_Schedule1.Text == "")
            {
                label_ErrorMessage.Text = "Schedule1 .csv file not exist";
                pictureBox_Schedule1.Image = Properties.Resources.ERROR;
            }


            if (ini12.INIRead(MainSettingPath, "Schedule2", "Exist", "") != "")
            {
                if (int.Parse(ini12.INIRead(MainSettingPath, "Schedule2", "Exist", "")) == 1)
                {
                    checkBox_Schedule2.Checked = true;
                    textBox_Schedule2.Text = ini12.INIRead(MainSettingPath, "Schedule2", "Path", "");
                    textBox_Schedule2Loop.Text = ini12.INIRead(MainSettingPath, "Schedule2", "Loop", "");
                }
                else
                {
                    checkBox_Schedule2.Checked = false;
                    button_Schedule2.Enabled = false;
                    textBox_Schedule2.Enabled = false;
                    textBox_Schedule2Loop.Enabled = false;
                }
            }
            else
            {
                ini12.INIWrite(MainSettingPath, "Schedule2", "Exist", "0");
                ini12.INIWrite(MainSettingPath, "Schedule2", "Loop", "0");
                checkBox_Schedule2.Checked = false;
                button_Schedule2.Enabled = false;
                textBox_Schedule2.Enabled = false;
                textBox_Schedule2Loop.Enabled = false;
            }

            if (ini12.INIRead(MainSettingPath, "Schedule3", "Exist", "") != "")
            {
                if (int.Parse(ini12.INIRead(MainSettingPath, "Schedule3", "Exist", "")) == 1)
                {
                    checkBox_Schedule3.Checked = true;
                    textBox_Schedule3.Text = ini12.INIRead(MainSettingPath, "Schedule3", "Path", "");
                    textBox_Schedule3Loop.Text = ini12.INIRead(MainSettingPath, "Schedule3", "Loop", "");
                }
                else
                {
                    checkBox_Schedule3.Checked = false;
                    button_Schedule3.Enabled = false;
                    textBox_Schedule3.Enabled = false;
                    textBox_Schedule3Loop.Enabled = false;
                }
            }
            else
            {
                ini12.INIWrite(MainSettingPath, "Schedule3", "Exist", "0");
                ini12.INIWrite(MainSettingPath, "Schedule3", "Loop", "0");
                checkBox_Schedule3.Checked = false;
                button_Schedule3.Enabled = false;
                textBox_Schedule3.Enabled = false;
                textBox_Schedule3Loop.Enabled = false;
            }

            if (ini12.INIRead(MainSettingPath, "Schedule4", "Exist", "") != "")
            {
                if (int.Parse(ini12.INIRead(MainSettingPath, "Schedule4", "Exist", "")) == 1)
                {
                    checkBox_Schedule4.Checked = true;
                    textBox_Schedule4.Text = ini12.INIRead(MainSettingPath, "Schedule4", "Path", "");
                    textBox_Schedule4Loop.Text = ini12.INIRead(MainSettingPath, "Schedule4", "Loop", "");
                }
                else
                {
                    checkBox_Schedule4.Checked = false;
                    button_Schedule4.Enabled = false;
                    textBox_Schedule4.Enabled = false;
                    textBox_Schedule4Loop.Enabled = false;
                }
            }
            else
            {
                ini12.INIWrite(MainSettingPath, "Schedule4", "Exist", "0");
                ini12.INIWrite(MainSettingPath, "Schedule4", "Loop", "0");
                checkBox_Schedule4.Checked = false;
                button_Schedule4.Enabled = false;
                textBox_Schedule4.Enabled = false;
                textBox_Schedule4Loop.Enabled = false;
            }

            if (ini12.INIRead(MainSettingPath, "Schedule5", "Exist", "") != "")
            {
                if (int.Parse(ini12.INIRead(MainSettingPath, "Schedule5", "Exist", "")) == 1)
                {
                    checkBox_Schedule5.Checked = true;
                    textBox_Schedule5.Text = ini12.INIRead(MainSettingPath, "Schedule5", "Path", "");
                    textBox_Schedule5Loop.Text = ini12.INIRead(MainSettingPath, "Schedule5", "Loop", "");
                }
                else
                {
                    checkBox_Schedule5.Checked = false;
                    button_Schedule5.Enabled = false;
                    textBox_Schedule5.Enabled = false;
                    textBox_Schedule5Loop.Enabled = false;
                }
            }
            else
            {
                ini12.INIWrite(MainSettingPath, "Schedule5", "Exist", "0");
                ini12.INIWrite(MainSettingPath, "Schedule5", "Loop", "0");
                checkBox_Schedule5.Checked = false;
                button_Schedule5.Enabled = false;
                textBox_Schedule5.Enabled = false;
                textBox_Schedule5Loop.Enabled = false;
            }

            if (ini12.INIRead(MainSettingPath, "Record", "Footprint Mode", "") == "1")
            {
                checkBox_FootprintMode.Checked = true;
            }
            else
            {
                checkBox_FootprintMode.Checked = false;
            }

            if (ini12.INIRead(MainSettingPath, "Record", "EachVideo", "") != "")
            {
                if (int.Parse(ini12.INIRead(MainSettingPath, "Record", "EachVideo", "")) == 1)
                    checkBox_VideoRecord.Checked = true;
                else
                    checkBox_VideoRecord.Checked = false;
            }
            else
            {
                ini12.INIWrite(MainSettingPath, "Record", "EachVideo", "0");
            }

            if (ini12.INIRead(MainSettingPath, "Device", "RunAfterStartUp", "") == "1")
            {
                checkBox_ScheduleAutoStart.Checked = true;
            }
            else
            {
                checkBox_ScheduleAutoStart.Checked = false;
            }

            setStyle();
        }

        #region LoadSchBtn
        private void LoadSchBtn1_Click(object sender, EventArgs e)      // Load Schedule1 Path
        {
            SchOpen1.Filter = "CSV files (*.csv)|*.CSV";
            SchOpen1.ShowDialog();
            if (SchOpen1.FileName == "SchOpen1")
                textBox_Schedule1.Text = textBox_Schedule1.Text;
            else
                textBox_Schedule1.Text = SchOpen1.FileName;
        }
        private void LoadSchBtn2_Click(object sender, EventArgs e)      // Load Schedule2 Path
        {
            SchOpen2.Filter = "CSV files (*.csv)|*.CSV";
            SchOpen2.ShowDialog();
            if (SchOpen2.FileName == "SchOpen2")
                textBox_Schedule2.Text = textBox_Schedule2.Text;
            else
                textBox_Schedule2.Text = SchOpen2.FileName;
        }
        private void LoadSchBtn3_Click(object sender, EventArgs e)      // Load Schedule3 Path
        {
            SchOpen3.Filter = "CSV files (*.csv)|*.CSV";
            SchOpen3.ShowDialog();
            if (SchOpen3.FileName == "SchOpen3")
                textBox_Schedule3.Text = textBox_Schedule3.Text;
            else
                textBox_Schedule3.Text = SchOpen3.FileName;
        }
        private void LoadSchBtn4_Click(object sender, EventArgs e)      // Load Schedule4 Path
        {
            SchOpen4.Filter = "CSV files (*.csv)|*.CSV";
            SchOpen4.ShowDialog();
            if (SchOpen4.FileName == "SchOpen4")
                textBox_Schedule4.Text = textBox_Schedule4.Text;
            else
                textBox_Schedule4.Text = SchOpen4.FileName;
        }
        private void LoadSchBtn5_Click(object sender, EventArgs e)      // Load Schedule5 Path
        {
            SchOpen5.Filter = "CSV files (*.csv)|*.CSV";
            SchOpen5.ShowDialog();
            if (SchOpen5.FileName == "SchOpen5")
                textBox_Schedule5.Text = textBox_Schedule5.Text;
            else
                textBox_Schedule5.Text = SchOpen5.FileName;
        }
        #endregion

        private void textBox_Schedule1_TextChanged(object sender, EventArgs e)
        {
            if (File.Exists(textBox_Schedule1.Text.Trim()) == true)
            {
                ini12.INIWrite(MainSettingPath, "Schedule1", "Path", textBox_Schedule1.Text.Trim());
                ini12.INIWrite(MainSettingPath, "Schedule1", "Exist", "1");
                ini12.INIWrite(MailPath, "Test Case", "TestCase1", Path.GetFileNameWithoutExtension(textBox_Schedule1.Text.Trim()));

                label_ErrorMessage.Text = "";
                pictureBox_Schedule1.Image = null;
            }
            else
            {
                label_ErrorMessage.Text = "Schedule1 .csv file not exist";
                pictureBox_Schedule1.Image = Properties.Resources.ERROR;
            }
        }

        private void textBox_Schedule1Loop_TextChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(textBox_Schedule1Loop.Text) || textBox_Schedule1Loop.Text == "0")
            {
                label_ErrorMessage.Text = "Please enter loop number for Schedule 1";
                pictureBox_Schedule1Loop.Image = Properties.Resources.ERROR;
            }
            else
            {
                ini12.INIWrite(MainSettingPath, "Schedule1", "Loop", textBox_Schedule1Loop.Text.Trim());

                label_ErrorMessage.Text = "";
                pictureBox_Schedule1Loop.Image = null;
            }
        }

        private void SchCheckBox2_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox_Schedule2.Checked == false)
            {
                button_Schedule2.Enabled = false;
                textBox_Schedule2.Enabled = false;
                textBox_Schedule2Loop.Enabled = false;

                ini12.INIWrite(MainSettingPath, "Schedule2", "Exist", "0");
                Global.Schedule_2_Exist = 0;

                ini12.INIWrite(MailPath, "Test Case", "TestCase2", "");

                label_ErrorMessage.Text = "";
                pictureBox_Schedule2.Image = null;
                pictureBox_Schedule2Loop.Image = null;
            }
            else
            {
                textBox_Schedule2_TextChanged(new TextBox(), new EventArgs());
                textBox_Schedule2Loop_TextChanged(new TextBox(), new EventArgs());

                button_Schedule2.Enabled = true;
                textBox_Schedule2.Enabled = true;
                textBox_Schedule2Loop.Enabled = true;
            }
            setStyle();
        }

        private void textBox_Schedule2_TextChanged(object sender, EventArgs e)
        {
            if (File.Exists(textBox_Schedule2.Text.Trim()) == true)
            {
                ini12.INIWrite(MainSettingPath, "Schedule2", "Path", textBox_Schedule2.Text.Trim());
                ini12.INIWrite(MainSettingPath, "Schedule2", "Exist", "1");
                Global.Schedule_2_Exist = 1;
                ini12.INIWrite(MailPath, "Test Case", "TestCase2", Path.GetFileNameWithoutExtension(textBox_Schedule2.Text.Trim()));

                label_ErrorMessage.Text = "";
                pictureBox_Schedule2.Image = null;
            }
            else
            {
                label_ErrorMessage.Text = "Schedule2 .csv file not exist";
                pictureBox_Schedule2.Image = Properties.Resources.ERROR;
            }
        }

        private void textBox_Schedule2Loop_TextChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(textBox_Schedule2Loop.Text) || textBox_Schedule2Loop.Text == "0")
            {
                label_ErrorMessage.Text = "Please enter loop number for Schedule 2";
                pictureBox_Schedule2Loop.Image = Properties.Resources.ERROR;
            }
            else
            {
                ini12.INIWrite(MainSettingPath, "Schedule2", "Loop", textBox_Schedule2Loop.Text.Trim());
                pictureBox_Schedule2Loop.Image = null;
            }
        }

        private void SchCheckBox3_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox_Schedule3.Checked == false)
            {
                button_Schedule3.Enabled = false;
                textBox_Schedule3.Enabled = false;
                textBox_Schedule3Loop.Enabled = false;

                ini12.INIWrite(MainSettingPath, "Schedule3", "Exist", "0");
                Global.Schedule_3_Exist = 0;

                ini12.INIWrite(MailPath, "Test Case", "TestCase3", "");

                label_ErrorMessage.Text = "";
                pictureBox_Schedule3.Image = null;
                pictureBox_Schedule3Loop.Image = null;
            }
            else
            {
                textBox_Schedule3_TextChanged(new TextBox(), new EventArgs());
                textBox_Schedule3Loop_TextChanged(new TextBox(), new EventArgs());

                button_Schedule3.Enabled = true;
                textBox_Schedule3.Enabled = true;
                textBox_Schedule3Loop.Enabled = true;
            }
            setStyle();
        }

        private void textBox_Schedule3_TextChanged(object sender, EventArgs e)
        {
            if (File.Exists(textBox_Schedule3.Text.Trim()) == true)
            {
                ini12.INIWrite(MainSettingPath, "Schedule3", "Path", textBox_Schedule3.Text.Trim());
                ini12.INIWrite(MainSettingPath, "Schedule3", "Exist", "1");
                Global.Schedule_3_Exist = 1;
                ini12.INIWrite(MailPath, "Test Case", "TestCase3", Path.GetFileNameWithoutExtension(textBox_Schedule3.Text.Trim()));

                label_ErrorMessage.Text = "";
                pictureBox_Schedule3.Image = null;
            }
            else
            {
                label_ErrorMessage.Text = "Schedule3 .csv file not exist";
                pictureBox_Schedule3.Image = Properties.Resources.ERROR;
            }
        }

        private void textBox_Schedule3Loop_TextChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(textBox_Schedule3Loop.Text) || textBox_Schedule3Loop.Text == "0")
            {
                label_ErrorMessage.Text = "Please enter loop number for Schedule 3";
                pictureBox_Schedule3Loop.Image = Properties.Resources.ERROR;
            }
            else
            {
                ini12.INIWrite(MainSettingPath, "Schedule3", "Loop", textBox_Schedule3Loop.Text.Trim());
                pictureBox_Schedule3Loop.Image = null;
            }
        }

        private void SchCheckBox4_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox_Schedule4.Checked == false)
            {
                button_Schedule4.Enabled = false;
                textBox_Schedule4.Enabled = false;
                textBox_Schedule4Loop.Enabled = false;

                ini12.INIWrite(MainSettingPath, "Schedule4", "Exist", "0");
                Global.Schedule_4_Exist = 0;

                ini12.INIWrite(MailPath, "Test Case", "TestCase4", "");

                label_ErrorMessage.Text = "";
                pictureBox_Schedule4.Image = null;
                pictureBox_Schedule4Loop.Image = null;
            }
            else
            {
                textBox_Schedule4_TextChanged(new TextBox(), new EventArgs());
                textBox_Schedule4Loop_TextChanged(new TextBox(), new EventArgs());

                button_Schedule4.Enabled = true;
                textBox_Schedule4.Enabled = true;
                textBox_Schedule4Loop.Enabled = true;
            }
            setStyle();
        }

        private void textBox_Schedule4_TextChanged(object sender, EventArgs e)
        {
            if (File.Exists(textBox_Schedule4.Text.Trim()) == true)
            {
                ini12.INIWrite(MainSettingPath, "Schedule4", "Path", textBox_Schedule4.Text.Trim());
                ini12.INIWrite(MainSettingPath, "Schedule4", "Exist", "1");
                Global.Schedule_4_Exist = 1;
                ini12.INIWrite(MailPath, "Test Case", "TestCase4", Path.GetFileNameWithoutExtension(textBox_Schedule4.Text.Trim()));

                label_ErrorMessage.Text = "";
                pictureBox_Schedule4.Image = null;
            }
            else
            {
                label_ErrorMessage.Text = "Schedule4 .csv file not exist";
                pictureBox_Schedule4.Image = Properties.Resources.ERROR;
            }
        }

        private void textBox_Schedule4Loop_TextChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(textBox_Schedule4Loop.Text) || textBox_Schedule4Loop.Text == "0")
            {
                label_ErrorMessage.Text = "Please enter loop number for Schedule 4";
                pictureBox_Schedule4Loop.Image = Properties.Resources.ERROR;
            }
            else
            {
                ini12.INIWrite(MainSettingPath, "Schedule4", "Loop", textBox_Schedule4Loop.Text.Trim());
                pictureBox_Schedule4Loop.Image = null;
            }
        }

        private void SchCheckBox5_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox_Schedule5.Checked == false)
            {
                button_Schedule5.Enabled = false;
                textBox_Schedule5.Enabled = false;
                textBox_Schedule5Loop.Enabled = false;

                ini12.INIWrite(MainSettingPath, "Schedule5", "Exist", "0");
                Global.Schedule_5_Exist = 0;

                ini12.INIWrite(MailPath, "Test Case", "TestCase5", "");

                label_ErrorMessage.Text = "";
                pictureBox_Schedule5.Image = null;
                pictureBox_Schedule5Loop.Image = null;
            }
            else
            {
                textBox_Schedule5_TextChanged(new TextBox(), new EventArgs());
                textBox_Schedule5Loop_TextChanged(new TextBox(), new EventArgs());

                button_Schedule5.Enabled = true;
                textBox_Schedule5.Enabled = true;
                textBox_Schedule5Loop.Enabled = true;
            }
            setStyle();
        }

        private void textBox_Schedule5_TextChanged(object sender, EventArgs e)
        {
            if (File.Exists(textBox_Schedule5.Text.Trim()) == true)
            {
                ini12.INIWrite(MainSettingPath, "Schedule5", "Path", textBox_Schedule5.Text.Trim());
                ini12.INIWrite(MainSettingPath, "Schedule5", "Exist", "1");
                Global.Schedule_5_Exist = 1;
                ini12.INIWrite(MailPath, "Test Case", "TestCase5", Path.GetFileNameWithoutExtension(textBox_Schedule5.Text.Trim()));

                label_ErrorMessage.Text = "";
                pictureBox_Schedule5.Image = null;
            }
            else
            {
                label_ErrorMessage.Text = "Schedule5 .csv file not exist";
                pictureBox_Schedule5.Image = Properties.Resources.ERROR;
            }
        }

        private void textBox_Schedule5Loop_TextChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(textBox_Schedule5Loop.Text) || textBox_Schedule5Loop.Text == "0")
            {
                label_ErrorMessage.Text = "Please enter loop number for Schedule 5";
                pictureBox_Schedule5Loop.Image = Properties.Resources.ERROR;
            }
            else
            {
                ini12.INIWrite(MainSettingPath, "Schedule5", "Loop", textBox_Schedule5Loop.Text.Trim());
                pictureBox_Schedule5Loop.Image = null;
            }
        }

        private void checkBoxFootprintMode_CheckedChanged(object sender, EventArgs e)
        {
            //足跡模式//
            if (checkBox_FootprintMode.Checked == true)
            {
                
                ini12.INIWrite(MainSettingPath, "Record", "Footprint Mode", "1");
            }
            else
            {
                ini12.INIWrite(MainSettingPath, "Record", "Footprint Mode", "0");
            }
        }

        private void RecVideo_CheckedChanged(object sender, EventArgs e)
        {
            //測試完成開始錄影//
            if (checkBox_VideoRecord.Checked == true)
            {
                
                ini12.INIWrite(MainSettingPath, "Record", "EachVideo", "1");
            }
            else
            {
                ini12.INIWrite(MainSettingPath, "Record", "EachVideo", "0");
            }
        }

        private void checkBox_ScheduleAutoStart_CheckedChanged(object sender, EventArgs e)
        {
            //程式啟動自動跑shchedule//
            if (checkBox_ScheduleAutoStart.Checked == true)
            {
                
                ini12.INIWrite(MainSettingPath, "Device", "RunAfterStartUp", "1");
            }
            else
            {
                ini12.INIWrite(MainSettingPath, "Device", "RunAfterStartUp", "0");
            }
        }
    }
}
