using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Woodpecker
{
    public class CAN_Parameter
    {
        private byte can_id;
        private byte can_data;
        private byte can_rate;

        public void CAN_Write()
        {
            can_id = 0;
            can_data = 0;
            can_rate = 0;
        }

        public void CAN_Data(byte id, byte data, byte rate)
        {
            can_id = id;
            can_data = data;
            can_rate = rate;
        }

        public List<byte> ToByteList()
        {
            List<byte> ret_list = new List<byte>();
            ret_list.Add(can_id);
            ret_list.Add(can_data);
            ret_list.Add(can_rate);
            return ret_list;
        }
    }

    class CAN_Write
    {
        private Queue<CAN_Parameter> CAN_Write_Data_Queue = new Queue<CAN_Parameter>();

        public void CAN_Write_Queue_Clear()
        {
            CAN_Write_Data_Queue.Clear();
        }

        public void CAN_Write_Queue_Add(CAN_Parameter can_parameter)
        {
            CAN_Write_Data_Queue.Enqueue(can_parameter);
        }
    }
}
