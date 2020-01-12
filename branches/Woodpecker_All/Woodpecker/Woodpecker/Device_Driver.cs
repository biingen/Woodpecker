using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DirectX.Capture;
using jini;
using BlueRatLibrary;
using Microsoft.Win32.SafeHandles;
using System.Runtime.InteropServices;
using RedRat.IR;
using RedRat.RedRat3;
using System.Timers;
using System.IO.Ports;

namespace Woodpecker
{
    class Device_Driver
    {
        private IRedRat3 redRat3 = null;
        private Add_ons Add_ons = new Add_ons();
        private RedRatDBParser RedRatData = new RedRatDBParser();
        private BlueRat MyBlueRat = new BlueRat();
        private static bool BlueRat_UART_Exception_status = false;

        private void ConnectAutoBox2(string curItem)
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
                Console.WriteLine("Woodpecker is already running.", "GP0_GP1_AC_ON Error");
            }
            PowerState = true;
        }

        private void GP0_GP1_AC_OFF_ON()
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
                Console.WriteLine("Woodpecker is already running.", "GP2_GP3_USB_PC Error");
            }
            USBState = true;
        }

        public void IO_INPUT()
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
            aTimer.Stop();
            aTimer.Dispose();
            //Console.WriteLine("RedRatDBViewer_Delay: End.");
        }

        #endregion

        #region -- Serial Port --
        protected SerialPortDataContainer OpenSerialPort1(SerialPort sp)
        {
            SerialPortDataContainer sp_data = new SerialPortDataContainer();
            sp_data.serial_port = sp;
            SerialPortDataContainer.SerialPortDictionary.Add(sp.PortName, sp_data);
            try
            {
                if (sp.IsOpen == false)
                {
                    string stopbit = ini12.INIRead(MainSettingPath, "Comport", "StopBits", "");
                    switch (stopbit)
                    {
                        case "One":
                            sp.StopBits = StopBits.One;
                            break;
                        case "Two":
                            sp.StopBits = StopBits.Two;
                            break;
                    }
                    sp.PortName = ini12.INIRead(MainSettingPath, "Comport", "PortName", "");
                    sp.BaudRate = int.Parse(ini12.INIRead(MainSettingPath, "Comport", "BaudRate", ""));
                    sp.DataBits = 8;
                    sp.Parity = (Parity)0;
                    sp.ReceivedBytesThreshold = 1;
                    // serialPort1.Encoding = System.Text.Encoding.GetEncoding(1252);

                    sp.DataReceived += new SerialDataReceivedEventHandler(SerialPort1_DataReceived);       // DataReceived呼叫函式
                    sp.Open();
                    Thread DataThread = new Thread(new ThreadStart(test));
                    DataThread.Start();
                }
            }
            catch (Exception Ex)
            {
                Console.WriteLine(Ex.Message.ToString(), "SerialPort Error");
            }

            return sp_data;
        }

        protected void CloseSerialPort1()
        {
            serialPort1.Dispose();
            serialPort1.Close();
        }

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

        public SerialPortDataContainer SerialPortA = new SerialPortDataContainer();
        private void SerialPort1_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            Object serial_data_obj;
            SerialPort sp = (SerialPort)sender;
            SerialPortDataContainer.SerialPortDictionary.TryGetValue(sp.PortName, out serial_data_obj);
            SerialPortDataContainer serial_port_data = (SerialPortDataContainer)serial_data_obj;

            if (serial_port_data == null)
                return;

            int data_to_read = sp.BytesToRead;

            if (data_to_read > 0)
            {
                Byte[] dataset = new Byte[data_to_read];
                sp.Read(dataset, 0, data_to_read);
                List<Byte> data_list = dataset.ToList();

                SerialReceivedData enqueue_data = new SerialReceivedData();
                enqueue_data.SetData(data_list);
                enqueue_data.SetTimeStamp(DateTime.Now);
                serial_port_data.data_queue.Enqueue(enqueue_data);
                SerialPortDataContainer.data_available = true;
                Thread DataThread = new Thread(new ThreadStart(test));
                DataThread.Start();
            }

            //if (SerialPortDataContainer.data_available == true)
            //{
            //Thread DataThread = new Thread(new ThreadStart(test));
            //    DataThread.Start();
            //}
            //else
            //{
            //    DataThread.Abort();
            //}

        } //

        //
        // Form UI portion of RS232 data are relocated here.
        //
        //private void Timer_rs232_data_recevied_Tick(object sender, EventArgs e)
        //{

        //}

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
                                string text = Encoding.ASCII.GetString(dataset);
                                dt = DateTime.Now;
                                text = text.Replace(Environment.NewLine, "\r\n" + "[" + dt.ToString("yyyy/MM/dd HH:mm:ss") + "]  "); //OK
                                textBox1.AppendText(text);
                            }
                        }
                    }
                }
            }
            test_is_running = false;
        }
        #endregion
    }
}
