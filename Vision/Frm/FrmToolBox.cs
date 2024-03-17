using System.Drawing;
using System.Windows.Forms;
using Vision.Core;
using Vision.Projects;
using Vision.Stations;
using Vision.Tools;
using Vision.Tools.ToolImpls;

namespace Vision.Frm
{
    public partial class FrmToolBox : Form
    {
        private TreeNode _rootNode;

        public FrmToolBox()
        {
            InitializeComponent();
            InitTools();
            this.StartPosition = FormStartPosition.CenterScreen;
            BringToFront();
        }

        public Station SelectedStation { get; set; }

        private void InitTools()
        {

            this.tvTools.Nodes.Clear();

            _rootNode = new TreeNode()
            {
                Text = "工具",
                ImageKey = "Root.png",
                SelectedImageKey = "Root.png"
            };
            try
            {
                var nodes = ToolFactory.Instance.GetToolGroupTreeNode("Group.png", "Tool.png");
                _rootNode.Nodes.AddRange(nodes);
                tvTools.Nodes.Add(_rootNode);
            }
            catch (System.Exception ex)
            {
                LogNetHelper.Log(ex.Message);
            }
        }

        private void FormTools_Load(object sender, System.EventArgs e)
        {
            tvTools.ExpandAll();
        }

        private void tvTools_MouseClick(object sender, MouseEventArgs e)
        {
            Point ClickPoint = new Point(e.X, e.Y);
            TreeNode CurrentNode = this.tvTools.GetNodeAt(ClickPoint);
            if (MouseButtons.Left == e.Button)
            {
                try
                {
                    if (CurrentNode != null && CurrentNode.Parent != null)
                    {
                        this.tvTools.SelectedNode = CurrentNode;
                        this.tbDescription.Text = "工具描述：\r\n    " + ToolFactory.Instance.GetToolDescription(CurrentNode.Text);
                    }
                }
                catch (System.Exception ex)
                {
                    LogNetHelper.Log(ex.Message);
                }
            }
        }

        private void tvTools_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            //双击创建工具
            Point ClickPoint = new Point(e.X, e.Y);
            TreeNode CurrentNode = this.tvTools.GetNodeAt(ClickPoint);
            if (MouseButtons.Left == e.Button)
            {
                if (CurrentNode != null && CurrentNode.Parent != null)
                {
                    this.tvTools.SelectedNode = CurrentNode;
                    if (!CurrentNode.Name.Contains("子工具"))
                    {
                        return;
                    }
                    try
                    {
                        ToolBase tool = ToolFactory.Instance.CreatToolByInfo(CurrentNode.Text);
                        if (tool != null)
                        {
                            Cursor = Cursors.WaitCursor;
                            ProjectManager.Instance.AddTool(SelectedStation, tool);
                            Cursor = Cursors.Default;
                        }
                        else
                        {
                            MessageBox.Show("工具选择错误", "提示");
                        }
                    }
                    catch (System.Exception ex)
                    {
                        LogUI.AddLog(ex.Message);
                    }
                }
            }
        }
    }
}