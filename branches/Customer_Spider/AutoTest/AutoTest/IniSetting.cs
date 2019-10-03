﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using jini;
using System.Windows.Forms;

namespace AutoTest
{
    public class Pri_Setting
    {
        private static string MainSettingPath = Application.StartupPath + "\\Config.ini";
        private static string MailSettingPath = Application.StartupPath + "\\Mail.ini";
        private static string RcSettingPath = Application.StartupPath + "\\RC.ini";

        #region -- Config.ini --
        private static string Device_AutoboxExist = ini12.INIRead(MainSettingPath, "Device", "AutoboxExist", "");
        private static string Device_AutoboxVerson = ini12.INIRead(MainSettingPath, "Device", "AutoboxVerson", "");
        private static string Device_AutoboxPort = ini12.INIRead(MainSettingPath, "Device", "AutoboxPort", "");
        private static string Device_CameraExist = ini12.INIRead(MainSettingPath, "Device", "CameraExist", "");
        private static string Device_RedRatExist = ini12.INIRead(MainSettingPath, "Device", "RedRatExist", "");
        private static string Device_CANbusExist = ini12.INIRead(MainSettingPath, "Device", "CANbusExist", "");
        private static string Device_KlineExist = ini12.INIRead(MainSettingPath, "Device", "KlineExist", "");
        private static string Cmd_DOS = ini12.INIRead(MainSettingPath, "Device", "DOS", "");
        private static string Cmd_RunAfterStartUp = ini12.INIRead(MainSettingPath, "Device", "RunAfterStartUp", "");

        private static string RedRat_Index = ini12.INIRead(MainSettingPath, "RedRat", "RedRatIndex", "");
        private static string RedRat_DBFile = ini12.INIRead(MainSettingPath, "RedRat", "DBFile", "");
        private static string RedRat_Brands = ini12.INIRead(MainSettingPath, "RedRat", "Brands", "");
        private static string RedRat_SerialNumber = ini12.INIRead(MainSettingPath, "RedRat", "SerialNumber", "");

        private static string Camera_VideoIndex = ini12.INIRead(MainSettingPath, "Camera", "VideoIndex", "");
        private static string Camera_VideoNumber = ini12.INIRead(MainSettingPath, "Camera", "VideoNumber", "");
        private static string Camera_VideoName = ini12.INIRead(MainSettingPath, "Camera", "VideoName", "");
        private static string Camera_Resolution = ini12.INIRead(MainSettingPath, "Camera", "Resolution", "");
        private static string Camera_AudioIndex = ini12.INIRead(MainSettingPath, "Camera", "AudioIndex", "");
        private static string Camera_AudioNumber = ini12.INIRead(MainSettingPath, "Camera", "AudioNumber", "");
        private static string Camera_AudioName = ini12.INIRead(MainSettingPath, "Camera", "AudioName", "");

        private static string Comport_Checked = ini12.INIRead(MainSettingPath, "Comport", "Checked", "");
        private static string Comport_PortName = ini12.INIRead(MainSettingPath, "Comport", "PortName", "");
        private static string Comport_VirtualName = ini12.INIRead(MainSettingPath, "Comport", "VirtualName", "");
        private static string Comport_BaudRate = ini12.INIRead(MainSettingPath, "Comport", "BaudRate", "");
        private static string Comport_DataBit = ini12.INIRead(MainSettingPath, "Comport", "DataBit", "");
        private static string Comport_StopBits = ini12.INIRead(MainSettingPath, "Comport", "StopBits", "");

        private static string ExtComport_Checked = ini12.INIRead(MainSettingPath, "ExtComport", "Checked", "");
        private static string ExtComport_PortName = ini12.INIRead(MainSettingPath, "ExtComport", "PortName", "");
        private static string ExtComport_VirtualName = ini12.INIRead(MainSettingPath, "ExtComport", "VirtualName", "");
        private static string ExtComport_BaudRate = ini12.INIRead(MainSettingPath, "ExtComport", "BaudRate", "");
        private static string ExtComport_DataBit = ini12.INIRead(MainSettingPath, "ExtComport", "DataBit", "");
        private static string ExtComport_StopBits = ini12.INIRead(MainSettingPath, "ExtComport", "StopBits", "");

        private static string TriComport_Checked = ini12.INIRead(MainSettingPath, "TriComport", "Checked", "");
        private static string TriComport_PortName = ini12.INIRead(MainSettingPath, "TriComport", "PortName", "");
        private static string TriComport_VirtualName = ini12.INIRead(MainSettingPath, "TriComport", "VirtualName", "");
        private static string TriComport_BaudRate = ini12.INIRead(MainSettingPath, "TriComport", "BaudRate", "");
        private static string TriComport_DataBit = ini12.INIRead(MainSettingPath, "TriComport", "DataBit", "");
        private static string TriComport_StopBits = ini12.INIRead(MainSettingPath, "TriComport", "StopBits", "");

        private static string Record_VideoPath = ini12.INIRead(MainSettingPath, "Record", "VideoPath", "");
        private static string Record_LogPath = ini12.INIRead(MainSettingPath, "Record", "LogPath", "");
        private static string Record_Generator = ini12.INIRead(MainSettingPath, "Record", "Generator", "");
        private static string Record_CompareChoose = ini12.INIRead(MainSettingPath, "Record", "CompareChoose", "");
        private static string Record_CompareDifferent = ini12.INIRead(MainSettingPath, "Record", "CompareDifferent", "");
        private static string Record_EachVideo = ini12.INIRead(MainSettingPath, "Record", "EachVideo", "");
        private static string Record_ImportDB = ini12.INIRead(MainSettingPath, "Record", "ImportDB", "");
        private static string Record_FootprintMode = ini12.INIRead(MainSettingPath, "Record", "Footprint Mode", "");
        private static string Record_CANbusLog = ini12.INIRead(MainSettingPath, "Record", "CANbusLog", "");

        private static string Schedule1_Exist = ini12.INIRead(MainSettingPath, "Schedule1", "Exist", "");
        private static string Schedule1_Loop = ini12.INIRead(MainSettingPath, "Schedule1", "Loop", "");
        private static string Schedule1_OnTimeStart = ini12.INIRead(MainSettingPath, "Schedule1", "OnTimeStart", "");
        private static string Schedule1_Timer = ini12.INIRead(MainSettingPath, "Schedule1", "Timer", "");
        private static string Schedule1_Path = ini12.INIRead(MainSettingPath, "Schedule1", "Path", "");

