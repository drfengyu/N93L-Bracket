using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 卓汇数据追溯系统
{
    public class GetUserInfo
    {
        public bool status { get; set; }
        public string msg { get; set; }
    }

    public class GetUserInfo2
    {
        public bool status { get; set; }
        public MSGData[] msg { get; set; }
    }

    public class MSGData
    {
        public string id { get; set; }

        public string type { get; set; }
    }
}
