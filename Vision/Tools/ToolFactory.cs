using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using Vision.Core;
using Vision.Stations;
using Vision.Tools.ToolImpls;

namespace Vision.Tools
{
    /// <summary>
    /// 工具工厂类
    /// </summary>
    public class ToolFactory
    {
        private static readonly object _syncRoot = new object();
        private static ToolFactory _instance;
        private List<AssemblyData> _toolAsmList;
        private List<ToolBoxInfo> _toolInfoList;

        private ToolFactory()
        {
        }

        public static ToolFactory Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_syncRoot)
                    {
                        if (_instance == null)
                        {
                            _instance = new ToolFactory();
                        }
                    }
                }
                return _instance;
            }
        }

        /// <summary>
        /// 工具集列表，只读
        /// </summary>
        public List<AssemblyData> ToolAsmList
        {
            get
            {
                if (_toolAsmList == null)
                {
                    _toolAsmList = GetToolAsmList();
                }
                return _toolAsmList;
            }
        }

        /// <summary>
        /// 工具列表
        /// </summary>
        public List<ToolBoxInfo> ToolInfoList
        {
            get
            {
                if (_toolInfoList == null)
                {
                    _toolInfoList = GetToolInfoList();
                }
                return _toolInfoList;
            }
        }

        /// <summary>
        /// 根据toolbase创建工具
        /// </summary>
        /// <param name="station"></param>
        /// <param name="tool"></param>
        /// <returns></returns>
        public static UserControl GetUI(Station station, ToolBase tool)
        {
            UserControl uc = null;
            if (tool != null)
            {
                switch (tool)
                {
                    case ImageAcqTool imageTool:
                        uc = new UcAcqTool(imageTool);
                        break;
                    case ImageTool iTool:
                        uc = new UcImageTool(iTool);
                        break;
                    case CenterDetectTool dTool:
                        uc = new UcCenterDetectTool(station, dTool);
                        break;
                    case ResultTool rTool:
                        uc = new UcResult(station, rTool);
                        break;
                    case CenterCalibTool centerTool:
                        uc = new UcCenterCalibTool(station, centerTool);
                        break;
                    case NPointCalibTool nTool:
                        uc = new UcNineCalibTool(station, nTool);
                        break;
                    case TriggerTool triggerTool:
                        uc = new UcTriggerTool(triggerTool);
                        break;
                    case KkRobotCalibTool kRTool:
                        uc = new UcKkRobotTool(kRTool);
                        break;
                    case EndTool endTool:
                        uc = new UcEndTool(endTool);
                        break;
                    case DetectTool detectTool:
                        uc = new UcDetectTool(station, detectTool);
                        break;
                }
            }
            return uc;
        }

        /// <summary>
        /// 获取工具列表树节点
        /// </summary>
        /// <returns></returns>
        public TreeNode[] GetToolGroupTreeNode(string groupImage, string toolImage)
        {
            List<TreeNode> nodes = new List<TreeNode>();
            //根据index进行组分类
            var result =
                from r in ToolInfoList
                group r by r.GroupIndex into g
                where g.Any()
                orderby g.Key
                select g;

            foreach (var toolGroup in result)
            {
                //var groupName = toolGroup.Key.ToString();
                var groupName = toolGroup.First().GroupName; ////组名称
                TreeNode tnGroup = new TreeNode(groupName);
                tnGroup.ImageKey = groupImage;
                tnGroup.SelectedImageKey = groupImage;
                tnGroup.Name = "工具组";

                var tool =
                        from r in toolGroup
                        orderby r.ToolIndex
                        select r;

                //遍历子工具
                foreach (ToolBoxInfo toolInfo in tool)
                {
                    
                    TreeNode tnTool = new TreeNode(toolInfo.ToolName);
                    tnTool.ImageKey = toolImage;
                    tnTool.SelectedImageKey = toolImage;
                    tnTool.Name = "子工具";
                    tnGroup.Nodes.Add(tnTool);
                }
                nodes.Add(tnGroup);
            }
            return nodes.ToArray();
        }

        /// <summary>
        /// 获取工具描述
        /// </summary>
        /// <param name="toolName"></param>
        /// <returns></returns>
        public string GetToolDescription(string toolName)
        {
            foreach (var item in ToolInfoList)
            {
                if (item.ToolName == toolName)
                {
                    return item.Description;
                }
            }
            return "";
        }

        /// <summary>
        /// 根据名称创建驱动
        /// </summary>
        public ToolBase CreatToolByInfo(string toolName)
        {
            try
            {
                foreach (var item in ToolAsmList)
                {
                    string name = item.Type.GetCustomAttribute<ToolNameAttribute>()?.Name;
                    if (name == toolName)
                    {
                        ToolBase tool = AssemblyFactory.CreateInstance<ToolBase>(item, null);
                        return tool;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return null;
        }

        /// <summary>
        /// 反射获取工具集列表
        /// </summary>
        private List<AssemblyData> GetToolAsmList()
        {
            List<AssemblyData> toolAsmList = new List<AssemblyData>();
            //获取当前正在执行的程序集
            Assembly asm = Assembly.GetExecutingAssembly();
            foreach (Type tChild in asm.GetTypes())
            {
                if (tChild.BaseType == typeof(ToolBase))
                {
                    toolAsmList.Add(new AssemblyData(asm, tChild));
                }
            }
            return toolAsmList;
        }

        /// <summary>
        /// 获取工具箱列表
        /// </summary>
        /// <returns></returns>
        public List<ToolBoxInfo> GetToolInfoList()
        {
            List<ToolBoxInfo> toolInfoList = new List<ToolBoxInfo>();
            if (ToolAsmList == null) return null;

            foreach (var item in ToolAsmList)
            {
                string g = item.Type.GetCustomAttribute<GroupInfoAttribute>()?.Name;
                string tool = item.Type.GetCustomAttribute<ToolNameAttribute>()?.Name;
                string des = item.Type.GetCustomAttribute<DescriptionAttribute>()?.Description;
                int? index = (item.Type.GetCustomAttribute<GroupInfoAttribute>()?.Index) ?? null;
                int? tIndex = (item.Type.GetCustomAttribute<ToolNameAttribute>()?.Index) ?? null;
                if (string.IsNullOrEmpty(g)) continue;
                ToolBoxInfo toolBoxInfo = new ToolBoxInfo()
                {
                    ToolName = tool,
                    Description = des,
                    GroupIndex = index ?? 0,
                    GroupName = g,
                    ToolIndex = tIndex ?? 0,
                };
                toolInfoList.Add(toolBoxInfo);
            }
            return toolInfoList;
        }
    }

}