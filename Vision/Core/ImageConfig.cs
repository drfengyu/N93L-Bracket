using System;

namespace Vision.Core
{
    /// <summary>
    /// 图像配置类
    /// </summary>
    [Serializable]
    public class ImageConfig
    {
        public ImageConfig()
        {
            IsSaveOKImage = true;
            IsSaveNGImage = true;
            SaveImageDir = "D:\\Images";
            IsDeleteBySize = false;
            IsDeleteByTime = false;
            DeleteSize = 10240;
            DeleteDayTime = 365;
        }

        /// <summary>
        /// 保存图像NG的文件夹
        /// </summary>
        public string SaveImageDir { get; set; }

        /// <summary>
        /// 是否保存NG图像
        /// </summary>
        public bool IsSaveNGImage { get; set; }

        /// <summary>
        /// 是否保存OK图像
        /// </summary>
        public bool IsSaveOKImage { get; set; }

        /// <summary>
        /// 按天删除
        /// </summary>
        public int DeleteDayTime { get; set; }

        /// <summary>
        /// 是否按天删除
        /// </summary>
        public bool IsDeleteByTime { get; set; }

        /// <summary>
        /// 按大小删除
        /// </summary>
        public int DeleteSize { get; set; }

        /// <summary>
        /// 是否按大小删除
        /// </summary>
        public bool IsDeleteBySize { get; set; }

        /// <summary>
        /// 深拷贝
        /// </summary>
        /// <returns></returns>
        public ImageConfig DeepClone()
        {
            return SerializerHelper.DeepClone(this);
        }
    }

    /// <summary>
    /// 系统配置类
    /// </summary>
    [Serializable]
    public class SystemConfig
    {
        /// <summary>
        /// 是否开机运行
        /// </summary>
        public bool AutoRun { get; set; }

        /// <summary>
        /// 心跳地址
        /// </summary>
        public string HeartAddress { get; set; }

        /// <summary>
        /// 联机地址
        /// </summary>
        public string OnlineAddress { get; set; }

        public SystemConfig()
        {
            HeartAddress = "D5000";
            OnlineAddress = "D5001";
        }
    }
}
