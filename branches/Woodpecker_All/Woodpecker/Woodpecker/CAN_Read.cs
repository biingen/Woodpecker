﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;

/*------------兼容ZLG的数据类型---------------------------------*/

//1.ZLGCAN系列接口卡信息的数据类型。
public struct VCI_BOARD_INFO
{
    public UInt16 hw_Version;
    public UInt16 fw_Version;
    public UInt16 dr_Version;
    public UInt16 in_Version;
    public UInt16 irq_Num;
    public byte can_Num;
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 20)] public byte[] str_Serial_Num;
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 40)]
    public byte[] str_hw_Type;
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
    public byte[] Reserved;
}

/////////////////////////////////////////////////////
//2.定义CAN信息帧的数据类型。
unsafe public struct VCI_CAN_OBJ  //使用不安全代码
{
    public uint ID;
    public uint TimeStamp;        //时间标识
    public byte TimeFlag;         //是否使用时间标识
    public byte SendType;         //发送标志。保留，未用
    public byte RemoteFlag;       //是否是远程帧
    public byte ExternFlag;       //是否是扩展帧
    public byte DataLen;          //数据长度
    public fixed byte Data[8];    //数据
    public fixed byte Reserved[3];//保留位

}

//3.定义初始化CAN的数据类型
public struct VCI_INIT_CONFIG
{
    public UInt32 AccCode;
    public UInt32 AccMask;
    public UInt32 Reserved;
    public byte Filter;   //0或1接收所有帧。2标准帧滤波，3是扩展帧滤波。
    public byte Timing0;  //波特率参数，具体配置，请查看二次开发库函数说明书。
    public byte Timing1;
    public byte Mode;     //模式，0表示正常模式，1表示只听模式,2自测模式
}

/*------------其他数据结构描述---------------------------------*/
//4.USB-CAN总线适配器板卡信息的数据类型1，该类型为VCI_FindUsbDevice函数的返回参数。
public struct VCI_BOARD_INFO1
{
    public UInt16 hw_Version;
    public UInt16 fw_Version;
    public UInt16 dr_Version;
    public UInt16 in_Version;
    public UInt16 irq_Num;
    public byte can_Num;
    public byte Reserved;
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)] public byte[] str_Serial_Num;
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
    public byte[] str_hw_Type;
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
    public byte[] str_Usb_Serial;
}

/*------------数据结构描述完成---------------------------------*/

public struct CHGDESIPANDPORT
{
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 10)]
    public byte[] szpwd;
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 20)]
    public byte[] szdesip;
    public Int32 desport;

    public void Init()
    {
        szpwd = new byte[10];
        szdesip = new byte[20];
    }
}

namespace Can_Reader_Lib
{
    public enum USB_DEVICE_ID
    {
        DEV_USBCAN = 3,
        DEV_USBCAN2 = 4
    };

    public class USB_CAN_Adaptor
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="DeviceType"></param>
        /// <param name="DeviceInd"></param>
        /// <param name="Reserved"></param>
        /// <returns></returns>
        // ------------兼容ZLG的函数描述--------------------------------- 
        [DllImport("controlcan.dll")]
        static extern UInt32 VCI_OpenDevice(UInt32 DeviceType, UInt32 DeviceInd, UInt32 Reserved);
        [DllImport("controlcan.dll")]
        static extern UInt32 VCI_CloseDevice(UInt32 DeviceType, UInt32 DeviceInd);
        [DllImport("controlcan.dll")]
        static extern UInt32 VCI_InitCAN(UInt32 DeviceType, UInt32 DeviceInd, UInt32 CANInd, ref VCI_INIT_CONFIG pInitConfig);

        [DllImport("controlcan.dll")]
        static extern UInt32 VCI_ReadBoardInfo(UInt32 DeviceType, UInt32 DeviceInd, ref VCI_BOARD_INFO pInfo);

