using System;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DirectX.Capture;
using BlueRatLibrary;
using Microsoft.Win32.SafeHandles;
using System.Runtime.InteropServices;
using RedRat.IR;
using RedRat.RedRat3;
using System.Timers;

namespace Woodpecker
{
    class Autokit_Device
    {
        private IRedRat3 redRat3 = null;
        private Add_ons Add_ons = new Add_ons();
        private RedRatDBParser RedRatData = new RedRatDBParser();
        public BlueRat MyBlueRat = new BlueRat();
        private bool BlueRat_UART_Exception_status = false;

        public void ConnectAutoBox2(string curItem)
        {
            uint temp_version;

            if (MyBlueRat.Connect(curItem) == true)
            {
                temp_version = MyBlueRat.FW_VER;
                float v = temp_version;

                // 在第一次/或長時間未使用之後,要開始使用BlueRat跑Schedule之前,建議執行這一行,確保BlueRat的起始狀態一致 -- 正常情況下不執行並不影響BlueRat運行,但為了找問題方便,還是請務必執行
                MyBlueRat.Force_Init_BlueRat();
                MyBlueRat.Reset_SX1509();

                byte SX1509_detect_status;
                SX1509_detect_status = MyBlueRat.TEST_Detect_SX1509();

                hCOM = MyBlueRat.ReturnSafeFileHandle();
                BlueRat_UART_Exception_status = false;
                UpdateRCFunctionButtonAfterConnection();
            }
            else
            {
                Console.WriteLine(DateTime.Now.ToString("h:mm:ss tt") + " - Cannot connect to BlueRat.\n");
            }
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

        public void GP0_GP1_AC_ON()
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
                Console.WriteLine("Woodpecker is already running.", "GP0_GP1_AC_ON Error");
            }
            PowerState = true;
        }

