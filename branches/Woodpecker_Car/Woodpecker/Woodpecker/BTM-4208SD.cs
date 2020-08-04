using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Woodpecker
{
    class Temperature_Measure
    {
        private byte temperature_channel;
        private double temperature_value;
        private int temperature_interval;
        private string temperature_unit;
        private bool temperature_shot;
        private bool temperature_pause;
        private bool temperature_stop;
        private bool temperature_acrestart;
        private bool temperature_mail;

        public Temperature_Measure(byte channel, double temperature, int interval, string unit, bool shot, bool pause, bool stop, bool acrestart, bool mail)
        {
            temperature_channel = channel;
            temperature_value = temperature;
            temperature_interval = interval;
            temperature_unit = unit;
            temperature_shot = shot;
            temperature_pause = pause;
            temperature_stop = stop;
            temperature_acrestart = acrestart;
            temperature_mail = mail;
        }
    }
}
