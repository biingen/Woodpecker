using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using jini;
using System.Linq;
using CAN_USB2C_Lib;

namespace Woodpecker
{
    public class CAN_USB2C 
    {

        //static USB_DEVICE_ID m_devtype = 4;//USBCAN2

        UInt32 m_bOpen = 0;
        //UInt32 m_devind = 0;
        //UInt32 m_canind = 0;

        public static int MAX_CAN_OBJ_ARRAY_LEN = 10000;
        string MainSettingPath = GlobalData.StartupPath + "\\Config.ini";

        VCI_CAN_OBJ[] m_recobj = new VCI_CAN_OBJ[MAX_CAN_OBJ_ARRAY_LEN];
        UInt32[] m_arrdevtype = new UInt32[20];

        // COPY - end


        //
        // Function for external use
        //
        public CAN_USB2C() { }
        ~CAN_USB2C() { }

        USB_DEVICE_ID default_devtype = USB_DEVICE_ID.DEV_USBCAN2;
        UInt32 default_devint = 0;
        UInt32 default_canind = 1;
        uint default_AccCode = 0x80000000;
        uint default_AccMask = 0xffffffff;
        byte default_Timing0 = 0x00;
        byte default_Timing1 = 0x1C;
        byte default_Filter = 0x01;
        byte default_Mode = 0x00;
        byte default_RemoteFlag = 0x00;
        byte default_ExternFlag = 0x00;

        USB_CAN_Adaptor can_adaptor = new USB_CAN_Adaptor();

        public uint Connect()
        {
            uint connection_status = ~(1U);
            if (m_bOpen == 0)
            {
                if (ini12.INIRead(MainSettingPath, "Canbus", "DevIndex", "") != "")
                    default_devint = Convert.ToUInt32(ini12.INIRead(MainSettingPath, "Canbus", "DevIndex", ""));
                //m_devtype = default_devtype;
                //m_devind = default_devint;
                can_adaptor.Config_CAN_Device(default_devtype, default_devint);
                //m_canind = default_canind; 

                try
                {
                    //connection_status = VCI_OpenDevice(m_devtype, m_devind, 0);
                    connection_status = can_adaptor.OpenDevice();
                }
                catch (DllNotFoundException Ex)
                {
                    Console.WriteLine(Ex.ToString());
                    MessageBox.Show("Please install CAN-bus driver.", "CAN-bus Error!");
                }
                if (connection_status == 1)
                {
                    string baudrate = ini12.INIRead(MainSettingPath, "Canbus", "BaudRate", "");
                    switch (baudrate)
                    {
                        case "10 Kbps":
                            default_Timing0 = 0x31;
                            default_Timing1 = 0x1C;
                            break;
                        case "20 Kbps":
                            default_Timing0 = 0x18;
                            default_Timing1 = 0x1C;
                            break;
                        case "40 Kbps":
                            default_Timing0 = 0x87;
                            default_Timing1 = 0xFF;
                            break;
                        case "50 Kbps":
                            default_Timing0 = 0x09;
                            default_Timing1 = 0x1C;
                            break;
                        case "80 Kbps":
                            default_Timing0 = 0x83;
                            default_Timing1 = 0xFF;
                            break;
                        case "100 Kbps":
                            default_Timing0 = 0x04;
                            default_Timing1 = 0x1C;
                            break;
                        case "125 Kbps":
                            default_Timing0 = 0x03;
                            default_Timing1 = 0x1C;
                            break;
                        case "200 Kbps":
                            default_Timing0 = 0x81;
                            default_Timing1 = 0xFA;
                            break;
                        case "250 Kbps":
                            default_Timing0 = 0x01;
                            default_Timing1 = 0x1C;
                            break;
                        case "400 Kbps":
                            default_Timing0 = 0x80;
                            default_Timing1 = 0xFA;
                            break;
                        case "500 Kbps":
                            default_Timing0 = 0x00;
                            default_Timing1 = 0x1C;
                            break;
                        case "666 Kbps":
                            default_Timing0 = 0x80;
                            default_Timing1 = 0xB6;
                            break;
                        case "800 Kbps":
                            default_Timing0 = 0x00;
                            default_Timing1 = 0x16;
                            break;
                        case "1000 Kbps":
                            default_Timing0 = 0x00;
                            default_Timing1 = 0x14;
                            break;
                        case "33.33 Kbps":
                            default_Timing0 = 0x09;
                            default_Timing1 = 0x6F;
                            break;
                        case "66.66 Kbps":
                            default_Timing0 = 0x04;
                            default_Timing1 = 0x6F;
                            break;
                        case "83.33 Kbps":
                            default_Timing0 = 0x03;
                            default_Timing1 = 0x6F;
                            break;
                        default:
                            Console.WriteLine("Default case");
                            break;
                    }
                    //VCI_INIT_CONFIG config = new VCI_INIT_CONFIG();
                    //config.AccCode = default_AccCode;
                    //config.AccMask = default_AccMask;
                    //config.Timing0 = default_Timing0;
                    //config.Timing1 = default_Timing1;
                    //config.Filter = default_Filter;
                    //config.Mode = default_Mode;
                    can_adaptor.Config_CAN_Param(default_AccCode, default_AccMask, default_Timing0, default_Timing1, default_Filter, default_Mode);
                    //connection_status = VCI_InitCAN(m_devtype, m_devind, m_canind, ref config);
                    connection_status = can_adaptor.InitCAN(default_canind);
                    if (connection_status == 1)
                    {
                        m_bOpen = 1;
                    }
                    else
                    {
                        Disconnect();
                        connection_status = 0x03;
                    }
                }
            }
            else
            {
                m_bOpen = 0;
                connection_status = 0x1000;
            }
            return connection_status;
        }