        private static string Schedule2_Exist = ini12.INIRead(MainSettingPath, "Schedule2", "Exist", "");
        private static string Schedule2_Loop = ini12.INIRead(MainSettingPath, "Schedule2", "Loop", "");
        private static string Schedule2_OnTimeStart = ini12.INIRead(MainSettingPath, "Schedule2", "OnTimeStart", "");
        private static string Schedule2_Timer = ini12.INIRead(MainSettingPath, "Schedule2", "Timer", "");
        private static string Schedule2_Path = ini12.INIRead(MainSettingPath, "Schedule2", "Path", "");

        private static string Schedule3_Exist = ini12.INIRead(MainSettingPath, "Schedule3", "Exist", "");
        private static string Schedule3_Loop = ini12.INIRead(MainSettingPath, "Schedule3", "Loop", "");
        private static string Schedule3_OnTimeStart = ini12.INIRead(MainSettingPath, "Schedule3", "OnTimeStart", "");
        private static string Schedule3_Timer = ini12.INIRead(MainSettingPath, "Schedule3", "Timer", "");
        private static string Schedule3_Path = ini12.INIRead(MainSettingPath, "Schedule3", "Path", "");

        private static string Schedule4_Exist = ini12.INIRead(MainSettingPath, "Schedule4", "Exist", "");
        private static string Schedule4_Loop = ini12.INIRead(MainSettingPath, "Schedule4", "Loop", "");
        private static string Schedule4_OnTimeStart = ini12.INIRead(MainSettingPath, "Schedule4", "OnTimeStart", "");
        private static string Schedule4_Timer = ini12.INIRead(MainSettingPath, "Schedule4", "Timer", "");
        private static string Schedule4_Path = ini12.INIRead(MainSettingPath, "Schedule4", "Path", "");

        private static string Schedule5_Exist = ini12.INIRead(MainSettingPath, "Schedule5", "Exist", "");
        private static string Schedule5_Loop = ini12.INIRead(MainSettingPath, "Schedule5", "Loop", "");
        private static string Schedule5_OnTimeStart = ini12.INIRead(MainSettingPath, "Schedule5", "OnTimeStart", "");
        private static string Schedule5_Timer = ini12.INIRead(MainSettingPath, "Schedule5", "Timer", "");
        private static string Schedule5_Path = ini12.INIRead(MainSettingPath, "Schedule5", "Path", "");

        private static string LogSearch_StartTime = ini12.INIRead(MainSettingPath, "LogSearch", "StartTime", "");
        private static string LogSearch_Comport1 = ini12.INIRead(MainSettingPath, "LogSearch", "Comport1", "");
        private static string LogSearch_Comport2 = ini12.INIRead(MainSettingPath, "LogSearch", "Comport2", "");
        private static string LogSearch_Camerarecord = ini12.INIRead(MainSettingPath, "LogSearch", "Camerarecord", "");
        private static string LogSearch_Camerashot = ini12.INIRead(MainSettingPath, "LogSearch", "Camerashot", "");
        private static string LogSearch_Sendmail = ini12.INIRead(MainSettingPath, "LogSearch", "Sendmail", "");
        private static string LogSearch_Savelog = ini12.INIRead(MainSettingPath, "LogSearch", "Savelog", "");
        private static string LogSearch_Showmessage = ini12.INIRead(MainSettingPath, "LogSearch", "Showmessage", "");
        private static string LogSearch_ACcontrol = ini12.INIRead(MainSettingPath, "LogSearch", "ACcontrol", "");
        private static string LogSearch_Stop = ini12.INIRead(MainSettingPath, "LogSearch", "Stop", "");
        private static string LogSearch_ACOFF = ini12.INIRead(MainSettingPath, "LogSearch", "AC OFF", "");
        private static string LogSearch_Nowvalue = ini12.INIRead(MainSettingPath, "LogSearch", "Nowvalue", "");
        private static string LogSearch_Text0 = ini12.INIRead(MainSettingPath, "LogSearch", "Text0", "");
        private static string LogSearch_Text1 = ini12.INIRead(MainSettingPath, "LogSearch", "Text1", "");
        private static string LogSearch_Text2 = ini12.INIRead(MainSettingPath, "LogSearch", "Text2", "");
        private static string LogSearch_Text3 = ini12.INIRead(MainSettingPath, "LogSearch", "Text3", "");
        private static string LogSearch_Text4 = ini12.INIRead(MainSettingPath, "LogSearch", "Text4", "");
        private static string LogSearch_Text5 = ini12.INIRead(MainSettingPath, "LogSearch", "Text5", "");
        private static string LogSearch_Text6 = ini12.INIRead(MainSettingPath, "LogSearch", "Text6", "");
        private static string LogSearch_Text7 = ini12.INIRead(MainSettingPath, "LogSearch", "Text7", "");
        private static string LogSearch_Text8 = ini12.INIRead(MainSettingPath, "LogSearch", "Text8", "");
        private static string LogSearch_Text9 = ini12.INIRead(MainSettingPath, "LogSearch", "Text9", "");
        private static string LogSearch_Times0 = ini12.INIRead(MainSettingPath, "LogSearch", "Times0", "");
        private static string LogSearch_Times1 = ini12.INIRead(MainSettingPath, "LogSearch", "Times1", "");
        private static string LogSearch_Times2 = ini12.INIRead(MainSettingPath, "LogSearch", "Times2", "");
        private static string LogSearch_Times3 = ini12.INIRead(MainSettingPath, "LogSearch", "Times3", "");
        private static string LogSearch_Times4 = ini12.INIRead(MainSettingPath, "LogSearch", "Times4", "");
        private static string LogSearch_Times5 = ini12.INIRead(MainSettingPath, "LogSearch", "Times5", "");
        private static string LogSearch_Times6 = ini12.INIRead(MainSettingPath, "LogSearch", "Times6", "");
        private static string LogSearch_Times7 = ini12.INIRead(MainSettingPath, "LogSearch", "Times7", "");
        private static string LogSearch_Times8 = ini12.INIRead(MainSettingPath, "LogSearch", "Times8", "");
        private static string LogSearch_Times9 = ini12.INIRead(MainSettingPath, "LogSearch", "Times9", "");
        private static string LogSearch_Display0 = ini12.INIRead(MainSettingPath, "LogSearch", "Display0", "");
        private static string LogSearch_Display1 = ini12.INIRead(MainSettingPath, "LogSearch", "Display1", "");
        private static string LogSearch_Display2 = ini12.INIRead(MainSettingPath, "LogSearch", "Display2", "");
        private static string LogSearch_Display3 = ini12.INIRead(MainSettingPath, "LogSearch", "Display3", "");
        private static string LogSearch_Display4 = ini12.INIRead(MainSettingPath, "LogSearch", "Display4", "");
        private static string LogSearch_Display5 = ini12.INIRead(MainSettingPath, "LogSearch", "Display5", "");
        private static string LogSearch_Display6 = ini12.INIRead(MainSettingPath, "LogSearch", "Display6", "");
        private static string LogSearch_Display7 = ini12.INIRead(MainSettingPath, "LogSearch", "Display7", "");
        private static string LogSearch_Display8 = ini12.INIRead(MainSettingPath, "LogSearch", "Display8", "");
        private static string LogSearch_Display9 = ini12.INIRead(MainSettingPath, "LogSearch", "Display9", "");