        [DllImport("controlcan.dll")]
        static extern UInt32 VCI_GetReceiveNum(UInt32 DeviceType, UInt32 DeviceInd, UInt32 CANInd);
        [DllImport("controlcan.dll")]
        static extern UInt32 VCI_ClearBuffer(UInt32 DeviceType, UInt32 DeviceInd, UInt32 CANInd);

        [DllImport("controlcan.dll")]
        static extern UInt32 VCI_StartCAN(UInt32 DeviceType, UInt32 DeviceInd, UInt32 CANInd);
        [DllImport("controlcan.dll")]
        static extern UInt32 VCI_ResetCAN(UInt32 DeviceType, UInt32 DeviceInd, UInt32 CANInd);

        [DllImport("controlcan.dll")]
        static extern UInt32 VCI_Transmit(UInt32 DeviceType, UInt32 DeviceInd, UInt32 CANInd, ref VCI_CAN_OBJ pSend, UInt32 Len);

        [DllImport("controlcan.dll")]
        static extern UInt32 VCI_Receive(UInt32 DeviceType, UInt32 DeviceInd, UInt32 CANInd, ref VCI_CAN_OBJ pReceive, UInt32 Len, Int32 WaitTime);

        // ------------其他函数描述---------------------------------

        [DllImport("controlcan.dll")]
        static extern UInt32 VCI_ConnectDevice(UInt32 DevType, UInt32 DevIndex);
        [DllImport("controlcan.dll")]
        static extern UInt32 VCI_UsbDeviceReset(UInt32 DevType, UInt32 DevIndex, UInt32 Reserved);
        [DllImport("controlcan.dll")]
        static extern UInt32 VCI_FindUsbDevice(ref VCI_BOARD_INFO1 pInfo);
        // ------------函数描述结束--------------------------------- 

        public const uint VCI_MAX_OBJECT_LENGTH = 10000;
        public const int VCI_RECEIVE_WAIT_TIME = 100;

        uint m_devtype = (uint)USB_DEVICE_ID.DEV_USBCAN2;
        uint m_devind = 0;
        //uint m_canind = 0;
        //UInt32 m_canind_src = 0;
        //UInt32 m_canind_dst = 1;
        VCI_INIT_CONFIG config = new VCI_INIT_CONFIG();
        VCI_CAN_OBJ[] m_recobj = new VCI_CAN_OBJ[VCI_MAX_OBJECT_LENGTH];
        VCI_BOARD_INFO current_open_board_info = new VCI_BOARD_INFO();

        public void Config_CAN_Device(USB_DEVICE_ID dev_id, uint dev_index)
        {
            m_devtype = (uint)dev_id;
            m_devind = dev_index;
        }

        //public void Config_CAN_TX_Dev_Index(uint src)
        //{
        //    m_canind_src = src;
        //}

        //public void Config_CAN_RX_Dev_Index(uint dst)
        //{
        //    m_canind_dst = dst;
        //}

        public void Config_CAN_Param(uint access_code, uint access_mask, Byte timing0, Byte timing1, Byte filter, Byte mode)
        {
            config.AccCode = access_code;
            config.AccMask = access_mask;
            config.Timing0 = timing0;
            config.Timing1 = timing1;
            config.Filter = filter;
            config.Mode = mode;
        }

        public uint CloseDevice()
        {
            return VCI_CloseDevice(m_devtype, m_devind);
        }

        public uint OpenDevice()
        {
            uint bRet = VCI_OpenDevice(m_devtype, m_devind, 0);      // last parameter is currently always 0
            if (bRet != 0)
            {
                uint dwRel = VCI_ReadBoardInfo(m_devtype, m_devind, ref current_open_board_info);
            }
            return bRet;
        }

        public uint InitCAN(uint can_index)
        {
            //m_canind = can_index;
            return VCI_InitCAN(m_devtype, m_devind, can_index, ref config);
        }

        public uint StartCAN(uint can_index)
        {
            return VCI_StartCAN(m_devtype, m_devind, can_index);
        }

