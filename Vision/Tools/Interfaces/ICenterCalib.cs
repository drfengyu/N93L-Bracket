using Vision.Core;

namespace Vision.Tools.Interfaces
{
    /// <summary>
    /// 旋转标定接口
    /// 需要用到旋转中心的数据的可以继承此接口
    /// </summary>
    public interface ICenterCalib
    {
        /// <summary>
        /// 旋转中心点
        /// </summary>
        PointD CenterPoint { get; set; }

        /// <summary>
        /// 是否已经标定
        /// </summary>
        bool IsCalibed { get; set; }
    }
}
