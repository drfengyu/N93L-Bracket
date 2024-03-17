using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 卓汇数据追溯系统
{
    public class PqStatus
    {
        public string status { get; set; }
        public PqMSG[] msg { get; set; }
    }

    public class PqMSG
    {
        public string machineNo { get; set; }
        public bool isPQ { get; set; }
    }
}

