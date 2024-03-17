using System;
using System.ComponentModel;
using System.IO;
using Cognex.VisionPro;
using Vision.Core;
using Vision.Stations;
using Vision.Tools.Interfaces;

namespace Vision.Tools.ToolImpls
{
    [Serializable]
    [GroupInfo("图像工具", 0)]
    [ToolName("相机采集",0)]
    [Description("通过相机进行采集的康耐视工具")]
    public class ImageAcqTool : ToolBase, IImageOut, IVpp
    {
        [NonSerialized]
        private Station _station;

        public string ToolPath { get; set; }

        [field: NonSerialized]
        public ICogImage ImageOut { get; private set; }

        /// <summary>
        /// 采集工具
        /// </summary>
        [field: NonSerialized]
        public CogAcqFifoTool AcqFifoTool { get; set; }

        [field: NonSerialized]
        public bool IsLoaded { get;private set; }

        public ImageAcqTool()
        {
        }

        #region 【vpp相关】
        /// <summary>
        /// 加载vpp
        /// </summary>
        /// <exception cref="Exception"></exception>
        public void LoadVpp()
        {
            if(!IsLoaded)
            {
                try
                {
                    AcqFifoTool = CogSerializer.LoadObjectFromFile(ToolPath) as CogAcqFifoTool;
                    IsLoaded = true;
                }
                catch(Exception ex)
                {
                    throw new Exception($"相机加载失败！" + ex.Message);
                }
            }
        }

        /// <summary>
        /// 保存vpp
        /// </summary>
        /// <exception cref="Exception"></exception>
        public void SaveVpp()
        {
            if(IsLoaded)
            {
                try
                {
                    CogSerializer.SaveObjectToFile(AcqFifoTool,ToolPath);
                }
                catch(Exception ex)
                {
                    throw new Exception($"相机保存失败！" + ex.Message);
                }
            }
        }

        /// <summary>
        /// 创建vpp
        /// </summary>
        /// <exception cref="Exception"></exception>
        public void CreateVpp()
        {
            if(!IsLoaded)
            {
                if(string.IsNullOrEmpty(ToolPath))
                {
                    throw new Exception("相机的路径不存在");
                }
                AcqFifoTool = new CogAcqFifoTool();
                IsLoaded = true;
            }
        }
        #endregion

        #region 【工具相关】

        /// <summary>
        /// 运行工具
        /// </summary>
        public override void Run()
        {
            if(!Enable) return;
            if(AcqFifoTool == null)
                throw new ToolException("AcqFifoTool为null");
            AcqFifoTool.Run();
            if(AcqFifoTool.RunStatus.Result != CogToolResultConstants.Accept)
            {
                throw new ToolException("相机取像失败！");
            }
            ImageOut = AcqFifoTool.OutputImage;
        }

        /// <summary>
        /// 关闭工具
        /// </summary>
        public override void Close()
        {
            base.Close();
            if(AcqFifoTool != null && AcqFifoTool.Operator != null &&
               AcqFifoTool.Operator.FrameGrabber != null)
            {
                AcqFifoTool.Operator.FrameGrabber.Disconnect(true);
                AcqFifoTool.Dispose();
                AcqFifoTool = null;
            }
            _station.StationNameChangedEvent -= Station_StationNameChanged;
        }

        /// <summary>
        /// 注册station
        /// </summary>
        /// <param name="station"></param>
        public void RegisterStation(Station station)
        {
            station.StationNameChangedEvent += Station_StationNameChanged;
            _station = station;
        }

        /// <summary>
        /// station名称改变事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Station_StationNameChanged(object sender,StationEventArgs e)
        {
            ToolPath = Path.Combine(e.Station.StationPath,$"{Name}.vpp");
        }

        #endregion
    }
}