using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 卓汇数据追溯系统
{
    public class GetTestResult
    {

        public bool status
        {
            get;
            set;
        }
        public msg[] msg
        {
            get;
            set;
        }

    }
    public class msg
    {
        public string pass
        {
            get;
            set;
        }
        public string fail
        {
            get;
            set;
        }

    }
}
