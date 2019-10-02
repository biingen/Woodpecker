using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using System.Timers;
using System.Windows.Forms;

namespace AutoTest 
{
    class SpiderSerial : IDisposable
    {
        // static member/function to shared aross all SpiderSerial
        static protected Dictionary<string, Object> MySerialDictionary = new Dictionary<string, Object>();
        static Object myserial_serial_obj = new Object();
        // Private member
        private SerialPort _serialPort;
        public Queue<byte> SearchLogQueue = new Queue<byte>();
        public Queue<byte> SaveLogQueue = new Queue<byte>();
        public bool DisplayingHex = false;
        public TextBox LogTextBox;

        public int Serial_BaudRate = 115200;  // BAUD_RATE_LIST.BR_115200;
        public Parity Serial_Parity = Parity.None;
        public int Serial_DataBits = 8;
        public StopBits Serial_StopBits = StopBits.One;

        public SpiderSerial(string com_port) { _serialPort = new SerialPort(com_port, Serial_BaudRate, Serial_Parity, Serial_DataBits, Serial_StopBits); }
        public string GetPortName() { return _serialPort.PortName; }

        public SpiderSerial()
        {
            _serialPort = new SerialPort
            {
                BaudRate = Serial_BaudRate,
                Parity = Serial_Parity,
                DataBits = Serial_DataBits,
                StopBits = Serial_StopBits
            };
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                // dispose managed resources
                _serialPort.Close();
            }
            // free native resources
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        static public List<string> FindAllSerialPort()
        {
            List<string> ListSerialPort = new List<string>();

            foreach (string comport_s in SerialPort.GetPortNames())
            {
                ListSerialPort.Add(comport_s);
            }

            return ListSerialPort;
        }


        public Boolean OpenPort()
        {
            Boolean bRet = false;
            _serialPort.Handshake = Handshake.None;
            _serialPort.Encoding = Encoding.UTF8;
            _serialPort.ReadTimeout = 1000;
            _serialPort.WriteTimeout = 1000;
            _serialPort.DataReceived += new SerialDataReceivedEventHandler(DataReceivedHandler);

            try
            {
                _serialPort.Open();
                Start_SerialReadThread();
                MySerialDictionary.Add(_serialPort.PortName, this);
                bRet = true;
            }
            catch (Exception ex232)
            {
                Console.WriteLine("MySerial_OpenPort Exception at PORT: " + _serialPort.PortName + " - " + ex232);
                bRet = false;
            }
            return bRet;
        }

        public Boolean OpenPort(string PortName)
        {
            Boolean bRet = false;
            _serialPort.PortName = PortName;
            bRet = OpenPort();
            return bRet;
        }

        public Boolean ClosePort()
        {
            Boolean bRet = false;
            MySerialDictionary.Remove(_serialPort.PortName);

            try
            {
                Stop_SerialReadThread();
                _serialPort.Close();
                bRet = true;
            }
            catch (Exception ex232)
            {
                Console.WriteLine("Serial_ClosePort Exception at PORT: " + _serialPort.PortName + " - " + ex232);
                bRet = false;
            }
            return bRet;
        }

        public Boolean IsPortOpened()
        {
            Boolean bRet = false;
            //if ((_serialPort.IsOpen == true) && (readThread.IsAlive))
            if (_serialPort.IsOpen == true)
            {
                bRet = true;
            }
            return bRet;
        }

        //
        // Start of read part
        //

        private Queue<byte> Rx_byte_buffer_QUEUE = new Queue<byte>();
        private Queue<string> UART_READ_MSG_QUEUE = new Queue<string>();
        public Queue<string> LOG_QUEUE = new Queue<string>();

        public byte GetRxByte() { byte ret_byte = Rx_byte_buffer_QUEUE.Dequeue(); return ret_byte; }
        public bool IsRxEmpty() { return (Rx_byte_buffer_QUEUE.Count <= 0) ? true : false; }

        public bool GetBreakState() { return _serialPort.BreakState; }

        private void Start_SerialReadThread()
        {
            LOG_QUEUE.Clear();
            UART_READ_MSG_QUEUE.Clear();
            Rx_byte_buffer_QUEUE.Clear();
        }

        private void Stop_SerialReadThread()
        {
            LOG_QUEUE.Clear();
            UART_READ_MSG_QUEUE.Clear();
            Rx_byte_buffer_QUEUE.Clear();
        }

