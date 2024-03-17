using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 卓汇数据追溯系统
{
    public class ErrorData
    {
        public Guid GUID { get; set; }
        public string EMT { get; set; }
        public string ClientPcName { get; set; }
        public string MAC { get; set; }
        public string Sw_version { get; set; }
        public string IP { get; set; }
        public int PoorNum { get; set; }
        public int TotalNum { get; set; }
        public string EventTime { get; set; }
        public string errorStatus
        {
            get;
            set;
        }
        public string errorCode
        {
            get;
            set;
        }
        public string errorinfo
        {
            get;
            set;
        }
        public string ModuleCode
        {
            get;
            set;
        }
        public string Moduleinfo
        {
            get;
            set;
        }
        public string start_time
        {
            get;
            set;
        }
        public string stop_time
        {
            get;
            set;
        }
    }
}
