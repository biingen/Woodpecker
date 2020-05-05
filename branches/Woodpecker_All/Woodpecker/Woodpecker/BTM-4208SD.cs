using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Woodpecker
{
    public class Temperature_Data
    {
        private byte dtc_high;
        private byte dtc_low;
        private byte status_of_dtc;

        public Temperature_Data(byte channel, double list, bool shot, bool pause)
        {
            temperatureChannel = channel;
            temperatureList = list;
            temperatureShot = shot;
            temperaturePause = pause;
        }

        public static float addTemperature
        {
            get; set;
        }

        public static float initialTemperature
        {
            get; set;
        }

        public static float finalTemperature
        {
            get; set;
        }

        public static int temperatureDuringtime
        {
            get; set;
        }

        public byte temperatureChannel
        {
            get; set;
        }

        public double temperatureList
        {
            get; set;
        }

        public bool temperatureShot
        {
            get; set;
        }

        public bool temperaturePause
        {
            get; set;
        }
    }

    class BTM_4208SD
    {

    }
}
