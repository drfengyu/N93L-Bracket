using Vision.Core;

namespace Vision.Tools.Interfaces
{
    /// <summary>
    /// KK移动一定的距离计算出的机械手的偏移量
    /// </summary>
    public interface IRobotDeltaPoint
    {
        /// <summary>
        /// 机械手偏移值
        /// </summary>
        PointD RobotDelta { get; set; }
    }
}
