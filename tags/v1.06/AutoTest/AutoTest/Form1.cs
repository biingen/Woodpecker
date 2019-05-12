using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging; 
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DirectX.Capture;
using System.Collections;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.Devices;
using System.Xml;
using System.Xml.Serialization;
using System.Threading;
using jini;
using System.Xml.Linq;
using RedRat.IR;
using RedRat.USB;
using RedRat.Util;
using RedRat.RedRat3;
using RedRat.RedRat3.USB;
using RedRat.AVDeviceMngmt;
using System.IO;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;
using System.Reflection;
using System.IO.Ports;
using CheckDisk;
using USBClassLibrary;

namespace AutoTest
{
    public partial class Form1 : Form
    {
        private Capture capture = null;
        private Filters filters = null;
        IRedRat3 redRat3 = null;
        private bool _captureInProgress;
        private bool jbutton1 = false;
        private bool pbutton1 = false;
        bool Vread = false;
        string videostring = "";
        string srtstring = "";

        public Form1()
        {
            InitializeComponent();

            //USB Connection
            USBPort = new USBClass();
            USBDeviceProperties = new USBClass.DeviceProperties();
            USBPort.USBDeviceAttached += new USBClass.USBDeviceEventHandler(USBPort_USBDeviceAttached);
            USBPort.USBDeviceRemoved += new USBClass.USBDeviceEventHandler(USBPort_USBDeviceRemoved);
            USBPort.RegisterForDeviceChange(true, this);
            USBTryHubConnection();
            USBTryRedratConnection();
            USBTryCameraConnection();
            MyUSBHubDeviceConnected = false;
            MyUSBRedratDeviceConnected = false;
            MyUSBCameraDeviceConnected = false;

            if (ini12.INIRead(Application.StartupPath + "\\Config.ini", "RedRatDev", "value", "") == "1")
            {
                OpenRedRat3(); //小紅鼠
            }
            else
            {
                pictureBox1.Image = Properties.Resources._02;
            }

            if (ini12.INIRead(Application.StartupPath + "\\Config.ini", "CameraDev", "value", "") == "1")
            {
                pictureBox2.Image = Properties.Resources._01;
                filters = new Filters();
            }
            else
            {
                pictureBox2.Image = Properties.Resources._02;
            }

            pictureBox3.Image = Properties.Resources._02;
        }

        #region USB Detect
        /// <summary>
        /// Try to connect to the device.
        /// </summary>
        /// <returns>True if success, false otherwise</returns>

        private bool USBTryHubConnection()
        {
            if (USBClass.GetUSBDevice(uint.Parse("05E3", System.Globalization.NumberStyles.AllowHexSpecifier), uint.Parse("0610", System.Globalization.NumberStyles.AllowHexSpecifier), ref USBDeviceProperties, false))
            {
                //My Device is attached
                UsbConnect();
                return true;
            }
            else
            {
                UsbDisconnect();
                return false;
            }
        }

