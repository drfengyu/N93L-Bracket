using Cognex.VisionPro;
using System;
using Vision.Stations;

namespace Vision.Core
{
    /// <summary>
    /// station事件参数
    /// </summary>
    public class StationEventArgs : EventArgs
    {
        public Station Station { get; private set; }

        public StationEventArgs(Station s)
        {
            Station = s;
        }
    }

    /// <summary>
    /// 图像显示事件对象
    /// </summary>
    public class ShowWindowEventArgs : EventArgs
    {
        /// <summary>
        /// 运行时间
        /// </summary>
        public TimeSpan Time { get; private set; }

        /// <summary>
        /// 运行结果
        /// </summary>
        public bool Result { get; private set; }

        /// <summary>
        /// 是否有输入图像
        /// </summary>
        public bool IsNullImage { get; private set; }

        /// <summary>
        /// NG时 错误描述
        /// </summary>
        public string ErrorMsg { get; private set; }

        public ShowWindowEventArgs(bool res, TimeSpan time,bool nullImage,string errMsg)
        {
            Time = time;
            Result = res;
            ErrorMsg = errMsg;
            IsNullImage = nullImage;
        }
    }

    /// <summary>
    /// 图像显示放大缩小事件类
    /// </summary>
    public class StationShowChangedEventArgs : EventArgs
    {
        public bool ShowOne { get; set; }

        public string StationName { get; set; }

        public StationShowChangedEventArgs(string name, bool showOne)
        {
            ShowOne = showOne;
            StationName = name;
        }
    }
}
