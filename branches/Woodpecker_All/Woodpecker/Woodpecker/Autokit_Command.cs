using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using KWP_2000;
using System.Text.RegularExpressions;
using System.Diagnostics;

namespace Woodpecker
{
    class Autokit_Command
    {
        private Autokit_Device Autokit_Device_1 = new Autokit_Device();
        private Autokit_Function Autokit_Function_1 = new Autokit_Function();
        private Serial_Port Serial_Device_1 = new Serial_Port();

        public static string columns_command, columns_times, columns_interval, columns_comport, columns_function, columns_subFunction, columns_serial, columns_switch, columns_wait, columns_remark;
        public static string logA_text, logB_text, logC_text, logD_text, logE_text, ca310_text, canbus_text, kline_text, schedule_text, logAll_text;
        string[] parts;

        public void Autokit_Commander(string value)
        {
            Init_Parameter.Config_initial();
            Autokit_Function_1.Device_Load();
            Autokit_Function_1.Start_Function();
            Read_command(value);
            Run_command(columns_command, columns_times, columns_interval, columns_comport, columns_function, 
                columns_subFunction, columns_serial, columns_switch, columns_wait, columns_remark);
        }

        private void Read_command(string CommandContent)
        {
            try
            {
                parts = Regex.Split(CommandContent, ",", RegexOptions.IgnoreCase);
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
            catch (Exception Ex)
            {
                //MessageBox.Show("Schedule cannot contain double quote ( \" \" ).", "Schedule foramt error");
            }
        }

        private void Run_command(string columns_command, string columns_times, string columns_interval, string columns_comport,
            string columns_function, string columns_subFunction, string columns_serial, string columns_switch, string columns_wait,
            string columns_remark)
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
                    Schedule_log = Schedule_log + delimiter_recordSch + parts[i].Trim();
                }
            }
            catch (Exception Ex)
            {
                //MessageBox.Show(Ex.Message.ToString(), "The schedule length incorrect!");
            }

