using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 卓汇数据追溯系统
{
    public class GetCount
    {
        public string age
        {
            get;
            set;
        }
        public string project
        {
            get;
            set;
        }
        public string component
        {
            get;
            set;
        }
        public string created
        {
            get;
            set;
        }
        public History2[] history
        {
            get;
            set;
        }
        public Serials2 serials
        {
            get;
            set;
        }
        public PRoperties2 properties
        {
            get;
            set;
        }

    }
    public class History2
    {
        public string id { get; set; }
        public Data2 data { get; set; }
        public string Event { get; set; }
        public string created { get; set; }
        public Serials2 serials { get; set; }
        public dynamic defects { get; set; }
        public string agent_id { get; set; }
        public string process_id { get; set; }
        public string component_id { get; set; }

    }
    public class Serials2
    {
        public string band { get; set; }

    }
    public class PRoperties2
    {
        public string Build { get; set; }
        public string Color { get; set; }
        public string Campus { get; set; }
        public string Sidefire { get; set; }
        public string Material { get; set; }

    }

    public class Data2
    {
        public dynamic items { get; set; }
        public Insight3 insight { get; set; }

    }
    public class Insight3
    {
        public Test_attributes test_attributes
        {
            get;
            set;
        }
        public Test_station_attributes test_station_attributes
        {
            get;
            set;
        }
        public Uut_attributes3 uut_attributes
        {
            get;
            set;
        }
        public Result3[] results
        {
            get;
            set;
        }

    }
    public class Result3
    {
        public string result
        {
            get;
            set;
        }
        public string test
        {
            get;
            set;
        }
        public string units
        {
            get;
            set;
        }
        public string value
        {
            get;
            set;
        }

    }
    public class Uut_attributes3
    {
        public string hatch { get; set; }
        public string full_sn { get; set; }
        public string spot_size { get; set; }
        public string fixture_id { get; set; }
        public string swing_freq { get; set; }
        public string pattern_type { get; set; }
        public string precitec_rev { get; set; }
        public string tossing_item { get; set; }
        public string STATION_STRING { get; set; }
        public string precitec_value { get; set; }
        public string laser_stop_time { get; set; }
        public string Mateswing_amplituderial { get; set; }
        public string laser_start_time { get; set; }
        public string precitec_grading { get; set; }

    }
}