        public UInt32 ReceiveData()
        {
            UInt32 res;

            //res = VCI_Receive(m_devtype, m_devind, m_canind, ref m_recobj[0], 1000, 100);
            res = can_adaptor.Receive(default_canind, ref m_recobj[0]);
            return res;
        }

        unsafe public void TransmitData(uint ID, byte[] Data)
        {
            if (m_bOpen == 0)
                return;

            List<VCI_CAN_OBJ> sendobj_list = new List<VCI_CAN_OBJ>();
            VCI_CAN_OBJ sendobj = new VCI_CAN_OBJ();
            //sendobj.Init();
            sendobj.RemoteFlag = default_RemoteFlag;
            sendobj.ExternFlag = default_ExternFlag;
            sendobj.ID = ID;
            int len = Data.Length;
            sendobj.DataLen = System.Convert.ToByte(len);
            for (int orginal_index = 0; orginal_index < len; orginal_index++)
            {
                // Get the character corresponding to the integral value.
                sendobj.Data[orginal_index] = Data[orginal_index];
            }

            sendobj_list.Add(sendobj);

            //sendobj.Data[7] ^= 0xff;        // for testing multiple sendout_obj
            //sendobj_list.Add(sendobj);      // for testing multiple sendout_obj
            VCI_CAN_OBJ[] sendout_obj = sendobj_list.ToArray();
            uint sendout_obj_len = (uint)sendobj_list.Count;
            if (can_adaptor.Transmit(default_canind, ref sendout_obj[0], sendout_obj_len) == 0)
            {
                Console.WriteLine("Failed to send!");
            //    MessageBox.Show("发送失败", "错误",
            //            MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            //sendobj.Data[6] ^= 0xff;        // for testing multiple sendout_obj
            //sendobj_list.Add(sendobj);      // for testing multiple sendout_obj
            //if (USB_CAN_device.Transmit(m_canind_src, ref sendobj_list) == 0)
            //{
            //    MessageBox.Show("发送失败", "错误",
            //            MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            //}
        }

        unsafe public void GetOneCommand(UInt32 index, out String str, out uint ID, out uint DLC, out byte[] DATA)
        {
            ID = 0;
            DLC = 0;
            const int DATA_LEN = 8;
            DATA = new byte[DATA_LEN];

            ////str = "接收到数据: ";
            ////str += "  帧ID:0x" + System.Convert.ToString(m_recobj[index].ID, 16);
            //str = "帧ID:0x" + System.Convert.ToString(m_recobj[index].ID, 16);
            ////str += "  帧格式:";
            //str += " 格式:";
            //if (m_recobj[index].RemoteFlag == 0)
            //    str += "数据帧 ";
            //else
            //    str += "远程帧 ";
            //if (m_recobj[index].ExternFlag == 0)
            //    str += "标准帧 ";
            //else
            //    str += "扩展帧 ";

            str = "";
            //str = "接收到数据: ";
            //str += "  帧ID:0x" + System.Convert.ToString(m_recobj[i].ID, 16);
            //str += "  帧格式:";

            if (m_recobj[index].ExternFlag == 0)
            {
                //str += "标准帧 ";
                str += "Base-format ";
            }
            else
            {
                //str += "扩展帧 ";
                str += "Extended-format ";
            }
            if (m_recobj[index].RemoteFlag == 0)
            {
                //str += "数据帧 ";
                str += "data-frame ";
            }
            else
                //str += "远程帧 ";
                str += "remote-frame ";

            str += " ID:0x" + System.Convert.ToString(m_recobj[index].ID, 16) + " ";
            ID = m_recobj[index].ID;

            if (m_recobj[index].RemoteFlag == 0)
            {
                byte len = (byte)(m_recobj[index].DataLen % 9);
                DLC = len;

                //str += "数据: ";
                str += "Data:";

                fixed (VCI_CAN_OBJ* m_recobj1 = &m_recobj[index])
                {
                    byte j = 0;
                    while ((j < len) && (j < DATA_LEN))
                    {
                        DATA[j] = m_recobj1->Data[j];
                        str += " " + System.Convert.ToString(DATA[j], 16);
                        j++;
                    }
                }
            }
        }

        public uint Disconnect()
        {
            uint status = ~(1U);
            if (m_bOpen == 1)
            {
                //status = VCI_CloseDevice(m_devtype, m_devind);
                status = can_adaptor.CloseDevice();
                m_bOpen = 0;
            }
            return status;
        }

        public uint StartCAN()
        {
            uint status = ~(1U);
            if (m_bOpen == 1)
            {
                //status = VCI_StartCAN(m_devtype, m_devind, m_canind);
                status = can_adaptor.StartCAN(default_canind);
            }
            return status;
        }

        public uint StopCAN()
        {
            uint status = ~(1U);
            if (m_bOpen == 1)
            {
                //status = VCI_ResetCAN(m_devtype, m_devind, m_canind);
                status = can_adaptor.ResetCAN(default_canind);
            }
            return status;
        }

        public List<String> FindUsbDevice()
        {
            List<String> ret_device_list = new List<String>();
            return ret_device_list = can_adaptor.FindUsbDevice();
        }
    }
}