using BlueRatLibrary;
using DirectX.Capture;
using jini;
using Microsoft.Win32.SafeHandles;
using RedRat.IR;
using RedRat.RedRat3;
using RedRat.RedRat3.USB;
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
        private string MailPath = Application.StartupPath + "\\Mail.ini";
        private string RcPath = Application.StartupPath + "\\RC.ini";

        private IRedRat3 redRat3 = null;
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
        //private bool excelstat = false;
        private bool VideoRecording = false;//是否正在錄影//
        private bool TimerPanel = false;
        //private bool VirtualRcPanel = false;
        private bool AcUsbPanel = false;
        private long timeCount = 0;
        private long TestTime = 0;
        private string videostring = "";
        private string srtstring = "";

        //宣告於keyword使用
        //public Queue<SerialReceivedData> data_queue;
        private Queue<byte> SearchLogQueue1 = new Queue<byte>();
        private Queue<byte> SearchLogQueue2 = new Queue<byte>();
        private Queue<byte> SearchLogQueue3 = new Queue<byte>();
        private Queue<byte> SearchLogQueue4 = new Queue<byte>();
        private Queue<byte> SearchLogQueue5 = new Queue<byte>();
        private char Keyword_SerialPort_1_temp_char;
        private byte Keyword_SerialPort_1_temp_byte;
        private char Keyword_SerialPort_2_temp_char;
        private byte Keyword_SerialPort_2_temp_byte;
        private char Keyword_SerialPort_3_temp_char;
        private byte Keyword_SerialPort_3_temp_byte;

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
        private String log1_text, log2_text, log3_text, log4_text, log5_text, canbus_text, kline_text, schedule_text;

        public Form1()
        {
            InitializeComponent();
            setStyle();
            //USB Connection//
            USBPort = new USBClass();
            USBDeviceProperties = new USBClass.DeviceProperties();
            USBPort.USBDeviceAttached += new USBClass.USBDeviceEventHandler(USBPort_USBDeviceAttached);
            USBPort.USBDeviceRemoved += new USBClass.USBDeviceEventHandler(USBPort_USBDeviceRemoved);
            USBPort.RegisterForDeviceChange(true, this);
            //USBTryBoxConnection();
            USBTryRedratConnection();
            USBTryCameraConnection();
            //MyUSBBoxDeviceConnected = false;
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
            //USBTryBoxConnection();
            USBTryRedratConnection();
            USBTryCameraConnection();
            //MyUSBBoxDeviceConnected = false;
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
            List<Button> buttonsList = new List<Button> { button_Start, button_Setting, button_Pause, button_Schedule, button_Camera, button_SerialPort, button_AcUsb,
                                                            button_VirtualRC, button_InsertRow, button_SaveSchedule, button_Copy, button_Schedule1, button_Schedule2, button_Schedule3,
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

            //Datagridview design
            DataGridView_Schedule.Rows[Global.Scheduler_Row].DefaultCellStyle.BackColor = Color.FromArgb(56, 56, 56);
            DataGridView_Schedule.Rows[Global.Scheduler_Row].DefaultCellStyle.ForeColor = Color.FromArgb(255, 255, 255);
            DataGridView_Schedule.Columns[0].DefaultCellStyle.BackColor = Color.FromArgb(56, 56, 56);
            DataGridView_Schedule.Columns[0].DefaultCellStyle.ForeColor = Color.FromArgb(255, 255, 255);
            //DataGridView_Schedule.Rows[Global.Scheduler_Row].DefaultCellStyle.SelectionBackColor = Color.FromArgb(153, 153, 153);
            
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
                if (ini12.INIRead(MainSettingPath, "Device", "AutoboxVerson", "") == "1")
                {
                    ConnectAutoBox1();
                }

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

            if (ini12.INIRead(MainSettingPath, "SerialPortA", "PortName", "") == "")
            {
                string[] DefaultCom = System.IO.Ports.SerialPort.GetPortNames();
                ini12.INIWrite(MainSettingPath, "SerialPortA", "PortName", DefaultCom.Last());
            }

            if (ini12.INIRead(MainSettingPath, "Device", "RedRatExist", "") == "1")
            {
                OpenRedRat3();
            }
            else
            {
                pictureBox_RedRat.Image = Properties.Resources.OFF;
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

            /* Hidden serial port.
            if (ini12.INIRead(MainSettingPath, "SerialPortA", "Checked", "") == "1")
            {
                button_SerialPort1.Visible = true;
                // this.myDelegate1 = new AddDataDelegate(AddDataMethod1);
            }
            else
            {
                ini12.INIWrite(MainSettingPath, "SerialPortA", "Checked", "0");
                button_SerialPort1.Visible = false;
            }

            if (ini12.INIRead(MainSettingPath, "SerialPortB", "Checked", "") == "1")
            {
                button_SerialPort2.Visible = true;
                // this.myDelegate2 = new AddDataDelegate(AddDataMethod2);
            }
            else
            {
                ini12.INIWrite(MainSettingPath, "SerialPortB", "Checked", "0");
                button_SerialPort2.Visible = false;
            }

            if (ini12.INIRead(MainSettingPath, "SerialPortC", "Checked", "") == "1")
            {
                button_SerialPort3.Visible = true;
                // this.myDelegate3 = new AddDataDelegate(AddDataMethod3);
            }
            else
            {
                ini12.INIWrite(MainSettingPath, "SerialPortC", "Checked", "0");
                button_SerialPort3.Visible = false;
            }

            if (ini12.INIRead(MainSettingPath, "Record", "CANbusLog", "") == "1")
            {
                button_CanbusPort.Visible = true;
            }
            else
            {
                ini12.INIWrite(MainSettingPath, "Record", "CANbusLog", "0");
                button_CanbusPort.Visible = false;
            }

            if (ini12.INIRead(MainSettingPath, "Kline", "Checked", "") == "1")
            {
                button_kline.Visible = true;
            }
            else
            {
                ini12.INIWrite(MainSettingPath, "Kline", "Checked", "0");
                button_kline.Visible = false;
            }
            */

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
        /*
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
                        return true;
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
        */
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
            /*
            if (!MyUSBBoxDeviceConnected)
            {
                Console.WriteLine("USBPort_USBDeviceAttached = " + MyUSBBoxDeviceConnected);
                if (USBTryBoxConnection())
                {
                    MyUSBBoxDeviceConnected = true;
                }
            }
            */

            if (!MyUSBRedratDeviceConnected)
            {
                if (USBTryRedratConnection())
                {
                    MyUSBRedratDeviceConnected = true;
                }
            }

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
            /*
            if (!USBClass.GetUSBDevice(uint.Parse("067B", System.Globalization.NumberStyles.AllowHexSpecifier), uint.Parse("2303", System.Globalization.NumberStyles.AllowHexSpecifier), ref USBDeviceProperties, false))
            {
                Console.WriteLine("USBPort_USBDeviceRemoved = " + MyUSBBoxDeviceConnected);
                //My Device is removed
                MyUSBBoxDeviceConnected = false;
                USBTryBoxConnection();
            }
            */

            if (!USBClass.GetUSBDevice(uint.Parse("112A", System.Globalization.NumberStyles.AllowHexSpecifier), uint.Parse("0005", System.Globalization.NumberStyles.AllowHexSpecifier), ref USBDeviceProperties, false))
            {
                //My Redrat is removed
                MyUSBRedratDeviceConnected = false;
                USBTryRedratConnection();
            }
            /*
            if (!USBClass.GetUSBDevice(uint.Parse("045E", System.Globalization.NumberStyles.AllowHexSpecifier), uint.Parse("0766", System.Globalization.NumberStyles.AllowHexSpecifier), ref USBDeviceProperties, false) ||
                !USBClass.GetUSBDevice(uint.Parse("114D", System.Globalization.NumberStyles.AllowHexSpecifier), uint.Parse("8C00", System.Globalization.NumberStyles.AllowHexSpecifier), ref USBDeviceProperties, false))
            {
                //My Camera is removed
                MyUSBCameraDeviceConnected = false;
                USBTryCameraConnection();
            }
            */
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

        private void RedratConnect()        //TO DO: Inset your connection code here
        {
            ini12.INIWrite(MainSettingPath, "Device", "RedRatExist", "1");
            pictureBox_RedRat.Image = Properties.Resources.ON;
        }

        private void RedratDisconnect()     //TO DO: Insert your disconnection code here
        {
            ini12.INIWrite(MainSettingPath, "Device", "RedRatExist", "0");
            pictureBox_RedRat.Image = Properties.Resources.OFF;
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
            //string ngFolder = "Schedule" + Global.Schedule_Num + "_NG";

            //圖片印字
            Bitmap newBitmap = CloneBitmap(e);
            newBitmap = CloneBitmap(e);
            pictureBox4.Image = newBitmap;

            if (ini12.INIRead(MainSettingPath, "Record", "CompareChoose", "") == "1")
            {
                // Create Compare folder
                string comparePath = ini12.INIRead(MainSettingPath, "Record", "ComparePath", "");
                //string ngPath = fName + "\\" + ngFolder;
                string compareFile = comparePath + "\\" + "cf-" + Global.Loop_Number + "_" + Global.caption_Num + ".png";
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
                if (Global.Loop_Number < 2)
                {

                }
                else
                {
                    Thread MyCompareThread = new Thread(new ThreadStart(MyCompareCamd));
                    MyCompareThread.Start();
                }
            }

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

        #region -- 圖片比對 --
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
        #endregion

        protected void OpenRedRat3()
        {
            int dev = 0;
            string intdev = ini12.INIRead(MainSettingPath, "RedRat", "RedRatIndex", ""); ;

            if (intdev != "-1")
                dev = int.Parse(intdev);

            var devices = RedRat3USBImpl.FindDevices();

            // 假若設定值大於目前device個數，直接更改為目前device個數
            if (dev >= devices.Count)
                dev = devices.Count - 1;

            if (devices.Count > 0)
            {
                //RedRat已連線
                redRat3 = (IRedRat3)devices[dev].GetRedRat();

                //pictureBox1綠燈
                pictureBox_RedRat.Image = Properties.Resources.ON;
            }
            else
                pictureBox_RedRat.Image = Properties.Resources.OFF;
        }

        private void ConnectAutoBox1()
        {   // RS232 Setting
            serialPortWood.StopBits = System.IO.Ports.StopBits.One;
            serialPortWood.PortName = ini12.INIRead(MainSettingPath, "Device", "AutoboxPort", "");
            //serialPort3.BaudRate = int.Parse(ini12.INIRead(sPath, "SerialPort", "Baudrate", ""));
            if (serialPortWood.IsOpen == false)
            {
                serialPortWood.Open();
                object stream = typeof(SerialPort).GetField("internalSerialStream", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(serialPortWood);
                hCOM = (SafeFileHandle)stream.GetType().GetField("_handle", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(stream);
            }
            else
            {
                Console.WriteLine(DateTime.Now.ToString("h:mm:ss tt") + " - Cannot connect to AutoBox.\n");
            }
        }

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
                UpdateRCFunctionButtonAfterConnection();
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

        public void Autocommand_RedRat(string Caller, string SigData)
        {
            string redcon = "";

            //讀取設備//
            if (Caller == "Form1")
            {
                RedRatData.RedRatLoadSignalDB(ini12.INIRead(MainSettingPath, "RedRat", "DBFile", ""));
                redcon = ini12.INIRead(MainSettingPath, "RedRat", "Brands", "");
            }
            else if (Caller == "FormRc")
            {
                string SelectRcLastTimePath = ini12.INIRead(RcPath, "Setting", "SelectRcLastTimePath", "");
                RedRatData.RedRatLoadSignalDB(ini12.INIRead(SelectRcLastTimePath, "Info", "DBFile", ""));
                redcon = ini12.INIRead(SelectRcLastTimePath, "Info", "Brands", "");
            }

            try
            {
                if (RedRatData.SignalDB.GetIRPacket(redcon, SigData).ToString() == "RedRat.IR.DoubleSignal")
                {
                    DoubleSignal sig = (DoubleSignal)RedRatData.SignalDB.GetIRPacket(redcon, SigData);
                    if (redRat3 != null)
                        redRat3.OutputModulatedSignal(sig);
                }
                else
                {
                    ModulatedSignal sig2 = (ModulatedSignal)RedRatData.SignalDB.GetIRPacket(redcon, SigData);
                    if (redRat3 != null)
                        redRat3.OutputModulatedSignal(sig2);
                }
            }
            catch (Exception Ex)
            {
                Console.WriteLine(Ex);
                MessageBox.Show(Ex.Message.ToString(), "Transmit RC signal fail !");
            }
        }

        private Boolean D = false;
        public void Autocommand_BlueRat(string Caller, string SigData)
        {
            try
            {
                if (Caller == "Form1")
                {
                    RedRatData.RedRatLoadSignalDB(ini12.INIRead(MainSettingPath, "RedRat", "DBFile", ""));
                    RedRatData.RedRatSelectDevice(ini12.INIRead(MainSettingPath, "RedRat", "Brands", ""));
                }
                else if (Caller == "FormRc")
                {
                    string SelectRcLastTimePath = ini12.INIRead(RcPath, "Setting", "SelectRcLastTimePath", "");
                    RedRatData.RedRatLoadSignalDB(ini12.INIRead(SelectRcLastTimePath, "Info", "DBFile", ""));
                    RedRatData.RedRatSelectDevice(ini12.INIRead(SelectRcLastTimePath, "Info", "Brands", ""));
                }

                RedRatData.RedRatSelectRCSignal(SigData, D);

                if (RedRatData.Signal_Type_Supported != true)
                {
                    return;
                }

                // Use UART to transmit RC signal
                int rc_duration = MyBlueRat.SendOneRC(RedRatData) / 1000 + 1;
                RedRatDBViewer_Delay(rc_duration);
                /*
                int SysDelay = int.Parse(columns_wait);
                if (SysDelay <= rc_duration)
                {
                    RedRatDBViewer_Delay(rc_duration);
                }
                */
                if ((RedRatData.RedRatSelectedSignalType() == (typeof(DoubleSignal))) || (RedRatData.RC_ToggleData_Length_Value() > 0))
                {
                    RedRatData.RedRatSelectRCSignal(SigData, D);
                    D = !D;
                }
            }
            catch (Exception Ex)
            {
                Console.WriteLine(Ex);
                MessageBox.Show(Ex.Message.ToString(), "Transmit RC signal fail !");
            }
        }

        private void UpdateRCFunctionButtonAfterConnection()
        {
            if ((MyBlueRat.CheckConnection() == true))
            {
                if ((RedRatData != null) && (RedRatData.SignalDB != null) && (RedRatData.SelectedDevice != null) && (RedRatData.SelectedSignal != null))
                {
                    button_Start.Enabled = true;
                }
                else
                {
                    button_Start.Enabled = false;
                }
            }
        }
        /*
                static async System.Threading.Tasks.Task Delay(int iSecond)
                {
                    await System.Threading.Tasks.Task.Delay(iSecond);
                }

                async Task RedRatDBViewer_Delay(int delay_ms)
                {
                    try
                    {
                        await Delay(delay_ms);
                        //System.Threading.Thread.Sleep(delay_ms);
                    }
                    catch (TaskCanceledException ex)
                    {
                        Console.WriteLine(ex.ToString());
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.ToString());
                    }
                }
        */

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


        private void Log(string msg)
        {
            textBox_serial.Invoke(new EventHandler(delegate
            {
                textBox_serial.Text = msg.Trim();
                serialPortA.WriteLine(msg.Trim());
            }));
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
                        if (serialPortA.IsOpen == false)
                        {
                            string stopbit = ini12.INIRead(MainSettingPath, "SerialPortA", "StopBits", "");
                            switch (stopbit)
                            {
                                case "One":
                                    serialPortA.StopBits = System.IO.Ports.StopBits.One;
                                    break;
                                case "Two":
                                    serialPortA.StopBits = System.IO.Ports.StopBits.Two;
                                    break;
                            }
                            serialPortA.PortName = ini12.INIRead(MainSettingPath, "SerialPortA", "PortName", "");
                            serialPortA.BaudRate = int.Parse(ini12.INIRead(MainSettingPath, "SerialPortA", "BaudRate", ""));
                            serialPortA.ReadTimeout = 2000;
                            // serialPort2.Encoding = System.Text.Encoding.GetEncoding(1252);

                            serialPortA.DataReceived += new SerialDataReceivedEventHandler(SerialPort1_DataReceived);       // DataReceived呼叫函式
                            serialPortA.Open();
                            object stream = typeof(SerialPort).GetField("internalSerialStream", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(serialPortA);
                        }
                    }
                    catch (Exception Ex)
                    {
                        MessageBox.Show(Ex.Message.ToString(), "SerialPortA Error");
                    }
                    break;
                case "B":
                    try
                    {
                        if (serialPortB.IsOpen == false)
                        {
                            string stopbit = ini12.INIRead(MainSettingPath, "SerialPortB", "StopBits", "");
                            switch (stopbit)
                            {
                                case "One":
                                    serialPortB.StopBits = System.IO.Ports.StopBits.One;
                                    break;
                                case "Two":
                                    serialPortB.StopBits = System.IO.Ports.StopBits.Two;
                                    break;
                            }
                            serialPortB.PortName = ini12.INIRead(MainSettingPath, "SerialPortB", "PortName", "");
                            serialPortB.BaudRate = int.Parse(ini12.INIRead(MainSettingPath, "SerialPortB", "BaudRate", ""));
                            serialPortB.ReadTimeout = 2000;
                            // serialPort2.Encoding = System.Text.Encoding.GetEncoding(1252);

                            serialPortB.DataReceived += new SerialDataReceivedEventHandler(SerialPort2_DataReceived);       // DataReceived呼叫函式
                            serialPortB.Open();
                            object stream = typeof(SerialPort).GetField("internalSerialStream", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(serialPortB);
                        }
                    }
                    catch (Exception Ex)
                    {
                        MessageBox.Show(Ex.Message.ToString(), "SerialPortB Error");
                    }
                    break;
                case "C":
                    try
                    {
                        if (serialPortC.IsOpen == false)
                        {
                            string stopbit = ini12.INIRead(MainSettingPath, "SerialPortC", "StopBits", "");
                            switch (stopbit)
                            {
                                case "One":
                                    serialPortC.StopBits = System.IO.Ports.StopBits.One;
                                    break;
                                case "Two":
                                    serialPortC.StopBits = System.IO.Ports.StopBits.Two;
                                    break;
                            }
                            serialPortC.PortName = ini12.INIRead(MainSettingPath, "SerialPortC", "PortName", "");
                            serialPortC.BaudRate = int.Parse(ini12.INIRead(MainSettingPath, "SerialPortC", "BaudRate", ""));
                            serialPortC.ReadTimeout = 2000;
                            // serialPort3.Encoding = System.Text.Encoding.GetEncoding(1252);

                            serialPortC.DataReceived += new SerialDataReceivedEventHandler(SerialPort3_DataReceived);       // DataReceived呼叫函式
                            serialPortC.Open();
                            object stream = typeof(SerialPort).GetField("internalSerialStream", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(serialPortC);
                        }
                    }
                    catch (Exception Ex)
                    {
                        MessageBox.Show(Ex.Message.ToString(), "SerialPortC Error");
                    }
                    break;
                case "D":
                    try
                    {
                        if (serialPortD.IsOpen == false)
                        {
                            string stopbit = ini12.INIRead(MainSettingPath, "SerialPortD", "StopBits", "");
                            switch (stopbit)
                            {
                                case "One":
                                    serialPortD.StopBits = System.IO.Ports.StopBits.One;
                                    break;
                                case "Two":
                                    serialPortD.StopBits = System.IO.Ports.StopBits.Two;
                                    break;
                            }
                            serialPortD.PortName = ini12.INIRead(MainSettingPath, "SerialPortD", "PortName", "");
                            serialPortD.BaudRate = int.Parse(ini12.INIRead(MainSettingPath, "SerialPortD", "BaudRate", ""));
                            serialPortD.ReadTimeout = 2000;
                            // serialPort3.Encoding = System.Text.Encoding.GetEncoding(1252);

                            serialPortD.DataReceived += new SerialDataReceivedEventHandler(SerialPort4_DataReceived);       // DataReceived呼叫函式
                            serialPortD.Open();
                            object stream = typeof(SerialPort).GetField("internalSerialStream", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(serialPortC);
                        }
                    }
                    catch (Exception Ex)
                    {
                        MessageBox.Show(Ex.Message.ToString(), "SerialPortD Error");
                    }
                    break;
                case "E":
                    try
                    {
                        if (serialPortE.IsOpen == false)
                        {
                            string stopbit = ini12.INIRead(MainSettingPath, "SerialPortE", "StopBits", "");
                            switch (stopbit)
                            {
                                case "One":
                                    serialPortE.StopBits = System.IO.Ports.StopBits.One;
                                    break;
                                case "Two":
                                    serialPortE.StopBits = System.IO.Ports.StopBits.Two;
                                    break;
                            }
                            serialPortE.PortName = ini12.INIRead(MainSettingPath, "SerialPortE", "PortName", "");
                            serialPortE.BaudRate = int.Parse(ini12.INIRead(MainSettingPath, "SerialPortE", "BaudRate", ""));
                            serialPortE.ReadTimeout = 2000;
                            // serialPort3.Encoding = System.Text.Encoding.GetEncoding(1252);

                            serialPortE.DataReceived += new SerialDataReceivedEventHandler(SerialPort4_DataReceived);       // DataReceived呼叫函式
                            serialPortE.Open();
                            object stream = typeof(SerialPort).GetField("internalSerialStream", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(serialPortC);
                        }
                    }
                    catch (Exception Ex)
                    {
                        MessageBox.Show(Ex.Message.ToString(), "SerialPortE Error");
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
                    serialPortA.Dispose();
                    serialPortA.Close();
                    break;
                case "B":
                    serialPortB.Dispose();
                    serialPortB.Close();
                    break;
                case "C":
                    serialPortC.Dispose();
                    serialPortC.Close();
                    break;
                case "D":
                    serialPortD.Dispose();
                    serialPortD.Close();
                    break;
                case "E":
                    serialPortE.Dispose();
                    serialPortE.Close();
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

        #region -- Old SerialPort Setup --
        protected void OpenSerialPort1()
        {
            try
            {
                if (serialPortA.IsOpen == false)
                {
                    string stopbit = ini12.INIRead(MainSettingPath, "SerialPortA", "StopBits", "");
                    switch (stopbit)
                    {
                        case "One":
                            serialPortA.StopBits = System.IO.Ports.StopBits.One;
                            break;
                        case "Two":
                            serialPortA.StopBits = System.IO.Ports.StopBits.Two;
                            break;
                    }
                    serialPortA.PortName = ini12.INIRead(MainSettingPath, "SerialPortA", "PortName", "");
                    serialPortA.BaudRate = int.Parse(ini12.INIRead(MainSettingPath, "SerialPortA", "BaudRate", ""));
                    serialPortA.ReadTimeout = 2000;
                    // serialPort2.Encoding = System.Text.Encoding.GetEncoding(1252);

                    serialPortA.DataReceived += new SerialDataReceivedEventHandler(SerialPort1_DataReceived);       // DataReceived呼叫函式
                    serialPortA.Open();
                    object stream = typeof(SerialPort).GetField("internalSerialStream", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(serialPortA);
                }
            }
            catch (Exception Ex)
            {
                MessageBox.Show(Ex.Message.ToString(), "SerialPort2 Error");
            }
        }
/*
        protected SerialPortDataContainer OpenSerialPort1(SerialPort sp)
        {
            SerialPortDataContainer sp_data = new SerialPortDataContainer();
            sp_data.serial_port = sp;
            SerialPortDataContainer.SerialPortDictionary.Add(sp.PortName, sp_data);
            try
            {
                if (serialPort1.IsOpen == false)
                {
                    string stopbit = ini12.INIRead(MainSettingPath, "SerialPortA", "StopBits", "");
                    switch (stopbit)
                    {
                        case "One":
                            serialPort1.StopBits = StopBits.One;
                            break;
                        case "Two":
                            serialPort1.StopBits = StopBits.Two;
                            break;
                    }
                    serialPort1.PortName = ini12.INIRead(MainSettingPath, "SerialPortA", "PortName", "");
                    serialPort1.BaudRate = int.Parse(ini12.INIRead(MainSettingPath, "SerialPortA", "BaudRate", ""));
                    serialPort1.DataBits = 8;
                    serialPort1.Parity = (Parity)0;
                    serialPort1.ReceivedBytesThreshold = 1;
                    serialPort1.ReadTimeout = 2000;
                    // serialPort1.Encoding = System.Text.Encoding.GetEncoding(1252);

                    serialPort1.DataReceived += new SerialDataReceivedEventHandler(SerialPort1_DataReceived);       // DataReceived呼叫函式
                    serialPort1.Open();
                }
            }
            catch (Exception Ex)
            {
                MessageBox.Show(Ex.Message.ToString(), "SerialPort1 Error");
            }
            return sp_data;
        }
*/
        protected void CloseSerialPort1()
        {
            serialPortA.Dispose();
            serialPortA.Close();
        }
        #endregion

        #region -- 接受SerialPort1資料 --
        /*
                public class SerialReceivedData
                {
                    private List<Byte> data;
                    private DateTime time_stamp;
                    public void SetData(List<Byte> d) { data = d; }
                    public void SetTimeStamp(DateTime t) { time_stamp = t; }
                    public List<Byte> GetData() { return data; }
                    public DateTime GetTimeStamp() { return time_stamp; }
                }

                public class SerialPortDataContainer
                {
                    static public Dictionary<string, Object> SerialPortDictionary;
                    static public bool data_available;
                    public SerialPort serial_port;
                    public Queue<SerialReceivedData> data_queue;
                    //public List<SerialReceivedData> received_data = new List<SerialReceivedData>(); // just-received and to be processed
                    public Queue<Byte> log_data; // processed and stored for log_save
                    public SerialPortDataContainer()
                    {
                        SerialPortDictionary = new Dictionary<string, Object>();
                        data_queue = new Queue<SerialReceivedData>();
                        log_data = new Queue<Byte>();
                        data_available = false;
                    }
                }
                byte[] dataset1 = new byte[0];
                byte[] dataset2 = new byte[0];
                byte[] dataset3 = new byte[0];
                public void AddDataMethod1(String myString)
                {
                    DateTime dt;
                    if (ini12.INIRead(MainSettingPath, "Displayhex", "Checked", "") == "1")
                    {
                        // hex to string
                        string hexValues = BitConverter.ToString(dataset1).Replace("-", "");
                        dt = DateTime.Now;

                        // Joseph
                        hexValues = hexValues.Replace(Environment.NewLine, "\r\n" + "[" + dt.ToString("yyyy/MM/dd HH:mm:ss") + "]  "); //OK
                                                                                                                                       // hexValues = String.Concat("[" + dt.ToString("yyyy/MM/dd HH:mm:ss") + "]  " + hexValues + "\r\n");
                        textBox1.AppendText(hexValues);
                        // End

                        // Jeremy
                        // textBox1.AppendText("[" + dt.ToString("yyyy/MM/dd HH:mm:ss") + "]  ");
                        // textBox1.AppendText(hexValues + "\r\n");
                        // End
                    }
                    else
                    {
                        // string text = String.Concat(Encoding.ASCII.GetString(dataset).Where(c => c != 0x00));
                        string text = Encoding.ASCII.GetString(dataset1);

                        dt = DateTime.Now;
                        text = text.Replace(Environment.NewLine, "\r\n" + "[" + dt.ToString("yyyy/MM/dd HH:mm:ss") + "]  "); //OK
                        textBox1.AppendText(text);
                    }
                    Thread.Sleep(1);
                }

                public SerialPortDataContainer SerialPortA = new SerialPortDataContainer();
        */

        private void SerialPort1_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                int data_to_read = serialPortA.BytesToRead;
                if (data_to_read > 0)
                {
                    byte[] dataset = new byte[data_to_read];

                    serialPortA.Read(dataset, 0, data_to_read);
                    int index = 0;
                    while (data_to_read > 0)
                    {
                        SearchLogQueue1.Enqueue(dataset[index]);
                        index++;
                        data_to_read--;
                    }
                    
                    // string s = "";
                    // textBox1.Invoke(this.myDelegate1, new Object[] { s });

                    DateTime dt;
                    if (ini12.INIRead(MainSettingPath, "Displayhex", "Checked", "") == "1")
                    {
                        // hex to string
                        string hexValues = BitConverter.ToString(dataset).Replace("-", "");
                        dt = DateTime.Now;

                        // Joseph
                        hexValues = hexValues.Replace(Environment.NewLine, "\r\n" + "[" + dt.ToString("yyyy/MM/dd HH:mm:ss") + "]  "); //OK
                        // hexValues = String.Concat("[" + dt.ToString("yyyy/MM/dd HH:mm:ss") + "]  " + hexValues + "\r\n");
                        log1_text = string.Concat(log1_text, hexValues);
                        // textBox1.AppendText(hexValues);
                        // End

                        // Jeremy
                        // textBox1.AppendText("[" + dt.ToString("yyyy/MM/dd HH:mm:ss") + "]  ");
                        // textBox1.AppendText(hexValues + "\r\n");
                        // End
                    }
                    else
                    {
                        // string text = String.Concat(Encoding.ASCII.GetString(dataset).Where(c => c != 0x00));
                        string strValues = Encoding.ASCII.GetString(dataset);

                        dt = DateTime.Now;
                        strValues = strValues.Replace(Environment.NewLine, "\r\n" + "[" + dt.ToString("yyyy/MM/dd HH:mm:ss") + "]  "); //OK
                        log1_text = string.Concat(log1_text, strValues);
                        // textBox1.AppendText(strValues);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        //Jeremy code
        /*
        private void SerialPort1_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            int data = 500;
            Object serial_data_obj;
            SerialPort sp = (SerialPort)sender;
            SerialPortDataContainer.SerialPortDictionary.TryGetValue(sp.PortName, out serial_data_obj);
            SerialPortDataContainer serial_port_data = (SerialPortDataContainer)serial_data_obj;

            if (serial_port_data == null)
                return;

            try
            {
                int data_to_read = serialPort1.BytesToRead;
                if (data_to_read > 0)
                {
                    //if (data_to_read > data)
                    //    data_to_read = data;
                    Byte[] dataset = new byte[data_to_read];
                    serialPort1.Read(dataset, 0, data_to_read);
                    List<Byte> data_list = dataset.ToList();

                    SerialReceivedData enqueue_data = new SerialReceivedData();
                    enqueue_data.SetData(data_list);
                    enqueue_data.SetTimeStamp(DateTime.Now);
                    serial_port_data.data_queue.Enqueue(enqueue_data);
                    SerialPortDataContainer.data_available = true;

                    Thread DataThread = new Thread(new ThreadStart(test));
                    DataThread.Start();
                    //SerialPortTxtbox1(Encoding.ASCII.GetString(dataset1), textBox1);
                    //textBoxBuffer.Put(Encoding.ASCII.GetString(dataset1));
                    //string s = "";
                    //textBox1.Invoke(this.myDelegate1, new Object[] { s });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        void Th1()
        {
                SerialPortTxtbox1(Encoding.ASCII.GetString(dataset1), textBox1);
                Thread.Sleep(1);
        }

        //委派 txtbox1
        private delegate void UpdateUISerialPort1(string value, Control ctrl);
        private void SerialPortTxtbox1(string value, Control ctrl)
        {
            if (ctrl == null || value == null) return;
            if (ctrl.InvokeRequired)
            {
                UpdateUISerialPort1 uu = new UpdateUISerialPort1(SerialPortTxtbox1);
                this.Invoke(uu, value, ctrl);
            }
            else
            {
                if (ctrl == textBox1)
                {
                    DateTime dt;
                    dt = DateTime.Now;
                    value = value.Replace(Environment.NewLine, "\r\n" + "[" + dt.ToString("yyyy/MM/dd HH:mm:ss.fff") + "]  "); //OK
                    textBox1.AppendText(value);
                }
            }
        }


        bool test_is_running = false;

        private void test()
        {
            while (test_is_running == true) { Thread.Sleep(1); }
            
            //while (true)
            {
                test_is_running = true;
                if (SerialPortDataContainer.data_available == true)
                {
                    SerialPortDataContainer.data_available = false;
                    foreach (var port in SerialPortDataContainer.SerialPortDictionary)
                    {
                        SerialPortDataContainer serial_port_data = (SerialPortDataContainer)port.Value;
                        while (serial_port_data.data_queue.Count > 0)
                        {
                            SerialReceivedData dequeue_data = serial_port_data.data_queue.Dequeue();
                            Byte[] dataset = dequeue_data.GetData().ToArray();
                            DateTime dt = dequeue_data.GetTimeStamp();

                            // The following code is almost the same as before

                            int index = 0;
                            int data_to_read = dequeue_data.GetData().Count;
                            while (data_to_read > 0)
                            {
                                serial_port_data.log_data.Enqueue(dataset[index]);
                                index++;
                                data_to_read--;
                            }
                            
                            if (ini12.INIRead(MainSettingPath, "Displayhex", "Checked", "") == "1")
                            {
                                // hex to string
                                string hexValues = BitConverter.ToString(dataset).Replace("-", "");
                                //DateTime.Now.ToShortTimeString();
                                //dt = DateTime.Now;

                                // Joseph
                                hexValues = hexValues.Replace(Environment.NewLine, "\r\n" + "[" + dt.ToString("yyyy/MM/dd HH:mm:ss") + "]  "); //OK
                                // hexValues = String.Concat("[" + dt.ToString("yyyy/MM/dd HH:mm:ss") + "]  " + hexValues + "\r\n");
                                log_text = string.Concat(log_text, hexValues);
                                // textBox1.AppendText(hexValues);
                                // End

                                // Jeremy
                                // textBox1.AppendText("[" + dt.ToString("yyyy/MM/dd HH:mm:ss") + "]  ");
                                // textBox1.AppendText(hexValues + "\r\n");
                                // End
                            }
                            else
                            {
                                // string text = String.Concat(Encoding.ASCII.GetString(dataset).Where(c => c != 0x00));
                                string strValues = Encoding.ASCII.GetString(dataset);
                                dt = DateTime.Now;
                                strValues = strValues.Replace(Environment.NewLine, "\r\n" + "[" + dt.ToString("yyyy/MM/dd HH:mm:ss") + "]  "); //OK
                                log_text = string.Concat(log_text, strValues);

                                //textBox1.AppendText(text);
                            }
                            
                        }
                    }
                }
            }
            test_is_running = false;
        }
        */
        #endregion

        #region -- 接受SerialPort2資料 --
        private void SerialPort2_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                int data_to_read = serialPortB.BytesToRead;
                if (data_to_read > 0)
                {
                    byte [] dataset = new byte[data_to_read];

                    serialPortB.Read(dataset, 0, data_to_read);
                    int index = 0;
                    while (data_to_read > 0)
                    {
                        SearchLogQueue2.Enqueue(dataset[index]);
                        index++;
                        data_to_read--;
                    }

                    DateTime dt;
                    if (ini12.INIRead(MainSettingPath, "Displayhex", "Checked", "") == "1")
                    {
                        // hex to string
                        string hexValues = BitConverter.ToString(dataset).Replace("-", "");
                        DateTime.Now.ToShortTimeString();
                        dt = DateTime.Now;

                        // Joseph
                        hexValues = hexValues.Replace(Environment.NewLine, "\r\n" + "[" + dt.ToString("yyyy/MM/dd HH:mm:ss") + "]  "); //OK
                        log2_text = string.Concat(log2_text, hexValues);
                        // textBox2.AppendText(hexValues);
                        // End

                        // Jeremy
                        // textBox2.AppendText("[" + dt.ToString("yyyy/MM/dd HH:mm:ss") + "]  ");
                        // textBox2.AppendText(hexValues + "\r\n");
                        // End
                    }
                    else
                    {
                        // string text = String.Concat(Encoding.ASCII.GetString(dataset).Where(c => c != 0x00));
                        string strValues = Encoding.ASCII.GetString(dataset);
                        dt = DateTime.Now;
                        strValues = strValues.Replace(Environment.NewLine, "\r\n" + "[" + dt.ToString("yyyy/MM/dd HH:mm:ss") + "]  "); //OK
                        log2_text = string.Concat(log2_text, strValues);
                        //textBox2.AppendText(strValues);
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
                int data_to_read = serialPortC.BytesToRead;
                if (data_to_read > 0)
                {
                    byte [] dataset = new byte[data_to_read];

                    serialPortC.Read(dataset, 0, data_to_read);
                    int index = 0;
                    while (data_to_read > 0)
                    {
                        SearchLogQueue3.Enqueue(dataset[index]);
                        index++;
                        data_to_read--;
                    }

                    DateTime dt;
                    if (ini12.INIRead(MainSettingPath, "Displayhex", "Checked", "") == "1")
                    {
                        // hex to string
                        string hexValues = BitConverter.ToString(dataset).Replace("-", "");
                        DateTime.Now.ToShortTimeString();
                        dt = DateTime.Now;

                        // Joseph
                        hexValues = hexValues.Replace(Environment.NewLine, "\r\n" + "[" + dt.ToString("yyyy/MM/dd HH:mm:ss") + "]  "); //OK
                        log3_text = string.Concat(log3_text, hexValues);
                        // textBox3.AppendText(hexValues);
                        // End

                        // Jeremy
                        // textBox3.AppendText("[" + dt.ToString("yyyy/MM/dd HH:mm:ss") + "]  ");
                        // textBox3.AppendText(hexValues + "\r\n");
                        // End
                    }
                    else
                    {
                        // string text = String.Concat(Encoding.ASCII.GetString(dataset).Where(c => c != 0x00));
                        string strValues = Encoding.ASCII.GetString(dataset);
                        dt = DateTime.Now;
                        strValues = strValues.Replace(Environment.NewLine, "\r\n" + "[" + dt.ToString("yyyy/MM/dd HH:mm:ss") + "]  "); //OK
                        log3_text = string.Concat(log3_text, strValues);
                        //textBox3.AppendText(strValues);
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
                int data_to_read = serialPortD.BytesToRead;
                if (data_to_read > 0)
                {
                    byte[] dataset = new byte[data_to_read];

                    serialPortD.Read(dataset, 0, data_to_read);
                    int index = 0;
                    while (data_to_read > 0)
                    {
                        SearchLogQueue4.Enqueue(dataset[index]);
                        index++;
                        data_to_read--;
                    }

                    DateTime dt;
                    if (ini12.INIRead(MainSettingPath, "Displayhex", "Checked", "") == "1")
                    {
                        // hex to string
                        string hexValues = BitConverter.ToString(dataset).Replace("-", "");
                        DateTime.Now.ToShortTimeString();
                        dt = DateTime.Now;

                        // Joseph
                        hexValues = hexValues.Replace(Environment.NewLine, "\r\n" + "[" + dt.ToString("yyyy/MM/dd HH:mm:ss") + "]  "); //OK
                        log4_text = string.Concat(log4_text, hexValues);
                        // textBox4.AppendText(hexValues);
                        // End

                        // Jeremy
                        // textBox4.AppendText("[" + dt.ToString("yyyy/MM/dd HH:mm:ss") + "]  ");
                        // textBox4.AppendText(hexValues + "\r\n");
                        // End
                    }
                    else
                    {
                        // string text = String.Concat(Encoding.ASCII.GetString(dataset).Where(c => c != 0x00));
                        string strValues = Encoding.ASCII.GetString(dataset);
                        dt = DateTime.Now;
                        strValues = strValues.Replace(Environment.NewLine, "\r\n" + "[" + dt.ToString("yyyy/MM/dd HH:mm:ss") + "]  "); //OK
                        log4_text = string.Concat(log4_text, strValues);
                        //textBox4.AppendText(strValues);
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
                int data_to_read = serialPortE.BytesToRead;
                if (data_to_read > 0)
                {
                    byte[] dataset = new byte[data_to_read];

                    serialPortE.Read(dataset, 0, data_to_read);
                    int index = 0;
                    while (data_to_read > 0)
                    {
                        SearchLogQueue5.Enqueue(dataset[index]);
                        index++;
                        data_to_read--;
                    }

                    DateTime dt;
                    if (ini12.INIRead(MainSettingPath, "Displayhex", "Checked", "") == "1")
                    {
                        // hex to string
                        string hexValues = BitConverter.ToString(dataset).Replace("-", "");
                        DateTime.Now.ToShortTimeString();
                        dt = DateTime.Now;

                        // Joseph
                        hexValues = hexValues.Replace(Environment.NewLine, "\r\n" + "[" + dt.ToString("yyyy/MM/dd HH:mm:ss") + "]  "); //OK
                        log5_text = string.Concat(log5_text, hexValues);
                        // textBox5.AppendText(hexValues);
                        // End

                        // Jeremy
                        // textBox5.AppendText("[" + dt.ToString("yyyy/MM/dd HH:mm:ss") + "]  ");
                        // textBox5.AppendText(hexValues + "\r\n");
                        // End
                    }
                    else
                    {
                        // string text = String.Concat(Encoding.ASCII.GetString(dataset).Where(c => c != 0x00));
                        string strValues = Encoding.ASCII.GetString(dataset);
                        dt = DateTime.Now;
                        strValues = strValues.Replace(Environment.NewLine, "\r\n" + "[" + dt.ToString("yyyy/MM/dd HH:mm:ss") + "]  "); //OK
                        log5_text = string.Concat(log5_text, strValues);
                        //textBox5.AppendText(strValues);
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
                    Txtbox1("", textBox_serial);
                    log1_text = String.Empty;
                    MessageBox.Show("Port A saved.");
                    break;
                case "B":
                    t = fName + "\\_PortB_" + DateTime.Now.ToString("yyyyMMddHHmmss") + "_" + label_LoopNumber_Value.Text + ".txt";
                    MYFILE = new StreamWriter(t, false, Encoding.ASCII);
                    MYFILE.Write(log2_text);
                    MYFILE.Close();
                    log2_text = String.Empty;
                    MessageBox.Show("Port B saved.");
                    break;
                case "C":
                    t = fName + "\\_PortC_" + DateTime.Now.ToString("yyyyMMddHHmmss") + "_" + label_LoopNumber_Value.Text + ".txt";
                    MYFILE = new StreamWriter(t, false, Encoding.ASCII);
                    MYFILE.Write(log3_text);
                    MYFILE.Close();
                    log3_text = String.Empty;
                    MessageBox.Show("Port C saved.");
                    break;
                case "D":
                    t = fName + "\\_PortD_" + DateTime.Now.ToString("yyyyMMddHHmmss") + "_" + label_LoopNumber_Value.Text + ".txt";
                    MYFILE = new StreamWriter(t, false, Encoding.ASCII);
                    MYFILE.Write(log4_text);
                    MYFILE.Close();
                    log4_text = String.Empty;
                    MessageBox.Show("Port D saved.");
                    break;
                case "E":
                    t = fName + "\\_PortE_" + DateTime.Now.ToString("yyyyMMddHHmmss") + "_" + label_LoopNumber_Value.Text + ".txt";
                    MYFILE = new StreamWriter(t, false, Encoding.ASCII);
                    MYFILE.Write(log5_text);
                    MYFILE.Close();
                    log5_text = String.Empty;
                    MessageBox.Show("Port E saved.");
                    break;
                case "Canbus":
                    t = fName + "\\_Canbus_" + DateTime.Now.ToString("yyyyMMddHHmmss") + "_" + label_LoopNumber_Value.Text + ".txt";
                    MYFILE = new StreamWriter(t, false, Encoding.ASCII);
                    MYFILE.Write(canbus_text);
                    MYFILE.Close();
                    canbus_text = String.Empty;
                    MessageBox.Show("Canbus saved.");
                    break;
                case "KlinePort":
                    t = fName + "\\_Kline_" + DateTime.Now.ToString("yyyyMMddHHmmss") + "_" + label_LoopNumber_Value.Text + ".txt";
                    MYFILE = new StreamWriter(t, false, Encoding.ASCII);
                    MYFILE.Write(kline_text);
                    MYFILE.Close();
                    kline_text = String.Empty;
                    MessageBox.Show("Kline Port saved.");
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

        #region -- 關鍵字比對 - serialport_1 --
        private void MyLog1Camd()
        {
            string my_string = "";
            string csvFile = ini12.INIRead(MainSettingPath, "Record", "LogPath", "") + "\\Log1_keyword.csv";
            int[] compare_number = new int[10];
            bool[] send_status = new bool[10];
            int compare_paremeter = Convert.ToInt32(ini12.INIRead(MainSettingPath, "LogSearch", "TextNum", ""));

            while (StartButtonPressed == true)
            {
                while (SearchLogQueue1.Count > 0)
                {
                    Keyword_SerialPort_1_temp_byte = SearchLogQueue1.Dequeue();
                    Keyword_SerialPort_1_temp_char = (char)Keyword_SerialPort_1_temp_byte;

                    if (Convert.ToInt32(ini12.INIRead(MainSettingPath, "LogSearch", "Comport1", "")) == 1 && Convert.ToInt32(ini12.INIRead(MainSettingPath, "LogSearch", "TextNum", "")) > 0)
                    {
                        #region \n
                        if ((Keyword_SerialPort_1_temp_char == '\n'))
                        {
                            for (int i = 0; i < compare_paremeter; i++)
                            {
                                string compare_string = ini12.INIRead(MainSettingPath, "LogSearch", "Text" + i, "");
                                int compare_num = Convert.ToInt32(ini12.INIRead(MainSettingPath, "LogSearch", "Times" + i, ""));
                                string[] ewords = my_string.Split(new string[] { compare_string }, StringSplitOptions.None);
                                if (Convert.ToInt32(ewords.Length - 1) >= 1)
                                {
                                    compare_number[i] = compare_number[i] + (ewords.Length - 1);
                                    //Console.WriteLine(compare_string + ": " + compare_number[i]);
                                    if (System.IO.File.Exists(csvFile) == false)
                                    {
                                        StreamWriter sw1 = new StreamWriter(csvFile, false, Encoding.UTF8);
                                        sw1.WriteLine("Key words, Setting times, Search times, Time");
                                        sw1.Dispose();
                                    }
                                    StreamWriter sw2 = new StreamWriter(csvFile, true);
                                    sw2.Write(compare_string + ",");
                                    sw2.Write(compare_num + ",");
                                    sw2.Write(compare_number[i] + ",");
                                    sw2.WriteLine(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));
                                    sw2.Close();

                                    ////////////////////////////////////////////////////////////////////////////////////////////////MAIL//////////////////
                                    if (compare_number[i] > compare_num && send_status[i] == false)
                                    {
                                        ini12.INIWrite(MainSettingPath, "LogSearch", "Nowvalue", i.ToString());
                                        ini12.INIWrite(MainSettingPath, "LogSearch", "Display" + i, compare_number[i].ToString());
                                        if (ini12.INIRead(MailPath, "Mail Info", "From", "") != ""
                                            && ini12.INIRead(MailPath, "Mail Info", "To", "") != ""
                                            && ini12.INIRead(MainSettingPath, "LogSearch", "Sendmail", "") == "1")
                                        {
                                            FormMail FormMail = new FormMail();
                                            FormMail.logsend();
                                            send_status[i] = true;
                                        }
                                    }
                                    ////////////////////////////////////////////////////////////////////////////////////////////////AC OFF ON//////////////////
                                    if (compare_number[i] % compare_num == 0
                                        && ini12.INIRead(MainSettingPath, "Device", "AutoboxExist", "") == "1"
                                        && ini12.INIRead(MainSettingPath, "LogSearch", "ACcontrol", "") == "1")
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
                                            if (bSuccess)
                                            {
                                                {
                                                    PowerState = false;
                                                    pictureBox_AcPower.Image = Properties.Resources.OFF;
                                                }
                                            }
                                        }

                                        System.Threading.Thread.Sleep(5000);

                                        jSuccess = PL2303_GP0_Enable(hCOM, 1);
                                        if (!jSuccess)
                                        {
                                            Log("GP0 output enable FAILED.");
                                        }
                                        else
                                        {
                                            uint val;
                                            val = (uint)int.Parse("1");
                                            bool bSuccess = PL2303_GP0_SetValue(hCOM, val);
                                            if (bSuccess)
                                            {
                                                {
                                                    PowerState = true;
                                                    pictureBox_AcPower.Image = Properties.Resources.ON;
                                                }
                                            }
                                        }
                                    }
                                    ////////////////////////////////////////////////////////////////////////////////////////////////AC OFF//////////////////
                                    if (compare_number[i] % compare_num == 0
                                        && ini12.INIRead(MainSettingPath, "Device", "AutoboxExist", "") == "1"
                                        && ini12.INIRead(MainSettingPath, "LogSearch", "AC OFF", "") == "1")
                                    {
                                        byte[] val1 = new byte[2];
                                        val1[0] = 0;
                                        uint val = (uint)int.Parse("0");

                                        bool Success_GP0_Enable = PL2303_GP0_Enable(hCOM, 1);
                                        bool Success_GP0_SetValue = PL2303_GP0_SetValue(hCOM, val);

                                        bool Success_GP1_Enable = PL2303_GP1_Enable(hCOM, 1);
                                        bool Success_GP1_SetValue = PL2303_GP1_SetValue(hCOM, val);

                                        PowerState = false;

                                        pictureBox_AcPower.Image = Properties.Resources.OFF;
                                    }
                                    ////////////////////////////////////////////////////////////////////////////////////////////////SAVE LOG//////////////////
                                    if (compare_number[i] % compare_num == 0 && ini12.INIRead(MainSettingPath, "LogSearch", "Savelog", "") == "1")
                                    {
                                        string fName = "";

                                        // 讀取ini中的路徑
                                        fName = ini12.INIRead(MainSettingPath, "Record", "LogPath", "");
                                        string t = fName + "\\_SaveLog1_" + DateTime.Now.ToString("yyyyMMddHHmmss") + "_" + label_LoopNumber_Value.Text + ".txt";

                                        StreamWriter MYFILE = new StreamWriter(t, false, Encoding.ASCII);
                                        MYFILE.Write(textBox_serial.Text);
                                        MYFILE.Close();
                                        Txtbox1("", textBox_serial);
                                    }
                                    ////////////////////////////////////////////////////////////////////////////////////////////////STOP//////////////////
                                    if (compare_number[i] % compare_num == 0 && ini12.INIRead(MainSettingPath, "LogSearch", "Stop", "") == "1")
                                    {
                                        button_Start.PerformClick();
                                    }
                                    ////////////////////////////////////////////////////////////////////////////////////////////////SCHEDULE//////////////////
                                    if (compare_number[i] % compare_num == 0)
                                    {
                                        int keyword_numer = i + 1;
                                        switch (keyword_numer)
                                        {
                                            case 1:
                                                Global.keyword_1 = "true";
                                                break;

                                            case 2:
                                                Global.keyword_2 = "true";
                                                break;

                                            case 3:
                                                Global.keyword_3 = "true";
                                                break;

                                            case 4:
                                                Global.keyword_4 = "true";
                                                break;

                                            case 5:
                                                Global.keyword_5 = "true";
                                                break;

                                            case 6:
                                                Global.keyword_6 = "true";
                                                break;

                                            case 7:
                                                Global.keyword_7 = "true";
                                                break;

                                            case 8:
                                                Global.keyword_8 = "true";
                                                break;

                                            case 9:
                                                Global.keyword_9 = "true";
                                                break;

                                            case 10:
                                                Global.keyword_10 = "true";
                                                break;
                                        }
                                    }
                                }
                            }
                            //textBox1.AppendText(my_string + '\n');
                            my_string = "";
                        }
                        #endregion
                        /*
                                                #region \r
                                                else if ((Keyword_SerialPort_1_temp_char == '\r'))
                                                {
                                                    for (int i = 0; i < compare_paremeter; i++)
                                                    {
                                                        string compare_string = ini12.INIRead(MainSettingPath, "LogSearch", "Text" + i, "");
                                                        int compare_num = Convert.ToInt32(ini12.INIRead(MainSettingPath, "LogSearch", "Times" + i, ""));
                                                        string[] ewords = my_string.Split(new string[] { compare_string }, StringSplitOptions.None);

                                                        if (Convert.ToInt32(ewords.Length - 1) >= 1)
                                                        {
                                                            compare_number[i] = compare_number[i] + (ewords.Length - 1);
                                                            //Console.WriteLine(compare_string + ": " + compare_number[i]);

                                                            //////////////////////////////////////////////////////////////////////Create the compare csv file////////////////////
                                                            if (System.IO.File.Exists(csvFile) == false)
                                                            {
                                                                StreamWriter sw1 = new StreamWriter(csvFile, false, Encoding.UTF8);
                                                                sw1.WriteLine("Key words, Setting times, Search times, Time");
                                                                sw1.Dispose();
                                                            }
                                                            StreamWriter sw2 = new StreamWriter(csvFile, true);
                                                            sw2.Write(compare_string + ",");
                                                            sw2.Write(compare_num + ",");
                                                            sw2.Write(compare_number[i] + ",");
                                                            sw2.WriteLine(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));
                                                            sw2.Close();

                                                            ////////////////////////////////////////////////////////////////////////////////////////////////MAIL//////////////////
                                                            if (compare_number[i] > compare_num && send_status[i] == false)
                                                            {
                                                                ini12.INIWrite(MainSettingPath, "LogSearch", "Nowvalue", i.ToString());
                                                                ini12.INIWrite(MainSettingPath, "LogSearch", "Display" + i, compare_number[i].ToString());
                                                                if (ini12.INIRead(MailPath, "Mail Info", "From", "") != ""
                                                                    && ini12.INIRead(MailPath, "Mail Info", "To", "") != ""
                                                                    && ini12.INIRead(MainSettingPath, "LogSearch", "Sendmail", "") == "1")
                                                                {
                                                                    FormMail FormMail = new FormMail();
                                                                    FormMail.logsend();
                                                                    send_status[i] = true;
                                                                }
                                                            }
                                                            ////////////////////////////////////////////////////////////////////////////////////////////////AC OFF ON//////////////////
                                                            if (compare_number[i] % compare_num == 0
                                                                && ini12.INIRead(MainSettingPath, "Device", "AutoboxExist", "") == "1"
                                                                && ini12.INIRead(MainSettingPath, "LogSearch", "ACcontrol", "") == "1")
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
                                                                    if (bSuccess)
                                                                    {
                                                                        {
                                                                            PowerState = false;
                                                                            pictureBox_AcPower.Image = Properties.Resources.OFF;
                                                                        }
                                                                    }
                                                                }

                                                                System.Threading.Thread.Sleep(5000);

                                                                jSuccess = PL2303_GP0_Enable(hCOM, 1);
                                                                if (!jSuccess)
                                                                {
                                                                    Log("GP0 output enable FAILED.");
                                                                }
                                                                else
                                                                {
                                                                    uint val;
                                                                    val = (uint)int.Parse("1");
                                                                    bool bSuccess = PL2303_GP0_SetValue(hCOM, val);
                                                                    if (bSuccess)
                                                                    {
                                                                        {
                                                                            PowerState = true;
                                                                            pictureBox_AcPower.Image = Properties.Resources.ON;
                                                                        }
                                                                    }
                                                                }
                                                            }
                                                            ////////////////////////////////////////////////////////////////////////////////////////////////AC OFF//////////////////
                                                            if (compare_number[i] % compare_num == 0
                                                                && ini12.INIRead(MainSettingPath, "Device", "AutoboxExist", "") == "1"
                                                                && ini12.INIRead(MainSettingPath, "LogSearch", "AC OFF", "") == "1")
                                                            {
                                                                byte[] val1 = new byte[2];
                                                                val1[0] = 0;
                                                                uint val = (uint)int.Parse("0");

                                                                bool Success_GP0_Enable = PL2303_GP0_Enable(hCOM, 1);
                                                                bool Success_GP0_SetValue = PL2303_GP0_SetValue(hCOM, val);

                                                                bool Success_GP1_Enable = PL2303_GP1_Enable(hCOM, 1);
                                                                bool Success_GP1_SetValue = PL2303_GP1_SetValue(hCOM, val);

                                                                PowerState = false;

                                                                pictureBox_AcPower.Image = Properties.Resources.OFF;
                                                            }
                                                            ////////////////////////////////////////////////////////////////////////////////////////////////SAVE LOG//////////////////
                                                            if (compare_number[i] % compare_num == 0 && ini12.INIRead(MainSettingPath, "LogSearch", "Savelog", "") == "1")
                                                            {
                                                                string fName = "";

                                                                // 讀取ini中的路徑
                                                                fName = ini12.INIRead(MainSettingPath, "Record", "LogPath", "");
                                                                string t = fName + "\\_SaveLog1_" + DateTime.Now.ToString("yyyyMMddHHmmss") + "_" + label_LoopNumber_Value.Text + ".txt";

                                                                StreamWriter MYFILE = new StreamWriter(t, false, Encoding.ASCII);
                                                                MYFILE.Write(textBox1.Text);
                                                                MYFILE.Close();
                                                                Txtbox1("", textBox1);
                                                            }
                                                            ////////////////////////////////////////////////////////////////////////////////////////////////STOP//////////////////
                                                            if (compare_number[i] % compare_num == 0 && ini12.INIRead(MainSettingPath, "LogSearch", "Stop", "") == "1")
                                                            {
                                                                button_Start.PerformClick();
                                                            }
                                                            ////////////////////////////////////////////////////////////////////////////////////////////////SCHEDULE//////////////////
                                                            if (compare_number[i] % compare_num == 0)
                                                            {
                                                                int keyword_numer = i + 1;
                                                                switch (keyword_numer)
                                                                {
                                                                    case 1:
                                                                        Global.keyword_1 = "true";
                                                                        break;

                                                                    case 2:
                                                                        Global.keyword_2 = "true";
                                                                        break;

                                                                    case 3:
                                                                        Global.keyword_3 = "true";
                                                                        break;

                                                                    case 4:
                                                                        Global.keyword_4 = "true";
                                                                        break;

                                                                    case 5:
                                                                        Global.keyword_5 = "true";
                                                                        break;

                                                                    case 6:
                                                                        Global.keyword_6 = "true";
                                                                        break;

                                                                    case 7:
                                                                        Global.keyword_7 = "true";
                                                                        break;

                                                                    case 8:
                                                                        Global.keyword_8 = "true";
                                                                        break;

                                                                    case 9:
                                                                        Global.keyword_9 = "true";
                                                                        break;

                                                                    case 10:
                                                                        Global.keyword_10 = "true";
                                                                        break;
                                                                }
                                                            }
                                                        }
                                                    }
                                                    //textBox1.AppendText(my_string + '\r');
                                                    my_string = "";
                                                }
                                                #endregion
                        */
                        else
                        {
                            my_string = my_string + Keyword_SerialPort_1_temp_char;
                        }
                    }
                    else
                    {
                        if ((Keyword_SerialPort_1_temp_char == '\n'))
                        {
                            //textBox1.AppendText(my_string + '\n');
                            my_string = "";
                        }
                        else if ((Keyword_SerialPort_1_temp_char == '\r'))
                        {
                            //textBox1.AppendText(my_string + '\r');
                            my_string = "";
                        }
                        else
                        {
                            my_string = my_string + Keyword_SerialPort_1_temp_char;
                        }
                    }
                }
                Thread.Sleep(500);
            }
        }
        #endregion

        #region -- 關鍵字比對 - serialport_2 --
        private void MyLog2Camd()
        {
            string my_string = "";
            string csvFile = ini12.INIRead(MainSettingPath, "Record", "LogPath", "") + "\\Log2_keyword.csv";
            int[] compare_number = new int[10];
            bool[] send_status = new bool[10];
            int compare_paremeter = Convert.ToInt32(ini12.INIRead(MainSettingPath, "LogSearch", "TextNum", ""));

            while (StartButtonPressed == true)
            {
                while (SearchLogQueue2.Count > 0)
                {
                    Keyword_SerialPort_2_temp_byte = SearchLogQueue2.Dequeue();
                    Keyword_SerialPort_2_temp_char = (char)Keyword_SerialPort_2_temp_byte;

                    if (Convert.ToInt32(ini12.INIRead(MainSettingPath, "LogSearch", "Comport2", "")) == 1 && Convert.ToInt32(ini12.INIRead(MainSettingPath, "LogSearch", "TextNum", "")) > 0)
                    {
                        #region \n
                        if ((Keyword_SerialPort_2_temp_char == '\n'))
                        {
                            for (int i = 0; i < compare_paremeter; i++)
                            {
                                string compare_string = ini12.INIRead(MainSettingPath, "LogSearch", "Text" + i, "");
                                int compare_num = Convert.ToInt32(ini12.INIRead(MainSettingPath, "LogSearch", "Times" + i, ""));
                                string[] ewords = my_string.Split(new string[] { compare_string }, StringSplitOptions.None);
                                if (Convert.ToInt32(ewords.Length - 1) >= 1)
                                {
                                    compare_number[i] = compare_number[i] + (ewords.Length - 1);
                                    //Console.WriteLine(compare_string + ": " + compare_number[i]);
                                    if (System.IO.File.Exists(csvFile) == false)
                                    {
                                        StreamWriter sw1 = new StreamWriter(csvFile, false, Encoding.UTF8);
                                        sw1.WriteLine("Key words, Setting times, Search times, Time");
                                        sw1.Dispose();
                                    }
                                    StreamWriter sw2 = new StreamWriter(csvFile, true);
                                    sw2.Write(compare_string + ",");
                                    sw2.Write(compare_num + ",");
                                    sw2.Write(compare_number[i] + ",");
                                    sw2.WriteLine(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));
                                    sw2.Close();

                                    ////////////////////////////////////////////////////////////////////////////////////////////////MAIL//////////////////
                                    if (compare_number[i] > compare_num && send_status[i] == false)
                                    {
                                        ini12.INIWrite(MainSettingPath, "LogSearch", "Nowvalue", i.ToString());
                                        ini12.INIWrite(MainSettingPath, "LogSearch", "Display" + i, compare_number[i].ToString());
                                        if (ini12.INIRead(MailPath, "Mail Info", "From", "") != ""
                                            && ini12.INIRead(MailPath, "Mail Info", "To", "") != ""
                                            && ini12.INIRead(MainSettingPath, "LogSearch", "Sendmail", "") == "1")
                                        {
                                            FormMail FormMail = new FormMail();
                                            FormMail.logsend();
                                            send_status[i] = true;
                                        }
                                    }
                                    ////////////////////////////////////////////////////////////////////////////////////////////////AC OFF ON//////////////////
                                    if (compare_number[i] % compare_num == 0
                                        && ini12.INIRead(MainSettingPath, "Device", "AutoboxExist", "") == "1"
                                        && ini12.INIRead(MainSettingPath, "LogSearch", "ACcontrol", "") == "1")
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
                                            if (bSuccess)
                                            {
                                                {
                                                    PowerState = false;
                                                    pictureBox_AcPower.Image = Properties.Resources.OFF;
                                                }
                                            }
                                        }

                                        System.Threading.Thread.Sleep(5000);

                                        jSuccess = PL2303_GP0_Enable(hCOM, 1);
                                        if (!jSuccess)
                                        {
                                            Log("GP0 output enable FAILED.");
                                        }
                                        else
                                        {
                                            uint val;
                                            val = (uint)int.Parse("1");
                                            bool bSuccess = PL2303_GP0_SetValue(hCOM, val);
                                            if (bSuccess)
                                            {
                                                {
                                                    PowerState = true;
                                                    pictureBox_AcPower.Image = Properties.Resources.ON;
                                                }
                                            }
                                        }
                                    }
                                    ////////////////////////////////////////////////////////////////////////////////////////////////AC OFF//////////////////
                                    if (compare_number[i] % compare_num == 0
                                        && ini12.INIRead(MainSettingPath, "Device", "AutoboxExist", "") == "1"
                                        && ini12.INIRead(MainSettingPath, "LogSearch", "AC OFF", "") == "1")
                                    {
                                        byte[] val1 = new byte[2];
                                        val1[0] = 0;
                                        uint val = (uint)int.Parse("0");

                                        bool Success_GP0_Enable = PL2303_GP0_Enable(hCOM, 1);
                                        bool Success_GP0_SetValue = PL2303_GP0_SetValue(hCOM, val);

                                        bool Success_GP1_Enable = PL2303_GP1_Enable(hCOM, 1);
                                        bool Success_GP1_SetValue = PL2303_GP1_SetValue(hCOM, val);

                                        PowerState = false;

                                        pictureBox_AcPower.Image = Properties.Resources.OFF;
                                    }
                                    ////////////////////////////////////////////////////////////////////////////////////////////////SAVE LOG//////////////////
                                    if (compare_number[i] % compare_num == 0 && ini12.INIRead(MainSettingPath, "LogSearch", "Savelog", "") == "1")
                                    {
                                        string fName = "";

                                        // 讀取ini中的路徑
                                        fName = ini12.INIRead(MainSettingPath, "Record", "LogPath", "");
                                        string t = fName + "\\_SaveLog2_" + DateTime.Now.ToString("yyyyMMddHHmmss") + "_" + label_LoopNumber_Value.Text + ".txt";

                                        StreamWriter MYFILE = new StreamWriter(t, false, Encoding.ASCII);
                                        MYFILE.Write(log2_text);
                                        MYFILE.Close();
                                    }
                                    ////////////////////////////////////////////////////////////////////////////////////////////////SCHEDULE//////////////////
                                    if (compare_number[i] % compare_num == 0 && ini12.INIRead(MainSettingPath, "LogSearch", "Stop", "") == "1")
                                    {
                                        button_Start.PerformClick();
                                    }

                                    if (compare_number[i] % compare_num == 0)
                                    {
                                        int keyword_numer = i + 1;
                                        switch (keyword_numer)
                                        {
                                            case 1:
                                                Global.keyword_1 = "true";
                                                break;

                                            case 2:
                                                Global.keyword_2 = "true";
                                                break;

                                            case 3:
                                                Global.keyword_3 = "true";
                                                break;

                                            case 4:
                                                Global.keyword_4 = "true";
                                                break;

                                            case 5:
                                                Global.keyword_5 = "true";
                                                break;

                                            case 6:
                                                Global.keyword_6 = "true";
                                                break;

                                            case 7:
                                                Global.keyword_7 = "true";
                                                break;

                                            case 8:
                                                Global.keyword_8 = "true";
                                                break;

                                            case 9:
                                                Global.keyword_9 = "true";
                                                break;

                                            case 10:
                                                Global.keyword_10 = "true";
                                                break;
                                        }
                                    }
                                }
                            }
                            //textBox2.AppendText(my_string + '\n');
                            my_string = "";
                        }
                        #endregion

                        #region \r
                        else if ((Keyword_SerialPort_2_temp_char == '\r'))
                        {
                            for (int i = 0; i < compare_paremeter; i++)
                            {
                                string compare_string = ini12.INIRead(MainSettingPath, "LogSearch", "Text" + i, "");
                                int compare_num = Convert.ToInt32(ini12.INIRead(MainSettingPath, "LogSearch", "Times" + i, ""));
                                string[] ewords = my_string.Split(new string[] { compare_string }, StringSplitOptions.None);
                                if (Convert.ToInt32(ewords.Length - 1) >= 1)
                                {
                                    compare_number[i] = compare_number[i] + (ewords.Length - 1);
                                    //Console.WriteLine(compare_string + ": " + compare_number[i]);

                                    //////////////////////////////////////////////////////////////////////Create the compare csv file////////////////////
                                    if (System.IO.File.Exists(csvFile) == false)
                                    {
                                        StreamWriter sw1 = new StreamWriter(csvFile, false, Encoding.UTF8);
                                        sw1.WriteLine("Key words, Setting times, Search times, Time");
                                        sw1.Dispose();
                                    }
                                    StreamWriter sw2 = new StreamWriter(csvFile, true);
                                    sw2.Write(compare_string + ",");
                                    sw2.Write(compare_num + ",");
                                    sw2.Write(compare_number[i] + ",");
                                    sw2.WriteLine(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));
                                    sw2.Close();

                                    ////////////////////////////////////////////////////////////////////////////////////////////////MAIL//////////////////
                                    if (compare_number[i] > compare_num && send_status[i] == false)
                                    {
                                        ini12.INIWrite(MainSettingPath, "LogSearch", "Nowvalue", i.ToString());
                                        ini12.INIWrite(MainSettingPath, "LogSearch", "Display" + i, compare_number[i].ToString());
                                        if (ini12.INIRead(MailPath, "Mail Info", "From", "") != ""
                                            && ini12.INIRead(MailPath, "Mail Info", "To", "") != ""
                                            && ini12.INIRead(MainSettingPath, "LogSearch", "Sendmail", "") == "1")
                                        {
                                            FormMail FormMail = new FormMail();
                                            FormMail.logsend();
                                            send_status[i] = true;
                                        }
                                    }
                                    ////////////////////////////////////////////////////////////////////////////////////////////////AC OFF ON//////////////////
                                    if (compare_number[i] % compare_num == 0
                                        && ini12.INIRead(MainSettingPath, "Device", "AutoboxExist", "") == "1"
                                        && ini12.INIRead(MainSettingPath, "LogSearch", "ACcontrol", "") == "1")
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
                                            if (bSuccess)
                                            {
                                                {
                                                    PowerState = false;
                                                    pictureBox_AcPower.Image = Properties.Resources.OFF;
                                                }
                                            }
                                        }

                                        System.Threading.Thread.Sleep(5000);

                                        jSuccess = PL2303_GP0_Enable(hCOM, 1);
                                        if (!jSuccess)
                                        {
                                            Log("GP0 output enable FAILED.");
                                        }
                                        else
                                        {
                                            uint val;
                                            val = (uint)int.Parse("1");
                                            bool bSuccess = PL2303_GP0_SetValue(hCOM, val);
                                            if (bSuccess)
                                            {
                                                {
                                                    PowerState = true;
                                                    pictureBox_AcPower.Image = Properties.Resources.ON;
                                                }
                                            }
                                        }
                                    }
                                    ////////////////////////////////////////////////////////////////////////////////////////////////AC OFF//////////////////
                                    if (compare_number[i] % compare_num == 0
                                        && ini12.INIRead(MainSettingPath, "Device", "AutoboxExist", "") == "1"
                                        && ini12.INIRead(MainSettingPath, "LogSearch", "AC OFF", "") == "1")
                                    {
                                        byte[] val1 = new byte[2];
                                        val1[0] = 0;
                                        uint val = (uint)int.Parse("0");

                                        bool Success_GP0_Enable = PL2303_GP0_Enable(hCOM, 1);
                                        bool Success_GP0_SetValue = PL2303_GP0_SetValue(hCOM, val);

                                        bool Success_GP1_Enable = PL2303_GP1_Enable(hCOM, 1);
                                        bool Success_GP1_SetValue = PL2303_GP1_SetValue(hCOM, val);

                                        PowerState = false;

                                        pictureBox_AcPower.Image = Properties.Resources.OFF;
                                    }
                                    ////////////////////////////////////////////////////////////////////////////////////////////////SAVE LOG//////////////////
                                    if (compare_number[i] % compare_num == 0 && ini12.INIRead(MainSettingPath, "LogSearch", "Savelog", "") == "1")
                                    {
                                        string fName = "";

                                        // 讀取ini中的路徑
                                        fName = ini12.INIRead(MainSettingPath, "Record", "LogPath", "");
                                        string t = fName + "\\_SaveLog2_" + DateTime.Now.ToString("yyyyMMddHHmmss") + "_" + label_LoopNumber_Value.Text + ".txt";

                                        StreamWriter MYFILE = new StreamWriter(t, false, Encoding.ASCII);
                                        MYFILE.Write(log2_text);
                                        MYFILE.Close();
                                    }
                                    ////////////////////////////////////////////////////////////////////////////////////////////////STOP//////////////////
                                    if (compare_number[i] % compare_num == 0 && ini12.INIRead(MainSettingPath, "LogSearch", "Stop", "") == "1")
                                    {
                                        button_Start.PerformClick();
                                    }
                                    ////////////////////////////////////////////////////////////////////////////////////////////////SCHEDULE//////////////////
                                    if (compare_number[i] % compare_num == 0)
                                    {
                                        int keyword_numer = i + 1;
                                        switch (keyword_numer)
                                        {
                                            case 1:
                                                Global.keyword_1 = "true";
                                                break;

                                            case 2:
                                                Global.keyword_2 = "true";
                                                break;

                                            case 3:
                                                Global.keyword_3 = "true";
                                                break;

                                            case 4:
                                                Global.keyword_4 = "true";
                                                break;

                                            case 5:
                                                Global.keyword_5 = "true";
                                                break;

                                            case 6:
                                                Global.keyword_6 = "true";
                                                break;

                                            case 7:
                                                Global.keyword_7 = "true";
                                                break;

                                            case 8:
                                                Global.keyword_8 = "true";
                                                break;

                                            case 9:
                                                Global.keyword_9 = "true";
                                                break;

                                            case 10:
                                                Global.keyword_10 = "true";
                                                break;
                                        }
                                    }
                                }
                            }
                            //textBox2.AppendText(my_string + '\r');
                            my_string = "";
                        }
                        #endregion

                        else
                        {
                            my_string = my_string + Keyword_SerialPort_2_temp_char;
                        }
                    }
                    else
                    {

                        if ((Keyword_SerialPort_2_temp_char == '\n'))
                        {
                            //textBox2.AppendText(my_string + '\n');
                            my_string = "";
                        }
                        else if ((Keyword_SerialPort_2_temp_char == '\r'))
                        {
                            //textBox2.AppendText(my_string + '\r');
                            my_string = "";
                        }
                        else
                        {
                            my_string = my_string + Keyword_SerialPort_2_temp_char;
                        }
                    }
                }
                Thread.Sleep(500);
            }
        }
        #endregion

        #region -- 關鍵字比對 - serialport_3 --
        private void MyLog3Camd()
        {
            string my_string = "";
            string csvFile = ini12.INIRead(MainSettingPath, "Record", "LogPath", "") + "\\Log3_keyword.csv";
            int[] compare_number = new int[10];
            bool[] send_status = new bool[10];
            int compare_paremeter = Convert.ToInt32(ini12.INIRead(MainSettingPath, "LogSearch", "TextNum", ""));

            while (StartButtonPressed == true)
            {
                while (SearchLogQueue3.Count > 0)
                {
                    Keyword_SerialPort_3_temp_byte = SearchLogQueue3.Dequeue();
                    Keyword_SerialPort_3_temp_char = (char)Keyword_SerialPort_3_temp_byte;

                    if (Convert.ToInt32(ini12.INIRead(MainSettingPath, "LogSearch", "Comport3", "")) == 1 && Convert.ToInt32(ini12.INIRead(MainSettingPath, "LogSearch", "TextNum", "")) > 0)
                    {
                        #region \n
                        if ((Keyword_SerialPort_3_temp_char == '\n'))
                        {
                            for (int i = 0; i < compare_paremeter; i++)
                            {
                                string compare_string = ini12.INIRead(MainSettingPath, "LogSearch", "Text" + i, "");
                                int compare_num = Convert.ToInt32(ini12.INIRead(MainSettingPath, "LogSearch", "Times" + i, ""));
                                string[] ewords = my_string.Split(new string[] { compare_string }, StringSplitOptions.None);
                                if (Convert.ToInt32(ewords.Length - 1) >= 1)
                                {
                                    compare_number[i] = compare_number[i] + (ewords.Length - 1);
                                    //Console.WriteLine(compare_string + ": " + compare_number[i]);
                                    if (System.IO.File.Exists(csvFile) == false)
                                    {
                                        StreamWriter sw1 = new StreamWriter(csvFile, false, Encoding.UTF8);
                                        sw1.WriteLine("Key words, Setting times, Search times, Time");
                                        sw1.Dispose();
                                    }
                                    StreamWriter sw2 = new StreamWriter(csvFile, true);
                                    sw2.Write(compare_string + ",");
                                    sw2.Write(compare_num + ",");
                                    sw2.Write(compare_number[i] + ",");
                                    sw2.WriteLine(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));
                                    sw2.Close();

                                    ////////////////////////////////////////////////////////////////////////////////////////////////MAIL//////////////////
                                    if (compare_number[i] > compare_num && send_status[i] == false)
                                    {
                                        ini12.INIWrite(MainSettingPath, "LogSearch", "Nowvalue", i.ToString());
                                        ini12.INIWrite(MainSettingPath, "LogSearch", "Display" + i, compare_number[i].ToString());
                                        if (ini12.INIRead(MailPath, "Mail Info", "From", "") != ""
                                            && ini12.INIRead(MailPath, "Mail Info", "To", "") != ""
                                            && ini12.INIRead(MainSettingPath, "LogSearch", "Sendmail", "") == "1")
                                        {
                                            FormMail FormMail = new FormMail();
                                            FormMail.logsend();
                                            send_status[i] = true;
                                        }
                                    }
                                    ////////////////////////////////////////////////////////////////////////////////////////////////AC OFF ON//////////////////
                                    if (compare_number[i] % compare_num == 0
                                        && ini12.INIRead(MainSettingPath, "Device", "AutoboxExist", "") == "1"
                                        && ini12.INIRead(MainSettingPath, "LogSearch", "ACcontrol", "") == "1")
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
                                            if (bSuccess)
                                            {
                                                {
                                                    PowerState = false;
                                                    pictureBox_AcPower.Image = Properties.Resources.OFF;
                                                }
                                            }
                                        }

                                        System.Threading.Thread.Sleep(5000);

                                        jSuccess = PL2303_GP0_Enable(hCOM, 1);
                                        if (!jSuccess)
                                        {
                                            Log("GP0 output enable FAILED.");
                                        }
                                        else
                                        {
                                            uint val;
                                            val = (uint)int.Parse("1");
                                            bool bSuccess = PL2303_GP0_SetValue(hCOM, val);
                                            if (bSuccess)
                                            {
                                                {
                                                    PowerState = true;
                                                    pictureBox_AcPower.Image = Properties.Resources.ON;
                                                }
                                            }
                                        }
                                    }
                                    ////////////////////////////////////////////////////////////////////////////////////////////////AC OFF//////////////////
                                    if (compare_number[i] % compare_num == 0
                                        && ini12.INIRead(MainSettingPath, "Device", "AutoboxExist", "") == "1"
                                        && ini12.INIRead(MainSettingPath, "LogSearch", "AC OFF", "") == "1")
                                    {
                                        byte[] val1 = new byte[2];
                                        val1[0] = 0;
                                        uint val = (uint)int.Parse("0");

                                        bool Success_GP0_Enable = PL2303_GP0_Enable(hCOM, 1);
                                        bool Success_GP0_SetValue = PL2303_GP0_SetValue(hCOM, val);

                                        bool Success_GP1_Enable = PL2303_GP1_Enable(hCOM, 1);
                                        bool Success_GP1_SetValue = PL2303_GP1_SetValue(hCOM, val);

                                        PowerState = false;

                                        pictureBox_AcPower.Image = Properties.Resources.OFF;
                                    }
                                    ////////////////////////////////////////////////////////////////////////////////////////////////SAVE LOG//////////////////
                                    if (compare_number[i] % compare_num == 0 && ini12.INIRead(MainSettingPath, "LogSearch", "Savelog", "") == "1")
                                    {
                                        string fName = "";

                                        // 讀取ini中的路徑
                                        fName = ini12.INIRead(MainSettingPath, "Record", "LogPath", "");
                                        string t = fName + "\\_SaveLog2_" + DateTime.Now.ToString("yyyyMMddHHmmss") + "_" + label_LoopNumber_Value.Text + ".txt";

                                        StreamWriter MYFILE = new StreamWriter(t, false, Encoding.ASCII);
                                        MYFILE.Write(log2_text);
                                        MYFILE.Close();
                                    }
                                    ////////////////////////////////////////////////////////////////////////////////////////////////SCHEDULE//////////////////
                                    if (compare_number[i] % compare_num == 0 && ini12.INIRead(MainSettingPath, "LogSearch", "Stop", "") == "1")
                                    {
                                        button_Start.PerformClick();
                                    }

                                    if (compare_number[i] % compare_num == 0)
                                    {
                                        int keyword_numer = i + 1;
                                        switch (keyword_numer)
                                        {
                                            case 1:
                                                Global.keyword_1 = "true";
                                                break;

                                            case 2:
                                                Global.keyword_2 = "true";
                                                break;

                                            case 3:
                                                Global.keyword_3 = "true";
                                                break;

                                            case 4:
                                                Global.keyword_4 = "true";
                                                break;

                                            case 5:
                                                Global.keyword_5 = "true";
                                                break;

                                            case 6:
                                                Global.keyword_6 = "true";
                                                break;

                                            case 7:
                                                Global.keyword_7 = "true";
                                                break;

                                            case 8:
                                                Global.keyword_8 = "true";
                                                break;

                                            case 9:
                                                Global.keyword_9 = "true";
                                                break;

                                            case 10:
                                                Global.keyword_10 = "true";
                                                break;
                                        }
                                    }
                                }
                            }
                            //textBox2.AppendText(my_string + '\n');
                            my_string = "";
                        }
                        #endregion

                        #region \r
                        else if ((Keyword_SerialPort_3_temp_char == '\r'))
                        {
                            for (int i = 0; i < compare_paremeter; i++)
                            {
                                string compare_string = ini12.INIRead(MainSettingPath, "LogSearch", "Text" + i, "");
                                int compare_num = Convert.ToInt32(ini12.INIRead(MainSettingPath, "LogSearch", "Times" + i, ""));
                                string[] ewords = my_string.Split(new string[] { compare_string }, StringSplitOptions.None);
                                if (Convert.ToInt32(ewords.Length - 1) >= 1)
                                {
                                    compare_number[i] = compare_number[i] + (ewords.Length - 1);
                                    //Console.WriteLine(compare_string + ": " + compare_number[i]);

                                    //////////////////////////////////////////////////////////////////////Create the compare csv file////////////////////
                                    if (System.IO.File.Exists(csvFile) == false)
                                    {
                                        StreamWriter sw1 = new StreamWriter(csvFile, false, Encoding.UTF8);
                                        sw1.WriteLine("Key words, Setting times, Search times, Time");
                                        sw1.Dispose();
                                    }
                                    StreamWriter sw2 = new StreamWriter(csvFile, true);
                                    sw2.Write(compare_string + ",");
                                    sw2.Write(compare_num + ",");
                                    sw2.Write(compare_number[i] + ",");
                                    sw2.WriteLine(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));
                                    sw2.Close();

                                    ////////////////////////////////////////////////////////////////////////////////////////////////MAIL//////////////////
                                    if (compare_number[i] > compare_num && send_status[i] == false)
                                    {
                                        ini12.INIWrite(MainSettingPath, "LogSearch", "Nowvalue", i.ToString());
                                        ini12.INIWrite(MainSettingPath, "LogSearch", "Display" + i, compare_number[i].ToString());
                                        if (ini12.INIRead(MailPath, "Mail Info", "From", "") != ""
                                            && ini12.INIRead(MailPath, "Mail Info", "To", "") != ""
                                            && ini12.INIRead(MainSettingPath, "LogSearch", "Sendmail", "") == "1")
                                        {
                                            FormMail FormMail = new FormMail();
                                            FormMail.logsend();
                                            send_status[i] = true;
                                        }
                                    }
                                    ////////////////////////////////////////////////////////////////////////////////////////////////AC OFF ON//////////////////
                                    if (compare_number[i] % compare_num == 0
                                        && ini12.INIRead(MainSettingPath, "Device", "AutoboxExist", "") == "1"
                                        && ini12.INIRead(MainSettingPath, "LogSearch", "ACcontrol", "") == "1")
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
                                            if (bSuccess)
                                            {
                                                {
                                                    PowerState = false;
                                                    pictureBox_AcPower.Image = Properties.Resources.OFF;
                                                }
                                            }
                                        }

                                        System.Threading.Thread.Sleep(5000);

                                        jSuccess = PL2303_GP0_Enable(hCOM, 1);
                                        if (!jSuccess)
                                        {
                                            Log("GP0 output enable FAILED.");
                                        }
                                        else
                                        {
                                            uint val;
                                            val = (uint)int.Parse("1");
                                            bool bSuccess = PL2303_GP0_SetValue(hCOM, val);
                                            if (bSuccess)
                                            {
                                                {
                                                    PowerState = true;
                                                    pictureBox_AcPower.Image = Properties.Resources.ON;
                                                }
                                            }
                                        }
                                    }
                                    ////////////////////////////////////////////////////////////////////////////////////////////////AC OFF//////////////////
                                    if (compare_number[i] % compare_num == 0
                                        && ini12.INIRead(MainSettingPath, "Device", "AutoboxExist", "") == "1"
                                        && ini12.INIRead(MainSettingPath, "LogSearch", "AC OFF", "") == "1")
                                    {
                                        byte[] val1 = new byte[2];
                                        val1[0] = 0;
                                        uint val = (uint)int.Parse("0");

                                        bool Success_GP0_Enable = PL2303_GP0_Enable(hCOM, 1);
                                        bool Success_GP0_SetValue = PL2303_GP0_SetValue(hCOM, val);

                                        bool Success_GP1_Enable = PL2303_GP1_Enable(hCOM, 1);
                                        bool Success_GP1_SetValue = PL2303_GP1_SetValue(hCOM, val);

                                        PowerState = false;

                                        pictureBox_AcPower.Image = Properties.Resources.OFF;
                                    }
                                    ////////////////////////////////////////////////////////////////////////////////////////////////SAVE LOG//////////////////
                                    if (compare_number[i] % compare_num == 0 && ini12.INIRead(MainSettingPath, "LogSearch", "Savelog", "") == "1")
                                    {
                                        string fName = "";

                                        // 讀取ini中的路徑
                                        fName = ini12.INIRead(MainSettingPath, "Record", "LogPath", "");
                                        string t = fName + "\\_SaveLog3_" + DateTime.Now.ToString("yyyyMMddHHmmss") + "_" + label_LoopNumber_Value.Text + ".txt";

                                        StreamWriter MYFILE = new StreamWriter(t, false, Encoding.ASCII);
                                        MYFILE.Write(log3_text);
                                        MYFILE.Close();
                                    }
                                    ////////////////////////////////////////////////////////////////////////////////////////////////STOP//////////////////
                                    if (compare_number[i] % compare_num == 0 && ini12.INIRead(MainSettingPath, "LogSearch", "Stop", "") == "1")
                                    {
                                        button_Start.PerformClick();
                                    }
                                    ////////////////////////////////////////////////////////////////////////////////////////////////SCHEDULE//////////////////
                                    if (compare_number[i] % compare_num == 0)
                                    {
                                        int keyword_numer = i + 1;
                                        switch (keyword_numer)
                                        {
                                            case 1:
                                                Global.keyword_1 = "true";
                                                break;

                                            case 2:
                                                Global.keyword_2 = "true";
                                                break;

                                            case 3:
                                                Global.keyword_3 = "true";
                                                break;

                                            case 4:
                                                Global.keyword_4 = "true";
                                                break;

                                            case 5:
                                                Global.keyword_5 = "true";
                                                break;

                                            case 6:
                                                Global.keyword_6 = "true";
                                                break;

                                            case 7:
                                                Global.keyword_7 = "true";
                                                break;

                                            case 8:
                                                Global.keyword_8 = "true";
                                                break;

                                            case 9:
                                                Global.keyword_9 = "true";
                                                break;

                                            case 10:
                                                Global.keyword_10 = "true";
                                                break;
                                        }
                                    }
                                }
                            }
                            //textBox3.AppendText(my_string + '\r');
                            my_string = "";
                        }
                        #endregion

                        else
                        {
                            my_string = my_string + Keyword_SerialPort_3_temp_char;
                        }
                    }
                    else
                    {

                        if ((Keyword_SerialPort_3_temp_char == '\n'))
                        {
                            //textBox3.AppendText(my_string + '\n');
                            my_string = "";
                        }
                        else if ((Keyword_SerialPort_3_temp_char == '\r'))
                        {
                            //textBox3.AppendText(my_string + '\r');
                            my_string = "";
                        }
                        else
                        {
                            my_string = my_string + Keyword_SerialPort_3_temp_char;
                        }
                    }
                }
                Thread.Sleep(500);
            }
        }
        #endregion

        #region -- 關鍵字比對 - serialport_4 --
        private void MyLog4Camd()
        {

        }
        #endregion

        #region -- 關鍵字比對 - serialport_5 --
        private void MyLog5Camd()
        {

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

            for (int l = 0; l <= Global.Schedule_Loop; l++)
            {
                Global.NGValue[l] = 0;
                Global.NGRateValue[l] = 0;
            }

            #region -- 匯出比對結果到CSV & EXCEL --
            if (ini12.INIRead(MainSettingPath, "Record", "CompareChoose", "") == "1" && StartButtonPressed == true)
            {
                string compareFolder = ini12.INIRead(MainSettingPath, "Record", "VideoPath", "") + "\\" + "Schedule" + Global.Schedule_Number + "_Original_" + DateTime.Now.ToString("yyyyMMddHHmmss");

                if (Directory.Exists(compareFolder))
                {

                }
                else
                {
                    Directory.CreateDirectory(compareFolder);
                    ini12.INIWrite(MainSettingPath, "Record", "ComparePath", compareFolder);
                }
                // 匯出csv記錄檔
                string csvFile = ini12.INIRead(MainSettingPath, "Record", "ComparePath", "") + "\\SimilarityReport_" + Global.Schedule_Number + ".csv";
                StreamWriter sw = new StreamWriter(csvFile, false, Encoding.UTF8);
                sw.WriteLine("Target, Source, Similarity, Sub-NG count, NGRate, Result");

                sw.Dispose();
                /*
                                #region Excel function
                                // 匯出excel記錄檔
                                Global.excel_Num = 1;
                                string excelFile = ini12.INIRead(sPath, "Record", "ComparePath", "") + "\\SimilarityReport_" + Global.Schedule_Num;

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
                UpdateUI(j.ToString(), label_LoopNumber_Value);
                ini12.INIWrite(MailPath, "Data Info", "CreateTime", string.Format("{0:R}", DateTime.Now));

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
                            MessageBox.Show(Ex.Message.ToString(), "The schedule length incorrect !");
                        }
                        string sch_log_text = "[" + sch_dt.ToString("yyyy/MM/dd HH:mm:ss.fff") + "]  " + Schedule_log + "\r\n";
                        schedule_text = string.Concat(schedule_text, sch_log_text);
                        //textBox_TestLog.AppendText(sch_log_text);
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
                                    MessageBox.Show("Please connect AutoBox!", "Autobox Open Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                                    MessageBox.Show("Please connect AutoBox!", "Autobox Open Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                                    MessageBox.Show("Please connect AutoBox!", "Autobox Open Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                                    MessageBox.Show("Please connect AutoBox!", "Autobox Open Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                                    MessageBox.Show("Please connect AutoBox!", "Autobox Open Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                                    MessageBox.Show("Please connect AutoBox!", "Autobox Open Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                                    MessageBox.Show("Please connect AutoBox!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                                    MessageBox.Show("Please connect AutoBox!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                                    MessageBox.Show("Please connect AutoBox!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                                    MessageBox.Show("Please connect AutoBox!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                                MessageBox.Show("Camera not exist", "Camera Open Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                                MessageBox.Show("Camera not exist", "Camera Open Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                button_Start.PerformClick();
                            }
                        }
                        #endregion
                        /*
                                                #region -- COM PORT --
                                                else if (columns_command == "_log1")
                                                {
                                                    if (ini12.INIRead(MainSettingPath, "SerialPortA", "Checked", "") == "1")
                                                    {
                                                        switch (columns_serial)
                                                        {
                                                            case "_clear":
                                                                textBox1 = string.empty; //清除textbox1
                                                                break;

                                                            case "_save":
                                                                Rs232save(); //存檔rs232
                                                                break;

                                                            default:
                                                                //byte[] data = Encoding.Unicode.GetBytes(DataGridView1.Rows[Global.Scheduler_Row].Cells[5].Value.ToString());
                                                                // string str = Convert.ToString(data);
                                                                serialPort1.WriteLine(columns_serial); //發送數據 Rs232
                                                                DateTime dt = DateTime.Now;
                                                                string text = "[" + dt.ToString("yyyy/MM/dd HH:mm:ss") + "]  " + columns_serial + "\n";
                                                                textBox1.AppendText(text);
                                                                break;
                                                        }
                                                        label_Command.Text = "(" + columns_command + ") " + columns_serial;
                                                    }
                                                }

                                                else if (columns_command == "_log2")
                                                {
                                                    if (ini12.INIRead(MainSettingPath, "SerialPortB", "Checked", "") == "1")
                                                    {
                                                        switch (columns_serial)
                                                        {
                                                            case "_clear":
                                                                textBox2.Clear(); //清除textbox2
                                                                break;

                                                            case "_save":
                                                                ExtRs232save(); //存檔rs232
                                                                break;

                                                            default:
                                                                //byte[] data = Encoding.Unicode.GetBytes(DataGridView1.Rows[Global.Scheduler_Row].Cells[5].Value.ToString());
                                                                // string str = Convert.ToString(data);
                                                                serialPort2.WriteLine(columns_serial); //發送數據 Rs232
                                                                DateTime dt = DateTime.Now;
                                                                string text = "[" + dt.ToString("yyyy/MM/dd HH:mm:ss") + "]  " + columns_serial + "\n";
                                                                textBox2.AppendText(text);
                                                                break;
                                                        }
                                                        label_Command.Text = "(" + columns_command + ") " + columns_serial;
                                                    }
                                                }

                                                else if (columns_command == "_log3")
                                                {
                                                    if (ini12.INIRead(MainSettingPath, "SerialPortC", "Checked", "") == "1")
                                                    {
                                                        switch (columns_serial)
                                                        {
                                                            case "_clear":
                                                                textBox3.Clear(); //清除textbox3
                                                                break;

                                                            case "_save":
                                                                TriRs232save(); //存檔rs232
                                                                break;

                                                            default:
                                                                //byte[] data = Encoding.Unicode.GetBytes(DataGridView1.Rows[Global.Scheduler_Row].Cells[5].Value.ToString());
                                                                // string str = Convert.ToString(data);
                                                                serialPort3.WriteLine(columns_serial); //發送數據 Rs232
                                                                DateTime dt = DateTime.Now;
                                                                string text = "[" + dt.ToString("yyyy/MM/dd HH:mm:ss") + "]  " + columns_serial + "\n";
                                                                textBox3.AppendText(text);
                                                                break;
                                                        }
                                                        label_Command.Text = "(" + columns_command + ") " + columns_serial;
                                                    }
                                                }
                                                #endregion
                        */
                        #region -- Ascii --
                        else if (columns_command == "_ascii")
                        {
                            if (ini12.INIRead(MainSettingPath, "SerialPortA", "Checked", "") == "1" && columns_comport == "A")
                            {
                                Console.WriteLine("Ascii Log: _SerialPortA");
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
                                    serialPortA.Write(columns_serial + "\r"); //發送數據 Rs232 + \r
                                    DateTime dt = DateTime.Now;
                                    string text = "[" + dt.ToString("yyyy/MM/dd HH:mm:ss") + "]  " + columns_serial + "\n\r";
                                    textBox_serial.AppendText(text);
                                    log1_text = string.Concat(log1_text, text);
                                }
                                else if (columns_serial != "" && columns_switch == @"\n")
                                {
                                    serialPortA.Write(columns_serial + "\n"); //發送數據 Rs232 + \n
                                    DateTime dt = DateTime.Now;
                                    string text = "[" + dt.ToString("yyyy/MM/dd HH:mm:ss") + "]  " + columns_serial + "\n\r";
                                    textBox_serial.AppendText(text);
                                    log1_text = string.Concat(log1_text, text);
                                }
                                else if (columns_serial != "" && columns_switch == @"\n\r")
                                {
                                    serialPortA.Write(columns_serial + "\n\r"); //發送數據 Rs232 + \n\r
                                    DateTime dt = DateTime.Now;
                                    string text = "[" + dt.ToString("yyyy/MM/dd HH:mm:ss") + "]  " + columns_serial + "\n\r";
                                    textBox_serial.AppendText(text);
                                    log1_text = string.Concat(log1_text, text);
                                }
                                else if (columns_serial != "" && columns_switch == @"\r\n")
                                {
                                    serialPortA.Write(columns_serial + "\r\n"); //發送數據 Rs232 + \r\n
                                    DateTime dt = DateTime.Now;
                                    string text = "[" + dt.ToString("yyyy/MM/dd HH:mm:ss") + "]  " + columns_serial + "\r\n";
                                    textBox_serial.AppendText(text);
                                    log1_text = string.Concat(log1_text, text);
                                }
                            }

                            if (ini12.INIRead(MainSettingPath, "SerialPortB", "Checked", "") == "1" && columns_comport == "B")
                            {
                                Console.WriteLine("Ascii Log: _SerialPortB");
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
                                    serialPortB.Write(columns_serial + "\r"); //發送數據 Rs232 + \r
                                    DateTime dt = DateTime.Now;
                                    string text = "[" + dt.ToString("yyyy/MM/dd HH:mm:ss") + "]  " + columns_serial + "\r\n";
                                    textBox_serial.AppendText(text);
                                    log2_text = string.Concat(log2_text, text);
                                }
                                else if (columns_serial != "" && columns_switch == @"\n")
                                {
                                    serialPortB.Write(columns_serial + "\n"); //發送數據 Rs232 + \n
                                    DateTime dt = DateTime.Now;
                                    string text = "[" + dt.ToString("yyyy/MM/dd HH:mm:ss") + "]  " + columns_serial + "\r\n";
                                    textBox_serial.AppendText(text);
                                    log2_text = string.Concat(log2_text, text);
                                }
                                else if (columns_serial != "" && columns_switch == @"\n\r")
                                {
                                    serialPortB.Write(columns_serial + "\n\r"); //發送數據 Rs232 + \n\r
                                    DateTime dt = DateTime.Now;
                                    string text = "[" + dt.ToString("yyyy/MM/dd HH:mm:ss") + "]  " + columns_serial + "\r\n";
                                    textBox_serial.AppendText(text);
                                    log2_text = string.Concat(log2_text, text);
                                }
                                else if (columns_serial != "" && columns_switch == @"\r\n")
                                {
                                    serialPortB.Write(columns_serial + "\r\n"); //發送數據 Rs232 + \r\n
                                    DateTime dt = DateTime.Now;
                                    string text = "[" + dt.ToString("yyyy/MM/dd HH:mm:ss") + "]  " + columns_serial + "\r\n";
                                    textBox_serial.AppendText(text);
                                    log2_text = string.Concat(log2_text, text);
                                }
                            }

                            if (ini12.INIRead(MainSettingPath, "SerialPortC", "Checked", "") == "1" && columns_comport == "C")
                            {
                                Console.WriteLine("Ascii Log: _SerialPortC");
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
                                    serialPortC.Write(columns_serial + "\r"); //發送數據 Rs232 + \r
                                    DateTime dt = DateTime.Now;
                                    string text = "[" + dt.ToString("yyyy/MM/dd HH:mm:ss") + "]  " + columns_serial + "\r\n";
                                    textBox_serial.AppendText(text);
                                    log3_text = string.Concat(log3_text, text);
                                }
                                else if (columns_serial != "" && columns_switch == @"\n")
                                {
                                    serialPortC.Write(columns_serial + "\n"); //發送數據 Rs232 + \n
                                    DateTime dt = DateTime.Now;
                                    string text = "[" + dt.ToString("yyyy/MM/dd HH:mm:ss") + "]  " + columns_serial + "\r\n";
                                    textBox_serial.AppendText(text);
                                    log3_text = string.Concat(log3_text, text);
                                }
                                else if (columns_serial != "" && columns_switch == @"\n\r")
                                {
                                    serialPortC.Write(columns_serial + "\n\r"); //發送數據 Rs232 + \n\r
                                    DateTime dt = DateTime.Now;
                                    string text = "[" + dt.ToString("yyyy/MM/dd HH:mm:ss") + "]  " + columns_serial + "\r\n";
                                    textBox_serial.AppendText(text);
                                    log3_text = string.Concat(log3_text, text);
                                }
                                else if (columns_serial != "" && columns_switch == @"\r\n")
                                {
                                    serialPortC.Write(columns_serial + "\r\n"); //發送數據 Rs232 + \r\n
                                    DateTime dt = DateTime.Now;
                                    string text = "[" + dt.ToString("yyyy/MM/dd HH:mm:ss") + "]  " + columns_serial + "\r\n";
                                    textBox_serial.AppendText(text);
                                    log3_text = string.Concat(log3_text, text);
                                }
                            }

                            if (ini12.INIRead(MainSettingPath, "SerialPortD", "Checked", "") == "1" && columns_comport == "D")
                            {
                                Console.WriteLine("Ascii Log: _SerialPortD");
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
                                    serialPortD.Write(columns_serial + "\r"); //發送數據 Rs232 + \r
                                    DateTime dt = DateTime.Now;
                                    string text = "[" + dt.ToString("yyyy/MM/dd HH:mm:ss") + "]  " + columns_serial + "\r\n";
                                    textBox_serial.AppendText(text);
                                    log4_text = string.Concat(log4_text, text);
                                }
                                else if (columns_serial != "" && columns_switch == @"\n")
                                {
                                    serialPortD.Write(columns_serial + "\n"); //發送數據 Rs232 + \n
                                    DateTime dt = DateTime.Now;
                                    string text = "[" + dt.ToString("yyyy/MM/dd HH:mm:ss") + "]  " + columns_serial + "\r\n";
                                    textBox_serial.AppendText(text);
                                    log4_text = string.Concat(log4_text, text);
                                }
                                else if (columns_serial != "" && columns_switch == @"\n\r")
                                {
                                    serialPortD.Write(columns_serial + "\n\r"); //發送數據 Rs232 + \n\r
                                    DateTime dt = DateTime.Now;
                                    string text = "[" + dt.ToString("yyyy/MM/dd HH:mm:ss") + "]  " + columns_serial + "\r\n";
                                    textBox_serial.AppendText(text);
                                    log4_text = string.Concat(log4_text, text);
                                }
                                else if (columns_serial != "" && columns_switch == @"\r\n")
                                {
                                    serialPortD.Write(columns_serial + "\r\n"); //發送數據 Rs232 + \r\n
                                    DateTime dt = DateTime.Now;
                                    string text = "[" + dt.ToString("yyyy/MM/dd HH:mm:ss") + "]  " + columns_serial + "\r\n";
                                    textBox_serial.AppendText(text);
                                    log4_text = string.Concat(log4_text, text);
                                }
                            }

                            if (ini12.INIRead(MainSettingPath, "SerialPortE", "Checked", "") == "1" && columns_comport == "E")
                            {
                                Console.WriteLine("Ascii Log: _SerialPortE");
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
                                    serialPortE.Write(columns_serial + "\r"); //發送數據 Rs232 + \r
                                    DateTime dt = DateTime.Now;
                                    string text = "[" + dt.ToString("yyyy/MM/dd HH:mm:ss") + "]  " + columns_serial + "\r\n";
                                    textBox_serial.AppendText(text);
                                    log5_text = string.Concat(log5_text, text);
                                }
                                else if (columns_serial != "" && columns_switch == @"\n")
                                {
                                    serialPortE.Write(columns_serial + "\n"); //發送數據 Rs232 + \n
                                    DateTime dt = DateTime.Now;
                                    string text = "[" + dt.ToString("yyyy/MM/dd HH:mm:ss") + "]  " + columns_serial + "\r\n";
                                    textBox_serial.AppendText(text);
                                    log5_text = string.Concat(log5_text, text);
                                }
                                else if (columns_serial != "" && columns_switch == @"\n\r")
                                {
                                    serialPortE.Write(columns_serial + "\n\r"); //發送數據 Rs232 + \n\r
                                    DateTime dt = DateTime.Now;
                                    string text = "[" + dt.ToString("yyyy/MM/dd HH:mm:ss") + "]  " + columns_serial + "\r\n";
                                    textBox_serial.AppendText(text);
                                    log5_text = string.Concat(log5_text, text);
                                }
                                else if (columns_serial != "" && columns_switch == @"\r\n")
                                {
                                    serialPortE.Write(columns_serial + "\r\n"); //發送數據 Rs232 + \r\n
                                    DateTime dt = DateTime.Now;
                                    string text = "[" + dt.ToString("yyyy/MM/dd HH:mm:ss") + "]  " + columns_serial + "\r\n";
                                    textBox_serial.AppendText(text);
                                    log5_text = string.Concat(log5_text, text);
                                }
                            }
                            label_Command.Text = "(" + columns_command + ") " + columns_serial;
                        }
                        #endregion

                        #region -- Hex --
                        else if (columns_command == "_HEX")
                        {
                            if (ini12.INIRead(MainSettingPath, "SerialPortA", "Checked", "") == "1" && columns_comport == "A")
                            {
                                Console.WriteLine("Hex Log: _SerialPortA");
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
                                    serialPortA.Write(bytes, 0, bytes.Length); //發送數據 Rs232
                                    DateTime dt = DateTime.Now;
                                    string text = "[" + dt.ToString("yyyy/MM/dd HH:mm:ss") + "]  " + columns_serial + "\r\n";
                                    textBox_serial.AppendText(text);
                                    log1_text = string.Concat(log1_text, text);
                                }
                            }

                            if (ini12.INIRead(MainSettingPath, "SerialPortB", "Checked", "") == "1" && columns_comport == "B")
                            {
                                Console.WriteLine("Hex Log: _SerialPortB");
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
                                    serialPortB.Write(bytes, 0, bytes.Length); //發送數據 Rs232
                                    DateTime dt = DateTime.Now;
                                    string text = "[" + dt.ToString("yyyy/MM/dd HH:mm:ss") + "]  " + columns_serial + "\r\n";
                                    textBox_serial.AppendText(text);
                                    log2_text = string.Concat(log2_text, text);
                                }
                            }

                            if (ini12.INIRead(MainSettingPath, "SerialPortC", "Checked", "") == "1" && columns_comport == "C")
                            {
                                Console.WriteLine("Hex Log: _SerialPortC");
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
                                    serialPortC.Write(bytes, 0, bytes.Length); //發送數據 Rs232
                                    DateTime dt = DateTime.Now;
                                    string text = "[" + dt.ToString("yyyy/MM/dd HH:mm:ss") + "]  " + columns_serial + "\r\n";
                                    textBox_serial.AppendText(text);
                                    log3_text = string.Concat(log3_text, text);
                                }
                            }

                            if (ini12.INIRead(MainSettingPath, "SerialPortD", "Checked", "") == "1" && columns_comport == "D")
                            {
                                Console.WriteLine("Hex Log: _SerialPortD");
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
                                    serialPortD.Write(bytes, 0, bytes.Length); //發送數據 Rs232
                                    DateTime dt = DateTime.Now;
                                    string text = "[" + dt.ToString("yyyy/MM/dd HH:mm:ss") + "]  " + columns_serial + "\r\n";
                                    textBox_serial.AppendText(text);
                                    log4_text = string.Concat(log4_text, text);
                                }
                            }

                            if (ini12.INIRead(MainSettingPath, "SerialPortE", "Checked", "") == "1" && columns_comport == "E")
                            {
                                Console.WriteLine("Hex Log: _SerialPortE");
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
                                    serialPortE.Write(bytes, 0, bytes.Length); //發送數據 Rs232
                                    DateTime dt = DateTime.Now;
                                    string text = "[" + dt.ToString("yyyy/MM/dd HH:mm:ss") + "]  " + columns_serial + "\r\n";
                                    textBox_serial.AppendText(text);
                                    log5_text = string.Concat(log5_text, text);
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
                                            MessageBox.Show("Content include other error code", "ABS code Open Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                        }
                                    }
                                }
                                else
                                {
                                    MessageBox.Show("DTC code file not exist", "File Open Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                }
                                label_Command.Text = "(" + columns_command + ") " + columns_serial;
                            }
                            catch (Exception Ex)
                            {
                                MessageBox.Show(Ex.Message.ToString(), "Kline_ABS library error !");
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
                                            MessageBox.Show("Content include other error code", "OBD code Open Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                        }
                                    }
                                }
                                else
                                {
                                    MessageBox.Show("DTC code file not exist", "File Open Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                                serialPortA.Write(startbit, 0, 7);

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
                                                serialPortA.Write(timebit, 0, 4);
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
                                serialPortA.Write(endbit, 0, 3);
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
                                                serialPortA.WriteLine(generator.Element("Signal").Value + "\r");
                                                serialPortA.WriteLine("ALLU" + "\r");
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
                                        serialPortA.WriteLine("AVST 0" + "\r");
                                        serialPortA.WriteLine("DVST 10" + "\r");
                                        serialPortA.WriteLine("FMTU" + "\r");
                                        break;
                                    case "YCbCr":
                                        // YCbCr mode
                                        serialPortA.WriteLine("AVST 0" + "\r");
                                        serialPortA.WriteLine("DVST 14" + "\r");
                                        serialPortA.WriteLine("FMTU" + "\r");
                                        break;
                                    case "xvYCC":
                                        // xvYCC mode
                                        serialPortA.WriteLine("AVST 0" + "\r");
                                        serialPortA.WriteLine("DVST 17" + "\r");
                                        serialPortA.WriteLine("FMTU" + "\r");
                                        break;
                                    case "4:4:4":
                                        // 4:4:4
                                        serialPortA.WriteLine("DVSM 4" + "\r");
                                        serialPortA.WriteLine("FMTU" + "\r");
                                        break;
                                    case "4:2:2":
                                        // 4:2:2
                                        serialPortA.WriteLine("DVSM 2" + "\r");
                                        serialPortA.WriteLine("FMTU" + "\r");
                                        break;
                                    case "8bits":
                                        // 8bits
                                        serialPortA.WriteLine("NBPC 8" + "\r");
                                        serialPortA.WriteLine("FMTU" + "\r");
                                        break;
                                    case "10bits":
                                        // 10bits
                                        serialPortA.WriteLine("NBPC 10" + "\r");
                                        serialPortA.WriteLine("FMTU" + "\r");
                                        break;
                                    case "12bits":
                                        // 12bits
                                        serialPortA.WriteLine("NBPC 12" + "\r");
                                        serialPortA.WriteLine("FMTU" + "\r");
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
                                    if (ini12.INIRead(MainSettingPath, "SerialPortA", "Checked", "") == "1" && columns_comport == "A")
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
                                            serialPortA.Write(columns_serial + "\r"); //發送數據 Rs232 + \r
                                            DateTime dt = DateTime.Now;
                                            string text = "[" + dt.ToString("yyyy/MM/dd HH:mm:ss") + "]  " + columns_serial + "\n\r";
                                            textBox_serial.AppendText(text);
                                            log1_text = string.Concat(log1_text, text);
                                        }
                                        else if (columns_serial != "" && columns_switch == @"\n")
                                        {
                                            serialPortA.Write(columns_serial + "\n"); //發送數據 Rs232 + \n
                                            DateTime dt = DateTime.Now;
                                            string text = "[" + dt.ToString("yyyy/MM/dd HH:mm:ss") + "]  " + columns_serial + "\n\r";
                                            textBox_serial.AppendText(text);
                                            log1_text = string.Concat(log1_text, text);
                                        }
                                        else if (columns_serial != "" && columns_switch == @"\n\r")
                                        {
                                            serialPortA.Write(columns_serial + "\n\r"); //發送數據 Rs232 + \n\r
                                            DateTime dt = DateTime.Now;
                                            string text = "[" + dt.ToString("yyyy/MM/dd HH:mm:ss") + "]  " + columns_serial + "\n\r";
                                            textBox_serial.AppendText(text);
                                            log1_text = string.Concat(log1_text, text);
                                        }
                                        else if (columns_serial != "" && columns_switch == @"\r\n")
                                        {
                                            serialPortA.Write(columns_serial + "\r\n"); //發送數據 Rs232 + \r\n
                                            DateTime dt = DateTime.Now;
                                            string text = "[" + dt.ToString("yyyy/MM/dd HH:mm:ss") + "]  " + columns_serial + "\r\n";
                                            textBox_serial.AppendText(text);
                                            log1_text = string.Concat(log1_text, text);
                                        }
                                        else if (columns_serial != "" && columns_switch == "")
                                        {
                                            serialPortA.Write(columns_serial); //發送數據 HEX Rs232
                                            DateTime dt = DateTime.Now;
                                            string text = "[" + dt.ToString("yyyy/MM/dd HH:mm:ss") + "]  " + columns_serial + "\r\n";
                                            textBox_serial.AppendText(text);
                                            log1_text = string.Concat(log1_text, text);
                                        }
                                    }
                                    else if (ini12.INIRead(MainSettingPath, "SerialPortB", "Checked", "") == "1" && columns_comport == "B")
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
                                            serialPortB.Write(columns_serial + "\r"); //發送數據 Rs232 + \r
                                            DateTime dt = DateTime.Now;
                                            string text = "[" + dt.ToString("yyyy/MM/dd HH:mm:ss") + "]  " + columns_serial + "\n\r";
                                            textBox_serial.AppendText(text);
                                            log2_text = string.Concat(log2_text, text);
                                        }
                                        else if (columns_serial != "" && columns_switch == @"\n")
                                        {
                                            serialPortB.Write(columns_serial + "\n"); //發送數據 Rs232 + \n
                                            DateTime dt = DateTime.Now;
                                            string text = "[" + dt.ToString("yyyy/MM/dd HH:mm:ss") + "]  " + columns_serial + "\n\r";
                                            textBox_serial.AppendText(text);
                                            log2_text = string.Concat(log2_text, text);
                                        }
                                        else if (columns_serial != "" && columns_switch == @"\n\r")
                                        {
                                            serialPortB.Write(columns_serial + "\n\r"); //發送數據 Rs232 + \n\r
                                            DateTime dt = DateTime.Now;
                                            string text = "[" + dt.ToString("yyyy/MM/dd HH:mm:ss") + "]  " + columns_serial + "\n\r";
                                            textBox_serial.AppendText(text);
                                            log2_text = string.Concat(log2_text, text);
                                        }
                                        else if (columns_serial != "" && columns_switch == @"\r\n")
                                        {
                                            serialPortB.Write(columns_serial + "\r\n"); //發送數據 Rs232 + \r\n
                                            DateTime dt = DateTime.Now;
                                            string text = "[" + dt.ToString("yyyy/MM/dd HH:mm:ss") + "]  " + columns_serial + "\r\n";
                                            textBox_serial.AppendText(text);
                                            log2_text = string.Concat(log2_text, text);
                                        }
                                        else if (columns_serial != "" && columns_switch == "")
                                        {
                                            serialPortB.Write(columns_serial); //發送數據 HEX Rs232
                                            DateTime dt = DateTime.Now;
                                            string text = "[" + dt.ToString("yyyy/MM/dd HH:mm:ss") + "]  " + columns_serial + "\r\n";
                                            textBox_serial.AppendText(text);
                                            log2_text = string.Concat(log2_text, text);
                                        }
                                    }
                                    else if (ini12.INIRead(MainSettingPath, "SerialPortC", "Checked", "") == "1" && columns_comport == "C")
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
                                            serialPortC.Write(columns_serial + "\r"); //發送數據 Rs232 + \r
                                            DateTime dt = DateTime.Now;
                                            string text = "[" + dt.ToString("yyyy/MM/dd HH:mm:ss") + "]  " + columns_serial + "\n\r";
                                            textBox_serial.AppendText(text);
                                            log3_text = string.Concat(log3_text, text);
                                        }
                                        else if (columns_serial != "" && columns_switch == @"\n")
                                        {
                                            serialPortC.Write(columns_serial + "\n"); //發送數據 Rs232 + \n
                                            DateTime dt = DateTime.Now;
                                            string text = "[" + dt.ToString("yyyy/MM/dd HH:mm:ss") + "]  " + columns_serial + "\n\r";
                                            textBox_serial.AppendText(text);
                                            log3_text = string.Concat(log3_text, text);
                                        }
                                        else if (columns_serial != "" && columns_switch == @"\n\r")
                                        {
                                            serialPortC.Write(columns_serial + "\n\r"); //發送數據 Rs232 + \n\r
                                            DateTime dt = DateTime.Now;
                                            string text = "[" + dt.ToString("yyyy/MM/dd HH:mm:ss") + "]  " + columns_serial + "\n\r";
                                            textBox_serial.AppendText(text);
                                            log3_text = string.Concat(log3_text, text);
                                        }
                                        else if (columns_serial != "" && columns_switch == @"\r\n")
                                        {
                                            serialPortC.Write(columns_serial + "\r\n"); //發送數據 Rs232 + \r\n
                                            DateTime dt = DateTime.Now;
                                            string text = "[" + dt.ToString("yyyy/MM/dd HH:mm:ss") + "]  " + columns_serial + "\r\n";
                                            textBox_serial.AppendText(text);
                                            log3_text = string.Concat(log3_text, text);
                                        }
                                        else if (columns_serial != "" && columns_switch == "")
                                        {
                                            serialPortC.Write(columns_serial); //發送數據 HEX Rs232
                                            DateTime dt = DateTime.Now;
                                            string text = "[" + dt.ToString("yyyy/MM/dd HH:mm:ss") + "]  " + columns_serial + "\r\n";
                                            textBox_serial.AppendText(text);
                                            log3_text = string.Concat(log3_text, text);
                                        }
                                    }
                                    else if (ini12.INIRead(MainSettingPath, "SerialPortD", "Checked", "") == "1" && columns_comport == "D")
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
                                            serialPortD.Write(columns_serial + "\r"); //發送數據 Rs232 + \r
                                            DateTime dt = DateTime.Now;
                                            string text = "[" + dt.ToString("yyyy/MM/dd HH:mm:ss") + "]  " + columns_serial + "\n\r";
                                            textBox_serial.AppendText(text);
                                            log4_text = string.Concat(log4_text, text);
                                        }
                                        else if (columns_serial != "" && columns_switch == @"\n")
                                        {
                                            serialPortD.Write(columns_serial + "\n"); //發送數據 Rs232 + \n
                                            DateTime dt = DateTime.Now;
                                            string text = "[" + dt.ToString("yyyy/MM/dd HH:mm:ss") + "]  " + columns_serial + "\n\r";
                                            textBox_serial.AppendText(text);
                                            log4_text = string.Concat(log4_text, text);
                                        }
                                        else if (columns_serial != "" && columns_switch == @"\n\r")
                                        {
                                            serialPortD.Write(columns_serial + "\n\r"); //發送數據 Rs232 + \n\r
                                            DateTime dt = DateTime.Now;
                                            string text = "[" + dt.ToString("yyyy/MM/dd HH:mm:ss") + "]  " + columns_serial + "\n\r";
                                            textBox_serial.AppendText(text);
                                            log4_text = string.Concat(log4_text, text);
                                        }
                                        else if (columns_serial != "" && columns_switch == @"\r\n")
                                        {
                                            serialPortD.Write(columns_serial + "\r\n"); //發送數據 Rs232 + \r\n
                                            DateTime dt = DateTime.Now;
                                            string text = "[" + dt.ToString("yyyy/MM/dd HH:mm:ss") + "]  " + columns_serial + "\r\n";
                                            textBox_serial.AppendText(text);
                                            log4_text = string.Concat(log4_text, text);
                                        }
                                        else if (columns_serial != "" && columns_switch == "")
                                        {
                                            serialPortD.Write(columns_serial); //發送數據 HEX Rs232
                                            DateTime dt = DateTime.Now;
                                            string text = "[" + dt.ToString("yyyy/MM/dd HH:mm:ss") + "]  " + columns_serial + "\r\n";
                                            textBox_serial.AppendText(text);
                                            log4_text = string.Concat(log4_text, text);
                                        }
                                    }
                                    else if (ini12.INIRead(MainSettingPath, "SerialPortE", "Checked", "") == "1" && columns_comport == "E")
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
                                            serialPortE.Write(columns_serial + "\r"); //發送數據 Rs232 + \r
                                            DateTime dt = DateTime.Now;
                                            string text = "[" + dt.ToString("yyyy/MM/dd HH:mm:ss") + "]  " + columns_serial + "\n\r";
                                            textBox_serial.AppendText(text);
                                            log5_text = string.Concat(log5_text, text);
                                        }
                                        else if (columns_serial != "" && columns_switch == @"\n")
                                        {
                                            serialPortE.Write(columns_serial + "\n"); //發送數據 Rs232 + \n
                                            DateTime dt = DateTime.Now;
                                            string text = "[" + dt.ToString("yyyy/MM/dd HH:mm:ss") + "]  " + columns_serial + "\n\r";
                                            textBox_serial.AppendText(text);
                                            log5_text = string.Concat(log5_text, text);
                                        }
                                        else if (columns_serial != "" && columns_switch == @"\n\r")
                                        {
                                            serialPortE.Write(columns_serial + "\n\r"); //發送數據 Rs232 + \n\r
                                            DateTime dt = DateTime.Now;
                                            string text = "[" + dt.ToString("yyyy/MM/dd HH:mm:ss") + "]  " + columns_serial + "\n\r";
                                            textBox_serial.AppendText(text);
                                            log5_text = string.Concat(log5_text, text);
                                        }
                                        else if (columns_serial != "" && columns_switch == @"\r\n")
                                        {
                                            serialPortE.Write(columns_serial + "\r\n"); //發送數據 Rs232 + \r\n
                                            DateTime dt = DateTime.Now;
                                            string text = "[" + dt.ToString("yyyy/MM/dd HH:mm:ss") + "]  " + columns_serial + "\r\n";
                                            textBox_serial.AppendText(text);
                                            log5_text = string.Concat(log5_text, text);
                                        }
                                        else if (columns_serial != "" && columns_switch == "")
                                        {
                                            serialPortE.Write(columns_serial); //發送數據 HEX Rs232
                                            DateTime dt = DateTime.Now;
                                            string text = "[" + dt.ToString("yyyy/MM/dd HH:mm:ss") + "]  " + columns_serial + "\r\n";
                                            textBox_serial.AppendText(text);
                                            log5_text = string.Concat(log5_text, text);
                                        }
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

                                    if (ini12.INIRead(MainSettingPath, "SerialPortA", "Checked", "") == "1" && columns_comport == "A")
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
                                            serialPortA.Write(reverse + "\r"); //發送數據 Rs232 + \r
                                            DateTime dt = DateTime.Now;
                                            string text = "[" + dt.ToString("yyyy/MM/dd HH:mm:ss") + "]  " + reverse + "\n\r";
                                            textBox_serial.AppendText(text);
                                            log1_text = string.Concat(log1_text, text);
                                        }
                                        else if (reverse != "" && columns_switch == @"\n")
                                        {
                                            serialPortA.Write(reverse + "\n"); //發送數據 Rs232 + \n
                                            DateTime dt = DateTime.Now;
                                            string text = "[" + dt.ToString("yyyy/MM/dd HH:mm:ss") + "]  " + reverse + "\n\r";
                                            textBox_serial.AppendText(text);
                                            log1_text = string.Concat(log1_text, text);
                                        }
                                        else if (reverse != "" && columns_switch == @"\n\r")
                                        {
                                            serialPortA.Write(reverse + "\n\r"); //發送數據 Rs232 + \n\r
                                            DateTime dt = DateTime.Now;
                                            string text = "[" + dt.ToString("yyyy/MM/dd HH:mm:ss") + "]  " + reverse + "\n\r";
                                            textBox_serial.AppendText(text);
                                            log1_text = string.Concat(log1_text, text);
                                        }
                                        else if (reverse != "" && columns_switch == @"\r\n")
                                        {
                                            serialPortA.Write(reverse + "\r\n"); //發送數據 Rs232 + \r\n
                                            DateTime dt = DateTime.Now;
                                            string text = "[" + dt.ToString("yyyy/MM/dd HH:mm:ss") + "]  " + reverse + "\r\n";
                                            textBox_serial.AppendText(text);
                                            log1_text = string.Concat(log1_text, text);
                                        }
                                        else if (reverse != "" && columns_switch == "")
                                        {
                                            serialPortA.Write(reverse); //發送數據 HEX Rs232
                                            DateTime dt = DateTime.Now;
                                            string text = "[" + dt.ToString("yyyy/MM/dd HH:mm:ss") + "]  " + reverse + "\r\n";
                                            textBox_serial.AppendText(text);
                                            log1_text = string.Concat(log1_text, text);
                                        }
                                    }
                                    else if (ini12.INIRead(MainSettingPath, "SerialPortB", "Checked", "") == "1" && columns_comport == "B")
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
                                            serialPortB.Write(reverse + "\r"); //發送數據 Rs232 + \r
                                            DateTime dt = DateTime.Now;
                                            string text = "[" + dt.ToString("yyyy/MM/dd HH:mm:ss") + "]  " + reverse + "\n\r";
                                            textBox_serial.AppendText(text);
                                            log2_text = string.Concat(log2_text, text);
                                        }
                                        else if (reverse != "" && columns_switch == @"\n")
                                        {
                                            serialPortB.Write(reverse + "\n"); //發送數據 Rs232 + \n
                                            DateTime dt = DateTime.Now;
                                            string text = "[" + dt.ToString("yyyy/MM/dd HH:mm:ss") + "]  " + reverse + "\n\r";
                                            textBox_serial.AppendText(text);
                                            log2_text = string.Concat(log2_text, text);
                                        }
                                        else if (reverse != "" && columns_switch == @"\n\r")
                                        {
                                            serialPortB.Write(reverse + "\n\r"); //發送數據 Rs232 + \n\r
                                            DateTime dt = DateTime.Now;
                                            string text = "[" + dt.ToString("yyyy/MM/dd HH:mm:ss") + "]  " + reverse + "\n\r";
                                            textBox_serial.AppendText(text);
                                            log2_text = string.Concat(log2_text, text);
                                        }
                                        else if (reverse != "" && columns_switch == @"\r\n")
                                        {
                                            serialPortB.Write(reverse + "\r\n"); //發送數據 Rs232 + \r\n
                                            DateTime dt = DateTime.Now;
                                            string text = "[" + dt.ToString("yyyy/MM/dd HH:mm:ss") + "]  " + reverse + "\r\n";
                                            textBox_serial.AppendText(text);
                                            log2_text = string.Concat(log2_text, text);
                                        }
                                        else if (reverse != "" && columns_switch == "")
                                        {
                                            serialPortB.Write(reverse); //發送數據 HEX Rs232
                                            DateTime dt = DateTime.Now;
                                            string text = "[" + dt.ToString("yyyy/MM/dd HH:mm:ss") + "]  " + reverse + "\r\n";
                                            textBox_serial.AppendText(text);
                                            log2_text = string.Concat(log2_text, text);
                                        }
                                    }
                                    else if (ini12.INIRead(MainSettingPath, "SerialPortC", "Checked", "") == "1" && columns_comport == "C")
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
                                            serialPortC.Write(reverse + "\r"); //發送數據 Rs232 + \r
                                            DateTime dt = DateTime.Now;
                                            string text = "[" + dt.ToString("yyyy/MM/dd HH:mm:ss") + "]  " + reverse + "\n\r";
                                            textBox_serial.AppendText(text);
                                            log3_text = string.Concat(log3_text, text);
                                        }
                                        else if (reverse != "" && columns_switch == @"\n")
                                        {
                                            serialPortC.Write(reverse + "\n"); //發送數據 Rs232 + \n
                                            DateTime dt = DateTime.Now;
                                            string text = "[" + dt.ToString("yyyy/MM/dd HH:mm:ss") + "]  " + reverse + "\n\r";
                                            textBox_serial.AppendText(text);
                                            log3_text = string.Concat(log3_text, text);
                                        }
                                        else if (reverse != "" && columns_switch == @"\n\r")
                                        {
                                            serialPortC.Write(reverse + "\n\r"); //發送數據 Rs232 + \n\r
                                            DateTime dt = DateTime.Now;
                                            string text = "[" + dt.ToString("yyyy/MM/dd HH:mm:ss") + "]  " + reverse + "\n\r";
                                            textBox_serial.AppendText(text);
                                            log3_text = string.Concat(log3_text, text);
                                        }
                                        else if (reverse != "" && columns_switch == @"\r\n")
                                        {
                                            serialPortC.Write(reverse + "\r\n"); //發送數據 Rs232 + \r\n
                                            DateTime dt = DateTime.Now;
                                            string text = "[" + dt.ToString("yyyy/MM/dd HH:mm:ss") + "]  " + reverse + "\r\n";
                                            textBox_serial.AppendText(text);
                                            log3_text = string.Concat(log3_text, text);
                                        }
                                        else if (reverse != "" && columns_switch == "")
                                        {
                                            serialPortC.Write(reverse); //發送數據 HEX Rs232
                                            DateTime dt = DateTime.Now;
                                            string text = "[" + dt.ToString("yyyy/MM/dd HH:mm:ss") + "]  " + reverse + "\r\n";
                                            textBox_serial.AppendText(text);
                                            log3_text = string.Concat(log3_text, text);
                                        }
                                    }
                                    else if (ini12.INIRead(MainSettingPath, "SerialPortD", "Checked", "") == "1" && columns_comport == "D")
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
                                            serialPortD.Write(reverse + "\r"); //發送數據 Rs232 + \r
                                            DateTime dt = DateTime.Now;
                                            string text = "[" + dt.ToString("yyyy/MM/dd HH:mm:ss") + "]  " + reverse + "\n\r";
                                            textBox_serial.AppendText(text);
                                            log4_text = string.Concat(log4_text, text);
                                        }
                                        else if (reverse != "" && columns_switch == @"\n")
                                        {
                                            serialPortD.Write(reverse + "\n"); //發送數據 Rs232 + \n
                                            DateTime dt = DateTime.Now;
                                            string text = "[" + dt.ToString("yyyy/MM/dd HH:mm:ss") + "]  " + reverse + "\n\r";
                                            textBox_serial.AppendText(text);
                                            log4_text = string.Concat(log4_text, text);
                                        }
                                        else if (reverse != "" && columns_switch == @"\n\r")
                                        {
                                            serialPortD.Write(reverse + "\n\r"); //發送數據 Rs232 + \n\r
                                            DateTime dt = DateTime.Now;
                                            string text = "[" + dt.ToString("yyyy/MM/dd HH:mm:ss") + "]  " + reverse + "\n\r";
                                            textBox_serial.AppendText(text);
                                            log4_text = string.Concat(log4_text, text);
                                        }
                                        else if (reverse != "" && columns_switch == @"\r\n")
                                        {
                                            serialPortD.Write(reverse + "\r\n"); //發送數據 Rs232 + \r\n
                                            DateTime dt = DateTime.Now;
                                            string text = "[" + dt.ToString("yyyy/MM/dd HH:mm:ss") + "]  " + reverse + "\r\n";
                                            textBox_serial.AppendText(text);
                                            log4_text = string.Concat(log4_text, text);
                                        }
                                        else if (reverse != "" && columns_switch == "")
                                        {
                                            serialPortD.Write(reverse); //發送數據 HEX Rs232
                                            DateTime dt = DateTime.Now;
                                            string text = "[" + dt.ToString("yyyy/MM/dd HH:mm:ss") + "]  " + reverse + "\r\n";
                                            textBox_serial.AppendText(text);
                                            log4_text = string.Concat(log4_text, text);
                                        }
                                    }
                                    else if (ini12.INIRead(MainSettingPath, "SerialPortE", "Checked", "") == "1" && columns_comport == "E")
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
                                            serialPortE.Write(reverse + "\r"); //發送數據 Rs232 + \r
                                            DateTime dt = DateTime.Now;
                                            string text = "[" + dt.ToString("yyyy/MM/dd HH:mm:ss") + "]  " + reverse + "\n\r";
                                            textBox_serial.AppendText(text);
                                            log5_text = string.Concat(log5_text, text);
                                        }
                                        else if (reverse != "" && columns_switch == @"\n")
                                        {
                                            serialPortE.Write(reverse + "\n"); //發送數據 Rs232 + \n
                                            DateTime dt = DateTime.Now;
                                            string text = "[" + dt.ToString("yyyy/MM/dd HH:mm:ss") + "]  " + reverse + "\n\r";
                                            textBox_serial.AppendText(text);
                                            log5_text = string.Concat(log5_text, text);
                                        }
                                        else if (reverse != "" && columns_switch == @"\n\r")
                                        {
                                            serialPortE.Write(reverse + "\n\r"); //發送數據 Rs232 + \n\r
                                            DateTime dt = DateTime.Now;
                                            string text = "[" + dt.ToString("yyyy/MM/dd HH:mm:ss") + "]  " + reverse + "\n\r";
                                            textBox_serial.AppendText(text);
                                            log5_text = string.Concat(log5_text, text);
                                        }
                                        else if (reverse != "" && columns_switch == @"\r\n")
                                        {
                                            serialPortE.Write(reverse + "\r\n"); //發送數據 Rs232 + \r\n
                                            DateTime dt = DateTime.Now;
                                            string text = "[" + dt.ToString("yyyy/MM/dd HH:mm:ss") + "]  " + reverse + "\r\n";
                                            textBox_serial.AppendText(text);
                                            log5_text = string.Concat(log5_text, text);
                                        }
                                        else if (reverse != "" && columns_switch == "")
                                        {
                                            serialPortE.Write(reverse); //發送數據 HEX Rs232
                                            DateTime dt = DateTime.Now;
                                            string text = "[" + dt.ToString("yyyy/MM/dd HH:mm:ss") + "]  " + reverse + "\r\n";
                                            textBox_serial.AppendText(text);
                                            log5_text = string.Concat(log5_text, text);
                                        }
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

                        #region -- MonkeyTest --
                        else if (columns_command == "_MonkeyTest")
                        {
                            Console.WriteLine("Android control: _MonkeyTest");
                            Add_ons MonkeyTest = new Add_ons();
                            MonkeyTest.MonkeyTest();
                            MonkeyTest.CreateExcelFile();
                        }
                        #endregion
                        /*
                                                #region -- Factory Command 控制 --
                                                else if (columns_command == "_SXP")
                                                {
                                                    if (ini12.INIRead(MainSettingPath, "SerialPortB", "Checked", "") == "1" &&
                                                        columns_serial == "_save")
                                                    {
                                                        string fName = "";

                                                        fName = ini12.INIRead(MainSettingPath, "Record", "LogPath", "");
                                                        string t = fName + "\\_Log2_" + DateTime.Now.ToString("yyyyMMddHHmmss") + "_" + label_LoopNumber_Value.Text + ".txt";

                                                        StreamWriter MYFILE = new StreamWriter(t, false, Encoding.ASCII);
                                                        MYFILE.WriteLine(textBox2.Text);
                                                        MYFILE.Close();

                                                        Txtbox2("", textBox2);
                                                    }

                                                    if (ini12.INIRead(MainSettingPath, "SerialPortB", "Checked", "") == "1" &&
                                                        columns_serial != "_save")
                                                    {
                                                        try
                                                        {
                                                            string str = columns_serial;
                                                            byte[] bytes = str.Split(' ').Select(s => Convert.ToByte(s, 16)).ToArray();
                                                            label_Command.Text = "(SXP CMD)" + columns_serial;
                                                            serialPort2.Write(bytes, 0, bytes.Length);
                                                            label_Command.Text = "(" + columns_command + ") " + columns_serial;
                                                            // DateTime dt = DateTime.Now;
                                                            // string text = "[" + dt.ToString("yyyy/MM/dd HH:mm:ss") + "]  " + columns_serial + "\r\n";
                                                            str = str.Replace(" ", "");
                                                            string text = str + "\r\n";
                                                            textBox2.AppendText(text);
                                                        }
                                                        catch (Exception ex)
                                                        {
                                                            Console.WriteLine(ex);
                                                        }
                                                    }
                                                    else
                                                    {
                                                        MessageBox.Show("Check your SerialPort2 setting.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Question);
                                                        Global.Break_Out_Schedule = 1;
                                                    }
                                                }
                                                #endregion
                        */
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
                        /*
                                                #region -- NI IO Input --
                                                else if (columns_command.Length >= 13 && columns_command.Substring(0, 11) == "_EXT_Input_")
                                                {
                                                    switch (columns_command.Substring(11, 2))
                                                    {
                                                        case "P0":
                                                            try
                                                            {
                                                                using (Task digitalWriteTask = new Task())
                                                                {
                                                                    //  Create an Digital Output channel and name it.
                                                                    digitalWriteTask.DOChannels.CreateChannel(DaqSystem.Local.GetPhysicalChannels(PhysicalChannelTypes.DOPort, PhysicalChannelAccess.External)[0].ToString(), "port0",
                                                                        ChannelLineGrouping.OneChannelForAllLines);

                                                                    //  Write digital port data. WriteDigitalSingChanSingSampPort writes a single sample
                                                                    //  of digital data on demand, so no timeout is necessary.
                                                                    DigitalSingleChannelWriter writer = new DigitalSingleChannelWriter(digitalWriteTask.Stream);
                                                                    writer.WriteSingleSamplePort(true, (UInt32)Convert.ToUInt32(columns_times));
                                                                }
                                                            }
                                                            catch (Exception ex)
                                                            {
                                                                MessageBox.Show(ex.Message);
                                                            }
                                                            break;
                                                        case "P1":
                                                            try
                                                            {
                                                                using (Task digitalWriteTask = new Task())
                                                                {
                                                                    //  Create an Digital Output channel and name it.
                                                                    digitalWriteTask.DOChannels.CreateChannel(DaqSystem.Local.GetPhysicalChannels(PhysicalChannelTypes.DOPort, PhysicalChannelAccess.External)[1].ToString(), "port0",
                                                                        ChannelLineGrouping.OneChannelForAllLines);

                                                                    //  Write digital port data. WriteDigitalSingChanSingSampPort writes a single sample
                                                                    //  of digital data on demand, so no timeout is necessary.
                                                                    DigitalSingleChannelWriter writer = new DigitalSingleChannelWriter(digitalWriteTask.Stream);
                                                                    writer.WriteSingleSamplePort(true, (UInt32)Convert.ToUInt32(columns_times));
                                                                }
                                                            }
                                                            catch (Exception ex)
                                                            {
                                                                MessageBox.Show(ex.Message);
                                                            }
                                                            break;
                                                        case "P2":
                                                            try
                                                            {
                                                                using (Task digitalWriteTask = new Task())
                                                                {
                                                                    //  Create an Digital Output channel and name it.
                                                                    digitalWriteTask.DOChannels.CreateChannel(DaqSystem.Local.GetPhysicalChannels(PhysicalChannelTypes.DOPort, PhysicalChannelAccess.External)[2].ToString(), "port0",
                                                                        ChannelLineGrouping.OneChannelForAllLines);

                                                                    //  Write digital port data. WriteDigitalSingChanSingSampPort writes a single sample
                                                                    //  of digital data on demand, so no timeout is necessary.
                                                                    DigitalSingleChannelWriter writer = new DigitalSingleChannelWriter(digitalWriteTask.Stream);
                                                                    writer.WriteSingleSamplePort(true, (UInt32)Convert.ToUInt32(columns_times));
                                                                }
                                                            }
                                                            catch (Exception ex)
                                                            {
                                                                MessageBox.Show(ex.Message);
                                                            }
                                                            break;
                                                    }
                                                    label_Command.Text = "(" + columns_command + ") " + columns_times;
                                                }
                                                #endregion

                                                #region -- NI IO Output --
                                                else if (columns_command.Length >= 14 && columns_command.Substring(0, 12) == "_EXT_Output_")
                                                {
                                                    switch (columns_command.Substring(12, 2))
                                                    {
                                                        case "P0":
                                                            try
                                                            {
                                                                using (Task digitalWriteTask = new Task())
                                                                {
                                                                    //  Create an Digital Output channel and name it.
                                                                    digitalWriteTask.DOChannels.CreateChannel(DaqSystem.Local.GetPhysicalChannels(PhysicalChannelTypes.DOPort, PhysicalChannelAccess.External)[0].ToString(), "port0",
                                                                        ChannelLineGrouping.OneChannelForAllLines);

                                                                    //  Write digital port data. WriteDigitalSingChanSingSampPort writes a single sample
                                                                    //  of digital data on demand, so no timeout is necessary.
                                                                    DigitalSingleChannelWriter writer = new DigitalSingleChannelWriter(digitalWriteTask.Stream);
                                                                    writer.WriteSingleSamplePort(true, (UInt32)Convert.ToUInt32(columns_times));
                                                                }
                                                            }
                                                            catch (Exception ex)
                                                            {
                                                                MessageBox.Show(ex.Message);
                                                            }
                                                            break;
                                                        case "P1":
                                                            try
                                                            {
                                                                using (Task digitalWriteTask = new Task())
                                                                {
                                                                    //  Create an Digital Output channel and name it.
                                                                    digitalWriteTask.DOChannels.CreateChannel(DaqSystem.Local.GetPhysicalChannels(PhysicalChannelTypes.DOPort, PhysicalChannelAccess.External)[1].ToString(), "port0",
                                                                        ChannelLineGrouping.OneChannelForAllLines);

                                                                    //  Write digital port data. WriteDigitalSingChanSingSampPort writes a single sample
                                                                    //  of digital data on demand, so no timeout is necessary.
                                                                    DigitalSingleChannelWriter writer = new DigitalSingleChannelWriter(digitalWriteTask.Stream);
                                                                    writer.WriteSingleSamplePort(true, (UInt32)Convert.ToUInt32(columns_times));
                                                                }
                                                            }
                                                            catch (Exception ex)
                                                            {
                                                                MessageBox.Show(ex.Message);
                                                            }
                                                            break;
                                                        case "P2":
                                                            try
                                                            {
                                                                using (Task digitalWriteTask = new Task())
                                                                {
                                                                    //  Create an Digital Output channel and name it.
                                                                    digitalWriteTask.DOChannels.CreateChannel(DaqSystem.Local.GetPhysicalChannels(PhysicalChannelTypes.DOPort, PhysicalChannelAccess.External)[2].ToString(), "port0",
                                                                        ChannelLineGrouping.OneChannelForAllLines);

                                                                    //  Write digital port data. WriteDigitalSingChanSingSampPort writes a single sample
                                                                    //  of digital data on demand, so no timeout is necessary.
                                                                    DigitalSingleChannelWriter writer = new DigitalSingleChannelWriter(digitalWriteTask.Stream);
                                                                    writer.WriteSingleSamplePort(true, (UInt32)Convert.ToUInt32(columns_times));
                                                                }
                                                            }
                                                            catch (Exception ex)
                                                            {
                                                                MessageBox.Show(ex.Message);
                                                            }
                                                            break;
                                                    }
                                                    label_Command.Text = "(" + columns_command + ") " + columns_times;
                                                }
                                                #endregion
                        */
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
                            if (ini12.INIRead(MainSettingPath, "SerialPortA", "Checked", "") == "1")
                            {
                                string pwm_output;
                                int result = 0;
                                if (columns_serial == "off")
                                {
                                    pwm_output = "set pwm_output 0";
                                    serialPortA.WriteLine(pwm_output);
                                }
                                else if (columns_serial == "on")
                                {
                                    pwm_output = "set pwm_output 1";
                                    serialPortA.WriteLine(pwm_output);
                                }
                                else if (int.TryParse(columns_serial, out result) == true)
                                {
                                    if (int.Parse(columns_serial) >= 0 && int.Parse(columns_serial) <= 100)
                                    {
                                        pwm_output = "set pwm_percent " + columns_serial;
                                        serialPortA.WriteLine(pwm_output);
                                    }
                                }
                                else
                                {
                                    pwm_output = columns_serial;
                                    serialPortA.WriteLine(pwm_output);
                                }
                            }
                        }
                        #endregion

                        #region -- PWM2 --
                        else if (columns_command == "_pwm2")
                        {
                            Console.WriteLine("PWM Control: _pwm2");
                            if (ini12.INIRead(MainSettingPath, "SerialPortB", "Checked", "") == "1")
                            {
                                string pwm_output;
                                int result = 0;
                                if (columns_serial == "off")
                                {
                                    pwm_output = "set pwm_output 0";
                                    serialPortB.WriteLine(pwm_output);
                                }
                                else if (columns_serial == "on")
                                {
                                    pwm_output = "set pwm_output 1";
                                    serialPortB.WriteLine(pwm_output);
                                }
                                else if (int.TryParse(columns_serial, out result) == true)
                                {
                                    if (int.Parse(columns_serial) >= 0 && int.Parse(columns_serial) <= 100)
                                    {
                                        pwm_output = "set pwm_percent " + columns_serial;
                                        serialPortB.WriteLine(pwm_output);
                                    }
                                }
                                else
                                {
                                    pwm_output = columns_serial;
                                    serialPortB.WriteLine(pwm_output);
                                }
                            }
                        }
                        #endregion

                        #region -- PWM3 --
                        else if (columns_command == "_pwm3")
                        {
                            Console.WriteLine("PWM Control: _pwm3");
                            if (ini12.INIRead(MainSettingPath, "SerialPortC", "Checked", "") == "1")
                            {
                                string pwm_output;
                                int result = 0;
                                if (columns_serial == "off")
                                {
                                    pwm_output = "set pwm_output 0";
                                    serialPortB.WriteLine(pwm_output);
                                }
                                else if (columns_serial == "on")
                                {
                                    pwm_output = "set pwm_output 1";
                                    serialPortB.WriteLine(pwm_output);
                                }
                                else if (int.TryParse(columns_serial, out result) == true)
                                {
                                    if (int.Parse(columns_serial) >= 0 && int.Parse(columns_serial) <= 100)
                                    {
                                        pwm_output = "set pwm_percent " + columns_serial;
                                        serialPortB.WriteLine(pwm_output);
                                    }
                                }
                                else
                                {
                                    pwm_output = columns_serial;
                                    serialPortB.WriteLine(pwm_output);
                                }
                            }
                        }
                        #endregion

                        #region -- 遙控器指令 --
                        else
                        {
                            Console.WriteLine("Remote Control: TV_rc_key");
                            for (int k = 0; k < stime; k++)
                            {
                                label_Command.Text = columns_command;
                                if (ini12.INIRead(MainSettingPath, "Device", "RedRatExist", "") == "1")
                                {
                                    //執行小紅鼠指令
                                    Autocommand_RedRat("Form1", columns_command);
                                }
                                else if (ini12.INIRead(MainSettingPath, "Device", "AutoboxExist", "") == "1")
                                {
                                    //執行小藍鼠指令
                                    Autocommand_BlueRat("Form1", columns_command);
                                }
                                else
                                {
                                    MessageBox.Show("Please connect AutoBox or RedRat!", "Redrat Open Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    button_Start.PerformClick();
                                }
                                videostring = columns_command;
                                RedRatDBViewer_Delay(sRepeat);
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
                        Console.WriteLine("CloseTime record.");
                        ini12.INIWrite(MailPath, "Data Info", "CloseTime", string.Format("{0:R}", DateTime.Now));


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

                        #region -- 足跡模式 --
                        //假如足跡模式打開則會append足跡上去
                        if (ini12.INIRead(MainSettingPath, "Record", "Footprint Mode", "") == "1" && SysDelay != 0)
                        {
                            Console.WriteLine("Footprint Mode.");
                            //檔案不存在則加入標題
                            if (File.Exists(Application.StartupPath + @"\StepRecord.csv") == false)
                            {
                                File.AppendAllText(Application.StartupPath + @"\StepRecord.csv", "LOOP,TIME,COMMAND,bit_0,bit_1,bit_2,bit_3,bit_4,bit_5," +
                                    "PA10_0,PA10_1," +
                                    "PA11_0,PA11_1," +
                                    "PA14_0,PA14_1," +
                                    "PA15_0,PA15_1," +
                                    "PB1_0,PB1_1," +
                                    "PB7_0,PB7_1" +
                                    Environment.NewLine);

                                File.AppendAllText(Application.StartupPath + @"\StepRecord.csv",
                                Global.Loop_Number + "," + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "," + label_Command.Text + "," + Global.IO_INPUT +
                                "," + Global.IO_PA10_0_COUNT + "," + Global.IO_PA10_1_COUNT +
                                "," + Global.IO_PA11_0_COUNT + "," + Global.IO_PA11_1_COUNT +
                                "," + Global.IO_PA14_0_COUNT + "," + Global.IO_PA14_1_COUNT +
                                "," + Global.IO_PA15_0_COUNT + "," + Global.IO_PA15_1_COUNT +
                                "," + Global.IO_PB1_0_COUNT + "," + Global.IO_PB1_1_COUNT +
                                "," + Global.IO_PB7_0_COUNT + "," + Global.IO_PB7_1_COUNT + Environment.NewLine);
                            }
                            else
                            {
                                File.AppendAllText(Application.StartupPath + @"\StepRecord.csv",
                                Global.Loop_Number + "," + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "," + label_Command.Text + "," + Global.IO_INPUT +
                                "," + Global.IO_PA10_0_COUNT + "," + Global.IO_PA10_1_COUNT +
                                "," + Global.IO_PA11_0_COUNT + "," + Global.IO_PA11_1_COUNT +
                                "," + Global.IO_PA14_0_COUNT + "," + Global.IO_PA14_1_COUNT +
                                "," + Global.IO_PA15_0_COUNT + "," + Global.IO_PA15_1_COUNT +
                                "," + Global.IO_PB1_0_COUNT + "," + Global.IO_PB1_1_COUNT +
                                "," + Global.IO_PB7_0_COUNT + "," + Global.IO_PB7_1_COUNT + Environment.NewLine);
                            }
                        }
                        #endregion
                        Console.WriteLine("End.");
                    }

                    #region -- Import database --
                    if (ini12.INIRead(MainSettingPath, "Record", "ImportDB", "") == "1")
                    {
                        string SQLServerURL = "server=192.168.56.2\\ATMS;database=Autobox;uid=AS;pwd=AS";

                        SqlConnection conn = new SqlConnection(SQLServerURL);
                        conn.Open();
                        SqlCommand s_com = new SqlCommand
                        {
                            //s_com.CommandText = "select * from Autobox.dbo.testresult";
                            CommandText = "insert into Autobox.dbo.testresult (ab_p_id, ab_result, ab_create, ab_time, ab_loop, ab_loop_time, ab_loop_step, ab_root, ab_user) values ('" + label_LoopNumber_Value.Text + "', 'Pass', '" + DateTime.Now.ToString("HH:mm:ss") + "', '" + label_LoopNumber_Value.Text + "', 1, 21000, 2, 0, 'Joseph')",
                            //s_com.CommandText = "update Autobox.dbo.testresult (ab_result, ab_close, ab_time, ab_loop, ab_root, ab_user) values ('Pass', '" + DateTime.Now.ToString("HH:mm:ss") + "', '" + label1.Text + "', 1, 21000, 'Joseph')";
                            //s_com.CommandText = "Update Autobox.dbo.testresult set ab_result='Pass', ab_close='2014/5/21 15:49:35', ab_time=600000, ab_loop=25, ab_root=0 where ab_num=2";
                            //s_com.CommandText = "Update Autobox.dbo.testresult set ab_result='NG', ab_close='2014/5/21 15:59:35', ab_time=1200000, ab_loop=50, ab_root=1 where ab_num=3";

                            Connection = conn
                        };

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

            /*
            #region Excel function
            if (ini12.INIRead(sPath, "Record", "CompareChoose", "") == "1" && excelstat == true)
            {
                string excelFile = ini12.INIRead(sPath, "Record", "ComparePath", "") + "\\SimilarityReport_" + Global.Schedule_Num;

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
                if (StartBtnShow_STOP == false)
                    Global.loop_Num--;
                Thread MyCompareThread = new Thread(new ThreadStart(MyCompareCamd));
                MyCompareThread.Start();
                RedratLable.Text = "Start Compare Picture...";
                Thread.Sleep(Global.loop_Num * Global.caption_Sum * 2000);
            }
            */

            #region -- schedule 切換 --
            if (StartButtonPressed != false)
            {
                if (Global.Schedule_2_Exist == 1 && Global.Schedule_Number == 1)
                {
                    if (ini12.INIRead(MainSettingPath, "Schedule2", "OnTimeStart", "") == "1" && StartButtonPressed == true)       //定時器時間未到進入等待<<<<<<<<<<<<<<
                    {
                        if (Global.Break_Out_Schedule == 0)
                        {
                            while (ini12.INIRead(MainSettingPath, "Schedule2", "Timer", "") != TimeLabel2.Text)
                            {
                                ScheduleWait.WaitOne();
                            }
                            ScheduleWait.Set();
                        }
                    }       //>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
                    ini12.INIWrite(MainSettingPath, "Schedule1", "OnTimeStart", "0");
                    button_Schedule2.PerformClick();
                    MyRunCamd();
                }
                else if (
                    Global.Schedule_3_Exist == 1 && Global.Schedule_Number == 1 ||
                    Global.Schedule_3_Exist == 1 && Global.Schedule_Number == 2)
                {
                    if (ini12.INIRead(MainSettingPath, "Schedule3", "OnTimeStart", "") == "1" && StartButtonPressed == true)
                    {
                        if (Global.Break_Out_Schedule == 0)
                        {
                            while (ini12.INIRead(MainSettingPath, "Schedule3", "Timer", "") != TimeLabel2.Text)
                            {
                                ScheduleWait.WaitOne();
                            }
                            ScheduleWait.Set();
                        }
                    }
                    ini12.INIWrite(MainSettingPath, "Schedule2", "OnTimeStart", "0");
                    button_Schedule3.PerformClick();
                    MyRunCamd();
                }
                else if (
                    Global.Schedule_4_Exist == 1 && Global.Schedule_Number == 1 ||
                    Global.Schedule_4_Exist == 1 && Global.Schedule_Number == 2 ||
                    Global.Schedule_4_Exist == 1 && Global.Schedule_Number == 3)
                {
                    if (ini12.INIRead(MainSettingPath, "Schedule4", "OnTimeStart", "") == "1" && StartButtonPressed == true)
                    {
                        if (Global.Break_Out_Schedule == 0)
                        {
                            while (ini12.INIRead(MainSettingPath, "Schedule4", "Timer", "") != TimeLabel2.Text)
                            {
                                ScheduleWait.WaitOne();
                            }
                            ScheduleWait.Set();
                        }
                    }
                    ini12.INIWrite(MainSettingPath, "Schedule3", "OnTimeStart", "0");
                    button_Schedule4.PerformClick();
                    MyRunCamd();
                }
                else if (
                    Global.Schedule_5_Exist == 1 && Global.Schedule_Number == 1 ||
                    Global.Schedule_5_Exist == 1 && Global.Schedule_Number == 2 ||
                    Global.Schedule_5_Exist == 1 && Global.Schedule_Number == 3 ||
                    Global.Schedule_5_Exist == 1 && Global.Schedule_Number == 4)
                {
                    if (ini12.INIRead(MainSettingPath, "Schedule5", "OnTimeStart", "") == "1" && StartButtonPressed == true)
                    {
                        if (Global.Break_Out_Schedule == 0)
                        {
                            while (ini12.INIRead(MainSettingPath, "Schedule5", "Timer", "") != TimeLabel2.Text)
                            {
                                ScheduleWait.WaitOne();
                            }
                            ScheduleWait.Set();
                        }
                    }
                    ini12.INIWrite(MainSettingPath, "Schedule4", "OnTimeStart", "0");
                    button_Schedule5.PerformClick();
                    MyRunCamd();
                }
            }
            #endregion

            //全部schedule跑完或是按下stop鍵以後會跑以下這段/////////////////////////////////////////
            if (StartButtonPressed == false)//按下STOP讓schedule結束//
            {
                Global.Break_Out_MyRunCamd = 1;
                ini12.INIWrite(MailPath, "Data Info", "CloseTime", string.Format("{0:R}", DateTime.Now));
                UpdateUI("START", button_Start);
                button_Start.Enabled = true;
                button_Setting.Enabled = true;
                button_Pause.Enabled = false;
                setStyle();
                button_SaveSchedule.Enabled = true;

                if (ini12.INIRead(MainSettingPath, "Device", "CameraExist", "") == "1")
                {
                    _captureInProgress = false;
                    OnOffCamera();
                    //button_VirtualRC.Enabled = true;
                }

                /*
                if (Directory.Exists(ini12.INIRead(sPath, "Record", "VideoPath", "") + "\\" + "Schedule" + Global.Schedule_Num + "_Original") == true)
                {
                    DirectoryInfo DIFO = new DirectoryInfo(ini12.INIRead(sPath, "Record", "VideoPath", "") + "\\" + "Schedule" + Global.Schedule_Num + "_Original");
                    DIFO.Delete(true);
                }
                */
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
                if (ini12.INIRead(MailPath, "Send Mail", "value", "") == "1")
                {
                    Global.Loop_Number = Global.Loop_Number - 1;
                    FormMail FormMail = new FormMail();
                    FormMail.send();
                }
            }

            label_Command.Text = "Completed!";
            label_Remark.Text = "";
            ini12.INIWrite(MainSettingPath, "Schedule" + Global.Schedule_Number, "OnTimeStart", "0");
            button_Schedule1.PerformClick();
            timer1.Stop();
            CloseDtplay();
            timeCount = Global.Schedule_1_TestTime;
            ConvertToRealTime(timeCount);
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
            else if (columns_serial == "_mail")
            {
                if (ini12.INIRead(MailPath, "Send Mail", "value", "") == "1")
                {
                    Global.Pass_Or_Fail = "NG";
                    FormMail FormMail = new FormMail();
                    FormMail.send();
                    label_Command.Text = "IO CMD_MAIL";
                }
                else
                {
                    MessageBox.Show("Please enable Mail Function in Settings.");
                }
            }
            else if (columns_serial.Substring(0, 3) == "_rc")
            {
                String rc_key = columns_serial;
                int startIndex = 4;
                int length = rc_key.Length - 4;
                String rc_key_substring = rc_key.Substring(startIndex, length);

                if (ini12.INIRead(MainSettingPath, "Device", "RedRatExist", "") == "1")
                {
                    Autocommand_RedRat("Form1", rc_key_substring);
                    label_Command.Text = rc_key_substring;
                }
                else if (ini12.INIRead(MainSettingPath, "Device", "AutoboxExist", "") == "1")
                {
                    Autocommand_BlueRat("Form1", rc_key_substring);
                    label_Command.Text = rc_key_substring;
                }
            }
            else if (columns_serial.Substring(0, 7) == "_logcmd")
            {
                String log_cmd = columns_serial;
                int startIndex = 8;
                int length = log_cmd.Length - 8;
                String log_cmd_substring = log_cmd.Substring(startIndex, length);

                if (ini12.INIRead(MainSettingPath, "SerialPortA", "Checked", "") == "1")
                {
                    serialPortA.WriteLine(log_cmd_substring);
                }
                else if (ini12.INIRead(MainSettingPath, "SerialPortB", "Checked", "") == "1")
                {
                    serialPortB.WriteLine(log_cmd_substring);
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
            else if (columns_serial == "_mail")
            {
                if (ini12.INIRead(MailPath, "Send Mail", "value", "") == "1")
                {
                    Global.Pass_Or_Fail = "NG";
                    FormMail FormMail = new FormMail();
                    FormMail.send();
                    label_Command.Text = "KEYWORD_MAIL";
                }
                else
                {
                    MessageBox.Show("Please enable Mail Function in Settings.");
                }
            }
            else if (columns_serial == "_savelog1")
            {
                string fName = "";

                fName = ini12.INIRead(MainSettingPath, "Record", "LogPath", "");
                string t = fName + "\\_SaveLog1_" + DateTime.Now.ToString("yyyyMMddHHmmss") + "_" + label_LoopNumber_Value.Text + ".txt";

                StreamWriter MYFILE = new StreamWriter(t, false, Encoding.ASCII);
                MYFILE.Write(log1_text);
                MYFILE.Close();
                Txtbox1("", textBox_serial);
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
                Txtbox2("", textBox_serial);
                label_Command.Text = "KEYWORD_SAVELOG2";
            }
            else if (columns_serial.Substring(0, 3) == "_rc")
            {
                String rc_key = columns_serial;
                int startIndex = 4;
                int length = rc_key.Length - 4;
                String rc_key_substring = rc_key.Substring(startIndex, length);

                if (ini12.INIRead(MainSettingPath, "Device", "RedRatExist", "") == "1")
                {
                    Autocommand_RedRat("Form1", rc_key_substring);
                    label_Command.Text = rc_key_substring;
                }
                else if (ini12.INIRead(MainSettingPath, "Device", "AutoboxExist", "") == "1")
                {
                    Autocommand_BlueRat("Form1", rc_key_substring);
                    label_Command.Text = rc_key_substring;
                }
            }
            else if (columns_serial.Substring(0, 7) == "_logcmd")
            {
                String log_cmd = columns_serial;
                int startIndex = 8;
                int length = log_cmd.Length - 8;
                String log_cmd_substring = log_cmd.Substring(startIndex, length);

                if (ini12.INIRead(MainSettingPath, "SerialPortA", "Checked", "") == "1")
                {
                    serialPortA.WriteLine(log_cmd_substring);
                }
                else if (ini12.INIRead(MainSettingPath, "SerialPortB", "Checked", "") == "1")
                {
                    serialPortB.WriteLine(log_cmd_substring);
                }
            }
        }
        #endregion

        #region -- 圖片比對 --
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
            //string ngPath = ini12.INIRead(sPath, "Record", "VideoPath", "") + "\\" + "Schedule" + Global.Schedule_Num + "_NG\\";
            string comparePath = ini12.INIRead(MainSettingPath, "Record", "ComparePath", "") + "\\";
            string csvFile = comparePath + "SimilarityReport_" + Global.Schedule_Number + ".csv";

            //Console.WriteLine("Loop Number: " + Global.loop_Num);

            // 讀取ini中的路徑
            //fNameNG = ini12.INIRead(sPath, "Record", "VideoPath", "") + "\\" + "Schedule" + Global.Schedule_Num + "_NG\\";

            string pathCompare1 = comparePath + "cf-" + Global.Loop_Number + "_" + Global.caption_Num + ".png";
            string pathCompare2 = comparePath + "cf-" + (Global.Loop_Number - 1) + "_" + Global.caption_Num + ".png";
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
                int differenceNum = Convert.ToInt32(ini12.INIRead(MainSettingPath, "Record", "CompareDifferent", ""));
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
                    Global.NGRateValue[Global.caption_Num] = (float)Global.NGValue[Global.caption_Num] / (Global.Loop_Number - 1);

                    /*
                                        string[] FileList = System.IO.Directory.GetFiles(fNameAll, "cf-" + Global.loop_Num + "_" + Global.caption_Num + ".png");
                                        foreach (string File in FileList)
                                        {
                                            System.IO.FileInfo fi = new System.IO.FileInfo(File);
                                            fi.CopyTo(fNameNG + fi.Name);
                                        }
                    */

                    Global.NGRateValue[Global.caption_Num] = (float)Global.NGValue[Global.caption_Num] / (Global.Loop_Number - 1);

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

                    sw.Write("=hyperlink(\"cf-" + (Global.Loop_Number - 1) + "_" + Global.caption_Num + ".png\"，\"" + (Global.Loop_Number - 1) + "-" + Global.caption_Num + "\")" + ",");
                    sw.Write("=hyperlink(\"cf-" + (Global.Loop_Number) + "_" + Global.caption_Num + ".png\"，\"" + (Global.Loop_Number) + "-" + Global.caption_Num + "\")" + ",");
                    sw.Write(differencePercent + ",");
                    sw.Write(Global.NGValue[Global.caption_Num] + ",");
                    sw.Write(Global.NGRateValue[Global.caption_Num] + ",");
                    sw.WriteLine("NG");
                }
                else
                {
                    Global.NGRateValue[Global.caption_Num] = (float)Global.NGValue[Global.caption_Num] / (Global.Loop_Number - 1);

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

                    sw.Write("=hyperlink(\"cf-" + (Global.Loop_Number - 1) + "_" + Global.caption_Num + ".png\"，\"" + (Global.Loop_Number - 1) + "-" + Global.caption_Num + "\")" + ",");
                    sw.Write("=hyperlink(\"cf-" + (Global.Loop_Number) + "_" + Global.caption_Num + ".png\"，\"" + (Global.Loop_Number) + "-" + Global.caption_Num + "\")" + ",");
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
                    Console.Write(ex.Message.ToString(), "Webcam doed not support 2304*1296!\n\r");
                    try
                    {
                        capture.FrameSize = new Size(1920, 1080);
                        ini12.INIWrite(MainSettingPath, "Camera", "Resolution", "1920*1080");
                    }
                    catch (Exception ex1)
                    {
                        Console.Write(ex1.Message.ToString(), "Webcam can't supported the 1920*1080 resolution!\n\r");
                        try
                        {
                            capture.FrameSize = new Size(1280, 720);
                            ini12.INIWrite(MainSettingPath, "Camera", "Resolution", "1280*720");
                        }
                        catch (Exception ex2)
                        {
                            Console.Write(ex2.Message.ToString(), "Webcam can't supported the 1280*720 resolution!\n\r");
                            try
                            {
                                capture.FrameSize = new Size(640, 480);
                                ini12.INIWrite(MainSettingPath, "Camera", "Resolution", "640*480");
                            }
                            catch (Exception ex3)
                            {
                                Console.Write(ex3.Message.ToString(), "Webcam can't supported the 640*480 resolution!\n\r");
                                try
                                {
                                    capture.FrameSize = new Size(320, 240);
                                    ini12.INIWrite(MainSettingPath, "Camera", "Resolution", "320*240");
                                }
                                catch (Exception ex4)
                                {
                                    Console.Write(ex4.Message.ToString(), "Webcam can't supported the 320*240 resolution!\n\r");
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
                    Console.Write(ex.Message.ToString(), "Please setting the supported resolution!\n\r");
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
            RedRatData.RedRatLoadSignalDB(ini12.INIRead(MainSettingPath, "RedRat", "DBFile", ""));
            RedRatData.RedRatSelectDevice(ini12.INIRead(MainSettingPath, "RedRat", "Brands", ""));

            DataGridViewComboBoxColumn RCDB = (DataGridViewComboBoxColumn)DataGridView_Schedule.Columns[0];

            string devicename = ini12.INIRead(MainSettingPath, "RedRat", "Brands", "");
            if (RedRatData.RedRatSelectDevice(devicename))
            {
                RCDB.Items.AddRange(RedRatData.RedRatGetRCNameList().ToArray());
                Global.Rc_List = RedRatData.RedRatGetRCNameList();
                Global.Rc_Number = RedRatData.RedRatGetRCNameList().Count;
            }
            else
            {
                Console.WriteLine("Select Device Error: " + devicename);
            }

            RCDB.Items.Add("------------------------");
            RCDB.Items.Add("_HEX");
            RCDB.Items.Add("_ascii");
            RCDB.Items.Add("_FuncKey");
            RCDB.Items.Add("_K_ABS");
            RCDB.Items.Add("_K_OBD");
            RCDB.Items.Add("_K_SEND");
            RCDB.Items.Add("_K_CLEAR");
            RCDB.Items.Add("_Temperature");
            RCDB.Items.Add("------------------------");
            RCDB.Items.Add("_shot");
            RCDB.Items.Add("_rec_start");
            RCDB.Items.Add("_rec_stop");
            RCDB.Items.Add("_cmd");
            RCDB.Items.Add("_DOS");
            RCDB.Items.Add("------------------------");
            RCDB.Items.Add("_WaterTemp");
            RCDB.Items.Add("_FuelDisplay");
            RCDB.Items.Add("_IO_Output");
            RCDB.Items.Add("_IO_Input");
            RCDB.Items.Add("_audio_debounce");
            RCDB.Items.Add("_Pin");
            RCDB.Items.Add("_keyword");
            RCDB.Items.Add("------------------------");
            RCDB.Items.Add("_quantum");
            RCDB.Items.Add("_astro");
            RCDB.Items.Add("_dektec");
            //RCDB.Items.Add("------------------------");
            //RCDB.Items.Add("_SXP");
            //RCDB.Items.Add("_log1");
            //RCDB.Items.Add("_log2");
            //RCDB.Items.Add("_log3");
            //RCDB.Items.Add("_pwm1");
            //RCDB.Items.Add("_pwm2");
            //RCDB.Items.Add("_pwm3");
            //RCDB.Items.Add("------------------------");
            //RCDB.Items.Add("_EXT_Output_P0");
            //RCDB.Items.Add("_EXT_Output_P1");
            //RCDB.Items.Add("_EXT_Output_P2");
            //RCDB.Items.Add("_EXT_Input_P0");
            //RCDB.Items.Add("_EXT_Input_P1");
            //RCDB.Items.Add("_EXT_Input_P2");
            //RCDB.Items.Add("------------------------");
            //RCDB.Items.Add("_MonkeyTest");
        }
        #endregion

        #region -- 讀取RC DB並填入Virtual RC Panel --
        Button[] Buttons;
        private void LoadVirtualRC()
        {
            //根據dpi調整按鍵寬度//
            Graphics graphics = CreateGraphics();
            float dpiX = graphics.DpiX;
            float dpiY = graphics.DpiY;
            int width, height;
            if (dpiX == 96 && dpiY == 96)
            {
                width = 75;
                height = 25;
            }
            else
            {
                width = 90;
                height = 25;
            }

            Buttons = new Button[Global.Rc_Number];

            for (int i = 0; i < Buttons.Length; i++)
            {
                Buttons[i] = new Button
                {
                    Size = new Size(width, height),
                    Text = Global.Rc_List[i],
                    AutoSize = false,
                    AutoSizeMode = AutoSizeMode.GrowAndShrink
                };

                if (i <= 11)
                {
                    Buttons[i].Location = new Point(0 + (i * width), 5);
                }
                else if (i > 11 && i <= 23)
                {
                    Buttons[i].Location = new Point(0 + ((i - 12) * width), 45);
                }
                else if (i > 23 && i <= 35)
                {
                    Buttons[i].Location = new Point(0 + ((i - 24) * width), 85);
                }
                else if (i > 35 && i <= 47)
                {
                    Buttons[i].Location = new Point(0 + ((i - 36) * width), 125);
                }
                else if (i > 47 && i <= 59)
                {
                    Buttons[i].Location = new Point(0 + ((i - 48) * width), 165);
                }
                else if (i > 59 && i <= 71)
                {
                    Buttons[i].Location = new Point(0 + ((i - 60) * width), 205);
                }
                else if (i > 71 && i <= 83)
                {
                    Buttons[i].Location = new Point(0 + ((i - 72) * width), 245);
                }
                else if (i > 83 && i <= 95)
                {
                    Buttons[i].Location = new Point(0 + ((i - 84) * width), 285);
                }

                int index = i;
                Buttons[i].Click += (sender1, ex) => Sand_Key(index + 1);
                panel_VirtualRC.Controls.Add(Buttons[i]);
            }
        }
        #endregion

        private void Sand_Key(int i)
        {
            if (ini12.INIRead(MainSettingPath, "Device", "AutoboxExist", "") == "1")
            {
                if (ini12.INIRead(MainSettingPath, "Device", "RedRatExist", "") == "1")
                {
                    Autocommand_RedRat("Form1", Buttons[i - 1].Text);
                }
                else if (ini12.INIRead(MainSettingPath, "Device", "AutoboxVerson", "") == "2")
                {
                    Autocommand_BlueRat("Form1", Buttons[i - 1].Text);
                }
            }
            else if (ini12.INIRead(MainSettingPath, "Device", "RedRatExist", "") == "1")
            {
                Autocommand_RedRat("Form1", Buttons[i - 1].Text);
            }
        }

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

            if (ini12.INIRead(MainSettingPath, "Python", "Exist", "") == "1")
            {
                Process p = new Process();
                string cmd = ini12.INIRead(MainSettingPath, "Python", "Parameter", "");
                p.StartInfo.UseShellExecute = false;
                p.StartInfo.RedirectStandardOutput = true;
                p.StartInfo.FileName = @"python.exe";
                p.StartInfo.Arguments = cmd;
                p.StartInfo.RedirectStandardInput = true;
                p.Start();
                StreamWriter myStreamWriter = p.StandardInput;
                myStreamWriter.WriteLine(cmd);
                string output = "";
                output = p.StandardOutput.ReadLine();
                Console.WriteLine(output);
                p.WaitForExit();
                p.Close();
            }

            
            if (AutoBox_Status == true)//如果電腦有接上AutoBox//
            {
                button_Schedule1.PerformClick();
                Thread MainThread = new Thread(new ThreadStart(MyRunCamd));
                Thread LogThread1 = new Thread(new ThreadStart(MyLog1Camd));
                Thread LogThread2 = new Thread(new ThreadStart(MyLog2Camd));
                Thread LogThread3 = new Thread(new ThreadStart(MyLog3Camd));
                Thread LogThread4 = new Thread(new ThreadStart(MyLog4Camd));
                Thread LogThread5 = new Thread(new ThreadStart(MyLog5Camd));
                //Thread Log1Data = new Thread(new ThreadStart(Log1_Receiving_Task));
                //Thread Log2Data = new Thread(new ThreadStart(Log2_Receiving_Task));

                if (StartButtonPressed == true)//按下STOP//
                {
                    Global.Break_Out_MyRunCamd = 1;//跳出倒數迴圈//
                    MainThread.Abort();//停止執行緒//
                    timer1.Stop();//停止倒數//
                    CloseDtplay();//關閉DtPlay//

                    if (ini12.INIRead(MainSettingPath, "SerialPortA", "Checked", "") == "1")
                    {
                        if (ini12.INIRead(MainSettingPath, "LogSearch", "TextNum", "") != "0")
                        {
                            LogThread1.Abort();
                            //Log1Data.Abort();
                        }
                    }

                    if (ini12.INIRead(MainSettingPath, "SerialPortB", "Checked", "") == "1")
                    {
                        if (ini12.INIRead(MainSettingPath, "LogSearch", "TextNum", "") != "0")
                        {
                            LogThread2.Abort();
                            //Log2Data.Abort();
                        }
                    }

                    if (ini12.INIRead(MainSettingPath, "SerialPortC", "Checked", "") == "1")
                    {
                        if (ini12.INIRead(MainSettingPath, "LogSearch", "TextNum", "") != "0")
                        {
                            LogThread3.Abort();
                            //Log3Data.Abort();
                        }
                    }

                    if (ini12.INIRead(MainSettingPath, "SerialPortD", "Checked", "") == "1")
                    {
                        if (ini12.INIRead(MainSettingPath, "LogSearch", "TextNum", "") != "0")
                        {
                            LogThread4.Abort();
                            //Log4Data.Abort();
                        }
                    }

                    if (ini12.INIRead(MainSettingPath, "SerialPortE", "Checked", "") == "1")
                    {
                        if (ini12.INIRead(MainSettingPath, "LogSearch", "TextNum", "") != "0")
                        {
                            LogThread5.Abort();
                            //Log4Data.Abort();
                        }
                    }

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

                    ini12.INIWrite(MainSettingPath, "LogSearch", "StartTime", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));
                    MainThread.Start();       // 啟動執行緒
                    timer1.Start();     //開始倒數
                    button_Start.Text = "STOP";

                    StartButtonPressed = true;
                    button_Setting.Enabled = false;
                    button_Pause.Enabled = true;
                    button_SaveSchedule.Enabled = false;

                    if (ini12.INIRead(MainSettingPath, "SerialPortA", "Checked", "") == "1")
                    {
                        OpenSerialPort("A");
                        textBox_serial.Clear();
                        //textBox1.Text = string.Empty;//清空serialport1//
                        if (ini12.INIRead(MainSettingPath, "LogSearch", "TextNum", "") != "0" && ini12.INIRead(MainSettingPath, "LogSearch", "Comport1", "") == "1")
                        {
                            LogThread1.IsBackground = true;
                            LogThread1.Start();
                        }
                    }

                    if (ini12.INIRead(MainSettingPath, "SerialPortB", "Checked", "") == "1")
                    {
                        OpenSerialPort("B");
                        if (ini12.INIRead(MainSettingPath, "LogSearch", "TextNum", "") != "0" && ini12.INIRead(MainSettingPath, "LogSearch", "Comport2", "") == "1")
                        {
                            LogThread2.IsBackground = true;
                            LogThread2.Start();
                        }
                    }

                    if (ini12.INIRead(MainSettingPath, "SerialPortC", "Checked", "") == "1")
                    {
                        OpenSerialPort("C");
                        if (ini12.INIRead(MainSettingPath, "LogSearch", "TextNum", "") != "0" && ini12.INIRead(MainSettingPath, "LogSearch", "Comport3", "") == "1")
                        {
                            LogThread3.IsBackground = true;
                            LogThread3.Start();
                        }
                    }

                    if (ini12.INIRead(MainSettingPath, "SerialPortD", "Checked", "") == "1")
                    {
                        OpenSerialPort("D");
                        if (ini12.INIRead(MainSettingPath, "LogSearch", "TextNum", "") != "0" && ini12.INIRead(MainSettingPath, "LogSearch", "Comport4", "") == "1")
                        {
                            LogThread4.IsBackground = true;
                            LogThread4.Start();
                        }
                    }

                    if (ini12.INIRead(MainSettingPath, "SerialPortE", "Checked", "") == "1")
                    {
                        OpenSerialPort("E");
                        if (ini12.INIRead(MainSettingPath, "LogSearch", "TextNum", "") != "0" && ini12.INIRead(MainSettingPath, "LogSearch", "Comport5", "") == "1")
                        {
                            LogThread5.IsBackground = true;
                            LogThread5.Start();
                        }
                    }

                    if (ini12.INIRead(MainSettingPath, "Kline", "Checked", "") == "1")
                    {
                        OpenSerialPort("kline");
                        textBox_serial.Text = ""; //清空kline//
                    }
                }
            }

            if (AutoBox_Status == false)//如果沒接AutoBox//
            {
                Thread MainThread = new Thread(new ThreadStart(MyRunCamd));

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
                }
            }
        }

        private void SettingBtn_Click(object sender, EventArgs e)
        {
            FormTabControl FormTabControl = new FormTabControl();
            Global.RCDB = ini12.INIRead(MainSettingPath, "RedRat", "Brands", "");

            //如果serialport開著則先關閉//
            if (serialPortA.IsOpen == true)
            {
                CloseSerialPort("A");
            }
            if (serialPortB.IsOpen == true)
            {
                CloseSerialPort("B");
            }
            if (serialPortC.IsOpen == true)
            {
                CloseSerialPort("C");
            }
            if (serialPortD.IsOpen == true)
            {
                CloseSerialPort("D");
            }
            if (serialPortE.IsOpen == true)
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
                if (ini12.INIRead(MainSettingPath, "RedRat", "Brands", "") != Global.RCDB)
                {
                    DataGridViewComboBoxColumn RCDB = (DataGridViewComboBoxColumn)DataGridView_Schedule.Columns[0];
                    RCDB.Items.Clear();
                    LoadRCDB();

                    //panel_VirtualRC.Controls.Clear();
                    //LoadVirtualRC();
                }

                if (ini12.INIRead(MainSettingPath, "Device", "RedRatExist", "") == "1")
                {
                    OpenRedRat3();
                    pictureBox_RedRat.Image = Properties.Resources.ON;
                }
                else
                {
                    pictureBox_RedRat.Image = Properties.Resources.OFF;
                }

                if (ini12.INIRead(MainSettingPath, "Device", "CameraExist", "") == "1")
                {
                    pictureBox_Camera.Image = Properties.Resources.ON;
                    _captureInProgress = false;
                    OnOffCamera();
                    button_VirtualRC.Enabled = true;
                    comboBox_CameraDevice.Enabled = false;
                }
                else
                {
                    pictureBox_Camera.Image = Properties.Resources.OFF;
                }
                /* Hidden serial port.
                button_SerialPort1.Visible = ini12.INIRead(MainSettingPath, "SerialPortA", "Checked", "") == "1" ? true : false;
                button_SerialPort2.Visible = ini12.INIRead(MainSettingPath, "SerialPortB", "Checked", "") == "1" ? true : false;
                button_SerialPort3.Visible = ini12.INIRead(MainSettingPath, "SerialPortC", "Checked", "") == "1" ? true : false;
                button_CanbusPort.Visible = ini12.INIRead(MainSettingPath, "Record", "CANbusLog", "") == "1" ? true : false;
                button_kline.Visible = ini12.INIRead(MainSettingPath, "Kline", "Checked", "") == "1" ? true : false;
                */
                List<string> SchExist = new List<string> { };
                for (int i = 2; i < 6; i++)
                {
                    SchExist.Add(ini12.INIRead(MainSettingPath, "Schedule" + i, "Exist", ""));
                }
                
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


            #region -- schedule timer --
            if (ini12.INIRead(MainSettingPath, "Schedule1", "OnTimeStart", "") == "1")
                labelSch1Timer.Text = "Schedule 1 will start at" + "\r\n" + ini12.INIRead(MainSettingPath, "Schedule1", "Timer", "");
            else if (ini12.INIRead(MainSettingPath, "Schedule1", "OnTimeStart", "") == "0")
                labelSch1Timer.Text = "";

            if (ini12.INIRead(MainSettingPath, "Schedule2", "OnTimeStart", "") == "1")
                labelSch2Timer.Text = "Schedule 2 will start at" + "\r\n" + ini12.INIRead(MainSettingPath, "Schedule2", "Timer", "");
            else if (ini12.INIRead(MainSettingPath, "Schedule2", "OnTimeStart", "") == "0")
                labelSch2Timer.Text = "";

            if (ini12.INIRead(MainSettingPath, "Schedule3", "OnTimeStart", "") == "1")
                labelSch3Timer.Text = "Schedule 3 will start at" + "\r\n" + ini12.INIRead(MainSettingPath, "Schedule3", "Timer", "");
            else if (ini12.INIRead(MainSettingPath, "Schedule3", "OnTimeStart", "") == "0")
                labelSch3Timer.Text = "";

            if (ini12.INIRead(MainSettingPath, "Schedule4", "OnTimeStart", "") == "1")
                labelSch4Timer.Text = "Schedule 4 will start at" + "\r\n" + ini12.INIRead(MainSettingPath, "Schedule4", "Timer", "");
            else if (ini12.INIRead(MainSettingPath, "Schedule4", "OnTimeStart", "") == "0")
                labelSch4Timer.Text = "";

            if (ini12.INIRead(MainSettingPath, "Schedule5", "OnTimeStart", "") == "1")
                labelSch5Timer.Text = "Schedule 5 will start at" + "\r\n" + ini12.INIRead(MainSettingPath, "Schedule5", "Timer", "");
            else if (ini12.INIRead(MainSettingPath, "Schedule5", "OnTimeStart", "") == "0")
                labelSch5Timer.Text = "";

            if (ini12.INIRead(MainSettingPath, "Schedule1", "OnTimeStart", "") == "1" &&
                ini12.INIRead(MainSettingPath, "Schedule1", "Timer", "") == TimeLabel2.Text)
                button_Start.PerformClick();
            if (ini12.INIRead(MainSettingPath, "Schedule2", "OnTimeStart", "") == "1" &&
                ini12.INIRead(MainSettingPath, "Schedule2", "Timer", "") == TimeLabel2.Text &&
                timeCount != 0)
                Global.Break_Out_Schedule = 1;
            if (ini12.INIRead(MainSettingPath, "Schedule3", "OnTimeStart", "") == "1" &&
                ini12.INIRead(MainSettingPath, "Schedule3", "Timer", "") == TimeLabel2.Text &&
                timeCount != 0)
                Global.Break_Out_Schedule = 1;
            if (ini12.INIRead(MainSettingPath, "Schedule4", "OnTimeStart", "") == "1" &&
                ini12.INIRead(MainSettingPath, "Schedule4", "Timer", "") == TimeLabel2.Text &&
                timeCount != 0)
                Global.Break_Out_Schedule = 1;
            if (ini12.INIRead(MainSettingPath, "Schedule5", "OnTimeStart", "") == "1" &&
                ini12.INIRead(MainSettingPath, "Schedule5", "Timer", "") == TimeLabel2.Text &&
                timeCount != 0)
                Global.Break_Out_Schedule = 1;
            #endregion
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

        private void LabelVersion_MouseClick(object sender, MouseEventArgs e)
        {
            FormSurp SurpriseForm = new FormSurp();
            SurpriseForm.Show(this);
        }

        private void Com1Btn_Click(object sender, EventArgs e)
        {
            OpenSerialPort("A");
            Controls.Add(textBox_serial);
            textBox_serial.BringToFront();
            Global.TEXTBOX_FOCUS = 1;
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

        private void MyExportCamd()
        {
            string ab_num = label_LoopNumber_Value.Text,                                                        //自動編號
                        ab_p_id = ini12.INIRead(MailPath, "Data Info", "ProjectNumber", ""),                    //Project number
                        ab_c_id = ini12.INIRead(MailPath, "Data Info", "TestCaseNumber", ""),                   //Test case number
                        ab_result = ini12.INIRead(MailPath, "Data Info", "Result", ""),                         //Woodpecker 測試結果
                        ab_version = ini12.INIRead(MailPath, "Mail Info", "Version", ""),                       //軟體版號
                        ab_ng = ini12.INIRead(MailPath, "Data Info", "NGfrequency", ""),                        //NG frequency
                        ab_create = ini12.INIRead(MailPath, "Data Info", "CreateTime", ""),                     //測試開始時間
                        ab_close = ini12.INIRead(MailPath, "Data Info", "CloseTime", ""),                       //測試結束時間
                        ab_time = ini12.INIRead(MailPath, "Total Test Time", "value", ""),                      //測試執行花費時間
                        ab_loop = Global.Schedule_Loop.ToString(),                                              //執行loop次數
                        ab_loop_time = ini12.INIRead(MailPath, "Total Test Time", "value", ""),                 //1個loop需要次數
                        ab_loop_step = (DataGridView_Schedule.Rows.Count - 1).ToString(),                       //1個loop的step數
                        ab_root = ini12.INIRead(MailPath, "Data Info", "Reboot", ""),                           //測試重啟次數
                        ab_user = ini12.INIRead(MailPath, "Mail Info", "Tester", ""),                           //測試人員
                        ab_mail = ini12.INIRead(MailPath, "Mail Info", "To", "");                               //Mail address 列表

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

            //Form_DGV_Autobox.DataInsert(DataList);
            //Form_DGV_Autobox.ToCsV(Form_DGV_Autobox.DGV_Autobox, "C:\\Woodpecker v2\\Report.xls");
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
            DataGridView_Schedule.Rows.Insert(DataGridView_Schedule.CurrentCell.RowIndex, new DataGridViewRow());
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
            ini12.INIWrite(MailPath, "Data Info", "TestCaseNumber", "0");
            ini12.INIWrite(MailPath, "Data Info", "Result", "N/A");
            ini12.INIWrite(MailPath, "Data Info", "NGfrequency", "0");
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
            else
            {
                /*SchedulePath = Application.StartupPath + @"\Schedule\shot_template.csv";
                i = 0;
                if ((File.Exists(SchedulePath) == true) && IsFileLocked(SchedulePath) == false)
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
                */
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
                ini12.INIWrite(MailPath, "Total Test Time", "value", d.ToString("0") + "d " + h.ToString("0") + "h " + s.ToString("0") + "m " + ms.ToString("0") + "s");

                // 寫入每個Schedule test time
                if (Global.Schedule_Number == 1)
                    ini12.INIWrite(MailPath, "Total Test Time", "value1", d.ToString("0") + "d " + h.ToString("0") + "h " + s.ToString("0") + "m " + ms.ToString("0") + "s");

                if (StartButtonPressed == true)
                {
                    switch (Global.Schedule_Number)
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
                SchedulePause.Reset();
            }
            else
            {
                button_Pause.Text = "PAUSE";
                button_Start.Enabled = true;
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
            ini12.INIWrite(MailPath, "Total Test Time", "How Long", d.ToString("0") + "d " + h.ToString("0") + "h " + s.ToString("0") + "m " + ms.ToString("0") + "s");
        }

        private void TimerPanelbutton_Click(object sender, EventArgs e)
        {
            TimerPanel = !TimerPanel;

            if (TimerPanel == true)
            {
                panel1.Show();
                panel1.BringToFront();
            }
            else
                panel1.Hide();
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

        private void button_Input_Click(object sender, EventArgs e)
        {
            //UInt32 gpio_input_value;
            //MyBlueRat.Get_GPIO_Input(out gpio_input_value);
            //byte GPIO_Read_Data = Convert.ToByte(gpio_input_value & 0xff);
            //labelGPIO_Input.Text = "GPIO_IN:" + GPIO_Read_Data.ToString();
            //Console.WriteLine("GPIO_IN:" + GPIO_Read_Data.ToString());

            UInt32 GPIO_input_value, retry_cnt;
            bool bRet = false;
            retry_cnt = 3;
            do
            {
                String modified0 = "";
                bRet = MyBlueRat.Get_GPIO_Input(out GPIO_input_value);

                if (GPIO_input_value == 31)
                {
                    modified0 = "0" + Convert.ToString(31, 2);
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
                Console.WriteLine(Global.IO_INPUT);
                Console.WriteLine(Global.IO_INPUT.Substring(0, 1));
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

        private void button_Output_Click(object sender, EventArgs e)
        {
            //string GPIO = "01010101";
            //byte GPIO_B = Convert.ToByte(GPIO, 2);
            //MyBlueRat.Set_GPIO_Output(GPIO_B);

            Graphics graphics = this.CreateGraphics();
            Console.WriteLine("dpiX = " + graphics.DpiX);
            Console.WriteLine("dpiY = " + graphics.DpiY);
            Console.WriteLine("-----------");
            Console.WriteLine("height = " + this.Size.Height);
            Console.WriteLine("width = " + this.Size.Width);
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

        private void button_VirtualRC_Click(object sender, EventArgs e)
        {
            /*
            VirtualRcPanel = !VirtualRcPanel;
            if (VirtualRcPanel == true)
            {
                LoadVirtualRC();
                panel_VirtualRC.Show();
                panel_VirtualRC.BringToFront();
            }
            else
            {
                panel_VirtualRC.Controls.Clear();
                panel_VirtualRC.Hide();
            }
            */
            FormRC formRC = new FormRC();
            formRC.Owner = this;
            if (Global.FormRC == false)
            {
                formRC.Show();
            }
        }

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
                DataGridView_Schedule.Rows[Breakpoint].DefaultCellStyle.BackColor = Color.White;
                DataGridView_Schedule.Rows[Breakpoint].DefaultCellStyle.SelectionBackColor = Color.PeachPuff;
                DataGridView_Schedule.Rows[Breakpoint].DefaultCellStyle.SelectionForeColor = Color.Black;
                DataGridView_Schedule.Rows[Nowpoint].DefaultCellStyle.BackColor = Color.Yellow;
                DataGridView_Schedule.Rows[Nowpoint].DefaultCellStyle.SelectionBackColor = Color.Yellow;
                DataGridView_Schedule.Rows[Nowpoint].DefaultCellStyle.SelectionForeColor = Color.Red;
                Breakpoint = Nowpoint;
                //Console.WriteLine("Change the Nowpoint");
            }
            else if (Breakfunction == true && Nowpoint == Breakpoint)
            {
                Breakfunction = false;
                DataGridView_Schedule.Rows[Breakpoint].DefaultCellStyle.BackColor = Color.White;
                DataGridView_Schedule.Rows[Breakpoint].DefaultCellStyle.SelectionBackColor = Color.PeachPuff;
                DataGridView_Schedule.Rows[Breakpoint].DefaultCellStyle.SelectionForeColor = Color.Black;
                DataGridView_Schedule.Rows[Nowpoint].DefaultCellStyle.SelectionBackColor = Color.PeachPuff;
                DataGridView_Schedule.Rows[Nowpoint].DefaultCellStyle.SelectionForeColor = Color.Black;
                Breakpoint = -1;
                //Console.WriteLine("Disable the Breakfunction");
            }
            else
            {
                Breakfunction = true;
                Breakpoint = Nowpoint;
                DataGridView_Schedule.Rows[Breakpoint].DefaultCellStyle.BackColor = Color.Yellow;
                DataGridView_Schedule.Rows[Breakpoint].DefaultCellStyle.SelectionBackColor = Color.Yellow;
                DataGridView_Schedule.Rows[Breakpoint].DefaultCellStyle.SelectionForeColor = Color.Red;
                Console.WriteLine("Enable the Breakfunction");
            }
        }

        private void button_Network_Click(object sender, EventArgs e)
        {
            string ip = ini12.INIRead(MainSettingPath, "Network", "IP", "");
            int port = int.Parse(ini12.INIRead(MainSettingPath, "Network", "Port", ""));

            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket.Connect(ip, port); // 1.設定 IP:Port 2.連線至伺服器
            NetworkStream stream = new NetworkStream(socket);
            StreamReader sr = new StreamReader(stream);
            StreamWriter sw = new StreamWriter(stream);

            sw.WriteLine("你好伺服器，我是客戶端。"); // 將資料寫入緩衝
            sw.Flush(); // 刷新緩衝並將資料上傳到伺服器

            Console.WriteLine("從伺服器接收的資料： " + sr.ReadLine());

            Console.ReadLine();

        }

        private void button_savelog_Click(object sender, EventArgs e)
        {
            string save_option = comboBox_savelog.Text;
            switch (save_option)
            {
                case "Port A":
                    Serialportsave("A");
                    break;
                case "Port B":
                    Serialportsave("B");
                    break;
                case "Port C":
                    Serialportsave("C");
                    break;
                case "Port D":
                    Serialportsave("D");
                    break;
                case "Port E":
                    Serialportsave("E");
                    break;
                case "Canbus":
                    Serialportsave("Canbus");
                    break;
                case "Kline":
                    Serialportsave("KlinePort");
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

        //Select & copy log from textbox
        private void Button_Copy_Click(object sender, EventArgs e)
        {
/*
            uint canBusStatus;
            canBusStatus = MYCanReader.Connect();

            if (Global.TEXTBOX_FOCUS == 1)
            {
                if (textBox_serial.SelectionLength == 0) //Determine if any text is selected in the TextBox control.
                {
                    CopyLog(textBox_serial);
                }
            }
            else if (Global.TEXTBOX_FOCUS == 2)
            {
                if (textBox2.SelectionLength == 0)
                {
                    CopyLog(textBox2);
                }
            }
            else if (Global.TEXTBOX_FOCUS == 3)
            {
                if (textBox_canbus.SelectionLength == 0)
                {
                    CopyLog(textBox3);
                }
            }
            else if (Global.TEXTBOX_FOCUS == 4)
            {
                CopyLog(textBox_kline);
            }

            //copy schedule log (might be removed in near future)
            else if (Global.TEXTBOX_FOCUS == 5)
            {
                CopyLog(textBox_canbus);
            }

            else if (Global.TEXTBOX_FOCUS == 6)
            {
                CopyLog(textBox_TestLog);
                string fName = "";

                // 讀取ini中的路徑
                fName = ini12.INIRead(MainSettingPath, "Record", "LogPath", "");
                string t = fName + "\\CanbusLog_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".txt";

                StreamWriter MYFILE = new StreamWriter(t, false, Encoding.ASCII);
                MYFILE.Write(textBox_TestLog.Text);
                MYFILE.Close();

                System.Diagnostics.Process CANLog = new System.Diagnostics.Process();
                System.Diagnostics.Process.Start(Application.StartupPath + @"\Canlog\CANLog.exe", fName);
            }
            */
        }

        public void CopyLog(TextBox tb)
        {
            //Select all text in the text box.
            tb.Focus();
            tb.SelectAll();
            // Copy the contents of the control to the Clipboard.
            tb.Copy();
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
                DisplayKLineBlockMessage(textBox_serial, "raw_input: " + raw_data_in_string + "\n\r");
                kline_text = string.Concat(kline_text, textBox_serial);
                DisplayKLineBlockMessage(textBox_serial, "In - " + in_message.GenerateDebugString() + "\n\r");
                kline_text = string.Concat(kline_text, textBox_serial);
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

                // Show output KLine message for debug purpose
                DisplayKLineBlockMessage(textBox_serial, "Out - " + out_message.GenerateDebugString() + "\n\r");
                kline_text = textBox_serial.Text;
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
