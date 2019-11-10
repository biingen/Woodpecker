using BlueRatLibrary;
using DirectX.Capture;
using jini;
using Microsoft.Win32.SafeHandles;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Management;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Timers;
using System.Windows.Forms;
using System.Xml.Linq;
using USBClassLibrary;
using System.Net.Sockets;
using System.Net;
using Can_Reader_Lib;
using BlockMessageLibrary;
using DTC_ABS;
using DTC_OBD;
using MySerialLibrary;
using KWP_2000;
using MaterialSkin.Controls;
using MaterialSkin;
//using NationalInstruments.DAQmx;

namespace Woodpecker
{
    public partial class Form1 : MaterialForm
    {
        private string _args;
        //private BackgroundWorker BackgroundWorker = new BackgroundWorker();
        //private Form_DGV_Autobox Form_DGV_Autobox = new Form_DGV_Autobox();
        //private TextBoxBuffer textBoxBuffer = new TextBoxBuffer(4096);

        private string MainSettingPath = Application.StartupPath + "\\Config.ini";
        private string RcPath = Application.StartupPath + "\\RC.ini";

        private Add_ons Add_ons = new Add_ons();
        private RedRatDBParser RedRatData = new RedRatDBParser();
        private BlueRat MyBlueRat = new BlueRat();
        private static bool BlueRat_UART_Exception_status = false;

        private static void BlueRat_UARTException(Object sender, EventArgs e)
        {
            BlueRat_UART_Exception_status = true;
        }

        private bool FormIsClosing = false;
        private Capture capture = null;
        private Filters filters = null;
        private bool _captureInProgress;
        private bool StartButtonPressed = false;//true = 按下START//false = 按下STOP//
        private bool VideoRecording = false;//是否正在錄影//
        private bool AcUsbPanel = false;
        private long timeCount = 0;
        private long TestTime = 0;
        private string videostring = "";
        private string srtstring = "";

        //Schedule暫停用的參數
        private bool Pause = false;
        private ManualResetEvent SchedulePause = new ManualResetEvent(true);
        private ManualResetEvent ScheduleWait = new ManualResetEvent(true);

        private SafeDataGridView portos_online;
        private int Breakpoint;
        private int Nowpoint;
        private bool Breakfunction = false;
        //private const int CS_DROPSHADOW = 0x20000;      //宣告陰影參數

        private MySerial MySerialPort = new MySerial();
        private List<BlockMessage> MyBlockMessageList = new List<BlockMessage>();
        private ProcessBlockMessage MyProcessBlockMessage = new ProcessBlockMessage();

        //拖動無窗體的控件>>>>>>>>>>>>>>>>>>>>
        [DllImport("user32.dll")]
        public static extern bool ReleaseCapture();

        [DllImport("user32.dll")]
        public static extern bool SendMessage(IntPtr hwnd, int wMsg, int wParam, int lParam);
        public const int WM_SYSCOMMAND = 0x0112;
        public const int SC_MOVE = 0xF010;
        public const int HTCAPTION = 0x0002;
        //<<<<<<<<<<<<<<<<<<<<<<<<<<<<<

        [DllImport("user32.dll")]
        public static extern bool SetForegroundWindow(IntPtr hWnd);

        //CanReader
        private CAN_Reader MYCanReader = new CAN_Reader();

        //Klite error code
        public int kline_send = 0;
        public List<DTC_Data> ABS_error_list = new List<DTC_Data>();
        public List<DTC_Data> OBD_error_list = new List<DTC_Data>();

        //Serial Port parameter
        public delegate void AddDataDelegate(String myString);
        public AddDataDelegate myDelegate1;
        private String log1_text, log2_text, log3_text, log4_text, log5_text, canbus_text, kline_text, schedule_text, logAll_text;

        public Form1()
        {
            InitializeComponent();
            setStyle();

            //Datagridview design
            DataGridView_Schedule.Rows[Global.Scheduler_Row].DefaultCellStyle.BackColor = Color.FromArgb(56, 56, 56);
            DataGridView_Schedule.Rows[Global.Scheduler_Row].DefaultCellStyle.ForeColor = Color.FromArgb(255, 255, 255);
            DataGridView_Schedule.Columns[0].DefaultCellStyle.BackColor = Color.FromArgb(56, 56, 56);
            DataGridView_Schedule.Columns[0].DefaultCellStyle.ForeColor = Color.FromArgb(255, 255, 255);

            initComboboxSaveLog();

            //USB Connection//
            USBPort = new USBClass();
            USBDeviceProperties = new USBClass.DeviceProperties();
            USBPort.USBDeviceAttached += new USBClass.USBDeviceEventHandler(USBPort_USBDeviceAttached);
            USBPort.USBDeviceRemoved += new USBClass.USBDeviceEventHandler(USBPort_USBDeviceRemoved);
            USBPort.RegisterForDeviceChange(true, this);
            USBTryCameraConnection();
            MyUSBRedratDeviceConnected = false;
            MyUSBCameraDeviceConnected = false;
        }

        public Form1(string value)
        {
            InitializeComponent();
            setStyle();
            if (!string.IsNullOrEmpty(value))
            {
                _args = value;
            }
            USBPort = new USBClass();
            USBDeviceProperties = new USBClass.DeviceProperties();
            USBPort.USBDeviceAttached += new USBClass.USBDeviceEventHandler(USBPort_USBDeviceAttached);
            USBPort.USBDeviceRemoved += new USBClass.USBDeviceEventHandler(USBPort_USBDeviceRemoved);
            USBPort.RegisterForDeviceChange(true, this);
            USBTryCameraConnection();
            MyUSBRedratDeviceConnected = false;
            MyUSBCameraDeviceConnected = false;
        }

