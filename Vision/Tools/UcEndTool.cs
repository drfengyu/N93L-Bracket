using System;
using System.ComponentModel;
using System.Windows.Forms;
using Vision.Core;
using Vision.Projects;
using Vision.Tools.ToolImpls;

namespace Vision.Tools
{
    [ToolboxItem(false)]
    public partial class UcEndTool : UserControl
    {
        public UcEndTool(EndTool tool)
        {
            InitializeComponent();
            _eTool = tool;
            ProjectManager.Instance.BeforeSaveProjectEvent += Instance_BeforeSaveProjectEvent;
        }

        private readonly EndTool _eTool;

        /// <summary>
        /// 初始化界面
        /// </summary>
        private void InitUI()
        {
            tbAddress.Text = _eTool?.EndAddress;
        }

        /// <summary>
        /// Load事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UcTriggerTool_Load(object sender, System.EventArgs e)
        {
            try
            {
                InitUI();
            }
            catch (Exception ex)
            {
                LogUI.AddLog(ex.Message);
            }
        }

        /// <summary>
        /// 项目保存前置事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Instance_BeforeSaveProjectEvent(object sender, EventArgs e)
        {
            if (_eTool != null)
            {
                _eTool.EndAddress = tbAddress.Text;
            }
        }

        /// <summary>
        /// 关闭UI
        /// </summary>
        private void Close()
        {
            ProjectManager.Instance.BeforeSaveProjectEvent -= Instance_BeforeSaveProjectEvent;
        }
    }
}
