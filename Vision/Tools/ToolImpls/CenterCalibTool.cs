using System;
using System.ComponentModel;
using System.IO;
using Cognex.VisionPro;
using Cognex.VisionPro.CalibFix;
using Cognex.VisionPro.Caliper;
using Cognex.VisionPro.PMAlign;
using Cognex.VisionPro.ToolBlock;
using Vision.Core;
using Vision.Projects;
using Vision.Stations;
using Vision.Tools.Interfaces;

namespace Vision.Tools.ToolImpls
{
    [Serializable]
    [ToolName("旋转标定", 1)]
    [GroupInfo(name: "标定工具", index: 1)]
    [Description("旋转中心标定工具")]
    public class CenterCalibTool : ToolBase, IVpp, ICenterCalib, IPointIn, IPointOut
    {
        //DetectTool工具完成得到的点位 传入此旋转工具进行计算
        [NonSerialized]
        private Station _station;

        /// <summary>
        /// 工具路径
        /// </summary>
        public string ToolPath { get; set; }

        /// <summary>
        /// 是否标定
        /// </summary>
        public bool IsCalibed { get; set; }

        /// <summary>
        /// 旋转中心点
        /// </summary>
        public PointD CenterPoint { get; set; }

        /// <summary>
        /// 输入数据的工具名称
        /// </summary>
        public string InputModelDataToolName { get; set; }

        /// <summary>
        /// 输入机器人偏移工具名称
        /// </summary>
        public string InputRobotDeltaToolName { get; set; }

        /// <summary>
        /// 9点标定偏移工具名称
        /// </summary>
        public string InputNPointDeltaToolName { get; set; }

        /// <summary>
        /// vpp是否加载成功
        /// </summary>
        [field: NonSerialized]
        public bool IsLoaded { get; set; }

        /// <summary>
        /// 标定vpp 标定时使用
        /// </summary>
        [field: NonSerialized]
        public CogToolBlock ToolBlock { get; set; }

        /// <summary>
        /// 需要进行旋转计算的点
        /// </summary>
        [field: NonSerialized]
        public PointA PointIn { get; set; }

        /// <summary>
        /// 由KK移动 计算得到的机械手的 偏移量
        /// </summary>
        [field: NonSerialized]
        public PointD RobotDelta { get; set; }

        /// <summary>
        /// 机械手旋转中心的偏移值
        /// </summary>
        [field: NonSerialized]
        public PointD CenterRobotDelta { get; set; }

        /// <summary>
        /// 输出结果
        /// </summary>
        [field: NonSerialized]
        public PointA PointOut { get; set; }

        public CenterCalibTool()
        {
        }

        #region 工具运行相关
        /// <summary>
        /// 旋转中心计算
        /// </summary>
        public override void Run()
        {
            if (!Enable) return;
            GetData();
            PointOut = GetCenterCalibPoint();
        }

        /// <summary>
        /// 关闭
        /// </summary>
        public override void Close()
        {
            base.Close();
            CloseCam();
            _station.StationNameChangedEvent -= Station_StationNameChanged;
        }

        /// <summary>
        /// 注册station到此工具
        /// </summary>
        /// <param name="station"></param>
        public void RegisterStation(Station station)
        {
            station.StationNameChangedEvent += Station_StationNameChanged;
            _station = station;
        }

