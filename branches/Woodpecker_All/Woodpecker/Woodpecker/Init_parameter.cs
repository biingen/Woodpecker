using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public struct Config
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
    public string SerialPort1_Checked;
    public string SerialPort1_PortName;
    public string SerialPort1_BaudRate;
    public string SerialPort1_DataBit;
    public string SerialPort1_StopBits;
    public string SerialPort2_Checked;
    public string SerialPort2_PortName;
    public string SerialPort2_BaudRate;
    public string SerialPort2_DataBit;
    public string SerialPort2_StopBits;
    public string SerialPort3_Checked;
    public string SerialPort3_PortName;
    public string SerialPort3_BaudRate;
    public string SerialPort3_DataBit;
    public string SerialPort3_StopBits;
    public string SerialPort4_Checked;
    public string SerialPort4_PortName;
    public string SerialPort4_BaudRate;
    public string SerialPort4_DataBit;
    public string SerialPort4_StopBits;
    public string SerialPort5_Checked;
    public string SerialPort5_PortName;
    public string SerialPort5_BaudRate;
    public string SerialPort5_DataBit;
    public string SerialPort5_StopBits;
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
    public string LogSearch_Comport1;
    public string LogSearch_Comport2;
    public string LogSearch_Comport3;
    public string LogSearch_Comport4;
    public string LogSearch_Comport5;
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
}

namespace Woodpecker
{
    class Init_Parameter
    {
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
