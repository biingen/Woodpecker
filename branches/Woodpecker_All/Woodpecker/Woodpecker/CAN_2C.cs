using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Can_Reader_Lib;

namespace USB_CAN2C
{
    public class CAN_Data
    {
        public uint ID;
        public uint TimeStamp;        //时间标识
        public byte TimeFlag;         //是否使用时间标识
        public byte SendType;         //发送标志。保留，未用
        public byte RemoteFlag;       //是否是远程帧
        public byte ExternFlag;       //是否是扩展帧
        public byte DataLen;          //数据长度
        public byte[] Data = new byte[8];    //数据
        public byte[] Reserved = new byte[3];//保留位
        public uint Rate;               //延遲時間

        public CAN_Data()
        {
            ID = 0;
            for (int i = 0; i < 8; i++)
            {
                Data[i] = 0;
            }
        }

        public CAN_Data(uint id, uint timestamp, byte[] data, byte length)
        {
            ID = id;
            TimeStamp = timestamp;
            RemoteFlag = 0x00;
            ExternFlag = 0x00;
            DataLen = length;
            Data = data;
        }
    }

    class USB_CAN_Process
    {
        private Queue<CAN_Data> CAN_Write_Data_Queue = new Queue<CAN_Data>();
        USB_CAN_Adaptor can_adaptor = new USB_CAN_Adaptor();
        List<VCI_CAN_OBJ> sendobj_list = new List<VCI_CAN_OBJ>();
        private static System.Timers.Timer aTimer;
        int timer = 0;

        public void CAN_Write_Queue_Clear()
        {
            CAN_Write_Data_Queue.Clear();
        }

        public void CAN_Write_Queue_Add(CAN_Data can_data)
        {
            CAN_Write_Data_Queue.Enqueue(can_data);
        }

        public void TransmitTimer(object sender, System.Timers.ElapsedEventArgs e)
        {
            UInt32 default_canind = 1;
            VCI_CAN_OBJ[] sendout_obj = sendobj_list.ToArray();
            uint sendout_obj_len = (uint)sendobj_list.Count;

            if (can_adaptor.Transmit(default_canind, ref sendout_obj[0], sendout_obj_len) == 0)
            {
                //MessageBox.Show("发送失败", "错误",
                //MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        unsafe public void CAN_Write_Queue_SendData()
        {
            VCI_CAN_OBJ sendobj = new VCI_CAN_OBJ();
            int id = 0, rate = 0;

            while ((CAN_Write_Data_Queue.Count > 0))
            {
                CAN_Data this_can_ctl = CAN_Write_Data_Queue.Dequeue();
                sendobj.RemoteFlag = this_can_ctl.RemoteFlag;
                sendobj.ExternFlag = this_can_ctl.ExternFlag;
                sendobj.ID = this_can_ctl.ID;
                id = (int)this_can_ctl.ID;
                sendobj.DataLen = this_can_ctl.DataLen;
                for (int i=0;i< this_can_ctl.DataLen; i++)
                {
                    sendobj.Data[i] = this_can_ctl.Data[i];
                }
                sendobj.TimeStamp = this_can_ctl.TimeStamp;
                rate = (int)this_can_ctl.TimeStamp;
                sendobj_list.Add(sendobj);
            }
            
            if (timer == 0)
            {
                timer = 1;
                aTimer = new System.Timers.Timer(rate);
                aTimer.Elapsed += TransmitTimer;
                aTimer.AutoReset = true;
                aTimer.Enabled = true;
            }
        }
    }
}