        private bool USBTryRedratConnection()
        {
            if (USBClass.GetUSBDevice(uint.Parse("112A", System.Globalization.NumberStyles.AllowHexSpecifier), uint.Parse("0005", System.Globalization.NumberStyles.AllowHexSpecifier), ref USBDeviceProperties, false))
            {
                //My Device is attached
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
            if (USBClass.GetUSBDevice(uint.Parse("046D", System.Globalization.NumberStyles.AllowHexSpecifier), uint.Parse("082B", System.Globalization.NumberStyles.AllowHexSpecifier), ref USBDeviceProperties, false) || USBClass.GetUSBDevice(uint.Parse("045E", System.Globalization.NumberStyles.AllowHexSpecifier), uint.Parse("0766", System.Globalization.NumberStyles.AllowHexSpecifier), ref USBDeviceProperties, false) || USBClass.GetUSBDevice(uint.Parse("114D", System.Globalization.NumberStyles.AllowHexSpecifier), uint.Parse("8C00", System.Globalization.NumberStyles.AllowHexSpecifier), ref USBDeviceProperties, false))
            {
                //My Device is attached
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
            if (!MyUSBHubDeviceConnected)
            {
                if (USBTryHubConnection())
                {
                    MyUSBHubDeviceConnected = true;
                }
            }

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
            if (!USBClass.GetUSBDevice(uint.Parse("05E3", System.Globalization.NumberStyles.AllowHexSpecifier), uint.Parse("0610", System.Globalization.NumberStyles.AllowHexSpecifier), ref USBDeviceProperties, false))
            {
                //My Device is removed
                MyUSBHubDeviceConnected = false;
                USBTryHubConnection();
            }

            if (!USBClass.GetUSBDevice(uint.Parse("112A", System.Globalization.NumberStyles.AllowHexSpecifier), uint.Parse("0005", System.Globalization.NumberStyles.AllowHexSpecifier), ref USBDeviceProperties, false))
            {
                //My Device is removed
                MyUSBRedratDeviceConnected = false;
                USBTryRedratConnection();
            }

            if (!USBClass.GetUSBDevice(uint.Parse("046D", System.Globalization.NumberStyles.AllowHexSpecifier), uint.Parse("082B", System.Globalization.NumberStyles.AllowHexSpecifier), ref USBDeviceProperties, false) || !USBClass.GetUSBDevice(uint.Parse("045E", System.Globalization.NumberStyles.AllowHexSpecifier), uint.Parse("0766", System.Globalization.NumberStyles.AllowHexSpecifier), ref USBDeviceProperties, false) || !USBClass.GetUSBDevice(uint.Parse("114D", System.Globalization.NumberStyles.AllowHexSpecifier), uint.Parse("8C00", System.Globalization.NumberStyles.AllowHexSpecifier), ref USBDeviceProperties, false))
            {
                //My Device is removed
                MyUSBCameraDeviceConnected = false;
                USBTryCameraConnection();
            }
        }

        protected override void WndProc(ref Message m)
        {
            USBPort.ProcessWindowsMessage(ref m);
            base.WndProc(ref m);
        }

        private void UsbConnect()
        {
            //TO DO: Inset your connection code here
            // 設定INI檔路徑
            String sPath;
            sPath = Application.StartupPath + "\\Config.ini";

            // 寫入Autobox關閉
            ini12.INIWrite(sPath, "AutoboxDev", "value", "1");
        }

        private void UsbDisconnect()
        {
            //TO DO: Insert your disconnection code here
            // 設定INI檔路徑
            String sPath;
            sPath = Application.StartupPath + "\\Config.ini";

            // 寫入Autobox關閉
            ini12.INIWrite(sPath, "AutoboxDev", "value", "0");
        }

        private void RedratConnect()
        {
            //TO DO: Inset your connection code here
            // 設定Redrat燈號
            pictureBox1.Image = Properties.Resources._01;
            // 設定INI檔路徑
            String sPath;
            sPath = Application.StartupPath + "\\Config.ini";

            // 寫入Redrat啟動
            ini12.INIWrite(sPath, "RedRatDev", "value", "1");
            // 設定Redrat燈號
            pictureBox1.Image = Properties.Resources._01;
        }

        private void RedratDisconnect()
        {
            //TO DO: Insert your disconnection code here
            // 設定Redrat燈號
            pictureBox1.Image = Properties.Resources._02;
            // 設定INI檔路徑
            String sPath;
            sPath = Application.StartupPath + "\\Config.ini";

            // 寫入Redrat關閉
            ini12.INIWrite(sPath, "RedRatDev", "value", "0");
            // 設定Redrat燈號
            pictureBox1.Image = Properties.Resources._02;
        }

        private void CameraConnect()
        {
            //TO DO: Inset your connection code here
            // 設定Camera燈號
            pictureBox2.Image = Properties.Resources._01;
            // 設定INI檔路徑
            String sPath;
            sPath = Application.StartupPath + "\\Config.ini";

            // 寫入Camera啟動
            ini12.INIWrite(sPath, "CameraDev", "value", "1");
            // 設定Camera燈號
            pictureBox2.Image = Properties.Resources._01;
        }

        private void CameraDisconnect()
        {
            //TO DO: Insert your disconnection code here
            // 設定Camera燈號
            pictureBox2.Image = Properties.Resources._02;
            // 設定INI檔路徑
            String sPath;
            sPath = Application.StartupPath + "\\Config.ini";

            // 寫入Camera關閉
            ini12.INIWrite(sPath, "CameraDev", "value", "0");
            // 設定Camera燈號
            pictureBox2.Image = Properties.Resources._02;
        }

        #endregion

        #region API DLL

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern bool DeviceIoControl(SafeFileHandle hDevice,
                                                                                uint dwIoControlCode,
                                                                                ref uint InBuffer,
                                                                                int nInBufferSize,
                                                                                byte[] OutBuffer,
                                                                                UInt32 nOutBufferSize,
                                                                                ref UInt32 out_count,
                                                                                IntPtr lpOverlapped);

        protected SafeFileHandle hCOM;
        //bool bOpend = false;
        bool bEnable = false;

        public const int FILE_ATTRIBUTE_NORMAL = 0x00000080;
        public const UInt64 GENERIC_READ = 0x80000000;
        public const uint OPEN_EXISTING = 3;
        public const UInt32 INVALID_HANDLE_VALUE = 0xffffffff;
        public const UInt64 GENERIC_WRITE = 0x40000000L;
        public const uint FILE_FLAG_OVERLAPPED = 0x40000000;
        private const uint FILE_DEVICE_UNKNOWN = 0x00000022;
        private const uint FILE_ATTRIBUTE_SYSTEM = 0x00000004;
        private const uint USB2SER_IOCTL_INDEX = 0x0800;
        private const uint METHOD_BUFFERED = 0;
        private const uint FILE_ANY_ACCESS = 0;
        private static uint GP0_SET_VALUE = CTL_CODE(FILE_DEVICE_UNKNOWN, USB2SER_IOCTL_INDEX + 22, METHOD_BUFFERED, FILE_ANY_ACCESS);
        private static uint GP0_OUTPUT_ENABLE = CTL_CODE(FILE_DEVICE_UNKNOWN, USB2SER_IOCTL_INDEX + 20, METHOD_BUFFERED, FILE_ANY_ACCESS);
        private static uint GP1_GET_VALUE = CTL_CODE(FILE_DEVICE_UNKNOWN, USB2SER_IOCTL_INDEX + 25, METHOD_BUFFERED, FILE_ANY_ACCESS);

        static uint CTL_CODE(uint DeviceType, uint Function, uint Method, uint Access)
        {
            return ((DeviceType << 16) | (Access << 14) | (Function << 2) | Method);
        }

        public bool Enable
        {
            get { return bEnable; }
            set { bEnable = value; }
        }

        private bool PL2303_GP0_SetValue(SafeFileHandle hDrv, uint val)
        {
            UInt32 nBytes = 0;
            byte[] addr = new byte[6];
            bool bSuccess = DeviceIoControl(hDrv, GP0_SET_VALUE, ref val, sizeof(uint), null, 0, ref nBytes, IntPtr.Zero);
            return bSuccess;
        }

        private bool PL2303_GP0_Enalbe(SafeFileHandle hDrv, uint enable)
        {
            UInt32 nBytes = 0;
            bool bSuccess = DeviceIoControl(hDrv, GP0_OUTPUT_ENABLE,
            ref enable, sizeof(byte), null, 0, ref nBytes, IntPtr.Zero);
            return bSuccess;
        }

        // gpio1
        private bool PL2303_GP1_GetValue(SafeFileHandle hDrv, Byte[] val)
        {
            UInt32 nBytes = 0;
            uint j = 0;
            bool bSuccess = DeviceIoControl(hDrv, GP1_GET_VALUE,
            ref j, 0, val, sizeof(byte), ref nBytes, IntPtr.Zero);
            return bSuccess;
        }

        #endregion

        #region JFunction

        private void OnCaptureComplete(object sender, EventArgs e)
        {
            // Demonstrate the Capture.CaptureComplete event.
            Debug.WriteLine("Capture complete.");
        }

        //執行緒控制label.text
        private delegate void UpdateUICallBack(string value, Control ctl);
        private void UpdateUI(string value, Control ctl)
        {
            if (this.InvokeRequired)
            {
                UpdateUICallBack uu = new UpdateUICallBack(UpdateUI);
                this.Invoke(uu, value, ctl);
            }
            else
            {
                ctl.Text = value;
            }
        }

        //執行緒控制 datagriveiew
        private delegate void UpdateUICallBack1(string value, DataGridView ctl);
        private void gridUI(string i, DataGridView gv)
        {
            if (this.InvokeRequired)
            {
                UpdateUICallBack1 uu = new UpdateUICallBack1(gridUI);
                this.Invoke(uu, i, gv);
            }
            else
            {
                DataGridView1.ClearSelection();
                gv.Rows[int.Parse(i)].Selected = true;
            }
        }

        // 執行緒控制 datagriverew的scorllingbar
        private delegate void UpdateUICallBack3(string value, DataGridView ctl);
        private void gridscroll(string i, DataGridView gv)
        {
            if (this.InvokeRequired)
            {
                UpdateUICallBack3 uu = new UpdateUICallBack3(gridscroll);
                this.Invoke(uu, i, gv);
            }
            else
            {
                //DataGridView1.ClearSelection();
                //gv.Rows[int.Parse(i)].Selected = true;
                gv.FirstDisplayedScrollingRowIndex = int.Parse(i);
            }
        }

        //執行緒控制 textbox1
        private delegate void UpdateUICallBack2(string value, Control ctl);
        private void txtbox1(string value, Control ctl)
        {
            if (this.InvokeRequired)
            {
                UpdateUICallBack2 uu = new UpdateUICallBack2(txtbox1);
                this.Invoke(uu, value, ctl);
            }
            else
            {
                ctl.Text = value + "\t\n";
            }
        }

        // 快照======>
        private void jes()
        {
            this.Invoke(new EventHandler(delegate
            {
                myshot();
            }));
        }
        private void myshot()
        {
            capture.FrameEvent2 += new Capture.HeFrame(CaptureDone);
            capture.GrapImg();
        }
        private void CaptureDone(System.Drawing.Bitmap e)
        {
            capture.FrameEvent2 -= new Capture.HeFrame(CaptureDone);
            string fName = "";
            // 設定INI檔路徑
            String sPath;
            sPath = Application.StartupPath + "\\Config.ini";
            // 讀取ini中的路徑
            fName = ini12.INIRead(sPath, "Video", "Path", "");

            //圖片印字
            this.pictureBox4.Image = e;
            Graphics bitMap_g = Graphics.FromImage(this.pictureBox4.Image);//底圖
            Brush tb = new SolidBrush(Color.Red);
            Font textfont = new Font("微軟正黑體", 20, FontStyle.Bold);
            
            bitMap_g.DrawString(RedratLable.Text, textfont, tb, new PointF(5, 400));//redrat
            bitMap_g.DrawString(TimeLable.Text, textfont, tb, new PointF(5, 440));//command
            
            textfont.Dispose();
            tb.Dispose();
            bitMap_g.Dispose();

            string t = fName + "\\" + "pic-" + DateTime.Now.ToString("yyyyMMddHHmmss") + "_" + label1.Text + ".jpg";
            pictureBox4.Image.Save(t);
        }
        // 快照<========

        // SRC指令副程式 ===>
        private void myrsccmd(string sysCmd)
        {
            this.Invoke(new EventHandler(delegate
            {
                rsccmd(sysCmd);
            }));
        }
        // src 指令
        private void rsccmd(string sysCmd)
        {
            // 指令
            string str = null;
            string dev;
            byte[] data = new byte[6];
            bool isok = false;

            // 設定INI檔路徑
            String sPath;
            sPath = Application.StartupPath + "\\Config.ini";

            string xmlfile = ini12.INIRead(sPath, "TPC", "Path", ""); // 戴入xml檔
            dev = ini12.INIRead(sPath, "TPC", "value", ""); //戴入設備檔

            XDocument myDoc = XDocument.Load(xmlfile);

            var sonySingnals = myDoc.Descendants("Avdevices")
                           .Where(p => p.Element("DeviceName").Value == dev)
                           .Elements("Singnals");
            foreach (var item in sonySingnals)
            {
                var rckey = item.Element("Rckey").Value;
                var tykey = item.Element("Type").Value;
                var mokey = item.Element("Mode").Value;
                var sykey = item.Element("SysCode").Value;
                var cmdkey = item.Element("Cmd").Value;

                if (isok == false)
                {
                    if (rckey.Equals(sysCmd) == true)
                    {
                        str = "58,06," + tykey + ',' + mokey + ',' + sykey + ',' + cmdkey;
                        string[] str2 = str.Split(',');
                        byte[] data2 = new byte[6];
                        for (int i = 0; i < str2.Length; i++)
                        {
                            data2[i] = byte.Parse(str2[i], System.Globalization.NumberStyles.HexNumber);
                        }
                        isok = true;
                        data = data2;
                    }
                    else
                    {
                        data = null;
                    }
                }
            }

            if (data != null)
            {
                try
                {
                    this.serialPort2.Write(data, 0, 6);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
            }
        }
        //Src搶令副程式 <====

        protected void autocommand(string SigData)
        {
            // 執行動作
            AVDeviceDB newAVDeviceDB;

            // 戴入scheduler CSV 檔
            string fName = "";
            string redcon = "";
            // 設定INI檔路徑
            String sPath;
            sPath = Application.StartupPath + "\\Config.ini";

            // 讀取ini中的路徑
            fName = ini12.INIRead(sPath, "RedRatCmd", "Path", "");
            // 讀取設備
            redcon = ini12.INIRead(sPath, "RedCon", "value", "");
            // string fName = "C:\\DeviceDB.xml";
            XmlSerializer ser = new XmlSerializer(typeof(AVDeviceDB));
            FileInfo fileInfo = new FileInfo(fName);
            FileStream fs = null;
            fs = new FileStream(fileInfo.FullName, FileMode.Open);
            newAVDeviceDB = (AVDeviceDB)ser.Deserialize(fs);

            ModulatedSignal sig = (ModulatedSignal)newAVDeviceDB.GetIRPacket(redcon, SigData);

            if (redRat3 != null)
            {
                try
                {
                    redRat3.OutputModulatedSignal((ModulatedSignal)sig);
                    //MessageBox.Show("d");
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
            }
            fs.Close();
        }

        protected void OpenRedRat3()
        {
            int dev = 0;
            // 設定INI檔路徑
            String sPath;
            string intdev = "";
            sPath = Application.StartupPath + "\\Config.ini";

            // 讀取ini中的路徑
            //if(ini12.INIRead(sPath, "RedRat", "value", "")!="")
            intdev = ini12.INIRead(sPath, "RedRat", "value", "");
            if (intdev != "")
            {
                dev = int.Parse(intdev);
            }

            string[] devices = RedRat3USBImpl.FindRedRat3s();
            if (devices.Length > 0)
            {
                //RedRat已連線
                //MessageBox.Show(devices[0]);
                redRat3 = RedRat3USBImpl.GetInstance(devices[dev]);
                //pictureBox1綠燈
                pictureBox1.Image = Properties.Resources._01;
            }
            else
            {
                //RedRat未連線
                /*
                                MessageBox.Show("No RedRat3 devices found.", "TestRemote Warning",
                                  MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                */
                pictureBox1.Image = Properties.Resources._02;
            }
        }

        protected void iniRs232()
        { //設定com port
            string stopbit = "";
            // 設定INI檔路徑
            String sPath;
            sPath = Application.StartupPath + "\\Config.ini";

            // 讀取ini中的路徑
            stopbit = ini12.INIRead(sPath, "Comport", "StopBits", "");
            switch (stopbit)
            {
                case "One":
                    serialPort1.StopBits = System.IO.Ports.StopBits.One;
                    break;
                case "Two":
                    serialPort1.StopBits = System.IO.Ports.StopBits.Two;
                    break;
            }
            serialPort1.PortName = ini12.INIRead(sPath, "Comport", "PortName", "");
            serialPort1.BaudRate = int.Parse(ini12.INIRead(sPath, "Comport", "BaudRate", ""));
            // serialPort1.Encoding = System.Text.Encoding.GetEncoding(1252);

            serialPort1.Open();
            object stream = typeof(SerialPort).GetField("internalSerialStream", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(serialPort1);
            hCOM = (SafeFileHandle)stream.GetType().GetField("_handle", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(stream);
        }
        protected void close232()
        {// 關閉232
            serialPort1.Close();
        }

        //啟動SRC的Com port連緒
        protected void rsc_Rs232()
        {
            string stopbit = "";
            // 設定INI檔路徑
            String sPath;
            sPath = Application.StartupPath + "\\Config.ini";
            // 讀取ini中的路徑
            stopbit = ini12.INIRead(sPath, "URC", "StopBits", "");
            switch (stopbit)
            {
                case "One":
                    serialPort2.StopBits = System.IO.Ports.StopBits.One;
                    break;
                case "Two":
                    serialPort2.StopBits = System.IO.Ports.StopBits.Two;
                    break;
            }

            serialPort2.PortName = ini12.INIRead(sPath, "URC", "PortName", "");
            serialPort2.BaudRate = int.Parse(ini12.INIRead(sPath, "URC", "BaudRate", ""));
            // serialPort1.Encoding = System.Text.Encoding.GetEncoding(1252);

            serialPort2.Open();

        }

        protected void closesrc()
        {// 關閉SRC的232
            serialPort2.Close();
        }

        private void Log(string msg)
        {
            textBox1.Invoke(new EventHandler(delegate
            {
                textBox1.Text = msg.Trim();

            }));
        }

        private void serialPort1_DataReceived(object sender, System.IO.Ports.SerialDataReceivedEventArgs e)
        {
            //接受com傳來的資料
            byte[] buff = null;
            //string s = "";

            buff = new byte[serialPort1.BytesToRead];
            //TextBox2.Text = TextBox2.Text & SerialPort1.ReadExisting

            serialPort1.Read(buff, 0, buff.Length);
            /*
            for (int i = 0; i <= buff.Length - 1; i++)
            {
                s += buff[i].ToString()+"\t\n";


            }
             */
            string text = Encoding.Default.GetString(buff) + "\t\n";
            serialPort1.DiscardInBuffer();
            //textBox1.Text = textBox1.Text + s + "\n";
            //txtbox1(textBox1.Text + text, textBox1);
            Log(textBox1.Text + text);
        }

        private void Rs232save()
        { // log檔 存檔

            string fName = "";
            // 設定INI檔路徑
            String sPath;
            sPath = Application.StartupPath + "\\Config.ini";

            // 讀取ini中的路徑
            fName = ini12.INIRead(sPath, "Log", "Path", "");
            string t = fName + "\\_Log_" + DateTime.Now.ToString("yyyyMMddHHmmss") + "_" + label1.Text + ".txt";

            StreamWriter MYFILE = new StreamWriter(t, false, Encoding.Default);
            MYFILE.WriteLine(textBox1.Text);
            MYFILE.Close();
            txtbox1("", textBox1);

        }

        public void readSch()
        {
            //讀取Scheduler檔
            // 戴入scheduler CSV 檔
            string fName = "";
            // 設定INI檔路徑
            String sPath;
            sPath = Application.StartupPath + "\\Config.ini";

            // 讀取ini中的路徑
            fName = ini12.INIRead(sPath, "Scheduler", "Path", "");


            string TextLine = "";
            string[] SplitLine;
            string[] WordLine;
            int i = 0;
            int j = 0;
            if ((System.IO.File.Exists(fName) == true))
            {
                this.DataGridView1.Rows.Clear();
                System.IO.StreamReader objReader = new System.IO.StreamReader(fName);
                while ((objReader.Peek() != -1))
                {
                    TextLine = objReader.ReadLine();
                    if (i != 0)
                    {
                        SplitLine = TextLine.Split(',');
                        this.DataGridView1.Rows.Add(SplitLine);
                    }
                    i++;
                }
                button1.Enabled = true;
            }
            else
            {
                MessageBox.Show("File doesn't exist", "File Open Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                button1.Enabled = false;
            }

            // 判斷Scheduler格式
            if (TextLine != "")
            {
                WordLine = TextLine.Split(',');
                j = Int32.Parse(WordLine.Length.ToString());
                if (j == 8)
                {
                    button1.Enabled = true;
                    MessageBox.Show("This csv file is older version", "File Open Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                if (j != 8 && j != 10)
                {
                    button1.Enabled = false;
                    MessageBox.Show("This csv file format error", "File Format Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }



        public void MyRunCamd()
        { // 執行緒
            int sRepeat = 0;
            int stime = 0;
            int SysDelay = 0;
            int myloop = 0;

            string fint = "";
            // 設定INI檔路徑
            String sPath;
            sPath = Application.StartupPath + "\\Config.ini";

            // 讀取ini中的路徑回圈數
            fint = ini12.INIRead(sPath, "Loop", "value", "");

            if (fint != "")
            {
                myloop = int.Parse(fint);
            }

            for (int j = 1; j < myloop + 1; j++)
            {
                UpdateUI(j.ToString(), label1);
                int V_scroll = 1;

                lock (this)
                {
                    for (int i = 0; i < this.DataGridView1.Rows.Count - 1; i++)
                    {
                        if (jbutton1 == false)
                        {
                            j = myloop;
                            UpdateUI(j.ToString(), label1);
                            break;
                        }

                        gridUI(i.ToString(), DataGridView1);
                        if (V_scroll < DataGridView1.Rows[1].Height)
                        {
                            V_scroll = i;
                        }
                        else
                        {
                            V_scroll = 0;
                        }
                        //DataGridView1.FirstDisplayedScrollingRowIndex = V_scroll;
                        gridscroll(V_scroll.ToString(), DataGridView1);
                        //MessageBox.Show(this.DataGridView1.Rows[i].Cells[1].Value.ToString());
                        if (this.DataGridView1.Rows[i].Cells[1].Value.ToString() != "")
                        {
                            stime = int.Parse(this.DataGridView1.Rows[i].Cells[1].Value.ToString()); // 次數
                        }
                        else
                        {
                            stime = 1;
                        }

                        if (this.DataGridView1.Rows[i].Cells[2].Value.ToString() != "")
                        {
                            sRepeat = int.Parse(this.DataGridView1.Rows[i].Cells[2].Value.ToString()); // 停止時間
                        }
                        else
                        {
                            sRepeat = 0;
                        }

                        if (this.DataGridView1.Rows[i].Cells[9].Value.ToString() != "")
                        {
                            SysDelay = int.Parse(this.DataGridView1.Rows[i].Cells[9].Value.ToString()); // 指令停止時間
                        }
                        else
                        {
                            SysDelay = 0;
                        }

                        if (this.DataGridView1.Rows[i].Cells[0].Value.ToString() == "_cmd")
                        {
                            // 一般指令
                            //快照
                            if (DataGridView1.Rows[i].Cells[3].Value.ToString() == "_shot")
                            {
                                if (ini12.INIRead(Application.StartupPath + "\\Config.ini", "CameraDev", "value", "") == "1")
                                {
                                    jes();
                                    videostring = DataGridView1.Rows[i].Cells[3].Value.ToString();
                                }
                                else
                                {
                                    MessageBox.Show("Can't open camera", "Camera Open Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                }
                            }

                            //開始錄影
                            if (DataGridView1.Rows[i].Cells[4].Value.ToString() == "_start")
                            {

                                if (ini12.INIRead(Application.StartupPath + "\\Config.ini", "CameraDev", "value", "") == "1")
                                {
                                    if (Vread == false)
                                    {
                                        mysvideo(); // 開新檔
                                        Vread = true;
                                        Thread oThreadC = new Thread(new ThreadStart(MySrtCamd));
                                        oThreadC.Start();
                                        // UpdateUI("true", label3);
                                    }
                                    videostring = DataGridView1.Rows[i].Cells[4].Value.ToString();
                                }
                                else
                                {
                                    MessageBox.Show("Can't open camera", "Camera Open Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                }
                            }

                            // 停止錄影

                            if (DataGridView1.Rows[i].Cells[4].Value.ToString() == "_stop")
                            {
                                if (ini12.INIRead(Application.StartupPath + "\\Config.ini", "CameraDev", "value", "") == "1")
                                {
                                    if (Vread == true) // 判斷是不是正在錄影
                                    {
                                        Vread = false;
                                        mysstop();// 先將先前的關掉
                                        // UpdateUI("false", label3);

                                    }
                                    videostring = DataGridView1.Rows[i].Cells[4].Value.ToString();
                                }
                                else
                                {
                                    MessageBox.Show("Can't open camera", "Camera Open Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                }
                            }

                            // 接收log
                            /*
                            if (DataGridView1.Rows[i].Cells[5].Value.ToString() == "_clear")
                            {
                               // iniRs232(); //開啟rs232
                                Log("");
                            }
                            if (DataGridView1.Rows[i].Cells[5].Value.ToString() == "_save")
                            {
                               // close232(); //關閉rs232
                                Rs232save(); //存檔rs232
                            }
                            */
                            switch (DataGridView1.Rows[i].Cells[5].Value.ToString())
                            {
                                case "_clear":
                                    Log(""); //清楚textbox1
                                    break;
                                case "_save":
                                    Rs232save(); //存檔rs232
                                    break;
                                default:
                                    //byte[] data = Encoding.Unicode.GetBytes(DataGridView1.Rows[i].Cells[5].Value.ToString());
                                    // string str = Convert.ToString(data);
                                    Log(DataGridView1.Rows[i].Cells[5].Value.ToString()); //送出的字串
                                    serialPort1.WriteLine(DataGridView1.Rows[i].Cells[5].Value.ToString() + (char)(13)); //發送數據 Rs232
                                    break;
                            }
                            // 控制電源開關
                            //開
                            if (DataGridView1.Rows[i].Cells[6].Value.ToString() == "_on")
                            {
                                byte[] val1;
                                val1 = new byte[2];
                                val1[0] = 0;

                                bool jSuccess = PL2303_GP0_Enalbe(hCOM, 1);
                                if (!jSuccess)
                                {
                                    Log("GP0 output enable FAILED.");
                                }
                                else
                                {
                                    uint val;

                                    val = (uint)int.Parse("1");
                                    bool bSuccess = PL2303_GP0_SetValue(hCOM, val);

                                    bool kbSuccess = PL2303_GP1_GetValue(hCOM, val1);
                                    if (kbSuccess)
                                    {
                                        if (val1[0] == 0)
                                        {
                                            pbutton1 = true;
                                            button3.Text = "Power OFF";
                                            pictureBox3.Image = Properties.Resources._01;
                                        }
                                        else
                                            pictureBox3.Image = Properties.Resources._02;
                                    }

                                    //if (bSuccess)
                                    //   Log("SET GP0 value Successfully!");
                                    //else
                                    //    Log("SET GP0 value FAILED.");
                                }
                            }

                            //關
                            if (DataGridView1.Rows[i].Cells[6].Value.ToString() == "_off")
                            {
                                byte[] val1;
                                val1 = new byte[2];
                                val1[0] = 0;

                                bool jSuccess = PL2303_GP0_Enalbe(hCOM, 1);
                                if (!jSuccess)
                                {
                                    Log("GP0 output enable FAILED.");
                                }
                                else
                                {
                                    uint val;
                                    val = (uint)int.Parse("0");
                                    bool bSuccess = PL2303_GP0_SetValue(hCOM, val);

                                    bool kbSuccess = PL2303_GP1_GetValue(hCOM, val1);

                                    if (kbSuccess)
                                    {
                                        //if (val1[0] == 0)
                                            pbutton1 = false;
                                            button3.Text = "Power ON";
                                            pictureBox3.Image = Properties.Resources._02;
                                        //else
                                            //pictureBox3.Image = Properties.Resources._01;
                                    }

                                    //if (bSuccess)
                                    //    Log("SET GP0 value Successfully!");
                                    //else
                                    //    Log("SET GP0 value FAILED.");
                                }
                            }
                        }

                        #region Astro Timing
                        else if (this.DataGridView1.Rows[i].Cells[0].Value.ToString() == "_astro")
                        {

                            // Astro指令
                            byte[] startbit = new byte[7] { 0x05, 0x24, 0x20, 0x02, 0xfd, 0x24, 0x20 };
                            serialPort1.Write(startbit, 0, 7);
                            switch (DataGridView1.Rows[i].Cells[7].Value.ToString())
                            {
                                case "VESA 640x480@60":
                                    byte[] timebit = new byte[4] { 0x31, 0x36, 0x30, 0x34 };
                                    serialPort1.Write(timebit, 0, 4);
                                    break;
                                case "VESA 640x480@72":
                                    timebit = new byte[4] { 0x31, 0x36, 0x30, 0x35 };
                                    serialPort1.Write(timebit, 0, 4);
                                    break;
                                case "VESA 640x480@75":
                                    timebit = new byte[4] { 0x31, 0x36, 0x30, 0x36 };
                                    serialPort1.Write(timebit, 0, 4);
                                    break;
                                case "VESA 640x480@85":
                                    timebit = new byte[4] { 0x31, 0x36, 0x30, 0x37 };
                                    serialPort1.Write(timebit, 0, 4);
                                    break;
                                case "720x400@70":
                                    timebit = new byte[4] { 0x31, 0x38, 0x37, 0x37 };
                                    serialPort1.Write(timebit, 0, 4);
                                    break;
                                case "VESA 720x400@85":
                                    timebit = new byte[4] { 0x31, 0x36, 0x30, 0x33 };
                                    serialPort1.Write(timebit, 0, 4);
                                    break;
                                case "VESA 800x600@56":
                                    timebit = new byte[4] { 0x31, 0x36, 0x30, 0x38 };
                                    serialPort1.Write(timebit, 0, 4);
                                    break;
                                case "VESA 800x600@60":
                                    timebit = new byte[4] { 0x31, 0x36, 0x30, 0x39 };
                                    serialPort1.Write(timebit, 0, 4);
                                    break;
                                case "VESA 800x600@72":
                                    timebit = new byte[4] { 0x31, 0x36, 0x31, 0x30 };
                                    serialPort1.Write(timebit, 0, 4);
                                    break;
                                case "VESA 800x600@75":
                                    timebit = new byte[4] { 0x31, 0x36, 0x31, 0x31 };
                                    serialPort1.Write(timebit, 0, 4);
                                    break;
                                case "VESA 800x600@85":
                                    timebit = new byte[4] { 0x31, 0x36, 0x31, 0x32 };
                                    serialPort1.Write(timebit, 0, 4);
                                    break;
                                case "VESA 1024x768@60":
                                    timebit = new byte[4] { 0x31, 0x36, 0x31, 0x36 };
                                    serialPort1.Write(timebit, 0, 4);
                                    break;
                                case "VESA 1024x768@70":
                                    timebit = new byte[4] { 0x31, 0x36, 0x31, 0x37 };
                                    serialPort1.Write(timebit, 0, 4);
                                    break;
                                case "VESA 1024x768@75":
                                    timebit = new byte[4] { 0x31, 0x36, 0x31, 0x38 };
                                    serialPort1.Write(timebit, 0, 4);
                                    break;
                                case "VESA 1024x768@85":
                                    timebit = new byte[4] { 0x31, 0x36, 0x31, 0x39 };
                                    serialPort1.Write(timebit, 0, 4);
                                    break;
                                case "VESA 1152x864@75":
                                    timebit = new byte[4] { 0x31, 0x36, 0x32, 0x31 };
                                    serialPort1.Write(timebit, 0, 4);
                                    break;
                                case "VESA 1280x768@60 (Reduced Blanking)":
                                    timebit = new byte[4] { 0x31, 0x36, 0x32, 0x32 };
                                    serialPort1.Write(timebit, 0, 4);
                                    break;
                                case "VESA 1280x768@60":
                                    timebit = new byte[4] { 0x31, 0x36, 0x32, 0x33 };
                                    serialPort1.Write(timebit, 0, 4);
                                    break;
                                case "VESA 1280x768@75":
                                    timebit = new byte[4] { 0x31, 0x36, 0x32, 0x34 };
                                    serialPort1.Write(timebit, 0, 4);
                                    break;
                                case "VESA 1280x768@85":
                                    timebit = new byte[4] { 0x31, 0x36, 0x32, 0x35 };
                                    serialPort1.Write(timebit, 0, 4);
                                    break;
                                case "VESA 1280x960@60":
                                    timebit = new byte[4] { 0x31, 0x36, 0x33, 0x32 };
                                    serialPort1.Write(timebit, 0, 4);
                                    break;
                                case "VESA 1280x960@85":
                                    timebit = new byte[4] { 0x31, 0x36, 0x33, 0x33 };
                                    serialPort1.Write(timebit, 0, 4);
                                    break;
                                case "VESA 1280x1024@60":
                                    timebit = new byte[4] { 0x31, 0x36, 0x33, 0x35 };
                                    serialPort1.Write(timebit, 0, 4);
                                    break;
                                case "VESA 1280x1024@75":
                                    timebit = new byte[4] { 0x31, 0x36, 0x33, 0x36 };
                                    serialPort1.Write(timebit, 0, 4);
                                    break;
                                case "VESA 1280x1024@85":
                                    timebit = new byte[4] { 0x31, 0x36, 0x33, 0x37 };
                                    serialPort1.Write(timebit, 0, 4);
                                    break;
                                case "VESA 1360x768@60 (Reduced Blanking)":
                                    timebit = new byte[4] { 0x31, 0x36, 0x33, 0x39 };
                                    serialPort1.Write(timebit, 0, 4);
                                    break;
                                case "VESA 1360x768@60":
                                    timebit = new byte[4] { 0x31, 0x36, 0x34, 0x30 };
                                    serialPort1.Write(timebit, 0, 4);
                                    break;
                                case "VESA 1400x1050@60  (Reduced Blanking)":
                                    timebit = new byte[4] { 0x31, 0x36, 0x34, 0x31 };
                                    serialPort1.Write(timebit, 0, 4);
                                    break;
                                case "VESA 1400x1050@60":
                                    timebit = new byte[4] { 0x31, 0x36, 0x34, 0x32 };
                                    serialPort1.Write(timebit, 0, 4);
                                    break;
                                case "VESA 1400x1050@75":
                                    timebit = new byte[4] { 0x31, 0x36, 0x34, 0x33 };
                                    serialPort1.Write(timebit, 0, 4);
                                    break;
                                case "VESA 1400x1050@85":
                                    timebit = new byte[4] { 0x31, 0x36, 0x34, 0x34 };
                                    serialPort1.Write(timebit, 0, 4);
                                    break;
                                case "VESA 1600x1200@60":
                                    timebit = new byte[4] { 0x31, 0x36, 0x35, 0x31 };
                                    serialPort1.Write(timebit, 0, 4);
                                    break;
                                case "VESA 1600x1200@65":
                                    timebit = new byte[4] { 0x31, 0x36, 0x35, 0x32 };
                                    serialPort1.Write(timebit, 0, 4);
                                    break;
                                case "VESA 1600x1200@70":
                                    timebit = new byte[4] { 0x31, 0x36, 0x35, 0x33 };
                                    serialPort1.Write(timebit, 0, 4);
                                    break;
                                case "VESA 1600x1200@75":
                                    timebit = new byte[4] { 0x31, 0x36, 0x35, 0x34 };
                                    serialPort1.Write(timebit, 0, 4);
                                    break;
                                case "VESA 1600x1200@85":
                                    timebit = new byte[4] { 0x31, 0x36, 0x35, 0x35 };
                                    serialPort1.Write(timebit, 0, 4);
                                    break;
                                case "VESA 1680x1050@60 (Reduced Blanking)":
                                    timebit = new byte[4] { 0x31, 0x36, 0x35, 0x37 };
                                    serialPort1.Write(timebit, 0, 4);
                                    break;
                                case "VESA 1680x1050@60":
                                    timebit = new byte[4] { 0x31, 0x36, 0x35, 0x38 };
                                    serialPort1.Write(timebit, 0, 4);
                                    break;
                                case "VESA 1680x1050@75":
                                    timebit = new byte[4] { 0x31, 0x36, 0x35, 0x39 };
                                    serialPort1.Write(timebit, 0, 4);
                                    break;
                                case "VESA 1680x1050@85":
                                    timebit = new byte[4] { 0x31, 0x36, 0x36, 0x30 };
                                    serialPort1.Write(timebit, 0, 4);
                                    break;
                                case "VESA 1920x1080@60":
                                    timebit = new byte[4] { 0x31, 0x36, 0x38, 0x31 };
                                    serialPort1.Write(timebit, 0, 4);
                                    break;
                                case "VESA 1920x1200@60 (Reduced Blanking)":
                                    timebit = new byte[4] { 0x31, 0x36, 0x36, 0x38 };
                                    serialPort1.Write(timebit, 0, 4);
                                    break;
                                case "VESA 1920x1200@60":
                                    timebit = new byte[4] { 0x31, 0x36, 0x36, 0x39 };
                                    serialPort1.Write(timebit, 0, 4);
                                    break;
                                case "VESA 1920x1200@75":
                                    timebit = new byte[4] { 0x31, 0x36, 0x37, 0x30 };
                                    serialPort1.Write(timebit, 0, 4);
                                    break;
                                case "VESA 1920x1200@85":
                                    timebit = new byte[4] { 0x31, 0x36, 0x37, 0x31 };
                                    serialPort1.Write(timebit, 0, 4);
                                    break;
                                case "640x480@66.67 (Mac)":
                                    timebit = new byte[4] { 0x31, 0x39, 0x30, 0x39 };
                                    serialPort1.Write(timebit, 0, 4);
                                    break;
                                case "832x624@74.55 (Mac)":
                                    timebit = new byte[4] { 0x31, 0x39, 0x31, 0x32 };
                                    serialPort1.Write(timebit, 0, 4);
                                    break;
                                case "1024x768@59.56 (Mac)":
                                    timebit = new byte[4] { 0x31, 0x39, 0x31, 0x33 };
                                    serialPort1.Write(timebit, 0, 4);
                                    break;
                                case "1024x768@74.93 (Mac)":
                                    timebit = new byte[4] { 0x31, 0x39, 0x31, 0x34 };
                                    serialPort1.Write(timebit, 0, 4);
                                    break;
                                case "1152x870@75 (Mac)":
                                    timebit = new byte[4] { 0x31, 0x39, 0x31, 0x35 };
                                    serialPort1.Write(timebit, 0, 4);
                                    break;
                                case "640x480p@59.94":
                                    timebit = new byte[4] { 0x31, 0x30, 0x30, 0x31 };
                                    serialPort1.Write(timebit, 0, 4);
                                    break;
                                case "720x480p@59.94":
                                    timebit = new byte[4] { 0x31, 0x30, 0x30, 0x33 };
                                    serialPort1.Write(timebit, 0, 4);
                                    break;
                                case "720x480p@60":
                                    timebit = new byte[4] { 0x31, 0x30, 0x30, 0x34 };
                                    serialPort1.Write(timebit, 0, 4);
                                    break;
                                case "1280x720p@59.94":
                                    timebit = new byte[4] { 0x31, 0x30, 0x30, 0x37 };
                                    serialPort1.Write(timebit, 0, 4);
                                    break;
                                case "1280x720p@60":
                                    timebit = new byte[4] { 0x31, 0x30, 0x30, 0x38 };
                                    serialPort1.Write(timebit, 0, 4);
                                    break;
                                case "1920x1080i@59.94":
                                    timebit = new byte[4] { 0x31, 0x30, 0x30, 0x39 };
                                    serialPort1.Write(timebit, 0, 4);
                                    break;
                                case "1920x1080i@60":
                                    timebit = new byte[4] { 0x31, 0x30, 0x31, 0x30 };
                                    serialPort1.Write(timebit, 0, 4);
                                    break;
                                case "1920x1080p@59.94":
                                    timebit = new byte[4] { 0x31, 0x30, 0x33, 0x39 };
                                    serialPort1.Write(timebit, 0, 4);
                                    break;
                                case "1920x1080p@60":
                                    timebit = new byte[4] { 0x31, 0x30, 0x34, 0x30 };
                                    serialPort1.Write(timebit, 0, 4);
                                    break;
                                case "2880x480i@59.94":
                                    timebit = new byte[4] { 0x31, 0x30, 0x32, 0x33 };
                                    serialPort1.Write(timebit, 0, 4);
                                    break;
                                case "2880x480i@60":
                                    timebit = new byte[4] { 0x31, 0x30, 0x32, 0x34 };
                                    serialPort1.Write(timebit, 0, 4);
                                    break;
                                case "2880x240p@60.05":
                                    timebit = new byte[4] { 0x31, 0x30, 0x32, 0x37 };
                                    serialPort1.Write(timebit, 0, 4);
                                    break;
                                case "2880x240p@60.11":
                                    timebit = new byte[4] { 0x31, 0x30, 0x32, 0x38 };
                                    serialPort1.Write(timebit, 0, 4);
                                    break;
                                case "2880x240p@59.83":
                                    timebit = new byte[4] { 0x31, 0x30, 0x32, 0x39 };
                                    serialPort1.Write(timebit, 0, 4);
                                    break;
                                case "2880x240p@59.89":
                                    timebit = new byte[4] { 0x31, 0x30, 0x33, 0x30 };
                                    serialPort1.Write(timebit, 0, 4);
                                    break;
                                case "1440x480p@59.94":
                                    timebit = new byte[4] { 0x31, 0x30, 0x33, 0x35 };
                                    serialPort1.Write(timebit, 0, 4);
                                    break;
                                case "1440x480p@60":
                                    timebit = new byte[4] { 0x31, 0x30, 0x33, 0x36 };
                                    serialPort1.Write(timebit, 0, 4);
                                    break;
                                case "2880x480p@59.94":
                                    timebit = new byte[4] { 0x31, 0x30, 0x36, 0x39 };
                                    serialPort1.Write(timebit, 0, 4);
                                    break;
                                case "2880x480p@60":
                                    timebit = new byte[4] { 0x31, 0x30, 0x37, 0x30 };
                                    serialPort1.Write(timebit, 0, 4);
                                    break;
                                case "720x576p@50":
                                    timebit = new byte[4] { 0x31, 0x30, 0x34, 0x31 };
                                    serialPort1.Write(timebit, 0, 4);
                                    break;
                                case "1280x720p@50":
                                    timebit = new byte[4] { 0x31, 0x31, 0x36, 0x30 };
                                    serialPort1.Write(timebit, 0, 4);
                                    break;
                                case "1920x1080i@50":
                                    timebit = new byte[4] { 0x31, 0x30, 0x34, 0x34 };
                                    serialPort1.Write(timebit, 0, 4);
                                    break;
                                case "1920x1080p@25":
                                    timebit = new byte[4] { 0x31, 0x30, 0x36, 0x36 };
                                    serialPort1.Write(timebit, 0, 4);
                                    break;
                                case "1920x1080p@50":
                                    timebit = new byte[4] { 0x31, 0x30, 0x36, 0x31 };
                                    serialPort1.Write(timebit, 0, 4);
                                    break;
                                case "2880x576i@50":
                                    timebit = new byte[4] { 0x31, 0x30, 0x35, 0x33 };
                                    serialPort1.Write(timebit, 0, 4);
                                    break;
                                case "2880x288p@50.08":
                                    timebit = new byte[4] { 0x31, 0x30, 0x35, 0x35 };
                                    serialPort1.Write(timebit, 0, 4);
                                    break;
                                case "2880x288p@49.92":
                                    timebit = new byte[4] { 0x31, 0x30, 0x35, 0x36 };
                                    serialPort1.Write(timebit, 0, 4);
                                    break;
                                case "2880x288p@49.76":
                                    timebit = new byte[4] { 0x31, 0x30, 0x35, 0x37 };
                                    serialPort1.Write(timebit, 0, 4);
                                    break;
                                case "1440x576p@50":
                                    timebit = new byte[4] { 0x31, 0x30, 0x36, 0x31 };
                                    serialPort1.Write(timebit, 0, 4);
                                    break;
                                case "2880x576p@50":
                                    timebit = new byte[4] { 0x31, 0x30, 0x37, 0x33 };
                                    serialPort1.Write(timebit, 0, 4);
                                    break;
                                default:
                                    break;
                            }
                            byte[] endbit = new byte[3] { 0x2c, 0x31, 0x03 };
                            serialPort1.Write(endbit, 0, 3);
                            RedratLable.Text = DataGridView1.Rows[i].Cells[7].Value.ToString();
                        }
                        #endregion

                        #region Quantum Timing
                        else if (this.DataGridView1.Rows[i].Cells[0].Value.ToString() == "_quantum")
                        {
                            // Quantum指令
                            switch (DataGridView1.Rows[i].Cells[7].Value.ToString())
                            {
                                case "VESA 640x480@60":
                                    serialPort1.WriteLine("FMTL DMT0660" + "\r");
                                    serialPort1.WriteLine("ALLU" + "\r");
                                    break;
                                case "VESA 640x480@72":
                                    serialPort1.WriteLine("FMTL DMT0672" + "\r");
                                    serialPort1.WriteLine("ALLU" + "\r");
                                    break;
                                case "VESA 640x480@75":
                                    serialPort1.WriteLine("FMTL DMT0675" + "\r");
                                    serialPort1.WriteLine("ALLU" + "\r");
                                    break;
                                case "VESA 640x480@85":
                                    serialPort1.WriteLine("FMTL DMT0685" + "\r");
                                    serialPort1.WriteLine("ALLU" + "\r");
                                    break;
                                case "720x400@70":
                                    serialPort1.WriteLine("FMTL IBM770H" + "\r");
                                    serialPort1.WriteLine("ALLU" + "\r");
                                    break;
                                case "VESA 720x400@85":
                                    serialPort1.WriteLine("FMTL DMT785H" + "\r");
                                    serialPort1.WriteLine("ALLU" + "\r");
                                    break;
                                case "VESA 800x600@56":
                                    serialPort1.WriteLine("FMTL DMT0856" + "\r");
                                    serialPort1.WriteLine("ALLU" + "\r");
                                    break;
                                case "VESA 800x600@60":
                                    serialPort1.WriteLine("FMTL DMT0860" + "\r");
                                    serialPort1.WriteLine("ALLU" + "\r");
                                    break;
                                case "VESA 800x600@72":
                                    serialPort1.WriteLine("FMTL DMT0872" + "\r");
                                    serialPort1.WriteLine("ALLU" + "\r");
                                    break;
                                case "VESA 800x600@75":
                                    serialPort1.WriteLine("FMTL DMT0875" + "\r");
                                    serialPort1.WriteLine("ALLU" + "\r");
                                    break;
                                case "VESA 800x600@85":
                                    serialPort1.WriteLine("FMTL DMT0885" + "\r");
                                    serialPort1.WriteLine("ALLU" + "\r");
                                    break;
                                case "VESA 1024x768@60":
                                    serialPort1.WriteLine("FMTL DMT1060" + "\r");
                                    serialPort1.WriteLine("ALLU" + "\r");
                                    break;
                                case "VESA 1024x768@70":
                                    serialPort1.WriteLine("FMTL DMT1070" + "\r");
                                    serialPort1.WriteLine("ALLU" + "\r");
                                    break;
                                case "VESA 1024x768@75":
                                    serialPort1.WriteLine("FMTL DMT1075" + "\r");
                                    serialPort1.WriteLine("ALLU" + "\r");
                                    break;
                                case "VESA 1024x768@85":
                                    serialPort1.WriteLine("FMTL DMT1085" + "\r");
                                    serialPort1.WriteLine("ALLU" + "\r");
                                    break;
                                case "VESA 1152x864@75":
                                    serialPort1.WriteLine("FMTL DMT1175" + "\r");
                                    serialPort1.WriteLine("ALLU" + "\r");
                                    break;
                                case "VESA 1280x720@60 (Reduced Blanking)":
                                    serialPort1.WriteLine("FMTL CVR1260H" + "\r");
                                    serialPort1.WriteLine("ALLU" + "\r");
                                    break;
                                case "VESA 1280x720@60":
                                    serialPort1.WriteLine("FMTL CVT1260H" + "\r");
                                    serialPort1.WriteLine("ALLU" + "\r");
                                    break;
                                case "VESA 1280x720@75":
                                    serialPort1.WriteLine("FMTL CVT1275H" + "\r");
                                    serialPort1.WriteLine("ALLU" + "\r");
                                    break;
                                case "VESA 1280x720@85":
                                    serialPort1.WriteLine("FMTL CVT1285H" + "\r");
                                    serialPort1.WriteLine("ALLU" + "\r");
                                    break;
                                case "VESA 1280x768@60 (Reduced Blanking)":
                                    serialPort1.WriteLine("FMTL CVR1260E" + "\r");
                                    serialPort1.WriteLine("ALLU" + "\r");
                                    break;
                                case "VESA 1280x768@60":
                                    serialPort1.WriteLine("FMTL CVT1260E" + "\r");
                                    serialPort1.WriteLine("ALLU" + "\r");
                                    break;
                                case "VESA 1280x768@75":
                                    serialPort1.WriteLine("FMTL CVT1275E" + "\r");
                                    serialPort1.WriteLine("ALLU" + "\r");
                                    break;
                                case "VESA 1280x768@85":
                                    serialPort1.WriteLine("FMTL CVT1285E" + "\r");
                                    serialPort1.WriteLine("ALLU" + "\r");
                                    break;
                                case "VESA 1280x960@60":
                                    serialPort1.WriteLine("FMTL DMT1260A" + "\r");
                                    serialPort1.WriteLine("ALLU" + "\r");
                                    break;
                                case "VESA 1280x960@85":
                                    serialPort1.WriteLine("FMTL DMT1285A" + "\r");
                                    serialPort1.WriteLine("ALLU" + "\r");
                                    break;
                                case "VESA 1280x1024@60":
                                    serialPort1.WriteLine("FMTL DMT1260G" + "\r");
                                    serialPort1.WriteLine("ALLU" + "\r");
                                    break;
                                case "VESA 1280x1024@75":
                                    serialPort1.WriteLine("FMTL DMT1275G" + "\r");
                                    serialPort1.WriteLine("ALLU" + "\r");
                                    break;
                                case "VESA 1280x1024@85":
                                    serialPort1.WriteLine("FMTL DMT1285G" + "\r");
                                    serialPort1.WriteLine("ALLU" + "\r");
                                    break;
                                case "VESA 1360x768@60 (Reduced Blanking)":
                                    serialPort1.WriteLine("FMTL CVR1360H" + "\r");
                                    serialPort1.WriteLine("ALLU" + "\r");
                                    break;
                                case "VESA 1360x768@60":
                                    serialPort1.WriteLine("FMTL CVT1360H" + "\r");
                                    serialPort1.WriteLine("ALLU" + "\r");
                                    break;
                                case "VESA 1360x768@75":
                                    serialPort1.WriteLine("FMTL CVT1375H" + "\r");
                                    serialPort1.WriteLine("ALLU" + "\r");
                                    break;
                                case "VESA 1360x768@85":
                                    serialPort1.WriteLine("FMTL CVT1385H" + "\r");
                                    serialPort1.WriteLine("ALLU" + "\r");
                                    break;
                                case "VESA 1400x1050@60  (Reduced Blanking)":
                                    serialPort1.WriteLine("FMTL CVR1460" + "\r");
                                    serialPort1.WriteLine("ALLU" + "\r");
                                    break;
                                case "VESA 1400x1050@60":
                                    serialPort1.WriteLine("FMTL CVT1460" + "\r");
                                    serialPort1.WriteLine("ALLU" + "\r");
                                    break;
                                case "VESA 1400x1050@75":
                                    serialPort1.WriteLine("FMTL CVT1475" + "\r");
                                    serialPort1.WriteLine("ALLU" + "\r");
                                    break;
                                case "VESA 1400x1050@85":
                                    serialPort1.WriteLine("FMTL CVT1485" + "\r");
                                    serialPort1.WriteLine("ALLU" + "\r");
                                    break;
                                case "VESA 1600x1200@60":
                                    serialPort1.WriteLine("FMTL DMT1660" + "\r");
                                    serialPort1.WriteLine("ALLU" + "\r");
                                    break;
                                case "VESA 1600x1200@65":
                                    serialPort1.WriteLine("FMTL DMT1665" + "\r");
                                    serialPort1.WriteLine("ALLU" + "\r");
                                    break;
                                case "VESA 1600x1200@70":
                                    serialPort1.WriteLine("FMTL DMT1670" + "\r");
                                    serialPort1.WriteLine("ALLU" + "\r");
                                    break;
                                case "VESA 1600x1200@75":
                                    serialPort1.WriteLine("FMTL DMT1675" + "\r");
                                    serialPort1.WriteLine("ALLU" + "\r");
                                    break;
                                case "VESA 1600x1200@85":
                                    serialPort1.WriteLine("FMTL DMT1685" + "\r");
                                    serialPort1.WriteLine("ALLU" + "\r");
                                    break;
                                case "VESA 1680x1050@60 (Reduced Blanking)":
                                    serialPort1.WriteLine("FMTL CVR1660D" + "\r");
                                    serialPort1.WriteLine("ALLU" + "\r");
                                    break;
                                case "VESA 1680x1050@60":
                                    serialPort1.WriteLine("FMTL CVT1660D" + "\r");
                                    serialPort1.WriteLine("ALLU" + "\r");
                                    break;
                                case "VESA 1680x1050@75":
                                    serialPort1.WriteLine("FMTL CVT1675D" + "\r");
                                    serialPort1.WriteLine("ALLU" + "\r");
                                    break;
                                case "VESA 1680x1050@85":
                                    serialPort1.WriteLine("FMTL CVT1685D" + "\r");
                                    serialPort1.WriteLine("ALLU" + "\r");
                                    break;
                                case "VESA 1920x1080@60 (Reduced Blanking)":
                                    serialPort1.WriteLine("FMTL CVR1960H" + "\r");
                                    serialPort1.WriteLine("ALLU" + "\r");
                                    break;
                                case "VESA 1920x1080@60":
                                    serialPort1.WriteLine("FMTL CVT1960H" + "\r");
                                    serialPort1.WriteLine("ALLU" + "\r");
                                    break;
                                case "VESA 1920x1080@75":
                                    serialPort1.WriteLine("FMTL CVT1975H" + "\r");
                                    serialPort1.WriteLine("ALLU" + "\r");
                                    break;
                                case "VESA 1920x1080@85":
                                    serialPort1.WriteLine("FMTL CVT1985H" + "\r");
                                    serialPort1.WriteLine("ALLU" + "\r");
                                    break;
                                case "VESA 1920x1200@60 (Reduced Blanking)":
                                    serialPort1.WriteLine("FMTL CVR1960D" + "\r");
                                    serialPort1.WriteLine("ALLU" + "\r");
                                    break;
                                case "VESA 1920x1200@60":
                                    serialPort1.WriteLine("FMTL CVT1960D" + "\r");
                                    serialPort1.WriteLine("ALLU" + "\r");
                                    break;
                                case "VESA 1920x1200@75":
                                    serialPort1.WriteLine("FMTL CVT1975D" + "\r");
                                    serialPort1.WriteLine("ALLU" + "\r");
                                    break;
                                case "VESA 1920x1200@85":
                                    serialPort1.WriteLine("FMTL CVT1985D" + "\r");
                                    serialPort1.WriteLine("ALLU" + "\r");
                                    break;
                                case "640x480@66.67 (Mac)":
                                    serialPort1.WriteLine("FMTL AAP0667" + "\r");
                                    serialPort1.WriteLine("ALLU" + "\r");
                                    break;
                                case "832x624@74.55 (Mac)":
                                    serialPort1.WriteLine("FMTL AAP0875" + "\r");
                                    serialPort1.WriteLine("ALLU" + "\r");
                                    break;
                                case "1024x768@59.56 (Mac)":
                                    serialPort1.WriteLine("FMTL AAP1069" + "\r");
                                    serialPort1.WriteLine("ALLU" + "\r");
                                    break;
                                case "1024x768@74.93 (Mac)":
                                    serialPort1.WriteLine("FMTL AAP1075" + "\r");
                                    serialPort1.WriteLine("ALLU" + "\r");
                                    break;
                                case "1152x870@75 (Mac)":
                                    serialPort1.WriteLine("FMTL AAP1175" + "\r");
                                    serialPort1.WriteLine("ALLU" + "\r");
                                    break;
                                case "720x480i@59.94":
                                    serialPort1.WriteLine("FMTL 2000" + "\r");
                                    serialPort1.WriteLine("ALLU" + "\r");
                                    break;
                                case "720x480i@60":
                                    serialPort1.WriteLine("FMTL 2001" + "\r");
                                    serialPort1.WriteLine("ALLU" + "\r");
                                    break;
                                case "640x480p@59.94":
                                    serialPort1.WriteLine("FMTL 2002" + "\r");
                                    serialPort1.WriteLine("ALLU" + "\r");
                                    break;
                                case "720x480p@59.94":
                                    serialPort1.WriteLine("FMTL 2003" + "\r");
                                    serialPort1.WriteLine("ALLU" + "\r");
                                    break;
                                case "720x480p@60":
                                    serialPort1.WriteLine("FMTL 2004" + "\r");
                                    serialPort1.WriteLine("ALLU" + "\r");
                                    break;
                                case "1280x720p@59.94":
                                    serialPort1.WriteLine("FMTL 2005" + "\r");
                                    serialPort1.WriteLine("ALLU" + "\r");
                                    break;
                                case "1280x720p@60":
                                    serialPort1.WriteLine("FMTL 2006" + "\r");
                                    serialPort1.WriteLine("ALLU" + "\r");
                                    break;
                                case "1920x1080i@59.94":
                                    serialPort1.WriteLine("FMTL 2007" + "\r");
                                    serialPort1.WriteLine("ALLU" + "\r");
                                    break;
                                case "1920x1080i@60":
                                    serialPort1.WriteLine("FMTL 2008" + "\r");
                                    serialPort1.WriteLine("ALLU" + "\r");
                                    break;
                                case "1920x1080p@23.97":
                                    serialPort1.WriteLine("FMTL 2009" + "\r");
                                    serialPort1.WriteLine("ALLU" + "\r");
                                    break;
                                case "1920x1080p@24":
                                    serialPort1.WriteLine("FMTL 2010" + "\r");
                                    serialPort1.WriteLine("ALLU" + "\r");
                                    break;
                                case "1920x1080p@29.97":
                                    serialPort1.WriteLine("FMTL 2011" + "\r");
                                    serialPort1.WriteLine("ALLU" + "\r");
                                    break;
                                case "1920x1080p@30":
                                    serialPort1.WriteLine("FMTL 2012" + "\r");
                                    serialPort1.WriteLine("ALLU" + "\r");
                                    break;
                                case "1920x1080p@59.94":
                                    serialPort1.WriteLine("FMTL 2013" + "\r");
                                    serialPort1.WriteLine("ALLU" + "\r");
                                    break;
                                case "1920x1080p@60":
                                    serialPort1.WriteLine("FMTL 2014" + "\r");
                                    serialPort1.WriteLine("ALLU" + "\r");
                                    break;
                                case "720x240p@60.05":
                                    serialPort1.WriteLine("FMTL 2015" + "\r");
                                    serialPort1.WriteLine("ALLU" + "\r");
                                    break;
                                case "720x240p@60.11":
                                    serialPort1.WriteLine("FMTL 2016" + "\r");
                                    serialPort1.WriteLine("ALLU" + "\r");
                                    break;
                                case "720x240p@59.83":
                                    serialPort1.WriteLine("FMTL 2017" + "\r");
                                    serialPort1.WriteLine("ALLU" + "\r");
                                    break;
                                case "720x240p@59.89":
                                    serialPort1.WriteLine("FMTL 2018" + "\r");
                                    serialPort1.WriteLine("ALLU" + "\r");
                                    break;
                                case "2880x480i@59.94":
                                    serialPort1.WriteLine("FMTL 2019" + "\r");
                                    serialPort1.WriteLine("ALLU" + "\r");
                                    break;
                                case "2880x480i@60":
                                    serialPort1.WriteLine("FMTL 2020" + "\r");
                                    serialPort1.WriteLine("ALLU" + "\r");
                                    break;
                                case "2880x240p@60.05":
                                    serialPort1.WriteLine("FMTL 2021" + "\r");
                                    serialPort1.WriteLine("ALLU" + "\r");
                                    break;
                                case "2880x240p@60.11":
                                    serialPort1.WriteLine("FMTL 2022" + "\r");
                                    serialPort1.WriteLine("ALLU" + "\r");
                                    break;
                                case "2880x240p@59.83":
                                    serialPort1.WriteLine("FMTL 2023" + "\r");
                                    serialPort1.WriteLine("ALLU" + "\r");
                                    break;
                                case "2880x240p@59.89":
                                    serialPort1.WriteLine("FMTL 2024" + "\r");
                                    serialPort1.WriteLine("ALLU" + "\r");
                                    break;
                                case "1440x480p@59.94":
                                    serialPort1.WriteLine("FMTL 2025" + "\r");
                                    serialPort1.WriteLine("ALLU" + "\r");
                                    break;
                                case "1440x480p@60":
                                    serialPort1.WriteLine("FMTL 2026" + "\r");
                                    serialPort1.WriteLine("ALLU" + "\r");
                                    break;
                                case "2880x480p@59.94":
                                    serialPort1.WriteLine("FMTL 2027" + "\r");
                                    serialPort1.WriteLine("ALLU" + "\r");
                                    break;
                                case "2880x480p@60":
                                    serialPort1.WriteLine("FMTL 2028" + "\r");
                                    serialPort1.WriteLine("ALLU" + "\r");
                                    break;
                                case "720x576i@50":
                                    serialPort1.WriteLine("FMTL 2029" + "\r");
                                    serialPort1.WriteLine("ALLU" + "\r");
                                    break;
                                case "720x576p@50":
                                    serialPort1.WriteLine("FMTL 2030" + "\r");
                                    serialPort1.WriteLine("ALLU" + "\r");
                                    break;
                                case "1280x720p@50":
                                    serialPort1.WriteLine("FMTL 2031" + "\r");
                                    serialPort1.WriteLine("ALLU" + "\r");
                                    break;
                                case "1920x1080i@50":
                                    serialPort1.WriteLine("FMTL 2032" + "\r");
                                    serialPort1.WriteLine("ALLU" + "\r");
                                    break;
                                case "1920x1080p@25":
                                    serialPort1.WriteLine("FMTL 2033" + "\r");
                                    serialPort1.WriteLine("ALLU" + "\r");
                                    break;
                                case "1920x1080p@50":
                                    serialPort1.WriteLine("FMTL 2034" + "\r");
                                    serialPort1.WriteLine("ALLU" + "\r");
                                    break;
                                case "720x288p@50.08":
                                    serialPort1.WriteLine("FMTL 2035" + "\r");
                                    serialPort1.WriteLine("ALLU" + "\r");
                                    break;
                                case "720x288p@49.92":
                                    serialPort1.WriteLine("FMTL 2036" + "\r");
                                    serialPort1.WriteLine("ALLU" + "\r");
                                    break;
                                case "720x288p@49.76":
                                    serialPort1.WriteLine("FMTL 2037" + "\r");
                                    serialPort1.WriteLine("ALLU" + "\r");
                                    break;
                                case "2880x576i@50":
                                    serialPort1.WriteLine("FMTL 2038" + "\r");
                                    serialPort1.WriteLine("ALLU" + "\r");
                                    break;
                                case "2880x288p@50.08":
                                    serialPort1.WriteLine("FMTL 2039" + "\r");
                                    serialPort1.WriteLine("ALLU" + "\r");
                                    break;
                                case "2880x288p@49.92":
                                    serialPort1.WriteLine("FMTL 2040" + "\r");
                                    serialPort1.WriteLine("ALLU" + "\r");
                                    break;
                                case "2880x288p@49.76":
                                    serialPort1.WriteLine("FMTL 2041" + "\r");
                                    serialPort1.WriteLine("ALLU" + "\r");
                                    break;
                                case "1440x576p@50":
                                    serialPort1.WriteLine("FMTL 2042" + "\r");
                                    serialPort1.WriteLine("ALLU" + "\r");
                                    break;
                                case "2880x576p@50":
                                    serialPort1.WriteLine("FMTL 2043" + "\r");
                                    serialPort1.WriteLine("ALLU" + "\r");
                                    break;
                                default:
                                    break;
                            }

                            switch (DataGridView1.Rows[i].Cells[8].Value.ToString())
                            {
                                case "RGB":
                                    // RGB mode
                                    serialPort1.WriteLine("AVST 0" + "\r");
                                    serialPort1.WriteLine("DVST 10" + "\r");
                                    serialPort1.WriteLine("FMTU" + "\r");
                                    break;
                                case "YCbCr":
                                    // YCbCr mode
                                    serialPort1.WriteLine("AVST 0" + "\r");
                                    serialPort1.WriteLine("DVST 14" + "\r");
                                    serialPort1.WriteLine("FMTU" + "\r");
                                    break;
                                case "xvYCC":
                                    // xvYCC mode
                                    serialPort1.WriteLine("AVST 0" + "\r");
                                    serialPort1.WriteLine("DVST 17" + "\r");
                                    serialPort1.WriteLine("FMTU" + "\r");
                                    break;
                                case "4:4:4":
                                    // 4:4:4
                                    serialPort1.WriteLine("DVSM 4" + "\r");
                                    serialPort1.WriteLine("FMTU" + "\r");
                                    break;
                                case "4:2:2":
                                    // 4:2:2
                                    serialPort1.WriteLine("DVSM 2" + "\r");
                                    serialPort1.WriteLine("FMTU" + "\r");
                                    break;
                                case "8bits":
                                    // 8bits
                                    serialPort1.WriteLine("NBPC 8" + "\r");
                                    serialPort1.WriteLine("FMTU" + "\r");
                                    break;
                                case "10bits":
                                    // 10bits
                                    serialPort1.WriteLine("NBPC 10" + "\r");
                                    serialPort1.WriteLine("FMTU" + "\r");
                                    break;
                                case "12bits":
                                    // 12bits
                                    serialPort1.WriteLine("NBPC 12" + "\r");
                                    serialPort1.WriteLine("FMTU" + "\r");
                                    break;
                                default:
                                    break;
                            }
                            RedratLable.Text = DataGridView1.Rows[i].Cells[7].Value.ToString() + DataGridView1.Rows[i].Cells[8].Value.ToString(); 
                        }
                        #endregion

                        //Schedule開關DtPlay
                        else if (this.DataGridView1.Rows[i].Cells[0].Value.ToString() == "_dektec")
                        {
                            if (this.DataGridView1.Rows[i].Cells[4].Value.ToString() == "_start")
                            {
                            String arguments;

                            System.Diagnostics.Process Dektec = new System.Diagnostics.Process();
                                Dektec.StartInfo.FileName = Application.StartupPath + "\\DektecPlayer\\Dtplay.exe";

                            Dektec.StartInfo.UseShellExecute = false;
                            Dektec.StartInfo.RedirectStandardInput = true;
                            Dektec.StartInfo.RedirectStandardOutput = true;
                            Dektec.StartInfo.RedirectStandardError = true;
                                Dektec.StartInfo.CreateNoWindow = true;

                                arguments = Application.StartupPath + "\\DektecPlayer\\" + DataGridView1.Rows[i].Cells[6].Value.ToString() + " -mt " + DataGridView1.Rows[i].Cells[7].Value.ToString() + " -r 0 -l 0 -mf " + DataGridView1.Rows[i].Cells[8].Value.ToString();
                            Dektec.StartInfo.Arguments = arguments;
                            Dektec.Start();
                        }

                            if (this.DataGridView1.Rows[i].Cells[4].Value.ToString() == "_stop")
                            {
                                CloseDtplay();
                            }
                        }

                        else
                        {
                            // RC指令
                            if (ini12.INIRead(Application.StartupPath + "\\Config.ini", "RedRatDev", "value", "") == "1")
                            {
                                for (int k = 0; k < stime; k++)
                                {
                                    if (ini12.INIRead(Application.StartupPath + "\\Config.ini", "IR Device", "value", "") == "1")
                                    {
                                        autocommand(DataGridView1.Rows[i].Cells[0].Value.ToString()); //執行小紅鼠指令
                                        RedratLable.Text = DataGridView1.Rows[i].Cells[0].Value.ToString();
                                    }
                                    if (ini12.INIRead(Application.StartupPath + "\\Config.ini", "IR Device", "value", "") == "2")
                                    {
                                        myrsccmd(DataGridView1.Rows[i].Cells[0].Value.ToString()); //執TPSRC行指令
                                    }
                                    videostring = DataGridView1.Rows[i].Cells[0].Value.ToString();
                                    Thread.Sleep(sRepeat);
                                }
                            }
                            else
                            {
                                MessageBox.Show("Can't open redrat", "Redrat Open Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                       
                        Thread.Sleep(SysDelay);
                    }
                }
            }

            //結束
            MessageBox.Show("Auto Test finished!", "Status Information", MessageBoxButtons.OK, MessageBoxIcon.Information);

            if (jbutton1 == false)
            {
                UpdateUI("Start Capture", button1);
                UpdateUI("Setting", button8);
                button1.Enabled = true;
                button8.Enabled = true;
            }
            else
            {
                UpdateUI("Start Capture", button1);
                UpdateUI("Setting", button8);
                button8.Enabled = true;
                jbutton1 = false;
                close232();
                _captureInProgress = false;
                capture.Stop();
                capture.Dispose();
            }
        }

        private void MySrtCamd()
        { // 字幕檔執行緒
            int count = 1;
            string starttime = "0:0:0";
            TimeSpan time_start = TimeSpan.Parse(DateTime.Now.ToString("HH:mm:ss"));

            while (Vread)
            {
                System.Threading.Thread.Sleep(1000);
                TimeSpan time_end = TimeSpan.Parse(DateTime.Now.ToString("HH:mm:ss")); //計時結束 取得目前時間
                //後面的時間減前面的時間後 轉型成TimeSpan即可印出時間差
                string endtime = (time_end - time_start).Hours.ToString() + ":" + (time_end - time_start).Minutes.ToString() + ":" + (time_end - time_start).Seconds.ToString();
                StreamWriter srtWriter = new StreamWriter(srtstring, true);
                srtWriter.WriteLine(count);

                srtWriter.WriteLine(starttime + ",001" + " --> " + endtime + ",000");
                srtWriter.WriteLine(RedratLable.Text + "     " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                srtWriter.WriteLine("");
                srtWriter.WriteLine("");
                srtWriter.Close();
                count++;
                starttime = endtime;
            }
        }

        // 停止錄影 ===>
        private void mysstop()
        {
            this.Invoke(new EventHandler(delegate
            {
                capture.Stop();
                capture.Dispose();
                // 釋放記憶體
                GC.Collect();
                camstart();
            }));
        }

        // 錄影 ===>
        private void mysvideo()
        {
            this.Invoke(new EventHandler(delegate
            {
                savevideo();
            }));
        }

        protected void savevideo()
        {
            //存檔
            //儲存視頻副程式
            //IntPtr avi = this.imageBox1;
            string fName = "";
            // 設定INI檔路徑
            String sPath;
            sPath = Application.StartupPath + "\\Config.ini";

            // 讀取ini中的路徑
            fName = ini12.INIRead(sPath, "Video", "Path", "");
         
            string t = fName + "\\" + "_pvr" + DateTime.Now.ToString("yyyyMMddHHmmss") + "__" + label1.Text + ".wmv";
            srtstring = fName + "\\" + "_pvr" + DateTime.Now.ToString("yyyyMMddHHmmss") + "__" + label1.Text + ".srt";

            if (!capture.Cued)
                capture.Filename = t;
                capture.RecFileMode = DirectX.Capture.Capture.RecFileModeType.Wmv; //宣告我要wmv檔格式
                capture.Cue(); // 創一個檔
                capture.Start(); // 開始錄影

            double chd; //檢查HD 空間 小於100M就停止錄影s
            chd = Class1.ChDisk(Class1.Dkroot(fName));
            if (chd < 0.1)
            {
                Vread = false;
                MessageBox.Show("Check the HD Capacity!", "HD Capacity Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        // 錄影<=======

        private void camstart()
        {
            // 設定INI檔路徑
            int scam = 0;
            int saud = 0;
            string sPath = Application.StartupPath + "\\Config.ini";

            // 讀取ini中的路徑
            scam = int.Parse(ini12.INIRead(sPath, "camera", "value", ""));
            saud = int.Parse(ini12.INIRead(sPath, "audio", "value", ""));

#if DEBUG
            capture = new Capture(filters.VideoInputDevices[scam], filters.AudioInputDevices[saud]);
            capture.CaptureComplete += new EventHandler(OnCaptureComplete);
#endif

            if (capture == null)
                MessageBox.Show("Please select a video and/or audio device.", "Camera Status Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            if (capture.PreviewWindow == null)
            {
                capture.PreviewWindow = panelVideo;
                // mnuPreview.Checked = true;
            }
            else
            {
                capture.PreviewWindow = null;
            }
        }

        private void startcamera() //啟動攝影機
        {
            if (_captureInProgress)
            {
                //stop the capture
                button1.Text = "Start Capture";
                capture.Stop();
                capture.Dispose();
            }
            else
            {
                button1.Text = "Stop";

                try
                {
                    camstart();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Unable to enable/disable preview. Please submit a bug report.\n\n" + ex.Message + "\n\n" + ex.ToString());
                }
            }
            _captureInProgress = !_captureInProgress;
        }
        #endregion

        private void button1_Click(object sender, EventArgs e)
        {
            //啟動程式
            byte[] val;
            val = new byte[2];
            val[0] = 0;
            bool status;

            //讀取Autobox的設定值
            if (ini12.INIRead(Application.StartupPath + "\\Config.ini", "AutoboxDev", "value", "") == "1")
            {
                status = true;
            }
            else
            {
                MessageBox.Show("Autobox doesn't exist", "Autobox Open Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                status = false;
            }

            //讀取Redrat的設定值來顯示燈號
            if (ini12.INIRead(Application.StartupPath + "\\Config.ini", "RedratDev", "value", "") == "1")
            {
                pictureBox1.Image = Properties.Resources._01;
            }
            else
            {
                pictureBox1.Image = Properties.Resources._02;
            }

            //讀取Camera的設定值來顯示燈號
            if (ini12.INIRead(Application.StartupPath + "\\Config.ini", "CameraDev", "value", "") == "1")
            {
                pictureBox2.Image = Properties.Resources._01;
                filters = new Filters();
                startcamera();
            }
            else
            {
                pictureBox2.Image = Properties.Resources._02;
            }

            //讀取Redrat的設定值來顯示燈號
            if (ini12.INIRead(Application.StartupPath + "\\Config.ini", "RedratDev", "value", "") == "1")
            {
                pictureBox1.Image = Properties.Resources._01;
            }
            else
            {
                pictureBox1.Image = Properties.Resources._02;
            }

            if (status == true)
            {
                Thread oThreadB = new Thread(new ThreadStart(MyRunCamd));
                if (jbutton1 == true)
                {
                    //oThreadB.Abort();//停止執行緒
                    if (ini12.INIRead(Application.StartupPath + "\\Config.ini", "IR Device", "value", "") == "2")
                    {
                        close232(); // 關閉232
                    }
                    close232(); // 關閉rs232
                    button3.Enabled = false;
                    jbutton1 = false;
                    button1.Text = "Please";
                    button8.Text = "Wait";
                    button1.Enabled = false;
                    button8.Enabled = false;
                    CloseDtplay();
                    MessageBox.Show("Please wait delay time", "Waiting Loop Message", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                else
                {
                    if (ini12.INIRead(Application.StartupPath + "\\Config.ini", "IR Device", "value", "") == "2")
                    {
                        rsc_Rs232(); //啟動src的232
                    }

                    iniRs232(); //啟動RS232
                    button3.Enabled = true;
                    button8.Enabled = false;
                    oThreadB.Start();// 啟動執行緒
                    bool bSuccess = PL2303_GP1_GetValue(hCOM, val);
                    if (bSuccess)
                    {
                        if (val[0] == 0)
                        {
                            pbutton1 = false;
                            button3.Text = "Power ON";
                            pictureBox3.Image = Properties.Resources._02;
                        }
                        else
                            pictureBox3.Image = Properties.Resources._01;
                    }
                    jbutton1 = true;
                    button1.Text = "Stop";
                }
            }
        }

        private void button8_Click(object sender, EventArgs e)
        {
            //開啟設定檔
            Setting objSetting = new Setting();
            // objSetting.Show();
            button1.Enabled = false;
            button8.Enabled = false;
            if (objSetting.ShowDialog() == DialogResult.OK)
            {
                button1.Enabled = true;
                button8.Enabled = true;
                readSch();

                // 設定INI檔路徑
                String sPath;
                sPath = Application.StartupPath + "\\Config.ini";

                if (ini12.INIRead(sPath, "RedRatDev", "value", "") == "1")
                {
                    pictureBox1.Image = Properties.Resources._01;
                }
                else
                {
                    pictureBox1.Image = Properties.Resources._02;
                }

                if (ini12.INIRead(sPath, "CameraDev", "value", "") == "1")
                {
                    pictureBox2.Image = Properties.Resources._01;
                }
                else
                {
                    pictureBox2.Image = Properties.Resources._02;
                }
            }
            else
            {
                button1.Enabled = true;
                button8.Enabled = true;
            }
            objSetting.Dispose();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            // 電源開或關
            byte[] val1;
            val1 = new byte[2];
            val1[0] = 0;

            bool jSuccess = PL2303_GP0_Enalbe(hCOM, 1);
            if (jSuccess && pbutton1 == false)
            {
                uint val;
                val = (uint)int.Parse("1");
                bool bSuccess = PL2303_GP0_SetValue(hCOM, val);

            bool kbSuccess = PL2303_GP1_GetValue(hCOM, val1);

            if (kbSuccess)
            {
                if (val1[0] == 0)
                    {
                        pbutton1 = true;
                        button3.Text = "Power OFF";
                    pictureBox3.Image = Properties.Resources._01;
                    }
                else
                    pictureBox3.Image = Properties.Resources._02;
            }
        }
            else if (jSuccess && pbutton1 == true)
            {
                uint val;
                val = (uint)int.Parse("0");
                bool bSuccess = PL2303_GP0_SetValue(hCOM, val);

                bool kbSuccess = PL2303_GP1_GetValue(hCOM, val1);

                if (kbSuccess)
                {
                    //if (val1[0] == 0)
                    pbutton1 = false;
                    button3.Text = "Power ON";
                    pictureBox3.Image = Properties.Resources._02;
                    //else
                    //pictureBox3.Image = Properties.Resources._01;
                }
            }
            else
            {
                Log("GP0 output enable FAILED.");
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            readSch();
            //////////////////////////////////////////////////////////////////////////////////////跨執行緒
            Form1.CheckForIllegalCrossThreadCalls = false;
        }

        private void button5_Click(object sender, EventArgs e)
        {
            // 設定INI檔路徑
            String sPath;
            sPath = Application.StartupPath + "\\Config.ini";

            // 寫入Redrat啟動或關閉
            ini12.INIWrite(sPath, "RedRatDev", "value", "0");
            pictureBox1.Image = Properties.Resources._02;
        }

        private void button6_Click(object sender, EventArgs e)
        {
            // 設定INI檔路徑
            String sPath;
            sPath = Application.StartupPath + "\\Config.ini";

            // 寫入Camera啟動或關閉
            ini12.INIWrite(sPath, "CameraDev", "value", "0");
            pictureBox2.Image = Properties.Resources._02;
        }

        //系統時間
        private void timer1_Tick(object sender, EventArgs e)
        {
            TimeLable.Text = DateTime.Now.ToLongDateString() + "   " + DateTime.Now.ToLongTimeString();
        }

        private void CloseDtplay()
        {
            Process[] processes = Process.GetProcessesByName("DtPlay");

            foreach (Process p in processes)
            {
                p.Kill();
            }
        }

        //關閉DtPlay
        private void Form1_FormClosing(Object sender, FormClosingEventArgs e)
        {
            CloseDtplay();
        }
    }
}
