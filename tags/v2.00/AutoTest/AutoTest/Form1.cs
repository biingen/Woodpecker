using System;
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
using Microsoft.Win32.SafeHandles;
using RedRat.AVDeviceMngmt;
using RedRat.IR;
using RedRat.RedRat3;
using RedRat.RedRat3.USB;
using RedRat.USB;
using RedRat.Util;
using USBClassLibrary;
using Microsoft.Win32;

namespace AutoTest
{
    public partial class Form1 : Form
    {
        //private BackgroundWorker BackgroundWorker = new BackgroundWorker();
        //private Form_DGV_Autobox Form_DGV_Autobox = new Form_DGV_Autobox();

        string sPath = Application.StartupPath + "\\Config.ini";
        string MailPath = Application.StartupPath + "\\Mail.ini";

        private Capture capture = null;
        private Filters filters = null;
        IRedRat3 redRat3 = null;
        private bool _captureInProgress;
        private bool jbutton1 = false;
        private bool pbutton1 = false;
        bool Vread = false;
        string videostring = "";
        string srtstring = "";

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

            USBPort = new USBClass();       //USB Connection>>>>>>>>>>>>>>>
            USBDeviceProperties = new USBClass.DeviceProperties();
            USBPort.USBDeviceAttached += new USBClass.USBDeviceEventHandler(USBPort_USBDeviceAttached);
            USBPort.USBDeviceRemoved += new USBClass.USBDeviceEventHandler(USBPort_USBDeviceRemoved);
            USBPort.RegisterForDeviceChange(true, this);
            USBTryHubConnection();
            USBTryRedratConnection();
            USBTryCameraConnection();
            MyUSBHubDeviceConnected = false;
            MyUSBRedratDeviceConnected = false;
            MyUSBCameraDeviceConnected = false;     //<<<<<<<<<<<<<<<<<<<<<<<<<<<<

            this.notifyIcon = new System.Windows.Forms.NotifyIcon(this.components);     //縮小到系統夾>>>>>>>>>>>>>>
            this.notifyIcon.Icon = new Icon(Application.StartupPath + "\\Resources\\App.ico");
            this.notifyIcon.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.notifyIcon_MouseDoubleClick);   //<<<<<<<<<<<<<

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
            toolTip_Main.SetToolTip(this.WriteBtn, "Save schedule");
            toolTip_Main.SetToolTip(this.SchBtn1, "Schedule 1");
            toolTip_Main.SetToolTip(this.SchBtn2, "Schedule 2");
            toolTip_Main.SetToolTip(this.SchBtn3, "Schedule 3");
            toolTip_Main.SetToolTip(this.SchBtn4, "Schedule 4");
            toolTip_Main.SetToolTip(this.SchBtn5, "Schedule 5");        //<<<<<<<<<<<<<<<<<<

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
            if (USBClass.GetUSBDevice(uint.Parse("046D", System.Globalization.NumberStyles.AllowHexSpecifier), uint.Parse("082B", System.Globalization.NumberStyles.AllowHexSpecifier), ref USBDeviceProperties, false) ||
                USBClass.GetUSBDevice(uint.Parse("045E", System.Globalization.NumberStyles.AllowHexSpecifier), uint.Parse("0766", System.Globalization.NumberStyles.AllowHexSpecifier), ref USBDeviceProperties, false) ||
                USBClass.GetUSBDevice(uint.Parse("114D", System.Globalization.NumberStyles.AllowHexSpecifier), uint.Parse("8C00", System.Globalization.NumberStyles.AllowHexSpecifier), ref USBDeviceProperties, false) ||
                USBClass.GetUSBDevice(uint.Parse("1E4E", System.Globalization.NumberStyles.AllowHexSpecifier), uint.Parse("0102", System.Globalization.NumberStyles.AllowHexSpecifier), ref USBDeviceProperties, false) ||
                USBClass.GetUSBDevice(uint.Parse("046D", System.Globalization.NumberStyles.AllowHexSpecifier), uint.Parse("081B", System.Globalization.NumberStyles.AllowHexSpecifier), ref USBDeviceProperties, false) ||
                USBClass.GetUSBDevice(uint.Parse("046D", System.Globalization.NumberStyles.AllowHexSpecifier), uint.Parse("0826", System.Globalization.NumberStyles.AllowHexSpecifier), ref USBDeviceProperties, false))
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

