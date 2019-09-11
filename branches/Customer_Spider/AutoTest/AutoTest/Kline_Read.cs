using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;
using System.Reflection;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;
using System.Timers;
using BlockMessageLibrary;
using DTC_ABS;
using DTC_OBD;
using MySerialLibrary;
using KWP_2000;

namespace BlockMessageLibrary
{
    // enum
    enum FORMAT_ID
    {
        NEW = 0,
        ID1 = 1,
        ID2 = 2,
        ID3 = 3,
        ID4 = 4,
        WAIT_FOR_ZERO,
        OUT_OF_RANGE
    };

    enum MSG_A1A0_MODE
    {
        NO_ADDRESS_INFO = 0,
        CARB_MODE = 1,
        WITH_ADDRESS_INFO = 2,
        FUNCTIONAL_ADDRESSING = 3
    }

    enum MSG_STAGE_FORMAT_01
    {
        FMT = 0,
        SID = 1,
        Data = 2,
        CS = 3,
        END
    };

    enum MSG_STAGE_FORMAT_02
    {
        FMT = 0,
        TA = 1,
        SA = 2,
        SID = 3,
        Data = 4,
        CS = 5,
        END
    };

    enum MSG_STAGE_FORMAT_03
    {
        FMT = 0,
        Len = 1,
        SID = 2,
        Data = 3,
        CS = 4,
        END
    };

    enum MSG_STAGE_FORMAT_04
    {
        FMT = 0,
        TA = 1,
        SA = 2,
        Len = 3,
        SID = 4,
        Data = 5,
        CS = 6,
        END
    };

    class BlockMessage
    {
        // Packet data
        private byte Fmt;
        private byte TA;
        private byte SA;
        private byte Len;
        private byte SID;
        private List<Byte> msg_data;
        private byte CheckSum;
        public const uint Max_Len_6Bit = 0x3f;

        public BlockMessage()
        {
            msg_data = new List<byte>(); ClearBlockMessage();
        }

        public BlockMessage(byte FMT_value, byte TA_value, byte SA_value, byte SID_value, List<byte> DataList, bool ExtraLenByte = false)
        {
            // For format 2/4
            msg_data = new List<byte>(); ClearBlockMessage();

            // returning_data
            foreach (byte res in DataList)
            {
                AddToDataList(res);
                UpdateCheckSum(res);
            }

            // FMT
            byte total_len_value = (byte)(GetDataListLen() + 1);
            if ((ExtraLenByte == true) || (total_len_value > Max_Len_6Bit))
            {
                Fmt = (byte)(FMT_value & ~Max_Len_6Bit);   // clear 6-bit LSB 
                UpdateCheckSum(Fmt);
                Len = total_len_value; // plus SID_byte
                UpdateCheckSum(Len);
            }
            else
            {
                Fmt = (byte)((FMT_value & ~Max_Len_6Bit) | (total_len_value & Max_Len_6Bit));   // clear 6-bit LSB then OR len_value
                UpdateCheckSum(Fmt);
                Len = 0;    // Clear for not-used
            }

            // TA
            TA = TA_value;
            UpdateCheckSum(TA_value);
            // SA
            SA = SA_value;
            UpdateCheckSum(SA_value);
            // SID
            SID = SID_value;
            UpdateCheckSum(SID_value);
        }

        public void ClearBlockMessage()
        {
            Fmt = TA = SA = Len = SID = CheckSum = 0;            // set to 0 as null message
            msg_data.Clear();
        }

        public byte GetFmt() { return Fmt; }
        public void SetFmt(byte NewFmt) { Fmt = NewFmt; }

        public byte GetTA() { return TA; }
        public void SetTA(byte NewTA) { TA = NewTA; }

        public byte GetSA() { return SA; }
        public void SetSA(byte NewSA) { SA = NewSA; }

        public byte GetLenByte() { return Len; }
        public void SetLenByte(byte NewLen) { Len = NewLen; }

        public uint GetMessageTotalLen()
        {
            uint temp_len = Fmt & Max_Len_6Bit;
            if (temp_len == 0)
            {
                temp_len = Len;
            }
            return temp_len;
        }

        public byte GetSID() { return SID; }
        public void SetSID(byte NewSID) { SID = NewSID; }

        public byte GetCheckSum() { return CheckSum; }
        public void SetCheckSum(byte NewCheckSum) { CheckSum = NewCheckSum; }

        public List<byte> GetDataList() { return msg_data; }
        public void AddToDataList(byte NewData) { msg_data.Add(NewData); }
        public void ClearDataList() { msg_data.Clear(); }
        public int GetDataListLen() { return msg_data.Count; }

        public byte UpdateCheckSum(byte next_byte) { CheckSum += next_byte; return CheckSum; }

        public bool GenerateSerialOutput(out List<byte> SerialOutputDataList)
        {
            bool bRet = false;
            byte byte_data;

            SerialOutputDataList = new List<byte>();

            // First calculate data length
            uint len = this.GetMessageTotalLen();

            if ((this.GetFmt() & ~BlockMessage.Max_Len_6Bit) == ((byte)(MSG_A1A0_MODE.WITH_ADDRESS_INFO) << 6))
            {
                // This for format 2 or 4 
                // Common portion
                byte_data = this.GetFmt();
                SerialOutputDataList.Add(byte_data);
                byte_data = this.GetTA();
                SerialOutputDataList.Add(byte_data);
                byte_data = this.GetSA();
                SerialOutputDataList.Add(byte_data);

                if ((this.GetFmt() & BlockMessage.Max_Len_6Bit) == 0x00)
                {
                    // Format 4
                    byte_data = this.GetLenByte();
                    SerialOutputDataList.Add(byte_data);
                    bRet = true;
                }
                else if (len <= BlockMessage.Max_Len_6Bit)     // max 6-bit when there isn't extra length byte
                {
                    // Format 2
                    bRet = true;
                }
                else
                {
                    // Error in Len
                }
                // Common Part
                byte_data = this.GetSID();
                SerialOutputDataList.Add(byte_data);
                foreach (byte element in this.GetDataList())
                {
                    SerialOutputDataList.Add(element);
                }
                byte_data = this.GetCheckSum();
                SerialOutputDataList.Add(byte_data);
            }
            else
            {
                // Format 1/3 to be implemented in the future
            }
            return bRet;
        }

        public String GenerateDebugString()
        {
            String out_msg_data_in_string = "";
            byte byte_data;

            // First calculate data length
            uint len = this.GetMessageTotalLen();

            if ((this.GetFmt() & ~BlockMessage.Max_Len_6Bit) == ((byte)(MSG_A1A0_MODE.WITH_ADDRESS_INFO) << 6))
            {
                // This for format 2 or 4 
                // Common portion
                out_msg_data_in_string = "";
                byte_data = this.GetFmt();
                out_msg_data_in_string += byte_data.ToString("X2") + " ";
                byte_data = this.GetTA();
                out_msg_data_in_string += byte_data.ToString("X2") + " ";
                byte_data = this.GetSA();
                out_msg_data_in_string += byte_data.ToString("X2") + " ";

                if (((this.GetFmt() & BlockMessage.Max_Len_6Bit) == 0x00) || (len > BlockMessage.Max_Len_6Bit))
                {
                    // Format 4
                    out_msg_data_in_string = "Format 4 - " + out_msg_data_in_string;
                    byte_data = this.GetLenByte();
                    out_msg_data_in_string += byte_data.ToString("X2") + " ";
                }
                else if (len <= BlockMessage.Max_Len_6Bit)     // max 6-bit when there isn't extra length byte
                {
                    // Format 2
                    out_msg_data_in_string = "Format 2 - " + out_msg_data_in_string;
                }
                else
                {
                    out_msg_data_in_string = "Data Error - to be checked.";
                }
                // Common Part
                byte_data = this.GetSID();
                out_msg_data_in_string += byte_data.ToString("X2") + " ";
                foreach (byte element in this.GetDataList())
                {
                    out_msg_data_in_string += element.ToString("X2") + " ";
                }
                byte_data = this.GetCheckSum();
                out_msg_data_in_string += byte_data.ToString("X2") + " ";
            }
            else
            {
                // Format 1/3 to be implemented in the future
            }

            return out_msg_data_in_string;
        }
    }

