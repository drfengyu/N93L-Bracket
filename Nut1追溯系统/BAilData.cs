using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 卓汇数据追溯系统
{
   public class BAilData
    {
        public string SN;
        public DateTime Weld_start_time;
        public DateTime Weld_stop_time;
        public string Weld_wait_ct;
        public string Actual_weld_ct;
        public DateTime Start_Time;
        public DateTime Stop_Time;
        public string Fixture_id = "";
        public string full_sn = "";
        public int air_pressure = 0;
        public bool test_fail = true;
        public int auto_send = 0;
        public string test_result = null;
        public int station = 0;
        public string tossing_item = "";
        public string STATION_STRING = "";
    }
}
