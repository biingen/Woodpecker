using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using jini;
using System.Xml.Linq;
using DirectX.Capture;
using USBClassLibrary;

namespace AutoTest
{
    public partial class Setting : Form
    {
        public Setting()
        {
            InitializeComponent();

/*
            //USB Connection
            USBPort = new USBClass();
            USBDeviceProperties = new USBClass.DeviceProperties();
            USBPort.USBDeviceAttached += new USBClass.USBDeviceEventHandler(USBPort_USBDeviceAttached);
            USBPort.USBDeviceRemoved += new USBClass.USBDeviceEventHandler(USBPort_USBDeviceRemoved);
            USBPort.RegisterForDeviceChange(true, this);
            USBTryRedratConnection();
            USBTryCameraConnection();
            MyUSBRedratDeviceConnected = false;
            MyUSBCameraDeviceConnected = false;
*/
        }
/*
        #region USB Detect
        /// <summary>
        /// Try to connect to the device.
        /// </summary>
        /// <returns>True if success, false otherwise</returns>
        private bool USBTryRedratConnection()
        {
            if (USBClass.GetUSBDevice(uint.Parse("112A", System.Globalization.NumberStyles.AllowHexSpecifier), uint.Parse("0005", System.Globalization.NumberStyles.AllowHexSpecifier), ref USBDeviceProperties,false))
            {
                //My Device is attached
                DeviceRedratTextBox.Text = USBDeviceProperties.DeviceLocation;
                RedratConnect();

                return true;
            }
            else
            {
                RedratDisconnect();
                return false;
            }
        }

        private bool USBTryCameraConnection()
        {
            if (USBClass.GetUSBDevice(uint.Parse("046D", System.Globalization.NumberStyles.AllowHexSpecifier), uint.Parse("082B", System.Globalization.NumberStyles.AllowHexSpecifier), ref USBDeviceProperties, false) || USBClass.GetUSBDevice(uint.Parse("045E", System.Globalization.NumberStyles.AllowHexSpecifier), uint.Parse("0766", System.Globalization.NumberStyles.AllowHexSpecifier), ref USBDeviceProperties, false)  || USBClass.GetUSBDevice(uint.Parse("114D", System.Globalization.NumberStyles.AllowHexSpecifier), uint.Parse("8C00", System.Globalization.NumberStyles.AllowHexSpecifier), ref USBDeviceProperties, false))
            {
                //My Device is attached
                DeviceCameraTextBox.Text = USBDeviceProperties.DeviceLocation;
                CameraConnect();

                return true;
            }
            else
            {
                CameraDisconnect();
                return false;
            }
        }

        private void USBPort_USBDeviceAttached(object sender, USBClass.USBDeviceEventArgs e)
        {
            if (!MyUSBRedratDeviceConnected)
            {
                if (USBTryRedratConnection())
                {
                    MyUSBRedratDeviceConnected = true;
                }
            }

            if (!MyUSBCameraDeviceConnected)
            {
                if (USBTryCameraConnection())
                {
                    MyUSBCameraDeviceConnected = true;
                }
            }
        }

        private void USBPort_USBDeviceRemoved(object sender, USBClass.USBDeviceEventArgs e)
        {
            if (!USBClass.GetUSBDevice(uint.Parse("112A", System.Globalization.NumberStyles.AllowHexSpecifier), uint.Parse("0005", System.Globalization.NumberStyles.AllowHexSpecifier), ref USBDeviceProperties, false))
            {
                //My Device is removed
                MyUSBRedratDeviceConnected = false;
                RedratDisconnect();
            }
            
            if (!USBClass.GetUSBDevice(uint.Parse("046D", System.Globalization.NumberStyles.AllowHexSpecifier), uint.Parse("082B", System.Globalization.NumberStyles.AllowHexSpecifier), ref USBDeviceProperties, false) || !USBClass.GetUSBDevice(uint.Parse("045E", System.Globalization.NumberStyles.AllowHexSpecifier), uint.Parse("0766", System.Globalization.NumberStyles.AllowHexSpecifier), ref USBDeviceProperties, false) || !USBClass.GetUSBDevice(uint.Parse("114D", System.Globalization.NumberStyles.AllowHexSpecifier), uint.Parse("8C00", System.Globalization.NumberStyles.AllowHexSpecifier), ref USBDeviceProperties, false))
            {
                //My Device is removed
                MyUSBCameraDeviceConnected = false;
                CameraDisconnect();
            }
        }

        protected override void WndProc(ref Message m)
        {
            USBPort.ProcessWindowsMessage(ref m);

            base.WndProc(ref m);
        }

        private void RedratConnect()
        {
            //TO DO: Inset your connection code here
            comboBox3.Enabled = true;
            comboBox5.Enabled = true;
            label16.SelectedIndex = 1;
            label16.Text = "Enable";
            loadxml();
            saveredrat();
        }

        private void RedratDisconnect()
        {
            //TO DO: Insert your disconnection code here
            comboBox3.Enabled = false;
            comboBox5.Enabled = false;
            label16.SelectedIndex = 0;
            label16.Text = "Disable";
            DeviceRedratTextBox.Text = String.Empty;
            saveredrat();
        }
        private void CameraConnect()
        {
            comboBox4.Enabled = true;
            comboBox6.Enabled = true;
            label17.SelectedIndex = 1;
            label17.Text = "Enable";
            
            //TO DO: Inset your connection code here
            loadcamera();
            initcamera();
            savecamera();
        }

        private void CameraDisconnect()
        {
            //TO DO: Insert your disconnection code here
            comboBox4.Enabled = false;
            comboBox6.Enabled = false;
            label17.SelectedIndex = 0;
            label17.Text = "Disable";
            DeviceCameraTextBox.Text = String.Empty;
            savecamera();
        }

        private void UpdateButton_Click(object sender, EventArgs e)
        {
            USBTryRedratConnection();
            USBTryCameraConnection();
        }

        private void saveredrat()
        {
            // 設定INI檔路徑
            String sPath;
            sPath = Application.StartupPath + "\\Config.ini";

            // 寫入Redrat啟動或關閉
            ini12.INIWrite(sPath, "RedRatDev", "value", label16.SelectedIndex.ToString());
        }

        private void loadcamera()
        {
            // 設定INI檔路徑
            String sPath;
            sPath = Application.StartupPath + "\\Config.ini";

            // 讀取ini中的路徑
            comboBox4.Text = ini12.INIRead(sPath, "camera", "value", "");
            comboBox6.Text = ini12.INIRead(sPath, "audio", "value", "");
        }

        private void savecamera()
        {
            // 設定INI檔路徑
            String sPath;
            sPath = Application.StartupPath + "\\Config.ini";

            // 寫入Camera啟動或關閉
            ini12.INIWrite(sPath, "CameraDev", "value", label17.SelectedIndex.ToString());
        }

        private void initcamera()
        {
            Filters filters = new Filters();
            Filter f;
            
            // 取得攝影機
            comboBox4.Items.Clear();
            for (int c = 0; c < filters.VideoInputDevices.Count; c++)
            {
                f = filters.VideoInputDevices[c];
                comboBox4.Items.Add(f.Name);

                // if (comboBox4.Text == "")

                comboBox4.Text = f.Name;
            }

            comboBox6.Items.Clear();
            for (int j = 0; j < filters.AudioInputDevices.Count; j++)
            {
                f = filters.AudioInputDevices[j];
                comboBox6.Items.Add(f.Name);

                // if (comboBox6.Text == "")

                comboBox6.Text = f.Name;
            }
        }

        private void closecamera()
        {
            Filters filters = null;
        }
        #endregion
*/
        private void loadxml()
        {
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

/*
        private void Tpvloadxml()
        { //戴入TPV的xml

            if (System.IO.File.Exists(textBox7.Text))
            {
                XDocument myDoc = XDocument.Load(textBox7.Text);
                var AVDevices = from pn in myDoc.Descendants("Avdevices")
                                where pn.Element("DeviceName") != null
                                select pn.Element("DeviceName").Value;


                foreach (var c in AVDevices)
                {


                    //textBox1.Text = textBox1.Text + c + "\t\n";
                    comboBox10.Items.Add(c);
                    if (comboBox10.Text == "")
                        comboBox10.Text = c;
                }

            }
           

        }
*/
        private void button2_Click(object sender, EventArgs e)
        {
            //Save Video Path 
            // 開啟檔案
            folderBrowserDialog1.ShowDialog();
            //openFileDialog1.Filter = "CSV files (*.csv)|*.CSV";
            textBox1.Text = folderBrowserDialog1.SelectedPath;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            // Save Scheduler Path
            // 開啟檔案

            openFileDialog1.Filter = "CSV files (*.csv)|*.CSV";
            openFileDialog1.ShowDialog();
            if (openFileDialog1.FileName == "openFileDialog1")
            {
                textBox2.Text = "";
            }
            else 
            {
            textBox2.Text = openFileDialog1.FileName;
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            // RedRat3 Command Path
            // 開啟檔案

            openFileDialog1.Filter = "XML files (*.xml)|*.xml";
            openFileDialog1.ShowDialog();
            if (openFileDialog1.FileName == "openFileDialog1")
            {
                textBox3.Text = "";
            }
            else
            {
                textBox3.Text = openFileDialog1.FileName;
            }
            loadxml();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            // log file save path
            folderBrowserDialog1.ShowDialog();
            //openFileDialog1.Filter = "CSV files (*.csv)|*.CSV";
            textBox5.Text = folderBrowserDialog1.SelectedPath;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //完成設定
            // 設定INI檔路徑
            String sPath;
            bool open = true;
            sPath = Application.StartupPath + "\\Config.ini";

            // 寫入ini檔
            ini12.INIWrite(sPath, "Comport", "BaudRate", baudratebox.Text.Trim());
            ini12.INIWrite(sPath, "Comport", "DataBit", ComboBox1.Text);
            ini12.INIWrite(sPath, "Comport", "StopBits", ComboBox2.Text);
            ini12.INIWrite(sPath, "Comport", "PortName", cmbCOM.Text);

            // 寫入URC設定檔到ini
/*
            ini12.INIWrite(sPath, "URC", "BaudRate", textBox6.Text.Trim());
            ini12.INIWrite(sPath, "URC", "DataBit", comboBox9.Text);
            ini12.INIWrite(sPath, "URC", "StopBits", comboBox8.Text);
            ini12.INIWrite(sPath, "URC", "PortName", comboBox7.Text);
*/
            //先判斷檔案是否存在
            if (System.IO.File.Exists(textBox3.Text.Trim()))
            {
                // 寫入ini檔 RedRat3 Command Path
                ini12.INIWrite(sPath, "RedRatCmd", "Path", textBox3.Text.Trim());
            }
            else
            {
                MessageBox.Show("RedRat File error!", "XML File Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                open = false;
            }

            //先判斷檔案是否存在
            if (System.IO.File.Exists(textBox2.Text.Trim()))
            {
                // 寫入ini檔 Save Scheduler Path
                ini12.INIWrite(sPath, "Scheduler", "Path", textBox2.Text.Trim());
            }
            else
            {
                MessageBox.Show("Scheduler File error!", "CSV File Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                open = false;
            }

            //先判斷目錄是否存在
            if (!System.IO.Directory.Exists(textBox1.Text.Trim()))
            {
                MessageBox.Show("Save Video Path does not exist", "Directory Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                open = false;
            }
            else
            {
                //寫入ini檔
                ini12.INIWrite(sPath, "Video", "Path", textBox1.Text.Trim());
            }

            ini12.INIWrite(sPath, "Loop", "value", maskedTextBox1.Text.Trim());

            // 寫入ini檔 設備名稱
            ini12.INIWrite(sPath, "RedCon", "value", comboBox3.Text.Trim());

            // 寫入ini檔 TPV設備名稱
/*
            ini12.INIWrite(sPath, "TPC", "value", comboBox10.Text.Trim());
*/
            //寫入 攝影機
            ini12.INIWrite(sPath, "camera", "value", comboBox4.SelectedIndex.ToString());
            ini12.INIWrite(sPath, "RedRat", "value", comboBox5.Text.Trim());
            // 寫入audio
            ini12.INIWrite(sPath, "audio", "value", comboBox6.SelectedIndex.ToString());
/*
            // 寫入Redrat啟動或關閉
            ini12.INIWrite(sPath, "RedRatDev", "value", comboBox7.SelectedIndex.ToString());

            // 寫入Camera啟動或關閉
            ini12.INIWrite(sPath, "CameraDev", "value", comboBox8.SelectedIndex.ToString());

            // IR Device Selection

            if (radioButton1.Checked == true)
            {
                ini12.INIWrite(sPath, "IR Device", "value", "1"); //RedRat
            }
            else
            {
                ini12.INIWrite(sPath, "IR Device", "value", "2"); // TPV URC
            }
*/
            //先判斷目錄是否存在
            if (!System.IO.Directory.Exists(textBox5.Text.Trim()))
            {
                MessageBox.Show("Save Video Path does not exist", "Path Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                open = false;
            }
            else
            {
                ini12.INIWrite(sPath, "Log", "Path", textBox5.Text.Trim());
            }

            //先判斷檔案是否存在
/*
            if (System.IO.File.Exists(textBox7.Text.Trim()))
            {
                // 寫入ini檔 Save Scheduler Path
                ini12.INIWrite(sPath, "TPC", "Path", textBox7.Text.Trim());
            }
            else
            {
                MessageBox.Show("Scheduler File error!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                open = false;
            }
*/
            //讀取Scheduler檔

            if (open == true)
            {
                MessageBox.Show("System configuration saved, please press Reload button to use new setting.", "Setting Finish Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.Hide();
            }
        }

        private void Setting_Load(object sender, EventArgs e)
        {
            //表單戴入時
            // 設定INI檔路徑
            String sPath;
            sPath = Application.StartupPath + "\\Config.ini";

            // 讀取ini中的路徑
            textBox1.Text = ini12.INIRead(sPath, "Video", "Path", "");
            textBox2.Text = ini12.INIRead(sPath, "Scheduler", "Path", "");
            textBox3.Text = ini12.INIRead(sPath, "RedRatCmd", "Path", "");
            baudratebox.Text = ini12.INIRead(sPath, "Comport", "BaudRate", "");
            ComboBox1.Text = ini12.INIRead(sPath, "Comport", "DataBit", "");
            ComboBox2.Text = ini12.INIRead(sPath, "Comport", "StopBits", "");
            cmbCOM.Text = ini12.INIRead(sPath, "Comport", "PortName", "");
            maskedTextBox1.Text = ini12.INIRead(sPath, "Loop", "value", "");
            comboBox3.Text = ini12.INIRead(sPath, "RedCon", "value", "");
            comboBox4.Text = ini12.INIRead(sPath, "camera", "value", "");
            comboBox5.Text = ini12.INIRead(sPath, "RedRat", "value", "");
            comboBox6.Text = ini12.INIRead(sPath, "audio", "value", "");
            textBox5.Text = ini12.INIRead(sPath, "Log", "Path", "");
            String Text;
            Text = ini12.INIRead(sPath, "RedRatDev", "value", "");
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
            Text = ini12.INIRead(sPath, "CameraDev", "value", "");
            if (Text == "0")
            {
                label17.Text = "Disable";
                comboBox4.Enabled = false;
                comboBox6.Enabled = false;
            }
            else
            {
                label17.Text = "Enable";
                comboBox4.Enabled = true;
                comboBox6.Enabled = true;
            }

/*
            comboBox10.Text = ini12.INIRead(sPath, "TPC", "value", "");

            textBox6.Text = ini12.INIRead(sPath, "URC", "BaudRate", "");
            comboBox9.Text = ini12.INIRead(sPath, "URC", "DataBit", "");
            comboBox8.Text = ini12.INIRead(sPath, "URC", "StopBits", "");
            comboBox7.Text = ini12.INIRead(sPath, "URC", "PortName", "");
            textBox7.Text = ini12.INIRead(sPath, "TPC", "Path", "");

            if (ini12.INIRead(sPath, "IR Device", "value", "") == "1")
            {
                radioButton1.Checked = true;
                radioButton2.Checked = false;
            }
            else
            {
                radioButton1.Checked = false;
                radioButton2.Checked = true;
            }
*/
            //戴入xml的

            loadxml();
/*
            Tpvloadxml();
*/

            if (ini12.INIRead(Application.StartupPath + "\\Config.ini", "CameraDev", "value", "") == "1")
            {
		    Filters filters = new Filters();
                    Filter f;

                   // 取得攝影機
                   for (int c = 0; c < filters.VideoInputDevices.Count; c++)
                   {
                       f = filters.VideoInputDevices[c];
                       comboBox4.Items.Add(f.Name);
                       // if (comboBox4.Text == "")
                           comboBox4.Text = f.Name;
                   }

                   for (int j = 0; j < filters.AudioInputDevices.Count; j++)
                   {
                       f = filters.AudioInputDevices[j];
                       comboBox6.Items.Add(f.Name);
                       // if (comboBox6.Text == "")
                           comboBox6.Text = f.Name;
                   }
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            MessageBox.Show("System configuration canceled, please press Reload button to use old setting.", "Cancel Setting Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
            this.Close();
        }

/*
        private void button7_Click(object sender, EventArgs e)
        {
            //TPV URC Cammand file path
            openFileDialog1.Filter = "XML files (*.xml)|*.xml";
            openFileDialog1.ShowDialog();

            textBox7.Text = openFileDialog1.FileName;
            Tpvloadxml();

        }
*/
    }
}
