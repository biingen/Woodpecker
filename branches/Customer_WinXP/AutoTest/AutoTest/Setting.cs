﻿using DirectX.Capture;
using jini;
using RedRat.RedRat3.USB;
using System;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Xml.Linq;

namespace AutoTest
{
    public partial class Setting : Form
    {
        public Setting()
        {
            InitializeComponent();
        }

        private void loadxml()
        {
            // Redrat Database
            if (System.IO.File.Exists(textBox3.Text))
            {
                XDocument myDoc = XDocument.Load(textBox3.Text);
                //textBox1.Text = myDoc.ToString();
                var AVDevices = from pn in myDoc.Descendants("AVDevice")
                                where pn.Element("Name") != null
                                select pn.Element("Name").Value;

                foreach (var c in AVDevices)
                {
                    //textBox1.Text = textBox1.Text + c + "\t\n";
                    comboBox3.Items.Add(c);
                    if (comboBox3.Text == "")
                        comboBox3.Text = c;
                }
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            // RedRat3 Command Path
            openFileDialog2.Filter = "XML files (*.xml)|*.xml";
            openFileDialog2.ShowDialog();
            if (openFileDialog2.FileName == "")
            {
                textBox3.Text = textBox3.Text;
            }
            else
            {
                textBox3.Text = openFileDialog2.FileName;
                comboBox3.Items.Clear();
                loadxml();
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            // Generator Command Path
            openFileDialog1.Filter = "XML files (*.xml)|*.xml";
            openFileDialog1.ShowDialog();
            if (openFileDialog1.FileName == "")
            {
                textBox2.Text = textBox2.Text;
            }
            else
            {
                textBox2.Text = openFileDialog1.FileName;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            //Save Video Path
            folderBrowserDialog1.ShowDialog();
            if (folderBrowserDialog1.SelectedPath == "")
            {
                textBox1.Text = textBox1.Text;
            }
            else
            {
                textBox1.Text = folderBrowserDialog1.SelectedPath;
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            //log file save path
            folderBrowserDialog2.ShowDialog();
            if (folderBrowserDialog2.SelectedPath == "")
            {
                textBox5.Text = textBox5.Text;
            }
            else
            {
                textBox5.Text = folderBrowserDialog2.SelectedPath;
            }
        }

        public void OkBtn_Click(object sender, EventArgs e)
        {
            String sPath = Application.StartupPath + "\\Config.ini";

            if (ini12.INIRead(sPath, "Device", "Autobox", "") == "1")
            {
                if (checkBox1.Checked != true && checkBox2.Checked != true)     //防止comport被全關
                {
                    SavedLabel.Text = "Com Port must exist !";
                    checkBox1.Checked = true;
                    ini12.INIWrite(sPath, "Comport", "BaudRate", baudratebox.Text.Trim());  //寫入com1>>
                    ini12.INIWrite(sPath, "Comport", "DataBit", "8");
                    ini12.INIWrite(sPath, "Comport", "StopBits", "One");
                    ini12.INIWrite(sPath, "Comport", "PortName", cmbCOM.Text);  //<<
                    return;
                }
                else if ((cmbCOM.Text == extcmbCOM.Text) && (checkBox1.Checked == true && checkBox2.Checked == true))
                {
                    SavedLabel.Text = "Com Port duplicate !";
                    checkBox2.Checked = false;
                    ini12.INIWrite(sPath, "ExtComport", "BaudRate", extbaudratebox.Text.Trim());    //寫入com2>>
                    ini12.INIWrite(sPath, "ExtComport", "DataBit", "8");
                    ini12.INIWrite(sPath, "ExtComport", "StopBits", "One");
                    ini12.INIWrite(sPath, "ExtComport", "PortName", extcmbCOM.Text);    //<<
                    return;
                }
                else
                {
                    ini12.INIWrite(sPath, "Comport", "BaudRate", baudratebox.Text.Trim());  //寫入com1>>
                    ini12.INIWrite(sPath, "Comport", "DataBit", "8");
                    ini12.INIWrite(sPath, "Comport", "StopBits", "One");
                    ini12.INIWrite(sPath, "Comport", "PortName", cmbCOM.Text);  //<<
                    ini12.INIWrite(sPath, "ExtComport", "BaudRate", extbaudratebox.Text.Trim());    //寫入com2>>
                    ini12.INIWrite(sPath, "ExtComport", "DataBit", "8");
                    ini12.INIWrite(sPath, "ExtComport", "StopBits", "One");
                    ini12.INIWrite(sPath, "ExtComport", "PortName", extcmbCOM.Text);    //<<
                }
            }

            //先判斷檔案是否存在
            if (System.IO.File.Exists(textBox3.Text.Trim()))
            {
                // 寫入ini檔 RedRat3 Command Path
                textBox3.BackColor = default(Color);
                ini12.INIWrite(sPath, "RedRat", "DBFile", textBox3.Text.Trim());
                ini12.INIWrite(sPath, "RedRat", "Device", "1");
            }
            else
            {
                SavedLabel.Text = "RC DB Path must exist !";
                textBox3.BackColor = System.Drawing.Color.Yellow;
                Global.FormSetting = false;
            }

            //先判斷檔案是否存在
            if (System.IO.File.Exists(textBox2.Text.Trim()))
            {
                // 寫入ini檔 Generator Command Path
                textBox2.BackColor = default(Color);
                ini12.INIWrite(sPath, "Record", "Generator", textBox2.Text.Trim());
            }
            else
            {
                SavedLabel.Text = "Astro or Quantum Path must exist !";
                textBox2.BackColor = System.Drawing.Color.Yellow;
                Global.FormSetting = false;
            }

            //先判斷目錄是否存在
            if (!System.IO.Directory.Exists(textBox1.Text.Trim()))
            {
                SavedLabel.Text = "Video Path must exist !";
                textBox1.BackColor = System.Drawing.Color.Yellow;
                Global.FormSetting = false;
            }
            else
            {
                textBox1.BackColor = default(Color);
                ini12.INIWrite(sPath, "Record", "VideoPath", textBox1.Text.Trim());
            }

            //先判斷目錄是否存在
            if (!System.IO.Directory.Exists(textBox5.Text.Trim()))
            {
                SavedLabel.Text = "Log Path must exist !";
                textBox5.BackColor = System.Drawing.Color.Yellow;
                Global.FormSetting = false;
            }
            else
            {
                textBox5.BackColor = default(Color);
                ini12.INIWrite(sPath, "Record", "LogPath", textBox5.Text.Trim());
            }

            // 寫入ini檔 設備名稱
            ini12.INIWrite(sPath, "RedRat", "Brands", comboBox3.Text.Trim());
            ini12.INIWrite(sPath, "RedRat", "Choose", comboBox5.Text.Trim());

            //寫入 攝影設備 & 收音設備
            ini12.INIWrite(sPath, "Camera", "Choose", comboBox4.SelectedIndex.ToString());
            ini12.INIWrite(sPath, "Camera", "Audio", comboBox6.SelectedIndex.ToString());

            if (textBox1.BackColor != System.Drawing.Color.Yellow && textBox3.BackColor != System.Drawing.Color.Yellow && textBox5.BackColor != System.Drawing.Color.Yellow)
            {
                SavedLabel.Text = "Save Successfully !";
                Global.FormSetting = true;
            }
        }

        private void Setting_Load(object sender, EventArgs e)
        {
            //表單戴入時
            // 設定INI檔路徑
            String sPath = Application.StartupPath + "\\Config.ini";

            // 讀取ini中的路徑
            textBox1.Text = ini12.INIRead(sPath, "Record", "VideoPath", "");
            textBox2.Text = ini12.INIRead(sPath, "Record", "Generator", "");
            textBox3.Text = ini12.INIRead(sPath, "RedRat", "DBFile", "");
            textBox5.Text = ini12.INIRead(sPath, "Record", "LogPath", "");

            if (ini12.INIRead(Application.StartupPath + "\\Config.ini", "Device", "Autobox", "") == "1")//偵測comport>>>>>>>>>>>>>>
            {
                string[] port1 = System.IO.Ports.SerialPort.GetPortNames();
                cmbCOM.DataSource = port1;
                cmbCOM.Text = port1.Last();
                string[] port2 = System.IO.Ports.SerialPort.GetPortNames();
                extcmbCOM.DataSource = port2;
                extcmbCOM.Text = port2.Last();

                if (ini12.INIRead(sPath, "Comport", "Choose", "") != "")
                {
                    if (int.Parse(ini12.INIRead(sPath, "Comport", "Choose", "")) == 1)
                    {
                        checkBox1.Checked = true;
                        baudratebox.Enabled = true;
                        cmbCOM.Enabled = true;
                    }
                    else
                    {
                        checkBox1.Checked = false;
                        baudratebox.Enabled = false;
                        cmbCOM.Enabled = false;
                    }
                }
                else
                {
                    ini12.INIWrite(sPath, "Comport", "Choose", "0");
                    ini12.INIWrite(sPath, "Comport", "BaudRate", "38400");
                    checkBox1.Checked = false;
                    baudratebox.Enabled = false;
                    cmbCOM.Enabled = false;
                }

                if (ini12.INIRead(sPath, "ExtComport", "Choose", "") != "")
                {
                    if (int.Parse(ini12.INIRead(sPath, "ExtComport", "Choose", "")) == 1)
                    {
                        checkBox2.Checked = true;
                        extbaudratebox.Enabled = true;
                        extcmbCOM.Enabled = true;
                    }
                    else
                    {
                        checkBox2.Checked = false;
                        extbaudratebox.Enabled = false;
                        extcmbCOM.Enabled = false;
                    }
                }
                else
                {
                    ini12.INIWrite(sPath, "ExtComport", "Choose", "0");
                    ini12.INIWrite(sPath, "ExtComport", "BaudRate", "115200");
                    checkBox2.Checked = false;
                    extbaudratebox.Enabled = false;
                    extcmbCOM.Enabled = false;
                }
            }
            else if(ini12.INIRead(Application.StartupPath + "\\Config.ini", "Device", "Autobox", "") == "0")
            {
                checkBox1.Checked = false;
                checkBox1.Enabled = false;
                checkBox2.Checked = false;
                checkBox2.Enabled = false;
            }//<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<

            string[] devices = RedRat3USBImpl.FindRedRat3s();       //偵測redrat>>>>>>>>>>>>>>
            if (devices.Length == 2)
            {
                comboBox5.Items.Add("1");
            }
            else if (devices.Length == 3)
            {
                comboBox5.Items.Add("1");
                comboBox5.Items.Add("2");
            }//<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<

            if (ini12.INIRead(sPath, "RedRat", "Choose", "") == "" && ini12.INIRead(sPath, "Device", "RedRat", "") == "1")
            {
                ini12.INIWrite(sPath, "RedRat", "Choose", "0");
            }

            baudratebox.Text = ini12.INIRead(sPath, "Comport", "BaudRate", "");
            cmbCOM.Text = ini12.INIRead(sPath, "Comport", "PortName", "");
            extbaudratebox.Text = ini12.INIRead(sPath, "ExtComport", "BaudRate", "");
            extcmbCOM.Text = ini12.INIRead(sPath, "ExtComport", "PortName", "");
            comboBox3.Text = ini12.INIRead(sPath, "RedRat", "Brands", "");
            comboBox5.Text = ini12.INIRead(sPath, "RedRat", "Choose", "");
            comboBox4.Text = ini12.INIRead(sPath, "Camera", "Choose", "");
            comboBox6.Text = ini12.INIRead(sPath, "Camera", "Audio", "");
            //VIDTextlabel.Text = ini12.INIRead(sPath, "Camera", "VID", "");
            //PIDTextlabel.Text = ini12.INIRead(sPath, "Camera", "PID", "");
            String Text;
            Text = ini12.INIRead(sPath, "Device", "RedRat", "");
            if (Text == "0")
            {
                label16.Text = "Disable";
                comboBox3.Enabled = false;
                comboBox5.Enabled = false;
            }
            else
            {
                label16.Text = "Enable";
                comboBox3.Enabled = true;
                comboBox5.Enabled = true;
            }
            Text = ini12.INIRead(sPath, "Device", "Camera", "");
            if (Text == "0")
            {
                label17.Text = "Disable";
                comboBox4.Enabled = false;
                comboBox6.Enabled = false;
                //comboBox1.Enabled = true;
            }
            else
            {
                label17.Text = "Enable";
                comboBox4.Enabled = true;
                comboBox6.Enabled = true;
                //comboBox1.Enabled = false;
            }

            loadxml();

            if (ini12.INIRead(Application.StartupPath + "\\Config.ini", "Device", "Camera", "") == "1")
            {
                Filters filters = new Filters();
                Filter f;

                // 取得攝影機
                for (int c = 0; c < filters.VideoInputDevices.Count; c++)
                {
                    f = filters.VideoInputDevices[c];
                    comboBox4.Items.Add(f.Name);

                    //if (comboBox4.Text == "")
                    comboBox4.Text = f.Name;
                }

                for (int j = 0; j < filters.AudioInputDevices.Count; j++)
                {
                    f = filters.AudioInputDevices[j];
                    comboBox6.Items.Add(f.Name);

                    //if (comboBox6.Text == "")
                    comboBox6.Text = f.Name;
                }
            }

            if (ini12.INIRead(sPath, "LogSearch", "TextNum", "") == "")
            {
                ini12.INIWrite(sPath, "LogSearch", "TextNum", "0");
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked == true)
            {
                // 設定INI檔路徑
                String sPath;
                sPath = Application.StartupPath + "\\Config.ini";

                // 寫入Com1啟動
                ini12.INIWrite(sPath, "Comport", "Choose", "1");

                baudratebox.Enabled = true;
                cmbCOM.Enabled = true;
            }
            else
            {
                // 設定INI檔路徑
                String sPath;
                sPath = Application.StartupPath + "\\Config.ini";

                // 寫入Com1關閉
                ini12.INIWrite(sPath, "Comport", "Choose", "0");

                baudratebox.Enabled = false;
                cmbCOM.Enabled = false;
            }
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox2.Checked == true)
            {
                // 設定INI檔路徑
                String sPath;
                sPath = Application.StartupPath + "\\Config.ini";

                // 寫入Com2啟動
                ini12.INIWrite(sPath, "ExtComport", "Choose", "1");

                extbaudratebox.Enabled = true;
                extcmbCOM.Enabled = true;
            }
            else
            {
                // 設定INI檔路徑
                String sPath;
                sPath = Application.StartupPath + "\\Config.ini";

                // 寫入Com2關閉
                ini12.INIWrite(sPath, "ExtComport", "Choose", "0");

                extbaudratebox.Enabled = false;
                extcmbCOM.Enabled = false;
            }
        }

        //自動調整ComboBox寬度的事件
        private void AdjustWidthComboBox_DropDown(object sender, System.EventArgs e)
        {
            ComboBox senderComboBox = (ComboBox)sender;
            int width = senderComboBox.DropDownWidth;
            Graphics g = senderComboBox.CreateGraphics();
            Font font = senderComboBox.Font;
            int vertScrollBarWidth =
                (senderComboBox.Items.Count > senderComboBox.MaxDropDownItems)
                ? SystemInformation.VerticalScrollBarWidth : 0;

            int newWidth;
            foreach (string s in ((ComboBox)sender).Items)
            {
                newWidth = (int)g.MeasureString(s, font).Width
                    + vertScrollBarWidth;
                if (width < newWidth)
                {
                    width = newWidth;
                }
            }
            senderComboBox.DropDownWidth = width;
        }
    }
}
