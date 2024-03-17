using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 卓汇数据追溯系统
{
     public class GetSNData
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
        public History[] history
        {
            get;
            set;
        }
        public Serials serials
        {
            get;
            set;
        }
        public PRoperties properties
        {
            get;
            set;
        }

    }
    public class History
    {
            
    }
    public class Serials
    {
        public string band { get; set; }
        public string bg { get; set; }
        public string fg { get; set; }
        public string sp { get; set; }
    }
    public class PRoperties
    {
        public string Build { get; set; }
        public string Color { get; set; }
        public string Project { get; set; }
        public string Sidefire { get; set; }
        public string DOE { get; set; }
        public string BG_vendor { get; set; }
    }

}
