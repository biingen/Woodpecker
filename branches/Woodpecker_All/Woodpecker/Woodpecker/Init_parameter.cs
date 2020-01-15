using jini;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Woodpecker
{
    class Init_Parameter
    {
        public string Device_AutoboxExist;
        public string Device_AutoboxVerson;
        public string Device_AutoboxPort;
        public string Device_DOS;
        public string Device_RunAfterStartUp;
        public string Device_CA310Exist;
        public string RedRat_Exist;
        public string RedRat_RedRatIndex;
        public string RedRat_DBFile;
        public string RedRat_Brands;
        public string RedRat_SerialNumber;
        public string Camera_Exist;
        public string Camera_VideoIndex;
        public string Camera_VideoNumber;
        public string Camera_VideoName;
        public string Camera_AudioIndex;
        public string Camera_AudioNumber;
        public string Camera_AudioName;
        public string Camera_Resolution;
        public string PortA_Checked;
        public string PortA_PortName;
        public string PortA_BaudRate;
        public string PortA_DataBit;
        public string PortA_StopBits;
        public string PortA_DisplayHex;
        public string PortB_Checked;
        public string PortB_PortName;
        public string PortB_BaudRate;
        public string PortB_DataBit;
        public string PortB_StopBits;
        public string PortB_DisplayHex;
        public string PortC_Checked;
        public string PortC_PortName;
        public string PortC_BaudRate;
        public string PortC_DataBit;
        public string PortC_StopBits;
        public string PortC_DisplayHex;
        public string PortD_Checked;
        public string PortD_PortName;
        public string PortD_BaudRate;
        public string PortD_DataBit;
        public string PortD_StopBits;
        public string PortD_DisplayHex;
        public string PortE_Checked;
        public string PortE_PortName;
        public string PortE_BaudRate;
        public string PortE_DataBit;
        public string PortE_StopBits;
        public string PortE_DisplayHex;
        public string Port_Displayhex;
        public string Record_VideoPath;
        public string Record_LogPath;
        public string Record_Generator;
        public string Record_CompareChoose;
        public string Record_CompareDifferent;
        public string Record_EachVideo;
        public string Record_ImportDB;
        public string Record_FootprintMode;
        public string Schedule1_Exist;
        public string Schedule1_Loop;
        public string Schedule1_OnTimeStart;
        public string Schedule1_Timer;
        public string Schedule1_Path;
        public string Schedule2_Exist;
        public string Schedule2_Loop;
        public string Schedule2_OnTimeStart;
        public string Schedule2_Timer;
        public string Schedule2_Path;
        public string Schedule3_Exist;
        public string Schedule3_Loop;
        public string Schedule3_OnTimeStart;
        public string Schedule3_Timer;
        public string Schedule3_Path;
        public string Schedule4_Exist;
        public string Schedule4_Loop;
        public string Schedule4_OnTimeStart;
        public string Schedule4_Timer;
        public string Schedule4_Path;
        public string Schedule5_Exist;
        public string Schedule5_Loop;
        public string Schedule5_OnTimeStart;
        public string Schedule5_Timer;
        public string Schedule5_Path;
        public string Kline_Exist;
        public string Kline_PortName;
        public string Canbus_Exist;
        public string Canbus_Log;
        public string Canbus_BaudRate;
        public string Canbus_DevIndex;
        public string LogSearch_PortA;
        public string LogSearch_PortB;
        public string LogSearch_PortC;
        public string LogSearch_PortD;
        public string LogSearch_PortE;
        public string LogSearch_TextNum;
        public string LogSearch_Camerarecord;
        public string LogSearch_Camerashot;
        public string LogSearch_Sendmail;
        public string LogSearch_Savelog;
        public string LogSearch_Showmessage;
        public string LogSearch_ACcontrol;
        public string LogSearch_ACOFF;
        public string LogSearch_Stop;
        public string LogSearch_Nowvalue;
        public string LogSearch_Times0;
        public string LogSearch_Times1;
        public string LogSearch_Times2;
        public string LogSearch_Times3;
        public string LogSearch_Times4;
        public string LogSearch_Times5;
        public string LogSearch_Times6;
        public string LogSearch_Times7;
        public string LogSearch_Times8;
        public string LogSearch_Times9;

        public string LogSearch_StartTime;
        public string LogSearch_Path;
        public string LogSearch_Text0;
        public string LogSearch_Text1;
        public string LogSearch_Text2;
        public string LogSearch_Text3;
        public string LogSearch_Text4;
        public string LogSearch_Text5;
        public string LogSearch_Text6;
        public string LogSearch_Text7;
        public string LogSearch_Text8;
        public string LogSearch_Text9;
        public string LogSearch_Display0;
        public string LogSearch_Display1;
        public string LogSearch_Display2;
        public string LogSearch_Display3;
        public string LogSearch_Display4;
        public string LogSearch_Display5;
        public string LogSearch_Display6;
        public string LogSearch_Display7;
        public string LogSearch_Display8;
        public string LogSearch_Display9;
        public string SendMail_Exist;
        public string DataInfo_TestCaseNumber;
        public string DataInfo_Result;
        public string DataInfo_NGfrequency;
        public string DataInfo_CreateTime;
        public string DataInfo_CloseTime;
        public string DataInfo_ProjectNumber;
        public string TotalTestTime_Exist;
        public string TotalTestTime_value1;
        public string TotalTestTime_value2;
        public string TotalTestTime_value3;
        public string TotalTestTime_value4;
        public string TotalTestTime_value5;
        public string TotalTestTime_HowLong;
        public string TestCase_value1;
        public string TestCase_value2;
        public string TestCase_value3;
        public string TestCase_value4;
        public string TestCase_value5;
        public string MailInfo_From;
        public string MailInfo_To;
        public string MailInfo_ProjectName;
        public string MailInfo_ModelName;
        public string MailInfo_Version;
        public string MailInfo_Tester;
        public string MailInfo_TeamViewerID;
        public string MailInfo_TeamViewerPassWord;
        public static Init_Parameter config_parameter = new Init_Parameter();
        static string MainSettingPath = Application.StartupPath + "\\Config.ini";
        static string MailPath = Application.StartupPath + "\\Mail.ini";

        public static void Config_initial()
        {
            config_parameter.Device_AutoboxExist = ini12.INIRead(MainSettingPath, "Device", "AutoboxExist", "");
            config_parameter.Device_AutoboxVerson = ini12.INIRead(MainSettingPath, "Device", "AutoboxVerson", "");
            config_parameter.Device_AutoboxPort = ini12.INIRead(MainSettingPath, "Device", "AutoboxPort", "");
            config_parameter.Device_DOS = ini12.INIRead(MainSettingPath, "Device", "DOS", "");
            config_parameter.Device_RunAfterStartUp = ini12.INIRead(MainSettingPath, "Device", "RunAfterStartUp", "");
            config_parameter.Device_CA310Exist = ini12.INIRead(MainSettingPath, "Device", "CA310Exist", "");
            config_parameter.RedRat_Exist = ini12.INIRead(MainSettingPath, "Device", "RedRatExist", "");
            config_parameter.RedRat_RedRatIndex = ini12.INIRead(MainSettingPath, "RedRat", "RedRatIndex", "");
            config_parameter.RedRat_DBFile = ini12.INIRead(MainSettingPath, "RedRat", "DBFile", "");
            config_parameter.RedRat_Brands = ini12.INIRead(MainSettingPath, "RedRat", "Brands", "");
            config_parameter.RedRat_SerialNumber = ini12.INIRead(MainSettingPath, "RedRat", "SerialNumber", "");
            config_parameter.Camera_Exist = ini12.INIRead(MainSettingPath, "Device", "CameraExist", "");
            config_parameter.Camera_VideoIndex = ini12.INIRead(MainSettingPath, "Camera", "VideoIndex", "");
            config_parameter.Camera_VideoNumber = ini12.INIRead(MainSettingPath, "Camera", "VideoNumber", "");
            config_parameter.Camera_VideoName = ini12.INIRead(MainSettingPath, "Camera", "VideoName", "");
            config_parameter.Camera_AudioIndex = ini12.INIRead(MainSettingPath, "Camera", "AudioIndex", "");
            config_parameter.Camera_AudioNumber = ini12.INIRead(MainSettingPath, "Camera", "AudioNumber", "");
            config_parameter.Camera_AudioName = ini12.INIRead(MainSettingPath, "Camera", "AudioName", "");
            config_parameter.Camera_Resolution = ini12.INIRead(MainSettingPath, "Camera", "Resolution", "");
            config_parameter.PortA_Checked = ini12.INIRead(MainSettingPath, "Port A", "Checked", "");
            config_parameter.PortA_PortName = ini12.INIRead(MainSettingPath, "Port A", "PortName", "");
            config_parameter.PortA_BaudRate = ini12.INIRead(MainSettingPath, "Port A", "BaudRate", "");
            config_parameter.PortA_DataBit = ini12.INIRead(MainSettingPath, "Port A", "DataBit", "");
            config_parameter.PortA_StopBits = ini12.INIRead(MainSettingPath, "Port A", "StopBits", "");
            config_parameter.PortA_DisplayHex = ini12.INIRead(MainSettingPath, "Port A", "DisplayHex", "");
            config_parameter.PortB_Checked = ini12.INIRead(MainSettingPath, "Port B", "Checked", "");
            config_parameter.PortB_PortName = ini12.INIRead(MainSettingPath, "Port B", "PortName", "");
            config_parameter.PortB_BaudRate = ini12.INIRead(MainSettingPath, "Port B", "BaudRate", "");
            config_parameter.PortB_DataBit = ini12.INIRead(MainSettingPath, "Port B", "DataBit", "");
            config_parameter.PortB_StopBits = ini12.INIRead(MainSettingPath, "Port B", "StopBits", "");
            config_parameter.PortB_DisplayHex = ini12.INIRead(MainSettingPath, "Port B", "DisplayHex", "");
            config_parameter.PortC_Checked = ini12.INIRead(MainSettingPath, "Port C", "Checked", "");
            config_parameter.PortC_PortName = ini12.INIRead(MainSettingPath, "Port C", "PortName", "");
            config_parameter.PortC_BaudRate = ini12.INIRead(MainSettingPath, "Port C", "BaudRate", "");
            config_parameter.PortC_DataBit = ini12.INIRead(MainSettingPath, "Port C", "DataBit", "");
            config_parameter.PortC_StopBits = ini12.INIRead(MainSettingPath, "Port C", "StopBits", "");
            config_parameter.PortC_DisplayHex = ini12.INIRead(MainSettingPath, "Port C", "DisplayHex", "");
            config_parameter.PortD_Checked = ini12.INIRead(MainSettingPath, "Port D", "Checked", "");
            config_parameter.PortD_PortName = ini12.INIRead(MainSettingPath, "Port D", "PortName", "");
            config_parameter.PortD_BaudRate = ini12.INIRead(MainSettingPath, "Port D", "BaudRate", "");
            config_parameter.PortD_DataBit = ini12.INIRead(MainSettingPath, "Port D", "DataBit", "");
            config_parameter.PortD_StopBits = ini12.INIRead(MainSettingPath, "Port D", "StopBits", "");
            config_parameter.PortD_DisplayHex = ini12.INIRead(MainSettingPath, "Port D", "DisplayHex", "");
            config_parameter.PortE_Checked = ini12.INIRead(MainSettingPath, "Port E", "Checked", "");
            config_parameter.PortE_PortName = ini12.INIRead(MainSettingPath, "Port E", "PortName", "");
            config_parameter.PortE_BaudRate = ini12.INIRead(MainSettingPath, "Port E", "BaudRate", "");
            config_parameter.PortE_DataBit = ini12.INIRead(MainSettingPath, "Port E", "DataBit", "");
            config_parameter.PortE_StopBits = ini12.INIRead(MainSettingPath, "Port E", "StopBits", "");
            config_parameter.PortE_DisplayHex = ini12.INIRead(MainSettingPath, "Port E", "DisplayHex", "");
            config_parameter.Port_Displayhex = ini12.INIRead(MainSettingPath, "Displayhex", "Checked", "");
            config_parameter.Record_VideoPath = ini12.INIRead(MainSettingPath, "Record", "VideoPath", "");
            config_parameter.Record_LogPath = ini12.INIRead(MainSettingPath, "Record", "LogPath", "");
            config_parameter.Record_Generator = ini12.INIRead(MainSettingPath, "Record", "Generator", "");
            config_parameter.Record_CompareChoose = ini12.INIRead(MainSettingPath, "Record", "CompareChoose", "");
            config_parameter.Record_CompareDifferent = ini12.INIRead(MainSettingPath, "Record", "CompareDifferent", "");
            config_parameter.Record_EachVideo = ini12.INIRead(MainSettingPath, "Record", "EachVideo", "");
            config_parameter.Record_ImportDB = ini12.INIRead(MainSettingPath, "Record", "ImportDB", "");
            config_parameter.Record_FootprintMode = ini12.INIRead(MainSettingPath, "Record", "Footprint Mode", "");
            config_parameter.Schedule1_Exist = ini12.INIRead(MainSettingPath, "Schedule1", "Exist", "");
            config_parameter.Schedule1_Loop = ini12.INIRead(MainSettingPath, "Schedule1", "Loop", "");
            config_parameter.Schedule1_OnTimeStart = ini12.INIRead(MainSettingPath, "Schedule1", "OnTimeStart", "");
            config_parameter.Schedule1_Timer = ini12.INIRead(MainSettingPath, "Schedule1", "Timer", "");
            config_parameter.Schedule1_Path = ini12.INIRead(MainSettingPath, "Schedule1", "Path", "");
            config_parameter.Schedule2_Exist = ini12.INIRead(MainSettingPath, "Schedule2", "Exist", "");
            config_parameter.Schedule2_Loop = ini12.INIRead(MainSettingPath, "Schedule2", "Loop", "");
            config_parameter.Schedule2_OnTimeStart = ini12.INIRead(MainSettingPath, "Schedule2", "OnTimeStart", "");
            config_parameter.Schedule2_Timer = ini12.INIRead(MainSettingPath, "Schedule2", "Timer", "");
            config_parameter.Schedule2_Path = ini12.INIRead(MainSettingPath, "Schedule2", "Path", "");
            config_parameter.Schedule3_Exist = ini12.INIRead(MainSettingPath, "Schedule3", "Exist", "");
            config_parameter.Schedule3_Loop = ini12.INIRead(MainSettingPath, "Schedule3", "Loop", "");
            config_parameter.Schedule3_OnTimeStart = ini12.INIRead(MainSettingPath, "Schedule3", "OnTimeStart", "");
            config_parameter.Schedule3_Timer = ini12.INIRead(MainSettingPath, "Schedule3", "Timer", "");
            config_parameter.Schedule3_Path = ini12.INIRead(MainSettingPath, "Schedule3", "Path", "");
            config_parameter.Schedule4_Exist = ini12.INIRead(MainSettingPath, "Schedule4", "Exist", "");
            config_parameter.Schedule4_Loop = ini12.INIRead(MainSettingPath, "Schedule4", "Loop", "");
            config_parameter.Schedule4_OnTimeStart = ini12.INIRead(MainSettingPath, "Schedule4", "OnTimeStart", "");
            config_parameter.Schedule4_Timer = ini12.INIRead(MainSettingPath, "Schedule4", "Timer", "");
            config_parameter.Schedule4_Path = ini12.INIRead(MainSettingPath, "Schedule4", "Path", "");
            config_parameter.Schedule5_Exist = ini12.INIRead(MainSettingPath, "Schedule5", "Exist", "");
            config_parameter.Schedule5_Loop = ini12.INIRead(MainSettingPath, "Schedule5", "Loop", "");
            config_parameter.Schedule5_OnTimeStart = ini12.INIRead(MainSettingPath, "Schedule5", "OnTimeStart", "");
            config_parameter.Schedule5_Timer = ini12.INIRead(MainSettingPath, "Schedule5", "Timer", "");
            config_parameter.Schedule5_Path = ini12.INIRead(MainSettingPath, "Schedule5", "Path", "");
            config_parameter.Kline_Exist = ini12.INIRead(MainSettingPath, "Device", "KlineExist", "");
            config_parameter.Kline_PortName = ini12.INIRead(MainSettingPath, "Kline", "PortName", "");
            config_parameter.Canbus_Exist = ini12.INIRead(MainSettingPath, "Device", "CANbusExist", "");
            config_parameter.Canbus_Log = ini12.INIRead(MainSettingPath, "Canbus", "Log", "");
            config_parameter.Canbus_BaudRate = ini12.INIRead(MainSettingPath, "Canbus", "BaudRate", "");
            config_parameter.Canbus_DevIndex = ini12.INIRead(MainSettingPath, "Canbus", "DevIndex", "");
            config_parameter.LogSearch_PortA = ini12.INIRead(MainSettingPath, "LogSearch", "Comport1", "");
            config_parameter.LogSearch_PortB = ini12.INIRead(MainSettingPath, "LogSearch", "Comport2", "");
            config_parameter.LogSearch_PortC = ini12.INIRead(MainSettingPath, "LogSearch", "Comport3", "");
            config_parameter.LogSearch_PortD = ini12.INIRead(MainSettingPath, "LogSearch", "Comport4", "");
            config_parameter.LogSearch_PortE = ini12.INIRead(MainSettingPath, "LogSearch", "Comport5", "");
            config_parameter.LogSearch_TextNum = ini12.INIRead(MainSettingPath, "LogSearch", "TextNum", "");
            config_parameter.LogSearch_Camerarecord = ini12.INIRead(MainSettingPath, "LogSearch", "Camerarecord", "");
            config_parameter.LogSearch_Camerashot = ini12.INIRead(MainSettingPath, "LogSearch", "Camerashot", "");
            config_parameter.LogSearch_Sendmail = ini12.INIRead(MainSettingPath, "LogSearch", "Sendmail", "");
            config_parameter.LogSearch_Savelog = ini12.INIRead(MainSettingPath, "LogSearch", "Savelog", "");
            config_parameter.LogSearch_Showmessage = ini12.INIRead(MainSettingPath, "LogSearch", "Showmessage", "");
            config_parameter.LogSearch_ACcontrol = ini12.INIRead(MainSettingPath, "LogSearch", "ACcontrol", "");
            config_parameter.LogSearch_ACOFF = ini12.INIRead(MainSettingPath, "LogSearch", "Stop", "");
            config_parameter.LogSearch_Stop = ini12.INIRead(MainSettingPath, "LogSearch", "AC OFF", "");
            config_parameter.LogSearch_Nowvalue = ini12.INIRead(MainSettingPath, "LogSearch", "Nowvalue", "");
            config_parameter.LogSearch_Times0 = ini12.INIRead(MainSettingPath, "LogSearch", "Times0", "");
            config_parameter.LogSearch_Times1 = ini12.INIRead(MainSettingPath, "LogSearch", "Times1", "");
            config_parameter.LogSearch_Times2 = ini12.INIRead(MainSettingPath, "LogSearch", "Times2", "");
            config_parameter.LogSearch_Times3 = ini12.INIRead(MainSettingPath, "LogSearch", "Times3", "");
            config_parameter.LogSearch_Times4 = ini12.INIRead(MainSettingPath, "LogSearch", "Times4", "");
            config_parameter.LogSearch_Times5 = ini12.INIRead(MainSettingPath, "LogSearch", "Times5", "");
            config_parameter.LogSearch_Times6 = ini12.INIRead(MainSettingPath, "LogSearch", "Times6", "");
            config_parameter.LogSearch_Times7 = ini12.INIRead(MainSettingPath, "LogSearch", "Times7", "");
            config_parameter.LogSearch_Times8 = ini12.INIRead(MainSettingPath, "LogSearch", "Times8", "");
            config_parameter.LogSearch_Times9 = ini12.INIRead(MainSettingPath, "LogSearch", "Times9", "");

            config_parameter.LogSearch_StartTime = ini12.INIRead(MainSettingPath, "LogSearch", "StartTime", "");
            config_parameter.LogSearch_Path = ini12.INIRead(MainSettingPath, "LogSearch", "Path", "");
            config_parameter.LogSearch_Text0 = ini12.INIRead(MainSettingPath, "LogSearch", "Text0", "");
            config_parameter.LogSearch_Text1 = ini12.INIRead(MainSettingPath, "LogSearch", "Text1", "");
            config_parameter.LogSearch_Text2 = ini12.INIRead(MainSettingPath, "LogSearch", "Text2", "");
            config_parameter.LogSearch_Text3 = ini12.INIRead(MainSettingPath, "LogSearch", "Text3", "");
            config_parameter.LogSearch_Text4 = ini12.INIRead(MainSettingPath, "LogSearch", "Text4", "");
            config_parameter.LogSearch_Text5 = ini12.INIRead(MainSettingPath, "LogSearch", "Text5", "");
            config_parameter.LogSearch_Text6 = ini12.INIRead(MainSettingPath, "LogSearch", "Text6", "");
            config_parameter.LogSearch_Text7 = ini12.INIRead(MainSettingPath, "LogSearch", "Text7", "");
            config_parameter.LogSearch_Text8 = ini12.INIRead(MainSettingPath, "LogSearch", "Text8", "");
            config_parameter.LogSearch_Text9 = ini12.INIRead(MainSettingPath, "LogSearch", "Text9", "");
            config_parameter.LogSearch_Display0 = ini12.INIRead(MainSettingPath, "LogSearch", "Display0", "");
            config_parameter.LogSearch_Display1 = ini12.INIRead(MainSettingPath, "LogSearch", "Display1", "");
            config_parameter.LogSearch_Display2 = ini12.INIRead(MainSettingPath, "LogSearch", "Display2", "");
            config_parameter.LogSearch_Display3 = ini12.INIRead(MainSettingPath, "LogSearch", "Display3", "");
            config_parameter.LogSearch_Display4 = ini12.INIRead(MainSettingPath, "LogSearch", "Display4", "");
            config_parameter.LogSearch_Display5 = ini12.INIRead(MainSettingPath, "LogSearch", "Display5", "");
            config_parameter.LogSearch_Display6 = ini12.INIRead(MainSettingPath, "LogSearch", "Display6", "");
            config_parameter.LogSearch_Display7 = ini12.INIRead(MainSettingPath, "LogSearch", "Display7", "");
            config_parameter.LogSearch_Display8 = ini12.INIRead(MainSettingPath, "LogSearch", "Display8", "");
            config_parameter.LogSearch_Display9 = ini12.INIRead(MainSettingPath, "LogSearch", "Display9", "");

            config_parameter.SendMail_Exist = ini12.INIRead(MailPath, "SendMail", "value", "");
            config_parameter.DataInfo_TestCaseNumber = ini12.INIRead(MailPath, "DataInfo", "TestCaseNumber", "");
            config_parameter.DataInfo_Result = ini12.INIRead(MailPath, "DataInfo", "Result", "");
            config_parameter.DataInfo_NGfrequency = ini12.INIRead(MailPath, "DataInfo", "NGfrequency", "");
            config_parameter.DataInfo_CreateTime = ini12.INIRead(MailPath, "DataInfo", "CreateTime", "");
            config_parameter.DataInfo_CloseTime = ini12.INIRead(MailPath, "DataInfo", "CloseTime", "");
            config_parameter.DataInfo_ProjectNumber = ini12.INIRead(MailPath, "DataInfo", "ProjectNumber", "");
            config_parameter.TotalTestTime_Exist = ini12.INIRead(MailPath, "TotalTestTime", "value", "");
            config_parameter.TotalTestTime_value1 = ini12.INIRead(MailPath, "TotalTestTime", "value1", "");
            config_parameter.TotalTestTime_value2 = ini12.INIRead(MailPath, "TotalTestTime", "value2", "");
            config_parameter.TotalTestTime_value3 = ini12.INIRead(MailPath, "TotalTestTime", "value3", "");
            config_parameter.TotalTestTime_value4 = ini12.INIRead(MailPath, "TotalTestTime", "value4", "");
            config_parameter.TotalTestTime_value5 = ini12.INIRead(MailPath, "TotalTestTime", "value5", "");
            config_parameter.TotalTestTime_HowLong = ini12.INIRead(MailPath, "TotalTestTime", "How Long", "");
            config_parameter.TestCase_value1 = ini12.INIRead(MailPath, "TestCase", "TestCase1", "");
            config_parameter.TestCase_value2 = ini12.INIRead(MailPath, "TestCase", "TestCase2", "");
            config_parameter.TestCase_value3 = ini12.INIRead(MailPath, "TestCase", "TestCase3", "");
            config_parameter.TestCase_value4 = ini12.INIRead(MailPath, "TestCase", "TestCase4", "");
            config_parameter.TestCase_value5 = ini12.INIRead(MailPath, "TestCase", "TestCase5", "");
            config_parameter.MailInfo_From = ini12.INIRead(MailPath, "MailInfo", "From", "");
            config_parameter.MailInfo_To = ini12.INIRead(MailPath, "MailInfo", "To", "");
            config_parameter.MailInfo_ProjectName = ini12.INIRead(MailPath, "MailInfo", "ProjectName", "");
            config_parameter.MailInfo_ModelName = ini12.INIRead(MailPath, "MailInfo", "ModelName", "");
            config_parameter.MailInfo_Version = ini12.INIRead(MailPath, "MailInfo", "Version", "");
            config_parameter.MailInfo_Tester = ini12.INIRead(MailPath, "MailInfo", "Tester", "");
            config_parameter.MailInfo_TeamViewerID = ini12.INIRead(MailPath, "MailInfo", "TeamViewerID", "");
            config_parameter.MailInfo_TeamViewerPassWord = ini12.INIRead(MailPath, "MailInfo", "TeamViewerPassWord", "");
        }

        public struct Device
        {
            public uint AutoboxExist;
            public uint AutoboxVerson;
            public string AutoboxPort;
            public string DOS;
            public uint RunAfterStartUp;
            public uint CA310Exist;
        }

        public struct RedRat
        {
            public uint RedRatExist;
            public uint RedRatIndex;
            public string DBFile;
            public string Brands;
            public string SerialNumber;
        }

        public struct Camera
        {
            public uint CameraExist;
            public uint VideoIndex;
            public uint VideoNumber;
            public uint VideoName;
            public uint AudioIndex;
            public uint AudioNumber;
            public uint AudioName;
        }

        public struct SerialPort
        {
            public uint Checked;
            public string PortName;
            public uint BaudRate;
            public string DataBit;
            public string StopBits;
            public uint DisplayHex;
        }

        public struct Record
        {
            public string VideoPath;
            public string LogPath;
            public string Generator;
            public uint CompareChoose;
            public uint CompareDifferent;
            public uint EachVideo;
            public uint ImportDB;
            public uint FootprintMode;
        }

        public struct Schedule
        {
            public uint Exist;
            public uint Loop;
            public uint OnTimeStart;
            public string Timer;
            public string Path;
        }

        public struct Kline
        {
            public uint Exist;
            public string PortName;
        }

        public struct Canbus
        {
            public uint Exist;
            public uint Log;
            public string BaudRate;
            public uint DevIndex;
        }

        public struct LogSearch
        {
            public uint Comport1;
            public uint Comport2;
            public uint Comport3;
            public uint Comport4;
            public uint Comport5;
            public uint TextNum;
            public uint Camerarecord;
            public uint Camerashot;
            public uint Sendmail;
            public uint Savelog;
            public uint Showmessage;
            public uint ACcontrol;
            public uint ACOFF;
            public uint Stop;
            public uint Nowvalue;
            public uint Times0;
            public uint Times1;
            public uint Times2;
            public uint Times3;
            public uint Times4;
            public uint Times5;
            public uint Times6;
            public uint Times7;
            public uint Times8;
            public uint Times9;

            public string StartTime;
            public string Path;
            public string Text0;
            public string Text1;
            public string Text2;
            public string Text3;
            public string Text4;
            public string Text5;
            public string Text6;
            public string Text7;
            public string Text8;
            public string Text9;
            public string Display0;
            public string Display1;
            public string Display2;
            public string Display3;
            public string Display4;
            public string Display5;
            public string Display6;
            public string Display7;
            public string Display8;
            public string Display9;
        }
    }
}