        private void UsbConnect()       //TO DO: Inset your connection code here
        {
            // 寫入Autobox關閉
            ini12.INIWrite(sPath, "AutoboxDev", "value", "1");
        }

        private void UsbDisconnect()        //TO DO: Insert your disconnection code here
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

        //執行緒控制 textbox2
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
            Global.caption_Num++;
            capture.FrameEvent2 -= new Capture.HeFrame(CaptureDone);
            string fName = "";
            string compareFolder = "compare";

            // 讀取ini中的路徑
            fName = ini12.INIRead(sPath, "Video", "Path", "");

            Bitmap newBitmap = CloneBitmap(e);      // 圖片印字>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
            this.pictureBox4.Image = newBitmap;
            Graphics bitMap_g = Graphics.FromImage(this.pictureBox4.Image);//底圖
            Brush tb = new SolidBrush(Color.Red);
            Font textfont = new Font("微軟正黑體", 20, FontStyle.Bold);

            bitMap_g.DrawString(this.DataGridView1.Rows[Global.Schedule_Step - 1].Cells[0].Value.ToString(), textfont, tb, new PointF(5, 400));     //redrat command
            bitMap_g.DrawString(TimeLable.Text, textfont, tb, new PointF(5, 440));      //現在時間

            textfont.Dispose();
            tb.Dispose();
            bitMap_g.Dispose();

            string t = fName + "\\" + "pic-" + DateTime.Now.ToString("yyyyMMddHHmmss") + "_" + label1.Text + ".jpg";
            pictureBox4.Image.Save(t);      //<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<

            if (ini12.INIRead(Application.StartupPath + "\\Config.ini", "ImportDB", "value", "") == "1")
            {
                // Create Compare folder
                string comparePath = fName + "\\" + compareFolder;
                string compare = comparePath + "\\" + "cf-" + Global.loop_Num + "_" + Global.caption_Num + ".jpg";
                if (Directory.Exists(comparePath))
                {

                }
                else
                {
                    Directory.CreateDirectory(comparePath);
                }

                // 圖片比較
                newBitmap = CloneBitmap(e);
                newBitmap = SobelEdgeDetect(newBitmap);
                this.pictureBox4.Image = newBitmap;
                pictureBox4.Image.Save(compare);

                if (Global.loop_Num < 3 || Global.caption_Num < 1)
                {

                }
                else
                {
                    Thread MyCompareThread = new Thread(new ThreadStart(MyCompareCamd));
                    MyCompareThread.Start();
                }

                // 比較目錄中僅會留存五個loop的拍照內容
                if (Global.loop_Num > 6)
                {
                    if (File.Exists(fName + "\\" + "cf-" + (Global.loop_Num - 6) + "_" + Global.caption_Num + ".jpg") == true)
                    {
                        File.Delete(fName + "\\" + "cf-" + (Global.loop_Num - 6) + "_" + Global.caption_Num + ".jpg");
                    }
                }
            }
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

        // Sobel 
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

