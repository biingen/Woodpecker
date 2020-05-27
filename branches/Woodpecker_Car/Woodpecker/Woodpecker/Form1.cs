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
using BlockMessageLibrary;
using DTC_ABS;
using DTC_OBD;
using MySerialLibrary;
using KWP_2000;
using USB_CAN2C;
using MaterialSkin.Controls;
using MaterialSkin;
using Microsoft.VisualBasic.FileIO;
using USB_VN1630A;

namespace Woodpecker
{
    public partial class Form1 : MaterialForm
    {
        private string _args;

        private string MainSettingPath = Application.StartupPath + "\\Config.ini";
        private string MailPath = Application.StartupPath + "\\Mail.ini";

        private Add_ons Add_ons = new Add_ons();
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
        //private const int CS_DROPSHADOW = 0x20000;      //宣告陰影參數

        private MySerial MySerialPort = new MySerial();
        private List<BlockMessage> MyBlockMessageList = new List<BlockMessage>();
        private ProcessBlockMessage MyProcessBlockMessage = new ProcessBlockMessage();

        //拖動無窗體的控件>>>>>>>>>>>>>>>>>>>>
        [DllImport("user32.dll")]
        new public static extern bool ReleaseCapture();

        [DllImport("user32.dll")]
        new public static extern bool SendMessage(IntPtr hwnd, int wMsg, int wParam, int lParam);
        public const int WM_SYSCOMMAND = 0x0112;
        public const int SC_MOVE = 0xF010;
        public const int HTCAPTION = 0x0002;
        //<<<<<<<<<<<<<<<<<<<<<<<<<<<<<

        [DllImport("user32.dll")]
        public static extern bool SetForegroundWindow(IntPtr hWnd);

        //CanReader
        private CAN_USB2C Can_Usb2C = new CAN_USB2C();
        private USB_VECTOR_Lib Can_1630A = new USB_VECTOR_Lib();
        private int can_send = 0;
        private List<USB_CAN2C.CAN_Data> can_data_list = new List<USB_CAN2C.CAN_Data>();
        private bool set_timer_rate = false;
        private uint can_id;
        private Dictionary<uint, uint> can_rate = new Dictionary<uint, uint>();
        private Dictionary<uint, byte[]> can_data = new Dictionary<uint, byte[]>();

        //Klite error code
        public int kline_send = 0;
        public List<DTC_Data> ABS_error_list = new List<DTC_Data>();
        public List<DTC_Data> OBD_error_list = new List<DTC_Data>();

        //Serial Port parameter
        public delegate void AddDataDelegate(String myString);
        public AddDataDelegate myDelegate1;
        private string logA_text, logB_text, logC_text, logD_text, logE_text, ca310_text, canbus_text, kline_text, logAll_text, debug_text;
        private int log_max_length = 10000000, debug_max_length = 10000000;
        //Search temperature parameter
        List<Temperature_Data> temperatureList = new List<Temperature_Data> { };
        Queue<double> temperatureDouble = new Queue<double> { };
        System.Timers.Timer duringTimer = new System.Timers.Timer();

        bool ifStatementFlag = false;
        bool ChamberIsFound = false;
        bool TemperatureIsFound = false;
        bool PowerSupplyIsFound = false;
        string MaxTemperature = "", MinTemperature = "";
        string expectedVoltage = string.Empty;
        string PowerSupplyCommandLog = string.Empty;

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
            MyUSBCameraDeviceConnected = false;
        }

        private void setStyle()
        {
            try
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
            catch (InvalidOperationException)
            {
                //MessageBox.Show(Ex.Message.ToString(), "setStyle Error");
            }
        }

