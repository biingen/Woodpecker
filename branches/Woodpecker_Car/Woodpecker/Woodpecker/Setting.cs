using DirectX.Capture;
using jini;
using MaterialSkin;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Management;
using System.Windows.Forms;
using System.Xml.Linq;

namespace Woodpecker
{
    public partial class Setting : Form
    {
        public Setting()
        {
            InitializeComponent();
            setStyle();

        }

        string MainSettingPath = Application.StartupPath + "\\Config.ini";
        string MailPath = Application.StartupPath + "\\Mail.ini";
        private CAN_Reader MYCanReader = new CAN_Reader();

        private void setStyle()
        {
            // Button design
            List<Button> buttonsList = new List<Button> { button_Save, button_ImagePath, button_LogPath, button_GeneratorPath, button_DosPath };
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

        private void button_ImagePath_Click(object sender, EventArgs e)
        {
            //Save Video Path
            folderBrowserDialog1.ShowDialog();
            if (folderBrowserDialog1.SelectedPath == "")
            {
                textBox_ImagePath.Text = textBox_ImagePath.Text;
            }
            else
            {
                textBox_ImagePath.Text = folderBrowserDialog1.SelectedPath;
            }
        }

        private void button_LogPath_Click(object sender, EventArgs e)
        {
            //log file save path
            folderBrowserDialog2.ShowDialog();
            if (folderBrowserDialog2.SelectedPath == "")
            {
                textBox_LogPath.Text = textBox_LogPath.Text;
            }
            else
            {
                textBox_LogPath.Text = folderBrowserDialog2.SelectedPath;
            }
        }

        private void button_GeneratorPath_Click(object sender, EventArgs e)
        {
            // Generator Command Path
            openFileDialog1.Filter = "XML files (*.xml)|*.xml";
            openFileDialog1.ShowDialog();
            if (openFileDialog1.FileName == "")
            {
                textBox_GeneratorPath.Text = textBox_GeneratorPath.Text;
            }
            else
            {
                textBox_GeneratorPath.Text = openFileDialog1.FileName;
            }
        }

        private void button_DosPath_Click(object sender, EventArgs e)
        {
            folderBrowserDialog3.ShowDialog();
            if (folderBrowserDialog3.SelectedPath == "")
            {
                textBox_DosPath.Text = textBox_DosPath.Text;
            }
            else
            {
                textBox_DosPath.Text = folderBrowserDialog3.SelectedPath;
            }
        }

        public void OkBtn_Click(object sender, EventArgs e)
        {
            //Image Path//
            ini12.INIWrite(MainSettingPath, "Record", "VideoPath", textBox_ImagePath.Text.Trim());

            //Log Path//
            ini12.INIWrite(MainSettingPath, "Record", "LogPath", textBox_LogPath.Text.Trim());

            //Generator Path//
            ini12.INIWrite(MainSettingPath, "Record", "Generator", textBox_GeneratorPath.Text.Trim());

            //DOS Path//
            ini12.INIWrite(MainSettingPath, "Device", "DOS", textBox_DosPath.Text.Trim());

            //Camera Device, Audio//
            ini12.INIWrite(MainSettingPath, "Camera", "VideoIndex", comboBox_CameraDevice.SelectedIndex.ToString());
            ini12.INIWrite(MainSettingPath, "Camera", "VideoName", comboBox_CameraDevice.Text);
            ini12.INIWrite(MainSettingPath, "Camera", "AudioIndex", comboBox_CameraAudio.SelectedIndex.ToString());
            ini12.INIWrite(MainSettingPath, "Camera", "AudioName", comboBox_CameraAudio.Text);

            //SerialPort1//
            if (checkBox_SerialPort1.Checked == true)
            {
                ini12.INIWrite(MainSettingPath, "Port A", "BaudRate", comboBox_SerialPort1_BaudRate_Value.Text.Trim());
                ini12.INIWrite(MainSettingPath, "Port A", "DataBit", "8");
                ini12.INIWrite(MainSettingPath, "Port A", "StopBits", "One");
                ini12.INIWrite(MainSettingPath, "Port A", "PortName", comboBox_SerialPort1_PortName_Value.Text);
            }

            //SerialPort2//
            if (checkBox_SerialPort2.Checked == true)
            {
                ini12.INIWrite(MainSettingPath, "Port B", "BaudRate", comboBox_SerialPort2_BaudRate_Value.Text.Trim());
                ini12.INIWrite(MainSettingPath, "Port B", "DataBit", "8");
                ini12.INIWrite(MainSettingPath, "Port B", "StopBits", "One");
                ini12.INIWrite(MainSettingPath, "Port B", "PortName", comboBox_SerialPort2_PortName_Value.Text);
            }

            //SerialPort3//
            if (checkBox_SerialPort3.Checked == true)
            {
                ini12.INIWrite(MainSettingPath, "Port C", "BaudRate", comboBox_SerialPort3_BaudRate_Value.Text.Trim());
                ini12.INIWrite(MainSettingPath, "Port C", "DataBit", "8");
                ini12.INIWrite(MainSettingPath, "Port C", "StopBits", "One");
                ini12.INIWrite(MainSettingPath, "Port C", "PortName", comboBox_SerialPort3_PortName_Value.Text);
            }
        }

        //Add_ons addOns = new Add_ons();
        private void Setting_Load(object sender, EventArgs e)
        {
            checkCamera();
            checkAutokit();

            //addOns.USB_Read();

            //Image欄位//
            if (Directory.Exists(ini12.INIRead(MainSettingPath, "Record", "VideoPath", "")))
            {
                textBox_ImagePath.Text = ini12.INIRead(MainSettingPath, "Record", "VideoPath", "");
            }
            else if (ini12.INIRead(MainSettingPath, "Record", "VideoPath", "") == "" || Directory.Exists(ini12.INIRead(MainSettingPath, "Record", "VideoPath", "")) == false)
            {
                Directory.CreateDirectory(System.Windows.Forms.Application.StartupPath + "\\Image");
                textBox_ImagePath.Text = System.Windows.Forms.Application.StartupPath + "\\Image";
                ini12.INIWrite(MainSettingPath, "Record", "VideoPath", textBox_ImagePath.Text.Trim());
            }
            else
            {
                textBox_ImagePath.Text = "";
                ini12.INIWrite(MainSettingPath, "Record", "VideoPath", textBox_ImagePath.Text.Trim());
                pictureBox_ImagePath.Image = Properties.Resources.ERROR;
            }

            //Log欄位//
            if (Directory.Exists(ini12.INIRead(MainSettingPath, "Record", "LogPath", "")))
            {
                textBox_LogPath.Text = ini12.INIRead(MainSettingPath, "Record", "LogPath", "");
            }
            else if (ini12.INIRead(MainSettingPath, "Record", "LogPath", "") == "" || Directory.Exists(ini12.INIRead(MainSettingPath, "Record", "LogPath", "")) == false)
            {
                Directory.CreateDirectory(System.Windows.Forms.Application.StartupPath + "\\Log");
                textBox_LogPath.Text = System.Windows.Forms.Application.StartupPath + "\\Log";
                ini12.INIWrite(MainSettingPath, "Record", "LogPath", textBox_LogPath.Text.Trim());
            }
            else
            {
                textBox_LogPath.Text = "";
                ini12.INIWrite(MainSettingPath, "Record", "LogPath", textBox_LogPath.Text.Trim());
                pictureBox_LogPath.Image = Properties.Resources.ERROR;
            }

            //Generator欄位//
            if (File.Exists(ini12.INIRead(MainSettingPath, "Record", "Generator", "")))
            {
                textBox_GeneratorPath.Text = ini12.INIRead(MainSettingPath, "Record", "Generator", "");
            }
            else
            {
                textBox_GeneratorPath.Text = "";
                //pictureBox_GeneratorPath.Image = Properties.Resources.ERROR;
            }

            //DOS欄位//
            if (Directory.Exists(ini12.INIRead(MainSettingPath, "Device", "DOS", "")))
            {
                textBox_DosPath.Text = ini12.INIRead(MainSettingPath, "Device", "DOS", "");
            }
            else
            {
                textBox_DosPath.Text = "";
                //pictureBox_DosPath.Image = Properties.Resources.ERROR;
            }


            if (ini12.INIRead(MainSettingPath, "Canbus", "Log", "") == "1")
            {
                checkBox_CAN_Log.Checked = true;
                comboBox_CAN_DevIndex.Enabled = true;
                comboBox_CAN_BaudRate_Value.Enabled = true;
            }
            else
            {
                checkBox_CAN_Log.Checked = false;
                comboBox_CAN_DevIndex.Enabled = false;
                comboBox_CAN_BaudRate_Value.Enabled = false;
            }

            #region -- SerialPort --
            if (ini12.INIRead(MainSettingPath, "Device", "AutoboxExist", "") == "1")
            {
                GroupBox_Rs232.Text = ini12.INIRead(MainSettingPath, "Device", "AutoboxPort", "") + " IS USING";

                comboBox_SerialPort1_PortName_Value.DataSource = System.IO.Ports.SerialPort.GetPortNames();
                comboBox_SerialPort2_PortName_Value.DataSource = System.IO.Ports.SerialPort.GetPortNames();
                comboBox_SerialPort3_PortName_Value.DataSource = System.IO.Ports.SerialPort.GetPortNames();
                comboBox_SerialPort4_PortName_Value.DataSource = System.IO.Ports.SerialPort.GetPortNames();
                comboBox_SerialPort5_PortName_Value.DataSource = System.IO.Ports.SerialPort.GetPortNames();
                comboBox_KlinePort_PortName_Value.DataSource = System.IO.Ports.SerialPort.GetPortNames();

                if (ini12.INIRead(MainSettingPath, "Port A", "Checked", "") == "1")
                {
                    checkBox_SerialPort1.Checked = true;
                    comboBox_SerialPort1_BaudRate_Value.Enabled = true;
                    comboBox_SerialPort1_PortName_Value.Enabled = true;
                }
                else if (ini12.INIRead(MainSettingPath, "Port A", "Checked", "") == "0" || ini12.INIRead(MainSettingPath, "Port A", "Checked", "") == "")
                {
                    checkBox_SerialPort1.Checked = false;
                    comboBox_SerialPort1_BaudRate_Value.Enabled = false;
                    comboBox_SerialPort1_PortName_Value.Enabled = false;
                }

                if (ini12.INIRead(MainSettingPath, "Port B", "Checked", "") == "1")
                {
                    checkBox_SerialPort2.Checked = true;
                    comboBox_SerialPort2_BaudRate_Value.Enabled = true;
                    comboBox_SerialPort2_PortName_Value.Enabled = true;
                }
                else if (ini12.INIRead(MainSettingPath, "Port B", "Checked", "") == "0" || ini12.INIRead(MainSettingPath, "Port B", "Checked", "") == "")
                {
                    checkBox_SerialPort2.Checked = false;
                    comboBox_SerialPort2_BaudRate_Value.Enabled = false;
                    comboBox_SerialPort2_PortName_Value.Enabled = false;
                }

                if (ini12.INIRead(MainSettingPath, "Port C", "Checked", "") == "1")
                {
                    checkBox_SerialPort3.Checked = true;
                    comboBox_SerialPort3_BaudRate_Value.Enabled = true;
                    comboBox_SerialPort3_PortName_Value.Enabled = true;
                }
                else if (ini12.INIRead(MainSettingPath, "Port C", "Checked", "") == "0" || ini12.INIRead(MainSettingPath, "Port C", "Checked", "") == "")
                {
                    checkBox_SerialPort3.Checked = false;
                    comboBox_SerialPort3_BaudRate_Value.Enabled = false;
                    comboBox_SerialPort3_PortName_Value.Enabled = false;
                }

                if (ini12.INIRead(MainSettingPath, "Port D", "Checked", "") == "1")
                {
                    checkBox_SerialPort4.Checked = true;
                    comboBox_SerialPort4_BaudRate_Value.Enabled = true;
                    comboBox_SerialPort4_PortName_Value.Enabled = true;
                }
                else if (ini12.INIRead(MainSettingPath, "Port D", "Checked", "") == "0" || ini12.INIRead(MainSettingPath, "Port D", "Checked", "") == "")
                {
                    checkBox_SerialPort4.Checked = false;
                    comboBox_SerialPort4_BaudRate_Value.Enabled = false;
                    comboBox_SerialPort4_PortName_Value.Enabled = false;
                }

                if (ini12.INIRead(MainSettingPath, "Port E", "Checked", "") == "1")
                {
                    checkBox_SerialPort5.Checked = true;
                    comboBox_SerialPort5_BaudRate_Value.Enabled = true;
                    comboBox_SerialPort5_PortName_Value.Enabled = true;
                }
                else if (ini12.INIRead(MainSettingPath, "Port E", "Checked", "") == "0" || ini12.INIRead(MainSettingPath, "Port E", "Checked", "") == "")
                {
                    checkBox_SerialPort5.Checked = false;
                    comboBox_SerialPort5_BaudRate_Value.Enabled = false;
                    comboBox_SerialPort5_PortName_Value.Enabled = false;
                }

                if (ini12.INIRead(MainSettingPath, "Displayhex", "Checked", "") == "1")
                {
                    checkBox_Displayhex.Checked = true;
                }
                else if (ini12.INIRead(MainSettingPath, "Displayhex", "Checked", "") == "0" || ini12.INIRead(MainSettingPath, "Displayhex", "Checked", "") == "")
                {
                    checkBox_Displayhex.Checked = false;
                }

            }
            else
            {
                GroupBox_Rs232.Text = "AUTOKIT IS NOT CONNECTED";

                comboBox_SerialPort1_PortName_Value.DataSource = System.IO.Ports.SerialPort.GetPortNames();
                comboBox_SerialPort2_PortName_Value.DataSource = System.IO.Ports.SerialPort.GetPortNames();
                comboBox_SerialPort3_PortName_Value.DataSource = System.IO.Ports.SerialPort.GetPortNames();
                comboBox_SerialPort4_PortName_Value.DataSource = System.IO.Ports.SerialPort.GetPortNames();
                comboBox_SerialPort5_PortName_Value.DataSource = System.IO.Ports.SerialPort.GetPortNames();
                comboBox_KlinePort_PortName_Value.DataSource = System.IO.Ports.SerialPort.GetPortNames();

                if (ini12.INIRead(MainSettingPath, "Port A", "Checked", "") == "1")
                {
                    checkBox_SerialPort1.Checked = true;
                    comboBox_SerialPort1_BaudRate_Value.Enabled = true;
                    comboBox_SerialPort1_PortName_Value.Enabled = true;
                }
                else if (ini12.INIRead(MainSettingPath, "Port A", "Checked", "") == "0" || ini12.INIRead(MainSettingPath, "Port A", "Checked", "") == "")
                {
                    checkBox_SerialPort1.Checked = false;
                    comboBox_SerialPort1_BaudRate_Value.Enabled = false;
                    comboBox_SerialPort1_PortName_Value.Enabled = false;
                }

                if (ini12.INIRead(MainSettingPath, "Port B", "Checked", "") == "1")
                {
                    checkBox_SerialPort2.Checked = true;
                    comboBox_SerialPort2_BaudRate_Value.Enabled = true;
                    comboBox_SerialPort2_PortName_Value.Enabled = true;
                }
                else if (ini12.INIRead(MainSettingPath, "Port B", "Checked", "") == "0" || ini12.INIRead(MainSettingPath, "Port B", "Checked", "") == "")
                {
                    checkBox_SerialPort2.Checked = false;
                    comboBox_SerialPort2_BaudRate_Value.Enabled = false;
                    comboBox_SerialPort2_PortName_Value.Enabled = false;
                }

                if (ini12.INIRead(MainSettingPath, "Port C", "Checked", "") == "1")
                {
                    checkBox_SerialPort3.Checked = true;
                    comboBox_SerialPort3_BaudRate_Value.Enabled = true;
                    comboBox_SerialPort3_PortName_Value.Enabled = true;
                }
                else if (ini12.INIRead(MainSettingPath, "Port C", "Checked", "") == "0" || ini12.INIRead(MainSettingPath, "Port C", "Checked", "") == "")
                {
                    checkBox_SerialPort3.Checked = false;
                    comboBox_SerialPort3_BaudRate_Value.Enabled = false;
                    comboBox_SerialPort3_PortName_Value.Enabled = false;
                }

                if (ini12.INIRead(MainSettingPath, "Port D", "Checked", "") == "1")
                {
                    checkBox_SerialPort4.Checked = true;
                    comboBox_SerialPort4_BaudRate_Value.Enabled = true;
                    comboBox_SerialPort4_PortName_Value.Enabled = true;
                }
                else if (ini12.INIRead(MainSettingPath, "Port D", "Checked", "") == "0" || ini12.INIRead(MainSettingPath, "Port D", "Checked", "") == "")
                {
                    checkBox_SerialPort4.Checked = false;
                    comboBox_SerialPort4_BaudRate_Value.Enabled = false;
                    comboBox_SerialPort4_PortName_Value.Enabled = false;
                }

                if (ini12.INIRead(MainSettingPath, "Port E", "Checked", "") == "1")
                {
                    checkBox_SerialPort5.Checked = true;
                    comboBox_SerialPort5_BaudRate_Value.Enabled = true;
                    comboBox_SerialPort5_PortName_Value.Enabled = true;
                }
                else if (ini12.INIRead(MainSettingPath, "Port E", "Checked", "") == "0" || ini12.INIRead(MainSettingPath, "Port E", "Checked", "") == "")
                {
                    checkBox_SerialPort5.Checked = false;
                    comboBox_SerialPort5_BaudRate_Value.Enabled = false;
                    comboBox_SerialPort5_PortName_Value.Enabled = false;
                }

                if (ini12.INIRead(MainSettingPath, "Displayhex", "Checked", "") == "1")
                {
                    checkBox_Displayhex.Checked = true;
                }
                else if (ini12.INIRead(MainSettingPath, "Displayhex", "Checked", "") == "0" || ini12.INIRead(MainSettingPath, "Displayhex", "Checked", "") == "")
                {
                    checkBox_Displayhex.Checked = false;
                }
            }

            if (ini12.INIRead(MainSettingPath, "Kline", "Checked", "") == "1")
            {
                checkBox_Kline.Checked = true;
                comboBox_KlinePort_PortName_Value.Enabled = true;
            }
            else if (ini12.INIRead(MainSettingPath, "Kline", "Checked", "") == "0" || ini12.INIRead(MainSettingPath, "Kline", "Checked", "") == "")
            {
                checkBox_Kline.Checked = false;
                comboBox_KlinePort_PortName_Value.Enabled = false;
            }

            comboBox_SerialPort1_BaudRate_Value.Text = ini12.INIRead(MainSettingPath, "Port A", "BaudRate", "");
            comboBox_SerialPort1_PortName_Value.Text = ini12.INIRead(MainSettingPath, "Port A", "PortName", "");
            comboBox_SerialPort2_BaudRate_Value.Text = ini12.INIRead(MainSettingPath, "Port B", "BaudRate", "");
            comboBox_SerialPort2_PortName_Value.Text = ini12.INIRead(MainSettingPath, "Port B", "PortName", "");
            comboBox_SerialPort3_BaudRate_Value.Text = ini12.INIRead(MainSettingPath, "Port C", "BaudRate", "");
            comboBox_SerialPort3_PortName_Value.Text = ini12.INIRead(MainSettingPath, "Port C", "PortName", "");
            comboBox_SerialPort4_BaudRate_Value.Text = ini12.INIRead(MainSettingPath, "Port D", "BaudRate", "");
            comboBox_SerialPort4_PortName_Value.Text = ini12.INIRead(MainSettingPath, "Port D", "PortName", "");
            comboBox_SerialPort5_BaudRate_Value.Text = ini12.INIRead(MainSettingPath, "Port E", "BaudRate", "");
            comboBox_SerialPort5_PortName_Value.Text = ini12.INIRead(MainSettingPath, "Port E", "PortName", "");
            comboBox_KlinePort_PortName_Value.Text = ini12.INIRead(MainSettingPath, "Kline", "PortName", "");
            #endregion

            #region -- Camera --
            if (ini12.INIRead(MainSettingPath, "Device", "CameraExist", "") == "1")//Camera存在//
            {
                comboBox_CameraDevice.Enabled = true;
                comboBox_CameraAudio.Enabled = true;
                try
                {
                    Filters filters = new Filters();
                    Filter f;

                    ini12.INIWrite(MainSettingPath, "Camera", "VideoNumber", filters.VideoInputDevices.Count.ToString());
                    ini12.INIWrite(MainSettingPath, "Camera", "AudioNumber", filters.AudioInputDevices.Count.ToString());

                    for (int c = 0; c < filters.VideoInputDevices.Count; c++)
                    {
                        f = filters.VideoInputDevices[c];
                        comboBox_CameraDevice.Items.Add(f.Name);
                        if (f.Name == ini12.INIRead(MainSettingPath, "Camera", "VideoName", ""))
                        {
                            comboBox_CameraDevice.Text = ini12.INIRead(MainSettingPath, "Camera", "VideoName", "");
                        }
                    }

                    if (comboBox_CameraDevice.Text == "" && filters.VideoInputDevices.Count > 0)
                    {
                        comboBox_CameraDevice.SelectedIndex = filters.VideoInputDevices.Count - 1;
                        ini12.INIWrite(MainSettingPath, "Camera", "VideoIndex", comboBox_CameraDevice.SelectedIndex.ToString());
                        ini12.INIWrite(MainSettingPath, "Camera", "VideoName", comboBox_CameraDevice.Text);
                    }

                    for (int j = 0; j < filters.AudioInputDevices.Count; j++)
                    {
                        f = filters.AudioInputDevices[j];
                        comboBox_CameraAudio.Items.Add(f.Name);
                        if (f.Name == ini12.INIRead(MainSettingPath, "Camera", "AudioName", ""))
                        {
                            comboBox_CameraAudio.Text = ini12.INIRead(MainSettingPath, "Camera", "AudioName", "");
                        }
                    }

                    if (comboBox_CameraAudio.Text == "" && filters.AudioInputDevices.Count > 0)
                    {
                        comboBox_CameraAudio.SelectedIndex = filters.AudioInputDevices.Count - 1;
                        ini12.INIWrite(MainSettingPath, "Camera", "AudioIndex", comboBox_CameraAudio.SelectedIndex.ToString());
                        ini12.INIWrite(MainSettingPath, "Camera", "AudioName", comboBox_CameraAudio.Text);
                    }
                    label_resolution.Text = ini12.INIRead(MainSettingPath, "Camera", "Resolution", "");
                }
                catch (Exception)
                {
                    MessageBox.Show("Please reload the setting to reset Camera.", "Camera Error");
                    ini12.INIWrite(MainSettingPath, "Device", "CameraExist", "0");
                }
            }
            else
            {
                comboBox_CameraDevice.Enabled = false;
                comboBox_CameraAudio.Enabled = false;
            }
            #endregion

            #region -- Canbus --
            if (ini12.INIRead(MainSettingPath, "Device", "CANbusExist", "") == "1")     //Canbus存在//
            {
                string[] dev_list = ini12.INIRead(MainSettingPath, "Canbus", "DevName", "").Split(',');
                comboBox_CAN_DevIndex.Items.Clear();
                foreach (String dev_str in dev_list)
                {
                    comboBox_CAN_DevIndex.Items.Add(dev_str);
                }

                if (comboBox_CAN_DevIndex.Items.Count > 0)
                {
                    if (ini12.INIRead(MainSettingPath, "Canbus", "DevIndex", "") != "")
                        comboBox_CAN_DevIndex.SelectedIndex = Convert.ToInt16(ini12.INIRead(MainSettingPath, "Canbus", "DevIndex", ""));
                    else
                        comboBox_CAN_DevIndex.SelectedIndex = 0;
                    comboBox_CAN_DevIndex.MaxDropDownItems = comboBox_CAN_DevIndex.Items.Count;
                }

                comboBox_CAN_BaudRate_Value.Text = ini12.INIRead(MainSettingPath, "Canbus", "BaudRate", "");
            }
            else
            {
                checkBox_CAN_Log.Checked = false;
                comboBox_CAN_DevIndex.Enabled = false;
                comboBox_CAN_BaudRate_Value.Enabled = false;
            }
            #endregion

            if (ini12.INIRead(MainSettingPath, "LogSearch", "TextNum", "") == "")
            {
                ini12.INIWrite(MainSettingPath, "LogSearch", "TextNum", "0");
            }
        }

        private void checkCamera()
        {
            ManagementObjectSearcher search = new ManagementObjectSearcher("SELECT * FROM Win32_PnPEntity");
            ManagementObjectCollection collection = search.Get();
            var usbList = from u in collection.Cast<ManagementBaseObject>()
                          select new
                          {
                              id = u.GetPropertyValue("DeviceID"),
                              name = u.GetPropertyValue("Name"),
                              description = u.GetPropertyValue("Description"),
                              status = u.GetPropertyValue("Status"),
                              system = u.GetPropertyValue("SystemName"),
                              caption = u.GetPropertyValue("Caption"),
                              pnp = u.GetPropertyValue("PNPDeviceID"),
                          };

            foreach (var usbDevice in usbList)
            {
                string deviceId = (string)usbDevice.id;
                string deviceTp = (string)usbDevice.name;
                string deviecDescription = (string)usbDevice.description;

                string deviceStatus = (string)usbDevice.status;
                string deviceSystem = (string)usbDevice.system;
                string deviceCaption = (string)usbDevice.caption;
                string devicePnp = (string)usbDevice.pnp;

                if (deviecDescription != null)
                {
                    if (deviecDescription.IndexOf("USB 視訊裝置", StringComparison.OrdinalIgnoreCase) >= 0 ||
                        deviecDescription.IndexOf("USB 视频设备", StringComparison.OrdinalIgnoreCase) >= 0 ||
                        deviceTp.IndexOf("Webcam", StringComparison.OrdinalIgnoreCase) >= 0 ||
                        deviceTp.IndexOf("Camera", StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        if (deviceId.IndexOf("VID_", StringComparison.OrdinalIgnoreCase) >= 0)
                        {
                            int vidIndex = deviceId.IndexOf("VID_");
                            string startingAtVid = deviceId.Substring(vidIndex + 4); // + 4 to remove "VID_"
                            string vid = startingAtVid.Substring(0, 4); // vid is four characters long
                            Global.VID.Add(vid);
                        }

                        if (deviceId.IndexOf("PID_", StringComparison.OrdinalIgnoreCase) >= 0)
                        {
                            int pidIndex = deviceId.IndexOf("PID_");
                            string startingAtPid = deviceId.Substring(pidIndex + 4); // + 4 to remove "PID_"
                            string pid = startingAtPid.Substring(0, 4); // pid is four characters long
                            Global.PID.Add(pid);
                        }

                        Console.WriteLine("-----------------Camera------------------");
                        Console.WriteLine("DeviceID: {0}\n" +
                                              "Name: {1}\n" +
                                              "Description: {2}\n" +
                                              "Status: {3}\n" +
                                              "System: {4}\n" +
                                              "Caption: {5}\n" +
                                              "Pnp: {6}\n"
                                              , deviceId, deviceTp, deviecDescription, deviceStatus, deviceSystem, deviceCaption, devicePnp);

                        //Camera存在
                        ini12.INIWrite(Global.MainSettingPath, "Device", "CameraExist", "1");
                    }

                }
                else
                {
                    ini12.INIWrite(Global.MainSettingPath, "Device", "CameraExist", "0");
                }
            }
        }

        private void checkAutokit()
        {
            ini12.INIWrite(Global.MainSettingPath, "Device", "AutoboxExist", "0");
            ManagementObjectSearcher search = new ManagementObjectSearcher("SELECT * FROM Win32_PnPEntity");
            ManagementObjectCollection collection = search.Get();
            var usbList = from u in collection.Cast<ManagementBaseObject>()
                          select new
                          {
                              id = u.GetPropertyValue("DeviceID"),
                              name = u.GetPropertyValue("Name"),
                              description = u.GetPropertyValue("Description"),
                              status = u.GetPropertyValue("Status"),
                              system = u.GetPropertyValue("SystemName"),
                              caption = u.GetPropertyValue("Caption"),
                              pnp = u.GetPropertyValue("PNPDeviceID"),
                          };

            foreach (var usbDevice in usbList)
            {
                string deviceId = (string)usbDevice.id;
                string deviceTp = (string)usbDevice.name;
                string deviecDescription = (string)usbDevice.description;

                string deviceStatus = (string)usbDevice.status;
                string deviceSystem = (string)usbDevice.system;
                string deviceCaption = (string)usbDevice.caption;
                string devicePnp = (string)usbDevice.pnp;

                if (deviecDescription != null)
                {
                    if (deviceId.IndexOf("&0&5", StringComparison.OrdinalIgnoreCase) >= 0 &&
                deviceId.IndexOf("USB\\VID_067B&PID_2303\\", StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        Console.WriteLine("-----------------AutoBox2------------------");
                        Console.WriteLine("DeviceID: {0}\n" +
                                              "Name: {1}\n" +
                                              "Description: {2}\n" +
                                              "Status: {3}\n" +
                                              "System: {4}\n" +
                                              "Caption: {5}\n" +
                                              "Pnp: {6}\n"
                                              , deviceId, deviceTp, deviecDescription, deviceStatus, deviceSystem, deviceCaption, devicePnp);

                        int FirstIndex = deviceTp.IndexOf("(");
                        string AutoBoxPortSubstring = deviceTp.Substring(FirstIndex + 1);
                        string AutoBoxPort = AutoBoxPortSubstring.Substring(0);

                        int AutoBoxPortLengh = AutoBoxPort.Length;
                        string AutoBoxPortFinal = AutoBoxPort.Remove(AutoBoxPortLengh - 1);

                        if (AutoBoxPortSubstring.Substring(0, 3) == "COM")
                        {
                            ini12.INIWrite(Global.MainSettingPath, "Device", "AutoboxExist", "1");
                            ini12.INIWrite(Global.MainSettingPath, "Device", "AutoboxVerson", "2");
                            ini12.INIWrite(Global.MainSettingPath, "Device", "AutoboxPort", AutoBoxPortFinal);
                        }
                    }
                }
            }
        }

        //自動調整ComboBox寬度
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

        private void checkBox_SerialPort1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox_SerialPort1.Checked == true)
            {
                ini12.INIWrite(MainSettingPath, "Port A", "Checked", "1");
                comboBox_SerialPort1_BaudRate_Value.Enabled = true;
                comboBox_SerialPort1_PortName_Value.Enabled = true;
                PortCheck();
            }
            else
            {
                ini12.INIWrite(MainSettingPath, "Port A", "Checked", "0");
                comboBox_SerialPort1_BaudRate_Value.Enabled = false;
                comboBox_SerialPort1_PortName_Value.Enabled = false;
                PortCheck();
            }
        }

        private void checkBox_SerialPort2_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox_SerialPort2.Checked == true)
            {
                ini12.INIWrite(MainSettingPath, "Port B", "Checked", "1");
                comboBox_SerialPort2_BaudRate_Value.Enabled = true;
                comboBox_SerialPort2_PortName_Value.Enabled = true;
                PortCheck();
            }
            else
            {
                ini12.INIWrite(MainSettingPath, "Port B", "Checked", "0");
                comboBox_SerialPort2_BaudRate_Value.Enabled = false;
                comboBox_SerialPort2_PortName_Value.Enabled = false;
                PortCheck();
            }
        }

