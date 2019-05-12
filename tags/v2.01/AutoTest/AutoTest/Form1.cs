﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Net.Mail;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using CheckDisk;
using DirectX.Capture;
using jini;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.Devices;
using Microsoft.Win32;
using Microsoft.Win32.SafeHandles;
using RedRat.AVDeviceMngmt;
using RedRat.IR;
using RedRat.RedRat3;
using RedRat.RedRat3.USB;
using RedRat.USB;
using RedRat.Util;
using USBClassLibrary;
using Excel = Microsoft.Office.Interop.Excel;

namespace AutoTest
{
    public partial class Form1 : Form
    {
        //private BackgroundWorker BackgroundWorker = new BackgroundWorker();
        private Form_DGV_Autobox Form_DGV_Autobox = new Form_DGV_Autobox();

        string sPath = Application.StartupPath + "\\Config.ini";
        string MailPath = Application.StartupPath + "\\Mail.ini";

        IRedRat3 redRat3 = null;
        private Capture capture = null;
        private Filters filters = null;
        private bool _captureInProgress;
        private bool jbutton1 = false;
        private bool pbutton1 = false;
        //private bool excelstat = false;
        bool Vread = false;
        bool TimerPanel = false;
        long timeCount = 0;
        string videostring = "";
        string srtstring = "";
        private Queue<byte> LogQueue1 = new Queue<byte>();
        private Queue<byte> LogQueue2 = new Queue<byte>();

        //Schedule暫停用的參數
        bool Pause = false;
        ManualResetEvent SchedulePause = new ManualResetEvent(true);
        ManualResetEvent ScheduleWait = new ManualResetEvent(true);

        private const int CS_DROPSHADOW = 0x20000;      //宣告陰影參數

        [DllImport("user32.dll")]       //拖動無窗體的控件>>>>>>>>>>>>>>
        public static extern bool ReleaseCapture();
        [DllImport("user32.dll")]
        public static extern bool SendMessage(IntPtr hwnd, int wMsg, int wParam, int lParam);
        public const int WM_SYSCOMMAND = 0x0112;
        public const int SC_MOVE = 0xF010;
        public const int HTCAPTION = 0x0002;        //<<<<<<<<<<<<<<<<<<<<<<<

        [DllImport("user32.dll")]
        public static extern bool SetForegroundWindow(IntPtr hWnd);

        public Form1()
        {
            InitializeComponent();

            USBPort = new USBClass();       //USB Connection>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
            USBDeviceProperties = new USBClass.DeviceProperties();
            USBPort.USBDeviceAttached += new USBClass.USBDeviceEventHandler(USBPort_USBDeviceAttached);
            USBPort.USBDeviceRemoved += new USBClass.USBDeviceEventHandler(USBPort_USBDeviceRemoved);
            USBPort.RegisterForDeviceChange(true, this);
            USBTryBoxConnection();
            USBTryRedratConnection();
            USBTryCameraConnection();
            MyUSBBoxDeviceConnected = false;
            MyUSBRedratDeviceConnected = false;
            MyUSBCameraDeviceConnected = false;     //<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<

            if (ini12.INIRead(sPath, "Comport", "PortName", "") == "")      //偵測comport>>>>>>>>>>>>>>
            {
                string[] DefaultCom = System.IO.Ports.SerialPort.GetPortNames();
                ini12.INIWrite(sPath, "Comport", "PortName", DefaultCom.Last());    //<<<<<<<<<<<<<<<<<
            }

            if (ini12.INIRead(Application.StartupPath + "\\Config.ini", "RedRatDev", "value", "") == "1")
                OpenRedRat3();
            else
                pictureBox1.Image = Properties.Resources._02;

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

            if (ini12.INIRead(Application.StartupPath + "\\Config.ini", "ExtComport", "value", "") == "1")
            {
                Com2Btn.Visible = true;
                Com2Btn.PerformClick();
            }
            else
                Com2Btn.Visible = false;

            if (ini12.INIRead(Application.StartupPath + "\\Config.ini", "Comport", "value", "") == "1")
            {
                Com1Btn.Visible = true;
                Com1Btn.PerformClick();
            }
            else
                Com1Btn.Visible = false;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //BackgroundWorker.DoWork += new DoWorkEventHandler(backgroundWorker_DoWork);

            ToolTip toolTip_Main = new ToolTip();   //物件提示>>>>>>>>>>>
            toolTip_Main.AutoPopDelay = 5000;
            toolTip_Main.InitialDelay = 500;
            toolTip_Main.ReshowDelay = 500;
            toolTip_Main.ShowAlways = true;
            toolTip_Main.SetToolTip(this.WriteBtn, "Save Schedule");
            toolTip_Main.SetToolTip(this.TimerPanelbutton, "On Time Schedule");
            toolTip_Main.SetToolTip(this.SchBtn1, "Schedule 1");
            toolTip_Main.SetToolTip(this.SchBtn2, "Schedule 2");
            toolTip_Main.SetToolTip(this.SchBtn3, "Schedule 3");
            toolTip_Main.SetToolTip(this.SchBtn4, "Schedule 4");
            toolTip_Main.SetToolTip(this.SchBtn5, "Schedule 5");        //<<<<<<<<<<<<<<<<<<

            LoadRCDB();

            List<string> SchExist = new List<string> { };
            for (int i = 2; i < 6; i++)
                SchExist.Add(ini12.INIRead(Application.StartupPath + "\\Config.ini", "Schedule" + i + " Exist", "value", ""));

            if (SchExist[0] == "0")
                SchBtn2.Visible = false;
            else
                SchBtn2.Visible = true;

            if (SchExist[1] == "0")
                SchBtn3.Visible = false;
            else
                SchBtn3.Visible = true;

            if (SchExist[2] == "0")
                SchBtn4.Visible = false;
            else
                SchBtn4.Visible = true;

            if (SchExist[3] == "0")
                SchBtn5.Visible = false;
            else
                SchBtn5.Visible = true;

            Global.Schedule_Num2_Exist = int.Parse(SchExist[0]);
            Global.Schedule_Num3_Exist = int.Parse(SchExist[1]);
            Global.Schedule_Num4_Exist = int.Parse(SchExist[2]);
            Global.Schedule_Num5_Exist = int.Parse(SchExist[3]);

            PauseButton.Enabled = false;
            SchBtn1.PerformClick();
            Com1Btn.PerformClick();
            //////////////////////////////////////////////////////////////////////////////////////跨執行緒
            Form1.CheckForIllegalCrossThreadCalls = false;
        }

        #region USB Detect
        /// <summary>
        /// Try to connect to the device.
        /// </summary>
        /// <returns>True if success, false otherwise</returns>

