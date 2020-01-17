using System;
using System.Windows.Controls;
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
using System.IO;
using System.Diagnostics;

namespace Woodpecker
{
    class Autokit_Device
    {
        private IRedRat3 redRat3 = null;
        private RedRatDBParser RedRatData = new RedRatDBParser();
        public BlueRat MyBlueRat = new BlueRat();
        private bool BlueRat_UART_Exception_status = false;
        private Capture capture = null;

        //CanReader
        public CAN_Reader MYCanReader = new CAN_Reader();
        System.Windows.Forms.Timer timer_canbus = new System.Windows.Forms.Timer();

        //CA310
        private CA200SRVRLib.Ca200 objCa200;
        private CA200SRVRLib.Ca objCa;
        private CA200SRVRLib.Probe objProbe;
        private Boolean isMsr;
        System.Windows.Forms.Timer timer_ca310 = new System.Windows.Forms.Timer();

        #region -- AutoBox --
        public void ConnectAutoBox2()
        {
            uint temp_version;
            string curItem = Init_Parameter.config_parameter.Device_AutoboxPort;
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

        private void DisconnectAutoBox2()
        {
            if (Init_Parameter.config_parameter.Device_AutoboxExist == "1")
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
        
        //關閉AutoBox
        private void CloseAutobox()
        {
            if (Init_Parameter.config_parameter.Device_AutoboxVerson == "2")
            {
                DisconnectAutoBox2();
            }
            Environment.Exit(Environment.ExitCode);
        }
        #endregion

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
        public void Autocommand_RedRat(string Caller, string SigData)
        {
            string redcon = "";

            //讀取設備//
            if (Caller == "Form1")
            {
                RedRatData.RedRatLoadSignalDB(Init_Parameter.config_parameter.RedRat_DBFile);
                redcon = Init_Parameter.config_parameter.RedRat_Brands;
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
        public void Autocommand_BlueRat(string Caller, string SigData)
        {
            try
            {
                if (Caller == "Form1")
                {
                    RedRatData.RedRatLoadSignalDB(Init_Parameter.config_parameter.RedRat_DBFile);
                    RedRatData.RedRatSelectDevice(Init_Parameter.config_parameter.RedRat_Brands);
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
            System.Windows.Forms.Label TimeLabel2 = new System.Windows.Forms.Label();
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

        private void OnCaptureComplete(object sender, EventArgs e)
        {
            // Demonstrate the Capture.CaptureComplete event.
            Debug.WriteLine("Capture complete.");
        }

        //private void Jes() => Invoke(new EventHandler(delegate { Myshot(); }));

        public void Myshot()
        {
            capture.FrameEvent2 += new Capture.HeFrame(CaptureDone);
            capture.GrapImg();
        }

        // 複製原始圖片
        protected Bitmap CloneBitmap(Bitmap source)
        {
            return new Bitmap(source);
        }

        private System.Windows.Forms.PictureBox pictureBox_save;

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
            if (Autokit_Command.columns_command == "_shot")
            {
                bitMap_g.DrawString(Autokit_Command.columns_remark,
                                Font,
                                FontColor,
                                new PointF(5, YPoint - 120));
                bitMap_g.DrawString(Autokit_Command.columns_command + "  ( " + Global.label_Command + " )",
                                Font,
                                FontColor,
                                new PointF(5, YPoint - 80));
            }

            //照片印上現在時間//
            bitMap_g.DrawString(string.Format("{0:R}", DateTime.Now),
                                Font,
                                FontColor,
                                new PointF(5, YPoint - 40));

            Font.Dispose();
            FontColor.Dispose();
            bitMap_g.Dispose();

            string t = fName + "\\" + "pic-" + DateTime.Now.ToString("yyyyMMddHHmmss") + "(" + Global.label_LoopNumber + "-" + Global.caption_Num + ").png";
            pictureBox_save.Image.Save(t);
        }
        #endregion

        #region -- 錄影 --
        public void MySrtCamd()
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
                StreamWriter srtWriter = new StreamWriter(Global.srtstring, true);
                srtWriter.WriteLine(count);

                srtWriter.WriteLine(starttime + ",001" + " --> " + endtime + ",000");
                srtWriter.WriteLine(Global.label_Command + "     " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                srtWriter.WriteLine(Autokit_Command.columns_remark);
                srtWriter.WriteLine("");
                srtWriter.WriteLine("");
                srtWriter.Close();
                count++;
                starttime = endtime;
            }
        }

        //private void Mysvideo() => Invoke(new EventHandler(delegate { Savevideo(); }));//開始錄影//

        public void Mysstop()
        {
            capture.Stop();
            capture.Dispose();
            Camstart();
        }

        public void Savevideo()//儲存影片//
        {
            string fName = Init_Parameter.config_parameter.Record_VideoPath;

            string t = fName + "\\" + "_rec" + DateTime.Now.ToString("yyyyMMddHHmmss") + "__" + Global.label_LoopNumber + ".avi";
            Global.srtstring = fName + "\\" + "_rec" + DateTime.Now.ToString("yyyyMMddHHmmss") + "__" + Global.label_LoopNumber + ".srt";

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
        #endregion

        #region -- Canbus --
        public void ConnectCanBus()
        {
            uint status;
            timer_canbus.Interval = 250;
            timer_canbus.Tick += new System.EventHandler(this.timer_canbus_Tick);

            status = MYCanReader.Connect();
            if (status == 1)
            {
                status = MYCanReader.StartCAN();
                if (status == 1)
                {
                    timer_canbus.Enabled = true;
                    //pictureBox_canbus.Image = Properties.Resources.ON;
                }
                else
                {
                    //pictureBox_canbus.Image = Properties.Resources.OFF;
                }
            }
            else
            {
                //pictureBox_canbus.Image = Properties.Resources.OFF;
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

                    //pictureBox_canbus.Image = Properties.Resources.OFF;

                    Init_Parameter.config_parameter.Canbus_Exist = "0";

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
                    MYCanReader.GetOneCommand(i, out str, out ID, out DLC, out DATA);
                    string canbus_log_text = "[Receive_Canbus] [" + dt.ToString("yyyy/MM/dd HH:mm:ss.fff") + "]  " + str + "\r\n";
                    Autokit_Command.canbus_text = string.Concat(Autokit_Command.canbus_text, canbus_log_text);
                    Autokit_Command.schedule_text = string.Concat(Autokit_Command.schedule_text, canbus_log_text);
                    if (MYCanReader.ReceiveData() >= CAN_Reader.MAX_CAN_OBJ_ARRAY_LEN)
                    {
                        timer_canbus.Enabled = false;
                        MYCanReader.StopCAN();
                        MYCanReader.Disconnect();
                        //pictureBox_canbus.Image = Properties.Resources.OFF;
                        Init_Parameter.config_parameter.Canbus_Exist = "0";
                        return;
                    }
                }
            }
        }

        #endregion

        #region -- CA310 --
        public void ConnectCA310()
        {
            uint status;

            status = ExeConnectCA310();
            if (status == 1)
            {
                status = ExeCalZero();
                if (status == 1)
                {
                    isMsr = true;
                    timer_ca310.Enabled = true;
                    //pictureBox_ca310.Image = Properties.Resources.ON;
                }
                else
                {
                    //pictureBox_ca310.Image = Properties.Resources.OFF;
                }
            }
            else
            {
                //pictureBox_ca310.Image = Properties.Resources.OFF;
            }
        }

        private uint ExeConnectCA310()
        {
            try
            {
                objCa200 = new CA200SRVRLib.Ca200();
                objCa200.AutoConnect();
                objCa = objCa200.SingleCa;
                objProbe = objCa.SingleProbe;
                return 1;
            }
            catch (Exception)
            {
                isMsr = false;
                return 0;
            }
        }

        private uint ExeCalZero()
        {
            try
            {
                objCa.CalZero();
                return 1;
            }
            catch (Exception)
            {
                isMsr = false;
                return 0;
            }
        }

        private void timer_ca310_Tick(object sender, EventArgs e)
        {
            if (Init_Parameter.config_parameter.Device_CA310Exist == "1")
            {
                try
                {
                    objCa.Measure();
                    string str = "Lv:" + objProbe.Lv.ToString("##0.00") + " Sx:" + objProbe.sx.ToString("0.0000") + " Sy:" + objProbe.sy.ToString("0.0000");
                    DateTime.Now.ToShortTimeString();
                    DateTime dt = DateTime.Now;
                    string ca310_log_text = "[Receive_CA310] [" + dt.ToString("yyyy/MM/dd HH:mm:ss.fff") + "]  " + str + "\r\n";
                    Autokit_Command.ca310_text = string.Concat(Autokit_Command.ca310_text, ca310_log_text);
                    Autokit_Command.schedule_text = string.Concat(Autokit_Command.schedule_text, ca310_log_text);
                }
                catch (Exception)
                {
                    isMsr = false;
                    timer_ca310.Enabled = false;
                    //pictureBox_ca310.Image = Properties.Resources.OFF;
                    //MessageBox.Show("CA310 already disconnected, please restart the Woodpecker.", "CA310 Open Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void CalZero()
        {
            bool calzero_success = false;

            while (calzero_success == false)
            {
                try
                {
                    objCa.CalZero();
                    calzero_success = true;
                }
                catch (Exception)
                {
                    objCa.RemoteMode = 0;
                }
            }
        }
        #endregion
    }
}