        private static string Kline_Checked = ini12.INIRead(MainSettingPath, "Kline", "Checked", "");
        private static string Kline_PortName = ini12.INIRead(MainSettingPath, "Kline", "PortName", "");
        #endregion

        #region -- Mail.ini --
        private static string SendMail_value = ini12.INIRead(MailSettingPath, "Send Mail", "value", "");

        private static string DataInfo_TestCaseNumber = ini12.INIRead(MailSettingPath, "Data Info", "TestCaseNumber", "");
        private static string DataInfo_Result = ini12.INIRead(MailSettingPath, "Data Info", "Result", "");
        private static string DataInfo_NGfrequency = ini12.INIRead(MailSettingPath, "Data Info", "NGfrequency", "");
        private static string DataInfo_CreateTime = ini12.INIRead(MailSettingPath, "Data Info", "CreateTime", "");
        private static string DataInfo_CloseTime = ini12.INIRead(MailSettingPath, "Data Info", "CloseTime", "");
        private static string DataInfo_ProjectNumber = ini12.INIRead(MailSettingPath, "Data Info", "ProjectNumber", "");

        private static string TotalTestTime_value = ini12.INIRead(MailSettingPath, "Total Test Time", "value", "");
        private static string TotalTestTime_value1 = ini12.INIRead(MailSettingPath, "Total Test Time", "value1", "");
        private static string TotalTestTime_value2 = ini12.INIRead(MailSettingPath, "Total Test Time", "value2", "");
        private static string TotalTestTime_value3 = ini12.INIRead(MailSettingPath, "Total Test Time", "value3", "");
        private static string TotalTestTime_value4 = ini12.INIRead(MailSettingPath, "Total Test Time", "value4", "");
        private static string TotalTestTime_value5 = ini12.INIRead(MailSettingPath, "Total Test Time", "value5", "");
        private static string TotalTestTime_HowLong = ini12.INIRead(MailSettingPath, "Total Test Time", "How Long", "");

        private static string TestCase_TestCase1 = ini12.INIRead(MailSettingPath, "Test Case", "TestCase1", "");
        private static string TestCase_TestCase2 = ini12.INIRead(MailSettingPath, "Test Case", "TestCase2", "");
        private static string TestCase_TestCase3 = ini12.INIRead(MailSettingPath, "Test Case", "TestCase3", "");
        private static string TestCase_TestCase4 = ini12.INIRead(MailSettingPath, "Test Case", "TestCase4", "");
        private static string TestCase_TestCase5 = ini12.INIRead(MailSettingPath, "Test Case", "TestCase5", "");

        private static string MailInfo_From = ini12.INIRead(MailSettingPath, "Mail Info", "From", "");
        private static string MailInfo_To = ini12.INIRead(MailSettingPath, "Mail Info", "To", "");
        private static string MailInfo_ProjectName = ini12.INIRead(MailSettingPath, "Mail Info", "ProjectName", "");
        private static string MailInfo_ModelName = ini12.INIRead(MailSettingPath, "Mail Info", "ModelName", "");
        private static string MailInfo_Version = ini12.INIRead(MailSettingPath, "Mail Info", "Version", "");
        private static string MailInfo_Tester = ini12.INIRead(MailSettingPath, "Mail Info", "Tester", "");
        private static string MailInfo_TeamViewerID = ini12.INIRead(MailSettingPath, "Mail Info", "TeamViewerID", "");
        private static string MailInfo_TeamViewerPassWord = ini12.INIRead(MailSettingPath, "Mail Info", "TeamViewerPassWord", "");
        #endregion

        #region -- RC.ini --
        private static string Setting_SelectRcLastTime = ini12.INIRead(RcSettingPath, "Setting", "SelectRcLastTime", "");
        private static string Setting_SelectRcLastTimePath = ini12.INIRead(RcSettingPath, "Setting", "SelectRcLastTimePath", "");
        #endregion
    }

    public class Pub_Setting
    {
        public static string MainSettingPath = Application.StartupPath + "\\Config.ini";
        public static string MailSettingPath = Application.StartupPath + "\\Mail.ini";
        public static string RcSettingPath = Application.StartupPath + "\\RC.ini";

        #region -- Config.ini --
        public static string Device_AutoboxExist = ini12.INIRead(MainSettingPath, "Device", "AutoboxExist", "");
        public static string Device_AutoboxVerson = ini12.INIRead(MainSettingPath, "Device", "AutoboxVerson", "");
        public static string Device_AutoboxPort = ini12.INIRead(MainSettingPath, "Device", "AutoboxPort", "");
        public static string Device_CameraExist = ini12.INIRead(MainSettingPath, "Device", "CameraExist", "");
        public static string Device_RedRatExist = ini12.INIRead(MainSettingPath, "Device", "RedRatExist", "");
        public static string Device_CANbusExist = ini12.INIRead(MainSettingPath, "Device", "CANbusExist", "");
        public static string Device_KlineExist = ini12.INIRead(MainSettingPath, "Device", "KlineExist", "");
        public static string Cmd_DOS = ini12.INIRead(MainSettingPath, "Device", "DOS", "");
        public static string Cmd_RunAfterStartUp = ini12.INIRead(MainSettingPath, "Device", "RunAfterStartUp", "");