        // This Handler is for reading all input without wating for a whole line
        private static void DataReceivedHandler(object sender, SerialDataReceivedEventArgs e)
        {
            // Find out which serial port --> which myserial
            SerialPort sp = (SerialPort)sender;
            MySerialDictionary.TryGetValue(sp.PortName, out myserial_serial_obj);
            SpiderSerial myserial = (SpiderSerial)myserial_serial_obj;
            int data_to_read = sp.BytesToRead;

            if (data_to_read > 0)
            {
                byte[] dataset = new byte[data_to_read];
                sp.Read(dataset, 0, data_to_read);

                int index = 0;
                while (data_to_read > 0)
                {
                    myserial.SearchLogQueue.Enqueue(dataset[index]);
                    index++;
                    data_to_read--;
                }

                DateTime dt;
                if (myserial.DisplayingHex == true)
                {
                    // hex to string
                    string hexValues = BitConverter.ToString(dataset).Replace("-", "");
                    DateTime.Now.ToShortTimeString();
                    dt = DateTime.Now;

                    // Joseph
                    hexValues = hexValues.Replace(Environment.NewLine, "\r\n" + "[" + dt.ToString("yyyy/MM/dd HH:mm:ss") + "]  "); //OK
                    // hexValues = String.Concat("[" + dt.ToString("yyyy/MM/dd HH:mm:ss") + "]  " + hexValues + "\r\n");
                    //Form1.PortTextBoxAppend(LogTextBox, hexValues);  //????
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
                    //Form1.PortATextBoxAppend(text);
                }

                //byte[] byteArray = System.Text.Encoding.ASCII.GetBytes(dt.ToString("yyyy/MM/dd HH:mm:ss"));
                //List<byte> dt_list_byte = new List<byte>(byteArray);
                //foreach (byte chr in dt_list_byte)
                //{
                //   log_serial_port1.Enqueue(chr);
                //}
                //data_to_read = index;
                //index = 0;
                //while (data_to_read > 0)
                //{
                //    log_serial_port1.Enqueue(dataset[index]);
                //    index++;
                //    data_to_read--;
                //}

                //string hex = ByteToHexStr(dataset);
                //File.AppendAllText(@"C:\WriteText.txt", text);
                /*
                //serialPort1.DiscardInBuffer();
                //serialPort1.DiscardOutBuffer();
                ///////////////////////////////////////////////先暫時拿掉此功能，因為_save可能會導致程式死當
                string hex = ByteToHexStr(dataset);
                if (DataGridView1.Rows[Global.Scheduler_Row].Cells[0].Value.ToString() == "_SXP")
                {
                    if (hex.Substring(0, 2) == "0E")
                    {
                        textBox1.AppendText("RX: " + hex);
                        Console.WriteLine("1---" + "RX: " + hex);

                        if (hex.Substring(hex.Length - 2) == "A5")
                        {
                            if (hex.Length >= 4 && hex.Substring(hex.Length - 4) == "A5A5")
                            {
                                textBox1.AppendText("\r\n");
                                Console.WriteLine("\r\n");
                                textBox1.AppendText("TX: " + DataGridView1.Rows[Global.Scheduler_Row].Cells[5].Value.ToString() + "\r\n");
                                Console.WriteLine("TX: " + DataGridView1.Rows[Global.Scheduler_Row].Cells[5].Value.ToString() + "\r\n");
                                textBox1.AppendText("\r\n");
                                Console.WriteLine("\r\n");
                            }
                            else
                            {
                                textBox1.AppendText(hex);
                                Console.WriteLine("2---" + hex);
                            }
                        }
                    }
                    else
                    {
                        textBox1.AppendText(hex);
                        Console.WriteLine("3---" + hex);
                        if(hex.Substring(hex.Length - 2) == "A5")
                        if (hex.Length >= 4 && hex.Substring(hex.Length - 4) == "A5A5")
                        {
                            textBox1.AppendText("\r\n");
                            Console.WriteLine("\r\n");
                            textBox1.AppendText("TX: " + DataGridView1.Rows[Global.Scheduler_Row].Cells[5].Value.ToString() + "\r\n");
                            Console.WriteLine("TX: " + DataGridView1.Rows[Global.Scheduler_Row].Cells[5].Value.ToString() + "\r\n");
                            textBox1.AppendText("\r\n");
                            Console.WriteLine("\r\n");
                        }

                        if (hex == "A5")
                        {
                            textBox1.AppendText("\r\n");
                            Console.WriteLine("\r\n");
                            textBox1.AppendText("TX: " + DataGridView1.Rows[Global.Scheduler_Row].Cells[5].Value.ToString() + "\r\n");
                            Console.WriteLine("TX: " + DataGridView1.Rows[Global.Scheduler_Row].Cells[5].Value.ToString() + "\r\n");
                            textBox1.AppendText("\r\n");
                            Console.WriteLine("\r\n");
                        }
                    }
                }
                else
                    textBox1.AppendText(text);
                */
            }


        }



    }
}
