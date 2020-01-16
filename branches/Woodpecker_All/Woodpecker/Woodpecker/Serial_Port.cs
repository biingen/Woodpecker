using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.IO.Ports;
using System.Reflection;
using System.Windows.Forms;
using System.Threading;
using BlockMessageLibrary;
using DTC_ABS;
using DTC_OBD;
using MySerialLibrary;
using KWP_2000;

namespace Woodpecker
{
    enum SerialErrorStatus
    {
        OK = 0,
        PortA_error,
        PortB_error,
        PortC_error,
        PortD_error,
        PortE_error,
        Kline_error,
        status_needed,
        undefined_error = int.MinValue,
    }

    class SerialErrorCode
    {
        public SerialErrorStatus status = SerialErrorStatus.status_needed;
        public Exception ex;
    }

    class SerialPortParemeter
    {
        public StopBits StopBits = StopBits.One;
        public string PortName;
        public int BaudRate = 115200;
        public int ReadTimeout = 2000;
        // public Encoding encoding = System.Text.Encoding.GetEncoding(1252);
    }

    #region -- Serial Port --
    class Serial_Port
    {
        public SerialPort PortA;
        public SerialPort PortB;
        public SerialPort PortC;
        public SerialPort PortD;
        public SerialPort PortE;

        private Queue<byte> SearchLogQueue_A = new Queue<byte>();
        private Queue<byte> SearchLogQueue_B = new Queue<byte>();
        private Queue<byte> SearchLogQueue_C = new Queue<byte>();
        private Queue<byte> SearchLogQueue_D = new Queue<byte>();
        private Queue<byte> SearchLogQueue_E = new Queue<byte>();
        private char Keyword_SerialPort_A_temp_char;
        private byte Keyword_SerialPort_A_temp_byte;
        private char Keyword_SerialPort_B_temp_char;
        private byte Keyword_SerialPort_B_temp_byte;
        private char Keyword_SerialPort_C_temp_char;
        private byte Keyword_SerialPort_C_temp_byte;
        private char Keyword_SerialPort_D_temp_char;
        private byte Keyword_SerialPort_D_temp_byte;
        private char Keyword_SerialPort_E_temp_char;
        private byte Keyword_SerialPort_E_temp_byte;

        private bool DisplayHexOn_A = false;
        private bool DisplayHexOn_B = false;
        private bool DisplayHexOn_C = false;
        private bool DisplayHexOn_D = false;
        private bool DisplayHexOn_E = false;

        //Klite error code
        private MySerial MySerialPort = new MySerial();
        public int kline_send = 0;
        public List<DTC_Data> ABS_error_list = new List<DTC_Data>();
        public List<DTC_Data> OBD_error_list = new List<DTC_Data>();
        private TextBox textBox_serial;

        //Canbus Reader
        public CAN_Reader MYCanReader = new CAN_Reader();

        private Autokit_Device Autokit_Device_1 = new Autokit_Device();
        private Autokit_Function Autokit_Function_1 = new Autokit_Function();

        public void Serial_Port_Init()
        {
            if (Init_Parameter.config_parameter.Port_Displayhex == "1")
            {
                DisplayHexOn_A = true;
                DisplayHexOn_B = true;
                DisplayHexOn_C = true;
                DisplayHexOn_D = true;
                DisplayHexOn_E = true;
            }
            else
            {
                DisplayHexOn_A = false;
                DisplayHexOn_B = false;
                DisplayHexOn_C = false;
                DisplayHexOn_D = false;
                DisplayHexOn_E = false;
            }

        }