        public static string RedRat_Index = ini12.INIRead(MainSettingPath, "RedRat", "RedRatIndex", "");
        public static string RedRat_DBFile = ini12.INIRead(MainSettingPath, "RedRat", "DBFile", "");
        public static string RedRat_Brands = ini12.INIRead(MainSettingPath, "RedRat", "Brands", "");
        public static string RedRat_SerialNumber = ini12.INIRead(MainSettingPath, "RedRat", "SerialNumber", "");

        public static string Camera_VideoIndex = ini12.INIRead(MainSettingPath, "Camera", "VideoIndex", "");
        public static string Camera_VideoNumber = ini12.INIRead(MainSettingPath, "Camera", "VideoNumber", "");
        public static string Camera_VideoName = ini12.INIRead(MainSettingPath, "Camera", "VideoName", "");
        public static string Camera_Resolution = ini12.INIRead(MainSettingPath, "Camera", "Resolution", "");
        public static string Camera_AudioIndex = ini12.INIRead(MainSettingPath, "Camera", "AudioIndex", "");
        public static string Camera_AudioNumber = ini12.INIRead(MainSettingPath, "Camera", "AudioNumber", "");
        public static string Camera_AudioName = ini12.INIRead(MainSettingPath, "Camera", "AudioName", "");

        public static string Comport_Checked = ini12.INIRead(MainSettingPath, "Comport", "Checked", "");
        public static string Comport_PortName = ini12.INIRead(MainSettingPath, "Comport", "PortName", "");
        public static string Comport_VirtualName = ini12.INIRead(MainSettingPath, "Comport", "VirtualName", "");
        public static string Comport_BaudRate = ini12.INIRead(MainSettingPath, "Comport", "BaudRate", "");
        public static string Comport_DataBit = ini12.INIRead(MainSettingPath, "Comport", "DataBit", "");
        public static string Comport_StopBits = ini12.INIRead(MainSettingPath, "Comport", "StopBits", "");

        public static string ExtComport_Checked = ini12.INIRead(MainSettingPath, "ExtComport", "Checked", "");
        public static string ExtComport_PortName = ini12.INIRead(MainSettingPath, "ExtComport", "PortName", "");
        public static string ExtComport_VirtualName = ini12.INIRead(MainSettingPath, "ExtComport", "VirtualName", "");
        public static string ExtComport_BaudRate = ini12.INIRead(MainSettingPath, "ExtComport", "BaudRate", "");
        public static string ExtComport_DataBit = ini12.INIRead(MainSettingPath, "ExtComport", "DataBit", "");
        public static string ExtComport_StopBits = ini12.INIRead(MainSettingPath, "ExtComport", "StopBits", "");

        public static string TriComport_Checked = ini12.INIRead(MainSettingPath, "TriComport", "Checked", "");
        public static string TriComport_PortName = ini12.INIRead(MainSettingPath, "TriComport", "PortName", "");
        public static string TriComport_VirtualName = ini12.INIRead(MainSettingPath, "TriComport", "VirtualName", "");
        public static string TriComport_BaudRate = ini12.INIRead(MainSettingPath, "TriComport", "BaudRate", "");
        public static string TriComport_DataBit = ini12.INIRead(MainSettingPath, "TriComport", "DataBit", "");
        public static string TriComport_StopBits = ini12.INIRead(MainSettingPath, "TriComport", "StopBits", "");

        public static string Record_VideoPath = ini12.INIRead(MainSettingPath, "Record", "VideoPath", "");
        public static string Record_LogPath = ini12.INIRead(MainSettingPath, "Record", "LogPath", "");
        public static string Record_Generator = ini12.INIRead(MainSettingPath, "Record", "Generator", "");
        public static string Record_CompareChoose = ini12.INIRead(MainSettingPath, "Record", "CompareChoose", "");
        public static string Record_CompareDifferent = ini12.INIRead(MainSettingPath, "Record", "CompareDifferent", "");
        public static string Record_EachVideo = ini12.INIRead(MainSettingPath, "Record", "EachVideo", "");
        public static string Record_ImportDB = ini12.INIRead(MainSettingPath, "Record", "ImportDB", "");
        public static string Record_FootprintMode = ini12.INIRead(MainSettingPath, "Record", "Footprint Mode", "");
        public static string Record_CANbusLog = ini12.INIRead(MainSettingPath, "Record", "CANbusLog", "");

        public static string Schedule1_Exist = ini12.INIRead(MainSettingPath, "Schedule1", "Exist", "");
        public static string Schedule1_Loop = ini12.INIRead(MainSettingPath, "Schedule1", "Loop", "");
        public static string Schedule1_OnTimeStart = ini12.INIRead(MainSettingPath, "Schedule1", "OnTimeStart", "");
        public static string Schedule1_Timer = ini12.INIRead(MainSettingPath, "Schedule1", "Timer", "");
        public static string Schedule1_Path = ini12.INIRead(MainSettingPath, "Schedule1", "Path", "");

        public static string Schedule2_Exist = ini12.INIRead(MainSettingPath, "Schedule2", "Exist", "");
        public static string Schedule2_Loop = ini12.INIRead(MainSettingPath, "Schedule2", "Loop", "");
        public static string Schedule2_OnTimeStart = ini12.INIRead(MainSettingPath, "Schedule2", "OnTimeStart", "");
        public static string Schedule2_Timer = ini12.INIRead(MainSettingPath, "Schedule2", "Timer", "");
        public static string Schedule2_Path = ini12.INIRead(MainSettingPath, "Schedule2", "Path", "");

        public static string Schedule3_Exist = ini12.INIRead(MainSettingPath, "Schedule3", "Exist", "");
        public static string Schedule3_Loop = ini12.INIRead(MainSettingPath, "Schedule3", "Loop", "");
        public static string Schedule3_OnTimeStart = ini12.INIRead(MainSettingPath, "Schedule3", "OnTimeStart", "");
        public static string Schedule3_Timer = ini12.INIRead(MainSettingPath, "Schedule3", "Timer", "");
        public static string Schedule3_Path = ini12.INIRead(MainSettingPath, "Schedule3", "Path", "");

        public static string Schedule4_Exist = ini12.INIRead(MainSettingPath, "Schedule4", "Exist", "");
        public static string Schedule4_Loop = ini12.INIRead(MainSettingPath, "Schedule4", "Loop", "");
        public static string Schedule4_OnTimeStart = ini12.INIRead(MainSettingPath, "Schedule4", "OnTimeStart", "");
        public static string Schedule4_Timer = ini12.INIRead(MainSettingPath, "Schedule4", "Timer", "");
        public static string Schedule4_Path = ini12.INIRead(MainSettingPath, "Schedule4", "Path", "");

