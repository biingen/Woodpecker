﻿using jini;
using MaterialSkin;
using MaterialSkin.Controls;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Woodpecker
{
    public partial class FormTabControl : MaterialForm
    {
        public FormTabControl()
        {
            InitializeComponent();
            setStyle();
        }

        //拖動無窗體的控件>>>>>>>>>>>>>>
        [DllImport("user32.dll")]
        new public static extern bool ReleaseCapture();
        [DllImport("user32.dll")]
        new public static extern bool SendMessage(IntPtr hwnd, int wMsg, int wParam, int lParam);
        public const int WM_SYSCOMMAND = 0x0112;
        public const int SC_MOVE = 0xF010;
        public const int HTCAPTION = 0x0002;
        //<<<<<<<<<<<<<<<<<<<<<<<<<<<<<

        private void setStyle()
        {
            // Form design
            //this.MinimumSize = new Size(1097, 659);
            this.BackColor = Color.FromArgb(18, 18, 18);

            //Init material skin
            var skinManager = MaterialSkinManager.Instance;
            skinManager.AddFormToManage(this);
            skinManager.Theme = MaterialSkinManager.Themes.DARK;
            skinManager.ColorScheme = new ColorScheme(Primary.BlueGrey800, Primary.BlueGrey900, Primary.BlueGrey500, Accent.LightBlue200, TextShade.WHITE);
        }

        private void Add_TabPage(string str, Form myForm)//載入Form到tab
        {
            if (tabControlCheckHave(tabControl, str))
            {
                return;
            }
            else
            {
                tabControl.TabPages.Add(str);
                tabControl.SelectTab(tabControl.TabPages.Count - 1);

                myForm.FormBorderStyle = FormBorderStyle.None;
                myForm.Dock = DockStyle.Fill;
                myForm.TopLevel = false;
                myForm.Show();
                myForm.Parent = tabControl.SelectedTab;
            }
        }

        private bool tabControlCheckHave(System.Windows.Forms.TabControl tab, String tabName)//設定tab索引編號
        {
            for (int i = 0; i < tab.TabCount; i++)
            {
                if (tab.TabPages[i].Text == tabName)
                {
                    tab.SelectedIndex = i;
                    return true;
                }
            }
            return false;
        }

        private void MainSettingBtn_Click(object sender, EventArgs e)
        {
            Setting FormSetting = new Setting();

            MainSettingBtn.Enabled = false;
            FormSetting.Dock = DockStyle.Fill;
            Add_TabPage("Main Setting", FormSetting);
        }

        private void ScheduleBtn_Click(object sender, EventArgs e)
        {
            FormSchedule FormSchedule = new FormSchedule();

            ScheduleSettingBtn.Enabled = false;
            FormSchedule.Dock = DockStyle.Fill;
            Add_TabPage("Multi Schedule Setting", FormSchedule);
        }

        private void MailSettingBtn_Click(object sender, EventArgs e)
        {
            FormMail FormMail = new FormMail();

            MailSettingBtn.Enabled = false;
            FormMail.Dock = DockStyle.Fill;
            Add_TabPage("Mail Setting", FormMail);
        }

        private void LogSettingBtn_Click(object sender, EventArgs e)
        {
            if (ini12.INIRead(MainSettingPath, "Device", "Software", "") == "All")
            {
                FormLog FormLog = new FormLog();

                LogSettingBtn.Enabled = false;
                FormLog.Dock = DockStyle.Fill;
                Add_TabPage("Log Setting", FormLog);
            }
        }

        private void buttonMonkeyTest_Click(object sender, EventArgs e)
        {
            FormMonkeyTest FormMonkeyTest = new FormMonkeyTest();

            buttonMonkeyTest.Enabled = false;
            FormMonkeyTest.Dock = DockStyle.Fill;
            Add_TabPage("Monket Test", FormMonkeyTest);
        }

        #region 滑鼠拖曳視窗
        private void gPanelTitleBack_MouseDown(object sender, MouseEventArgs e)
        {
            ReleaseCapture();
            SendMessage(Handle, WM_SYSCOMMAND, SC_MOVE + HTCAPTION, 0);//*********************調用移動無窗體控件函數
        }
        #endregion

        #region 關閉按鈕
        private void ClosePicBox_Enter(object sender, EventArgs e)
        {
            ClosePicBox.Image = Properties.Resources.close2;
        }

        private void ClosePicBox_Leave(object sender, EventArgs e)
        {
            ClosePicBox.Image = Properties.Resources.close1;
        }

        private void ClosePicBox_Click(object sender, EventArgs e)
        {
            if (GlobalData.FormSetting == true && GlobalData.FormSchedule == true && GlobalData.FormMail == true && GlobalData.FormLog == true)
            {
                DialogResult = DialogResult.OK;
            }
            else
            {
                if (GlobalData.FormSetting == false)
                    MessageBox.Show("Main Setting Error !");

                if (GlobalData.FormSchedule == false)
                    MessageBox.Show("Schedule Setting Error !");

                if (GlobalData.FormMail == false)
                    MessageBox.Show("Mail Setting Error !");

                if (GlobalData.FormLog == false)
                    MessageBox.Show("Log Setting Error !");
            }
        }
        #endregion

        Setting FormSetting = new Setting
        {
            TopLevel = false,
            Dock = DockStyle.Fill
        };

        FormSchedule FormSchedule = new FormSchedule
        {
            TopLevel = false,
            Dock = DockStyle.Fill
        };

        FormMail FormMail = new FormMail
        {
            TopLevel = false,
            Dock = DockStyle.Fill
        };

        FormLog FormLog = new FormLog
        {
            TopLevel = false,
            Dock = DockStyle.Fill
        };

        private void FormTabControl_Load(object sender, EventArgs e)
        {
            FormSetting.Show();
            tabControl.TabPages[0].Controls.Add(FormSetting);

            FormSchedule.Show();
            tabControl.TabPages[1].Controls.Add(FormSchedule);

            FormMail.Show();
            tabControl.TabPages[2].Controls.Add(FormMail);

            if (ini12.INIRead(MainSettingPath, "Device", "Software", "") == "All")
            {
                FormLog.Show();
                tabControl.TabPages[3].Controls.Add(FormLog);
            }
        }

        private void FormTabControl_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (GlobalData.FormSetting == true &&
                GlobalData.FormSchedule == true &&
                GlobalData.FormMail == true &&
                GlobalData.FormLog == true)
            {
                DialogResult = DialogResult.OK;
            }
            else
            {
                if (GlobalData.FormSetting == false)
                {
                    MessageBox.Show("Settings are not saved.", "Main Setting", MessageBoxButtons.OK, MessageBoxIcon.Question);
                    tabControl.SelectedTab = tabPage_MainSetting;
                }

                if (GlobalData.FormSchedule == false)
                {
                    MessageBox.Show("Settings are not saved.", "Schedule", MessageBoxButtons.OK, MessageBoxIcon.Question);
                    tabControl.SelectedTab = tabPage_MultiSchedule;
                }

                if (GlobalData.FormMail == false)
                {
                    MessageBox.Show("Settings are not saved.", "Mail", MessageBoxButtons.OK, MessageBoxIcon.Question);
                    tabControl.SelectedTab = tabPage_Mail;
                }

                if (GlobalData.FormLog == false)
                {
                    MessageBox.Show("Settings are not saved.", "Keyword", MessageBoxButtons.OK, MessageBoxIcon.Question);
                    tabControl.SelectedTab = tabPage_KeywordSearch;
                }

                e.Cancel = true;
            }

            checkSimilarity(e);
            checkLoopIsEmpty(e);
            checkBaudRateIsEmpty(e);
            checkMailReceiver(e);
            checkErrorMessage(e);
        }

        private void checkErrorMessage(FormClosingEventArgs e)
        {
            if (FormSetting.label_ErrorMessage.Text != "" || FormSchedule.label_ErrorMessage.Text != "" || FormMail.label_ErrorMessage.Text != "")
            {
                MessageBox.Show("Please check if there is any error in Settings.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void checkSimilarity(FormClosingEventArgs e)
        {
            if (FormSchedule.checkBox_Similarity.Checked == true && FormSchedule.comboBox_Similarity.SelectedItem == null)
            {
                e.Cancel = true;
                FormSchedule.label_ErrorMessage.Text = "Please select the percentage of Similarity!"; ;
            }
        }

        private void checkLoopIsEmpty(FormClosingEventArgs e)
        {
            List<TextBox> scheduleTextBoxList = new List<TextBox> { FormSchedule.textBox_Schedule1, FormSchedule.textBox_Schedule2, FormSchedule.textBox_Schedule3,
                FormSchedule.textBox_Schedule4, FormSchedule.textBox_Schedule5 };
            List<TextBox> loopTextBoxList = new List<TextBox> { FormSchedule.textBox_Schedule1Loop, FormSchedule.textBox_Schedule2Loop, FormSchedule.textBox_Schedule3Loop,
                FormSchedule.textBox_Schedule4Loop, FormSchedule.textBox_Schedule5Loop };

            var scheduleAndLoop = scheduleTextBoxList.Zip(loopTextBoxList, (s, l) => new { schedule = s, loop = l });
            foreach(var number in scheduleAndLoop)
            {
                if (String.IsNullOrEmpty(number.schedule.Text) == false && (String.IsNullOrEmpty(number.loop.Text) == true || number.loop.Text == "0"))
                {
                    e.Cancel = true;
                    FormSchedule.label_ErrorMessage.Text = "Please enter loop number!";
                }
            }
        }

        private void checkBaudRateIsEmpty(FormClosingEventArgs e)
        {
            List<CheckBox> portCheckBoxList = new List<CheckBox> { FormSetting.checkBox_SerialPort1, FormSetting.checkBox_SerialPort2, FormSetting.checkBox_SerialPort3,
                FormSetting.checkBox_SerialPort4, FormSetting.checkBox_SerialPort5, FormSetting.checkBox_CAN_Log };
            List<ComboBox> baudRateComboBoxList = new List<ComboBox> { FormSetting.comboBox_SerialPort1_BaudRate_Value, FormSetting.comboBox_SerialPort2_BaudRate_Value,
                FormSetting.comboBox_SerialPort3_BaudRate_Value, FormSetting.comboBox_SerialPort4_BaudRate_Value, FormSetting.comboBox_SerialPort5_BaudRate_Value, FormSetting.comboBox_CAN_BaudRate };

            var portAndBaudRate = portCheckBoxList.Zip(baudRateComboBoxList, (p, b) => new { port = p, baudRate = b });
            foreach (var number in portAndBaudRate)
            {
                if (number.port.Checked == true && number.baudRate.SelectedItem == null)
                {
                    e.Cancel = true;
                    FormSetting.label_ErrorMessage.Text = "Please select a proper Baud Rate!";
                }
            }
        }

        private void checkMailReceiver(FormClosingEventArgs e)
        {
            if (FormMail.SendMailcheckBox.Checked == true && string.IsNullOrEmpty(FormMail.textBox_To.Text))
            {
                e.Cancel = true;
                FormMail.label_ErrorMessage.Text = "Receiver cannot be empty!";
            }
        }

        string MainSettingPath = GlobalData.MainSettingPath;    //Application.StartupPath + "\\Config.ini";
        private void FormTabControl_FormClosed(object sender, FormClosedEventArgs e)
        {
            string portLabel_A = "Port A", portLabel_B = "Port B", portLabel_C = "Port C", portLabel_D = "Port D", portLabel_E = "Port E", portLabel_K = "Kline", portLabel_Arduino = "Arduino";
            string[] labelArray = new string[7] { portLabel_A, portLabel_B, portLabel_C, portLabel_D, portLabel_E, portLabel_K, portLabel_Arduino };
            string[] configArray = new string[7] { "PortA", "PortB", "PortC", "PortD", "PortE", "Kline", "Arduino" };
            List <bool> chkValList = new List<bool> { FormSetting.checkBox_SerialPort1.Checked, FormSetting.checkBox_SerialPort2.Checked, FormSetting.checkBox_SerialPort3.Checked,
                FormSetting.checkBox_SerialPort4.Checked, FormSetting.checkBox_SerialPort5.Checked, FormSetting.checkBox_Kline.Checked, FormSetting.checkBox_Arduino.Checked };
            List<string> portNameList = new List<string> { FormSetting.comboBox_SerialPort1_PortName_Value.Text, FormSetting.comboBox_SerialPort2_PortName_Value.Text,
                FormSetting.comboBox_SerialPort3_PortName_Value.Text, FormSetting.comboBox_SerialPort4_PortName_Value.Text,
                FormSetting.comboBox_SerialPort5_PortName_Value.Text, FormSetting.comboBox_KlinePort_PortName_Value.Text,
				FormSetting.comboBox_ArduinoPort_PortName_Value.Text };
            List<string> portBrList = new List<string> { FormSetting.comboBox_SerialPort1_BaudRate_Value.Text, FormSetting.comboBox_SerialPort2_BaudRate_Value.Text,
                FormSetting.comboBox_SerialPort3_BaudRate_Value.Text, FormSetting.comboBox_SerialPort4_BaudRate_Value.Text,
                FormSetting.comboBox_SerialPort5_BaudRate_Value.Text, FormSetting.comboBox_KlinePort_PortName_Value.Text,
				FormSetting.comboBox_ArduinoPort_PortName_Value.Text };

            //Write Port Config Parameters once exiting TabControl (Setting) Form
            if (GlobalData._portConfigList.Count == labelArray.Length && GlobalData._portConfigList.Count == configArray.Length)
            {
                int i = 0;
                foreach (var portConfig in GlobalData._portConfigList)
                {
                    portConfig.portLabel = labelArray[i];
                    portConfig.checkedValue = chkValList.ElementAt<bool>(i);
                    portConfig.portConfig = configArray[i];
                    portConfig.portName = portNameList.ElementAt<string>(i);
                    portConfig.portBR = portBrList.ElementAt<string>(i);
                    if (!portConfig.checkedValue)
                        ini12.INIWrite(MainSettingPath, portConfig.portLabel, "Checked", "0");
                    else if (portConfig.checkedValue)
                        ini12.INIWrite(MainSettingPath, portConfig.portLabel, "Checked", "1");
                    i++;
                }
            }
            else
            {
                //This is used to check if count of portConfigList is the same as local array size
                Console.WriteLine("[portConfigList] Out of index!!!");
                Application.Exit();
            }

            ini12.INIWrite(MainSettingPath, portLabel_A, "PortName", FormSetting.comboBox_SerialPort1_PortName_Value.Text);
            ini12.INIWrite(MainSettingPath, portLabel_A, "DataBit", "8");
            ini12.INIWrite(MainSettingPath, portLabel_A, "StopBits", "One");
            ini12.INIWrite(MainSettingPath, portLabel_B, "PortName", FormSetting.comboBox_SerialPort2_PortName_Value.Text);
            ini12.INIWrite(MainSettingPath, portLabel_B, "DataBit", "8");
            ini12.INIWrite(MainSettingPath, portLabel_B, "StopBits", "One");
            ini12.INIWrite(MainSettingPath, portLabel_C, "PortName", FormSetting.comboBox_SerialPort3_PortName_Value.Text);
            ini12.INIWrite(MainSettingPath, portLabel_C, "DataBit", "8");
            ini12.INIWrite(MainSettingPath, portLabel_C, "StopBits", "One");
            ini12.INIWrite(MainSettingPath, portLabel_D, "PortName", FormSetting.comboBox_SerialPort4_PortName_Value.Text);
            ini12.INIWrite(MainSettingPath, portLabel_D, "DataBit", "8");
            ini12.INIWrite(MainSettingPath, portLabel_D, "StopBits", "One");
            ini12.INIWrite(MainSettingPath, portLabel_E, "PortName", FormSetting.comboBox_SerialPort5_PortName_Value.Text);
            ini12.INIWrite(MainSettingPath, portLabel_E, "DataBit", "8");
            ini12.INIWrite(MainSettingPath, portLabel_E, "StopBits", "One");
            ini12.INIWrite(MainSettingPath, portLabel_K, "PortName", FormSetting.comboBox_KlinePort_PortName_Value.Text);
            ini12.INIWrite(MainSettingPath, portLabel_Arduino, "PortName", FormSetting.comboBox_ArduinoPort_PortName_Value.Text);
            ini12.INIWrite(MainSettingPath, portLabel_Arduino, "DataBit", "8");
            ini12.INIWrite(MainSettingPath, portLabel_Arduino, "StopBits", "One");
        }
    }
}