        private bool USBTryBoxConnection()
        {
            if (USBClass.GetUSBDevice(uint.Parse("05E3", System.Globalization.NumberStyles.AllowHexSpecifier), uint.Parse("0610", System.Globalization.NumberStyles.AllowHexSpecifier), ref USBDeviceProperties, false))
            {
                //My Device is attached
                BoxConnect();
                return true;
            }
            else
            {
                BoxDisconnect();
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
            if (
                USBClass.GetUSBDevice(uint.Parse("045E", System.Globalization.NumberStyles.AllowHexSpecifier), uint.Parse("0766", System.Globalization.NumberStyles.AllowHexSpecifier), ref USBDeviceProperties, false) ||
                USBClass.GetUSBDevice(uint.Parse("046D", System.Globalization.NumberStyles.AllowHexSpecifier), uint.Parse("081B", System.Globalization.NumberStyles.AllowHexSpecifier), ref USBDeviceProperties, false) ||
                USBClass.GetUSBDevice(uint.Parse("046D", System.Globalization.NumberStyles.AllowHexSpecifier), uint.Parse("0826", System.Globalization.NumberStyles.AllowHexSpecifier), ref USBDeviceProperties, false) ||
                USBClass.GetUSBDevice(uint.Parse("046D", System.Globalization.NumberStyles.AllowHexSpecifier), uint.Parse("082B", System.Globalization.NumberStyles.AllowHexSpecifier), ref USBDeviceProperties, false) ||
                USBClass.GetUSBDevice(uint.Parse("046D", System.Globalization.NumberStyles.AllowHexSpecifier), uint.Parse("082C", System.Globalization.NumberStyles.AllowHexSpecifier), ref USBDeviceProperties, false) ||
                USBClass.GetUSBDevice(uint.Parse("046D", System.Globalization.NumberStyles.AllowHexSpecifier), uint.Parse("082D", System.Globalization.NumberStyles.AllowHexSpecifier), ref USBDeviceProperties, false) ||
                USBClass.GetUSBDevice(uint.Parse("0C45", System.Globalization.NumberStyles.AllowHexSpecifier), uint.Parse("6340", System.Globalization.NumberStyles.AllowHexSpecifier), ref USBDeviceProperties, false) ||
                USBClass.GetUSBDevice(uint.Parse("114D", System.Globalization.NumberStyles.AllowHexSpecifier), uint.Parse("8C00", System.Globalization.NumberStyles.AllowHexSpecifier), ref USBDeviceProperties, false) ||
                USBClass.GetUSBDevice(uint.Parse("1BCF", System.Globalization.NumberStyles.AllowHexSpecifier), uint.Parse("2C4E", System.Globalization.NumberStyles.AllowHexSpecifier), ref USBDeviceProperties, false) ||
                USBClass.GetUSBDevice(uint.Parse("1E4E", System.Globalization.NumberStyles.AllowHexSpecifier), uint.Parse("0102", System.Globalization.NumberStyles.AllowHexSpecifier), ref USBDeviceProperties, false)
                )
            {
                //My Device is attached
                CameraConnect();
                return true;
            }
            else if (ini12.INIRead(Application.StartupPath + "\\Config.ini", "camera", "UserDefine", "") == "1")
            {
                string VID_Value = ini12.INIRead(Application.StartupPath + "\\Config.ini", "camera", "VID", "");
                string PID_Value = ini12.INIRead(Application.StartupPath + "\\Config.ini", "camera", "PID", "");
                if (USBClass.GetUSBDevice(uint.Parse(VID_Value, System.Globalization.NumberStyles.AllowHexSpecifier), uint.Parse(PID_Value, System.Globalization.NumberStyles.AllowHexSpecifier), ref USBDeviceProperties, false))
                {
                    CameraConnect();
                    return true;
                }
                else
                {
                    CameraDisconnect();
                    return false;
                }
            }
            else
            {
                CameraDisconnect();
                return false;
            }

        }

        private void USBPort_USBDeviceAttached(object sender, USBClass.USBDeviceEventArgs e)
        {
            if (!MyUSBBoxDeviceConnected)
            {
                if (USBTryBoxConnection())
                {
                    MyUSBBoxDeviceConnected = true;
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
                MyUSBBoxDeviceConnected = false;
                USBTryBoxConnection();
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

        private void BoxConnect()       //TO DO: Inset your connection code here
        {
            // 寫入Autobox關閉
            ini12.INIWrite(sPath, "AutoboxDev", "value", "1");
        }

        private void BoxDisconnect()        //TO DO: Insert your disconnection code here
        {
            // 寫入Autobox關閉
            ini12.INIWrite(sPath, "AutoboxDev", "value", "0");
        }

        private void RedratConnect()        //TO DO: Inset your connection code here
        {
            // 寫入Redrat啟動
            ini12.INIWrite(sPath, "RedRatDev", "value", "1");
            // 設定Redrat燈號
            pictureBox1.Image = Properties.Resources._01;
        }

        private void RedratDisconnect()     //TO DO: Insert your disconnection code here
        {
            // 寫入Redrat關閉
            ini12.INIWrite(sPath, "RedRatDev", "value", "0");
            // 設定Redrat燈號
            pictureBox1.Image = Properties.Resources._02;
        }

        private void CameraConnect()        //TO DO: Inset your connection code here
        {
            // 寫入Camera啟動
            ini12.INIWrite(sPath, "CameraDev", "value", "1");
            // 設定Camera燈號
            pictureBox2.Image = Properties.Resources._01;
            CamPreviewBtn.Enabled = true;
        }

        private void CameraDisconnect()     //TO DO: Insert your disconnection code here
        {
            // 寫入Camera關閉
            ini12.INIWrite(sPath, "CameraDev", "value", "0");
            // 設定Camera燈號
            pictureBox2.Image = Properties.Resources._02;
            CamPreviewBtn.Enabled = false;
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

        private bool PL2303_GP0_Enable(SafeFileHandle hDrv, uint enable)
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

        //執行緒控制 txtbox1
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

        //執行緒控制 txtbox2
        private delegate void UpdateUICallBack4(string value, Control ctl);
        private void txtbox2(string value, Control ctl)
        {
            if (this.InvokeRequired)
            {
                UpdateUICallBack4 uu = new UpdateUICallBack4(txtbox2);
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
            StartBtn.Enabled = false;
            capture.FrameEvent2 += new Capture.HeFrame(CaptureDone);
            capture.GrapImg();
        }

        // 複製原始圖片
        protected Bitmap CloneBitmap(Bitmap source)
        {
            return new Bitmap(source);
        }

        private void CaptureDone(System.Drawing.Bitmap e)
        {
            capture.FrameEvent2 -= new Capture.HeFrame(CaptureDone);
            string fName = ini12.INIRead(sPath, "Video", "Path", "");
            //string ngFolder = "Schedule" + Global.Schedule_Num + "_NG";

            // 讀取ini中的路徑
            Bitmap newBitmap = CloneBitmap(e);      // 圖片印字>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
            newBitmap = CloneBitmap(e);
            this.pictureBox4.Image = newBitmap;

            if (ini12.INIRead(Application.StartupPath + "\\Config.ini", "ComparePIC", "value", "") == "1")
            {
                // Create Compare folder
                string comparePath = ini12.INIRead(sPath, "ComparePIC", "Path", "");
                //string ngPath = fName + "\\" + ngFolder;
                string compareFile = comparePath + "\\" + "cf-" + Global.loop_Num + "_" + Global.caption_Num + ".png";
                if (Global.caption_Num == 0)
                    Global.caption_Num++;
/*
                if (Directory.Exists(ngPath))
                {

                }
                else
                {
                    Directory.CreateDirectory(ngPath);
                }
*/
                // 圖片比較
                
/*
                newBitmap = CloneBitmap(e);
                newBitmap = RGB2Gray(newBitmap);
                newBitmap = ConvertTo1Bpp2(newBitmap);
                newBitmap = SobelEdgeDetect(newBitmap);                
                this.pictureBox4.Image = newBitmap;
*/
                pictureBox4.Image.Save(compareFile);
                if (Global.loop_Num < 2)
                {

                }
                else
                {
                    Thread MyCompareThread = new Thread(new ThreadStart(MyCompareCamd));
                    MyCompareThread.Start();
                }
            }

            Graphics bitMap_g = Graphics.FromImage(this.pictureBox4.Image);//底圖
            Brush tb = new SolidBrush(Color.Red);
            Font textfont = new Font("微軟正黑體", 20, FontStyle.Bold);

            if (Global.Schedule_Step == 0)
                bitMap_g.DrawString(this.DataGridView1.Rows[Global.Schedule_Step].Cells[0].Value.ToString(), textfont, tb, new PointF(5, 400));
            else
                bitMap_g.DrawString(this.DataGridView1.Rows[Global.Schedule_Step - 1].Cells[0].Value.ToString(), textfont, tb, new PointF(5, 400));     //redrat command

            bitMap_g.DrawString(TimeLabel.Text, textfont, tb, new PointF(5, 440));      //現在時間

            textfont.Dispose();
            tb.Dispose();
            bitMap_g.Dispose();

            string t = fName + "\\" + "pic-" + DateTime.Now.ToString("yyyyMMddHHmmss") + "(" + label1.Text + "-" + Global.caption_Num + ").png";
            pictureBox4.Image.Save(t);      //<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<

            StartBtn.Enabled = true;
        }
        // 快照<========

        // 內存法
        public static Bitmap RGB2Gray(Bitmap srcBitmap)
        {
            Rectangle rect = new Rectangle(0, 0, srcBitmap.Width, srcBitmap.Height);
            System.Drawing.Imaging.BitmapData bmpdata = srcBitmap.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
            IntPtr ptr = bmpdata.Scan0;

            int bytes = srcBitmap.Width * srcBitmap.Height * 3;
            byte[] rgbvalues = new byte[bytes];

            System.Runtime.InteropServices.Marshal.Copy(ptr, rgbvalues, 0, bytes);

            double colortemp = 0;
            for (int i = 0; i < rgbvalues.Length; i += 3)
            {
                colortemp = rgbvalues[i + 2] * 0.299 + rgbvalues[i + 1] * 0.587 + rgbvalues[i] * 0.114;
                rgbvalues[i] = rgbvalues[i + 1] = rgbvalues[i + 2] = (byte)colortemp;
            }

            System.Runtime.InteropServices.Marshal.Copy(rgbvalues, 0, ptr, bytes);

            srcBitmap.UnlockBits(bmpdata);
            return (srcBitmap);
        }  

        // Sobel法 
        private Bitmap SobelEdgeDetect(Bitmap original)
        {
            Bitmap b = original;
            Bitmap bb = original;
            int width = b.Width;
            int height = b.Height;
            int[,] gx = new int[,] { { -1, 0, 1 }, { -2, 0, 2 }, { -1, 0, 1 } };
            int[,] gy = new int[,] { { 1, 2, 1 }, { 0, 0, 0 }, { -1, -2, -1 } };

            int[,] allPixR = new int[width, height];
            int[,] allPixG = new int[width, height];
            int[,] allPixB = new int[width, height];

            int limit = 128 * 128;

            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    allPixR[i, j] = b.GetPixel(i, j).R;
                    allPixG[i, j] = b.GetPixel(i, j).G;
                    allPixB[i, j] = b.GetPixel(i, j).B;
                }
            }

            int new_rx = 0, new_ry = 0;
            int new_gx = 0, new_gy = 0;
            int new_bx = 0, new_by = 0;
            int rc, gc, bc;
            for (int i = 1; i < b.Width - 1; i++)
            {
                for (int j = 1; j < b.Height - 1; j++)
                {

                    new_rx = 0;
                    new_ry = 0;
                    new_gx = 0;
                    new_gy = 0;
                    new_bx = 0;
                    new_by = 0;
                    rc = 0;
                    gc = 0;
                    bc = 0;

                    for (int wi = -1; wi < 2; wi++)
                    {
                        for (int hw = -1; hw < 2; hw++)
                        {
                            rc = allPixR[i + hw, j + wi];
                            new_rx += gx[wi + 1, hw + 1] * rc;
                            new_ry += gy[wi + 1, hw + 1] * rc;

                            gc = allPixG[i + hw, j + wi];
                            new_gx += gx[wi + 1, hw + 1] * gc;
                            new_gy += gy[wi + 1, hw + 1] * gc;

                            bc = allPixB[i + hw, j + wi];
                            new_bx += gx[wi + 1, hw + 1] * bc;
                            new_by += gy[wi + 1, hw + 1] * bc;
                        }
                    }
                    if (new_rx * new_rx + new_ry * new_ry > limit || new_gx * new_gx + new_gy * new_gy > limit || new_bx * new_bx + new_by * new_by > limit)
                        bb.SetPixel(i, j, Color.Black);

                    //bb.SetPixel (i, j, Color.FromArgb(allPixR[i,j],allPixG[i,j],allPixB[i,j]));
                    else
                        bb.SetPixel(i, j, Color.Transparent);
                }
            }
            return bb;
        }

        public static bool ImageCompareString(Bitmap firstImage, Bitmap secondImage)
        {
            MemoryStream ms = new MemoryStream();
            firstImage.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
            String firstBitmap = Convert.ToBase64String(ms.ToArray());
            ms.Position = 0;
            secondImage.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
            String secondBitmap = Convert.ToBase64String(ms.ToArray());
            if (firstBitmap.Equals(secondBitmap))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// 圖片內容比較1
        /// Refer: http://www.programmer-club.com.tw/ShowSameTitleN/csharp/9880.html
        public float Similarity(System.Drawing.Bitmap img1, System.Drawing.Bitmap img2)
        {
            int rc, bc, gc;
            float cc = 0, hc = 0;

            for (int i = 0; i < img1.Size.Width; i++)
            {
                for (int j = 0; j < img1.Size.Height; j++)
                {
                    System.Drawing.Color c1 = img1.GetPixel(i, j);
                    System.Drawing.Color c2 = img2.GetPixel(i, j);

                    rc = Math.Abs(c1.R - c2.R);
                    bc = Math.Abs(c1.B - c2.B);
                    gc = Math.Abs(c1.G - c2.G);
                    cc = (float)(rc + bc + gc);

                    float f1 = (float)(255 * 3 * img1.Size.Width * img1.Size.Height);
                    hc += cc / f1;
                }
            }
            hc = hc * 100;
            return hc;
        }

        // GetHisogram 取long
        public long[] GetHistogram(System.Drawing.Bitmap picture)
        {
            long[] myHistogram = new long[256];

            for (int i = 0; i < picture.Size.Width; i++)
                for (int j = 0; j < picture.Size.Height; j++)
                {
                    System.Drawing.Color c = picture.GetPixel(i, j);

                    long Temp = 0;
                    Temp += c.R;
                    Temp += c.G;
                    Temp += c.B;

                    Temp = (int)Temp / 3;
                    myHistogram[Temp]++;
                }

            return myHistogram;
        }

        // GetHisogram 取int
        public int[] GetHisogram(Bitmap img)
        {
            BitmapData data = img.LockBits(new System.Drawing.Rectangle(0, 0, img.Width, img.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);          
            int[] histogram = new int[256];
            unsafe
            {
                byte* ptr = (byte*)data.Scan0;
                int remain = data.Stride - data.Width * 3;
                for (int i = 0; i < histogram.Length; i++)
                    histogram[i] = 0;
                for (int i = 0; i < data.Height; i++)
                {
                    for (int j = 0; j < data.Width; j++)
                    {
                        int mean = ptr[0] + ptr[1] + ptr[2];
                        mean /= 3;
                        histogram[mean]++;
                        ptr += 3;
                    }
                    ptr += remain;
                }
            }
            img.UnlockBits(data);
            return histogram;
        }

        //計算相減後的絕對值
        private float GetAbs(int firstNum, int secondNum)
        {
            float abs = Math.Abs((float)firstNum - (float)secondNum);
            float result = Math.Max(firstNum, secondNum);
            if (result == 0)
                result = 1;
            return abs / result;
        }

        //最終計算結果
        public float GetResult(int[] firstNum, int[] scondNum)
        {
            if (firstNum.Length != scondNum.Length)
            {
                return 0;
            }
            else
            {
                float result = 0;
                int j = firstNum.Length;
                for (int i = 0; i < j; i++)
                {
                    result += 1 - GetAbs(firstNum[i], scondNum[i]);
                }
                return result / j;
            }
        }

		/// <summary>
        /// 判断图形里是否存在另外一个图形 并返回所在位置
        /// </summary>
        /// <param name=”p_SourceBitmap”>原始图形</param>
        /// <param name=”p_PartBitmap”>小图形</param>
        /// <param name=”p_Float”>溶差</param>
        /// <returns>坐标</returns>
        public Point GetImageContains(Bitmap p_SourceBitmap, Bitmap p_PartBitmap, int p_Float)
        {
            int _SourceWidth = p_SourceBitmap.Width;
            int _SourceHeight = p_SourceBitmap.Height;
            int _PartWidth = p_PartBitmap.Width;
            int _PartHeight = p_PartBitmap.Height;
            Bitmap _SourceBitmap = new Bitmap(_SourceWidth, _SourceHeight);
            Graphics _Graphics = Graphics.FromImage(_SourceBitmap);
            _Graphics.DrawImage(p_SourceBitmap, new Rectangle(0, 0, _SourceWidth, _SourceHeight));
            _Graphics.Dispose();
            BitmapData _SourceData = _SourceBitmap.LockBits(new Rectangle(0, 0, _SourceWidth, _SourceHeight), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
            byte[] _SourceByte = new byte[_SourceData.Stride * _SourceHeight];
            Marshal.Copy(_SourceData.Scan0, _SourceByte, 0, _SourceByte.Length);  //复制出p_SourceBitmap的相素信息
            _SourceBitmap.UnlockBits(_SourceData);
            Bitmap _PartBitmap = new Bitmap(_PartWidth, _PartHeight);
            _Graphics = Graphics.FromImage(_PartBitmap);
            _Graphics.DrawImage(p_PartBitmap, new Rectangle(0, 0, _PartWidth, _PartHeight));
            _Graphics.Dispose();
            BitmapData _PartData = _PartBitmap.LockBits(new Rectangle(0, 0, _PartWidth, _PartHeight), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
            byte[] _PartByte = new byte[_PartData.Stride * _PartHeight];
            Marshal.Copy(_PartData.Scan0, _PartByte, 0, _PartByte.Length);   //复制出p_PartBitmap的相素信息
            _PartBitmap.UnlockBits(_PartData);
            for (int i = 0; i != _SourceHeight; i++)
            {
                if (_SourceHeight - i < _PartHeight) return new Point(-1, -1);  //如果 剩余的高 比需要比较的高 还要小 就直接返回
                int _PointX = -1;    //临时存放坐标 需要包正找到的是在一个X点上
                bool _SacnOver = true;   //是否都比配的上
                for (int z = 0; z != _PartHeight - 1; z++)       //循环目标进行比较
                {
                    int _TrueX = GetImageContains(_SourceByte, _PartByte, (i + z) * _SourceData.Stride, z * _PartData.Stride, _SourceWidth, _PartWidth, p_Float);
                    if (_TrueX == -1)   //如果没找到
                    {
                        _PointX = -1;    //设置坐标为没找到
                        _SacnOver = false;   //设置不进行返回
                        break;
                    }
                    else
                    {
                        if (z == 0) _PointX = _TrueX;
                        if (_PointX != _TrueX)   //如果找到了 也的保证坐标和上一行的坐标一样 否则也返回
                        {
                            _PointX = -1;//设置坐标为没找到
                            _SacnOver = false;  //设置不进行返回
                            break;
                        }
                    }
                }
                if (_SacnOver) return new Point(_PointX, i);
            }
            return new Point(-1, -1);
        }

        /// <summary>
        /// 判断图形里是否存在另外一个图形 所在行的索引
        /// </summary>
        /// <param name=”p_Source”>原始图形数据</param>
        /// <param name=”p_Part”>小图形数据</param>
        /// <param name=”p_SourceIndex”>开始位置</param>
        /// <param name=”p_SourceWidth”>原始图形宽</param>
        /// <param name=”p_PartWidth”>小图宽</param>
        /// <param name=”p_Float”>溶差</param>
        /// <returns>所在行的索引 如果找不到返回-1</returns>
        private int GetImageContains(byte[] p_Source, byte[] p_Part, int p_SourceIndex, int p_PartIndex, int p_SourceWidth, int p_PartWidth, int p_Float)
        {
            int _PartIndex = p_PartIndex;//
            int _PartRVA = _PartIndex;//p_PartX轴起点
            int _SourceIndex = p_SourceIndex;//p_SourceX轴起点
            for (int i = 0; i < p_SourceWidth; i++)
            {
                if (p_SourceWidth - i < p_PartWidth) return -1;
                Color _CurrentlyColor = Color.FromArgb((int)p_Source[_SourceIndex + 3], (int)p_Source[_SourceIndex + 2], (int)p_Source[_SourceIndex + 1], (int)p_Source[_SourceIndex]);
                Color _CompareColoe = Color.FromArgb((int)p_Part[_PartRVA + 3], (int)p_Part[_PartRVA + 2], (int)p_Part[_PartRVA + 1], (int)p_Part[_PartRVA]);
                _SourceIndex += 4;//成功，p_SourceX轴加4
                bool _ScanColor = ScanColor(_CurrentlyColor, _CompareColoe, p_Float);
                if (_ScanColor)
                {
                    _PartRVA += 4;//成功，p_PartX轴加4
                    int _SourceRVA = _SourceIndex;
                    bool _Equals = true;
                    for (int z = 0; z != p_PartWidth - 1; z++)
                    {
                        _CurrentlyColor = Color.FromArgb((int)p_Source[_SourceRVA + 3], (int)p_Source[_SourceRVA + 2], (int)p_Source[_SourceRVA + 1], (int)p_Source[_SourceRVA]);
                        _CompareColoe = Color.FromArgb((int)p_Part[_PartRVA + 3], (int)p_Part[_PartRVA + 2], (int)p_Part[_PartRVA + 1], (int)p_Part[_PartRVA]);
                        if (!ScanColor(_CurrentlyColor, _CompareColoe, p_Float))
                        {
                            _PartRVA = _PartIndex;//失败，重置p_PartX轴开始
                            _Equals = false;
                            break;
                        }
                        _PartRVA += 4;//成功，p_PartX轴加4
                        _SourceRVA += 4;//成功，p_SourceX轴加4
                    }
                    if (_Equals) return i;
                }
                else
                {
                    _PartRVA = _PartIndex;//失败，重置p_PartX轴开始
                }
            }
            return -1;
        }

        /// <summary>
        /// 检查色彩(可以根据这个更改比较方式
        /// </summary>
        /// <param name=”p_CurrentlyColor”>当前色彩</param>
        /// <param name=”p_CompareColor”>比较色彩</param>
        /// <param name=”p_Float”>溶差</param>
        /// <returns></returns>
        private bool ScanColor(Color p_CurrentlyColor, Color p_CompareColor, int p_Float)
        {
            int _R = p_CurrentlyColor.R;
            int _G = p_CurrentlyColor.G;
            int _B = p_CurrentlyColor.B;
            return (_R <= p_CompareColor.R + p_Float && _R >= p_CompareColor.R - p_Float) && (_G <= p_CompareColor.G + p_Float && _G >= p_CompareColor.G - p_Float) && (_B <= p_CompareColor.B + p_Float && _B >= p_CompareColor.B - p_Float);
        }

        /// <summary>
        /// 图像二值化1：取图片的平均灰度作为阈值，低于该值的全都为0，高于该值的全都为255
        /// </summary>
        /// <param name="bmp"></param>
        /// <returns></returns>
        public static Bitmap ConvertTo1Bpp1(Bitmap bmp)
        {
            int average = 0;
            for (int i = 0; i < bmp.Width; i++)
            {
                for (int j = 0; j < bmp.Height; j++)
                {
                    Color color = bmp.GetPixel(i, j);
                    average += color.B;
                }
            }
            average = (int)average / (bmp.Width * bmp.Height);

            for (int i = 0; i < bmp.Width; i++)
            {
                for (int j = 0; j < bmp.Height; j++)
                {
                    //获取该点的像素的RGB的颜色
                    Color color = bmp.GetPixel(i, j);
                    int value = 255 - color.B;
                    Color newColor = value > average ? Color.FromArgb(0, 0, 0) : Color.FromArgb(255, 255, 255);
                    bmp.SetPixel(i, j, newColor);
                }
            }
            return bmp;
        }

        /// <summary>
        /// 图像二值化2
        /// </summary>
        /// <param name="img"></param>
        /// <returns></returns>
        public static Bitmap ConvertTo1Bpp2(Bitmap img)
        {
            int w = img.Width;
            int h = img.Height;
            Bitmap bmp = new Bitmap(w, h, PixelFormat.Format1bppIndexed);
            BitmapData data = bmp.LockBits(new Rectangle(0, 0, w, h), ImageLockMode.ReadWrite, PixelFormat.Format1bppIndexed);
            for (int y = 0; y < h; y++)
            {
                byte[] scan = new byte[(w + 7) / 8];
                for (int x = 0; x < w; x++)
                {
                    Color c = img.GetPixel(x, y);
                    if (c.GetBrightness() >= 0.5) scan[x / 8] |= (byte)(0x80 >> (x % 8));
                }
                Marshal.Copy(scan, 0, (IntPtr)((int)data.Scan0 + data.Stride * y), scan.Length);
            }
            return bmp;
        }

        /// <summary>
        /// 圖片內容比較2-1
        /// Refer: http://fecbob.pixnet.net/blog/post/38125033-c%23-%E5%9C%96%E7%89%87%E5%85%A7%E5%AE%B9%E6%AF%94%E8%BC%83
        /// </summary>
        /// <param name="img"></param>
        /// <returns></returns>
        public struct RGBdata
        {
            public int r;
            public int g;
            public int b;

            public int GetLargest()
            {
                if (r > b)
                {
                    if (r > g)
                    {
                        return 1;
                    }
                    else
                    {
                        return 2;
                    }
                }
                else
                {
                    return 3;
                }
            }
        }

        /// <summary>
        /// 圖片內容比較2-2
        /// Refer: http://fecbob.pixnet.net/blog/post/38125033-c%23-%E5%9C%96%E7%89%87%E5%85%A7%E5%AE%B9%E6%AF%94%E8%BC%83
        /// </summary>
        /// <param name="img"></param>
        /// <returns></returns>
        private RGBdata ProcessBitmap(Bitmap a)
        {
            BitmapData bmpData = a.LockBits(new Rectangle(0, 0, a.Width, a.Height), ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
            IntPtr ptr = bmpData.Scan0;
            RGBdata data = new RGBdata();

            unsafe
            {
                byte* p = (byte*)(void*)ptr;
                int offset = bmpData.Stride - a.Width * 3;
                int width = a.Width * 3;
                for (int y = 0; y < a.Height; ++y)
                {
                    for (int x = 0; x < width; ++x)
                    {
                        data.r += p[0];             //gets red values
                        data.g += p[1];             //gets green values
                        data.b += p[2];             //gets blue values
                        ++p;
                    }
                    p += offset;
                }
            }
            a.UnlockBits(bmpData);
            return data;
        }

        /// <summary>
        /// 圖片內容比較2-3
        /// Refer: http://fecbob.pixnet.net/blog/post/38125033-c%23-%E5%9C%96%E7%89%87%E5%85%A7%E5%AE%B9%E6%AF%94%E8%BC%83
        /// </summary>
        /// <param name="img"></param>
        /// <returns></returns>
        public double GetSimilarity(Bitmap a, Bitmap b)
        {
            RGBdata dataA = ProcessBitmap(a);
            RGBdata dataB = ProcessBitmap(b);
            double result = 0;
            int averageA = 0;
            int averageB = 0;
            int maxA = 0;
            int maxB = 0;
            maxA = ((a.Width * 3) * a.Height);
            maxB = ((b.Width * 3) * b.Height);

            switch (dataA.GetLargest())            //Find dominant color to compare
            {
                case 1:
                    {
                        averageA = Math.Abs(dataA.r / maxA);
                        averageB = Math.Abs(dataB.r / maxB);
                        result = (averageA - averageB) / 2;
                        break;
                    }
                case 2:
                    {
                        averageA = Math.Abs(dataA.g / maxA);
                        averageB = Math.Abs(dataB.g / maxB);
                        result = (averageA - averageB) / 2;
                        break;
                    }
                case 3:
                    {
                        averageA = Math.Abs(dataA.b / maxA);
                        averageB = Math.Abs(dataB.b / maxB);
                        result = (averageA - averageB) / 2;
                        break;
                    }
            }

            result = Math.Abs((result + 100) / 100);
            if (result > 1.0)
            {
                result -= 1.0;
            }

            return result;
        }

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

        //讀取RC xml檔並且丟給電視
        protected void autocommand(string SigData)
        {
            // 執行動作
            AVDeviceDB newAVDeviceDB;

            // 戴入Schedule CSV 檔
            string fName = "";
            string redcon = "";

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

            // 假若設定值大於目前device個數，直接更改為目前device個數
            if (dev >= devices.Length)
            {
                dev = devices.Length - 1;
            }

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

        // 建立RS232 Port1
        protected void iniRs232()
        { //設定com port
            string stopbit = "";

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
            if (ini12.INIRead(sPath, "AutoboxDev", "value", "") == "1")
            {
                hCOM = (SafeFileHandle)stream.GetType().GetField("_handle", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(stream);
            }
        }

        // 關閉RS232 Port1
        protected void close232()
        {// 關閉232
            serialPort1.Close();
        }

        // 建立RS232 Port2
        protected void ExtiniRs232()
        { //設定com port
            string stopbit = "";

            // 讀取ini中的路徑
            stopbit = ini12.INIRead(sPath, "ExtComport", "StopBits", "");
            switch (stopbit)
            {
                case "One":
                    serialPort2.StopBits = System.IO.Ports.StopBits.One;
                    break;
                case "Two":
                    serialPort2.StopBits = System.IO.Ports.StopBits.Two;
                    break;
            }
            serialPort2.PortName = ini12.INIRead(sPath, "ExtComport", "PortName", "");
            serialPort2.BaudRate = int.Parse(ini12.INIRead(sPath, "ExtComport", "BaudRate", ""));
            // serialPort2.Encoding = System.Text.Encoding.GetEncoding(1252);

            serialPort2.Open();
            object stream = typeof(SerialPort).GetField("internalSerialStream", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(serialPort2);
        }

        // 關閉RS232 Port2
        protected void Extclose232()
        {// 關閉232
            serialPort2.Close();
        }

        //啟動SRC的Com port連緒
        protected void rsc_Rs232()
        {
            string stopbit = "";

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
            // serialPort2.Encoding = System.Text.Encoding.GetEncoding(1252);

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
                //textBox1.Text = msg.Trim();
                serialPort1.WriteLine(msg.Trim());
            }));
        }

        private void ExtLog(string msg)
        {
            textBox2.Invoke(new EventHandler(delegate
            {
                //textBox2.Text = msg.Trim();
                serialPort2.WriteLine(msg.Trim());
            }));
        }

        private void serialPort1_DataReceived(object sender, System.IO.Ports.SerialDataReceivedEventArgs e)     //接受com1傳來的資料>>>>>>>>>>>>
        {
            byte[] buff = null;
            int i = 0;

            buff = new byte[serialPort1.BytesToRead];

            serialPort1.Read(buff, 0, buff.Length);

            while (i < buff.Length)
            {
                LogQueue1.Enqueue(buff[i]);
                i++;
            }

            string text = Encoding.ASCII.GetString(buff);

            serialPort1.DiscardInBuffer();
            serialPort1.DiscardOutBuffer();

            textBox1.AppendText(text);
        }

        private void serialPort2_DataReceived(object sender, System.IO.Ports.SerialDataReceivedEventArgs e)     //接受com2傳來的資料>>>>>>>>>>>>
        {
            byte[] buff = null;
            int i = 0;

            buff = new byte[serialPort2.BytesToRead];

            serialPort2.Read(buff, 0, buff.Length);

            while (i < buff.Length)
            {
                LogQueue2.Enqueue(buff[i]);
                i++;
            }

            string text = Encoding.ASCII.GetString(buff);

            serialPort2.DiscardInBuffer();
            serialPort2.DiscardOutBuffer();

            textBox2.AppendText(text);
        }

        private void Rs232save()        //儲存com1的log>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
        {
            string fName = "";

            // 讀取ini中的路徑
            fName = ini12.INIRead(sPath, "Log", "Path", "");
            string t = fName + "\\_Log1_" + DateTime.Now.ToString("yyyyMMddHHmmss") + "_" + label1.Text + ".txt";

            StreamWriter MYFILE = new StreamWriter(t, false, Encoding.ASCII);
            while (LogQueue1.Count > 0)
            {
                char temp_char;
                byte temp_byte;

                temp_byte = LogQueue1.Dequeue();
                temp_char = (char)temp_byte;

                MYFILE.Write(temp_char);
            }
            MYFILE.Close();
            txtbox1("", textBox1);
        }

        private void ExtRs232save()     //儲存com2的log>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
        {
            string fName = "";

            // 讀取ini中的路徑
            fName = ini12.INIRead(sPath, "Log", "Path", "");
            string t = fName + "\\_Log2_" + DateTime.Now.ToString("yyyyMMddHHmmss") + "_" + label1.Text + ".txt";

            StreamWriter MYFILE = new StreamWriter(t, false, Encoding.ASCII);
            while (LogQueue2.Count > 0)
            {
                char temp_char;
                byte temp_byte;

                temp_byte = LogQueue2.Dequeue();
                temp_char = (char)temp_byte;

                MYFILE.Write(temp_char);
            }
            MYFILE.Close();
            txtbox2("", textBox2);
        }

        protected override void OnPaint(PaintEventArgs e)         
        {             
            try             
            {                 
                base.OnPaint(e);             
            }             
            catch             
            {                 
                Invalidate();             
            }         
        }     

        #region 跑Schedule的指令集
        public void MyRunCamd()
        {
            // 執行緒
            int sRepeat = 0;
            int stime = 0;
            int SysDelay = 0;

            Global.loop_Num = 1;
            Global.break_sch = 0;

            for (int l = 0; l <= Global.Schedule_Loop; l++)
            {
                Global.NGValue[l] = 0;
                Global.NGRateValue[l] = 0;
            }
            
            #region 匯出比對結果到CSV & EXCEL
            if (ini12.INIRead(Application.StartupPath + "\\Config.ini", "ComparePIC", "value", "") == "1" && jbutton1 == true)
            {
                string compareFolder = ini12.INIRead(sPath, "Video", "Path", "") + "\\" + "Schedule" + Global.Schedule_Num + "_Original_" + DateTime.Now.ToString("yyyyMMddHHmmss");

                if (Directory.Exists(compareFolder))
                {

                }
                else
                {
                    Directory.CreateDirectory(compareFolder);
                    ini12.INIWrite(sPath, "ComparePIC", "Path", compareFolder);
                }
                // 匯出csv記錄檔
                string csvFile = ini12.INIRead(sPath, "ComparePIC", "Path", "") + "\\SimilarityReport_" + Global.Schedule_Num + ".csv";
                StreamWriter sw = new StreamWriter(csvFile, false, Encoding.UTF8);
                sw.WriteLine("Target, Source, Similarity, Sub-NG count, NGRate, Result");
                
                sw.Dispose();
/*
                #region Excel function
                // 匯出excel記錄檔
                Global.excel_Num = 1;
                string excelFile = ini12.INIRead(sPath, "ComparePIC", "Path", "") + "\\SimilarityReport_" + Global.Schedule_Num;

                excelApp = new Excel.Application();
                //excelApp.Visible = true;
                excelApp.DisplayAlerts = false;
                excelApp.Workbooks.Add(Type.Missing);
                wBook = excelApp.Workbooks[1];
                wBook.Activate();
                excelstat = true;

                try
                {
                    // 引用第一個工作表
                    wSheet = (Excel._Worksheet)wBook.Worksheets[1];

                    // 命名工作表的名稱
                    wSheet.Name = "全部測試資料";

                    // 設定工作表焦點
                    wSheet.Activate();

                    excelApp.Cells[1, 1] = "All Data";

                    // 設定第1列資料
                    excelApp.Cells[1, 1] = "Target";
                    excelApp.Cells[1, 2] = "Source";
                    excelApp.Cells[1, 3] = "Similarity";
                    excelApp.Cells[1, 4] = "Sub-NG count";
                    excelApp.Cells[1, 5] = "NGRate";
                    excelApp.Cells[1, 6] = "Result";
                    // 設定第1列顏色
                    wRange = wSheet.Range[wSheet.Cells[1, 1], wSheet.Cells[1, 6]];
                    wRange.Select();
                    wRange.Font.Color = ColorTranslator.ToOle(Color.White);
                    wRange.Interior.Color = ColorTranslator.ToOle(Color.DimGray);
                    wRange.AutoFilter(1, Type.Missing, Microsoft.Office.Interop.Excel.XlAutoFilterOperator.xlAnd, Type.Missing);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("產生報表時出錯！" + Environment.NewLine + ex.Message);
                }
                #endregion
*/
            }
            #endregion

            for (int j = 1; j < Global.Schedule_Loop + 1; j++)
            {
                Global.caption_Num = 0;
                
                //Global.caption_Total_Num[Global.Schedule_Loop] = 0;
                UpdateUI(j.ToString(), label1);
                int V_scroll = 1;
                ini12.INIWrite(MailPath, "Data Info", "CreateTime", string.Format("{0:R}", DateTime.Now));

                lock (this)
                {
                    for (int i = 0; i < this.DataGridView1.Rows.Count - 1; i++)
                    {
                        Global.Schedule_Step = i;
                        if (jbutton1 == false)
                        {
                            Console.WriteLine("break1");
                            j = Global.Schedule_Loop;
                            UpdateUI(j.ToString(), label1);
                            break;
                        }
                        
                        gridUI(i.ToString(), DataGridView1);

                        if (V_scroll < DataGridView1.Rows[1].Height)
                            V_scroll = i;
                        else
                            V_scroll = 0;

                        gridscroll(V_scroll.ToString(), DataGridView1);

                        if (this.DataGridView1.Rows[i].Cells[1].Value.ToString() != "")
                            stime = int.Parse(this.DataGridView1.Rows[i].Cells[1].Value.ToString()); // 次數
                        else
                            stime = 1;

                        if (this.DataGridView1.Rows[i].Cells[2].Value.ToString() != "")
                            sRepeat = int.Parse(this.DataGridView1.Rows[i].Cells[2].Value.ToString()); // 停止時間
                        else
                            sRepeat = 0;

                        if (this.DataGridView1.Rows[i].Cells[9].Value.ToString() != "")
                            SysDelay = int.Parse(this.DataGridView1.Rows[i].Cells[9].Value.ToString()); // 指令停止時間
                        else
                            SysDelay = 0;

                        if (this.DataGridView1.Rows[i].Cells[0].Value.ToString() == "_cmd")
                        {
                            #region 拍照
                            if (DataGridView1.Rows[i].Cells[3].Value.ToString() == "_shot")
                            {
                                if (ini12.INIRead(Application.StartupPath + "\\Config.ini", "CameraDev", "value", "") == "1")
                                {
                                    Global.caption_Num++;
                                    if (Global.loop_Num == 1)
                                        Global.caption_Sum = Global.caption_Num;
                                    jes();
                                    RedratLable.Text = "Take Picture";
                                }
                                else
                                {
                                    StartBtn.PerformClick();
                                }
                            }
                            #endregion

                            #region 錄影
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
                                    }
                                    RedratLable.Text = "Start Recording";
                                }
                                else
                                {
                                    MessageBox.Show("Camera not exist", "Camera Open Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    StartBtn.PerformClick();
                                }
                            }

                            if (DataGridView1.Rows[i].Cells[4].Value.ToString() == "_stop")
                            {
                                if (ini12.INIRead(Application.StartupPath + "\\Config.ini", "CameraDev", "value", "") == "1")
                                {
                                    if (Vread == true)       //判斷是不是正在錄影
                                    {
                                        Vread = false;
                                        mysstop();      //先將先前的關掉
                                    }
                                    RedratLable.Text = "Stop Recording";
                                }
                                else
                                {
                                    MessageBox.Show("Camera not exist", "Camera Open Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    StartBtn.PerformClick();
                                }
                            }
                            #endregion

                            #region AC ON/OFF
                            if (DataGridView1.Rows[i].Cells[6].Value.ToString() == "_on")
                            {
                                if (ini12.INIRead(Application.StartupPath + "\\Config.ini", "AutoboxDev", "value", "") == "1")
                                {
                                    byte[] val1;
                                    val1 = new byte[2];
                                    val1[0] = 0;

                                    bool jSuccess = PL2303_GP0_Enable(hCOM, 1);
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
                                            //if (val1[0] == 0)
                                            {
                                                pbutton1 = true;
                                                pictureBox3.Image = Properties.Resources._01;
                                            }
                                            //else
                                            //pictureBox3.Image = Properties.Resources._02;
                                        }

                                        //if (bSuccess)
                                        //   Log("SET GP0 value Successfully!");
                                        //else
                                        //    Log("SET GP0 value FAILED.");
                                    }
                                    RedratLable.Text = "AC ON";
                                }
                                else
                                {
                                    MessageBox.Show("Can't open autobox", "Autobox Open Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                }
                            }

                            if (DataGridView1.Rows[i].Cells[6].Value.ToString() == "_off")
                            {
                                if (ini12.INIRead(Application.StartupPath + "\\Config.ini", "AutoboxDev", "value", "") == "1")
                                {
                                    byte[] val1;
                                    val1 = new byte[2];
                                    val1[0] = 0;

                                    bool jSuccess = PL2303_GP0_Enable(hCOM, 1);
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
                                            {
                                                pbutton1 = false;
                                                pictureBox3.Image = Properties.Resources._02;
                                            }
                                            //else
                                            //pictureBox3.Image = Properties.Resources._01;
                                        }

                                        //if (bSuccess)
                                        //    Log("SET GP0 value Successfully!");
                                        //else
                                        //    Log("SET GP0 value FAILED.");
                                    }
                                    RedratLable.Text = "AC OFF";
                                }
                                else
                                {
                                    MessageBox.Show("Can't open autobox", "Autobox Open Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                }
                            }
                            #endregion
                        }

                        #region COM PORT
                        else if (this.DataGridView1.Rows[i].Cells[0].Value.ToString() == "_log1")
                        {
                            if (ini12.INIRead(Application.StartupPath + "\\Config.ini", "Comport", "value", "") == "1")
                            {
                                switch (DataGridView1.Rows[i].Cells[5].Value.ToString())
                                {
                                    case "_clear":
                                        textBox1.Text = ""; //清除textbox1
                                        break;
                                    case "_save":
                                        Rs232save(); //存檔rs232
                                        break;
                                    default:
                                        //byte[] data = Encoding.Unicode.GetBytes(DataGridView1.Rows[i].Cells[5].Value.ToString());
                                        // string str = Convert.ToString(data);
                                        serialPort1.WriteLine(DataGridView1.Rows[i].Cells[5].Value.ToString() + (char)(13)); //發送數據 Rs232
                                        break;
                                }
                                RedratLable.Text = "(" + DataGridView1.Rows[i].Cells[0].Value.ToString() + ") " + DataGridView1.Rows[i].Cells[5].Value.ToString();
                            }
                        }

                        else if (this.DataGridView1.Rows[i].Cells[0].Value.ToString() == "_log2")
                        {
                            if (ini12.INIRead(Application.StartupPath + "\\Config.ini", "ExtComport", "value", "") == "1")
                            {
                                switch (DataGridView1.Rows[i].Cells[5].Value.ToString())
                                {
                                    case "_clear":
                                        textBox2.Text = ""; //清除textbox2
                                        break;
                                    case "_save":
                                        ExtRs232save(); //存檔rs232
                                        break;
                                    default:
                                        //byte[] data = Encoding.Unicode.GetBytes(DataGridView1.Rows[i].Cells[5].Value.ToString());
                                        // string str = Convert.ToString(data);
                                        serialPort2.WriteLine(DataGridView1.Rows[i].Cells[5].Value.ToString() + (char)(13)); //發送數據 Rs232
                                        break;
                                }
                                RedratLable.Text = "(" + DataGridView1.Rows[i].Cells[0].Value.ToString() + ") " + DataGridView1.Rows[i].Cells[5].Value.ToString();
                            }
                        }
                        #endregion

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
                                case "VESA 1360x768@60":
                                    timebit = new byte[4] { 0x31, 0x36, 0x33, 0x39 };
                                    serialPort1.Write(timebit, 0, 4);
                                    break;
                                case "VESA 1360x768@120":
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
                                case "1920x1080p@24":
                                    timebit = new byte[4] { 0x31, 0x34, 0x32, 0x39 };
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
                                    timebit = new byte[4] { 0x31, 0x30, 0x36, 0x33 };
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
                                case "1440x576i@50":
                                    timebit = new byte[4] { 0x31, 0x30, 0x34, 0x35 };
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
                            RedratLable.Text = "(" + DataGridView1.Rows[i].Cells[0].Value.ToString() + ") " + DataGridView1.Rows[i].Cells[7].Value.ToString();
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
                                case "720x480i@59.94":
                                    serialPort1.WriteLine("FMTL 480I2X29" + "\r");
                                    serialPort1.WriteLine("ALLU" + "\r");
                                    break;
                                case "720x480i@60":
                                    serialPort1.WriteLine("FMTL 480I2X30" + "\r");
                                    serialPort1.WriteLine("ALLU" + "\r");
                                    break;
                                case "640x480p@59.94":
                                    serialPort1.WriteLine("FMTL 480P#" + "\r");
                                    serialPort1.WriteLine("ALLU" + "\r");
                                    break;
                                case "720x480p@59.94":
                                    serialPort1.WriteLine("FMTL 480P59" + "\r");
                                    serialPort1.WriteLine("ALLU" + "\r");
                                    break;
                                case "720x480p@60":
                                    serialPort1.WriteLine("FMTL 480P60" + "\r");
                                    serialPort1.WriteLine("ALLU" + "\r");
                                    break;
                                case "1280x720p@59.94":
                                    serialPort1.WriteLine("FMTL 720P59" + "\r");
                                    serialPort1.WriteLine("ALLU" + "\r");
                                    break;
                                case "1280x720p@60":
                                    serialPort1.WriteLine("FMTL 720P60" + "\r");
                                    serialPort1.WriteLine("ALLU" + "\r");
                                    break;
                                case "1920x1080i@59.94":
                                    serialPort1.WriteLine("FMTL 1080I29" + "\r");
                                    serialPort1.WriteLine("ALLU" + "\r");
                                    break;
                                case "1920x1080i@60":
                                    serialPort1.WriteLine("FMTL 1080I30" + "\r");
                                    serialPort1.WriteLine("ALLU" + "\r");
                                    break;
                                case "1920x1080p@23.97":
                                    serialPort1.WriteLine("FMTL 1080P23" + "\r");
                                    serialPort1.WriteLine("ALLU" + "\r");
                                    break;
                                case "1920x1080p@24":
                                    serialPort1.WriteLine("FMTL 1080P24" + "\r");
                                    serialPort1.WriteLine("ALLU" + "\r");
                                    break;
                                case "1920x1080p@29.97":
                                    serialPort1.WriteLine("FMTL 1080P29" + "\r");
                                    serialPort1.WriteLine("ALLU" + "\r");
                                    break;
                                case "1920x1080p@30":
                                    serialPort1.WriteLine("FMTL 1080P30" + "\r");
                                    serialPort1.WriteLine("ALLU" + "\r");
                                    break;
                                case "1920x1080p@59.94":
                                    serialPort1.WriteLine("FMTL 1080P59" + "\r");
                                    serialPort1.WriteLine("ALLU" + "\r");
                                    break;
                                case "1920x1080p@60":
                                    serialPort1.WriteLine("FMTL 1080P60" + "\r");
                                    serialPort1.WriteLine("ALLU" + "\r");
                                    break;
                                case "720x240p@60.05":
                                    serialPort1.WriteLine("FMTL 240P2X_1" + "\r");
                                    serialPort1.WriteLine("ALLU" + "\r");
                                    break;
                                case "720x240p@60.11":
                                    serialPort1.WriteLine("FMTL 240P2X_2" + "\r");
                                    serialPort1.WriteLine("ALLU" + "\r");
                                    break;
                                case "720x240p@59.83":
                                    serialPort1.WriteLine("FMTL 240P2X_3" + "\r");
                                    serialPort1.WriteLine("ALLU" + "\r");
                                    break;
                                case "720x240p@59.89":
                                    serialPort1.WriteLine("FMTL 240P2X_4" + "\r");
                                    serialPort1.WriteLine("ALLU" + "\r");
                                    break;
                                case "2880x480i@59.94":
                                    serialPort1.WriteLine("FMTL 480I4X29" + "\r");
                                    serialPort1.WriteLine("ALLU" + "\r");
                                    break;
                                case "2880x480i@60":
                                    serialPort1.WriteLine("FMTL 480I4X30" + "\r");
                                    serialPort1.WriteLine("ALLU" + "\r");
                                    break;
                                case "2880x240p@60.05":
                                    serialPort1.WriteLine("FMTL 240P4X_1" + "\r");
                                    serialPort1.WriteLine("ALLU" + "\r");
                                    break;
                                case "2880x240p@60.11":
                                    serialPort1.WriteLine("FMTL 240P4X_2" + "\r");
                                    serialPort1.WriteLine("ALLU" + "\r");
                                    break;
                                case "2880x240p@59.83":
                                    serialPort1.WriteLine("FMTL 240P4X_3" + "\r");
                                    serialPort1.WriteLine("ALLU" + "\r");
                                    break;
                                case "2880x240p@59.89":
                                    serialPort1.WriteLine("FMTL 240P4X_4" + "\r");
                                    serialPort1.WriteLine("ALLU" + "\r");
                                    break;
                                case "720x576i@50":
                                    serialPort1.WriteLine("FMTL 576I2X25" + "\r");
                                    serialPort1.WriteLine("ALLU" + "\r");
                                    break;
                                case "720x576p@50":
                                    serialPort1.WriteLine("FMTL 576P50" + "\r");
                                    serialPort1.WriteLine("ALLU" + "\r");
                                    break;
                                case "1280x720p@50":
                                    serialPort1.WriteLine("FMTL 720P50" + "\r");
                                    serialPort1.WriteLine("ALLU" + "\r");
                                    break;
                                case "1920x1080i@50":
                                    serialPort1.WriteLine("FMTL 1080I25" + "\r");
                                    serialPort1.WriteLine("ALLU" + "\r");
                                    break;
                                case "1920x1080p@25":
                                    serialPort1.WriteLine("FMTL 1080P25" + "\r");
                                    serialPort1.WriteLine("ALLU" + "\r");
                                    break;
                                case "1920x1080p@50":
                                    serialPort1.WriteLine("FMTL 1080P50" + "\r");
                                    serialPort1.WriteLine("ALLU" + "\r");
                                    break;
                                case "720x288p@50.08":
                                    serialPort1.WriteLine("FMTL 288P2X_1" + "\r");
                                    serialPort1.WriteLine("ALLU" + "\r");
                                    break;
                                case "720x288p@49.92":
                                    serialPort1.WriteLine("FMTL 288P2X_2" + "\r");
                                    serialPort1.WriteLine("ALLU" + "\r");
                                    break;
                                case "720x288p@49.76":
                                    serialPort1.WriteLine("FMTL 288P2X_3" + "\r");
                                    serialPort1.WriteLine("ALLU" + "\r");
                                    break;
                                case "2880x576i@50":
                                    serialPort1.WriteLine("FMTL 576I4X25" + "\r");
                                    serialPort1.WriteLine("ALLU" + "\r");
                                    break;
                                case "2880x288p@50.08":
                                    serialPort1.WriteLine("FMTL 288P4X_1" + "\r");
                                    serialPort1.WriteLine("ALLU" + "\r");
                                    break;
                                case "2880x288p@49.92":
                                    serialPort1.WriteLine("FMTL 288P4X_2" + "\r");
                                    serialPort1.WriteLine("ALLU" + "\r");
                                    break;
                                case "2880x288p@49.76":
                                    serialPort1.WriteLine("FMTL 288P4X_3" + "\r");
                                    serialPort1.WriteLine("ALLU" + "\r");
                                    break;
                                case "1440x576i@50":
                                    serialPort1.WriteLine("FMTL 576I2X50" + "\r");
                                    serialPort1.WriteLine("ALLU" + "\r");
                                    break;
                                case "1440x576p@50":
                                    serialPort1.WriteLine("FMTL 576P2X50" + "\r");
                                    serialPort1.WriteLine("ALLU" + "\r");
                                    break;
                                case "2880x576p@50":
                                    serialPort1.WriteLine("FMTL 576P4X50" + "\r");
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
                            RedratLable.Text = "(" + DataGridView1.Rows[i].Cells[0].Value.ToString() + ") " + DataGridView1.Rows[i].Cells[7].Value.ToString() + DataGridView1.Rows[i].Cells[8].Value.ToString();
                        }
                        #endregion

                        #region Dektec
                        else if (this.DataGridView1.Rows[i].Cells[0].Value.ToString() == "_dektec")
                        {
                            if (this.DataGridView1.Rows[i].Cells[4].Value.ToString() == "_start")
                            {
                                String arguments = Application.StartupPath + "\\DektecPlayer\\" + DataGridView1.Rows[i].Cells[6].Value.ToString() + " -mt " + DataGridView1.Rows[i].Cells[7].Value.ToString() + " -r 0 -l 0 -mf " + DataGridView1.Rows[i].Cells[8].Value.ToString(); ;

                                System.Diagnostics.Process Dektec = new System.Diagnostics.Process();
                                Dektec.StartInfo.FileName = Application.StartupPath + "\\DektecPlayer\\DtPlay.exe";

                                Dektec.StartInfo.UseShellExecute = false;
                                Dektec.StartInfo.RedirectStandardInput = true;
                                Dektec.StartInfo.RedirectStandardOutput = true;
                                Dektec.StartInfo.RedirectStandardError = true;
                                Dektec.StartInfo.CreateNoWindow = true;

                                Dektec.StartInfo.Arguments = arguments;
                                Dektec.Start();
                                RedratLable.Text = "(" + DataGridView1.Rows[i].Cells[0].Value.ToString() + ") " + DataGridView1.Rows[i].Cells[6].Value.ToString();
                            }

                            if (this.DataGridView1.Rows[i].Cells[4].Value.ToString() == "_stop")
                            {
                                CloseDtplay();
                            }
                        }
                        #endregion

                        #region 小紅鼠指令
                        else
                        {
                            if (ini12.INIRead(Application.StartupPath + "\\Config.ini", "RedRatDev", "value", "") == "1")
                            {
                                for (int k = 0; k < stime; k++)
                                {
                                    if (ini12.INIRead(Application.StartupPath + "\\Config.ini", "IR Device", "value", "") == "1")
                                    {
                                        autocommand(DataGridView1.Rows[i].Cells[0].Value.ToString()); //執行小紅鼠指令
                                    }
                                    if (ini12.INIRead(Application.StartupPath + "\\Config.ini", "IR Device", "value", "") == "2")
                                    {
                                        myrsccmd(DataGridView1.Rows[i].Cells[0].Value.ToString()); //執TPSRC行指令
                                    }
                                    videostring = DataGridView1.Rows[i].Cells[0].Value.ToString();
                                    Thread.Sleep(sRepeat);
                                }
                                RedratLable.Text = DataGridView1.Rows[i].Cells[0].Value.ToString();
                            }
                            else
                            {
                                MessageBox.Show("Redrat not exist", "Redrat Open Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                StartBtn.PerformClick();
                            }
                        }
                        #endregion

                        //backgroundWorker.RunWorkerAsync();
                        //Thread MyExportText = new Thread(new ThreadStart(MyExportCamd));
                        //MyExportText.Start();

                        ini12.INIWrite(MailPath, "Data Info", "CloseTime", string.Format("{0:R}", DateTime.Now));

                        if (Global.break_sch == 1)      //定時器時間到跳出迴圈
                        {
                            Console.WriteLine("break2");
                            j = Global.Schedule_Loop;
                            UpdateUI(j.ToString(), label1);
                            break;
                        }

                        if (Pause == true)      //如果按下暫停鈕
                        {
                            Thread.Sleep(SysDelay);
                            timer1.Stop();
                            SchedulePause.WaitOne();
                        }
                        else
                            Thread.Sleep(SysDelay);
                    }
                    #region Import database
                    if (ini12.INIRead(Application.StartupPath + "\\Config.ini", "ImportDB", "value", "") == "1")
                    {
                        string SQLServerURL = "server=192.168.56.2\\ATMS;database=Autobox;uid=AS;pwd=AS";

                        SqlConnection conn = new SqlConnection(SQLServerURL);
                        conn.Open();
                        SqlCommand s_com = new SqlCommand();
                        //s_com.CommandText = "select * from Autobox.dbo.testresult";
                        s_com.CommandText = "insert into Autobox.dbo.testresult (ab_p_id, ab_result, ab_create, ab_time, ab_loop, ab_loop_time, ab_loop_step, ab_root, ab_user) values ('" + label1.Text + "', 'Pass', '" + DateTime.Now.ToString("HH:mm:ss") + "', '" + label1.Text + "', 1, 21000, 2, 0, 'Joseph')";
                        //s_com.CommandText = "update Autobox.dbo.testresult (ab_result, ab_close, ab_time, ab_loop, ab_root, ab_user) values ('Pass', '" + DateTime.Now.ToString("HH:mm:ss") + "', '" + label1.Text + "', 1, 21000, 'Joseph')";
                        //s_com.CommandText = "Update Autobox.dbo.testresult set ab_result='Pass', ab_close='2014/5/21 15:49:35', ab_time=600000, ab_loop=25, ab_root=0 where ab_num=2";
                        //s_com.CommandText = "Update Autobox.dbo.testresult set ab_result='NG', ab_close='2014/5/21 15:59:35', ab_time=1200000, ab_loop=50, ab_root=1 where ab_num=3";

                        s_com.Connection = conn;

                        SqlDataReader s_read = s_com.ExecuteReader();
                        try
                        {
                            while (s_read.Read())
                            {
                                Console.WriteLine("Log> Find {0}", s_read["ab_p_id"].ToString());
                            }
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e);
                        }
                        s_read.Close();

                        conn.Close();
                    }
                    #endregion
                }
                Global.loop_Num++;
            }

            #region RecVideo
            if (ini12.INIRead(Application.StartupPath + "\\Config.ini", "RecVideo", "value", "") == "1")
            {
                if (jbutton1 == true)
                {
                    if (ini12.INIRead(Application.StartupPath + "\\Config.ini", "CameraDev", "value", "") == "1")
                    {
                        if (Vread == false)
                        {
                            RedratLable.Text = "Record Video...";
                            Thread.Sleep(1500);
                            mysvideo(); // 開新檔
                            Vread = true;
                            Thread oThreadC = new Thread(new ThreadStart(MySrtCamd));
                            oThreadC.Start();
                            Thread.Sleep(60000); // 錄影60秒

                            Vread = false;
                            mysstop();
                            oThreadC.Abort();
                            Thread.Sleep(1500);
                            RedratLable.Text = "Vdieo recording completely.";
                        }
                    }
                    else
                    {
                        MessageBox.Show("Camera not exist", "Camera Open Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            #endregion
/*
            #region Excel function
            if (ini12.INIRead(Application.StartupPath + "\\Config.ini", "ComparePIC", "value", "") == "1" && excelstat == true)
            {
                string excelFile = ini12.INIRead(sPath, "ComparePIC", "Path", "") + "\\SimilarityReport_" + Global.Schedule_Num;

                try
                {
                    //另存活頁簿
                    wBook.SaveAs(excelFile, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Excel.XlSaveAsAccessMode.xlNoChange, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);
                    Console.WriteLine("儲存文件於 " + Environment.NewLine + excelFile);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("儲存檔案出錯，檔案可能正在使用" + Environment.NewLine + ex.Message);
                }

                //關閉活頁簿
                //wBook.Close(false, Type.Missing, Type.Missing);
                
                //關閉Excel
                excelApp.Quit();
                
                //釋放Excel資源
                System.Runtime.InteropServices.Marshal.ReleaseComObject(excelApp);
                excelApp = null;
                System.Runtime.InteropServices.Marshal.ReleaseComObject(wBook);
                wBook = null;
                System.Runtime.InteropServices.Marshal.ReleaseComObject(wSheet);
                wSheet = null;
                System.Runtime.InteropServices.Marshal.ReleaseComObject(wRange);
                wRange = null;

                GC.Collect();
                excelstat = false;

                //Console.Read();

                CloseExcel();
            }
            #endregion

            if (Global.loop_Num < 3)
            {
            }
            else
            {
                if (jbutton1 == false)
                    Global.loop_Num--;
                Thread MyCompareThread = new Thread(new ThreadStart(MyCompareCamd));
                MyCompareThread.Start();
                RedratLable.Text = "Start Compare Picture...";
                Thread.Sleep(Global.loop_Num * Global.caption_Sum * 2000);
            }
*/
            if (jbutton1 != false)
            {
                #region schedule 切換
                if (Global.Schedule_Num2_Exist == 1 && Global.Schedule_Num == 1)
                {
                    if (ini12.INIRead(Application.StartupPath + "\\Config.ini", "Schedule2", "On Time Start", "") == "1" && jbutton1 == true)       //定時器時間未到進入等待<<<<<<<<<<<<<<
                    {
                        if (Global.break_sch == 0)
                        {
                            while (ini12.INIRead(Application.StartupPath + "\\Config.ini", "Schedule2", "Timer", "") != TimeLabel2.Text)
                            {
                                ScheduleWait.WaitOne();
                            }
                            ScheduleWait.Set();
                        }
                    }       //>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
                    ini12.INIWrite(sPath, "Schedule1", "On Time Start", "0");
                    SchBtn2.PerformClick();
                    MyRunCamd();
                }
                else if (
                    Global.Schedule_Num3_Exist == 1 && Global.Schedule_Num == 1 ||
                    Global.Schedule_Num3_Exist == 1 && Global.Schedule_Num == 2)
                {
                    if (ini12.INIRead(Application.StartupPath + "\\Config.ini", "Schedule3", "On Time Start", "") == "1" && jbutton1 == true)
                    {
                        if (Global.break_sch == 0)
                        {
                            while (ini12.INIRead(Application.StartupPath + "\\Config.ini", "Schedule3", "Timer", "") != TimeLabel2.Text)
                            {
                                ScheduleWait.WaitOne();
                            }
                            ScheduleWait.Set();
                        }
                    }
                    ini12.INIWrite(sPath, "Schedule2", "On Time Start", "0");
                    SchBtn3.PerformClick();
                    MyRunCamd();
                }
                else if (
                    Global.Schedule_Num4_Exist == 1 && Global.Schedule_Num == 1 ||
                    Global.Schedule_Num4_Exist == 1 && Global.Schedule_Num == 2 ||
                    Global.Schedule_Num4_Exist == 1 && Global.Schedule_Num == 3)
                {
                    if (ini12.INIRead(Application.StartupPath + "\\Config.ini", "Schedule4", "On Time Start", "") == "1" && jbutton1 == true)
                    {
                        if (Global.break_sch == 0)
                        {
                            while (ini12.INIRead(Application.StartupPath + "\\Config.ini", "Schedule4", "Timer", "") != TimeLabel2.Text)
                            {
                                ScheduleWait.WaitOne();
                            }
                            ScheduleWait.Set();
                        }
                    }
                    ini12.INIWrite(sPath, "Schedule3", "On Time Start", "0");
                    SchBtn4.PerformClick();
                    MyRunCamd();
                }
                else if (
                    Global.Schedule_Num5_Exist == 1 && Global.Schedule_Num == 1 ||
                    Global.Schedule_Num5_Exist == 1 && Global.Schedule_Num == 2 ||
                    Global.Schedule_Num5_Exist == 1 && Global.Schedule_Num == 3 ||
                    Global.Schedule_Num5_Exist == 1 && Global.Schedule_Num == 4)
                {
                    if (ini12.INIRead(Application.StartupPath + "\\Config.ini", "Schedule5", "On Time Start", "") == "1" && jbutton1 == true)
                    {
                        if (Global.break_sch == 0)
                        {
                            while (ini12.INIRead(Application.StartupPath + "\\Config.ini", "Schedule5", "Timer", "") != TimeLabel2.Text)
                            {
                                ScheduleWait.WaitOne();
                            }
                            ScheduleWait.Set();
                        }
                    }
                    ini12.INIWrite(sPath, "Schedule4", "On Time Start", "0");
                    SchBtn5.PerformClick();
                    MyRunCamd();
                }
            }
            #endregion

            //全部schedule跑完或是按下stop鍵以後會跑以下這段>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
            if (jbutton1 == false)      //按下stop*******************************************************
            {
                ini12.INIWrite(MailPath, "Data Info", "CloseTime", string.Format("{0:R}", DateTime.Now));
                UpdateUI("START", StartBtn);
                StartBtn.Enabled = true;
                SettingBtn.Enabled = true;
                PauseButton.Enabled = false;
                WriteBtn.Enabled = true;

                close232();
                Extclose232();

                if (ini12.INIRead(Application.StartupPath + "\\Config.ini", "CameraDev", "value", "") == "1")
                {
                    _captureInProgress = false;
                    OnOffCamera();
                    CamPreviewBtn.Enabled = true;
                }

/*
                if (Directory.Exists(ini12.INIRead(Application.StartupPath + "\\Config.ini", "Video", "Path", "") + "\\" + "Schedule" + Global.Schedule_Num + "_Original") == true)
                {
                    DirectoryInfo DIFO = new DirectoryInfo(ini12.INIRead(Application.StartupPath + "\\Config.ini", "Video", "Path", "") + "\\" + "Schedule" + Global.Schedule_Num + "_Original");
                    DIFO.Delete(true);
                }
*/
            }
            else
            {
                jbutton1 = false;       //全部schedule自動跑完************************************************

                UpdateUI("START", StartBtn);
                SettingBtn.Enabled = true;
                PauseButton.Enabled = false;
                WriteBtn.Enabled = true;
                
                close232();
                Extclose232();

                if (ini12.INIRead(Application.StartupPath + "\\Config.ini", "CameraDev", "value", "") == "1")
                {
                    _captureInProgress = false;
                    OnOffCamera();
                    CamPreviewBtn.Enabled = true;
                }

                Global.Total_Time = Global.Schedule_Num1_Time + Global.Schedule_Num2_Time + Global.Schedule_Num3_Time + Global.Schedule_Num4_Time + Global.Schedule_Num5_Time;
                ConvertToRealTime(Global.Total_Time);
                if (ini12.INIRead(MailPath, "Send Mail", "value", "") == "1")
                {
                    FormMail FormMail = new FormMail();
                    FormMail.send();
                }
            }
            //backgroundWorker.CancelAsync();
            RedratLable.Text = "AutoTest Finished!";
            ini12.INIWrite(sPath, "Schedule" + Global.Schedule_Num, "On Time Start", "0");
            SchBtn1.PerformClick();
            timer1.Stop();
            timeCount = Global.Schedule_Num1_Time;
            ConvertToRealTime(timeCount);
            progressBar1.Value = 0;
            //<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<
        }
        #endregion

        // 圖片比較執行緒
        private void MyCompareCamd()
        {
            //String fNameAll = "";
            //String fNameNG = "";
/*            
            int i, j = 1;
            int TotalDelay = 0;

            switch (Global.Schedule_Num)
            {
                case 1:
                    TotalDelay = (Convert.ToInt32(Global.Schedule_Num1_Time) / Global.Schedule_Loop);
                    break;
                case 2:
                    TotalDelay = (Convert.ToInt32(Global.Schedule_Num2_Time) / Global.Schedule_Loop);
                    break;
                case 3:
                    TotalDelay = (Convert.ToInt32(Global.Schedule_Num3_Time) / Global.Schedule_Loop);
                    break;
                case 4:
                    TotalDelay = (Convert.ToInt32(Global.Schedule_Num4_Time) / Global.Schedule_Loop);
                    break;
                case 5:
                    TotalDelay = (Convert.ToInt32(Global.Schedule_Num5_Time) / Global.Schedule_Loop);
                    break;
            }       //<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<


            //float[,] ReferenceResult = new float[Global.Schedule_Loop, Global.caption_Sum + 1];
            //float[] MeanValue = new float[Global.Schedule_Loop];
            //int[] TotalValue = new int[Global.Schedule_Loop];
*/
            //string ngPath = ini12.INIRead(sPath, "Video", "Path", "") + "\\" + "Schedule" + Global.Schedule_Num + "_NG\\";
            string comparePath = ini12.INIRead(sPath, "ComparePIC", "Path", "") + "\\";
            string csvFile = comparePath + "SimilarityReport_" + Global.Schedule_Num + ".csv";

            //Console.WriteLine("Loop Number: " + Global.loop_Num);

            // 讀取ini中的路徑
            //fNameNG = ini12.INIRead(sPath, "Video", "Path", "") + "\\" + "Schedule" + Global.Schedule_Num + "_NG\\";

            string pathCompare1 = comparePath + "cf-" + Global.loop_Num + "_" + Global.caption_Num + ".png";
            string pathCompare2 = comparePath + "cf-" + (Global.loop_Num - 1) + "_" + Global.caption_Num + ".png";
            if (Global.caption_Num == 0)
            {
                Console.WriteLine("Path Compare1: " + pathCompare1);
                Console.WriteLine("Path Compare2: " + pathCompare2);
            }
            if (System.IO.File.Exists(pathCompare1) && System.IO.File.Exists(pathCompare2))
            {
                string oHashCode = ImageHelper.produceFingerPrint(pathCompare1);
                string nHashCode = ImageHelper.produceFingerPrint(pathCompare2);
                int difference = ImageHelper.hammingDistance(oHashCode, nHashCode);
                int differenceNum = Convert.ToInt32(ini12.INIRead(sPath, "ComparePIC", "DifferentNum", ""));
                string differencePercent = "";

                if (difference == 0)
                {
                    differencePercent = "100%";
                }
                else if (difference <= 10)
                {
                    differencePercent = "90%";
                }
                else if (difference <= 20)
                {
                    differencePercent = "80%";
                }
                else if (difference <= 30)
                {
                    differencePercent = "70%";
                }
                else if (difference <= 40)
                {
                    differencePercent = "60%";
                }
                else if (difference <= 50)
                {
                    differencePercent = "50%";
                }
                else if (difference <= 60)
                {
                    differencePercent = "40%";
                }
                else if (difference <= 70)
                {
                    differencePercent = "30%";
                }
                else if (difference <= 80)
                {
                    differencePercent = "20%";
                }
                else if (difference <= 90)
                {
                    differencePercent = "10%";
                }
                else
                {
                    differencePercent = "0%";
                }
                // 匯出csv記錄檔
                StreamWriter sw = new StreamWriter(csvFile, true);

                // 比對值設定
                Global.excel_Num++;
                if (difference > differenceNum)
                {
                    Global.NGValue[Global.caption_Num]++;
                    Global.NGRateValue[Global.caption_Num] = (float)Global.NGValue[Global.caption_Num] / (Global.loop_Num - 1);
/*
                    string[] FileList = System.IO.Directory.GetFiles(fNameAll, "cf-" + Global.loop_Num + "_" + Global.caption_Num + ".png");
                    foreach (string File in FileList)
                    {
                        System.IO.FileInfo fi = new System.IO.FileInfo(File);
                        fi.CopyTo(fNameNG + fi.Name);
                    }
*/
                    Global.NGRateValue[Global.caption_Num] = (float)Global.NGValue[Global.caption_Num] / (Global.loop_Num - 1);
/*
                    #region Excel function
                    try
                    {
                        // 引用第一個工作表
                        wSheet = (Excel._Worksheet)wBook.Worksheets[1];

                        // 命名工作表的名稱
                        wSheet.Name = "全部測試資料";

                        // 設定工作表焦點
                        wSheet.Activate();

                        // 設定第n列資料
                        excelApp.Cells[Global.excel_Num, 1] = " " + (Global.loop_Num - 1) + "-" + Global.caption_Num;
                        wSheet.Hyperlinks.Add(excelApp.Cells[Global.excel_Num, 1], "cf-" + (Global.loop_Num - 1) + "_" + Global.caption_Num + ".png", Type.Missing, Type.Missing, Type.Missing);
                        excelApp.Cells[Global.excel_Num, 2] = " " + (Global.loop_Num) + "-" + Global.caption_Num;
                        wSheet.Hyperlinks.Add(excelApp.Cells[Global.excel_Num, 2], "cf-" + (Global.loop_Num) + "_" + Global.caption_Num + ".png", Type.Missing, Type.Missing, Type.Missing);
                        excelApp.Cells[Global.excel_Num, 3] = differencePercent;
                        excelApp.Cells[Global.excel_Num, 4] = Global.NGValue[Global.caption_Num];
                        excelApp.Cells[Global.excel_Num, 5] = Global.NGRateValue[Global.caption_Num];
                        excelApp.Cells[Global.excel_Num, 6] = "NG";

                        // 設定第n列顏色
                        wRange = wSheet.Range[wSheet.Cells[Global.excel_Num, 1], wSheet.Cells[Global.excel_Num, 2]];
                        wRange.Select();
                        wRange.Font.Color = ColorTranslator.ToOle(Color.Blue);
                        wRange = wSheet.Range[wSheet.Cells[Global.excel_Num, 3], wSheet.Cells[Global.excel_Num, 6]];
                        wRange.Select();
                        wRange.Font.Color = ColorTranslator.ToOle(Color.Red);

                        // 自動調整欄寬
                        wRange = wSheet.Range[wSheet.Cells[Global.excel_Num, 1], wSheet.Cells[Global.excel_Num, 6]];
                        wRange.EntireRow.AutoFit();
                        wRange.EntireColumn.AutoFit();

                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("產生報表時出錯！" + Environment.NewLine + ex.Message);
                    }
                    #endregion
*/
                    sw.Write("=hyperlink(\"cf-" + (Global.loop_Num - 1) + "_" + Global.caption_Num + ".png\"，\"" + (Global.loop_Num - 1) + "-" + Global.caption_Num + "\")" + ",");
                    sw.Write("=hyperlink(\"cf-" + (Global.loop_Num) + "_" + Global.caption_Num + ".png\"，\"" + (Global.loop_Num) + "-" + Global.caption_Num + "\")" + ",");
                    sw.Write(differencePercent + ",");
                    sw.Write(Global.NGValue[Global.caption_Num] + ",");
                    sw.Write(Global.NGRateValue[Global.caption_Num] + ",");
                    sw.WriteLine("NG");
                }
                else
                {
                    Global.NGRateValue[Global.caption_Num] = (float)Global.NGValue[Global.caption_Num] / (Global.loop_Num -1);
/*
                    #region Excel function
                    try
                    {
                        // 引用第一個工作表
                        wSheet = (Excel._Worksheet)wBook.Worksheets[1];

                        // 命名工作表的名稱
                        wSheet.Name = "全部測試資料";

                        // 設定工作表焦點
                        wSheet.Activate();

                        // 設定第n列資料
                        excelApp.Cells[Global.excel_Num, 1] = " " + (Global.loop_Num - 1) + "-" + Global.caption_Num;
                        wSheet.Hyperlinks.Add(excelApp.Cells[Global.excel_Num, 1], "cf-" + (Global.loop_Num - 1) + "_" + Global.caption_Num + ".png", Type.Missing, Type.Missing, Type.Missing);
                        excelApp.Cells[Global.excel_Num, 2] = " " + (Global.loop_Num) + "-" + Global.caption_Num;
                        wSheet.Hyperlinks.Add(excelApp.Cells[Global.excel_Num, 2], "cf-" + (Global.loop_Num) + "_" + Global.caption_Num + ".png", Type.Missing, Type.Missing, Type.Missing);
                        excelApp.Cells[Global.excel_Num, 3] = differencePercent;
                        excelApp.Cells[Global.excel_Num, 4] = Global.NGValue[Global.caption_Num];
                        excelApp.Cells[Global.excel_Num, 5] = Global.NGRateValue[Global.caption_Num];
                        excelApp.Cells[Global.excel_Num, 6] = "Pass";

                        // 設定第n列顏色
                        wRange = wSheet.Range[wSheet.Cells[Global.excel_Num, 1], wSheet.Cells[Global.excel_Num, 2]];
                        wRange.Select();
                        wRange.Font.Color = ColorTranslator.ToOle(Color.Blue);
                        wRange = wSheet.Range[wSheet.Cells[Global.excel_Num, 3], wSheet.Cells[Global.excel_Num, 6]];
                        wRange.Select();
                        wRange.Font.Color = ColorTranslator.ToOle(Color.Green);

                        // 自動調整欄寬
                        wRange = wSheet.Range[wSheet.Cells[Global.excel_Num, 1], wSheet.Cells[Global.excel_Num, 6]];
                        wRange.EntireRow.AutoFit();
                        wRange.EntireColumn.AutoFit();

                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("產生報表時出錯！" + Environment.NewLine + ex.Message);
                    }
                    #endregion
*/ 
                    sw.Write("=hyperlink(\"cf-" + (Global.loop_Num - 1) + "_" + Global.caption_Num + ".png\"，\"" + (Global.loop_Num - 1) + "-" + Global.caption_Num + "\")" + ",");
                    sw.Write("=hyperlink(\"cf-" + (Global.loop_Num) + "_" + Global.caption_Num + ".png\"，\"" + (Global.loop_Num) + "-" + Global.caption_Num + "\")" + ",");
                    sw.Write(differencePercent + ",");
                    sw.Write(Global.NGValue[Global.caption_Num] + ",");
                    sw.Write(Global.NGRateValue[Global.caption_Num] + ",");
                    sw.WriteLine("Pass");
                }
                sw.Close();
/*
                Bitmap picCompare1 = (Bitmap)Image.FromFile(pathCompare1);
                Bitmap picCompare2 = (Bitmap)Image.FromFile(pathCompare2);
                float CompareValue = Similarity(picCompare1, picCompare2);
                ReferenceResult[(Global.loop_Num - 1), Global.caption_Num] = CompareValue;
                Console.WriteLine("Reference(" + (Global.loop_Num - 1) + "," + Global.caption_Num + ") = " + ReferenceResult[(Global.loop_Num - 1), Global.caption_Num]);

                Global.SumValue[Global.caption_Num] = Global.SumValue[Global.caption_Num] + ReferenceResult[(Global.loop_Num - 1), Global.caption_Num];
                Console.WriteLine("SumValue" + Global.caption_Num + " = " + Global.SumValue[Global.caption_Num]);

                MeanValue[Global.caption_Num] = Global.SumValue[Global.caption_Num] / (Global.loop_Num - 1);
                Console.WriteLine("MeanValue" + Global.caption_Num + " = " + MeanValue[Global.caption_Num]);

                for (i = Global.loop_Num - 11; i < Global.loop_Num - 1; i++)
                {
                    for (j = 1; j < Global.caption_Sum + 1; j++)
                    {
                        string pathCompare1 = fNameAll + "cf-" + i + "_" + j + ".png";
                        string pathCompare2 = fNameAll + "cf-" + (i - 1) + "_" + j + ".png";
                        Bitmap picCompare1 = (Bitmap)Image.FromFile(pathCompare1);
                        Bitmap picCompare2 = (Bitmap)Image.FromFile(pathCompare2);
                        float CompareValue = Similarity(picCompare1, picCompare2);
                        ReferenceResult[i, j] = CompareValue;
                        Console.WriteLine("Reference(" + i + "," + j + ") = " + ReferenceResult[i, j]);
                       
                        //int[] GetHisogram1 = GetHisogram(picCompare1);
                        //int[] GetHisogram2 = GetHisogram(picCompare2);
                        //float CompareResult = GetResult(GetHisogram1, GetHisogram2);
                       
                        //long[] GetHistogram1 = GetHistogram(picCompare1);
                        //long[] GetHistogram2 = GetHistogram(picCompare2);
                        //float CompareResult = GetResult(GetHistogram1, GetHistogram2);

                    }
                    //Thread.Sleep(TotalDelay);
                }

                for (j = 1; j < Global.caption_Sum + 1; j++)
                {
                    for (i = 1; i < Global.loop_Num - 1; i++)
                    {
                        SumValue[j] = SumValue[j] + ReferenceResult[i, j];
                        TotalValue[j]++;
                        //Console.WriteLine("SumValue" + j + " = " + SumValue[j]);
                    }
                    //Thread.Sleep(TotalDelay);
                    MeanValue[j] = SumValue[j] / (Global.loop_Num - 2);
                    //Console.WriteLine("MeanValue" + j + " = " + MeanValue[j]);
                }

                StreamWriter sw = new StreamWriter(csvFile, true);
                if (Global.loop_Num == 2 && Global.caption_Num == 1)
                    sw.WriteLine("Point(X), Point(Y), MeanValue, Reference, NGValue, TotalValue, NGRate, Test Result");

                if (ReferenceResult[(Global.loop_Num - 1), Global.caption_Num] > (MeanValue[Global.caption_Num] + 0.5) || ReferenceResult[(Global.loop_Num - 1), Global.caption_Num] < (MeanValue[Global.caption_Num] - 0.5))
                {
                    Global.NGValue[Global.caption_Num]++;
                    Global.NGRateValue[Global.caption_Num] = (float)Global.NGValue[Global.caption_Num] / Global.loop_Num;
                    string[] FileList = System.IO.Directory.GetFiles(fNameAll, "cf-" + Global.loop_Num + "_" + Global.caption_Num + ".png");
                    foreach (string File in FileList)
                    {
                        System.IO.FileInfo fi = new System.IO.FileInfo(File);
                        fi.CopyTo(fNameNG + fi.Name);
                    }
                    sw.Write((Global.loop_Num - 1) + ", " + Global.caption_Num + ", ");
                    sw.Write(MeanValue[Global.caption_Num] + ", ");
                    sw.Write(ReferenceResult[(Global.loop_Num - 1), Global.caption_Num] + ", ");
                    sw.Write(Global.NGValue[Global.caption_Num] + ", ");
                    sw.Write(Global.loop_Num + ", ");
                    sw.Write(Global.NGRateValue[Global.caption_Num] + ", ");
                    sw.WriteLine("NG");
                }
                else
                {
                    Global.NGRateValue[Global.caption_Num] = (float)Global.NGValue[Global.caption_Num] / Global.loop_Num;
                    sw.Write((Global.loop_Num - 1) + ", " + Global.caption_Num + ", ");
                    sw.Write(MeanValue[Global.caption_Num] + ", ");
                    sw.Write(ReferenceResult[(Global.loop_Num - 1), Global.caption_Num] + ", ");
                    sw.Write(Global.NGValue[Global.caption_Num] + ", ");
                    sw.Write(Global.loop_Num + ", ");
                    sw.Write(Global.NGRateValue[Global.caption_Num] + ", ");
                    sw.WriteLine("Pass");
                }
                sw.Close();

                RedratLable.Text = "End Compare Picture.";
*/
            }
        }

        // 字幕檔執行緒
        private void MySrtCamd()
        {
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

        // 停止錄影
        private void mysstop()
        {
            this.Invoke(new EventHandler(delegate
            {
                capture.Stop();
                capture.Dispose();

                GC.Collect();       // 釋放記憶體
                GC.WaitForPendingFinalizers();

                camstart();
            }));
        }

        // 錄影
        private void mysvideo()
        {
            this.Invoke(new EventHandler(delegate{savevideo();}));
        }

        //儲存影片
        protected void savevideo()
        {
            //存檔
            //儲存視頻副程式
            string fName = "";

            // 讀取ini中的路徑
            fName = ini12.INIRead(sPath, "Video", "Path", "");

            string t = fName + "\\" + "_pvr" + DateTime.Now.ToString("yyyyMMddHHmmss") + "__" + label1.Text + ".wmv";
            srtstring = fName + "\\" + "_pvr" + DateTime.Now.ToString("yyyyMMddHHmmss") + "__" + label1.Text + ".srt";

            if (!capture.Cued)
                capture.Filename = t;

            capture.RecFileMode = DirectX.Capture.Capture.RecFileModeType.Wmv; //宣告我要wmv檔格式
            capture.Cue(); // 創一個檔
            capture.Start(); // 開始錄影
            
            /*
            double chd; //檢查HD 空間 小於100M就停止錄影s
            chd = ImageOpacity.ChDisk(ImageOpacity.Dkroot(fName));
            if (chd < 0.1)
            {
                Vread = false;
                MessageBox.Show("Check the HD Capacity!", "HD Capacity Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }*/
        }

        private void OnOffCamera() //啟動攝影機
        {
            if (_captureInProgress == true)
            {
                camstart();
            }
            else if (_captureInProgress == false && CamPreviewBtn.Enabled == false)
            {
                capture.Stop();
                capture.Dispose();
            }
            else
            {
            }
        }

        private void camstart()
        {
            // 設定INI檔路徑
            int scam = 0;
            int saud = 0;

            // 讀取ini中的路徑
            scam = int.Parse(ini12.INIRead(sPath, "camera", "value", ""));
            saud = int.Parse(ini12.INIRead(sPath, "audio", "value", ""));

            #if DEBUG
            capture = new Capture(filters.VideoInputDevices[scam], filters.AudioInputDevices[saud]);
            capture.CaptureComplete += new EventHandler(OnCaptureComplete);
            #endif

            if (capture == null)
                MessageBox.Show("Please select a video and/or audio device", "Camera Status Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            if (capture.PreviewWindow == null)
            {
                capture.PreviewWindow = panelVideo;
            }
            else
            {
                capture.PreviewWindow = null;
            }
        }

        private void CamPreviewBtn_Click(object sender, EventArgs e)
        {
            _captureInProgress = true;
            OnOffCamera();

            CamPreviewBtn.Enabled = false;
        }
        #endregion

        #region 讀取RC DB並填入combobox
        private void LoadRCDB()
        {
            string xmlfile = ini12.INIRead(sPath, "RedRatCmd", "Path", "");

            DataGridViewComboBoxColumn RCDB = (DataGridViewComboBoxColumn)this.DataGridView1.Columns[0];

            using (XmlReader reader = XmlReader.Create(xmlfile))
            {
                while (reader.Read())
                {
                    if (reader.IsStartElement())
                    {
                        switch (reader.Name.ToString())
                        {
                            case "Name":
                                RCDB.Items.Add(reader.ReadString());
                                break;
                        }
                    }
                }
            }
            RCDB.Items.Add("_cmd");
            RCDB.Items.Add("_log1");
            RCDB.Items.Add("_log2");
            RCDB.Items.Add("_astro");
            RCDB.Items.Add("_quantum");
            RCDB.Items.Add("_dektec");
        }
        #endregion

        private void StartBtn_Click(object sender, EventArgs e)
        {
            //啟動程式
            byte[] val;
            val = new byte[2];
            val[0] = 0;
            bool status;

            //讀取Autobox的設定值
            if (ini12.INIRead(Application.StartupPath + "\\Config.ini", "AutoboxDev", "value", "") == "1")
                status = true;
            else
                status = false;

            if (ini12.INIRead(Application.StartupPath + "\\Config.ini", "CameraDev", "value", "") == "1")
            {
                filters = new Filters();
                CamPreviewBtn.PerformClick();
            }//<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<
            if (status == true)
            {
                SchBtn1.PerformClick();
                Thread oThreadB = new Thread(new ThreadStart(MyRunCamd));

                if (jbutton1 == true)
                {
                    oThreadB.Abort();//停止執行緒
                    timer1.Stop();      //停止倒數
                    if (ini12.INIRead(Application.StartupPath + "\\Config.ini", "IR Device", "value", "") == "2")
                        close232(); // 關閉232

                    if (ini12.INIRead(Application.StartupPath + "\\Config.ini", "Comport", "value", "") == "1")
                        close232(); // 關閉rs232

                    if (ini12.INIRead(Application.StartupPath + "\\Config.ini", "ExtComport", "value", "") == "1")
                        Extclose232(); // 關閉rs232

                    jbutton1 = false;
                    StartBtn.Enabled = false;
                    SettingBtn.Enabled = false;
                    WriteBtn.Enabled = false;
                    PauseButton.Enabled = true;
                    
                    CloseDtplay();
                    RedratLable.Text = "Please wait...";
                }
                else
                {
/*
                    for (int i = 1; i < 6; i++)
                    {
                        if (Directory.Exists(ini12.INIRead(Application.StartupPath + "\\Config.ini", "Video", "Path", "") + "\\" + "Schedule" + i + "_Original") == true)
                        {
                            DirectoryInfo DIFO = new DirectoryInfo(ini12.INIRead(Application.StartupPath + "\\Config.ini", "Video", "Path", "") + "\\" + "Schedule" + i + "_Original");
                            DIFO.Delete(true);
                        }

                        if (Directory.Exists(ini12.INIRead(Application.StartupPath + "\\Config.ini", "Video", "Path", "") + "\\" + "Schedule" + i + "_NG") == true)
                        {
                            DirectoryInfo DIFO = new DirectoryInfo(ini12.INIRead(Application.StartupPath + "\\Config.ini", "Video", "Path", "") + "\\" + "Schedule" + i + "_NG");
                            DIFO.Delete(true);
                        }                
                    }
*/
                    if (ini12.INIRead(Application.StartupPath + "\\Config.ini", "IR Device", "value", "") == "2")
                    {
                        rsc_Rs232(); //啟動src的232
                    }
                    if (ini12.INIRead(Application.StartupPath + "\\Config.ini", "Comport", "value", "") == "1")
                    {
                        iniRs232(); //啟動RS232
                    }
                    if (ini12.INIRead(Application.StartupPath + "\\Config.ini", "ExtComport", "value", "") == "1")
                    {
                        ExtiniRs232();
                    }
                    jbutton1 = true;
                    SettingBtn.Enabled = false;
                    PauseButton.Enabled = true;
                    WriteBtn.Enabled = false;
                    oThreadB.Start();       // 啟動執行緒
                    timer1.Start();     //開始倒數
                    bool bSuccess = PL2303_GP1_GetValue(hCOM, val);
                    if (bSuccess)
                    {
                        if (val[0] == 0)
                        {
                            pbutton1 = false;
                            pictureBox3.Image = Properties.Resources._02;
                        }
                        else
                        {
                            pbutton1 = true;
                            pictureBox3.Image = Properties.Resources._01;
                        }
                    }
                    StartBtn.Text = "STOP";
                    //CamPreviewBtn.PerformClick();
                }
            }
            else
            {
                Thread oThreadB = new Thread(new ThreadStart(MyRunCamd));
                if (jbutton1 == true)
                {
                    oThreadB.Abort();//停止執行緒
                    timer1.Stop();      //停止倒數
                    jbutton1 = false;
                    StartBtn.Enabled = false;
                    SettingBtn.Enabled = false;
                    PauseButton.Enabled = true;
                    WriteBtn.Enabled = false;
                    CloseDtplay();
                    RedratLable.Text = "Please wait...";
                }
                else
                {
                    oThreadB.Start();// 啟動執行緒
                    timer1.Start();     //開始倒數
                    jbutton1 = true;
                    SettingBtn.Enabled = false;
                    PauseButton.Enabled = true;
                    pictureBox3.Image = Properties.Resources._02;
                }
            }
        }

        private void SettingBtn_Click(object sender, EventArgs e)
        {
            SettingBtn.Enabled = false;
            FormTabControl FormTabControl = new FormTabControl();

            Global.RCDB = ini12.INIRead(sPath, "RedCon", "value", "");

            if (FormTabControl.ShowDialog() == DialogResult.OK)     //關閉FormTabControl以後會讀這段>>>>>>>>>>>>>>>>>>>>>>>
            {
                if (ini12.INIRead(sPath, "RedCon", "value", "") != Global.RCDB)
                {
                    DataGridViewComboBoxColumn RCDB = (DataGridViewComboBoxColumn)this.DataGridView1.Columns[0];
                    RCDB.Items.Clear();
                    LoadRCDB();
                }

                SettingBtn.Enabled = true;

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
                    _captureInProgress = false;
                    OnOffCamera();
                    CamPreviewBtn.Enabled = true;
                }
                else
                {
                    pictureBox2.Image = Properties.Resources._02;
                }
                
                if (ini12.INIRead(Application.StartupPath + "\\Config.ini", "ExtComport", "value", "") == "1")
                {
                    Com2Btn.Visible = true;
                    Com2Btn.PerformClick();
                }
                else
                    Com2Btn.Visible = false;

                if (ini12.INIRead(Application.StartupPath + "\\Config.ini", "Comport", "value", "") == "1")
                {
                    Com1Btn.Visible = true;
                    Com1Btn.PerformClick();
                }
                else
                    Com1Btn.Visible = false;

                List<string> SchExist = new List<string> { };
                for (int i = 2; i < 6; i++)
                    SchExist.Add(ini12.INIRead(Application.StartupPath + "\\Config.ini", "Schedule" + i + " Exist", "value", ""));

                if (SchExist[0] == "0")
                    SchBtn2.Visible = false;
                else
                {
                    SchBtn2.Visible = true;
                }

                if (SchExist[1] == "0")
                    SchBtn3.Visible = false;
                else
                    SchBtn3.Visible = true;

                if (SchExist[2] == "0")
                    SchBtn4.Visible = false;
                else
                    SchBtn4.Visible = true;

                if (SchExist[3] == "0")
                    SchBtn5.Visible = false;
                else
                    SchBtn5.Visible = true;
            }       //<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<
            FormTabControl.Dispose();
            SchBtn1.Enabled = true;
            SchBtn1.PerformClick();      
        }

        private void PowerBtn_Click(object sender, EventArgs e)
        {
            // 電源開或關
            byte[] val1;
            val1 = new byte[2];
            val1[0] = 0;

            bool jSuccess = PL2303_GP0_Enable(hCOM, 1);
            if (jSuccess && pbutton1 == false)
            {
                uint val;
                val = (uint)int.Parse("1");
                bool bSuccess = PL2303_GP0_SetValue(hCOM, val);
                bool kbSuccess = PL2303_GP1_GetValue(hCOM, val1);
                if (kbSuccess)
                {
                    //if (val1[0] == 0)
                    {
                        pbutton1 = true;
                        pictureBox3.Image = Properties.Resources._01;
                    }
                    //else
                    //pictureBox3.Image = Properties.Resources._02;
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
                    {
                        pbutton1 = false;
                        pictureBox3.Image = Properties.Resources._02;
                    }
                    //else
                    //pictureBox3.Image = Properties.Resources._01;
                }
            }
            else
            {
                Log("GP0 output enable FAILED.");
            }
        }

        //系統時間
        private void timer1_Tick(object sender, EventArgs e)
        {
            DateTime dt = DateTime.Now;
            TimeLabel.Text = string.Format("{0:R}", dt);
            TimeLabel2.Text = string.Format("{0:yyyy-MM-dd  HH:mm:ss}", dt);

            #region schedule timer
            if (ini12.INIRead(Application.StartupPath + "\\Config.ini", "Schedule1", "On Time Start", "") == "1")
                labelSch1Timer.Text = "Schedule 1 will start at" + "\r\n" + ini12.INIRead(Application.StartupPath + "\\Config.ini", "Schedule1", "Timer", "");
            else if (ini12.INIRead(Application.StartupPath + "\\Config.ini", "Schedule1", "On Time Start", "") == "0")
                labelSch1Timer.Text = "";

            if (ini12.INIRead(Application.StartupPath + "\\Config.ini", "Schedule2", "On Time Start", "") == "1")
                labelSch2Timer.Text = "Schedule 2 will start at" + "\r\n" + ini12.INIRead(Application.StartupPath + "\\Config.ini", "Schedule2", "Timer", "");
            else if (ini12.INIRead(Application.StartupPath + "\\Config.ini", "Schedule2", "On Time Start", "") == "0")
                labelSch2Timer.Text = "";

            if (ini12.INIRead(Application.StartupPath + "\\Config.ini", "Schedule3", "On Time Start", "") == "1")
                labelSch3Timer.Text = "Schedule 3 will start at" + "\r\n" + ini12.INIRead(Application.StartupPath + "\\Config.ini", "Schedule3", "Timer", "");
            else if (ini12.INIRead(Application.StartupPath + "\\Config.ini", "Schedule3", "On Time Start", "") == "0")
                labelSch3Timer.Text = "";

            if (ini12.INIRead(Application.StartupPath + "\\Config.ini", "Schedule4", "On Time Start", "") == "1")
                labelSch4Timer.Text = "Schedule 4 will start at" + "\r\n" + ini12.INIRead(Application.StartupPath + "\\Config.ini", "Schedule4", "Timer", "");
            else if (ini12.INIRead(Application.StartupPath + "\\Config.ini", "Schedule4", "On Time Start", "") == "0")
                labelSch4Timer.Text = "";

            if (ini12.INIRead(Application.StartupPath + "\\Config.ini", "Schedule5", "On Time Start", "") == "1")
                labelSch5Timer.Text = "Schedule 5 will start at" + "\r\n" + ini12.INIRead(Application.StartupPath + "\\Config.ini", "Schedule5", "Timer", "");
            else if (ini12.INIRead(Application.StartupPath + "\\Config.ini", "Schedule5", "On Time Start", "") == "0")
                labelSch5Timer.Text = "";

            if (ini12.INIRead(Application.StartupPath + "\\Config.ini", "Schedule1", "On Time Start", "") == "1" && 
                ini12.INIRead(Application.StartupPath + "\\Config.ini", "Schedule1", "Timer", "") == TimeLabel2.Text)
                StartBtn.PerformClick();
            if (ini12.INIRead(Application.StartupPath + "\\Config.ini", "Schedule2", "On Time Start", "") == "1" &&
                ini12.INIRead(Application.StartupPath + "\\Config.ini", "Schedule2", "Timer", "") == TimeLabel2.Text &&
                timeCount != 0)
                Global.break_sch = 1;
            if (ini12.INIRead(Application.StartupPath + "\\Config.ini", "Schedule3", "On Time Start", "") == "1" &&
                ini12.INIRead(Application.StartupPath + "\\Config.ini", "Schedule3", "Timer", "") == TimeLabel2.Text &&
                timeCount != 0)
                Global.break_sch = 1;
            if (ini12.INIRead(Application.StartupPath + "\\Config.ini", "Schedule4", "On Time Start", "") == "1" &&
                ini12.INIRead(Application.StartupPath + "\\Config.ini", "Schedule4", "Timer", "") == TimeLabel2.Text &&
                timeCount != 0)
                Global.break_sch = 1;
            if (ini12.INIRead(Application.StartupPath + "\\Config.ini", "Schedule5", "On Time Start", "") == "1" &&
                ini12.INIRead(Application.StartupPath + "\\Config.ini", "Schedule5", "Timer", "") == TimeLabel2.Text &&
                timeCount != 0)
                Global.break_sch = 1;
            #endregion

            //if (TimeLabel.Text)
            //TimeLable.Text = DateTime.Now.ToLongDateString() + "   " + DateTime.Now.ToLongTimeString();
        }

        //關閉Excel
        private void CloseExcel()
        {
            Process[] processes = Process.GetProcessesByName("EXCEL");

            foreach (Process p in processes)
            {
                p.Kill();
            }
        }

        //關閉DtPlay
        private void CloseDtplay()
        {
            Process[] processes = Process.GetProcessesByName("DtPlay");

            foreach (Process p in processes)
            {
                p.Kill();
            }
        }

        //關閉AutoBox
        private void CloseAutobox()
        {
            Application.ExitThread();
            Application.Exit();
            Environment.Exit(Environment.ExitCode);
        }

        private void labelVersion_MouseClick(object sender, MouseEventArgs e)
        {
            FormSurp SurpriseForm = new FormSurp();
            SurpriseForm.Show(this);
        }

        private void Com1Btn_Click(object sender, EventArgs e)
        {
            Com1Btn.Enabled = false;
            Com2Btn.Enabled = true;
            this.Controls.Add(textBox1);
            textBox1.BringToFront();
            Com1Btn.BringToFront();
            Com2Btn.BringToFront();
        }

        private void Com2Btn_Click(object sender, EventArgs e)
        {
            Com2Btn.Enabled = false;
            Com1Btn.Enabled = true;
            this.Controls.Add(textBox2);
            textBox2.BringToFront();
            Com1Btn.BringToFront();
            Com2Btn.BringToFront();
        }

        private void MyExportCamd()
        {
            string ab_num = label1.Text,     //自動編號
                        ab_p_id = ini12.INIRead(MailPath, "Data Info", "ProjectNumber", ""),                                                              //Project number
                        ab_c_id = ini12.INIRead(MailPath, "Data Info", "TestCaseNumber", ""),                                                              //Test case number
                        ab_result = ini12.INIRead(MailPath, "Data Info", "Result", ""),                                                       //AutoTest 測試結果
                        ab_version = ini12.INIRead(MailPath, "Mail Info", "Version", ""),                                             //軟體版號
                        ab_ng = ini12.INIRead(MailPath, "Data Info", "NGfrequency", ""),                                                                //NG frequency
                        ab_create = ini12.INIRead(MailPath, "Data Info", "CreateTime", ""),                                                        //測試開始時間
                        ab_close = ini12.INIRead(MailPath, "Data Info", "CloseTime", ""),                                                          //測試結束時間
                        ab_time = ini12.INIRead(MailPath, "Total Test Time", "value", ""),                                                          //測試執行花費時間
                        ab_loop = Global.Schedule_Loop.ToString(),                                                          //執行loop次數
                        ab_loop_time = ini12.INIRead(MailPath, "Total Test Time", "value", ""),                                                //1個loop需要次數
                        ab_loop_step = (DataGridView1.Rows.Count - 1).ToString(),                                                //1個loop的step數
                        ab_root = ini12.INIRead(MailPath, "Data Info", "Reboot", ""),                                                          //測試重啟次數
                        ab_user = ini12.INIRead(MailPath, "Mail Info", "Tester", ""),                                                //測試人員
                        ab_mail = ini12.INIRead(MailPath, "Mail Info", "To", "");                                                          //Mail address 列表

            List<string> DataList = new List<string> { };
            DataList.Add(ab_num);
            DataList.Add(ab_p_id);
            DataList.Add(ab_c_id);
            DataList.Add(ab_result);
            DataList.Add(ab_version);
            DataList.Add(ab_ng);
            DataList.Add(ab_create);
            DataList.Add(ab_close);
            DataList.Add(ab_time);
            DataList.Add(ab_loop);
            DataList.Add(ab_loop_time);
            DataList.Add(ab_loop_step);
            DataList.Add(ab_root);
            DataList.Add(ab_user);
            DataList.Add(ab_mail);

            Form_DGV_Autobox.DataInsert(DataList);
            Form_DGV_Autobox.ToCsV(Form_DGV_Autobox.DGV_Autobox, "C:\\AutoTest v2\\Report.xls");
        }

        #region 另存Schedule
        private void WriteBtn_Click(object sender, EventArgs e)
        {
            string delimiter = ",";

            System.Windows.Forms.SaveFileDialog sfd = new System.Windows.Forms.SaveFileDialog();
            sfd.Filter = "CSV files (*.csv)|*.csv";
            sfd.FileName = ini12.INIRead(Application.StartupPath + "\\Config.ini", "Schedule" + Global.Schedule_Num, "Path", "");
            if (sfd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                using (System.IO.StreamWriter sw = new System.IO.StreamWriter(sfd.FileName, false))
                {
                    //output header data
                    string strHeader = "";
                    for (int i = 0; i < DataGridView1.Columns.Count; i++)
                    {
                        strHeader += DataGridView1.Columns[i].HeaderText + delimiter;
                    }
                    sw.WriteLine(strHeader);

                    //output rows data
                    for (int j = 0; j < DataGridView1.Rows.Count - 1; j++)
                    {
                        string strRowValue = "";

                        for (int k = 0; k < DataGridView1.Columns.Count; k++)
                        {
                            strRowValue += DataGridView1.Rows[j].Cells[k].Value + delimiter;
                        }
                        sw.WriteLine(strRowValue);
                    }
                    sw.Close();
                }
            }
            readSch();
        }
        #endregion

        #region Form1的Schedule 1~5按鈕功能
        private void SchBtn1_Click(object sender, EventArgs e)          ////////////Schedule1
        {
            Global.Schedule_Num = 1;
            string loop = ini12.INIRead(sPath, "Schedule1 Loop", "value", "");
            if (loop != "Enter Loop")
                Global.Schedule_Loop = int.Parse(loop);
            label2.Text = "Loop : " + Global.Schedule_Loop + " / ";
            SchBtn1.Enabled = false;
            SchBtn2.Enabled = true;
            SchBtn3.Enabled = true;
            SchBtn4.Enabled = true;
            SchBtn5.Enabled = true;
            readSch();
            ini12.INIWrite(MailPath, "Data Info", "TestCaseNumber", "0");
            ini12.INIWrite(MailPath, "Data Info", "Result", "N/A");
            ini12.INIWrite(MailPath, "Data Info", "NGfrequency", "0");
        }
        private void SchBtn2_Click(object sender, EventArgs e)          ////////////Schedule2
        {
            Global.Schedule_Num = 2;
            string loop = "";
            loop = ini12.INIRead(sPath, "Schedule2 Loop", "value", "");
            if (loop != "Enter Loop")
                Global.Schedule_Loop = int.Parse(loop);
            label2.Text = "Loop : " + Global.Schedule_Loop + " / ";
            SchBtn1.Enabled = true;
            SchBtn2.Enabled = false;
            SchBtn3.Enabled = true;
            SchBtn4.Enabled = true;
            SchBtn5.Enabled = true;
            readSch();
        }
        private void SchBtn3_Click(object sender, EventArgs e)          ////////////Schedule3
        {
            Global.Schedule_Num = 3;
            string loop = ini12.INIRead(sPath, "Schedule3 Loop", "value", "");
            if (loop != "Enter Loop")
                Global.Schedule_Loop = int.Parse(loop);
            label2.Text = "Loop : " + Global.Schedule_Loop + " / ";
            SchBtn1.Enabled = true;
            SchBtn2.Enabled = true;
            SchBtn3.Enabled = false;
            SchBtn4.Enabled = true;
            SchBtn5.Enabled = true;
            readSch();
        }
        private void SchBtn4_Click(object sender, EventArgs e)          ////////////Schedule4
        {
            Global.Schedule_Num = 4;
            string loop = ini12.INIRead(sPath, "Schedule4 Loop", "value", "");
            if (loop != "Enter Loop")
                Global.Schedule_Loop = int.Parse(loop);
            label2.Text = "Loop : " + Global.Schedule_Loop + " / ";
            SchBtn1.Enabled = true;
            SchBtn2.Enabled = true;
            SchBtn3.Enabled = true;
            SchBtn4.Enabled = false;
            SchBtn5.Enabled = true;
            readSch();
        }
        private void SchBtn5_Click(object sender, EventArgs e)          ////////////Schedule5
        {
            Global.Schedule_Num = 5;
            string loop = ini12.INIRead(sPath, "Schedule5 Loop", "value", "");
            if (loop != "Enter Loop")
                Global.Schedule_Loop = int.Parse(loop);
            label2.Text = "Loop : " + Global.Schedule_Loop + " / ";
            SchBtn1.Enabled = true;
            SchBtn2.Enabled = true;
            SchBtn3.Enabled = true;
            SchBtn4.Enabled = true;
            SchBtn5.Enabled = false;
            readSch();
        }
        public void readSch()
        {
            // Console.WriteLine(Global.Schedule_Num);
            // 戴入Schedule CSV 檔
            string fName = "";
            // 讀取ini中的路徑
            fName = ini12.INIRead(sPath, "Schedule" + Global.Schedule_Num, "Path", "");
            string Exist = ini12.INIRead(sPath, "Schedule" + Global.Schedule_Num + " Exist", "value", "");

            string TextLine = "";
            string[] SplitLine;
            int i = 0;
            if ((System.IO.File.Exists(fName) == true) && Exist == "1")
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
                objReader.Close();
            }
            else
            {
                MessageBox.Show("Schedule not exist", "CSV File Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                SchBtn1.PerformClick();
            }

            if (TextLine != "")
            {
                int j = Int32.Parse(TextLine.Split(',').Length.ToString());

                if (j == 11 || j == 10)
                {
                    long TotalDelay = 0;        //計算各個schedule測試時間>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
                    long RepeatTime = 0;
                    StartBtn.Enabled = true;
                    for (int z = 0; z < this.DataGridView1.Rows.Count - 1; z++)
                    {
                        if (this.DataGridView1.Rows[z].Cells[9].Value.ToString() != "")
                        {
                            if (this.DataGridView1.Rows[z].Cells[2].Value.ToString() != "")
                            {
                                RepeatTime = (long.Parse(this.DataGridView1.Rows[z].Cells[1].Value.ToString())) * (long.Parse(this.DataGridView1.Rows[z].Cells[2].Value.ToString()));
                            }
                            TotalDelay += (long.Parse(this.DataGridView1.Rows[z].Cells[9].Value.ToString()) + RepeatTime);
                            RepeatTime = 0;
                        }
                    }       //$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$

                    if (ini12.INIRead(Application.StartupPath + "\\Config.ini", "RecVideo", "value", "") == "1")
                    {
                        ConvertToRealTime(((TotalDelay * Global.Schedule_Loop) + 63000) / 1000);
                    }
                    else
                    {
                        ConvertToRealTime((TotalDelay * Global.Schedule_Loop) / 1000);
                    }

                    switch (Global.Schedule_Num)
                    {
                        case 1:
                            Global.Schedule_Num1_Time = (TotalDelay * Global.Schedule_Loop) / 1000;
                            timeCount = Global.Schedule_Num1_Time;
                            progressBar1.Maximum = Convert.ToInt32(Global.Schedule_Num1_Time);
                            break;
                        case 2:
                            Global.Schedule_Num2_Time = (TotalDelay * Global.Schedule_Loop) / 1000;
                            timeCount = Global.Schedule_Num2_Time;
                            progressBar1.Maximum = Convert.ToInt32(Global.Schedule_Num2_Time);
                            break;
                        case 3:
                            Global.Schedule_Num3_Time = (TotalDelay * Global.Schedule_Loop) / 1000;
                            timeCount = Global.Schedule_Num3_Time;
                            progressBar1.Maximum = Convert.ToInt32(Global.Schedule_Num3_Time);
                            break;
                        case 4:
                            Global.Schedule_Num4_Time = (TotalDelay * Global.Schedule_Loop) / 1000;
                            timeCount = Global.Schedule_Num4_Time;
                            progressBar1.Maximum = Convert.ToInt32(Global.Schedule_Num4_Time);
                            break;
                        case 5:
                            Global.Schedule_Num5_Time = (TotalDelay * Global.Schedule_Loop) / 1000;
                            timeCount = Global.Schedule_Num5_Time;
                            progressBar1.Maximum = Convert.ToInt32(Global.Schedule_Num5_Time);
                            break;
                    }       //<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<
                }
                else
                {
                    StartBtn.Enabled = false;
                    MessageBox.Show("This csv file format error", "File Format Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        #endregion

        #region 測試時間
        private string ConvertToRealTime(long iMs)
        {
            long ms, s, h, d = new int();
            ms = 0; s = 0; h = 0; d = 0;
            string sResult = "";
            try
            {
                ms = iMs % 60;
                if (iMs >= 60)
                {
                    s = iMs / 60;
                    if (s >= 60)
                    {
                        h = s / 60;
                        s = s % 60;
                        if (h >= 24)
                        {
                            d = (h) / 24;
                            h = h % 24;
                        }
                    }
                }
                EndTimelabel.Text = "Test Time : " + d.ToString("0") + "d " + h.ToString("0") + "h " + s.ToString("0") + "m " + ms.ToString("0") + "s";
                ini12.INIWrite(MailPath, "Total Test Time", "value", d.ToString("0") + "d " + h.ToString("0") + "h " + s.ToString("0") + "m " + ms.ToString("0") + "s");

                // 寫入每個Schedule test time
                if (Global.Schedule_Num == 1)
                    ini12.INIWrite(MailPath, "Total Test Time", "value1", d.ToString("0") + "d " + h.ToString("0") + "h " + s.ToString("0") + "m " + ms.ToString("0") + "s");
                
                if (jbutton1 == true)
                {
                    switch (Global.Schedule_Num)
                    {
                        case 2:
                            ini12.INIWrite(MailPath, "Total Test Time", "value2", d.ToString("0") + "d " + h.ToString("0") + "h " + s.ToString("0") + "m " + ms.ToString("0") + "s");
                            break;
                        case 3:
                            ini12.INIWrite(MailPath, "Total Test Time", "value3", d.ToString("0") + "d " + h.ToString("0") + "h " + s.ToString("0") + "m " + ms.ToString("0") + "s");
                            break;
                        case 4:
                            ini12.INIWrite(MailPath, "Total Test Time", "value4", d.ToString("0") + "d " + h.ToString("0") + "h " + s.ToString("0") + "m " + ms.ToString("0") + "s");
                            break;
                        case 5:
                            ini12.INIWrite(MailPath, "Total Test Time", "value5", d.ToString("0") + "d " + h.ToString("0") + "h " + s.ToString("0") + "m " + ms.ToString("0") + "s");
                            break;
                    }
                }
            }
            catch
            {
                sResult = "Error!";
            }
            return sResult;
        }
        #endregion

        #region UI相關
        #region 陰影
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                if (!DesignMode)
                {
                    cp.ClassStyle |= CS_DROPSHADOW;
                }
                return cp;
            }
        }
        #endregion

        #region 關閉、縮小按鈕
        private void ClosePicBox_Click(object sender, EventArgs e)
        {
            CloseDtplay();
            CloseAutobox();
        }

        private void MiniPicBox_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }
        #endregion

        #region 滑鼠拖曳視窗
        private void gPanelTitleBack_MouseDown(object sender, MouseEventArgs e)
        {
            ReleaseCapture();
            SendMessage(this.Handle, WM_SYSCOMMAND, SC_MOVE + HTCAPTION, 0);        //調用移動無窗體控件函數
        }
        #endregion
        #endregion

        #region 電源指示燈控制
        private void pictureBox3_Click(object sender, EventArgs e)
        {
            if (jbutton1 == true)
            {
                // 電源開或關
                byte[] val1;
                val1 = new byte[2];
                val1[0] = 0;

                bool jSuccess = PL2303_GP0_Enable(hCOM, 1);
                if (jSuccess && pbutton1 == false)
                {
                    uint val;
                    val = (uint)int.Parse("1");
                    bool bSuccess = PL2303_GP0_SetValue(hCOM, val);
                    bool kbSuccess = PL2303_GP1_GetValue(hCOM, val1);
                    if (kbSuccess)
                    {
                        //if (val1[0] == 0)
                        {
                            pbutton1 = true;
                            pictureBox3.Image = Properties.Resources._01;
                        }
                        //else
                        //pictureBox3.Image = Properties.Resources._02;
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
                        {
                            pbutton1 = false;
                            pictureBox3.Image = Properties.Resources._02;
                        }
                        //else
                        //pictureBox3.Image = Properties.Resources._01;
                    }
                }
                else
                {
                    Log("GP0 output enable FAILED.");
                }
            }
        }
        #endregion        

        private void dataGridView1_DataError(object sender, DataGridViewDataErrorEventArgs anError)
        {
            DataGridView1.CausesValidation = false;
        }

        private void DataBtn_Click(object sender, EventArgs e)            //背景執行填入測試步驟然後匯出reprot>>>>>>>>>>>>>
        {
            Form_DGV_Autobox.ShowDialog();
        }

        private void PauseButton_Click(object sender, EventArgs e)      //暫停SCHEDULE
        {
            Pause = !Pause;

            if (Pause == true)
            {
                PauseButton.Text = "RESUME";
                StartBtn.Enabled = false;
                SchedulePause.Reset();
                //timer1.Stop();
            }
            else
            {
                PauseButton.Text = "PAUSE";
                StartBtn.Enabled = true;
                SchedulePause.Set();
                timer1.Start();
            }
        }
        
        public void timer1_Tick_1(object sender, EventArgs e)
        {
            timer1.Interval = 1000;
            Console.WriteLine(timeCount);
            if (timeCount > 0)
            {
                EndTimelabel.Text = (--timeCount).ToString();
                ConvertToRealTime(timeCount);
                progressBar1.Increment(1);
            }
            if (progressBar1.Value == progressBar1.Maximum)
                progressBar1.Value = 0;
        }

        private void TimerPanelbutton_Click(object sender, EventArgs e)
        {
            TimerPanel = !TimerPanel;

            if (TimerPanel == true)
            {
                this.panel1.Show();
                this.panel1.BringToFront();
            }
            else
                this.panel1.Hide();
        }
/*
        private void backgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
*/


        //<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<
    }

    public class Global     //全域變數
    {
        public static int break_sch = 0;        //定時器中斷變數
        public static int Schedule_Num = 0;
        public static int Schedule_Num1_Exist = 0;
        public static int Schedule_Num2_Exist = 0;
        public static int Schedule_Num3_Exist = 0;
        public static int Schedule_Num4_Exist = 0;
        public static int Schedule_Num5_Exist = 0;

        public static long Schedule_Num1_Time = 0;
        public static long Schedule_Num2_Time = 0;
        public static long Schedule_Num3_Time = 0;
        public static long Schedule_Num4_Time = 0;
        public static long Schedule_Num5_Time = 0;
        public static long Total_Time = 0;

        public static int loop_Num = 0;
        public static int Schedule_Loop = 999999;
        public static int Schedule_Step;
        public static int Total_Loop = 0;

        public static int caption_Num = 0;
        public static int caption_Sum = 0;
        public static int excel_Num = 0;

        public static int[] caption_NG_Num = new int[Schedule_Loop];
        public static int[] caption_Total_Num = new int[Schedule_Loop];
        public static float[] SumValue = new float[Schedule_Loop];
        public static int[] NGValue = new int[Global.Schedule_Loop];
        public static float[] NGRateValue = new float[Global.Schedule_Loop];
        //public static float[] ReferenceResult = new float[Schedule_Loop];

        public static bool FormSetting = true;
        public static bool FormSchedule = true;
        public static bool FormMail = true;
        public static bool FormLog = true;

        public static string RCDB = "";
    } 
}
