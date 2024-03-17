using System;
using System.ComponentModel;
using Vision.Core;
using Vision.Projects;

namespace Vision.Tools.ToolImpls
{
    [Serializable]
    [ToolName("结束信号",1)]
    [GroupInfo(name: "通讯工具", index: 4)]
    [Description("相机处理结束信号")]
    public class EndTool : ToolBase
    {
        public EndTool()
        {
        }

        /// <summary>
        /// 结束信号地址
        /// </summary>
        public string EndAddress { get; set; }

        /// <summary>
        /// 运行工具
        /// </summary>
        /// <exception cref="ToolException"></exception>
        public override void Run()
        {
            var plc = ProjectManager.Instance.ProjectData.MxPlc;
            if (plc != null && plc.IsConnected)
            {
                if (EndAddress != null)
                {
                    plc.WriteShort(EndAddress,1);
                    //LogUI.AddLog($"发送结束信号{EndAddress}！");
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
