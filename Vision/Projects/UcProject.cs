using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Vision.Core;
using Vision.Frm;
using Vision.Stations;
using Vision.Tools;
using Vision.Tools.ToolImpls;

namespace Vision.Projects
{
    [ToolboxItem(false)]
    public partial class UcProject : UserControl
    {
        private TreeNode _selectTool;   //选择的工具Node
        private TreeNode _selectedNode; //选择的Node
        private TreeNode _rootNode;     //根节点
        private UcToolBase _toolUI;     //工具UI
        private int cnt = 0;            // 记录鼠标（左键）点击次数

        public UcProject()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 添加工具UI
        /// </summary>
        /// <param name="station"></param>
        /// <param name="tool"></param>
        public void AddControl(Station station, ToolBase tool)
        {
            if (panelMain.Controls.Count > 0)
            {
                foreach (UserControl item in panelMain.Controls)
                {
                    switch (item)
                    {
                        case UcToolBase tTool:
                            tTool.ToolEnableChangedEvent -= _toolUI_ToolEnableChangedEvent;
                            item.Dispose();
                            break;
                        case UcDebug sShow:
                            sShow.Dispose();
                            break;
                    }
                }
            }
            panelMain.Controls.Clear();

            //如果清除界面
            if (station == null && tool == null)
            {
                return;
            }

            //如果添加的是UcStationShow

            if (station != null && tool == null)
            {
                UcDebug ucStation = new UcDebug(station);
                ucStation.Dock = DockStyle.Fill;
                panelMain.Controls.Add(ucStation);
            }

            //如果添加的是UcToolBase界面
            else if (station != null)
            {
                _toolUI = new UcToolBase(station, tool);
                _toolUI.ToolEnableChangedEvent += _toolUI_ToolEnableChangedEvent;
                _toolUI.Dock = DockStyle.Fill;
                panelMain.Controls.Add(_toolUI);
            }
        }

        /// <summary>
        /// 工具启用/关闭 变化事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _toolUI_ToolEnableChangedEvent(object sender, bool e)
        {
            if (_selectTool != null)
            {
                _selectTool.ForeColor = e ? Color.Black : Color.Gray;
            }
        }

        /// <summary>
        /// treeview 单击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tvProject_MouseClick(object sender, MouseEventArgs e)
        {
            if (ProjectManager.Instance == null)
            {
                return;
            }

            Point clickPoint = new Point(e.X, e.Y);
            _selectedNode = tvProject.GetNodeAt(clickPoint);
            if (e.Button == MouseButtons.Right)
            {
                _selectedNode.ContextMenuStrip = null;
                if (_selectedNode == null)
                {
                    return;
                }
                if (_selectedNode.Parent != null) //非总节点
                {
                    if (_selectedNode.Parent != _rootNode) //工具节点
                    {
                        _selectTool = _selectedNode;
                        _selectedNode.ContextMenuStrip = cmsTool;
                    }
                    else  //station节点
                    {
                        _selectedNode.ContextMenuStrip = cmsStation;
                    }
                }
                else  //总节点
                {
                    _selectedNode.ContextMenuStrip = cmsProject;
                }
                tvProject.SelectedNode = _selectedNode;
            }
        }

        /// <summary>
        /// treeview 双击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tvProject_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (ProjectManager.Instance == null)
            {
                return;
            }

            Point clickPoint = new Point(e.X, e.Y);
            _selectedNode = tvProject.GetNodeAt(clickPoint);
            if (e.Button == MouseButtons.Left)
            {
                if (_selectedNode == null) return;
                if (_selectedNode.Parent == null) return;
                
                var station = GetStation(_selectedNode);
                if (_selectedNode.Parent == _rootNode)
                {
                    //工位被双击
                    AddControl(station, null);
                }
                else
                {
                    //工具被双击
                    _selectTool = _selectedNode;
                    //获取被点击的工具
                    var tool = GetTool();
                    AddControl(station, tool);
                }
                SetNodeColor(_selectedNode);
            }
        }

