using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public struct Config
{
    public uint Device_AutoboxExist;
    public uint Device_AutoboxVerson;
    public string Device_AutoboxPort;
    public string Device_DOS;
    public uint Device_RunAfterStartUp;
    public uint Device_CA310Exist;
    public uint RedRat_Exist;
    public uint RedRat_RedRatIndex;
    public string RedRat_DBFile;
    public string RedRat_Brands;
    public string RedRat_SerialNumber;
    public uint Camera_Exist;
    public uint Camera_VideoIndex;
    public uint Camera_VideoNumber;
    public uint Camera_VideoName;
    public uint Camera_AudioIndex;
    public uint Camera_AudioNumber;
    public uint Camera_AudioName;
    public uint SerialPort1_Checked;
    public string SerialPort1_PortName;
    public uint SerialPort1_BaudRate;
    public string SerialPort1_DataBit;
    public string SerialPort1_StopBits;
    public uint SerialPort2_Checked;
    public string SerialPort2_PortName;
    public uint SerialPort2_BaudRate;
    public string SerialPort2_DataBit;
    public string SerialPort2_StopBits;
    public uint SerialPort3_Checked;
    public string SerialPort3_PortName;
    public uint SerialPort3_BaudRate;
    public string SerialPort3_DataBit;
    public string SerialPort3_StopBits;
    public uint SerialPort4_Checked;
    public string SerialPort4_PortName;
    public uint SerialPort4_BaudRate;
    public string SerialPort4_DataBit;
    public string SerialPort4_StopBits;
    public uint SerialPort5_Checked;
    public string SerialPort5_PortName;
    public uint SerialPort5_BaudRate;
    public string SerialPort5_DataBit;
    public string SerialPort5_StopBits;
    public string Record_VideoPath;
    public string Record_LogPath;
    public string Record_Generator;
    public uint Record_CompareChoose;
    public uint Record_CompareDifferent;
    public uint Record_EachVideo;
    public uint Record_ImportDB;
    public uint Record_FootprintMode;
    public uint Schedule1_Exist;
    public uint Schedule1_Loop;
    public uint Schedule1_OnTimeStart;
    public string Schedule1_Timer;
    public string Schedule1_Path;
    public uint Schedule2_Exist;
    public uint Schedule2_Loop;
    public uint Schedule2_OnTimeStart;
    public string Schedule2_Timer;
    public string Schedule2_Path;
    public uint Schedule3_Exist;
    public uint Schedule3_Loop;
    public uint Schedule3_OnTimeStart;
    public string Schedule3_Timer;
    public string Schedule3_Path;
    public uint Schedule4_Exist;
    public uint Schedule4_Loop;
    public uint Schedule4_OnTimeStart;
    public string Schedule4_Timer;
    public string Schedule4_Path;
    public uint Schedule5_Exist;
    public uint Schedule5_Loop;
    public uint Schedule5_OnTimeStart;
    public string Schedule5_Timer;
    public string Schedule5_Path;
    public uint Kline_Exist;
    public string Kline_PortName;
    public uint Canbus_Exist;
    public uint Canbus_Log;
    public string Canbus_BaudRate;
    public uint Canbus_DevIndex;
    public uint LogSearch_Comport1;
    public uint LogSearch_Comport2;
    public uint LogSearch_Comport3;
    public uint LogSearch_Comport4;
    public uint LogSearch_Comport5;
    public uint LogSearch_TextNum;
    public uint LogSearch_Camerarecord;
    public uint LogSearch_Camerashot;
    public uint LogSearch_Sendmail;
    public uint LogSearch_Savelog;
    public uint LogSearch_Showmessage;
    public uint LogSearch_ACcontrol;
    public uint LogSearch_ACOFF;
    public uint LogSearch_Stop;
    public uint LogSearch_Nowvalue;
    public uint LogSearch_Times0;
    public uint LogSearch_Times1;
    public uint LogSearch_Times2;
    public uint LogSearch_Times3;
    public uint LogSearch_Times4;
    public uint LogSearch_Times5;
    public uint LogSearch_Times6;
    public uint LogSearch_Times7;
    public uint LogSearch_Times8;
    public uint LogSearch_Times9;

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
    class Init_parameter
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
