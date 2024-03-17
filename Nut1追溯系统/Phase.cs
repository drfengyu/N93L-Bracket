using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 卓汇数据追溯系统
{
    class Phase
    {
        public string age { get; set; }
        public string project { get; set; }
        public string componen { get; set; }
        public DateTime created { get; set; }
        public History1[] history { get; set; }
        public Serials1 serials { get; set; }
        public ProperTies properties { get; set; }

}
    public class History1
    {
        public string id { get; set; }
        public string data { get; set; }
        public string Event { get; set; }
        public string created { get; set; }
        public Serials serials { get; set; }
        public Defects defects { get; set; }
        public string agent_id { get; set; }
        public string process_id { get; set; }
        public string component_id { get; set; }
    }
    public class Serials1
    {
        public string band { get; set; }
    }
    public class Defects
    {

    }
    public class ProperTies
    {
        public string Build { get; set; }
        public string Color { get; set; }
        public string Phase { get; set; }
        public string Sidefire { get; set; }
    }


}
