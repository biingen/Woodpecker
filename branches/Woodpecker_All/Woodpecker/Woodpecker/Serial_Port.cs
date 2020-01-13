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
        SerialPort PortA;
        SerialPort PortB;
        SerialPort PortC;
        SerialPort PortD;
        SerialPort PortE;
        private Queue<byte> SearchLogQueue1 = new Queue<byte>();
        private Queue<byte> SearchLogQueue2 = new Queue<byte>();
        private Queue<byte> SearchLogQueue3 = new Queue<byte>();
        private Queue<byte> SearchLogQueue4 = new Queue<byte>();
        private Queue<byte> SearchLogQueue5 = new Queue<byte>();
        private string log1_text, log2_text, log3_text, log4_text, log5_text, kline_text, logAll_text;

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

        public void Serial_Port_Init()
        {
            if (ini12.INIRead(MainSettingPath, "Displayhex", "Checked", "") == "1")
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
        protected SerialErrorCode OpenSerialPort(string Port, SerialPortParemeter param)
        {
            SerialErrorCode return_code = new SerialErrorCode();
            System.Windows.Forms.Timer timer_kline = new System.Windows.Forms.Timer();
            timer_kline.Interval = 250;
            timer_kline.Tick += new System.EventHandler(this.Timer_kline_Tick);

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
                case "kline":
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
                    break;
                default:
                    return_code.status = SerialErrorStatus.undefined_error;
                    break;
            }
            return return_code;
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
                        // string hexValues = BitConverter.ToString(dataset).Replace("-", "");
                        string hexValues = BitConverter.ToString(dataset).Replace("-", "");
                        dt = DateTime.Now;
                        hexValues = "[Receive_Port_A] [" + dt.ToString("yyyy/MM/dd HH:mm:ss.fff") + "]  " + hexValues + "\r\n"; //OK
                        log1_text = string.Concat(log1_text, hexValues);
                    }
                    else
                    {
                        // string strValues = String.Concat(Encoding.ASCII.GetString(dataset).Where(c => c != 0x00));
                        string strValues = Encoding.ASCII.GetString(dataset);
                        dt = DateTime.Now;
                        strValues = "[Receive_Port_A] [" + dt.ToString("yyyy/MM/dd HH:mm:ss.fff") + "]  " + strValues + "\r\n"; //OK
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
                        hexValues = "[Receive_Port_B] [" + dt.ToString("yyyy/MM/dd HH:mm:ss.fff") + "]  " + hexValues + "\r\n"; //OK
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
                        strValues = "[Receive_Port_B] [" + dt.ToString("yyyy/MM/dd HH:mm:ss.fff") + "]  " + strValues + "\r\n"; //OK
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
                int data_to_read = PortC.BytesToRead;
                if (data_to_read > 0)
                {
                    byte[] dataset = new byte[data_to_read];

                    PortC.Read(dataset, 0, data_to_read);
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
                        hexValues = "[Receive_Port_C] [" + dt.ToString("yyyy/MM/dd HH:mm:ss.fff") + "]  " + hexValues + "\r\n"; //OK
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
                        strValues = "[Receive_Port_C] [" + dt.ToString("yyyy/MM/dd HH:mm:ss.fff") + "]  " + strValues + "\r\n"; //OK
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
                int data_to_read = PortD.BytesToRead;
                if (data_to_read > 0)
                {
                    byte[] dataset = new byte[data_to_read];

                    PortD.Read(dataset, 0, data_to_read);
                    int index = 0;
                    while (data_to_read > 0)
                    {
                        SearchLogQueue4.Enqueue(dataset[index]);
                        index++;
                        data_to_read--;
                    }

                    DateTime dt;
                    if (DisplayHexOn_D==true)
                    {
                        // hex to string
                        string hexValues = BitConverter.ToString(dataset).Replace("-", "");
                        DateTime.Now.ToShortTimeString();
                        dt = DateTime.Now;

                        // Joseph
                        hexValues = "[Receive_Port_D] [" + dt.ToString("yyyy/MM/dd HH:mm:ss.fff") + "]  " + hexValues + "\r\n"; //OK
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
                        strValues = "[Receive_Port_D] [" + dt.ToString("yyyy/MM/dd HH:mm:ss.fff") + "]  " + strValues + "\r\n"; //OK
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
                int data_to_read = PortE.BytesToRead;
                if (data_to_read > 0)
                {
                    byte[] dataset = new byte[data_to_read];

                    PortE.Read(dataset, 0, data_to_read);
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
                        hexValues = "[Receive_Port_E] [" + dt.ToString("yyyy/MM/dd HH:mm:ss.fff") + "]  " + hexValues + "\r\n"; //OK
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
                        strValues = "[Receive_Port_E] [" + dt.ToString("yyyy/MM/dd HH:mm:ss.fff") + "]  " + strValues + "\r\n"; //OK
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
                case "CA310":
                    t = fName + "\\_CA310_" + DateTime.Now.ToString("yyyyMMddHHmmss") + "_" + label_LoopNumber_Value.Text + ".txt";
                    MYFILE = new StreamWriter(t, false, Encoding.ASCII);
                    MYFILE.Write(ca310_text);
                    MYFILE.Close();
                    ca310_text = String.Empty;
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