        #region -- SerialPort Setup --
        public SerialErrorCode OpenSerialPort(string Port, SerialPortParemeter param)
        {
            SerialErrorCode return_code = new SerialErrorCode();

            switch (Port)
            {
                case "A":
                    try
                    {
                        if (PortA.IsOpen == false)
                        {
                            PortA.StopBits = param.StopBits;
                            PortA.PortName = param.PortName;
                            PortA.BaudRate = param.BaudRate;
                            PortA.ReadTimeout = param.ReadTimeout;
                            // serialPort1.Encoding = System.Text.Encoding.GetEncoding(1252);

                            PortA.DataReceived += new SerialDataReceivedEventHandler(SerialPort1_DataReceived);       // DataReceived呼叫函式
                            PortA.Open();
                            object stream = typeof(SerialPort).GetField("internalSerialStream", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(PortA);

                            return_code.status = SerialErrorStatus.OK;
                        }
                    }
                    catch (Exception Ex)
                    {
                        return_code.status = SerialErrorStatus.PortA_error;
                        return_code.ex = Ex;
                        // MessageBox.Show(Ex.Message.ToString(), "PortA Error");
                    }
                    break;
                case "B":
                    try
                    {
                        if (PortB.IsOpen == false)
                        {
                            PortB.StopBits = param.StopBits;
                            PortB.PortName = param.PortName;
                            PortB.BaudRate = param.BaudRate;
                            PortB.ReadTimeout = param.ReadTimeout;
                            // serialPort2.Encoding = System.Text.Encoding.GetEncoding(1252);

                            PortB.DataReceived += new SerialDataReceivedEventHandler(SerialPort2_DataReceived);       // DataReceived呼叫函式
                            PortB.Open();
                            object stream = typeof(SerialPort).GetField("internalSerialStream", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(PortB);
                        }
                    }
                    catch (Exception Ex)
                    {
                        return_code.status = SerialErrorStatus.PortB_error;
                        return_code.ex = Ex;
                        //MessageBox.Show(Ex.Message.ToString(), "PortB Error");
                    }
                    break;
                case "C":
                    try
                    {
                        if (PortC.IsOpen == false)
                        {
                            PortC.StopBits = param.StopBits;
                            PortC.PortName = param.PortName;
                            PortC.BaudRate = param.BaudRate;
                            PortC.ReadTimeout = param.ReadTimeout;
                            // serialPort3.Encoding = System.Text.Encoding.GetEncoding(1252);

                            PortC.DataReceived += new SerialDataReceivedEventHandler(SerialPort3_DataReceived);       // DataReceived呼叫函式
                            PortC.Open();
                            object stream = typeof(SerialPort).GetField("internalSerialStream", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(PortC);
                        }
                    }
                    catch (Exception Ex)
                    {
                        return_code.status = SerialErrorStatus.PortC_error;
                        return_code.ex = Ex;
                        //MessageBox.Show(Ex.Message.ToString(), "PortC Error");
                    }
                    break;
                case "D":
                    try
                    {
                        if (PortD.IsOpen == false)
                        {
                            PortD.StopBits = param.StopBits;
                            PortD.PortName = param.PortName;
                            PortD.BaudRate = param.BaudRate;
                            PortD.ReadTimeout = param.ReadTimeout;
                            // serialPort4.Encoding = System.Text.Encoding.GetEncoding(1252);

                            PortD.DataReceived += new SerialDataReceivedEventHandler(SerialPort4_DataReceived);       // DataReceived呼叫函式
                            PortD.Open();
                            object stream = typeof(SerialPort).GetField("internalSerialStream", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(PortC);
                        }
                    }
                    catch (Exception Ex)
                    {
                        return_code.status = SerialErrorStatus.PortD_error;
                        return_code.ex = Ex;
                        //MessageBox.Show(Ex.Message.ToString(), "PortD Error");
                    }
                    break;
                case "E":
                    try
                    {
                        if (PortE.IsOpen == false)
                        {
                            PortE.StopBits = param.StopBits;
                            PortE.PortName = param.PortName;
                            PortE.BaudRate = param.BaudRate;
                            PortE.ReadTimeout = param.ReadTimeout;
                            // serialPort5.Encoding = System.Text.Encoding.GetEncoding(1252);

                            PortE.DataReceived += new SerialDataReceivedEventHandler(SerialPort4_DataReceived);       // DataReceived呼叫函式
                            PortE.Open();
                            object stream = typeof(SerialPort).GetField("internalSerialStream", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(PortC);
                        }
                    }
                    catch (Exception Ex)
                    {
                        return_code.status = SerialErrorStatus.PortE_error;
                        return_code.ex = Ex;
                        //MessageBox.Show(Ex.Message.ToString(), "PortE Error");
                    }
                    break;
                default:
                    return_code.status = SerialErrorStatus.undefined_error;
                    break;
            }
            return return_code;
        }

        public void OpenKlinePort()
        {
            SerialErrorCode return_code = new SerialErrorCode();
            System.Windows.Forms.Timer timer_kline = new System.Windows.Forms.Timer();
            timer_kline.Interval = 250;
            timer_kline.Tick += new System.EventHandler(this.Timer_kline_Tick);

            try
            {
                if (MySerialPort.IsPortOpened() == false)
                {
                    string curItem = MySerialPort.GetPortName();
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
                return_code.status = SerialErrorStatus.Kline_error;
                return_code.ex = Ex;
                //MessageBox.Show(Ex.Message.ToString(), "KlinePort Error");
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

        public void Serialportsave(string Port)
        {
            string fName = "";

            // 讀取ini中的路徑
            fName = Init_Parameter.config_parameter.Record_LogPath;
            switch (Port)
            {
                case "A":
                    string t = fName + "\\_PortA_" + DateTime.Now.ToString("yyyyMMddHHmmss") + "_" + Global.label_LoopNumber + ".txt";
                    StreamWriter MYFILE = new StreamWriter(t, false, Encoding.ASCII);
                    MYFILE.Write(Extra_Commander.logA_text);
                    MYFILE.Close();
                    Extra_Commander.logA_text = String.Empty;
                    break;
                case "B":
                    t = fName + "\\_PortB_" + DateTime.Now.ToString("yyyyMMddHHmmss") + "_" + Global.label_LoopNumber + ".txt";
                    MYFILE = new StreamWriter(t, false, Encoding.ASCII);
                    MYFILE.Write(Extra_Commander.logB_text);
                    MYFILE.Close();
                    Extra_Commander.logB_text = String.Empty;
                    break;
                case "C":
                    t = fName + "\\_PortC_" + DateTime.Now.ToString("yyyyMMddHHmmss") + "_" + Global.label_LoopNumber + ".txt";
                    MYFILE = new StreamWriter(t, false, Encoding.ASCII);
                    MYFILE.Write(Extra_Commander.logC_text);
                    MYFILE.Close();
                    Extra_Commander.logC_text = String.Empty;
                    break;
                case "D":
                    t = fName + "\\_PortD_" + DateTime.Now.ToString("yyyyMMddHHmmss") + "_" + Global.label_LoopNumber + ".txt";
                    MYFILE = new StreamWriter(t, false, Encoding.ASCII);
                    MYFILE.Write(Extra_Commander.logD_text);
                    MYFILE.Close();
                    Extra_Commander.logD_text = String.Empty;
                    break;
                case "E":
                    t = fName + "\\_PortE_" + DateTime.Now.ToString("yyyyMMddHHmmss") + "_" + Global.label_LoopNumber + ".txt";
                    MYFILE = new StreamWriter(t, false, Encoding.ASCII);
                    MYFILE.Write(Extra_Commander.logE_text);
                    MYFILE.Close();
                    Extra_Commander.logE_text = String.Empty;
                    break;
                case "CA310":
                    t = fName + "\\_CA310_" + DateTime.Now.ToString("yyyyMMddHHmmss") + "_" + Global.label_LoopNumber + ".txt";
                    MYFILE = new StreamWriter(t, false, Encoding.ASCII);
                    MYFILE.Write(Extra_Commander.ca310_text);
                    MYFILE.Close();
                    Extra_Commander.ca310_text = String.Empty;
                    break;
                case "Canbus":
                    t = fName + "\\_Canbus_" + DateTime.Now.ToString("yyyyMMddHHmmss") + "_" + Global.label_LoopNumber + ".txt";
                    MYFILE = new StreamWriter(t, false, Encoding.ASCII);
                    MYFILE.Write(Extra_Commander.canbus_text);
                    MYFILE.Close();
                    Extra_Commander.canbus_text = String.Empty;
                    break;
                case "KlinePort":
                    t = fName + "\\_Kline_" + DateTime.Now.ToString("yyyyMMddHHmmss") + "_" + Global.label_LoopNumber + ".txt";
                    MYFILE = new StreamWriter(t, false, Encoding.ASCII);
                    MYFILE.Write(Extra_Commander.kline_text);
                    MYFILE.Close();
                    Extra_Commander.kline_text = String.Empty;
                    break;
                case "All":
                    t = fName + "\\_AllPort_" + DateTime.Now.ToString("yyyyMMddHHmmss") + "_" + Global.label_LoopNumber + ".txt";
                    MYFILE = new StreamWriter(t, false, Encoding.ASCII);
                    MYFILE.Write(Extra_Commander.logAll_text);
                    MYFILE.Close();
                    Extra_Commander.logAll_text = String.Empty;
                    break;
            }
        }

        public void ReplaceNewLine(SerialPort port, string columns_serial, string columns_switch)
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
                    int index = 0;
                    while (data_to_read > 0)
                    {
                        SearchLogQueue_A.Enqueue(dataset[index]);
                        index++;
                        data_to_read--;
                    }

                    // string s = "";
                    // textBox1.Invoke(this.myDelegate1, new Object[] { s });

                    DateTime dt;
                    if (DisplayHexOn_A)
                    {
                        // string hexValues = BitConverter.ToString(dataset).Replace("-", "");
                        string hexValues = BitConverter.ToString(dataset).Replace("-", "");
                        dt = DateTime.Now;
                        hexValues = "[Receive_Port_A] [" + dt.ToString("yyyy/MM/dd HH:mm:ss.fff") + "]  " + hexValues + "\r\n"; //OK
                        Extra_Commander.logA_text = string.Concat(Extra_Commander.logA_text, hexValues);
                    }
                    else
                    {
                        // string strValues = String.Concat(Encoding.ASCII.GetString(dataset).Where(c => c != 0x00));
                        string strValues = Encoding.ASCII.GetString(dataset);
                        dt = DateTime.Now;
                        strValues = "[Receive_Port_A] [" + dt.ToString("yyyy/MM/dd HH:mm:ss.fff") + "]  " + strValues + "\r\n"; //OK
                        Extra_Commander.logA_text = string.Concat(Extra_Commander.logA_text, strValues);
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
                    int index = 0;
                    while (data_to_read > 0)
                    {
                        SearchLogQueue_B.Enqueue(dataset[index]);
                        index++;
                        data_to_read--;
                    }

                    DateTime dt;
                    if (DisplayHexOn_B)
                    {
                        // hex to string
                        string hexValues = BitConverter.ToString(dataset).Replace("-", "");
                        DateTime.Now.ToShortTimeString();
                        dt = DateTime.Now;

                        // Joseph
                        hexValues = "[Receive_Port_B] [" + dt.ToString("yyyy/MM/dd HH:mm:ss.fff") + "]  " + hexValues + "\r\n"; //OK
                        Extra_Commander.logB_text = string.Concat(Extra_Commander.logB_text, hexValues);
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
                        strValues = "[Receive_Port_B] [" + dt.ToString("yyyy/MM/dd HH:mm:ss.fff") + "]  " + strValues + "\r\n"; //OK
                        Extra_Commander.logB_text = string.Concat(Extra_Commander.logB_text, strValues);
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
                int data_to_read = PortC.BytesToRead;
                if (data_to_read > 0)
                {
                    byte[] dataset = new byte[data_to_read];

                    PortC.Read(dataset, 0, data_to_read);
                    int index = 0;
                    while (data_to_read > 0)
                    {
                        SearchLogQueue_C.Enqueue(dataset[index]);
                        index++;
                        data_to_read--;
                    }

                    DateTime dt;
                    if (DisplayHexOn_C)
                    {
                        // hex to string
                        string hexValues = BitConverter.ToString(dataset).Replace("-", "");
                        DateTime.Now.ToShortTimeString();
                        dt = DateTime.Now;

                        // Joseph
                        hexValues = "[Receive_Port_C] [" + dt.ToString("yyyy/MM/dd HH:mm:ss.fff") + "]  " + hexValues + "\r\n"; //OK
                        Extra_Commander.logC_text = string.Concat(Extra_Commander.logC_text, hexValues);
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
                        strValues = "[Receive_Port_C] [" + dt.ToString("yyyy/MM/dd HH:mm:ss.fff") + "]  " + strValues + "\r\n"; //OK
                        Extra_Commander.logC_text = string.Concat(Extra_Commander.logC_text, strValues);
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
                int data_to_read = PortD.BytesToRead;
                if (data_to_read > 0)
                {
                    byte[] dataset = new byte[data_to_read];

                    PortD.Read(dataset, 0, data_to_read);
                    int index = 0;
                    while (data_to_read > 0)
                    {
                        SearchLogQueue_D.Enqueue(dataset[index]);
                        index++;
                        data_to_read--;
                    }

                    DateTime dt;
                    if (DisplayHexOn_D)
                    {
                        // hex to string
                        string hexValues = BitConverter.ToString(dataset).Replace("-", "");
                        DateTime.Now.ToShortTimeString();
                        dt = DateTime.Now;

                        // Joseph
                        hexValues = "[Receive_Port_D] [" + dt.ToString("yyyy/MM/dd HH:mm:ss.fff") + "]  " + hexValues + "\r\n"; //OK
                        Extra_Commander.logD_text = string.Concat(Extra_Commander.logD_text, hexValues);
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
                        strValues = "[Receive_Port_D] [" + dt.ToString("yyyy/MM/dd HH:mm:ss.fff") + "]  " + strValues + "\r\n"; //OK
                        Extra_Commander.logD_text = string.Concat(Extra_Commander.logD_text, strValues);
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
                int data_to_read = PortE.BytesToRead;
                if (data_to_read > 0)
                {
                    byte[] dataset = new byte[data_to_read];

                    PortE.Read(dataset, 0, data_to_read);
                    int index = 0;
                    while (data_to_read > 0)
                    {
                        SearchLogQueue_E.Enqueue(dataset[index]);
                        index++;
                        data_to_read--;
                    }

                    DateTime dt;
                    if (DisplayHexOn_E)
                    {
                        // hex to string
                        string hexValues = BitConverter.ToString(dataset).Replace("-", "");
                        DateTime.Now.ToShortTimeString();
                        dt = DateTime.Now;

                        // Joseph
                        hexValues = "[Receive_Port_E] [" + dt.ToString("yyyy/MM/dd HH:mm:ss.fff") + "]  " + hexValues + "\r\n"; //OK
                        Extra_Commander.logE_text = string.Concat(Extra_Commander.logE_text, hexValues);
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
                        strValues = "[Receive_Port_E] [" + dt.ToString("yyyy/MM/dd HH:mm:ss.fff") + "]  " + strValues + "\r\n"; //OK
                        Extra_Commander.logE_text = string.Concat(Extra_Commander.logE_text, strValues);
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
/*
        #region -- 關鍵字比對 - serialport_1 --
        public void MyLog1Camd()
        {
            string my_string = "";
            string csvFile = Init_Parameter.config_parameter.Record_LogPath + "\\PortA_keyword.csv";
            int[] compare_number = new int[10];
            bool[] send_status = new bool[10];
            int compare_paremeter = Convert.ToInt32(Init_Parameter.config_parameter.LogSearch_TextNum);

            while (Global.StartButtonPressed == true)
            {
                while (SearchLogQueue_A.Count > 0)
                {
                    Keyword_SerialPort_A_temp_byte = SearchLogQueue_A.Dequeue();
                    Keyword_SerialPort_A_temp_char = (char)Keyword_SerialPort_A_temp_byte;

                    if (Convert.ToInt32(Init_Parameter.config_parameter.LogSearch_PortA) == 1 && Convert.ToInt32(Init_Parameter.config_parameter.LogSearch_TextNum) > 0)
                    {
                        #region \n
                        if ((Keyword_SerialPort_A_temp_char == '\n'))
                        {
                            for (int i = 0; i < compare_paremeter; i++)
                            {
                                Init_Parameter.config_parameter.LogSearch_Num = i;
                                string compare_string = Init_Parameter.config_parameter.LogSearch_Text + Init_Parameter.config_parameter.LogSearch_Num;
                                int compare_num = Convert.ToInt32(ini12.INIRead(MainSettingPath, "LogSearch", "Times" + i, ""));
                                string[] ewords = my_string.Split(new string[] { compare_string }, StringSplitOptions.None);
                                if (Convert.ToInt32(ewords.Length - 1) >= 1 && my_string.Contains(compare_string) == true)
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
                                        Init_Parameter.config_parameter.LogSearch_Nowvalue = i.ToString();
                                        ini12.INIWrite(MainSettingPath, "LogSearch", "Display" + i, compare_number[i].ToString());
                                        if (Init_Parameter.config_parameter.MailInfo_From != ""
                                            && Init_Parameter.config_parameter.MailInfo_To != ""
                                            && Init_Parameter.config_parameter.LogSearch_Sendmail == "1")
                                        {
                                            FormMail FormMail = new FormMail();
                                            FormMail.logsend();
                                            send_status[i] = true;
                                        }
                                    }
                                    ////////////////////////////////////////////////////////////////////////////////////////////////AC OFF ON//////////////////
                                    if (compare_number[i] % compare_num == 0
                                        && Init_Parameter.config_parameter.Device_AutoboxExist == "1"
                                        && Init_Parameter.config_parameter.LogSearch_ACcontrol == "1")
                                    {
                                        byte[] val1;
                                        val1 = new byte[2];
                                        val1[0] = 0;

                                        bool jSuccess = Autokit_Device_1.PL2303_GP0_Enable(Autokit_Device_1.hCOM, 1);
                                        if (!jSuccess)
                                        {
                                            Log("GP0 output enable FAILED.");
                                        }
                                        else
                                        {
                                            uint val;
                                            val = (uint)int.Parse("0");
                                            bool bSuccess = Autokit_Device_1.PL2303_GP0_SetValue(Autokit_Device_1.hCOM, val);
                                            if (bSuccess)
                                            {
                                                {
                                                    Autokit_Device_1.PowerState = false;
                                                    //pictureBox_AcPower.Image = Properties.Resources.OFF;
                                                }
                                            }
                                        }

                                        System.Threading.Thread.Sleep(5000);

                                        jSuccess = Autokit_Device_1.PL2303_GP0_Enable(Autokit_Device_1.hCOM, 1);
                                        if (!jSuccess)
                                        {
                                            Log("GP0 output enable FAILED.");
                                        }
                                        else
                                        {
                                            uint val;
                                            val = (uint)int.Parse("1");
                                            bool bSuccess = Autokit_Device_1.PL2303_GP0_SetValue(Autokit_Device_1.hCOM, val);
                                            if (bSuccess)
                                            {
                                                {
                                                    Autokit_Device_1.PowerState = true;
                                                    //pictureBox_AcPower.Image = Properties.Resources.ON;
                                                }
                                            }
                                        }
                                    }
                                    ////////////////////////////////////////////////////////////////////////////////////////////////AC OFF//////////////////
                                    if (compare_number[i] % compare_num == 0
                                        && Init_Parameter.config_parameter.Device_AutoboxExist == "1"
                                        && Init_Parameter.config_parameter.LogSearch_ACOFF == "1")
                                    {
                                        byte[] val1 = new byte[2];
                                        val1[0] = 0;
                                        uint val = (uint)int.Parse("0");

                                        bool Success_GP0_Enable = Autokit_Device_1.PL2303_GP0_Enable(Autokit_Device_1.hCOM, 1);
                                        bool Success_GP0_SetValue = Autokit_Device_1.PL2303_GP0_SetValue(Autokit_Device_1.hCOM, val);

                                        bool Success_GP1_Enable = Autokit_Device_1.PL2303_GP1_Enable(Autokit_Device_1.hCOM, 1);
                                        bool Success_GP1_SetValue = Autokit_Device_1.PL2303_GP1_SetValue(Autokit_Device_1.hCOM, val);

                                        Autokit_Device_1.PowerState = false;

                                        //pictureBox_AcPower.Image = Properties.Resources.OFF;
                                    }
                                    ////////////////////////////////////////////////////////////////////////////////////////////////SAVE LOG//////////////////
                                    if (compare_number[i] % compare_num == 0 && Init_Parameter.config_parameter.LogSearch_Savelog == "1")
                                    {
                                        string fName = "";

                                        // 讀取ini中的路徑
                                        fName = Init_Parameter.config_parameter.Record_LogPath;
                                        string t = fName + "\\_SaveLogA_" + DateTime.Now.ToString("yyyyMMddHHmmss") + "_" + Global.label_LoopNumber + ".txt";

                                        StreamWriter MYFILE = new StreamWriter(t, false, Encoding.ASCII);
                                        MYFILE.Write(Extra_Commander.logA_text);
                                        MYFILE.Close();
                                        Extra_Commander.logA_text = string.Empty;
                                        //Txtbox1("", textBox_serial);
                                    }
                                    ////////////////////////////////////////////////////////////////////////////////////////////////STOP//////////////////
                                    if (compare_number[i] % compare_num == 0 && Init_Parameter.config_parameter.LogSearch_Stop == "1")
                                    {
                                        Autokit_Function_1.Start_Function();
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

                        #region \r
                        else if ((Keyword_SerialPort_A_temp_char == '\r'))
                        {
                            for (int i = 0; i < compare_paremeter; i++)
                            {
                                string compare_string = ini12.INIRead(MainSettingPath, "LogSearch", "Text" + i, "");
                                int compare_num = Convert.ToInt32(ini12.INIRead(MainSettingPath, "LogSearch", "Times" + i, ""));
                                string[] ewords = my_string.Split(new string[] { compare_string }, StringSplitOptions.None);

                                if (Convert.ToInt32(ewords.Length - 1) >= 1 && my_string.Contains(compare_string) == true)
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
                                        Init_Parameter.config_parameter.LogSearch_Nowvalue = i.ToString();
                                        ini12.INIWrite(MainSettingPath, "LogSearch", "Display" + i, compare_number[i].ToString());
                                        if (Init_Parameter.config_parameter.MailInfo_From != ""
                                            && Init_Parameter.config_parameter.MailInfo_To  != ""
                                            && Init_Parameter.config_parameter.LogSearch_Sendmail == "1")
                                        {
                                            FormMail FormMail = new FormMail();
                                            FormMail.logsend();
                                            send_status[i] = true;
                                        }
                                    }
                                    ////////////////////////////////////////////////////////////////////////////////////////////////AC OFF ON//////////////////
                                    if (compare_number[i] % compare_num == 0
                                        && Init_Parameter.config_parameter.Device_AutoboxExist == "1"
                                        && Init_Parameter.config_parameter.LogSearch_ACcontrol == "1")
                                    {
                                        byte[] val1;
                                        val1 = new byte[2];
                                        val1[0] = 0;

                                        bool jSuccess = Autokit_Device_1.PL2303_GP0_Enable(Autokit_Device_1.hCOM, 1);
                                        if (!jSuccess)
                                        {
                                            Log("GP0 output enable FAILED.");
                                        }
                                        else
                                        {
                                            uint val;
                                            val = (uint)int.Parse("0");
                                            bool bSuccess = Autokit_Device_1.PL2303_GP0_SetValue(Autokit_Device_1.hCOM, val);
                                            if (bSuccess)
                                            {
                                                {
                                                    Autokit_Device_1.PowerState = false;
                                                }
                                            }
                                        }

                                        System.Threading.Thread.Sleep(5000);

                                        jSuccess = Autokit_Device_1.PL2303_GP0_Enable(Autokit_Device_1.hCOM, 1);
                                        if (!jSuccess)
                                        {
                                            Log("GP0 output enable FAILED.");
                                        }
                                        else
                                        {
                                            uint val;
                                            val = (uint)int.Parse("1");
                                            bool bSuccess = Autokit_Device_1.PL2303_GP0_SetValue(Autokit_Device_1.hCOM, val);
                                            if (bSuccess)
                                            {
                                                {
                                                    Autokit_Device_1.PowerState = true;
                                                }
                                            }
                                        }
                                    }
                                    ////////////////////////////////////////////////////////////////////////////////////////////////AC OFF//////////////////
                                    if (compare_number[i] % compare_num == 0
                                        && Init_Parameter.config_parameter.Device_AutoboxExist == "1"
                                        && Init_Parameter.config_parameter.LogSearch_ACOFF == "1")
                                    {
                                        byte[] val1 = new byte[2];
                                        val1[0] = 0;
                                        uint val = (uint)int.Parse("0");

                                        bool Success_GP0_Enable = Autokit_Device_1.PL2303_GP0_Enable(Autokit_Device_1.hCOM, 1);
                                        bool Success_GP0_SetValue = Autokit_Device_1.PL2303_GP0_SetValue(Autokit_Device_1.hCOM, val);

                                        bool Success_GP1_Enable = Autokit_Device_1.PL2303_GP1_Enable(Autokit_Device_1.hCOM, 1);
                                        bool Success_GP1_SetValue = Autokit_Device_1.PL2303_GP1_SetValue(Autokit_Device_1.hCOM, val);

                                        Autokit_Device_1.PowerState = false;

                                    }
                                    ////////////////////////////////////////////////////////////////////////////////////////////////SAVE LOG//////////////////
                                    if (compare_number[i] % compare_num == 0 && Init_Parameter.config_parameter.LogSearch_Savelog == "1")
                                    {
                                        string fName = "";

                                        // 讀取ini中的路徑
                                        fName = Init_Parameter.config_parameter.Record_LogPath;
                                        string t = fName + "\\_SaveLogA_" + DateTime.Now.ToString("yyyyMMddHHmmss") + "_" + Global.label_LoopNumber + ".txt";

                                        StreamWriter MYFILE = new StreamWriter(t, false, Encoding.ASCII);
                                        MYFILE.Write(Extra_Commander.logA_text);
                                        MYFILE.Close();
                                        Extra_Commander.logA_text = string.Empty;
                                        //Txtbox1("", textBox_serial);
                                    }
                                    ////////////////////////////////////////////////////////////////////////////////////////////////STOP//////////////////
                                    if (compare_number[i] % compare_num == 0 && Init_Parameter.config_parameter.LogSearch_Stop == "1")
                                    {
                                        Autokit_Function_1.Start_Function();
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
                        else
                        {
                            my_string = my_string + Keyword_SerialPort_A_temp_char;
                        }
                    }
                    else
                    {
                        if ((Keyword_SerialPort_A_temp_char == '\n'))
                        {
                            //textBox1.AppendText(my_string + '\n');
                            my_string = "";
                        }
                        else if ((Keyword_SerialPort_A_temp_char == '\r'))
                        {
                            //textBox1.AppendText(my_string + '\r');
                            my_string = "";
                        }
                        else
                        {
                            my_string = my_string + Keyword_SerialPort_A_temp_char;
                        }
                    }
                }
                Thread.Sleep(500);
            }
        }
        #endregion

        #region -- 關鍵字比對 - serialport_2 --
        public void MyLog2Camd()
        {
            string my_string = "";
            string csvFile = Init_Parameter.config_parameter.Record_LogPath + "\\PortB_keyword.csv";
            int[] compare_number = new int[10];
            bool[] send_status = new bool[10];
            int compare_paremeter = Convert.ToInt32(Init_Parameter.config_parameter.LogSearch_TextNum);

            while (Global.StartButtonPressed == true)
            {
                while (SearchLogQueue_B.Count > 0)
                {
                    Keyword_SerialPort_B_temp_byte = SearchLogQueue_B.Dequeue();
                    Keyword_SerialPort_B_temp_char = (char)Keyword_SerialPort_B_temp_byte;

                    if (Convert.ToInt32(Init_Parameter.config_parameter.LogSearch_PortB) == 1 && Convert.ToInt32(Init_Parameter.config_parameter.LogSearch_TextNum) > 0)
                    {
                        #region \n
                        if ((Keyword_SerialPort_B_temp_char == '\n'))
                        {
                            for (int i = 0; i < compare_paremeter; i++)
                            {
                                string compare_string = ini12.INIRead(MainSettingPath, "LogSearch", "Text" + i, "");
                                int compare_num = Convert.ToInt32(ini12.INIRead(MainSettingPath, "LogSearch", "Times" + i, ""));
                                string[] ewords = my_string.Split(new string[] { compare_string }, StringSplitOptions.None);
                                if (Convert.ToInt32(ewords.Length - 1) >= 1 && my_string.Contains(compare_string) == true)
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
                                        Init_Parameter.config_parameter.LogSearch_Nowvalue = i.ToString();
                                        ini12.INIWrite(MainSettingPath, "LogSearch", "Display" + i, compare_number[i].ToString());
                                        if (Init_Parameter.config_parameter.MailInfo_From != ""
                                            && Init_Parameter.config_parameter.MailInfo_To  != ""
                                            && Init_Parameter.config_parameter.LogSearch_Sendmail == "1")
                                        {
                                            FormMail FormMail = new FormMail();
                                            FormMail.logsend();
                                            send_status[i] = true;
                                        }
                                    }
                                    ////////////////////////////////////////////////////////////////////////////////////////////////AC OFF ON//////////////////
                                    if (compare_number[i] % compare_num == 0
                                        && Init_Parameter.config_parameter.Device_AutoboxExist == "1"
                                        && Init_Parameter.config_parameter.LogSearch_ACcontrol == "1")
                                    {
                                        byte[] val1;
                                        val1 = new byte[2];
                                        val1[0] = 0;

                                        bool jSuccess = Autokit_Device_1.PL2303_GP0_Enable(Autokit_Device_1.hCOM, 1);
                                        if (!jSuccess)
                                        {
                                            Log("GP0 output enable FAILED.");
                                        }
                                        else
                                        {
                                            uint val;
                                            val = (uint)int.Parse("0");
                                            bool bSuccess = Autokit_Device_1.PL2303_GP0_SetValue(Autokit_Device_1.hCOM, val);
                                            if (bSuccess)
                                            {
                                                {
                                                    Autokit_Device_1.PowerState = false;
                                                    //pictureBox_AcPower.Image = Properties.Resources.OFF;
                                                }
                                            }
                                        }

                                        System.Threading.Thread.Sleep(5000);

                                        jSuccess = Autokit_Device_1.PL2303_GP0_Enable(Autokit_Device_1.hCOM, 1);
                                        if (!jSuccess)
                                        {
                                            Log("GP0 output enable FAILED.");
                                        }
                                        else
                                        {
                                            uint val;
                                            val = (uint)int.Parse("1");
                                            bool bSuccess = Autokit_Device_1.PL2303_GP0_SetValue(Autokit_Device_1.hCOM, val);
                                            if (bSuccess)
                                            {
                                                {
                                                    Autokit_Device_1.PowerState = true;
                                                    //pictureBox_AcPower.Image = Properties.Resources.ON;
                                                }
                                            }
                                        }
                                    }
                                    ////////////////////////////////////////////////////////////////////////////////////////////////AC OFF//////////////////
                                    if (compare_number[i] % compare_num == 0
                                        && Init_Parameter.config_parameter.Device_AutoboxExist == "1"
                                        && Init_Parameter.config_parameter.LogSearch_ACOFF == "1")
                                    {
                                        byte[] val1 = new byte[2];
                                        val1[0] = 0;
                                        uint val = (uint)int.Parse("0");

                                        bool Success_GP0_Enable = Autokit_Device_1.PL2303_GP0_Enable(Autokit_Device_1.hCOM, 1);
                                        bool Success_GP0_SetValue = Autokit_Device_1.PL2303_GP0_SetValue(Autokit_Device_1.hCOM, val);

                                        bool Success_GP1_Enable = Autokit_Device_1.PL2303_GP1_Enable(Autokit_Device_1.hCOM, 1);
                                        bool Success_GP1_SetValue = Autokit_Device_1.PL2303_GP1_SetValue(Autokit_Device_1.hCOM, val);

                                        Autokit_Device_1.PowerState = false;

                                        //pictureBox_AcPower.Image = Properties.Resources.OFF;
                                    }
                                    ////////////////////////////////////////////////////////////////////////////////////////////////SAVE LOG//////////////////
                                    if (compare_number[i] % compare_num == 0 && Init_Parameter.config_parameter.LogSearch_Savelog == "1")
                                    {
                                        string fName = "";

                                        // 讀取ini中的路徑
                                        fName = Init_Parameter.config_parameter.Record_LogPath;
                                        string t = fName + "\\_SaveLogB_" + DateTime.Now.ToString("yyyyMMddHHmmss") + "_" + Global.label_LoopNumber + ".txt";

                                        StreamWriter MYFILE = new StreamWriter(t, false, Encoding.ASCII);
                                        MYFILE.Write(Extra_Commander.logB_text);
                                        MYFILE.Close();
                                        Extra_Commander.logB_text = string.Empty;
                                    }
                                    ////////////////////////////////////////////////////////////////////////////////////////////////SCHEDULE//////////////////
                                    if (compare_number[i] % compare_num == 0 && Init_Parameter.config_parameter.LogSearch_Stop == "1")
                                    {
                                        Autokit_Function_1.Start_Function();
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
                        else if ((Keyword_SerialPort_B_temp_char == '\r'))
                        {
                            for (int i = 0; i < compare_paremeter; i++)
                            {
                                string compare_string = ini12.INIRead(MainSettingPath, "LogSearch", "Text" + i, "");
                                int compare_num = Convert.ToInt32(ini12.INIRead(MainSettingPath, "LogSearch", "Times" + i, ""));
                                string[] ewords = my_string.Split(new string[] { compare_string }, StringSplitOptions.None);
                                if (Convert.ToInt32(ewords.Length - 1) >= 1 && my_string.Contains(compare_string) == true)
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
                                        Init_Parameter.config_parameter.LogSearch_Nowvalue = i.ToString();
                                        ini12.INIWrite(MainSettingPath, "LogSearch", "Display" + i, compare_number[i].ToString());
                                        if (Init_Parameter.config_parameter.MailInfo_From != ""
                                            && Init_Parameter.config_parameter.MailInfo_To  != ""
                                            && Init_Parameter.config_parameter.LogSearch_Sendmail == "1")
                                        {
                                            FormMail FormMail = new FormMail();
                                            FormMail.logsend();
                                            send_status[i] = true;
                                        }
                                    }
                                    ////////////////////////////////////////////////////////////////////////////////////////////////AC OFF ON//////////////////
                                    if (compare_number[i] % compare_num == 0
                                        && Init_Parameter.config_parameter.Device_AutoboxExist == "1"
                                        && Init_Parameter.config_parameter.LogSearch_ACcontrol == "1")
                                    {
                                        byte[] val1;
                                        val1 = new byte[2];
                                        val1[0] = 0;

                                        bool jSuccess = Autokit_Device_1.PL2303_GP0_Enable(Autokit_Device_1.hCOM, 1);
                                        if (!jSuccess)
                                        {
                                            Log("GP0 output enable FAILED.");
                                        }
                                        else
                                        {
                                            uint val;
                                            val = (uint)int.Parse("0");
                                            bool bSuccess = Autokit_Device_1.PL2303_GP0_SetValue(Autokit_Device_1.hCOM, val);
                                            if (bSuccess)
                                            {
                                                {
                                                    Autokit_Device_1.PowerState = false;
                                                    //pictureBox_AcPower.Image = Properties.Resources.OFF;
                                                }
                                            }
                                        }

                                        System.Threading.Thread.Sleep(5000);

                                        jSuccess = Autokit_Device_1.PL2303_GP0_Enable(Autokit_Device_1.hCOM, 1);
                                        if (!jSuccess)
                                        {
                                            Log("GP0 output enable FAILED.");
                                        }
                                        else
                                        {
                                            uint val;
                                            val = (uint)int.Parse("1");
                                            bool bSuccess = Autokit_Device_1.PL2303_GP0_SetValue(Autokit_Device_1.hCOM, val);
                                            if (bSuccess)
                                            {
                                                {
                                                    Autokit_Device_1.PowerState = true;
                                                    //pictureBox_AcPower.Image = Properties.Resources.ON;
                                                }
                                            }
                                        }
                                    }
                                    ////////////////////////////////////////////////////////////////////////////////////////////////AC OFF//////////////////
                                    if (compare_number[i] % compare_num == 0
                                        && Init_Parameter.config_parameter.Device_AutoboxExist == "1"
                                        && Init_Parameter.config_parameter.LogSearch_ACOFF == "1")
                                    {
                                        byte[] val1 = new byte[2];
                                        val1[0] = 0;
                                        uint val = (uint)int.Parse("0");

                                        bool Success_GP0_Enable = Autokit_Device_1.PL2303_GP0_Enable(Autokit_Device_1.hCOM, 1);
                                        bool Success_GP0_SetValue = Autokit_Device_1.PL2303_GP0_SetValue(Autokit_Device_1.hCOM, val);

                                        bool Success_GP1_Enable = Autokit_Device_1.PL2303_GP1_Enable(Autokit_Device_1.hCOM, 1);
                                        bool Success_GP1_SetValue = Autokit_Device_1.PL2303_GP1_SetValue(Autokit_Device_1.hCOM, val);

                                        Autokit_Device_1.PowerState = false;

                                        //pictureBox_AcPower.Image = Properties.Resources.OFF;
                                    }
                                    ////////////////////////////////////////////////////////////////////////////////////////////////SAVE LOG//////////////////
                                    if (compare_number[i] % compare_num == 0 && Init_Parameter.config_parameter.LogSearch_Savelog == "1")
                                    {
                                        string fName = "";

                                        // 讀取ini中的路徑
                                        fName = Init_Parameter.config_parameter.Record_LogPath;
                                        string t = fName + "\\_SaveLogB_" + DateTime.Now.ToString("yyyyMMddHHmmss") + "_" + Global.label_LoopNumber + ".txt";

                                        StreamWriter MYFILE = new StreamWriter(t, false, Encoding.ASCII);
                                        MYFILE.Write(Extra_Commander.logB_text);
                                        MYFILE.Close();
                                        Extra_Commander.logB_text = string.Empty;
                                    }
                                    ////////////////////////////////////////////////////////////////////////////////////////////////STOP//////////////////
                                    if (compare_number[i] % compare_num == 0 && Init_Parameter.config_parameter.LogSearch_Stop == "1")
                                    {
                                        Autokit_Function_1.Start_Function();
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
                            my_string = my_string + Keyword_SerialPort_B_temp_char;
                        }
                    }
                    else
                    {

                        if ((Keyword_SerialPort_B_temp_char == '\n'))
                        {
                            //textBox2.AppendText(my_string + '\n');
                            my_string = "";
                        }
                        else if ((Keyword_SerialPort_B_temp_char == '\r'))
                        {
                            //textBox2.AppendText(my_string + '\r');
                            my_string = "";
                        }
                        else
                        {
                            my_string = my_string + Keyword_SerialPort_B_temp_char;
                        }
                    }
                }
                Thread.Sleep(500);
            }
        }
        #endregion

        #region -- 關鍵字比對 - serialport_3 --
        public void MyLog3Camd()
        {
            string my_string = "";
            string csvFile = Init_Parameter.config_parameter.Record_LogPath + "\\PortC_keyword.csv";
            int[] compare_number = new int[10];
            bool[] send_status = new bool[10];
            int compare_paremeter = Convert.ToInt32(Init_Parameter.config_parameter.LogSearch_TextNum);

            while (Global.StartButtonPressed == true)
            {
                while (SearchLogQueue_C.Count > 0)
                {
                    Keyword_SerialPort_C_temp_byte = SearchLogQueue_C.Dequeue();
                    Keyword_SerialPort_C_temp_char = (char)Keyword_SerialPort_C_temp_byte;

                    if (Convert.ToInt32(Init_Parameter.config_parameter.LogSearch_PortC) == 1 && Convert.ToInt32(Init_Parameter.config_parameter.LogSearch_TextNum) > 0)
                    {
                        #region \n
                        if ((Keyword_SerialPort_C_temp_char == '\n'))
                        {
                            for (int i = 0; i < compare_paremeter; i++)
                            {
                                string compare_string = ini12.INIRead(MainSettingPath, "LogSearch", "Text" + i, "");
                                int compare_num = Convert.ToInt32(ini12.INIRead(MainSettingPath, "LogSearch", "Times" + i, ""));
                                string[] ewords = my_string.Split(new string[] { compare_string }, StringSplitOptions.None);
                                if (Convert.ToInt32(ewords.Length - 1) >= 1 && my_string.Contains(compare_string) == true)
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
                                        Init_Parameter.config_parameter.LogSearch_Nowvalue = i.ToString();
                                        ini12.INIWrite(MainSettingPath, "LogSearch", "Display" + i, compare_number[i].ToString());
                                        if (Init_Parameter.config_parameter.MailInfo_From != ""
                                            && Init_Parameter.config_parameter.MailInfo_To  != ""
                                            && Init_Parameter.config_parameter.LogSearch_Sendmail == "1")
                                        {
                                            FormMail FormMail = new FormMail();
                                            FormMail.logsend();
                                            send_status[i] = true;
                                        }
                                    }
                                    ////////////////////////////////////////////////////////////////////////////////////////////////AC OFF ON//////////////////
                                    if (compare_number[i] % compare_num == 0
                                        && Init_Parameter.config_parameter.Device_AutoboxExist == "1"
                                        && Init_Parameter.config_parameter.LogSearch_ACcontrol == "1")
                                    {
                                        byte[] val1;
                                        val1 = new byte[2];
                                        val1[0] = 0;

                                        bool jSuccess = Autokit_Device_1.PL2303_GP0_Enable(Autokit_Device_1.hCOM, 1);
                                        if (!jSuccess)
                                        {
                                            Log("GP0 output enable FAILED.");
                                        }
                                        else
                                        {
                                            uint val;
                                            val = (uint)int.Parse("0");
                                            bool bSuccess = Autokit_Device_1.PL2303_GP0_SetValue(Autokit_Device_1.hCOM, val);
                                            if (bSuccess)
                                            {
                                                {
                                                    Autokit_Device_1.PowerState = false;
                                                    //pictureBox_AcPower.Image = Properties.Resources.OFF;
                                                }
                                            }
                                        }

                                        System.Threading.Thread.Sleep(5000);

                                        jSuccess = Autokit_Device_1.PL2303_GP0_Enable(Autokit_Device_1.hCOM, 1);
                                        if (!jSuccess)
                                        {
                                            Log("GP0 output enable FAILED.");
                                        }
                                        else
                                        {
                                            uint val;
                                            val = (uint)int.Parse("1");
                                            bool bSuccess = Autokit_Device_1.PL2303_GP0_SetValue(Autokit_Device_1.hCOM, val);
                                            if (bSuccess)
                                            {
                                                {
                                                    Autokit_Device_1.PowerState = true;
                                                    //pictureBox_AcPower.Image = Properties.Resources.ON;
                                                }
                                            }
                                        }
                                    }
                                    ////////////////////////////////////////////////////////////////////////////////////////////////AC OFF//////////////////
                                    if (compare_number[i] % compare_num == 0
                                        && Init_Parameter.config_parameter.Device_AutoboxExist == "1"
                                        && Init_Parameter.config_parameter.LogSearch_ACOFF == "1")
                                    {
                                        byte[] val1 = new byte[2];
                                        val1[0] = 0;
                                        uint val = (uint)int.Parse("0");

                                        bool Success_GP0_Enable = Autokit_Device_1.PL2303_GP0_Enable(Autokit_Device_1.hCOM, 1);
                                        bool Success_GP0_SetValue = Autokit_Device_1.PL2303_GP0_SetValue(Autokit_Device_1.hCOM, val);

                                        bool Success_GP1_Enable = Autokit_Device_1.PL2303_GP1_Enable(Autokit_Device_1.hCOM, 1);
                                        bool Success_GP1_SetValue = Autokit_Device_1.PL2303_GP1_SetValue(Autokit_Device_1.hCOM, val);

                                        Autokit_Device_1.PowerState = false;

                                        //pictureBox_AcPower.Image = Properties.Resources.OFF;
                                    }
                                    ////////////////////////////////////////////////////////////////////////////////////////////////SAVE LOG//////////////////
                                    if (compare_number[i] % compare_num == 0 && Init_Parameter.config_parameter.LogSearch_Savelog == "1")
                                    {
                                        string fName = "";

                                        // 讀取ini中的路徑
                                        fName = Init_Parameter.config_parameter.Record_LogPath;
                                        string t = fName + "\\_SaveLogC_" + DateTime.Now.ToString("yyyyMMddHHmmss") + "_" + Global.label_LoopNumber + ".txt";

                                        StreamWriter MYFILE = new StreamWriter(t, false, Encoding.ASCII);
                                        MYFILE.Write(Extra_Commander.logC_text);
                                        MYFILE.Close();
                                        Extra_Commander.logC_text = string.Empty;
                                    }
                                    ////////////////////////////////////////////////////////////////////////////////////////////////SCHEDULE//////////////////
                                    if (compare_number[i] % compare_num == 0 && Init_Parameter.config_parameter.LogSearch_Stop == "1")
                                    {
                                        Autokit_Function_1.Start_Function();
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
                        else if ((Keyword_SerialPort_C_temp_char == '\r'))
                        {
                            for (int i = 0; i < compare_paremeter; i++)
                            {
                                string compare_string = Init_Parameter.config_parameter.LogSearch_Text;
                                int compare_num = Convert.ToInt32(ini12.INIRead(MainSettingPath, "LogSearch", "Times" + i, ""));
                                string[] ewords = my_string.Split(new string[] { compare_string }, StringSplitOptions.None);
                                if (Convert.ToInt32(ewords.Length - 1) >= 1 && my_string.Contains(compare_string) == true)
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
                                        Init_Parameter.config_parameter.LogSearch_Nowvalue = i.ToString();
                                        ini12.INIWrite(MainSettingPath, "LogSearch", "Display" + i, compare_number[i].ToString());
                                        if (Init_Parameter.config_parameter.MailInfo_From != ""
                                            && Init_Parameter.config_parameter.MailInfo_To  != ""
                                            && Init_Parameter.config_parameter.LogSearch_Sendmail == "1")
                                        {
                                            FormMail FormMail = new FormMail();
                                            FormMail.logsend();
                                            send_status[i] = true;
                                        }
                                    }
                                    ////////////////////////////////////////////////////////////////////////////////////////////////AC OFF ON//////////////////
                                    if (compare_number[i] % compare_num == 0
                                        && Init_Parameter.config_parameter.Device_AutoboxExist == "1"
                                        && Init_Parameter.config_parameter.LogSearch_ACcontrol == "1")
                                    {
                                        byte[] val1;
                                        val1 = new byte[2];
                                        val1[0] = 0;

                                        bool jSuccess = Autokit_Device_1.PL2303_GP0_Enable(Autokit_Device_1.hCOM, 1);
                                        if (!jSuccess)
                                        {
                                            Log("GP0 output enable FAILED.");
                                        }
                                        else
                                        {
                                            uint val;
                                            val = (uint)int.Parse("0");
                                            bool bSuccess = Autokit_Device_1.PL2303_GP0_SetValue(Autokit_Device_1.hCOM, val);
                                            if (bSuccess)
                                            {
                                                {
                                                    Autokit_Device_1.PowerState = false;
                                                    //pictureBox_AcPower.Image = Properties.Resources.OFF;
                                                }
                                            }
                                        }

                                        System.Threading.Thread.Sleep(5000);

                                        jSuccess = Autokit_Device_1.PL2303_GP0_Enable(Autokit_Device_1.hCOM, 1);
                                        if (!jSuccess)
                                        {
                                            Log("GP0 output enable FAILED.");
                                        }
                                        else
                                        {
                                            uint val;
                                            val = (uint)int.Parse("1");
                                            bool bSuccess = Autokit_Device_1.PL2303_GP0_SetValue(Autokit_Device_1.hCOM, val);
                                            if (bSuccess)
                                            {
                                                {
                                                    Autokit_Device_1.PowerState = true;
                                                    //pictureBox_AcPower.Image = Properties.Resources.ON;
                                                }
                                            }
                                        }
                                    }
                                    ////////////////////////////////////////////////////////////////////////////////////////////////AC OFF//////////////////
                                    if (compare_number[i] % compare_num == 0
                                        && Init_Parameter.config_parameter.Device_AutoboxExist == "1"
                                        && Init_Parameter.config_parameter.LogSearch_ACOFF == "1")
                                    {
                                        byte[] val1 = new byte[2];
                                        val1[0] = 0;
                                        uint val = (uint)int.Parse("0");

                                        bool Success_GP0_Enable = Autokit_Device_1.PL2303_GP0_Enable(Autokit_Device_1.hCOM, 1);
                                        bool Success_GP0_SetValue = Autokit_Device_1.PL2303_GP0_SetValue(Autokit_Device_1.hCOM, val);

                                        bool Success_GP1_Enable = Autokit_Device_1.PL2303_GP1_Enable(Autokit_Device_1.hCOM, 1);
                                        bool Success_GP1_SetValue = Autokit_Device_1.PL2303_GP1_SetValue(Autokit_Device_1.hCOM, val);

                                        Autokit_Device_1.PowerState = false;

                                        //pictureBox_AcPower.Image = Properties.Resources.OFF;
                                    }
                                    ////////////////////////////////////////////////////////////////////////////////////////////////SAVE LOG//////////////////
                                    if (compare_number[i] % compare_num == 0 && Init_Parameter.config_parameter.LogSearch_Savelog == "1")
                                    {
                                        string fName = "";

                                        // 讀取ini中的路徑
                                        fName = Init_Parameter.config_parameter.Record_LogPath;
                                        string t = fName + "\\_SaveLogC_" + DateTime.Now.ToString("yyyyMMddHHmmss") + "_" + Global.label_LoopNumber + ".txt";

                                        StreamWriter MYFILE = new StreamWriter(t, false, Encoding.ASCII);
                                        MYFILE.Write(Extra_Commander.logC_text);
                                        MYFILE.Close();
                                        Extra_Commander.logC_text = string.Empty;
                                    }
                                    ////////////////////////////////////////////////////////////////////////////////////////////////STOP//////////////////
                                    if (compare_number[i] % compare_num == 0 && Init_Parameter.config_parameter.LogSearch_Stop == "1")
                                    {
                                        Autokit_Function_1.Start_Function();
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
                            my_string = my_string + Keyword_SerialPort_C_temp_char;
                        }
                    }
                    else
                    {

                        if ((Keyword_SerialPort_C_temp_char == '\n'))
                        {
                            //textBox3.AppendText(my_string + '\n');
                            my_string = "";
                        }
                        else if ((Keyword_SerialPort_C_temp_char == '\r'))
                        {
                            //textBox3.AppendText(my_string + '\r');
                            my_string = "";
                        }
                        else
                        {
                            my_string = my_string + Keyword_SerialPort_C_temp_char;
                        }
                    }
                }
                Thread.Sleep(500);
            }
        }
        #endregion

        #region -- 關鍵字比對 - serialport_4 --
        public void MyLog4Camd()
        {
            string my_string = "";
            string csvFile = Init_Parameter.config_parameter.Record_LogPath + "\\PortC_keyword.csv";
            int[] compare_number = new int[10];
            bool[] send_status = new bool[10];
            int compare_paremeter = Convert.ToInt32(Init_Parameter.config_parameter.LogSearch_TextNum);

            while (Global.StartButtonPressed == true)
            {
                while (SearchLogQueue_D.Count > 0)
                {
                    Keyword_SerialPort_D_temp_byte = SearchLogQueue_D.Dequeue();
                    Keyword_SerialPort_D_temp_char = (char)Keyword_SerialPort_D_temp_byte;

                    if (Convert.ToInt32(Init_Parameter.config_parameter.LogSearch_PortD) == 1 && Convert.ToInt32(Init_Parameter.config_parameter.LogSearch_TextNum) > 0)
                    {
                        #region \n
                        if ((Keyword_SerialPort_D_temp_char == '\n'))
                        {
                            for (int i = 0; i < compare_paremeter; i++)
                            {
                                string compare_string = ini12.INIRead(MainSettingPath, "LogSearch", "Text" + i, "");
                                int compare_num = Convert.ToInt32(ini12.INIRead(MainSettingPath, "LogSearch", "Times" + i, ""));
                                string[] ewords = my_string.Split(new string[] { compare_string }, StringSplitOptions.None);
                                if (Convert.ToInt32(ewords.Length - 1) >= 1 && my_string.Contains(compare_string) == true)
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
                                        Init_Parameter.config_parameter.LogSearch_Nowvalue = i.ToString();
                                        ini12.INIWrite(MainSettingPath, "LogSearch", "Display" + i, compare_number[i].ToString());
                                        if (Init_Parameter.config_parameter.MailInfo_From != ""
                                            && Init_Parameter.config_parameter.MailInfo_To  != ""
                                            && Init_Parameter.config_parameter.LogSearch_Sendmail == "1")
                                        {
                                            FormMail FormMail = new FormMail();
                                            FormMail.logsend();
                                            send_status[i] = true;
                                        }
                                    }
                                    ////////////////////////////////////////////////////////////////////////////////////////////////AC OFF ON//////////////////
                                    if (compare_number[i] % compare_num == 0
                                        && Init_Parameter.config_parameter.Device_AutoboxExist == "1"
                                        && Init_Parameter.config_parameter.LogSearch_ACcontrol == "1")
                                    {
                                        byte[] val1;
                                        val1 = new byte[2];
                                        val1[0] = 0;

                                        bool jSuccess = Autokit_Device_1.PL2303_GP0_Enable(Autokit_Device_1.hCOM, 1);
                                        if (!jSuccess)
                                        {
                                            Log("GP0 output enable FAILED.");
                                        }
                                        else
                                        {
                                            uint val;
                                            val = (uint)int.Parse("0");
                                            bool bSuccess = Autokit_Device_1.PL2303_GP0_SetValue(Autokit_Device_1.hCOM, val);
                                            if (bSuccess)
                                            {
                                                {
                                                    Autokit_Device_1.PowerState = false;
                                                    //pictureBox_AcPower.Image = Properties.Resources.OFF;
                                                }
                                            }
                                        }

                                        System.Threading.Thread.Sleep(5000);

                                        jSuccess = Autokit_Device_1.PL2303_GP0_Enable(Autokit_Device_1.hCOM, 1);
                                        if (!jSuccess)
                                        {
                                            Log("GP0 output enable FAILED.");
                                        }
                                        else
                                        {
                                            uint val;
                                            val = (uint)int.Parse("1");
                                            bool bSuccess = Autokit_Device_1.PL2303_GP0_SetValue(Autokit_Device_1.hCOM, val);
                                            if (bSuccess)
                                            {
                                                {
                                                    Autokit_Device_1.PowerState = true;
                                                    //pictureBox_AcPower.Image = Properties.Resources.ON;
                                                }
                                            }
                                        }
                                    }
                                    ////////////////////////////////////////////////////////////////////////////////////////////////AC OFF//////////////////
                                    if (compare_number[i] % compare_num == 0
                                        && Init_Parameter.config_parameter.Device_AutoboxExist == "1"
                                        && Init_Parameter.config_parameter.LogSearch_ACOFF == "1")
                                    {
                                        byte[] val1 = new byte[2];
                                        val1[0] = 0;
                                        uint val = (uint)int.Parse("0");

                                        bool Success_GP0_Enable = Autokit_Device_1.PL2303_GP0_Enable(Autokit_Device_1.hCOM, 1);
                                        bool Success_GP0_SetValue = Autokit_Device_1.PL2303_GP0_SetValue(Autokit_Device_1.hCOM, val);

                                        bool Success_GP1_Enable = Autokit_Device_1.PL2303_GP1_Enable(Autokit_Device_1.hCOM, 1);
                                        bool Success_GP1_SetValue = Autokit_Device_1.PL2303_GP1_SetValue(Autokit_Device_1.hCOM, val);

                                        Autokit_Device_1.PowerState = false;

                                        //pictureBox_AcPower.Image = Properties.Resources.OFF;
                                    }
                                    ////////////////////////////////////////////////////////////////////////////////////////////////SAVE LOG//////////////////
                                    if (compare_number[i] % compare_num == 0 && Init_Parameter.config_parameter.LogSearch_Savelog == "1")
                                    {
                                        string fName = "";

                                        // 讀取ini中的路徑
                                        fName = Init_Parameter.config_parameter.Record_LogPath;
                                        string t = fName + "\\_SaveLogD_" + DateTime.Now.ToString("yyyyMMddHHmmss") + "_" + Global.label_LoopNumber + ".txt";

                                        StreamWriter MYFILE = new StreamWriter(t, false, Encoding.ASCII);
                                        MYFILE.Write(Extra_Commander.logD_text);
                                        MYFILE.Close();
                                        Extra_Commander.logD_text = string.Empty;
                                    }
                                    ////////////////////////////////////////////////////////////////////////////////////////////////SCHEDULE//////////////////
                                    if (compare_number[i] % compare_num == 0 && Init_Parameter.config_parameter.LogSearch_Stop == "1")
                                    {
                                        Autokit_Function_1.Start_Function();
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
                        else if ((Keyword_SerialPort_D_temp_char == '\r'))
                        {
                            for (int i = 0; i < compare_paremeter; i++)
                            {
                                string compare_string = ini12.INIRead(MainSettingPath, "LogSearch", "Text" + i, "");
                                int compare_num = Convert.ToInt32(ini12.INIRead(MainSettingPath, "LogSearch", "Times" + i, ""));
                                string[] ewords = my_string.Split(new string[] { compare_string }, StringSplitOptions.None);
                                if (Convert.ToInt32(ewords.Length - 1) >= 1 && my_string.Contains(compare_string) == true)
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
                                        Init_Parameter.config_parameter.LogSearch_Nowvalue = i.ToString();
                                        ini12.INIWrite(MainSettingPath, "LogSearch", "Display" + i, compare_number[i].ToString());
                                        if (Init_Parameter.config_parameter.MailInfo_From != ""
                                            && Init_Parameter.config_parameter.MailInfo_To  != ""
                                            && Init_Parameter.config_parameter.LogSearch_Sendmail == "1")
                                        {
                                            FormMail FormMail = new FormMail();
                                            FormMail.logsend();
                                            send_status[i] = true;
                                        }
                                    }
                                    ////////////////////////////////////////////////////////////////////////////////////////////////AC OFF ON//////////////////
                                    if (compare_number[i] % compare_num == 0
                                        && Init_Parameter.config_parameter.Device_AutoboxExist == "1"
                                        && Init_Parameter.config_parameter.LogSearch_ACcontrol == "1")
                                    {
                                        byte[] val1;
                                        val1 = new byte[2];
                                        val1[0] = 0;

                                        bool jSuccess = Autokit_Device_1.PL2303_GP0_Enable(Autokit_Device_1.hCOM, 1);
                                        if (!jSuccess)
                                        {
                                            Log("GP0 output enable FAILED.");
                                        }
                                        else
                                        {
                                            uint val;
                                            val = (uint)int.Parse("0");
                                            bool bSuccess = Autokit_Device_1.PL2303_GP0_SetValue(Autokit_Device_1.hCOM, val);
                                            if (bSuccess)
                                            {
                                                {
                                                    Autokit_Device_1.PowerState = false;
                                                    //pictureBox_AcPower.Image = Properties.Resources.OFF;
                                                }
                                            }
                                        }

                                        System.Threading.Thread.Sleep(5000);

                                        jSuccess = Autokit_Device_1.PL2303_GP0_Enable(Autokit_Device_1.hCOM, 1);
                                        if (!jSuccess)
                                        {
                                            Log("GP0 output enable FAILED.");
                                        }
                                        else
                                        {
                                            uint val;
                                            val = (uint)int.Parse("1");
                                            bool bSuccess = Autokit_Device_1.PL2303_GP0_SetValue(Autokit_Device_1.hCOM, val);
                                            if (bSuccess)
                                            {
                                                {
                                                    Autokit_Device_1.PowerState = true;
                                                    //pictureBox_AcPower.Image = Properties.Resources.ON;
                                                }
                                            }
                                        }
                                    }
                                    ////////////////////////////////////////////////////////////////////////////////////////////////AC OFF//////////////////
                                    if (compare_number[i] % compare_num == 0
                                        && Init_Parameter.config_parameter.Device_AutoboxExist == "1"
                                        && Init_Parameter.config_parameter.LogSearch_ACOFF == "1")
                                    {
                                        byte[] val1 = new byte[2];
                                        val1[0] = 0;
                                        uint val = (uint)int.Parse("0");

                                        bool Success_GP0_Enable = Autokit_Device_1.PL2303_GP0_Enable(Autokit_Device_1.hCOM, 1);
                                        bool Success_GP0_SetValue = Autokit_Device_1.PL2303_GP0_SetValue(Autokit_Device_1.hCOM, val);

                                        bool Success_GP1_Enable = Autokit_Device_1.PL2303_GP1_Enable(Autokit_Device_1.hCOM, 1);
                                        bool Success_GP1_SetValue = Autokit_Device_1.PL2303_GP1_SetValue(Autokit_Device_1.hCOM, val);

                                        Autokit_Device_1.PowerState = false;

                                        //pictureBox_AcPower.Image = Properties.Resources.OFF;
                                    }
                                    ////////////////////////////////////////////////////////////////////////////////////////////////SAVE LOG//////////////////
                                    if (compare_number[i] % compare_num == 0 && Init_Parameter.config_parameter.LogSearch_Savelog == "1")
                                    {
                                        string fName = "";

                                        // 讀取ini中的路徑
                                        fName = Init_Parameter.config_parameter.Record_LogPath;
                                        string t = fName + "\\_SaveLogD_" + DateTime.Now.ToString("yyyyMMddHHmmss") + "_" + Global.label_LoopNumber + ".txt";

                                        StreamWriter MYFILE = new StreamWriter(t, false, Encoding.ASCII);
                                        MYFILE.Write(Extra_Commander.logD_text);
                                        MYFILE.Close();
                                        Extra_Commander.logD_text = string.Empty;
                                    }
                                    ////////////////////////////////////////////////////////////////////////////////////////////////STOP//////////////////
                                    if (compare_number[i] % compare_num == 0 && Init_Parameter.config_parameter.LogSearch_Stop == "1")
                                    {
                                        Autokit_Function_1.Start_Function();
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
                            my_string = my_string + Keyword_SerialPort_D_temp_char;
                        }
                    }
                    else
                    {

                        if ((Keyword_SerialPort_D_temp_char == '\n'))
                        {
                            //textBox3.AppendText(my_string + '\n');
                            my_string = "";
                        }
                        else if ((Keyword_SerialPort_D_temp_char == '\r'))
                        {
                            //textBox3.AppendText(my_string + '\r');
                            my_string = "";
                        }
                        else
                        {
                            my_string = my_string + Keyword_SerialPort_D_temp_char;
                        }
                    }
                }
                Thread.Sleep(500);
            }
        }
        #endregion

        #region -- 關鍵字比對 - serialport_5 --
        public void MyLog5Camd()
        {
            string my_string = "";
            string csvFile = Init_Parameter.config_parameter.Record_LogPath + "\\PortE_keyword.csv";
            int[] compare_number = new int[10];
            bool[] send_status = new bool[10];
            int compare_paremeter = Convert.ToInt32(Init_Parameter.config_parameter.LogSearch_TextNum);

            while (Global.StartButtonPressed == true)
            {
                while (SearchLogQueue_E.Count > 0)
                {
                    Keyword_SerialPort_E_temp_byte = SearchLogQueue_E.Dequeue();
                    Keyword_SerialPort_E_temp_char = (char)Keyword_SerialPort_E_temp_byte;

                    if (Convert.ToInt32(Init_Parameter.config_parameter.LogSearch_PortE) == 1 && Convert.ToInt32(Init_Parameter.config_parameter.LogSearch_TextNum) > 0)
                    {
                        #region \n
                        if ((Keyword_SerialPort_E_temp_char == '\n'))
                        {
                            for (int i = 0; i < compare_paremeter; i++)
                            {
                                string compare_string = ini12.INIRead(MainSettingPath, "LogSearch", "Text" + i, "");
                                int compare_num = Convert.ToInt32(ini12.INIRead(MainSettingPath, "LogSearch", "Times" + i, ""));
                                string[] ewords = my_string.Split(new string[] { compare_string }, StringSplitOptions.None);
                                if (Convert.ToInt32(ewords.Length - 1) >= 1 && my_string.Contains(compare_string) == true)
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
                                        Init_Parameter.config_parameter.LogSearch_Nowvalue = i.ToString();
                                        ini12.INIWrite(MainSettingPath, "LogSearch", "Display" + i, compare_number[i].ToString());
                                        if (Init_Parameter.config_parameter.MailInfo_From != ""
                                            && Init_Parameter.config_parameter.MailInfo_To  != ""
                                            && Init_Parameter.config_parameter.LogSearch_Sendmail == "1")
                                        {
                                            FormMail FormMail = new FormMail();
                                            FormMail.logsend();
                                            send_status[i] = true;
                                        }
                                    }
                                    ////////////////////////////////////////////////////////////////////////////////////////////////AC OFF ON//////////////////
                                    if (compare_number[i] % compare_num == 0
                                        && Init_Parameter.config_parameter.Device_AutoboxExist == "1"
                                        && Init_Parameter.config_parameter.LogSearch_ACcontrol == "1")
                                    {
                                        byte[] val1;
                                        val1 = new byte[2];
                                        val1[0] = 0;

                                        bool jSuccess = Autokit_Device_1.PL2303_GP0_Enable(Autokit_Device_1.hCOM, 1);
                                        if (!jSuccess)
                                        {
                                            Log("GP0 output enable FAILED.");
                                        }
                                        else
                                        {
                                            uint val;
                                            val = (uint)int.Parse("0");
                                            bool bSuccess = Autokit_Device_1.PL2303_GP0_SetValue(Autokit_Device_1.hCOM, val);
                                            if (bSuccess)
                                            {
                                                {
                                                    Autokit_Device_1.PowerState = false;
                                                    //pictureBox_AcPower.Image = Properties.Resources.OFF;
                                                }
                                            }
                                        }

                                        System.Threading.Thread.Sleep(5000);

                                        jSuccess = Autokit_Device_1.PL2303_GP0_Enable(Autokit_Device_1.hCOM, 1);
                                        if (!jSuccess)
                                        {
                                            Log("GP0 output enable FAILED.");
                                        }
                                        else
                                        {
                                            uint val;
                                            val = (uint)int.Parse("1");
                                            bool bSuccess = Autokit_Device_1.PL2303_GP0_SetValue(Autokit_Device_1.hCOM, val);
                                            if (bSuccess)
                                            {
                                                {
                                                    Autokit_Device_1.PowerState = true;
                                                    //pictureBox_AcPower.Image = Properties.Resources.ON;
                                                }
                                            }
                                        }
                                    }
                                    ////////////////////////////////////////////////////////////////////////////////////////////////AC OFF//////////////////
                                    if (compare_number[i] % compare_num == 0
                                        && Init_Parameter.config_parameter.Device_AutoboxExist == "1"
                                        && Init_Parameter.config_parameter.LogSearch_ACOFF == "1")
                                    {
                                        byte[] val1 = new byte[2];
                                        val1[0] = 0;
                                        uint val = (uint)int.Parse("0");

                                        bool Success_GP0_Enable = Autokit_Device_1.PL2303_GP0_Enable(Autokit_Device_1.hCOM, 1);
                                        bool Success_GP0_SetValue = Autokit_Device_1.PL2303_GP0_SetValue(Autokit_Device_1.hCOM, val);

                                        bool Success_GP1_Enable = Autokit_Device_1.PL2303_GP1_Enable(Autokit_Device_1.hCOM, 1);
                                        bool Success_GP1_SetValue = Autokit_Device_1.PL2303_GP1_SetValue(Autokit_Device_1.hCOM, val);

                                        Autokit_Device_1.PowerState = false;

                                        //pictureBox_AcPower.Image = Properties.Resources.OFF;
                                    }
                                    ////////////////////////////////////////////////////////////////////////////////////////////////SAVE LOG//////////////////
                                    if (compare_number[i] % compare_num == 0 && Init_Parameter.config_parameter.LogSearch_Savelog == "1")
                                    {
                                        string fName = "";

                                        // 讀取ini中的路徑
                                        fName = Init_Parameter.config_parameter.Record_LogPath;
                                        string t = fName + "\\_SaveLogE_" + DateTime.Now.ToString("yyyyMMddHHmmss") + "_" + Global.label_LoopNumber + ".txt";

                                        StreamWriter MYFILE = new StreamWriter(t, false, Encoding.ASCII);
                                        MYFILE.Write(Extra_Commander.logE_text);
                                        MYFILE.Close();
                                        Extra_Commander.logE_text = string.Empty;
                                    }
                                    ////////////////////////////////////////////////////////////////////////////////////////////////SCHEDULE//////////////////
                                    if (compare_number[i] % compare_num == 0 && Init_Parameter.config_parameter.LogSearch_Stop == "1")
                                    {
                                        Autokit_Function_1.Start_Function();
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
                        else if ((Keyword_SerialPort_E_temp_char == '\r'))
                        {
                            for (int i = 0; i < compare_paremeter; i++)
                            {
                                string compare_string = ini12.INIRead(MainSettingPath, "LogSearch", "Text" + i, "");
                                int compare_num = Convert.ToInt32(ini12.INIRead(MainSettingPath, "LogSearch", "Times" + i, ""));
                                string[] ewords = my_string.Split(new string[] { compare_string }, StringSplitOptions.None);
                                if (Convert.ToInt32(ewords.Length - 1) >= 1 && my_string.Contains(compare_string) == true)
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
                                        Init_Parameter.config_parameter.LogSearch_Nowvalue = i.ToString();
                                        ini12.INIWrite(MainSettingPath, "LogSearch", "Display" + i, compare_number[i].ToString());
                                        if (Init_Parameter.config_parameter.MailInfo_From != ""
                                            && Init_Parameter.config_parameter.MailInfo_To  != ""
                                            && Init_Parameter.config_parameter.LogSearch_Sendmail == "1")
                                        {
                                            FormMail FormMail = new FormMail();
                                            FormMail.logsend();
                                            send_status[i] = true;
                                        }
                                    }
                                    ////////////////////////////////////////////////////////////////////////////////////////////////AC OFF ON//////////////////
                                    if (compare_number[i] % compare_num == 0
                                        && Init_Parameter.config_parameter.Device_AutoboxExist == "1"
                                        && Init_Parameter.config_parameter.LogSearch_ACcontrol == "1")
                                    {
                                        byte[] val1;
                                        val1 = new byte[2];
                                        val1[0] = 0;

                                        bool jSuccess = Autokit_Device_1.PL2303_GP0_Enable(Autokit_Device_1.hCOM, 1);
                                        if (!jSuccess)
                                        {
                                            Log("GP0 output enable FAILED.");
                                        }
                                        else
                                        {
                                            uint val;
                                            val = (uint)int.Parse("0");
                                            bool bSuccess = Autokit_Device_1.PL2303_GP0_SetValue(Autokit_Device_1.hCOM, val);
                                            if (bSuccess)
                                            {
                                                {
                                                    Autokit_Device_1.PowerState = false;
                                                    //pictureBox_AcPower.Image = Properties.Resources.OFF;
                                                }
                                            }
                                        }

                                        System.Threading.Thread.Sleep(5000);

                                        jSuccess = Autokit_Device_1.PL2303_GP0_Enable(Autokit_Device_1.hCOM, 1);
                                        if (!jSuccess)
                                        {
                                            Log("GP0 output enable FAILED.");
                                        }
                                        else
                                        {
                                            uint val;
                                            val = (uint)int.Parse("1");
                                            bool bSuccess = Autokit_Device_1.PL2303_GP0_SetValue(Autokit_Device_1.hCOM, val);
                                            if (bSuccess)
                                            {
                                                {
                                                    Autokit_Device_1.PowerState = true;
                                                    //pictureBox_AcPower.Image = Properties.Resources.ON;
                                                }
                                            }
                                        }
                                    }
                                    ////////////////////////////////////////////////////////////////////////////////////////////////AC OFF//////////////////
                                    if (compare_number[i] % compare_num == 0
                                        && Init_Parameter.config_parameter.Device_AutoboxExist == "1"
                                        && Init_Parameter.config_parameter.LogSearch_ACOFF == "1")
                                    {
                                        byte[] val1 = new byte[2];
                                        val1[0] = 0;
                                        uint val = (uint)int.Parse("0");

                                        bool Success_GP0_Enable = Autokit_Device_1.PL2303_GP0_Enable(Autokit_Device_1.hCOM, 1);
                                        bool Success_GP0_SetValue = Autokit_Device_1.PL2303_GP0_SetValue(Autokit_Device_1.hCOM, val);

                                        bool Success_GP1_Enable = Autokit_Device_1.PL2303_GP1_Enable(Autokit_Device_1.hCOM, 1);
                                        bool Success_GP1_SetValue = Autokit_Device_1.PL2303_GP1_SetValue(Autokit_Device_1.hCOM, val);

                                        Autokit_Device_1.PowerState = false;

                                        //pictureBox_AcPower.Image = Properties.Resources.OFF;
                                    }
                                    ////////////////////////////////////////////////////////////////////////////////////////////////SAVE LOG//////////////////
                                    if (compare_number[i] % compare_num == 0 && Init_Parameter.config_parameter.LogSearch_Savelog == "1")
                                    {
                                        string fName = "";

                                        // 讀取ini中的路徑
                                        fName = Init_Parameter.config_parameter.Record_LogPath;
                                        string t = fName + "\\_SaveLogE_" + DateTime.Now.ToString("yyyyMMddHHmmss") + "_" + Global.label_LoopNumber + ".txt";

                                        StreamWriter MYFILE = new StreamWriter(t, false, Encoding.ASCII);
                                        MYFILE.Write(Extra_Commander.logE_text);
                                        MYFILE.Close();
                                        Extra_Commander.logE_text = string.Empty;
                                    }
                                    ////////////////////////////////////////////////////////////////////////////////////////////////STOP//////////////////
                                    if (compare_number[i] % compare_num == 0 && Init_Parameter.config_parameter.LogSearch_Stop == "1")
                                    {
                                        Autokit_Function_1.Start_Function();
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
                            my_string = my_string + Keyword_SerialPort_E_temp_char;
                        }
                    }
                    else
                    {

                        if ((Keyword_SerialPort_E_temp_char == '\n'))
                        {
                            //textBox3.AppendText(my_string + '\n');
                            my_string = "";
                        }
                        else if ((Keyword_SerialPort_E_temp_char == '\r'))
                        {
                            //textBox3.AppendText(my_string + '\r');
                            my_string = "";
                        }
                        else
                        {
                            my_string = my_string + Keyword_SerialPort_E_temp_char;
                        }
                    }
                }
                Thread.Sleep(500);
            }
        }
        #endregion
*/
        #region -- Klite error code --
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
                Extra_Commander.kline_text = string.Concat(Extra_Commander.kline_text, textBox_serial);
                DisplayKLineBlockMessage(textBox_serial, "In - " + in_message.GenerateDebugString() + "\n\r");
                Extra_Commander.kline_text = string.Concat(Extra_Commander.kline_text, textBox_serial);
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
                Extra_Commander.kline_text = textBox_serial.Text;
            }
        }

        private void DisplayKLineBlockMessage(System.Windows.Forms.TextBox rtb, String msg)
        {
            String current_time_str = DateTime.Now.ToString("[HH:mm:ss.fff] ");
            rtb.AppendText(current_time_str + msg + "\n");
            rtb.ScrollToCaret();
        }
        #endregion
    }
    #endregion
}