        private void checkBox_SerialPort3_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox_SerialPort3.Checked == true)
            {
                ini12.INIWrite(MainSettingPath, "Port C", "Checked", "1");
                comboBox_SerialPort3_BaudRate_Value.Enabled = true;
                comboBox_SerialPort3_PortName_Value.Enabled = true;
                PortCheck();
            }
            else
            {
                ini12.INIWrite(MainSettingPath, "Port C", "Checked", "0");
                comboBox_SerialPort3_BaudRate_Value.Enabled = false;
                comboBox_SerialPort3_PortName_Value.Enabled = false;
                PortCheck();
            }
        }

        private void checkBox_SerialPort4_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox_SerialPort4.Checked == true)
            {
                ini12.INIWrite(MainSettingPath, "Port D", "Checked", "1");
                comboBox_SerialPort4_BaudRate_Value.Enabled = true;
                comboBox_SerialPort4_PortName_Value.Enabled = true;
                PortCheck();
            }
            else
            {
                ini12.INIWrite(MainSettingPath, "Port D", "Checked", "0");
                comboBox_SerialPort4_BaudRate_Value.Enabled = false;
                comboBox_SerialPort4_PortName_Value.Enabled = false;
                PortCheck();
            }
        }

        private void checkBox_SerialPort5_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox_SerialPort5.Checked == true)
            {
                ini12.INIWrite(MainSettingPath, "Port E", "Checked", "1");
                comboBox_SerialPort5_BaudRate_Value.Enabled = true;
                comboBox_SerialPort5_PortName_Value.Enabled = true;
                PortCheck();
            }
            else
            {
                ini12.INIWrite(MainSettingPath, "Port E", "Checked", "0");
                comboBox_SerialPort5_BaudRate_Value.Enabled = false;
                comboBox_SerialPort5_PortName_Value.Enabled = false;
                PortCheck();
            }
        }

        private void checkBox_Displayhex_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox_Displayhex.Checked == true)
            {
                ini12.INIWrite(MainSettingPath, "Displayhex", "Checked", "1");
            }
            else
            {
                ini12.INIWrite(MainSettingPath, "Displayhex", "Checked", "0");
            }
        }

        private void PortCheck()
        {
            if (checkBox_SerialPort1.Checked == true)
            {
                if (comboBox_SerialPort1_PortName_Value.Text == ini12.INIRead(MainSettingPath, "Device", "AutoboxPort", "") && comboBox_SerialPort1_PortName_Value.Text != "")
                {
                    label_ErrorMessage.Text = ini12.INIRead(MainSettingPath, "Device", "AutoboxPort", "") + " is using!";
                    pictureBox_SerialPort1.Image = Properties.Resources.ERROR;
                }
                else if (checkBox_SerialPort2.Checked == true &&
                        (comboBox_SerialPort1_PortName_Value.Text == comboBox_SerialPort2_PortName_Value.Text))
                {
                    label_ErrorMessage.Text = "Duplicate SerialPort ";
                    pictureBox_SerialPort1.Image = Properties.Resources.ERROR;
                }
                else if (checkBox_SerialPort3.Checked == true &&
                        (comboBox_SerialPort1_PortName_Value.Text == comboBox_SerialPort3_PortName_Value.Text))
                {
                    label_ErrorMessage.Text = "Duplicate SerialPort ";
                    pictureBox_SerialPort1.Image = Properties.Resources.ERROR;
                }
                else if (checkBox_SerialPort4.Checked == true &&
                        (comboBox_SerialPort1_PortName_Value.Text == comboBox_SerialPort4_PortName_Value.Text))
                {
                    label_ErrorMessage.Text = "Duplicate SerialPort ";
                    pictureBox_SerialPort1.Image = Properties.Resources.ERROR;
                }
                else if (checkBox_SerialPort5.Checked == true &&
                        (comboBox_SerialPort1_PortName_Value.Text == comboBox_SerialPort5_PortName_Value.Text))
                {
                    label_ErrorMessage.Text = "Duplicate SerialPort ";
                    pictureBox_SerialPort1.Image = Properties.Resources.ERROR;
                }
                else if (checkBox_Kline.Checked == true &&
                        (comboBox_SerialPort1_PortName_Value.Text == comboBox_KlinePort_PortName_Value.Text))
                {
                    label_ErrorMessage.Text = "Duplicate SerialPort ";
                    pictureBox_SerialPort1.Image = Properties.Resources.ERROR;
                }
                else
                {
                    pictureBox_SerialPort1.Image = null;
                }
            }
            else if (checkBox_SerialPort1.Checked == false)
            {
                pictureBox_SerialPort1.Image = null;
            }

            if (checkBox_SerialPort2.Checked == true)
            {
                if (comboBox_SerialPort2_PortName_Value.Text == ini12.INIRead(MainSettingPath, "Device", "AutoboxPort", "") && comboBox_SerialPort2_PortName_Value.Text != "")
                {
                    label_ErrorMessage.Text = ini12.INIRead(MainSettingPath, "Device", "AutoboxPort", "") + " is using!";
                    pictureBox_SerialPort2.Image = Properties.Resources.ERROR;
                }
                else if (checkBox_SerialPort1.Checked == true &&
                        (comboBox_SerialPort2_PortName_Value.Text == comboBox_SerialPort1_PortName_Value.Text))
                {
                    label_ErrorMessage.Text = "Duplicate SerialPort ";
                    pictureBox_SerialPort2.Image = Properties.Resources.ERROR;
                }
                else if (checkBox_SerialPort3.Checked == true &&
                        (comboBox_SerialPort2_PortName_Value.Text == comboBox_SerialPort3_PortName_Value.Text))
                {
                    label_ErrorMessage.Text = "Duplicate SerialPort ";
                    pictureBox_SerialPort2.Image = Properties.Resources.ERROR;
                }
                else if (checkBox_SerialPort4.Checked == true &&
                        (comboBox_SerialPort2_PortName_Value.Text == comboBox_SerialPort4_PortName_Value.Text))
                {
                    label_ErrorMessage.Text = "Duplicate SerialPort ";
                    pictureBox_SerialPort2.Image = Properties.Resources.ERROR;
                }
                else if (checkBox_SerialPort5.Checked == true &&
                        (comboBox_SerialPort2_PortName_Value.Text == comboBox_SerialPort5_PortName_Value.Text))
                {
                    label_ErrorMessage.Text = "Duplicate SerialPort ";
                    pictureBox_SerialPort2.Image = Properties.Resources.ERROR;
                }
                else if (checkBox_Kline.Checked == true &&
                        (comboBox_SerialPort2_PortName_Value.Text == comboBox_KlinePort_PortName_Value.Text))
                {
                    label_ErrorMessage.Text = "Duplicate SerialPort ";
                    pictureBox_SerialPort2.Image = Properties.Resources.ERROR;
                }
                else
                {
                    pictureBox_SerialPort2.Image = null;
                }
            }
            else if (checkBox_SerialPort2.Checked == false)
            {
                pictureBox_SerialPort2.Image = null;
            }

            if (checkBox_SerialPort3.Checked == true)
            {
                if (comboBox_SerialPort3_PortName_Value.Text == ini12.INIRead(MainSettingPath, "Device", "AutoboxPort", "") && comboBox_SerialPort3_PortName_Value.Text != "")
                {
                    label_ErrorMessage.Text = ini12.INIRead(MainSettingPath, "Device", "AutoboxPort", "") + " is using!";
                    pictureBox_SerialPort3.Image = Properties.Resources.ERROR;
                }
                else if (checkBox_SerialPort1.Checked == true &&
                        (comboBox_SerialPort3_PortName_Value.Text == comboBox_SerialPort1_PortName_Value.Text))
                {
                    label_ErrorMessage.Text = "Duplicate SerialPort ";
                    pictureBox_SerialPort3.Image = Properties.Resources.ERROR;
                }
                else if (checkBox_SerialPort2.Checked == true &&
                        (comboBox_SerialPort3_PortName_Value.Text == comboBox_SerialPort2_PortName_Value.Text))
                {
                    label_ErrorMessage.Text = "Duplicate SerialPort ";
                    pictureBox_SerialPort3.Image = Properties.Resources.ERROR;
                }
                else if (checkBox_SerialPort4.Checked == true &&
                        (comboBox_SerialPort3_PortName_Value.Text == comboBox_SerialPort4_PortName_Value.Text))
                {
                    label_ErrorMessage.Text = "Duplicate SerialPort ";
                    pictureBox_SerialPort3.Image = Properties.Resources.ERROR;
                }
                else if (checkBox_SerialPort5.Checked == true &&
                        (comboBox_SerialPort3_PortName_Value.Text == comboBox_SerialPort5_PortName_Value.Text))
                {
                    label_ErrorMessage.Text = "Duplicate SerialPort ";
                    pictureBox_SerialPort3.Image = Properties.Resources.ERROR;
                }
                else if (checkBox_Kline.Checked == true &&
                        (comboBox_SerialPort3_PortName_Value.Text == comboBox_KlinePort_PortName_Value.Text))
                {
                    label_ErrorMessage.Text = "Duplicate SerialPort ";
                    pictureBox_SerialPort3.Image = Properties.Resources.ERROR;
                }
                else
                {
                    pictureBox_SerialPort3.Image = null;
                }
            }
            else if (checkBox_SerialPort3.Checked == false)
            {
                pictureBox_SerialPort3.Image = null;
            }

            if (checkBox_SerialPort4.Checked == true)
            {
                if (comboBox_SerialPort4_PortName_Value.Text == ini12.INIRead(MainSettingPath, "Device", "AutoboxPort", "") && comboBox_SerialPort4_PortName_Value.Text != "")
                {
                    label_ErrorMessage.Text = ini12.INIRead(MainSettingPath, "Device", "AutoboxPort", "") + " is using!";
                    pictureBox_SerialPort4.Image = Properties.Resources.ERROR;
                }
                else if (checkBox_SerialPort1.Checked == true &&
                        (comboBox_SerialPort4_PortName_Value.Text == comboBox_SerialPort1_PortName_Value.Text))
                {
                    label_ErrorMessage.Text = "Duplicate SerialPort ";
                    pictureBox_SerialPort4.Image = Properties.Resources.ERROR;
                }
                else if (checkBox_SerialPort2.Checked == true &&
                        (comboBox_SerialPort4_PortName_Value.Text == comboBox_SerialPort2_PortName_Value.Text))
                {
                    label_ErrorMessage.Text = "Duplicate SerialPort ";
                    pictureBox_SerialPort4.Image = Properties.Resources.ERROR;
                }
                else if (checkBox_SerialPort3.Checked == true &&
                        (comboBox_SerialPort4_PortName_Value.Text == comboBox_SerialPort3_PortName_Value.Text))
                {
                    label_ErrorMessage.Text = "Duplicate SerialPort ";
                    pictureBox_SerialPort4.Image = Properties.Resources.ERROR;
                }
                else if (checkBox_SerialPort5.Checked == true &&
                        (comboBox_SerialPort4_PortName_Value.Text == comboBox_SerialPort5_PortName_Value.Text))
                {
                    label_ErrorMessage.Text = "Duplicate SerialPort ";
                    pictureBox_SerialPort4.Image = Properties.Resources.ERROR;
                }
                else if (checkBox_Kline.Checked == true &&
                        (comboBox_SerialPort4_PortName_Value.Text == comboBox_KlinePort_PortName_Value.Text))
                {
                    label_ErrorMessage.Text = "Duplicate SerialPort ";
                    pictureBox_SerialPort4.Image = Properties.Resources.ERROR;
                }
                else
                {
                    pictureBox_SerialPort4.Image = null;
                }
            }
            else if (checkBox_SerialPort4.Checked == false)
            {
                pictureBox_SerialPort4.Image = null;
            }

            if (checkBox_SerialPort5.Checked == true)
            {
                if (comboBox_SerialPort5_PortName_Value.Text == ini12.INIRead(MainSettingPath, "Device", "AutoboxPort", "") && comboBox_SerialPort5_PortName_Value.Text != "")
                {
                    label_ErrorMessage.Text = ini12.INIRead(MainSettingPath, "Device", "AutoboxPort", "") + " is using!";
                    pictureBox_SerialPort5.Image = Properties.Resources.ERROR;
                }
                else if (checkBox_SerialPort1.Checked == true &&
                        (comboBox_SerialPort5_PortName_Value.Text == comboBox_SerialPort1_PortName_Value.Text))
                {
                    label_ErrorMessage.Text = "Duplicate SerialPort ";
                    pictureBox_SerialPort5.Image = Properties.Resources.ERROR;
                }
                else if (checkBox_SerialPort2.Checked == true &&
                        (comboBox_SerialPort5_PortName_Value.Text == comboBox_SerialPort2_PortName_Value.Text))
                {
                    label_ErrorMessage.Text = "Duplicate SerialPort ";
                    pictureBox_SerialPort4.Image = Properties.Resources.ERROR;
                }
                else if (checkBox_SerialPort3.Checked == true &&
                        (comboBox_SerialPort5_PortName_Value.Text == comboBox_SerialPort3_PortName_Value.Text))
                {
                    label_ErrorMessage.Text = "Duplicate SerialPort ";
                    pictureBox_SerialPort4.Image = Properties.Resources.ERROR;
                }
                else if (checkBox_SerialPort4.Checked == true &&
                        (comboBox_SerialPort5_PortName_Value.Text == comboBox_SerialPort4_PortName_Value.Text))
                {
                    label_ErrorMessage.Text = "Duplicate SerialPort ";
                    pictureBox_SerialPort4.Image = Properties.Resources.ERROR;
                }
                else if (checkBox_Kline.Checked == true &&
                        (comboBox_SerialPort5_PortName_Value.Text == comboBox_KlinePort_PortName_Value.Text))
                {
                    label_ErrorMessage.Text = "Duplicate SerialPort ";
                    pictureBox_SerialPort5.Image = Properties.Resources.ERROR;
                }
                else
                {
                    pictureBox_SerialPort5.Image = null;
                }
            }
            else if (checkBox_SerialPort5.Checked == false)
            {
                pictureBox_SerialPort5.Image = null;
            }

            if (checkBox_Kline.Checked == true)
            {
                if (comboBox_KlinePort_PortName_Value.Text == ini12.INIRead(MainSettingPath, "Device", "AutoboxPort", ""))
                {
                    label_ErrorMessage.Text = ini12.INIRead(MainSettingPath, "Device", "AutoboxPort", "") + " is using!";
                    pictureBox_klinePort.Image = Properties.Resources.ERROR;
                }
                else if (checkBox_SerialPort1.Checked == true &&
                        (comboBox_KlinePort_PortName_Value.Text == comboBox_SerialPort1_PortName_Value.Text))
                {
                    label_ErrorMessage.Text = "Duplicate SerialPort ";
                    pictureBox_klinePort.Image = Properties.Resources.ERROR;
                }
                else if (checkBox_SerialPort2.Checked == true &&
                        (comboBox_KlinePort_PortName_Value.Text == comboBox_SerialPort2_PortName_Value.Text))
                {
                    label_ErrorMessage.Text = "Duplicate SerialPort ";
                    pictureBox_klinePort.Image = Properties.Resources.ERROR;
                }
                else if (checkBox_SerialPort3.Checked == true &&
                        (comboBox_KlinePort_PortName_Value.Text == comboBox_SerialPort3_PortName_Value.Text))
                {
                    label_ErrorMessage.Text = "Duplicate SerialPort ";
                    pictureBox_klinePort.Image = Properties.Resources.ERROR;
                }
                else if (checkBox_SerialPort4.Checked == true &&
                        (comboBox_KlinePort_PortName_Value.Text == comboBox_SerialPort4_PortName_Value.Text))
                {
                    label_ErrorMessage.Text = "Duplicate SerialPort ";
                    pictureBox_klinePort.Image = Properties.Resources.ERROR;
                }
                else if (checkBox_SerialPort5.Checked == true &&
                        (comboBox_KlinePort_PortName_Value.Text == comboBox_SerialPort5_PortName_Value.Text))
                {
                    label_ErrorMessage.Text = "Duplicate SerialPort ";
                    pictureBox_klinePort.Image = Properties.Resources.ERROR;
                }
                else
                {
                    pictureBox_klinePort.Image = null;
                }
            }
            else if (checkBox_Kline.Checked == false)
            {
                pictureBox_klinePort.Image = null;
            }

            if (pictureBox_SerialPort1.Image == null &&
                pictureBox_SerialPort2.Image == null &&
                pictureBox_SerialPort3.Image == null &&
                pictureBox_SerialPort4.Image == null &&
                pictureBox_SerialPort5.Image == null &&
                pictureBox_klinePort.Image == null)
            {
                label_ErrorMessage.Text = ""; // SerialPort save behavior on FormTabControl.cs file.
            }
        }

        private void textBox_ImagePath_TextChanged(object sender, EventArgs e)
        {
            if (Directory.Exists(textBox_ImagePath.Text.Trim()) == true && textBox_ImagePath.Text.Trim() != @"\")
            {
                ini12.INIWrite(MainSettingPath, "Record", "VideoPath", textBox_ImagePath.Text.Trim());
                pictureBox_ImagePath.Image = null;
            }
            else
            {
                pictureBox_ImagePath.Image = Properties.Resources.ERROR;
            }
        }

        private void textBox_LogPath_TextChanged(object sender, EventArgs e)
        {
            if (Directory.Exists(textBox_LogPath.Text.Trim()) == true && textBox_LogPath.Text.Trim() != @"\")
            {
                ini12.INIWrite(MainSettingPath, "Record", "LogPath", textBox_LogPath.Text.Trim());
                pictureBox_LogPath.Image = null;
            }
            else
            {
                pictureBox_LogPath.Image = Properties.Resources.ERROR;
            }
        }

        private void textBox_GeneratorPath_TextChanged(object sender, EventArgs e)
        {
            if (File.Exists(textBox_GeneratorPath.Text.Trim()) == true && textBox_GeneratorPath.Text.Trim() != @"\")
            {
                ini12.INIWrite(MainSettingPath, "Record", "Generator", textBox_GeneratorPath.Text.Trim());
                pictureBox_GeneratorPath.Image = null;
            }
            else
            {
                pictureBox_GeneratorPath.Image = Properties.Resources.ERROR;
            }
        }

        private void textBox_DosPath_TextChanged(object sender, EventArgs e)
        {
            if (Directory.Exists(textBox_DosPath.Text.Trim()) == true && textBox_DosPath.Text.Trim() != @"\")
            {
                ini12.INIWrite(MainSettingPath, "Device", "DOS", textBox_DosPath.Text.Trim());
                pictureBox_DosPath.Image = null;
            }
            else
            {
                pictureBox_DosPath.Image = Properties.Resources.ERROR;
            }
        }

        private void comboBox_CameraDevice_SelectedIndexChanged(object sender, EventArgs e)
        {
            ini12.INIWrite(MainSettingPath, "Camera", "VideoIndex", comboBox_CameraDevice.SelectedIndex.ToString());
            ini12.INIWrite(MainSettingPath, "Camera", "VideoName", comboBox_CameraDevice.Text);
        }

        private void comboBox_CameraAudio_SelectedIndexChanged(object sender, EventArgs e)
        {
            ini12.INIWrite(MainSettingPath, "Camera", "AudioIndex", comboBox_CameraAudio.SelectedIndex.ToString());
            ini12.INIWrite(MainSettingPath, "Camera", "AudioName", comboBox_CameraAudio.Text);
        }

        private void comboBox_SerialPort1_PortName_Value_SelectedIndexChanged(object sender, EventArgs e)
        {
            PortCheck();
        }

        private void comboBox_SerialPort1_BaudRate_Value_SelectedIndexChanged(object sender, EventArgs e)
        {
            ini12.INIWrite(MainSettingPath, "Port A", "BaudRate", comboBox_SerialPort1_BaudRate_Value.Text.Trim());

            if (comboBox_SerialPort1_BaudRate_Value != null)
            {
                label_ErrorMessage.Text = "";
            }
        }

        private void comboBox_SerialPort2_PortName_Value_SelectedIndexChanged(object sender, EventArgs e)
        {
            PortCheck();
        }

        private void comboBox_SerialPort2_BaudRate_Value_SelectedIndexChanged(object sender, EventArgs e)
        {
            ini12.INIWrite(MainSettingPath, "Port B", "BaudRate", comboBox_SerialPort2_BaudRate_Value.Text.Trim());

            if (comboBox_SerialPort2_BaudRate_Value != null)
            {
                label_ErrorMessage.Text = "";
            }
        }

        private void comboBox_SerialPort3_PortName_Value_SelectedIndexChanged(object sender, EventArgs e)
        {
            PortCheck();
        }

        private void comboBox_SerialPort3_BaudRate_Value_SelectedIndexChanged(object sender, EventArgs e)
        {
            ini12.INIWrite(MainSettingPath, "Port C", "BaudRate", comboBox_SerialPort3_BaudRate_Value.Text.Trim());

            if (comboBox_SerialPort3_BaudRate_Value != null)
            {
                label_ErrorMessage.Text = "";
            }
        }

        private void comboBox_SerialPort4_PortName_Value_SelectedIndexChanged(object sender, EventArgs e)
        {
            PortCheck();
        }

        private void comboBox_SerialPort4_BaudRate_Value_SelectedIndexChanged(object sender, EventArgs e)
        {
            ini12.INIWrite(MainSettingPath, "Port D", "BaudRate", comboBox_SerialPort4_BaudRate_Value.Text.Trim());

            if (comboBox_SerialPort4_BaudRate_Value != null)
            {
                label_ErrorMessage.Text = "";
            }
        }

        private void comboBox_SerialPort5_PortName_Value_SelectedIndexChanged(object sender, EventArgs e)
        {
            PortCheck();
        }

        private void comboBox_SerialPort5_BaudRate_Value_SelectedIndexChanged(object sender, EventArgs e)
        {
            ini12.INIWrite(MainSettingPath, "Port E", "BaudRate", comboBox_SerialPort5_BaudRate_Value.Text.Trim());

            if (comboBox_SerialPort5_BaudRate_Value != null)
            {
                label_ErrorMessage.Text = "";
            }
        }

        private void checkBox_Kline_CheckedChanged(object sender, EventArgs e)
        {
            //自動跑KlineLog//
            if (checkBox_Kline.Checked == true)
            {
                ini12.INIWrite(MainSettingPath, "Kline", "Checked", "1");
                comboBox_KlinePort_PortName_Value.Enabled = true;
                PortCheck();
            }
            else
            {
                ini12.INIWrite(MainSettingPath, "Kline", "Checked", "0");
                comboBox_KlinePort_PortName_Value.Enabled = false;
                PortCheck();
            }
        }

        private void comboBox_Kline_SelectedIndexChanged(object sender, EventArgs e)
        {
            PortCheck();
        }

        private void checkBox_CANLog_CheckedChanged(object sender, EventArgs e)
        {
            //自動跑CANbusLog//
            if (checkBox_CAN_Log.Checked == true)
            {
                comboBox_CAN_DevIndex.Enabled = true;
                comboBox_CAN_BaudRate_Value.Enabled = true;
                ini12.INIWrite(MainSettingPath, "Canbus", "Log", "1");
            }
            else
            {
                comboBox_CAN_DevIndex.Enabled = false;
                comboBox_CAN_BaudRate_Value.Enabled = false;
                ini12.INIWrite(MainSettingPath, "Canbus", "Log", "0");
            }
        }

        private void comboBox_CANDevIndex_SelectedIndexChanged(object sender, EventArgs e)
        {
            ini12.INIWrite(MainSettingPath, "Canbus", "DevIndex", comboBox_CAN_DevIndex.SelectedIndex.ToString());
        }

        private void comboBox_CAN_BaudRate_Value_SelectedIndexChanged(object sender, EventArgs e)
        {
            ini12.INIWrite(MainSettingPath, "Canbus", "BaudRate", comboBox_CAN_BaudRate_Value.Text.Trim());
        }
    }
}