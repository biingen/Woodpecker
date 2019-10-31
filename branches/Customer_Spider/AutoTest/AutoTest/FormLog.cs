using jini;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace Woodpecker
{
    public partial class FormLog : Form
    {
        public FormLog()
        {
            InitializeComponent();
        }

        string MainSettingPath = Application.StartupPath + "\\Config.ini";
        string MailPath = Application.StartupPath + "\\Mail.ini";

        public void FormLog_Load(object sender, EventArgs e)
        {
            label_ErrorMessage.Text = "";
            if (ini12.INIRead(MainSettingPath, "LogSearch", "Comport1", "") != "")
            {
                if (int.Parse(ini12.INIRead(MainSettingPath, "LogSearch", "Comport1", "")) == 1)
                {
                    checkBox_SerialPort1.Checked = true;
                }
                else
                {
                    checkBox_SerialPort1.Checked = false;
                }
            }
            else
            {
                ini12.INIWrite(MainSettingPath, "LogSearch", "Comport1", "0");
                checkBox_SerialPort1.Checked = false;
                comboBox_KeywordNum.Text = "0";
            }

            if (ini12.INIRead(MainSettingPath, "LogSearch", "Comport2", "") != "")
            {
                if (int.Parse(ini12.INIRead(MainSettingPath, "LogSearch", "Comport2", "")) == 1)
                {
                    checkBox_SerialPort2.Checked = true;
                }
                else
                {
                    checkBox_SerialPort2.Checked = false;
                }
            }
            else
            {
                ini12.INIWrite(MainSettingPath, "LogSearch", "Comport2", "0");
                checkBox_SerialPort2.Checked = false;
                comboBox_KeywordNum.Text = "0";
            }

            if (ini12.INIRead(MainSettingPath, "LogSearch", "Comport3", "") != "")
            {
                if (int.Parse(ini12.INIRead(MainSettingPath, "LogSearch", "Comport3", "")) == 1)
                {
                    checkBox_SerialPort3.Checked = true;
                }
                else
                {
                    checkBox_SerialPort3.Checked = false;
                }
            }
            else
            {
                ini12.INIWrite(MainSettingPath, "LogSearch", "Comport3", "0");
                checkBox_SerialPort3.Checked = false;
                comboBox_KeywordNum.Text = "0";
            }

            if (checkBox_SerialPort1.Checked == false && checkBox_SerialPort2.Checked == false && checkBox_SerialPort3.Checked == false)
            {
                comboBox_KeywordNum.Enabled = false;
            }

            if (ini12.INIRead(MainSettingPath, "LogSearch", "TextNum", "") != "")
            {
                comboBox_KeywordNum.Text = ini12.INIRead(MainSettingPath, "LogSearch", "TextNum", "");
                int caseSwitch = Convert.ToInt32(ini12.INIRead(MainSettingPath, "LogSearch", "TextNum", ""));
                switch (caseSwitch)
                {
                    case 0:
                        label_KeywordNum.Visible = true;
                        comboBox_KeywordNum.Visible = true;
                        label_Keyword.Visible = false;
                        label_Times.Visible = false;
                        label_Keyword0.Visible = false;
                        textBox_Keyword0.Visible = false;
                        textBox_Times0.Visible = false;
                        label_Keyword1.Visible = false;
                        textBox_Keyword1.Visible = false;
                        textBox_Times1.Visible = false;
                        label_Keyword2.Visible = false;
                        textBox_Keyword2.Visible = false;
                        textBox_Times2.Visible = false;
                        label_Keyword3.Visible = false;
                        textBox_Keyword3.Visible = false;
                        textBox_Times3.Visible = false;
                        label_Keyword4.Visible = false;
                        textBox_Keyword4.Visible = false;
                        textBox_Times4.Visible = false;
                        label_Keyword5.Visible = false;
                        textBox_Keyword5.Visible = false;
                        textBox_Times5.Visible = false;
                        label_Keyword6.Visible = false;
                        textBox_Keyword6.Visible = false;
                        textBox_Times6.Visible = false;
                        label_Keyword7.Visible = false;
                        textBox_Keyword7.Visible = false;
                        textBox_Times7.Visible = false;
                        label_Keyword8.Visible = false;
                        textBox_Keyword8.Visible = false;
                        textBox_Times8.Visible = false;
                        label_Keyword9.Visible = false;
                        textBox_Keyword9.Visible = false;
                        textBox_Times9.Visible = false;
                        break;

                    case 1:
                        label_KeywordNum.Visible = true;
                        comboBox_KeywordNum.Visible = true;
                        label_Keyword.Visible = true;
                        label_Times.Visible = true;
                        label_Keyword0.Visible = true;
                        textBox_Keyword0.Visible = true;
                        textBox_Times0.Visible = true;
                        label_Keyword1.Visible = false;
                        textBox_Keyword1.Visible = false;
                        textBox_Times1.Visible = false;
                        label_Keyword2.Visible = false;
                        textBox_Keyword2.Visible = false;
                        textBox_Times2.Visible = false;
                        label_Keyword3.Visible = false;
                        textBox_Keyword3.Visible = false;
                        textBox_Times3.Visible = false;
                        label_Keyword4.Visible = false;
                        textBox_Keyword4.Visible = false;
                        textBox_Times4.Visible = false;
                        label_Keyword5.Visible = false;
                        textBox_Keyword5.Visible = false;
                        textBox_Times5.Visible = false;
                        label_Keyword6.Visible = false;
                        textBox_Keyword6.Visible = false;
                        textBox_Times6.Visible = false;
                        label_Keyword7.Visible = false;
                        textBox_Keyword7.Visible = false;
                        textBox_Times7.Visible = false;
                        label_Keyword8.Visible = false;
                        textBox_Keyword8.Visible = false;
                        textBox_Times8.Visible = false;
                        label_Keyword9.Visible = false;
                        textBox_Keyword9.Visible = false;
                        textBox_Times9.Visible = false;
                        break;

                    case 2:
                        label_KeywordNum.Visible = true;
                        comboBox_KeywordNum.Visible = true;
                        label_Keyword.Visible = true;
                        label_Times.Visible = true;
                        label_Keyword0.Visible = true;
                        textBox_Keyword0.Visible = true;
                        textBox_Times0.Visible = true;
                        label_Keyword1.Visible = true;
                        textBox_Keyword1.Visible = true;
                        textBox_Times1.Visible = true;
                        label_Keyword2.Visible = false;
                        textBox_Keyword2.Visible = false;
                        textBox_Times2.Visible = false;
                        label_Keyword3.Visible = false;
                        textBox_Keyword3.Visible = false;
                        textBox_Times3.Visible = false;
                        label_Keyword4.Visible = false;
                        textBox_Keyword4.Visible = false;
                        textBox_Times4.Visible = false;
                        label_Keyword5.Visible = false;
                        textBox_Keyword5.Visible = false;
                        textBox_Times5.Visible = false;
                        label_Keyword6.Visible = false;
                        textBox_Keyword6.Visible = false;
                        textBox_Times6.Visible = false;
                        label_Keyword7.Visible = false;
                        textBox_Keyword7.Visible = false;
                        textBox_Times7.Visible = false;
                        label_Keyword8.Visible = false;
                        textBox_Keyword8.Visible = false;
                        textBox_Times8.Visible = false;
                        label_Keyword9.Visible = false;
                        textBox_Keyword9.Visible = false;
                        textBox_Times9.Visible = false;
                        break;

                    case 3:
                        label_KeywordNum.Visible = true;
                        comboBox_KeywordNum.Visible = true;
                        label_Keyword.Visible = true;
                        label_Times.Visible = true;
                        label_Keyword0.Visible = true;
                        textBox_Keyword0.Visible = true;
                        textBox_Times0.Visible = true;
                        label_Keyword1.Visible = true;
                        textBox_Keyword1.Visible = true;
                        textBox_Times1.Visible = true;
                        label_Keyword2.Visible = true;
                        textBox_Keyword2.Visible = true;
                        textBox_Times2.Visible = true;
                        label_Keyword3.Visible = false;
                        textBox_Keyword3.Visible = false;
                        textBox_Times3.Visible = false;
                        label_Keyword4.Visible = false;
                        textBox_Keyword4.Visible = false;
                        textBox_Times4.Visible = false;
                        label_Keyword5.Visible = false;
                        textBox_Keyword5.Visible = false;
                        textBox_Times5.Visible = false;
                        label_Keyword6.Visible = false;
                        textBox_Keyword6.Visible = false;
                        textBox_Times6.Visible = false;
                        label_Keyword7.Visible = false;
                        textBox_Keyword7.Visible = false;
                        textBox_Times7.Visible = false;
                        label_Keyword8.Visible = false;
                        textBox_Keyword8.Visible = false;
                        textBox_Times8.Visible = false;
                        label_Keyword9.Visible = false;
                        textBox_Keyword9.Visible = false;
                        textBox_Times9.Visible = false;
                        break;

                    case 4:
                        label_KeywordNum.Visible = true;
                        comboBox_KeywordNum.Visible = true;
                        label_Keyword.Visible = true;
                        label_Times.Visible = true;
                        label_Keyword0.Visible = true;
                        textBox_Keyword0.Visible = true;
                        textBox_Times0.Visible = true;
                        label_Keyword1.Visible = true;
                        textBox_Keyword1.Visible = true;
                        textBox_Times1.Visible = true;
                        label_Keyword2.Visible = true;
                        textBox_Keyword2.Visible = true;
                        textBox_Times2.Visible = true;
                        label_Keyword3.Visible = true;
                        textBox_Keyword3.Visible = true;
                        textBox_Times3.Visible = true;
                        label_Keyword4.Visible = false;
                        textBox_Keyword4.Visible = false;
                        textBox_Times4.Visible = false;
                        label_Keyword5.Visible = false;
                        textBox_Keyword5.Visible = false;
                        textBox_Times5.Visible = false;
                        label_Keyword6.Visible = false;
                        textBox_Keyword6.Visible = false;
                        textBox_Times6.Visible = false;
                        label_Keyword7.Visible = false;
                        textBox_Keyword7.Visible = false;
                        textBox_Times7.Visible = false;
                        label_Keyword8.Visible = false;
                        textBox_Keyword8.Visible = false;
                        textBox_Times8.Visible = false;
                        label_Keyword9.Visible = false;
                        textBox_Keyword9.Visible = false;
                        textBox_Times9.Visible = false;
                        break;

                    case 5:
                        label_KeywordNum.Visible = true;
                        comboBox_KeywordNum.Visible = true;
                        label_Keyword.Visible = true;
                        label_Times.Visible = true;
                        label_Keyword0.Visible = true;
                        textBox_Keyword0.Visible = true;
                        textBox_Times0.Visible = true;
                        label_Keyword1.Visible = true;
                        textBox_Keyword1.Visible = true;
                        textBox_Times1.Visible = true;
                        label_Keyword2.Visible = true;
                        textBox_Keyword2.Visible = true;
                        textBox_Times2.Visible = true;
                        label_Keyword3.Visible = true;
                        textBox_Keyword3.Visible = true;
                        textBox_Times3.Visible = true;
                        label_Keyword4.Visible = true;
                        textBox_Keyword4.Visible = true;
                        textBox_Times4.Visible = true;
                        label_Keyword5.Visible = false;
                        textBox_Keyword5.Visible = false;
                        textBox_Times5.Visible = false;
                        label_Keyword6.Visible = false;
                        textBox_Keyword6.Visible = false;
                        textBox_Times6.Visible = false;
                        label_Keyword7.Visible = false;
                        textBox_Keyword7.Visible = false;
                        textBox_Times7.Visible = false;
                        label_Keyword8.Visible = false;
                        textBox_Keyword8.Visible = false;
                        textBox_Times8.Visible = false;
                        label_Keyword9.Visible = false;
                        textBox_Keyword9.Visible = false;
                        textBox_Times9.Visible = false;
                        break;

                    case 6:
                        label_KeywordNum.Visible = true;
                        comboBox_KeywordNum.Visible = true;
                        label_Keyword.Visible = true;
                        label_Times.Visible = true;
                        label_Keyword0.Visible = true;
                        textBox_Keyword0.Visible = true;
                        textBox_Times0.Visible = true;
                        label_Keyword1.Visible = true;
                        textBox_Keyword1.Visible = true;
                        textBox_Times1.Visible = true;
                        label_Keyword2.Visible = true;
                        textBox_Keyword2.Visible = true;
                        textBox_Times2.Visible = true;
                        label_Keyword3.Visible = true;
                        textBox_Keyword3.Visible = true;
                        textBox_Times3.Visible = true;
                        label_Keyword4.Visible = true;
                        textBox_Keyword4.Visible = true;
                        textBox_Times4.Visible = true;
                        label_Keyword5.Visible = true;
                        textBox_Keyword5.Visible = true;
                        textBox_Times5.Visible = true;
                        label_Keyword6.Visible = false;
                        textBox_Keyword6.Visible = false;
                        textBox_Times6.Visible = false;
                        label_Keyword7.Visible = false;
                        textBox_Keyword7.Visible = false;
                        textBox_Times7.Visible = false;
                        label_Keyword8.Visible = false;
                        textBox_Keyword8.Visible = false;
                        textBox_Times8.Visible = false;
                        label_Keyword9.Visible = false;
                        textBox_Keyword9.Visible = false;
                        textBox_Times9.Visible = false;
                        break;

                    case 7:
                        label_KeywordNum.Visible = true;
                        comboBox_KeywordNum.Visible = true;
                        label_Keyword.Visible = true;
                        label_Times.Visible = true;
                        label_Keyword0.Visible = true;
                        textBox_Keyword0.Visible = true;
                        textBox_Times0.Visible = true;
                        label_Keyword1.Visible = true;
                        textBox_Keyword1.Visible = true;
                        textBox_Times1.Visible = true;
                        label_Keyword2.Visible = true;
                        textBox_Keyword2.Visible = true;
                        textBox_Times2.Visible = true;
                        label_Keyword3.Visible = true;
                        textBox_Keyword3.Visible = true;
                        textBox_Times3.Visible = true;
                        label_Keyword4.Visible = true;
                        textBox_Keyword4.Visible = true;
                        textBox_Times4.Visible = true;
                        label_Keyword5.Visible = true;
                        textBox_Keyword5.Visible = true;
                        textBox_Times5.Visible = true;
                        label_Keyword6.Visible = true;
                        textBox_Keyword6.Visible = true;
                        textBox_Times6.Visible = true;
                        label_Keyword7.Visible = false;
                        textBox_Keyword7.Visible = false;
                        textBox_Times7.Visible = false;
                        label_Keyword8.Visible = false;
                        textBox_Keyword8.Visible = false;
                        textBox_Times8.Visible = false;
                        label_Keyword9.Visible = false;
                        textBox_Keyword9.Visible = false;
                        textBox_Times9.Visible = false;
                        break;

                    case 8:
                        label_KeywordNum.Visible = true;
                        comboBox_KeywordNum.Visible = true;
                        label_Keyword.Visible = true;
                        label_Times.Visible = true;
                        label_Keyword0.Visible = true;
                        textBox_Keyword0.Visible = true;
                        textBox_Times0.Visible = true;
                        label_Keyword1.Visible = true;
                        textBox_Keyword1.Visible = true;
                        textBox_Times1.Visible = true;
                        label_Keyword2.Visible = true;
                        textBox_Keyword2.Visible = true;
                        textBox_Times2.Visible = true;
                        label_Keyword3.Visible = true;
                        textBox_Keyword3.Visible = true;
                        textBox_Times3.Visible = true;
                        label_Keyword4.Visible = true;
                        textBox_Keyword4.Visible = true;
                        textBox_Times4.Visible = true;
                        label_Keyword5.Visible = true;
                        textBox_Keyword5.Visible = true;
                        textBox_Times5.Visible = true;
                        label_Keyword6.Visible = true;
                        textBox_Keyword6.Visible = true;
                        textBox_Times6.Visible = true;
                        label_Keyword7.Visible = true;
                        textBox_Keyword7.Visible = true;
                        textBox_Times7.Visible = true;
                        label_Keyword8.Visible = false;
                        textBox_Keyword8.Visible = false;
                        textBox_Times8.Visible = false;
                        label_Keyword9.Visible = false;
                        textBox_Keyword9.Visible = false;
                        textBox_Times9.Visible = false;
                        break;

                    case 9:
                        label_KeywordNum.Visible = true;
                        comboBox_KeywordNum.Visible = true;
                        label_Keyword.Visible = true;
                        label_Times.Visible = true;
                        label_Keyword0.Visible = true;
                        textBox_Keyword0.Visible = true;
                        textBox_Times0.Visible = true;
                        label_Keyword1.Visible = true;
                        textBox_Keyword1.Visible = true;
                        textBox_Times1.Visible = true;
                        label_Keyword2.Visible = true;
                        textBox_Keyword2.Visible = true;
                        textBox_Times2.Visible = true;
                        label_Keyword3.Visible = true;
                        textBox_Keyword3.Visible = true;
                        textBox_Times3.Visible = true;
                        label_Keyword4.Visible = true;
                        textBox_Keyword4.Visible = true;
                        textBox_Times4.Visible = true;
                        label_Keyword5.Visible = true;
                        textBox_Keyword5.Visible = true;
                        textBox_Times5.Visible = true;
                        label_Keyword6.Visible = true;
                        textBox_Keyword6.Visible = true;
                        textBox_Times6.Visible = true;
                        label_Keyword7.Visible = true;
                        textBox_Keyword7.Visible = true;
                        textBox_Times7.Visible = true;
                        label_Keyword8.Visible = true;
                        textBox_Keyword8.Visible = true;
                        textBox_Times8.Visible = true;
                        label_Keyword9.Visible = false;
                        textBox_Keyword9.Visible = false;
                        textBox_Times9.Visible = false;
                        break;

                    case 10:
                        label_KeywordNum.Visible = true;
                        comboBox_KeywordNum.Visible = true;
                        label_Keyword.Visible = true;
                        label_Times.Visible = true;
                        label_Keyword0.Visible = true;
                        textBox_Keyword0.Visible = true;
                        textBox_Times0.Visible = true;
                        label_Keyword1.Visible = true;
                        textBox_Keyword1.Visible = true;
                        textBox_Times1.Visible = true;
                        label_Keyword2.Visible = true;
                        textBox_Keyword2.Visible = true;
                        textBox_Times2.Visible = true;
                        label_Keyword3.Visible = true;
                        textBox_Keyword3.Visible = true;
                        textBox_Times3.Visible = true;
                        label_Keyword4.Visible = true;
                        textBox_Keyword4.Visible = true;
                        textBox_Times4.Visible = true;
                        label_Keyword5.Visible = true;
                        textBox_Keyword5.Visible = true;
                        textBox_Times5.Visible = true;
                        label_Keyword6.Visible = true;
                        textBox_Keyword6.Visible = true;
                        textBox_Times6.Visible = true;
                        label_Keyword7.Visible = true;
                        textBox_Keyword7.Visible = true;
                        textBox_Times7.Visible = true;
                        label_Keyword8.Visible = true;
                        textBox_Keyword8.Visible = true;
                        textBox_Times8.Visible = true;
                        label_Keyword9.Visible = true;
                        textBox_Keyword9.Visible = true;
                        textBox_Times9.Visible = true;
                        break;
                }
            }
            else
            {
                ini12.INIWrite(MainSettingPath, "LogSearch", "TextNum", "0");
                label_KeywordNum.Visible = true;
                comboBox_KeywordNum.Visible = true;
                label_Keyword.Visible = false;
                label_Times.Visible = false;
                label_Keyword0.Visible = false;
                textBox_Keyword0.Visible = false;
                textBox_Times0.Visible = false;
                label_Keyword1.Visible = false;
                textBox_Keyword1.Visible = false;
                textBox_Times1.Visible = false;
                label_Keyword2.Visible = false;
                textBox_Keyword2.Visible = false;
                textBox_Times2.Visible = false;
                label_Keyword3.Visible = false;
                textBox_Keyword3.Visible = false;
                textBox_Times3.Visible = false;
                label_Keyword4.Visible = false;
                textBox_Keyword4.Visible = false;
                textBox_Times4.Visible = false;
                label_Keyword5.Visible = false;
                textBox_Keyword5.Visible = false;
                textBox_Times5.Visible = false;
                label_Keyword6.Visible = false;
                textBox_Keyword6.Visible = false;
                textBox_Times6.Visible = false;
                label_Keyword7.Visible = false;
                textBox_Keyword7.Visible = false;
                textBox_Times7.Visible = false;
                label_Keyword8.Visible = false;
                textBox_Keyword8.Visible = false;
                textBox_Times8.Visible = false;
                label_Keyword9.Visible = false;
                textBox_Keyword9.Visible = false;
                textBox_Times9.Visible = false;
            }

            textBox_Keyword0.Text = ini12.INIRead(MainSettingPath, "LogSearch", "Text0", "");
            textBox_Times0.Text = ini12.INIRead(MainSettingPath, "LogSearch", "Times0", "");
            textBox_Keyword1.Text = ini12.INIRead(MainSettingPath, "LogSearch", "Text1", "");
            textBox_Times1.Text = ini12.INIRead(MainSettingPath, "LogSearch", "Times1", "");
            textBox_Keyword2.Text = ini12.INIRead(MainSettingPath, "LogSearch", "Text2", "");
            textBox_Times2.Text = ini12.INIRead(MainSettingPath, "LogSearch", "Times2", "");
            textBox_Keyword3.Text = ini12.INIRead(MainSettingPath, "LogSearch", "Text3", "");
            textBox_Times3.Text = ini12.INIRead(MainSettingPath, "LogSearch", "Times3", "");
            textBox_Keyword4.Text = ini12.INIRead(MainSettingPath, "LogSearch", "Text4", "");
            textBox_Times4.Text = ini12.INIRead(MainSettingPath, "LogSearch", "Times4", "");
            textBox_Keyword5.Text = ini12.INIRead(MainSettingPath, "LogSearch", "Text5", "");
            textBox_Times5.Text = ini12.INIRead(MainSettingPath, "LogSearch", "Times5", "");
            textBox_Keyword6.Text = ini12.INIRead(MainSettingPath, "LogSearch", "Text6", "");
            textBox_Times6.Text = ini12.INIRead(MainSettingPath, "LogSearch", "Times6", "");
            textBox_Keyword7.Text = ini12.INIRead(MainSettingPath, "LogSearch", "Text7", "");
            textBox_Times7.Text = ini12.INIRead(MainSettingPath, "LogSearch", "Times7", "");
            textBox_Keyword8.Text = ini12.INIRead(MainSettingPath, "LogSearch", "Text8", "");
            textBox_Times8.Text = ini12.INIRead(MainSettingPath, "LogSearch", "Times8", "");
            textBox_Keyword9.Text = ini12.INIRead(MainSettingPath, "LogSearch", "Text9", "");
            textBox_Times9.Text = ini12.INIRead(MainSettingPath, "LogSearch", "Times9", "");
        }

        private void SaveBtn_Click(object sender, EventArgs e)
        {
            ini12.INIWrite(MainSettingPath, "LogSearch", "TextNum", comboBox_KeywordNum.Text);

            if (textBox_Keyword0.Visible == true)
            {
                if (string.IsNullOrEmpty(textBox_Keyword0.Text))
                {
                    textBox_Keyword0.BackColor = System.Drawing.Color.Yellow;
                    label_ErrorMessage.Text = "Keyword cannot be empty!";
                    Global.FormLog = false;
                }
                else if (string.IsNullOrEmpty(textBox_Times0.Text) || textBox_Times0.Text == "0")
                {
                    textBox_Times0.BackColor = System.Drawing.Color.Yellow;
                    label_ErrorMessage.Text = "Times cannot be empty!";
                    Global.FormLog = false;
                }
                else
                {
                    textBox_Keyword0.BackColor = default(Color);
                    textBox_Times0.BackColor = default(Color);
                    ini12.INIWrite(MainSettingPath, "LogSearch", "Text0", textBox_Keyword0.Text.Trim());
                    ini12.INIWrite(MainSettingPath, "LogSearch", "Times0", textBox_Times0.Text.Trim());
                }
            }
            else
            {
                textBox_Keyword0.BackColor = default(Color);
                textBox_Times0.BackColor = default(Color);
            }

            if (textBox_Keyword1.Visible == true)
            {
                if (string.IsNullOrEmpty(textBox_Keyword1.Text))
                {
                    textBox_Keyword1.BackColor = System.Drawing.Color.Yellow;
                    label_ErrorMessage.Text = "Keyword cannot be empty!";
                    Global.FormLog = false;
                }
                else if (string.IsNullOrEmpty(textBox_Times1.Text) || textBox_Times1.Text == "0")
                {
                    textBox_Times1.BackColor = System.Drawing.Color.Yellow;
                    label_ErrorMessage.Text = "Times cannot be empty!";
                    Global.FormLog = false;
                }
                else
                {
                    textBox_Keyword1.BackColor = default(Color);
                    textBox_Times1.BackColor = default(Color);
                    ini12.INIWrite(MainSettingPath, "LogSearch", "Text1", textBox_Keyword1.Text.Trim());
                    ini12.INIWrite(MainSettingPath, "LogSearch", "Times1", textBox_Times1.Text.Trim());
                }
            }
            else
            {
                textBox_Keyword1.BackColor = default(Color);
                textBox_Times1.BackColor = default(Color);
            }

            if (textBox_Keyword2.Visible == true)
            {
                if (string.IsNullOrEmpty(textBox_Keyword2.Text))
                {
                    textBox_Keyword2.BackColor = System.Drawing.Color.Yellow;
                    label_ErrorMessage.Text = "Keyword cannot be empty!";
                    Global.FormLog = false;
                }
                else if (string.IsNullOrEmpty(textBox_Times2.Text) || textBox_Times2.Text == "0")
                {
                    textBox_Times2.BackColor = System.Drawing.Color.Yellow;
                    label_ErrorMessage.Text = "Times cannot be empty!";
                    Global.FormLog = false;
                }
                else
                {
                    textBox_Keyword2.BackColor = default(Color);
                    textBox_Times2.BackColor = default(Color);
                    ini12.INIWrite(MainSettingPath, "LogSearch", "Text2", textBox_Keyword2.Text.Trim());
                    ini12.INIWrite(MainSettingPath, "LogSearch", "Times2", textBox_Times2.Text.Trim());
                }
            }
            else
            {
                textBox_Keyword2.BackColor = default(Color);
                textBox_Times2.BackColor = default(Color);
            }

            if (textBox_Keyword3.Visible == true)
            {
                if (string.IsNullOrEmpty(textBox_Keyword3.Text))
                {
                    textBox_Keyword3.BackColor = System.Drawing.Color.Yellow;
                    label_ErrorMessage.Text = "Keyword cannot be empty!";
                    Global.FormLog = false;
                }
                else if (string.IsNullOrEmpty(textBox_Times3.Text) || textBox_Times3.Text == "0")
                {
                    textBox_Times3.BackColor = System.Drawing.Color.Yellow;
                    label_ErrorMessage.Text = "Times cannot be empty!";
                    Global.FormLog = false;
                }
                else
                {
                    textBox_Keyword3.BackColor = default(Color);
                    textBox_Times3.BackColor = default(Color);
                    ini12.INIWrite(MainSettingPath, "LogSearch", "Text3", textBox_Keyword3.Text.Trim());
                    ini12.INIWrite(MainSettingPath, "LogSearch", "Times3", textBox_Times3.Text.Trim());
                }
            }
            else
            {
                textBox_Keyword3.BackColor = default(Color);
                textBox_Times3.BackColor = default(Color);
            }

            if (textBox_Keyword4.Visible == true)
            {
                if (string.IsNullOrEmpty(textBox_Keyword4.Text))
                {
                    textBox_Keyword4.BackColor = System.Drawing.Color.Yellow;
                    label_ErrorMessage.Text = "Keyword cannot be empty!";
                    Global.FormLog = false;
                }
                else if (string.IsNullOrEmpty(textBox_Times4.Text) || textBox_Times4.Text == "0")
                {
                    textBox_Times4.BackColor = System.Drawing.Color.Yellow;
                    label_ErrorMessage.Text = "Times cannot be empty!";
                    Global.FormLog = false;
                }
                else
                {
                    textBox_Keyword4.BackColor = default(Color);
                    textBox_Times4.BackColor = default(Color);
                    ini12.INIWrite(MainSettingPath, "LogSearch", "Text4", textBox_Keyword4.Text.Trim());
                    ini12.INIWrite(MainSettingPath, "LogSearch", "Times4", textBox_Times4.Text.Trim());
                }
            }
            else
            {
                textBox_Keyword4.BackColor = default(Color);
                textBox_Times4.BackColor = default(Color);
            }

            if (textBox_Keyword5.Visible == true)
            {
                if (string.IsNullOrEmpty(textBox_Keyword5.Text))
                {
                    textBox_Keyword5.BackColor = System.Drawing.Color.Yellow;
                    label_ErrorMessage.Text = "Keyword cannot be empty!";
                    Global.FormLog = false;
                }
                else if (string.IsNullOrEmpty(textBox_Times5.Text) || textBox_Times5.Text == "0")
                {
                    textBox_Times5.BackColor = System.Drawing.Color.Yellow;
                    label_ErrorMessage.Text = "Times cannot be empty!";
                    Global.FormLog = false;
                }
                else
                {
                    textBox_Keyword5.BackColor = default(Color);
                    textBox_Times5.BackColor = default(Color);
                    ini12.INIWrite(MainSettingPath, "LogSearch", "Text5", textBox_Keyword5.Text.Trim());
                    ini12.INIWrite(MainSettingPath, "LogSearch", "Times5", textBox_Times5.Text.Trim());
                }
            }
            else
            {
                textBox_Keyword5.BackColor = default(Color);
                textBox_Times5.BackColor = default(Color);
            }

            if (textBox_Keyword6.Visible == true)
            {
                if (string.IsNullOrEmpty(textBox_Keyword6.Text))
                {
                    textBox_Keyword6.BackColor = System.Drawing.Color.Yellow;
                    label_ErrorMessage.Text = "Keyword cannot be empty!";
                    Global.FormLog = false;
                }
                else if (string.IsNullOrEmpty(textBox_Times6.Text) || textBox_Times6.Text == "0")
                {
                    textBox_Times6.BackColor = System.Drawing.Color.Yellow;
                    label_ErrorMessage.Text = "Times cannot be empty!";
                    Global.FormLog = false;
                }
                else
                {
                    textBox_Keyword6.BackColor = default(Color);
                    textBox_Times6.BackColor = default(Color);
                    ini12.INIWrite(MainSettingPath, "LogSearch", "Text6", textBox_Keyword6.Text.Trim());
                    ini12.INIWrite(MainSettingPath, "LogSearch", "Times6", textBox_Times6.Text.Trim());
                }
            }
            else
            {
                textBox_Keyword6.BackColor = default(Color);
                textBox_Times6.BackColor = default(Color);
            }

            if (textBox_Keyword7.Visible == true)
            {
                if (string.IsNullOrEmpty(textBox_Keyword7.Text))
                {
                    textBox_Keyword7.BackColor = System.Drawing.Color.Yellow;
                    label_ErrorMessage.Text = "Keyword cannot be empty!";
                    Global.FormLog = false;
                }
                else if (string.IsNullOrEmpty(textBox_Times7.Text) || textBox_Times7.Text == "0")
                {
                    textBox_Times7.BackColor = System.Drawing.Color.Yellow;
                    label_ErrorMessage.Text = "Times cannot be empty!";
                    Global.FormLog = false;
                }
                else
                {
                    textBox_Keyword7.BackColor = default(Color);
                    textBox_Times7.BackColor = default(Color);
                    ini12.INIWrite(MainSettingPath, "LogSearch", "Text7", textBox_Keyword7.Text.Trim());
                    ini12.INIWrite(MainSettingPath, "LogSearch", "Times7", textBox_Times7.Text.Trim());
                }
            }
            else
            {
                textBox_Keyword7.BackColor = default(Color);
                textBox_Times7.BackColor = default(Color);
            }

            if (textBox_Keyword8.Visible == true)
            {
                if (string.IsNullOrEmpty(textBox_Keyword8.Text))
                {
                    textBox_Keyword8.BackColor = System.Drawing.Color.Yellow;
                    label_ErrorMessage.Text = "Keyword cannot be empty!";
                    Global.FormLog = false;
                }
                else if (string.IsNullOrEmpty(textBox_Times8.Text) || textBox_Times8.Text == "0")
                {
                    textBox_Times8.BackColor = System.Drawing.Color.Yellow;
                    label_ErrorMessage.Text = "Times cannot be empty!";
                    Global.FormLog = false;
                }
                else
                {
                    textBox_Keyword8.BackColor = default(Color);
                    textBox_Times8.BackColor = default(Color);
                    ini12.INIWrite(MainSettingPath, "LogSearch", "Text8", textBox_Keyword8.Text.Trim());
                    ini12.INIWrite(MainSettingPath, "LogSearch", "Times8", textBox_Times8.Text.Trim());
                }
            }
            else
            {
                textBox_Keyword8.BackColor = default(Color);
                textBox_Times8.BackColor = default(Color);
            }

            if (textBox_Keyword9.Visible == true)
            {
                if (string.IsNullOrEmpty(textBox_Keyword9.Text))
                {
                    textBox_Keyword9.BackColor = System.Drawing.Color.Yellow;
                    label_ErrorMessage.Text = "Keyword cannot be empty!";
                    Global.FormLog = false;
                }
                else if (string.IsNullOrEmpty(textBox_Times9.Text) || textBox_Times9.Text == "0")
                {
                    textBox_Times9.BackColor = System.Drawing.Color.Yellow;
                    label_ErrorMessage.Text = "Times cannot be empty!";
                    Global.FormLog = false;
                }
                else
                {
                    textBox_Keyword9.BackColor = default(Color);
                    textBox_Times9.BackColor = default(Color);
                    ini12.INIWrite(MainSettingPath, "LogSearch", "Text9", textBox_Keyword9.Text.Trim());
                    ini12.INIWrite(MainSettingPath, "LogSearch", "Times9", textBox_Times9.Text.Trim());
                }
            }
            else
            {
                textBox_Keyword9.BackColor = default(Color);
                textBox_Times9.BackColor = default(Color);
            }

            if (
                textBox_Keyword0.BackColor != System.Drawing.Color.Yellow &&
                textBox_Times0.BackColor != System.Drawing.Color.Yellow &&
                textBox_Keyword1.BackColor != System.Drawing.Color.Yellow &&
                textBox_Times1.BackColor != System.Drawing.Color.Yellow &&
                textBox_Keyword2.BackColor != System.Drawing.Color.Yellow &&
                textBox_Times2.BackColor != System.Drawing.Color.Yellow &&
                textBox_Keyword3.BackColor != System.Drawing.Color.Yellow &&
                textBox_Times3.BackColor != System.Drawing.Color.Yellow &&
                textBox_Keyword4.BackColor != System.Drawing.Color.Yellow &&
                textBox_Times4.BackColor != System.Drawing.Color.Yellow &&
                textBox_Keyword5.BackColor != System.Drawing.Color.Yellow &&
                textBox_Times5.BackColor != System.Drawing.Color.Yellow &&
                textBox_Keyword6.BackColor != System.Drawing.Color.Yellow &&
                textBox_Times6.BackColor != System.Drawing.Color.Yellow &&
                textBox_Keyword7.BackColor != System.Drawing.Color.Yellow &&
                textBox_Times7.BackColor != System.Drawing.Color.Yellow &&
                textBox_Keyword8.BackColor != System.Drawing.Color.Yellow &&
                textBox_Times8.BackColor != System.Drawing.Color.Yellow &&
                textBox_Keyword9.BackColor != System.Drawing.Color.Yellow &&
                textBox_Times9.BackColor != System.Drawing.Color.Yellow
                )
            {
                label_ErrorMessage.Text = "Save Successfully !";
                Global.FormLog = true;
            }
        }

        private void ItemcomboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            int caseSwitch = Convert.ToInt32(comboBox_KeywordNum.Text.Trim());
            ini12.INIWrite(MainSettingPath, "LogSearch", "TextNum", caseSwitch.ToString());

            switch (caseSwitch)
            {
                case 1:
                    label_Keyword.Visible = true;
                    label_Times.Visible = true;

                    label_Keyword0.Visible = true;
                    label_Keyword1.Visible = false;
                    label_Keyword2.Visible = false;
                    label_Keyword3.Visible = false;
                    label_Keyword4.Visible = false;
                    label_Keyword5.Visible = false;
                    label_Keyword6.Visible = false;
                    label_Keyword7.Visible = false;
                    label_Keyword8.Visible = false;
                    label_Keyword9.Visible = false;
                    textBox_Keyword0.Visible = true;
                    textBox_Keyword1.Visible = false;
                    textBox_Keyword2.Visible = false;
                    textBox_Keyword3.Visible = false;
                    textBox_Keyword4.Visible = false;
                    textBox_Keyword5.Visible = false;
                    textBox_Keyword6.Visible = false;
                    textBox_Keyword7.Visible = false;
                    textBox_Keyword8.Visible = false;
                    textBox_Keyword9.Visible = false;
                    textBox_Times0.Visible = true;
                    textBox_Times1.Visible = false;
                    textBox_Times2.Visible = false;
                    textBox_Times3.Visible = false;
                    textBox_Times4.Visible = false;
                    textBox_Times5.Visible = false;
                    textBox_Times6.Visible = false;
                    textBox_Times7.Visible = false;
                    textBox_Times8.Visible = false;
                    textBox_Times9.Visible = false;
                    break;

                case 2:
                    label_Keyword.Visible = true;
                    label_Times.Visible = true;

                    label_Keyword0.Visible = true;
                    label_Keyword1.Visible = true;
                    label_Keyword2.Visible = false;
                    label_Keyword3.Visible = false;
                    label_Keyword4.Visible = false;
                    label_Keyword5.Visible = false;
                    label_Keyword6.Visible = false;
                    label_Keyword7.Visible = false;
                    label_Keyword8.Visible = false;
                    label_Keyword9.Visible = false;
                    textBox_Keyword0.Visible = true;
                    textBox_Keyword1.Visible = true;
                    textBox_Keyword2.Visible = false;
                    textBox_Keyword3.Visible = false;
                    textBox_Keyword4.Visible = false;
                    textBox_Keyword5.Visible = false;
                    textBox_Keyword6.Visible = false;
                    textBox_Keyword7.Visible = false;
                    textBox_Keyword8.Visible = false;
                    textBox_Keyword9.Visible = false;
                    textBox_Times0.Visible = true;
                    textBox_Times1.Visible = true;
                    textBox_Times2.Visible = false;
                    textBox_Times3.Visible = false;
                    textBox_Times4.Visible = false;
                    textBox_Times5.Visible = false;
                    textBox_Times6.Visible = false;
                    textBox_Times7.Visible = false;
                    textBox_Times8.Visible = false;
                    textBox_Times9.Visible = false;
                    break;

                case 3:
                    label_Keyword.Visible = true;
                    label_Times.Visible = true;

                    label_Keyword0.Visible = true;
                    label_Keyword1.Visible = true;
                    label_Keyword2.Visible = true;
                    label_Keyword3.Visible = false;
                    label_Keyword4.Visible = false;
                    label_Keyword5.Visible = false;
                    label_Keyword6.Visible = false;
                    label_Keyword7.Visible = false;
                    label_Keyword8.Visible = false;
                    label_Keyword9.Visible = false;
                    textBox_Keyword0.Visible = true;
                    textBox_Keyword1.Visible = true;
                    textBox_Keyword2.Visible = true;
                    textBox_Keyword3.Visible = false;
                    textBox_Keyword4.Visible = false;
                    textBox_Keyword5.Visible = false;
                    textBox_Keyword6.Visible = false;
                    textBox_Keyword7.Visible = false;
                    textBox_Keyword8.Visible = false;
                    textBox_Keyword9.Visible = false;
                    textBox_Times0.Visible = true;
                    textBox_Times1.Visible = true;
                    textBox_Times2.Visible = true;
                    textBox_Times3.Visible = false;
                    textBox_Times4.Visible = false;
                    textBox_Times5.Visible = false;
                    textBox_Times6.Visible = false;
                    textBox_Times7.Visible = false;
                    textBox_Times8.Visible = false;
                    textBox_Times9.Visible = false;
                    break;

                case 4:
                    label_Keyword.Visible = true;
                    label_Times.Visible = true;

                    label_Keyword0.Visible = true;
                    label_Keyword1.Visible = true;
                    label_Keyword2.Visible = true;
                    label_Keyword3.Visible = true;
                    label_Keyword4.Visible = false;
                    label_Keyword5.Visible = false;
                    label_Keyword6.Visible = false;
                    label_Keyword7.Visible = false;
                    label_Keyword8.Visible = false;
                    label_Keyword9.Visible = false;
                    textBox_Keyword0.Visible = true;
                    textBox_Keyword1.Visible = true;
                    textBox_Keyword2.Visible = true;
                    textBox_Keyword3.Visible = true;
                    textBox_Keyword4.Visible = false;
                    textBox_Keyword5.Visible = false;
                    textBox_Keyword6.Visible = false;
                    textBox_Keyword7.Visible = false;
                    textBox_Keyword8.Visible = false;
                    textBox_Keyword9.Visible = false;
                    textBox_Times0.Visible = true;
                    textBox_Times1.Visible = true;
                    textBox_Times2.Visible = true;
                    textBox_Times3.Visible = true;
                    textBox_Times4.Visible = false;
                    textBox_Times5.Visible = false;
                    textBox_Times6.Visible = false;
                    textBox_Times7.Visible = false;
                    textBox_Times8.Visible = false;
                    textBox_Times9.Visible = false;
                    break;

                case 5:
                    label_Keyword.Visible = true;
                    label_Times.Visible = true;

                    label_Keyword0.Visible = true;
                    label_Keyword1.Visible = true;
                    label_Keyword2.Visible = true;
                    label_Keyword3.Visible = true;
                    label_Keyword4.Visible = true;
                    label_Keyword5.Visible = false;
                    label_Keyword6.Visible = false;
                    label_Keyword7.Visible = false;
                    label_Keyword8.Visible = false;
                    label_Keyword9.Visible = false;
                    textBox_Keyword0.Visible = true;
                    textBox_Keyword1.Visible = true;
                    textBox_Keyword2.Visible = true;
                    textBox_Keyword3.Visible = true;
                    textBox_Keyword4.Visible = true;
                    textBox_Keyword5.Visible = false;
                    textBox_Keyword6.Visible = false;
                    textBox_Keyword7.Visible = false;
                    textBox_Keyword8.Visible = false;
                    textBox_Keyword9.Visible = false;
                    textBox_Times0.Visible = true;
                    textBox_Times1.Visible = true;
                    textBox_Times2.Visible = true;
                    textBox_Times3.Visible = true;
                    textBox_Times4.Visible = true;
                    textBox_Times5.Visible = false;
                    textBox_Times6.Visible = false;
                    textBox_Times7.Visible = false;
                    textBox_Times8.Visible = false;
                    textBox_Times9.Visible = false;
                    break;

                case 6:
                    label_Keyword.Visible = true;
                    label_Times.Visible = true;

                    label_Keyword0.Visible = true;
                    label_Keyword1.Visible = true;
                    label_Keyword2.Visible = true;
                    label_Keyword3.Visible = true;
                    label_Keyword4.Visible = true;
                    label_Keyword5.Visible = true;
                    label_Keyword6.Visible = false;
                    label_Keyword7.Visible = false;
                    label_Keyword8.Visible = false;
                    label_Keyword9.Visible = false;
                    textBox_Keyword0.Visible = true;
                    textBox_Keyword1.Visible = true;
                    textBox_Keyword2.Visible = true;
                    textBox_Keyword3.Visible = true;
                    textBox_Keyword4.Visible = true;
                    textBox_Keyword5.Visible = true;
                    textBox_Keyword6.Visible = false;
                    textBox_Keyword7.Visible = false;
                    textBox_Keyword8.Visible = false;
                    textBox_Keyword9.Visible = false;
                    textBox_Times0.Visible = true;
                    textBox_Times1.Visible = true;
                    textBox_Times2.Visible = true;
                    textBox_Times3.Visible = true;
                    textBox_Times4.Visible = true;
                    textBox_Times5.Visible = true;
                    textBox_Times6.Visible = false;
                    textBox_Times7.Visible = false;
                    textBox_Times8.Visible = false;
                    textBox_Times9.Visible = false;
                    break;

                case 7:
                    label_Keyword.Visible = true;
                    label_Times.Visible = true;

                    label_Keyword0.Visible = true;
                    label_Keyword1.Visible = true;
                    label_Keyword2.Visible = true;
                    label_Keyword3.Visible = true;
                    label_Keyword4.Visible = true;
                    label_Keyword5.Visible = true;
                    label_Keyword6.Visible = true;
                    label_Keyword7.Visible = false;
                    label_Keyword8.Visible = false;
                    label_Keyword9.Visible = false;
                    textBox_Keyword0.Visible = true;
                    textBox_Keyword1.Visible = true;
                    textBox_Keyword2.Visible = true;
                    textBox_Keyword3.Visible = true;
                    textBox_Keyword4.Visible = true;
                    textBox_Keyword5.Visible = true;
                    textBox_Keyword6.Visible = true;
                    textBox_Keyword7.Visible = false;
                    textBox_Keyword8.Visible = false;
                    textBox_Keyword9.Visible = false;
                    textBox_Times0.Visible = true;
                    textBox_Times1.Visible = true;
                    textBox_Times2.Visible = true;
                    textBox_Times3.Visible = true;
                    textBox_Times4.Visible = true;
                    textBox_Times5.Visible = true;
                    textBox_Times6.Visible = true;
                    textBox_Times7.Visible = false;
                    textBox_Times8.Visible = false;
                    textBox_Times9.Visible = false;
                    break;

                case 8:
                    label_Keyword.Visible = true;
                    label_Times.Visible = true;

                    label_Keyword0.Visible = true;
                    label_Keyword1.Visible = true;
                    label_Keyword2.Visible = true;
                    label_Keyword3.Visible = true;
                    label_Keyword4.Visible = true;
                    label_Keyword5.Visible = true;
                    label_Keyword6.Visible = true;
                    label_Keyword7.Visible = true;
                    label_Keyword8.Visible = false;
                    label_Keyword9.Visible = false;
                    textBox_Keyword0.Visible = true;
                    textBox_Keyword1.Visible = true;
                    textBox_Keyword2.Visible = true;
                    textBox_Keyword3.Visible = true;
                    textBox_Keyword4.Visible = true;
                    textBox_Keyword5.Visible = true;
                    textBox_Keyword6.Visible = true;
                    textBox_Keyword7.Visible = true;
                    textBox_Keyword8.Visible = false;
                    textBox_Keyword9.Visible = false;
                    textBox_Times0.Visible = true;
                    textBox_Times1.Visible = true;
                    textBox_Times2.Visible = true;
                    textBox_Times3.Visible = true;
                    textBox_Times4.Visible = true;
                    textBox_Times5.Visible = true;
                    textBox_Times6.Visible = true;
                    textBox_Times7.Visible = true;
                    textBox_Times8.Visible = false;
                    textBox_Times9.Visible = false;
                    break;

                case 9:
                    label_Keyword.Visible = true;
                    label_Times.Visible = true;

                    label_Keyword0.Visible = true;
                    label_Keyword1.Visible = true;
                    label_Keyword2.Visible = true;
                    label_Keyword3.Visible = true;
                    label_Keyword4.Visible = true;
                    label_Keyword5.Visible = true;
                    label_Keyword6.Visible = true;
                    label_Keyword7.Visible = true;
                    label_Keyword8.Visible = true;
                    label_Keyword9.Visible = false;
                    textBox_Keyword0.Visible = true;
                    textBox_Keyword1.Visible = true;
                    textBox_Keyword2.Visible = true;
                    textBox_Keyword3.Visible = true;
                    textBox_Keyword4.Visible = true;
                    textBox_Keyword5.Visible = true;
                    textBox_Keyword6.Visible = true;
                    textBox_Keyword7.Visible = true;
                    textBox_Keyword8.Visible = true;
                    textBox_Keyword9.Visible = false;
                    textBox_Times0.Visible = true;
                    textBox_Times1.Visible = true;
                    textBox_Times2.Visible = true;
                    textBox_Times3.Visible = true;
                    textBox_Times4.Visible = true;
                    textBox_Times5.Visible = true;
                    textBox_Times6.Visible = true;
                    textBox_Times7.Visible = true;
                    textBox_Times8.Visible = true;
                    textBox_Times9.Visible = false;
                    break;

                case 10:
                    label_Keyword.Visible = true;
                    label_Times.Visible = true;

                    label_Keyword0.Visible = true;
                    label_Keyword1.Visible = true;
                    label_Keyword2.Visible = true;
                    label_Keyword3.Visible = true;
                    label_Keyword4.Visible = true;
                    label_Keyword5.Visible = true;
                    label_Keyword6.Visible = true;
                    label_Keyword7.Visible = true;
                    label_Keyword8.Visible = true;
                    label_Keyword9.Visible = true;
                    textBox_Keyword0.Visible = true;
                    textBox_Keyword1.Visible = true;
                    textBox_Keyword2.Visible = true;
                    textBox_Keyword3.Visible = true;
                    textBox_Keyword4.Visible = true;
                    textBox_Keyword5.Visible = true;
                    textBox_Keyword6.Visible = true;
                    textBox_Keyword7.Visible = true;
                    textBox_Keyword8.Visible = true;
                    textBox_Keyword9.Visible = true;
                    textBox_Times0.Visible = true;
                    textBox_Times1.Visible = true;
                    textBox_Times2.Visible = true;
                    textBox_Times3.Visible = true;
                    textBox_Times4.Visible = true;
                    textBox_Times5.Visible = true;
                    textBox_Times6.Visible = true;
                    textBox_Times7.Visible = true;
                    textBox_Times8.Visible = true;
                    textBox_Times9.Visible = true;
                    break;

                default:
                    label_Keyword.Visible = false;
                    label_Times.Visible = false;

                    label_Keyword0.Visible = false;
                    label_Keyword1.Visible = false;
                    label_Keyword2.Visible = false;
                    label_Keyword3.Visible = false;
                    label_Keyword4.Visible = false;
                    label_Keyword5.Visible = false;
                    label_Keyword6.Visible = false;
                    label_Keyword7.Visible = false;
                    label_Keyword8.Visible = false;
                    label_Keyword9.Visible = false;
                    textBox_Keyword0.Visible = false;
                    textBox_Keyword1.Visible = false;
                    textBox_Keyword2.Visible = false;
                    textBox_Keyword3.Visible = false;
                    textBox_Keyword4.Visible = false;
                    textBox_Keyword5.Visible = false;
                    textBox_Keyword6.Visible = false;
                    textBox_Keyword7.Visible = false;
                    textBox_Keyword8.Visible = false;
                    textBox_Keyword9.Visible = false;
                    textBox_Times0.Visible = false;
                    textBox_Times1.Visible = false;
                    textBox_Times2.Visible = false;
                    textBox_Times3.Visible = false;
                    textBox_Times4.Visible = false;
                    textBox_Times5.Visible = false;
                    textBox_Times6.Visible = false;
                    textBox_Times7.Visible = false;
                    textBox_Times8.Visible = false;
                    textBox_Times9.Visible = false;
                    break;
            }
        }

        private void Comport1checkBox_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox_SerialPort1.Checked == true)
            {
                ini12.INIWrite(MainSettingPath, "LogSearch", "Comport1", "1");
                comboBox_KeywordNum.Enabled = true;
            }
            else
            {
                if (checkBox_SerialPort2.Checked == true)
                {
                    ini12.INIWrite(MainSettingPath, "LogSearch", "Comport1", "0");
                }
                else if (checkBox_SerialPort3.Checked == true)
                {
                    ini12.INIWrite(MainSettingPath, "LogSearch", "Comport1", "0");
                }
                else
                {
                    comboBox_KeywordNum.Text = "0";
                    comboBox_KeywordNum.Enabled = false;
                    SetErrorPictureAsNull();
                    ini12.INIWrite(MainSettingPath, "LogSearch", "Comport1", "0");
                }
            }
        }

        private void Comport2checkBox_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox_SerialPort2.Checked == true)
            {
                ini12.INIWrite(MainSettingPath, "LogSearch", "Comport2", "1");
                comboBox_KeywordNum.Enabled = true;
            }
            else
            {
                if (checkBox_SerialPort1.Checked == true)
                {
                    ini12.INIWrite(MainSettingPath, "LogSearch", "Comport2", "0");
                }
                else if (checkBox_SerialPort3.Checked == true)
                {
                    ini12.INIWrite(MainSettingPath, "LogSearch", "Comport3", "0");
                }
                else
                {
                    comboBox_KeywordNum.Text = "0";
                    comboBox_KeywordNum.Enabled = false;
                    SetErrorPictureAsNull();
                    ini12.INIWrite(MainSettingPath, "LogSearch", "Comport2", "0");
                }
            }
        }

        private void checkBox_SerialPort3_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox_SerialPort3.Checked == true)
            {
                ini12.INIWrite(MainSettingPath, "LogSearch", "Comport3", "1");
                comboBox_KeywordNum.Enabled = true;
            }
            else
            {
                if (checkBox_SerialPort1.Checked == true)
                {
                    ini12.INIWrite(MainSettingPath, "LogSearch", "Comport3", "0");
                }
                else if (checkBox_SerialPort2.Checked == true)
                {
                    ini12.INIWrite(MainSettingPath, "LogSearch", "Comport3", "0");
                }
                else
                {
                    comboBox_KeywordNum.Text = "0";
                    comboBox_KeywordNum.Enabled = false;
                    SetErrorPictureAsNull();
                    ini12.INIWrite(MainSettingPath, "LogSearch", "Comport3", "0");
                }
            }
        }

        private void KeywordCheck(object sender)
        {
            if (string.IsNullOrEmpty(textBox_Keyword0.Text))
            {
                textBox_Keyword0.BackColor = System.Drawing.Color.Yellow;
                label_ErrorMessage.Text = "Keyword cannot be empty!";
                Global.FormLog = false;
            }
            else if (string.IsNullOrEmpty(textBox_Times0.Text) || textBox_Times0.Text == "0")
            {
                textBox_Times0.BackColor = System.Drawing.Color.Yellow;
                label_ErrorMessage.Text = "Times cannot be empty!";
                Global.FormLog = false;
            }
            else
            {
                textBox_Keyword0.BackColor = default(Color);
                textBox_Times0.BackColor = default(Color);
                ini12.INIWrite(MainSettingPath, "LogSearch", "Text0", textBox_Keyword0.Text.Trim());
                ini12.INIWrite(MainSettingPath, "LogSearch", "Times0", textBox_Times0.Text.Trim());
            }
        }

        private void textBox_Keyword0_TextChanged(object sender, EventArgs e)
        {
            if (textBox_Keyword0.Visible == true)
            {
                if (string.IsNullOrEmpty(textBox_Keyword0.Text))
                {
                    pictureBox_Keyword0.Image = Properties.Resources.ERROR;
                    label_ErrorMessage.Text = "Keyword cannot be empty!";
                    comboBox_KeywordNum.Enabled = false;
                }
                else
                {
                    ini12.INIWrite(MainSettingPath, "LogSearch", "Text0", textBox_Keyword0.Text.Trim());
                    pictureBox_Keyword0.Image = null;
                    label_ErrorMessage.Text = "";
                    comboBox_KeywordNum.Enabled = true;
                }
            }
        }

        private void textBox_Times0_TextChanged(object sender, EventArgs e)
        {
            if (textBox_Times0.Visible == true)
            {
                if (string.IsNullOrEmpty(textBox_Times0.Text) || textBox_Times0.Text == "0")
                {
                    pictureBox_Keyword0.Image = Properties.Resources.ERROR;
                    label_ErrorMessage.Text = "Times can't set 0 or null !";
                    comboBox_KeywordNum.Enabled = false;
                }
                else
                {
                    ini12.INIWrite(MainSettingPath, "LogSearch", "Times0", textBox_Times0.Text.Trim());
                    pictureBox_Keyword0.Image = null;
                    label_ErrorMessage.Text = "";
                    comboBox_KeywordNum.Enabled = true;
                }
            }
        }

        private void textBox_Keyword1_TextChanged(object sender, EventArgs e)
        {
            if (textBox_Keyword1.Visible == true)
            {
                if (string.IsNullOrEmpty(textBox_Keyword1.Text))
                {
                    pictureBox_Keyword1.Image = Properties.Resources.ERROR;
                    label_ErrorMessage.Text = "Keyword cannot be empty!";
                    comboBox_KeywordNum.Enabled = false;
                }
                else
                {
                    ini12.INIWrite(MainSettingPath, "LogSearch", "Text1", textBox_Keyword1.Text.Trim());
                    pictureBox_Keyword1.Image = null;
                    label_ErrorMessage.Text = "";
                    comboBox_KeywordNum.Enabled = true;
                }
            }
        }

        private void textBox_Times1_TextChanged(object sender, EventArgs e)
        {
            if (textBox_Times1.Visible == true)
            {
                if (string.IsNullOrEmpty(textBox_Times1.Text) || textBox_Times1.Text == "0")
                {
                    pictureBox_Keyword1.Image = Properties.Resources.ERROR;
                    label_ErrorMessage.Text = "Times can't set 0 or null !";
                    comboBox_KeywordNum.Enabled = false;
                }
                else
                {
                    ini12.INIWrite(MainSettingPath, "LogSearch", "Times1", textBox_Times1.Text.Trim());
                    pictureBox_Keyword1.Image = null;
                    label_ErrorMessage.Text = "";
                    comboBox_KeywordNum.Enabled = true;
                }
            }
        }

        private void textBox_Keyword2_TextChanged(object sender, EventArgs e)
        {
            if (textBox_Keyword2.Visible == true)
            {
                if (string.IsNullOrEmpty(textBox_Keyword2.Text))
                {
                    pictureBox_Keyword2.Image = Properties.Resources.ERROR;
                    label_ErrorMessage.Text = "Keyword cannot be empty!";
                    comboBox_KeywordNum.Enabled = false;
                }
                else
                {
                    ini12.INIWrite(MainSettingPath, "LogSearch", "Text2", textBox_Keyword2.Text.Trim());
                    pictureBox_Keyword2.Image = null;
                    label_ErrorMessage.Text = "";
                    comboBox_KeywordNum.Enabled = true;
                }
            }
        }

        private void textBox_Times2_TextChanged(object sender, EventArgs e)
        {
            if (textBox_Times2.Visible == true)
            {
                if (string.IsNullOrEmpty(textBox_Times2.Text) || textBox_Times2.Text == "0")
                {
                    pictureBox_Keyword2.Image = Properties.Resources.ERROR;
                    label_ErrorMessage.Text = "Times can't set 0 or null !";
                    comboBox_KeywordNum.Enabled = false;
                }
                else
                {
                    ini12.INIWrite(MainSettingPath, "LogSearch", "Times2", textBox_Times2.Text.Trim());
                    pictureBox_Keyword2.Image = null;
                    label_ErrorMessage.Text = "";
                    comboBox_KeywordNum.Enabled = true;
                }
            }
        }

        private void textBox_Keyword3_TextChanged(object sender, EventArgs e)
        {
            if (textBox_Keyword3.Visible == true)
            {
                if (string.IsNullOrEmpty(textBox_Keyword3.Text))
                {
                    pictureBox_Keyword3.Image = Properties.Resources.ERROR;
                    label_ErrorMessage.Text = "Keyword cannot be empty!";
                    comboBox_KeywordNum.Enabled = false;
                }
                else
                {
                    ini12.INIWrite(MainSettingPath, "LogSearch", "Text3", textBox_Keyword3.Text.Trim());
                    pictureBox_Keyword3.Image = null;
                    label_ErrorMessage.Text = "";
                    comboBox_KeywordNum.Enabled = true;
                }
            }
        }

        private void textBox_Times3_TextChanged(object sender, EventArgs e)
        {
            if (textBox_Times3.Visible == true)
            {
                if (string.IsNullOrEmpty(textBox_Times3.Text) || textBox_Times3.Text == "0")
                {
                    pictureBox_Keyword3.Image = Properties.Resources.ERROR;
                    label_ErrorMessage.Text = "Times can't set 0 or null !";
                    comboBox_KeywordNum.Enabled = false;
                }
                else
                {
                    ini12.INIWrite(MainSettingPath, "LogSearch", "Times3", textBox_Times3.Text.Trim());
                    pictureBox_Keyword3.Image = null;
                    label_ErrorMessage.Text = "";
                    comboBox_KeywordNum.Enabled = true;
                }
            }
        }

        private void textBox_Keyword4_TextChanged(object sender, EventArgs e)
        {
            if (textBox_Keyword4.Visible == true)
            {
                if (string.IsNullOrEmpty(textBox_Keyword4.Text))
                {
                    pictureBox_Keyword4.Image = Properties.Resources.ERROR;
                    label_ErrorMessage.Text = "Keyword cannot be empty!";
                    comboBox_KeywordNum.Enabled = false;
                }
                else
                {
                    ini12.INIWrite(MainSettingPath, "LogSearch", "Text4", textBox_Keyword4.Text.Trim());
                    pictureBox_Keyword4.Image = null;
                    label_ErrorMessage.Text = "";
                    comboBox_KeywordNum.Enabled = true;
                }
            }
        }

        private void textBox_Times4_TextChanged(object sender, EventArgs e)
        {
            if (textBox_Times4.Visible == true)
            {
                if (string.IsNullOrEmpty(textBox_Times4.Text) || textBox_Times4.Text == "0")
                {
                    pictureBox_Keyword4.Image = Properties.Resources.ERROR;
                    label_ErrorMessage.Text = "Times can't set 0 or null !";
                    comboBox_KeywordNum.Enabled = false;
                }
                else
                {
                    ini12.INIWrite(MainSettingPath, "LogSearch", "Times4", textBox_Times4.Text.Trim());
                    pictureBox_Keyword4.Image = null;
                    label_ErrorMessage.Text = "";
                    comboBox_KeywordNum.Enabled = true;
                }
            }
        }

        private void textBox_Keyword5_TextChanged(object sender, EventArgs e)
        {
            if (textBox_Keyword5.Visible == true)
            {
                if (string.IsNullOrEmpty(textBox_Keyword5.Text))
                {
                    pictureBox_Keyword5.Image = Properties.Resources.ERROR;
                    label_ErrorMessage.Text = "Keyword cannot be empty!";
                    comboBox_KeywordNum.Enabled = false;
                }
                else
                {
                    ini12.INIWrite(MainSettingPath, "LogSearch", "Text5", textBox_Keyword5.Text.Trim());
                    pictureBox_Keyword5.Image = null;
                    label_ErrorMessage.Text = "";
                    comboBox_KeywordNum.Enabled = true;
                }
            }
        }

        private void textBox_Times5_TextChanged(object sender, EventArgs e)
        {
            if (textBox_Times5.Visible == true)
            {
                if (string.IsNullOrEmpty(textBox_Times5.Text) || textBox_Times5.Text == "0")
                {
                    pictureBox_Keyword5.Image = Properties.Resources.ERROR;
                    label_ErrorMessage.Text = "Times can't set 0 or null !";
                    comboBox_KeywordNum.Enabled = false;
                }
                else
                {
                    ini12.INIWrite(MainSettingPath, "LogSearch", "Times5", textBox_Times5.Text.Trim());
                    pictureBox_Keyword5.Image = null;
                    label_ErrorMessage.Text = "";
                    comboBox_KeywordNum.Enabled = true;
                }
            }
        }

        private void textBox_Keyword6_TextChanged(object sender, EventArgs e)
        {
            if (textBox_Keyword6.Visible == true)
            {
                if (string.IsNullOrEmpty(textBox_Keyword6.Text))
                {
                    pictureBox_Keyword6.Image = Properties.Resources.ERROR;
                    label_ErrorMessage.Text = "Keyword cannot be empty!";
                    comboBox_KeywordNum.Enabled = false;
                }
                else
                {
                    ini12.INIWrite(MainSettingPath, "LogSearch", "Text6", textBox_Keyword6.Text.Trim());
                    pictureBox_Keyword6.Image = null;
                    label_ErrorMessage.Text = "";
                    comboBox_KeywordNum.Enabled = true;
                }
            }
        }

        private void textBox_Times6_TextChanged(object sender, EventArgs e)
        {
            if (textBox_Times6.Visible == true)
            {
                if (string.IsNullOrEmpty(textBox_Times6.Text) || textBox_Times6.Text == "0")
                {
                    pictureBox_Keyword6.Image = Properties.Resources.ERROR;
                    label_ErrorMessage.Text = "Times can't set 0 or null !";
                    comboBox_KeywordNum.Enabled = false;
                }
                else
                {
                    ini12.INIWrite(MainSettingPath, "LogSearch", "Times6", textBox_Times6.Text.Trim());
                    pictureBox_Keyword6.Image = null;
                    label_ErrorMessage.Text = "";
                    comboBox_KeywordNum.Enabled = true;
                }
            }
        }

        private void textBox_Keyword7_TextChanged(object sender, EventArgs e)
        {
            if (textBox_Keyword7.Visible == true)
            {
                if (string.IsNullOrEmpty(textBox_Keyword7.Text))
                {
                    pictureBox_Keyword7.Image = Properties.Resources.ERROR;
                    label_ErrorMessage.Text = "Keyword cannot be empty!";
                    comboBox_KeywordNum.Enabled = false;
                }
                else
                {
                    ini12.INIWrite(MainSettingPath, "LogSearch", "Text7", textBox_Keyword7.Text.Trim());
                    pictureBox_Keyword7.Image = null;
                    label_ErrorMessage.Text = "";
                    comboBox_KeywordNum.Enabled = true;
                }
            }
        }

        private void textBox_Times7_TextChanged(object sender, EventArgs e)
        {
            if (textBox_Times7.Visible == true)
            {
                if (string.IsNullOrEmpty(textBox_Times7.Text) || textBox_Times7.Text == "0")
                {
                    pictureBox_Keyword7.Image = Properties.Resources.ERROR;
                    label_ErrorMessage.Text = "Times can't set 0 or null !";
                    comboBox_KeywordNum.Enabled = false;
                }
                else
                {
                    ini12.INIWrite(MainSettingPath, "LogSearch", "Times7", textBox_Times7.Text.Trim());
                    pictureBox_Keyword7.Image = null;
                    label_ErrorMessage.Text = "";
                    comboBox_KeywordNum.Enabled = true;
                }
            }
        }

        private void textBox_Keyword8_TextChanged(object sender, EventArgs e)
        {
            if (textBox_Keyword8.Visible == true)
            {
                if (string.IsNullOrEmpty(textBox_Keyword8.Text))
                {
                    pictureBox_Keyword8.Image = Properties.Resources.ERROR;
                    label_ErrorMessage.Text = "Keyword cannot be empty!";
                    comboBox_KeywordNum.Enabled = false;
                }
                else
                {
                    ini12.INIWrite(MainSettingPath, "LogSearch", "Text8", textBox_Keyword8.Text.Trim());
                    pictureBox_Keyword8.Image = null;
                    label_ErrorMessage.Text = "";
                    comboBox_KeywordNum.Enabled = true;
                }
            }
        }

        private void textBox_Times8_TextChanged(object sender, EventArgs e)
        {
            if (textBox_Times8.Visible == true)
            {
                if (string.IsNullOrEmpty(textBox_Times8.Text) || textBox_Times8.Text == "0")
                {
                    pictureBox_Keyword8.Image = Properties.Resources.ERROR;
                    label_ErrorMessage.Text = "Times can't set 0 or null !";
                    comboBox_KeywordNum.Enabled = false;
                }
                else
                {
                    ini12.INIWrite(MainSettingPath, "LogSearch", "Times8", textBox_Times8.Text.Trim());
                    pictureBox_Keyword8.Image = null;
                    label_ErrorMessage.Text = "";
                    comboBox_KeywordNum.Enabled = true;
                }
            }
        }

        private void textBox_Keyword9_TextChanged(object sender, EventArgs e)
        {
            if (textBox_Keyword9.Visible == true)
            {
                if (string.IsNullOrEmpty(textBox_Keyword9.Text))
                {
                    pictureBox_Keyword9.Image = Properties.Resources.ERROR;
                    label_ErrorMessage.Text = "Keyword cannot be empty!";
                    comboBox_KeywordNum.Enabled = false;
                }
                else
                {
                    ini12.INIWrite(MainSettingPath, "LogSearch", "Text9", textBox_Keyword9.Text.Trim());
                    pictureBox_Keyword9.Image = null;
                    label_ErrorMessage.Text = "";
                    comboBox_KeywordNum.Enabled = true;
                }
            }
        }

        private void textBox_Times9_TextChanged(object sender, EventArgs e)
        {
            if (textBox_Times9.Visible == true)
            {
                if (string.IsNullOrEmpty(textBox_Times9.Text) || textBox_Times9.Text == "0")
                {
                    pictureBox_Keyword9.Image = Properties.Resources.ERROR;
                    label_ErrorMessage.Text = "Times can't set 0 or null !";
                    comboBox_KeywordNum.Enabled = false;
                }
                else
                {
                    ini12.INIWrite(MainSettingPath, "LogSearch", "Times9", textBox_Times9.Text.Trim());
                    pictureBox_Keyword9.Image = null;
                    label_ErrorMessage.Text = "";
                    comboBox_KeywordNum.Enabled = true;
                }
            }
        }

        private void SetErrorPictureAsNull()
        {
            pictureBox_Keyword0.Image = null;
            pictureBox_Keyword1.Image = null;
            pictureBox_Keyword2.Image = null;
            pictureBox_Keyword3.Image = null;
            pictureBox_Keyword4.Image = null;
            pictureBox_Keyword5.Image = null;
            pictureBox_Keyword6.Image = null;
            pictureBox_Keyword7.Image = null;
            pictureBox_Keyword8.Image = null;
            pictureBox_Keyword9.Image = null;
            label_ErrorMessage.Text = "";
        }
    }
}
