using System;
using System.Collections.Generic;
using System.ComponentModel;
using Vision.Core;
using Vision.Projects;
using Vision.Stations;
using Vision.Tools.Interfaces;

namespace Vision.Tools.ToolImpls
{
    [Serializable]
    [GroupInfo("结果工具", 3)]
    [ToolName("结果编辑",0)]
    [Description("编辑结果和其他控制系统进行交互")]
    public class ResultTool : ToolBase, IResult, IRegisterStation
    {
        [NonSerialized]
        private Station _station;

        /// <summary>
        /// 所有保存的结果信息
        /// </summary>
        public List<ResultInfo> ResultData { get; set; } = new List<ResultInfo>();

        public ResultTool()
        {
        }

        /// <summary>
        /// 注册station
        /// </summary>
        /// <param name="station"></param>
        public void RegisterStation(Station station)
        {
            _station = station;
        }

        /// <summary>
        /// 运行工具
        /// </summary>
        public override void Run()
        {
            if (!Enable) return;
            GetResult();
            SendData();
        }

        /// <summary>
        /// 获取结果
        /// </summary>
        /// <exception cref="ToolException"></exception>
        private void GetResult()
        {
            if (ResultData.Count == 0)
                throw new ToolException("结果分配有误，请检查");
            foreach (ResultInfo result in ResultData)
            {
                //先获取工具
                var toolName = result.Source.Split('.')[0];
                var tool = _station[toolName];

                //数据源
                var data = result.Source.Split('.')[1];

                //分为2种
                //1、detectTool
                //2、IPointOut
                switch (tool)
                {
                    case CenterDetectTool dTool:
                        //根据名称获取结果
                        result.Value = dTool.GetValue(data);
                        break;

                    case DetectTool dTool:
                        //根据名称获取结果
                        result.Value = dTool.GetValue(data);
                        break;

                    case IPointOut pointTool:
                        //总共就3个结果
                        switch (data)
                        {
                            case "X":
                                result.Value = pointTool.PointOut.X;
                                break;
                            case "Y":
                                result.Value = pointTool.PointOut.Y;
                                break;
                            case "Angle":
                                result.Value = pointTool.PointOut.Angle;
                                break;
                        }
                        break;
                }
            }
        }

        /// <summary>
        /// 发送数据
        /// </summary>
        /// <exception cref="ToolException"></exception>
        /// <exception cref="Exception"></exception>
        private void SendData()
        {
            if (ResultData.Count <= 0)
            {
                throw new ToolException("结果分配有误，请检查");
            }
            if (!ProjectManager.Instance.IsLoaded || ProjectManager.Instance.ProjectData == null)
            {
                throw new ToolException("程序未加载完成！");
            }
            var plc = ProjectManager.Instance.ProjectData.MxPlc;
            if (plc == null)
            {
                throw new ToolException("plc未连接！");
            }

            try
            {
                foreach (ResultInfo res in ResultData)
                {
                    switch (res.Type)
                    {
                        case ResultType.Bool:
                            {
                                plc.WriteBool(res.Address, Convert.ToBoolean(res.Value));
                            }
                            break;
                        case ResultType.Short:
                            {
                                plc.WriteShort(res.Address, Convert.ToInt16(res.Value));
                            }
                            break;
                        case ResultType.Int:
                            {
                                plc.WriteInt(res.Address, Convert.ToInt32(res.Value));
                            }
                            break;
                        case ResultType.Double:
                            {
                                plc.WriteDouble(res.Address, Convert.ToDouble(res.Value));
                            }
                            break;
                        case ResultType.String:
                            {
                                plc.WriteString(res.Address, res.Value.ToString());
                            }
                            break;
                    }

                    LogNetHelper.Log($"数据地址：{res.Address} 数据结果:{res.Value}");
                    //LogUI.AddLog($"发送地址:{res.Address} 数据:{res.Value}");
                }
            }
            catch (Exception ex)
            {
                LogNetHelper.Log("发送PLC结果失败：" + ex.Message);
                LogUI.AddLog("发送PLC结果失败：" + ex.Message);
            }
        }
    }
}
