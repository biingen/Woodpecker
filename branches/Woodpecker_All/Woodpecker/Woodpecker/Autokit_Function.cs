﻿using System;
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
        private SerialPortParemeter Serial_Paremeter_1 = new SerialPortParemeter();
        private Serial_Port Serial_Device_1 = new Serial_Port();

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

        #region -- Start 指令集 --
        public void Start_Function()
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
                //button_Schedule1.PerformClick();

                //Thread Log1Data = new Thread(new ThreadStart(Log1_Receiving_Task));
                //Thread Log2Data = new Thread(new ThreadStart(Log2_Receiving_Task));

                if (Global.StartButtonPressed == true)//按下STOP//
                {
                    Global.Break_Out_MyRunCamd = 1;//跳出倒數迴圈//
                    //MainThread.Abort();//停止執行緒//
                    //timer1.Stop();//停止倒數//
                    CloseDtplay();//關閉DtPlay//

                    if (Init_Parameter.config_parameter.PortA_Checked == "1")
                    {
                        if (Init_Parameter.config_parameter.LogSearch_TextNum != "0")
                        {
                            //LogThread1.Abort();
                            //Log1Data.Abort();
                        }
                    }

                    if (Init_Parameter.config_parameter.PortB_Checked == "1")
                    {
                        if (Init_Parameter.config_parameter.LogSearch_TextNum != "0")
                        {
                            //LogThread2.Abort();
                            //Log2Data.Abort();
                        }
                    }

                    if (Init_Parameter.config_parameter.PortC_Checked == "1")
                    {
                        if (Init_Parameter.config_parameter.LogSearch_TextNum != "0")
                        {
                            //LogThread3.Abort();
                            //Log3Data.Abort();
                        }
                    }

                    if (Init_Parameter.config_parameter.PortD_Checked == "1")
                    {
                        if (Init_Parameter.config_parameter.LogSearch_TextNum != "0")
                        {
                            //LogThread4.Abort();
                            //Log4Data.Abort();
                        }
                    }

                    if (Init_Parameter.config_parameter.PortE_Checked == "1")
                    {
                        if (Init_Parameter.config_parameter.LogSearch_TextNum != "0")
                        {
                            //LogThread5.Abort();
                            //Log5Data.Abort();
                        }
                    }

                    Global.StartButtonPressed = false;
                    //button_Start.Enabled = false;
                    //button_Setting.Enabled = false;
                    //button_SaveSchedule.Enabled = false;
                    //button_Pause.Enabled = true;
                    //setStyle();
                    Global.label_Command = "Please wait...";
                }
                else//按下START//
                {
                    /*
                    for (int i = 1; i < 6; i++)
                    {
                        if (Directory.Exists(ini12.INIRead(sPath, "Record", "VideoPath", "") + "\\" + "Schedule" + i + "_Original") == true)
                        {
                            DirectoryInfo DIFO = new DirectoryInfo(ini12.INIRead(sPath, "Record", "VideoPath", "") + "\\" + "Schedule" + i + "_Original");
                            DIFO.Delete(true);
                        }

                        if (Directory.Exists(ini12.INIRead(sPath, "Record", "VideoPath", "") + "\\" + "Schedule" + i + "_NG") == true)
                        {
                            DirectoryInfo DIFO = new DirectoryInfo(ini12.INIRead(sPath, "Record", "VideoPath", "") + "\\" + "Schedule" + i + "_NG");
                            DIFO.Delete(true);
                        }                
                    }
                    */
                    Global.Break_Out_MyRunCamd = 0;

                    Init_Parameter.config_parameter.LogSearch_StartTime = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
                    //MainThread.Start();       // 啟動執行緒
                    //timer1.Start();     //開始倒數
                    //button_Start.Text = "STOP";

                    Global.StartButtonPressed = true;
                    //button_Setting.Enabled = false;
                    //button_Pause.Enabled = true;
                    //button_SaveSchedule.Enabled = false;
                    //setStyle();

                    if (Init_Parameter.config_parameter.PortA_Checked == "1")
                    {
                        Serial_Paremeter_1.PortName = Init_Parameter.config_parameter.PortA_PortName;
                        Serial_Device_1.OpenSerialPort("A", Serial_Paremeter_1);
                        //textBox1.Text = string.Empty;//清空serialport1//
                        if (Init_Parameter.config_parameter.LogSearch_TextNum != "0" && Init_Parameter.config_parameter.LogSearch_PortA == "1")
                        {
                            //LogThread1.IsBackground = true;
                            //LogThread1.Start();
                        }
                    }

                    if (Init_Parameter.config_parameter.PortB_Checked == "1")
                    {
                        Serial_Paremeter_1.PortName = Init_Parameter.config_parameter.PortB_PortName;
                        Serial_Device_1.OpenSerialPort("B", Serial_Paremeter_1);
                        if (Init_Parameter.config_parameter.LogSearch_TextNum != "0" && Init_Parameter.config_parameter.LogSearch_PortB == "1")
                        {
                            //LogThread2.IsBackground = true;
                            //LogThread2.Start();
                        }
                    }

                    if (Init_Parameter.config_parameter.PortC_Checked == "1")
                    {
                        Serial_Paremeter_1.PortName = Init_Parameter.config_parameter.PortC_PortName;
                        Serial_Device_1.OpenSerialPort("C", Serial_Paremeter_1);
                        if (Init_Parameter.config_parameter.LogSearch_TextNum != "0" && Init_Parameter.config_parameter.LogSearch_PortC == "1")
                        {
                            //LogThread3.IsBackground = true;
                            //LogThread3.Start();
                        }
                    }

                    if (Init_Parameter.config_parameter.PortD_Checked == "1")
                    {
                        Serial_Paremeter_1.PortName = Init_Parameter.config_parameter.PortD_PortName;
                        Serial_Device_1.OpenSerialPort("D", Serial_Paremeter_1);
                        if (Init_Parameter.config_parameter.LogSearch_TextNum != "0" && Init_Parameter.config_parameter.LogSearch_PortD == "1")
                        {
                            //LogThread4.IsBackground = true;
                            //LogThread4.Start();
                        }
                    }

                    if (Init_Parameter.config_parameter.PortE_Checked == "1")
                    {
                        Serial_Paremeter_1.PortName = Init_Parameter.config_parameter.PortE_PortName;
                        Serial_Device_1.OpenSerialPort("E", Serial_Paremeter_1);
                        if (Init_Parameter.config_parameter.LogSearch_TextNum != "0" && Init_Parameter.config_parameter.LogSearch_PortE == "1")
                        {
                            //LogThread5.IsBackground = true;
                            //LogThread5.Start();
                        }
                    }

                    if (Init_Parameter.config_parameter.Kline_Exist == "1")
                    {
                        Serial_Device_1.OpenKlinePort();
                        //textBox_serial.Text = ""; //清空kline//
                    }
                }
            }
            else//如果沒接AutoBox//
            {
                if (Global.StartButtonPressed == true)//按下STOP//
                {
                    Global.Break_Out_MyRunCamd = 1;    //跳出倒數迴圈
                    //MainThread.Abort(); //停止執行緒
                    //timer1.Stop();  //停止倒數
                    CloseDtplay();

                    if (Init_Parameter.config_parameter.PortA_Checked == "1")
                    {
                        if (Init_Parameter.config_parameter.LogSearch_TextNum != "0")
                        {
                            //LogThread1.Abort();
                            //Log1Data.Abort();
                        }
                    }

                    if (Init_Parameter.config_parameter.PortB_Checked == "1")
                    {
                        if (Init_Parameter.config_parameter.LogSearch_TextNum != "0")
                        {
                            //LogThread2.Abort();
                            //Log2Data.Abort();
                        }
                    }

                    if (Init_Parameter.config_parameter.PortC_Checked == "1")
                    {
                        if (Init_Parameter.config_parameter.LogSearch_TextNum != "0")
                        {
                            //LogThread3.Abort();
                            //Log3Data.Abort();
                        }
                    }

                    if (Init_Parameter.config_parameter.PortD_Checked == "1")
                    {
                        if (Init_Parameter.config_parameter.LogSearch_TextNum != "0")
                        {
                            //LogThread4.Abort();
                            //Log4Data.Abort();
                        }
                    }

                    if (Init_Parameter.config_parameter.PortE_Checked == "1")
                    {
                        if (Init_Parameter.config_parameter.LogSearch_TextNum != "0")
                        {
                            //LogThread5.Abort();
                            //Log4Data.Abort();
                        }
                    }

                    Global.label_Command = "Please wait...";
                }
                else//按下START//
                {
                    Global.Break_Out_MyRunCamd = 0;
                    //MainThread.Start();// 啟動執行緒
                    //timer1.Start();     //開始倒數

                    Global.StartButtonPressed = true;
                    //button_Setting.Enabled = false;
                    //button_Pause.Enabled = true;
                    //pictureBox_AcPower.Image = Properties.Resources.OFF;
                    //button_Start.Text = "STOP";
                    //setStyle();

                    if (Init_Parameter.config_parameter.PortA_Checked == "1")
                    {
                        Serial_Paremeter_1.PortName = Init_Parameter.config_parameter.PortA_PortName;
                        Serial_Device_1.OpenSerialPort("A", Serial_Paremeter_1);
                        //textBox_serial.Clear();
                        //textBox1.Text = string.Empty;//清空serialport1//
                        if (Init_Parameter.config_parameter.LogSearch_TextNum != "0" && Init_Parameter.config_parameter.LogSearch_PortA == "1")
                        {
                            //LogThread1.IsBackground = true;
                            //LogThread1.Start();
                        }
                    }

                    if (Init_Parameter.config_parameter.PortB_Checked == "1")
                    {
                        Serial_Paremeter_1.PortName = Init_Parameter.config_parameter.PortB_PortName;
                        Serial_Device_1.OpenSerialPort("B", Serial_Paremeter_1);
                        if (Init_Parameter.config_parameter.LogSearch_TextNum != "0" && Init_Parameter.config_parameter.LogSearch_PortB == "1")
                        {
                            //LogThread2.IsBackground = true;
                            //LogThread2.Start();
                        }
                    }

                    if (Init_Parameter.config_parameter.PortC_Checked == "1")
                    {
                        Serial_Paremeter_1.PortName = Init_Parameter.config_parameter.PortC_PortName;
                        Serial_Device_1.OpenSerialPort("C", Serial_Paremeter_1);
                        if (Init_Parameter.config_parameter.LogSearch_TextNum != "0" && Init_Parameter.config_parameter.LogSearch_PortC == "1")
                        {
                            //LogThread3.IsBackground = true;
                            //LogThread3.Start();
                        }
                    }

                    if (Init_Parameter.config_parameter.PortD_Checked == "1")
                    {
                        Serial_Paremeter_1.PortName = Init_Parameter.config_parameter.PortD_PortName;
                        Serial_Device_1.OpenSerialPort("D", Serial_Paremeter_1);
                        if (Init_Parameter.config_parameter.LogSearch_TextNum != "0" && Init_Parameter.config_parameter.LogSearch_PortD == "1")
                        {
                            //LogThread4.IsBackground = true;
                            //LogThread4.Start();
                        }
                    }

                    if (Init_Parameter.config_parameter.PortE_Checked == "1")
                    {
                        Serial_Paremeter_1.PortName = Init_Parameter.config_parameter.PortE_PortName;
                        Serial_Device_1.OpenSerialPort("E", Serial_Paremeter_1);
                        if (Init_Parameter.config_parameter.LogSearch_TextNum != "0" && Init_Parameter.config_parameter.LogSearch_PortE == "1")
                        {
                            //LogThread5.IsBackground = true;
                            //LogThread5.Start();
                        }
                    }

                    if (Init_Parameter.config_parameter.Kline_Exist == "1")
                    {
                        Serial_Device_1.OpenKlinePort();
                        //textBox_serial.Text = ""; //清空kline//
                    }
                }
            }
        }
        #endregion

        #region -- Dektec Play 指令集 --
        public void StartDtplay()
        {
            string StreamName = Extra_Commander.columns_serial;
            string TvSystem = Extra_Commander.columns_function;
            string Freq = Extra_Commander.columns_subFunction;
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
            string columns_serial = Extra_Commander.columns_serial;
            if (columns_serial == "_pause")
            {
                Pause_Function();
                Global.label_Command = "IO CMD_PAUSE";
            }
            else if (columns_serial == "_stop")
            {
                Start_Function();
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
                    Serial_Device_1.PortA.WriteLine(log_cmd_substring);
                }
                else if (Init_Parameter.config_parameter.PortB_Checked == "1" && log_cmd_serialport == "B")
                {
                    Serial_Device_1.PortB.WriteLine(log_cmd_substring);
                }
                else if (Init_Parameter.config_parameter.PortC_Checked == "1" && log_cmd_serialport == "C")
                {
                    Serial_Device_1.PortC.WriteLine(log_cmd_substring);
                }
                else if (Init_Parameter.config_parameter.PortD_Checked == "1" && log_cmd_serialport == "D")
                {
                    Serial_Device_1.PortD.WriteLine(log_cmd_substring);
                }
                else if (Init_Parameter.config_parameter.PortE_Checked == "1" && log_cmd_serialport == "E")
                {
                    Serial_Device_1.PortE.WriteLine(log_cmd_substring);
                }
                else if (log_cmd_serialport == "O")
                {
                    if (Init_Parameter.config_parameter.PortA_Checked == "1")
                        Serial_Device_1.PortA.WriteLine(log_cmd_substring);
                    if (Init_Parameter.config_parameter.PortB_Checked == "1")
                        Serial_Device_1.PortB.WriteLine(log_cmd_substring);
                    if (Init_Parameter.config_parameter.PortC_Checked == "1")
                        Serial_Device_1.PortC.WriteLine(log_cmd_substring);
                    if (Init_Parameter.config_parameter.PortD_Checked == "1")
                        Serial_Device_1.PortD.WriteLine(log_cmd_substring);
                    if (Init_Parameter.config_parameter.PortE_Checked == "1")
                        Serial_Device_1.PortE.WriteLine(log_cmd_substring);
                }
            }
        }
        #endregion

        #region -- KEYWORD 指令集 --
        public void KeywordCommand()
        {
            string columns_serial = Extra_Commander.columns_serial;
            if (columns_serial == "_pause")
            {
                Pause_Function();
                Global.label_Command = "KEYWORD_PAUSE";
            }
            else if (columns_serial == "_stop")
            {
                Start_Function();
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
                    MYFILE.Write(Extra_Commander.logA_text);
                    MYFILE.Close();
                }
                else if (savelog_serialport == "B")
                {
                    string t = fName + "\\_SaveLogB_" + DateTime.Now.ToString("yyyyMMddHHmmss") + "_" + Global.label_LoopNumber + ".txt";
                    StreamWriter MYFILE = new StreamWriter(t, false, Encoding.ASCII);
                    MYFILE.Write(Extra_Commander.logB_text);
                    MYFILE.Close();
                }
                else if (savelog_serialport == "C")
                {
                    string t = fName + "\\_SaveLogC_" + DateTime.Now.ToString("yyyyMMddHHmmss") + "_" + Global.label_LoopNumber + ".txt";
                    StreamWriter MYFILE = new StreamWriter(t, false, Encoding.ASCII);
                    MYFILE.Write(Extra_Commander.logC_text);
                    MYFILE.Close();
                }
                else if (savelog_serialport == "D")
                {
                    string t = fName + "\\_SaveLogD_" + DateTime.Now.ToString("yyyyMMddHHmmss") + "_" + Global.label_LoopNumber + ".txt";
                    StreamWriter MYFILE = new StreamWriter(t, false, Encoding.ASCII);
                    MYFILE.Write(Extra_Commander.logD_text);
                    MYFILE.Close();
                }
                else if (savelog_serialport == "E")
                {
                    string t = fName + "\\_SaveLogE_" + DateTime.Now.ToString("yyyyMMddHHmmss") + "_" + Global.label_LoopNumber + ".txt";
                    StreamWriter MYFILE = new StreamWriter(t, false, Encoding.ASCII);
                    MYFILE.Write(Extra_Commander.logE_text);
                    MYFILE.Close();
                }
                else if (savelog_serialport == "O")
                {
                    string t = fName + "\\_SaveLogAll_" + DateTime.Now.ToString("yyyyMMddHHmmss") + "_" + Global.label_LoopNumber + ".txt";
                    StreamWriter MYFILE = new StreamWriter(t, false, Encoding.ASCII);
                    MYFILE.Write(Extra_Commander.logAll_text);
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
                    Serial_Device_1.PortA.WriteLine(log_cmd_substring);
                }
                else if (Init_Parameter.config_parameter.PortB_Checked == "1" && log_cmd_serialport == "B")
                {
                    Serial_Device_1.PortB.WriteLine(log_cmd_substring);
                }
                else if (Init_Parameter.config_parameter.PortC_Checked == "1" && log_cmd_serialport == "C")
                {
                    Serial_Device_1.PortC.WriteLine(log_cmd_substring);
                }
                else if (Init_Parameter.config_parameter.PortD_Checked == "1" && log_cmd_serialport == "D")
                {
                    Serial_Device_1.PortD.WriteLine(log_cmd_substring);
                }
                else if (Init_Parameter.config_parameter.PortE_Checked == "1" && log_cmd_serialport == "E")
                {
                    Serial_Device_1.PortE.WriteLine(log_cmd_substring);
                }
                else if (log_cmd_serialport == "O")
                {
                    if (Init_Parameter.config_parameter.PortA_Checked == "1")
                        Serial_Device_1.PortA.WriteLine(log_cmd_substring);
                    if (Init_Parameter.config_parameter.PortB_Checked == "1")
                        Serial_Device_1.PortB.WriteLine(log_cmd_substring);
                    if (Init_Parameter.config_parameter.PortC_Checked == "1")
                        Serial_Device_1.PortC.WriteLine(log_cmd_substring);
                    if (Init_Parameter.config_parameter.PortD_Checked == "1")
                        Serial_Device_1.PortD.WriteLine(log_cmd_substring);
                    if (Init_Parameter.config_parameter.PortE_Checked == "1")
                        Serial_Device_1.PortE.WriteLine(log_cmd_substring);
                }
                Global.label_Command = "KEYWORD_LOGCMD";
            }
        }
        #endregion
    }
}
