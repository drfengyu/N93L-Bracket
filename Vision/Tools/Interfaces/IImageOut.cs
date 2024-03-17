using Cognex.VisionPro;

namespace Vision.Tools.Interfaces
{
    /// <summary>
    /// 图像输出接口
    /// 所有带输出图像的工具都实现此接口
    /// </summary>
    public interface IImageOut
    {
        /// <summary>
        /// 输出图像
        /// </summary>
        ICogImage ImageOut { get; }
    }
}
