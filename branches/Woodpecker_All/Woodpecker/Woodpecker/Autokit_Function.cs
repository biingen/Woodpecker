using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Woodpecker
{
    class Autokit_Function
    {
        private Autokit_Device Autokit_Device_1 = new Autokit_Device();
        private Serial_Port Serial_Port_1 = new Serial_Port();

        #region -- Pre-Load 指令集 --
        public void Device_Load()
        {
            if (Init_Parameter.config_parameter.Device_AutoboxExist == "1")
            {
                if (Init_Parameter.config_parameter.Device_AutoboxVerson == "1")
                {
                    //Autokit_Device_1.ConnectAutoBox1();
                }

                if (Init_Parameter.config_parameter.Device_AutoboxVerson == "2")
                {
                    Autokit_Device_1.ConnectAutoBox2();
                }

                //pictureBox_BlueRat.Image = Properties.Resources.ON;
                Autokit_Device_1.GP0_GP1_AC_ON();
                Autokit_Device_1.GP2_GP3_USB_PC();
            }
            else
            {
                //pictureBox_BlueRat.Image = Properties.Resources.OFF;
                //pictureBox_AcPower.Image = Properties.Resources.OFF;
                //pictureBox_ext_board.Image = Properties.Resources.OFF;
                //button_AcUsb.Enabled = false;
            }

            if (Init_Parameter.config_parameter.RedRat_Exist == "1")
            {
                //Autokit_Device_1.OpenRedRat3();
            }
            else
            {
                //pictureBox_RedRat.Image = Properties.Resources.OFF;
            }

            if (Init_Parameter.config_parameter.Camera_Exist == "1")
            {
                Autokit_Device_1.Camstart();
            }
            else
            {
                //pictureBox_Camera.Image = Properties.Resources.OFF;
            }

            if (Init_Parameter.config_parameter.Device_CA310Exist == "1")
            {
                Autokit_Device_1.ConnectCA310();
                //pictureBox_ca310.Image = Properties.Resources.ON;
            }
            else
            {
                //pictureBox_ca310.Image = Properties.Resources.OFF;
            }

            if (Init_Parameter.config_parameter.Canbus_Exist == "1")
            {
                String can_name;
                List<String> dev_list = Autokit_Device_1.MYCanReader.FindUsbDevice();
                can_name = string.Join(",", dev_list);
                //Init_Parameter.config_parameter.Canbus_DevName = can_name;
                if (Init_Parameter.config_parameter.Canbus_DevIndex == "")
                    Init_Parameter.config_parameter.Canbus_DevIndex = "0";
                if (Init_Parameter.config_parameter.Canbus_BaudRate == "")
                    Init_Parameter.config_parameter.Canbus_BaudRate = "500 Kbps";
                Autokit_Device_1.ConnectCanBus();
                //pictureBox_canbus.Image = Properties.Resources.ON;
            }
            else
            {
                //pictureBox_canbus.Image = Properties.Resources.OFF;
            }
            /*
            if (ini12.INIWrite(MainSettingPath, "Record", "ImportDB", "") == "1")
                button_Analysis.Visible = true;
            else
                button_Analysis.Visible = false;
            */
            /* Hidden serial port.
            if (ini12.INIRead(MainSettingPath, "Port A", "Checked", "") == "1")
            {
                button_SerialPort1.Visible = true;
                // this.myDelegate1 = new AddDataDelegate(AddDataMethod1);
            }
            else
            {
                ini12.INIWrite(MainSettingPath, "Port A", "Checked", "0");
                button_SerialPort1.Visible = false;
            }
            */
            /*
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
            */

            //setStyle();
        }
        #endregion

        #region -- Pause 指令集 --
        //Schedule暫停用的參數
        public bool Pause = false;
        public ManualResetEvent SchedulePause = new ManualResetEvent(true);
        public ManualResetEvent ScheduleWait = new ManualResetEvent(true);

        private void Pause_Function()      //暫停SCHEDULE
        {
            Pause = !Pause;

            if (Pause == true)
            {
                SchedulePause.Reset();
            }
            else
            {
                SchedulePause.Set();
            }
        }
        #endregion

        #region -- Stop 指令集 --
        private void Stop_Function()
        {
            byte[] val = new byte[2];
            val[0] = 0;
            bool AutoBox_Status;
            long delayTime = 0;
            long repeatTime = 0;
            long timeCountUpdated = 0;

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

            AutoBox_Status = Init_Parameter.config_parameter.Device_AutoboxExist == "1" ? true : false;

            if (AutoBox_Status)//如果電腦有接上AutoBox//
            {
                CloseDtplay();//關閉DtPlay//
                /*
                if (ini12.INIRead(MainSettingPath, "Port A", "Checked", "") == "1")
                {
                    if (ini12.INIRead(MainSettingPath, "LogSearch", "TextNum", "") != "0")
                    {
                        LogThread1.Abort();
                        //Log1Data.Abort();
                    }
                }

                if (ini12.INIRead(MainSettingPath, "Port B", "Checked", "") == "1")
                {
                    if (ini12.INIRead(MainSettingPath, "LogSearch", "TextNum", "") != "0")
                    {
                        LogThread2.Abort();
                        //Log2Data.Abort();
                    }
                }

                if (ini12.INIRead(MainSettingPath, "Port C", "Checked", "") == "1")
                {
                    if (ini12.INIRead(MainSettingPath, "LogSearch", "TextNum", "") != "0")
                    {
                        LogThread3.Abort();
                        //Log3Data.Abort();
                    }
                }

                if (ini12.INIRead(MainSettingPath, "Port D", "Checked", "") == "1")
                {
                    if (ini12.INIRead(MainSettingPath, "LogSearch", "TextNum", "") != "0")
                    {
                        LogThread4.Abort();
                        //Log4Data.Abort();
                    }
                }

                if (ini12.INIRead(MainSettingPath, "Port E", "Checked", "") == "1")
                {
                    if (ini12.INIRead(MainSettingPath, "LogSearch", "TextNum", "") != "0")
                    {
                        LogThread5.Abort();
                        //Log5Data.Abort();
                    }
                }
                */
            }
            else//如果沒接AutoBox//
            {
                CloseDtplay();//關閉DtPlay//
                /*
                if (ini12.INIRead(MainSettingPath, "Port A", "Checked", "") == "1")
                {
                    if (ini12.INIRead(MainSettingPath, "LogSearch", "TextNum", "") != "0")
                    {
                        LogThread1.Abort();
                        //Log1Data.Abort();
                    }
                }

                if (ini12.INIRead(MainSettingPath, "Port B", "Checked", "") == "1")
                {
                    if (ini12.INIRead(MainSettingPath, "LogSearch", "TextNum", "") != "0")
                    {
                        LogThread2.Abort();
                        //Log2Data.Abort();
                    }
                }

                if (ini12.INIRead(MainSettingPath, "Port C", "Checked", "") == "1")
                {
                    if (ini12.INIRead(MainSettingPath, "LogSearch", "TextNum", "") != "0")
                    {
                        LogThread3.Abort();
                        //Log3Data.Abort();
                    }
                }

                if (ini12.INIRead(MainSettingPath, "Port D", "Checked", "") == "1")
                {
                    if (ini12.INIRead(MainSettingPath, "LogSearch", "TextNum", "") != "0")
                    {
                        LogThread4.Abort();
                        //Log4Data.Abort();
                    }
                }

                if (ini12.INIRead(MainSettingPath, "Port E", "Checked", "") == "1")
                {
                    if (ini12.INIRead(MainSettingPath, "LogSearch", "TextNum", "") != "0")
                    {
                        LogThread5.Abort();
                        //Log5Data.Abort();
                    }
                }
                */
            }
        }
        #endregion

        #region -- Dektec Play 指令集 --
        public void StartDtplay()
        {
            string StreamName = Autokit_Command.columns_serial;
            string TvSystem = Autokit_Command.columns_function;
            string Freq = Autokit_Command.columns_subFunction;
            string arguments = Global.StartupPath + @"\\DektecPlayer\\" + StreamName + " " +
                               "-mt " + TvSystem + " " +
                               "-mf " + Freq + " " +
                               "-r 0 " +
                               "-l 0";

            Console.WriteLine(arguments);
            System.Diagnostics.Process Dektec = new System.Diagnostics.Process();
            Dektec.StartInfo.FileName = Global.StartupPath + @"\\DektecPlayer\\DtPlay.exe";
            Dektec.StartInfo.UseShellExecute = false;
            Dektec.StartInfo.RedirectStandardInput = true;
            Dektec.StartInfo.RedirectStandardOutput = true;
            Dektec.StartInfo.RedirectStandardError = true;
            Dektec.StartInfo.CreateNoWindow = true;

            Dektec.StartInfo.Arguments = arguments;
            Dektec.Start();
        }

        //關閉DtPlay
        public void CloseDtplay()
        {
            Process[] processes = Process.GetProcessesByName("DtPlay");

            foreach (Process p in processes)
            {
                p.Kill();
            }
        }
        #endregion

        #region -- IO CMD 指令集 --
        public void IO_CMD()
        {
            string columns_serial = Autokit_Command.columns_serial;
            if (columns_serial == "_pause")
            {
                Pause_Function();
                Global.label_Command = "IO CMD_PAUSE";
            }
            else if (columns_serial == "_stop")
            {
                Stop_Function();
                Global.label_Command = "IO CMD_STOP";
            }
            else if (columns_serial == "_ac_restart")
            {
                Autokit_Device_1.GP0_GP1_AC_OFF_ON();
                Thread.Sleep(10);
                Autokit_Device_1.GP0_GP1_AC_OFF_ON();
                Global.label_Command = "IO CMD_AC_RESTART";
            }
            else if (columns_serial == "_shot")
            {
                Global.caption_Num++;
                if (Global.Loop_Number == 1)
                    Global.caption_Sum = Global.caption_Num;
                Autokit_Device_1.Myshot();
                Global.label_Command = "IO CMD_SHOT";
            }
            else if (columns_serial == "_mail")
            {
                if (Init_Parameter.config_parameter.SendMail_Exist == "1")
                {
                    Global.Pass_Or_Fail = "NG";
                    FormMail FormMail = new FormMail();
                    FormMail.send();
                    Global.label_Command = "IO CMD_MAIL";
                }
                else
                {
                    //MessageBox.Show("Please enable Mail Function in Settings.");
                }
            }
            else if (columns_serial.Substring(0, 3) == "_rc")
            {
                String rc_key = columns_serial;
                int startIndex = 4;
                int length = rc_key.Length - 4;
                String rc_key_substring = rc_key.Substring(startIndex, length);

                if (Init_Parameter.config_parameter.RedRat_Exist == "1")
                {
                    Autokit_Device_1.Autocommand_RedRat("Form1", rc_key_substring);
                    Global.label_Command = rc_key_substring;
                }
                else if (Init_Parameter.config_parameter.Device_AutoboxExist == "1")
                {
                    Autokit_Device_1.Autocommand_BlueRat("Form1", rc_key_substring);
                    Global.label_Command = rc_key_substring;
                }
            }
            else if (columns_serial.Substring(0, 7) == "_logcmd")
            {
                String log_cmd = columns_serial;
                int startIndex = 10;
                int length = log_cmd.Length - 10;
                String log_cmd_substring = log_cmd.Substring(startIndex, length);
                String log_cmd_serialport = log_cmd.Substring(8, 1);

                if (Init_Parameter.config_parameter.PortA_Checked == "1" && log_cmd_serialport == "A")
                {
                    Serial_Port_1.PortA.WriteLine(log_cmd_substring);
                }
                else if (Init_Parameter.config_parameter.PortB_Checked == "1" && log_cmd_serialport == "B")
                {
                    Serial_Port_1.PortB.WriteLine(log_cmd_substring);
                }
                else if (Init_Parameter.config_parameter.PortC_Checked == "1" && log_cmd_serialport == "C")
                {
                    Serial_Port_1.PortC.WriteLine(log_cmd_substring);
                }
                else if (Init_Parameter.config_parameter.PortD_Checked == "1" && log_cmd_serialport == "D")
                {
                    Serial_Port_1.PortD.WriteLine(log_cmd_substring);
                }
                else if (Init_Parameter.config_parameter.PortE_Checked == "1" && log_cmd_serialport == "E")
                {
                    Serial_Port_1.PortE.WriteLine(log_cmd_substring);
                }
                else if (log_cmd_serialport == "O")
                {
                    if (Init_Parameter.config_parameter.PortA_Checked == "1")
                        Serial_Port_1.PortA.WriteLine(log_cmd_substring);
                    if (Init_Parameter.config_parameter.PortB_Checked == "1")
                        Serial_Port_1.PortB.WriteLine(log_cmd_substring);
                    if (Init_Parameter.config_parameter.PortC_Checked == "1")
                        Serial_Port_1.PortC.WriteLine(log_cmd_substring);
                    if (Init_Parameter.config_parameter.PortD_Checked == "1")
                        Serial_Port_1.PortD.WriteLine(log_cmd_substring);
                    if (Init_Parameter.config_parameter.PortE_Checked == "1")
                        Serial_Port_1.PortE.WriteLine(log_cmd_substring);
                }
            }
        }
        #endregion

        #region -- KEYWORD 指令集 --
        public void KeywordCommand()
        {
            string columns_serial = Autokit_Command.columns_serial;
            if (columns_serial == "_pause")
            {
                Pause_Function();
                Global.label_Command = "KEYWORD_PAUSE";
            }
            else if (columns_serial == "_stop")
            {
                Stop_Function();
                Global.label_Command = "KEYWORD_STOP";
            }
            else if (columns_serial == "_ac_restart")
            {
                Autokit_Device_1.GP0_GP1_AC_OFF_ON();
                Thread.Sleep(10);
                Autokit_Device_1.GP0_GP1_AC_OFF_ON();
                Global.label_Command = "KEYWORD_AC_RESTART";
            }
            else if (columns_serial == "_shot")
            {
                Global.caption_Num++;
                if (Global.Loop_Number == 1)
                    Global.caption_Sum = Global.caption_Num;
                Autokit_Device_1.Myshot();
                Global.label_Command = "KEYWORD_SHOT";
            }
            else if (columns_serial == "_mail")
            {
                if (Init_Parameter.config_parameter.SendMail_Exist == "1")
                {
                    Global.Pass_Or_Fail = "NG";
                    FormMail FormMail = new FormMail();
                    FormMail.send();
                    Global.label_Command = "KEYWORD_MAIL";
                }
                else
                {
                    //MessageBox.Show("Please enable Mail Function in Settings.");
                }
            }
            else if (columns_serial.Substring(0, 7) == "_savelog")
            {
                string fName = "";
                fName = Init_Parameter.config_parameter.Record_LogPath;
                String savelog_serialport = columns_serial.Substring(9, 1);
                if (savelog_serialport == "A")
                {
                    string t = fName + "\\_SaveLogA_" + DateTime.Now.ToString("yyyyMMddHHmmss") + "_" + Global.label_LoopNumber + ".txt";
                    StreamWriter MYFILE = new StreamWriter(t, false, Encoding.ASCII);
                    MYFILE.Write(Autokit_Command.logA_text);
                    MYFILE.Close();
                }
                else if (savelog_serialport == "B")
                {
                    string t = fName + "\\_SaveLogB_" + DateTime.Now.ToString("yyyyMMddHHmmss") + "_" + Global.label_LoopNumber + ".txt";
                    StreamWriter MYFILE = new StreamWriter(t, false, Encoding.ASCII);
                    MYFILE.Write(Autokit_Command.logB_text);
                    MYFILE.Close();
                }
                else if (savelog_serialport == "C")
                {
                    string t = fName + "\\_SaveLogC_" + DateTime.Now.ToString("yyyyMMddHHmmss") + "_" + Global.label_LoopNumber + ".txt";
                    StreamWriter MYFILE = new StreamWriter(t, false, Encoding.ASCII);
                    MYFILE.Write(Autokit_Command.logC_text);
                    MYFILE.Close();
                }
                else if (savelog_serialport == "D")
                {
                    string t = fName + "\\_SaveLogD_" + DateTime.Now.ToString("yyyyMMddHHmmss") + "_" + Global.label_LoopNumber + ".txt";
                    StreamWriter MYFILE = new StreamWriter(t, false, Encoding.ASCII);
                    MYFILE.Write(Autokit_Command.logD_text);
                    MYFILE.Close();
                }
                else if (savelog_serialport == "E")
                {
                    string t = fName + "\\_SaveLogE_" + DateTime.Now.ToString("yyyyMMddHHmmss") + "_" + Global.label_LoopNumber + ".txt";
                    StreamWriter MYFILE = new StreamWriter(t, false, Encoding.ASCII);
                    MYFILE.Write(Autokit_Command.logE_text);
                    MYFILE.Close();
                }
                else if (savelog_serialport == "O")
                {
                    string t = fName + "\\_SaveLogAll_" + DateTime.Now.ToString("yyyyMMddHHmmss") + "_" + Global.label_LoopNumber + ".txt";
                    StreamWriter MYFILE = new StreamWriter(t, false, Encoding.ASCII);
                    MYFILE.Write(Autokit_Command.logAll_text);
                    MYFILE.Close();
                }
                Global.label_Command = "KEYWORD_SAVELOG";
            }
            else if (columns_serial.Substring(0, 3) == "_rc")
            {
                String rc_key = columns_serial;
                int startIndex = 4;
                int length = rc_key.Length - 4;
                String rc_key_substring = rc_key.Substring(startIndex, length);

                if (Init_Parameter.config_parameter.RedRat_Exist == "1")
                {
                    Autokit_Device_1.Autocommand_RedRat("Form1", rc_key_substring);
                    Global.label_Command = rc_key_substring;
                }
                else if (Init_Parameter.config_parameter.Device_AutoboxExist == "1")
                {
                    Autokit_Device_1.Autocommand_BlueRat("Form1", rc_key_substring);
                    Global.label_Command = rc_key_substring;
                }
            }
            else if (columns_serial.Substring(0, 7) == "_logcmd")
            {
                String log_cmd = columns_serial;
                int startIndex = 10;
                int length = log_cmd.Length - 10;
                String log_cmd_substring = log_cmd.Substring(startIndex, length);
                String log_cmd_serialport = log_cmd.Substring(8, 1);

                if (Init_Parameter.config_parameter.PortA_Checked == "1" && log_cmd_serialport == "A")
                {
                    Serial_Port_1.PortA.WriteLine(log_cmd_substring);
                }
                else if (Init_Parameter.config_parameter.PortB_Checked == "1" && log_cmd_serialport == "B")
                {
                    Serial_Port_1.PortB.WriteLine(log_cmd_substring);
                }
                else if (Init_Parameter.config_parameter.PortC_Checked == "1" && log_cmd_serialport == "C")
                {
                    Serial_Port_1.PortC.WriteLine(log_cmd_substring);
                }
                else if (Init_Parameter.config_parameter.PortD_Checked == "1" && log_cmd_serialport == "D")
                {
                    Serial_Port_1.PortD.WriteLine(log_cmd_substring);
                }
                else if (Init_Parameter.config_parameter.PortE_Checked == "1" && log_cmd_serialport == "E")
                {
                    Serial_Port_1.PortE.WriteLine(log_cmd_substring);
                }
                else if (log_cmd_serialport == "O")
                {
                    if (Init_Parameter.config_parameter.PortA_Checked == "1")
                        Serial_Port_1.PortA.WriteLine(log_cmd_substring);
                    if (Init_Parameter.config_parameter.PortB_Checked == "1")
                        Serial_Port_1.PortB.WriteLine(log_cmd_substring);
                    if (Init_Parameter.config_parameter.PortC_Checked == "1")
                        Serial_Port_1.PortC.WriteLine(log_cmd_substring);
                    if (Init_Parameter.config_parameter.PortD_Checked == "1")
                        Serial_Port_1.PortD.WriteLine(log_cmd_substring);
                    if (Init_Parameter.config_parameter.PortE_Checked == "1")
                        Serial_Port_1.PortE.WriteLine(log_cmd_substring);
                }
                Global.label_Command = "KEYWORD_LOGCMD";
            }
        }
        #endregion
    }
}
