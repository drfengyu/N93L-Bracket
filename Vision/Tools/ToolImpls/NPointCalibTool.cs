using System;
using System.ComponentModel;
using System.IO;
using Cognex.VisionPro;
using Cognex.VisionPro.CalibFix;
using Cognex.VisionPro.Caliper;
using Cognex.VisionPro.PMAlign;
using Cognex.VisionPro.ToolBlock;
using Vision.Core;
using Vision.Stations;
using Vision.Tools.Interfaces;

namespace Vision.Tools.ToolImpls
{
    [Serializable]
    [GroupInfo("标定工具", 1)]
    [ToolName("9点标定",0)]
    [Description("标准9点标定工具")]
    public class NPointCalibTool : ToolBase, IImageIn, IVpp, IImageOut, INPoint
    {
        //vp的9点标定会直接在图像中转换为实际坐标
        //所以输出的坐标为实际机械手标定时的对应的机械坐标

        public NPointCalibTool()
        {
        }

        [NonSerialized]
        private Station _station;

        public string ImageInName { get; set; }

        public string ToolPath { get; set; }

        [field: NonSerialized]
        public ICogImage ImageIn { get; set; }

        [field: NonSerialized]
        public CogToolBlock ToolBlock { get; set; }

        [field: NonSerialized]
        public bool IsLoaded { get; set; }

        [field: NonSerialized]
        public ICogImage ImageOut { get; private set; }

        #region 【vpp相关】
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
                ToolBlock.Inputs.Add(new CogToolBlockTerminal("InputImage", typeof(ICogImage)));
                ToolBlock.Outputs.Add(new CogToolBlockTerminal("OutputImage", typeof(ICogImage)));

                AddTools(ToolBlock);
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

        #region 【工具相关】

        /// <summary>
        /// 运行工具
        /// </summary>
        /// <exception cref="ToolException"></exception>
        public override void Run()
        {
            if (!Enable) return;
            GetImageIn();
            if (ImageIn != null)
            {
                if (ToolBlock != null)
                {
                    ToolBlock.Inputs["InputImage"].Value = ImageIn;
                    ToolBlock.Run();
                    if (ToolBlock.RunStatus.Result != CogToolResultConstants.Accept)
                    {
                        throw new ToolException($"工具[{Name}]运行失败！")
                        {
                            ToolName = Name,
                            RunError = true
                        };
                    }
                    ImageOut = (ICogImage)ToolBlock.Outputs["OutputImage"].Value;
                }
            }
        }

        /// <summary>
        /// 获取输入图像
        /// </summary>
        private bool GetImageIn()
        {
            if (_station == null || ImageInName == null) return false;

            var tool = _station[ImageInName];

            if (tool == null) return false;

            ImageIn = ((IImageOut)tool).ImageOut;
            return true;
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
        /// 工具名称改变事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Station_StationNameChanged(object sender, StationEventArgs e)
        {
            ToolPath = Path.Combine(e.Station.StationPath, $"{Name}.vpp");
        }

        /// <summary>
        /// 添加vpp工具
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

            CogFindCircleTool circleTool = new CogFindCircleTool();
            circleTool.Name = "CogFindCircleTool1";
            string[] s3 = new string[3];
            string[] s4 = new string[3];
            s3[0] = "|InputImage|InputImage";
            s3[1] = "|RunParams.ExpectedCircularArc.CenterX|RunParams.ExpectedCircularArc.CenterX";
            s3[2] = "|RunParams.ExpectedCircularArc.CenterY|RunParams.ExpectedCircularArc.CenterY";

            s4[0] = "|Results.GetCircle().CenterX|Results.GetCircle().CenterX";
            s4[1] = "|Results.GetCircle().CenterY|Results.GetCircle().CenterY";
            s4[2] = "|Results.GetCircle().Radius|Results.GetCircle().Radius";

            circleTool.UserData.Add("_ToolInputTerminals", s3);//添加终端-InputImage
            circleTool.UserData.Add("_ToolOutputTerminals", s4);


            //CogCalibCheckerboardTool cTool = new CogCalibCheckerboardTool();
            //cTool.Name = "CogCalibCheckerboardTool1";
            //string[] s1 = new string[1];
            //string[] s2 = new string[1];
            //s1[0] = "|InputImage|InputImage";
            //s2[0] = "|OutputImage|OutputImage";
            //cTool.UserData.Add("_ToolInputTerminals", s1);//添加终端-InputImage
            //cTool.UserData.Add("_ToolOutputTerminals", s2);

            CogCalibNPointToNPointTool nTool = new CogCalibNPointToNPointTool();
            nTool.Name = "CogCalibNPointToNPointTool1";
            string[] s5 = new string[1];
            string[] s6 = new string[1];
            s5[0] = "|InputImage|InputImage";

            s6[0] = "|OutputImage|OutputImage";

            nTool.UserData.Add("_ToolInputTerminals", s5);//添加终端-InputImage
            nTool.UserData.Add("_ToolOutputTerminals", s6);

            CogAcqFifoTool acqTool = new CogAcqFifoTool();
            acqTool.Name = "CogAcqFifoTool1";
            string[] s7 = new string[1];
            s7[0] = "|OutputImage|OutputImage";

            acqTool.UserData.Add("_ToolOutputTerminals", s7);

            tb.Tools.Add(acqTool);
            tb.Tools.Add(pmaTool);
            tb.Tools.Add(circleTool);
            tb.Tools.Add(nTool);
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

        public override void Close()
        {
            base.Close();
            CloseCam();
        }
        #endregion
    }
}
