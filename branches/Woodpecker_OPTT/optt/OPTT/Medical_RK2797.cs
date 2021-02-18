using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OPTT
{
    class Medical_RK2797
    {
        // copy to mei
        enum command_index
        {
            // Calibration Command
            SET_GAMMA_INDEX = 0,
            GET_GAMMA_INDEX,
            //SET_OUTPUT_GAMMA_TABLE_INDEX,
            GET_OUTPUT_GAMMA_TABLE_INDEX,
            SET_COLOR_GAMUT_INDEX,
            GET_COLOR_GAMUT_INDEX,
            //SET_INPUT_GAMMA_TABLE_INDEX,
            GET_INPUT_GAMMA_TABLE_INDEX,
            //SET_PCM_MARTIX_TABLE_INDEX,
            GET_PCM_MARTIX_TABLE_INDEX,
            SET_COLOR_TEMP_INDEX,
            GET_COLOR_TEMP_INDEX,
            SET_RGB_GAIN_INDEX,
            GET_RGB_GAIN_INDEX,

            // Control Command
            SET_BACKLIGHT_INDEX,
            GET_BACKLIGHT_INDEX,
            SET_PQ_ONOFF_INDEX,
            SET_INTERNAL_PATTERN_INDEX,
            SET_PATTERN_RGB_INDEX,
            SET_SHARPNESS_INDEX,
            GET_SHARPNESS_INDEX,
            GET_BACKLIGHT_SENSOR_INDEX,
            GET_THERMAL_SENSOR_INDEX,
            SET_SPI_PORT_INDEX,
            GET_SPI_PORT_INDEX,
            SET_UART_PORT_INDEX,
            GET_UART_PORT_INDEX,
            SET_BRIGHTNESS_INDEX,
            GET_BRIGHTNESS_INDEX,
            SET_CONTRAST_INDEX,
            GET_CONTRAST_INDEX,
            SET_MAIN_INPUT_INDEX,
            GET_MAIN_INPUT_INDEX,
            SET_SUB_INPUT_INDEX,
            GET_SUB_INPUT_INDEX,
            SET_PIP_MODE_INDEX,
            GET_PIP_MODE_INDEX,

            // Write Data Command
            GET_SCALER_TYPE_INDEX,
            //SET_MODEL_NAME_INDEX,
            GET_MODEL_NAME_INDEX,
            //SET_EDID_INDEX,
            GET_EDID_INDEX,
            //SET_HDCP14_INDEX,
            GET_HDCP14_INDEX,
            //SET_HDCP2x_INDEX,
            GET_HDCP2x_INDEX,
            //SET_SERIAL_NUMBER_INDEX,
            GET_SERIAL_NUMBER_INDEX,
            GET_FW_VERSION_INDEX,
            GET_FAC_EEPROM_DATA_INDEX,

            // BenQ Command
            //SET_BENQ_MODEL_NAME_INDEX,
            //SET_BENQ_SERIAL_NAME_INDEX,
            //SET_BENQ_FW_VERSION_INDEX,
            //SET_BENQ_MONITOR_ID_INDEX,
            //SET_BENQ_DNA_VERSION_INDEX,
            //SET_BENQ_MANUFACTURE_YEARANDDATE_INDEX,
            //SET_BENQ_EEPROM_INIT_INDEX,
            //GET_BENQ_EEPROM_INDEX,
        }

        byte[][] Command_Packet =
        {
			// Calibration Command
            new byte[] { 0x06, 0x00, 0xE0, 0x00, 0xff, 0xff },              ///SET_GAMMA_INDEX,
            new byte[] { 0x05, 0x00, 0xE0, 0x01, 0xff },                    ///GET_GAMMA_INDEX,
            //new byte[] { 0xff, 0x00, 0xE0, 0x02, 0xff, 0xff, 0xff },      ///SET_OUTPUT_GAMMA_TABLE_INDEX,
            new byte[] { 0x07, 0x00, 0xE0, 0x03, 0xff, 0xff, 0xff },        ///GET_OUTPUT_GAMMA_TABLE_INDEX,
            new byte[] { 0x06, 0x00, 0xE0, 0x04, 0xff, 0xff },              ///SET_COLOR_GAMUT_INDEX,
            new byte[] { 0x05, 0x00, 0xE0, 0x05, 0xff },                    ///GET_COLOR_GAMUT_INDEX,
            //new byte[] { 0xff, 0x00, 0xE0, 0x06, 0xff, 0xff, 0xff, 0xff, 0xff },						///SET_INPUT_GAMMA_TABLE_INDEX,
            new byte[] { 0x05, 0x00, 0xE0, 0x07, 0xff },              		///GET_INPUT_GAMMA_TABLE_INDEX,
            //new byte[] { 0xff, 0x00, 0xE0, 0x08, 0xff, 0xff, 0xff },      ///SET_PCM_MARTIX_TABLE_INDEX,
            new byte[] { 0x05, 0x00, 0xE0, 0x09, 0xff },                    ///GET_PCM_MARTIX_TABLE_INDEX,
            new byte[] { 0x06, 0x00, 0xE0, 0x0A, 0xff, 0xff },              ///SET_COLOR_TEMP_INDEX,
            new byte[] { 0x05, 0x00, 0xE0, 0x0B, 0xff },              		///GET_COLOR_TEMP_INDEX,
            new byte[] { 0x07, 0x00, 0xE0, 0x0C, 0xff, 0xff, 0xff },		///SET_RGB_GAIN_INDEX,
            new byte[] { 0x05, 0x00, 0xE0, 0x0D, 0xff },              		///GET_RGB_GAIN_INDEX,		
            
            // Control Command
            new byte[] { 0x06, 0x01, 0xE0, 0x00, 0xff, 0xff },		    	///SET_BACKLIGHT_INDEX,
            new byte[] { 0x05, 0x01, 0xE0, 0x01, 0xff },           			///GET_BACKLIGHT_INDEX,			
            new byte[] { 0x07, 0x01, 0xE0, 0x02, 0xff, 0xff, 0xff },        ///SET_PQ_ONOFF_INDEX,
            new byte[] { 0x06, 0x01, 0xE0, 0x03, 0xff, 0xff },              ///SET_INTERNAL_PATTERN_INDEX,
            new byte[] { 0x0B, 0x01, 0xE0, 0x04, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff },            ///SET_PATTERN_RGB_INDEX,
            new byte[] { 0x06, 0x01, 0xE0, 0x05, 0xff, 0xff },              ///SET_SHARPNESS_INDEX,
            new byte[] { 0x05, 0x01, 0xE0, 0x06, 0xff },              		///GET_SHARPNESS_INDEX,
            new byte[] { 0x05, 0x01, 0xE0, 0x07, 0xff },              		///GET_BACKLIGHT_SENSOR_INDEX,
            new byte[] { 0x05, 0x01, 0xE0, 0x08, 0xff },                    ///GET_THERMAL_SENSOR_INDEX,
            //new byte[] { 0x06, 0x01, 0xE0, 0x08, 0xff, 0xff },            ///GET_THERMAL_SENSOR_INDEX,
            new byte[] { 0x06, 0x01, 0xE0, 0x09, 0xff, 0xff },              ///SET_SPI_PORT_INDEX,
            new byte[] { 0x05, 0x01, 0xE0, 0x0A, 0xff },              		///GET_SPI_PORT_INDEX,
            new byte[] { 0x06, 0x01, 0xE0, 0x0B, 0xff, 0xff },              ///SET_UART_PORT_INDEX,
            new byte[] { 0x05, 0x01, 0xE0, 0x0C, 0xff },              		///GET_UART_PORT_INDEX,
            new byte[] { 0x06, 0x01, 0xE0, 0x0D, 0xff, 0xff },              ///SET_BRIGHTNESS_INDEX,
            new byte[] { 0x05, 0x01, 0xE0, 0x0E, 0xff },        		    ///GET_BRIGHTNESS_INDEX,
            new byte[] { 0x06, 0x01, 0xE0, 0x0F, 0xff, 0xff },              ///SET_CONTRAST_INDEX,
            new byte[] { 0x05, 0x01, 0xE0, 0x10, 0xff },              		///GET_CONTRAST_INDEX,
            new byte[] { 0x06, 0x01, 0xE0, 0x11, 0xff, 0xff },              ///SET_MAIN_INPUT_INDEX,
            new byte[] { 0x05, 0x01, 0xE0, 0x12, 0xff },              		///GET_MAIN_INPUT_INDEX,
            new byte[] { 0x06, 0x01, 0xE0, 0x13, 0xff, 0xff },              ///SET_SUB_INPUT_INDEX,
            new byte[] { 0x05, 0x01, 0xE0, 0x14, 0xff },              		///GET_SUB_INPUT_INDEX,
            new byte[] { 0x06, 0x01, 0xE0, 0x15, 0xff, 0xff },              ///SET_PIP_MODE_INDEX,
            new byte[] { 0x05, 0x01, 0xE0, 0x16, 0xff },              		///GET_PIP_MODE_INDEX,
			
            // Write Data Command
            new byte[] { 0x05, 0x02, 0xE0, 0x00, 0xff },              		///GET_SCALER_TYPE_INDEX,
            //new byte[] { 0xff, 0x02, 0xE0, 0x01, 0xff, 0xff, 0xff },      ///SET_MODEL_NAME_INDEX,
            new byte[] { 0x05, 0x02, 0xE0, 0x02, 0xff },              		///GET_MODEL_NAME_INDEX,
            //new byte[] { 0xff, 0x02, 0xE0, 0x03, 0xff, 0xff, 0xff, 0xff, 0xff },              ///SET_EDID_INDEX,
            new byte[] { 0x06, 0x02, 0xE0, 0x04, 0xff, 0xff },              ///GET_EDID_INDEX,
            //new byte[] { 0xff, 0x02, 0xE0, 0x05, 0xff, 0xff, 0xff, 0xff },///SET_HDCP14_INDEX,
            new byte[] { 0x05, 0x02, 0xE0, 0x06, 0xff },              		///GET_HDCP14_INDEX,
            //new byte[] { 0xff, 0x02, 0xE0, 0x07, 0xff, 0xff, 0xff, 0xff },///SET_HDCP2x_INDEX,
            new byte[] { 0x06, 0x02, 0xE0, 0x08, 0xff },              		///GET_HDCP2x_INDEX,
            //new byte[] { 0xff, 0x02, 0xE0, 0x09, 0xff, 0xff, 0xff },      ///SET_SERIAL_NUMBER_INDEX,
            new byte[] { 0x05, 0x02, 0xE0, 0x0A, 0xff },              		///GET_SERIAL_NUMBER_INDEX,
            new byte[] { 0x05, 0x02, 0xE0, 0x0B, 0xff },              		///GET_FW_VERSION_INDEX,
            new byte[] { 0x05, 0x02, 0xE0, 0x0C, 0xff },              		///GET_FAC_EEPROM_DATA_INDEX,
			
            // BenQ Command
            //new byte[] { 0xff, 0x00, 0xE0, 0x00, 0xff, 0xff, 0xff },      ///SET_BENQ_MODEL_NAME_INDEX,
            //new byte[] { 0xff, 0x00, 0xE0, 0x01, 0xff, 0xff, 0xff },      ///SET_BENQ_SERIAL_NAME_INDEX,
            //new byte[] { 0xff, 0x00, 0xE0, 0x02, 0xff, 0xff, 0xff },      ///SET_BENQ_FW_VERSION_INDEX,
            //new byte[] { 0xff, 0x00, 0xE0, 0x03, 0xff, 0xff, 0xff },      ///SET_BENQ_MONITOR_ID_INDEX,
            //new byte[] { 0xff, 0x00, 0xE0, 0x04, 0xff, 0xff, 0xff },      ///SET_BENQ_DNA_VERSION_INDEX,
            //new byte[] { 0xff, 0x00, 0xE0, 0x05, 0xff, 0xff, 0xff },      ///SET_BENQ_MANUFACTURE_YEARANDDATE_INDEX,
            //new byte[] { 0xff, 0x00, 0xE0, 0x06, 0xff, 0xff, 0xff },      ///SET_BENQ_EEPROM_INIT_INDEX,
            //new byte[] { 0xff, 0x00, 0xE0, 0x07, 0xff, 0xff, 0xff },      ///GET_BENQ_EEPROM_INDEX,
        };
        // copy to mei

        byte[][] Parsing_Packet =
        {
            //// Calibration Command
            new byte [] { }, //SET_GAMMA_INDEX = 0,
            new byte [] { 0x06, 0x00, 0xE0, 0x01 }, //GET_GAMMA_INDEX,
            //new byte [] { }, //SET_OUTPUT_GAMMA_TABLE_INDEX,
            new byte [] { }, //GET_OUTPUT_GAMMA_TABLE_INDEX,
            new byte [] { }, //SET_COLOR_GAMUT_INDEX,
            new byte [] { 0x06, 0x00, 0xE0, 0x05 }, //GET_COLOR_GAMUT_INDEX,
            //new byte [] { }, //SET_INPUT_GAMMA_TABLE_INDEX,
            new byte [] { }, //GET_INPUT_GAMMA_TABLE_INDEX,
            //new byte [] { }, //SET_PCM_MARTIX_TABLE_INDEX,
            new byte [] { }, //GET_PCM_MARTIX_TABLE_INDEX,
            new byte [] { }, //SET_COLOR_TEMP_INDEX,
            new byte [] { 0x06, 0x00, 0xE0, 0x0B }, //GET_COLOR_TEMP_INDEX,
            new byte [] { }, //SET_RGB_GAIN_INDEX,
            new byte [] { 0x08, 0x00, 0xE0, 0x0D }, //GET_RGB_GAIN_INDEX,
            
            //// Control Command
            new byte [] { }, //SET_BACKLIGHT_INDEX,
            new byte [] { 0x06, 0x01, 0xE0, 0x01 },  //GET_BACKLIGHT_INDEX,
            new byte [] { }, //SET_PQ_ONOFF_INDEX,
            new byte [] { }, //SET_INTERNAL_PATTERN_INDEX,
            new byte [] { }, //SET_PATTERN_RGB_INDEX,
            new byte [] { }, //SET_SHARPNESS_INDEX,
            new byte [] { 0x06, 0x01, 0xE0, 0x06 }, //GET_SHARPNESS_INDEX,
            new byte [] { 0x07, 0x01, 0xE0, 0x07 }, //GET_BACKLIGHT_SENSOR_INDEX,
            new byte [] { 0x09, 0x01, 0xE0, 0x08 }, //GET_THERMAL_SENSOR_INDEX,
            new byte [] { }, //SET_SPI_PORT_INDEX,
            new byte [] { 0x07, 0x01, 0xE0, 0x0A }, //GET_SPI_PORT_INDEX,
            new byte [] { }, //SET_UART_PORT_INDEX,
            new byte [] { 0x07, 0x01, 0xE0, 0x0C }, //GET_UART_PORT_INDEX,
            new byte [] { }, //SET_BRIGHTNESS_INDEX,
            new byte [] { 0x06, 0x01, 0xE0, 0x0E }, //GET_BRIGHTNESS_INDEX,
            new byte [] { }, //SET_CONTRAST_INDEX,
            new byte [] { 0x06, 0x01, 0xE0, 0x10 }, //GET_CONTRAST_INDEX,
            new byte [] { }, //SET_MAIN_INPUT_INDEX,
            new byte [] { 0x06, 0x01, 0xE0, 0x12 }, //GET_MAIN_INPUT_INDEX,
            new byte [] { }, //SET_SUB_INPUT_INDEX,
            new byte [] { 0x06, 0x01, 0xE0, 0x14 }, //GET_SUB_INPUT_INDEX,
            new byte [] { }, //SET_PIP_MODE_INDEX,
            new byte [] { 0x06, 0x01, 0xE0, 0x16 }, //GET_PIP_MODE_INDEX,
            
            //new byte [] { }, // Write Data Command
            new byte [] { }, //GET_SCALER_TYPE_INDEX,
            //new byte [] { }, //SET_MODEL_NAME_INDEX,
            new byte [] { }, //GET_MODEL_NAME_INDEX,
            //new byte [] { }, //SET_EDID_INDEX,
            new byte [] { }, //GET_EDID_INDEX,
            //new byte [] { }, //SET_HDCP14_INDEX,
            new byte [] { }, //GET_HDCP14_INDEX,
            //new byte [] { }, //SET_HDCP2x_INDEX,
            new byte [] { }, //GET_HDCP2x_INDEX,
            //new byte [] { }, //SET_SERIAL_NUMBER_INDEX,
            new byte [] { }, //GET_SERIAL_NUMBER_INDEX,
            new byte [] { }, //GET_FW_VERSION_INDEX,
            new byte [] { }, //GET_FAC_EEPROM_DATA_INDEX,
            
            //// BenQ Command
            //new byte [] { }, //SET_BENQ_MODEL_NAME_INDEX,
            //new byte [] { }, //SET_BENQ_SERIAL_NAME_INDEX,
            //new byte [] { }, //SET_BENQ_FW_VERSION_INDEX,
            //new byte [] { }, //SET_BENQ_MONITOR_ID_INDEX,
            //new byte [] { }, //SET_BENQ_DNA_VERSION_INDEX,
            //new byte [] { }, //SET_BENQ_MANUFACTURE_YEARANDDATE_INDEX,
            //new byte [] { }, //SET_BENQ_EEPROM_INIT_INDEX,
            //new byte [] { }, //GET_BENQ_EEPROM_INDEX,
       };


        private bool Send_and_Receive_Packet(command_index cmd_index, byte[] data_array)
        {
            return Send_and_Receive_Packet((int)cmd_index, data_array);
        }

        private bool Send_and_Receive_Packet(int cmd_index, byte[] data_array)
        {
            bool ret_value = false;

            // prepare and send get_value 
            int PACKET_INDEX = (int)cmd_index;
            int packet_len = Command_Packet[PACKET_INDEX].Length;
            for (int index = 0; index < data_array.Length; index++)
            {
                Command_Packet[PACKET_INDEX][index + 4] = data_array[index];
            }
            Command_Packet[PACKET_INDEX][packet_len - 1] = XOR_Byte(Command_Packet[PACKET_INDEX], (packet_len - 1));
            serialPort1.Write(Command_Packet[PACKET_INDEX], 0, packet_len);

            // wait until packet received
            flag_wait_for_receive = true;
            while (flag_wait_for_receive) { }

            if (dataListbyte.Count() > 0)
            {
                ret_value = true;
            }

            return ret_value;
        }

        public static byte XOR_Byte(byte[] bHEX1, int length)
        {
            byte bHEX_OUT = bHEX1[0];
            for (int i = 1; i < length; i++)
            {
                bHEX_OUT = (byte)(bHEX_OUT ^ bHEX1[i]);
            }
            return bHEX_OUT;
        }

        public static byte XOR_List(List<byte> bHEX1, int length)
        {
            byte bHEX_OUT = bHEX1[0];
            for (int i = 1; i < length; i++)
            {
                bHEX_OUT = (byte)(bHEX_OUT ^ bHEX1[i]);
            }
            return bHEX_OUT;
        }
    }
}