            string sch_log_text = "[Schedule] [" + sch_dt.ToString("yyyy/MM/dd HH:mm:ss.fff") + "]  " + Schedule_log + "\r\n";
            logA_text = string.Concat(logA_text, sch_log_text);
            logB_text = string.Concat(logB_text, sch_log_text);
            logC_text = string.Concat(logC_text, sch_log_text);
            logD_text = string.Concat(logD_text, sch_log_text);
            logE_text = string.Concat(logE_text, sch_log_text);
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
                    if (Init_Parameter.config_parameter.Device_AutoboxExist == "1")
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
                                    Global.label_Command = "AC ON";
                                }
                            }
                        }
                    }
                    else
                    {
                        //MessageBox.Show("Please connect an AutoKit!", "Autobox Open Error", //MessageBoxButtons.OK, //MessageBoxIcon.Error);
                    }
                }
                if (columns_switch == "_off")
                {
                    Console.WriteLine("AC SWITCH OLD: _off");
                    if (Init_Parameter.config_parameter.Device_AutoboxExist == "1")
                    {
                        if (Autokit_Device_1.PL2303_GP0_Enable(Autokit_Device_1.hCOM, 1) == true)
                        {
                            uint val = (uint)int.Parse("0");
                            bool bSuccess = Autokit_Device_1.PL2303_GP0_SetValue(Autokit_Device_1.hCOM, val);
                            if (bSuccess)
                            {
                                {
                                    Autokit_Device_1.PowerState = false;
                                    Global.label_Command = "AC OFF";
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
                                    Global.label_Command = "AC OFF";
                                }
                            }
                        }
                    }
                    else
                    {
                        //MessageBox.Show("Please connect an AutoKit!", "Autobox Open Error", //MessageBoxButtons.OK, //MessageBoxIcon.Error);
                    }
                }
                #endregion

                #region -- AC SWITCH --
                if (columns_switch == "_AC1_ON")
                {
                    Console.WriteLine("AC SWITCH: _AC1_ON");
                    if (Init_Parameter.config_parameter.Device_AutoboxExist == "1")
                    {
                        if (Autokit_Device_1.PL2303_GP0_Enable(Autokit_Device_1.hCOM, 1) == true)
                        {
                            uint val = (uint)int.Parse("1");
                            bool bSuccess = Autokit_Device_1.PL2303_GP0_SetValue(Autokit_Device_1.hCOM, val);
                            if (bSuccess)
                            {
                                {
                                    Autokit_Device_1.PowerState = true;
                                    Global.label_Command = "AC1 => POWER ON";
                                }
                            }
                        }
                    }
                    else
                    {
                        //MessageBox.Show("Please connect an AutoKit!", "Autobox Open Error", //MessageBoxButtons.OK, //MessageBoxIcon.Error);
                    }
                }
                if (columns_switch == "_AC1_OFF")
                {
                    Console.WriteLine("AC SWITCH: _AC1_OFF");
                    if (Init_Parameter.config_parameter.Device_AutoboxExist == "1")
                    {
                        if (Autokit_Device_1.PL2303_GP0_Enable(Autokit_Device_1.hCOM, 1) == true)
                        {
                            uint val = (uint)int.Parse("0");
                            bool bSuccess = Autokit_Device_1.PL2303_GP0_SetValue(Autokit_Device_1.hCOM, val);
                            if (bSuccess)
                            {
                                {
                                    Autokit_Device_1.PowerState = false;
                                    Global.label_Command = "AC1 => POWER OFF";
                                }
                            }
                        }
                    }
                    else
                    {
                        //MessageBox.Show("Please connect an AutoKit!", "Autobox Open Error", //MessageBoxButtons.OK, //MessageBoxIcon.Error);
                    }
                }

                if (columns_switch == "_AC2_ON")
                {
                    Console.WriteLine("AC SWITCH: _AC2_ON");
                    if (Init_Parameter.config_parameter.Device_AutoboxExist == "1")
                    {
                        if (Autokit_Device_1.PL2303_GP1_Enable(Autokit_Device_1.hCOM, 1) == true)
                        {
                            uint val = (uint)int.Parse("1");
                            bool bSuccess = Autokit_Device_1.PL2303_GP1_SetValue(Autokit_Device_1.hCOM, val);
                            if (bSuccess)
                            {
                                {
                                    Autokit_Device_1.PowerState = true;
                                    Global.label_Command = "AC2 => POWER ON";
                                }
                            }
                        }
                    }
                    else
                    {
                        //MessageBox.Show("Please connect an AutoKit!", "Autobox Open Error", //MessageBoxButtons.OK, //MessageBoxIcon.Error);
                    }
                }
                if (columns_switch == "_AC2_OFF")
                {
                    Console.WriteLine("AC SWITCH: _AC2_OFF");
                    if (Init_Parameter.config_parameter.Device_AutoboxExist == "1")
                    {
                        if (Autokit_Device_1.PL2303_GP1_Enable(Autokit_Device_1.hCOM, 1) == true)
                        {
                            uint val = (uint)int.Parse("0");
                            bool bSuccess = Autokit_Device_1.PL2303_GP1_SetValue(Autokit_Device_1.hCOM, val);
                            if (bSuccess)
                            {
                                {
                                    Autokit_Device_1.PowerState = false;
                                    Global.label_Command = "AC2 => POWER OFF";
                                }
                            }
                        }
                    }
                    else
                    {
                        //MessageBox.Show("Please connect an AutoKit!", "Autobox Open Error", //MessageBoxButtons.OK, //MessageBoxIcon.Error);
                    }
                }
                #endregion

                #region -- USB SWITCH --
                if (columns_switch == "_USB1_DUT")
                {
                    Console.WriteLine("USB SWITCH: _USB1_DUT");
                    if (Init_Parameter.config_parameter.Device_AutoboxExist == "1")
                    {
                        if (Autokit_Device_1.PL2303_GP2_Enable(Autokit_Device_1.hCOM, 1) == true)
                        {
                            uint val = (uint)int.Parse("1");
                            bool bSuccess = Autokit_Device_1.PL2303_GP2_SetValue(Autokit_Device_1.hCOM, val);
                            if (bSuccess == true)
                            {
                                {
                                    Autokit_Device_1.USBState = false;
                                    Global.label_Command = "USB1 => DUT";
                                }
                            }
                        }
                    }
                    else
                    {
                        //MessageBox.Show("Please connect an AutoKit!", "Error", //MessageBoxButtons.OK, //MessageBoxIcon.Error);
                    }
                }
                else if (columns_switch == "_USB1_PC")
                {
                    Console.WriteLine("USB SWITCH: _USB1_PC");
                    if (Init_Parameter.config_parameter.Device_AutoboxExist == "1")
                    {
                        if (Autokit_Device_1.PL2303_GP2_Enable(Autokit_Device_1.hCOM, 1) == true)
                        {
                            uint val = (uint)int.Parse("0");
                            bool bSuccess = Autokit_Device_1.PL2303_GP2_SetValue(Autokit_Device_1.hCOM, val);
                            if (bSuccess == true)
                            {
                                {
                                    Autokit_Device_1.USBState = true;
                                    Global.label_Command = "USB1 => PC";
                                }
                            }
                        }
                    }
                    else
                    {
                        //MessageBox.Show("Please connect an AutoKit!", "Error", //MessageBoxButtons.OK, //MessageBoxIcon.Error);
                    }
                }

                if (columns_switch == "_USB2_DUT")
                {
                    Console.WriteLine("USB SWITCH: _USB2_DUT");
                    if (Init_Parameter.config_parameter.Device_AutoboxExist == "1")
                    {
                        if (Autokit_Device_1.PL2303_GP3_Enable(Autokit_Device_1.hCOM, 1) == true)
                        {
                            uint val = (uint)int.Parse("1");
                            bool bSuccess = Autokit_Device_1.PL2303_GP3_SetValue(Autokit_Device_1.hCOM, val);
                            if (bSuccess == true)
                            {
                                {
                                    Autokit_Device_1.USBState = false;
                                    Global.label_Command = "USB2 => DUT";
                                }
                            }
                        }
                    }
                    else
                    {
                        //MessageBox.Show("Please connect an AutoKit!", "Error", //MessageBoxButtons.OK, //MessageBoxIcon.Error);
                    }
                }
                else if (columns_switch == "_USB2_PC")
                {
                    Console.WriteLine("USB SWITCH: _USB2_PC");
                    if (Init_Parameter.config_parameter.Device_AutoboxExist == "1")
                    {
                        if (Autokit_Device_1.PL2303_GP3_Enable(Autokit_Device_1.hCOM, 1) == true)
                        {
                            uint val = (uint)int.Parse("0");
                            bool bSuccess = Autokit_Device_1.PL2303_GP3_SetValue(Autokit_Device_1.hCOM, val);
                            if (bSuccess == true)
                            {
                                {
                                    Autokit_Device_1.USBState = true;
                                    Global.label_Command = "USB2 => PC";
                                }
                            }
                        }
                    }
                    else
                    {
                        //MessageBox.Show("Please connect an AutoKit!", "Error", //MessageBoxButtons.OK, //MessageBoxIcon.Error);
                    }
                }
                #endregion
            }
            #endregion

            #region -- 拍照 --
            else if (columns_command == "_shot")
            {
                Console.WriteLine("Take Picture: _shot");
                if (Init_Parameter.config_parameter.Camera_Exist == "1")
                {
                    Global.caption_Num++;
                    if (Global.Loop_Number == 1)
                        Global.caption_Sum = Global.caption_Num;
                    Autokit_Device_1.Myshot();
                    Global.label_Command = "Take Picture";
                }
                else
                {
                    //MessageBox.Show("Camera is not connected!\r\nPlease go to Settings to reload the device list.", "Connection Error");
                }
            }
            #endregion

            #region -- 錄影 --
            else if (columns_command == "_rec_start")
            {
                Console.WriteLine("Take Record: _rec_start");
                if (Init_Parameter.config_parameter.Camera_Exist == "1")
                {
                    if (Global.VideoRecording == false)
                    {
                        Autokit_Device_1.Savevideo(); // 開新檔
                        Global.VideoRecording = true;
                        Autokit_Device_1.MySrtCamd();
                    }
                    Global.label_Command = "Start Recording";
                }
                else
                {
                    //MessageBox.Show("Camera is not connected", "Camera Open Error", //MessageBoxButtons.OK, //MessageBoxIcon.Error);
                }
            }

            else if (columns_command == "_rec_stop")
            {
                Console.WriteLine("Take Record: _rec_stop");
                if (Init_Parameter.config_parameter.Camera_Exist == "1")
                {
                    if (Global.VideoRecording == true)       //判斷是不是正在錄影
                    {
                        Global.VideoRecording = false;
                        Autokit_Device_1.Mysstop();      //先將先前的關掉
                    }
                    Global.label_Command = "Stop Recording";
                }
                else
                {
                    //MessageBox.Show("Camera is not connected", "Camera Open Error", //MessageBoxButtons.OK, //MessageBoxIcon.Error);
                }
            }
            #endregion

            #region -- Ascii --
            else if (columns_command == "_ascii")
            {
                if (Init_Parameter.config_parameter.PortA_Checked == "1" && columns_comport == "A")
                {
                    Console.WriteLine("Ascii Log: _PortA");
                    if (columns_serial == "_save")
                    {
                        Serial_Device_1.Serialportsave("A"); //存檔rs232
                    }
                    else if (columns_serial == "_clear")
                    {
                        logA_text = string.Empty; //清除logA_text
                    }
                    else if (columns_serial != "" || columns_switch != "")
                    {
                        Serial_Device_1.ReplaceNewLine(Serial_Device_1.PortA, columns_serial, columns_switch);
                    }
                    else if (columns_serial == "" && columns_switch == "")
                    {
                        //MessageBox.Show("Ascii command is fail, please check the format.");
                    }

                    DateTime dt = DateTime.Now;
                    string text = "[Send_Port_A] [" + dt.ToString("yyyy/MM/dd HH:mm:ss.fff") + "]  " + columns_serial + "\n\r";
                    logA_text = string.Concat(logA_text, text);
                    logAll_text = string.Concat(logAll_text, text);

                }

                if (Init_Parameter.config_parameter.PortB_Checked == "1" && columns_comport == "B")
                {
                    Console.WriteLine("Ascii Log: _PortB");
                    if (columns_serial == "_save")
                    {
                        Serial_Device_1.Serialportsave("B"); //存檔rs232
                    }
                    else if (columns_serial == "_clear")
                    {
                        logB_text = string.Empty; //清除logB_text
                    }
                    else if (columns_serial != "" || columns_switch != "")
                    {
                        Serial_Device_1.ReplaceNewLine(Serial_Device_1.PortB, columns_serial, columns_switch);
                    }
                    else if (columns_serial == "" && columns_switch == "")
                    {
                        //MessageBox.Show("Ascii command is fail, please check the format.");
                    }

                    DateTime dt = DateTime.Now;
                    string text = "[Send_Port_B] [" + dt.ToString("yyyy/MM/dd HH:mm:ss.fff") + "]  " + columns_serial + "\r\n";
                    logB_text = string.Concat(logB_text, text);
                    logAll_text = string.Concat(logAll_text, text);

                }

                if (Init_Parameter.config_parameter.PortC_Checked == "1" && columns_comport == "C")
                {
                    Console.WriteLine("Ascii Log: _PortC");
                    if (columns_serial == "_save")
                    {
                        Serial_Device_1.Serialportsave("C"); //存檔rs232
                    }
                    else if (columns_serial == "_clear")
                    {
                        logC_text = string.Empty; //清除logC_text
                    }
                    else if (columns_serial != "" || columns_switch != "")
                    {
                        Serial_Device_1.ReplaceNewLine(Serial_Device_1.PortC, columns_serial, columns_switch);
                    }
                    else if (columns_serial == "" && columns_switch == "")
                    {
                        //MessageBox.Show("Ascii command is fail, please check the format.");
                    }

                    DateTime dt = DateTime.Now;
                    string text = "[Send_Port_C] [" + dt.ToString("yyyy/MM/dd HH:mm:ss.fff") + "]  " + columns_serial + "\r\n";
                    logC_text = string.Concat(logC_text, text);
                    logAll_text = string.Concat(logAll_text, text);

                }

                if (Init_Parameter.config_parameter.PortD_Checked == "1" && columns_comport == "D")
                {
                    Console.WriteLine("Ascii Log: _PortD");
                    if (columns_serial == "_save")
                    {
                        Serial_Device_1.Serialportsave("D"); //存檔rs232
                    }
                    else if (columns_serial == "_clear")
                    {
                        logD_text = string.Empty; //清除logD_text
                    }
                    else if (columns_serial != "" || columns_switch != "")
                    {
                        Serial_Device_1.ReplaceNewLine(Serial_Device_1.PortD, columns_serial, columns_switch);
                    }
                    else if (columns_serial == "" && columns_switch == "")
                    {
                        //MessageBox.Show("Ascii command is fail, please check the format.");
                    }

                    DateTime dt = DateTime.Now;
                    string text = "[Send_Port_D] [" + dt.ToString("yyyy/MM/dd HH:mm:ss.fff") + "]  " + columns_serial + "\r\n";
                    logD_text = string.Concat(logD_text, text);
                    logAll_text = string.Concat(logAll_text, text);

                }

                if (Init_Parameter.config_parameter.PortE_Checked == "1" && columns_comport == "E")
                {
                    Console.WriteLine("Ascii Log: _PortE");
                    if (columns_serial == "_save")
                    {
                        Serial_Device_1.Serialportsave("E"); //存檔rs232
                    }
                    else if (columns_serial == "_clear")
                    {
                        logE_text = string.Empty; //清除logE_text
                    }
                    else if (columns_serial != "" || columns_switch != "")
                    {
                        Serial_Device_1.ReplaceNewLine(Serial_Device_1.PortE, columns_serial, columns_switch);
                    }
                    else if (columns_serial == "" && columns_switch == "")
                    {
                        //MessageBox.Show("Ascii command is fail, please check the format.");
                    }

                    DateTime dt = DateTime.Now;
                    string text = "[Send_Port_E] [" + dt.ToString("yyyy/MM/dd HH:mm:ss.fff") + "]  " + columns_serial + "\r\n";
                    logE_text = string.Concat(logE_text, text);
                    logAll_text = string.Concat(logAll_text, text);

                }

                if (columns_comport == "ALL")
                {
                    Console.WriteLine("Ascii Log: _All");
                    string[] serial_content = columns_serial.Split(new string[] { "|" }, StringSplitOptions.RemoveEmptyEntries);
                    string[] switch_content = columns_switch.Split(new string[] { "|" }, StringSplitOptions.RemoveEmptyEntries);

                    if (columns_serial == "_save")
                    {
                        Serial_Device_1.Serialportsave("All"); //存檔rs232
                    }
                    else if (columns_serial == "_clear")
                    {
                        logAll_text = string.Empty; //清除logAll_text
                    }

                    if (Init_Parameter.config_parameter.PortA_Checked == "1" && columns_comport == "ALL" && serial_content[0] != "" && switch_content[0] != "")
                    {
                        Serial_Device_1.ReplaceNewLine(Serial_Device_1.PortA, serial_content[0], switch_content[0]);
                        DateTime dt = DateTime.Now;
                        string text = "[Port_A] [" + dt.ToString("yyyy/MM/dd HH:mm:ss.fff") + "]  " + columns_serial + "\r\n";
                        logA_text = string.Concat(logA_text, text);
                        logAll_text = string.Concat(logAll_text, text);
                    }
                    if (Init_Parameter.config_parameter.PortB_Checked == "1" && columns_comport == "ALL" && serial_content[1] != "" && switch_content[1] != "")
                    {
                        Serial_Device_1.ReplaceNewLine(Serial_Device_1.PortB, serial_content[1], switch_content[1]);
                        DateTime dt = DateTime.Now;
                        string text = "[Port_B] [" + dt.ToString("yyyy/MM/dd HH:mm:ss.fff") + "]  " + columns_serial + "\r\n";
                        logB_text = string.Concat(logB_text, text);
                        logAll_text = string.Concat(logAll_text, text);
                    }
                    if (Init_Parameter.config_parameter.PortC_Checked == "1" && columns_comport == "ALL" && serial_content[2] != "" && switch_content[2] != "")
                    {
                        Serial_Device_1.ReplaceNewLine(Serial_Device_1.PortC, serial_content[2], switch_content[2]);
                        DateTime dt = DateTime.Now;
                        string text = "[Port_C] [" + dt.ToString("yyyy/MM/dd HH:mm:ss.fff") + "]  " + columns_serial + "\r\n";
                        logC_text = string.Concat(logC_text, text);
                        logAll_text = string.Concat(logAll_text, text);
                    }
                    if (Init_Parameter.config_parameter.PortD_Checked == "1" && columns_comport == "ALL" && serial_content[3] != "" && switch_content[3] != "")
                    {
                        Serial_Device_1.ReplaceNewLine(Serial_Device_1.PortD, serial_content[3], switch_content[3]);
                        DateTime dt = DateTime.Now;
                        string text = "[Port_D] [" + dt.ToString("yyyy/MM/dd HH:mm:ss.fff") + "]  " + columns_serial + "\r\n";
                        logD_text = string.Concat(logD_text, text);
                        logAll_text = string.Concat(logAll_text, text);
                    }
                    if (Init_Parameter.config_parameter.PortE_Checked == "1" && columns_comport == "ALL" && serial_content[4] != "" && switch_content[4] != "")
                    {
                        Serial_Device_1.ReplaceNewLine(Serial_Device_1.PortE, serial_content[4], switch_content[4]);
                        DateTime dt = DateTime.Now;
                        string text = "[Port_E] [" + dt.ToString("yyyy/MM/dd HH:mm:ss.fff") + "]  " + columns_serial + "\r\n";
                        logE_text = string.Concat(logE_text, text);
                        logAll_text = string.Concat(logAll_text, text);
                    }
                }

                Global.label_Command = "(" + columns_command + ") " + columns_serial;
            }
            #endregion

            #region -- Hex --
            else if (columns_command == "_HEX")
            {
                string Outputstring = "";
                if (Init_Parameter.config_parameter.PortA_Checked == "1" && columns_comport == "A")
                {
                    Console.WriteLine("Hex Log: _PortA");
                    if (columns_serial == "_save")
                    {
                        Serial_Device_1.Serialportsave("A"); //存檔rs232
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
                        Serial_Device_1.PortA.Write(Outputbytes, 0, Outputbytes.Length); //發送數據 Rs232 + Crc16
                    }
                    else if (columns_serial != "_save" &&
                             columns_serial != "_clear" &&
                             columns_serial != "" &&
                             columns_function == "")
                    {
                        string hexValues = columns_serial;
                        byte[] Outputbytes = new byte[hexValues.Split(' ').Count()];
                        Outputbytes = HexConverter.StrToByte(hexValues);
                        Serial_Device_1.PortA.Write(Outputbytes, 0, Outputbytes.Length); //發送數據 Rs232
                    }
                    DateTime dt = DateTime.Now;
                    string text = "[Send_Port_A] [" + dt.ToString("yyyy/MM/dd HH:mm:ss.fff") + "]  " + Outputstring + "\r\n";
                    logA_text = string.Concat(logA_text, text);
                    logAll_text = string.Concat(logAll_text, text);
                }

                if (Init_Parameter.config_parameter.PortB_Checked == "1" && columns_comport == "B")
                {
                    Console.WriteLine("Hex Log: _PortB");
                    if (columns_serial == "_save")
                    {
                        Serial_Device_1.Serialportsave("B"); //存檔rs232
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
                        Serial_Device_1.PortB.Write(Outputbytes, 0, Outputbytes.Length); //發送數據 Rs232 + Crc16
                    }
                    else if (columns_serial != "_save" &&
                             columns_serial != "_clear" &&
                             columns_serial != "" &&
                             columns_function == "")
                    {
                        string hexValues = columns_serial;
                        byte[] Outputbytes = new byte[hexValues.Split(' ').Count()];
                        Outputbytes = HexConverter.StrToByte(hexValues);
                        Serial_Device_1.PortB.Write(Outputbytes, 0, Outputbytes.Length); //發送數據 Rs232
                    }
                    DateTime dt = DateTime.Now;
                    string text = "[Send_Port_B] [" + dt.ToString("yyyy/MM/dd HH:mm:ss.fff") + "]  " + Outputstring + "\r\n";
                    logB_text = string.Concat(logB_text, text);
                    logAll_text = string.Concat(logAll_text, text);
                }

                if (Init_Parameter.config_parameter.PortC_Checked == "1" && columns_comport == "C")
                {
                    Console.WriteLine("Hex Log: _PortC");
                    if (columns_serial == "_save")
                    {
                        Serial_Device_1.Serialportsave("C"); //存檔rs232
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
                        Serial_Device_1.PortC.Write(Outputbytes, 0, Outputbytes.Length); //發送數據 Rs232 + Crc16
                    }
                    else if (columns_serial != "_save" &&
                             columns_serial != "_clear" &&
                             columns_serial != "" &&
                             columns_function == "")
                    {
                        string hexValues = columns_serial;
                        byte[] Outputbytes = new byte[hexValues.Split(' ').Count()];
                        Outputbytes = HexConverter.StrToByte(hexValues);
                        Serial_Device_1.PortC.Write(Outputbytes, 0, Outputbytes.Length); //發送數據 Rs232
                    }
                    DateTime dt = DateTime.Now;
                    string text = "[Send_Port_C] [" + dt.ToString("yyyy/MM/dd HH:mm:ss.fff") + "]  " + Outputstring + "\r\n";
                    logC_text = string.Concat(logC_text, text);
                    logAll_text = string.Concat(logAll_text, text);
                }

                if (Init_Parameter.config_parameter.PortD_Checked == "1" && columns_comport == "D")
                {
                    Console.WriteLine("Hex Log: _PortD");
                    if (columns_serial == "_save")
                    {
                        Serial_Device_1.Serialportsave("D"); //存檔rs232
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
                        Serial_Device_1.PortD.Write(Outputbytes, 0, Outputbytes.Length); //發送數據 Rs232 + Crc16
                    }
                    else if (columns_serial != "_save" &&
                             columns_serial != "_clear" &&
                             columns_serial != "" &&
                             columns_function == "")
                    {
                        string hexValues = columns_serial;
                        byte[] Outputbytes = new byte[hexValues.Split(' ').Count()];
                        Outputbytes = HexConverter.StrToByte(hexValues);
                        Serial_Device_1.PortD.Write(Outputbytes, 0, Outputbytes.Length); //發送數據 Rs232
                    }
                    DateTime dt = DateTime.Now;
                    string text = "[Send_Port_D] [" + dt.ToString("yyyy/MM/dd HH:mm:ss.fff") + "]  " + Outputstring + "\r\n";
                    logD_text = string.Concat(logD_text, text);
                    logAll_text = string.Concat(logAll_text, text);
                }

                if (Init_Parameter.config_parameter.PortE_Checked == "1" && columns_comport == "E")
                {
                    Console.WriteLine("Hex Log: _PortE");
                    if (columns_serial == "_save")
                    {
                        Serial_Device_1.Serialportsave("E"); //存檔rs232
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
                        Serial_Device_1.PortE.Write(Outputbytes, 0, Outputbytes.Length); //發送數據 Rs232 + Crc16
                    }
                    else if (columns_serial != "_save" &&
                             columns_serial != "_clear" &&
                             columns_serial != "" &&
                             columns_function == "")
                    {
                        string hexValues = columns_serial;
                        byte[] Outputbytes = new byte[hexValues.Split(' ').Count()];
                        Outputbytes = HexConverter.StrToByte(hexValues);
                        Serial_Device_1.PortE.Write(Outputbytes, 0, Outputbytes.Length); //發送數據 Rs232
                    }
                    DateTime dt = DateTime.Now;
                    string text = "[Send_Port_E] [" + dt.ToString("yyyy/MM/dd HH:mm:ss.fff") + "]  " + Outputstring + "\r\n";
                    logE_text = string.Concat(logE_text, text);
                    logAll_text = string.Concat(logAll_text, text);
                }

                if (columns_comport == "ALL")
                {
                    Console.WriteLine("Hex Log: _All");
                    string[] serial_content = columns_serial.Split(new string[] { "|" }, StringSplitOptions.RemoveEmptyEntries);

                    if (columns_serial == "_save")
                    {
                        Serial_Device_1.Serialportsave("All"); //存檔rs232
                    }
                    else if (columns_serial == "_clear")
                    {
                        logAll_text = string.Empty; //清除logAll_text
                    }

                    if (Init_Parameter.config_parameter.PortA_Checked == "1" && columns_comport == "ALL" && serial_content[0] != "")
                    {
                        string orginal_data = serial_content[0];
                        if (columns_function == "CRC16_Modbus")
                        {
                            string crc16_data = Crc16.PID_CRC16(orginal_data);
                            Outputstring = orginal_data + crc16_data;
                            byte[] Outputbytes = new byte[Outputstring.Split(' ').Count()];
                            Outputbytes = HexConverter.StrToByte(Outputstring);
                            Serial_Device_1.PortA.Write(Outputbytes, 0, Outputbytes.Length); //發送數據 Rs232 + Crc16
                        }
                        else
                        {
                            Outputstring = orginal_data;
                            byte[] Outputbytes = serial_content[0].Split(' ').Select(s => Convert.ToByte(s, 16)).ToArray();
                            Serial_Device_1.PortA.Write(Outputbytes, 0, Outputbytes.Length); //發送數據 Rs232
                        }
                        DateTime dt = DateTime.Now;
                        string text = "[Send_Port_A] [" + dt.ToString("yyyy/MM/dd HH:mm:ss.fff") + "]  " + Outputstring + "\r\n";
                        logA_text = string.Concat(logA_text, text);
                        logAll_text = string.Concat(logAll_text, text);
                    }
                    if (Init_Parameter.config_parameter.PortB_Checked == "1" && columns_comport == "ALL" && serial_content[1] != "")
                    {
                        string orginal_data = serial_content[1];
                        if (columns_function == "CRC16_Modbus")
                        {
                            string crc16_data = Crc16.PID_CRC16(orginal_data);
                            Outputstring = orginal_data + crc16_data;
                            byte[] Outputbytes = new byte[Outputstring.Split(' ').Count()];
                            Outputbytes = HexConverter.StrToByte(Outputstring);
                            Serial_Device_1.PortB.Write(Outputbytes, 0, Outputbytes.Length); //發送數據 Rs232 + Crc16
                        }
                        else
                        {
                            Outputstring = orginal_data;
                            byte[] Outputbytes = serial_content[1].Split(' ').Select(s => Convert.ToByte(s, 16)).ToArray();
                            Serial_Device_1.PortB.Write(Outputbytes, 0, Outputbytes.Length); //發送數據 Rs232
                        }
                        DateTime dt = DateTime.Now;
                        string text = "[Send_Port_B] [" + dt.ToString("yyyy/MM/dd HH:mm:ss.fff") + "]  " + Outputstring + "\r\n";
                        logB_text = string.Concat(logB_text, text);
                        logAll_text = string.Concat(logAll_text, text);
                    }
                    if (Init_Parameter.config_parameter.PortC_Checked == "1" && columns_comport == "ALL" && serial_content[2] != "")
                    {
                        string orginal_data = serial_content[2];
                        if (columns_function == "CRC16_Modbus")
                        {
                            string crc16_data = Crc16.PID_CRC16(orginal_data);
                            Outputstring = orginal_data + crc16_data;
                            byte[] Outputbytes = new byte[Outputstring.Split(' ').Count()];
                            Outputbytes = HexConverter.StrToByte(Outputstring);
                            Serial_Device_1.PortC.Write(Outputbytes, 0, Outputbytes.Length); //發送數據 Rs232 + Crc16
                        }
                        else
                        {
                            Outputstring = orginal_data;
                            byte[] Outputbytes = serial_content[2].Split(' ').Select(s => Convert.ToByte(s, 16)).ToArray();
                            Serial_Device_1.PortC.Write(Outputbytes, 0, Outputbytes.Length); //發送數據 Rs232
                        }
                        DateTime dt = DateTime.Now;
                        string text = "[Send_Port_C] [" + dt.ToString("yyyy/MM/dd HH:mm:ss.fff") + "]  " + Outputstring + "\r\n";
                        logC_text = string.Concat(logC_text, text);
                        logAll_text = string.Concat(logAll_text, text);
                    }
                    if (Init_Parameter.config_parameter.PortD_Checked == "1" && columns_comport == "ALL" && serial_content[3] != "")
                    {
                        string orginal_data = serial_content[3];
                        if (columns_function == "CRC16_Modbus")
                        {
                            string crc16_data = Crc16.PID_CRC16(orginal_data);
                            Outputstring = orginal_data + crc16_data;
                            byte[] Outputbytes = new byte[Outputstring.Split(' ').Count()];
                            Outputbytes = HexConverter.StrToByte(Outputstring);
                            Serial_Device_1.PortD.Write(Outputbytes, 0, Outputbytes.Length); //發送數據 Rs232 + Crc16
                        }
                        else
                        {
                            Outputstring = orginal_data;
                            byte[] Outputbytes = serial_content[3].Split(' ').Select(s => Convert.ToByte(s, 16)).ToArray();
                            Serial_Device_1.PortD.Write(Outputbytes, 0, Outputbytes.Length); //發送數據 Rs232
                        }
                        DateTime dt = DateTime.Now;
                        string text = "[Send_Port_D] [" + dt.ToString("yyyy/MM/dd HH:mm:ss.fff") + "]  " + Outputstring + "\r\n";
                        logD_text = string.Concat(logD_text, text);
                        logAll_text = string.Concat(logAll_text, text);
                    }
                    if (Init_Parameter.config_parameter.PortE_Checked == "1" && columns_comport == "ALL" && serial_content[4] != "")
                    {
                        string orginal_data = serial_content[4];
                        if (columns_function == "CRC16_Modbus")
                        {
                            string crc16_data = Crc16.PID_CRC16(orginal_data);
                            Outputstring = orginal_data + crc16_data;
                            byte[] Outputbytes = new byte[Outputstring.Split(' ').Count()];
                            Outputbytes = HexConverter.StrToByte(Outputstring);
                            Serial_Device_1.PortE.Write(Outputbytes, 0, Outputbytes.Length); //發送數據 Rs232 + Crc16
                        }
                        else
                        {
                            Outputstring = orginal_data;
                            byte[] Outputbytes = serial_content[4].Split(' ').Select(s => Convert.ToByte(s, 16)).ToArray();
                            Serial_Device_1.PortE.Write(Outputbytes, 0, Outputbytes.Length); //發送數據 Rs232
                        }
                        DateTime dt = DateTime.Now;
                        string text = "[Send_Port_E] [" + dt.ToString("yyyy/MM/dd HH:mm:ss.fff") + "]  " + serial_content[4] + "\r\n";
                        logE_text = string.Concat(logE_text, text);
                        logAll_text = string.Concat(logAll_text, text);
                    }
                }
                Global.label_Command = "(" + columns_command + ") " + Outputstring;
            }
            #endregion

            #region -- K-Line --
            else if (columns_command == "_K_ABS")
            {
                Console.WriteLine("K-line control: _K_ABS");
                try
                {
                    // K-lite ABS指令檔案匯入
                    string xmlfile = Init_Parameter.config_parameter.Record_Generator;
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
                                    Serial_Device_1.ABS_error_list.Add(new DTC_Data(abs_code_high, abs_code_low, abs_code_status));
                                }
                            }
                            else
                            {
                                //MessageBox.Show("Content includes other error code", "ABS code Open Error", //MessageBoxButtons.OK, //MessageBoxIcon.Error);
                            }
                        }
                    }
                    else
                    {
                        //MessageBox.Show("DTC code file does not exist", "File Open Error", //MessageBoxButtons.OK, //MessageBoxIcon.Error);
                    }
                    Global.label_Command = "(" + columns_command + ") " + columns_serial;
                }
                catch (Exception Ex)
                {
                    //MessageBox.Show(Ex.Message.ToString(), "Kline_ABS library error!");
                }
            }
            else if (columns_command == "_K_OBD")
            {
                Console.WriteLine("K-line control: _K_OBD");
                try
                {
                    // K-lite OBD指令檔案匯入
                    string xmlfile = Init_Parameter.config_parameter.Record_Generator;
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
                                    Serial_Device_1.OBD_error_list.Add(new DTC_Data(obd_code_high, obd_code_low, obd_code_status));
                                }
                            }
                            else
                            {
                                //MessageBox.Show("Content includes other error code", "OBD code Open Error", //MessageBoxButtons.OK, //MessageBoxIcon.Error);
                            }
                        }
                    }
                    else
                    {
                        //MessageBox.Show("DTC code file does not exist", "File Open Error", //MessageBoxButtons.OK, //MessageBoxIcon.Error);
                    }
                    Global.label_Command = "(" + columns_command + ") " + columns_serial;
                }
                catch (Exception Ex)
                {
                    //MessageBox.Show(Ex.Message.ToString(), "Kline_OBD library error !");
                }
            }
            else if (columns_command == "_K_SEND")
            {
                Serial_Device_1.kline_send = 1;
            }
            else if (columns_command == "_K_CLEAR")
            {
                Serial_Device_1.kline_send = 0;
                Serial_Device_1.ABS_error_list.Clear();
                Serial_Device_1.OBD_error_list.Clear();
            }
            #endregion

            #region -- I2C Read --
            else if (columns_command == "_TX_I2C_Read")
            {
                if (Init_Parameter.config_parameter.PortA_Checked == "1" && columns_comport == "A")
                {
                    Console.WriteLine("I2C Read Log: _TX_I2C_Read_PortA");
                    if (columns_times != "" && columns_function != "")
                    {
                        string orginal_data = columns_times + " " + columns_function + " " + "20";
                        string crc32_data = Crc32.I2C_CRC32(orginal_data);
                        string Outputstring = "79 6D " + columns_times + " 06 " + columns_function + " 20 " + crc32_data;
                        byte[] Outputbytes = new byte[Outputstring.Split(' ').Count()];
                        Outputbytes = HexConverter.StrToByte(Outputstring);
                        Serial_Device_1.PortA.Write(Outputbytes, 0, Outputbytes.Length); //發送數據 Rs232
                        DateTime dt = DateTime.Now;
                        string text = "[Send_Port_A] [" + dt.ToString("yyyy/MM/dd HH:mm:ss.fff") + "]  " + Outputstring + "\r\n";
                        logA_text = string.Concat(logA_text, text);
                        logAll_text = string.Concat(logAll_text, text);
                    }
                }

                if (Init_Parameter.config_parameter.PortB_Checked == "1" && columns_comport == "B")
                {
                    Console.WriteLine("I2C Read Log: _TX_I2C_Read_PortB");
                    if (columns_times != "" && columns_function != "")
                    {
                        string orginal_data = columns_times + " " + columns_function + " " + "20";
                        string crc32_data = Crc32.I2C_CRC32(orginal_data);
                        string Outputstring = "79 6D " + columns_times + " 06 " + columns_function + " 20 " + crc32_data;
                        byte[] Outputbytes = new byte[Outputstring.Split(' ').Count()];
                        Outputbytes = HexConverter.StrToByte(Outputstring);
                        Serial_Device_1.PortB.Write(Outputbytes, 0, Outputbytes.Length); //發送數據 Rs232
                        DateTime dt = DateTime.Now;
                        string text = "[Send_Port_B] [" + dt.ToString("yyyy/MM/dd HH:mm:ss.fff") + "]  " + Outputstring + "\r\n";
                        logB_text = string.Concat(logB_text, text);
                        logAll_text = string.Concat(logAll_text, text);
                    }
                }

                if (Init_Parameter.config_parameter.PortC_Checked == "1" && columns_comport == "C")
                {
                    Console.WriteLine("I2C Read Log: _TX_I2C_Read_PortC");
                    if (columns_times != "" && columns_function != "")
                    {
                        string orginal_data = columns_times + " " + columns_function + " " + "20";
                        string crc32_data = Crc32.I2C_CRC32(orginal_data);
                        string Outputstring = "79 6D " + columns_times + " 06 " + columns_function + " 20 " + crc32_data;
                        byte[] Outputbytes = new byte[Outputstring.Split(' ').Count()];
                        Outputbytes = HexConverter.StrToByte(Outputstring);
                        Serial_Device_1.PortC.Write(Outputbytes, 0, Outputbytes.Length); //發送數據 Rs232
                        DateTime dt = DateTime.Now;
                        string text = "[Send_Port_C] [" + dt.ToString("yyyy/MM/dd HH:mm:ss.fff") + "]  " + Outputstring + "\r\n";
                        logC_text = string.Concat(logC_text, text);
                        logAll_text = string.Concat(logAll_text, text);
                    }
                }

                if (Init_Parameter.config_parameter.PortD_Checked == "1" && columns_comport == "D")
                {
                    Console.WriteLine("I2C Read Log: _TX_I2C_Read_PortD");
                    if (columns_times != "" && columns_function != "")
                    {
                        string orginal_data = columns_times + " " + columns_function + " " + "20";
                        string crc32_data = Crc32.I2C_CRC32(orginal_data);
                        string Outputstring = "79 6D " + columns_times + " 06 " + columns_function + " 20 " + crc32_data;
                        byte[] Outputbytes = new byte[Outputstring.Split(' ').Count()];
                        Outputbytes = HexConverter.StrToByte(Outputstring);
                        Serial_Device_1.PortD.Write(Outputbytes, 0, Outputbytes.Length); //發送數據 Rs232
                        DateTime dt = DateTime.Now;
                        string text = "[Send_Port_D] [" + dt.ToString("yyyy/MM/dd HH:mm:ss.fff") + "]  " + Outputstring + "\r\n";
                        logD_text = string.Concat(logD_text, text);
                        logAll_text = string.Concat(logAll_text, text);
                    }
                }

                if (Init_Parameter.config_parameter.PortE_Checked == "1" && columns_comport == "E")
                {
                    Console.WriteLine("I2C Read Log: _TX_I2C_Read_PortE");
                    if (columns_times != "" && columns_function != "")
                    {
                        string orginal_data = columns_times + " " + columns_function + " " + "20";
                        string crc32_data = Crc32.I2C_CRC32(orginal_data);
                        string Outputstring = "79 6D " + columns_times + " 06 " + columns_function + " 20 " + crc32_data;
                        byte[] Outputbytes = new byte[Outputstring.Split(' ').Count()];
                        Outputbytes = HexConverter.StrToByte(Outputstring);
                        Serial_Device_1.PortE.Write(Outputbytes, 0, Outputbytes.Length); //發送數據 Rs232
                        DateTime dt = DateTime.Now;
                        string text = "[Send_Port_E] [" + dt.ToString("yyyy/MM/dd HH:mm:ss.fff") + "]  " + Outputstring + "\r\n";
                        logE_text = string.Concat(logE_text, text);
                        logAll_text = string.Concat(logAll_text, text);
                    }
                }
            }
            #endregion

            #region -- I2C Write --
            else if (columns_command == "_TX_I2C_Write")
            {
                if (Init_Parameter.config_parameter.PortA_Checked == "1" && columns_comport == "A")
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
                        Serial_Device_1.PortA.Write(Outputbytes, 0, Outputbytes.Length); //發送數據 Rs232
                        DateTime dt = DateTime.Now;
                        string text = "[Send_Port_A] [" + dt.ToString("yyyy/MM/dd HH:mm:ss.fff") + "]  " + Outputstring + "\r\n";
                        logA_text = string.Concat(logA_text, text);
                        logAll_text = string.Concat(logAll_text, text);
                    }
                }

                if (Init_Parameter.config_parameter.PortB_Checked == "1" && columns_comport == "B")
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
                        Serial_Device_1.PortB.Write(Outputbytes, 0, Outputbytes.Length); //發送數據 Rs232
                        DateTime dt = DateTime.Now;
                        string text = "[Send_Port_B] [" + dt.ToString("yyyy/MM/dd HH:mm:ss.fff") + "]  " + Outputstring + "\r\n";
                        logB_text = string.Concat(logB_text, text);
                        logAll_text = string.Concat(logAll_text, text);
                    }
                }

                if (Init_Parameter.config_parameter.PortC_Checked == "1" && columns_comport == "C")
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
                        Serial_Device_1.PortC.Write(Outputbytes, 0, Outputbytes.Length); //發送數據 Rs232
                        DateTime dt = DateTime.Now;
                        string text = "[Send_Port_C] [" + dt.ToString("yyyy/MM/dd HH:mm:ss.fff") + "]  " + Outputstring + "\r\n";
                        logC_text = string.Concat(logC_text, text);
                        logAll_text = string.Concat(logAll_text, text);
                    }
                }

                if (Init_Parameter.config_parameter.PortD_Checked == "1" && columns_comport == "D")
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
                        Serial_Device_1.PortD.Write(Outputbytes, 0, Outputbytes.Length); //發送數據 Rs232
                        DateTime dt = DateTime.Now;
                        string text = "[Send_Port_D] [" + dt.ToString("yyyy/MM/dd HH:mm:ss.fff") + "]  " + Outputstring + "\r\n";
                        logD_text = string.Concat(logD_text, text);
                        logAll_text = string.Concat(logAll_text, text);
                    }
                }

                if (Init_Parameter.config_parameter.PortE_Checked == "1" && columns_comport == "E")
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
                        Serial_Device_1.PortE.Write(Outputbytes, 0, Outputbytes.Length); //發送數據 Rs232
                        DateTime dt = DateTime.Now;
                        string text = "[Send_Port_E] [" + dt.ToString("yyyy/MM/dd HH:mm:ss.fff") + "]  " + Outputstring + "\r\n";
                        logE_text = string.Concat(logE_text, text);
                        logAll_text = string.Concat(logAll_text, text);
                    }
                }
            }
            #endregion

            #region -- Canbus Send --
            else if (columns_command == "_Canbus_Send")
            {
                if (Init_Parameter.config_parameter.Canbus_Exist == "1")
                {
                    Console.WriteLine("Canbus Send: _Canbus_Send");
                    if (columns_times != "" && columns_serial != "")
                    {
                        Autokit_Device_1.MYCanReader.TransmitData(columns_times, columns_serial);

                        string Outputstring = "ID: 0x";
                        Outputstring += columns_times + " Data: " + columns_serial;
                        DateTime dt = DateTime.Now;
                        string canbus_log_text = "[Send_Canbus] [" + dt.ToString("yyyy/MM/dd HH:mm:ss.fff") + "]  " + Outputstring + "\r\n";
                        canbus_text = string.Concat(canbus_text, canbus_log_text);
                        schedule_text = string.Concat(schedule_text, canbus_log_text);
                    }
                }
                Global.label_Command = "(" + columns_command + ") " + columns_serial;
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
                    Serial_Device_1.PortA.Write(startbit, 0, 7);

                    // Astro指令檔案匯入
                    string xmlfile = Init_Parameter.config_parameter.Record_Generator;
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
                                    Serial_Device_1.PortA.Write(timebit, 0, 4);
                                }
                            }
                            else
                            {
                                //MessageBox.Show("Content include other signal", "Astro Signal Open Error", //MessageBoxButtons.OK, //MessageBoxIcon.Error);
                            }
                        }
                    }
                    else
                    {
                        //MessageBox.Show("Signal Generator not exist", "File Open Error", //MessageBoxButtons.OK, //MessageBoxIcon.Error);
                    }

                    byte[] endbit = new byte[3] { 0x2c, 0x31, 0x03 };
                    Serial_Device_1.PortA.Write(endbit, 0, 3);
                    Global.label_Command = "(" + columns_command + ") " + columns_switch;
                }
                catch (Exception Ex)
                {
                    //MessageBox.Show(Ex.Message.ToString(), "Transmit the Astro command fail !");
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
                    string xmlfile = Init_Parameter.config_parameter.Record_Generator;
                    if (System.IO.File.Exists(xmlfile) == true)
                    {
                        var allTiming = XDocument.Load(xmlfile).Root.Element("Generator").Elements("Device");
                        foreach (var generator in allTiming)
                        {
                            if (generator.Attribute("Name").Value == "_quantum")
                            {
                                if (columns_function == generator.Element("Timing").Value)
                                {
                                    Serial_Device_1.PortA.WriteLine(generator.Element("Signal").Value + "\r");
                                    Serial_Device_1.PortA.WriteLine("ALLU" + "\r");
                                }
                            }
                            else
                            {
                                //MessageBox.Show("Content include other signal", "Quantum Signal Open Error", //MessageBoxButtons.OK, //MessageBoxIcon.Error);
                            }
                        }
                    }
                    else
                    {
                        //MessageBox.Show("Signal Generator not exist", "File Open Error", //MessageBoxButtons.OK, //MessageBoxIcon.Error);
                    }

                    switch (columns_subFunction)
                    {
                        case "RGB":
                            // RGB mode
                            Serial_Device_1.PortA.WriteLine("AVST 0" + "\r");
                            Serial_Device_1.PortA.WriteLine("DVST 10" + "\r");
                            Serial_Device_1.PortA.WriteLine("FMTU" + "\r");
                            break;
                        case "YCbCr":
                            // YCbCr mode
                            Serial_Device_1.PortA.WriteLine("AVST 0" + "\r");
                            Serial_Device_1.PortA.WriteLine("DVST 14" + "\r");
                            Serial_Device_1.PortA.WriteLine("FMTU" + "\r");
                            break;
                        case "xvYCC":
                            // xvYCC mode
                            Serial_Device_1.PortA.WriteLine("AVST 0" + "\r");
                            Serial_Device_1.PortA.WriteLine("DVST 17" + "\r");
                            Serial_Device_1.PortA.WriteLine("FMTU" + "\r");
                            break;
                        case "4:4:4":
                            // 4:4:4
                            Serial_Device_1.PortA.WriteLine("DVSM 4" + "\r");
                            Serial_Device_1.PortA.WriteLine("FMTU" + "\r");
                            break;
                        case "4:2:2":
                            // 4:2:2
                            Serial_Device_1.PortA.WriteLine("DVSM 2" + "\r");
                            Serial_Device_1.PortA.WriteLine("FMTU" + "\r");
                            break;
                        case "8bits":
                            // 8bits
                            Serial_Device_1.PortA.WriteLine("NBPC 8" + "\r");
                            Serial_Device_1.PortA.WriteLine("FMTU" + "\r");
                            break;
                        case "10bits":
                            // 10bits
                            Serial_Device_1.PortA.WriteLine("NBPC 10" + "\r");
                            Serial_Device_1.PortA.WriteLine("FMTU" + "\r");
                            break;
                        case "12bits":
                            // 12bits
                            Serial_Device_1.PortA.WriteLine("NBPC 12" + "\r");
                            Serial_Device_1.PortA.WriteLine("FMTU" + "\r");
                            break;
                        default:
                            break;
                    }
                    Global.label_Command = "(" + columns_command + ") " + columns_switch + columns_remark;
                }
                catch (Exception Ex)
                {
                    //MessageBox.Show(Ex.Message.ToString(), "Transmit the Quantum command fail !");
                }
            }
            #endregion

            #region -- Dektec --
            else if (columns_command == "_dektec")
            {
                if (columns_switch == "_start")
                {
                    Console.WriteLine("Dektec control: _start");
                    Autokit_Function_1.StartDtplay();
                    Global.label_Command = "(" + columns_command + ") " + columns_serial;
                }

                if (columns_switch == "_stop")
                {
                    Console.WriteLine("Dektec control: _stop");
                    Autokit_Function_1.CloseDtplay();
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
                    p.StartInfo.WorkingDirectory = Init_Parameter.config_parameter.Device_DOS;
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
                        Global.label_Command = "DOS CMD_" + columns_serial;
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
                Autokit_Device_1.IO_INPUT();
            }

            else if (columns_command == "_IO_Output")
            {
                Console.WriteLine("GPIO control: _IO_Output");
                //string GPIO = "01010101";
                string GPIO = columns_times;
                byte GPIO_B = Convert.ToByte(GPIO, 2);
                Autokit_Device_1.MyBlueRat.Set_GPIO_Output(GPIO_B);
                Global.label_Command = "(" + columns_command + ") " + columns_times;
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
                    //MessageBox.Show("Please check the value equal nine.");
                }
                Global.label_Command = "(" + columns_command + ") " + columns_times;
            }

            else if (columns_command == "_FuelDisplay")
            {
                Console.WriteLine("Extend GPIO control: _FuelDisplay");
                string GPIO = columns_times;
                if (GPIO.Length == 9)
                {
                    for (int i = 0; i < 9; i++)
                    {
                        Autokit_Device_1.MyBlueRat.Set_IO_Extend_Set_Pin(Convert.ToByte(i + 16), Convert.ToByte(GPIO.Substring(8 - i, 1)));
                        Thread.Sleep(50);
                    }
                }
                else
                {
                    //MessageBox.Show("Please check the value equal nine.");
                }
                Global.label_Command = "(" + columns_command + ") " + columns_times;
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
                        Autokit_Device_1.MyBlueRat.Set_MCP42xxx(224);
                    else if (GPIO_B >= -17 && GPIO_B < -12)
                        Autokit_Device_1.MyBlueRat.Set_MCP42xxx(172);
                    else if (GPIO_B >= -12 && GPIO_B < -7)
                        Autokit_Device_1.MyBlueRat.Set_MCP42xxx(130);
                    else if (GPIO_B >= -7 && GPIO_B < -2)
                        Autokit_Device_1.MyBlueRat.Set_MCP42xxx(101);
                    else if (GPIO_B >= -2 && GPIO_B < 3)
                        Autokit_Device_1.MyBlueRat.Set_MCP42xxx(78);
                    else if (GPIO_B >= 3 && GPIO_B < 8)
                        Autokit_Device_1.MyBlueRat.Set_MCP42xxx(61);
                    else if (GPIO_B >= 8 && GPIO_B < 13)
                        Autokit_Device_1.MyBlueRat.Set_MCP42xxx(47);
                    else if (GPIO_B >= 13 && GPIO_B < 18)
                        Autokit_Device_1.MyBlueRat.Set_MCP42xxx(36);
                    else if (GPIO_B >= 18 && GPIO_B < 23)
                        Autokit_Device_1.MyBlueRat.Set_MCP42xxx(29);
                    else if (GPIO_B >= 23 && GPIO_B < 28)
                        Autokit_Device_1.MyBlueRat.Set_MCP42xxx(23);
                    else if (GPIO_B >= 28 && GPIO_B < 33)
                        Autokit_Device_1.MyBlueRat.Set_MCP42xxx(19);
                    else if (GPIO_B >= 33 && GPIO_B < 38)
                        Autokit_Device_1.MyBlueRat.Set_MCP42xxx(15);
                    else if (GPIO_B >= 38 && GPIO_B < 43)
                        Autokit_Device_1.MyBlueRat.Set_MCP42xxx(12);
                    else if (GPIO_B >= 43 && GPIO_B < 48)
                        Autokit_Device_1.MyBlueRat.Set_MCP42xxx(10);
                    else if (GPIO_B >= 48 && GPIO_B <= 50)
                        Autokit_Device_1.MyBlueRat.Set_MCP42xxx(8);
                    Thread.Sleep(50);
                }
                Global.label_Command = "(" + columns_command + ") " + columns_times;
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
                        Global.label_Command = "(Push CMD)" + columns_serial;
                        if (Init_Parameter.config_parameter.PortA_Checked == "1" && columns_comport == "A")
                        {
                            if (columns_serial == "_save")
                            {
                                Serial_Device_1.Serialportsave("A"); //存檔rs232
                            }
                            else if (columns_serial == "_clear")
                            {
                                logA_text = string.Empty; //清除textbox1
                            }
                            else if (columns_serial != "" && columns_switch == @"\r")
                            {
                                Serial_Device_1.PortA.Write(columns_serial + "\r"); //發送數據 Rs232 + \r
                            }
                            else if (columns_serial != "" && columns_switch == @"\n")
                            {
                                Serial_Device_1.PortA.Write(columns_serial + "\n"); //發送數據 Rs232 + \n
                            }
                            else if (columns_serial != "" && columns_switch == @"\n\r")
                            {
                                Serial_Device_1.PortA.Write(columns_serial + "\n\r"); //發送數據 Rs232 + \n\r
                            }
                            else if (columns_serial != "" && columns_switch == @"\r\n")
                            {
                                Serial_Device_1.PortA.Write(columns_serial + "\r\n"); //發送數據 Rs232 + \r\n
                            }
                            else if (columns_serial != "" && columns_switch == "")
                            {
                                Serial_Device_1.PortA.Write(columns_serial); //發送數據 HEX Rs232
                            }
                            DateTime dt = DateTime.Now;
                            string text = "[Port_A] [" + dt.ToString("yyyy/MM/dd HH:mm:ss.fff") + "]  " + columns_serial + "\r\n";
                            logA_text = string.Concat(logA_text, text);
                            logAll_text = string.Concat(logAll_text, text);
                        }
                        else if (Init_Parameter.config_parameter.PortB_Checked == "1" && columns_comport == "B")
                        {
                            if (columns_serial == "_save")
                            {
                                Serial_Device_1.Serialportsave("B"); //存檔rs232
                            }
                            else if (columns_serial == "_clear")
                            {
                                logB_text = string.Empty; //清除logB_text
                            }
                            else if (columns_serial != "" && columns_switch == @"\r")
                            {
                                Serial_Device_1.PortB.Write(columns_serial + "\r"); //發送數據 Rs232 + \r
                            }
                            else if (columns_serial != "" && columns_switch == @"\n")
                            {
                                Serial_Device_1.PortB.Write(columns_serial + "\n"); //發送數據 Rs232 + \n
                            }
                            else if (columns_serial != "" && columns_switch == @"\n\r")
                            {
                                Serial_Device_1.PortB.Write(columns_serial + "\n\r"); //發送數據 Rs232 + \n\r
                            }
                            else if (columns_serial != "" && columns_switch == @"\r\n")
                            {
                                Serial_Device_1.PortB.Write(columns_serial + "\r\n"); //發送數據 Rs232 + \r\n
                            }
                            else if (columns_serial != "" && columns_switch == "")
                            {
                                Serial_Device_1.PortB.Write(columns_serial); //發送數據 HEX Rs232
                            }
                            DateTime dt = DateTime.Now;
                            string text = "[Port_B] [" + dt.ToString("yyyy/MM/dd HH:mm:ss.fff") + "]  " + columns_serial + "\r\n";
                            logB_text = string.Concat(logB_text, text);
                            logAll_text = string.Concat(logAll_text, text);
                        }
                        else if (Init_Parameter.config_parameter.PortC_Checked == "1" && columns_comport == "C")
                        {
                            if (columns_serial == "_save")
                            {
                                Serial_Device_1.Serialportsave("C"); //存檔rs232
                            }
                            else if (columns_serial == "_clear")
                            {
                                logC_text = string.Empty; //清除logC_text
                            }
                            else if (columns_serial != "" && columns_switch == @"\r")
                            {
                                Serial_Device_1.PortC.Write(columns_serial + "\r"); //發送數據 Rs232 + \r
                            }
                            else if (columns_serial != "" && columns_switch == @"\n")
                            {
                                Serial_Device_1.PortC.Write(columns_serial + "\n"); //發送數據 Rs232 + \n
                            }
                            else if (columns_serial != "" && columns_switch == @"\n\r")
                            {
                                Serial_Device_1.PortC.Write(columns_serial + "\n\r"); //發送數據 Rs232 + \n\r
                            }
                            else if (columns_serial != "" && columns_switch == @"\r\n")
                            {
                                Serial_Device_1.PortC.Write(columns_serial + "\r\n"); //發送數據 Rs232 + \r\n
                            }
                            else if (columns_serial != "" && columns_switch == "")
                            {
                                Serial_Device_1.PortC.Write(columns_serial); //發送數據 HEX Rs232
                            }
                            DateTime dt = DateTime.Now;
                            string text = "[Port_C] [" + dt.ToString("yyyy/MM/dd HH:mm:ss.fff") + "]  " + columns_serial + "\r\n";
                            logC_text = string.Concat(logC_text, text);
                            logAll_text = string.Concat(logAll_text, text);
                        }
                        else if (Init_Parameter.config_parameter.PortD_Checked == "1" && columns_comport == "D")
                        {
                            if (columns_serial == "_save")
                            {
                                Serial_Device_1.Serialportsave("D"); //存檔rs232
                            }
                            else if (columns_serial == "_clear")
                            {
                                logD_text = string.Empty; //清除logD_text
                            }
                            else if (columns_serial != "" && columns_switch == @"\r")
                            {
                                Serial_Device_1.PortD.Write(columns_serial + "\r"); //發送數據 Rs232 + \r
                            }
                            else if (columns_serial != "" && columns_switch == @"\n")
                            {
                                Serial_Device_1.PortD.Write(columns_serial + "\n"); //發送數據 Rs232 + \n
                            }
                            else if (columns_serial != "" && columns_switch == @"\n\r")
                            {
                                Serial_Device_1.PortD.Write(columns_serial + "\n\r"); //發送數據 Rs232 + \n\r
                            }
                            else if (columns_serial != "" && columns_switch == @"\r\n")
                            {
                                Serial_Device_1.PortD.Write(columns_serial + "\r\n"); //發送數據 Rs232 + \r\n
                            }
                            else if (columns_serial != "" && columns_switch == "")
                            {
                                Serial_Device_1.PortD.Write(columns_serial); //發送數據 HEX Rs232
                            }
                            DateTime dt = DateTime.Now;
                            string text = "[Port_D] [" + dt.ToString("yyyy/MM/dd HH:mm:ss.fff") + "]  " + columns_serial + "\r\n";
                            logD_text = string.Concat(logD_text, text);
                            logAll_text = string.Concat(logAll_text, text);
                        }
                        else if (Init_Parameter.config_parameter.PortE_Checked == "1" && columns_comport == "E")
                        {
                            if (columns_serial == "_save")
                            {
                                Serial_Device_1.Serialportsave("E"); //存檔rs232
                            }
                            else if (columns_serial == "_clear")
                            {
                                logE_text = string.Empty; //清除logE_text
                            }
                            else if (columns_serial != "" && columns_switch == @"\r")
                            {
                                Serial_Device_1.PortE.Write(columns_serial + "\r"); //發送數據 Rs232 + \r
                            }
                            else if (columns_serial != "" && columns_switch == @"\n")
                            {
                                Serial_Device_1.PortE.Write(columns_serial + "\n"); //發送數據 Rs232 + \n
                            }
                            else if (columns_serial != "" && columns_switch == @"\n\r")
                            {
                                Serial_Device_1.PortE.Write(columns_serial + "\n\r"); //發送數據 Rs232 + \n\r
                            }
                            else if (columns_serial != "" && columns_switch == @"\r\n")
                            {
                                Serial_Device_1.PortE.Write(columns_serial + "\r\n"); //發送數據 Rs232 + \r\n
                            }
                            else if (columns_serial != "" && columns_switch == "")
                            {
                                Serial_Device_1.PortE.Write(columns_serial); //發送數據 HEX Rs232
                            }
                            DateTime dt = DateTime.Now;
                            string text = "[Port_E] [" + dt.ToString("yyyy/MM/dd HH:mm:ss.fff") + "]  " + columns_serial + "\r\n";
                            logE_text = string.Concat(logE_text, text);
                            logAll_text = string.Concat(logAll_text, text);
                        }
                        //Global.label_Command = "(" + columns_command + ") " + columns_serial;
                        Console.WriteLine("Extend GPIO control: _FuncKey Delay:" + sRepeat + " ms");
                        Thread.Sleep(sRepeat);
                        int length = columns_serial.Length;
                        string status = columns_serial.Substring(length - 1, 1);
                        string reverse = "";
                        if (status == "0")
                            reverse = columns_serial.Substring(0, length - 1) + "1";
                        else if (status == "1")
                            reverse = columns_serial.Substring(0, length - 1) + "0";
                        Global.label_Command = "(Release CMD)" + reverse;

                        if (Init_Parameter.config_parameter.PortA_Checked == "1" && columns_comport == "A")
                        {
                            if (reverse == "_save")
                            {
                                Serial_Device_1.Serialportsave("A"); //存檔rs232
                            }
                            else if (reverse == "_clear")
                            {
                                logA_text = string.Empty; //清除textbox1
                            }
                            else if (reverse != "" && columns_switch == @"\r")
                            {
                                Serial_Device_1.PortA.Write(reverse + "\r"); //發送數據 Rs232 + \r
                            }
                            else if (reverse != "" && columns_switch == @"\n")
                            {
                                Serial_Device_1.PortA.Write(reverse + "\n"); //發送數據 Rs232 + \n
                            }
                            else if (reverse != "" && columns_switch == @"\n\r")
                            {
                                Serial_Device_1.PortA.Write(reverse + "\n\r"); //發送數據 Rs232 + \n\r
                            }
                            else if (reverse != "" && columns_switch == @"\r\n")
                            {
                                Serial_Device_1.PortA.Write(reverse + "\r\n"); //發送數據 Rs232 + \r\n
                            }
                            else if (reverse != "" && columns_switch == "")
                            {
                                Serial_Device_1.PortA.Write(reverse); //發送數據 HEX Rs232
                            }
                            DateTime dt = DateTime.Now;
                            string text = "[Port_A] [" + dt.ToString("yyyy/MM/dd HH:mm:ss.fff") + "]  " + reverse + "\r\n";
                            logA_text = string.Concat(logA_text, text);
                            logAll_text = string.Concat(logAll_text, text);
                        }
                        else if (Init_Parameter.config_parameter.PortB_Checked == "1" && columns_comport == "B")
                        {
                            if (reverse == "_save")
                            {
                                Serial_Device_1.Serialportsave("B"); //存檔rs232
                            }
                            else if (reverse == "_clear")
                            {
                                logB_text = string.Empty; //清除logB_text
                            }
                            else if (reverse != "" && columns_switch == @"\r")
                            {
                                Serial_Device_1.PortB.Write(reverse + "\r"); //發送數據 Rs232 + \r
                            }
                            else if (reverse != "" && columns_switch == @"\n")
                            {
                                Serial_Device_1.PortB.Write(reverse + "\n"); //發送數據 Rs232 + \n
                            }
                            else if (reverse != "" && columns_switch == @"\n\r")
                            {
                                Serial_Device_1.PortB.Write(reverse + "\n\r"); //發送數據 Rs232 + \n\r
                            }
                            else if (reverse != "" && columns_switch == @"\r\n")
                            {
                                Serial_Device_1.PortB.Write(reverse + "\r\n"); //發送數據 Rs232 + \r\n
                            }
                            else if (reverse != "" && columns_switch == "")
                            {
                                Serial_Device_1.PortB.Write(reverse); //發送數據 HEX Rs232
                            }
                            DateTime dt = DateTime.Now;
                            string text = "[Port_B] [" + dt.ToString("yyyy/MM/dd HH:mm:ss.fff") + "]  " + reverse + "\r\n";
                            logB_text = string.Concat(logB_text, text);
                            logAll_text = string.Concat(logAll_text, text);
                        }
                        else if (Init_Parameter.config_parameter.PortC_Checked == "1" && columns_comport == "C")
                        {
                            if (reverse == "_save")
                            {
                                Serial_Device_1.Serialportsave("C"); //存檔rs232
                            }
                            else if (reverse == "_clear")
                            {
                                logC_text = string.Empty; //清除logC_text
                            }
                            else if (reverse != "" && columns_switch == @"\r")
                            {
                                Serial_Device_1.PortC.Write(reverse + "\r"); //發送數據 Rs232 + \r
                            }
                            else if (reverse != "" && columns_switch == @"\n")
                            {
                                Serial_Device_1.PortC.Write(reverse + "\n"); //發送數據 Rs232 + \n
                            }
                            else if (reverse != "" && columns_switch == @"\n\r")
                            {
                                Serial_Device_1.PortC.Write(reverse + "\n\r"); //發送數據 Rs232 + \n\r
                            }
                            else if (reverse != "" && columns_switch == @"\r\n")
                            {
                                Serial_Device_1.PortC.Write(reverse + "\r\n"); //發送數據 Rs232 + \r\n
                            }
                            else if (reverse != "" && columns_switch == "")
                            {
                                Serial_Device_1.PortC.Write(reverse); //發送數據 HEX Rs232
                            }
                            DateTime dt = DateTime.Now;
                            string text = "[Port_C] [" + dt.ToString("yyyy/MM/dd HH:mm:ss.fff") + "]  " + reverse + "\r\n";
                            logC_text = string.Concat(logC_text, text);
                            logAll_text = string.Concat(logAll_text, text);
                        }
                        else if (Init_Parameter.config_parameter.PortD_Checked == "1" && columns_comport == "D")
                        {
                            if (reverse == "_save")
                            {
                                Serial_Device_1.Serialportsave("D"); //存檔rs232
                            }
                            else if (reverse == "_clear")
                            {
                                logD_text = string.Empty; //清除logD_text
                            }
                            else if (reverse != "" && columns_switch == @"\r")
                            {
                                Serial_Device_1.PortD.Write(reverse + "\r"); //發送數據 Rs232 + \r
                            }
                            else if (reverse != "" && columns_switch == @"\n")
                            {
                                Serial_Device_1.PortD.Write(reverse + "\n"); //發送數據 Rs232 + \n
                            }
                            else if (reverse != "" && columns_switch == @"\n\r")
                            {
                                Serial_Device_1.PortD.Write(reverse + "\n\r"); //發送數據 Rs232 + \n\r
                            }
                            else if (reverse != "" && columns_switch == @"\r\n")
                            {
                                Serial_Device_1.PortD.Write(reverse + "\r\n"); //發送數據 Rs232 + \r\n
                            }
                            else if (reverse != "" && columns_switch == "")
                            {
                                Serial_Device_1.PortD.Write(reverse); //發送數據 HEX Rs232
                            }
                            DateTime dt = DateTime.Now;
                            string text = "[Port_D] [" + dt.ToString("yyyy/MM/dd HH:mm:ss.fff") + "]  " + reverse + "\r\n";
                            logD_text = string.Concat(logD_text, text);
                            logAll_text = string.Concat(logAll_text, text);
                        }
                        else if (Init_Parameter.config_parameter.PortE_Checked == "1" && columns_comport == "E")
                        {
                            if (reverse == "_save")
                            {
                                Serial_Device_1.Serialportsave("E"); //存檔rs232
                            }
                            else if (reverse == "_clear")
                            {
                                logE_text = string.Empty; //清除logE_text
                            }
                            else if (reverse != "" && columns_switch == @"\r")
                            {
                                Serial_Device_1.PortE.Write(reverse + "\r"); //發送數據 Rs232 + \r
                            }
                            else if (reverse != "" && columns_switch == @"\n")
                            {
                                Serial_Device_1.PortE.Write(reverse + "\n"); //發送數據 Rs232 + \n
                            }
                            else if (reverse != "" && columns_switch == @"\n\r")
                            {
                                Serial_Device_1.PortE.Write(reverse + "\n\r"); //發送數據 Rs232 + \n\r
                            }
                            else if (reverse != "" && columns_switch == @"\r\n")
                            {
                                Serial_Device_1.PortE.Write(reverse + "\r\n"); //發送數據 Rs232 + \r\n
                            }
                            else if (reverse != "" && columns_switch == "")
                            {
                                Serial_Device_1.PortE.Write(reverse); //發送數據 HEX Rs232
                            }
                            DateTime dt = DateTime.Now;
                            string text = "[Port_E] [" + dt.ToString("yyyy/MM/dd HH:mm:ss.fff") + "]  " + reverse + "\r\n";
                            logE_text = string.Concat(logE_text, text);
                            logAll_text = string.Concat(logAll_text, text);
                        }
                        //Global.label_Command = "(" + columns_command + ") " + columns_serial;
                        Thread.Sleep(500);
                    }
                }
                catch (Exception Ex)
                {
                    //MessageBox.Show(Ex.Message.ToString(), "SerialPort setting fail !");
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
                                    Global.label_Command = "IO CMD_ACCUMULATE";
                                }
                                else
                                {
                                    Autokit_Function_1.IO_CMD();
                                }
                            }
                            else if (columns_comport.Substring(6, 1) == "1" &&
                                Global.IO_INPUT.Substring(10, 1) == "1")
                            {
                                if (columns_serial == "_accumulate")
                                {
                                    Global.IO_PA10_1_COUNT++;
                                    Global.label_Command = "IO CMD_ACCUMULATE";
                                }
                                else
                                {
                                    Autokit_Function_1.IO_CMD();
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
                                    Global.label_Command = "IO CMD_ACCUMULATE";
                                }
                                else
                                    Autokit_Function_1.IO_CMD();
                            }
                            else if (columns_comport.Substring(6, 1) == "1" &&
                                Global.IO_INPUT.Substring(8, 1) == "1")
                            {
                                if (columns_serial == "_accumulate")
                                {
                                    Global.IO_PA11_1_COUNT++;
                                    Global.label_Command = "IO CMD_ACCUMULATE";
                                }
                                else
                                    Autokit_Function_1.IO_CMD();
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
                                    Global.label_Command = "IO CMD_ACCUMULATE";
                                }
                                else
                                    Autokit_Function_1.IO_CMD();
                            }
                            else if (columns_comport.Substring(6, 1) == "1" &&
                                Global.IO_INPUT.Substring(6, 1) == "1")
                            {
                                if (columns_serial == "_accumulate")
                                {
                                    Global.IO_PA14_1_COUNT++;
                                    Global.label_Command = "IO CMD_ACCUMULATE";
                                }
                                else
                                    Autokit_Function_1.IO_CMD();
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
                                    Global.label_Command = "IO CMD_ACCUMULATE";
                                }
                                else
                                    Autokit_Function_1.IO_CMD();
                            }
                            else if (columns_comport.Substring(6, 1) == "1" &&
                                Global.IO_INPUT.Substring(4, 1) == "1")
                            {
                                if (columns_serial == "_accumulate")
                                {
                                    Global.IO_PA15_1_COUNT++;
                                    Global.label_Command = "IO CMD_ACCUMULATE";
                                }
                                else
                                    Autokit_Function_1.IO_CMD();
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
                                    Global.label_Command = "IO CMD_ACCUMULATE";
                                }

                                else
                                    Autokit_Function_1.IO_CMD();
                            }
                            else if (columns_comport.Substring(6, 1) == "1" &&
                                Global.IO_INPUT.Substring(2, 1) == "1")
                            {
                                if (columns_serial == "_accumulate")
                                {
                                    Global.IO_PB1_1_COUNT++;
                                    Global.label_Command = "IO CMD_ACCUMULATE";
                                }
                                else
                                    Autokit_Function_1.IO_CMD();
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
                                    Global.label_Command = "IO CMD_ACCUMULATE";
                                }
                                else
                                    Autokit_Function_1.IO_CMD();
                            }
                            else if (columns_comport.Substring(6, 1) == "1" &&
                                Global.IO_INPUT.Substring(0, 1) == "1")
                            {
                                if (columns_serial == "_accumulate")
                                {
                                    Global.IO_PB7_1_COUNT++;
                                    Global.label_Command = "IO CMD_ACCUMULATE";
                                }
                                else
                                    Autokit_Function_1.IO_CMD();
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
                    Autokit_Device_1.MyBlueRat.Set_Input_GPIO_Low_Debounce_Time_PB1(Convert.ToUInt16(columns_interval));
                    Autokit_Device_1.MyBlueRat.Set_Input_GPIO_Low_Debounce_Time_PB7(Convert.ToUInt16(columns_interval));
                    Debounce_Time_PB1 = Autokit_Device_1.MyBlueRat.Set_Input_GPIO_Low_Debounce_Time_PB1(Convert.ToUInt16(columns_interval));
                    Debounce_Time_PB7 = Autokit_Device_1.MyBlueRat.Set_Input_GPIO_Low_Debounce_Time_PB7(Convert.ToUInt16(columns_interval));
                }
                else
                {
                    Autokit_Device_1.MyBlueRat.Set_Input_GPIO_Low_Debounce_Time_PB1();
                    Autokit_Device_1.MyBlueRat.Set_Input_GPIO_Low_Debounce_Time_PB7();
                    Debounce_Time_PB1 = Autokit_Device_1.MyBlueRat.Set_Input_GPIO_Low_Debounce_Time_PB1();
                    Debounce_Time_PB7 = Autokit_Device_1.MyBlueRat.Set_Input_GPIO_Low_Debounce_Time_PB7();
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
                            Autokit_Function_1.KeywordCommand();
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
                            Autokit_Function_1.KeywordCommand();
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
                            Autokit_Function_1.KeywordCommand();
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
                            Autokit_Function_1.KeywordCommand();
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
                            Autokit_Function_1.KeywordCommand();
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
                            Autokit_Function_1.KeywordCommand();
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
                            Autokit_Function_1.KeywordCommand();
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
                            Autokit_Function_1.KeywordCommand();
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
                            Autokit_Function_1.KeywordCommand();
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
                                Autokit_Function_1.KeywordCommand();
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
                    Global.label_Command = columns_command;
                    if (Init_Parameter.config_parameter.RedRat_Exist == "1")
                    {
                        //執行小紅鼠指令
                        Autokit_Device_1.Autocommand_RedRat("Form1", columns_command);
                    }
                    else if (Init_Parameter.config_parameter.Device_AutoboxExist == "1")
                    {
                        //執行小藍鼠指令
                        Autokit_Device_1.Autocommand_BlueRat("Form1", columns_command);
                    }
                    else
                    {
                        //MessageBox.Show("Please connect AutoBox or RedRat!", "Redrat Open Error", //MessageBoxButtons.OK, //MessageBoxIcon.Error);
                    }
                    Autokit_Device_1.RedRatDBViewer_Delay(sRepeat);
                }
            }
            #endregion

            #region -- Remark --
            if (columns_remark != "")
            {
                Global.label_Remark = columns_remark;
                //label_Remark.Text = columns_remark;
            }
            else
            {
                Global.label_Remark = "";
            }
            #endregion

            //Thread MyExportText = new Thread(new ThreadStart(MyExportCamd));
            //MyExportText.Start();
            Console.WriteLine("CloseTime record.");
            Init_Parameter.config_parameter.DataInfo_CloseTime = string.Format("{0:R}", DateTime.Now);

            Autokit_Device_1.RedRatDBViewer_Delay(SysDelay);

            #region -- 足跡模式 --
            //假如足跡模式打開則會append足跡上去
            if (Init_Parameter.config_parameter.Record_FootprintMode == "1" && SysDelay != 0)
            {
                Console.WriteLine("Footprint Mode.");
                //檔案不存在則加入標題
                if (File.Exists(Global.StartupPath + @"\StepRecord.csv") == false)
                {
                    File.AppendAllText(Global.StartupPath + @"\StepRecord.csv", "LOOP,TIME,COMMAND,PB07_Status,PB01_Status,PA15_Status,PA14_Status,PA11_Status,PA10_Status," +
                        "PA10_0,PA10_1," +
                        "PA11_0,PA11_1," +
                        "PA14_0,PA14_1," +
                        "PA15_0,PA15_1," +
                        "PB1_0,PB1_1," +
                        "PB7_0,PB7_1" +
                        Environment.NewLine);

                    File.AppendAllText(Global.StartupPath + @"\StepRecord.csv",
                    Global.Loop_Number + "," + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "," + Global.label_Command + "," + Global.IO_INPUT +
                    "," + Global.IO_PA10_0_COUNT + "," + Global.IO_PA10_1_COUNT +
                    "," + Global.IO_PA11_0_COUNT + "," + Global.IO_PA11_1_COUNT +
                    "," + Global.IO_PA14_0_COUNT + "," + Global.IO_PA14_1_COUNT +
                    "," + Global.IO_PA15_0_COUNT + "," + Global.IO_PA15_1_COUNT +
                    "," + Global.IO_PB1_0_COUNT + "," + Global.IO_PB1_1_COUNT +
                    "," + Global.IO_PB7_0_COUNT + "," + Global.IO_PB7_1_COUNT + Environment.NewLine);
                }
                else
                {
                    File.AppendAllText(Global.StartupPath + @"\StepRecord.csv",
                    Global.Loop_Number + "," + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "," + Global.label_Command + "," + Global.IO_INPUT +
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
    }
}
