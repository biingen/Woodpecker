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
            textBox2.Text = openFileDialog1.FileName;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            // RedRat3 Command Path
            // 開啟檔案

            openFileDialog1.Filter = "XML files (*.xml)|*.xml";
            openFileDialog1.ShowDialog();
            textBox3.Text = openFileDialog1.FileName;

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
            ini12.INIWrite(sPath, "Comport", "BaudRate", textBox4.Text.Trim());
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
                MessageBox.Show("RedRat File error!");
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
                MessageBox.Show("Scheduler File error!");
                open = false;
            }

            //先判斷目錄是否存在
            if (!System.IO.Directory.Exists(textBox1.Text.Trim()))
            {
                MessageBox.Show("Save Video Path does not exist");
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

            // IR Device Selection
/*
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
                MessageBox.Show("Save Video Path does not exist");
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
                MessageBox.Show("Scheduler File error!");
                open = false;
            }
*/
            //讀取Scheduler檔

            if (open == true)
            {
                MessageBox.Show("System configuration changed, please restart Auto Test to use new setting.");
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
            textBox4.Text = ini12.INIRead(sPath, "Comport", "BaudRate", "");
            ComboBox1.Text = ini12.INIRead(sPath, "Comport", "DataBit", "");
            ComboBox2.Text = ini12.INIRead(sPath, "Comport", "StopBits", "");
            cmbCOM.Text = ini12.INIRead(sPath, "Comport", "PortName", "");
            maskedTextBox1.Text = ini12.INIRead(sPath, "Loop", "value", "");
            comboBox3.Text = ini12.INIRead(sPath, "RedCon", "value", "");
            comboBox4.Text = ini12.INIRead(sPath, "camera", "value", "");
            comboBox5.Text = ini12.INIRead(sPath, "RedRat", "value", "");
            textBox5.Text = ini12.INIRead(sPath, "Log", "Path", "");
            comboBox6.Text = ini12.INIRead(sPath, "audio", "value", "");
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

           
		    Filters filters = new Filters();
            Filter f;

            // 取得攝影機
            for (int c = 0; c < filters.VideoInputDevices.Count; c++)
            {
                f = filters.VideoInputDevices[c];
                comboBox4.Items.Add(f.Name);
/*
                if (comboBox4.Text == "")
*/
                    comboBox4.Text = f.Name;
            }

            for (int j = 0; j < filters.AudioInputDevices.Count; j++)
            {
                f = filters.AudioInputDevices[j];
                comboBox6.Items.Add(f.Name);
/*
                if (comboBox6.Text == "")
*/
                    comboBox6.Text = f.Name;
            }

        }

        private void button6_Click(object sender, EventArgs e)
        {
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