    // This class is for processing input block message from serial input
    class ProcessBlockMessage
    {
        // Internal data
        private BlockMessage BlockMessageInProcess;
        private FORMAT_ID Format_ID;
        private int ExpectedDataListLen;
        private uint msg_field_index;

        // Private function
        private void StartNewProcess()
        {
            BlockMessageInProcess.ClearBlockMessage();
            ExpectedDataListLen = 0;
            msg_field_index = 0;
            Format_ID = FORMAT_ID.WAIT_FOR_ZERO;
        }

        private bool ProcessFormat1(byte next_byte)
        {
            bool bRet = false;
            switch ((MSG_STAGE_FORMAT_01)msg_field_index)
            {
                case MSG_STAGE_FORMAT_01.FMT:
                    BlockMessageInProcess.SetFmt(next_byte);
                    ExpectedDataListLen = (next_byte & 0x3f) - 1;           // minus SID byte
                    BlockMessageInProcess.UpdateCheckSum(next_byte);
                    msg_field_index++;
                    break;
                case MSG_STAGE_FORMAT_01.SID:
                    BlockMessageInProcess.SetSID(next_byte);
                    BlockMessageInProcess.UpdateCheckSum(next_byte);
                    if (ExpectedDataListLen > 0)
                    {
                        msg_field_index++;
                    }
                    else
                    {
                        msg_field_index += 2;
                    }
                    break;
                case MSG_STAGE_FORMAT_01.Data:
                    BlockMessageInProcess.AddToDataList(next_byte);
                    BlockMessageInProcess.UpdateCheckSum(next_byte);
                    if (BlockMessageInProcess.GetDataListLen() >= ExpectedDataListLen)
                    {
                        msg_field_index++;
                    }
                    break;
                case MSG_STAGE_FORMAT_01.CS:
                    byte current_checksum = BlockMessageInProcess.GetCheckSum();
                    bRet = (current_checksum == next_byte) ? true : false;      // data available if checksum is ok
                    Format_ID = FORMAT_ID.NEW;
                    msg_field_index = 0;
                    break;
            }
            return bRet;
        }

        private bool ProcessFormat2(byte next_byte)
        {
            bool bRet = false;
            switch ((MSG_STAGE_FORMAT_02)msg_field_index)
            {
                case MSG_STAGE_FORMAT_02.FMT:
                    BlockMessageInProcess.SetFmt(next_byte);
                    ExpectedDataListLen = (next_byte & 0x3f) - 1;           // minus SID byte
                    BlockMessageInProcess.UpdateCheckSum(next_byte);
                    msg_field_index++;
                    break;
                case MSG_STAGE_FORMAT_02.TA:
                    BlockMessageInProcess.SetTA(next_byte);
                    BlockMessageInProcess.UpdateCheckSum(next_byte);
                    msg_field_index++;
                    break;
                case MSG_STAGE_FORMAT_02.SA:
                    BlockMessageInProcess.SetSA(next_byte);
                    BlockMessageInProcess.UpdateCheckSum(next_byte);
                    msg_field_index++;
                    break;
                case MSG_STAGE_FORMAT_02.SID:
                    BlockMessageInProcess.SetSID(next_byte);
                    BlockMessageInProcess.UpdateCheckSum(next_byte);
                    if (ExpectedDataListLen > 0)
                    {
                        msg_field_index++;
                    }
                    else
                    {
                        msg_field_index += 2;
                    }
                    break;
                case MSG_STAGE_FORMAT_02.Data:
                    BlockMessageInProcess.AddToDataList(next_byte);
                    BlockMessageInProcess.UpdateCheckSum(next_byte);
                    if (BlockMessageInProcess.GetDataListLen() >= ExpectedDataListLen)
                    {
                        msg_field_index++;
                    }
                    break;
                case MSG_STAGE_FORMAT_02.CS:
                    byte current_checksum = BlockMessageInProcess.GetCheckSum();
                    Format_ID = FORMAT_ID.NEW;
                    msg_field_index = 0;
                    if (current_checksum == next_byte)
                    {
                        bRet = true;
                    }
                    else
                    {
                        bRet = false;
                    }
                    break;
            }
            return bRet;
        }

        private bool ProcessFormat3(byte next_byte)
        {
            bool bRet = false;
            switch ((MSG_STAGE_FORMAT_03)msg_field_index)
            {
                case MSG_STAGE_FORMAT_03.FMT:
                    BlockMessageInProcess.SetFmt(next_byte);
                    //                   ExpectedDataListLen = (next_byte & 0x3f) - 1;       // minus SID byte
                    BlockMessageInProcess.UpdateCheckSum(next_byte);
                    msg_field_index++;
                    break;
                case MSG_STAGE_FORMAT_03.Len:
                    BlockMessageInProcess.SetLenByte(next_byte);
                    ExpectedDataListLen = next_byte - 1;       // minus SID byte
                    BlockMessageInProcess.UpdateCheckSum(next_byte);
                    msg_field_index++;
                    break;
                case MSG_STAGE_FORMAT_03.SID:
                    BlockMessageInProcess.SetSID(next_byte);
                    BlockMessageInProcess.UpdateCheckSum(next_byte);
                    if (ExpectedDataListLen > 0)
                    {
                        msg_field_index++;
                    }
                    else
                    {
                        msg_field_index += 2;
                    }
                    break;
                case MSG_STAGE_FORMAT_03.Data:
                    BlockMessageInProcess.AddToDataList(next_byte);
                    BlockMessageInProcess.UpdateCheckSum(next_byte);
                    if (BlockMessageInProcess.GetDataListLen() >= ExpectedDataListLen)
                    {
                        msg_field_index++;
                    }
                    break;
                case MSG_STAGE_FORMAT_03.CS:
                    byte current_checksum = BlockMessageInProcess.GetCheckSum();
                    bRet = (current_checksum == next_byte) ? true : false;      // data available if checksum is ok
                    Format_ID = FORMAT_ID.NEW;
                    msg_field_index = 0;
                    break;
            }
            return bRet;
        }

        private bool ProcessFormat4(byte next_byte)
        {
            bool bRet = false;
            switch ((MSG_STAGE_FORMAT_04)msg_field_index)
            {
                case MSG_STAGE_FORMAT_04.FMT:
                    BlockMessageInProcess.SetFmt(next_byte);
                    //                    ExpectedDataListLen = (next_byte & 0x3f) - 1;       // minus SID byte
                    BlockMessageInProcess.UpdateCheckSum(next_byte);
                    msg_field_index++;
                    break;
                case MSG_STAGE_FORMAT_04.TA:
                    BlockMessageInProcess.SetTA(next_byte);
                    BlockMessageInProcess.UpdateCheckSum(next_byte);
                    msg_field_index++;
                    break;
                case MSG_STAGE_FORMAT_04.SA:
                    BlockMessageInProcess.SetSA(next_byte);
                    BlockMessageInProcess.UpdateCheckSum(next_byte);
                    msg_field_index++;
                    break;
                case MSG_STAGE_FORMAT_04.Len:
                    BlockMessageInProcess.SetLenByte(next_byte);
                    ExpectedDataListLen = next_byte - 1;       // minus SID byte
                    BlockMessageInProcess.UpdateCheckSum(next_byte);
                    msg_field_index++;
                    break;
                case MSG_STAGE_FORMAT_04.SID:
                    BlockMessageInProcess.SetSID(next_byte);
                    BlockMessageInProcess.UpdateCheckSum(next_byte);
                    if (ExpectedDataListLen > 0)
                    {
                        msg_field_index++;
                    }
                    else
                    {
                        msg_field_index += 2;
                    }
                    break;
                case MSG_STAGE_FORMAT_04.Data:
                    BlockMessageInProcess.AddToDataList(next_byte);
                    BlockMessageInProcess.UpdateCheckSum(next_byte);
                    if (BlockMessageInProcess.GetDataListLen() >= ExpectedDataListLen)
                    {
                        msg_field_index++;
                    }
                    break;
                case MSG_STAGE_FORMAT_04.CS:
                    byte current_checksum = BlockMessageInProcess.GetCheckSum();
                    bRet = (current_checksum == next_byte) ? true : false;      // data available if checksum is ok
                    Format_ID = FORMAT_ID.NEW;
                    msg_field_index = 0;
                    break;
            }
            return bRet;
        }

