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

namespace Woodpecker
{
    public partial class Extra_Commander : Form
    {
        private Config config_parameter = new Config();
        string MainSettingPath = Application.StartupPath + "\\Config.ini";
        string MailPath = Application.StartupPath + "\\Mail.ini";

        public Extra_Commander(string value)
        {
            InitializeComponent();
            Environment_initial();
        }

        private void Extra_Commander_Load(object sender, EventArgs e)
        {

        }

        public void Environment_initial()
        {
            config_parameter.Device_AutoboxExist = Convert.ToUInt16(ini12.INIRead(MainSettingPath, "Device", "AutoboxExist", ""));
            config_parameter.Device_AutoboxVerson = Convert.ToUInt16(ini12.INIRead(MainSettingPath, "Device", "AutoboxVerson", ""));
            config_parameter.Device_AutoboxPort = ini12.INIRead(MainSettingPath, "Device", "AutoboxPort", "");
            config_parameter.Device_DOS = ini12.INIRead(MainSettingPath, "Device", "DOS", "");
            config_parameter.Device_RunAfterStartUp = Convert.ToUInt16(ini12.INIRead(MainSettingPath, "Device", "AutoboxExist", ""));
            config_parameter.Device_CA310Exist = Convert.ToUInt16(ini12.INIRead(MainSettingPath, "Device", "AutoboxExist", ""));
            config_parameter.RedRat_Exist = Convert.ToUInt16(ini12.INIRead(MainSettingPath, "Device", "AutoboxExist", ""));
            config_parameter.RedRat_RedRatIndex = Convert.ToUInt16(ini12.INIRead(MainSettingPath, "Device", "AutoboxExist", ""));
            config_parameter.RedRat_DBFile = ini12.INIRead(MainSettingPath, "Device", "AutoboxExist", "");
            config_parameter.RedRat_Brands = ini12.INIRead(MainSettingPath, "Device", "AutoboxExist", "");
            config_parameter.RedRat_SerialNumber = ini12.INIRead(MainSettingPath, "Device", "AutoboxExist", "");
            config_parameter.Camera_Exist = Convert.ToUInt16(ini12.INIRead(MainSettingPath, "Device", "AutoboxExist", ""));
            config_parameter.Camera_VideoIndex = Convert.ToUInt16(ini12.INIRead(MainSettingPath, "Device", "AutoboxExist", ""));
            config_parameter.Camera_VideoNumber = Convert.ToUInt16(ini12.INIRead(MainSettingPath, "Device", "AutoboxExist", ""));
            config_parameter.Camera_VideoName = Convert.ToUInt16(ini12.INIRead(MainSettingPath, "Device", "AutoboxExist", ""));
            config_parameter.Camera_AudioIndex = Convert.ToUInt16(ini12.INIRead(MainSettingPath, "Device", "AutoboxExist", ""));
            config_parameter.Camera_AudioNumber = Convert.ToUInt16(ini12.INIRead(MainSettingPath, "Device", "AutoboxExist", ""));
            config_parameter.Camera_AudioName = Convert.ToUInt16(ini12.INIRead(MainSettingPath, "Device", "AutoboxExist", ""));
            config_parameter.SerialPort1_Checked = Convert.ToUInt16(ini12.INIRead(MainSettingPath, "Device", "AutoboxExist", ""));
            config_parameter.SerialPort1_PortName = ini12.INIRead(MainSettingPath, "Device", "AutoboxExist", "");
            config_parameter.SerialPort1_BaudRate = Convert.ToUInt16(ini12.INIRead(MainSettingPath, "Device", "AutoboxExist", ""));
            config_parameter.SerialPort1_DataBit = ini12.INIRead(MainSettingPath, "Device", "AutoboxExist", "");
            config_parameter.SerialPort1_StopBits = ini12.INIRead(MainSettingPath, "Device", "AutoboxExist", "");
            config_parameter.SerialPort2_Checked = Convert.ToUInt16(ini12.INIRead(MainSettingPath, "Device", "AutoboxExist", ""));
            config_parameter.SerialPort2_PortName = ini12.INIRead(MainSettingPath, "Device", "AutoboxExist", "");
            config_parameter.SerialPort2_BaudRate = Convert.ToUInt16(ini12.INIRead(MainSettingPath, "Device", "AutoboxExist", ""));
            config_parameter.SerialPort2_DataBit = ini12.INIRead(MainSettingPath, "Device", "AutoboxExist", "");
            config_parameter.SerialPort2_StopBits = ini12.INIRead(MainSettingPath, "Device", "AutoboxExist", "");
            config_parameter.SerialPort3_Checked = Convert.ToUInt16(ini12.INIRead(MainSettingPath, "Device", "AutoboxExist", ""));
            config_parameter.SerialPort3_PortName = ini12.INIRead(MainSettingPath, "Device", "AutoboxExist", "");
            config_parameter.SerialPort3_BaudRate = Convert.ToUInt16(ini12.INIRead(MainSettingPath, "Device", "AutoboxExist", ""));
            config_parameter.SerialPort3_DataBit = ini12.INIRead(MainSettingPath, "Device", "AutoboxExist", "");
            config_parameter.SerialPort3_StopBits = ini12.INIRead(MainSettingPath, "Device", "AutoboxExist", "");
            config_parameter.SerialPort4_Checked = Convert.ToUInt16(ini12.INIRead(MainSettingPath, "Device", "AutoboxExist", ""));
            config_parameter.SerialPort4_PortName = ini12.INIRead(MainSettingPath, "Device", "AutoboxExist", "");
            config_parameter.SerialPort4_BaudRate = Convert.ToUInt16(ini12.INIRead(MainSettingPath, "Device", "AutoboxExist", ""));
            config_parameter.SerialPort4_DataBit = ini12.INIRead(MainSettingPath, "Device", "AutoboxExist", "");
            config_parameter.SerialPort4_StopBits = ini12.INIRead(MainSettingPath, "Device", "AutoboxExist", "");
            config_parameter.SerialPort5_Checked = Convert.ToUInt16(ini12.INIRead(MainSettingPath, "Device", "AutoboxExist", ""));
            config_parameter.SerialPort5_PortName = ini12.INIRead(MainSettingPath, "Device", "AutoboxExist", "");
            config_parameter.SerialPort5_BaudRate = Convert.ToUInt16(ini12.INIRead(MainSettingPath, "Device", "AutoboxExist", ""));
            config_parameter.SerialPort5_DataBit = ini12.INIRead(MainSettingPath, "Device", "AutoboxExist", "");
            config_parameter.SerialPort5_StopBits = ini12.INIRead(MainSettingPath, "Device", "AutoboxExist", "");
            config_parameter.Record_VideoPath = ini12.INIRead(MainSettingPath, "Device", "AutoboxExist", "");
            config_parameter.Record_LogPath = ini12.INIRead(MainSettingPath, "Device", "AutoboxExist", "");
            config_parameter.Record_Generator = ini12.INIRead(MainSettingPath, "Device", "AutoboxExist", "");
            config_parameter.Record_CompareChoose = Convert.ToUInt16(ini12.INIRead(MainSettingPath, "Device", "AutoboxExist", ""));
            config_parameter.Record_CompareDifferent = Convert.ToUInt16(ini12.INIRead(MainSettingPath, "Device", "AutoboxExist", ""));
            config_parameter.Record_EachVideo = Convert.ToUInt16(ini12.INIRead(MainSettingPath, "Device", "AutoboxExist", ""));
            config_parameter.Record_ImportDB = Convert.ToUInt16(ini12.INIRead(MainSettingPath, "Device", "AutoboxExist", ""));
            config_parameter.Record_FootprintMode = Convert.ToUInt16(ini12.INIRead(MainSettingPath, "Device", "AutoboxExist", ""));
            config_parameter.Schedule1_Exist = Convert.ToUInt16(ini12.INIRead(MainSettingPath, "Device", "AutoboxExist", ""));
            config_parameter.Schedule1_Loop = Convert.ToUInt16(ini12.INIRead(MainSettingPath, "Device", "AutoboxExist", ""));
            config_parameter.Schedule1_OnTimeStart = Convert.ToUInt16(ini12.INIRead(MainSettingPath, "Device", "AutoboxExist", ""));
            config_parameter.Schedule1_Timer = ini12.INIRead(MainSettingPath, "Device", "AutoboxExist", "");
            config_parameter.Schedule1_Path = ini12.INIRead(MainSettingPath, "Device", "AutoboxExist", "");
            config_parameter.Schedule2_Exist = Convert.ToUInt16(ini12.INIRead(MainSettingPath, "Device", "AutoboxExist", ""));
            config_parameter.Schedule2_Loop = Convert.ToUInt16(ini12.INIRead(MainSettingPath, "Device", "AutoboxExist", ""));
            config_parameter.Schedule2_OnTimeStart = Convert.ToUInt16(ini12.INIRead(MainSettingPath, "Device", "AutoboxExist", ""));
            config_parameter.Schedule2_Timer = ini12.INIRead(MainSettingPath, "Device", "AutoboxExist", "");
            config_parameter.Schedule2_Path = ini12.INIRead(MainSettingPath, "Device", "AutoboxExist", "");
            config_parameter.Schedule3_Exist = Convert.ToUInt16(ini12.INIRead(MainSettingPath, "Device", "AutoboxExist", ""));
            config_parameter.Schedule3_Loop = Convert.ToUInt16(ini12.INIRead(MainSettingPath, "Device", "AutoboxExist", ""));
            config_parameter.Schedule3_OnTimeStart = Convert.ToUInt16(ini12.INIRead(MainSettingPath, "Device", "AutoboxExist", ""));
            config_parameter.Schedule3_Timer = ini12.INIRead(MainSettingPath, "Device", "AutoboxExist", "");
            config_parameter.Schedule3_Path = ini12.INIRead(MainSettingPath, "Device", "AutoboxExist", "");
            config_parameter.Schedule4_Exist = Convert.ToUInt16(ini12.INIRead(MainSettingPath, "Device", "AutoboxExist", ""));
            config_parameter.Schedule4_Loop = Convert.ToUInt16(ini12.INIRead(MainSettingPath, "Device", "AutoboxExist", ""));
            config_parameter.Schedule4_OnTimeStart = Convert.ToUInt16(ini12.INIRead(MainSettingPath, "Device", "AutoboxExist", ""));
            config_parameter.Schedule4_Timer = ini12.INIRead(MainSettingPath, "Device", "AutoboxExist", "");
            config_parameter.Schedule4_Path = ini12.INIRead(MainSettingPath, "Device", "AutoboxExist", "");
            config_parameter.Schedule5_Exist = Convert.ToUInt16(ini12.INIRead(MainSettingPath, "Device", "AutoboxExist", ""));
            config_parameter.Schedule5_Loop = Convert.ToUInt16(ini12.INIRead(MainSettingPath, "Device", "AutoboxExist", ""));
            config_parameter.Schedule5_OnTimeStart = Convert.ToUInt16(ini12.INIRead(MainSettingPath, "Device", "AutoboxExist", ""));
            config_parameter.Schedule5_Timer = ini12.INIRead(MainSettingPath, "Device", "AutoboxExist", "");
            config_parameter.Schedule5_Path = ini12.INIRead(MainSettingPath, "Device", "AutoboxExist", "");
            config_parameter.Kline_Exist = Convert.ToUInt16(ini12.INIRead(MainSettingPath, "Device", "AutoboxExist", ""));
            config_parameter.Kline_PortName = ini12.INIRead(MainSettingPath, "Device", "AutoboxExist", "");
            config_parameter.Canbus_Exist = Convert.ToUInt16(ini12.INIRead(MainSettingPath, "Device", "AutoboxExist", ""));
            config_parameter.Canbus_Log = Convert.ToUInt16(ini12.INIRead(MainSettingPath, "Device", "AutoboxExist", ""));
            config_parameter.Canbus_BaudRate = ini12.INIRead(MainSettingPath, "Device", "AutoboxExist", "");
            config_parameter.Canbus_DevIndex = Convert.ToUInt16(ini12.INIRead(MainSettingPath, "Device", "AutoboxExist", ""));
            config_parameter.LogSearch_Comport1 = Convert.ToUInt16(ini12.INIRead(MainSettingPath, "Device", "AutoboxExist", ""));
            config_parameter.LogSearch_Comport2 = Convert.ToUInt16(ini12.INIRead(MainSettingPath, "Device", "AutoboxExist", ""));
            config_parameter.LogSearch_Comport3 = Convert.ToUInt16(ini12.INIRead(MainSettingPath, "Device", "AutoboxExist", ""));
            config_parameter.LogSearch_Comport4 = Convert.ToUInt16(ini12.INIRead(MainSettingPath, "Device", "AutoboxExist", ""));
            config_parameter.LogSearch_Comport5 = Convert.ToUInt16(ini12.INIRead(MainSettingPath, "Device", "AutoboxExist", ""));
            config_parameter.LogSearch_TextNum = Convert.ToUInt16(ini12.INIRead(MainSettingPath, "Device", "AutoboxExist", ""));
            config_parameter.LogSearch_Camerarecord = Convert.ToUInt16(ini12.INIRead(MainSettingPath, "Device", "AutoboxExist", ""));
            config_parameter.LogSearch_Camerashot = Convert.ToUInt16(ini12.INIRead(MainSettingPath, "Device", "AutoboxExist", ""));
            config_parameter.LogSearch_Sendmail = Convert.ToUInt16(ini12.INIRead(MainSettingPath, "Device", "AutoboxExist", ""));
            config_parameter.LogSearch_Savelog = Convert.ToUInt16(ini12.INIRead(MainSettingPath, "Device", "AutoboxExist", ""));
            config_parameter.LogSearch_Showmessage = Convert.ToUInt16(ini12.INIRead(MainSettingPath, "Device", "AutoboxExist", ""));
            config_parameter.LogSearch_ACcontrol = Convert.ToUInt16(ini12.INIRead(MainSettingPath, "Device", "AutoboxExist", ""));
            config_parameter.LogSearch_ACOFF = Convert.ToUInt16(ini12.INIRead(MainSettingPath, "Device", "AutoboxExist", ""));
            config_parameter.LogSearch_Stop = Convert.ToUInt16(ini12.INIRead(MainSettingPath, "Device", "AutoboxExist", ""));
            config_parameter.LogSearch_Nowvalue = Convert.ToUInt16(ini12.INIRead(MainSettingPath, "Device", "AutoboxExist", ""));
            config_parameter.LogSearch_Times0 = Convert.ToUInt16(ini12.INIRead(MainSettingPath, "Device", "AutoboxExist", ""));
            config_parameter.LogSearch_Times1 = Convert.ToUInt16(ini12.INIRead(MainSettingPath, "Device", "AutoboxExist", ""));
            config_parameter.LogSearch_Times2 = Convert.ToUInt16(ini12.INIRead(MainSettingPath, "Device", "AutoboxExist", ""));
            config_parameter.LogSearch_Times3 = Convert.ToUInt16(ini12.INIRead(MainSettingPath, "Device", "AutoboxExist", ""));
            config_parameter.LogSearch_Times4 = Convert.ToUInt16(ini12.INIRead(MainSettingPath, "Device", "AutoboxExist", ""));
            config_parameter.LogSearch_Times5 = Convert.ToUInt16(ini12.INIRead(MainSettingPath, "Device", "AutoboxExist", ""));
            config_parameter.LogSearch_Times6 = Convert.ToUInt16(ini12.INIRead(MainSettingPath, "Device", "AutoboxExist", ""));
            config_parameter.LogSearch_Times7 = Convert.ToUInt16(ini12.INIRead(MainSettingPath, "Device", "AutoboxExist", ""));
            config_parameter.LogSearch_Times8 = Convert.ToUInt16(ini12.INIRead(MainSettingPath, "Device", "AutoboxExist", ""));
            config_parameter.LogSearch_Times9 = Convert.ToUInt16(ini12.INIRead(MainSettingPath, "Device", "AutoboxExist", ""));

            config_parameter.LogSearch_StartTime = ini12.INIRead(MainSettingPath, "Device", "AutoboxExist", "");
            config_parameter.LogSearch_Path = ini12.INIRead(MainSettingPath, "Device", "AutoboxExist", "");
            config_parameter.LogSearch_Text0 = ini12.INIRead(MainSettingPath, "Device", "AutoboxExist", "");
            config_parameter.LogSearch_Text1 = ini12.INIRead(MainSettingPath, "Device", "AutoboxExist", "");
            config_parameter.LogSearch_Text2 = ini12.INIRead(MainSettingPath, "Device", "AutoboxExist", "");
            config_parameter.LogSearch_Text3 = ini12.INIRead(MainSettingPath, "Device", "AutoboxExist", "");
            config_parameter.LogSearch_Text4 = ini12.INIRead(MainSettingPath, "Device", "AutoboxExist", "");
            config_parameter.LogSearch_Text5 = ini12.INIRead(MainSettingPath, "Device", "AutoboxExist", "");
            config_parameter.LogSearch_Text6 = ini12.INIRead(MainSettingPath, "Device", "AutoboxExist", "");
            config_parameter.LogSearch_Text7 = ini12.INIRead(MainSettingPath, "Device", "AutoboxExist", "");
            config_parameter.LogSearch_Text8 = ini12.INIRead(MainSettingPath, "Device", "AutoboxExist", "");
            config_parameter.LogSearch_Text9 = ini12.INIRead(MainSettingPath, "Device", "AutoboxExist", "");
            config_parameter.LogSearch_Display0 = ini12.INIRead(MainSettingPath, "Device", "AutoboxExist", "");
            config_parameter.LogSearch_Display1 = ini12.INIRead(MainSettingPath, "Device", "AutoboxExist", "");
            config_parameter.LogSearch_Display2 = ini12.INIRead(MainSettingPath, "Device", "AutoboxExist", "");
            config_parameter.LogSearch_Display3 = ini12.INIRead(MainSettingPath, "Device", "AutoboxExist", "");
            config_parameter.LogSearch_Display4 = ini12.INIRead(MainSettingPath, "Device", "AutoboxExist", "");
            config_parameter.LogSearch_Display5 = ini12.INIRead(MainSettingPath, "Device", "AutoboxExist", "");
            config_parameter.LogSearch_Display6 = ini12.INIRead(MainSettingPath, "Device", "AutoboxExist", "");
            config_parameter.LogSearch_Display7 = ini12.INIRead(MainSettingPath, "Device", "AutoboxExist", "");
            config_parameter.LogSearch_Display8 = ini12.INIRead(MainSettingPath, "Device", "AutoboxExist", "");
            config_parameter.LogSearch_Display9 = ini12.INIRead(MainSettingPath, "Device", "AutoboxExist", "");
        }
    }
}