        private void initComboboxSaveLog()
        {
            List<string> portList = new List<string> { "Port A", "Port B", "Port C", "Port D", "Port E", "Kline", "Canbus" };

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

            if (ini12.INIRead(MainSettingPath, "Canbus", "Log", "") == "1")
                comboBox_savelog.Items.Add("Canbus");

            if (comboBox_savelog.Items.Count > 1)
                comboBox_savelog.Items.Add("Port All");

            if (comboBox_savelog.Items.Count == 0)
            {
                button_savelog.Enabled = false;
                comboBox_savelog.Enabled = false;
            }
            else
            {
                button_savelog.Enabled = true;
                comboBox_savelog.Enabled = true;
                comboBox_savelog.SelectedIndex = 0;
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
            int intPercent = (dpiX == 96) ? 100 : (dpiX == 120) ? 125 : 150;

            // 針對字體變更Form的大小
            this.Height = this.Height * intPercent / 100;

            // FwVersion
            label_FwVersion.Text = "Ver. " + Assembly.GetExecutingAssembly().GetName().Version.ToString();

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
                button_AcUsb.Enabled = false;
            }

            if (ini12.INIRead(MainSettingPath, "Device", "CameraExist", "") == "1")
            {
                try
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
                catch (Exception Ex)
                {
                    Console.WriteLine(Ex);
                    MessageBox.Show(Ex.Message.ToString(), "Camera audio open error!");
                }
            }
            else
            {
                pictureBox_Camera.Image = Properties.Resources.OFF;
            }

            if (ini12.INIRead(MainSettingPath, "Device", "UsbCANExist", "") == "1" || ini12.INIRead(MainSettingPath, "Device", "CAN1630AExist", "") == "1")
            {
                if (ini12.INIRead(MainSettingPath, "Device", "UsbCANExist", "") == "1")
                {
                    String can_name;
                    List<String> dev_list = Can_Usb2C.FindUsbDevice();
                    can_name = string.Join(",", dev_list);
                    ini12.INIWrite(MainSettingPath, "Canbus", "DevName", can_name);
                    if (ini12.INIRead(MainSettingPath, "Canbus", "DevIndex", "") == "")
                        ini12.INIWrite(MainSettingPath, "Canbus", "DevIndex", "0");
                    if (ini12.INIRead(MainSettingPath, "Canbus", "BaudRate", "") == "")
                        ini12.INIWrite(MainSettingPath, "Canbus", "BaudRate", "500 Kbps");
                    ConnectUsbCAN();
                    pictureBox_canbus.Image = Properties.Resources.ON;
                }

                if (ini12.INIRead(MainSettingPath, "Device", "CAN1630AExist", "") == "1")
                {
                    ConnectVectorCAN();
                    pictureBox_canbus.Image = Properties.Resources.ON;
                }
            }
            else
            {
                pictureBox_canbus.Image = Properties.Resources.OFF;
            }

            LoadRCDB();

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


        private void ConnectAutoBox2()
        {
            uint temp_version;
            string curItem = ini12.INIRead(MainSettingPath, "Device", "AutoboxPort", "");
            if (MyBlueRat.Connect(curItem) == true)
            {
                temp_version = MyBlueRat.FW_VER;
                float v = temp_version;
                label_BoxVersion.Text = "_" + (v / 100).ToString("0.00");

                MyBlueRat.Force_Init_BlueRat();
                MyBlueRat.Reset_SX1509();

                byte SX1509_detect_status;
                SX1509_detect_status = MyBlueRat.TEST_Detect_SX1509();

                if (SX1509_detect_status == 3)
                {
                    pictureBox_ext_board.Image = Properties.Resources.ON;
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

        protected void ConnectUsbCAN()
        {
            uint status;

            status = Can_Usb2C.Connect();
            if (status == 1)
            {
                status = Can_Usb2C.StartCAN();
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

        protected void ConnectVectorCAN()
        {
            uint status;

            status = Can_1630A.Connect();
            if (status == 1)
            {
                status = Can_1630A.StartCAN();
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
        // Woodpecker debug function 
        private void debug_process(string log)
        {
            try
            {
                debug_text = string.Concat(debug_text, "[Debug] [" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff") + "]  " + log + "\r\n");
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message.ToString());
                Serialportsave("Debug");
            }
        }

        // Log record function 
        private void log_process(string port, string log)
        {
            try
            {
                switch (port)
                {
                    case "A":
                        logA_text = string.Concat(logA_text, log);
                        break;
                    case "B":
                        logB_text = string.Concat(logB_text, log);
                        break;
                    case "C":
                        logC_text = string.Concat(logC_text, log);
                        break;
                    case "D":
                        logD_text = string.Concat(logD_text, log);
                        break;
                    case "E":
                        logE_text = string.Concat(logE_text, log);
                        break;
                    case "CA310":
                        ca310_text = string.Concat(ca310_text, log);
                        break;
                    case "Canbus":
                        canbus_text = string.Concat(canbus_text, log);
                        break;
                    case "KlinePort":
                        kline_text = string.Concat(kline_text, log);
                        break;
                    case "All":
                        logAll_text = string.Concat(logAll_text, log);
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message.ToString());
                Serialportsave("All");
            }
        }
        // 這個主程式專用的delay的內部資料與function
        static bool RedRatDBViewer_Delay_TimeOutIndicator = false;
        private void RedRatDBViewer_Delay_OnTimedEvent(object source, ElapsedEventArgs e)
        {
            //debug_process("RedRatDBViewer_Delay_TimeOutIndicator_True");
            RedRatDBViewer_Delay_TimeOutIndicator = true;
        }

        private void RedRatDBViewer_Delay(int delay_ms)
        {
            debug_process("RedRatDBViewer_Delay_S");
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
                if (temperatureDouble.Count() > 0 || timer_matched)
                {
                    if (temperatureDouble.Count() > 0)
                    {
                        currentTemperature = temperatureDouble.Dequeue();
                        label_Command.Text = "Condition: " + currentTemperature + ", SHOT: " + currentTemperature;
                    }
                    else if (timer_matched)
                    {
                        label_Command.Text = "Timer: matched.";
                        timer_matched = false;
                    }

                    Global.caption_Num++;
                    if (Global.Loop_Number == 1)
                        Global.caption_Sum = Global.caption_Num;
                    Jes();
                }

                if (logA_text.Length > log_max_length)
                {
                    if (ini12.INIRead(MainSettingPath, "Autosavelog", "Checked", "") == "1")
                    {
                        Serialportsave("A");
                    }
                    else
                    {
                        logA_text = string.Empty;
                    }
                }

                if (logB_text.Length > log_max_length)
                {
                    if (ini12.INIRead(MainSettingPath, "Autosavelog", "Checked", "") == "1")
                    {
                        Serialportsave("B");
                    }
                    else
                    {
                        logB_text = string.Empty;
                    }
                }

                if (logC_text.Length > log_max_length)
                {
                    if (ini12.INIRead(MainSettingPath, "Autosavelog", "Checked", "") == "1")
                    {
                        Serialportsave("C");
                    }
                    else
                    {
                        logC_text = string.Empty;
                    }
                }

                if (logD_text.Length > log_max_length)
                {
                    if (ini12.INIRead(MainSettingPath, "Autosavelog", "Checked", "") == "1")
                    {
                        Serialportsave("D");
                    }
                    else
                    {
                        logD_text = string.Empty;
                    }
                }

                if (logE_text.Length > log_max_length)
                {
                    if (ini12.INIRead(MainSettingPath, "Autosavelog", "Checked", "") == "1")
                    {
                        Serialportsave("E");
                    }
                    else
                    {
                        logE_text = string.Empty;
                    }
                }

                if (logAll_text.Length > log_max_length)
                {
                    if (ini12.INIRead(MainSettingPath, "Autosavelog", "Checked", "") == "1")
                    {
                        Serialportsave("All");
                    }
                    else
                    {
                        logAll_text = string.Empty;
                    }
                }

                if (canbus_text.Length > log_max_length)
                {
                    if (ini12.INIRead(MainSettingPath, "Autosavelog", "Checked", "") == "1")
                    {
                        Serialportsave("Canbus");
                    }
                    else
                    {
                        canbus_text = string.Empty;
                    }
                }

                if (kline_text.Length > log_max_length)
                {
                    if (ini12.INIRead(MainSettingPath, "Autosavelog", "Checked", "") == "1")
                    {
                        Serialportsave("KlinePort");
                    }
                    else
                    {
                        kline_text = string.Empty;
                    }
                }

                if (debug_text.Length > debug_max_length)
                {
                    Serialportsave("Debug");
                }

                //debug_process("RedRatDBViewer_Delay_TimeOutIndicator_false");
                Application.DoEvents();
                System.Threading.Thread.Sleep(1);//釋放CPU//

                if (Global.Break_Out_MyRunCamd == 1)//強制讓schedule直接停止//
                {
                    Global.Break_Out_MyRunCamd = 0;
                    debug_process("Break_Out_MyRunCamd_0");
                    break;
                }
            }

            aTimer.Stop();
            aTimer.Dispose();
            debug_process("RedRatDBViewer_Delay_E");
        }

        // 這個usbcan專用的delay的內部資料與function
        static bool UsbCAN_Delay_TimeOutIndicator = false;
        static UInt64 UsbCAN_Count = 0;
        private void UsbCAN_Delay_UsbOnTimedEvent(object source, ElapsedEventArgs e)
        {
            uint columns_times = can_id;
            byte[] columns_serial = can_data[columns_times];
            int columns_interval = (int)can_rate[columns_times];
            Can_Usb2C.TransmitData(columns_times, columns_serial);
            Console.WriteLine("USB_Can_Send (Repeat): " + UsbCAN_Count + " times.");

            string Outputstring = "ID: 0x";
            //Outputstring += columns_times + " Data: " + columns_serial;
            DateTime dt = DateTime.Now;
            string canbus_log_text = "[Send_UsbCAN] [" + dt.ToString("yyyy/MM/dd HH:mm:ss.fff") + "]  " + Outputstring + "\r\n";
            log_process("Canbus", canbus_log_text);
            UsbCAN_Count++;
            UsbCAN_Delay_TimeOutIndicator = true;
        }

        private void UsbCAN_Delay(int delay_ms)
        {
            //Console.WriteLine("UsbCAN_Delay: Start.");
            if (delay_ms <= 0) return;
            System.Timers.Timer UsbCAN_Timer = new System.Timers.Timer(delay_ms);
            UsbCAN_Timer.Interval = delay_ms;
            UsbCAN_Timer.Elapsed += new ElapsedEventHandler(UsbCAN_Delay_UsbOnTimedEvent);
            UsbCAN_Timer.Enabled = true;
            UsbCAN_Timer.Start();
            UsbCAN_Timer.AutoReset = true;

            while ((FormIsClosing == false) && (UsbCAN_Delay_TimeOutIndicator == false))
            {
                //Console.WriteLine("UsbCAN_Delay_TimeOutIndicator: false.");
                Application.DoEvents();
                System.Threading.Thread.Sleep(1);//釋放CPU//

                if (Global.Break_Out_MyRunCamd == 1)//強制讓schedule直接停止//
                {
                    Global.Break_Out_MyRunCamd = 0;
                    //Console.WriteLine("Break_Out_MyRunCamd = 0");
                    break;
                }
            }
            UsbCAN_Timer.Stop();
            UsbCAN_Timer.Dispose();
            //Console.WriteLine("UsbCAN_Delay: Stop.");
        }

        // 這個vectorcan專用的delay的內部資料與function
        static bool VectorCAN_Delay_TimeOutIndicator = false;
        static UInt64 VectorCAN_Count = 0;
        private void VectorCAN_Delay_UsbOnTimedEvent(object source, ElapsedEventArgs e)
        {
            uint columns_times = can_id;
            byte[] columns_serial = can_data[columns_times];
            int columns_interval = (int)can_rate[columns_times];
            Can_1630A.LoopCANTransmit(columns_times, (uint)columns_interval, columns_serial);
            Console.WriteLine("VectorCAN_Can_Send (Repeat): " + VectorCAN_Count + " times.");

            string Outputstring = "ID: 0x";
            //Outputstring += columns_times + " Data: " + columns_serial;
            DateTime dt = DateTime.Now;
            string canbus_log_text = "[Send_VectorCAN] [" + dt.ToString("yyyy/MM/dd HH:mm:ss.fff") + "]  " + Outputstring + "\r\n";
            log_process("Canbus", canbus_log_text);
            VectorCAN_Count++;
            VectorCAN_Delay_TimeOutIndicator = true;
        }

        private void VectorCAN_Delay(int delay_ms)
        {
            //Console.WriteLine("VectorCAN_Delay: Start.");
            if (delay_ms <= 0) return;
            System.Timers.Timer VectorCAN_Timer = new System.Timers.Timer(delay_ms);
            VectorCAN_Timer.Interval = delay_ms;
            VectorCAN_Timer.Elapsed += new ElapsedEventHandler(VectorCAN_Delay_UsbOnTimedEvent);
            VectorCAN_Timer.Enabled = true;
            VectorCAN_Timer.Start();
            VectorCAN_Timer.AutoReset = true;

            while ((FormIsClosing == false) && (VectorCAN_Delay_TimeOutIndicator == false))
            {
                //Console.WriteLine("VectorCAN_Delay_TimeOutIndicator: false.");
                Application.DoEvents();
                System.Threading.Thread.Sleep(1);//釋放CPU//

                if (Global.Break_Out_MyRunCamd == 1)//強制讓schedule直接停止//
                {
                    Global.Break_Out_MyRunCamd = 0;
                    //Console.WriteLine("Break_Out_MyRunCamd = 0");
                    break;
                }
            }
            VectorCAN_Timer.Stop();
            VectorCAN_Timer.Dispose();
            //Console.WriteLine("VectorCAN_Delay: Stop.");
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

                            // PortA.DataReceived += new SerialDataReceivedEventHandler(SerialPort1_DataReceived);       // DataReceived呼叫函式
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

                            // PortB.DataReceived += new SerialDataReceivedEventHandler(SerialPort2_DataReceived);       // DataReceived呼叫函式
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

                            // PortC.DataReceived += new SerialDataReceivedEventHandler(SerialPort3_DataReceived);       // DataReceived呼叫函式
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

                            // PortD.DataReceived += new SerialDataReceivedEventHandler(SerialPort4_DataReceived);       // DataReceived呼叫函式
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

                            // PortE.DataReceived += new SerialDataReceivedEventHandler(SerialPort4_DataReceived);       // DataReceived呼叫函式
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

        private void logA_analysis()
        {
            while (PortA.IsOpen == true)
            {
                int data_to_read = PortA.BytesToRead;
                if (data_to_read > 0)
                {
                    byte[] dataset = new byte[data_to_read];
                    PortA.Read(dataset, 0, data_to_read);

                    for (int index = 0; index < data_to_read; index++)
                    {
                        byte input_ch = dataset[index];
                        logA_recorder(input_ch);
                        if (TemperatureIsFound == true)
                        {
                            log_temperature(input_ch);
                        }
                    }
                    //else
                    //{
                    //    logA_recorder(0x00,true); // tell log_recorder no more data for now.
                    //}
                }
                //else
                //{
                //    logA_recorder(0x00,true); // tell log_recorder no more data for now.
                //}
            }
        }

        const int byteMessage_max_Hex = 16;
        const int byteMessage_max_Ascii = 256;
        byte[] byteMessage_A = new byte[Math.Max(byteMessage_max_Ascii, byteMessage_max_Hex)];
        int byteMessage_length_A = 0;

        private void logA_recorder(byte ch, bool SaveToLog = false)
        {
            if (ini12.INIRead(MainSettingPath, "Displayhex", "Checked", "") == "1")
            {
                // if (SaveToLog == false)
                {
                    byteMessage_A[byteMessage_length_A] = ch;
                    byteMessage_length_A++;
                }
                if ((ch == 0x0A) || (ch == 0x0D) || (byteMessage_length_A >= byteMessage_max_Hex) /*|| (SaveToLog == true)*/)
                {
                    byteMessage_A[byteMessage_length_A] = ch;
                    byteMessage_length_A++;
                    string dataValue = BitConverter.ToString(byteMessage_A).Replace("-", "").Substring(0, byteMessage_length_A * 2);
                    if (ini12.INIRead(MainSettingPath, "Timestamp", "Checked", "") == "1")
                    {
                        DateTime dt = DateTime.Now;
                        dataValue = "[Receive_Port_A] [" + dt.ToString("yyyy/MM/dd HH:mm:ss.fff") + "]  " + dataValue + "\r\n"; //OK
                    }
                    log_process("A", dataValue);
                    log_process("All", dataValue);
                    byteMessage_length_A = 0;
                }
            }
            else
            {
                if ((ch == 0x0A) || (ch == 0x0D) || (byteMessage_length_A >= byteMessage_max_Ascii))
                {
                    byteMessage_A[byteMessage_length_A] = ch;
                    byteMessage_length_A++;
                    string dataValue = Encoding.ASCII.GetString(byteMessage_A).Substring(0, byteMessage_length_A);
                    if (ini12.INIRead(MainSettingPath, "Timestamp", "Checked", "") == "1")
                    {
                        DateTime dt = DateTime.Now;
                        dataValue = "[Receive_Port_A] [" + dt.ToString("yyyy/MM/dd HH:mm:ss.fff") + "]  " + dataValue + "\r\n"; //OK
                    }
                    log_process("A", dataValue);
                    log_process("All", dataValue);
                    byteMessage_length_A = 0;
                }
                else
                {
                    byteMessage_A[byteMessage_length_A] = ch;
                    byteMessage_length_A++;
                }
            }
        }

        const int byteTemperature_max = 64;
        byte[] byteTemperature = new byte[byteTemperature_max];
        int byteTemperature_length = 0;

        private void log_temperature(byte ch)
        {
            const int packet_len = 16;
            const int header_offset_1 = -16;
            const int header_offset_2 = -15;
            const int temp_ch_offset = -14;
            const int temp_unit_02 = -13;
            const int temp_unit_01 = -12;
            const int temp_polarity_offset = -11;
            const int temp_dp_offset = -10;
            const int temp_data8_offset = -9;
            const int temp_data7_offset = -8;
            const int temp_data6_offset = -7;
            const int temp_data5_offset = -6;
            const int temp_data4_offset = -5;
            const int temp_data3_offset = -4;
            const int temp_data2_offset = -3;
            const int temp_data1_offset = -2;
            const double temp_abs_value = 0.05;

            // If data_buffer is too long, cut off data not needed
            if(byteTemperature_length>= byteTemperature_max)
            {
                int destinationIndex = 0;
                for (int i = (byteTemperature_max-packet_len); i < byteTemperature_max; i++)
                {
                    byteTemperature[destinationIndex++] = byteTemperature[i];
                }
                byteTemperature_length = destinationIndex;
            }

            byteTemperature[byteTemperature_length] = ch;
            byteTemperature_length++;

            if (ch == 0x0D)
            {
                if ( ((byteTemperature_length + header_offset_1)>=0) &&
                     (byteTemperature[byteTemperature_length + header_offset_1] == 0x02) &&
                     (byteTemperature[byteTemperature_length + header_offset_2] == '4') )
                {
                    // Packet is valid here
                    if (byteTemperature[byteTemperature_length + temp_ch_offset] == Temperature_Data.temperatureChannel)
                    {
                        // Channel number is checked and ok here
                        if ((byteTemperature[byteTemperature_length + temp_unit_02] == '0'))
                        {
                            if ((byteTemperature[byteTemperature_length + temp_unit_01] == '1')
                                || (byteTemperature[byteTemperature_length + temp_unit_01] == '2'))
                            {
                                if ((byteTemperature[byteTemperature_length + temp_data1_offset] != 0x18))
                                {
                                    // data is valid
                                    int DP_convert = '0';
                                    int byteArray_position = 0;
                                    byte[] byteArray = new byte[8];
                                    for (int pos = byteTemperature_length + temp_data8_offset;
                                                pos <= (byteTemperature_length + temp_data1_offset);
                                                pos++)
                                    {
                                        byteArray[byteArray_position] = byteTemperature[pos];
                                        byteArray_position++;
                                    }

                                    string tempSubstring = System.Text.Encoding.Default.GetString(byteArray);
                                    double digit = Math.Pow(10, Convert.ToInt64(byteTemperature[byteTemperature_length + temp_dp_offset] - DP_convert));
                                    double currentTemperature = Convert.ToDouble(Convert.ToInt32(tempSubstring) / digit);

                                    // is value negative?
                                    if (byteTemperature[byteTemperature_length + temp_polarity_offset] == '1')
                                    {
                                        currentTemperature = -currentTemperature;
                                    }

                                    // is value Fahrenheit?
                                    if (byteTemperature[byteTemperature_length + temp_unit_01] == '2')
                                    { 
                                        currentTemperature = (currentTemperature - 32) / 1.8;
                                        currentTemperature = Math.Round((currentTemperature),2,MidpointRounding.AwayFromZero);
                                    }

                                    // check whether 2 temperatures are close enough
                                    if (Math.Abs(previousTemperature-currentTemperature) >= temp_abs_value)
                                    {
                                        previousTemperature = currentTemperature;
                                        foreach (Temperature_Data item in temperatureList)
                                        {
                                            if (item.temperatureList == currentTemperature &&
                                                item.temperatureShot == true)
                                            {
                                                Console.WriteLine("~~~ targetTemperature ~~~ " + previousTemperature + " ~~~ currentTemperature ~~~ " + currentTemperature);
                                                temperatureDouble.Enqueue(currentTemperature);
                                                Console.WriteLine("~~~ Enqueue temperature ~~~ " + currentTemperature);
                                            }

                                            if (item.temperatureList == currentTemperature &&
                                                item.temperaturePause == true)
                                            {
                                                label_Command.Text = "Condition: " + item.temperatureList + ", PAUSE: " + currentTemperature;
                                                button_Pause.PerformClick();
                                                Console.WriteLine("Temperature: " + currentTemperature + "~~~~~~~~~Temperature matched. Pause the schedule.~~~~~~~~~");
                                            }

                                            if (item.temperatureList == currentTemperature &&
                                                     item.temperaturePort != "" &&
                                                     item.temperatureLog != "" &&
                                                     item.temperatureNewline != "")
                                            {
                                                label_Command.Text = "Condition: " + item.temperatureList + ", Log: " + currentTemperature;
                                                if (item.temperatureLog.Contains('|'))
                                                {
                                                    string[] logArray = item.temperatureLog.Split('|');
                                                    switch (item.temperaturePort)
                                                    {
                                                        case "A":
                                                            for (int i = 0; i < logArray.Length; i++)
                                                                ReplaceNewLine(PortA, logArray[i], item.temperatureNewline);
                                                            break;
                                                        case "B":
                                                            for (int i = 0; i < logArray.Length; i++)
                                                                ReplaceNewLine(PortB, logArray[i], item.temperatureNewline);
                                                            break;
                                                        case "C":
                                                            for (int i = 0; i < logArray.Length; i++)
                                                                ReplaceNewLine(PortC, logArray[i], item.temperatureNewline);
                                                            break;
                                                        case "D":
                                                            for (int i = 0; i < logArray.Length; i++)
                                                                ReplaceNewLine(PortD, logArray[i], item.temperatureNewline);
                                                            break;
                                                        case "E":
                                                            for (int i = 0; i < logArray.Length; i++)
                                                                ReplaceNewLine(PortE, logArray[i], item.temperatureNewline);
                                                            break;
                                                    }
                                                }
                                                else
                                                {
                                                    switch (item.temperaturePort)
                                                    {
                                                        case "A":
                                                            ReplaceNewLine(PortA, item.temperatureLog, item.temperatureNewline);
                                                            break;
                                                        case "B":
                                                            ReplaceNewLine(PortB, item.temperatureLog, item.temperatureNewline);
                                                            break;
                                                        case "C":
                                                            ReplaceNewLine(PortC, item.temperatureLog, item.temperatureNewline);
                                                            break;
                                                        case "D":
                                                            ReplaceNewLine(PortD, item.temperatureLog, item.temperatureNewline);
                                                            break;
                                                        case "E":
                                                            ReplaceNewLine(PortE, item.temperatureLog, item.temperatureNewline);
                                                            break;
                                                    }
                                                }
                                                Console.WriteLine("Temperature: " + currentTemperature + "~~~~~~~~~Temperature matched. Send the log to device.~~~~~~~~~");
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                byteTemperature_length = 0;
            }
        }

        const int byteChamber_max = 64;
        byte[] byteChamber = new byte[byteChamber_max];
        int byteChamber_length = 0;

        private void logA_chamber(byte ch)
        {
            const int header_data1_offset = -9;
            const int header_data2_offset = -8;
            const int length_data_offset = -7;
            const int data_actual2_offset = -6;
            const int data_actual1_offset = -5;
            const int data_target2_offset = -4;
            const int data_target1_offset = -3;
            const int crc16_highbit_offset = -2;
            const int crc16_lowbit_offset = -1;

            byteChamber[byteChamber_length] = ch;
            byteChamber_length++;

            if ((byteChamber[byteChamber_length + header_data1_offset] == 0x01) &&
                (byteChamber[byteChamber_length + header_data2_offset] == 0x03) &&
                (byteChamber[byteChamber_length + length_data_offset] == 0x04))
            {
                byte[] byteActual = new byte[2];
                byte[] byteTarget = new byte[2];
                byteActual[0] = byteChamber[byteChamber_length + data_actual2_offset];
                byteActual[1] = byteChamber[byteChamber_length + data_actual1_offset];
                byteTarget[0] = byteChamber[byteChamber_length + data_target2_offset];
                byteTarget[1] = byteChamber[byteChamber_length + data_target1_offset];
                string stringActual = System.Text.Encoding.Default.GetString(byteActual);
                string stringTarget = System.Text.Encoding.Default.GetString(byteTarget);
                int intActual = Convert.ToInt32(stringActual, 16);
                int intTarget = Convert.ToInt32(stringTarget, 16);
            }
            byteChamber_length = 0;
        }
        #endregion


        #region -- 接受SerialPort2資料 --

        private void logB_analysis()
        {
            while (PortB.IsOpen == true)
            {
                int data_to_read = PortB.BytesToRead;
                if (data_to_read > 0)
                {
                    byte[] dataset = new byte[data_to_read];
                    PortB.Read(dataset, 0, data_to_read);

                    for (int index = 0; index < data_to_read; index++)
                    {
                        byte input_ch = dataset[index];
                        logB_recorder(input_ch);
                        if (TemperatureIsFound == true)
                        {
                            log_temperature(input_ch);
                        }
                    }
                }
                //else
                //{
                //    logB_recorder(0x00,true); // tell log_recorder no more data for now.
                //}
            }
        }

        byte[] byteMessage_B = new byte[Math.Max(byteMessage_max_Ascii, byteMessage_max_Hex)];
        int byteMessage_length_B = 0;

        private void logB_recorder(byte ch, bool SaveToLog = false)
        {
            if (ini12.INIRead(MainSettingPath, "Displayhex", "Checked", "") == "1")
            {
                // if (SaveToLog == false)
                {
                    byteMessage_B[byteMessage_length_B] = ch;
                    byteMessage_length_B++;
                }
                if ((ch == 0x0A) || (ch == 0x0D) || (byteMessage_length_B >= byteMessage_max_Hex) /*|| (SaveToLog == true)*/)
                {
                    string dataValue = BitConverter.ToString(byteMessage_B).Replace("-", "").Substring(0, byteMessage_length_B * 2);
                    if (ini12.INIRead(MainSettingPath, "Timestamp", "Checked", "") == "1")
                    {
                        DateTime dt = DateTime.Now;
                        dataValue = "[Receive_Port_B] [" + dt.ToString("yyyy/MM/dd HH:mm:ss.fff") + "]  " + dataValue + "\r\n"; //OK
                    }
                    log_process("B", dataValue);
                    log_process("All", dataValue);
                    byteMessage_length_B = 0;
                }
            }
            else
            {
                if ((ch == 0x0A) || (ch == 0x0D) || (byteMessage_length_B >= byteMessage_max_Ascii))
                {
                    string dataValue = Encoding.ASCII.GetString(byteMessage_B).Substring(0, byteMessage_length_B);
                    if (ini12.INIRead(MainSettingPath, "Timestamp", "Checked", "") == "1")
                    {
                        DateTime dt = DateTime.Now;
                        dataValue = "[Receive_Port_B] [" + dt.ToString("yyyy/MM/dd HH:mm:ss.fff") + "]  " + dataValue + "\r\n"; //OK
                    }
                    log_process("B", dataValue);
                    log_process("All", dataValue);
                    byteMessage_length_B = 0;
                }
                else
                {
                    byteMessage_B[byteMessage_length_B] = ch;
                    byteMessage_length_B++;
                }
            }
        }
        #endregion

        #region -- 接受SerialPort3資料 --
        private void logC_analysis()
        {
            while (PortC.IsOpen == true)
            {
                int data_to_read = PortC.BytesToRead;
                if (data_to_read > 0)
                {
                    byte[] dataset = new byte[data_to_read];
                    PortC.Read(dataset, 0, data_to_read);

                    for (int index = 0; index < data_to_read; index++)
                    {
                        byte input_ch = dataset[index];
                        logC_recorder(input_ch);
                        if (TemperatureIsFound == true)
                        {
                            log_temperature(input_ch);
                        }
                    }
                }
                //else
                //{
                //    logD_recorder(0x00,true); // tell log_recorder no more data for now.
                //}
            }
        }

        byte[] byteMessage_C = new byte[Math.Max(byteMessage_max_Ascii, byteMessage_max_Hex)];
        int byteMessage_length_C = 0;

        private void logC_recorder(byte ch, bool SaveToLog = false)
        {
            if (ini12.INIRead(MainSettingPath, "Displayhex", "Checked", "") == "1")
            {
                // if (SaveToLog == false)
                {
                    byteMessage_C[byteMessage_length_C] = ch;
                    byteMessage_length_C++;
                }
                if ((ch == 0x0A) || (ch == 0x0D) || (byteMessage_length_C >= byteMessage_max_Hex) /*|| (SaveToLog == true)*/)
                {
                    string dataValue = BitConverter.ToString(byteMessage_C).Replace("-", "").Substring(0, byteMessage_length_C * 2);
                    if (ini12.INIRead(MainSettingPath, "Timestamp", "Checked", "") == "1")
                    {
                        DateTime dt = DateTime.Now;
                        dataValue = "[Receive_Port_C] [" + dt.ToString("yyyy/MM/dd HH:mm:ss.fff") + "]  " + dataValue + "\r\n"; //OK
                    }
                    log_process("C", dataValue);
                    log_process("All", dataValue);
                    byteMessage_length_C = 0;
                }
            }
            else
            {
                if ((ch == 0x0A) || (ch == 0x0D) || (byteMessage_length_C >= byteMessage_max_Ascii))
                {
                    string dataValue = Encoding.ASCII.GetString(byteMessage_C).Substring(0, byteMessage_length_C);
                    if (ini12.INIRead(MainSettingPath, "Timestamp", "Checked", "") == "1")
                    {
                        DateTime dt = DateTime.Now;
                        dataValue = "[Receive_Port_C] [" + dt.ToString("yyyy/MM/dd HH:mm:ss.fff") + "]  " + dataValue + "\r\n"; //OK
                    }
                    log_process("C", dataValue);
                    log_process("All", dataValue);
                    byteMessage_length_C = 0;
                }
                else
                {
                    byteMessage_C[byteMessage_length_C] = ch;
                    byteMessage_length_C++;
                }
            }
        }
        #endregion

        #region -- 接受SerialPort4資料 --
        private void logD_analysis()
        {
            while (PortD.IsOpen == true)
            {
                int data_to_read = PortD.BytesToRead;
                if (data_to_read > 0)
                {
                    byte[] dataset = new byte[data_to_read];
                    PortD.Read(dataset, 0, data_to_read);

                    for (int index = 0; index < data_to_read; index++)
                    {
                        byte input_ch = dataset[index];
                        logD_recorder(input_ch);
                        if (TemperatureIsFound == true)
                        {
                            log_temperature(input_ch);
                        }
                    }
                }
                //else
                //{
                //    logD_recorder(0x00,true); // tell log_recorder no more data for now.
                //}
            }
        }

        byte[] byteMessage_D = new byte[Math.Max(byteMessage_max_Ascii, byteMessage_max_Hex)];
        int byteMessage_length_D = 0;

        private void logD_recorder(byte ch, bool SaveToLog = false)
        {
            if (ini12.INIRead(MainSettingPath, "Displayhex", "Checked", "") == "1")
            {
                // if (SaveToLog == false)
                {
                    byteMessage_D[byteMessage_length_D] = ch;
                    byteMessage_length_D++;
                }
                if ((ch == 0x0A) || (ch == 0x0D) || (byteMessage_length_D >= byteMessage_max_Hex) /*|| (SaveToLog == true)*/)
                {
                    string dataValue = BitConverter.ToString(byteMessage_D).Replace("-", "").Substring(0, byteMessage_length_D * 2);
                    if (ini12.INIRead(MainSettingPath, "Timestamp", "Checked", "") == "1")
                    {
                        DateTime dt = DateTime.Now;
                        dataValue = "[Receive_Port_D] [" + dt.ToString("yyyy/MM/dd HH:mm:ss.fff") + "]  " + dataValue + "\r\n"; //OK
                    }
                    log_process("D", dataValue);
                    log_process("All", dataValue);
                    byteMessage_length_D = 0;
                }
            }
            else
            {
                if ((ch == 0x0A) || (ch == 0x0D) || (byteMessage_length_D >= byteMessage_max_Ascii))
                {
                    string dataValue = Encoding.ASCII.GetString(byteMessage_D).Substring(0, byteMessage_length_D);
                    if (ini12.INIRead(MainSettingPath, "Timestamp", "Checked", "") == "1")
                    {
                        DateTime dt = DateTime.Now;
                        dataValue = "[Receive_Port_D] [" + dt.ToString("yyyy/MM/dd HH:mm:ss.fff") + "]  " + dataValue + "\r\n"; //OK
                    }
                    log_process("D", dataValue);
                    log_process("All", dataValue);
                    byteMessage_length_D = 0;
                }
                else
                {
                    byteMessage_D[byteMessage_length_D] = ch;
                    byteMessage_length_D++;
                }
            }
        }
        #endregion

        #region -- 接受SerialPort5資料 --
        private void logE_analysis()
        {
            while (PortE.IsOpen == true)
            {
                int data_to_read = PortE.BytesToRead;
                if (data_to_read > 0)
                {
                    byte[] dataset = new byte[data_to_read];
                    PortE.Read(dataset, 0, data_to_read);

                    for (int index = 0; index < data_to_read; index++)
                    {
                        byte input_ch = dataset[index];
                        logE_recorder(input_ch);
                        if (TemperatureIsFound == true)
                        {
                            log_temperature(input_ch);
                        }
                    }
                }
                //else
                //{
                //    logB_recorder(0x00,true); // tell log_recorder no more data for now.
                //}
            }
        }

        byte[] byteMessage_E = new byte[Math.Max(byteMessage_max_Ascii, byteMessage_max_Hex)];
        int byteMessage_length_E = 0;

        private void logE_recorder(byte ch, bool SaveToLog = false)
        {
            if (ini12.INIRead(MainSettingPath, "Displayhex", "Checked", "") == "1")
            {
                // if (SaveToLog == false)
                {
                    byteMessage_E[byteMessage_length_E] = ch;
                    byteMessage_length_E++;
                }
                if ((ch == 0x0A) || (ch == 0x0D) || (byteMessage_length_E >= byteMessage_max_Hex) /*|| (SaveToLog == true)*/)
                {
                    string dataValue = BitConverter.ToString(byteMessage_E).Replace("-", "").Substring(0, byteMessage_length_E * 2);
                    if (ini12.INIRead(MainSettingPath, "Timestamp", "Checked", "") == "1")
                    {
                        DateTime dt = DateTime.Now;
                        dataValue = "[Receive_Port_E] [" + dt.ToString("yyyy/MM/dd HH:mm:ss.fff") + "]  " + dataValue + "\r\n"; //OK
                    }
                    log_process("E", dataValue);
                    log_process("All", dataValue);
                    byteMessage_length_E = 0;
                }
            }
            else
            {
                if ((ch == 0x0A) || (ch == 0x0D) || (byteMessage_length_E >= byteMessage_max_Ascii))
                {
                    string dataValue = Encoding.ASCII.GetString(byteMessage_E).Substring(0, byteMessage_length_E);
                    if (ini12.INIRead(MainSettingPath, "Timestamp", "Checked", "") == "1")
                    {
                        DateTime dt = DateTime.Now;
                        dataValue = "[Receive_Port_E] [" + dt.ToString("yyyy/MM/dd HH:mm:ss.fff") + "]  " + dataValue + "\r\n"; //OK
                    }
                    log_process("E", dataValue);
                    log_process("All", dataValue);
                    byteMessage_length_E = 0;
                }
                else
                {
                    byteMessage_E[byteMessage_length_E] = ch;
                    byteMessage_length_E++;
                }
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
                    MYFILE.Write(logA_text);
                    MYFILE.Close();
                    logA_text = String.Empty;
                    break;
                case "B":
                    t = fName + "\\_PortB_" + DateTime.Now.ToString("yyyyMMddHHmmss") + "_" + label_LoopNumber_Value.Text + ".txt";
                    MYFILE = new StreamWriter(t, false, Encoding.ASCII);
                    MYFILE.Write(logB_text);
                    MYFILE.Close();
                    logB_text = String.Empty;
                    break;
                case "C":
                    t = fName + "\\_PortC_" + DateTime.Now.ToString("yyyyMMddHHmmss") + "_" + label_LoopNumber_Value.Text + ".txt";
                    MYFILE = new StreamWriter(t, false, Encoding.ASCII);
                    MYFILE.Write(logC_text);
                    MYFILE.Close();
                    logC_text = String.Empty;
                    break;
                case "D":
                    t = fName + "\\_PortD_" + DateTime.Now.ToString("yyyyMMddHHmmss") + "_" + label_LoopNumber_Value.Text + ".txt";
                    MYFILE = new StreamWriter(t, false, Encoding.ASCII);
                    MYFILE.Write(logD_text);
                    MYFILE.Close();
                    logD_text = String.Empty;
                    break;
                case "E":
                    t = fName + "\\_PortE_" + DateTime.Now.ToString("yyyyMMddHHmmss") + "_" + label_LoopNumber_Value.Text + ".txt";
                    MYFILE = new StreamWriter(t, false, Encoding.ASCII);
                    MYFILE.Write(logE_text);
                    MYFILE.Close();
                    logE_text = String.Empty;
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
                case "Debug":
                    t = fName + "\\_Debug_" + DateTime.Now.ToString("yyyyMMddHHmmss") + "_" + label_LoopNumber_Value.Text + ".txt";
                    MYFILE = new StreamWriter(t, false, Encoding.ASCII);
                    MYFILE.Write(debug_text);
                    MYFILE.Close();
                    debug_text = String.Empty;
                    break;
            }
        }
        #endregion

        double currentTemperature = 0;
        double previousTemperature = -300;
        bool ChamberCheck = false;
        bool PowerSupplyCheck = false;
        bool timer_matched = false;
        
        private void timer_duringShot_Tick(object sender, EventArgs e)
        {
            timer_matched = true;
        }

        #region -- 換行符號置換 --
        private void ReplaceNewLine(SerialPort port, string columns_serial, string columns_switch)
        {
            List<string> originLineList = new List<string> { "\\r", "\\n", "\\r\\n", "\\n\\r" };
            List<string> newLineList = new List<string> { "\r", "\n", "\r\n", "\n\r" };
            var originAndNewLine = originLineList.Zip(newLineList, (o, n) => new { origin = o, newLine = n });
            foreach (var line in originAndNewLine)
            {
                if (columns_switch.Contains(line.origin))
                {
                    port.Write(columns_serial + columns_switch.Replace(line.origin, line.newLine)); //發送數據 Rs232
                }
            }
        }
        #endregion

        #region -- 跑Schedule的指令集 --
        private void MyRunCamd()
        {
            int sRepeat = 0, stime = 0, SysDelay = 0;

            Global.Loop_Number = 1;
            Global.Break_Out_Schedule = 0;
            Global.Pass_Or_Fail = "PASS";

            label_TestTime_Value.Text = "0d 0h 0m 0s 0ms";
            TestTime = 0;

            for (int j = 1; j < Global.Schedule_Loop + 1; j++)
            {
                Global.caption_Num = 0;
                UpdateUI(j.ToString(), label_LoopNumber_Value);
                Global.label_LoopNumber = j.ToString();
                ini12.INIWrite(MailPath, "Data Info", "CreateTime", string.Format("{0:R}", DateTime.Now));

                lock (this)
                {
                    for (Global.Scheduler_Row = 0; Global.Scheduler_Row < DataGridView_Schedule.Rows.Count - 1; Global.Scheduler_Row++)
                    {
                        //Schedule All columns list
                        string columns_command = DataGridView_Schedule.Rows[Global.Scheduler_Row].Cells[0].Value.ToString().Trim();
                        string columns_times = DataGridView_Schedule.Rows[Global.Scheduler_Row].Cells[1].Value.ToString().Trim();
                        string columns_interval = DataGridView_Schedule.Rows[Global.Scheduler_Row].Cells[2].Value.ToString().Trim();
                        string columns_comport = DataGridView_Schedule.Rows[Global.Scheduler_Row].Cells[3].Value.ToString().Trim();
                        string columns_function = DataGridView_Schedule.Rows[Global.Scheduler_Row].Cells[4].Value.ToString().Trim();
                        string columns_subFunction = DataGridView_Schedule.Rows[Global.Scheduler_Row].Cells[5].Value.ToString().Trim();
                        string columns_serial = DataGridView_Schedule.Rows[Global.Scheduler_Row].Cells[6].Value.ToString().Trim();
                        string columns_switch = DataGridView_Schedule.Rows[Global.Scheduler_Row].Cells[7].Value.ToString().Trim();
                        string columns_wait = DataGridView_Schedule.Rows[Global.Scheduler_Row].Cells[8].Value.ToString().Trim();
                        string columns_remark = DataGridView_Schedule.Rows[Global.Scheduler_Row].Cells[9].Value.ToString().Trim();

                        IO_INPUT();//先讀取IO值，避免schedule第一行放IO CMD會出錯//

                        Global.Schedule_Step = Global.Scheduler_Row;
                        if (StartButtonPressed == false)
                        {
                            j = Global.Schedule_Loop;
                            UpdateUI(j.ToString(), label_LoopNumber_Value);
                            Global.label_LoopNumber = j.ToString();
                            break;
                        }

                        Schedule_Time();
                        if (columns_wait != "")
                        {
                            if (columns_wait.Contains('m'))
                            {
                                //Console.WriteLine("Datagridview highlight.");
                                GridUI(Global.Scheduler_Row.ToString(), DataGridView_Schedule);//控制Datagridview highlight//
                                                                                               //Console.WriteLine("Datagridview scollbar.");
                                Gridscroll(Global.Scheduler_Row.ToString(), DataGridView_Schedule);//控制Datagridview scollbar//
                            }
                            else
                            {
                                if (int.Parse(columns_wait) > 500)  //DataGridView UI update 
                                {
                                    //Console.WriteLine("Datagridview highlight.");
                                    GridUI(Global.Scheduler_Row.ToString(), DataGridView_Schedule);//控制Datagridview highlight//
                                                                                                   //Console.WriteLine("Datagridview scollbar.");
                                    Gridscroll(Global.Scheduler_Row.ToString(), DataGridView_Schedule);//控制Datagridview scollbar//
                                }
                            }
                        }

                        if (columns_times != "" && int.TryParse(columns_times, out stime) == true)
                            stime = int.Parse(columns_times); // 次數
                        else
                            stime = 1;

                        if (columns_interval != "" && int.TryParse(columns_interval, out sRepeat) == true)
                            sRepeat = int.Parse(columns_interval); // 停止時間
                        else
                            sRepeat = 0;

                        if (columns_wait != "" && int.TryParse(columns_wait, out SysDelay) == true && columns_wait.Contains('m') == false)
                            SysDelay = int.Parse(columns_wait); // 指令停止時間
                        else if (columns_wait != "" && columns_wait.Contains('m') == true)
                            SysDelay = int.Parse(columns_wait.Replace('m', ' ').Trim()) * 60000; // 指令停止時間(分)
                        else
                            SysDelay = 0;

                        #region -- Record Schedule --
                        string delimiter_recordSch = ",";
                        string Schedule_log = "";
                        DateTime.Now.ToShortTimeString();
                        DateTime sch_dt = DateTime.Now;

                        debug_process("Record Schedule");
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
                        log_process("A", sch_log_text);
                        log_process("B", sch_log_text);
                        log_process("C", sch_log_text);
                        log_process("D", sch_log_text);
                        log_process("E", sch_log_text);
                        log_process("All", sch_log_text);
                        log_process("Canbus", sch_log_text);
                        log_process("KlinePort", sch_log_text);
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
                            debug_process("_shot");
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
                                MessageBox.Show("Camera is not connected!\r\nPlease go to Settings to reload the device list.", "Connection Error");
                                setStyle();
                            }
                            debug_process("Take Picture: _shot_stop");
                        }
                        #endregion

                        #region -- 錄影 --
                        else if (columns_command == "_rec_start")
                        {
                            debug_process("Take Record: _rec_start");
                            if (ini12.INIRead(MainSettingPath, "Device", "CameraExist", "") == "1")
                            {
                                if (Global.VideoRecording == false)
                                {
                                    Mysvideo(); // 開新檔
                                    Global.VideoRecording = true;
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
                            debug_process("Take Record: _rec_stop");
                            if (ini12.INIRead(MainSettingPath, "Device", "CameraExist", "") == "1")
                            {
                                if (Global.VideoRecording == true)       //判斷是不是正在錄影
                                {
                                    Global.VideoRecording = false;
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
                                debug_process("Ascii Log: _PortA");
                                if (columns_serial == "_save")
                                {
                                    Serialportsave("A"); //存檔rs232
                                }
                                else if (columns_serial == "_clear")
                                {
                                    logA_text = string.Empty; //清除logA_text
                                }
                                else if (columns_serial != "" || columns_switch != "")
                                {
                                    ReplaceNewLine(PortA, columns_serial, columns_switch);
                                }
                                else if (columns_serial == "" && columns_switch == "")
                                {
                                    MessageBox.Show("Ascii command is fail, please check the format.");
                                }
                            }

                            if (ini12.INIRead(MainSettingPath, "Port B", "Checked", "") == "1" && columns_comport == "B")
                            {
                                debug_process("Ascii Log: _PortB");
                                if (columns_serial == "_save")
                                {
                                    Serialportsave("B"); //存檔rs232
                                }
                                else if (columns_serial == "_clear")
                                {
                                    logB_text = string.Empty; //清除logB_text
                                }
                                else if (columns_serial != "" || columns_switch != "")
                                {
                                    ReplaceNewLine(PortB, columns_serial, columns_switch);
                                }
                                else if (columns_serial == "" && columns_switch == "")
                                {
                                    MessageBox.Show("Ascii command is fail, please check the format.");
                                }
                            }

                            if (ini12.INIRead(MainSettingPath, "Port C", "Checked", "") == "1" && columns_comport == "C")
                            {
                                debug_process("Ascii Log: _PortC");
                                if (columns_serial == "_save")
                                {
                                    Serialportsave("C"); //存檔rs232
                                }
                                else if (columns_serial == "_clear")
                                {
                                    logC_text = string.Empty; //清除logC_text
                                }
                                else if (columns_serial != "" || columns_switch != "")
                                {
                                    ReplaceNewLine(PortC, columns_serial, columns_switch);
                                }
                                else if (columns_serial == "" && columns_switch == "")
                                {
                                    MessageBox.Show("Ascii command is fail, please check the format.");
                                }
                            }

                            if (ini12.INIRead(MainSettingPath, "Port D", "Checked", "") == "1" && columns_comport == "D")
                            {
                                debug_process("Ascii Log: _PortD");
                                if (columns_serial == "_save")
                                {
                                    Serialportsave("D"); //存檔rs232
                                }
                                else if (columns_serial == "_clear")
                                {
                                    logD_text = string.Empty; //清除logD_text
                                }
                                else if (columns_serial != "" || columns_switch != "")
                                {
                                    ReplaceNewLine(PortD, columns_serial, columns_switch);
                                }
                                else if (columns_serial == "" && columns_switch == "")
                                {
                                    MessageBox.Show("Ascii command is fail, please check the format.");
                                }
                            }

                            if (ini12.INIRead(MainSettingPath, "Port E", "Checked", "") == "1" && columns_comport == "E")
                            {
                                debug_process("Ascii Log: _PortE");
                                if (columns_serial == "_save")
                                {
                                    Serialportsave("E"); //存檔rs232
                                }
                                else if (columns_serial == "_clear")
                                {
                                    logE_text = string.Empty; //清除logE_text
                                }
                                else if (columns_serial != "" || columns_switch != "")
                                {
                                    ReplaceNewLine(PortE, columns_serial, columns_switch);
                                }
                                else if (columns_serial == "" && columns_switch == "")
                                {
                                    MessageBox.Show("Ascii command is fail, please check the format.");
                                }
                            }

                            if (columns_comport == "ALL")
                            {
                                debug_process("Ascii Log: _All");
                                string[] serial_content = columns_serial.Split(new string[] { "|" }, StringSplitOptions.RemoveEmptyEntries);
                                string[] switch_content = columns_switch.Split(new string[] { "|" }, StringSplitOptions.RemoveEmptyEntries);

                                if (columns_serial == "_save")
                                {
                                    Serialportsave("All"); //存檔rs232
                                }
                                else if (columns_serial == "_clear")
                                {
                                    logAll_text = string.Empty; //清除logAll_text
                                }

                                if (ini12.INIRead(MainSettingPath, "Port A", "Checked", "") == "1" && columns_comport == "ALL" && serial_content[0] != "" && switch_content[0] != "")
                                {
                                    ReplaceNewLine(PortA, serial_content[0], switch_content[0]);
                                    DateTime dt = DateTime.Now;
                                    string dataValue = "[Send_Port_A] [" + dt.ToString("yyyy/MM/dd HH:mm:ss.fff") + "]  " + columns_serial + "\r\n";
                                    log_process("A", dataValue);
                                    logAll_text = string.Concat(logAll_text, dataValue);
                                }
                                if (ini12.INIRead(MainSettingPath, "Port B", "Checked", "") == "1" && columns_comport == "ALL" && serial_content[1] != "" && switch_content[1] != "")
                                {
                                    ReplaceNewLine(PortB, serial_content[1], switch_content[1]);
                                    DateTime dt = DateTime.Now;
                                    string dataValue = "[Send_Port_B] [" + dt.ToString("yyyy/MM/dd HH:mm:ss.fff") + "]  " + columns_serial + "\r\n";
                                    log_process("B", dataValue);
                                    logAll_text = string.Concat(logAll_text, dataValue);
                                }
                                if (ini12.INIRead(MainSettingPath, "Port C", "Checked", "") == "1" && columns_comport == "ALL" && serial_content[2] != "" && switch_content[2] != "")
                                {
                                    ReplaceNewLine(PortC, serial_content[2], switch_content[2]);
                                    DateTime dt = DateTime.Now;
                                    string dataValue = "[Send_Port_C] [" + dt.ToString("yyyy/MM/dd HH:mm:ss.fff") + "]  " + columns_serial + "\r\n";
                                    log_process("C", dataValue);
                                    log_process("All", dataValue);
                                }
                                if (ini12.INIRead(MainSettingPath, "Port D", "Checked", "") == "1" && columns_comport == "ALL" && serial_content[3] != "" && switch_content[3] != "")
                                {
                                    ReplaceNewLine(PortD, serial_content[3], switch_content[3]);
                                    DateTime dt = DateTime.Now;
                                    string dataValue = "[Send_Port_D] [" + dt.ToString("yyyy/MM/dd HH:mm:ss.fff") + "]  " + columns_serial + "\r\n";
                                    log_process("D", dataValue);
                                    log_process("All", dataValue);
                                }
                                if (ini12.INIRead(MainSettingPath, "Port E", "Checked", "") == "1" && columns_comport == "ALL" && serial_content[4] != "" && switch_content[4] != "")
                                {
                                    ReplaceNewLine(PortE, serial_content[4], switch_content[4]);
                                    DateTime dt = DateTime.Now;
                                    string dataValue = "[Send_Port_E] [" + dt.ToString("yyyy/MM/dd HH:mm:ss.fff") + "]  " + columns_serial + "\r\n";
                                    log_process("E", dataValue);
                                    log_process("All", dataValue);
                                }
                            }
                            label_Command.Text = "(" + columns_command + ") " + columns_serial;
                        }
                        #endregion

                        #region -- Execute --
                        else if (columns_command == "_Execute")
                        {
                            //If voltage matched the expected value
                            if (PowerSupplyCheck)
                            {
                                IO_CMD();
                            }
                            PowerSupplyCheck = false;

                            if (columns_serial == "_pause")
                            {
                                foreach (Temperature_Data item in temperatureList)
                                {
                                    item.temperaturePause = true;
                                }
                            }
                            else if (columns_serial == "_shot")
                            {
                                foreach (Temperature_Data item in temperatureList)
                                {
                                    item.temperatureShot = true;
                                }
                            }
                            else if (columns_comport == "A" || columns_comport == "B" || columns_comport == "C" || columns_comport == "D" || columns_comport == "E")
                            {
                                foreach (Temperature_Data item in temperatureList)
                                {
                                    item.temperaturePort = columns_comport;
                                    item.temperatureLog = columns_serial;
                                    item.temperatureNewline = columns_switch;
                                }
                            }
                        }

                        #endregion

                        #region -- Condition_AND --
                        else if (columns_command == "_Condition_AND")
                        {
                            //if (columns_command.Substring(13) == "1")
                            //{
                            if (ini12.INIRead(MainSettingPath, "Port A", "Checked", "") == "1" ||
                                ini12.INIRead(MainSettingPath, "Port B", "Checked", "") == "1" ||
                                ini12.INIRead(MainSettingPath, "Port C", "Checked", "") == "1" ||
                                ini12.INIRead(MainSettingPath, "Port D", "Checked", "") == "1" ||
                                ini12.INIRead(MainSettingPath, "Port E", "Checked", "") == "1")
                            {
                                if (columns_function == "start")
                                {
                                    ifStatementFlag = true;
                                    expectedVoltage = "";
                                    if (columns_serial != "")
                                    {
                                        columns_serial.Replace(" ", "");
                                        if (columns_serial.Contains("chamber_temp"))
                                        {
                                            ChamberIsFound = true;
                                            Temperature_Data.initialTemperature = Int16.Parse(columns_serial.Substring(columns_serial.IndexOf("=") + 1, columns_serial.IndexOf("~") - columns_serial.IndexOf("=") - 1));
                                            Temperature_Data.finalTemperature = Int16.Parse(columns_serial.Substring(columns_serial.IndexOf("~") + 1, columns_serial.IndexOf("/") - columns_serial.IndexOf("~") - 1));
                                            if (columns_serial.Contains("/-"))
                                            {
                                                Temperature_Data.addTemperature = float.Parse("-" + columns_serial.Substring(columns_serial.IndexOf("-") + 1));
                                            }
                                            else
                                            {
                                                Temperature_Data.addTemperature = float.Parse(columns_serial.Substring(columns_serial.IndexOf("+") + 1));
                                            }
                                        }
                                        else if (columns_serial.Contains("PowerSupply_Voltage"))
                                        {
                                            PowerSupplyIsFound = true;
                                            expectedVoltage = columns_serial.Substring(columns_serial.IndexOf("=") + 1);

                                            string powerCommand = "MEASure1:ALL?"; //Read Power Supply information
                                            ReplaceNewLine(PortA, powerCommand, columns_switch);

                                            //Append Power Supply command to log
                                            DateTime dt = DateTime.Now;
                                            PowerSupplyCommandLog = "[Send_Port_A] [" + dt.ToString("yyyy/MM/dd HH:mm:ss.fff") + "]  " + powerCommand + "\r\n";
                                            logA_text = string.Concat(logA_text, PowerSupplyCommandLog);
                                        }
                                        else if (columns_serial.Contains("Temperature"))
                                        {
                                            try
                                            {
                                                //	lt：less than 小於
                                                //	le：less than or equal to 小於等於
                                                //	eq：equal to 等於
                                                //	ne：not equal to 不等於
                                                //	ge：greater than or equal to 大於等於
                                                //	gt：greater than 大於

                                                TemperatureIsFound = true;
                                                int symbel_equal_7e = columns_serial.IndexOf("~");
                                                int symbel_equal_28 = columns_serial.IndexOf("(");
                                                int symbel_equal_29 = columns_serial.IndexOf(")");
                                                int symbel_equal_6d29 = columns_serial.IndexOf("m)");
                                                int symbel_equal_3d = columns_serial.IndexOf("=");
                                                int symbel_equal_3c = columns_serial.IndexOf("<");
                                                int symbel_equal_3e = columns_serial.IndexOf(">");
                                                int symbel_equal_3d3d = columns_serial.IndexOf("==");
                                                int symbel_equal_3c3e = columns_serial.IndexOf("<>");
                                                int symbel_equal_3c3d = columns_serial.IndexOf("<=");
                                                int symbel_equal_3e3d = columns_serial.IndexOf(">=");
                                                int duringTimeInt = 0;
                                                int parameter_equal_Temperature = columns_serial.IndexOf("Temperature");

                                                if (columns_serial.Contains("~") && columns_serial.Contains("<>") && columns_serial.Contains("/") == false)
                                                {
                                                    MaxTemperature = string.Format("{0:0.0}", columns_serial.Substring(symbel_equal_3c3e + 1, symbel_equal_7e - symbel_equal_3c3e - 1));
                                                    MinTemperature = string.Format("{0:0.0}", columns_serial.Substring(symbel_equal_7e + 1, symbel_equal_28 - symbel_equal_7e - 1));
                                                }
                                                else if (columns_serial.Contains("~") && columns_serial.Contains("=") && columns_serial.Contains("/") == false)
                                                {
                                                    MinTemperature = string.Format("{0:0.0}", columns_serial.Substring(symbel_equal_3d + 1, symbel_equal_7e - symbel_equal_3d - 1));
                                                    MaxTemperature = string.Format("{0:0.0}", columns_serial.Substring(symbel_equal_7e + 1, symbel_equal_28 - symbel_equal_7e - 1));
                                                }
                                                else if (columns_serial.Contains("~") == false && columns_serial.Contains("/") == false)
                                                {
                                                    if (columns_serial.Contains("<"))
                                                        MinTemperature = string.Format("{0:0.0}", columns_serial.Substring(symbel_equal_3c + 1, symbel_equal_28 - symbel_equal_3c - 1));
                                                    else if (columns_serial.Contains("<="))
                                                        MinTemperature = string.Format("{0:0.0}", columns_serial.Substring(symbel_equal_3c3d + 1, symbel_equal_28 - symbel_equal_3c3d - 1));
                                                    else if (columns_serial.Contains("=="))
                                                        MinTemperature = string.Format("{0:0.0}", columns_serial.Substring(symbel_equal_3d3d + 1, symbel_equal_28 - symbel_equal_3d3d - 1));
                                                    else if (columns_serial.Contains("<>"))
                                                        MinTemperature = string.Format("{0:0.0}", columns_serial.Substring(symbel_equal_3c3e + 1, symbel_equal_28 - symbel_equal_3c3e - 1));
                                                    else if (columns_serial.Contains(">"))
                                                        MinTemperature = string.Format("{0:0.0}", columns_serial.Substring(symbel_equal_3e + 1, symbel_equal_28 - symbel_equal_3e - 1));
                                                    else if (columns_serial.Contains(">="))
                                                        MinTemperature = string.Format("{0:0.0}", columns_serial.Substring(symbel_equal_3e3d + 1, symbel_equal_28 - symbel_equal_3e3d - 1));
                                                }
                                                
                                                string string_temperatureChannel = columns_serial.Substring(parameter_equal_Temperature + 11, symbel_equal_3d - parameter_equal_Temperature - 11);

                                                if (columns_serial.Contains("m)"))
                                                    duringTimeInt = Int16.Parse(columns_serial.Substring(symbel_equal_28 + 1, symbel_equal_6d29 - symbel_equal_28 - 1)) * 60000;
                                                else
                                                    duringTimeInt = Int16.Parse(columns_serial.Substring(symbel_equal_28 + 1, symbel_equal_29 - symbel_equal_28 - 1));

                                                byte temperatureChannel = Convert.ToByte(int.Parse(string_temperatureChannel) + 48);

                                                if (duringTimeInt > 0)
                                                {
                                                    // Create a timer and set a two second interval.
                                                    timer_duringShot.Interval = duringTimeInt;

                                                    // Start the timer
                                                    timer_duringShot.Start();
                                                }
                                            }
                                            catch (Exception Ex)
                                            {
                                                MessageBox.Show(Ex.Message.ToString(), "Temperature data parameter error!");
                                            }
                                        }
                                    }
                                }
                                else if (columns_function == "end")
                                {
                                    ifStatementFlag = false;

                                    PowerSupplyCheck = false;
                                    PowerSupplyIsFound = false;

                                    ChamberCheck = false;
                                    ChamberIsFound = false;

                                    timer_duringShot.Stop();

                                    foreach (Temperature_Data item in temperatureList)
                                    {
                                        item.temperaturePause = false;
                                        item.temperatureShot = false;
                                    }

                                }
                            }
                            //}
                        }
                        #endregion

                        #region -- Condition_OR --
                        else if (columns_command == "_Condition_OR")
                        {
                            //if (columns_command.Substring(13) == "1")
                            //{
                                if (ini12.INIRead(MainSettingPath, "Port A", "Checked", "") == "1" ||
                                    ini12.INIRead(MainSettingPath, "Port B", "Checked", "") == "1" ||
                                    ini12.INIRead(MainSettingPath, "Port C", "Checked", "") == "1" ||
                                    ini12.INIRead(MainSettingPath, "Port D", "Checked", "") == "1" ||
                                    ini12.INIRead(MainSettingPath, "Port E", "Checked", "") == "1")
                                {
                                    if (columns_function == "start")
                                    {
                                        ifStatementFlag = true;
                                        expectedVoltage = "";
                                        if (columns_serial != "")
                                        {
                                            columns_serial.Replace(" ", "");
                                            if (columns_serial.Contains("chamber_temp"))
                                            {
                                                ChamberIsFound = true;
                                                Temperature_Data.initialTemperature = Int16.Parse(columns_serial.Substring(columns_serial.IndexOf("=") + 1, columns_serial.IndexOf("~") - columns_serial.IndexOf("=") - 1));
                                                Temperature_Data.finalTemperature = Int16.Parse(columns_serial.Substring(columns_serial.IndexOf("~") + 1, columns_serial.IndexOf("/") - columns_serial.IndexOf("~") - 1));
                                                if (columns_serial.Contains("/-"))
                                                {
                                                    Temperature_Data.addTemperature = float.Parse("-" + columns_serial.Substring(columns_serial.IndexOf("-") + 1));
                                                }
                                                else
                                                {
                                                    Temperature_Data.addTemperature = float.Parse(columns_serial.Substring(columns_serial.IndexOf("+") + 1));
                                                }
                                            }
                                            else if (columns_serial.Contains("PowerSupply_Voltage"))
                                            {
                                                PowerSupplyIsFound = true;
                                                expectedVoltage = columns_serial.Substring(columns_serial.IndexOf("=") + 1);

                                                string powerCommand = "MEASure1:ALL?"; //Read Power Supply information
                                                ReplaceNewLine(PortA, powerCommand, columns_switch);

                                                //Append Power Supply command to log
                                                DateTime dt = DateTime.Now;
                                                PowerSupplyCommandLog = "[Send_Port_A] [" + dt.ToString("yyyy/MM/dd HH:mm:ss.fff") + "]  " + powerCommand + "\r\n";
                                                logA_text = string.Concat(logA_text, PowerSupplyCommandLog);
                                            }
                                            else if (columns_serial.Contains("Temperature"))
                                            {
                                                try
                                                {
                                                    //	lt：less than 小於
                                                    //	le：less than or equal to 小於等於
                                                    //	eq：equal to 等於
                                                    //	ne：not equal to 不等於
                                                    //	ge：greater than or equal to 大於等於
                                                    //	gt：greater than 大於

                                                    TemperatureIsFound = true;
                                                    temperatureList.Clear();
                                                    int symbel_equal_3d = columns_serial.IndexOf("=");
                                                    int symbel_equal_7e = columns_serial.IndexOf("~");
                                                    int symbel_equal_2f = columns_serial.IndexOf("/");
                                                    int symbel_equal_28 = columns_serial.IndexOf("(");
                                                    int symbel_equal_29 = columns_serial.IndexOf(")");
                                                    int symbel_equal_6d29 = columns_serial.IndexOf("m)");
                                                    int symbel_equal_3c = columns_serial.IndexOf("<");
                                                    int symbel_equal_3e = columns_serial.IndexOf(">");
                                                    int symbel_equal_3d3d = columns_serial.IndexOf("==");
                                                    int symbel_equal_3c3e = columns_serial.IndexOf("<>");
                                                    int symbel_equal_3c3d = columns_serial.IndexOf("<=");
                                                    int symbel_equal_3e3d = columns_serial.IndexOf(">=");
                                                    int duringTimeInt = 0;
                                                    int parameter_equal_Temperature = columns_serial.IndexOf("Temperature");
                                                    string initialTemperature = "", finalTemperature = "", addTemperature = "", symbel_equal_Math = "";
                                                    string temperatureChannel = columns_serial.Substring(parameter_equal_Temperature + 11, symbel_equal_3d - parameter_equal_Temperature - 11);

                                                    if (columns_serial.Contains("~") && columns_serial.Contains("/"))
                                                    {
                                                        initialTemperature = string.Format("{0:0.00}", columns_serial.Substring(symbel_equal_3d + 1, symbel_equal_7e - symbel_equal_3d - 1));
                                                        finalTemperature = string.Format("{0:0.00}", columns_serial.Substring(symbel_equal_7e + 1, symbel_equal_2f - symbel_equal_7e - 1));
                                                        addTemperature = string.Format("{0:0.00}", columns_serial.Substring(symbel_equal_2f + 1, symbel_equal_28 - symbel_equal_2f - 1));
                                                    }
                                                    else if (columns_serial.Contains("~") && columns_serial.Contains("<>") && columns_serial.Contains("/") == false)
                                                    {
                                                        initialTemperature = string.Format("{0:0.0}", columns_serial.Substring(symbel_equal_3c3e + 1, symbel_equal_7e - symbel_equal_3c3e - 1));
                                                        finalTemperature = string.Format("{0:0.0}", columns_serial.Substring(symbel_equal_7e + 1, symbel_equal_28 - symbel_equal_7e - 1));
                                                        symbel_equal_Math = "<>";
                                                    }
                                                    else if (columns_serial.Contains("~") && columns_serial.Contains("=") && columns_serial.Contains("/") == false)
                                                    {
                                                        finalTemperature = string.Format("{0:0.0}", columns_serial.Substring(symbel_equal_3d + 1, symbel_equal_7e - symbel_equal_3d - 1));
                                                        initialTemperature = string.Format("{0:0.0}", columns_serial.Substring(symbel_equal_7e + 1, symbel_equal_28 - symbel_equal_7e - 1));
                                                        symbel_equal_Math = "==";
                                                    }
                                                    else if (columns_serial.Contains("~") == false && columns_serial.Contains("/") == false)
                                                    {
                                                        if (columns_serial.Contains("<"))
                                                        {
                                                            symbel_equal_Math = "<";
                                                            finalTemperature = string.Format("{0:0.0}", columns_serial.Substring(symbel_equal_3c + 1, symbel_equal_28 - symbel_equal_3c - 1));
                                                        }
                                                        else if (columns_serial.Contains("<="))
                                                        {
                                                            symbel_equal_Math = "<=";
                                                            finalTemperature = string.Format("{0:0.0}", columns_serial.Substring(symbel_equal_3c3d + 1, symbel_equal_28 - symbel_equal_3c3d - 1));
                                                        }
                                                        else if (columns_serial.Contains("=="))
                                                        {
                                                            symbel_equal_Math = "==";
                                                            finalTemperature = string.Format("{0:0.0}", columns_serial.Substring(symbel_equal_3d3d + 1, symbel_equal_28 - symbel_equal_3d3d - 1));
                                                        }
                                                        else if (columns_serial.Contains("<>"))
                                                        {
                                                            symbel_equal_Math = "<>";
                                                            finalTemperature = string.Format("{0:0.0}", columns_serial.Substring(symbel_equal_3c3e + 1, symbel_equal_28 - symbel_equal_3c3e - 1));
                                                        }
                                                        else if (columns_serial.Contains(">"))
                                                        {
                                                            symbel_equal_Math = ">";
                                                            finalTemperature = string.Format("{0:0.0}", columns_serial.Substring(symbel_equal_3e + 1, symbel_equal_28 - symbel_equal_3e - 1));
                                                        }
                                                        else if (columns_serial.Contains(">="))
                                                        {
                                                            symbel_equal_Math = ">=";
                                                            finalTemperature = string.Format("{0:0.0}", columns_serial.Substring(symbel_equal_3e3d + 1, symbel_equal_28 - symbel_equal_3e3d - 1));
                                                        }
                                                    }

                                                    if (columns_serial.Contains("m)"))
                                                        duringTimeInt = Int16.Parse(columns_serial.Substring(symbel_equal_28 + 1, symbel_equal_6d29 - symbel_equal_28 - 1)) * 60000;
                                                    else
                                                        duringTimeInt = Int16.Parse(columns_serial.Substring(symbel_equal_28 + 1, symbel_equal_29 - symbel_equal_28 - 1));

                                                    if (columns_serial.Contains("~"))
                                                        Temperature_Data.initialTemperature = float.Parse(initialTemperature);
                                                    Temperature_Data.finalTemperature = float.Parse(finalTemperature);
                                                    Temperature_Data.temperatureChannel = Convert.ToByte(int.Parse(temperatureChannel) + 48);
                                                    if (columns_serial.Contains("~") && columns_serial.Contains("/"))
                                                    {
                                                        Temperature_Data.addTemperature = float.Parse(addTemperature);
                                                    }
                                                    float addTemperatureInt = Temperature_Data.addTemperature;

                                                    if (duringTimeInt > 0)
                                                    {
                                                        // Create a timer and set a two second interval.
                                                        timer_duringShot.Interval = duringTimeInt;

                                                        // Start the timer
                                                        timer_duringShot.Start();
                                                    }

                                                    if (addTemperatureInt < 0)
                                                    {
                                                        for (float i = Temperature_Data.initialTemperature; i >= Temperature_Data.finalTemperature; i += addTemperatureInt)
                                                        {
                                                            double conditionList = Convert.ToDouble(string.Format("{0:0.0}", i));
                                                            temperatureList.Add(new Temperature_Data(conditionList, false, false, "", "", ""));
                                                        }
                                                    }
                                                    else if (addTemperatureInt >= 0)
                                                    {
                                                        for (float i = Temperature_Data.initialTemperature; i <= Temperature_Data.finalTemperature; i += addTemperatureInt)
                                                        {
                                                            double conditionList = Convert.ToDouble(string.Format("{0:0.0}", i));
                                                            temperatureList.Add(new Temperature_Data(conditionList, false, false, "", "", ""));
                                                        }
                                                    }
                                                    else
                                                    {
                                                        switch (symbel_equal_Math)
                                                        {
                                                            case "<":

                                                                break;
                                                            case "<=":

                                                                break;
                                                            case "==":

                                                                break;
                                                            case "<>":

                                                                break;
                                                            case ">":

                                                                break;
                                                            case "=>":

                                                                break;
                                                        }
                                                    }
                                                }
                                                catch(Exception Ex)
                                                {
                                                    MessageBox.Show(Ex.Message.ToString(), "Temperature data parameter error!");
                                                }
                                            }
                                        }
                                    }
                                    else if (columns_function == "end")
                                    {
                                        ifStatementFlag = false;

                                        PowerSupplyCheck = false;
                                        PowerSupplyIsFound = false;

                                        ChamberCheck = false;
                                        ChamberIsFound = false;

                                        timer_duringShot.Stop();

                                        temperatureList.Clear();

                                    }
                                }
                            //}
                        }
                        #endregion

                        #region -- Hex --
                        else if (columns_command == "_HEX")
                        {
                            string Outputstring = "";
                            if (ini12.INIRead(MainSettingPath, "Port A", "Checked", "") == "1" && columns_comport == "A")
                            {
                                debug_process("Hex Log: _PortA");
                                if (columns_serial == "_save")
                                {
                                    Serialportsave("A"); //存檔rs232
                                }
                                else if (columns_serial == "_clear")
                                {
                                    logA_text = string.Empty; //清除logA_text
                                }
                                else if (columns_serial != "_save" &&
                                         columns_serial != "_clear" &&
                                         columns_serial != "" &&
                                         columns_function == "CRC16_Modbus")
                                {
                                    string orginal_data = columns_serial;
                                    string crc16_data = Crc16.PID_CRC16(orginal_data);
                                    Outputstring = orginal_data + crc16_data;
                                    byte[] Outputbytes = new byte[Outputstring.Split(' ').Count()];
                                    Outputbytes = HexConverter.StrToByte(Outputstring);
                                    PortA.Write(Outputbytes, 0, Outputbytes.Length); //發送數據 Rs232 + Crc16
                                }
                                else if (columns_serial != "_save" &&
                                         columns_serial != "_clear" &&
                                         columns_serial != "" &&
                                         columns_function == "")
                                {
                                    string hexValues = columns_serial;
                                    byte[] Outputbytes = new byte[hexValues.Split(' ').Count()];
                                    Outputbytes = HexConverter.StrToByte(hexValues);
                                    PortA.Write(Outputbytes, 0, Outputbytes.Length); //發送數據 Rs232
                                }
                                DateTime dt = DateTime.Now;
                                string dataValue = "[Send_Port_A] [" + dt.ToString("yyyy/MM/dd HH:mm:ss.fff") + "]  " + Outputstring + "\r\n";
                                log_process("A", dataValue);
                                log_process("All", dataValue);
                            }

                            if (ini12.INIRead(MainSettingPath, "Port B", "Checked", "") == "1" && columns_comport == "B")
                            {
                                debug_process("Hex Log: _PortB");
                                if (columns_serial == "_save")
                                {
                                    Serialportsave("B"); //存檔rs232
                                }
                                else if (columns_serial == "_clear")
                                {
                                    logB_text = string.Empty; //清除logB_text
                                }
                                else if (columns_serial != "_save" &&
                                         columns_serial != "_clear" &&
                                         columns_serial != "" &&
                                         columns_function == "CRC16_Modbus")
                                {
                                    string orginal_data = columns_serial;
                                    string crc16_data = Crc16.PID_CRC16(orginal_data);
                                    Outputstring = orginal_data + crc16_data;
                                    byte[] Outputbytes = new byte[Outputstring.Split(' ').Count()];
                                    Outputbytes = HexConverter.StrToByte(Outputstring);
                                    PortB.Write(Outputbytes, 0, Outputbytes.Length); //發送數據 Rs232 + Crc16
                                }
                                else if (columns_serial != "_save" &&
                                         columns_serial != "_clear" &&
                                         columns_serial != "" &&
                                         columns_function == "")
                                {
                                    string hexValues = columns_serial;
                                    byte[] Outputbytes = new byte[hexValues.Split(' ').Count()];
                                    Outputbytes = HexConverter.StrToByte(hexValues);
                                    PortB.Write(Outputbytes, 0, Outputbytes.Length); //發送數據 Rs232
                                }
                                DateTime dt = DateTime.Now;
                                string dataValue = "[Send_Port_B] [" + dt.ToString("yyyy/MM/dd HH:mm:ss.fff") + "]  " + Outputstring + "\r\n";
                                log_process("B", dataValue);
                                log_process("All", dataValue);
                            }

                            if (ini12.INIRead(MainSettingPath, "Port C", "Checked", "") == "1" && columns_comport == "C")
                            {
                                debug_process("Hex Log: _PortC");
                                if (columns_serial == "_save")
                                {
                                    Serialportsave("C"); //存檔rs232
                                }
                                else if (columns_serial == "_clear")
                                {
                                    logC_text = string.Empty; //清除logC_text
                                }
                                else if (columns_serial != "_save" &&
                                         columns_serial != "_clear" &&
                                         columns_serial != "" &&
                                         columns_function == "CRC16_Modbus")
                                {
                                    string orginal_data = columns_serial;
                                    string crc16_data = Crc16.PID_CRC16(orginal_data);
                                    Outputstring = orginal_data + crc16_data;
                                    byte[] Outputbytes = new byte[Outputstring.Split(' ').Count()];
                                    Outputbytes = HexConverter.StrToByte(Outputstring);
                                    PortC.Write(Outputbytes, 0, Outputbytes.Length); //發送數據 Rs232 + Crc16
                                }
                                else if (columns_serial != "_save" &&
                                         columns_serial != "_clear" &&
                                         columns_serial != "" &&
                                         columns_function == "")
                                {
                                    string hexValues = columns_serial;
                                    byte[] Outputbytes = new byte[hexValues.Split(' ').Count()];
                                    Outputbytes = HexConverter.StrToByte(hexValues);
                                    PortC.Write(Outputbytes, 0, Outputbytes.Length); //發送數據 Rs232
                                }
                                DateTime dt = DateTime.Now;
                                string dataValue = "[Send_Port_C] [" + dt.ToString("yyyy/MM/dd HH:mm:ss.fff") + "]  " + Outputstring + "\r\n";
                                log_process("C", dataValue);
                                log_process("All", dataValue);
                            }

                            if (ini12.INIRead(MainSettingPath, "Port D", "Checked", "") == "1" && columns_comport == "D")
                            {
                                debug_process("Hex Log: _PortD");
                                if (columns_serial == "_save")
                                {
                                    Serialportsave("D"); //存檔rs232
                                }
                                else if (columns_serial == "_clear")
                                {
                                    logD_text = string.Empty; //清除logD_text
                                }
                                else if (columns_serial != "_save" &&
                                         columns_serial != "_clear" &&
                                         columns_serial != "" &&
                                         columns_function == "CRC16_Modbus")
                                {
                                    string orginal_data = columns_serial;
                                    string crc16_data = Crc16.PID_CRC16(orginal_data);
                                    Outputstring = orginal_data + crc16_data;
                                    byte[] Outputbytes = new byte[Outputstring.Split(' ').Count()];
                                    Outputbytes = HexConverter.StrToByte(Outputstring);
                                    PortD.Write(Outputbytes, 0, Outputbytes.Length); //發送數據 Rs232 + Crc16
                                }
                                else if (columns_serial != "_save" &&
                                         columns_serial != "_clear" &&
                                         columns_serial != "" &&
                                         columns_function == "")
                                {
                                    string hexValues = columns_serial;
                                    byte[] Outputbytes = new byte[hexValues.Split(' ').Count()];
                                    Outputbytes = HexConverter.StrToByte(hexValues);
                                    PortD.Write(Outputbytes, 0, Outputbytes.Length); //發送數據 Rs232
                                }
                                DateTime dt = DateTime.Now;
                                string dataValue = "[Send_Port_D] [" + dt.ToString("yyyy/MM/dd HH:mm:ss.fff") + "]  " + Outputstring + "\r\n";
                                log_process("D", dataValue);
                                log_process("All", dataValue);
                            }

                            if (ini12.INIRead(MainSettingPath, "Port E", "Checked", "") == "1" && columns_comport == "E")
                            {
                                debug_process("Hex Log: _PortE");
                                if (columns_serial == "_save")
                                {
                                    Serialportsave("E"); //存檔rs232
                                }
                                else if (columns_serial == "_clear")
                                {
                                    logE_text = string.Empty; //清除logE_text
                                }
                                else if (columns_serial != "_save" &&
                                         columns_serial != "_clear" &&
                                         columns_serial != "" &&
                                         columns_function == "CRC16_Modbus")
                                {
                                    string orginal_data = columns_serial;
                                    string crc16_data = Crc16.PID_CRC16(orginal_data);
                                    Outputstring = orginal_data + crc16_data;
                                    byte[] Outputbytes = new byte[Outputstring.Split(' ').Count()];
                                    Outputbytes = HexConverter.StrToByte(Outputstring);
                                    PortE.Write(Outputbytes, 0, Outputbytes.Length); //發送數據 Rs232 + Crc16
                                }
                                else if (columns_serial != "_save" &&
                                         columns_serial != "_clear" &&
                                         columns_serial != "" &&
                                         columns_function == "")
                                {
                                    string hexValues = columns_serial;
                                    byte[] Outputbytes = new byte[hexValues.Split(' ').Count()];
                                    Outputbytes = HexConverter.StrToByte(hexValues);
                                    PortE.Write(Outputbytes, 0, Outputbytes.Length); //發送數據 Rs232
                                }
                                DateTime dt = DateTime.Now;
                                string dataValue = "[Send_Port_E] [" + dt.ToString("yyyy/MM/dd HH:mm:ss.fff") + "]  " + Outputstring + "\r\n";
                                log_process("E", dataValue);
                                log_process("All", dataValue);
                            }

                            if (columns_comport == "ALL")
                            {
                                debug_process("Hex Log: _All");
                                string[] serial_content = columns_serial.Split(new string[] { "|" }, StringSplitOptions.RemoveEmptyEntries);

                                if (columns_serial == "_save")
                                {
                                    Serialportsave("All"); //存檔rs232
                                }
                                else if (columns_serial == "_clear")
                                {
                                    logAll_text = string.Empty; //清除logAll_text
                                }

                                if (ini12.INIRead(MainSettingPath, "Port A", "Checked", "") == "1" && columns_comport == "ALL" && serial_content[0] != "")
                                {
                                    string orginal_data = serial_content[0];
                                    if (columns_function == "CRC16_Modbus")
                                    {
                                        string crc16_data = Crc16.PID_CRC16(orginal_data);
                                        Outputstring = orginal_data + crc16_data;
                                        byte[] Outputbytes = new byte[Outputstring.Split(' ').Count()];
                                        Outputbytes = HexConverter.StrToByte(Outputstring);
                                        PortA.Write(Outputbytes, 0, Outputbytes.Length); //發送數據 Rs232 + Crc16
                                    }
                                    else
                                    {
                                        Outputstring = orginal_data;
                                        byte[] Outputbytes = serial_content[0].Split(' ').Select(s => Convert.ToByte(s, 16)).ToArray();
                                        PortA.Write(Outputbytes, 0, Outputbytes.Length); //發送數據 Rs232
                                    }
                                    DateTime dt = DateTime.Now;
                                    string dataValue = "[Send_Port_A] [" + dt.ToString("yyyy/MM/dd HH:mm:ss.fff") + "]  " + Outputstring + "\r\n";
                                    log_process("A", dataValue);
                                    log_process("All", dataValue);
                                }
                                if (ini12.INIRead(MainSettingPath, "Port B", "Checked", "") == "1" && columns_comport == "ALL" && serial_content[1] != "")
                                {
                                    string orginal_data = serial_content[1];
                                    if (columns_function == "CRC16_Modbus")
                                    {
                                        string crc16_data = Crc16.PID_CRC16(orginal_data);
                                        Outputstring = orginal_data + crc16_data;
                                        byte[] Outputbytes = new byte[Outputstring.Split(' ').Count()];
                                        Outputbytes = HexConverter.StrToByte(Outputstring);
                                        PortB.Write(Outputbytes, 0, Outputbytes.Length); //發送數據 Rs232 + Crc16
                                    }
                                    else
                                    {
                                        Outputstring = orginal_data;
                                        byte[] Outputbytes = serial_content[1].Split(' ').Select(s => Convert.ToByte(s, 16)).ToArray();
                                        PortB.Write(Outputbytes, 0, Outputbytes.Length); //發送數據 Rs232
                                    }
                                    DateTime dt = DateTime.Now;
                                    string dataValue = "[Send_Port_B] [" + dt.ToString("yyyy/MM/dd HH:mm:ss.fff") + "]  " + Outputstring + "\r\n";
                                    log_process("B", dataValue);
                                    log_process("All", dataValue);
                                }
                                if (ini12.INIRead(MainSettingPath, "Port C", "Checked", "") == "1" && columns_comport == "ALL" && serial_content[2] != "")
                                {
                                    string orginal_data = serial_content[2];
                                    if (columns_function == "CRC16_Modbus")
                                    {
                                        string crc16_data = Crc16.PID_CRC16(orginal_data);
                                        Outputstring = orginal_data + crc16_data;
                                        byte[] Outputbytes = new byte[Outputstring.Split(' ').Count()];
                                        Outputbytes = HexConverter.StrToByte(Outputstring);
                                        PortC.Write(Outputbytes, 0, Outputbytes.Length); //發送數據 Rs232 + Crc16
                                    }
                                    else
                                    {
                                        Outputstring = orginal_data;
                                        byte[] Outputbytes = serial_content[2].Split(' ').Select(s => Convert.ToByte(s, 16)).ToArray();
                                        PortC.Write(Outputbytes, 0, Outputbytes.Length); //發送數據 Rs232
                                    }
                                    DateTime dt = DateTime.Now;
                                    string dataValue = "[Send_Port_C] [" + dt.ToString("yyyy/MM/dd HH:mm:ss.fff") + "]  " + Outputstring + "\r\n";
                                    log_process("C", dataValue);
                                    log_process("All", dataValue);
                                }
                                if (ini12.INIRead(MainSettingPath, "Port D", "Checked", "") == "1" && columns_comport == "ALL" && serial_content[3] != "")
                                {
                                    string orginal_data = serial_content[3];
                                    if (columns_function == "CRC16_Modbus")
                                    {
                                        string crc16_data = Crc16.PID_CRC16(orginal_data);
                                        Outputstring = orginal_data + crc16_data;
                                        byte[] Outputbytes = new byte[Outputstring.Split(' ').Count()];
                                        Outputbytes = HexConverter.StrToByte(Outputstring);
                                        PortD.Write(Outputbytes, 0, Outputbytes.Length); //發送數據 Rs232 + Crc16
                                    }
                                    else
                                    {
                                        Outputstring = orginal_data;
                                        byte[] Outputbytes = serial_content[3].Split(' ').Select(s => Convert.ToByte(s, 16)).ToArray();
                                        PortD.Write(Outputbytes, 0, Outputbytes.Length); //發送數據 Rs232
                                    }
                                    DateTime dt = DateTime.Now;
                                    string dataValue = "[Send_Port_D] [" + dt.ToString("yyyy/MM/dd HH:mm:ss.fff") + "]  " + Outputstring + "\r\n";
                                    log_process("D", dataValue);
                                    log_process("All", dataValue);
                                }
                                if (ini12.INIRead(MainSettingPath, "Port E", "Checked", "") == "1" && columns_comport == "ALL" && serial_content[4] != "")
                                {
                                    string orginal_data = serial_content[4];
                                    if (columns_function == "CRC16_Modbus")
                                    {
                                        string crc16_data = Crc16.PID_CRC16(orginal_data);
                                        Outputstring = orginal_data + crc16_data;
                                        byte[] Outputbytes = new byte[Outputstring.Split(' ').Count()];
                                        Outputbytes = HexConverter.StrToByte(Outputstring);
                                        PortE.Write(Outputbytes, 0, Outputbytes.Length); //發送數據 Rs232 + Crc16
                                    }
                                    else
                                    {
                                        Outputstring = orginal_data;
                                        byte[] Outputbytes = serial_content[4].Split(' ').Select(s => Convert.ToByte(s, 16)).ToArray();
                                        PortE.Write(Outputbytes, 0, Outputbytes.Length); //發送數據 Rs232
                                    }
                                    DateTime dt = DateTime.Now;
                                    string dataValue = "[Send_Port_E] [" + dt.ToString("yyyy/MM/dd HH:mm:ss.fff") + "]  " + serial_content[4] + "\r\n";
                                    log_process("E", dataValue);
                                    log_process("All", dataValue);
                                }
                            }
                            label_Command.Text = "(" + columns_command + ") " + Outputstring;
                        }
                        #endregion

                        #region -- K-Line --
                        else if (columns_command == "_K_ABS")
                        {
                            debug_process("K-line control: _K_ABS");
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
                            debug_process("K-line control: _K_OBD");
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

                        #region -- I2C Read --
                        else if (columns_command == "_TX_I2C_Read")
                        {
                            if (ini12.INIRead(MainSettingPath, "Port A", "Checked", "") == "1" && columns_comport == "A")
                            {
                                debug_process("I2C Read Log: _TX_I2C_Read_PortA");
                                if (columns_times != "" && columns_function != "")
                                {
                                    string orginal_data = columns_times + " " + columns_function + " " + "20";
                                    string crc32_data = Crc32.I2C_CRC32(orginal_data);
                                    string Outputstring = "79 6D " + columns_times + " 06 " + columns_function + " 20 " + crc32_data;
                                    byte[] Outputbytes = new byte[Outputstring.Split(' ').Count()];
                                    Outputbytes = HexConverter.StrToByte(Outputstring);
                                    PortA.Write(Outputbytes, 0, Outputbytes.Length); //發送數據 Rs232
                                    DateTime dt = DateTime.Now;
                                    string dataValue = "[Send_Port_A] [" + dt.ToString("yyyy/MM/dd HH:mm:ss.fff") + "]  " + Outputstring + "\r\n";
                                    log_process("A", dataValue);
                                    log_process("All", dataValue);
                                }
                            }

                            if (ini12.INIRead(MainSettingPath, "Port B", "Checked", "") == "1" && columns_comport == "B")
                            {
                                debug_process("I2C Read Log: _TX_I2C_Read_PortB");
                                if (columns_times != "" && columns_function != "")
                                {
                                    string orginal_data = columns_times + " " + columns_function + " " + "20";
                                    string crc32_data = Crc32.I2C_CRC32(orginal_data);
                                    string Outputstring = "79 6D " + columns_times + " 06 " + columns_function + " 20 " + crc32_data;
                                    byte[] Outputbytes = new byte[Outputstring.Split(' ').Count()];
                                    Outputbytes = HexConverter.StrToByte(Outputstring);
                                    PortB.Write(Outputbytes, 0, Outputbytes.Length); //發送數據 Rs232
                                    DateTime dt = DateTime.Now;
                                    string dataValue = "[Send_Port_B] [" + dt.ToString("yyyy/MM/dd HH:mm:ss.fff") + "]  " + Outputstring + "\r\n";
                                    log_process("B", dataValue);
                                    log_process("All", dataValue);
                                }
                            }

                            if (ini12.INIRead(MainSettingPath, "Port C", "Checked", "") == "1" && columns_comport == "C")
                            {
                                debug_process("I2C Read Log: _TX_I2C_Read_PortC");
                                if (columns_times != "" && columns_function != "")
                                {
                                    string orginal_data = columns_times + " " + columns_function + " " + "20";
                                    string crc32_data = Crc32.I2C_CRC32(orginal_data);
                                    string Outputstring = "79 6D " + columns_times + " 06 " + columns_function + " 20 " + crc32_data;
                                    byte[] Outputbytes = new byte[Outputstring.Split(' ').Count()];
                                    Outputbytes = HexConverter.StrToByte(Outputstring);
                                    PortC.Write(Outputbytes, 0, Outputbytes.Length); //發送數據 Rs232
                                    DateTime dt = DateTime.Now;
                                    string dataValue = "[Send_Port_C] [" + dt.ToString("yyyy/MM/dd HH:mm:ss.fff") + "]  " + Outputstring + "\r\n";
                                    log_process("C", dataValue);
                                    log_process("All", dataValue);
                                }
                            }

                            if (ini12.INIRead(MainSettingPath, "Port D", "Checked", "") == "1" && columns_comport == "D")
                            {
                                debug_process("I2C Read Log: _TX_I2C_Read_PortD");
                                if (columns_times != "" && columns_function != "")
                                {
                                    string orginal_data = columns_times + " " + columns_function + " " + "20";
                                    string crc32_data = Crc32.I2C_CRC32(orginal_data);
                                    string Outputstring = "79 6D " + columns_times + " 06 " + columns_function + " 20 " + crc32_data;
                                    byte[] Outputbytes = new byte[Outputstring.Split(' ').Count()];
                                    Outputbytes = HexConverter.StrToByte(Outputstring);
                                    PortD.Write(Outputbytes, 0, Outputbytes.Length); //發送數據 Rs232
                                    DateTime dt = DateTime.Now;
                                    string dataValue = "[Send_Port_D] [" + dt.ToString("yyyy/MM/dd HH:mm:ss.fff") + "]  " + Outputstring + "\r\n";
                                    log_process("D", dataValue);
                                    log_process("All", dataValue);
                                }
                            }

                            if (ini12.INIRead(MainSettingPath, "Port E", "Checked", "") == "1" && columns_comport == "E")
                            {
                                debug_process("I2C Read Log: _TX_I2C_Read_PortE");
                                if (columns_times != "" && columns_function != "")
                                {
                                    string orginal_data = columns_times + " " + columns_function + " " + "20";
                                    string crc32_data = Crc32.I2C_CRC32(orginal_data);
                                    string Outputstring = "79 6D " + columns_times + " 06 " + columns_function + " 20 " + crc32_data;
                                    byte[] Outputbytes = new byte[Outputstring.Split(' ').Count()];
                                    Outputbytes = HexConverter.StrToByte(Outputstring);
                                    PortE.Write(Outputbytes, 0, Outputbytes.Length); //發送數據 Rs232
                                    DateTime dt = DateTime.Now;
                                    string dataValue = "[Send_Port_E] [" + dt.ToString("yyyy/MM/dd HH:mm:ss.fff") + "]  " + Outputstring + "\r\n";
                                    log_process("E", dataValue);
                                    log_process("All", dataValue);
                                }
                            }
                        }
                        #endregion

                        #region -- I2C Write --
                        else if (columns_command == "_TX_I2C_Write")
                        {
                            if (ini12.INIRead(MainSettingPath, "Port A", "Checked", "") == "1" && columns_comport == "A")
                            {
                                debug_process("I2C Write Log: _TX_I2C_Write_PortA");
                                if (columns_function != "" && columns_subFunction != "")
                                {
                                    int Data_length = columns_subFunction.Split(' ').Count();
                                    string orginal_data = (Data_length + 1).ToString("X2") + " " + columns_function + " " + columns_subFunction + " 20";
                                    string crc32_data = Crc32.I2C_CRC32(orginal_data);
                                    string Outputstring = "79 6C " + (Data_length + 1).ToString("X2") + " " + (Data_length + 6).ToString("X2") + " " + columns_function + " " + columns_subFunction + " 20 " + crc32_data;
                                    byte[] Outputbytes = new byte[Outputstring.Split(' ').Count()];
                                    Outputbytes = HexConverter.StrToByte(Outputstring);
                                    PortA.Write(Outputbytes, 0, Outputbytes.Length); //發送數據 Rs232
                                    DateTime dt = DateTime.Now;
                                    string dataValue = "[Send_Port_A] [" + dt.ToString("yyyy/MM/dd HH:mm:ss.fff") + "]  " + Outputstring + "\r\n";
                                    log_process("A", dataValue);
                                    log_process("All", dataValue);
                                }
                            }

                            if (ini12.INIRead(MainSettingPath, "Port B", "Checked", "") == "1" && columns_comport == "B")
                            {
                                debug_process("I2C Write Log: _TX_I2C_Write_PortB");
                                if (columns_function != "" && columns_subFunction != "")
                                {
                                    int Data_length = columns_subFunction.Split(' ').Count();
                                    string orginal_data = (Data_length + 1).ToString("X2") + " " + columns_function + " " + columns_subFunction + " 20";
                                    string crc32_data = Crc32.I2C_CRC32(orginal_data);
                                    string Outputstring = "79 6C " + (Data_length + 1).ToString("X2") + " " + (Data_length + 6).ToString("X2") + " " + columns_function + " " + columns_subFunction + " 20 " + crc32_data;
                                    byte[] Outputbytes = new byte[Outputstring.Split(' ').Count()];
                                    Outputbytes = HexConverter.StrToByte(Outputstring);
                                    PortB.Write(Outputbytes, 0, Outputbytes.Length); //發送數據 Rs232
                                    DateTime dt = DateTime.Now;
                                    string dataValue = "[Send_Port_B] [" + dt.ToString("yyyy/MM/dd HH:mm:ss.fff") + "]  " + Outputstring + "\r\n";
                                    log_process("B", dataValue);
                                    log_process("All", dataValue);
                                }
                            }

                            if (ini12.INIRead(MainSettingPath, "Port C", "Checked", "") == "1" && columns_comport == "C")
                            {
                                debug_process("I2C Write Log: _TX_I2C_Write_PortC");
                                if (columns_function != "" && columns_subFunction != "")
                                {
                                    int Data_length = columns_subFunction.Split(' ').Count();
                                    string orginal_data = (Data_length + 1).ToString("X2") + " " + columns_function + " " + columns_subFunction + " 20";
                                    string crc32_data = Crc32.I2C_CRC32(orginal_data);
                                    string Outputstring = "79 6C " + (Data_length + 1).ToString("X2") + " " + (Data_length + 6).ToString("X2") + " " + columns_function + " " + columns_subFunction + " 20 " + crc32_data;
                                    byte[] Outputbytes = new byte[Outputstring.Split(' ').Count()];
                                    Outputbytes = HexConverter.StrToByte(Outputstring);
                                    PortC.Write(Outputbytes, 0, Outputbytes.Length); //發送數據 Rs232
                                    DateTime dt = DateTime.Now;
                                    string dataValue = "[Send_Port_C] [" + dt.ToString("yyyy/MM/dd HH:mm:ss.fff") + "]  " + Outputstring + "\r\n";
                                    log_process("C", dataValue);
                                    log_process("All", dataValue);
                                }
                            }

                            if (ini12.INIRead(MainSettingPath, "Port D", "Checked", "") == "1" && columns_comport == "D")
                            {
                                debug_process("I2C Write Log: _TX_I2C_Write_PortD");
                                if (columns_function != "" && columns_subFunction != "")
                                {
                                    int Data_length = columns_subFunction.Split(' ').Count();
                                    string orginal_data = (Data_length + 1).ToString("X2") + " " + columns_function + " " + columns_subFunction + " 20";
                                    string crc32_data = Crc32.I2C_CRC32(orginal_data);
                                    string Outputstring = "79 6C " + (Data_length + 1).ToString("X2") + " " + (Data_length + 6).ToString("X2") + " " + columns_function + " " + columns_subFunction + " 20 " + crc32_data;
                                    byte[] Outputbytes = new byte[Outputstring.Split(' ').Count()];
                                    Outputbytes = HexConverter.StrToByte(Outputstring);
                                    PortD.Write(Outputbytes, 0, Outputbytes.Length); //發送數據 Rs232
                                    DateTime dt = DateTime.Now;
                                    string dataValue = "[Send_Port_D] [" + dt.ToString("yyyy/MM/dd HH:mm:ss.fff") + "]  " + Outputstring + "\r\n";
                                    log_process("D", dataValue);
                                    log_process("All", dataValue);
                                }
                            }

                            if (ini12.INIRead(MainSettingPath, "Port E", "Checked", "") == "1" && columns_comport == "E")
                            {
                                debug_process("I2C Write Log: _TX_I2C_Write_PortE");
                                if (columns_function != "" && columns_subFunction != "")
                                {
                                    int Data_length = columns_subFunction.Split(' ').Count();
                                    string orginal_data = (Data_length + 1).ToString("X2") + " " + columns_function + " " + columns_subFunction + " 20";
                                    string crc32_data = Crc32.I2C_CRC32(orginal_data);
                                    string Outputstring = "79 6C " + (Data_length + 1).ToString("X2") + " " + (Data_length + 6).ToString("X2") + " " + columns_function + " " + columns_subFunction + " 20 " + crc32_data;
                                    byte[] Outputbytes = new byte[Outputstring.Split(' ').Count()];
                                    Outputbytes = HexConverter.StrToByte(Outputstring);
                                    PortE.Write(Outputbytes, 0, Outputbytes.Length); //發送數據 Rs232
                                    DateTime dt = DateTime.Now;
                                    string dataValue = "[Send_Port_E] [" + dt.ToString("yyyy/MM/dd HH:mm:ss.fff") + "]  " + Outputstring + "\r\n";
                                    log_process("E", dataValue);
                                    log_process("All", dataValue);
                                }
                            }
                        }
                        #endregion

                        #region -- Canbus Send --
                        else if (columns_command == "_Canbus_Send")
                        {
                            if (ini12.INIRead(MainSettingPath, "Device", "UsbCANExist", "") == "1" && ini12.INIRead(MainSettingPath, "Canbus", "Device", "") == "UsbCAN")
                            {
                                if (columns_times != "" && columns_interval == "" && columns_serial != "")
                                {
                                    debug_process("Canbus Send (Event): _Canbus_Send");
                                    byte[] Outputdata = new byte[columns_serial.Split(' ').Count()];
                                    Outputdata = HexConverter.StrToByte(columns_serial);
                                    Can_Usb2C.TransmitData(Convert.ToUInt32(columns_times), Outputdata);

                                    string Outputstring = "ID: 0x";
                                    Outputstring += columns_times + " Data: " + columns_serial;
                                    DateTime dt = DateTime.Now;
                                    string canbus_log_text = "[Send_Canbus] [" + dt.ToString("yyyy/MM/dd HH:mm:ss.fff") + "]  " + Outputstring + "\r\n";
                                    log_process("Canbus", canbus_log_text);
                                    log_process("All", canbus_log_text);
                                }
                                else if (columns_times != "" && columns_interval != "" && columns_serial != "")
                                {
                                    set_timer_rate = true;
                                    can_id = System.Convert.ToUInt16("0x" + columns_times, 16);
                                    byte[] Outputdata = new byte[columns_serial.Split(' ').Count()];
                                    Outputdata = HexConverter.StrToByte(columns_serial);
                                    if (can_rate.Count > 0)
                                    {
                                        foreach (var OneItem in can_rate)
                                        {
                                            if (can_rate.ContainsKey(can_id))
                                            {
                                                can_rate[can_id] = Convert.ToUInt32(columns_interval);
                                                can_data[can_id] = Outputdata;
                                                break;
                                            }
                                            else
                                            {
                                                can_rate.Add(can_id, Convert.ToUInt32(columns_interval));
                                                can_data.Add(can_id, Outputdata);
                                                UsbCAN_Count = 0;
                                                UsbCAN_Delay(Convert.ToInt16(columns_interval));
                                            }
                                        }
                                    }
                                    else
                                    {
                                        can_rate.Add(can_id, Convert.ToUInt32(columns_interval));
                                        can_data.Add(can_id, Outputdata);
                                        UsbCAN_Count = 0;
                                        UsbCAN_Delay(Convert.ToInt16(columns_interval));
                                    }
                                }
                            }
                            else if (ini12.INIRead(MainSettingPath, "Device", "CAN1630AExist", "") == "1" && ini12.INIRead(MainSettingPath, "Canbus", "Device", "") == "Vector")
                            {
                                if (columns_times != "" && columns_interval == "" && columns_serial != "")
                                {
                                    debug_process("Canbus Send: Vector_Canbus_once");
                                    byte[] Outputdata = new byte[columns_serial.Split(' ').Count()];
                                    Outputdata = HexConverter.StrToByte(columns_serial);
                                    Can_1630A.LoopCANTransmit(Convert.ToUInt32(columns_times), Convert.ToUInt32(columns_interval), Outputdata);

                                    string Outputstring = "ID: 0x";
                                    Outputstring += columns_times + " Data: " + columns_serial;
                                    DateTime dt = DateTime.Now;
                                    string canbus_log_text = "[Send_Canbus] [" + dt.ToString("yyyy/MM/dd HH:mm:ss.fff") + "]  " + Outputstring + "\r\n";
                                    log_process("Canbus", canbus_log_text);
                                    log_process("All", canbus_log_text);
                                }
                                else if (columns_times != "" && columns_interval != "" && columns_serial != "")
                                {
                                    debug_process("Canbus Send: Vector_Canbus_loop");
                                    set_timer_rate = true;
                                    can_id = System.Convert.ToUInt16("0x" + columns_times, 16);
                                    byte[] Outputdata = new byte[columns_serial.Split(' ').Count()];
                                    Outputdata = HexConverter.StrToByte(columns_serial);
                                    if (can_rate.Count > 0)
                                    {
                                        foreach (var OneItem in can_rate)
                                        {
                                            if (can_rate.ContainsKey(can_id))
                                            {
                                                can_rate[can_id] = Convert.ToUInt32(columns_interval);
                                                can_data[can_id] = Outputdata;
                                                break;
                                            }
                                            else
                                            {
                                                can_rate.Add(can_id, Convert.ToUInt32(columns_interval));
                                                can_data.Add(can_id, Outputdata);
                                                //VectorCAN_Count = 0;
                                                //VectorCAN_Delay(Convert.ToInt16(columns_interval));
                                                Thread CanSetTimeRate = new Thread(new ThreadStart(vectorcanloop));
                                                CanSetTimeRate.Start();
                                            }
                                        }
                                    }
                                    else
                                    {
                                        can_rate.Add(can_id, Convert.ToUInt32(columns_interval));
                                        can_data.Add(can_id, Outputdata);
                                        //VectorCAN_Count = 0;
                                        //VectorCAN_Delay(Convert.ToInt16(columns_interval));
                                        Thread CanSetTimeRate = new Thread(new ThreadStart(vectorcanloop));
                                        CanSetTimeRate.Start();
                                    }
                                }
                            }
                            label_Command.Text = "(" + columns_command + ") " + columns_serial;
                        }
                        #endregion

                        #region -- Canbus Queue --
                        else if (columns_command == "_Canbus_Queue")
                        {
                            if (ini12.INIRead(MainSettingPath, "Device", "UsbCANExist", "") == "1" && ini12.INIRead(MainSettingPath, "Canbus", "Device", "") == "UsbCAN")
                            {
                                if (columns_times != "" && columns_interval != "" && columns_serial != "")
                                {
                                    debug_process("Canbus Write: UsbCAN_Canbus_Queue_data");
                                    byte[] Outputbytes = new byte[columns_serial.Split(' ').Count()];
                                    Outputbytes = HexConverter.StrToByte(columns_serial);
                                    can_data_list.Add(new USB_CAN2C.CAN_Data(System.Convert.ToUInt16("0x" + columns_times, 16), System.Convert.ToUInt32(columns_interval), Outputbytes, Convert.ToByte(columns_serial.Split(' ').Count())));
                                }
                                else if (columns_function == "send")
                                {
                                    debug_process("Canbus Write: UsbCAN_Canbus_Queue_send");
                                    can_send = 1;
                                }
                                else if (columns_function == "clear")
                                {
                                    debug_process("Canbus Write: UsbCAN_Canbus_Queue_clean");
                                    can_send = 0;
                                    can_data_list.Clear();
                                }
                            }
                            else if (ini12.INIRead(MainSettingPath, "Device", "CAN1630AExist", "") == "1" && ini12.INIRead(MainSettingPath, "Canbus", "Device", "") == "Vector")
                            {
                                if (columns_times != "" && columns_interval != "" && columns_serial != "")
                                {
                                    debug_process("Canbus Write: Vector_Canbus_Queue_data");
                                    byte[] Outputbytes = new byte[columns_serial.Split(' ').Count()];
                                    Outputbytes = HexConverter.StrToByte(columns_serial);
                                    can_data_list.Add(new USB_CAN2C.CAN_Data(System.Convert.ToUInt16("0x" + columns_times, 16), System.Convert.ToUInt32(columns_interval), Outputbytes, Convert.ToByte(columns_serial.Split(' ').Count())));
                                }
                                else if (columns_function == "send")
                                {
                                    debug_process("Canbus Write: Vector_Canbus_Queue_send");
                                    can_send = 1;
                                }
                                else if (columns_function == "clear")
                                {
                                    debug_process("Canbus Write: Vector_Canbus_Queue_clean");
                                    can_send = 0;
                                    can_data_list.Clear();
                                }
                            }
                            label_Command.Text = "(" + columns_command + ") " + columns_serial;
                        }
                        #endregion

                        #region -- 命令提示 --
                        else if (columns_command == "_DOS")
                        {
                            debug_process("DOS command: _DOS");
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
                            debug_process("GPIO control: _IO_Input");
                            IO_INPUT();
                        }

                        else if (columns_command == "_IO_Output")
                        {
                            debug_process("GPIO control: _IO_Output");
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
                            debug_process("Extend GPIO control: _WaterTemp");
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
                            debug_process("Extend GPIO control: _FuelDisplay");
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
                            debug_process("Extend GPIO control: _Temperature");
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
                                    debug_process("Extend GPIO control: _FuncKey:" + k + " times");
                                    label_Command.Text = "(Push CMD)" + columns_serial;
                                    if (ini12.INIRead(MainSettingPath, "Port A", "Checked", "") == "1" && columns_comport == "A")
                                    {
                                        if (columns_serial == "_save")
                                        {
                                            Serialportsave("A"); //存檔rs232
                                        }
                                        else if (columns_serial == "_clear")
                                        {
                                            logA_text = string.Empty; //清除textbox1
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
                                        string dataValue = "[Send_Port_A] [" + dt.ToString("yyyy/MM/dd HH:mm:ss.fff") + "]  " + columns_serial + "\r\n";
                                        log_process("A", dataValue);
                                        log_process("All", dataValue);
                                    }
                                    else if (ini12.INIRead(MainSettingPath, "Port B", "Checked", "") == "1" && columns_comport == "B")
                                    {
                                        if (columns_serial == "_save")
                                        {
                                            Serialportsave("B"); //存檔rs232
                                        }
                                        else if (columns_serial == "_clear")
                                        {
                                            logB_text = string.Empty; //清除logB_text
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
                                        string dataValue = "[Send_Port_B] [" + dt.ToString("yyyy/MM/dd HH:mm:ss.fff") + "]  " + columns_serial + "\r\n";
                                        log_process("B", dataValue);
                                        log_process("All", dataValue);
                                    }
                                    else if (ini12.INIRead(MainSettingPath, "Port C", "Checked", "") == "1" && columns_comport == "C")
                                    {
                                        if (columns_serial == "_save")
                                        {
                                            Serialportsave("C"); //存檔rs232
                                        }
                                        else if (columns_serial == "_clear")
                                        {
                                            logC_text = string.Empty; //清除logC_text
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
                                        string dataValue = "[Send_Port_C] [" + dt.ToString("yyyy/MM/dd HH:mm:ss.fff") + "]  " + columns_serial + "\r\n";
                                        log_process("C", dataValue);
                                        log_process("All", dataValue);
                                    }
                                    else if (ini12.INIRead(MainSettingPath, "Port D", "Checked", "") == "1" && columns_comport == "D")
                                    {
                                        if (columns_serial == "_save")
                                        {
                                            Serialportsave("D"); //存檔rs232
                                        }
                                        else if (columns_serial == "_clear")
                                        {
                                            logD_text = string.Empty; //清除logD_text
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
                                        string dataValue = "[Send_Port_D] [" + dt.ToString("yyyy/MM/dd HH:mm:ss.fff") + "]  " + columns_serial + "\r\n";
                                        log_process("D", dataValue);
                                        log_process("All", dataValue);
                                    }
                                    else if (ini12.INIRead(MainSettingPath, "Port E", "Checked", "") == "1" && columns_comport == "E")
                                    {
                                        if (columns_serial == "_save")
                                        {
                                            Serialportsave("E"); //存檔rs232
                                        }
                                        else if (columns_serial == "_clear")
                                        {
                                            logE_text = string.Empty; //清除logE_text
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
                                        string dataValue = "[Send_Port_E] [" + dt.ToString("yyyy/MM/dd HH:mm:ss.fff") + "]  " + columns_serial + "\r\n";
                                        log_process("E", dataValue);
                                        log_process("All", dataValue);
                                    }
                                    //label_Command.Text = "(" + columns_command + ") " + columns_serial;
                                    debug_process("Extend GPIO control: _FuncKey Delay:" + sRepeat + " ms");

                                    RedRatDBViewer_Delay(sRepeat);
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
                                            logA_text = string.Empty; //清除textbox1
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
                                        string dataValue = "[Send_Port_A] [" + dt.ToString("yyyy/MM/dd HH:mm:ss.fff") + "]  " + reverse + "\r\n";
                                        log_process("A", dataValue);
                                        log_process("All", dataValue);
                                    }
                                    else if (ini12.INIRead(MainSettingPath, "Port B", "Checked", "") == "1" && columns_comport == "B")
                                    {
                                        if (reverse == "_save")
                                        {
                                            Serialportsave("B"); //存檔rs232
                                        }
                                        else if (reverse == "_clear")
                                        {
                                            logB_text = string.Empty; //清除logB_text
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
                                        string dataValue = "[Send_Port_B] [" + dt.ToString("yyyy/MM/dd HH:mm:ss.fff") + "]  " + reverse + "\r\n";
                                        log_process("B", dataValue);
                                        log_process("All", dataValue);
                                    }
                                    else if (ini12.INIRead(MainSettingPath, "Port C", "Checked", "") == "1" && columns_comport == "C")
                                    {
                                        if (reverse == "_save")
                                        {
                                            Serialportsave("C"); //存檔rs232
                                        }
                                        else if (reverse == "_clear")
                                        {
                                            logC_text = string.Empty; //清除logC_text
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
                                        string dataValue = "[Send_Port_C] [" + dt.ToString("yyyy/MM/dd HH:mm:ss.fff") + "]  " + reverse + "\r\n";
                                        log_process("C", dataValue);
                                        log_process("All", dataValue);
                                    }
                                    else if (ini12.INIRead(MainSettingPath, "Port D", "Checked", "") == "1" && columns_comport == "D")
                                    {
                                        if (reverse == "_save")
                                        {
                                            Serialportsave("D"); //存檔rs232
                                        }
                                        else if (reverse == "_clear")
                                        {
                                            logD_text = string.Empty; //清除logD_text
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
                                        string dataValue = "[Send_Port_D] [" + dt.ToString("yyyy/MM/dd HH:mm:ss.fff") + "]  " + reverse + "\r\n";
                                        log_process("D", dataValue);
                                        log_process("All", dataValue);
                                    }
                                    else if (ini12.INIRead(MainSettingPath, "Port E", "Checked", "") == "1" && columns_comport == "E")
                                    {
                                        if (reverse == "_save")
                                        {
                                            Serialportsave("E"); //存檔rs232
                                        }
                                        else if (reverse == "_clear")
                                        {
                                            logE_text = string.Empty; //清除logE_text
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
                                        string dataValue = "[Send_Port_E] [" + dt.ToString("yyyy/MM/dd HH:mm:ss.fff") + "]  " + reverse + "\r\n";
                                        log_process("E", dataValue);
                                        log_process("All", dataValue);
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
                                        debug_process("IO CMD: PA10");
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
                                        debug_process("IO CMD: PA11");
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
                                        debug_process("IO CMD: PA14");
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
                                        debug_process("IO CMD: PA15");
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
                                        debug_process("IO CMD: PB01");
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
                                        debug_process("IO CMD: PB07");
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

                        #region -- Remark --
                        if (columns_remark != "")
                        {
                            label_Remark.Invoke((MethodInvoker)(() => label_Remark.Text = columns_remark));
                        }
                        else
                        {
                            label_Remark.Text = "";
                        }
                        #endregion

                        debug_process("CloseTime record.");
                        ini12.INIWrite(MailPath, "Data Info", "CloseTime", string.Format("{0:R}", DateTime.Now));

                        if (Global.Break_Out_Schedule == 1)//定時器時間到跳出迴圈//
                        {
                            debug_process("Break schedule.");
                            j = Global.Schedule_Loop;
                            UpdateUI(j.ToString(), label_LoopNumber_Value);
                            Global.label_LoopNumber = j.ToString();
                            break;
                        }

                        if (Pause == true)//如果按下暫停鈕//
                        {
                            timer1.Stop();
                            SchedulePause.WaitOne();
                            debug_process("SchedulePause_WaitOne");
                        }
                        else
                        {
                            RedRatDBViewer_Delay(SysDelay);
                            debug_process("RedRatDBViewer_Delay: " + SysDelay + ",\r\n");
                        }

                        debug_process("End.");
                    }
                }
                debug_process("Loop_Number: " + Global.Loop_Number + ", \r\n");
                Serialportsave("Debug");
				DisposeRam();
                Global.Loop_Number++;
            }


            #region -- Each video record when completed the schedule --
            if (ini12.INIRead(MainSettingPath, "Record", "EachVideo", "") == "1")
            {
                if (StartButtonPressed == true)
                {
                    if (ini12.INIRead(MainSettingPath, "Device", "CameraExist", "") == "1")
                    {
                        if (Global.VideoRecording == false)
                        {
                            label_Command.Text = "Record Video...";
                            Thread.Sleep(1500);
                            Mysvideo(); // 開新檔
                            Global.VideoRecording = true;
                            Thread oThreadC = new Thread(new ThreadStart(MySrtCamd));
                            oThreadC.Start();
                            Thread.Sleep(60000); // 錄影60秒

                            Global.VideoRecording = false;
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
                    timer_duringShot.Stop();
                    MyRunCamd();
                }
                else if (
                    Global.Schedule_3_Exist == 1 && Global.Schedule_Number == 1 ||
                    Global.Schedule_3_Exist == 1 && Global.Schedule_Number == 2)
                {
                    button_Schedule3.PerformClick();
                    timer_duringShot.Stop();
                    MyRunCamd();
                }
                else if (
                    Global.Schedule_4_Exist == 1 && Global.Schedule_Number == 1 ||
                    Global.Schedule_4_Exist == 1 && Global.Schedule_Number == 2 ||
                    Global.Schedule_4_Exist == 1 && Global.Schedule_Number == 3)
                {
                    button_Schedule4.PerformClick();
                    timer_duringShot.Stop();
                    MyRunCamd();
                }
                else if (
                    Global.Schedule_5_Exist == 1 && Global.Schedule_Number == 1 ||
                    Global.Schedule_5_Exist == 1 && Global.Schedule_Number == 2 ||
                    Global.Schedule_5_Exist == 1 && Global.Schedule_Number == 3 ||
                    Global.Schedule_5_Exist == 1 && Global.Schedule_Number == 4)
                {
                    button_Schedule5.PerformClick();
                    timer_duringShot.Stop();
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
                if (ini12.INIRead(MailPath, "Send Mail", "value", "") == "1")
                {
                    Global.Loop_Number = Global.Loop_Number - 1;
                    FormMail FormMail = new FormMail();
                    FormMail.send();
                }
            }

            label_Command.Text = "Completed!";
            label_Remark.Text = "";
            button_Schedule1.PerformClick();
            timer1.Stop();
            timer_duringShot.Stop();
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
            if (Can_Usb2C.Connect() == 1)
            {
                Can_Usb2C.StopCAN();
                Can_Usb2C.Disconnect();
            }

            timeCount = Global.Schedule_1_TestTime;
            ConvertToRealTime(timeCount);
            Global.Scheduler_Row = 0;
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
            else if (columns_serial.Substring(0, 7) == "_logcmd")
            {
                String log_cmd = columns_serial;
                int startIndex = 10;
                int length = log_cmd.Length - 10;
                String log_cmd_substring = log_cmd.Substring(startIndex, length);
                String log_cmd_serialport = log_cmd.Substring(8, 1);

                if (ini12.INIRead(MainSettingPath, "Port A", "Checked", "") == "1" && log_cmd_serialport == "A")
                {
                    PortA.WriteLine(log_cmd_substring);
                }
                else if (ini12.INIRead(MainSettingPath, "Port B", "Checked", "") == "1" && log_cmd_serialport == "B")
                {
                    PortB.WriteLine(log_cmd_substring);
                }
                else if (ini12.INIRead(MainSettingPath, "Port C", "Checked", "") == "1" && log_cmd_serialport == "C")
                {
                    PortC.WriteLine(log_cmd_substring);
                }
                else if (ini12.INIRead(MainSettingPath, "Port D", "Checked", "") == "1" && log_cmd_serialport == "D")
                {
                    PortD.WriteLine(log_cmd_substring);
                }
                else if (ini12.INIRead(MainSettingPath, "Port E", "Checked", "") == "1" && log_cmd_serialport == "E")
                {
                    PortE.WriteLine(log_cmd_substring);
                }
                else if (log_cmd_serialport == "O")
                {
                    if (ini12.INIRead(MainSettingPath, "Port A", "Checked", "") == "1")
                        PortA.WriteLine(log_cmd_substring);
                    if (ini12.INIRead(MainSettingPath, "Port B", "Checked", "") == "1")
                        PortB.WriteLine(log_cmd_substring);
                    if (ini12.INIRead(MainSettingPath, "Port C", "Checked", "") == "1")
                        PortC.WriteLine(log_cmd_substring);
                    if (ini12.INIRead(MainSettingPath, "Port D", "Checked", "") == "1")
                        PortD.WriteLine(log_cmd_substring);
                    if (ini12.INIRead(MainSettingPath, "Port E", "Checked", "") == "1")
                        PortE.WriteLine(log_cmd_substring);
                }
            }
        }
        #endregion

        #region -- 拍照 --
        private void Jes() => Invoke(new EventHandler(delegate { Myshot(); }));

        private void Myshot()
        {
            debug_process("Start Myshot");
            button_Start.Enabled = false;
            setStyle();
            capture.FrameEvent2 += new Capture.HeFrame(CaptureDone);
            capture.GrapImg();
            debug_process("Stop Myshot");
        }

        // 複製原始圖片
        protected Bitmap CloneBitmap(Bitmap source)
        {
            debug_process("CloneBitmap");
            return new Bitmap(source);
        }

        private void CaptureDone(System.Drawing.Bitmap e)
        {
            debug_process("CaptureDone");
            capture.FrameEvent2 -= new Capture.HeFrame(CaptureDone);
            string fName = ini12.INIRead(MainSettingPath, "Record", "VideoPath", "");
            //string ngFolder = "Schedule" + Global.Schedule_Num + "_NG";

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
                bitMap_g.DrawString(DataGridView_Schedule.Rows[Global.Schedule_Step].Cells[9].Value.ToString(),
                                Font,
                                FontColor,
                                new PointF(5, YPoint - 120));
                bitMap_g.DrawString(DataGridView_Schedule.Rows[Global.Schedule_Step].Cells[0].Value.ToString() + "  ( " + label_Command.Text + " )",
                                Font,
                                FontColor,
                                new PointF(5, YPoint - 80));
            }
            else
            {
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
            debug_process("Save the CaptureDone Picture");
            button_Start.Enabled = true;
            setStyle();
            debug_process("Stop the CaptureDone function");
        }
        #endregion

        #region -- 字幕 --
        private void MySrtCamd()
        {
            int count = 1;
            string starttime = "0:0:0";
            TimeSpan time_start = TimeSpan.Parse(DateTime.Now.ToString("HH:mm:ss"));

            while (Global.VideoRecording)
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

        #region -- 錄影 --
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

            string t = fName + "\\" + "_rec" + DateTime.Now.ToString("yyyyMMddHHmmss") + "__" + label_LoopNumber_Value.Text + ".avi";
            srtstring = fName + "\\" + "_rec" + DateTime.Now.ToString("yyyyMMddHHmmss") + "__" + label_LoopNumber_Value.Text + ".srt";

            if (!capture.Cued)
                capture.Filename = t;

            capture.RecFileMode = DirectX.Capture.Capture.RecFileModeType.Avi; //宣告我要avi檔格式
            capture.Cue(); // 創一個檔
            capture.Start(); // 開始錄影
        }
        #endregion

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
            try
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

                int scam, saud, VideoNum, AudioNum = 0;
                if (ini12.INIRead(MainSettingPath, "Camera", "VideoIndex", "") == "")
                    scam = 0;
                else
                    scam = int.Parse(ini12.INIRead(MainSettingPath, "Camera", "VideoIndex", ""));

                if (ini12.INIRead(MainSettingPath, "Camera", "AudioIndex", "") == "")
                    saud = 0;
                else
                    saud = int.Parse(ini12.INIRead(MainSettingPath, "Camera", "AudioIndex", ""));

                if (ini12.INIRead(MainSettingPath, "Camera", "VideoNumber", "") == "")
                    VideoNum = 0;
                else
                    VideoNum = int.Parse(ini12.INIRead(MainSettingPath, "Camera", "VideoNumber", ""));

                if (ini12.INIRead(MainSettingPath, "Camera", "AudioNumber", "") == "")
                    AudioNum = 0;
                else
                    AudioNum = int.Parse(ini12.INIRead(MainSettingPath, "Camera", "AudioNumber", ""));

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
            catch (NotSupportedException)
            {
                MessageBox.Show("Camera is disconnected unexpectedly!\r\nPlease go to Settings to reload the device list.", "Connection Error");
                button_Start.PerformClick();
            };
        }

        #region -- 讀取RC DB並填入combobox --
        private void LoadRCDB()
        {
            DataGridViewComboBoxColumn RCDB = (DataGridViewComboBoxColumn)DataGridView_Schedule.Columns[0];

            RCDB.Items.Add("_Execute");
            //RCDB.Items.Add("_Condition_AND");
            RCDB.Items.Add("_Condition_OR");
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
            RCDB.Items.Add("_TX_I2C_Read");
            RCDB.Items.Add("_TX_I2C_Write");
            RCDB.Items.Add("_Canbus_Send");
            RCDB.Items.Add("_Canbus_Queue");
            RCDB.Items.Add("------------------------");
            RCDB.Items.Add("_shot");
            RCDB.Items.Add("_rec_start");
            RCDB.Items.Add("_rec_stop");
            RCDB.Items.Add("_cmd");
            RCDB.Items.Add("_DOS");
            RCDB.Items.Add("------------------------");
            RCDB.Items.Add("_IO_Output");
            RCDB.Items.Add("_IO_Input");
            RCDB.Items.Add("_Pin");
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

        private DateTime startTime;
        long delayTime = 0;
        long repeatTime = 0;
        long timeCountUpdated = 0;

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

            delayTime = 0;
            repeatTime = 0;

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
            Thread LogAThread = new Thread(new ThreadStart(logA_analysis));
            Thread LogBThread = new Thread(new ThreadStart(logB_analysis));
            Thread LogCThread = new Thread(new ThreadStart(logC_analysis));
            Thread LogDThread = new Thread(new ThreadStart(logD_analysis));
            Thread LogEThread = new Thread(new ThreadStart(logE_analysis));

            startTime = DateTime.Now;

            if (AutoBox_Status)//如果電腦有接上Autokit//
            {
                button_Schedule1.PerformClick();
                
                if (StartButtonPressed == true)//按下STOP//
                {
                    Global.Break_Out_MyRunCamd = 1;//跳出倒數迴圈//
                    MainThread.Abort();//停止執行緒//
                    timer1.Stop();//停止倒數//
                    duringTimer.Enabled = false;
                    can_send = 0;
                    set_timer_rate = false;
                    can_rate.Clear();
                    can_data.Clear();
                    Serialportsave("Debug");

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
            else//如果沒接Autokit//
            {
                if (StartButtonPressed == true)//按下STOP//
                {
                    Global.Break_Out_MyRunCamd = 1;    //跳出倒數迴圈
                    MainThread.Abort(); //停止執行緒
                    timer1.Stop();  //停止倒數
                    duringTimer.Enabled = false;
                    can_send = 0;
                    set_timer_rate = false;
                    can_rate.Clear();
                    can_data.Clear();
                    Serialportsave("Debug");

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
                    label_Command.Text = "";
                }
            }
        }

        
        private void SettingBtn_Click(object sender, EventArgs e)
        {
            FormTabControl FormTabControl = new FormTabControl();

            //關閉SETTING以後會讀這段>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
            if (FormTabControl.ShowDialog() == DialogResult.OK)
            {
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
                    button_AcUsb.Enabled = false;
                    PowerState = false;
                    MyBlueRat.Disconnect(); //Prevent from System.ObjectDisposedException
                }

                if (ini12.INIRead(MainSettingPath, "Device", "CameraExist", "") == "1")
                {
                    try
                    {
                        pictureBox_Camera.Image = Properties.Resources.ON;
                        _captureInProgress = false;
                        OnOffCamera();
                        comboBox_CameraDevice.Enabled = false;
                        button_Camera.Enabled = true;
                        string[] cameraDevice = ini12.INIRead(MainSettingPath, "Camera", "CameraDevice", "").Split(',');
                        comboBox_CameraDevice.Items.Clear();
                        foreach (string cd in cameraDevice)
                        {
                            comboBox_CameraDevice.Items.Add(cd);
                        }
                        comboBox_CameraDevice.SelectedIndex = Int32.Parse(ini12.INIRead(MainSettingPath, "Camera", "VideoIndex", ""));
                    }
                    catch (ArgumentOutOfRangeException)
                    {

                    }
                }
                else
                {
                    pictureBox_Camera.Image = Properties.Resources.OFF;
                    button_Camera.Enabled = false;
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

            FormTabControl.Dispose();
            button_Schedule1.Enabled = true;
            button_Schedule1.PerformClick();

            setStyle();
        }

        //系統時間
        private void timer_Tick(object sender, EventArgs e)
        {
            DateTime dt = DateTime.Now;
            TimeLabel.Text = string.Format("{0:R}", dt);            //拍照打印時間
            TimeLabel2.Text = string.Format("{0:yyyy-MM-dd  HH:mm:ss}", dt);
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
                            string scheduleOutput = DataGridView_Schedule.Rows[j].Cells[k].Value + "";
                            if (scheduleOutput.Contains(","))
                            {
                                scheduleOutput = String.Format("\"{0}\"", scheduleOutput);
                            }
                            strRowValue += scheduleOutput + delimiter;
                        }
                        sw.WriteLine(strRowValue);
                    }
                    sw.Close();
                }

                if (sfd.FileName != ini12.INIRead(MainSettingPath, "Schedule" + Global.Schedule_Number, "Path", ""))
                {
                    ini12.INIWrite(MainSettingPath, "Schedule" + Global.Schedule_Number, "Path", sfd.FileName);
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
            catch (Exception)
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

            //string TextLine = "";
            //string[] SplitLine;
            int i = 0;
            if ((File.Exists(SchedulePath) == true) && ScheduleExist == "1" && IsFileLocked(SchedulePath) == false)
            {
                DataGridView_Schedule.Rows.Clear();
                /*StreamReader objReader = new StreamReader(SchedulePath);
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
                objReader.Close();*/

                TextFieldParser parser = new TextFieldParser(SchedulePath);
                parser.Delimiters = new string[] { "," };
                string[] parts = new string[11];
                while (!parser.EndOfData)
                {
                    try
                    {
                        parts = parser.ReadFields();
                        if (parts == null)
                        {
                            break;
                        }

                        if (i != 0)
                        {
                            DataGridView_Schedule.Rows.Add(parts);
                        }
                        i++;
                    }
                    catch (MalformedLineException)
                    {
                        MessageBox.Show("Schedule cannot contain double quote ( \" \" ).", "Schedule foramt error");
                    }
                }
                parser.Close();

                int j = parts.Length;
                if ((j == 11 || j == 10))
                {
                    long TotalDelay = 0;        //計算各個schedule測試時間
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

                            if (DataGridView_Schedule.Rows[z].Cells[8].Value.ToString().Contains('m') == true)
                                TotalDelay += (Convert.ToInt64(DataGridView_Schedule.Rows[z].Cells[8].Value.ToString().Replace('m', ' ').Trim()) * 60000 + RepeatTime);
                            else
                                TotalDelay += (long.Parse(DataGridView_Schedule.Rows[z].Cells[8].Value.ToString()) + RepeatTime);

                            RepeatTime = 0;
                        }
                    }

                    if (ini12.INIRead(MainSettingPath, "Record", "EachVideo", "") == "1")
                    {
                        ConvertToRealTime(((TotalDelay * Global.Schedule_Loop) + 63000));
                    }
                    else
                    {
                        ConvertToRealTime((TotalDelay * Global.Schedule_Loop));
                    }

                    switch (Global.Schedule_Number)
                    {
                        case 1:
                            Global.Schedule_1_TestTime = (TotalDelay * Global.Schedule_Loop);
                            timeCount = Global.Schedule_1_TestTime;
                            break;
                        case 2:
                            Global.Schedule_2_TestTime = (TotalDelay * Global.Schedule_Loop);
                            timeCount = Global.Schedule_2_TestTime;
                            break;
                        case 3:
                            Global.Schedule_3_TestTime = (TotalDelay * Global.Schedule_Loop);
                            timeCount = Global.Schedule_3_TestTime;
                            break;
                        case 4:
                            Global.Schedule_4_TestTime = (TotalDelay * Global.Schedule_Loop);
                            timeCount = Global.Schedule_4_TestTime;
                            break;
                        case 5:
                            Global.Schedule_5_TestTime = (TotalDelay * Global.Schedule_Loop);
                            timeCount = Global.Schedule_5_TestTime;
                            break;
                    }
                }
                else
                {
                    button_Start.Enabled = false;
                    MessageBox.Show("Please check your .csv file format.", "Schedule format error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else if (IsFileLocked(SchedulePath))
            {
                MessageBox.Show("Please check your .csv file is closed, then press Settings to reload the schedule.", "File lock error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                button_Start.Enabled = false;
                button_Schedule1.PerformClick();
            }
            else
            {
                button_Start.Enabled = false;
                button_Schedule1.PerformClick();
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
        private string ConvertToRealTime(long ms)
        {
            string sResult = "";
            try
            {
                TimeSpan finishTime = TimeSpan.FromMilliseconds(ms);
                label_ScheduleTime_Value.Invoke((MethodInvoker)(() => label_ScheduleTime_Value.Text = finishTime.Days.ToString("0") + "d " + finishTime.Hours.ToString("0") + "h " + finishTime.Minutes.ToString("0") + "m " + finishTime.Seconds.ToString("0") + "s " + finishTime.Milliseconds.ToString("0") + "ms"));
                ini12.INIWrite(MailPath, "Total Test Time", "value", finishTime.Days.ToString("0") + "d " + finishTime.Hours.ToString("0") + "h " + finishTime.Minutes.ToString("0") + "m " + finishTime.Seconds.ToString("0") + "s " + finishTime.Milliseconds.ToString("0") + "ms");

                // 寫入每個Schedule test time
                if (Global.Schedule_Number == 1)
                    ini12.INIWrite(MailPath, "Total Test Time", "value1", finishTime.Days.ToString("0") + "d " + finishTime.Hours.ToString("0") + "h " + finishTime.Minutes.ToString("0") + "m " + finishTime.Seconds.ToString("0") + "s " + finishTime.Milliseconds.ToString("0") + "ms");

                if (StartButtonPressed == true)
                {
                    switch (Global.Schedule_Number)
                    {
                        case 2:
                            ini12.INIWrite(MailPath, "Total Test Time", "value2", finishTime.Days.ToString("0") + "d " + finishTime.Hours.ToString("0") + "h " + finishTime.Minutes.ToString("0") + "m " + finishTime.Seconds.ToString("0") + "s " + finishTime.Milliseconds.ToString("0") + "ms");
                            break;
                        case 3:
                            ini12.INIWrite(MailPath, "Total Test Time", "value3", finishTime.Days.ToString("0") + "d " + finishTime.Hours.ToString("0") + "h " + finishTime.Minutes.ToString("0") + "m " + finishTime.Seconds.ToString("0") + "s " + finishTime.Milliseconds.ToString("0") + "ms");
                            break;
                        case 4:
                            ini12.INIWrite(MailPath, "Total Test Time", "value4", finishTime.Days.ToString("0") + "d " + finishTime.Hours.ToString("0") + "h " + finishTime.Minutes.ToString("0") + "m " + finishTime.Seconds.ToString("0") + "s " + finishTime.Milliseconds.ToString("0") + "ms");
                            break;
                        case 5:
                            ini12.INIWrite(MailPath, "Total Test Time", "value5", finishTime.Days.ToString("0") + "d " + finishTime.Hours.ToString("0") + "h " + finishTime.Minutes.ToString("0") + "m " + finishTime.Seconds.ToString("0") + "s " + finishTime.Milliseconds.ToString("0") + "ms");
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

                debug_process("Datagridview highlight.");
                GridUI(Global.Scheduler_Row.ToString(), DataGridView_Schedule);//控制Datagridview highlight//
                debug_process("Datagridview scollbar.");
                Gridscroll(Global.Scheduler_Row.ToString(), DataGridView_Schedule);//控制Datagridview scollbar//
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

        private void Schedule_Time()        //Estimated schedule time
        {
            if (timeCount > 0)
            {
                if (!String.IsNullOrEmpty(DataGridView_Schedule.Rows[Global.Scheduler_Row].Cells[8].Value.ToString()))
                {
                    if (DataGridView_Schedule.Rows[Global.Scheduler_Row].Cells[2].Value.ToString() != "")
                    {
                        repeatTime = (long.Parse(DataGridView_Schedule.Rows[Global.Scheduler_Row].Cells[1].Value.ToString())) * (long.Parse(DataGridView_Schedule.Rows[Global.Scheduler_Row].Cells[2].Value.ToString()));
                    }
                    if (DataGridView_Schedule.Rows[Global.Scheduler_Row].Cells[8].Value.ToString().Contains("m") == true)
                        delayTime = (long.Parse(DataGridView_Schedule.Rows[Global.Scheduler_Row].Cells[8].Value.ToString().Replace('m', ' ').Trim()) * 60000 + repeatTime);
                    else
                        delayTime = (long.Parse(DataGridView_Schedule.Rows[Global.Scheduler_Row].Cells[8].Value.ToString()) + repeatTime);

                    if (Global.Schedule_Step == 0 && Global.Loop_Number == 1)
                    {
                        timeCountUpdated = timeCount - delayTime;
                        ConvertToRealTime(timeCountUpdated);
                    }
                    else
                    {
                        timeCountUpdated = timeCountUpdated - delayTime;
                        ConvertToRealTime(timeCountUpdated);
                    }
                    repeatTime = 0;
                }
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            timer1.Interval = 500;
            TimeSpan timeElapsed = DateTime.Now - startTime;

            /*
            if (timeCount > 0)
            {
                if (Convert.ToInt64(timeElapsed.TotalMilliseconds) <= timeCount)
                {
                    ConvertToRealTime(timeCount - Convert.ToInt64(timeElapsed.TotalMilliseconds));
                }
                else
                {
                    ConvertToRealTime(0);
                }
            }*/

            label_TestTime_Value.Invoke((MethodInvoker)(() => label_TestTime_Value.Text = timeElapsed.Days.ToString("0") + "d " + timeElapsed.Hours.ToString("0") + "h " + timeElapsed.Minutes.ToString("0") + "m " + timeElapsed.Seconds.ToString("0") + "s " + timeElapsed.Milliseconds.ToString("0") + "ms"));
            ini12.INIWrite(MailPath, "Total Test Time", "How Long", timeElapsed.Days.ToString("0") + "d " + timeElapsed.Hours.ToString("0") + "h " + timeElapsed.Minutes.ToString("0") + "m " + timeElapsed.Seconds.ToString("0") + "s" + timeElapsed.Milliseconds.ToString("0") + "ms");
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
            Serialportsave("Debug");
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
            catch (Exception)
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
            catch (Exception)
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
                case "Port All":
                    Serialportsave("All");
                    MessageBox.Show("All Port is saved.", "Reminder");
                    break;
                default:
                    break;
            }
        }

        private void vectorcanloop()
        {
            UInt64 CAN_Count = 0;
            while (set_timer_rate)
            {
                Console.WriteLine("Vector_Can_Send (Repeat): " + CAN_Count + " times.");
                uint columns_times = can_id;
                byte[] columns_serial = can_data[columns_times];
                int columns_interval = (int)can_rate[columns_times];
                Can_1630A.LoopCANTransmit(columns_times, (uint)columns_interval, columns_serial);

                string Outputstring = "ID: 0x";
                //Outputstring += columns_times + " Data: " + columns_serial;
                DateTime dt = DateTime.Now;
                string canbus_log_text = "[Send_Canbus] [" + dt.ToString("yyyy/MM/dd HH:mm:ss.fff") + "]  " + Outputstring + "\r\n";
                log_process("Canbus", canbus_log_text);
                log_process("All", canbus_log_text);
                CAN_Count++;
            }

        }

        unsafe private void timer_canbus_Tick(object sender, EventArgs e)
        {
            UInt32 res = new UInt32();
            res = Can_Usb2C.ReceiveData();
            USB_CAN_Process usb_can_2c = new USB_CAN_Process();

            if (can_send == 1)
            {
                if (ini12.INIRead(MainSettingPath, "Device", "UsbCANExist", "") == "1" && ini12.INIRead(MainSettingPath, "Canbus", "Device", "") == "UsbCAN")
                {
                    foreach (var can in can_data_list)
                    {
                        usb_can_2c.CAN_Write_Queue_Add(can);
                    }
                    usb_can_2c.CAN_Write_Queue_SendData();
                    can_data_list.Clear();
                }
                else if (ini12.INIRead(MainSettingPath, "Device", "CAN1630AExist", "") == "1" && ini12.INIRead(MainSettingPath, "Canbus", "Device", "") == "Vector")
                {
                    foreach (var can in can_data_list)
                    {
                        Can_1630A.CAN_Write_Queue_Add(can);
                    }
                    Can_1630A.CAN_Write_Queue_SendData();
                    can_data_list.Clear();
                }
            }
            else
            {
                usb_can_2c.CAN_Write_Queue_Clear();
                Can_1630A.CAN_Write_Queue_Clear();
            }

            if (ini12.INIRead(MainSettingPath, "Device", "UsbCANExist", "") == "1" && ini12.INIRead(MainSettingPath, "Canbus", "Device", "") == "UsbCAN")
            {
                if (res == 0)
                {
                    if (res >= CAN_USB2C.MAX_CAN_OBJ_ARRAY_LEN)     // Must be something wrong
                    {
                        timer_canbus.Enabled = false;
                        Can_Usb2C.StopCAN();
                        Can_Usb2C.Disconnect();

                        pictureBox_canbus.Image = Properties.Resources.OFF;

                        ini12.INIWrite(MainSettingPath, "Device", "UsbCANExist", "0");

                        return;
                    }
                    return;
                }
                else
                {
                    uint ID = 0, DLC = 0;
                    const int DATA_LEN = 8;
                    byte[] DATA = new byte[DATA_LEN];

                    String str = "";
                    for (UInt32 i = 0; i < res; i++)
                    {
                        DateTime.Now.ToShortTimeString();
                        DateTime dt = DateTime.Now;
                        Can_Usb2C.GetOneCommand(i, out str, out ID, out DLC, out DATA);
                        string canbus_log_text = "[Receive_Canbus] [" + dt.ToString("yyyy/MM/dd HH:mm:ss.fff") + "]  " + str + "\r\n";
                        log_process("Canbus", canbus_log_text);
                        log_process("All", canbus_log_text);
                        if (Can_Usb2C.ReceiveData() >= CAN_USB2C.MAX_CAN_OBJ_ARRAY_LEN)
                        {
                            timer_canbus.Enabled = false;
                            Can_Usb2C.StopCAN();
                            Can_Usb2C.Disconnect();
                            pictureBox_canbus.Image = Properties.Resources.OFF;
                            ini12.INIWrite(MainSettingPath, "Device", "UsbCANExist", "0");
                            return;
                        }
                    }
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
        public static string StartupPath = Application.StartupPath;

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
        public static bool FormSetting = true;
        public static bool FormSchedule = true;
        public static bool FormMail = true;
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
        public static string Pass_Or_Fail = "";//測試結果//
        public static int Break_Out_Schedule = 0;//定時器中斷變數//
        public static int Break_Out_MyRunCamd;//是否跳出倒數迴圈，1為跳出//
        public static bool FormRC = false;
        public static string label_Command = "";
        public static string label_Remark = "";
        public static string label_LoopNumber = "";
        public static bool VideoRecording = false;
        public static string srtstring = "";
    }
    class Temperature_Data
    {
        public Temperature_Data(double list, bool shot, bool pause, string port, string log, string line)
        {
            temperatureList = list;
            temperatureShot = shot;
            temperaturePause = pause;
            temperaturePort = port;
            temperatureLog = log;
            temperatureNewline = line;
        }

        public static byte temperatureChannel
        {
            get; set;
        }

        public static float addTemperature
        {
            get; set;
        }

        public static float initialTemperature
        {
            get; set;
        }

        public static float finalTemperature
        {
            get; set;
        }

        public static int temperatureDuringtime
        {
            get; set;
        }

        public double temperatureList
        {
            get; set;
        }

        public bool temperatureShot
        {
            get; set;
        }

        public bool temperaturePause
        {
            get; set;
        }

        public string temperaturePort
        {
            get; set;
        }

        public string temperatureLog
        {
            get; set;
        }
        public string temperatureNewline
        {
            get; set;
        }
    }
}