        public static string Schedule5_Exist = ini12.INIRead(MainSettingPath, "Schedule5", "Exist", "");
        public static string Schedule5_Loop = ini12.INIRead(MainSettingPath, "Schedule5", "Loop", "");
        public static string Schedule5_OnTimeStart = ini12.INIRead(MainSettingPath, "Schedule5", "OnTimeStart", "");
        public static string Schedule5_Timer = ini12.INIRead(MainSettingPath, "Schedule5", "Timer", "");
        public static string Schedule5_Path = ini12.INIRead(MainSettingPath, "Schedule5", "Path", "");

        public static string LogSearch_StartTime = ini12.INIRead(MainSettingPath, "LogSearch", "StartTime", "");
        public static string LogSearch_Comport1 = ini12.INIRead(MainSettingPath, "LogSearch", "Comport1", "");
        public static string LogSearch_Comport2 = ini12.INIRead(MainSettingPath, "LogSearch", "Comport2", "");
        public static string LogSearch_Camerarecord = ini12.INIRead(MainSettingPath, "LogSearch", "Camerarecord", "");
        public static string LogSearch_Camerashot = ini12.INIRead(MainSettingPath, "LogSearch", "Camerashot", "");
        public static string LogSearch_Sendmail = ini12.INIRead(MainSettingPath, "LogSearch", "Sendmail", "");
        public static string LogSearch_Savelog = ini12.INIRead(MainSettingPath, "LogSearch", "Savelog", "");
        public static string LogSearch_Showmessage = ini12.INIRead(MainSettingPath, "LogSearch", "Showmessage", "");
        public static string LogSearch_ACcontrol = ini12.INIRead(MainSettingPath, "LogSearch", "ACcontrol", "");
        public static string LogSearch_Stop = ini12.INIRead(MainSettingPath, "LogSearch", "Stop", "");
        public static string LogSearch_ACOFF = ini12.INIRead(MainSettingPath, "LogSearch", "AC OFF", "");
        public static string LogSearch_Nowvalue = ini12.INIRead(MainSettingPath, "LogSearch", "Nowvalue", "");
        public static string LogSearch_Text0 = ini12.INIRead(MainSettingPath, "LogSearch", "Text0", "");
        public static string LogSearch_Text1 = ini12.INIRead(MainSettingPath, "LogSearch", "Text1", "");
        public static string LogSearch_Text2 = ini12.INIRead(MainSettingPath, "LogSearch", "Text2", "");
        public static string LogSearch_Text3 = ini12.INIRead(MainSettingPath, "LogSearch", "Text3", "");
        public static string LogSearch_Text4 = ini12.INIRead(MainSettingPath, "LogSearch", "Text4", "");
        public static string LogSearch_Text5 = ini12.INIRead(MainSettingPath, "LogSearch", "Text5", "");
        public static string LogSearch_Text6 = ini12.INIRead(MainSettingPath, "LogSearch", "Text6", "");
        public static string LogSearch_Text7 = ini12.INIRead(MainSettingPath, "LogSearch", "Text7", "");
        public static string LogSearch_Text8 = ini12.INIRead(MainSettingPath, "LogSearch", "Text8", "");
        public static string LogSearch_Text9 = ini12.INIRead(MainSettingPath, "LogSearch", "Text9", "");
        public static string LogSearch_Times0 = ini12.INIRead(MainSettingPath, "LogSearch", "Times0", "");
        public static string LogSearch_Times1 = ini12.INIRead(MainSettingPath, "LogSearch", "Times1", "");
        public static string LogSearch_Times2 = ini12.INIRead(MainSettingPath, "LogSearch", "Times2", "");
        public static string LogSearch_Times3 = ini12.INIRead(MainSettingPath, "LogSearch", "Times3", "");
        public static string LogSearch_Times4 = ini12.INIRead(MainSettingPath, "LogSearch", "Times4", "");
        public static string LogSearch_Times5 = ini12.INIRead(MainSettingPath, "LogSearch", "Times5", "");
        public static string LogSearch_Times6 = ini12.INIRead(MainSettingPath, "LogSearch", "Times6", "");
        public static string LogSearch_Times7 = ini12.INIRead(MainSettingPath, "LogSearch", "Times7", "");
        public static string LogSearch_Times8 = ini12.INIRead(MainSettingPath, "LogSearch", "Times8", "");
        public static string LogSearch_Times9 = ini12.INIRead(MainSettingPath, "LogSearch", "Times9", "");
        public static string LogSearch_Display0 = ini12.INIRead(MainSettingPath, "LogSearch", "Display0", "");
        public static string LogSearch_Display1 = ini12.INIRead(MainSettingPath, "LogSearch", "Display1", "");
        public static string LogSearch_Display2 = ini12.INIRead(MainSettingPath, "LogSearch", "Display2", "");
        public static string LogSearch_Display3 = ini12.INIRead(MainSettingPath, "LogSearch", "Display3", "");
        public static string LogSearch_Display4 = ini12.INIRead(MainSettingPath, "LogSearch", "Display4", "");
        public static string LogSearch_Display5 = ini12.INIRead(MainSettingPath, "LogSearch", "Display5", "");
        public static string LogSearch_Display6 = ini12.INIRead(MainSettingPath, "LogSearch", "Display6", "");
        public static string LogSearch_Display7 = ini12.INIRead(MainSettingPath, "LogSearch", "Display7", "");
        public static string LogSearch_Display8 = ini12.INIRead(MainSettingPath, "LogSearch", "Display8", "");
        public static string LogSearch_Display9 = ini12.INIRead(MainSettingPath, "LogSearch", "Display9", "");

        public static string Kline_Checked = ini12.INIRead(MainSettingPath, "Kline", "Checked", "");
        public static string Kline_PortName = ini12.INIRead(MainSettingPath, "Kline", "PortName", "");
        #endregion

        #region -- Mail.ini --
        public static string SendMail_value = ini12.INIRead(MailSettingPath, "Send Mail", "value", "");

