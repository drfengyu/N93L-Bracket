using System;
using System.ComponentModel;
using System.Threading;
using Vision.Core;
using Vision.Projects;

namespace Vision.Tools.ToolImpls
{
    [Serializable]
    [ToolName("触发信号",0)]
    [GroupInfo(name: "通讯工具", index: 4)]
    [Description("PLC触发相机拍照信号")]
    public class TriggerTool : ToolBase
    {
        public TriggerTool()
        {
        }

        /// <summary>
        /// 触发信号地址
        /// </summary>
        public string TriggerAddress { get; set; }

        /// <summary>
        /// 运行工具
        /// </summary>
        /// <exception cref="ToolException"></exception>
        public override void Run()
        {
            var plc = ProjectManager.Instance.ProjectData.MxPlc;
            if (plc != null && plc.IsConnected)
            {
                if (TriggerAddress != null)
                {
                    while (true)
                    {
                        var flag = plc.ReadShort(TriggerAddress);
                        if (flag == 1)
                        {
                            //LogUI.AddLog($"收到触发信号{TriggerAddress}！");
                            //复位
                            plc.WriteShort(TriggerAddress, 0);
                           // LogUI.AddLog($"复位触发信号{TriggerAddress}！");
                            break;
                        }
                        Thread.Sleep(10);
                    }
                }
            }
            else
            {
                throw new ToolException("plc未连接！")
                {
                    ImageInNull = true
                };
            }
        }
    }
}
