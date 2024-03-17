using Cognex.VisionPro;

namespace Vision.Tools.Interfaces
{
    /// <summary>
    /// 图像输入接口
    /// 所有需要输入图像的工具都实现此接口
    /// </summary>
    public interface IImageIn
    {
        /// <summary>
        /// 输入图像
        /// </summary>
        ICogImage ImageIn { get; set; }

        /// <summary>
        /// 输入图像工具名称
        /// </summary>
        string ImageInName { get; set; }
    }
}
