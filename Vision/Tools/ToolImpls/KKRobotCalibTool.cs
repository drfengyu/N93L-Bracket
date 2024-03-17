using Cognex.VisionPro;
using Cognex.VisionPro.CalibFix;
using System;
using System.ComponentModel;
using System.IO;
using Vision.Core;
using Vision.Projects;
using Vision.Stations;
using Vision.Tools.Interfaces;

namespace Vision.Tools.ToolImpls
{
    [Serializable]
    [GroupInfo("标定工具",1)]
    [ToolName("kk机械手标定",2)]
    [Description("kk轴和机械手的标定工具")]
    public class KkRobotCalibTool : ToolBase, IVpp, IRobotDeltaPoint
    {
        public KkRobotCalibTool()
        {
        }

        [NonSerialized]
        private Station _station;

        /// <summary>
        /// vpp是否加载
        /// </summary>
        [field: NonSerialized]
        public bool IsLoaded { get; private set; }

        /// <summary>
        /// vpp路径
        /// </summary>
        public string ToolPath { get; set; }

        /// <summary>
        /// 机械手的偏移值
        /// </summary>
        [field: NonSerialized]
        public PointD RobotDelta { get; set; }

        /// <summary>
        /// plc中存储kk x的地址
        /// </summary>
        public string AddressX { get; set; }

        /// <summary>
        /// plc中存储kk y的地址
        /// </summary>
        public string AddressY { get; set; }

        /// <summary>
        /// kk初始坐标
        /// </summary>
        public PointD KKOriginPosition { get; set; }

        /// <summary>
        /// 9点标定vpp
        /// </summary>
        [field: NonSerialized]
        public CogCalibNPointToNPointTool CalibTool { get; set; }

        #region 【工具相关】

        /// <summary>
        /// 工具运行 -- 计算机械手的偏移值
        /// </summary>
        /// <exception cref="ToolException"></exception>
        public override void Run()
        {
            if(!Enable) return;
            if(CalibTool == null)
            {
                throw new ToolException("CogCalibNPointToNPointTool不存在！");
            }

            //先获取kk的原始坐标
            var kkOrigin = KKOriginPosition;

            //读取现在的kk坐标
            if(string.IsNullOrEmpty(AddressX) || string.IsNullOrEmpty(AddressY))
            {
                throw new ToolException("kk坐标的地址不存在！");
            }

            var plc = ProjectManager.Instance.ProjectData.MxPlc;
            if (plc == null)
            {
                throw new ToolException("plc未连接！");
            }




            var x = plc.ReadDouble(AddressX,4);
            var y = plc.ReadDouble(AddressY,4);

            //LogUI.AddLog($"KK当前坐标 x:{x} y:{y}");

            //根据当前的KK坐标 计算 机械手坐标
            var cogtranform2DLinear = CalibTool.Calibration.GetComputedUncalibratedFromCalibratedTransform();
            cogtranform2DLinear.InvertBase().MapPoint(x,y,out var currentRx,out var currentRy);

            //初始点kk坐标 计算 机械手坐标
            cogtranform2DLinear.InvertBase().MapPoint(kkOrigin.X,kkOrigin.Y,out var originRx,out var originRy);

            //机械手deta值
            double deltaRx = currentRx - originRx;
            double deltaRy = currentRy - originRy;

            //LogUI.AddLog($"机械手delta值 x:{deltaRx.ToString("f3")} y:{deltaRy.ToString("f3")}");

            RobotDelta = new PointD(deltaRx,deltaRy);
        }

        /// <summary>
        /// 关闭工具
        /// </summary>
        public override void Close()
        {
            base.Close();
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
                    CalibTool = CogSerializer.LoadObjectFromFile(ToolPath) as CogCalibNPointToNPointTool;
                    IsLoaded = true;
                }
                catch(Exception ex)
                {
                    throw new Exception($"工具vpp加载失败.\r\n{ex.Message}");
                }
            }
        }

        /// <summary>
        /// 保存vpp
        /// </summary>
        public void SaveVpp()
        {
            if(IsLoaded)
            {
                CogSerializer.SaveObjectToFile(CalibTool,ToolPath);
            }
        }

        /// <summary>
        /// 创建vpp
        /// </summary>
        /// <exception cref="Exception">标定文件不存在</exception>
        public void CreateVpp()
        {
            if(!IsLoaded)
            {
                if(string.IsNullOrEmpty(ToolPath))
                {
                    throw new Exception("标定文件的路径不存在");
                }
                CalibTool = new CogCalibNPointToNPointTool();
                IsLoaded = true;
            }
        }

        #endregion
    }
}