        public void GP0_GP1_AC_OFF_ON()
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
                    }
                }
            }
        }

        public void GP2_GP3_USB_PC()
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
                Console.WriteLine("Woodpecker is already running.", "GP2_GP3_USB_PC Error");
            }
            USBState = true;
        }

        public string IO_INPUT()
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

                return Global.IO_INPUT = modified5;
            }
            while ((bRet == false) && (--retry_cnt > 0));
        }
        #endregion

        #region -- Redrat --
        public void Autocommand_RedRat(string Caller, string SigData, string RedRat_DBFile, string RedRat_Brands)
        {
            string redcon = "";

            //讀取設備//
            if (Caller == "Form1")
            {
                RedRatData.RedRatLoadSignalDB(RedRat_DBFile);
                redcon = RedRat_Brands;
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
            }
        }

        private Boolean D = false;
        public void Autocommand_BlueRat(string Caller, string SigData, string RedRat_DBFile, string RedRat_Brands)
        {
            try
            {
                if (Caller == "Form1")
                {
                    RedRatData.RedRatLoadSignalDB(RedRat_DBFile);
                    RedRatData.RedRatSelectDevice(RedRat_Brands);
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
            }
        }

        private void UpdateRCFunctionButtonAfterConnection()
        {
            if ((MyBlueRat.CheckConnection() == true))
            {
                if ((RedRatData != null) && (RedRatData.SignalDB != null) && (RedRatData.SelectedDevice != null) && (RedRatData.SelectedSignal != null))
                {

                }
            }
        }

        // 這個主程式專用的delay的內部資料與function
        static bool RedRatDBViewer_Delay_TimeOutIndicator = false;
        private void RedRatDBViewer_Delay_OnTimedEvent(object source, ElapsedEventArgs e)
        {
            //Console.WriteLine("RedRatDBViewer_Delay_TimeOutIndicator: True.");
            RedRatDBViewer_Delay_TimeOutIndicator = true;
        }

        public void RedRatDBViewer_Delay(int delay_ms)
        {
            Label TimeLabel2 = new Label();
            //Console.WriteLine("RedRatDBViewer_Delay: Start.");
            if (delay_ms <= 0) return;
            System.Timers.Timer aTimer = new System.Timers.Timer(delay_ms);
            //aTimer.Interval = delay_ms;
            aTimer.Elapsed += new ElapsedEventHandler(RedRatDBViewer_Delay_OnTimedEvent);
            aTimer.SynchronizingObject = TimeLabel2;
            RedRatDBViewer_Delay_TimeOutIndicator = false;
            aTimer.Enabled = true;
            aTimer.Start();
            aTimer.Stop();
            aTimer.Dispose();
            //Console.WriteLine("RedRatDBViewer_Delay: End.");
        }
        #endregion

        #region -- 拍照 --
        public void Camstart()
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
                if (Init_Parameter.config_parameter.Camera_VideoIndex == "")
                    scam = 0;
                else
                    scam = int.Parse(Init_Parameter.config_parameter.Camera_VideoIndex);

                if (Init_Parameter.config_parameter.Camera_AudioIndex == "")
                    saud = 0;
                else
                    saud = int.Parse(Init_Parameter.config_parameter.Camera_AudioIndex);

                if (Init_Parameter.config_parameter.Camera_VideoNumber == "")
                    VideoNum = 0;
                else
                    VideoNum = int.Parse(Init_Parameter.config_parameter.Camera_VideoNumber);

                if (Init_Parameter.config_parameter.Camera_AudioNumber == "")
                    AudioNum = 0;
                else
                    AudioNum = int.Parse(Init_Parameter.config_parameter.Camera_AudioNumber);

                if (filters.VideoInputDevices.Count < VideoNum ||
                    filters.AudioInputDevices.Count < AudioNum)
                {

                }
                else
                {
                    capture = new Capture(filters.VideoInputDevices[scam], filters.AudioInputDevices[saud]);
                    try
                    {
                        capture.FrameSize = new Size(2304, 1296);
                        Init_Parameter.config_parameter.Camera_Resolution = "2304*1296";
                    }
                    catch (Exception ex)
                    {
                        Console.Write(ex.Message.ToString(), "Webcam does not support 2304*1296!\n\r");
                        try
                        {
                            capture.FrameSize = new Size(1920, 1080);
                            Init_Parameter.config_parameter.Camera_Resolution = "1920*1080";
                        }
                        catch (Exception ex1)
                        {
                            Console.Write(ex1.Message.ToString(), "Webcam does not support 1920*1080!\n\r");
                            try
                            {
                                capture.FrameSize = new Size(1280, 720);
                                Init_Parameter.config_parameter.Camera_Resolution = "1280*720";
                            }
                            catch (Exception ex2)
                            {
                                Console.Write(ex2.Message.ToString(), "Webcam does not support 1280*720!\n\r");
                                try
                                {
                                    capture.FrameSize = new Size(640, 480);
                                    Init_Parameter.config_parameter.Camera_Resolution = "640*480";
                                }
                                catch (Exception ex3)
                                {
                                    Console.Write(ex3.Message.ToString(), "Webcam does not support 640*480!\n\r");
                                    try
                                    {
                                        capture.FrameSize = new Size(320, 240);
                                        Init_Parameter.config_parameter.Camera_Resolution = "320*240";
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
            }
            catch (NotSupportedException)
            {
                Console.Write("Camera is disconnected unexpectedly!\r\nPlease go to Settings to reload the device list.", "Connection Error");
            };
        }

        private void Jes() => Invoke(new EventHandler(delegate { Myshot(); }));

        private void Myshot()
        {
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
            string fName = Init_Parameter.config_parameter.Record_VideoPath;
            //string ngFolder = "Schedule" + Global.Schedule_Num + "_NG";

            //圖片印字
            Bitmap newBitmap = CloneBitmap(e);
            newBitmap = CloneBitmap(e);
            pictureBox_save.Image = newBitmap;

            Graphics bitMap_g = Graphics.FromImage(pictureBox_save.Image);//底圖
            Font Font = new Font("Microsoft JhengHei Light", 16, FontStyle.Bold);
            Brush FontColor = new SolidBrush(Color.Red);
            string[] Resolution = Init_Parameter.config_parameter.Camera_Resolution.Split('*');
            int YPoint = int.Parse(Resolution[1]);

            //照片印上現在步驟//
            if (columns_command == "_shot")
            {
                bitMap_g.DrawString(columns_remark,
                                Font,
                                FontColor,
                                new PointF(5, YPoint - 120));
                bitMap_g.DrawString(columns_command + "  ( " + label_Command.Text + " )",
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
            pictureBox_save.Image.Save(t);
        }
        #endregion

        #region -- IO CMD 指令集 --
        private void IO_CMD()
        {
            if (columns_serial == "_pause")
            {
                label_Command.Text = "IO CMD_PAUSE";
            }
            else if (columns_serial == "_stop")
            {
                label_Command.Text = "IO CMD_STOP";
            }
            else if (columns_serial == "_ac_restart")
            {
                Autokit_Device_1.GP0_GP1_AC_OFF_ON();
                Thread.Sleep(10);
                Autokit_Device_1.GP0_GP1_AC_OFF_ON();
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
        
        #region -- KEYWORD 指令集 --
        private void KeywordCommand()
        {
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
            else if (columns_serial.Substring(0, 7) == "_savelog")
            {
                string fName = "";
                fName = ini12.INIRead(MainSettingPath, "Record", "LogPath", "");
                String savelog_serialport = columns_serial.Substring(9, 1);
                if (savelog_serialport == "A")
                {
                    string t = fName + "\\_SaveLogA_" + DateTime.Now.ToString("yyyyMMddHHmmss") + "_" + label_LoopNumber_Value.Text + ".txt";
                    StreamWriter MYFILE = new StreamWriter(t, false, Encoding.ASCII);
                    MYFILE.Write(log1_text);
                    MYFILE.Close();
                }
                else if (savelog_serialport == "B")
                {
                    string t = fName + "\\_SaveLogB_" + DateTime.Now.ToString("yyyyMMddHHmmss") + "_" + label_LoopNumber_Value.Text + ".txt";
                    StreamWriter MYFILE = new StreamWriter(t, false, Encoding.ASCII);
                    MYFILE.Write(log2_text);
                    MYFILE.Close();
                }
                else if (savelog_serialport == "C")
                {
                    string t = fName + "\\_SaveLogC_" + DateTime.Now.ToString("yyyyMMddHHmmss") + "_" + label_LoopNumber_Value.Text + ".txt";
                    StreamWriter MYFILE = new StreamWriter(t, false, Encoding.ASCII);
                    MYFILE.Write(log3_text);
                    MYFILE.Close();
                }
                else if (savelog_serialport == "D")
                {
                    string t = fName + "\\_SaveLogD_" + DateTime.Now.ToString("yyyyMMddHHmmss") + "_" + label_LoopNumber_Value.Text + ".txt";
                    StreamWriter MYFILE = new StreamWriter(t, false, Encoding.ASCII);
                    MYFILE.Write(log4_text);
                    MYFILE.Close();
                }
                else if (savelog_serialport == "E")
                {
                    string t = fName + "\\_SaveLogE_" + DateTime.Now.ToString("yyyyMMddHHmmss") + "_" + label_LoopNumber_Value.Text + ".txt";
                    StreamWriter MYFILE = new StreamWriter(t, false, Encoding.ASCII);
                    MYFILE.Write(log5_text);
                    MYFILE.Close();
                }
                else if (savelog_serialport == "O")
                {
                    string t = fName + "\\_SaveLogAll_" + DateTime.Now.ToString("yyyyMMddHHmmss") + "_" + label_LoopNumber_Value.Text + ".txt";
                    StreamWriter MYFILE = new StreamWriter(t, false, Encoding.ASCII);
                    MYFILE.Write(logAll_text);
                    MYFILE.Close();
                }
                label_Command.Text = "KEYWORD_SAVELOG";
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
                label_Command.Text = "KEYWORD_LOGCMD";
            }
        }
        #endregion
    }
}