        public const int P2_Time_min = 25;
        public const int P2_Time_max = 50;
        public const int P3_Time_min = 25;
        public const int P3_Time_max = 5000; // 5000ms
        private static Timer aTimer = new Timer(P3_Time_max);
        private static bool P3_Timeout_Flag = false;

        private static void SetP3Timer()
        {
            aTimer = new System.Timers.Timer(P3_Time_max);
            aTimer.Elapsed += OnP3TimedEvent;
            aTimer.AutoReset = false;
            P3_Timeout_Flag = false;
            aTimer.Enabled = true;
        }

        private static void OnP3TimedEvent(Object source, System.Timers.ElapsedEventArgs e)
        {
            P3_Timeout_Flag = true;
            aTimer.Enabled = false;
        }

        private void ClearP3TimeoutFlag()
        {
            aTimer.Enabled = false;
            P3_Timeout_Flag = false;
            //aTimer.Dispose();
        }

        private bool CheckAndClearP3Timeout()
        {
            bool bRet = P3_Timeout_Flag;
            ClearP3TimeoutFlag();
            return (bRet);
        }

        // Public function
        public ProcessBlockMessage() { BlockMessageInProcess = new BlockMessage(); StartNewProcess(); }

        public bool ProcessNextByte(byte next_byte)
        {
            bool bRet = false;

            switch (Format_ID)
            {
                case FORMAT_ID.WAIT_FOR_ZERO:
                    if (next_byte == 0)
                    {
                        Format_ID = FORMAT_ID.NEW;
                        msg_field_index = 0;
                    }
                    break;
                case FORMAT_ID.NEW:
                    if (CheckAndClearP3Timeout() == true)
                    {
                        // Already timeout, must be a zero for a valid data
                        if (next_byte != 0)
                        {
                            Format_ID = FORMAT_ID.WAIT_FOR_ZERO;
                        }
                        else
                        {
                            msg_field_index = 0;
                        }
                    }
                    else if (next_byte == 0)
                    {
                        msg_field_index = 0;
                    }
                    else
                    {
                        BlockMessageInProcess.ClearBlockMessage();
                        MSG_A1A0_MODE mode_info = (MSG_A1A0_MODE)((next_byte & 0xc0) >> 6);
                        switch (mode_info)
                        {
                            case MSG_A1A0_MODE.NO_ADDRESS_INFO:      // No Address Information
                                if ((next_byte & 0x3f) == 0)
                                {
                                    Format_ID = FORMAT_ID.ID3;
                                    ProcessFormat3(next_byte);
                                }
                                else
                                {
                                    Format_ID = FORMAT_ID.ID1;
                                    ProcessFormat1(next_byte);
                                }
                                break;
                            case MSG_A1A0_MODE.CARB_MODE:
                                // CARB mode - to be checked & implemented
                                Format_ID = FORMAT_ID.OUT_OF_RANGE;
                                break;
                            case MSG_A1A0_MODE.WITH_ADDRESS_INFO:
                                if ((next_byte & 0x3f) == 0)
                                {
                                    Format_ID = FORMAT_ID.ID4;
                                    ProcessFormat4(next_byte);
                                }
                                else
                                {
                                    Format_ID = FORMAT_ID.ID2;
                                    ProcessFormat2(next_byte);
                                }
                                break;
                            case MSG_A1A0_MODE.FUNCTIONAL_ADDRESSING:
                                // Functional Addressing mode are not supported here
                                Format_ID = FORMAT_ID.OUT_OF_RANGE;
                                break;
                        }
                    }
                    break;
                case FORMAT_ID.ID1:
                    bRet = ProcessFormat1(next_byte);
                    break;
                case FORMAT_ID.ID2:
                    bRet = ProcessFormat2(next_byte);
                    break;
                case FORMAT_ID.ID3:
                    bRet = ProcessFormat3(next_byte);
                    break;
                case FORMAT_ID.ID4:
                    bRet = ProcessFormat4(next_byte);
                    break;
                default:
                    if (next_byte == 0)
                    {
                        Format_ID = FORMAT_ID.NEW;
                        msg_field_index = 0;
                    }
                    break;
            }
            if (bRet == true)
            {
                SetP3Timer();
            }
            return bRet;
        }

        public BlockMessage GetProcessedBlockMessage() { return BlockMessageInProcess; }

    }
}

namespace DTC_ABS
{
    enum ABS_DTC_Code
    {
        ECU_Control_unit_failure = 0x5055,
        VR_Valve_Relay_Fault = 0x5019,
        Valves_EV_Inlet_value_Failure_F = 0x5017,
        Valves_EV_Inlet_value_Failure_R = 0x5013,
        Valves_AV_Outlet_value_Failure_F = 0x5018,
        Valves_AV_Outlet_value_Failure_R = 0x5014,
        UZ_Batter_Voltage_fault_Over_Voltage = 0x5053,
        UZ_Batter_Voltage_fault_Under_Voltage = 0x5052,
        RFP_Pump_Motor_Failure = 0x5035,
        WSS_ohmic_WSS_ohmic_failure_F = 0x5043,
        WSS_ohmic_WSS_ohmic_failure_R = 0x5045,
        WSS_plausibility_failure_F = 0x5042,
        WSS_plausibility_failure_R = 0x5044,
        WSS_generic_failure = 0x5025,
        END
    }

    class CMD_E_ABS_DTC
    {
        private uint byte_index;
        private uint bit_index;
        private string failure_type;
        private string description;
        private ABS_DTC_Code dtc_code;

        public CMD_E_ABS_DTC(uint byte_idx, uint bit_idx, ABS_DTC_Code DTC_value, string type, string desc)
        {
            byte_index = byte_idx;
            bit_index = bit_idx;
            failure_type = type;
            description = desc;
            dtc_code = DTC_value;
        }

        public uint ByteIndex
        {
            get { return byte_index; }
        }
        public uint BitIndex
        {
            get { return bit_index; }
        }
        public string FailureType
        {
            get { return failure_type; }
        }
        public string Description
        {
            get { return failure_type; }
        }
        public ABS_DTC_Code DTC
        {
            get { return dtc_code; }
        }
    }

    static class ABS_DTC_Table
    {
        static List<CMD_E_ABS_DTC> abs_dtc_table = new List<CMD_E_ABS_DTC>();

        static ABS_DTC_Table()
        {
            abs_dtc_table.Add(new CMD_E_ABS_DTC(0, 0, ABS_DTC_Code.ECU_Control_unit_failure,
                    "ECU", "Control Unit Failure"));
            abs_dtc_table.Add(new CMD_E_ABS_DTC(0, 1, ABS_DTC_Code.VR_Valve_Relay_Fault,
                    "VR", "Valve Replay Fault"));
            abs_dtc_table.Add(new CMD_E_ABS_DTC(0, 2, ABS_DTC_Code.Valves_EV_Inlet_value_Failure_F,
                    "Valves EV", "Inlet Valve Failure - Front"));
            abs_dtc_table.Add(new CMD_E_ABS_DTC(0, 3, ABS_DTC_Code.Valves_EV_Inlet_value_Failure_R,
                    "Valves EV", "Inlet Valve Failure - Rear"));
            abs_dtc_table.Add(new CMD_E_ABS_DTC(0, 4, ABS_DTC_Code.Valves_AV_Outlet_value_Failure_F,
                    "Valves AV", "Outlet Valve Failure - Front"));
            abs_dtc_table.Add(new CMD_E_ABS_DTC(0, 5, ABS_DTC_Code.Valves_AV_Outlet_value_Failure_R,
                    "Valves AV", "Outlet Valve Failure - Rear"));
            abs_dtc_table.Add(new CMD_E_ABS_DTC(0, 6, ABS_DTC_Code.UZ_Batter_Voltage_fault_Over_Voltage,
                    "UZ", "Battery Voltage Fault (Over-Voltage)"));
            abs_dtc_table.Add(new CMD_E_ABS_DTC(0, 7, ABS_DTC_Code.UZ_Batter_Voltage_fault_Under_Voltage,
                    "UZ", "Battery Voltage Fault (Under-Voltage)"));
            abs_dtc_table.Add(new CMD_E_ABS_DTC(1, 0, ABS_DTC_Code.RFP_Pump_Motor_Failure,
                    "RFP/RFP_HW", "Pump Motor Failure"));
            abs_dtc_table.Add(new CMD_E_ABS_DTC(1, 1, ABS_DTC_Code.WSS_ohmic_WSS_ohmic_failure_F,
                    "WSS_Ohmic", "WSS ohmic Failure - Front"));
            abs_dtc_table.Add(new CMD_E_ABS_DTC(1, 2, ABS_DTC_Code.WSS_ohmic_WSS_ohmic_failure_R,
                    "WSS_Ohmic", "WSS ohmic Failure - Rear"));
            abs_dtc_table.Add(new CMD_E_ABS_DTC(1, 3, ABS_DTC_Code.WSS_plausibility_failure_F,
                    "WSS_Plausibility", "WSS Plausibility Failure - Front"));
            abs_dtc_table.Add(new CMD_E_ABS_DTC(1, 4, ABS_DTC_Code.WSS_plausibility_failure_R,
                    "WSS_Plausibility", "WSS Plausibility Failure - Rear"));
            abs_dtc_table.Add(new CMD_E_ABS_DTC(1, 5, ABS_DTC_Code.WSS_generic_failure,
                    "WSS_Generic", "WSS Generic Failure"));
        }

