using System;

namespace Vision.Core
{
    /// <summary>
    /// 图像格式
    /// </summary>
    public enum ImageType
    {
        BMP,
        JPG,
    }

    /// <summary>
    /// 结果格式
    /// </summary>
    [Serializable]
    public enum ResultType
    {
        Bool,
        Short,
        Int,
        Double,
        String,
    }
}
