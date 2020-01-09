using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Woodpecker
{
    class Init
    {

        public struct Device
        {
            public uint AutoboxExist;
            public uint AutoboxVerson;
            public string AutoboxPort;
            public uint CameraExist;
            public uint RedRatExist;
            public string DOS;
            public uint RunAfterStartUp;
            public uint CANbusExist;
            public uint KlineExist;
            public uint CA310Exist;
        }

        public struct RedRat
        {
            public uint RedRatIndex;
            public string DBFile;
            public string Brands;
            public string SerialNumber;
        }

        public struct Camera
        {
            public uint VideoIndex;
            public uint VideoNumber;
            public uint VideoName;
            public uint AudioIndex;
            public uint AudioNumber;
            public uint AudioName;
        }

        public struct PortA
        {
            public uint Checked;
            public string PortName;
            public uint BaudRate;
            public string DataBit;
            public string StopBits;
        }

        public struct PortB
        {
            public uint Checked;
            public string PortName;
            public uint BaudRate;
            public string DataBit;
            public string StopBits;
        }

        public struct PortC
        {
            public uint Checked;
            public string PortName;
            public uint BaudRate;
            public string DataBit;
            public string StopBits;
        }

        public struct PortD
        {
            public uint Checked;
            public string PortName;
            public uint BaudRate;
            public string DataBit;
            public string StopBits;
        }

        public struct PortE
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

        public struct Schedule1
        {
            public uint Exist;
            public uint Loop;
            public uint OnTimeStart;
            public string Timer;
            public string Path;
        }

        public struct Schedule2
        {
            public uint Exist;
            public uint Loop;
            public uint OnTimeStart;
            public string Timer;
            public string Path;
        }

        public struct Schedule3
        {
            public uint Exist;
            public uint Loop;
            public uint OnTimeStart;
            public string Timer;
            public string Path;
        }

        public struct Schedule4
        {
            public uint Exist;
            public uint Loop;
            public uint OnTimeStart;
            public string Timer;
            public string Path;
        }

        public struct Schedule5
        {
            public uint Exist;
            public uint Loop;
            public uint OnTimeStart;
            public string Timer;
            public string Path;
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

        public struct Kline
        {
            public string PortName;
        }

        public struct Canbus
        {
            public uint Log;
            public string BaudRate;
            public uint DevIndex;
        }

    }
}