        static public CMD_E_ABS_DTC Find_ABS_DTC(ABS_DTC_Code code)
        {
            foreach (CMD_E_ABS_DTC item in abs_dtc_table)
            {
                if (item.DTC == code)
                    return item;
            }
            return null;
        }

        static public CMD_E_ABS_DTC Find_ABS_DTC(int index)
        {
            if ((index >= 0) && (index < abs_dtc_table.Count))
                return abs_dtc_table.ElementAt(index);
            else
                return null;
        }

        static public CMD_E_ABS_DTC Find_ABS_DTC(uint byte_idx, uint bit_idx)
        {
            foreach (CMD_E_ABS_DTC item in abs_dtc_table)
            {
                if ((item.ByteIndex == byte_idx) && (item.BitIndex == bit_idx))
                    return item;
            }
            return null;
        }
        static public int Count()
        {
            return abs_dtc_table.Count();
        }
    }
}

namespace DTC_OBD
{
    // P: 0x00
    // C: 0x04
    // B: 0x08
    // U: 0x0C

    enum OBD_DTC_Code
    {
        P0503 = 0x0503,
        C0083 = 0x4083,
        C0085 = 0x4085,
        P0105 = 0x0105,
        P0110 = 0x0110,
        P0115 = 0x0115,
        P0120 = 0x0120,
        P0130 = 0x0130,

        P0135 = 0x0135,
        P0150 = 0x0150,
        P0155 = 0x0155,
        P0201 = 0x0201,
        P0202 = 0x0202,
        P0217 = 0x0217,
        P0230 = 0x0230,
        P0335 = 0x0335,

        P0336 = 0x0336,
        P0351 = 0x0351,
        P0352 = 0x0352,
        P0410 = 0x0410,
        P0480 = 0x0480,
        P0500 = 0x0500,
        P0501 = 0x0501,
        P0505 = 0x0505,

        P0512 = 0x0512,
        P0560 = 0x0560,
        P0601 = 0x0601,
        P0604 = 0x0604,
        P0605 = 0x0605,
        P0606 = 0x0606,
        P0620_PIN2 = 0x0620,
        P0620_PIN31 = 0x0620,

        P0650 = 0x0650,
        P0655 = 0x0655,
        P0A0F = 0x0A0F,
        P1300 = 0x1300,
        P1310 = 0x1310,
        P1536 = 0x1536,
        P1607 = 0x1607,
        P1800 = 0x1800,

        P2158 = 0x2158,
        P2600 = 0x2600,
        U0001 = 0xC001,
        U0002 = 0xC002,
        U0121 = 0xC121,
        U0122 = 0xC122,
        U0128 = 0xC128,
        U0140 = 0xC140,

        U0426 = 0xC426,
        U0486 = 0xC486,
        END
    }

    class CMD_F_OBD_DTC
    {
        private uint byte_index;
        private uint bit_index;
        private string failure_type;
        private string description;
        private OBD_DTC_Code dtc_code;

        public CMD_F_OBD_DTC(uint byte_idx, uint bit_idx, OBD_DTC_Code DTC_value, string type, string desc)
        {
            byte_index = byte_idx;
            bit_index = bit_idx;
            failure_type = type;
            description = desc;
            dtc_code = DTC_value;
        }

        public uint ByteIndex
        {
            get { return byte_index; }
        }
        public uint BitIndex
        {
            get { return bit_index; }
        }
        public string FailureType
        {
            get { return failure_type; }
        }
        public string Description
        {
            get { return failure_type; }
        }
        public OBD_DTC_Code DTC
        {
            get { return dtc_code; }
        }
    }

    static class OBD_DTC_Table
    {
        static List<CMD_F_OBD_DTC> obd_dtc_table = new List<CMD_F_OBD_DTC>();