        // GetHisogram
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
            hCOM = (SafeFileHandle)stream.GetType().GetField("_handle", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(stream);
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
            hCOM = (SafeFileHandle)stream.GetType().GetField("_handle", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(stream);
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

            buff = new byte[serialPort1.BytesToRead];

            serialPort1.Read(buff, 0, buff.Length);

            string text = Encoding.Default.GetString(buff);
            serialPort1.DiscardInBuffer();

            textBox1.AppendText(text);
        }

        private void serialPort2_DataReceived(object sender, System.IO.Ports.SerialDataReceivedEventArgs e)     //接受com2傳來的資料>>>>>>>>>>>>
        {
            byte[] buff = null;

            buff = new byte[serialPort2.BytesToRead];

            serialPort2.Read(buff, 0, buff.Length);

            string text = Encoding.Default.GetString(buff);
            serialPort2.DiscardInBuffer();
            
            textBox2.AppendText(text);
        }

        private void Rs232save()        //儲存com1的log>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
        {
            string fName = "";

            // 讀取ini中的路徑
            fName = ini12.INIRead(sPath, "Log", "Path", "");
            string t = fName + "\\_Log1_" + DateTime.Now.ToString("yyyyMMddHHmmss") + "_" + label1.Text + ".txt";

            StreamWriter MYFILE = new StreamWriter(t, false, Encoding.Default);
            MYFILE.WriteLine(textBox1.Text);
            MYFILE.Close();
            txtbox1("", textBox1);
        }

        private void ExtRs232save()     //儲存com2的log>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
        {
            string fName = "";

            // 讀取ini中的路徑
            fName = ini12.INIRead(sPath, "Log", "Path", "");
            string t = fName + "\\_Log2_" + DateTime.Now.ToString("yyyyMMddHHmmss") + "_" + label1.Text + ".txt";

            StreamWriter MYFILE = new StreamWriter(t, false, Encoding.Default);
            MYFILE.WriteLine(textBox2.Text);
            MYFILE.Close();
            txtbox2("", textBox2);
        }



        #region 跑Schedule的指令集
        public void MyRunCamd()
        {
            // 執行緒
            int sRepeat = 0;
            int stime = 0;
            int SysDelay = 0;
            //int myloop = 0;

            //string fint = "";

            /*
            // 讀取ini中的路徑回圈數
            fint = ini12.INIRead(sPath, "Loop", "value", "");

            if (fint != "")
            {
                myloop = int.Parse(fint);
            }
            */
            Global.loop_Num = 1;

            for (int j = 1; j < Global.Schedule_Loop + 1; j++)
            {
                Global.caption_Num = 0;
                UpdateUI(j.ToString(), label1);
                int V_scroll = 1;

                lock (this)
                {
                    for (int i = 0; i < this.DataGridView1.Rows.Count - 1; i++)
                    {
                        Global.Schedule_Step = i;
                        if (jbutton1 == false)
                        {
                            j = Global.Schedule_Loop;
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
                                    //BackgroundWorker.RunWorkerAsync();
                                }
                                else
                                {
                                    MessageBox.Show("Camera not exist", "Camera Open Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                                    }
                                    videostring = DataGridView1.Rows[i].Cells[4].Value.ToString();
                                }
                                else
                                {
                                    MessageBox.Show("Camera not exist", "Camera Open Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                }
                            }

                            // 停止錄影

                            if (DataGridView1.Rows[i].Cells[4].Value.ToString() == "_stop")
                            {
                                if (ini12.INIRead(Application.StartupPath + "\\Config.ini", "CameraDev", "value", "") == "1")
                                {
                                    if (Vread == true)       //判斷是不是正在錄影
                                    {
                                        Vread = false;
                                        mysstop();      //先將先前的關掉
                                    }
                                    videostring = DataGridView1.Rows[i].Cells[4].Value.ToString();
                                }
                                else
                                {
                                    MessageBox.Show("Camera not exist", "Camera Open Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                }
                            }
                            /*
                            if (ini12.INIRead(Application.StartupPath + "\\Config.ini", "Comport", "value", "") == "1")
                            {
                                switch (DataGridView1.Rows[i].Cells[5].Value.ToString())
                                {
                                    case "_clearCom1":
                                        Log(""); //清除textbox1
                                        break;
                                    case "_saveCom1":
                                        Rs232save(); //存檔rs232
                                        break;
                                    default:
                                        //byte[] data = Encoding.Unicode.GetBytes(DataGridView1.Rows[i].Cells[5].Value.ToString());
                                        // string str = Convert.ToString(data);
                                        Log(DataGridView1.Rows[i].Cells[5].Value.ToString()); //送出的字串
                                        serialPort1.WriteLine(DataGridView1.Rows[i].Cells[5].Value.ToString() + (char)(13)); //發送數據 Rs232
                                        break;
                                }
                            }

                            if (ini12.INIRead(Application.StartupPath + "\\Config.ini", "ExtComport", "value", "") == "1")
                            {
                                switch (DataGridView1.Rows[i].Cells[5].Value.ToString())
                                {
                                    case "_clearCom2":
                                        ExtLog(""); //清除textbox2
                                        break;
                                    case "_saveCom2":
                                        ExtRs232save(); //存檔rs232
                                        break;
                                    default:
                                        //byte[] data = Encoding.Unicode.GetBytes(DataGridView1.Rows[i].Cells[5].Value.ToString());
                                        // string str = Convert.ToString(data);
                                        ExtLog(DataGridView1.Rows[i].Cells[5].Value.ToString()); //送出的字串
                                        serialPort2.WriteLine(DataGridView1.Rows[i].Cells[5].Value.ToString() + (char)(13)); //發送數據 Rs232
                                        break;
                                }
                            }*/
                            
                            // 控制電源開關
                            //開
                            if (DataGridView1.Rows[i].Cells[6].Value.ToString() == "_on")
                            {
                                if (ini12.INIRead(Application.StartupPath + "\\Config.ini", "AutoboxDev", "value", "") == "1")
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
                                            //if (val1[0] == 0)
                                            {
                                                pbutton1 = true;
                                                PowerBtn.Text = "POWER OFF";
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
                                }
                                else
                                {
                                    MessageBox.Show("Can't open autobox", "Autobox Open Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                }
                            }

                            //關
                            if (DataGridView1.Rows[i].Cells[6].Value.ToString() == "_off")
                            {
                                if (ini12.INIRead(Application.StartupPath + "\\Config.ini", "AutoboxDev", "value", "") == "1")
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
                                            {
                                                pbutton1 = false;
                                                PowerBtn.Text = "POWER ON";
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
                                }
                                else
                                {
                                    MessageBox.Show("Can't open autobox", "Autobox Open Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                }
                            }
                        }

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
                                Dektec.StartInfo.FileName = Application.StartupPath + "\\DektecPlayer\\DtPlay.exe";

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
                                MessageBox.Show("Redrat not exist", "Redrat Open Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
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

            if (Global.Schedule_Num2_Exist == 1 && Global.Schedule_Num == 1)
            {
                SchBtn2.PerformClick();
                MyRunCamd();
            }
            else if (
                Global.Schedule_Num3_Exist == 1 && Global.Schedule_Num == 1 ||
                Global.Schedule_Num3_Exist == 1 && Global.Schedule_Num == 2)
            {
                SchBtn3.PerformClick();
                MyRunCamd();
            }
            else if (
                Global.Schedule_Num4_Exist == 1 && Global.Schedule_Num == 1 ||
                Global.Schedule_Num4_Exist == 1 && Global.Schedule_Num == 2 ||
                Global.Schedule_Num4_Exist == 1 && Global.Schedule_Num == 3)
            {
                SchBtn4.PerformClick();
                MyRunCamd();
            }
            else if (
                Global.Schedule_Num5_Exist == 1 && Global.Schedule_Num == 1 ||
                Global.Schedule_Num5_Exist == 1 && Global.Schedule_Num == 2 ||
                Global.Schedule_Num5_Exist == 1 && Global.Schedule_Num == 3 ||
                Global.Schedule_Num5_Exist == 1 && Global.Schedule_Num == 4)
            {
                SchBtn5.PerformClick();
                MyRunCamd();
            }

            //全部schedule跑完或是按下stop鍵以後會跑以下這段>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
            if (jbutton1 == false)      //按下stop*******************************************************
            {
                UpdateUI("START", StartBtn);
                StartBtn.Enabled = true;
                SettingBtn.Enabled = true;
                WriteBtn.Enabled = true;
                PowerBtn.Enabled = false;
                CamPreviewBtn.Enabled = true;

                close232();
                Extclose232();
                _captureInProgress = false;
                OnOffCamera();

                /*
                if (Directory.Exists(ini12.INIRead(Application.StartupPath + "\\Config.ini", "Video", "Path", "") + "\\" + "compare") == true)
                {
                    DirectoryInfo DIFO = new DirectoryInfo(ini12.INIRead(Application.StartupPath + "\\Config.ini", "Video", "Path", "") + "\\" + "compare");
                    DIFO.Delete(true);
                }
                */
            }
            else
            {
                jbutton1 = false;       //全部schedule自動跑完************************************************

                UpdateUI("START", StartBtn);
                SettingBtn.Enabled = true;
                WriteBtn.Enabled = true;
                PowerBtn.Enabled = false;
                CamPreviewBtn.Enabled = true;
                
                close232();
                Extclose232();
                _captureInProgress = false;
                OnOffCamera();

                Global.Total_Time = Global.Schedule_Num1_Time + Global.Schedule_Num2_Time + Global.Schedule_Num3_Time + Global.Schedule_Num4_Time + Global.Schedule_Num5_Time;
                ConvertToRealTime(Global.Total_Time);
                if (ini12.INIRead(MailPath, "Send Mail", "value", "") == "1")
                {
                    FormMail FormMail = new FormMail();
                    FormMail.send();
                }

                /*
                if (Directory.Exists(ini12.INIRead(Application.StartupPath + "\\Config.ini", "Video", "Path", "") + "\\" + "compare") == true)
                {
                    DirectoryInfo DIFO = new DirectoryInfo(ini12.INIRead(Application.StartupPath + "\\Config.ini", "Video", "Path", "") + "\\" + "compare");
                    DIFO.Delete(true);
                }
                */
            }
            RedratLable.Text = "AutoTest finished!";
            label1.Text = "------------------";     //<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<
        }
        #endregion

        // 圖片比較執行緒
        private void MyCompareCamd()
        {
            String fName = "";

            // 讀取ini中的路徑
            fName = ini12.INIRead(sPath, "Video", "Path", "") + "\\" + "compare";
            string pathCompare1 = fName + "\\" + "cf-" + (Global.loop_Num - 1) + "_" + Global.caption_Num + ".jpg";
            string pathCompare2 = fName + "\\" + "cf-" + (Global.loop_Num - 2) + "_" + Global.caption_Num + ".jpg";
            Bitmap picCompare1 = (Bitmap)Image.FromFile(pathCompare1);
            Bitmap picCompare2 = (Bitmap)Image.FromFile(pathCompare2);
            int[] GetHisogram1 = GetHisogram(picCompare1);
            int[] GetHisogram2 = GetHisogram(picCompare2);
            float CompareResult = GetResult(GetHisogram1, GetHisogram2);
            //Console.WriteLine(CompareResult);
            //Console.WriteLine(pathCompare1);
            //Console.WriteLine(pathCompare2);
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

            else if (_captureInProgress == false)
            {
                capture.Stop();
                capture.Dispose();
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

            //讀取Redrat的設定值來顯示燈號
            if (ini12.INIRead(Application.StartupPath + "\\Config.ini", "RedratDev", "value", "") == "1")
                pictureBox1.Image = Properties.Resources._01;
            else
                pictureBox1.Image = Properties.Resources._02;       //<<<<<<<<<<<<<<<<<<<<<<<<<<

            //讀取Camera的設定值來顯示燈號
            if (ini12.INIRead(Application.StartupPath + "\\Config.ini", "CameraDev", "value", "") == "1")
            {
                pictureBox2.Image = Properties.Resources._01;
                filters = new Filters();
            }
            else
            {
                pictureBox2.Image = Properties.Resources._02;
            }//<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<

            if (status == true)
            {
                SchBtn1.PerformClick();
                Thread oThreadB = new Thread(new ThreadStart(MyRunCamd));

                if (jbutton1 == true)
                {
                    //oThreadB.Abort();//停止執行緒
                    if (ini12.INIRead(Application.StartupPath + "\\Config.ini", "IR Device", "value", "") == "2")
                        close232(); // 關閉232

                    if (ini12.INIRead(Application.StartupPath + "\\Config.ini", "Comport", "value", "") == "1")
                        close232(); // 關閉rs232

                    if (ini12.INIRead(Application.StartupPath + "\\Config.ini", "ExtComport", "value", "") == "1")
                        Extclose232(); // 關閉rs232

                    PowerBtn.Enabled = false;
                    jbutton1 = false;
                    StartBtn.Enabled = false;
                    SettingBtn.Enabled = false;
                    WriteBtn.Enabled = false;
                    
                    CloseDtplay();
                    RedratLable.Text = "Please wait...";
                }
                else
                {
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
                    PowerBtn.Enabled = true;
                    SettingBtn.Enabled = false;
                    WriteBtn.Enabled = false;
                    oThreadB.Start();// 啟動執行緒
                    bool bSuccess = PL2303_GP1_GetValue(hCOM, val);
                    if (bSuccess)
                    {
                        if (val[0] == 0)
                        {
                            pbutton1 = false;
                            PowerBtn.Text = "POWER ON";
                            pictureBox3.Image = Properties.Resources._02;
                        }
                        else
                        {
                            pbutton1 = true;
                            PowerBtn.Text = "POWER OFF";
                            pictureBox3.Image = Properties.Resources._01;
                        }
                    }
                    jbutton1 = true;
                    StartBtn.Text = "STOP";
                    CamPreviewBtn.PerformClick();
                }
            }
            else
            {
                Thread oThreadB = new Thread(new ThreadStart(MyRunCamd));
                if (jbutton1 == true)
                {
                    oThreadB.Abort();//停止執行緒
                    PowerBtn.Enabled = false;
                    jbutton1 = false;
                    StartBtn.Enabled = false;
                    SettingBtn.Enabled = false;
                    WriteBtn.Enabled = false;
                    CloseDtplay();
                    RedratLable.Text = "Please wait...";
                }
                else
                {
                    oThreadB.Start();// 啟動執行緒
                    jbutton1 = true;
                    PowerBtn.Enabled = false;
                    SettingBtn.Enabled = false;
                    pictureBox3.Image = Properties.Resources._02;
                }
            }
        }

        private void SettingBtn_Click(object sender, EventArgs e)
        {
            SettingBtn.Enabled = false;
            FormTabControl FormTabControl = new FormTabControl();

            if (FormTabControl.ShowDialog() == DialogResult.OK)     //關閉FormTabControl以後會讀這段>>>>>>>>>>>>>>>>>>>>>>>
            {
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

            bool jSuccess = PL2303_GP0_Enalbe(hCOM, 1);
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
                        PowerBtn.Text = "POWER OFF";
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
                        PowerBtn.Text = "POWER ON";
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
            TimeLable.Text = string.Format("{0:R}", dt);
            //TimeLable.Text = DateTime.Now.ToLongDateString() + "   " + DateTime.Now.ToLongTimeString();
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
            SchBtn1.Enabled = false;
            SchBtn2.Enabled = true;
            SchBtn3.Enabled = true;
            SchBtn4.Enabled = true;
            SchBtn5.Enabled = true;
            readSch();
        }
        private void SchBtn2_Click(object sender, EventArgs e)          ////////////Schedule2
        {
            Global.Schedule_Num = 2;
            string loop = "";
            loop = ini12.INIRead(sPath, "Schedule2 Loop", "value", "");
            if (loop != "Enter Loop")
                Global.Schedule_Loop = int.Parse(loop);
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
            SchBtn1.Enabled = true;
            SchBtn2.Enabled = true;
            SchBtn3.Enabled = true;
            SchBtn4.Enabled = true;
            SchBtn5.Enabled = false;
            readSch();
        }
        public void readSch()
        {
            Console.WriteLine(Global.Schedule_Num);
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
                Console.WriteLine(TextLine);
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
                            break;
                        case 2:
                            Global.Schedule_Num2_Time = (TotalDelay * Global.Schedule_Loop) / 1000;
                            break;
                        case 3:
                            Global.Schedule_Num3_Time = (TotalDelay * Global.Schedule_Loop) / 1000;
                            break;
                        case 4:
                            Global.Schedule_Num4_Time = (TotalDelay * Global.Schedule_Loop) / 1000;
                            break;
                        case 5:
                            Global.Schedule_Num5_Time = (TotalDelay * Global.Schedule_Loop) / 1000;
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
                RedratLable.Text = "Test Time : " + d.ToString("0") + "d " + h.ToString("0") + "h " + s.ToString("0") + "m " + ms.ToString("0") + "s";
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
            this.notifyIcon.Visible = false;
        }

        private void MiniPicBox_Click(object sender, EventArgs e)
        {
            this.Hide();
            this.WindowState = FormWindowState.Minimized;
            this.notifyIcon.Visible = true;
            this.notifyIcon.ShowBalloonTip(100, "", "AutoTest is running", ToolTipIcon.Info);
        }

        private void notifyIcon_MouseDoubleClick(object sender, EventArgs e)        // 雙擊ICON打開程式
        {
            this.notifyIcon.Visible = false;
            this.Show();
            this.WindowState = FormWindowState.Normal;
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

                bool jSuccess = PL2303_GP0_Enalbe(hCOM, 1);
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
                            PowerBtn.Text = "POWER OFF";
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
                            PowerBtn.Text = "POWER ON";
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

        /*
        private void DataBtn_Click(object sender, EventArgs e)            //背景執行填入測試步驟然後匯出reprot>>>>>>>>>>>>>
        {
            Form_DGV_Autobox.ShowDialog();
        }
        private void backgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            string ab_num = Global.Schedule_Num.ToString(),     //自動編號
                        ab_p_id = "0",                                                              //Project number
                        ab_c_id = "0",                                                              //Test case number
                        ab_result = "NG",                                                       //AutoTest 測試結果
                        ab_version = "v0.7788",                                             //軟體版號
                        ab_ng = "0",                                                                //NG frequency
                        ab_create = "0",                                                        //測試開始時間
                        ab_close = "0",                                                          //測試結束時間
                        ab_time = "0",                                                          //測試執行花費時間
                        ab_loop = "0",                                                          //執行loop次數
                        ab_loop_time = "0",                                                //1個loop需要次數
                        ab_loop_step = "0",                                                //1個loop的step數
                        ab_root = "0",                                                          //測試重啟次數
                        ab_user = "馬英告",                                                //測試人員
                        ab_mail = "0";                                                          //Mail address 列表

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
            Form_DGV_Autobox.ToCsV(Form_DGV_Autobox.DGV_Autobox, "C:\\Report.xls");
        }*/
        //<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<
    }

    public class Global     //全域變數
    {
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
        public static int Schedule_Loop = 0;
        public static int Schedule_Step;
        public static int Total_Loop = 0;

        public static int caption_Num = 0;

        public static bool FormSetting = true;
        public static bool FormSchedule = true;
        public static bool FormMail = true;
    } 
}