        /// <summary>
        /// root节点点击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cmsProject_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            var item = e.ClickedItem;
            if (item == null) return;
            if (item.Text == "新增工位")
            {
                Cursor = Cursors.WaitCursor;
                AddStation();
                Cursor = Cursors.Default;
            }
        }

        private void UcProject_Load(object sender, System.EventArgs e)
        {
            _rootNode = new TreeNode()
            {
                Text = "项目",
                SelectedImageKey = "Project.png",
                ImageKey = "Project.png",
            };
            ProjectManager.Instance.UpdateProjectUIEvent += UpdateProjectUI;
            UpdateProjectUI(this, EventArgs.Empty);
        }

        /// <summary>
        /// 工具点击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cmsTool_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            var station = GetStation(_selectedNode);
            if (station == null) return;
            var tool = station[_selectedNode.Text];
            var item = e.ClickedItem;
            if (item == null) return;
            switch (item.Text)
            {
                case "删除工具":
                    try
                    {
                        ProjectManager.Instance.DeleteTool(station, tool);
                        AddControl(null, null);
                    }
                    catch (Exception ex)
                    {
                        LogUI.AddLog(ex.Message);
                    }
                    break;

                case "重命名":
                    FrmRename frm = new FrmRename();
                    frm.OldName = _selectedNode.Text;
                    if (frm.ShowDialog() == DialogResult.OK)
                    {
                        string newName = frm.NewName;
                        ProjectManager.Instance.RenameTool(station, tool, newName);
                    }
                    break;

                case "上移":
                    ProjectManager.Instance.UpTool(station, tool);
                    break;

                case "下移":
                    ProjectManager.Instance.DownTool(station, tool);
                    break;
            }
        }

        /// <summary>
        /// 工位点击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cmsStation_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            var station = GetStation(_selectedNode);
            if (station == null) return;


            var item = e.ClickedItem;
            if (item == null) return;
            switch (item.Text)
            {
                case "新建工具":
                    FrmToolBox frm = new FrmToolBox()
                    {
                        SelectedStation = station
                    };
                    frm.ShowDialog();
                    break;

                case "工位重命名":
                    FrmRename frm2 = new FrmRename();
                    frm2.OldName = _selectedNode.Text;
                    if (frm2.ShowDialog() == DialogResult.OK)
                    {
                        string newName = frm2.NewName;
                        ProjectManager.Instance.RenameStation(station, newName);
                    }
                    break;

                case "工位上移":
                    ProjectManager.Instance.UpStation(station);
                    break;

                case "工位下移":
                    ProjectManager.Instance.DownStation(station);
                    break;

                case "删除全部工具":
                    ProjectManager.Instance.DeleteAllTool(station);
                    AddControl(null,null);
                    break;

                case "删除工位":
                    ProjectManager.Instance.DeleteStation(station);
                    AddControl(null,null);
                    break;
            }
        }

        /// <summary>
        /// 设置node颜色
        /// </summary>
        /// <param name="node"></param>
        private void SetNodeColor(TreeNode node)
        {
            foreach (TreeNode tNode in tvProject.Nodes)
            {
                //工位
                foreach (TreeNode item in tNode.Nodes)
                {
                    item.BackColor = Color.Transparent;
                    //工具
                    foreach (TreeNode item2 in item.Nodes)
                    {
                        item2.BackColor = Color.Transparent;
                    }
                }
            }
            node.BackColor = Color.LimeGreen;
        }

        /// <summary>
        /// 根据node名称获取工位
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        private Station GetStation(TreeNode node)
        {
            if (node == null || node.Parent == null) return null;
            if (node.Parent == _rootNode) //station节点
            {
                return ProjectManager.Instance.ProjectData[node.Text];
            }
            else //工具节点
            {
                return ProjectManager.Instance.ProjectData[node.Parent.Text];
            }
        }

        /// <summary>
        /// 添加工位
        /// </summary>
        private void AddStation()
        {
            string stationName = ProjectManager.Instance.ProjectData.GenDefaultStationName();
            Station station = new Station()
            {
                Name = stationName,
                StationPath = Path.Combine(ProjectManager.Instance.ProjectDir + $"\\{stationName}"),
            };
            station.RegisterViewDisplay();
            ProjectManager.Instance.AddStation(station);
        }

        /// <summary>
        /// 获取工具
        /// </summary>
        /// <returns></returns>
        private ToolBase GetTool()
        {
            var station = GetStation(_selectedNode);
            var tool = station?[_selectedNode.Text];
            return tool;
        }

        /// <summary>
        /// 更新project UI
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UpdateProjectUI(object sender, EventArgs e)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action<object, EventArgs>(UpdateProjectUI), sender, e);
                return;
            }
            tvProject.Nodes.Clear();
            _rootNode.Nodes.Clear();
            var data = ProjectManager.Instance.ProjectData;
            if (data != null)
            {
                if (data.StationList.Count > 0)
                {
                    foreach (var station in data.StationList)
                    {
                        TreeNode sNode = new TreeNode
                        {
                            Text = station.Name,
                            ImageKey = "Station.png",
                            SelectedImageKey = "Station.png"
                        };
                        foreach (var tool in station.ToolList)
                        {
                            TreeNode tNode = new TreeNode
                            {
                                Text = tool.Name,
                                ImageKey = "Tool.png",
                                SelectedImageKey = "Tool.png",
                                ForeColor = tool.Enable ? Color.Black : Color.Gray
                            };
                            sNode.Nodes.Add(tNode);
                        }

                        _rootNode.Nodes.Add(sNode);
                    }
                }
            }
            tvProject.Nodes.Add(_rootNode);
            tvProject.ExpandAll();
        }

        private void Close()
        {
            if (_toolUI != null)
            {
                _toolUI.ToolEnableChangedEvent -= _toolUI_ToolEnableChangedEvent;
            }
            ProjectManager.Instance.UpdateProjectUIEvent -= UpdateProjectUI;
        }

        #region <treeview双击不折叠>
        private void tvProject_MouseDown(object sender, MouseEventArgs e)
        {
            // 统计左键点击次数
            if (e.Button == MouseButtons.Left)
                cnt = e.Clicks;
        }

        private void tvProject_BeforeCollapse(object sender, TreeViewCancelEventArgs e)
        {
            if(_selectedNode == null)
                return;
            if (_selectedNode.Parent == _rootNode)
            {
                e.Cancel = cnt > 1;
            }
        }

        private void tvProject_BeforeExpand(object sender, TreeViewCancelEventArgs e)
        {
            if(_selectedNode == null)
                return;

            if(_selectedNode.Parent == _rootNode)
            {
                e.Cancel = cnt > 1;
            }
        }
        #endregion
    }
}