        static OBD_DTC_Table()
        {
            obd_dtc_table.Add(new CMD_F_OBD_DTC(0, 0, OBD_DTC_Code.P0503,
                    "Engine idling control", "Vehicle Speed Sensor Intermittent/Erratic/High"));
            obd_dtc_table.Add(new CMD_F_OBD_DTC(0, 1, OBD_DTC_Code.C0083,
                    "Chassis", "Tire Pressure Monitor Malfunction Indicator (Subfault)"));
            obd_dtc_table.Add(new CMD_F_OBD_DTC(0, 2, OBD_DTC_Code.C0085,
                    "Chassis", "Traction Disable Indicator (Subfault)"));
            obd_dtc_table.Add(new CMD_F_OBD_DTC(0, 3, OBD_DTC_Code.P0105,
                    "air/fuel mixture control", "Manifold Absolute Pressure/Barometric Pressure Circuit Malfunction"));
            obd_dtc_table.Add(new CMD_F_OBD_DTC(0, 4, OBD_DTC_Code.P0110,
                    "air/fuel mixture control", "Intake Air Temperature Circuit Malfunction"));
            obd_dtc_table.Add(new CMD_F_OBD_DTC(0, 5, OBD_DTC_Code.P0115,
                    "air/fuel mixture control", "Engine Coolant Temperature Circuit Malfunction"));
            obd_dtc_table.Add(new CMD_F_OBD_DTC(0, 6, OBD_DTC_Code.P0120,
                    "air/fuel mixture control", "Throttle Pedal Position Sensor/Switch A Circuit Malfunction"));
            obd_dtc_table.Add(new CMD_F_OBD_DTC(0, 7, OBD_DTC_Code.P0130,
                    "air/fuel mixture control", "O2 Sensor Circuit Malfunction (Bank 2 Sensor 1)"));

            obd_dtc_table.Add(new CMD_F_OBD_DTC(1, 0, OBD_DTC_Code.P0135,
                    "air/fuel mixture control", "O2 Sensor Heater Circuit Malfunction (Bank 2 Sensor 1)"));
            obd_dtc_table.Add(new CMD_F_OBD_DTC(1, 1, OBD_DTC_Code.P0150,
                    "air/fuel mixture control", "O2 Sensor Circuit Malfunction (Bank 2 Sensor 1)"));
            obd_dtc_table.Add(new CMD_F_OBD_DTC(1, 2, OBD_DTC_Code.P0155,
                    "air/fuel mixture control", "O2 Sensor Heater Circuit Malfunction (Bank 2 Sensor 1)"));
            obd_dtc_table.Add(new CMD_F_OBD_DTC(1, 3, OBD_DTC_Code.P0201,
                    "air/fuel mixture control", "Injector Circuit Malfunction - Cylinder 1"));
            obd_dtc_table.Add(new CMD_F_OBD_DTC(1, 4, OBD_DTC_Code.P0202,
                    "air/fuel mixture control", "Injector Circuit Malfunction - Cylinder 2"));
            obd_dtc_table.Add(new CMD_F_OBD_DTC(1, 5, OBD_DTC_Code.P0217,
                    "air/fuel mixture control", "Engine Overtemp Condition"));
            obd_dtc_table.Add(new CMD_F_OBD_DTC(1, 6, OBD_DTC_Code.P0230,
                    "air/fuel mixture control", "Fuel Pump Primary Circuit Malfunction"));
            // Failure Type and Description needs to be updated from here
            obd_dtc_table.Add(new CMD_F_OBD_DTC(1, 7, OBD_DTC_Code.P0335,
                    "WSS_Generic", "WSS Generic Failure"));

            obd_dtc_table.Add(new CMD_F_OBD_DTC(2, 0, OBD_DTC_Code.P0336,
                    "WSS_Generic", "WSS Generic Failure"));
            obd_dtc_table.Add(new CMD_F_OBD_DTC(2, 1, OBD_DTC_Code.P0351,
                    "WSS_Generic", "WSS Generic Failure"));
            obd_dtc_table.Add(new CMD_F_OBD_DTC(2, 2, OBD_DTC_Code.P0352,
                    "WSS_Generic", "WSS Generic Failure"));
            obd_dtc_table.Add(new CMD_F_OBD_DTC(2, 3, OBD_DTC_Code.P0410,
                    "WSS_Generic", "WSS Generic Failure"));
            obd_dtc_table.Add(new CMD_F_OBD_DTC(2, 4, OBD_DTC_Code.P0480,
                    "WSS_Generic", "WSS Generic Failure"));
            obd_dtc_table.Add(new CMD_F_OBD_DTC(2, 5, OBD_DTC_Code.P0500,
                    "WSS_Generic", "WSS Generic Failure"));
            obd_dtc_table.Add(new CMD_F_OBD_DTC(2, 6, OBD_DTC_Code.P0501,
                    "WSS_Generic", "WSS Generic Failure"));
            obd_dtc_table.Add(new CMD_F_OBD_DTC(2, 7, OBD_DTC_Code.P0505,
                    "WSS_Generic", "WSS Generic Failure"));

            obd_dtc_table.Add(new CMD_F_OBD_DTC(3, 0, OBD_DTC_Code.P0512,
                    "WSS_Generic", "WSS Generic Failure"));
            obd_dtc_table.Add(new CMD_F_OBD_DTC(3, 1, OBD_DTC_Code.P0560,
                    "WSS_Generic", "WSS Generic Failure"));
            obd_dtc_table.Add(new CMD_F_OBD_DTC(3, 2, OBD_DTC_Code.P0601,
                    "WSS_Generic", "WSS Generic Failure"));
            obd_dtc_table.Add(new CMD_F_OBD_DTC(3, 3, OBD_DTC_Code.P0604,
                    "WSS_Generic", "WSS Generic Failure"));
            obd_dtc_table.Add(new CMD_F_OBD_DTC(3, 4, OBD_DTC_Code.P0605,
                    "WSS_Generic", "WSS Generic Failure"));
            obd_dtc_table.Add(new CMD_F_OBD_DTC(3, 5, OBD_DTC_Code.P0606,
                    "WSS_Generic", "WSS Generic Failure"));
            obd_dtc_table.Add(new CMD_F_OBD_DTC(3, 6, OBD_DTC_Code.P0620_PIN2,
                    "Onboard computer and ancillary outputs", "Generator Control Circuit Malfunction"));
            obd_dtc_table.Add(new CMD_F_OBD_DTC(3, 7, OBD_DTC_Code.P0620_PIN31,
                    "Onboard computer and ancillary outputs", "Generator Control Circuit Malfunction"));

            obd_dtc_table.Add(new CMD_F_OBD_DTC(4, 0, OBD_DTC_Code.P0650,
                    "WSS_Generic", "WSS Generic Failure"));
            obd_dtc_table.Add(new CMD_F_OBD_DTC(4, 1, OBD_DTC_Code.P0655,
                    "WSS_Generic", "WSS Generic Failure"));
            obd_dtc_table.Add(new CMD_F_OBD_DTC(4, 2, OBD_DTC_Code.P0A0F,
                    "WSS_Generic", "WSS Generic Failure"));
            obd_dtc_table.Add(new CMD_F_OBD_DTC(4, 3, OBD_DTC_Code.P1300,
                    "WSS_Generic", "WSS Generic Failure"));
            obd_dtc_table.Add(new CMD_F_OBD_DTC(4, 4, OBD_DTC_Code.P1310,
                    "WSS_Generic", "WSS Generic Failure"));
            obd_dtc_table.Add(new CMD_F_OBD_DTC(4, 5, OBD_DTC_Code.P1536,
                    "WSS_Generic", "WSS Generic Failure"));
            obd_dtc_table.Add(new CMD_F_OBD_DTC(4, 6, OBD_DTC_Code.P1607,
                    "WSS_Generic", "WSS Generic Failure"));
            obd_dtc_table.Add(new CMD_F_OBD_DTC(4, 7, OBD_DTC_Code.P1800,
                    "WSS_Generic", "WSS Generic Failure"));

            obd_dtc_table.Add(new CMD_F_OBD_DTC(5, 0, OBD_DTC_Code.P2158,
                    "WSS_Generic", "WSS Generic Failure"));
            obd_dtc_table.Add(new CMD_F_OBD_DTC(5, 1, OBD_DTC_Code.P2600,
                    "WSS_Generic", "WSS Generic Failure"));
            obd_dtc_table.Add(new CMD_F_OBD_DTC(5, 2, OBD_DTC_Code.U0001,
                    "WSS_Generic", "WSS Generic Failure"));
            obd_dtc_table.Add(new CMD_F_OBD_DTC(5, 3, OBD_DTC_Code.U0002,
                    "WSS_Generic", "WSS Generic Failure"));
            obd_dtc_table.Add(new CMD_F_OBD_DTC(5, 4, OBD_DTC_Code.U0121,
                    "WSS_Generic", "WSS Generic Failure"));
            obd_dtc_table.Add(new CMD_F_OBD_DTC(5, 5, OBD_DTC_Code.U0122,
                    "WSS_Generic", "WSS Generic Failure"));
            obd_dtc_table.Add(new CMD_F_OBD_DTC(5, 6, OBD_DTC_Code.U0128,
                    "WSS_Generic", "WSS Generic Failure"));
            obd_dtc_table.Add(new CMD_F_OBD_DTC(5, 7, OBD_DTC_Code.U0140,
                    "WSS_Generic", "WSS Generic Failure"));

            obd_dtc_table.Add(new CMD_F_OBD_DTC(6, 0, OBD_DTC_Code.U0426,
                    "WSS_Generic", "WSS Generic Failure"));
            obd_dtc_table.Add(new CMD_F_OBD_DTC(6, 1, OBD_DTC_Code.U0486,
                    "WSS_Generic", "WSS Generic Failure"));
        }

        static public CMD_F_OBD_DTC Find_OBD_DTC(OBD_DTC_Code code)
        {
            foreach (CMD_F_OBD_DTC item in obd_dtc_table)
            {
                if (item.DTC == code)
                    return item;
            }
            return null;
        }

        static public CMD_F_OBD_DTC Find_OBD_DTC(int index)
        {
            if ((index >= 0) && (index < obd_dtc_table.Count))
                return obd_dtc_table.ElementAt(index);
            else
                return null;
        }

        static public CMD_F_OBD_DTC Find_OBD_DTC(uint byte_idx, uint bit_idx)
        {
            foreach (CMD_F_OBD_DTC item in obd_dtc_table)
            {
                if ((item.ByteIndex == byte_idx) && (item.BitIndex == bit_idx))
                    return item;
            }
            return null;
        }

        static public int Count()
        {
            return OBD_DTC_Table.Count();
        }

        enum DTC_Prefix
        {
            P = 0x0000,
            C = 0x4000,
            B = 0x8000,
            U = 0xC000
        }

        static public OBD_DTC_Code GetDTCValueFromString(String dtc_code_str)
        {
            uint dtc_value = 0;

            dtc_code_str.ToUpper();
            char chr = dtc_code_str.ElementAt(0);
            switch (chr)
            {
                case 'P':
                    dtc_value |= (uint)DTC_Prefix.P;
                    break;
                case 'C':
                    dtc_value |= (uint)DTC_Prefix.C;
                    break;
                case 'B':
                    dtc_value |= (uint)DTC_Prefix.B;
                    break;
                case 'U':
                    dtc_value |= (uint)DTC_Prefix.U;
                    break;
            }
            dtc_code_str = dtc_code_str.Substring(1, dtc_code_str.Length - 1);
            dtc_value |= (Convert.ToUInt32(dtc_code_str, 16) & 0x3fff);

            return (OBD_DTC_Code)dtc_value;
        }

