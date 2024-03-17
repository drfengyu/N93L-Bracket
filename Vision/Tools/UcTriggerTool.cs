using System;
using System.ComponentModel;
using System.Windows.Forms;
using Vision.Core;
using Vision.Projects;
using Vision.Tools.ToolImpls;

namespace Vision.Tools
{
    [ToolboxItem(false)]
    public partial class UcTriggerTool : UserControl
    {
        public UcTriggerTool(TriggerTool tool)
        {
            InitializeComponent();
            _tTool = tool;
            ProjectManager.Instance.BeforeSaveProjectEvent += Instance_BeforeSaveProjectEvent;
        }

        private readonly TriggerTool _tTool;

        /// <summary>
        /// 初始化界面
        /// </summary>
        private void InitUI()
        {
            tbAddress.Text = _tTool?.TriggerAddress;
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
        private void Instance_BeforeSaveProjectEvent(object sender,EventArgs e)
        {
            if (_tTool != null)
            {
                _tTool.TriggerAddress = tbAddress.Text;
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
