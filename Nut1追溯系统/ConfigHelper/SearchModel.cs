using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using 卓汇数据追溯系统;

namespace 卓汇数据追溯系统.Models
{
    [Serializable]
    public class SearchModel:ViewModelBase
    {
      
        public int Index { set; get; }
        public string ImgUrl { set; get; }
        public string LeiBie { set; get; }
        public string LinkObj { set; get; }
        public string Start { set; get; }
        public string End { set; get; }
        public List<string> Value { set; get; }

        public string DefaultValue { set; get; }

    }
}