        static public String GetDTCStringFromValue(uint dtc_value)
        {
            String ret_str = "";
            dtc_value &= 0xffff;

            switch ((DTC_Prefix)(dtc_value & 0xC000))
            {
                case DTC_Prefix.P:
                    ret_str = "P";
                    break;
                case DTC_Prefix.C:
                    ret_str = "C";
                    break;
                case DTC_Prefix.B:
                    ret_str = "B";
                    break;
                case DTC_Prefix.U:
                    ret_str = "U";
                    break;
            }
            ret_str += (dtc_value & 0x3fff).ToString("X4");

            return ret_str;
        }

        static public String GetDTCStringFromValue(OBD_DTC_Code dtc_code)
        {
            return GetDTCStringFromValue((uint)dtc_code);
        }
    }
}

namespace KWP_2000
{
    class DTC_Data
    {
        private byte dtc_high;
        private byte dtc_low;
        private byte status_of_dtc;

        public DTC_Data(byte high, byte low, byte status)
        {
            dtc_high = high;
            dtc_low = low;
            status_of_dtc = status;
        }

        public List<byte> ToByteList()
        {
            List<byte> ret_list = new List<byte>();
            ret_list.Add(dtc_high);
            ret_list.Add(dtc_low);
            ret_list.Add(status_of_dtc);
            return ret_list;
        }
    }

    class KWP_2000_Process
    {
        public const byte ADDRESS_ABS = 0x28;
        public const byte ADDRESS_OBD = 0x10;
        public const int min_delay_before_response = ProcessBlockMessage.P2_Time_min;
        public const byte RETURN_SID_OR_VALUE = 0x40;
        public const byte NEGATIVE_RESPONSE_SID = 0x7F;
        public const int ReadABSDiagnosticCodesByStatus_MaxNumberOfDTC = 15;
        public const int ReadOBDDiagnosticCodesByStatus_MaxNumberOfDTC = 51;

        enum ENUM_SID
        {
            ReadDiagnosticTroubleCodesByStatus = 0x18,
            StartCommunication = 0x81,
            StopCommunication = 0x82,
            MAX_SID_PLUS_1
        };

        private BlockMessage ResponseMessage = new BlockMessage();

        public KWP_2000_Process()
        {
            ResponseMessage.ClearBlockMessage();
        }

        public bool ProcessMessage(BlockMessage in_msg, ref BlockMessage out_msg)
        {
            bool bRet = false;

            if (in_msg.GetTA() == ADDRESS_ABS)
            {
                bRet = ProcessMessage_ABS(in_msg, ref out_msg);
            }
            else if (in_msg.GetTA() == ADDRESS_OBD)
            {
                bRet = ProcessMessage_OBD(in_msg, ref out_msg);
            }
            return bRet;
        }

        private BlockMessage PrepareResponse_StopCommunication_ABS(BlockMessage in_msg, ref BlockMessage out_msg)
        {
            List<byte> out_list = new List<byte>();
            out_msg = new BlockMessage((byte)((((uint)MSG_A1A0_MODE.WITH_ADDRESS_INFO) << 6)), in_msg.GetSA(), in_msg.GetTA(),
                                                (byte)(in_msg.GetSID() | RETURN_SID_OR_VALUE), out_list, true); // for format_4
            return out_msg;
        }

        private uint ABS_KeyByte_for_StartCommunication = 0x8FEF;
        private BlockMessage PrepareResponse_StartCommunication_ABS(BlockMessage in_msg, ref BlockMessage out_msg)
        {
            List<byte> out_list = new List<byte>();
            out_list.Add((byte)(ABS_KeyByte_for_StartCommunication & 0xff));
            out_list.Add((byte)((ABS_KeyByte_for_StartCommunication >> 8) & 0xff));
            out_msg = new BlockMessage((byte)((((uint)MSG_A1A0_MODE.WITH_ADDRESS_INFO) << 6)), in_msg.GetSA(), in_msg.GetTA(),
                                                (byte)(in_msg.GetSID() | RETURN_SID_OR_VALUE), out_list, true); // for format_4
            return out_msg;
        }

        private uint ReadDiagnosticTroubleCodesByStatus_ABS_StatusOfDTC = 0;
        private uint ReadDiagnosticTroubleCodesByStatus_ABS_GroupOfDTC = 0;
        //        private byte[] fixed_response_data_abs = { 0x04, 0x50, 0x43, 0xE0, 0x50, 0x45, 0xE0, 0x50, 0x52, 0xA0, 0x50, 0x53, 0xA0 };
        // 0x5043
        // 0x5045
        // 0x5052
        // 0x5053
        public const byte Lamp_OFF_Failure_Set = 0x60;
        public const byte Lamp_OFF_Failure_Reset = 0x20;
        public const byte Lamp_ON_Failure_Set = 0xE0;
        public const byte Lamp_ON_Failure_Reset = 0xA0;
        public byte[] dtc_status_table = { Lamp_ON_Failure_Reset, Lamp_ON_Failure_Set };  // bit 7: lamp off/on (always on here); bit 6,5: 01/11 = failure is reset/set at time of request
        private byte[] dtc_status_table_lamp_may_by_off = { Lamp_OFF_Failure_Reset, Lamp_OFF_Failure_Set, Lamp_ON_Failure_Reset, Lamp_ON_Failure_Set };
        public byte[] dtc_status_table_for_obd = { 0x61, 0x62 };
        private Queue<DTC_Data> ABS_DTC_Data_Queue = new Queue<DTC_Data>();
        private Queue<DTC_Data> OBD_DTC_Data_Queue = new Queue<DTC_Data>();

        public void ABS_DTC_Queue_Clear()
        {
            ABS_DTC_Data_Queue.Clear();
        }

        public void ABS_DTC_Queue_Add(byte dtc_high, byte dtc_low, byte lamp_status)
        {
            ABS_DTC_Data_Queue.Enqueue(new DTC_Data(dtc_high, dtc_low, lamp_status));
        }

        public void ABS_DTC_Queue_Add(CMD_E_ABS_DTC new_dtc, byte lamp_status)
        {
            uint dtc = (uint)new_dtc.DTC;
            ABS_DTC_Data_Queue.Enqueue(new DTC_Data((byte)(dtc >> 8), (byte)(dtc & 0xff), lamp_status));
        }

        public void OBD_DTC_Queue_Clear()
        {
            OBD_DTC_Data_Queue.Clear();
        }

        public void OBD_DTC_Queue_Add(byte dtc_high, byte dtc_low, byte lamp_status)
        {
            OBD_DTC_Data_Queue.Enqueue(new DTC_Data(dtc_high, dtc_low, lamp_status));
        }

        public void OBD_DTC_Queue_Add(CMD_F_OBD_DTC new_dtc, byte lamp_status)
        {
            uint dtc = (uint)new_dtc.DTC;
            OBD_DTC_Data_Queue.Enqueue(new DTC_Data((byte)(dtc >> 8), (byte)(dtc & 0xff), lamp_status));
        }

        private List<byte> GenerateQueuedResponseData_ABS()
        {
            List<byte> ret_list = new List<byte>();
            List<byte> status_of_dtc_list = new List<byte>();
            Byte DTC_no = 0;

            while ((ABS_DTC_Data_Queue.Count > 0) && (DTC_no < ReadABSDiagnosticCodesByStatus_MaxNumberOfDTC))
            {
                DTC_Data this_abs_dtc = ABS_DTC_Data_Queue.Dequeue();
                ret_list.AddRange(this_abs_dtc.ToByteList());
                DTC_no++;
            }
            status_of_dtc_list.Add(DTC_no);
            status_of_dtc_list.AddRange(ret_list);
            return status_of_dtc_list;
        }

        private List<byte> GenerateQueuedResponseData_OBD()
        {
            List<byte> ret_list = new List<byte>();
            List<byte> status_of_dtc_list = new List<byte>();
            Byte DTC_no = 0;

            while ((OBD_DTC_Data_Queue.Count > 0) && (DTC_no < ReadOBDDiagnosticCodesByStatus_MaxNumberOfDTC))
            {
                DTC_Data this_obd_dtc = OBD_DTC_Data_Queue.Dequeue();
                ret_list.AddRange(this_obd_dtc.ToByteList());
                DTC_no++;
            }
            status_of_dtc_list.Add(DTC_no);
            status_of_dtc_list.AddRange(ret_list);
            return status_of_dtc_list;
        }