        /// <summary>
        /// 计算当机械手旋转后的坐标
        /// </summary>
        /// <returns></returns>
        private PointA GetCenterCalibPoint()
        {
            try
            {
                if (!IsCalibed)
                    throw new Exception("旋转中心未标定！");

                #region 带旋转标定


                //相当于将机械手从原模板位移动到现在的位置
                var a1 = PointIn.Angle;
                var a2 = _station.ModelPosition.Angle;
                //if (a2 < -Math.PI / 2)
                //{
                //    a2 = a2 + Math.PI;
                //}

                var deltaAngle = a1 - a2;


                //计算模板点的实际坐标绕旋转中心旋转后得到的新的坐标
                //RotatedAffine.Math_Transfer(PointIn.X, PointIn.Y, deltaAngle,
                //    CenterPoint.X + CenterRobotDelta.X, CenterPoint.Y + CenterRobotDelta.Y,
                //    out var rotatedX, out var rotatedY);

                // 旋转后的坐标 - 模板的坐标 = delta
                //var deltaX = rotatedX - _station.ModelPosition.X ;
                //var deltaY = rotatedY - _station.ModelPosition.Y ;


                RotatedAffine.Math_Transfer(_station.ModelPosition.X, _station.ModelPosition.Y, deltaAngle,
                   CenterPoint.X + CenterRobotDelta.X, CenterPoint.Y + CenterRobotDelta.Y,
                   out var rotatedX, out var rotatedY);

                var deltaX = PointIn.X - rotatedX;
                var deltaY = PointIn.Y - rotatedY;


                //系统补偿
                var offset = GetOffset();

                PointA point = new PointA();

                //机械手示教位 + 系统补偿 + kk机械手的偏移量+ delta 
                point.X = (_station.RobotOriginPosition.X + offset.X) + RobotDelta.X + deltaX;
                point.Y = (_station.RobotOriginPosition.Y + offset.Y) + RobotDelta.Y + deltaY;
                //角度就是当前角度 - 模板角度
                point.Angle = deltaAngle * 180 / Math.PI + _station.RobotOriginPosition.Angle;

                LogUI.AddLog($"机械手坐标 {point}");
                return point;
                #endregion

                #region 不带旋转标定
                //var deltaAngle = (PointIn.Angle * 180) / Math.PI - (_station.ModelPosition.Angle * 180) / Math.PI;
                ////计算模板点的实际坐标绕旋转中心旋转后得到的新的坐标
                ////RotatedAffine.Math_Transfer(PointIn.X, PointIn.Y, deltaAngle,
                ////    CenterPoint.X, CenterPoint.Y, out var rotatedX, out var rotatedY);

                ////模板的坐标 - 旋转后的坐标 = delta
                //var deltaX = _station.ModelPosition.X - PointIn.X;
                //var deltaY = _station.ModelPosition.Y - PointIn.Y;


                //PointA point = new PointA();

                ////机械手示教位+ kk机械手的偏移量+ delta
                //point.X = _station.RobotOriginPosition.X + RobotDelta.X + deltaX;
                //point.Y = _station.RobotOriginPosition.Y + RobotDelta.Y + deltaY;
                //point.Angle = deltaAngle + _station.RobotOriginPosition.Angle;

                //LogUI.AddLog($"计算结果 机械手坐标 {point.ToString()}");
                //return point;
                #endregion
            }
            catch (Exception ex)
            {
                ex.Message.MsgBox();
                return null;
            }
        }

        /// <summary>
        /// 获取此前工具的输出数据 和机械手偏差值 
        /// </summary>
        private void GetData()
        {
            var modelTool = _station[InputModelDataToolName];
            var robotDeltaTool = _station[InputRobotDeltaToolName];
            if (modelTool == null)
                throw new ToolException("旋转标定的模板点位未设置！");
            if (robotDeltaTool == null)
                throw new ToolException("旋转标定的机械手补偿值未设置！");
            PointIn = ((IModelPoint)modelTool).ModelPoint ?? new PointA();
            RobotDelta = ((IRobotDeltaPoint)robotDeltaTool).RobotDelta ?? new PointD();

            if (CenterRobotDelta == null)
            {
                CenterRobotDelta = new PointD();
            }
            if (_station.CenterCalibRobotPoint == null)
            {
                _station.CenterCalibRobotPoint = new PointD();
            }
            CenterRobotDelta.X = _station.RobotOriginPosition.X - _station.CenterCalibRobotPoint.X;
            CenterRobotDelta.Y = _station.RobotOriginPosition.Y - _station.CenterCalibRobotPoint.Y;
        }

        private PointD GetOffset()
        {
            var point = new PointD();
            //if (ProjectManager.Instance.ProjectData.Offset == null)
            //{
            //    ProjectManager.Instance.ProjectData.Offset = new PointD();
            //}
            point = ProjectManager.Instance.ProjectData.Offset;
            return point;
        }

        #endregion 

        #region vpp相关

        /// <summary>
        /// 创建vpp
        /// </summary>
        /// <exception cref="Exception"></exception>
        public void CreateVpp()
        {
            if (!IsLoaded)
            {
                if (string.IsNullOrEmpty(ToolPath))
                {
                    throw new Exception("vpp的路径不存在");
                }

                ToolBlock = new CogToolBlock();
                IsLoaded = true;
            }
        }

        /// <summary>
        /// 加载vpp
        /// </summary>
        /// <exception cref="Exception"></exception>
        public void LoadVpp()
        {
            if (!IsLoaded)
            {
                try
                {
                    ToolBlock = CogSerializer.LoadObjectFromFile(ToolPath) as CogToolBlock;
                    IsLoaded = true;
                }
                catch (Exception ex)
                {
                    throw new Exception($"工具vpp加载失败.\r\n{ex.Message}");
                }
            }
        }

        /// <summary>
        /// 添加9点标定vpp工具
        /// </summary>
        /// <param name="nPointCalibTool"></param>
        public void AddNPointCalibTool(NPointCalibTool nPointCalibTool)
        {
            if (IsLoaded)
            {
                if (nPointCalibTool != null)
                {
                    var tb = (CogCalibNPointToNPointTool)nPointCalibTool.ToolBlock.Tools["CogCalibNPointToNPointTool1"];
                    AddAcqTool(ToolBlock);
                    ToolBlock.Tools.Add(tb);
                    AddTools(ToolBlock);
                }
            }
        }

        /// <summary>
        /// 移除9点标定vpp工具
        /// </summary>
        public void RemoveNPointCalibTool()
        {
            if (IsLoaded)
            {
                int count = ToolBlock.Tools.Count;
                for (int i = 0; i < count; i++)
                {
                    ToolBlock.Tools.RemoveAt(0);
                }
            }
        }

