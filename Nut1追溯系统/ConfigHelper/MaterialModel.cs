using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 卓汇数据追溯系统.ConfigHelper
{
    public class MaterialRule
    {
        public string status { set; get; }
        public MSG msg { set; get; }

    }
    public class RouteStepInfo
    {
        public string ip;
        public string type;
        public string clientVer;
        public string softwareVer;
        public string line;
        public string config;
    }
    public class MSG
    {
        public MaterialRuleData[] materialCheckRules
        {
            get;
            set;
        }
        public RouteStepInfo[] routeStepInfo
        {
            get;
            set;
        }
    }
    public class MaterialRuleData
    {

        public string type
        {
            get;
            set;
        }
        public string linkObj
        {
            get;
            set;
        }
        public string begin
        {
            get;
            set;
        }
        public string end
        {
            get;
            set;
        }
        public string[] value
        {
            get;
            set;
        }
        public string currentSeeting
        {
            get;
            set;
        }

    }


    public class linkDatass
    {
        public List<linkData> linkData { set; get; }

    }
    public class linkData
    {
        public string linkObj;
        public string linkValue;
        public string linkTime;
    }
}