        private BlockMessage PrepareResponse_ReadDiagnosticTroubleCodesByStatus_ABS(BlockMessage in_msg, ref BlockMessage out_msg)
        {
            List<byte> out_list = GenerateQueuedResponseData_ABS();
            out_msg = new BlockMessage((byte)((((uint)MSG_A1A0_MODE.WITH_ADDRESS_INFO) << 6)), in_msg.GetSA(), in_msg.GetTA(),
                                                (byte)(in_msg.GetSID() | RETURN_SID_OR_VALUE), out_list, true); // for format_4
            return out_msg;
        }

        private const byte NegativeResponse_DuringInit_ReadDiagnosticTroubleCodesByStatus_ResponseCode = 0x78;
        private BlockMessage PrepareNegativeResponse_DuringInit_ReadDiagnosticTroubleCodesByStatus_ABS(BlockMessage in_msg, ref BlockMessage out_msg)
        {
            List<byte> out_list = new List<byte>();

            out_list.Add(in_msg.GetSID());
            out_list.Add(NegativeResponse_DuringInit_ReadDiagnosticTroubleCodesByStatus_ResponseCode);
            out_msg = new BlockMessage((byte)((((uint)MSG_A1A0_MODE.WITH_ADDRESS_INFO) << 6)), in_msg.GetSA(), in_msg.GetTA(),
                                                NEGATIVE_RESPONSE_SID, out_list, true); // for format_4
            return out_msg;
        }

        private const byte NegativeResponse_MsgError_ReadDiagnosticTroubleCodesByStatus_ResponseCode = 0x12;
        private BlockMessage PrepareNegativeResponse_MsgErrort_ReadDiagnosticTroubleCodesByStatus_ABS(BlockMessage in_msg, ref BlockMessage out_msg)
        {
            List<byte> out_list = new List<byte>();

            out_list.Add(in_msg.GetSID());
            out_list.Add(NegativeResponse_MsgError_ReadDiagnosticTroubleCodesByStatus_ResponseCode);
            out_msg = new BlockMessage((byte)((((uint)MSG_A1A0_MODE.WITH_ADDRESS_INFO) << 6)), in_msg.GetSA(), in_msg.GetTA(),
                                                NEGATIVE_RESPONSE_SID, out_list, true); // for format_4
            return out_msg;
        }


        private bool ProcessMessage_ABS(BlockMessage abs_msg, ref BlockMessage ResponseMessage)
        {
            bool bRet = false;

            switch ((ENUM_SID)abs_msg.GetSID())
            {
                case ENUM_SID.ReadDiagnosticTroubleCodesByStatus:
                    // Read Status of DTC & Group of DTC
                    List<byte> in_list = abs_msg.GetDataList();
                    ReadDiagnosticTroubleCodesByStatus_ABS_StatusOfDTC = (uint)in_list.IndexOf(0);
                    ReadDiagnosticTroubleCodesByStatus_ABS_GroupOfDTC = (uint)in_list.IndexOf(1) + ((uint)in_list.IndexOf(2)) << 8;
                    PrepareResponse_ReadDiagnosticTroubleCodesByStatus_ABS(abs_msg, ref ResponseMessage);
                    bRet = true;
                    break;
                case ENUM_SID.StartCommunication:
                    PrepareResponse_StartCommunication_ABS(abs_msg, ref ResponseMessage);
                    bRet = true;
                    break;
                case ENUM_SID.StopCommunication:
                    PrepareResponse_StopCommunication_ABS(abs_msg, ref ResponseMessage);
                    bRet = true;
                    break;
                default:
                    break;
            }

            return bRet;
        }

        private BlockMessage PrepareResponse_StopCommunication_OBD(BlockMessage in_msg, ref BlockMessage out_msg)
        {
            List<byte> out_list = new List<byte>();
            out_msg = new BlockMessage((byte)((((uint)MSG_A1A0_MODE.WITH_ADDRESS_INFO) << 6)), in_msg.GetSA(), in_msg.GetTA(),
                                                (byte)(in_msg.GetSID() | RETURN_SID_OR_VALUE), out_list, false); // for format_2
            return out_msg;
        }

        private uint OBD_KeyByte_for_StartCommunication = 0x8FEF;
        private BlockMessage PrepareResponse_StartCommunication_OBD(BlockMessage in_msg, ref BlockMessage out_msg)
        {
            List<byte> out_list = new List<byte>();
            out_list.Add((byte)(OBD_KeyByte_for_StartCommunication & 0xff));
            out_list.Add((byte)((OBD_KeyByte_for_StartCommunication >> 8) & 0xff));
            out_msg = new BlockMessage((byte)((((uint)MSG_A1A0_MODE.WITH_ADDRESS_INFO) << 6)), in_msg.GetSA(), in_msg.GetTA(),
                                                (byte)(in_msg.GetSID() | RETURN_SID_OR_VALUE), out_list, false); // for format_2
            return out_msg;
        }

        private uint ReadDiagnosticTroubleCodesByStatus_OBD_StatusOfDTC = 0;
        private uint ReadDiagnosticTroubleCodesByStatus_OBD_GroupOfDTC = 0;

        private BlockMessage PrepareResponse_ReadDiagnosticTroubleCodesByStatus_OBD(BlockMessage in_msg, ref BlockMessage out_msg)
        {
            //List<byte> out_list = GenerateFixednResponseData_OBD();
            List<byte> out_list = GenerateQueuedResponseData_OBD();
            out_msg = new BlockMessage((byte)((((uint)MSG_A1A0_MODE.WITH_ADDRESS_INFO) << 6)), in_msg.GetSA(), in_msg.GetTA(),
                                                (byte)(in_msg.GetSID() | RETURN_SID_OR_VALUE), out_list, false); // for format_2
            return out_msg;
        }

        private bool ProcessMessage_OBD(BlockMessage obd_msg, ref BlockMessage ResponseMessage)
        {
            bool bRet = false;

            switch ((ENUM_SID)obd_msg.GetSID())
            {
                case ENUM_SID.ReadDiagnosticTroubleCodesByStatus:
                    List<byte> in_list = obd_msg.GetDataList();
                    ReadDiagnosticTroubleCodesByStatus_OBD_StatusOfDTC = (uint)in_list.IndexOf(0);
                    ReadDiagnosticTroubleCodesByStatus_OBD_GroupOfDTC = (uint)in_list.IndexOf(1) + ((uint)in_list.IndexOf(2)) << 8;
                    PrepareResponse_ReadDiagnosticTroubleCodesByStatus_OBD(obd_msg, ref ResponseMessage);
                    bRet = true;
                    break;
                case ENUM_SID.StartCommunication:
                    PrepareResponse_StartCommunication_OBD(obd_msg, ref ResponseMessage);
                    bRet = true;
                    break;
                case ENUM_SID.StopCommunication:
                    PrepareResponse_StopCommunication_OBD(obd_msg, ref ResponseMessage);
                    bRet = true;
                    break;
                default:
                    break;
            }

            return bRet;
        }

    }
}

namespace MySerialLibrary
{
    class MySerial : IDisposable
    {
        // static member/function to shared aross all MySerial
        static protected Dictionary<string, Object> MySerialDictionary = new Dictionary<string, Object>();

        // Private member
        private SerialPort _serialPort;

        //
        // public functions
        //
        enum BAUD_RATE_LIST
        {
            BR_9600 = 9600,
            BR_K_Line = 10400,
            BR_115200 = 115200,
            BR_230400 = 230400,
        };

        enum RX_PROCESSOR
        {
            ENQUEUE = 0,
            READLINE,
            K_LINE
        }

        private RX_PROCESSOR Rx_Processor_Selection = RX_PROCESSOR.K_LINE;              // ENQUEUE

        public const int Serial_BaudRate = (int)BAUD_RATE_LIST.BR_K_Line;  // BAUD_RATE_LIST.BR_115200;
        public const Parity Serial_Parity = Parity.None;
        public const int Serial_DataBits = 8;
        public const StopBits Serial_StopBits = StopBits.One;