        public static string DataInfo_TestCaseNumber = ini12.INIRead(MailSettingPath, "Data Info", "TestCaseNumber", "");
        public static string DataInfo_Result = ini12.INIRead(MailSettingPath, "Data Info", "Result", "");
        public static string DataInfo_NGfrequency = ini12.INIRead(MailSettingPath, "Data Info", "NGfrequency", "");
        public static string DataInfo_CreateTime = ini12.INIRead(MailSettingPath, "Data Info", "CreateTime", "");
        public static string DataInfo_CloseTime = ini12.INIRead(MailSettingPath, "Data Info", "CloseTime", "");
        public static string DataInfo_ProjectNumber = ini12.INIRead(MailSettingPath, "Data Info", "ProjectNumber", "");

        public static string TotalTestTime_value = ini12.INIRead(MailSettingPath, "Total Test Time", "value", "");
        public static string TotalTestTime_value1 = ini12.INIRead(MailSettingPath, "Total Test Time", "value1", "");
        public static string TotalTestTime_value2 = ini12.INIRead(MailSettingPath, "Total Test Time", "value2", "");
        public static string TotalTestTime_value3 = ini12.INIRead(MailSettingPath, "Total Test Time", "value3", "");
        public static string TotalTestTime_value4 = ini12.INIRead(MailSettingPath, "Total Test Time", "value4", "");
        public static string TotalTestTime_value5 = ini12.INIRead(MailSettingPath, "Total Test Time", "value5", "");
        public static string TotalTestTime_HowLong = ini12.INIRead(MailSettingPath, "Total Test Time", "How Long", "");

        public static string TestCase_TestCase1 = ini12.INIRead(MailSettingPath, "Test Case", "TestCase1", "");
        public static string TestCase_TestCase2 = ini12.INIRead(MailSettingPath, "Test Case", "TestCase2", "");
        public static string TestCase_TestCase3 = ini12.INIRead(MailSettingPath, "Test Case", "TestCase3", "");
        public static string TestCase_TestCase4 = ini12.INIRead(MailSettingPath, "Test Case", "TestCase4", "");
        public static string TestCase_TestCase5 = ini12.INIRead(MailSettingPath, "Test Case", "TestCase5", "");

        public static string MailInfo_From = ini12.INIRead(MailSettingPath, "Mail Info", "From", "");
        public static string MailInfo_To = ini12.INIRead(MailSettingPath, "Mail Info", "To", "");
        public static string MailInfo_ProjectName = ini12.INIRead(MailSettingPath, "Mail Info", "ProjectName", "");
        public static string MailInfo_ModelName = ini12.INIRead(MailSettingPath, "Mail Info", "ModelName", "");
        public static string MailInfo_Version = ini12.INIRead(MailSettingPath, "Mail Info", "Version", "");
        public static string MailInfo_Tester = ini12.INIRead(MailSettingPath, "Mail Info", "Tester", "");
        public static string MailInfo_TeamViewerID = ini12.INIRead(MailSettingPath, "Mail Info", "TeamViewerID", "");
        public static string MailInfo_TeamViewerPassWord = ini12.INIRead(MailSettingPath, "Mail Info", "TeamViewerPassWord", "");
        #endregion

        #region -- RC.ini --
        public static string Setting_SelectRcLastTime = ini12.INIRead(RcSettingPath, "Setting", "SelectRcLastTime", "");
        public static string Setting_SelectRcLastTimePath = ini12.INIRead(RcSettingPath, "Setting", "SelectRcLastTimePath", "");
        #endregion