        public void CloseCam()
        {
            if (ToolBlock.Tools.Contains("CogAcqFifoTool1"))
            {
                var acqTool = ToolBlock.Tools["CogAcqFifoTool1"] as CogAcqFifoTool;
                if (acqTool != null && acqTool.Operator != null && acqTool.Operator.FrameGrabber != null)
                {
                    acqTool.Operator.FrameGrabber.Disconnect(true);
                    acqTool.Dispose();
                }
            }
        }

        /// <summary>
        /// station名称改变事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Station_StationNameChanged(object sender, StationEventArgs e)
        {
            ToolPath = Path.Combine(e.Station.StationPath, $"{Name}.vpp");
        }

        /// <summary>
        /// 添加工具
        /// </summary>
        /// <param name="tb"></param>
        private void AddTools(CogToolBlock tb)
        {
            CogPMAlignTool pmaTool = new CogPMAlignTool();
            pmaTool.Name = "CogPMAlignTool1";
            string[] s1 = new string[1];
            string[] s2 = new string[5];
            s1[0] = "|InputImage|InputImage";

            s2[0] = "|Results.Item[0].GetPose()|Results.Item[0].GetPose()";
            s2[1] = "|Results.Item[0].GetPose().TranslationX|Results.Item[0].GetPose().TranslationX";
            s2[2] = "|Results.Item[0].GetPose().TranslationY|Results.Item[0].GetPose().TranslationY";
            s2[3] = "|Results.Item[0].GetPose().Rotation|Results.Item[0].GetPose().Rotation";
            s2[4] = "|Results.Item[0].Score|Results.Item[0].Score";

            pmaTool.UserData.Add("_ToolInputTerminals", s1);//添加终端-InputImage
            pmaTool.UserData.Add("_ToolOutputTerminals", s2);

            CogFindCircleTool findCircleTool = new CogFindCircleTool();
            findCircleTool.Name = "CogFindCircleTool1";
            string[] s3 = new string[3];
            string[] s4 = new string[3];
            s3[0] = "|InputImage|InputImage";
            s3[1] = "|RunParams.ExpectedCircularArc.CenterX|RunParams.ExpectedCircularArc.CenterX";
            s3[2] = "|RunParams.ExpectedCircularArc.CenterY|RunParams.ExpectedCircularArc.CenterY";

            s4[0] = "|Results.GetCircle().CenterX|Results.GetCircle().CenterX";
            s4[1] = "|Results.GetCircle().CenterY|Results.GetCircle().CenterY";
            s4[2] = "|Results.GetCircle().Radius|Results.GetCircle().Radius";

            findCircleTool.UserData.Add("_ToolInputTerminals", s3);//添加终端-InputImage
            findCircleTool.UserData.Add("_ToolOutputTerminals", s4);


            //CogCalibCheckerboardTool cTool = new CogCalibCheckerboardTool();
            //cTool.Name = "CogCalibCheckerboardTool1";
            //string[] s1 = new string[1];
            //string[] s2 = new string[1];
            //s1[0] = "|InputImage|InputImage";
            //s2[0] = "|OutputImage|OutputImage";
            //cTool.UserData.Add("_ToolInputTerminals", s1);//添加终端-InputImage
            //cTool.UserData.Add("_ToolOutputTerminals", s2);

            CogFitCircleTool fitCircleTool = new CogFitCircleTool();
            fitCircleTool.Name = "CogFitCircleTool1";
            string[] s5 = new string[1];
            string[] s6 = new string[4];
            s5[0] = "|InputImage|InputImage";

            s6[0] = "|Result.GetCircle()|Result.GetCircle()";
            s6[1] = "|Result.GetCircle().CenterX|Result.GetCircle().CenterX";
            s6[2] = "|Result.GetCircle().CenterY|Result.GetCircle().CenterY";
            s6[3] = "|Result.GetCircle().Radius|Result.GetCircle().Radius";

            fitCircleTool.UserData.Add("_ToolInputTerminals", s5);//添加终端-InputImage
            fitCircleTool.UserData.Add("_ToolOutputTerminals", s6);


            tb.Tools.Add(pmaTool);
            tb.Tools.Add(findCircleTool);
            tb.Tools.Add(fitCircleTool);
        }

        /// <summary>
        /// 添加取像工具
        /// </summary>
        /// <param name="tb"></param>
        private void AddAcqTool(CogToolBlock tb)
        {
            CogAcqFifoTool acqTool = new CogAcqFifoTool();
            acqTool.Name = "CogAcqFifoTool1";
            string[] s1 = new string[1];
            s1[0] = "|OutputImage|OutputImage";

            acqTool.UserData.Add("_ToolOutputTerminals", s1);

            tb.Tools.Add(acqTool);
        }

        /// <summary>
        /// 保存vpp
        /// </summary>
        public void SaveVpp()
        {
            if (IsLoaded)
            {
                CogSerializer.SaveObjectToFile(ToolBlock, ToolPath);
            }
        }

        #endregion

    }
}