        public MySerial(string com_port) { _serialPort = new SerialPort(com_port, Serial_BaudRate, Serial_Parity, Serial_DataBits, Serial_StopBits); }
        public string GetPortName() { return _serialPort.PortName; }

        public MySerial()
        {
            _serialPort = new SerialPort
            {
                BaudRate = Serial_BaudRate,
                Parity = Serial_Parity,
                DataBits = Serial_DataBits,
                StopBits = Serial_StopBits
            };
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                // dispose managed resources
                _serialPort.Close();
            }
            // free native resources
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        static public List<string> FindAllSerialPort()
        {
            List<string> ListSerialPort = new List<string>();

            foreach (string comport_s in SerialPort.GetPortNames())
            {
                ListSerialPort.Add(comport_s);
            }

            return ListSerialPort;
        }


        public Boolean OpenPort()
        {
            Boolean bRet = false;
            _serialPort.Handshake = Handshake.None;
            _serialPort.Encoding = Encoding.UTF8;
            _serialPort.ReadTimeout = 1000;
            _serialPort.WriteTimeout = 1000;
            switch (Rx_Processor_Selection)
            {
                case RX_PROCESSOR.ENQUEUE:
                    _serialPort.DataReceived += new SerialDataReceivedEventHandler(DataReceivedHandler);
                    break;
                case RX_PROCESSOR.K_LINE:
                    _serialPort.DataReceived += new SerialDataReceivedEventHandler(DataReceivedHandler_KLine);
                    break;
                case RX_PROCESSOR.READLINE:
                    // To-be-added.
                    break;
            }

            try
            {
                _serialPort.Open();
                Start_SerialReadThread();
                MySerialDictionary.Add(_serialPort.PortName, this);
                bRet = true;
            }
            catch (Exception ex232)
            {
                Console.WriteLine("MySerial_OpenPort Exception at PORT: " + _serialPort.PortName + " - " + ex232);
                bRet = false;
            }
            return bRet;
        }

        public Boolean OpenPort(string PortName)
        {
            Boolean bRet = false;
            _serialPort.PortName = PortName;
            bRet = OpenPort();
            return bRet;
        }

        public Boolean ClosePort()
        {
            Boolean bRet = false;
            MySerialDictionary.Remove(_serialPort.PortName);

            try
            {
                Stop_SerialReadThread();
                _serialPort.Close();
                bRet = true;
            }
            catch (Exception ex232)
            {
                Console.WriteLine("Serial_ClosePort Exception at PORT: " + _serialPort.PortName + " - " + ex232);
                bRet = false;
            }
            return bRet;
        }

        public Boolean IsPortOpened()
        {
            Boolean bRet = false;
            //if ((_serialPort.IsOpen == true) && (readThread.IsAlive))
            if (_serialPort.IsOpen == true)
            {
                bRet = true;
            }
            return bRet;
        }

        //
        // Start of read part
        //

        private Queue<byte> Rx_byte_buffer_QUEUE = new Queue<byte>();
        private Queue<string> UART_READ_MSG_QUEUE = new Queue<string>();
        public Queue<string> LOG_QUEUE = new Queue<string>();

        public byte GetRxByte() { byte ret_byte = Rx_byte_buffer_QUEUE.Dequeue(); return ret_byte; }
        public bool IsRxEmpty() { return (Rx_byte_buffer_QUEUE.Count <= 0) ? true : false; }

        public bool GetBreakState() { return _serialPort.BreakState; }

        private void Start_SerialReadThread()
        {
            LOG_QUEUE.Clear();
            UART_READ_MSG_QUEUE.Clear();
            Rx_byte_buffer_QUEUE.Clear();
        }

        private void Stop_SerialReadThread()
        {
            LOG_QUEUE.Clear();
            UART_READ_MSG_QUEUE.Clear();
            Rx_byte_buffer_QUEUE.Clear();
        }

        // This Handler is for reading all input without wating for a whole line
        private static void DataReceivedHandler(object sender, SerialDataReceivedEventArgs e)
        {
            // Find out which serial port --> which myserial
            SerialPort sp = (SerialPort)sender;
            MySerialDictionary.TryGetValue(sp.PortName, out Object myserial_serial_obj);
            MySerial myserial = (MySerial)myserial_serial_obj;
            //Rx_char_buffer_QUEUE
            int buf_len = sp.BytesToRead;
            if (buf_len > 0)
            {
                // Read in all char
                byte[] input_buf = new byte[buf_len];
                sp.Read(input_buf, 0, buf_len);
                {
                    int ch_index = 0;
                    while (ch_index < buf_len)
                    {
                        byte byte_data = input_buf[ch_index];
                        myserial.Rx_byte_buffer_QUEUE.Enqueue(byte_data);
                        ch_index++;
                    }
                }
            }
        }

        //
        // K-line portion of code
        //

        public List<BlockMessage> KLineBlockMessageList = new List<BlockMessage>();
        public List<String> KLineRawDataInStringList = new List<String>();
        private String RawDataInString = "";

        private ProcessBlockMessage KLineKWP2000Process = new ProcessBlockMessage();

        private bool ECU_filtering = false;
        private List<byte> ECU_data_to_be_filtered = new List<byte>();

        public void Enable_ECU_Filtering(bool enabled)
        {
            ECU_filtering = enabled;
        }

        public void Add_ECU_Filtering_Data(List<byte> filter_Data)
        {
            ECU_data_to_be_filtered.Clear();
            ECU_data_to_be_filtered.AddRange(filter_Data);
        }

        private static void DataReceivedHandler_KLine(object sender, SerialDataReceivedEventArgs e)
        {
            // Find out which serial port --> which myserial
            SerialPort sp = (SerialPort)sender;
            MySerialDictionary.TryGetValue(sp.PortName, out Object myserial_serial_obj);
            MySerial myserial = (MySerial)myserial_serial_obj;

            while (sp.BytesToRead > 0)
            {
                // Read in all char
                bool IsMessageReady = false;
                byte byte_data = (byte)sp.ReadByte();
                if (myserial.ECU_filtering == true)
                {
                    if (myserial.ECU_data_to_be_filtered.Count > 0)
                    {
                        myserial.ECU_data_to_be_filtered.RemoveAt(0);
                    }
                    if (myserial.ECU_data_to_be_filtered.Count == 0)
                    {
                        myserial.ECU_filtering = false;
                    }
                }
                else
                {
                    myserial.RawDataInString += byte_data.ToString("X2") + " ";
                    IsMessageReady = myserial.KLineKWP2000Process.ProcessNextByte(byte_data);
                    if (IsMessageReady)
                    {
                        BlockMessage new_message = myserial.KLineKWP2000Process.GetProcessedBlockMessage();
                        myserial.KLineBlockMessageList.Add(new_message);
                        myserial.KLineRawDataInStringList.Add(myserial.RawDataInString);
                        myserial.RawDataInString = "";
                        IsMessageReady = false;
                        //break;
                    }
                }
            }
        }

        //
        // END of K-line portion of code
        //

        //
        // End of read part
        //

        public bool SendToSerial(byte[] byte_to_sent)
        {
            bool return_value = false;

            if (_serialPort.IsOpen == true)
            {
                //Application.DoEvents();
                try
                {
                    int temp_index = 0;
                    const int fixed_length = 16;

                    while ((temp_index < byte_to_sent.Length) && (_serialPort.IsOpen == true))
                    {
                        if ((temp_index + fixed_length) < byte_to_sent.Length)
                        {
                            _serialPort.Write(byte_to_sent, temp_index, fixed_length);
                            temp_index += fixed_length;
                        }
                        else
                        {
                            _serialPort.Write(byte_to_sent, temp_index, (byte_to_sent.Length - temp_index));
                            temp_index = byte_to_sent.Length;
                        }
                    }
                    return_value = true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("BlueRatSendToSerial - " + ex);
                    return_value = false;
                }
            }
            else
            {
                Console.WriteLine("COM is closed and cannot send byte data\n");
                return_value = false;
            }
            return return_value;
        }

        //
        // To process UART IO Exception
        //
        protected virtual void OnUARTException(EventArgs e)
        {
            UARTException?.Invoke(this, e);
        }

        public event EventHandler UARTException;


    }
}