using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 卓汇数据追溯系统
{
    public class FiveQData
    {
        public bool status { get; set; }
        public MSG[] msg { get; set; }
    }
    public class MSG
    {
        public string name { get; set; }
        public string shiftday { get; set; }
    }
}
