using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 卓汇数据追溯系统.Models
{
    public class Laseripqc { 
        public serials serials { set; get; }
        public data data { set; get; }
        }
    public class serials { 
        public string band { set; get; }
    }
    public class data { 
        public items items { set; get; }
    }
    public class items
    {
        public string actual_power { set; get; }
        public string actual_power_judgment { set; get; }
        public string actual_power_measure_time { set; get; }
        public string laser_settings_frequency { set; get; }
        public string laser_settings_laser_speed { set; get; }
        public string laser_settings_peak_power { set; get; }
        public string laser_settings_power { set; get; }
        public string laser_settings_pulse_energy { set; get; }
        public string laser_settings_q_release { set; get; }
        public string laser_settings_waveform { set; get; }
        public string machine_RFID { set; get; }
        public string machine_id { set; get; }
        public string power_ll { set; get; }
        public string power_ul { set; get; }
        public string process_name { set; get; }
        public string station_id { set; get; }
    }
}
