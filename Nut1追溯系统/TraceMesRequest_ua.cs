﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 卓汇数据追溯系统
{
 
    public class TraceMesRequest_ua
    {
        public SN serials
        {
            get;
            set;
        }

        public data data
        {
            get;
            set;
        }
    }

    public class SN
    {
        public string band
        {
            get;
            set;
        }
        public string sp
        {
            get;
            set;
        }
    }

    public class data
    {
        public Insight insight
        {
            get;
            set;
        }
        public dynamic items
        {
            get;
            set;
        }
    }

    public class Insight
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
        public Uut_attributes uut_attributes
        {
            get;
            set;
        }
        public Result[] results
        {
            get;
            set;
        }
    }

    public class Test_attributes
    {
        public string test_result
        {
            get;
            set;
        }
        public string unit_serial_number
        {
            get;
            set;
        }
        public string uut_start
        {
            get;
            set;
        }
        public string uut_stop
        {
            get;
            set;
        }
    }

    public class Test_station_attributes
    {
        public string fixture_id
        {
            get;
            set;
        }
        public string head_id
        {
            get;
            set;
        }
        public string line_id
        {
            get;
            set;
        }
        public string software_name
        {
            get;
            set;
        }
        public string software_version
        {
            get;
            set;
        }
        public string station_id
        {
            get;
            set;
        }
    }

    public class Uut_attributes
    {
        public string STATION_STRING
        {
            get;
            set;
        }
        public string fixture_id
        {
            get;
            set;
        }
        public string location1
        {
            get;
            set;
        }
        public string laser_machine_id
        {
            get;
            set;
        }
        public string laser_pulse_profile_measure_setting
        {
            get;
            set;
        }
        public string actual_power_judgment
        {
            get;
            set;
        }
        public string actual_power_measure_time
        {
            get;
            set;
        }
        public string location1_layer1_pulse_profile
        {
            get;
            set;
        }
        public string location1_layer2_pulse_profile
        {
            get;
            set;
        }
        public string location2
        {
            get;
            set;
        }
        public string location2_layer1_pulse_profile
        {
            get;
            set;
        }
        public string location2_layer2_pulse_profile
        {
            get;
            set;
        }
        public string location3
        {
            get;
            set;
        }
        public string location3_layer1_pulse_profile
        {
            get;
            set;
        }
        public string location3_layer2_pulse_profile
        {
            get;
            set;
        }
        public string location4
        {
            get;
            set;
        }
        public string location4_layer1_pulse_profile
        {
            get;
            set;
        }
        public string location4_layer2_pulse_profile
        {
            get;
            set;
        }
        public string location5
        {
            get;
            set;
        }
        public string location5_layer1_pulse_profile
        {
            get;
            set;
        }
        public string location5_layer2_pulse_profile
        {
            get;
            set;
        }
        public string location6
        {
            get;
            set;
        }
        public string location6_layer1_pulse_profile
        {
            get;
            set;
        }
        public string location6_layer2_pulse_profile
        {
            get;
            set;
        }
        public string ml_result
        {
            get;
            set;
        }
        public string precitec_grading
        {
            get;
            set;
        }
        public string precitec_rev
        {
            get;
            set;
        }
        public string pattern_type
        {
            get;
            set;
        }
        public string precitec_value
        {
            get;
            set;
        }
        public string spot_size
        {
            get;
            set;
        }
        public string hatch
        {
            get;
            set;
        }
        public string swing_amplitude
        {
            get;
            set;
        }
        public string swing_freq
        {
            get;
            set;
        }
        public string full_sn
        {
            get;
            set;
        }
        public string tossing_item
        {
            get;
            set;
        }
        public string laser_start_time
        {
            get;
            set;
        }
        public string laser_stop_time
        {
            get;
            set;
        }
    }


    public class Result
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
        public string sub_sub_test
        {
            get;
            set;
        }
        public string sub_test
        {
            get;
            set;
        }

    }

}
