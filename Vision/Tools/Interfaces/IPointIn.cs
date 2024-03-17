using Vision.Core;

namespace Vision.Tools.Interfaces
{
    /// <summary>
    /// 需要进行计算的点位的数据接口
    /// </summary>
    public interface IPointIn
    {
        /// <summary>
        /// 输入点位
        /// </summary>
        PointA PointIn { get; set; }
    }
}