        public uint ResetCAN(uint can_index)
        {
            return VCI_ResetCAN(m_devtype, m_devind, can_index);
        }

        public uint Receive(uint can_index, ref VCI_CAN_OBJ obj_ref, uint obj_length = VCI_MAX_OBJECT_LENGTH, int rec_wait_time = VCI_RECEIVE_WAIT_TIME)
        {
            return VCI_Receive(m_devtype, m_devind, can_index, ref obj_ref, obj_length, rec_wait_time);
        }

        public uint Receive(uint can_index, ref List<VCI_CAN_OBJ> obj_ref_list, int rec_wait_time = VCI_RECEIVE_WAIT_TIME)
        {
            VCI_CAN_OBJ[] obj_ref_array = new VCI_CAN_OBJ[VCI_MAX_OBJECT_LENGTH];
            uint ret_value = VCI_Receive(m_devtype, m_devind, can_index, ref obj_ref_array[0], VCI_MAX_OBJECT_LENGTH, rec_wait_time);

            obj_ref_list.Clear();
            if (ret_value == 0xFFFFFFFF)
            {
                return ret_value;
            }
            for (int index = 0; index < ret_value; index++)
            {
                obj_ref_list.Add(obj_ref_array[index]);
            }
            return ret_value;
        }

        public uint Transmit(uint can_index, ref VCI_CAN_OBJ obj_ref, uint obj_length)
        {
            return VCI_Transmit((uint)m_devtype, m_devind, can_index, ref obj_ref, obj_length);
        }

        public uint Transmit(uint can_index, ref List<VCI_CAN_OBJ> obj_ref_list)
        {
            VCI_CAN_OBJ[] obj_ref_array = obj_ref_list.ToArray();
            uint obj_ref_len = (uint)obj_ref_list.Count;
            return Transmit(can_index, ref obj_ref_array[0], obj_ref_len);
        }

        public List<String> FindUsbDevice()
        {
            List<String> ret_device_list = new List<String>();
            String ProductSn;
            VCI_BOARD_INFO1 vbi_1 = new VCI_BOARD_INFO1();
            uint num = VCI_FindUsbDevice(ref vbi_1);
            int serial_index = 0;
            for (uint i = 0; i < num; i++)
            {
                ProductSn = "CAN-";
                for (int j = 0; j < 4; j++)
                {
                    ProductSn += Convert.ToChar(vbi_1.str_Usb_Serial[serial_index]);
                    serial_index++;
                }
                ret_device_list.Add(ProductSn);
            }
            return ret_device_list;
        }

        internal static void Config_CAN_Device(uint m_devtype, uint m_devind)
        {
            throw new NotImplementedException();
        }
    }


    public class CAN_Reader 
    {

        //static USB_DEVICE_ID m_devtype = 4;//USBCAN2

        UInt32 m_bOpen = 0;
        //UInt32 m_devind = 0;
        //UInt32 m_canind = 0;

        public static int MAX_CAN_OBJ_ARRAY_LEN = 1000;

        VCI_CAN_OBJ[] m_recobj = new VCI_CAN_OBJ[MAX_CAN_OBJ_ARRAY_LEN];

        UInt32[] m_arrdevtype = new UInt32[20];

        // COPY - end


        //
        // Function for external use
        //
        public CAN_Reader() { }
        ~CAN_Reader() { }

        USB_DEVICE_ID default_devtype = USB_DEVICE_ID.DEV_USBCAN2;
        UInt32 default_devint = 0;
        UInt32 default_canind = 1;
        uint default_AccCode = 0x80000000;
        uint default_AccMask = 0xffffffff;
        byte default_Timing0 = 0x00;
        byte default_Timing1 = 0x1C;
        byte default_Filter = 0x01;
        byte default_Mode = 0x00;

        USB_CAN_Adaptor can_adaptor = new USB_CAN_Adaptor();

        public uint Connect()
        {
            uint connection_status = ~(1U);
            if (m_bOpen == 0)
            {
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
    }
}