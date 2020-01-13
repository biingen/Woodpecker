using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using jini;
using DirectX.Capture;
using System.IO;
using System.Timers;
using Microsoft.VisualBasic.FileIO;

namespace Woodpecker
{
    public partial class Extra_Commander : Form
    {
        private Capture capture = null;
        private Filters filters = null;
        private string log1_text, log2_text, log3_text, log4_text, log5_text, ca310_text, canbus_text, kline_text, schedule_text, logAll_text;
        string columns_command, columns_times, columns_interval, columns_comport, columns_function, columns_subFunction, columns_serial, columns_switch, columns_wait, columns_remark;
        private Autokit_Device Autokit_Device_1 = new Autokit_Device();
        private Init_Parameter Config = new Init_Parameter();

        private Extra_Commander(string orginal_data)
        {
            InitializeComponent();
            Config.Environment_initial();
            Readsch(orginal_data);
            string[] columns = orginal_data.Split(',');
            //Schedule All columns list
            Run_command();
        }

        private void Readsch(string ScheduleContent)
        {
            TextFieldParser parser = new TextFieldParser(ScheduleContent);
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
                    columns_command = parts[0].Trim();
                    columns_times = parts[1].Trim();
                    columns_interval = parts[2].Trim();
                    columns_comport = parts[3].Trim();
                    columns_function = parts[4].Trim();
                    columns_subFunction = parts[5].Trim();
                    columns_serial = parts[6].Trim();
                    columns_switch = parts[7].Trim();
                    columns_wait = parts[8].Trim();
                    columns_remark = parts[9].Trim();
                }
                catch (MalformedLineException)
                {
                    MessageBox.Show("Schedule cannot contain double quote ( \" \" ).", "Schedule foramt error");
                }
            }
            parser.Close();
        }

        private void Run_command(string[] columns)
        {

            int sRepeat = 0, stime = 0, SysDelay = 0;

            string GPIO_INPUT = Autokit_Device_1.IO_INPUT();//先讀取IO值，避免schedule第一行放IO CMD會出錯//

            Global.Schedule_Step = Global.Scheduler_Row;
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

            Console.WriteLine("Record Schedule.");
            Schedule_log = columns_command;
            try
            {
                for (int i = 1; i < 10; i++)
                {
                    Schedule_log = Schedule_log + delimiter_recordSch + columns[i].Trim();
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
            logAll_text = string.Concat(logAll_text, sch_log_text);
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
                    if (Config.Device_AutoboxExist == "1")
                    {
                        if (Autokit_Device_1.PL2303_GP0_Enable(Autokit_Device_1.hCOM, 1) == true)
                        {
                            uint val = (uint)int.Parse("1");
                            bool bSuccess = Autokit_Device_1.PL2303_GP0_SetValue(Autokit_Device_1.hCOM, val);
                            if (bSuccess)
                            {
                                {
                                    Autokit_Device_1.PowerState = true;
                                }
                            }
                        }
                        if (Autokit_Device_1.PL2303_GP1_Enable(Autokit_Device_1.hCOM, 1) == true)
                        {
                            uint val = (uint)int.Parse("1");
                            bool bSuccess = Autokit_Device_1.PL2303_GP1_SetValue(Autokit_Device_1.hCOM, val);
                            if (bSuccess)
                            {
                                {
                                    Autokit_Device_1.PowerState = true;
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
                    if (Config.Device_AutoboxExist == "1")
                    {
                        if (Autokit_Device_1.PL2303_GP0_Enable(Autokit_Device_1.hCOM, 1) == true)
                        {
                            uint val = (uint)int.Parse("0");
                            bool bSuccess = Autokit_Device_1.PL2303_GP0_SetValue(Autokit_Device_1.hCOM, val);
                            if (bSuccess)
                            {
                                {
                                    Autokit_Device_1.PowerState = false;
                                    label_Command.Text = "AC OFF";
                                }
                            }
                        }
                        if (Autokit_Device_1.PL2303_GP1_Enable(Autokit_Device_1.hCOM, 1) == true)
                        {
                            uint val = (uint)int.Parse("0");
                            bool bSuccess = Autokit_Device_1.PL2303_GP1_SetValue(Autokit_Device_1.hCOM, val);
                            if (bSuccess)
                            {
                                {
                                    Autokit_Device_1.PowerState = false;
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
                    if (Config.Device_AutoboxExist == "1")
                    {
                        if (Autokit_Device_1.PL2303_GP0_Enable(Autokit_Device_1.hCOM, 1) == true)
                        {
                            uint val = (uint)int.Parse("1");
                            bool bSuccess = Autokit_Device_1.PL2303_GP0_SetValue(Autokit_Device_1.hCOM, val);
                            if (bSuccess)
                            {
                                {
                                    Autokit_Device_1.PowerState = true;
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
                    if (Config.Device_AutoboxExist == "1")
                    {
                        if (Autokit_Device_1.PL2303_GP0_Enable(Autokit_Device_1.hCOM, 1) == true)
                        {
                            uint val = (uint)int.Parse("0");
                            bool bSuccess = Autokit_Device_1.PL2303_GP0_SetValue(Autokit_Device_1.hCOM, val);
                            if (bSuccess)
                            {
                                {
                                    Autokit_Device_1.PowerState = false;
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
                    if (Config.Device_AutoboxExist == "1")
                    {
                        if (Autokit_Device_1.PL2303_GP1_Enable(Autokit_Device_1.hCOM, 1) == true)
                        {
                            uint val = (uint)int.Parse("1");
                            bool bSuccess = Autokit_Device_1.PL2303_GP1_SetValue(Autokit_Device_1.hCOM, val);
                            if (bSuccess)
                            {
                                {
                                    Autokit_Device_1.PowerState = true;
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
                    if (Config.Device_AutoboxExist == "1")
                    {
                        if (Autokit_Device_1.PL2303_GP1_Enable(Autokit_Device_1.hCOM, 1) == true)
                        {
                            uint val = (uint)int.Parse("0");
                            bool bSuccess = Autokit_Device_1.PL2303_GP1_SetValue(Autokit_Device_1.hCOM, val);
                            if (bSuccess)
                            {
                                {
                                    Autokit_Device_1.PowerState = false;
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
                    if (Config.Device_AutoboxExist == "1")
                    {
                        if (Autokit_Device_1.PL2303_GP2_Enable(Autokit_Device_1.hCOM, 1) == true)
                        {
                            uint val = (uint)int.Parse("1");
                            bool bSuccess = Autokit_Device_1.PL2303_GP2_SetValue(Autokit_Device_1.hCOM, val);
                            if (bSuccess == true)
                            {
                                {
                                    Autokit_Device_1.USBState = false;
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
                    if (Config.Device_AutoboxExist == "1")
                    {
                        if (Autokit_Device_1.PL2303_GP2_Enable(Autokit_Device_1.hCOM, 1) == true)
                        {
                            uint val = (uint)int.Parse("0");
                            bool bSuccess = Autokit_Device_1.PL2303_GP2_SetValue(Autokit_Device_1.hCOM, val);
                            if (bSuccess == true)
                            {
                                {
                                    Autokit_Device_1.USBState = true;
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
                    if (Config.Device_AutoboxExist == "1")
                    {
                        if (Autokit_Device_1.PL2303_GP3_Enable(Autokit_Device_1.hCOM, 1) == true)
                        {
                            uint val = (uint)int.Parse("1");
                            bool bSuccess = Autokit_Device_1.PL2303_GP3_SetValue(Autokit_Device_1.hCOM, val);
                            if (bSuccess == true)
                            {
                                {
                                    Autokit_Device_1.USBState = false;
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
                    if (Config.Device_AutoboxExist == "1")
                    {
                        if (Autokit_Device_1.PL2303_GP3_Enable(Autokit_Device_1.hCOM, 1) == true)
                        {
                            uint val = (uint)int.Parse("0");
                            bool bSuccess = Autokit_Device_1.PL2303_GP3_SetValue(Autokit_Device_1.hCOM, val);
                            if (bSuccess == true)
                            {
                                {
                                    Autokit_Device_1.USBState = true;
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
                    MessageBox.Show("Camera is not connected!\r\nPlease go to Settings to reload the device list.", "Connection Error");
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
                    else if (columns_serial != "" || columns_switch != "")
                    {
                        ReplaceNewLine(PortA, columns_serial, columns_switch);
                    }
                    else if (columns_serial == "" && columns_switch == "")
                    {
                        MessageBox.Show("Ascii command is fail, please check the format.");
                    }

                    DateTime dt = DateTime.Now;
                    string text = "[Send_Port_A] [" + dt.ToString("yyyy/MM/dd HH:mm:ss.fff") + "]  " + columns_serial + "\n\r";
                    textBox_serial.AppendText(text);
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
                    else if (columns_serial != "" || columns_switch != "")
                    {
                        ReplaceNewLine(PortB, columns_serial, columns_switch);
                    }
                    else if (columns_serial == "" && columns_switch == "")
                    {
                        MessageBox.Show("Ascii command is fail, please check the format.");
                    }

                    DateTime dt = DateTime.Now;
                    string text = "[Send_Port_B] [" + dt.ToString("yyyy/MM/dd HH:mm:ss.fff") + "]  " + columns_serial + "\r\n";
                    textBox_serial.AppendText(text);
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
                    else if (columns_serial != "" || columns_switch != "")
                    {
                        ReplaceNewLine(PortC, columns_serial, columns_switch);
                    }
                    else if (columns_serial == "" && columns_switch == "")
                    {
                        MessageBox.Show("Ascii command is fail, please check the format.");
                    }

                    DateTime dt = DateTime.Now;
                    string text = "[Send_Port_C] [" + dt.ToString("yyyy/MM/dd HH:mm:ss.fff") + "]  " + columns_serial + "\r\n";
                    textBox_serial.AppendText(text);
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
                    else if (columns_serial != "" || columns_switch != "")
                    {
                        ReplaceNewLine(PortD, columns_serial, columns_switch);
                    }
                    else if (columns_serial == "" && columns_switch == "")
                    {
                        MessageBox.Show("Ascii command is fail, please check the format.");
                    }

                    DateTime dt = DateTime.Now;
                    string text = "[Send_Port_D] [" + dt.ToString("yyyy/MM/dd HH:mm:ss.fff") + "]  " + columns_serial + "\r\n";
                    textBox_serial.AppendText(text);
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
                    else if (columns_serial != "" || columns_switch != "")
                    {
                        ReplaceNewLine(PortE, columns_serial, columns_switch);
                    }
                    else if (columns_serial == "" && columns_switch == "")
                    {
                        MessageBox.Show("Ascii command is fail, please check the format.");
                    }

                    DateTime dt = DateTime.Now;
                    string text = "[Send_Port_E] [" + dt.ToString("yyyy/MM/dd HH:mm:ss.fff") + "]  " + columns_serial + "\r\n";
                    textBox_serial.AppendText(text);
                    log5_text = string.Concat(log5_text, text);
                    logAll_text = string.Concat(logAll_text, text);

                }

                if (columns_comport == "ALL")
                {
                    Console.WriteLine("Ascii Log: _All");
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
                        string text = "[Port_A] [" + dt.ToString("yyyy/MM/dd HH:mm:ss.fff") + "]  " + columns_serial + "\r\n";
                        textBox_serial.AppendText(text);
                        log1_text = string.Concat(log1_text, text);
                        logAll_text = string.Concat(logAll_text, text);
                    }
                    if (ini12.INIRead(MainSettingPath, "Port B", "Checked", "") == "1" && columns_comport == "ALL" && serial_content[1] != "" && switch_content[1] != "")
                    {
                        ReplaceNewLine(PortB, serial_content[1], switch_content[1]);
                        DateTime dt = DateTime.Now;
                        string text = "[Port_B] [" + dt.ToString("yyyy/MM/dd HH:mm:ss.fff") + "]  " + columns_serial + "\r\n";
                        textBox_serial.AppendText(text);
                        log2_text = string.Concat(log2_text, text);
                        logAll_text = string.Concat(logAll_text, text);
                    }
                    if (ini12.INIRead(MainSettingPath, "Port C", "Checked", "") == "1" && columns_comport == "ALL" && serial_content[2] != "" && switch_content[2] != "")
                    {
                        ReplaceNewLine(PortC, serial_content[2], switch_content[2]);
                        DateTime dt = DateTime.Now;
                        string text = "[Port_C] [" + dt.ToString("yyyy/MM/dd HH:mm:ss.fff") + "]  " + columns_serial + "\r\n";
                        textBox_serial.AppendText(text);
                        log3_text = string.Concat(log3_text, text);
                        logAll_text = string.Concat(logAll_text, text);
                    }
                    if (ini12.INIRead(MainSettingPath, "Port D", "Checked", "") == "1" && columns_comport == "ALL" && serial_content[3] != "" && switch_content[3] != "")
                    {
                        ReplaceNewLine(PortD, serial_content[3], switch_content[3]);
                        DateTime dt = DateTime.Now;
                        string text = "[Port_D] [" + dt.ToString("yyyy/MM/dd HH:mm:ss.fff") + "]  " + columns_serial + "\r\n";
                        textBox_serial.AppendText(text);
                        log4_text = string.Concat(log4_text, text);
                        logAll_text = string.Concat(logAll_text, text);
                    }
                    if (ini12.INIRead(MainSettingPath, "Port E", "Checked", "") == "1" && columns_comport == "ALL" && serial_content[4] != "" && switch_content[4] != "")
                    {
                        ReplaceNewLine(PortE, serial_content[4], switch_content[4]);
                        DateTime dt = DateTime.Now;
                        string text = "[Port_E] [" + dt.ToString("yyyy/MM/dd HH:mm:ss.fff") + "]  " + columns_serial + "\r\n";
                        textBox_serial.AppendText(text);
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
                string Outputstring = "";
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
                    string text = "[Send_Port_A] [" + dt.ToString("yyyy/MM/dd HH:mm:ss.fff") + "]  " + Outputstring + "\r\n";
                    textBox_serial.AppendText(text);
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
                    string text = "[Send_Port_B] [" + dt.ToString("yyyy/MM/dd HH:mm:ss.fff") + "]  " + Outputstring + "\r\n";
                    textBox_serial.AppendText(text);
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
                    string text = "[Send_Port_C] [" + dt.ToString("yyyy/MM/dd HH:mm:ss.fff") + "]  " + Outputstring + "\r\n";
                    textBox_serial.AppendText(text);
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
                    string text = "[Send_Port_D] [" + dt.ToString("yyyy/MM/dd HH:mm:ss.fff") + "]  " + Outputstring + "\r\n";
                    textBox_serial.AppendText(text);
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
                    string text = "[Send_Port_E] [" + dt.ToString("yyyy/MM/dd HH:mm:ss.fff") + "]  " + Outputstring + "\r\n";
                    textBox_serial.AppendText(text);
                    log5_text = string.Concat(log5_text, text);
                    logAll_text = string.Concat(logAll_text, text);
                }

                if (columns_comport == "ALL")
                {
                    Console.WriteLine("Hex Log: _All");
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
                        string text = "[Send_Port_A] [" + dt.ToString("yyyy/MM/dd HH:mm:ss.fff") + "]  " + Outputstring + "\r\n";
                        textBox_serial.AppendText(text);
                        log1_text = string.Concat(log1_text, text);
                        logAll_text = string.Concat(logAll_text, text);
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
                        string text = "[Send_Port_B] [" + dt.ToString("yyyy/MM/dd HH:mm:ss.fff") + "]  " + Outputstring + "\r\n";
                        textBox_serial.AppendText(text);
                        log2_text = string.Concat(log2_text, text);
                        logAll_text = string.Concat(logAll_text, text);
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
                        string text = "[Send_Port_C] [" + dt.ToString("yyyy/MM/dd HH:mm:ss.fff") + "]  " + Outputstring + "\r\n";
                        textBox_serial.AppendText(text);
                        log3_text = string.Concat(log3_text, text);
                        logAll_text = string.Concat(logAll_text, text);
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
                        string text = "[Send_Port_D] [" + dt.ToString("yyyy/MM/dd HH:mm:ss.fff") + "]  " + Outputstring + "\r\n";
                        textBox_serial.AppendText(text);
                        log4_text = string.Concat(log4_text, text);
                        logAll_text = string.Concat(logAll_text, text);
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
                        string text = "[Send_Port_E] [" + dt.ToString("yyyy/MM/dd HH:mm:ss.fff") + "]  " + serial_content[4] + "\r\n";
                        textBox_serial.AppendText(text);
                        log5_text = string.Concat(log5_text, text);
                        logAll_text = string.Concat(logAll_text, text);
                    }
                }
                label_Command.Text = "(" + columns_command + ") " + Outputstring;
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

            #region -- I2C Read --
            else if (columns_command == "_TX_I2C_Read")
            {
                if (ini12.INIRead(MainSettingPath, "Port A", "Checked", "") == "1" && columns_comport == "A")
                {
                    Console.WriteLine("I2C Read Log: _TX_I2C_Read_PortA");
                    if (columns_times != "" && columns_function != "")
                    {
                        string orginal_data = columns_times + " " + columns_function + " " + "20";
                        string crc32_data = Crc32.I2C_CRC32(orginal_data);
                        string Outputstring = "79 6D " + columns_times + " 06 " + columns_function + " 20 " + crc32_data;
                        byte[] Outputbytes = new byte[Outputstring.Split(' ').Count()];
                        Outputbytes = HexConverter.StrToByte(Outputstring);
                        PortA.Write(Outputbytes, 0, Outputbytes.Length); //發送數據 Rs232
                        DateTime dt = DateTime.Now;
                        string text = "[Send_Port_A] [" + dt.ToString("yyyy/MM/dd HH:mm:ss.fff") + "]  " + Outputstring + "\r\n";
                        log1_text = string.Concat(log1_text, text);
                        logAll_text = string.Concat(logAll_text, text);
                    }
                }

                if (ini12.INIRead(MainSettingPath, "Port B", "Checked", "") == "1" && columns_comport == "B")
                {
                    Console.WriteLine("I2C Read Log: _TX_I2C_Read_PortB");
                    if (columns_times != "" && columns_function != "")
                    {
                        string orginal_data = columns_times + " " + columns_function + " " + "20";
                        string crc32_data = Crc32.I2C_CRC32(orginal_data);
                        string Outputstring = "79 6D " + columns_times + " 06 " + columns_function + " 20 " + crc32_data;
                        byte[] Outputbytes = new byte[Outputstring.Split(' ').Count()];
                        Outputbytes = HexConverter.StrToByte(Outputstring);
                        PortB.Write(Outputbytes, 0, Outputbytes.Length); //發送數據 Rs232
                        DateTime dt = DateTime.Now;
                        string text = "[Send_Port_B] [" + dt.ToString("yyyy/MM/dd HH:mm:ss.fff") + "]  " + Outputstring + "\r\n";
                        log2_text = string.Concat(log2_text, text);
                        logAll_text = string.Concat(logAll_text, text);
                    }
                }

                if (ini12.INIRead(MainSettingPath, "Port C", "Checked", "") == "1" && columns_comport == "C")
                {
                    Console.WriteLine("I2C Read Log: _TX_I2C_Read_PortC");
                    if (columns_times != "" && columns_function != "")
                    {
                        string orginal_data = columns_times + " " + columns_function + " " + "20";
                        string crc32_data = Crc32.I2C_CRC32(orginal_data);
                        string Outputstring = "79 6D " + columns_times + " 06 " + columns_function + " 20 " + crc32_data;
                        byte[] Outputbytes = new byte[Outputstring.Split(' ').Count()];
                        Outputbytes = HexConverter.StrToByte(Outputstring);
                        PortC.Write(Outputbytes, 0, Outputbytes.Length); //發送數據 Rs232
                        DateTime dt = DateTime.Now;
                        string text = "[Send_Port_C] [" + dt.ToString("yyyy/MM/dd HH:mm:ss.fff") + "]  " + Outputstring + "\r\n";
                        log3_text = string.Concat(log3_text, text);
                        logAll_text = string.Concat(logAll_text, text);
                    }
                }

                if (ini12.INIRead(MainSettingPath, "Port D", "Checked", "") == "1" && columns_comport == "D")
                {
                    Console.WriteLine("I2C Read Log: _TX_I2C_Read_PortD");
                    if (columns_times != "" && columns_function != "")
                    {
                        string orginal_data = columns_times + " " + columns_function + " " + "20";
                        string crc32_data = Crc32.I2C_CRC32(orginal_data);
                        string Outputstring = "79 6D " + columns_times + " 06 " + columns_function + " 20 " + crc32_data;
                        byte[] Outputbytes = new byte[Outputstring.Split(' ').Count()];
                        Outputbytes = HexConverter.StrToByte(Outputstring);
                        PortD.Write(Outputbytes, 0, Outputbytes.Length); //發送數據 Rs232
                        DateTime dt = DateTime.Now;
                        string text = "[Send_Port_D] [" + dt.ToString("yyyy/MM/dd HH:mm:ss.fff") + "]  " + Outputstring + "\r\n";
                        log4_text = string.Concat(log4_text, text);
                        logAll_text = string.Concat(logAll_text, text);
                    }
                }

                if (ini12.INIRead(MainSettingPath, "Port E", "Checked", "") == "1" && columns_comport == "E")
                {
                    Console.WriteLine("I2C Read Log: _TX_I2C_Read_PortE");
                    if (columns_times != "" && columns_function != "")
                    {
                        string orginal_data = columns_times + " " + columns_function + " " + "20";
                        string crc32_data = Crc32.I2C_CRC32(orginal_data);
                        string Outputstring = "79 6D " + columns_times + " 06 " + columns_function + " 20 " + crc32_data;
                        byte[] Outputbytes = new byte[Outputstring.Split(' ').Count()];
                        Outputbytes = HexConverter.StrToByte(Outputstring);
                        PortE.Write(Outputbytes, 0, Outputbytes.Length); //發送數據 Rs232
                        DateTime dt = DateTime.Now;
                        string text = "[Send_Port_E] [" + dt.ToString("yyyy/MM/dd HH:mm:ss.fff") + "]  " + Outputstring + "\r\n";
                        log5_text = string.Concat(log5_text, text);
                        logAll_text = string.Concat(logAll_text, text);
                    }
                }
            }
            #endregion

            #region -- I2C Write --
            else if (columns_command == "_TX_I2C_Write")
            {
                if (ini12.INIRead(MainSettingPath, "Port A", "Checked", "") == "1" && columns_comport == "A")
                {
                    Console.WriteLine("I2C Write Log: _TX_I2C_Write_PortA");
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
                        string text = "[Send_Port_A] [" + dt.ToString("yyyy/MM/dd HH:mm:ss.fff") + "]  " + Outputstring + "\r\n";
                        log1_text = string.Concat(log1_text, text);
                        logAll_text = string.Concat(logAll_text, text);
                    }
                }

                if (ini12.INIRead(MainSettingPath, "Port B", "Checked", "") == "1" && columns_comport == "B")
                {
                    Console.WriteLine("I2C Write Log: _TX_I2C_Write_PortB");
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
                        string text = "[Send_Port_B] [" + dt.ToString("yyyy/MM/dd HH:mm:ss.fff") + "]  " + Outputstring + "\r\n";
                        log2_text = string.Concat(log2_text, text);
                        logAll_text = string.Concat(logAll_text, text);
                    }
                }

                if (ini12.INIRead(MainSettingPath, "Port C", "Checked", "") == "1" && columns_comport == "C")
                {
                    Console.WriteLine("I2C Write Log: _TX_I2C_Write_PortC");
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
                        string text = "[Send_Port_C] [" + dt.ToString("yyyy/MM/dd HH:mm:ss.fff") + "]  " + Outputstring + "\r\n";
                        log3_text = string.Concat(log3_text, text);
                        logAll_text = string.Concat(logAll_text, text);
                    }
                }

                if (ini12.INIRead(MainSettingPath, "Port D", "Checked", "") == "1" && columns_comport == "D")
                {
                    Console.WriteLine("I2C Write Log: _TX_I2C_Write_PortD");
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
                        string text = "[Send_Port_D] [" + dt.ToString("yyyy/MM/dd HH:mm:ss.fff") + "]  " + Outputstring + "\r\n";
                        log4_text = string.Concat(log4_text, text);
                        logAll_text = string.Concat(logAll_text, text);
                    }
                }

                if (ini12.INIRead(MainSettingPath, "Port E", "Checked", "") == "1" && columns_comport == "E")
                {
                    Console.WriteLine("I2C Write Log: _TX_I2C_Write_PortE");
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
                        string text = "[Send_Port_E] [" + dt.ToString("yyyy/MM/dd HH:mm:ss.fff") + "]  " + Outputstring + "\r\n";
                        log5_text = string.Concat(log5_text, text);
                        logAll_text = string.Concat(logAll_text, text);
                    }
                }
            }
            #endregion

            #region -- Canbus Send --
            else if (columns_command == "_Canbus_Send")
            {
                if (ini12.INIRead(MainSettingPath, "Device", "CANbusExist", "") == "1")
                {
                    Console.WriteLine("Canbus Send: _Canbus_Send");
                    if (columns_times != "" && columns_serial != "")
                    {
                        MYCanReader.TransmitData(columns_times, columns_serial);

                        string Outputstring = "ID: 0x";
                        Outputstring += columns_times + " Data: " + columns_serial;
                        DateTime dt = DateTime.Now;
                        string canbus_log_text = "[Send_Canbus] [" + dt.ToString("yyyy/MM/dd HH:mm:ss.fff") + "]  " + Outputstring + "\r\n";
                        canbus_text = string.Concat(canbus_text, canbus_log_text);
                        schedule_text = string.Concat(schedule_text, canbus_log_text);
                    }
                }
                label_Command.Text = "(" + columns_command + ") " + columns_serial;
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
                        Autokit_Device_1.MyBlueRat.Set_IO_Extend_Set_Pin(Convert.ToByte(i), Convert.ToByte(GPIO.Substring(8 - i, 1)));
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
                            textBox_serial.AppendText(text);
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
                            textBox_serial.AppendText(text);
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
                            textBox_serial.AppendText(text);
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
                            textBox_serial.AppendText(text);
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
                            textBox_serial.AppendText(text);
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
                            textBox_serial.AppendText(text);
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
                            textBox_serial.AppendText(text);
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
                            textBox_serial.AppendText(text);
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
                            textBox_serial.AppendText(text);
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
                            textBox_serial.AppendText(text);
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

            #region -- MonkeyTest --
            else if (columns_command == "_MonkeyTest")
            {
                Console.WriteLine("Android control: _MonkeyTest");
                Add_ons MonkeyTest = new Add_ons();
                MonkeyTest.MonkeyTest();
                MonkeyTest.CreateExcelFile();
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
                        Device_Driver1.Autocommand_RedRat("Form1", columns_command, config_parameter.RedRat_DBFile, config_parameter.RedRat_Brands);
                    }
                    else if (Config.Device_AutoboxExist == "1")
                    {
                        //執行小藍鼠指令
                        Device_Driver1.Autocommand_BlueRat("Form1", columns_command, config_parameter.RedRat_DBFile, config_parameter.RedRat_Brands);
                    }
                    else
                    {
                        MessageBox.Show("Please connect AutoBox or RedRat!", "Redrat Open Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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

            RedRatDBViewer_Delay(SysDelay);

            #region -- 足跡模式 --
            //假如足跡模式打開則會append足跡上去
            if (ini12.INIRead(MainSettingPath, "Record", "Footprint Mode", "") == "1" && SysDelay != 0)
            {
                Console.WriteLine("Footprint Mode.");
                //檔案不存在則加入標題
                if (File.Exists(Application.StartupPath + @"\StepRecord.csv") == false)
                {
                    File.AppendAllText(Application.StartupPath + @"\StepRecord.csv", "LOOP,TIME,COMMAND,PB07_Status,PB01_Status,PA15_Status,PA14_Status,PA11_Status,PA10_Status," +
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

        #region -- 拍照 --
        public void Camstart(string MainSettingPath, string Camera_VideoIndex, string Camera_AudioIndex, string Camera_VideoNumber, string Camera_AudioNumber)
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
                if (Camera_VideoIndex == "")
                    scam = 0;
                else
                    scam = int.Parse(Camera_VideoIndex);

                if (Camera_AudioIndex == "")
                    saud = 0;
                else
                    saud = int.Parse(Camera_AudioIndex);

                if (Camera_VideoNumber == "")
                    VideoNum = 0;
                else
                    VideoNum = int.Parse(Camera_VideoNumber);

                if (Camera_AudioNumber == "")
                    AudioNum = 0;
                else
                    AudioNum = int.Parse(Camera_AudioNumber);

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
            }
            catch (NotSupportedException)
            {
                Console.Write("Camera is disconnected unexpectedly!\r\nPlease go to Settings to reload the device list.", "Connection Error");
            };
        }

        #region -- 拍照 --
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
            string fName = ini12.INIRead(MainSettingPath, "Record", "VideoPath", "");
            //string ngFolder = "Schedule" + Global.Schedule_Num + "_NG";

            //圖片印字
            Bitmap newBitmap = CloneBitmap(e);
            newBitmap = CloneBitmap(e);
            pictureBox_save.Image = newBitmap;

            Graphics bitMap_g = Graphics.FromImage(pictureBox_save.Image);//底圖
            Font Font = new Font("Microsoft JhengHei Light", 16, FontStyle.Bold);
            Brush FontColor = new SolidBrush(Color.Red);
            string[] Resolution = ini12.INIRead(MainSettingPath, "Camera", "Resolution", "").Split('*');
            int YPoint = int.Parse(Resolution[1]);

            //照片印上現在步驟//
            if (config_parameter. == "_shot")
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
            pictureBox_save.Image.Save(t);
        }
        #endregion
    }
}
