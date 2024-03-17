using System;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Forms;
using Vision.Core;
using Vision.Stations;
using Vision.Tools.ToolImpls;

namespace Vision.Tools
{
    [ToolboxItem(false)]
    public partial class UcToolBase : UserControl
    {
        public UcToolBase(Station station, ToolBase tool)
        {
            InitializeComponent();
            _baseTool = tool;
            _station = station;
            ChangeToolUI(station, tool);
        }

        private readonly ToolBase _baseTool;
        private readonly Station _station;

        public event EventHandler<bool> ToolEnableChangedEvent;

        /// <summary>
        /// 切换工具UI
        /// </summary>
        /// <param name="station"></param>
        /// <param name="tool"></param>
        private void ChangeToolUI(Station station, ToolBase tool)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action<Station, ToolBase>(ChangeToolUI), station, tool);
                return;
            }
            if (tool == null)
            {
                //清除control
                if (panelMain.Controls.Count > 0)
                {
                    foreach (UserControl c in panelMain.Controls)
                    {
                        c.Dispose();
                    }
                }
                panelMain.Controls.Clear();
                return;
            }

            if (panelMain.Controls.Count > 0)
            {
                foreach (UserControl c in panelMain.Controls)
                {
                    c.Dispose();
                }
            }
            panelMain.Controls.Clear();
            var ui = ToolFactory.GetUI(station, tool);
            ui.Dock = DockStyle.Fill;
            panelMain.Controls.Add(ui);
            UpdateToolStatu(tool);
            OnToolEnableChangedEvent(this, tool.Enable);
        }

        /// <summary>
        /// 更新工具界面显示（启用/禁用）
        /// </summary>
        /// <param name="tool"></param>
        private void UpdateToolStatu(ToolBase tool)
        {
            tsDisable.Enabled = tool.Enable;
            tsEnable.Enabled = !tool.Enable;
        }

        /// <summary>
        /// 工具启用/关闭事件触发器
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="b"></param>
        private void OnToolEnableChangedEvent(object sender, bool b)
        {
            if (ToolEnableChangedEvent != null)
            {
                var e = ToolEnableChangedEvent;
                e.Invoke(sender, b);
            }
        }

        /// <summary>
        /// 功能按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            var toolStrip = e.ClickedItem;
            if (_baseTool == null)
            {
                LogUI.AddLog("未选择工具");
                return;
            }
            try
            {
                switch (toolStrip.Text)
                {
                    case "运行工具":
                        _station.DebugRun();
                        break;
                    case "禁用工具":
                        _baseTool.Enable = false;
                        UpdateToolStatu(_baseTool);
                        OnToolEnableChangedEvent(this, false);
                        break;
                    case "启用工具":
                        _baseTool.Enable = true;
                        UpdateToolStatu(_baseTool);
                        OnToolEnableChangedEvent(this, true);
                        break;
                    case "保存工具":
                        _baseTool.Save();
                        break;
                }
            }
            catch (ToolException ex)
            {
                LogUI.AddLog(ex.Message);
            }
        }

        /// <summary>
        /// 界面Load事件 添加一个实时log显示线程
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UcToolBase_Load(object sender, EventArgs e)
        {
            //Task.Run(async () =>
            //{
            //    while (true)
            //    {
            //        var s = LogUI.GetToolLog();
            //        tbMsg.BeginInvoke(new Action(() =>
            //        {
            //            if (s != null)
            //                tbMsg.AppendText(s);
            //        }));
            //        await Task.Delay(100);
            //    }
            //});
        }

    }
}
