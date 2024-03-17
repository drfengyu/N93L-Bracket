using System.Collections.Generic;
using Vision.Core;

namespace Vision.Tools.Interfaces
{
    /// <summary>
    /// 结果工具接口 
    /// </summary>
    public interface IResult
    {
        /// <summary>
        /// 运行的结果数据
        /// </summary>
        List<ResultInfo> ResultData { get; }
    }
}