        private void setStyle()
        {
            // Form design
            this.MinimumSize = new Size(1097, 659);
            this.BackColor = Color.FromArgb(18, 18, 18);

            //Init material skin
            var skinManager = MaterialSkinManager.Instance;
            skinManager.AddFormToManage(this);
            skinManager.Theme = MaterialSkinManager.Themes.DARK;
            skinManager.ColorScheme = new ColorScheme(Primary.BlueGrey800, Primary.BlueGrey900, Primary.BlueGrey500, Accent.LightBlue200, TextShade.WHITE);

            // Button design
            List<Button> buttonsList = new List<Button> { button_Start, button_Setting, button_Pause, button_Schedule, button_Camera, button_AcUsb,
                                                            button_InsertRow, button_SaveSchedule, button_Schedule1, button_Schedule2, button_Schedule3,
                                                            button_Schedule4, button_Schedule5, button_savelog};
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

        private void initComboboxSaveLog()
        {
            List<string> portList = new List<string> { "Port A", "Port B", "Port C", "Port D", "Port E", "Kline" };

            foreach (string port in portList)
            {
                if (ini12.INIRead(MainSettingPath, port, "Checked", "") == "1")
                {
                    comboBox_savelog.Items.Add(port);
                }
                else if (ini12.INIRead(MainSettingPath, port, "Checked", "") == "0" || ini12.INIRead(MainSettingPath, port, "Checked", "") == "")
                {
                    comboBox_savelog.Items.Remove(port);
                }
            }

            if (comboBox_savelog.Items.Count == 0)
            {
                button_savelog.Enabled = false;
                comboBox_savelog.Enabled = false;
            }

            else
            {
                button_savelog.Enabled = true;
                comboBox_savelog.Enabled = true;
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.TopMost = true;
            this.WindowState = FormWindowState.Normal;

            //根據dpi調整視窗尺寸
            Graphics graphics = CreateGraphics();
            float dpiX = graphics.DpiX;
            float dpiY = graphics.DpiY;
            /*if (dpiX == 96 && dpiY == 96)
            {
                this.Height = 600;
                this.Width = 1120;
            }*/
            int intPercent = (dpiX == 96) ? 100 : (dpiX == 120) ? 125 : 150;

            // 針對字體變更Form的大小
            this.Height = this.Height * intPercent / 100;

            if (ini12.INIRead(MainSettingPath, "Device", "AutoboxExist", "") == "1")
            {
                if (ini12.INIRead(MainSettingPath, "Device", "AutoboxVerson", "") == "2")
                {
                    ConnectAutoBox2();
                }

                pictureBox_BlueRat.Image = Properties.Resources.ON;
                GP0_GP1_AC_ON();
                GP2_GP3_USB_PC();
            }
            else
            {
                pictureBox_BlueRat.Image = Properties.Resources.OFF;
                pictureBox_AcPower.Image = Properties.Resources.OFF;
                pictureBox_ext_board.Image = Properties.Resources.OFF;
                pictureBox_canbus.Image = Properties.Resources.OFF;
            }

            if (ini12.INIRead(MainSettingPath, "Device", "CameraExist", "") == "1")
            {
                pictureBox_Camera.Image = Properties.Resources.ON;
                filters = new Filters();
                Filter f;

                comboBox_CameraDevice.Enabled = true;
                ini12.INIWrite(MainSettingPath, "Camera", "VideoNumber", filters.VideoInputDevices.Count.ToString());

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
                comboBox_CameraDevice.Enabled = false;
            }
            else
            {
                pictureBox_Camera.Image = Properties.Resources.OFF;
            }

            LoadRCDB();
            ConnectCanBus();

            List<string> SchExist = new List<string> { };
            for (int i = 2; i < 6; i++)
            {
                SchExist.Add(ini12.INIRead(MainSettingPath, "Schedule" + i, "Exist", ""));
            }

            if (SchExist[0] != "")
            {
                if (SchExist[0] == "0")
                    button_Schedule2.Visible = false;
                else
                    button_Schedule2.Visible = true;
            }
            else
            {
                SchExist[0] = "0";
                button_Schedule2.Visible = false;
            }

            if (SchExist[1] != "")
            {
                if (SchExist[1] == "0")
                    button_Schedule3.Visible = false;
                else
                    button_Schedule3.Visible = true;
            }
            else
            {
                SchExist[1] = "0";
                button_Schedule3.Visible = false;
            }

            if (SchExist[2] != "")
            {
                if (SchExist[2] == "0")
                    button_Schedule4.Visible = false;
                else
                    button_Schedule4.Visible = true;
            }
            else
            {
                SchExist[2] = "0";
                button_Schedule4.Visible = false;
            }

            if (SchExist[3] != "")
            {
                if (SchExist[3] == "0")
                    button_Schedule5.Visible = false;
                else
                    button_Schedule5.Visible = true;
            }
            else
            {
                SchExist[3] = "0";
                button_Schedule5.Visible = false;
            }

            Global.Schedule_2_Exist = int.Parse(SchExist[0]);
            Global.Schedule_3_Exist = int.Parse(SchExist[1]);
            Global.Schedule_4_Exist = int.Parse(SchExist[2]);
            Global.Schedule_5_Exist = int.Parse(SchExist[3]);

            button_Pause.Enabled = false;
            button_Schedule.PerformClick();
            button_Schedule1.PerformClick();
            CheckForIllegalCrossThreadCalls = false;
            TopMost = true;
            TopMost = false;

            setStyle();
        }

        #region -- USB Detect --
        //暫時移除有關盒子的插拔偵測，因為有其他無相關裝置運用到相同的VID和PID
        private bool USBTryBoxConnection()
        {
            if (Global.AutoBoxComport.Count != 0)
            {
                for (int i = 0; i < Global.AutoBoxComport.Count; i++)
                {
                    if (USBClass.GetUSBDevice(
                        uint.Parse("067B", System.Globalization.NumberStyles.AllowHexSpecifier),
                        uint.Parse("2303", System.Globalization.NumberStyles.AllowHexSpecifier),
                        ref USBDeviceProperties,
                        true))
                    {
                        if (Global.AutoBoxComport[i] == "COM15")
                        {
                            BoxConnect();
                        }
                    }
                }
                return true;
            }
            else
            {
                BoxDisconnect();
                return false;
            }
        }

        private bool USBTryCameraConnection()
        {
            int DeviceNumber = Global.VID.Count;
            int VidCount = Global.VID.Count - 1;
            int PidCount = Global.PID.Count - 1;

            if (DeviceNumber != 0)
            {
                for (int i = 0; i < DeviceNumber; i++)
                {
                    if (USBClass.GetUSBDevice(uint.Parse(Global.VID[i], style: System.Globalization.NumberStyles.AllowHexSpecifier), uint.Parse(Global.PID[i], System.Globalization.NumberStyles.AllowHexSpecifier), ref USBDeviceProperties, false))
                    {
                        CameraConnect();
                    }
                }
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
            if (!MyUSBCameraDeviceConnected)
            {
                if (USBTryCameraConnection() == true)
                {
                    MyUSBCameraDeviceConnected = true;
                }
            }
        }

        private void USBPort_USBDeviceRemoved(object sender, USBClass.USBDeviceEventArgs e)
        {
            int DeviceNumber = Global.VID.Count;

            if (DeviceNumber != 0)
            {
                for (int i = 0; i < DeviceNumber; i++)
                {
                    if (!USBClass.GetUSBDevice(uint.Parse(Global.VID[i], style: System.Globalization.NumberStyles.AllowHexSpecifier), uint.Parse(Global.PID[i], System.Globalization.NumberStyles.AllowHexSpecifier), ref USBDeviceProperties, false))
                    {
                        MyUSBCameraDeviceConnected = false;
                        USBTryCameraConnection();
                    }
                }
            }
        }

        private void BoxConnect()       //TO DO: Inset your connection code here
        {
            pictureBox_BlueRat.Image = Properties.Resources.ON;
        }

        private void BoxDisconnect()        //TO DO: Insert your disconnection code here
        {
            pictureBox_BlueRat.Image = Properties.Resources.OFF;
        }

        private void CameraConnect()        //TO DO: Inset your connection code here
        {
            if (ini12.INIRead(MainSettingPath, "Device", "Name", "") != "")
            {
                ini12.INIWrite(MainSettingPath, "Device", "CameraExist", "1");
                pictureBox_Camera.Image = Properties.Resources.ON;
                if (StartButtonPressed == false)
                    button_Camera.Enabled = true;
            }
        }

        private void CameraDisconnect()     //TO DO: Insert your disconnection code here
        {
            ini12.INIWrite(MainSettingPath, "Device", "CameraExist", "0");
            pictureBox_Camera.Image = Properties.Resources.OFF;
            if (StartButtonPressed == false)
                button_Camera.Enabled = false;
        }

        protected override void WndProc(ref Message m)
        {
            USBPort.ProcessWindowsMessage(ref m);
            base.WndProc(ref m);
        }
        #endregion

        private void OnCaptureComplete(object sender, EventArgs e)
        {
            // Demonstrate the Capture.CaptureComplete event.
            Debug.WriteLine("Capture complete.");
        }

        //執行緒控制label.text
        private delegate void UpdateUICallBack(string value, Control ctl);
        private void UpdateUI(string value, Control ctl)
        {
            if (InvokeRequired)
            {
                UpdateUICallBack uu = new UpdateUICallBack(UpdateUI);
                Invoke(uu, value, ctl);
            }
            else
            {
                ctl.Text = value;
            }
        }

        //執行緒控制 datagriveiew
        private delegate void UpdateUICallBack1(string value, DataGridView ctl);
        private void GridUI(string i, DataGridView gv)
        {
            if (InvokeRequired)
            {
                UpdateUICallBack1 uu = new UpdateUICallBack1(GridUI);
                Invoke(uu, i, gv);
            }
            else
            {
                DataGridView_Schedule.ClearSelection();
                gv.Rows[int.Parse(i)].Selected = true;
            }
        }

        // 執行緒控制 datagriverew的scorllingbar
        private delegate void UpdateUICallBack3(string value, DataGridView ctl);
        private void Gridscroll(string i, DataGridView gv)
        {
            if (InvokeRequired)
            {
                UpdateUICallBack3 uu = new UpdateUICallBack3(Gridscroll);
                Invoke(uu, i, gv);
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
        private void Txtbox1(string value, Control ctl)
        {
            if (InvokeRequired)
            {
                UpdateUICallBack2 uu = new UpdateUICallBack2(Txtbox1);
                Invoke(uu, value, ctl);
            }
            else
            {
                ctl.Text = value;
            }
        }

        //執行緒控制 txtbox2
        private delegate void UpdateUICallBack4(string value, Control ctl);
        private void Txtbox2(string value, Control ctl)
        {
            if (InvokeRequired)
            {
                UpdateUICallBack4 uu = new UpdateUICallBack4(Txtbox2);
                Invoke(uu, value, ctl);
            }
            else
            {
                ctl.Text = value;
            }
        }

        //執行緒控制 txtbox3
        private delegate void UpdateUICallBack5(string value, Control ctl);
        private void Txtbox3(string value, Control ctl)
        {
            if (InvokeRequired)
            {
                UpdateUICallBack5 uu = new UpdateUICallBack5(Txtbox3);
                Invoke(uu, value, ctl);
            }
            else
            {
                ctl.Text = value;
            }
        }

        #region -- 拍照 --
        private void Jes() => Invoke(new EventHandler(delegate { Myshot(); }));

        private void Myshot()
        {
            button_Start.Enabled = false;
            setStyle();
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
            string fName = ini12.INIRead(MainSettingPath, "Record", "VideoPath", "");

            //圖片印字
            Bitmap newBitmap = CloneBitmap(e);
            newBitmap = CloneBitmap(e);
            pictureBox4.Image = newBitmap;

            Graphics bitMap_g = Graphics.FromImage(pictureBox4.Image);//底圖
            Font Font = new Font("Microsoft JhengHei Light", 16, FontStyle.Bold);
            Brush FontColor = new SolidBrush(Color.Red);
            string[] Resolution = ini12.INIRead(MainSettingPath, "Camera", "Resolution", "").Split('*');
            int YPoint = int.Parse(Resolution[1]);

            //照片印上現在步驟//
            if (DataGridView_Schedule.Rows[Global.Schedule_Step].Cells[0].Value.ToString() == "_shot")
            {
                if (Global.Schedule_Step == 0)
                {
                    bitMap_g.DrawString("  ( " + label_Command.Text + " )",
                                    Font,
                                    FontColor,
                                    new PointF(5, YPoint - 80));
                }
                else
                {
                    bitMap_g.DrawString(DataGridView_Schedule.Rows[Global.Schedule_Step].Cells[9].Value.ToString(),
                                    Font,
                                    FontColor,
                                    new PointF(5, YPoint - 120));
                    bitMap_g.DrawString(DataGridView_Schedule.Rows[Global.Schedule_Step].Cells[0].Value.ToString() + "  ( " + label_Command.Text + " )",
                                    Font,
                                    FontColor,
                                    new PointF(5, YPoint - 80));
                }
            }
            else
            {
                bitMap_g.DrawString(DataGridView_Schedule.Rows[Global.Schedule_Step].Cells[9].Value.ToString(),
                                Font,
                                FontColor,
                                new PointF(5, YPoint - 120));
                bitMap_g.DrawString(DataGridView_Schedule.Rows[Global.Schedule_Step].Cells[0].Value.ToString() + "  ( " + label_Command.Text + " )",
                                    Font,
                                    FontColor,
                                    new PointF(5, YPoint - 80));
            }

            //照片印上現在時間//
            bitMap_g.DrawString(TimeLabel.Text,
                                Font,
                                FontColor,
                                new PointF(5, YPoint - 40));

            Font.Dispose();
            FontColor.Dispose();
            bitMap_g.Dispose();

            string t = fName + "\\" + "pic-" + DateTime.Now.ToString("yyyyMMddHHmmss") + "(" + label_LoopNumber_Value.Text + "-" + Global.caption_Num + ").png";
            pictureBox4.Image.Save(t);
            button_Start.Enabled = true;
            setStyle();
        }
        #endregion

        private void ConnectAutoBox2()
        {
            uint temp_version;
            string curItem = ini12.INIRead(MainSettingPath, "Device", "AutoboxPort", "");
            if (MyBlueRat.Connect(curItem) == true)
            {
                temp_version = MyBlueRat.FW_VER;
                float v = temp_version;
                label_BoxVersion.Text = "_" + (v / 100).ToString();

                // 在第一次/或長時間未使用之後,要開始使用BlueRat跑Schedule之前,建議執行這一行,確保BlueRat的起始狀態一致 -- 正常情況下不執行並不影響BlueRat運行,但為了找問題方便,還是請務必執行
                MyBlueRat.Force_Init_BlueRat();
                MyBlueRat.Reset_SX1509();

                byte SX1509_detect_status;
                SX1509_detect_status = MyBlueRat.TEST_Detect_SX1509();

                if (SX1509_detect_status == 3)
                {
                    pictureBox_ext_board.Image = Properties.Resources.ON;
                    // Error, need to check SX1509 connection
                }
                else
                {
                    pictureBox_ext_board.Image = Properties.Resources.OFF;
                }

                hCOM = MyBlueRat.ReturnSafeFileHandle();
                BlueRat_UART_Exception_status = false;
            }
            else
            {
                Console.WriteLine(DateTime.Now.ToString("h:mm:ss tt") + " - Cannot connect to BlueRat.\n");
            }
        }

        private void DisconnectAutoBox1()
        {
            serialPortWood.Close();
        }

        private void DisconnectAutoBox2()
        {
            if (ini12.INIRead(MainSettingPath, "Device", "AutoboxExist", "") == "1")
            {
                if (MyBlueRat.Disconnect() == true)
                {
                    if (BlueRat_UART_Exception_status)
                    {
                        //Serial_UpdatePortName(); 
                    }
                    BlueRat_UART_Exception_status = false;
                }
                else
                {
                    Console.WriteLine(DateTime.Now.ToString("h:mm:ss tt") + " - Cannot disconnect from RS232.\n");
                }
            }
        }

        protected void ConnectCanBus()
        {
            uint status;

            status = MYCanReader.Connect();
            if (status == 1)
            {
                status = MYCanReader.StartCAN();
                if (status == 1)
                {
                    timer_canbus.Enabled = true;
                    pictureBox_canbus.Image = Properties.Resources.ON;
                }
                else
                {
                    pictureBox_canbus.Image = Properties.Resources.OFF;
                }
            }
            else
            {
                pictureBox_canbus.Image = Properties.Resources.OFF;
            }
        }

        // 這個主程式專用的delay的內部資料與function
        static bool RedRatDBViewer_Delay_TimeOutIndicator = false;
        private void RedRatDBViewer_Delay_OnTimedEvent(object source, ElapsedEventArgs e)
        {
            //Console.WriteLine("RedRatDBViewer_Delay_TimeOutIndicator: True.");
            RedRatDBViewer_Delay_TimeOutIndicator = true;
        }

        private void RedRatDBViewer_Delay(int delay_ms)
        {
            //Console.WriteLine("RedRatDBViewer_Delay: Start.");
            if (delay_ms <= 0) return;
            System.Timers.Timer aTimer = new System.Timers.Timer(delay_ms);
            //aTimer.Interval = delay_ms;
            aTimer.Elapsed += new ElapsedEventHandler(RedRatDBViewer_Delay_OnTimedEvent);
            aTimer.SynchronizingObject = this.TimeLabel2;
            RedRatDBViewer_Delay_TimeOutIndicator = false;
            aTimer.Enabled = true;
            aTimer.Start();
            while ((FormIsClosing == false) && (RedRatDBViewer_Delay_TimeOutIndicator == false))
            {
                //Console.WriteLine("RedRatDBViewer_Delay_TimeOutIndicator: false.");
                Application.DoEvents();
                System.Threading.Thread.Sleep(1);//釋放CPU//

                if (Global.Break_Out_MyRunCamd == 1)//強制讓schedule直接停止//
                {
                    Global.Break_Out_MyRunCamd = 0;
                    //Console.WriteLine("Break_Out_MyRunCamd = 0");
                    break;
                }
            }

            aTimer.Stop();
            aTimer.Dispose();
            //Console.WriteLine("RedRatDBViewer_Delay: End.");
        }

        public static string ByteToHexStr(byte[] bytes)
        {
            string returnStr = "";
            if (bytes != null)
            {
                for (int i = 0; i < bytes.Length; i++)
                {
                    returnStr += bytes[i].ToString("X2");
                }
            }
            return returnStr;
        }

        #region -- SerialPort Setup --
        protected void OpenSerialPort(string Port)
        {
            switch (Port)
            {
                case "A":
                    try
                    {
                        if (PortA.IsOpen == false)
                        {
                            string stopbit = ini12.INIRead(MainSettingPath, "Port A", "StopBits", "");
                            switch (stopbit)
                            {
                                case "One":
                                    PortA.StopBits = System.IO.Ports.StopBits.One;
                                    break;
                                case "Two":
                                    PortA.StopBits = System.IO.Ports.StopBits.Two;
                                    break;
                            }
                            PortA.PortName = ini12.INIRead(MainSettingPath, "Port A", "PortName", "");
                            PortA.BaudRate = int.Parse(ini12.INIRead(MainSettingPath, "Port A", "BaudRate", ""));
                            PortA.ReadTimeout = 2000;
                            // serialPort2.Encoding = System.Text.Encoding.GetEncoding(1252);

                            PortA.DataReceived += new SerialDataReceivedEventHandler(SerialPort1_DataReceived);       // DataReceived呼叫函式
                            PortA.Open();
                            object stream = typeof(SerialPort).GetField("internalSerialStream", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(PortA);
                        }
                    }
                    catch (Exception Ex)
                    {
                        MessageBox.Show(Ex.Message.ToString(), "PortA Error");
                    }
                    break;
                case "B":
                    try
                    {
                        if (PortB.IsOpen == false)
                        {
                            string stopbit = ini12.INIRead(MainSettingPath, "Port B", "StopBits", "");
                            switch (stopbit)
                            {
                                case "One":
                                    PortB.StopBits = System.IO.Ports.StopBits.One;
                                    break;
                                case "Two":
                                    PortB.StopBits = System.IO.Ports.StopBits.Two;
                                    break;
                            }
                            PortB.PortName = ini12.INIRead(MainSettingPath, "Port B", "PortName", "");
                            PortB.BaudRate = int.Parse(ini12.INIRead(MainSettingPath, "Port B", "BaudRate", ""));
                            PortB.ReadTimeout = 2000;
                            // serialPort2.Encoding = System.Text.Encoding.GetEncoding(1252);

                            PortB.DataReceived += new SerialDataReceivedEventHandler(SerialPort2_DataReceived);       // DataReceived呼叫函式
                            PortB.Open();
                            object stream = typeof(SerialPort).GetField("internalSerialStream", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(PortB);
                        }
                    }
                    catch (Exception Ex)
                    {
                        MessageBox.Show(Ex.Message.ToString(), "PortB Error");
                    }
                    break;
                case "C":
                    try
                    {
                        if (PortC.IsOpen == false)
                        {
                            string stopbit = ini12.INIRead(MainSettingPath, "Port C", "StopBits", "");
                            switch (stopbit)
                            {
                                case "One":
                                    PortC.StopBits = System.IO.Ports.StopBits.One;
                                    break;
                                case "Two":
                                    PortC.StopBits = System.IO.Ports.StopBits.Two;
                                    break;
                            }
                            PortC.PortName = ini12.INIRead(MainSettingPath, "Port C", "PortName", "");
                            PortC.BaudRate = int.Parse(ini12.INIRead(MainSettingPath, "Port C", "BaudRate", ""));
                            PortC.ReadTimeout = 2000;
                            // serialPort3.Encoding = System.Text.Encoding.GetEncoding(1252);

                            PortC.DataReceived += new SerialDataReceivedEventHandler(SerialPort3_DataReceived);       // DataReceived呼叫函式
                            PortC.Open();
                            object stream = typeof(SerialPort).GetField("internalSerialStream", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(PortC);
                        }
                    }
                    catch (Exception Ex)
                    {
                        MessageBox.Show(Ex.Message.ToString(), "PortC Error");
                    }
                    break;
                case "D":
                    try
                    {
                        if (PortD.IsOpen == false)
                        {
                            string stopbit = ini12.INIRead(MainSettingPath, "Port D", "StopBits", "");
                            switch (stopbit)
                            {
                                case "One":
                                    PortD.StopBits = System.IO.Ports.StopBits.One;
                                    break;
                                case "Two":
                                    PortD.StopBits = System.IO.Ports.StopBits.Two;
                                    break;
                            }
                            PortD.PortName = ini12.INIRead(MainSettingPath, "Port D", "PortName", "");
                            PortD.BaudRate = int.Parse(ini12.INIRead(MainSettingPath, "Port D", "BaudRate", ""));
                            PortD.ReadTimeout = 2000;
                            // serialPort3.Encoding = System.Text.Encoding.GetEncoding(1252);

                            PortD.DataReceived += new SerialDataReceivedEventHandler(SerialPort4_DataReceived);       // DataReceived呼叫函式
                            PortD.Open();
                            object stream = typeof(SerialPort).GetField("internalSerialStream", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(PortC);
                        }
                    }
                    catch (Exception Ex)
                    {
                        MessageBox.Show(Ex.Message.ToString(), "PortD Error");
                    }
                    break;
                case "E":
                    try
                    {
                        if (PortE.IsOpen == false)
                        {
                            string stopbit = ini12.INIRead(MainSettingPath, "Port E", "StopBits", "");
                            switch (stopbit)
                            {
                                case "One":
                                    PortE.StopBits = System.IO.Ports.StopBits.One;
                                    break;
                                case "Two":
                                    PortE.StopBits = System.IO.Ports.StopBits.Two;
                                    break;
                            }
                            PortE.PortName = ini12.INIRead(MainSettingPath, "Port E", "PortName", "");
                            PortE.BaudRate = int.Parse(ini12.INIRead(MainSettingPath, "Port E", "BaudRate", ""));
                            PortE.ReadTimeout = 2000;
                            // serialPort3.Encoding = System.Text.Encoding.GetEncoding(1252);

                            PortE.DataReceived += new SerialDataReceivedEventHandler(SerialPort4_DataReceived);       // DataReceived呼叫函式
                            PortE.Open();
                            object stream = typeof(SerialPort).GetField("internalSerialStream", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(PortC);
                        }
                    }
                    catch (Exception Ex)
                    {
                        MessageBox.Show(Ex.Message.ToString(), "PortE Error");
                    }
                    break;
                case "kline":
                    try
                    {
                        string Kline_Exist = ini12.INIRead(MainSettingPath, "Kline", "Checked", "");

                        if (Kline_Exist == "1" && MySerialPort.IsPortOpened() == false)
                        {
                            string curItem = ini12.INIRead(MainSettingPath, "Kline", "PortName", "");
                            if (MySerialPort.OpenPort(curItem) == true)
                            {
                                //BlueRat_UART_Exception_status = false;
                                timer_kline.Enabled = true;
                            }
                            else
                            {
                                timer_kline.Enabled = false;
                            }
                        }
                    }
                    catch (Exception Ex)
                    {
                        MessageBox.Show(Ex.Message.ToString(), "KlinePort Error");
                    }
                    break;
                default:
                    break;
            }
        }

        protected void CloseSerialPort(string Port)
        {
            switch (Port)
            {
                case "A":
                    PortA.Dispose();
                    PortA.Close();
                    break;
                case "B":
                    PortB.Dispose();
                    PortB.Close();
                    break;
                case "C":
                    PortC.Dispose();
                    PortC.Close();
                    break;
                case "D":
                    PortD.Dispose();
                    PortD.Close();
                    break;
                case "E":
                    PortE.Dispose();
                    PortE.Close();
                    break;
                case "kline":
                    MySerialPort.Dispose();
                    MySerialPort.ClosePort();
                    break;
                default:
                    break;
            }
        }
        #endregion

        #region -- 接受SerialPort1資料 --
        private void SerialPort1_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                int data_to_read = PortA.BytesToRead;
                if (data_to_read > 0)
                {
                    byte[] dataset = new byte[data_to_read];

                    PortA.Read(dataset, 0, data_to_read);

                    DateTime dt;
                    if (ini12.INIRead(MainSettingPath, "Displayhex", "Checked", "") == "1")
                    {
                        // hex to string
                        string hexValues = BitConverter.ToString(dataset).Replace("-", "");

                        dt = DateTime.Now;
                        hexValues = hexValues.Replace(Environment.NewLine, "\r\n" + "[Port_A] [" + dt.ToString("yyyy/MM/dd HH:mm:ss.fff") + "]  "); //OK
                        log1_text = string.Concat(log1_text, hexValues);
                    }
                    else
                    {
                        string strValues = Encoding.ASCII.GetString(dataset);

                        dt = DateTime.Now;
                        strValues = strValues.Replace(Environment.NewLine, "\r\n" + "[Port_A] [" + dt.ToString("yyyy/MM/dd HH:mm:ss.fff") + "]  "); //OK
                        log1_text = string.Concat(log1_text, strValues);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        #endregion

        #region -- 接受SerialPort2資料 --
        private void SerialPort2_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                int data_to_read = PortB.BytesToRead;
                if (data_to_read > 0)
                {
                    byte[] dataset = new byte[data_to_read];

                    PortB.Read(dataset, 0, data_to_read);

                    DateTime dt;
                    if (ini12.INIRead(MainSettingPath, "Displayhex", "Checked", "") == "1")
                    {
                        string hexValues = BitConverter.ToString(dataset).Replace("-", "");

                        DateTime.Now.ToShortTimeString();
                        dt = DateTime.Now;
                        hexValues = hexValues.Replace(Environment.NewLine, "\r\n" + "[Port_B] [" + dt.ToString("yyyy/MM/dd HH:mm:ss.fff") + "]  "); //OK
                        log2_text = string.Concat(log2_text, hexValues);
                    }
                    else
                    {
                        string strValues = Encoding.ASCII.GetString(dataset);

                        dt = DateTime.Now;
                        strValues = strValues.Replace(Environment.NewLine, "\r\n" + "[Port_B] [" + dt.ToString("yyyy/MM/dd HH:mm:ss.fff") + "]  "); //OK
                        log2_text = string.Concat(log2_text, strValues);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        #endregion

        #region -- 接受SerialPort3資料 --
        private void SerialPort3_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                int data_to_read = PortC.BytesToRead;
                if (data_to_read > 0)
                {
                    byte[] dataset = new byte[data_to_read];

                    PortC.Read(dataset, 0, data_to_read);

                    DateTime dt;
                    if (ini12.INIRead(MainSettingPath, "Displayhex", "Checked", "") == "1")
                    {
                        // hex to string
                        string hexValues = BitConverter.ToString(dataset).Replace("-", "");

                        DateTime.Now.ToShortTimeString();
                        dt = DateTime.Now;
                        hexValues = hexValues.Replace(Environment.NewLine, "\r\n" + "[Port_C] [" + dt.ToString("yyyy/MM/dd HH:mm:ss.fff") + "]  "); //OK
                        log3_text = string.Concat(log3_text, hexValues);
                    }
                    else
                    {
                        string strValues = Encoding.ASCII.GetString(dataset);

                        dt = DateTime.Now;
                        strValues = strValues.Replace(Environment.NewLine, "\r\n" + "[Port_C] [" + dt.ToString("yyyy/MM/dd HH:mm:ss.fff") + "]  "); //OK
                        log3_text = string.Concat(log3_text, strValues);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        #endregion

        #region -- 接受SerialPort4資料 --
        private void SerialPort4_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                int data_to_read = PortD.BytesToRead;
                if (data_to_read > 0)
                {
                    byte[] dataset = new byte[data_to_read];

                    PortD.Read(dataset, 0, data_to_read);

                    DateTime dt;
                    if (ini12.INIRead(MainSettingPath, "Displayhex", "Checked", "") == "1")
                    {
                        string hexValues = BitConverter.ToString(dataset).Replace("-", "");

                        DateTime.Now.ToShortTimeString();
                        dt = DateTime.Now;
                        hexValues = hexValues.Replace(Environment.NewLine, "\r\n" + "[Port_D] [" + dt.ToString("yyyy/MM/dd HH:mm:ss.fff") + "]  "); //OK
                        log4_text = string.Concat(log4_text, hexValues);
                    }
                    else
                    {
                        string strValues = Encoding.ASCII.GetString(dataset);

                        dt = DateTime.Now;
                        strValues = strValues.Replace(Environment.NewLine, "\r\n" + "[Port_D] [" + dt.ToString("yyyy/MM/dd HH:mm:ss.fff") + "]  "); //OK
                        log4_text = string.Concat(log4_text, strValues);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        #endregion

        #region -- 接受SerialPort5資料 --
        private void SerialPort5_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                int data_to_read = PortE.BytesToRead;
                if (data_to_read > 0)
                {
                    byte[] dataset = new byte[data_to_read];

                    PortE.Read(dataset, 0, data_to_read);

                    DateTime dt;
                    if (ini12.INIRead(MainSettingPath, "Displayhex", "Checked", "") == "1")
                    {
                        string hexValues = BitConverter.ToString(dataset).Replace("-", "");

                        DateTime.Now.ToShortTimeString();
                        dt = DateTime.Now;
                        hexValues = hexValues.Replace(Environment.NewLine, "\r\n" + "[Port_E] [" + dt.ToString("yyyy/MM/dd HH:mm:ss.fff") + "]  "); //OK
                        log5_text = string.Concat(log5_text, hexValues);
                    }
                    else
                    {
                        string strValues = Encoding.ASCII.GetString(dataset);

                        dt = DateTime.Now;
                        strValues = strValues.Replace(Environment.NewLine, "\r\n" + "[Port_E] [" + dt.ToString("yyyy/MM/dd HH:mm:ss.fff") + "]  "); //OK
                        log5_text = string.Concat(log5_text, strValues);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        #endregion

        #region -- 儲存SerialPort的log --
        private void Serialportsave(string Port)
        {
            string fName = "";

            // 讀取ini中的路徑
            fName = ini12.INIRead(MainSettingPath, "Record", "LogPath", "");
            switch (Port)
            {
                case "A":
                    string t = fName + "\\_PortA_" + DateTime.Now.ToString("yyyyMMddHHmmss") + "_" + label_LoopNumber_Value.Text + ".txt";
                    StreamWriter MYFILE = new StreamWriter(t, false, Encoding.ASCII);
                    MYFILE.Write(log1_text);
                    MYFILE.Close();
                    log1_text = String.Empty;
                    break;
                case "B":
                    t = fName + "\\_PortB_" + DateTime.Now.ToString("yyyyMMddHHmmss") + "_" + label_LoopNumber_Value.Text + ".txt";
                    MYFILE = new StreamWriter(t, false, Encoding.ASCII);
                    MYFILE.Write(log2_text);
                    MYFILE.Close();
                    log2_text = String.Empty;
                    break;
                case "C":
                    t = fName + "\\_PortC_" + DateTime.Now.ToString("yyyyMMddHHmmss") + "_" + label_LoopNumber_Value.Text + ".txt";
                    MYFILE = new StreamWriter(t, false, Encoding.ASCII);
                    MYFILE.Write(log3_text);
                    MYFILE.Close();
                    log3_text = String.Empty;
                    break;
                case "D":
                    t = fName + "\\_PortD_" + DateTime.Now.ToString("yyyyMMddHHmmss") + "_" + label_LoopNumber_Value.Text + ".txt";
                    MYFILE = new StreamWriter(t, false, Encoding.ASCII);
                    MYFILE.Write(log4_text);
                    MYFILE.Close();
                    log4_text = String.Empty;
                    break;
                case "E":
                    t = fName + "\\_PortE_" + DateTime.Now.ToString("yyyyMMddHHmmss") + "_" + label_LoopNumber_Value.Text + ".txt";
                    MYFILE = new StreamWriter(t, false, Encoding.ASCII);
                    MYFILE.Write(log5_text);
                    MYFILE.Close();
                    log5_text = String.Empty;
                    break;
                case "Canbus":
                    t = fName + "\\_Canbus_" + DateTime.Now.ToString("yyyyMMddHHmmss") + "_" + label_LoopNumber_Value.Text + ".txt";
                    MYFILE = new StreamWriter(t, false, Encoding.ASCII);
                    MYFILE.Write(canbus_text);
                    MYFILE.Close();
                    canbus_text = String.Empty;
                    break;
                case "KlinePort":
                    t = fName + "\\_Kline_" + DateTime.Now.ToString("yyyyMMddHHmmss") + "_" + label_LoopNumber_Value.Text + ".txt";
                    MYFILE = new StreamWriter(t, false, Encoding.ASCII);
                    MYFILE.Write(kline_text);
                    MYFILE.Close();
                    kline_text = String.Empty;
                    break;
                case "All":
                    t = fName + "\\_AllPort_" + DateTime.Now.ToString("yyyyMMddHHmmss") + "_" + label_LoopNumber_Value.Text + ".txt";
                    MYFILE = new StreamWriter(t, false, Encoding.ASCII);
                    MYFILE.Write(logAll_text);
                    MYFILE.Close();
                    logAll_text = String.Empty;
                    break;
            }
        }
        #endregion

        #region -- Old儲存CANbus的log --
        private void CanbusRS232save()
        {
            string fName = "";

            // 讀取ini中的路徑
            fName = ini12.INIRead(MainSettingPath, "Record", "LogPath", "");
            string t = fName + "\\_CANbus_" + DateTime.Now.ToString("yyyyMMddHHmmss") + "_" + label_LoopNumber_Value.Text + ".txt";

            StreamWriter MYFILE = new StreamWriter(t, false, Encoding.ASCII);
            MYFILE.Write(canbus_text);
            /*
            Console.WriteLine("Save Log By Queue");
            while (LogQueue3.Count > 0)
            {
                char temp_char;
                byte temp_byte;

                temp_byte = LogQueue3.Dequeue();
                temp_char = (char)temp_byte;

                MYFILE.Write(temp_char);
            }
            */
            MYFILE.Close();
            canbus_text = string.Empty;
        }
        #endregion

        #region -- 跑Schedule的指令集 --
        private void MyRunCamd()
        {
            int sRepeat = 0, stime = 0, SysDelay = 0;

            Global.Loop_Number = 1;
            Global.Break_Out_Schedule = 0;
            Global.Pass_Or_Fail = "PASS";

            label_TestTime_Value.Text = "0d 0h 0m 0s";
            TestTime = 0;

            for (int j = 1; j < Global.Schedule_Loop + 1; j++)
            {
                Global.caption_Num = 0;
                UpdateUI(j.ToString(), label_LoopNumber_Value);

                lock (this)
                {
                    for (Global.Scheduler_Row = 0; Global.Scheduler_Row < DataGridView_Schedule.Rows.Count - 1; Global.Scheduler_Row++)
                    {
                        IO_INPUT();//先讀取IO值，避免schedule第一行放IO CMD會出錯//

                        //Schedule All columns list
                        string columns_command = DataGridView_Schedule.Rows[Global.Scheduler_Row].Cells[0].Value.ToString();
                        string columns_times = DataGridView_Schedule.Rows[Global.Scheduler_Row].Cells[1].Value.ToString();
                        string columns_interval = DataGridView_Schedule.Rows[Global.Scheduler_Row].Cells[2].Value.ToString();
                        string columns_comport = DataGridView_Schedule.Rows[Global.Scheduler_Row].Cells[3].Value.ToString();
                        string columns_function = DataGridView_Schedule.Rows[Global.Scheduler_Row].Cells[4].Value.ToString();
                        string columns_subFunction = DataGridView_Schedule.Rows[Global.Scheduler_Row].Cells[5].Value.ToString();
                        string columns_serial = DataGridView_Schedule.Rows[Global.Scheduler_Row].Cells[6].Value.ToString();
                        string columns_switch = DataGridView_Schedule.Rows[Global.Scheduler_Row].Cells[7].Value.ToString();
                        string columns_wait = DataGridView_Schedule.Rows[Global.Scheduler_Row].Cells[8].Value.ToString();
                        string columns_remark = DataGridView_Schedule.Rows[Global.Scheduler_Row].Cells[9].Value.ToString();

                        Global.Schedule_Step = Global.Scheduler_Row;

                        if (StartButtonPressed == false)
                        {
                            j = Global.Schedule_Loop;
                            UpdateUI(j.ToString(), label_LoopNumber_Value);
                            break;
                        }

                        //Console.WriteLine("Datagridview highlight.");
                        GridUI(Global.Scheduler_Row.ToString(), DataGridView_Schedule);//控制Datagridview highlight//
                        //Console.WriteLine("Datagridview scollbar.");
                        Gridscroll(Global.Scheduler_Row.ToString(), DataGridView_Schedule);//控制Datagridview scollbar//

                        if (columns_times != "" && int.TryParse(columns_times, out stime) == true)
                            stime = int.Parse(columns_times); // 次數
                        else
                            stime = 1;

                        if (columns_interval != "" && int.TryParse(columns_interval, out sRepeat) == true)
                            sRepeat = int.Parse(columns_interval); // 停止時間
                        else
                            sRepeat = 0;

                        if (columns_wait != "" && int.TryParse(columns_wait, out SysDelay) == true)
                            SysDelay = int.Parse(columns_wait); // 指令停止時間
                        else
                            SysDelay = 0;

                        #region -- Record Schedule --
                        string delimiter_recordSch = ",";
                        string Schedule_log = "";
                        DateTime.Now.ToShortTimeString();
                        DateTime sch_dt = DateTime.Now;

                        Console.WriteLine("Record Schedule.");
                        Schedule_log = columns_command;
                        try
                        {
                            for (int i = 1; i < 10; i++)
                            {
                                Schedule_log = Schedule_log + delimiter_recordSch + DataGridView_Schedule.Rows[Global.Scheduler_Row].Cells[i].Value.ToString();
                            }
                        }
                        catch (Exception Ex)
                        {
                            MessageBox.Show(Ex.Message.ToString(), "The schedule length incorrect!");
                        }

                        string sch_log_text = "[Schedule] [" + sch_dt.ToString("yyyy/MM/dd HH:mm:ss.fff") + "]  " + Schedule_log + "\r\n";
                        log1_text = string.Concat(log1_text, sch_log_text);
                        log2_text = string.Concat(log2_text, sch_log_text);
                        log3_text = string.Concat(log3_text, sch_log_text);
                        log4_text = string.Concat(log4_text, sch_log_text);
                        log5_text = string.Concat(log5_text, sch_log_text);
                        canbus_text = string.Concat(canbus_text, sch_log_text);
                        kline_text = string.Concat(kline_text, sch_log_text);
                        #endregion

                        #region -- _cmd --
                        if (columns_command == "_cmd")
                        {
                            #region -- AC SWITCH OLD --
                            if (columns_switch == "_on")
                            {
                                Console.WriteLine("AC SWITCH OLD: _on");
                                if (ini12.INIRead(MainSettingPath, "Device", "AutoboxExist", "") == "1")
                                {
                                    if (PL2303_GP0_Enable(hCOM, 1) == true)
                                    {
                                        uint val = (uint)int.Parse("1");
                                        bool bSuccess = PL2303_GP0_SetValue(hCOM, val);
                                        if (bSuccess)
                                        {
                                            {
                                                PowerState = true;
                                                pictureBox_AcPower.Image = Properties.Resources.ON;
                                                label_Command.Text = "AC ON";
                                            }
                                        }
                                    }
                                    if (PL2303_GP1_Enable(hCOM, 1) == true)
                                    {
                                        uint val = (uint)int.Parse("1");
                                        bool bSuccess = PL2303_GP1_SetValue(hCOM, val);
                                        if (bSuccess)
                                        {
                                            {
                                                PowerState = true;
                                                pictureBox_AcPower.Image = Properties.Resources.ON;
                                                label_Command.Text = "AC ON";
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    MessageBox.Show("Please connect an AutoKit!", "Autobox Open Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                }
                            }
                            if (columns_switch == "_off")
                            {
                                Console.WriteLine("AC SWITCH OLD: _off");
                                if (ini12.INIRead(MainSettingPath, "Device", "AutoboxExist", "") == "1")
                                {
                                    if (PL2303_GP0_Enable(hCOM, 1) == true)
                                    {
                                        uint val = (uint)int.Parse("0");
                                        bool bSuccess = PL2303_GP0_SetValue(hCOM, val);
                                        if (bSuccess)
                                        {
                                            {
                                                PowerState = false;
                                                pictureBox_AcPower.Image = Properties.Resources.OFF;
                                                label_Command.Text = "AC OFF";
                                            }
                                        }
                                    }
                                    if (PL2303_GP1_Enable(hCOM, 1) == true)
                                    {
                                        uint val = (uint)int.Parse("0");
                                        bool bSuccess = PL2303_GP1_SetValue(hCOM, val);
                                        if (bSuccess)
                                        {
                                            {
                                                PowerState = false;
                                                pictureBox_AcPower.Image = Properties.Resources.OFF;
                                                label_Command.Text = "AC OFF";
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    MessageBox.Show("Please connect an AutoKit!", "Autobox Open Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                }
                            }
                            #endregion

                            #region -- AC SWITCH --
                            if (columns_switch == "_AC1_ON")
                            {
                                Console.WriteLine("AC SWITCH: _AC1_ON");
                                if (ini12.INIRead(MainSettingPath, "Device", "AutoboxExist", "") == "1")
                                {
                                    if (PL2303_GP0_Enable(hCOM, 1) == true)
                                    {
                                        uint val = (uint)int.Parse("1");
                                        bool bSuccess = PL2303_GP0_SetValue(hCOM, val);
                                        if (bSuccess)
                                        {
                                            {
                                                PowerState = true;
                                                pictureBox_AcPower.Image = Properties.Resources.ON;
                                                label_Command.Text = "AC1 => POWER ON";
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    MessageBox.Show("Please connect an AutoKit!", "Autobox Open Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                }
                            }
                            if (columns_switch == "_AC1_OFF")
                            {
                                Console.WriteLine("AC SWITCH: _AC1_OFF");
                                if (ini12.INIRead(MainSettingPath, "Device", "AutoboxExist", "") == "1")
                                {
                                    if (PL2303_GP0_Enable(hCOM, 1) == true)
                                    {
                                        uint val = (uint)int.Parse("0");
                                        bool bSuccess = PL2303_GP0_SetValue(hCOM, val);
                                        if (bSuccess)
                                        {
                                            {
                                                PowerState = false;
                                                pictureBox_AcPower.Image = Properties.Resources.OFF;
                                                label_Command.Text = "AC1 => POWER OFF";
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    MessageBox.Show("Please connect an AutoKit!", "Autobox Open Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                }
                            }

                            if (columns_switch == "_AC2_ON")
                            {
                                Console.WriteLine("AC SWITCH: _AC2_ON");
                                if (ini12.INIRead(MainSettingPath, "Device", "AutoboxExist", "") == "1")
                                {
                                    if (PL2303_GP1_Enable(hCOM, 1) == true)
                                    {
                                        uint val = (uint)int.Parse("1");
                                        bool bSuccess = PL2303_GP1_SetValue(hCOM, val);
                                        if (bSuccess)
                                        {
                                            {
                                                PowerState = true;
                                                pictureBox_AcPower.Image = Properties.Resources.ON;
                                                label_Command.Text = "AC2 => POWER ON";
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    MessageBox.Show("Please connect an AutoKit!", "Autobox Open Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                }
                            }
                            if (columns_switch == "_AC2_OFF")
                            {
                                Console.WriteLine("AC SWITCH: _AC2_OFF");
                                if (ini12.INIRead(MainSettingPath, "Device", "AutoboxExist", "") == "1")
                                {
                                    if (PL2303_GP1_Enable(hCOM, 1) == true)
                                    {
                                        uint val = (uint)int.Parse("0");
                                        bool bSuccess = PL2303_GP1_SetValue(hCOM, val);
                                        if (bSuccess)
                                        {
                                            {
                                                PowerState = false;
                                                pictureBox_AcPower.Image = Properties.Resources.OFF;
                                                label_Command.Text = "AC2 => POWER OFF";
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    MessageBox.Show("Please connect an AutoKit!", "Autobox Open Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                }
                            }
                            #endregion

                            #region -- USB SWITCH --
                            if (columns_switch == "_USB1_DUT")
                            {
                                Console.WriteLine("USB SWITCH: _USB1_DUT");
                                if (ini12.INIRead(MainSettingPath, "Device", "AutoboxExist", "") == "1")
                                {
                                    if (PL2303_GP2_Enable(hCOM, 1) == true)
                                    {
                                        uint val = (uint)int.Parse("1");
                                        bool bSuccess = PL2303_GP2_SetValue(hCOM, val);
                                        if (bSuccess == true)
                                        {
                                            {
                                                USBState = false;
                                                label_Command.Text = "USB1 => DUT";
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    MessageBox.Show("Please connect an AutoKit!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                }
                            }
                            else if (columns_switch == "_USB1_PC")
                            {
                                Console.WriteLine("USB SWITCH: _USB1_PC");
                                if (ini12.INIRead(MainSettingPath, "Device", "AutoboxExist", "") == "1")
                                {
                                    if (PL2303_GP2_Enable(hCOM, 1) == true)
                                    {
                                        uint val = (uint)int.Parse("0");
                                        bool bSuccess = PL2303_GP2_SetValue(hCOM, val);
                                        if (bSuccess == true)
                                        {
                                            {
                                                USBState = true;
                                                label_Command.Text = "USB1 => PC";
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    MessageBox.Show("Please connect an AutoKit!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                }
                            }

                            if (columns_switch == "_USB2_DUT")
                            {
                                Console.WriteLine("USB SWITCH: _USB2_DUT");
                                if (ini12.INIRead(MainSettingPath, "Device", "AutoboxExist", "") == "1")
                                {
                                    if (PL2303_GP3_Enable(hCOM, 1) == true)
                                    {
                                        uint val = (uint)int.Parse("1");
                                        bool bSuccess = PL2303_GP3_SetValue(hCOM, val);
                                        if (bSuccess == true)
                                        {
                                            {
                                                USBState = false;
                                                label_Command.Text = "USB2 => DUT";
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    MessageBox.Show("Please connect an AutoKit!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                }
                            }
                            else if (columns_switch == "_USB2_PC")
                            {
                                Console.WriteLine("USB SWITCH: _USB2_PC");
                                if (ini12.INIRead(MainSettingPath, "Device", "AutoboxExist", "") == "1")
                                {
                                    if (PL2303_GP3_Enable(hCOM, 1) == true)
                                    {
                                        uint val = (uint)int.Parse("0");
                                        bool bSuccess = PL2303_GP3_SetValue(hCOM, val);
                                        if (bSuccess == true)
                                        {
                                            {
                                                USBState = true;
                                                label_Command.Text = "USB2 => PC";
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    MessageBox.Show("Please connect an AutoKit!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                }
                            }
                            #endregion
                        }
                        #endregion

                        #region -- 拍照 --
                        else if (columns_command == "_shot")
                        {
                            Console.WriteLine("Take Picture: _shot");
                            if (ini12.INIRead(MainSettingPath, "Device", "CameraExist", "") == "1")
                            {
                                Global.caption_Num++;
                                if (Global.Loop_Number == 1)
                                    Global.caption_Sum = Global.caption_Num;
                                Jes();
                                label_Command.Text = "Take Picture";
                            }
                            else
                            {
                                button_Start.PerformClick();
                                MessageBox.Show("Camera is not connected!", "Error");
                                setStyle();
                            }
                        }
                        #endregion

                        #region -- 錄影 --
                        else if (columns_command == "_rec_start")
                        {
                            Console.WriteLine("Take Record: _rec_start");
                            if (ini12.INIRead(MainSettingPath, "Device", "CameraExist", "") == "1")
                            {
                                if (VideoRecording == false)
                                {
                                    Mysvideo(); // 開新檔
                                    VideoRecording = true;
                                    Thread oThreadC = new Thread(new ThreadStart(MySrtCamd));
                                    oThreadC.Start();
                                }
                                label_Command.Text = "Start Recording";
                            }
                            else
                            {
                                MessageBox.Show("Camera is not connected", "Camera Open Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                button_Start.PerformClick();
                            }
                        }

                        else if (columns_command == "_rec_stop")
                        {
                            Console.WriteLine("Take Record: _rec_stop");
                            if (ini12.INIRead(MainSettingPath, "Device", "CameraExist", "") == "1")
                            {
                                if (VideoRecording == true)       //判斷是不是正在錄影
                                {
                                    VideoRecording = false;
                                    Mysstop();      //先將先前的關掉
                                }
                                label_Command.Text = "Stop Recording";
                            }
                            else
                            {
                                MessageBox.Show("Camera is not connected", "Camera Open Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                button_Start.PerformClick();
                            }
                        }
                        #endregion

                        #region -- Ascii --
                        else if (columns_command == "_ascii")
                        {
                            if (ini12.INIRead(MainSettingPath, "Port A", "Checked", "") == "1" && columns_comport == "A")
                            {
                                Console.WriteLine("Ascii Log: _PortA");
                                if (columns_serial == "_save")
                                {
                                    Serialportsave("A"); //存檔rs232
                                }
                                else if (columns_serial == "_clear")
                                {
                                    log1_text = string.Empty; //清除log1_text
                                }
                                else if (columns_serial != "" && columns_switch == @"\r")
                                {
                                    PortA.Write(columns_serial + "\r"); //發送數據 Rs232 + \r
                                }
                                else if (columns_serial != "" && columns_switch == @"\n")
                                {
                                    PortA.Write(columns_serial + "\n"); //發送數據 Rs232 + \n
                                }
                                else if (columns_serial != "" && columns_switch == @"\n\r")
                                {
                                    PortA.Write(columns_serial + "\n\r"); //發送數據 Rs232 + \n\r
                                }
                                else if (columns_serial != "" && columns_switch == @"\r\n")
                                {
                                    PortA.Write(columns_serial + "\r\n"); //發送數據 Rs232 + \r\n
                                }
                                DateTime dt = DateTime.Now;
                                string text = "[Port_A] [" + dt.ToString("yyyy/MM/dd HH:mm:ss.fff") + "]  " + columns_serial + "\n\r";
                                log1_text = string.Concat(log1_text, text);
                                logAll_text = string.Concat(logAll_text, text);
                            }

                            if (ini12.INIRead(MainSettingPath, "Port B", "Checked", "") == "1" && columns_comport == "B")
                            {
                                Console.WriteLine("Ascii Log: _PortB");
                                if (columns_serial == "_save")
                                {
                                    Serialportsave("B"); //存檔rs232
                                }
                                else if (columns_serial == "_clear")
                                {
                                    log2_text = string.Empty; //清除log2_text
                                }
                                else if (columns_serial != "" && columns_switch == @"\r")
                                {
                                    PortB.Write(columns_serial + "\r"); //發送數據 Rs232 + \r
                                }
                                else if (columns_serial != "" && columns_switch == @"\n")
                                {
                                    PortB.Write(columns_serial + "\n"); //發送數據 Rs232 + \n
                                }
                                else if (columns_serial != "" && columns_switch == @"\n\r")
                                {
                                    PortB.Write(columns_serial + "\n\r"); //發送數據 Rs232 + \n\r
                                }
                                else if (columns_serial != "" && columns_switch == @"\r\n")
                                {
                                    PortB.Write(columns_serial + "\r\n"); //發送數據 Rs232 + \r\n
                                }
                                DateTime dt = DateTime.Now;
                                string text = "[Port_B] [" + dt.ToString("yyyy/MM/dd HH:mm:ss.fff") + "]  " + columns_serial + "\r\n";
                                log2_text = string.Concat(log2_text, text);
                                logAll_text = string.Concat(logAll_text, text);
                            }

                            if (ini12.INIRead(MainSettingPath, "Port C", "Checked", "") == "1" && columns_comport == "C")
                            {
                                Console.WriteLine("Ascii Log: _PortC");
                                if (columns_serial == "_save")
                                {
                                    Serialportsave("C"); //存檔rs232
                                }
                                else if (columns_serial == "_clear")
                                {
                                    log3_text = string.Empty; //清除log3_text
                                }
                                else if (columns_serial != "" && columns_switch == @"\r")
                                {
                                    PortC.Write(columns_serial + "\r"); //發送數據 Rs232 + \r
                                }
                                else if (columns_serial != "" && columns_switch == @"\n")
                                {
                                    PortC.Write(columns_serial + "\n"); //發送數據 Rs232 + \n
                                }
                                else if (columns_serial != "" && columns_switch == @"\n\r")
                                {
                                    PortC.Write(columns_serial + "\n\r"); //發送數據 Rs232 + \n\r
                                }
                                else if (columns_serial != "" && columns_switch == @"\r\n")
                                {
                                    PortC.Write(columns_serial + "\r\n"); //發送數據 Rs232 + \r\n
                                }
                                DateTime dt = DateTime.Now;
                                string text = "[Port_C] [" + dt.ToString("yyyy/MM/dd HH:mm:ss.fff") + "]  " + columns_serial + "\r\n";
                                log3_text = string.Concat(log3_text, text);
                                logAll_text = string.Concat(logAll_text, text);
                            }

                            if (ini12.INIRead(MainSettingPath, "Port D", "Checked", "") == "1" && columns_comport == "D")
                            {
                                Console.WriteLine("Ascii Log: _PortD");
                                if (columns_serial == "_save")
                                {
                                    Serialportsave("D"); //存檔rs232
                                }
                                else if (columns_serial == "_clear")
                                {
                                    log4_text = string.Empty; //清除log4_text
                                }
                                else if (columns_serial != "" && columns_switch == @"\r")
                                {
                                    PortD.Write(columns_serial + "\r"); //發送數據 Rs232 + \r
                                }
                                else if (columns_serial != "" && columns_switch == @"\n")
                                {
                                    PortD.Write(columns_serial + "\n"); //發送數據 Rs232 + \n
                                }
                                else if (columns_serial != "" && columns_switch == @"\n\r")
                                {
                                    PortD.Write(columns_serial + "\n\r"); //發送數據 Rs232 + \n\r
                                }
                                else if (columns_serial != "" && columns_switch == @"\r\n")
                                {
                                    PortD.Write(columns_serial + "\r\n"); //發送數據 Rs232 + \r\n
                                }
                                DateTime dt = DateTime.Now;
                                string text = "[Port_D] [" + dt.ToString("yyyy/MM/dd HH:mm:ss.fff") + "]  " + columns_serial + "\r\n";
                                log4_text = string.Concat(log4_text, text);
                                logAll_text = string.Concat(logAll_text, text);
                            }

                            if (ini12.INIRead(MainSettingPath, "Port E", "Checked", "") == "1" && columns_comport == "E")
                            {
                                Console.WriteLine("Ascii Log: _PortE");
                                if (columns_serial == "_save")
                                {
                                    Serialportsave("E"); //存檔rs232
                                }
                                else if (columns_serial == "_clear")
                                {
                                    log5_text = string.Empty; //清除log5_text
                                }
                                else if (columns_serial != "" && columns_switch == @"\r")
                                {
                                    PortE.Write(columns_serial + "\r"); //發送數據 Rs232 + \r
                                }
                                else if (columns_serial != "" && columns_switch == @"\n")
                                {
                                    PortE.Write(columns_serial + "\n"); //發送數據 Rs232 + \n
                                }
                                else if (columns_serial != "" && columns_switch == @"\n\r")
                                {
                                    PortE.Write(columns_serial + "\n\r"); //發送數據 Rs232 + \n\r
                                }
                                else if (columns_serial != "" && columns_switch == @"\r\n")
                                {
                                    PortE.Write(columns_serial + "\r\n"); //發送數據 Rs232 + \r\n
                                }
                                DateTime dt = DateTime.Now;
                                string text = "[Port_E] [" + dt.ToString("yyyy/MM/dd HH:mm:ss.fff") + "]  " + columns_serial + "\r\n";
                                log5_text = string.Concat(log5_text, text);
                                logAll_text = string.Concat(logAll_text, text);
                            }

                            if (columns_comport == "ALL")
                            {
                                Console.WriteLine("Ascii Log: _All");
                                string [] serial_content = columns_serial.Split(new string[] { "|" }, StringSplitOptions.RemoveEmptyEntries);
                                string [] switch_content = columns_switch.Split(new string[] { "|" }, StringSplitOptions.RemoveEmptyEntries);

                                if (columns_serial == "_save")
                                {
                                    Serialportsave("ALL"); //存檔rs232
                                }
                                else if (columns_serial == "_clear")
                                {
                                    logAll_text = string.Empty; //清除logAll_text
                                }

                                if (ini12.INIRead(MainSettingPath, "Port A", "Checked", "") == "1" && columns_comport == "ALL" && serial_content[0] != "" && switch_content[0] != "")
                                {
                                    PortA.Write(serial_content[0] + switch_content[0]);
                                    DateTime dt = DateTime.Now;
                                    string text = "[Port_A] [" + dt.ToString("yyyy/MM/dd HH:mm:ss.fff") + "]  " + columns_serial + "\r\n";
                                    log1_text = string.Concat(log1_text, text);
                                    logAll_text = string.Concat(logAll_text, text);
                                }
                                if (ini12.INIRead(MainSettingPath, "Port B", "Checked", "") == "1" && columns_comport == "ALL" && serial_content[1] != "" && switch_content[1] != "")
                                {
                                    PortB.Write(serial_content[1] + switch_content[1]);
                                    DateTime dt = DateTime.Now;
                                    string text = "[Port_B] [" + dt.ToString("yyyy/MM/dd HH:mm:ss.fff") + "]  " + columns_serial + "\r\n";
                                    log2_text = string.Concat(log2_text, text);
                                    logAll_text = string.Concat(logAll_text, text);
                                }
                                if (ini12.INIRead(MainSettingPath, "Port C", "Checked", "") == "1" && columns_comport == "ALL" && serial_content[2] != "" && switch_content[2] != "")
                                {
                                    PortC.Write(serial_content[2] + switch_content[2]);
                                    DateTime dt = DateTime.Now;
                                    string text = "[Port_C] [" + dt.ToString("yyyy/MM/dd HH:mm:ss.fff") + "]  " + columns_serial + "\r\n";
                                    log3_text = string.Concat(log3_text, text);
                                    logAll_text = string.Concat(logAll_text, text);
                                }
                                if (ini12.INIRead(MainSettingPath, "Port D", "Checked", "") == "1" && columns_comport == "ALL" && serial_content[3] != "" && switch_content[3] != "")
                                {
                                    PortD.Write(serial_content[3] + switch_content[3]);
                                    DateTime dt = DateTime.Now;
                                    string text = "[Port_D] [" + dt.ToString("yyyy/MM/dd HH:mm:ss.fff") + "]  " + columns_serial + "\r\n";
                                    log4_text = string.Concat(log4_text, text);
                                    logAll_text = string.Concat(logAll_text, text);
                                }
                                if (ini12.INIRead(MainSettingPath, "Port E", "Checked", "") == "1" && columns_comport == "ALL" && serial_content[4] != "" && switch_content[4] != "")
                                {
                                    PortE.Write(serial_content[4] + switch_content[4]);
                                    DateTime dt = DateTime.Now;
                                    string text = "[Port_E] [" + dt.ToString("yyyy/MM/dd HH:mm:ss.fff") + "]  " + columns_serial + "\r\n";
                                    log5_text = string.Concat(log5_text, text);
                                    logAll_text = string.Concat(logAll_text, text);
                                }
                            }
                            label_Command.Text = "(" + columns_command + ") " + columns_serial;
                        }
                        #endregion

                        #region -- Hex --
                        else if (columns_command == "_HEX")
                        {
                            if (ini12.INIRead(MainSettingPath, "Port A", "Checked", "") == "1" && columns_comport == "A")
                            {
                                Console.WriteLine("Hex Log: _PortA");
                                if (columns_serial == "_save")
                                {
                                    Serialportsave("A"); //存檔rs232
                                }
                                else if (columns_serial == "_clear")
                                {
                                    log1_text = string.Empty; //清除log1_text
                                }
                                else if (columns_serial != "_save" &&
                                         columns_serial != "_clear" &&
                                         columns_serial != "")
                                {
                                    string hexValues = columns_serial;
                                    byte[] bytes = hexValues.Split(' ').Select(s => Convert.ToByte(s, 16)).ToArray();
                                    PortA.Write(bytes, 0, bytes.Length); //發送數據 Rs232
                                }
                                DateTime dt = DateTime.Now;
                                string text = "[Port_A] [" + dt.ToString("yyyy/MM/dd HH:mm:ss.fff") + "]  " + columns_serial + "\r\n";
                                log1_text = string.Concat(log1_text, text);
                                logAll_text = string.Concat(logAll_text, text);
                            }

                            if (ini12.INIRead(MainSettingPath, "Port B", "Checked", "") == "1" && columns_comport == "B")
                            {
                                Console.WriteLine("Hex Log: _PortB");
                                if (columns_serial == "_save")
                                {
                                    Serialportsave("B"); //存檔rs232
                                }
                                else if (columns_serial == "_clear")
                                {
                                    log2_text = string.Empty; //清除log2_text
                                }
                                else if (columns_serial != "_save" &&
                                         columns_serial != "_clear" &&
                                         columns_serial != "")
                                {
                                    string hexValues = columns_serial;
                                    byte[] bytes = hexValues.Split(' ').Select(s => Convert.ToByte(s, 16)).ToArray();
                                    PortB.Write(bytes, 0, bytes.Length); //發送數據 Rs232
                                }
                                DateTime dt = DateTime.Now;
                                string text = "[Port_B] [" + dt.ToString("yyyy/MM/dd HH:mm:ss.fff") + "]  " + columns_serial + "\r\n";
                                log2_text = string.Concat(log2_text, text);
                                logAll_text = string.Concat(logAll_text, text);
                            }

                            if (ini12.INIRead(MainSettingPath, "Port C", "Checked", "") == "1" && columns_comport == "C")
                            {
                                Console.WriteLine("Hex Log: _PortC");
                                if (columns_serial == "_save")
                                {
                                    Serialportsave("C"); //存檔rs232
                                }
                                else if (columns_serial == "_clear")
                                {
                                    log3_text = string.Empty; //清除log3_text
                                }
                                else if (columns_serial != "_save" &&
                                         columns_serial != "_clear" &&
                                         columns_serial != "")
                                {
                                    string hexValues = columns_serial;
                                    byte[] bytes = hexValues.Split(' ').Select(s => Convert.ToByte(s, 16)).ToArray();
                                    PortC.Write(bytes, 0, bytes.Length); //發送數據 Rs232
                                }
                                DateTime dt = DateTime.Now;
                                string text = "[Port_C] [" + dt.ToString("yyyy/MM/dd HH:mm:ss.fff") + "]  " + columns_serial + "\r\n";
                                log3_text = string.Concat(log3_text, text);
                                logAll_text = string.Concat(logAll_text, text);
                            }

                            if (ini12.INIRead(MainSettingPath, "Port D", "Checked", "") == "1" && columns_comport == "D")
                            {
                                Console.WriteLine("Hex Log: _PortD");
                                if (columns_serial == "_save")
                                {
                                    Serialportsave("D"); //存檔rs232
                                }
                                else if (columns_serial == "_clear")
                                {
                                    log4_text = string.Empty; //清除log4_text
                                }
                                else if (columns_serial != "_save" &&
                                         columns_serial != "_clear" &&
                                         columns_serial != "")
                                {
                                    string hexValues = columns_serial;
                                    byte[] bytes = hexValues.Split(' ').Select(s => Convert.ToByte(s, 16)).ToArray();
                                    PortD.Write(bytes, 0, bytes.Length); //發送數據 Rs232
                                }
                                DateTime dt = DateTime.Now;
                                string text = "[Port_D] [" + dt.ToString("yyyy/MM/dd HH:mm:ss.fff") + "]  " + columns_serial + "\r\n";
                                log4_text = string.Concat(log4_text, text);
                                logAll_text = string.Concat(logAll_text, text);
                            }

                            if (ini12.INIRead(MainSettingPath, "Port E", "Checked", "") == "1" && columns_comport == "E")
                            {
                                Console.WriteLine("Hex Log: _PortE");
                                if (columns_serial == "_save")
                                {
                                    Serialportsave("E"); //存檔rs232
                                }
                                else if (columns_serial == "_clear")
                                {
                                    log5_text = string.Empty; //清除log5_text
                                }
                                else if (columns_serial != "_save" &&
                                         columns_serial != "_clear" &&
                                         columns_serial != "")
                                {
                                    string hexValues = columns_serial;
                                    byte[] bytes = hexValues.Split(' ').Select(s => Convert.ToByte(s, 16)).ToArray();
                                    PortE.Write(bytes, 0, bytes.Length); //發送數據 Rs232
                                }
                                DateTime dt = DateTime.Now;
                                string text = "[Port_E] [" + dt.ToString("yyyy/MM/dd HH:mm:ss.fff") + "]  " + columns_serial + "\r\n";
                                log5_text = string.Concat(log5_text, text);
                                logAll_text = string.Concat(logAll_text, text);
                            }

                            if (columns_comport == "ALL")
                            {
                                Console.WriteLine("Hex Log: _All");
                                string[] serial_content = columns_serial.Split(new string[] { "|" }, StringSplitOptions.RemoveEmptyEntries);

                                if (columns_serial == "_save")
                                {
                                    Serialportsave("ALL"); //存檔rs232
                                }
                                else if (columns_serial == "_clear")
                                {
                                    logAll_text = string.Empty; //清除logAll_text
                                }

                                if (ini12.INIRead(MainSettingPath, "Port A", "Checked", "") == "1" && columns_comport == "ALL" && serial_content[0] != "")
                                {
                                    PortA.Write(serial_content[0]);
                                    DateTime dt = DateTime.Now;
                                    string text = "[Port_A] [" + dt.ToString("yyyy/MM/dd HH:mm:ss.fff") + "]  " + columns_serial + "\r\n";
                                    log1_text = string.Concat(log1_text, text);
                                    logAll_text = string.Concat(logAll_text, text);
                                }
                                if (ini12.INIRead(MainSettingPath, "Port B", "Checked", "") == "1" && columns_comport == "ALL" && serial_content[1] != "")
                                {
                                    PortB.Write(serial_content[1]);
                                    DateTime dt = DateTime.Now;
                                    string text = "[Port_B] [" + dt.ToString("yyyy/MM/dd HH:mm:ss.fff") + "]  " + columns_serial + "\r\n";
                                    log2_text = string.Concat(log2_text, text);
                                    logAll_text = string.Concat(logAll_text, text);
                                }
                                if (ini12.INIRead(MainSettingPath, "Port C", "Checked", "") == "1" && columns_comport == "ALL" && serial_content[2] != "")
                                {
                                    PortC.Write(serial_content[2]);
                                    DateTime dt = DateTime.Now;
                                    string text = "[Port_C] [" + dt.ToString("yyyy/MM/dd HH:mm:ss.fff") + "]  " + columns_serial + "\r\n";
                                    log3_text = string.Concat(log3_text, text);
                                    logAll_text = string.Concat(logAll_text, text);
                                }
                                if (ini12.INIRead(MainSettingPath, "Port D", "Checked", "") == "1" && columns_comport == "ALL" && serial_content[3] != "")
                                {
                                    PortD.Write(serial_content[3]);
                                    DateTime dt = DateTime.Now;
                                    string text = "[Port_D] [" + dt.ToString("yyyy/MM/dd HH:mm:ss.fff") + "]  " + columns_serial + "\r\n";
                                    log4_text = string.Concat(log4_text, text);
                                    logAll_text = string.Concat(logAll_text, text);
                                }
                                if (ini12.INIRead(MainSettingPath, "Port E", "Checked", "") == "1" && columns_comport == "ALL" && serial_content[4] != "")
                                {
                                    PortE.Write(serial_content[4]);
                                    DateTime dt = DateTime.Now;
                                    string text = "[Port_E] [" + dt.ToString("yyyy/MM/dd HH:mm:ss.fff") + "]  " + columns_serial + "\r\n";
                                    log5_text = string.Concat(log5_text, text);
                                    logAll_text = string.Concat(logAll_text, text);
                                }
                            }
                            label_Command.Text = "(" + columns_command + ") " + columns_serial;
                        }
                        #endregion

                        #region -- K-Line --
                        else if (columns_command == "_K_ABS")
                        {
                            Console.WriteLine("K-line control: _K_ABS");
                            try
                            {
                                // K-lite ABS指令檔案匯入
                                string xmlfile = ini12.INIRead(MainSettingPath, "Record", "Generator", "");
                                if (System.IO.File.Exists(xmlfile) == true)
                                {
                                    var allDTC = XDocument.Load(xmlfile).Root.Element("ABS_ErrorCode").Elements("DTC");
                                    foreach (var ErrorCode in allDTC)
                                    {
                                        if (ErrorCode.Attribute("Name").Value == "_ABS")
                                        {
                                            if (columns_serial == ErrorCode.Element("DTC_D").Value)
                                            {
                                                UInt16 int_abs_code = Convert.ToUInt16(ErrorCode.Element("DTC_C").Value, 16);
                                                byte abs_code_high = Convert.ToByte(int_abs_code >> 8);
                                                byte abs_code_low = Convert.ToByte(int_abs_code & 0xff);
                                                byte abs_code_status = Convert.ToByte(ErrorCode.Element("DTC_S").Value, 16);
                                                ABS_error_list.Add(new DTC_Data(abs_code_high, abs_code_low, abs_code_status));
                                            }
                                        }
                                        else
                                        {
                                            MessageBox.Show("Content includes other error code", "ABS code Open Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                        }
                                    }
                                }
                                else
                                {
                                    MessageBox.Show("DTC code file does not exist", "File Open Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                }
                                label_Command.Text = "(" + columns_command + ") " + columns_serial;
                            }
                            catch (Exception Ex)
                            {
                                MessageBox.Show(Ex.Message.ToString(), "Kline_ABS library error!");
                            }
                        }
                        else if (columns_command == "_K_OBD")
                        {
                            Console.WriteLine("K-line control: _K_OBD");
                            try
                            {
                                // K-lite OBD指令檔案匯入
                                string xmlfile = ini12.INIRead(MainSettingPath, "Record", "Generator", "");
                                if (System.IO.File.Exists(xmlfile) == true)
                                {
                                    var allDTC = XDocument.Load(xmlfile).Root.Element("OBD_ErrorCode").Elements("DTC");
                                    foreach (var ErrorCode in allDTC)
                                    {
                                        if (ErrorCode.Attribute("Name").Value == "_OBD")
                                        {
                                            if (columns_serial == ErrorCode.Element("DTC_D").Value)
                                            {
                                                UInt16 obd_code_int16 = Convert.ToUInt16(ErrorCode.Element("DTC_C").Value, 16);
                                                byte obd_code_high = Convert.ToByte(obd_code_int16 >> 8);
                                                byte obd_code_low = Convert.ToByte(obd_code_int16 & 0xff);
                                                byte obd_code_status = Convert.ToByte(ErrorCode.Element("DTC_S").Value, 16);
                                                OBD_error_list.Add(new DTC_Data(obd_code_high, obd_code_low, obd_code_status));
                                            }
                                        }
                                        else
                                        {
                                            MessageBox.Show("Content includes other error code", "OBD code Open Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                        }
                                    }
                                }
                                else
                                {
                                    MessageBox.Show("DTC code file does not exist", "File Open Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                }
                                label_Command.Text = "(" + columns_command + ") " + columns_serial;
                            }
                            catch (Exception Ex)
                            {
                                MessageBox.Show(Ex.Message.ToString(), "Kline_OBD library error !");
                            }
                        }
                        else if (columns_command == "_K_SEND")
                        {
                            kline_send = 1;
                        }
                        else if (columns_command == "_K_CLEAR")
                        {
                            kline_send = 0;
                            ABS_error_list.Clear();
                            OBD_error_list.Clear();
                        }
                        #endregion

                        #region -- Astro Timing --
                        else if (columns_command == "_astro")
                        {
                            Console.WriteLine("Astro control: _astro");
                            try
                            {
                                // Astro指令
                                byte[] startbit = new byte[7] { 0x05, 0x24, 0x20, 0x02, 0xfd, 0x24, 0x20 };
                                PortA.Write(startbit, 0, 7);

                                // Astro指令檔案匯入
                                string xmlfile = ini12.INIRead(MainSettingPath, "Record", "Generator", "");
                                if (System.IO.File.Exists(xmlfile) == true)
                                {
                                    var allTiming = XDocument.Load(xmlfile).Root.Element("Generator").Elements("Device");
                                    foreach (var generator in allTiming)
                                    {
                                        if (generator.Attribute("Name").Value == "_astro")
                                        {
                                            if (columns_function == generator.Element("Timing").Value)
                                            {
                                                string[] timestrs = generator.Element("Signal").Value.Split(new string[] { "." }, StringSplitOptions.RemoveEmptyEntries);
                                                byte[] timebit1 = Encoding.ASCII.GetBytes(timestrs[0]);
                                                byte[] timebit2 = Encoding.ASCII.GetBytes(timestrs[1]);
                                                byte[] timebit3 = Encoding.ASCII.GetBytes(timestrs[2]);
                                                byte[] timebit4 = Encoding.ASCII.GetBytes(timestrs[3]);
                                                byte[] timebit = new byte[4] { timebit1[1], timebit2[1], timebit3[1], timebit4[1] };
                                                PortA.Write(timebit, 0, 4);
                                            }
                                        }
                                        else
                                        {
                                            MessageBox.Show("Content include other signal", "Astro Signal Open Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                        }
                                    }
                                }
                                else
                                {
                                    MessageBox.Show("Signal Generator not exist", "File Open Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                }

                                byte[] endbit = new byte[3] { 0x2c, 0x31, 0x03 };
                                PortA.Write(endbit, 0, 3);
                                label_Command.Text = "(" + columns_command + ") " + columns_switch;
                            }
                            catch (Exception Ex)
                            {
                                MessageBox.Show(Ex.Message.ToString(), "Transmit the Astro command fail !");
                            }
                        }
                        #endregion

                        #region -- Quantum Timing --
                        else if (columns_command == "_quantum")
                        {
                            Console.WriteLine("Quantum control: _quantum");
                            try
                            {
                                // Quantum指令檔案匯入
                                string xmlfile = ini12.INIRead(MainSettingPath, "Record", "Generator", "");
                                if (System.IO.File.Exists(xmlfile) == true)
                                {
                                    var allTiming = XDocument.Load(xmlfile).Root.Element("Generator").Elements("Device");
                                    foreach (var generator in allTiming)
                                    {
                                        if (generator.Attribute("Name").Value == "_quantum")
                                        {
                                            if (columns_function == generator.Element("Timing").Value)
                                            {
                                                PortA.WriteLine(generator.Element("Signal").Value + "\r");
                                                PortA.WriteLine("ALLU" + "\r");
                                            }
                                        }
                                        else
                                        {
                                            MessageBox.Show("Content include other signal", "Quantum Signal Open Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                        }
                                    }
                                }
                                else
                                {
                                    MessageBox.Show("Signal Generator not exist", "File Open Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                }

                                switch (columns_subFunction)
                                {
                                    case "RGB":
                                        // RGB mode
                                        PortA.WriteLine("AVST 0" + "\r");
                                        PortA.WriteLine("DVST 10" + "\r");
                                        PortA.WriteLine("FMTU" + "\r");
                                        break;
                                    case "YCbCr":
                                        // YCbCr mode
                                        PortA.WriteLine("AVST 0" + "\r");
                                        PortA.WriteLine("DVST 14" + "\r");
                                        PortA.WriteLine("FMTU" + "\r");
                                        break;
                                    case "xvYCC":
                                        // xvYCC mode
                                        PortA.WriteLine("AVST 0" + "\r");
                                        PortA.WriteLine("DVST 17" + "\r");
                                        PortA.WriteLine("FMTU" + "\r");
                                        break;
                                    case "4:4:4":
                                        // 4:4:4
                                        PortA.WriteLine("DVSM 4" + "\r");
                                        PortA.WriteLine("FMTU" + "\r");
                                        break;
                                    case "4:2:2":
                                        // 4:2:2
                                        PortA.WriteLine("DVSM 2" + "\r");
                                        PortA.WriteLine("FMTU" + "\r");
                                        break;
                                    case "8bits":
                                        // 8bits
                                        PortA.WriteLine("NBPC 8" + "\r");
                                        PortA.WriteLine("FMTU" + "\r");
                                        break;
                                    case "10bits":
                                        // 10bits
                                        PortA.WriteLine("NBPC 10" + "\r");
                                        PortA.WriteLine("FMTU" + "\r");
                                        break;
                                    case "12bits":
                                        // 12bits
                                        PortA.WriteLine("NBPC 12" + "\r");
                                        PortA.WriteLine("FMTU" + "\r");
                                        break;
                                    default:
                                        break;
                                }
                                label_Command.Text = "(" + columns_command + ") " + columns_switch + columns_remark;
                            }
                            catch (Exception Ex)
                            {
                                MessageBox.Show(Ex.Message.ToString(), "Transmit the Quantum command fail !");
                            }
                        }
                        #endregion

                        #region -- Dektec --
                        else if (columns_command == "_dektec")
                        {
                            if (columns_switch == "_start")
                            {
                                Console.WriteLine("Dektec control: _start");
                                string StreamName = columns_serial;
                                string TvSystem = columns_function;
                                string Freq = columns_subFunction;
                                string arguments = Application.StartupPath + @"\\DektecPlayer\\" + StreamName + " " +
                                                   "-mt " + TvSystem + " " +
                                                   "-mf " + Freq + " " +
                                                   "-r 0 " +
                                                   "-l 0";

                                Console.WriteLine(arguments);
                                System.Diagnostics.Process Dektec = new System.Diagnostics.Process();
                                Dektec.StartInfo.FileName = Application.StartupPath + @"\\DektecPlayer\\DtPlay.exe";
                                Dektec.StartInfo.UseShellExecute = false;
                                Dektec.StartInfo.RedirectStandardInput = true;
                                Dektec.StartInfo.RedirectStandardOutput = true;
                                Dektec.StartInfo.RedirectStandardError = true;
                                Dektec.StartInfo.CreateNoWindow = true;

                                Dektec.StartInfo.Arguments = arguments;
                                Dektec.Start();
                                label_Command.Text = "(" + columns_command + ") " + columns_serial;
                            }

                            if (columns_switch == "_stop")
                            {
                                Console.WriteLine("Dektec control: _stop");
                                CloseDtplay();
                            }
                        }
                        #endregion

                        #region -- 命令提示 --
                        else if (columns_command == "_DOS")
                        {
                            Console.WriteLine("DOS command: _DOS");
                            if (columns_serial != "")
                            {
                                string Command = columns_serial;

                                System.Diagnostics.Process p = new Process();
                                p.StartInfo.FileName = "cmd.exe";
                                p.StartInfo.WorkingDirectory = ini12.INIRead(MainSettingPath, "Device", "DOS", "");
                                p.StartInfo.UseShellExecute = false;
                                p.StartInfo.RedirectStandardInput = true;
                                p.StartInfo.RedirectStandardOutput = true;
                                p.StartInfo.RedirectStandardError = true;
                                p.StartInfo.CreateNoWindow = true; //不跳出cmd視窗
                                string strOutput = null;

                                try
                                {
                                    p.Start();
                                    p.StandardInput.WriteLine(Command);
                                    label_Command.Text = "DOS CMD_" + columns_serial;
                                    //p.StandardInput.WriteLine("exit");
                                    //strOutput = p.StandardOutput.ReadToEnd();//匯出整個執行過程
                                    //p.WaitForExit();
                                    //p.Close();
                                }
                                catch (Exception e)
                                {
                                    strOutput = e.Message;
                                }
                            }
                        }
                        #endregion

                        #region -- GPIO_INPUT_OUTPUT --
                        else if (columns_command == "_IO_Input")
                        {
                            Console.WriteLine("GPIO control: _IO_Input");
                            IO_INPUT();
                        }

                        else if (columns_command == "_IO_Output")
                        {
                            Console.WriteLine("GPIO control: _IO_Output");
                            //string GPIO = "01010101";
                            string GPIO = columns_times;
                            byte GPIO_B = Convert.ToByte(GPIO, 2);
                            MyBlueRat.Set_GPIO_Output(GPIO_B);
                            label_Command.Text = "(" + columns_command + ") " + columns_times;
                        }
                        #endregion

                        #region -- Extend_GPIO_OUTPUT --
                        else if (columns_command == "_WaterTemp")
                        {
                            Console.WriteLine("Extend GPIO control: _WaterTemp");
                            string GPIO = columns_times; // GPIO = "010101010";
                            if (GPIO.Length == 9)
                            {
                                for (int i = 0; i < 9; i++)
                                {
                                    MyBlueRat.Set_IO_Extend_Set_Pin(Convert.ToByte(i), Convert.ToByte(GPIO.Substring(8 - i, 1)));
                                    Thread.Sleep(50);
                                }
                            }
                            else
                            {
                                MessageBox.Show("Please check the value equal nine.");
                            }
                            label_Command.Text = "(" + columns_command + ") " + columns_times;
                        }

                        else if (columns_command == "_FuelDisplay")
                        {
                            Console.WriteLine("Extend GPIO control: _FuelDisplay");
                            string GPIO = columns_times;
                            if (GPIO.Length == 9)
                            {
                                for (int i = 0; i < 9; i++)
                                {
                                    MyBlueRat.Set_IO_Extend_Set_Pin(Convert.ToByte(i + 16), Convert.ToByte(GPIO.Substring(8 - i, 1)));
                                    Thread.Sleep(50);
                                }
                            }
                            else
                            {
                                MessageBox.Show("Please check the value equal nine.");
                            }
                            label_Command.Text = "(" + columns_command + ") " + columns_times;
                        }

                        else if (columns_command == "_Temperature")
                        {
                            Console.WriteLine("Extend GPIO control: _Temperature");
                            //string GPIO = "01010101";
                            string GPIO = columns_serial;
                            int GPIO_B = int.Parse(GPIO);
                            if (GPIO_B >= -20 && GPIO_B <= 50)
                            {
                                if (GPIO_B >= -20 && GPIO_B < -17)
                                    MyBlueRat.Set_MCP42xxx(224);
                                else if (GPIO_B >= -17 && GPIO_B < -12)
                                    MyBlueRat.Set_MCP42xxx(172);
                                else if (GPIO_B >= -12 && GPIO_B < -7)
                                    MyBlueRat.Set_MCP42xxx(130);
                                else if (GPIO_B >= -7 && GPIO_B < -2)
                                    MyBlueRat.Set_MCP42xxx(101);
                                else if (GPIO_B >= -2 && GPIO_B < 3)
                                    MyBlueRat.Set_MCP42xxx(78);
                                else if (GPIO_B >= 3 && GPIO_B < 8)
                                    MyBlueRat.Set_MCP42xxx(61);
                                else if (GPIO_B >= 8 && GPIO_B < 13)
                                    MyBlueRat.Set_MCP42xxx(47);
                                else if (GPIO_B >= 13 && GPIO_B < 18)
                                    MyBlueRat.Set_MCP42xxx(36);
                                else if (GPIO_B >= 18 && GPIO_B < 23)
                                    MyBlueRat.Set_MCP42xxx(29);
                                else if (GPIO_B >= 23 && GPIO_B < 28)
                                    MyBlueRat.Set_MCP42xxx(23);
                                else if (GPIO_B >= 28 && GPIO_B < 33)
                                    MyBlueRat.Set_MCP42xxx(19);
                                else if (GPIO_B >= 33 && GPIO_B < 38)
                                    MyBlueRat.Set_MCP42xxx(15);
                                else if (GPIO_B >= 38 && GPIO_B < 43)
                                    MyBlueRat.Set_MCP42xxx(12);
                                else if (GPIO_B >= 43 && GPIO_B < 48)
                                    MyBlueRat.Set_MCP42xxx(10);
                                else if (GPIO_B >= 48 && GPIO_B <= 50)
                                    MyBlueRat.Set_MCP42xxx(8);
                                Thread.Sleep(50);
                            }
                            label_Command.Text = "(" + columns_command + ") " + columns_times;
                        }
                        #endregion

                        #region -- Push_Release_Function--
                        else if (columns_command == "_FuncKey")
                        {
                            try
                            {
                                for (int k = 0; k < stime; k++)
                                {
                                    Console.WriteLine("Extend GPIO control: _FuncKey:" + k + " times");
                                    label_Command.Text = "(Push CMD)" + columns_serial;
                                    if (ini12.INIRead(MainSettingPath, "Port A", "Checked", "") == "1" && columns_comport == "A")
                                    {
                                        if (columns_serial == "_save")
                                        {
                                            Serialportsave("A"); //存檔rs232
                                        }
                                        else if (columns_serial == "_clear")
                                        {
                                            log1_text = string.Empty; //清除textbox1
                                        }
                                        else if (columns_serial != "" && columns_switch == @"\r")
                                        {
                                            PortA.Write(columns_serial + "\r"); //發送數據 Rs232 + \r
                                        }
                                        else if (columns_serial != "" && columns_switch == @"\n")
                                        {
                                            PortA.Write(columns_serial + "\n"); //發送數據 Rs232 + \n
                                        }
                                        else if (columns_serial != "" && columns_switch == @"\n\r")
                                        {
                                            PortA.Write(columns_serial + "\n\r"); //發送數據 Rs232 + \n\r
                                        }
                                        else if (columns_serial != "" && columns_switch == @"\r\n")
                                        {
                                            PortA.Write(columns_serial + "\r\n"); //發送數據 Rs232 + \r\n
                                        }
                                        else if (columns_serial != "" && columns_switch == "")
                                        {
                                            PortA.Write(columns_serial); //發送數據 HEX Rs232
                                        }
                                        DateTime dt = DateTime.Now;
                                        string text = "[Port_A] [" + dt.ToString("yyyy/MM/dd HH:mm:ss.fff") + "]  " + columns_serial + "\r\n";
                                        log1_text = string.Concat(log1_text, text);
                                        logAll_text = string.Concat(logAll_text, text);
                                    }
                                    else if (ini12.INIRead(MainSettingPath, "Port B", "Checked", "") == "1" && columns_comport == "B")
                                    {
                                        if (columns_serial == "_save")
                                        {
                                            Serialportsave("B"); //存檔rs232
                                        }
                                        else if (columns_serial == "_clear")
                                        {
                                            log2_text = string.Empty; //清除log2_text
                                        }
                                        else if (columns_serial != "" && columns_switch == @"\r")
                                        {
                                            PortB.Write(columns_serial + "\r"); //發送數據 Rs232 + \r
                                        }
                                        else if (columns_serial != "" && columns_switch == @"\n")
                                        {
                                            PortB.Write(columns_serial + "\n"); //發送數據 Rs232 + \n
                                        }
                                        else if (columns_serial != "" && columns_switch == @"\n\r")
                                        {
                                            PortB.Write(columns_serial + "\n\r"); //發送數據 Rs232 + \n\r
                                        }
                                        else if (columns_serial != "" && columns_switch == @"\r\n")
                                        {
                                            PortB.Write(columns_serial + "\r\n"); //發送數據 Rs232 + \r\n
                                        }
                                        else if (columns_serial != "" && columns_switch == "")
                                        {
                                            PortB.Write(columns_serial); //發送數據 HEX Rs232
                                        }
                                        DateTime dt = DateTime.Now;
                                        string text = "[Port_B] [" + dt.ToString("yyyy/MM/dd HH:mm:ss.fff") + "]  " + columns_serial + "\r\n";
                                        log2_text = string.Concat(log2_text, text);
                                        logAll_text = string.Concat(logAll_text, text);
                                    }
                                    else if (ini12.INIRead(MainSettingPath, "Port C", "Checked", "") == "1" && columns_comport == "C")
                                    {
                                        if (columns_serial == "_save")
                                        {
                                            Serialportsave("C"); //存檔rs232
                                        }
                                        else if (columns_serial == "_clear")
                                        {
                                            log3_text = string.Empty; //清除log3_text
                                        }
                                        else if (columns_serial != "" && columns_switch == @"\r")
                                        {
                                            PortC.Write(columns_serial + "\r"); //發送數據 Rs232 + \r
                                        }
                                        else if (columns_serial != "" && columns_switch == @"\n")
                                        {
                                            PortC.Write(columns_serial + "\n"); //發送數據 Rs232 + \n
                                        }
                                        else if (columns_serial != "" && columns_switch == @"\n\r")
                                        {
                                            PortC.Write(columns_serial + "\n\r"); //發送數據 Rs232 + \n\r
                                        }
                                        else if (columns_serial != "" && columns_switch == @"\r\n")
                                        {
                                            PortC.Write(columns_serial + "\r\n"); //發送數據 Rs232 + \r\n
                                        }
                                        else if (columns_serial != "" && columns_switch == "")
                                        {
                                            PortC.Write(columns_serial); //發送數據 HEX Rs232
                                        }
                                        DateTime dt = DateTime.Now;
                                        string text = "[Port_C] [" + dt.ToString("yyyy/MM/dd HH:mm:ss.fff") + "]  " + columns_serial + "\r\n";
                                        log3_text = string.Concat(log3_text, text);
                                        logAll_text = string.Concat(logAll_text, text);
                                    }
                                    else if (ini12.INIRead(MainSettingPath, "Port D", "Checked", "") == "1" && columns_comport == "D")
                                    {
                                        if (columns_serial == "_save")
                                        {
                                            Serialportsave("D"); //存檔rs232
                                        }
                                        else if (columns_serial == "_clear")
                                        {
                                            log4_text = string.Empty; //清除log4_text
                                        }
                                        else if (columns_serial != "" && columns_switch == @"\r")
                                        {
                                            PortD.Write(columns_serial + "\r"); //發送數據 Rs232 + \r
                                        }
                                        else if (columns_serial != "" && columns_switch == @"\n")
                                        {
                                            PortD.Write(columns_serial + "\n"); //發送數據 Rs232 + \n
                                        }
                                        else if (columns_serial != "" && columns_switch == @"\n\r")
                                        {
                                            PortD.Write(columns_serial + "\n\r"); //發送數據 Rs232 + \n\r
                                        }
                                        else if (columns_serial != "" && columns_switch == @"\r\n")
                                        {
                                            PortD.Write(columns_serial + "\r\n"); //發送數據 Rs232 + \r\n
                                        }
                                        else if (columns_serial != "" && columns_switch == "")
                                        {
                                            PortD.Write(columns_serial); //發送數據 HEX Rs232
                                        }
                                        DateTime dt = DateTime.Now;
                                        string text = "[Port_D] [" + dt.ToString("yyyy/MM/dd HH:mm:ss.fff") + "]  " + columns_serial + "\r\n";
                                        log4_text = string.Concat(log4_text, text);
                                        logAll_text = string.Concat(logAll_text, text);
                                    }
                                    else if (ini12.INIRead(MainSettingPath, "Port E", "Checked", "") == "1" && columns_comport == "E")
                                    {
                                        if (columns_serial == "_save")
                                        {
                                            Serialportsave("E"); //存檔rs232
                                        }
                                        else if (columns_serial == "_clear")
                                        {
                                            log5_text = string.Empty; //清除log5_text
                                        }
                                        else if (columns_serial != "" && columns_switch == @"\r")
                                        {
                                            PortE.Write(columns_serial + "\r"); //發送數據 Rs232 + \r
                                        }
                                        else if (columns_serial != "" && columns_switch == @"\n")
                                        {
                                            PortE.Write(columns_serial + "\n"); //發送數據 Rs232 + \n
                                        }
                                        else if (columns_serial != "" && columns_switch == @"\n\r")
                                        {
                                            PortE.Write(columns_serial + "\n\r"); //發送數據 Rs232 + \n\r
                                        }
                                        else if (columns_serial != "" && columns_switch == @"\r\n")
                                        {
                                            PortE.Write(columns_serial + "\r\n"); //發送數據 Rs232 + \r\n
                                        }
                                        else if (columns_serial != "" && columns_switch == "")
                                        {
                                            PortE.Write(columns_serial); //發送數據 HEX Rs232
                                        }
                                        DateTime dt = DateTime.Now;
                                        string text = "[Port_E] [" + dt.ToString("yyyy/MM/dd HH:mm:ss.fff") + "]  " + columns_serial + "\r\n";
                                        log5_text = string.Concat(log5_text, text);
                                        logAll_text = string.Concat(logAll_text, text);
                                    }
                                    //label_Command.Text = "(" + columns_command + ") " + columns_serial;
                                    Console.WriteLine("Extend GPIO control: _FuncKey Delay:" + sRepeat + " ms");
                                    Thread.Sleep(sRepeat);
                                    int length = columns_serial.Length;
                                    string status = columns_serial.Substring(length - 1, 1);
                                    string reverse = "";
                                    if (status == "0")
                                        reverse = columns_serial.Substring(0, length - 1) + "1";
                                    else if (status == "1")
                                        reverse = columns_serial.Substring(0, length - 1) + "0";
                                    label_Command.Text = "(Release CMD)" + reverse;

                                    if (ini12.INIRead(MainSettingPath, "Port A", "Checked", "") == "1" && columns_comport == "A")
                                    {
                                        if (reverse == "_save")
                                        {
                                            Serialportsave("A"); //存檔rs232
                                        }
                                        else if (reverse == "_clear")
                                        {
                                            log1_text = string.Empty; //清除textbox1
                                        }
                                        else if (reverse != "" && columns_switch == @"\r")
                                        {
                                            PortA.Write(reverse + "\r"); //發送數據 Rs232 + \r
                                        }
                                        else if (reverse != "" && columns_switch == @"\n")
                                        {
                                            PortA.Write(reverse + "\n"); //發送數據 Rs232 + \n
                                        }
                                        else if (reverse != "" && columns_switch == @"\n\r")
                                        {
                                            PortA.Write(reverse + "\n\r"); //發送數據 Rs232 + \n\r
                                        }
                                        else if (reverse != "" && columns_switch == @"\r\n")
                                        {
                                            PortA.Write(reverse + "\r\n"); //發送數據 Rs232 + \r\n
                                        }
                                        else if (reverse != "" && columns_switch == "")
                                        {
                                            PortA.Write(reverse); //發送數據 HEX Rs232
                                        }
                                        DateTime dt = DateTime.Now;
                                        string text = "[Port_A] [" + dt.ToString("yyyy/MM/dd HH:mm:ss.fff") + "]  " + reverse + "\r\n";
                                        log1_text = string.Concat(log1_text, text);
                                        logAll_text = string.Concat(logAll_text, text);
                                    }
                                    else if (ini12.INIRead(MainSettingPath, "Port B", "Checked", "") == "1" && columns_comport == "B")
                                    {
                                        if (reverse == "_save")
                                        {
                                            Serialportsave("B"); //存檔rs232
                                        }
                                        else if (reverse == "_clear")
                                        {
                                            log2_text = string.Empty; //清除log2_text
                                        }
                                        else if (reverse != "" && columns_switch == @"\r")
                                        {
                                            PortB.Write(reverse + "\r"); //發送數據 Rs232 + \r
                                        }
                                        else if (reverse != "" && columns_switch == @"\n")
                                        {
                                            PortB.Write(reverse + "\n"); //發送數據 Rs232 + \n
                                        }
                                        else if (reverse != "" && columns_switch == @"\n\r")
                                        {
                                            PortB.Write(reverse + "\n\r"); //發送數據 Rs232 + \n\r
                                        }
                                        else if (reverse != "" && columns_switch == @"\r\n")
                                        {
                                            PortB.Write(reverse + "\r\n"); //發送數據 Rs232 + \r\n
                                        }
                                        else if (reverse != "" && columns_switch == "")
                                        {
                                            PortB.Write(reverse); //發送數據 HEX Rs232
                                        }
                                        DateTime dt = DateTime.Now;
                                        string text = "[Port_B] [" + dt.ToString("yyyy/MM/dd HH:mm:ss.fff") + "]  " + reverse + "\r\n";
                                        log2_text = string.Concat(log2_text, text);
                                        logAll_text = string.Concat(logAll_text, text);
                                    }
                                    else if (ini12.INIRead(MainSettingPath, "Port C", "Checked", "") == "1" && columns_comport == "C")
                                    {
                                        if (reverse == "_save")
                                        {
                                            Serialportsave("C"); //存檔rs232
                                        }
                                        else if (reverse == "_clear")
                                        {
                                            log3_text = string.Empty; //清除log3_text
                                        }
                                        else if (reverse != "" && columns_switch == @"\r")
                                        {
                                            PortC.Write(reverse + "\r"); //發送數據 Rs232 + \r
                                        }
                                        else if (reverse != "" && columns_switch == @"\n")
                                        {
                                            PortC.Write(reverse + "\n"); //發送數據 Rs232 + \n
                                        }
                                        else if (reverse != "" && columns_switch == @"\n\r")
                                        {
                                            PortC.Write(reverse + "\n\r"); //發送數據 Rs232 + \n\r
                                        }
                                        else if (reverse != "" && columns_switch == @"\r\n")
                                        {
                                            PortC.Write(reverse + "\r\n"); //發送數據 Rs232 + \r\n
                                        }
                                        else if (reverse != "" && columns_switch == "")
                                        {
                                            PortC.Write(reverse); //發送數據 HEX Rs232
                                        }
                                        DateTime dt = DateTime.Now;
                                        string text = "[Port_C] [" + dt.ToString("yyyy/MM/dd HH:mm:ss.fff") + "]  " + reverse + "\r\n";
                                        log3_text = string.Concat(log3_text, text);
                                        logAll_text = string.Concat(logAll_text, text);
                                    }
                                    else if (ini12.INIRead(MainSettingPath, "Port D", "Checked", "") == "1" && columns_comport == "D")
                                    {
                                        if (reverse == "_save")
                                        {
                                            Serialportsave("D"); //存檔rs232
                                        }
                                        else if (reverse == "_clear")
                                        {
                                            log4_text = string.Empty; //清除log4_text
                                        }
                                        else if (reverse != "" && columns_switch == @"\r")
                                        {
                                            PortD.Write(reverse + "\r"); //發送數據 Rs232 + \r
                                        }
                                        else if (reverse != "" && columns_switch == @"\n")
                                        {
                                            PortD.Write(reverse + "\n"); //發送數據 Rs232 + \n
                                        }
                                        else if (reverse != "" && columns_switch == @"\n\r")
                                        {
                                            PortD.Write(reverse + "\n\r"); //發送數據 Rs232 + \n\r
                                        }
                                        else if (reverse != "" && columns_switch == @"\r\n")
                                        {
                                            PortD.Write(reverse + "\r\n"); //發送數據 Rs232 + \r\n
                                        }
                                        else if (reverse != "" && columns_switch == "")
                                        {
                                            PortD.Write(reverse); //發送數據 HEX Rs232
                                        }
                                        DateTime dt = DateTime.Now;
                                        string text = "[Port_D] [" + dt.ToString("yyyy/MM/dd HH:mm:ss.fff") + "]  " + reverse + "\r\n";
                                        log4_text = string.Concat(log4_text, text);
                                        logAll_text = string.Concat(logAll_text, text);
                                    }
                                    else if (ini12.INIRead(MainSettingPath, "Port E", "Checked", "") == "1" && columns_comport == "E")
                                    {
                                        if (reverse == "_save")
                                        {
                                            Serialportsave("E"); //存檔rs232
                                        }
                                        else if (reverse == "_clear")
                                        {
                                            log5_text = string.Empty; //清除log5_text
                                        }
                                        else if (reverse != "" && columns_switch == @"\r")
                                        {
                                            PortE.Write(reverse + "\r"); //發送數據 Rs232 + \r
                                        }
                                        else if (reverse != "" && columns_switch == @"\n")
                                        {
                                            PortE.Write(reverse + "\n"); //發送數據 Rs232 + \n
                                        }
                                        else if (reverse != "" && columns_switch == @"\n\r")
                                        {
                                            PortE.Write(reverse + "\n\r"); //發送數據 Rs232 + \n\r
                                        }
                                        else if (reverse != "" && columns_switch == @"\r\n")
                                        {
                                            PortE.Write(reverse + "\r\n"); //發送數據 Rs232 + \r\n
                                        }
                                        else if (reverse != "" && columns_switch == "")
                                        {
                                            PortE.Write(reverse); //發送數據 HEX Rs232
                                        }
                                        DateTime dt = DateTime.Now;
                                        string text = "[Port_E] [" + dt.ToString("yyyy/MM/dd HH:mm:ss.fff") + "]  " + reverse + "\r\n";
                                        log5_text = string.Concat(log5_text, text);
                                        logAll_text = string.Concat(logAll_text, text);
                                    }
                                    //label_Command.Text = "(" + columns_command + ") " + columns_serial;
                                    Thread.Sleep(500);
                                }
                            }
                            catch (Exception Ex)
                            {
                                MessageBox.Show(Ex.Message.ToString(), "SerialPort setting fail !");
                            }
                        }
                        #endregion

                        #region -- IO CMD --
                        else if (columns_command == "_Pin" && columns_comport.Length >= 7 && columns_comport.Substring(0, 3) == "_PA" ||
                                 columns_command == "_Pin" && columns_comport.Length >= 7 && columns_comport.Substring(0, 3) == "_PB")
                        {
                            {
                                switch (columns_comport.Substring(3, 2))
                                {
                                    #region -- PA10 --
                                    case "10":
                                        Console.WriteLine("IO CMD: PA10");
                                        if (columns_comport.Substring(6, 1) == "0" &&
                                            Global.IO_INPUT.Substring(10, 1) == "0")
                                        {
                                            if (columns_serial == "_accumulate")
                                            {
                                                Global.IO_PA10_0_COUNT++;
                                                label_Command.Text = "IO CMD_ACCUMULATE";
                                            }
                                            else
                                            {
                                                IO_CMD();
                                            }
                                        }
                                        else if (columns_comport.Substring(6, 1) == "1" &&
                                            Global.IO_INPUT.Substring(10, 1) == "1")
                                        {
                                            if (columns_serial == "_accumulate")
                                            {
                                                Global.IO_PA10_1_COUNT++;
                                                label_Command.Text = "IO CMD_ACCUMULATE";
                                            }
                                            else
                                            {
                                                IO_CMD();
                                            }
                                        }
                                        else
                                        {
                                            SysDelay = 0;
                                        }
                                        break;
                                    #endregion

                                    #region -- PA11 --
                                    case "11":
                                        Console.WriteLine("IO CMD: PA11");
                                        if (columns_comport.Substring(6, 1) == "0" &&
                                            Global.IO_INPUT.Substring(8, 1) == "0")
                                        {
                                            if (columns_serial == "_accumulate")
                                            {
                                                Global.IO_PA11_0_COUNT++;
                                                label_Command.Text = "IO CMD_ACCUMULATE";
                                            }
                                            else
                                                IO_CMD();
                                        }
                                        else if (columns_comport.Substring(6, 1) == "1" &&
                                            Global.IO_INPUT.Substring(8, 1) == "1")
                                        {
                                            if (columns_serial == "_accumulate")
                                            {
                                                Global.IO_PA11_1_COUNT++;
                                                label_Command.Text = "IO CMD_ACCUMULATE";
                                            }
                                            else
                                                IO_CMD();
                                        }
                                        else
                                        {
                                            SysDelay = 0;
                                        }
                                        break;
                                    #endregion

                                    #region -- PA14 --
                                    case "14":
                                        Console.WriteLine("IO CMD: PA14");
                                        if (columns_comport.Substring(6, 1) == "0" &&
                                            Global.IO_INPUT.Substring(6, 1) == "0")
                                        {
                                            if (columns_serial == "_accumulate")
                                            {
                                                Global.IO_PA14_0_COUNT++;
                                                label_Command.Text = "IO CMD_ACCUMULATE";
                                            }
                                            else
                                                IO_CMD();
                                        }
                                        else if (columns_comport.Substring(6, 1) == "1" &&
                                            Global.IO_INPUT.Substring(6, 1) == "1")
                                        {
                                            if (columns_serial == "_accumulate")
                                            {
                                                Global.IO_PA14_1_COUNT++;
                                                label_Command.Text = "IO CMD_ACCUMULATE";
                                            }
                                            else
                                                IO_CMD();
                                        }
                                        else
                                        {
                                            SysDelay = 0;
                                        }
                                        break;
                                    #endregion

                                    #region -- PA15 --
                                    case "15":
                                        Console.WriteLine("IO CMD: PA15");
                                        if (columns_comport.Substring(6, 1) == "0" &&
                                            Global.IO_INPUT.Substring(4, 1) == "0")
                                        {
                                            if (columns_serial == "_accumulate")
                                            {
                                                Global.IO_PA15_0_COUNT++;
                                                label_Command.Text = "IO CMD_ACCUMULATE";
                                            }
                                            else
                                                IO_CMD();
                                        }
                                        else if (columns_comport.Substring(6, 1) == "1" &&
                                            Global.IO_INPUT.Substring(4, 1) == "1")
                                        {
                                            if (columns_serial == "_accumulate")
                                            {
                                                Global.IO_PA15_1_COUNT++;
                                                label_Command.Text = "IO CMD_ACCUMULATE";
                                            }
                                            else
                                                IO_CMD();
                                        }
                                        else
                                        {
                                            SysDelay = 0;
                                        }
                                        break;
                                    #endregion

                                    #region -- PB01 --
                                    case "01":
                                        Console.WriteLine("IO CMD: PB01");
                                        if (columns_comport.Substring(6, 1) == "0" &&
                                            Global.IO_INPUT.Substring(2, 1) == "0")
                                        {
                                            if (columns_serial == "_accumulate")
                                            {
                                                Global.IO_PB1_0_COUNT++;
                                                label_Command.Text = "IO CMD_ACCUMULATE";
                                            }

                                            else
                                                IO_CMD();
                                        }
                                        else if (columns_comport.Substring(6, 1) == "1" &&
                                            Global.IO_INPUT.Substring(2, 1) == "1")
                                        {
                                            if (columns_serial == "_accumulate")
                                            {
                                                Global.IO_PB1_1_COUNT++;
                                                label_Command.Text = "IO CMD_ACCUMULATE";
                                            }
                                            else
                                                IO_CMD();
                                        }
                                        else
                                        {
                                            SysDelay = 0;
                                        }
                                        break;
                                    #endregion

                                    #region -- PB07 --
                                    case "07":
                                        Console.WriteLine("IO CMD: PB07");
                                        if (columns_comport.Substring(6, 1) == "0" &&
                                            Global.IO_INPUT.Substring(0, 1) == "0")
                                        {
                                            if (columns_serial == "_accumulate")
                                            {
                                                Global.IO_PB7_0_COUNT++;
                                                label_Command.Text = "IO CMD_ACCUMULATE";
                                            }
                                            else
                                                IO_CMD();
                                        }
                                        else if (columns_comport.Substring(6, 1) == "1" &&
                                            Global.IO_INPUT.Substring(0, 1) == "1")
                                        {
                                            if (columns_serial == "_accumulate")
                                            {
                                                Global.IO_PB7_1_COUNT++;
                                                label_Command.Text = "IO CMD_ACCUMULATE";
                                            }
                                            else
                                                IO_CMD();
                                        }
                                        else
                                        {
                                            SysDelay = 0;
                                        }
                                        break;
                                        #endregion
                                }
                            }
                        }
                        #endregion

                        #region -- Audio Debounce --
                        else if (columns_command == "_audio_debounce")
                        {
                            Console.WriteLine("Audio Detect: _audio_debounce");
                            bool Debounce_Time_PB1, Debounce_Time_PB7;
                            if (columns_interval != "")
                            {
                                MyBlueRat.Set_Input_GPIO_Low_Debounce_Time_PB1(Convert.ToUInt16(columns_interval));
                                MyBlueRat.Set_Input_GPIO_Low_Debounce_Time_PB7(Convert.ToUInt16(columns_interval));
                                Debounce_Time_PB1 = MyBlueRat.Set_Input_GPIO_Low_Debounce_Time_PB1(Convert.ToUInt16(columns_interval));
                                Debounce_Time_PB7 = MyBlueRat.Set_Input_GPIO_Low_Debounce_Time_PB7(Convert.ToUInt16(columns_interval));
                            }
                            else
                            {
                                MyBlueRat.Set_Input_GPIO_Low_Debounce_Time_PB1();
                                MyBlueRat.Set_Input_GPIO_Low_Debounce_Time_PB7();
                                Debounce_Time_PB1 = MyBlueRat.Set_Input_GPIO_Low_Debounce_Time_PB1();
                                Debounce_Time_PB7 = MyBlueRat.Set_Input_GPIO_Low_Debounce_Time_PB7();
                            }
                        }
                        #endregion

                        #region -- Keyword Search --
                        else if (columns_command == "_keyword")
                        {
                            switch (columns_times)
                            {
                                case "1":
                                    Console.WriteLine("Keyword Search: 1");
                                    if (Global.keyword_1 == "true")
                                    {
                                        KeywordCommand();
                                    }
                                    else
                                    {
                                        SysDelay = 0;
                                    }
                                    Global.keyword_1 = "false";
                                    break;

                                case "2":
                                    Console.WriteLine("Keyword Search: 2");
                                    if (Global.keyword_2 == "true")
                                    {
                                        KeywordCommand();
                                    }
                                    else
                                    {
                                        SysDelay = 0;
                                    }
                                    Global.keyword_2 = "false";
                                    break;

                                case "3":
                                    Console.WriteLine("Keyword Search: 3");
                                    if (Global.keyword_3 == "true")
                                    {
                                        KeywordCommand();
                                    }
                                    else
                                    {
                                        SysDelay = 0;
                                    }
                                    Global.keyword_3 = "false";
                                    break;

                                case "4":
                                    Console.WriteLine("Keyword Search: 4");
                                    if (Global.keyword_4 == "true")
                                    {
                                        KeywordCommand();
                                    }
                                    else
                                    {
                                        SysDelay = 0;
                                    }
                                    Global.keyword_4 = "false";
                                    break;

                                case "5":
                                    Console.WriteLine("Keyword Search: 5");
                                    if (Global.keyword_5 == "true")
                                    {
                                        KeywordCommand();
                                    }
                                    else
                                    {
                                        SysDelay = 0;
                                    }
                                    Global.keyword_5 = "false";
                                    break;

                                case "6":
                                    Console.WriteLine("Keyword Search: 6");
                                    if (Global.keyword_6 == "true")
                                    {
                                        KeywordCommand();
                                    }
                                    else
                                    {
                                        SysDelay = 0;
                                    }
                                    Global.keyword_6 = "false";
                                    break;

                                case "7":
                                    Console.WriteLine("Keyword Search: 7");
                                    if (Global.keyword_7 == "true")
                                    {
                                        KeywordCommand();
                                    }
                                    else
                                    {
                                        SysDelay = 0;
                                    }
                                    Global.keyword_7 = "false";
                                    break;

                                case "8":
                                    Console.WriteLine("Keyword Search: 8");
                                    if (Global.keyword_8 == "true")
                                    {
                                        KeywordCommand();
                                    }
                                    else
                                    {
                                        SysDelay = 0;
                                    }
                                    Global.keyword_8 = "false";
                                    break;

                                case "9":
                                    Console.WriteLine("Keyword Search: 9");
                                    if (Global.keyword_9 == "true")
                                    {
                                        KeywordCommand();
                                    }
                                    else
                                    {
                                        SysDelay = 0;
                                    }
                                    Global.keyword_9 = "false";
                                    break;

                                default:
                                    Console.WriteLine("Keyword Search: 10");
                                    if (columns_times == "10")
                                    {
                                        if (Global.keyword_10 == "true")
                                        {
                                            KeywordCommand();
                                        }
                                        else
                                        {
                                            SysDelay = 0;
                                        }
                                        Global.keyword_10 = "false";
                                    }
                                    Console.WriteLine("keyword not found_schedule");
                                    break;

                            }
                        }
                        #endregion

                        #region -- PWM1 --
                        else if (columns_command == "_pwm1")
                        {
                            Console.WriteLine("PWM Control: _pwm1");
                            if (ini12.INIRead(MainSettingPath, "Port A", "Checked", "") == "1")
                            {
                                string pwm_output;
                                int result = 0;
                                if (columns_serial == "off")
                                {
                                    pwm_output = "set pwm_output 0";
                                    PortA.WriteLine(pwm_output);
                                }
                                else if (columns_serial == "on")
                                {
                                    pwm_output = "set pwm_output 1";
                                    PortA.WriteLine(pwm_output);
                                }
                                else if (int.TryParse(columns_serial, out result) == true)
                                {
                                    if (int.Parse(columns_serial) >= 0 && int.Parse(columns_serial) <= 100)
                                    {
                                        pwm_output = "set pwm_percent " + columns_serial;
                                        PortA.WriteLine(pwm_output);
                                    }
                                }
                                else
                                {
                                    pwm_output = columns_serial;
                                    PortA.WriteLine(pwm_output);
                                }
                            }
                        }
                        #endregion

                        #region -- PWM2 --
                        else if (columns_command == "_pwm2")
                        {
                            Console.WriteLine("PWM Control: _pwm2");
                            if (ini12.INIRead(MainSettingPath, "Port B", "Checked", "") == "1")
                            {
                                string pwm_output;
                                int result = 0;
                                if (columns_serial == "off")
                                {
                                    pwm_output = "set pwm_output 0";
                                    PortB.WriteLine(pwm_output);
                                }
                                else if (columns_serial == "on")
                                {
                                    pwm_output = "set pwm_output 1";
                                    PortB.WriteLine(pwm_output);
                                }
                                else if (int.TryParse(columns_serial, out result) == true)
                                {
                                    if (int.Parse(columns_serial) >= 0 && int.Parse(columns_serial) <= 100)
                                    {
                                        pwm_output = "set pwm_percent " + columns_serial;
                                        PortB.WriteLine(pwm_output);
                                    }
                                }
                                else
                                {
                                    pwm_output = columns_serial;
                                    PortB.WriteLine(pwm_output);
                                }
                            }
                        }
                        #endregion

                        #region -- PWM3 --
                        else if (columns_command == "_pwm3")
                        {
                            Console.WriteLine("PWM Control: _pwm3");
                            if (ini12.INIRead(MainSettingPath, "Port C", "Checked", "") == "1")
                            {
                                string pwm_output;
                                int result = 0;
                                if (columns_serial == "off")
                                {
                                    pwm_output = "set pwm_output 0";
                                    PortB.WriteLine(pwm_output);
                                }
                                else if (columns_serial == "on")
                                {
                                    pwm_output = "set pwm_output 1";
                                    PortB.WriteLine(pwm_output);
                                }
                                else if (int.TryParse(columns_serial, out result) == true)
                                {
                                    if (int.Parse(columns_serial) >= 0 && int.Parse(columns_serial) <= 100)
                                    {
                                        pwm_output = "set pwm_percent " + columns_serial;
                                        PortB.WriteLine(pwm_output);
                                    }
                                }
                                else
                                {
                                    pwm_output = columns_serial;
                                    PortB.WriteLine(pwm_output);
                                }
                            }
                        }
                        #endregion

                        #region -- Remark --
                        if (columns_remark != "")
                        {
                            label_Remark.Invoke((MethodInvoker)(() => label_Remark.Text = columns_remark));
                            //label_Remark.Text = columns_remark;
                        }
                        else
                        {
                            label_Remark.Text = "";
                        }
                        #endregion

                        //Thread MyExportText = new Thread(new ThreadStart(MyExportCamd));
                        //MyExportText.Start();

                        if (Global.Break_Out_Schedule == 1)//定時器時間到跳出迴圈//
                        {
                            Console.WriteLine("Break schedule.");
                            j = Global.Schedule_Loop;
                            UpdateUI(j.ToString(), label_LoopNumber_Value);
                            break;
                        }

                        Nowpoint = DataGridView_Schedule.Rows[Global.Scheduler_Row].Index;
                        Console.WriteLine("Nowpoint record: " + Nowpoint);
                        if (Breakfunction == true)
                        {
                            Console.WriteLine("Breakfunction.");
                            if (Breakpoint == Nowpoint)
                            {
                                Console.WriteLine("Breakpoint = Nowpoint");
                                button_Pause.PerformClick();
                            }
                        }

                        if (Pause == true)//如果按下暫停鈕//
                        {
                            timer1.Stop();
                            SchedulePause.WaitOne();
                        }
                        else
                        {
                            RedRatDBViewer_Delay(SysDelay);
                            Console.WriteLine("RedRatDBViewer_Delay.");
                        }

                        Console.WriteLine("End.");
                    }
                }
                Console.WriteLine("Loop_Number: " + Global.Loop_Number);
                Global.Loop_Number++;
            }

            #region -- Each video record when completed the schedule --
            if (ini12.INIRead(MainSettingPath, "Record", "EachVideo", "") == "1")
            {
                if (StartButtonPressed == true)
                {
                    if (ini12.INIRead(MainSettingPath, "Device", "CameraExist", "") == "1")
                    {
                        if (VideoRecording == false)
                        {
                            label_Command.Text = "Record Video...";
                            Thread.Sleep(1500);
                            Mysvideo(); // 開新檔
                            VideoRecording = true;
                            Thread oThreadC = new Thread(new ThreadStart(MySrtCamd));
                            oThreadC.Start();
                            Thread.Sleep(60000); // 錄影60秒

                            VideoRecording = false;
                            Mysstop();
                            oThreadC.Abort();
                            Thread.Sleep(1500);
                            label_Command.Text = "Vdieo recording completely.";
                        }
                    }
                    else
                    {
                        MessageBox.Show("Camera not exist", "Camera Open Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            #endregion

            #region -- schedule 切換 --
            if (StartButtonPressed != false)
            {
                if (Global.Schedule_2_Exist == 1 && Global.Schedule_Number == 1)
                {
                    button_Schedule2.PerformClick();
                    MyRunCamd();
                }
                else if (
                    Global.Schedule_3_Exist == 1 && Global.Schedule_Number == 1 ||
                    Global.Schedule_3_Exist == 1 && Global.Schedule_Number == 2)
                {
                    button_Schedule3.PerformClick();
                    MyRunCamd();
                }
                else if (
                    Global.Schedule_4_Exist == 1 && Global.Schedule_Number == 1 ||
                    Global.Schedule_4_Exist == 1 && Global.Schedule_Number == 2 ||
                    Global.Schedule_4_Exist == 1 && Global.Schedule_Number == 3)
                {
                    button_Schedule4.PerformClick();
                    MyRunCamd();
                }
                else if (
                    Global.Schedule_5_Exist == 1 && Global.Schedule_Number == 1 ||
                    Global.Schedule_5_Exist == 1 && Global.Schedule_Number == 2 ||
                    Global.Schedule_5_Exist == 1 && Global.Schedule_Number == 3 ||
                    Global.Schedule_5_Exist == 1 && Global.Schedule_Number == 4)
                {
                    button_Schedule5.PerformClick();
                    MyRunCamd();
                }
            }
            #endregion

            //全部schedule跑完或是按下stop鍵以後會跑以下這段/////////////////////////////////////////
            if (StartButtonPressed == false)//按下STOP讓schedule結束//
            {
                Global.Break_Out_MyRunCamd = 1;
                UpdateUI("START", button_Start);
                button_Start.Enabled = true;
                button_Setting.Enabled = true;
                button_Pause.Enabled = false;
                button_SaveSchedule.Enabled = true;
                setStyle();

                if (ini12.INIRead(MainSettingPath, "Device", "CameraExist", "") == "1")
                {
                    _captureInProgress = false;
                    OnOffCamera();
                }
            }
            else//schedule自動跑完//
            {
                StartButtonPressed = false;
                UpdateUI("START", button_Start);
                button_Setting.Enabled = true;
                button_Pause.Enabled = false;
                button_SaveSchedule.Enabled = true;
                button_Start.Enabled = true;

                if (ini12.INIRead(MainSettingPath, "Device", "CameraExist", "") == "1")
                {
                    _captureInProgress = false;
                    OnOffCamera();
                }

                Global.Total_Test_Time = Global.Schedule_1_TestTime + Global.Schedule_2_TestTime + Global.Schedule_3_TestTime + Global.Schedule_4_TestTime + Global.Schedule_5_TestTime;
                ConvertToRealTime(Global.Total_Test_Time);
            }

            label_Command.Text = "Completed!";
            label_Remark.Text = "";
            button_Schedule1.PerformClick();
            timer1.Stop();
            CloseDtplay();
            timeCount = Global.Schedule_1_TestTime;
            ConvertToRealTime(timeCount);
            setStyle();
        }
        #endregion

        #region -- IO CMD 指令集 --
        private void IO_CMD()
        {
            string columns_serial = DataGridView_Schedule.Rows[Global.Scheduler_Row].Cells[6].Value.ToString();
            if (columns_serial == "_pause")
            {
                button_Pause.PerformClick();
                label_Command.Text = "IO CMD_PAUSE";
            }
            else if (columns_serial == "_stop")
            {
                button_Start.PerformClick();
                label_Command.Text = "IO CMD_STOP";
            }
            else if (columns_serial == "_ac_restart")
            {
                GP0_GP1_AC_OFF_ON();
                Thread.Sleep(10);
                GP0_GP1_AC_OFF_ON();
                label_Command.Text = "IO CMD_AC_RESTART";
            }
            else if (columns_serial == "_shot")
            {
                Global.caption_Num++;
                if (Global.Loop_Number == 1)
                    Global.caption_Sum = Global.caption_Num;
                Jes();
                label_Command.Text = "IO CMD_SHOT";
            }
            else if (columns_serial.Substring(0, 7) == "_logcmd")
            {
                String log_cmd = columns_serial;
                int startIndex = 8;
                int length = log_cmd.Length - 8;
                String log_cmd_substring = log_cmd.Substring(startIndex, length);

                if (ini12.INIRead(MainSettingPath, "Port A", "Checked", "") == "1")
                {
                    PortA.WriteLine(log_cmd_substring);
                }
                else if (ini12.INIRead(MainSettingPath, "Port B", "Checked", "") == "1")
                {
                    PortB.WriteLine(log_cmd_substring);
                }
            }
        }
        #endregion

        #region -- KEYWORD 指令集 --
        private void KeywordCommand()
        {
            string columns_serial = DataGridView_Schedule.Rows[Global.Scheduler_Row].Cells[6].Value.ToString();
            if (columns_serial == "_pause")
            {
                button_Pause.PerformClick();
                label_Command.Text = "KEYWORD_PAUSE";
            }
            else if (columns_serial == "_stop")
            {
                button_Start.PerformClick();
                label_Command.Text = "KEYWORD_STOP";
            }
            else if (columns_serial == "_ac_restart")
            {
                GP0_GP1_AC_OFF_ON();
                Thread.Sleep(10);
                GP0_GP1_AC_OFF_ON();
                label_Command.Text = "KEYWORD_AC_RESTART";
            }
            else if (columns_serial == "_shot")
            {
                Global.caption_Num++;
                if (Global.Loop_Number == 1)
                    Global.caption_Sum = Global.caption_Num;
                Jes();
                label_Command.Text = "KEYWORD_SHOT";
            }
            else if (columns_serial == "_savelog1")
            {
                string fName = "";

                fName = ini12.INIRead(MainSettingPath, "Record", "LogPath", "");
                string t = fName + "\\_SaveLog1_" + DateTime.Now.ToString("yyyyMMddHHmmss") + "_" + label_LoopNumber_Value.Text + ".txt";

                StreamWriter MYFILE = new StreamWriter(t, false, Encoding.ASCII);
                MYFILE.Write(log1_text);
                MYFILE.Close();
                label_Command.Text = "KEYWORD_SAVELOG1";
            }
            else if (columns_serial == "_savelog2")
            {
                string fName = "";

                fName = ini12.INIRead(MainSettingPath, "Record", "LogPath", "");
                string t = fName + "\\_SaveLog2_" + DateTime.Now.ToString("yyyyMMddHHmmss") + "_" + label_LoopNumber_Value.Text + ".txt";

                StreamWriter MYFILE = new StreamWriter(t, false, Encoding.ASCII);
                MYFILE.Write(log2_text);
                MYFILE.Close();
                label_Command.Text = "KEYWORD_SAVELOG2";
            }
            else if (columns_serial.Substring(0, 7) == "_logcmd")
            {
                String log_cmd = columns_serial;
                int startIndex = 8;
                int length = log_cmd.Length - 8;
                String log_cmd_substring = log_cmd.Substring(startIndex, length);

                if (ini12.INIRead(MainSettingPath, "Port A", "Checked", "") == "1")
                {
                    PortA.WriteLine(log_cmd_substring);
                }
                else if (ini12.INIRead(MainSettingPath, "Port B", "Checked", "") == "1")
                {
                    PortB.WriteLine(log_cmd_substring);
                }
            }
        }
        #endregion

        #region -- 字幕 --
        private void MySrtCamd()
        {
            int count = 1;
            string starttime = "0:0:0";
            TimeSpan time_start = TimeSpan.Parse(DateTime.Now.ToString("HH:mm:ss"));

            while (VideoRecording)
            {
                System.Threading.Thread.Sleep(1000);
                TimeSpan time_end = TimeSpan.Parse(DateTime.Now.ToString("HH:mm:ss")); //計時結束 取得目前時間
                //後面的時間減前面的時間後 轉型成TimeSpan即可印出時間差
                string endtime = (time_end - time_start).Hours.ToString() + ":" + (time_end - time_start).Minutes.ToString() + ":" + (time_end - time_start).Seconds.ToString();
                StreamWriter srtWriter = new StreamWriter(srtstring, true);
                srtWriter.WriteLine(count);

                srtWriter.WriteLine(starttime + ",001" + " --> " + endtime + ",000");
                srtWriter.WriteLine(label_Command.Text + "     " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                srtWriter.WriteLine(label_Remark.Text);
                srtWriter.WriteLine("");
                srtWriter.WriteLine("");
                srtWriter.Close();
                count++;
                starttime = endtime;
            }
        }
        #endregion

        private void Mysvideo() => Invoke(new EventHandler(delegate { Savevideo(); }));//開始錄影//

        private void Mysstop() => Invoke(new EventHandler(delegate//停止錄影//
        {
            capture.Stop();
            capture.Dispose();
            Camstart();
        }));

        private void Savevideo()//儲存影片//
        {
            string fName = ini12.INIRead(MainSettingPath, "Record", "VideoPath", "");

            string t = fName + "\\" + "_pvr" + DateTime.Now.ToString("yyyyMMddHHmmss") + "__" + label_LoopNumber_Value.Text + ".avi";
            srtstring = fName + "\\" + "_pvr" + DateTime.Now.ToString("yyyyMMddHHmmss") + "__" + label_LoopNumber_Value.Text + ".srt";

            if (!capture.Cued)
                capture.Filename = t;

            capture.RecFileMode = DirectX.Capture.Capture.RecFileModeType.Avi; //宣告我要avi檔格式
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

        private void OnOffCamera()//啟動攝影機//
        {
            if (_captureInProgress == true)
            {
                Camstart();
            }

            if (_captureInProgress == false && capture != null)
            {
                capture.Stop();
                capture.Dispose();
            }
        }

        private void Camstart()
        {
            Filters filters = new Filters();
            Filter f;

            List<string> video = new List<string> { };
            for (int c = 0; c < filters.VideoInputDevices.Count; c++)
            {
                f = filters.VideoInputDevices[c];
                video.Add(f.Name);
            }

            List<string> audio = new List<string> { };
            for (int j = 0; j < filters.AudioInputDevices.Count; j++)
            {
                f = filters.AudioInputDevices[j];
                audio.Add(f.Name);
            }

            int scam = int.Parse(ini12.INIRead(MainSettingPath, "Camera", "VideoIndex", ""));
            int saud = int.Parse(ini12.INIRead(MainSettingPath, "Camera", "AudioIndex", ""));
            int VideoNum = int.Parse(ini12.INIRead(MainSettingPath, "Camera", "VideoNumber", ""));
            int AudioNum = int.Parse(ini12.INIRead(MainSettingPath, "Camera", "AudioNumber", ""));

            if (filters.VideoInputDevices.Count < VideoNum ||
                filters.AudioInputDevices.Count < AudioNum)
            {
                MessageBox.Show("Please reset video or audio device and select OK.", "Camera Status Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                button_Setting.PerformClick();
            }
            else
            {
                capture = new Capture(filters.VideoInputDevices[scam], filters.AudioInputDevices[saud]);
                try
                {
                    capture.FrameSize = new Size(2304, 1296);
                    ini12.INIWrite(MainSettingPath, "Camera", "Resolution", "2304*1296");
                }
                catch (Exception ex)
                {
                    Console.Write(ex.Message.ToString(), "Webcam does not support 2304*1296!\n\r");
                    try
                    {
                        capture.FrameSize = new Size(1920, 1080);
                        ini12.INIWrite(MainSettingPath, "Camera", "Resolution", "1920*1080");
                    }
                    catch (Exception ex1)
                    {
                        Console.Write(ex1.Message.ToString(), "Webcam does not support 1920*1080!\n\r");
                        try
                        {
                            capture.FrameSize = new Size(1280, 720);
                            ini12.INIWrite(MainSettingPath, "Camera", "Resolution", "1280*720");
                        }
                        catch (Exception ex2)
                        {
                            Console.Write(ex2.Message.ToString(), "Webcam does not support 1280*720!\n\r");
                            try
                            {
                                capture.FrameSize = new Size(640, 480);
                                ini12.INIWrite(MainSettingPath, "Camera", "Resolution", "640*480");
                            }
                            catch (Exception ex3)
                            {
                                Console.Write(ex3.Message.ToString(), "Webcam does not support 640*480!\n\r");
                                try
                                {
                                    capture.FrameSize = new Size(320, 240);
                                    ini12.INIWrite(MainSettingPath, "Camera", "Resolution", "320*240");
                                }
                                catch (Exception ex4)
                                {
                                    Console.Write(ex4.Message.ToString(), "Webcam does not support 320*240!\n\r");
                                }
                            }
                        }
                    }
                }
                capture.CaptureComplete += new EventHandler(OnCaptureComplete);
            }

            if (capture.PreviewWindow == null)
            {
                try
                {
                    capture.PreviewWindow = panelVideo;
                }
                catch (Exception ex)
                {
                    Console.Write(ex.Message.ToString(), "Please set the supported resolution!\n\r");
                }
            }
            else
            {
                capture.PreviewWindow = null;
            }
        }

        #region -- 讀取RC DB並填入combobox --
        private void LoadRCDB()
        {
            DataGridViewComboBoxColumn RCDB = (DataGridViewComboBoxColumn)DataGridView_Schedule.Columns[0];

            RCDB.Items.Add("------------------------");
            RCDB.Items.Add("_HEX");
            RCDB.Items.Add("_ascii");
            RCDB.Items.Add("------------------------");
            RCDB.Items.Add("_FuncKey");
            RCDB.Items.Add("_K_ABS");
            RCDB.Items.Add("_K_OBD");
            RCDB.Items.Add("_K_SEND");
            RCDB.Items.Add("_K_CLEAR");
            RCDB.Items.Add("_WaterTemp");
            RCDB.Items.Add("_FuelDisplay");
            RCDB.Items.Add("_Temperature");
            RCDB.Items.Add("------------------------");
            RCDB.Items.Add("_shot");
            RCDB.Items.Add("_rec_start");
            RCDB.Items.Add("_rec_stop");
            RCDB.Items.Add("_cmd");
            RCDB.Items.Add("_DOS");
            RCDB.Items.Add("------------------------");
            RCDB.Items.Add("_IO_Output");
            RCDB.Items.Add("_IO_Input");
            RCDB.Items.Add("_audio_debounce");
            RCDB.Items.Add("_Pin");
            RCDB.Items.Add("_keyword");
            RCDB.Items.Add("------------------------");
            RCDB.Items.Add("_quantum");
            RCDB.Items.Add("_astro");
            RCDB.Items.Add("_dektec");
            RCDB.Items.Add("------------------------");
        }
        #endregion

        void DataGridView1_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            ComboBox cmb = e.Control as ComboBox;
            if (cmb != null)
            {
                cmb.DropDown -= new EventHandler(cmb_DropDown);
                cmb.DropDown += new EventHandler(cmb_DropDown);
            }
        }

        //自動調整ComboBox寬度//
        void cmb_DropDown(object sender, EventArgs e)
        {
            ComboBox cmb = sender as ComboBox;
            int width = cmb.DropDownWidth;
            Graphics g = cmb.CreateGraphics();
            Font font = cmb.Font;
            int vertScrollBarWidth = 0;
            if (cmb.Items.Count > cmb.MaxDropDownItems)
            {
                vertScrollBarWidth = SystemInformation.VerticalScrollBarWidth;
            }

            int maxWidth;
            foreach (string s in cmb.Items)
            {
                maxWidth = (int)g.MeasureString(s, font).Width + vertScrollBarWidth;
                if (width < maxWidth)
                {
                    width = maxWidth;
                }
            }

            DataGridViewComboBoxColumn c =
                DataGridView_Schedule.Columns[0] as DataGridViewComboBoxColumn;
            if (c != null)
            {
                c.DropDownWidth = width;
            }
        }

        private void StartBtn_Click(object sender, EventArgs e)
        {
            byte[] val = new byte[2];
            val[0] = 0;
            bool AutoBox_Status;

            Global.IO_PA10_0_COUNT = 0;
            Global.IO_PA10_1_COUNT = 0;
            Global.IO_PA11_0_COUNT = 0;
            Global.IO_PA11_1_COUNT = 0;
            Global.IO_PA14_0_COUNT = 0;
            Global.IO_PA14_1_COUNT = 0;
            Global.IO_PA15_0_COUNT = 0;
            Global.IO_PA15_1_COUNT = 0;
            Global.IO_PB1_0_COUNT = 0;
            Global.IO_PB1_1_COUNT = 0;
            Global.IO_PB7_0_COUNT = 0;
            Global.IO_PB7_1_COUNT = 0;

            AutoBox_Status = ini12.INIRead(MainSettingPath, "Device", "AutoboxExist", "") == "1" ? true : false;

            if (ini12.INIRead(MainSettingPath, "Device", "CameraExist", "") == "1")
            {
                if (!_captureInProgress)
                {
                    _captureInProgress = true;
                    OnOffCamera();
                }
            }

            Thread MainThread = new Thread(new ThreadStart(MyRunCamd));

            if (AutoBox_Status)//如果電腦有接上AutoBox//
            {
                button_Schedule1.PerformClick();
                
                if (StartButtonPressed == true)//按下STOP//
                {
                    Global.Break_Out_MyRunCamd = 1;//跳出倒數迴圈//
                    MainThread.Abort();//停止執行緒//
                    timer1.Stop();//停止倒數//
                    CloseDtplay();//關閉DtPlay//

                    StartButtonPressed = false;
                    button_Start.Enabled = false;
                    button_Setting.Enabled = false;
                    button_SaveSchedule.Enabled = false;
                    button_Pause.Enabled = true;
                    setStyle();
                    label_Command.Text = "Please wait...";
                }
                else//按下START//
                {
                    /*
                    for (int i = 1; i < 6; i++)
                    {
                        if (Directory.Exists(ini12.INIRead(sPath, "Record", "VideoPath", "") + "\\" + "Schedule" + i + "_Original") == true)
                        {
                            DirectoryInfo DIFO = new DirectoryInfo(ini12.INIRead(sPath, "Record", "VideoPath", "") + "\\" + "Schedule" + i + "_Original");
                            DIFO.Delete(true);
                        }

                        if (Directory.Exists(ini12.INIRead(sPath, "Record", "VideoPath", "") + "\\" + "Schedule" + i + "_NG") == true)
                        {
                            DirectoryInfo DIFO = new DirectoryInfo(ini12.INIRead(sPath, "Record", "VideoPath", "") + "\\" + "Schedule" + i + "_NG");
                            DIFO.Delete(true);
                        }                
                    }
                    */
                    Global.Break_Out_MyRunCamd = 0;

                    MainThread.Start();       // 啟動執行緒
                    timer1.Start();     //開始倒數
                    button_Start.Text = "STOP";

                    StartButtonPressed = true;
                    button_Setting.Enabled = false;
                    button_Pause.Enabled = true;
                    button_SaveSchedule.Enabled = false;
                    setStyle();

                    if (ini12.INIRead(MainSettingPath, "Port A", "Checked", "") == "1")
                    {
                        OpenSerialPort("A");
                    }

                    if (ini12.INIRead(MainSettingPath, "Port B", "Checked", "") == "1")
                    {
                        OpenSerialPort("B");
                    }

                    if (ini12.INIRead(MainSettingPath, "Port C", "Checked", "") == "1")
                    {
                        OpenSerialPort("C");
                    }

                    if (ini12.INIRead(MainSettingPath, "Port D", "Checked", "") == "1")
                    {
                        OpenSerialPort("D");
                    }

                    if (ini12.INIRead(MainSettingPath, "Port E", "Checked", "") == "1")
                    {
                        OpenSerialPort("E");
                    }

                    if (ini12.INIRead(MainSettingPath, "Kline", "Checked", "") == "1")
                    {
                        OpenSerialPort("kline");
                    }
                }
            }
            else//如果沒接AutoBox//
            {
                if (StartButtonPressed == true)//按下STOP//
                {
                    Global.Break_Out_MyRunCamd = 1;    //跳出倒數迴圈
                    MainThread.Abort(); //停止執行緒
                    timer1.Stop();  //停止倒數
                    CloseDtplay();

                    StartButtonPressed = false;
                    button_Start.Enabled = false;
                    button_Setting.Enabled = false;
                    button_Pause.Enabled = true;
                    button_SaveSchedule.Enabled = false;
                    setStyle();

                    label_Command.Text = "Please wait...";
                }
                else//按下START//
                {
                    Global.Break_Out_MyRunCamd = 0;
                    MainThread.Start();// 啟動執行緒
                    timer1.Start();     //開始倒數

                    StartButtonPressed = true;
                    button_Setting.Enabled = false;
                    button_Pause.Enabled = true;
                    pictureBox_AcPower.Image = Properties.Resources.OFF;
                    button_Start.Text = "STOP";
                    setStyle();

                    if (ini12.INIRead(MainSettingPath, "Port A", "Checked", "") == "1")
                    {
                        OpenSerialPort("A");
                        //textBox1.Text = string.Empty;//清空serialport1//
                    }

                    if (ini12.INIRead(MainSettingPath, "Port B", "Checked", "") == "1")
                    {
                        OpenSerialPort("B");
                    }

                    if (ini12.INIRead(MainSettingPath, "Port C", "Checked", "") == "1")
                    {
                        OpenSerialPort("C");
                    }

                    if (ini12.INIRead(MainSettingPath, "Port D", "Checked", "") == "1")
                    {
                        OpenSerialPort("D");
                    }

                    if (ini12.INIRead(MainSettingPath, "Port E", "Checked", "") == "1")
                    {
                        OpenSerialPort("E");
                    }

                    if (ini12.INIRead(MainSettingPath, "Kline", "Checked", "") == "1")
                    {
                        OpenSerialPort("kline");
                    }
                }
            }
        }

        
        private void SettingBtn_Click(object sender, EventArgs e)
        {
            FormTabControl FormTabControl = new FormTabControl();

            //如果serialport開著則先關閉//
            if (PortA.IsOpen == true)
            {
                CloseSerialPort("A");
            }
            if (PortB.IsOpen == true)
            {
                CloseSerialPort("B");
            }
            if (PortC.IsOpen == true)
            {
                CloseSerialPort("C");
            }
            if (PortD.IsOpen == true)
            {
                CloseSerialPort("D");
            }
            if (PortE.IsOpen == true)
            {
                CloseSerialPort("E");
            }
            if (MySerialPort.IsPortOpened() == true)
            {
                CloseSerialPort("kline");
            }

            //關閉SETTING以後會讀這段>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
            if (FormTabControl.ShowDialog() == DialogResult.OK)
            {
                if (ini12.INIRead(MainSettingPath, "Device", "CameraExist", "") == "1")
                {
                    pictureBox_Camera.Image = Properties.Resources.ON;
                    _captureInProgress = false;
                    OnOffCamera();
                    comboBox_CameraDevice.Enabled = false;
                }
                else
                {
                    pictureBox_Camera.Image = Properties.Resources.OFF;
                }
                List<string> SchExist = new List<string> { };
                for (int i = 2; i < 6; i++)
                {
                    SchExist.Add(ini12.INIRead(MainSettingPath, "Schedule" + i, "Exist", ""));
                }

                comboBox_savelog.Items.Clear();
                initComboboxSaveLog();

                button_Schedule2.Visible = SchExist[0] == "0" ? false : true;
                button_Schedule3.Visible = SchExist[1] == "0" ? false : true;
                button_Schedule4.Visible = SchExist[2] == "0" ? false : true;
                button_Schedule5.Visible = SchExist[3] == "0" ? false : true;
            }
            //<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<

            FormTabControl.Dispose();
            button_Schedule1.Enabled = true;
            button_Schedule1.PerformClick();

            setStyle();
        }

        //系統時間
        private void Timer1_Tick(object sender, EventArgs e)
        {

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
            FormIsClosing = true;
            if (ini12.INIRead(MainSettingPath, "Device", "AutoboxVerson", "") == "1")
            {
                DisconnectAutoBox1();
            }

            if (ini12.INIRead(MainSettingPath, "Device", "AutoboxVerson", "") == "2")
            {
                DisconnectAutoBox2();
            }

            Application.ExitThread();
            Application.Exit();
            Environment.Exit(Environment.ExitCode);
        }

        private void Button_TabScheduler_Click(object sender, EventArgs e) => DataGridView_Schedule.BringToFront();
        private void Button_TabCamera_Click(object sender, EventArgs e)
        {
            if (!_captureInProgress)
            {
                _captureInProgress = true;
                OnOffCamera();
            }
            panelVideo.BringToFront();
            comboBox_CameraDevice.Enabled = true;
            comboBox_CameraDevice.BringToFront();
        }

        #region -- 另存Schedule --
        private void WriteBtn_Click(object sender, EventArgs e)
        {
            string delimiter = ",";

            System.Windows.Forms.SaveFileDialog sfd = new System.Windows.Forms.SaveFileDialog();
            sfd.Filter = "CSV files (*.csv)|*.csv";
            sfd.FileName = ini12.INIRead(MainSettingPath, "Schedule" + Global.Schedule_Number, "Path", "");
            if (sfd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                using (System.IO.StreamWriter sw = new System.IO.StreamWriter(sfd.FileName, false))
                {
                    //output header data
                    string strHeader = "";
                    for (int i = 0; i < DataGridView_Schedule.Columns.Count; i++)
                    {
                        strHeader += DataGridView_Schedule.Columns[i].HeaderText + delimiter;
                    }
                    sw.WriteLine(strHeader.Replace("\r\n", "~"));

                    //output rows data
                    for (int j = 0; j < DataGridView_Schedule.Rows.Count - 1; j++)
                    {
                        string strRowValue = "";

                        for (int k = 0; k < DataGridView_Schedule.Columns.Count; k++)
                        {
                            strRowValue += DataGridView_Schedule.Rows[j].Cells[k].Value + delimiter;
                        }
                        sw.WriteLine(strRowValue);
                    }
                    sw.Close();
                }
            }
            ReadSch();
        }
        #endregion

        private void button_insert_a_row_Click(object sender, EventArgs e)
        {
            try
            {
                DataGridView_Schedule.Rows.Insert(DataGridView_Schedule.CurrentCell.RowIndex, new DataGridViewRow());
            }
            catch (Exception Ex)
            {
                MessageBox.Show("Please load or write a new schedule", "Schedule Error");
            }
        }

        #region -- Form1的Schedule 1~5按鈕功能 --
        private void SchBtn1_Click(object sender, EventArgs e)          ////////////Schedule1
        {
            portos_online = new SafeDataGridView();
            Global.Schedule_Number = 1;
            string loop = ini12.INIRead(MainSettingPath, "Schedule1", "Loop", "");
            if (loop != "")
                Global.Schedule_Loop = int.Parse(loop);
            labellabel_LoopTimes_Value.Text = Global.Schedule_Loop.ToString();
            button_Schedule1.Enabled = false;
            button_Schedule2.Enabled = true;
            button_Schedule3.Enabled = true;
            button_Schedule4.Enabled = true;
            button_Schedule5.Enabled = true;
            ReadSch();
        }
        private void SchBtn2_Click(object sender, EventArgs e)          ////////////Schedule2
        {
            portos_online = new SafeDataGridView();
            Global.Schedule_Number = 2;
            string loop = "";
            loop = ini12.INIRead(MainSettingPath, "Schedule2", "Loop", "");
            if (loop != "")
                Global.Schedule_Loop = int.Parse(loop);
            labellabel_LoopTimes_Value.Text = Global.Schedule_Loop.ToString();
            button_Schedule1.Enabled = true;
            button_Schedule2.Enabled = false;
            button_Schedule3.Enabled = true;
            button_Schedule4.Enabled = true;
            button_Schedule5.Enabled = true;
            LoadRCDB();
            ReadSch();
        }
        private void SchBtn3_Click(object sender, EventArgs e)          ////////////Schedule3
        {
            portos_online = new SafeDataGridView();
            Global.Schedule_Number = 3;
            string loop = ini12.INIRead(MainSettingPath, "Schedule3", "Loop", "");
            if (loop != "")
                Global.Schedule_Loop = int.Parse(loop);
            labellabel_LoopTimes_Value.Text = Global.Schedule_Loop.ToString();
            button_Schedule1.Enabled = true;
            button_Schedule2.Enabled = true;
            button_Schedule3.Enabled = false;
            button_Schedule4.Enabled = true;
            button_Schedule5.Enabled = true;
            ReadSch();
        }
        private void SchBtn4_Click(object sender, EventArgs e)          ////////////Schedule4
        {
            portos_online = new SafeDataGridView();
            Global.Schedule_Number = 4;
            string loop = ini12.INIRead(MainSettingPath, "Schedule4", "Loop", "");
            if (loop != "")
                Global.Schedule_Loop = int.Parse(loop);
            labellabel_LoopTimes_Value.Text = Global.Schedule_Loop.ToString();
            button_Schedule1.Enabled = true;
            button_Schedule2.Enabled = true;
            button_Schedule3.Enabled = true;
            button_Schedule4.Enabled = false;
            button_Schedule5.Enabled = true;
            ReadSch();
        }
        private void SchBtn5_Click(object sender, EventArgs e)          ////////////Schedule5
        {
            portos_online = new SafeDataGridView();
            Global.Schedule_Number = 5;
            string loop = ini12.INIRead(MainSettingPath, "Schedule5", "Loop", "");
            if (loop != "")
                Global.Schedule_Loop = int.Parse(loop);
            labellabel_LoopTimes_Value.Text = Global.Schedule_Loop.ToString();
            button_Schedule1.Enabled = true;
            button_Schedule2.Enabled = true;
            button_Schedule3.Enabled = true;
            button_Schedule4.Enabled = true;
            button_Schedule5.Enabled = false;
            ReadSch();
        }
        private void ReadSch()
        {
            // Console.WriteLine(Global.Schedule_Num);
            // 戴入Schedule CSV 檔
            string SchedulePath = ini12.INIRead(MainSettingPath, "Schedule" + Global.Schedule_Number, "Path", "");
            string ScheduleExist = ini12.INIRead(MainSettingPath, "Schedule" + Global.Schedule_Number, "Exist", "");

            string TextLine = "";
            string[] SplitLine;
            int i = 0;
            if ((File.Exists(SchedulePath) == true) && ScheduleExist == "1" && IsFileLocked(SchedulePath) == false)
            {
                DataGridView_Schedule.Rows.Clear();
                StreamReader objReader = new StreamReader(SchedulePath);
                while ((objReader.Peek() != -1))
                {
                    TextLine = objReader.ReadLine();
                    if (i != 0)
                    {
                        SplitLine = TextLine.Split(',');
                        DataGridView_Schedule.Rows.Add(SplitLine);
                    }
                    i++;
                }
                objReader.Close();
            }
            else if (IsFileLocked(SchedulePath))
            {
                MessageBox.Show("Please check your .csv file is closed, then press Settings to reload schedule.", "Error");
                button_Start.Enabled = false;
                button_Schedule1.PerformClick();
            }
            else
            {
                button_Start.Enabled = false;
                button_Schedule1.PerformClick();
            }



            if (TextLine != "")
            {
                int j = Int32.Parse(TextLine.Split(',').Length.ToString());

                if ((j == 11 || j == 10))
                {
                    long TotalDelay = 0;        //計算各個schedule測試時間>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
                    long RepeatTime = 0;
                    button_Start.Enabled = true;
                    for (int z = 0; z < DataGridView_Schedule.Rows.Count - 1; z++)
                    {
                        if (DataGridView_Schedule.Rows[z].Cells[8].Value.ToString() != "")
                        {
                            if (DataGridView_Schedule.Rows[z].Cells[2].Value.ToString() != "")
                            {
                                RepeatTime = (long.Parse(DataGridView_Schedule.Rows[z].Cells[1].Value.ToString())) * (long.Parse(DataGridView_Schedule.Rows[z].Cells[2].Value.ToString()));
                            }
                            TotalDelay += (long.Parse(DataGridView_Schedule.Rows[z].Cells[8].Value.ToString()) + RepeatTime);
                            RepeatTime = 0;
                        }
                    }       //$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$

                    if (ini12.INIRead(MainSettingPath, "Record", "EachVideo", "") == "1")
                    {
                        ConvertToRealTime(((TotalDelay * Global.Schedule_Loop) + 63000) / 1000);
                    }
                    else
                    {
                        ConvertToRealTime((TotalDelay * Global.Schedule_Loop) / 1000);
                    }

                    switch (Global.Schedule_Number)
                    {
                        case 1:
                            Global.Schedule_1_TestTime = (TotalDelay * Global.Schedule_Loop) / 1000;
                            timeCount = Global.Schedule_1_TestTime;
                            break;
                        case 2:
                            Global.Schedule_2_TestTime = (TotalDelay * Global.Schedule_Loop) / 1000;
                            timeCount = Global.Schedule_2_TestTime;
                            break;
                        case 3:
                            Global.Schedule_3_TestTime = (TotalDelay * Global.Schedule_Loop) / 1000;
                            timeCount = Global.Schedule_3_TestTime;
                            break;
                        case 4:
                            Global.Schedule_4_TestTime = (TotalDelay * Global.Schedule_Loop) / 1000;
                            timeCount = Global.Schedule_4_TestTime;
                            break;
                        case 5:
                            Global.Schedule_5_TestTime = (TotalDelay * Global.Schedule_Loop) / 1000;
                            timeCount = Global.Schedule_5_TestTime;
                            break;
                    }       //<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<
                }
                else
                {
                    button_Start.Enabled = false;
                    MessageBox.Show("Schedule format error! Please check your .csv file.", "File Format Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            setStyle();
        }
        #endregion

        public static bool IsFileLocked(string file)
        {
            try
            {
                using (File.Open(file, FileMode.Open, FileAccess.Write, FileShare.None))
                {
                    return false;
                }
            }
            catch (IOException exception)
            {
                var errorCode = Marshal.GetHRForException(exception) & 65535;
                return errorCode == 32 || errorCode == 33;
            }
            catch (Exception)
            {
                return false;
            }
        }

        #region -- 測試時間 --
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
                label_ScheduleTime_Value.Invoke((MethodInvoker)(() => label_ScheduleTime_Value.Text = d.ToString("0") + "d " + h.ToString("0") + "h " + s.ToString("0") + "m " + ms.ToString("0") + "s"));
                //label_ScheduleTime_Value.Text = d.ToString("0") + "d " + h.ToString("0") + "h " + s.ToString("0") + "m " + ms.ToString("0") + "s";
            }
            catch
            {
                sResult = "Error!";
            }
            return sResult;
        }
        #endregion

        #region -- UI相關 --
        /*
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
        */
        #region -- 關閉、縮小按鈕 --
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
            CloseDtplay();
            CloseAutobox();
        }

        private void MiniPicBox_Enter(object sender, EventArgs e)
        {
            MiniPicBox.Image = Properties.Resources.mini2;
        }

        private void MiniPicBox_Leave(object sender, EventArgs e)
        {
            MiniPicBox.Image = Properties.Resources.mini1;
        }

        private void MiniPicBox_Click(object sender, EventArgs e)
        {
            WindowState = FormWindowState.Minimized;
        }
        #endregion

        #region -- 滑鼠拖曳視窗 --
        private void GPanelTitleBack_MouseDown(object sender, MouseEventArgs e)
        {
            ReleaseCapture();
            SendMessage(Handle, WM_SYSCOMMAND, SC_MOVE + HTCAPTION, 0);        //調用移動無窗體控件函數
        }
        #endregion

        #endregion

        private void DataGridView1_DataError(object sender, DataGridViewDataErrorEventArgs anError)
        {
            DataGridView_Schedule.CausesValidation = false;
        }

        private void DataBtn_Click(object sender, EventArgs e)            //背景執行填入測試步驟然後匯出reprot>>>>>>>>>>>>>
        {
            //Form_DGV_Autobox.ShowDialog();
        }

        private void PauseButton_Click(object sender, EventArgs e)      //暫停SCHEDULE
        {
            Pause = !Pause;

            if (Pause == true)
            {
                button_Pause.Text = "RESUME";
                button_Start.Enabled = false;
                setStyle();
                SchedulePause.Reset();
            }
            else
            {
                button_Pause.Text = "PAUSE";
                button_Start.Enabled = true;
                setStyle();
                SchedulePause.Set();
                timer1.Start();
            }
        }

        private void Timer1_Tick_1(object sender, EventArgs e)
        {
            timer1.Interval = 1000;

            if (timeCount > 0)
            {
                label_ScheduleTime_Value.Text = (--timeCount).ToString();
                ConvertToRealTime(timeCount);
            }

            TestTime++;
            long ms, s, h, d = new int();
            ms = 0; s = 0; h = 0; d = 0;

            ms = TestTime % 60;
            if (TestTime >= 60)
            {
                s = TestTime / 60;
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
            label_TestTime_Value.Invoke((MethodInvoker)(() => label_TestTime_Value.Text = d.ToString("0") + "d " + h.ToString("0") + "h " + s.ToString("0") + "m " + ms.ToString("0") + "s"));
            //label_TestTime_Value.Text = d.ToString("0") + "d " + h.ToString("0") + "h " + s.ToString("0") + "m " + ms.ToString("0") + "s";
        }

        static List<USBDeviceInfo> GetUSBDevices()
        {
            List<USBDeviceInfo> devices = new List<USBDeviceInfo>();

            ManagementObjectCollection collection;
            using (var searcher = new ManagementObjectSearcher(@"Select * From Win32_USBHub"))
                collection = searcher.Get();

            foreach (var device in collection)
            {
                devices.Add(new USBDeviceInfo(
                (string)device.GetPropertyValue("DeviceID"),
                (string)device.GetPropertyValue("PNPDeviceID"),
                (string)device.GetPropertyValue("Description")
                ));
            }

            collection.Dispose();
            return devices;
        }

        class USBDeviceInfo
        {
            public USBDeviceInfo(string deviceID, string pnpDeviceID, string description)
            {
                DeviceID = deviceID;
                PnpDeviceID = pnpDeviceID;
                Description = description;
            }
            public string DeviceID { get; private set; }
            public string PnpDeviceID { get; private set; }
            public string Description { get; private set; }
        }

        //釋放記憶體//
        [System.Runtime.InteropServices.DllImportAttribute("kernel32.dll", EntryPoint = "SetProcessWorkingSetSize", ExactSpelling = true, CharSet = System.Runtime.InteropServices.CharSet.Ansi, SetLastError = true)]
        private static extern int SetProcessWorkingSetSize(IntPtr process, int minimumWorkingSetSize, int maximumWorkingSetSize);
        private void DisposeRam()
        {
            GC.Collect();
            GC.SuppressFinalize(this);
            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
            {
                SetProcessWorkingSetSize(Process.GetCurrentProcess().Handle, -1, -1);
            }
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            CloseAutobox();
        }

        #region -- GPIO --
        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool DeviceIoControl(SafeFileHandle hDevice,
                                                   uint dwIoControlCode,
                                                   ref uint InBuffer,
                                                   int nInBufferSize,
                                                   byte[] OutBuffer,
                                                   UInt32 nOutBufferSize,
                                                   ref UInt32 out_count,
                                                   IntPtr lpOverlapped);
        public SafeFileHandle hCOM;

        public const uint FILE_DEVICE_UNKNOWN = 0x00000022;
        public const uint USB2SER_IOCTL_INDEX = 0x0800;
        public const uint METHOD_BUFFERED = 0;
        public const uint FILE_ANY_ACCESS = 0;

        public bool PowerState;
        public bool USBState;

        public static uint GP0_SET_VALUE = CTL_CODE(FILE_DEVICE_UNKNOWN, USB2SER_IOCTL_INDEX + 22, METHOD_BUFFERED, FILE_ANY_ACCESS);
        public static uint GP1_SET_VALUE = CTL_CODE(FILE_DEVICE_UNKNOWN, USB2SER_IOCTL_INDEX + 23, METHOD_BUFFERED, FILE_ANY_ACCESS);
        public static uint GP2_SET_VALUE = CTL_CODE(FILE_DEVICE_UNKNOWN, USB2SER_IOCTL_INDEX + 47, METHOD_BUFFERED, FILE_ANY_ACCESS);
        public static uint GP3_SET_VALUE = CTL_CODE(FILE_DEVICE_UNKNOWN, USB2SER_IOCTL_INDEX + 48, METHOD_BUFFERED, FILE_ANY_ACCESS);

        public static uint GP0_GET_VALUE = CTL_CODE(FILE_DEVICE_UNKNOWN, USB2SER_IOCTL_INDEX + 24, METHOD_BUFFERED, FILE_ANY_ACCESS);
        public static uint GP1_GET_VALUE = CTL_CODE(FILE_DEVICE_UNKNOWN, USB2SER_IOCTL_INDEX + 25, METHOD_BUFFERED, FILE_ANY_ACCESS);
        public static uint GP2_GET_VALUE = CTL_CODE(FILE_DEVICE_UNKNOWN, USB2SER_IOCTL_INDEX + 49, METHOD_BUFFERED, FILE_ANY_ACCESS);
        public static uint GP3_GET_VALUE = CTL_CODE(FILE_DEVICE_UNKNOWN, USB2SER_IOCTL_INDEX + 50, METHOD_BUFFERED, FILE_ANY_ACCESS);

        public static uint GP0_OUTPUT_ENABLE = CTL_CODE(FILE_DEVICE_UNKNOWN, USB2SER_IOCTL_INDEX + 20, METHOD_BUFFERED, FILE_ANY_ACCESS);
        public static uint GP1_OUTPUT_ENABLE = CTL_CODE(FILE_DEVICE_UNKNOWN, USB2SER_IOCTL_INDEX + 21, METHOD_BUFFERED, FILE_ANY_ACCESS);
        public static uint GP2_OUTPUT_ENABLE = CTL_CODE(FILE_DEVICE_UNKNOWN, USB2SER_IOCTL_INDEX + 45, METHOD_BUFFERED, FILE_ANY_ACCESS);
        public static uint GP3_OUTPUT_ENABLE = CTL_CODE(FILE_DEVICE_UNKNOWN, USB2SER_IOCTL_INDEX + 46, METHOD_BUFFERED, FILE_ANY_ACCESS);

        static uint CTL_CODE(uint DeviceType, uint Function, uint Method, uint Access)
        {
            return ((DeviceType << 16) | (Access << 14) | (Function << 2) | Method);
        }

        #region -- GP0 --
        public bool PL2303_GP0_Enable(SafeFileHandle hDrv, uint enable)
        {
            UInt32 nBytes = 0;
            bool bSuccess = DeviceIoControl(hDrv, GP0_OUTPUT_ENABLE,
            ref enable, sizeof(byte), null, 0, ref nBytes, IntPtr.Zero);
            return bSuccess;
        }
        public bool PL2303_GP0_SetValue(SafeFileHandle hDrv, uint val)
        {
            UInt32 nBytes = 0;
            byte[] addr = new byte[6];
            bool bSuccess = DeviceIoControl(hDrv, GP0_SET_VALUE, ref val, sizeof(uint), null, 0, ref nBytes, IntPtr.Zero);
            return bSuccess;
        }
        #endregion

        #region -- GP1 --
        public bool PL2303_GP1_Enable(SafeFileHandle hDrv, uint enable)
        {
            UInt32 nBytes = 0;
            bool bSuccess = DeviceIoControl(hDrv, GP1_OUTPUT_ENABLE,
            ref enable, sizeof(byte), null, 0, ref nBytes, IntPtr.Zero);
            return bSuccess;
        }
        public bool PL2303_GP1_SetValue(SafeFileHandle hDrv, uint val)
        {
            UInt32 nBytes = 0;
            byte[] addr = new byte[6];
            bool bSuccess = DeviceIoControl(hDrv, GP1_SET_VALUE, ref val, sizeof(uint), null, 0, ref nBytes, IntPtr.Zero);
            return bSuccess;
        }
        #endregion

        #region -- GP2 --
        public bool PL2303_GP2_Enable(SafeFileHandle hDrv, uint enable)
        {
            UInt32 nBytes = 0;
            bool bSuccess = DeviceIoControl(hDrv, GP2_OUTPUT_ENABLE,
            ref enable, sizeof(byte), null, 0, ref nBytes, IntPtr.Zero);
            return bSuccess;
        }
        public bool PL2303_GP2_SetValue(SafeFileHandle hDrv, uint val)
        {
            UInt32 nBytes = 0;
            byte[] addr = new byte[6];
            bool bSuccess = DeviceIoControl(hDrv, GP2_SET_VALUE, ref val, sizeof(uint), null, 0, ref nBytes, IntPtr.Zero);
            return bSuccess;
        }
        #endregion

        #region -- GP3 --
        public bool PL2303_GP3_Enable(SafeFileHandle hDrv, uint enable)
        {
            UInt32 nBytes = 0;
            bool bSuccess = DeviceIoControl(hDrv, GP3_OUTPUT_ENABLE,
            ref enable, sizeof(byte), null, 0, ref nBytes, IntPtr.Zero);
            return bSuccess;
        }
        public bool PL2303_GP3_SetValue(SafeFileHandle hDrv, uint val)
        {
            UInt32 nBytes = 0;
            byte[] addr = new byte[6];
            bool bSuccess = DeviceIoControl(hDrv, GP3_SET_VALUE, ref val, sizeof(uint), null, 0, ref nBytes, IntPtr.Zero);
            return bSuccess;
        }
        #endregion

        private void GP0_GP1_AC_ON()
        {
            byte[] val1 = new byte[2];
            val1[0] = 0;
            uint val = (uint)int.Parse("1");
            try
            {
                bool Success_GP0_Enable = PL2303_GP0_Enable(hCOM, 1);
                bool Success_GP0_SetValue = PL2303_GP0_SetValue(hCOM, val);

                bool Success_GP1_Enable = PL2303_GP1_Enable(hCOM, 1);
                bool Success_GP1_SetValue = PL2303_GP1_SetValue(hCOM, val);
            }
            catch (Exception Ex)
            {
                MessageBox.Show("Woodpecker is already running.", "GP0_GP1_AC_ON Error");
            }
            PowerState = true;
            pictureBox_AcPower.Image = Properties.Resources.ON;
        }

        private void GP0_GP1_AC_OFF_ON()
        {
            if (StartButtonPressed == true)
            {
                // 電源開或關
                byte[] val1;
                val1 = new byte[2];
                val1[0] = 0;

                bool Success_GP0_Enable = PL2303_GP0_Enable(hCOM, 1);
                bool Success_GP1_Enable = PL2303_GP1_Enable(hCOM, 1);
                if (Success_GP0_Enable && Success_GP1_Enable && PowerState == false)
                {
                    uint val;
                    val = (uint)int.Parse("1");
                    bool Success_GP0_SetValue = PL2303_GP0_SetValue(hCOM, val);
                    bool Success_GP1_SetValue = PL2303_GP1_SetValue(hCOM, val);
                    if (Success_GP0_SetValue && Success_GP1_SetValue)
                    {
                        {
                            PowerState = true;
                            pictureBox_AcPower.Image = Properties.Resources.ON;
                        }
                    }
                }
                else if (Success_GP0_Enable && Success_GP1_Enable && PowerState == true)
                {
                    uint val;
                    val = (uint)int.Parse("0");
                    bool Success_GP0_SetValue = PL2303_GP0_SetValue(hCOM, val);
                    bool Success_GP1_SetValue = PL2303_GP1_SetValue(hCOM, val);
                    if (Success_GP0_SetValue && Success_GP1_SetValue)
                    {
                        {
                            PowerState = false;
                            pictureBox_AcPower.Image = Properties.Resources.OFF;
                        }
                    }
                }
            }
        }

        private void GP2_GP3_USB_PC()
        {
            byte[] val1 = new byte[2];
            val1[0] = 0;
            uint val = (uint)int.Parse("0");

            try
            {
                bool Success_GP2_Enable = PL2303_GP2_Enable(hCOM, 1);
                bool Success_GP2_SetValue = PL2303_GP2_SetValue(hCOM, val);

                bool Success_GP3_Enable = PL2303_GP3_Enable(hCOM, 1);
                bool Success_GP3_SetValue = PL2303_GP3_SetValue(hCOM, val);
            }
            catch (Exception Ex)
            {
                MessageBox.Show("Woodpecker is already running.", "GP2_GP3_USB_PC Error");
            }
            USBState = true;
        }

        private void IO_INPUT()
        {
            UInt32 GPIO_input_value, retry_cnt;
            bool bRet = false;
            retry_cnt = 3;
            do
            {
                String modified0 = "";
                bRet = MyBlueRat.Get_GPIO_Input(out GPIO_input_value);
                if (Convert.ToString(GPIO_input_value, 2).Length == 5)
                {
                    modified0 = "0" + Convert.ToString(GPIO_input_value, 2);
                }
                else if (Convert.ToString(GPIO_input_value, 2).Length == 4)
                {
                    modified0 = "0" + "0" + Convert.ToString(GPIO_input_value, 2);
                }
                else if (Convert.ToString(GPIO_input_value, 2).Length == 3)
                {
                    modified0 = "0" + "0" + "0" + Convert.ToString(GPIO_input_value, 2);
                }
                else if (Convert.ToString(GPIO_input_value, 2).Length == 2)
                {
                    modified0 = "0" + "0" + "0" + "0" + Convert.ToString(GPIO_input_value, 2);
                }
                else if (Convert.ToString(GPIO_input_value, 2).Length == 1)
                {
                    modified0 = "0" + "0" + "0" + "0" + "0" + Convert.ToString(GPIO_input_value, 2);
                }
                else
                {
                    modified0 = Convert.ToString(GPIO_input_value, 2);
                }

                string modified1 = modified0.Insert(1, ",");
                string modified2 = modified1.Insert(3, ",");
                string modified3 = modified2.Insert(5, ",");
                string modified4 = modified3.Insert(7, ",");
                string modified5 = modified4.Insert(9, ",");

                Global.IO_INPUT = modified5;
            }
            while ((bRet == false) && (--retry_cnt > 0));

            if (bRet)
            {
                labelGPIO_Input.Text = "GPIO_input: " + GPIO_input_value.ToString();
            }
            else
            {
                labelGPIO_Input.Text = "GPIO_input fail after retry";
            }
        }
        #endregion

        private void button_AcUsb_Click(object sender, EventArgs e)
        {
            AcUsbPanel = !AcUsbPanel;

            if (AcUsbPanel == true)
            {
                panel_AcUsb.Show();
                panel_AcUsb.BringToFront();
            }
            else
            {
                panel_AcUsb.Hide();
            }
        }

        private void pictureBox_Ac1_Click(object sender, EventArgs e)
        {
            byte[] val1 = new byte[2];
            val1[0] = 0;

            bool jSuccess = PL2303_GP0_Enable(hCOM, 1);
            if (PowerState == false) //Set GPIO Value as 1
            {
                uint val;
                val = (uint)int.Parse("1");
                bool bSuccess = PL2303_GP0_SetValue(hCOM, val);
                if (bSuccess == true)
                {
                    {
                        PowerState = true;
                        pictureBox_Ac1.Image = Properties.Resources.Switch_On_AC;
                    }
                }
            }
            else if (PowerState == true) //Set GPIO Value as 0
            {
                uint val;
                val = (uint)int.Parse("0");
                bool bSuccess = PL2303_GP0_SetValue(hCOM, val);
                if (bSuccess == true)
                {
                    {
                        PowerState = false;
                        pictureBox_Ac1.Image = Properties.Resources.Switch_Off_AC;
                    }
                }
            }
        }

        private void pictureBox_Ac2_Click(object sender, EventArgs e)
        {
            byte[] val1 = new byte[2];
            val1[0] = 0;

            bool jSuccess = PL2303_GP1_Enable(hCOM, 1);
            if (PowerState == false) //Set GPIO Value as 1
            {
                uint val;
                val = (uint)int.Parse("1");
                bool bSuccess = PL2303_GP1_SetValue(hCOM, val);
                if (bSuccess == true)
                {
                    {
                        PowerState = true;
                        pictureBox_Ac2.Image = Properties.Resources.Switch_On_AC;
                    }
                }
            }
            else if (PowerState == true) //Set GPIO Value as 0
            {
                uint val;
                val = (uint)int.Parse("0");
                bool bSuccess = PL2303_GP1_SetValue(hCOM, val);
                if (bSuccess == true)
                {
                    {
                        PowerState = false;
                        pictureBox_Ac2.Image = Properties.Resources.Switch_Off_AC;
                    }
                }
            }
        }

        private void pictureBox_Usb1_Click(object sender, EventArgs e)
        {
            byte[] val1 = new byte[2];
            val1[0] = 0;

            bool jSuccess = PL2303_GP2_Enable(hCOM, 1);
            if (USBState == true) //Set GPIO Value as 1
            {
                uint val;
                val = (uint)int.Parse("1");
                bool bSuccess = PL2303_GP2_SetValue(hCOM, val);
                if (bSuccess == true)
                {
                    {
                        USBState = false;
                        pictureBox_Usb1.Image = Properties.Resources.Switch_to_TV;
                    }
                }
            }
            else if (USBState == false) //Set GPIO Value as 0
            {
                uint val;
                val = (uint)int.Parse("0");
                bool bSuccess = PL2303_GP2_SetValue(hCOM, val);
                if (bSuccess == true)
                {
                    {
                        USBState = true;
                        pictureBox_Usb1.Image = Properties.Resources.Switch_to_PC;
                    }
                }
            }
        }

        private void pictureBox_Usb2_Click(object sender, EventArgs e)
        {
            byte[] val1 = new byte[2];
            val1[0] = 0;

            bool jSuccess = PL2303_GP3_Enable(hCOM, 1);
            if (USBState == true) //Set GPIO Value as 1
            {
                uint val;
                val = (uint)int.Parse("1");
                bool bSuccess = PL2303_GP3_SetValue(hCOM, val);
                if (bSuccess == true)
                {
                    {
                        USBState = false;
                        pictureBox_Usb2.Image = Properties.Resources.Switch_to_TV;
                    }
                }
            }
            else if (USBState == false) //Set GPIO Value as 0
            {
                uint val;
                val = (uint)int.Parse("0");
                bool bSuccess = PL2303_GP3_SetValue(hCOM, val);
                if (bSuccess == true)
                {
                    {
                        USBState = true;
                        pictureBox_Usb2.Image = Properties.Resources.Switch_to_PC;
                    }
                }
            }
        }

        private void Form1_Shown(object sender, EventArgs e)
        {
            if (ini12.INIRead(MainSettingPath, "Device", "RunAfterStartUp", "") == "1")
            {
                button_Start.PerformClick();
            }
        }

        private void DataGridView_Schedule_CellMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            FormScriptHelper formScriptHelper = new FormScriptHelper();
            formScriptHelper.Owner = this;
            

            try
            {
                if (DataGridView_Schedule.Rows[e.RowIndex].Cells[0].Value.ToString() == "_cmd" &&
                    DataGridView_Schedule.Columns[e.ColumnIndex].HeaderText == "Picture" ||
                    DataGridView_Schedule.Rows[e.RowIndex].Cells[0].Value.ToString() == "_cmd" &&
                    DataGridView_Schedule.Columns[e.ColumnIndex].HeaderText == "AC/USB Switch" ||

                    DataGridView_Schedule.Rows[e.RowIndex].Cells[0].Value.ToString() == "_ascii" &&
                    DataGridView_Schedule.Columns[e.ColumnIndex].HeaderText == ">COM  >Pin" ||
                    DataGridView_Schedule.Rows[e.RowIndex].Cells[0].Value.ToString() == "_HEX" &&
                    DataGridView_Schedule.Columns[e.ColumnIndex].HeaderText == ">COM  >Pin" ||
                    DataGridView_Schedule.Rows[e.RowIndex].Cells[0].Value.ToString() == "_ascii" &&
                    DataGridView_Schedule.Columns[e.ColumnIndex].HeaderText == "AC/USB Switch" ||
                    DataGridView_Schedule.Rows[e.RowIndex].Cells[0].Value.ToString() == "_ascii" &&
                    DataGridView_Schedule.Columns[e.ColumnIndex].HeaderText == ">SerialPort                   >I/O cmd" ||
                    DataGridView_Schedule.Rows[e.RowIndex].Cells[0].Value.ToString() == "_HEX" &&
                    DataGridView_Schedule.Columns[e.ColumnIndex].HeaderText == ">SerialPort                   >I/O cmd" ||

                    DataGridView_Schedule.Rows[e.RowIndex].Cells[0].Value.ToString() == "_Pin" &&
                    DataGridView_Schedule.Columns[e.ColumnIndex].HeaderText == ">COM  >Pin" ||
                    DataGridView_Schedule.Rows[e.RowIndex].Cells[0].Value.ToString() == "_Pin" &&
                    DataGridView_Schedule.Columns[e.ColumnIndex].HeaderText == ">SerialPort                   >I/O cmd")
                {
                    formScriptHelper.RCKeyForm1 = DataGridView_Schedule.Rows[e.RowIndex].Cells[0].Value.ToString();
                    formScriptHelper.SetValue(DataGridView_Schedule.Columns[e.ColumnIndex].HeaderText);
                    formScriptHelper.ShowDialog();

                    DataGridView_Schedule[DataGridView_Schedule.CurrentCell.ColumnIndex,
                                          DataGridView_Schedule.CurrentCell.RowIndex].Value = strValue;
                    DataGridView_Schedule.RefreshEdit();
                }

                if (DataGridView_Schedule.Rows[e.RowIndex].Cells[0].Value.ToString().Length >= 8)
                {
                    if (DataGridView_Schedule.Rows[e.RowIndex].Cells[0].Value.ToString() == "_keyword" &&
                    DataGridView_Schedule.Columns[e.ColumnIndex].HeaderText == ">Times >Keyword#" ||
                    DataGridView_Schedule.Rows[e.RowIndex].Cells[0].Value.ToString() == "_keyword" &&
                    DataGridView_Schedule.Columns[e.ColumnIndex].HeaderText == ">SerialPort                   >I/O cmd")
                    {
                        formScriptHelper.RCKeyForm1 = DataGridView_Schedule.Rows[e.RowIndex].Cells[0].Value.ToString();
                        formScriptHelper.SetValue(DataGridView_Schedule.Columns[e.ColumnIndex].HeaderText);
                        formScriptHelper.ShowDialog();

                        DataGridView_Schedule[DataGridView_Schedule.CurrentCell.ColumnIndex,
                                              DataGridView_Schedule.CurrentCell.RowIndex].Value = strValue;
                        DataGridView_Schedule.RefreshEdit();
                    }

                    if (DataGridView_Schedule.Rows[e.RowIndex].Cells[0].Value.ToString().Substring(0, 10) == "_IO_Output" &&
                    DataGridView_Schedule.Columns[e.ColumnIndex].HeaderText == ">Times >Keyword#")
                    {
                        DataGridViewTextBoxColumn targetColumn = (DataGridViewTextBoxColumn)DataGridView_Schedule.Columns[e.ColumnIndex];
                        targetColumn.MaxInputLength = 8;
                    }

                    if ((DataGridView_Schedule.Rows[e.RowIndex].Cells[0].Value.ToString().Substring(0, 10) == "_WaterTemp" || DataGridView_Schedule.Rows[e.RowIndex].Cells[0].Value.ToString().Substring(0, 12) == "_FuelDisplay") &&
                    DataGridView_Schedule.Columns[e.ColumnIndex].HeaderText == ">Times >Keyword#")
                    {
                        DataGridViewTextBoxColumn targetColumn = (DataGridViewTextBoxColumn)DataGridView_Schedule.Columns[e.ColumnIndex];
                        targetColumn.MaxInputLength = 9;
                    }
                }
                strValue = "";
            }
            catch (Exception error)
            {
                Console.WriteLine(error);
            }

        }

        private string strValue;
        public string StrValue
        {
            set
            {
                strValue = value;
            }
        }

        private void comboBox_CameraDevice_SelectedIndexChanged(object sender, EventArgs e)
        {
            ini12.INIWrite(MainSettingPath, "Camera", "VideoIndex", comboBox_CameraDevice.SelectedIndex.ToString());
            if (_captureInProgress == true)
            {
                capture.Stop();
                capture.Dispose();
                Camstart();
            }
        }

        private void DataGridView_Schedule_RowHeaderMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            Nowpoint = DataGridView_Schedule.Rows[e.RowIndex].Index;

            if (Breakfunction == true && Nowpoint != Breakpoint)
            {
                DataGridView_Schedule.Rows[Breakpoint].DefaultCellStyle.BackColor = Color.FromArgb(51, 51, 51);
                DataGridView_Schedule.Rows[Breakpoint].DefaultCellStyle.SelectionBackColor = Color.FromArgb(3, 218, 198);
                DataGridView_Schedule.Rows[Breakpoint].DefaultCellStyle.SelectionForeColor = Color.White;
                DataGridView_Schedule.Rows[Nowpoint].DefaultCellStyle.BackColor = Color.FromArgb(3, 218, 198);
                DataGridView_Schedule.Rows[Nowpoint].DefaultCellStyle.SelectionBackColor = Color.FromArgb(3, 218, 198);
                DataGridView_Schedule.Rows[Nowpoint].DefaultCellStyle.SelectionForeColor = Color.White;
                Breakpoint = Nowpoint;
                //Console.WriteLine("Change the Nowpoint");
            }
            else if (Breakfunction == true && Nowpoint == Breakpoint)
            {
                Breakfunction = false;
                DataGridView_Schedule.Rows[Breakpoint].DefaultCellStyle.BackColor = Color.FromArgb(51, 51, 51);
                DataGridView_Schedule.Rows[Breakpoint].DefaultCellStyle.SelectionBackColor = Color.FromArgb(3, 218, 198);
                DataGridView_Schedule.Rows[Breakpoint].DefaultCellStyle.SelectionForeColor = Color.White;
                DataGridView_Schedule.Rows[Nowpoint].DefaultCellStyle.SelectionBackColor = Color.FromArgb(51, 51, 51);
                DataGridView_Schedule.Rows[Nowpoint].DefaultCellStyle.SelectionForeColor = Color.White;
                Breakpoint = -1;
                //Console.WriteLine("Disable the Breakfunction");
            }
            else
            {
                Breakfunction = true;
                Breakpoint = Nowpoint;
                DataGridView_Schedule.Rows[Breakpoint].DefaultCellStyle.BackColor = Color.FromArgb(3, 218, 198);
                DataGridView_Schedule.Rows[Breakpoint].DefaultCellStyle.SelectionBackColor = Color.FromArgb(3, 218, 198);
                DataGridView_Schedule.Rows[Breakpoint].DefaultCellStyle.SelectionForeColor = Color.White;
                Console.WriteLine("Enable the Breakfunction");
            }
        }

        private void button_savelog_Click(object sender, EventArgs e)
        {
            string save_option = comboBox_savelog.Text;
            switch (save_option)
            {
                case "Port A":
                    Serialportsave("A");
                    MessageBox.Show("Port A is saved.", "Reminder");
                    break;
                case "Port B":
                    Serialportsave("B");
                    MessageBox.Show("Port B is saved.", "Reminder");
                    break;
                case "Port C":
                    Serialportsave("C");
                    MessageBox.Show("Port C is saved.", "Reminder");
                    break;
                case "Port D":
                    Serialportsave("D");
                    MessageBox.Show("Port D is saved.", "Reminder");
                    break;
                case "Port E":
                    Serialportsave("E");
                    MessageBox.Show("Port E is saved.", "Reminder");
                    break;
                case "Canbus":
                    Serialportsave("Canbus");
                    MessageBox.Show("Canbus is saved.", "Reminder");
                    break;
                case "Kline":
                    Serialportsave("KlinePort");
                    MessageBox.Show("Kline Port is saved.", "Reminder");
                    break;
                default:
                    break;
            }
        }

        unsafe private void timer_canbus_Tick(object sender, EventArgs e)
        {
            UInt32 res = new UInt32();

            res = MYCanReader.ReceiveData();

            if (res == 0)
            {
                if (res >= CAN_Reader.MAX_CAN_OBJ_ARRAY_LEN)     // Must be something wrong
                {
                    timer_canbus.Enabled = false;
                    MYCanReader.StopCAN();
                    MYCanReader.Disconnect();

                    pictureBox_canbus.Image = Properties.Resources.OFF;

                    ini12.INIWrite(MainSettingPath, "Device", "CANbusExist", "0");

                    return;
                }
                return;
            }

            uint ID = 0, DLC = 0;
            const int DATA_LEN = 8;
            byte[] DATA = new byte[DATA_LEN];

            if (ini12.INIRead(MainSettingPath, "Device", "CANbusExist", "") == "1")
            {
                String str = "";
                for (UInt32 i = 0; i < res; i++)
                {
                    DateTime.Now.ToShortTimeString();
                    DateTime dt = DateTime.Now;
                    MYCanReader.GetOneCommand(i, out str, out ID, out DLC, out DATA);
                    string canbus_log_text = "[" + dt.ToString("yyyy/MM/dd HH:mm:ss.fff") + "]  " + str + "\r\n";
                    canbus_text = string.Concat(canbus_text, canbus_log_text);
                    schedule_text = string.Concat(schedule_text, canbus_log_text);
                }
            }
        }

        private void Timer_kline_Tick(object sender, EventArgs e)
        {
            // Regularly polling request message
            while (MySerialPort.KLineBlockMessageList.Count() > 0)
            {
                // Pop 1st KLine Block Message
                BlockMessage in_message = MySerialPort.KLineBlockMessageList[0];
                MySerialPort.KLineBlockMessageList.RemoveAt(0);

                // Display debug message on RichTextBox
                String raw_data_in_string = MySerialPort.KLineRawDataInStringList[0];
                MySerialPort.KLineRawDataInStringList.RemoveAt(0);
                // Process input Kline message and generate output KLine message
                KWP_2000_Process kwp_2000_process = new KWP_2000_Process();
                BlockMessage out_message = new BlockMessage();

                //Use_Random_DTC(kwp_2000_process);  // Random Test
                //Use_Fixed_DTC_from_HQ(kwp_2000_process);  // Simulate response from a ECU device
                //Scan_DTC_from_UI(kwp_2000_process);  // Scan Checkbox status and add DTC into queue
                if (kline_send == 1)
                {
                    foreach (var dtc in ABS_error_list)
                    {
                        kwp_2000_process.ABS_DTC_Queue_Add(dtc);
                    }
                    foreach (var dtc in OBD_error_list)
                    {
                        kwp_2000_process.OBD_DTC_Queue_Add(dtc);
                    }
                }
                else
                {
                    kwp_2000_process.ABS_DTC_Queue_Clear();
                    kwp_2000_process.OBD_DTC_Queue_Clear();
                }


                // Generate output block message according to input message and DTC codes
                kwp_2000_process.ProcessMessage(in_message, ref out_message);

                // Convert output block message to List<byte> so that it can be sent via UART
                List<byte> output_data;
                out_message.GenerateSerialOutput(out output_data);

                // NOTE: because we will also receive all data sent by us, we need to tell UART to skip all data to be sent by SendToSerial
                MySerialPort.Add_ECU_Filtering_Data(output_data);
                MySerialPort.Enable_ECU_Filtering(true);
                // Send output KLine message via UART (after some delay)
                Thread.Sleep((KWP_2000_Process.min_delay_before_response - 1));
                MySerialPort.SendToSerial(output_data.ToArray());
            }
        }

        private void DisplayKLineBlockMessage(TextBox rtb, String msg)
        {
            String current_time_str = DateTime.Now.ToString("[HH:mm:ss.fff] ");
            rtb.AppendText(current_time_str + msg + "\n");
            rtb.ScrollToCaret();
        }

        private void button_Start_EnabledChanged(object sender, EventArgs e)
        {
            button_Start.FlatAppearance.BorderColor = Color.FromArgb(242, 242, 242);
            button_Start.FlatAppearance.BorderSize = 3;
            button_Start.BackColor = System.Drawing.Color.FromArgb(242, 242, 242);
        }

        private void button_Pause_EnabledChanged(object sender, EventArgs e)
        {
            button_Pause.FlatAppearance.BorderColor = Color.FromArgb(242, 242, 242);
            button_Pause.FlatAppearance.BorderSize = 3;
            button_Pause.BackColor = System.Drawing.Color.FromArgb(242, 242, 242);
        }

        private void button_Camera_EnabledChanged(object sender, EventArgs e)
        {
            button_Camera.FlatAppearance.BorderColor = Color.FromArgb(242, 242, 242);
            button_Camera.FlatAppearance.BorderSize = 3;
            button_Camera.BackColor = System.Drawing.Color.FromArgb(242, 242, 242);
        }

        private void panel_VirtualRC_Paint(object sender, PaintEventArgs e)
        {

        }
    }

    public class SafeDataGridView : DataGridView
    {
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
    }


    public class Global//全域變數//
    {
        public static string MainSettingPath = Application.StartupPath + "\\Config.ini";
        public static string MailSettingPath = Application.StartupPath + "\\Mail.ini";
        public static string RcSettingPath = Application.StartupPath + "\\RC.ini";

        public static int Scheduler_Row = 0;
        public static List<string> VID = new List<string> { };
        public static List<string> PID = new List<string> { };
        public static List<string> AutoBoxComport = new List<string> { };
        public static int Schedule_Number = 0;
        public static int Schedule_1_Exist = 0;
        public static int Schedule_2_Exist = 0;
        public static int Schedule_3_Exist = 0;
        public static int Schedule_4_Exist = 0;
        public static int Schedule_5_Exist = 0;
        public static long Schedule_1_TestTime = 0;
        public static long Schedule_2_TestTime = 0;
        public static long Schedule_3_TestTime = 0;
        public static long Schedule_4_TestTime = 0;
        public static long Schedule_5_TestTime = 0;
        public static long Total_Test_Time = 0;
        public static int Loop_Number = 0;
        public static int Total_Loop = 0;
        public static int Schedule_Loop = 999999;
        public static int Schedule_Step;
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
        public static string IO_INPUT = "";
        public static int IO_PA10_0_COUNT = 0;
        public static int IO_PA10_1_COUNT = 0;
        public static int IO_PA11_0_COUNT = 0;
        public static int IO_PA11_1_COUNT = 0;
        public static int IO_PA14_0_COUNT = 0;
        public static int IO_PA14_1_COUNT = 0;
        public static int IO_PA15_0_COUNT = 0;
        public static int IO_PA15_1_COUNT = 0;
        public static int IO_PB1_0_COUNT = 0;
        public static int IO_PB1_1_COUNT = 0;
        public static int IO_PB7_0_COUNT = 0;
        public static int IO_PB7_1_COUNT = 0;
        public static string keyword_1 = "false";
        public static string keyword_2 = "false";
        public static string keyword_3 = "false";
        public static string keyword_4 = "false";
        public static string keyword_5 = "false";
        public static string keyword_6 = "false";
        public static string keyword_7 = "false";
        public static string keyword_8 = "false";
        public static string keyword_9 = "false";
        public static string keyword_10 = "false";
        public static List<string> Rc_List = new List<string> { };
        public static int Rc_Number = 0;
        public static string Pass_Or_Fail = "";//測試結果//
        public static int Break_Out_Schedule = 0;//定時器中斷變數//
        public static int Break_Out_MyRunCamd;//是否跳出倒數迴圈，1為跳出//
        public static bool FormRC = false;
        public static int TEXTBOX_FOCUS = 0;

        //MessageBox.Show("RC Key is empty", "Error", MessageBoxButtons.OK, MessageBoxIcon.Question);//MessageBox範例
    }

    /// <summary>
    /// 日期类型转换工具
    /// </summary>
    public class TimestampHelper
    {

        /// <summary>
        /// Unix时间戳转为C#格式时间
        /// </summary>
        /// <param name="timeStamp">Unix时间戳格式,例如:1482115779, 或long类型</param>
        /// <returns>C#格式时间</returns>
        public static DateTime GetDateTime(string timeStamp)
        {
            DateTime dtStart = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
            long lTime = long.Parse(timeStamp + "0000000");
            TimeSpan toNow = new TimeSpan(lTime);
            return dtStart.Add(toNow);
        }

        /// <summary>
        /// 时间戳转为C#格式时间
        /// </summary>
        /// <param name="timeStamp">Unix时间戳格式</param>
        /// <returns>C#格式时间</returns>
        public static DateTime GetDateTime(long timeStamp)
        {
            DateTime time = new DateTime();
            try
            {
                DateTime dtStart = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
                long lTime = long.Parse(timeStamp + "0000000");
                TimeSpan toNow = new TimeSpan(lTime);
                time = dtStart.Add(toNow);
            }
            catch
            {
                time = DateTime.Now.AddDays(-30);
            }
            return time;
        }

        /// <summary>
        /// DateTime时间格式转换为Unix时间戳格式
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public static long ToLong(System.DateTime time)
        {
            System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1));
            return (long)(time - startTime).TotalSeconds;
        }

        /// <summary>
        /// 获取时间戳
        /// </summary>
        /// <returns></returns>
        public static string GetTimeStamp()
        {
            TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return Convert.ToInt64(ts.TotalSeconds).ToString();
        }

        public static byte[] ConvertToNtp(DateTime datetime)
        {
            ulong milliseconds = (ulong)((datetime - new DateTime(1900, 1, 1)).TotalMilliseconds);

            ulong intpart = 0, fractpart = 0;
            var ntpData = new byte[8];

            intpart = milliseconds / 1000;
            fractpart = ((milliseconds % 1000) * 0x100000000L) / 1000;

            //Debug.WriteLine("intpart:      " + intpart);
            //Debug.WriteLine("fractpart:    " + fractpart);
            //Debug.WriteLine("milliseconds: " + milliseconds);

            var temp = intpart;
            for (var i = 3; i >= 0; i--)
            {
                ntpData[i] = (byte)(temp % 256);
                temp = temp / 256;
            }

            temp = fractpart;
            for (var i = 7; i >= 4; i--)
            {
                ntpData[i] = (byte)(temp % 256);
                temp = temp / 256;
            }
            return ntpData;
        }
    }

    class TimerCustom : System.Timers.Timer
    {
        public Queue<int> queue = new Queue<int>();

        public object lockMe = new object();

        /// <summary>
        /// 为保持连贯性，默认锁住两个
        /// </summary>
        public long lockNum = 0;

        public TimerCustom()
        {
            for (int i = 0; i < short.MaxValue; i++)
            {
                queue.Enqueue(i);
            }
        }
    }
}
