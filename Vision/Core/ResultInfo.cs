using System;

namespace Vision.Core
{
    /// <summary>
    /// 结果类
    /// </summary>
    [Serializable]
    public class ResultInfo
    {
        public string Source { get; set; }

        public ResultType Type { get; set; }

        public string Address { get; set; }

        [field: NonSerialized]
        public object Value { get; set; }

        public ResultInfo()
        {

        }

        public ResultInfo(string source,ResultType type,string address)
        {
            Type = type;
            Source = source;
            Address = address;
        }
    }
}