        public void Config_Save()
        {
            string MainSettingPath = Application.StartupPath + "\\Config.ini";
            string MailSettingPath = Application.StartupPath + "\\Mail.ini";
            string RcSettingPath = Application.StartupPath + "\\RC.ini";

            #region -- Config.ini --
            ini12.INIWrite(MainSettingPath, "Device", "AutoboxExist", Device_AutoboxExist);
            ini12.INIWrite(MainSettingPath, "Device", "AutoboxVerson", Device_AutoboxVerson);
            ini12.INIWrite(MainSettingPath, "Device", "AutoboxPort", Device_AutoboxPort);
            ini12.INIWrite(MainSettingPath, "Device", "CameraExist", Device_CameraExist);
            ini12.INIWrite(MainSettingPath, "Device", "RedRatExist", Device_RedRatExist);
            ini12.INIWrite(MainSettingPath, "Device", "CANbusExist", Device_CANbusExist);
            ini12.INIWrite(MainSettingPath, "Device", "KlineExist", Device_KlineExist);
            ini12.INIWrite(MainSettingPath, "Device", "DOS", Cmd_DOS);
            ini12.INIWrite(MainSettingPath, "Device", "RunAfterStartUp", Cmd_RunAfterStartUp);

            ini12.INIWrite(MainSettingPath, "RedRat", "RedRatIndex", RedRat_Index);
            ini12.INIWrite(MainSettingPath, "RedRat", "DBFile", RedRat_DBFile);
            ini12.INIWrite(MainSettingPath, "RedRat", "Brands", RedRat_Brands);
            ini12.INIWrite(MainSettingPath, "RedRat", "SerialNumber", RedRat_SerialNumber);

            ini12.INIWrite(MainSettingPath, "Camera", "VideoIndex", Camera_VideoIndex);
            ini12.INIWrite(MainSettingPath, "Camera", "VideoNumber", Camera_VideoNumber);
            ini12.INIWrite(MainSettingPath, "Camera", "VideoName", Camera_VideoName);
            ini12.INIWrite(MainSettingPath, "Camera", "Resolution", Camera_Resolution);
            ini12.INIWrite(MainSettingPath, "Camera", "AudioIndex", Camera_AudioIndex);
            ini12.INIWrite(MainSettingPath, "Camera", "AudioNumber", Camera_AudioNumber);
            ini12.INIWrite(MainSettingPath, "Camera", "AudioName", Camera_AudioName);

            ini12.INIWrite(MainSettingPath, "Comport", "Checked", Comport_Checked);
            ini12.INIWrite(MainSettingPath, "Comport", "PortName", Comport_PortName);
            ini12.INIWrite(MainSettingPath, "Comport", "VirtualName", Comport_VirtualName);
            ini12.INIWrite(MainSettingPath, "Comport", "BaudRate", Comport_BaudRate);
            ini12.INIWrite(MainSettingPath, "Comport", "DataBit", Comport_DataBit);
            ini12.INIWrite(MainSettingPath, "Comport", "StopBits", Comport_StopBits);

            ini12.INIWrite(MainSettingPath, "ExtComport", "Checked", ExtComport_Checked);
            ini12.INIWrite(MainSettingPath, "ExtComport", "PortName", ExtComport_PortName);
            ini12.INIWrite(MainSettingPath, "ExtComport", "VirtualName", ExtComport_VirtualName);
            ini12.INIWrite(MainSettingPath, "ExtComport", "BaudRate", ExtComport_BaudRate);
            ini12.INIWrite(MainSettingPath, "ExtComport", "DataBit", ExtComport_DataBit);
            ini12.INIWrite(MainSettingPath, "ExtComport", "StopBits", ExtComport_StopBits);

            ini12.INIWrite(MainSettingPath, "TriComport", "Checked", TriComport_Checked);
            ini12.INIWrite(MainSettingPath, "TriComport", "PortName", TriComport_PortName);
            ini12.INIWrite(MainSettingPath, "TriComport", "VirtualName", TriComport_VirtualName);
            ini12.INIWrite(MainSettingPath, "TriComport", "BaudRate", TriComport_BaudRate);
            ini12.INIWrite(MainSettingPath, "TriComport", "DataBit", TriComport_DataBit);
            ini12.INIWrite(MainSettingPath, "TriComport", "StopBits", TriComport_StopBits);

            ini12.INIWrite(MainSettingPath, "Record", "VideoPath", Record_VideoPath);
            ini12.INIWrite(MainSettingPath, "Record", "LogPath", Record_LogPath);
            ini12.INIWrite(MainSettingPath, "Record", "Generator", Record_Generator);
            ini12.INIWrite(MainSettingPath, "Record", "CompareChoose", Record_CompareChoose);
            ini12.INIWrite(MainSettingPath, "Record", "CompareDifferent", Record_CompareDifferent);
            ini12.INIWrite(MainSettingPath, "Record", "EachVideo", Record_EachVideo);
            ini12.INIWrite(MainSettingPath, "Record", "ImportDB", Record_ImportDB);
            ini12.INIWrite(MainSettingPath, "Record", "Footprint Mode", Record_FootprintMode);
            ini12.INIWrite(MainSettingPath, "Record", "CANbusLog", Record_CANbusLog);

            ini12.INIWrite(MainSettingPath, "Schedule1", "Exist", Schedule1_Exist);
            ini12.INIWrite(MainSettingPath, "Schedule1", "Loop", Schedule1_Loop);
            ini12.INIWrite(MainSettingPath, "Schedule1", "OnTimeStart", Schedule1_OnTimeStart);
            ini12.INIWrite(MainSettingPath, "Schedule1", "Timer", Schedule1_Timer);
            ini12.INIWrite(MainSettingPath, "Schedule1", "Path", Schedule1_Path);

            ini12.INIWrite(MainSettingPath, "Schedule2", "Exist", Schedule2_Exist);
            ini12.INIWrite(MainSettingPath, "Schedule2", "Loop", Schedule2_Loop);
            ini12.INIWrite(MainSettingPath, "Schedule2", "OnTimeStart", Schedule2_OnTimeStart);
            ini12.INIWrite(MainSettingPath, "Schedule2", "Timer", Schedule2_Timer);
            ini12.INIWrite(MainSettingPath, "Schedule2", "Path", Schedule2_Path);

            ini12.INIWrite(MainSettingPath, "Schedule3", "Exist", Schedule3_Exist);
            ini12.INIWrite(MainSettingPath, "Schedule3", "Loop", Schedule3_Loop);
            ini12.INIWrite(MainSettingPath, "Schedule3", "OnTimeStart", Schedule3_OnTimeStart);
            ini12.INIWrite(MainSettingPath, "Schedule3", "Timer", Schedule3_Timer);
            ini12.INIWrite(MainSettingPath, "Schedule3", "Path", Schedule3_Path);

            ini12.INIWrite(MainSettingPath, "Schedule4", "Exist", Schedule4_Exist);
            ini12.INIWrite(MainSettingPath, "Schedule4", "Loop", Schedule4_Loop);
            ini12.INIWrite(MainSettingPath, "Schedule4", "OnTimeStart", Schedule4_OnTimeStart);
            ini12.INIWrite(MainSettingPath, "Schedule4", "Timer", Schedule4_Timer);
            ini12.INIWrite(MainSettingPath, "Schedule4", "Path", Schedule4_Path);

            ini12.INIWrite(MainSettingPath, "Schedule5", "Exist", Schedule5_Exist);
            ini12.INIWrite(MainSettingPath, "Schedule5", "Loop", Schedule5_Loop);
            ini12.INIWrite(MainSettingPath, "Schedule5", "OnTimeStart", Schedule5_OnTimeStart);
            ini12.INIWrite(MainSettingPath, "Schedule5", "Timer", Schedule5_Timer);
            ini12.INIWrite(MainSettingPath, "Schedule5", "Path", Schedule5_Path);

            ini12.INIWrite(MainSettingPath, "LogSearch", "StartTime", LogSearch_StartTime);
            ini12.INIWrite(MainSettingPath, "LogSearch", "Comport1", LogSearch_Comport1);
            ini12.INIWrite(MainSettingPath, "LogSearch", "Comport2", LogSearch_Comport2);
            ini12.INIWrite(MainSettingPath, "LogSearch", "Camerarecord", LogSearch_Camerarecord);
            ini12.INIWrite(MainSettingPath, "LogSearch", "Camerashot", LogSearch_Camerashot);
            ini12.INIWrite(MainSettingPath, "LogSearch", "Sendmail", LogSearch_Sendmail);
            ini12.INIWrite(MainSettingPath, "LogSearch", "Savelog", LogSearch_Savelog);
            ini12.INIWrite(MainSettingPath, "LogSearch", "Showmessage", LogSearch_Showmessage);
            ini12.INIWrite(MainSettingPath, "LogSearch", "ACcontrol", LogSearch_ACcontrol);
            ini12.INIWrite(MainSettingPath, "LogSearch", "Stop", LogSearch_Stop);
            ini12.INIWrite(MainSettingPath, "LogSearch", "AC OFF", LogSearch_ACOFF);
            ini12.INIWrite(MainSettingPath, "LogSearch", "Nowvalue", LogSearch_Nowvalue);
            ini12.INIWrite(MainSettingPath, "LogSearch", "Text0", LogSearch_Text0);
            ini12.INIWrite(MainSettingPath, "LogSearch", "Text1", LogSearch_Text1);
            ini12.INIWrite(MainSettingPath, "LogSearch", "Text2", LogSearch_Text2);
            ini12.INIWrite(MainSettingPath, "LogSearch", "Text3", LogSearch_Text3);
            ini12.INIWrite(MainSettingPath, "LogSearch", "Text4", LogSearch_Text4);
            ini12.INIWrite(MainSettingPath, "LogSearch", "Text5", LogSearch_Text5);
            ini12.INIWrite(MainSettingPath, "LogSearch", "Text6", LogSearch_Text6);
            ini12.INIWrite(MainSettingPath, "LogSearch", "Text7", LogSearch_Text7);
            ini12.INIWrite(MainSettingPath, "LogSearch", "Text8", LogSearch_Text8);
            ini12.INIWrite(MainSettingPath, "LogSearch", "Text9", LogSearch_Text9);
            ini12.INIWrite(MainSettingPath, "LogSearch", "Times0", LogSearch_Times0);
            ini12.INIWrite(MainSettingPath, "LogSearch", "Times1", LogSearch_Times1);
            ini12.INIWrite(MainSettingPath, "LogSearch", "Times2", LogSearch_Times2);
            ini12.INIWrite(MainSettingPath, "LogSearch", "Times3", LogSearch_Times3);
            ini12.INIWrite(MainSettingPath, "LogSearch", "Times4", LogSearch_Times4);
            ini12.INIWrite(MainSettingPath, "LogSearch", "Times5", LogSearch_Times5);
            ini12.INIWrite(MainSettingPath, "LogSearch", "Times6", LogSearch_Times6);
            ini12.INIWrite(MainSettingPath, "LogSearch", "Times7", LogSearch_Times7);
            ini12.INIWrite(MainSettingPath, "LogSearch", "Times8", LogSearch_Times8);
            ini12.INIWrite(MainSettingPath, "LogSearch", "Times9", LogSearch_Times9);
            ini12.INIWrite(MainSettingPath, "LogSearch", "Display0", LogSearch_Display0);
            ini12.INIWrite(MainSettingPath, "LogSearch", "Display1", LogSearch_Display1);
            ini12.INIWrite(MainSettingPath, "LogSearch", "Display2", LogSearch_Display2);
            ini12.INIWrite(MainSettingPath, "LogSearch", "Display3", LogSearch_Display3);
            ini12.INIWrite(MainSettingPath, "LogSearch", "Display4", LogSearch_Display4);
            ini12.INIWrite(MainSettingPath, "LogSearch", "Display5", LogSearch_Display5);
            ini12.INIWrite(MainSettingPath, "LogSearch", "Display6", LogSearch_Display6);
            ini12.INIWrite(MainSettingPath, "LogSearch", "Display7", LogSearch_Display7);
            ini12.INIWrite(MainSettingPath, "LogSearch", "Display8", LogSearch_Display8);
            ini12.INIWrite(MainSettingPath, "LogSearch", "Display9", LogSearch_Display9);

            ini12.INIWrite(MainSettingPath, "Kline", "Checked", Kline_Checked);
            ini12.INIWrite(MainSettingPath, "Kline", "PortName", Kline_PortName);
            #endregion

            #region -- Mail.ini --
            ini12.INIWrite(MailSettingPath, "Send Mail", "value", SendMail_value);

            ini12.INIWrite(MailSettingPath, "Data Info", "TestCaseNumber", DataInfo_TestCaseNumber);
            ini12.INIWrite(MailSettingPath, "Data Info", "Result", DataInfo_Result);
            ini12.INIWrite(MailSettingPath, "Data Info", "NGfrequency", DataInfo_NGfrequency);
            ini12.INIWrite(MailSettingPath, "Data Info", "CreateTime", DataInfo_CreateTime);
            ini12.INIWrite(MailSettingPath, "Data Info", "CloseTime", DataInfo_CloseTime);
            ini12.INIWrite(MailSettingPath, "Data Info", "ProjectNumber", DataInfo_ProjectNumber);

            ini12.INIWrite(MailSettingPath, "Total Test Time", "value", TotalTestTime_value);
            ini12.INIWrite(MailSettingPath, "Total Test Time", "value1", TotalTestTime_value1);
            ini12.INIWrite(MailSettingPath, "Total Test Time", "value2", TotalTestTime_value2);
            ini12.INIWrite(MailSettingPath, "Total Test Time", "value3", TotalTestTime_value3);
            ini12.INIWrite(MailSettingPath, "Total Test Time", "value4", TotalTestTime_value4);
            ini12.INIWrite(MailSettingPath, "Total Test Time", "value5", TotalTestTime_value5);
            ini12.INIWrite(MailSettingPath, "Total Test Time", "How Long", TotalTestTime_HowLong);

            ini12.INIWrite(MailSettingPath, "Test Case", "TestCase1", TestCase_TestCase1);
            ini12.INIWrite(MailSettingPath, "Test Case", "TestCase2", TestCase_TestCase2);
            ini12.INIWrite(MailSettingPath, "Test Case", "TestCase3", TestCase_TestCase3);
            ini12.INIWrite(MailSettingPath, "Test Case", "TestCase4", TestCase_TestCase4);
            ini12.INIWrite(MailSettingPath, "Test Case", "TestCase5", TestCase_TestCase5);

            ini12.INIWrite(MailSettingPath, "Mail Info", "From", MailInfo_From);
            ini12.INIWrite(MailSettingPath, "Mail Info", "To", MailInfo_To);
            ini12.INIWrite(MailSettingPath, "Mail Info", "ProjectName", MailInfo_ProjectName);
            ini12.INIWrite(MailSettingPath, "Mail Info", "ModelName", MailInfo_ModelName);
            ini12.INIWrite(MailSettingPath, "Mail Info", "Version", MailInfo_Version);
            ini12.INIWrite(MailSettingPath, "Mail Info", "Tester", MailInfo_Tester);
            ini12.INIWrite(MailSettingPath, "Mail Info", "TeamViewerID", MailInfo_TeamViewerID);
            ini12.INIWrite(MailSettingPath, "Mail Info", "TeamViewerPassWord", MailInfo_TeamViewerPassWord);
            #endregion

            #region -- RC.ini --
            ini12.INIWrite(RcSettingPath, "Setting", "SelectRcLastTime", Setting_SelectRcLastTime);
            ini12.INIWrite(RcSettingPath, "Setting", "SelectRcLastTimePath", Setting_SelectRcLastTimePath);
            #endregion
        }
    } 
}

