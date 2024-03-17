using System.ComponentModel;
using System.Windows.Forms;
using Vision.Tools.ToolImpls;

namespace Vision.Tools
{
    [ToolboxItem(false)]
    public partial class UcImageTool : UserControl
    {
        private bool _init;

        private int _select;

        private readonly ImageTool _imageTool;

        public UcImageTool(ImageTool imageTool)
        {
            InitializeComponent();
            _imageTool = imageTool;
            _imageTool.ImageShowEvent += _imageTool_ImageShowEvent;
        }

        /// <summary>
        /// 处理imageTool的图形显示事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _imageTool_ImageShowEvent(object sender,Cognex.VisionPro.ICogImage e)
        {
            cogDisplay1.StaticGraphics.Clear();
            cogDisplay1.InteractiveGraphics.Clear();
            cogDisplay1.Image = e;
            cogDisplay1.Fit();
        }

        private void Close()
        {
            _imageTool.ImageShowEvent -= _imageTool_ImageShowEvent;
        }

        /// <summary>
        /// 初始化界面
        /// </summary>
        private void InitControl()
        {
            this.SuspendLayout();
            if(_imageTool == null)
            {
                return;
            }
            radDir.Checked = _imageTool.EmulationType == EmulationType.Dir;
            radFile.Checked = _imageTool.EmulationType == EmulationType.File;
            tbPath.Text = _imageTool.Path;
            _select = radDir.Checked ? 1 : 2;
            this.ResumeLayout(false);
        }

        private void UcLocalImageTool_Load(object sender,System.EventArgs e)
        {
            _init = false;
            InitControl();
            _init = true;
        }

        /// <summary>
        /// 加载文件夹
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void radDir_CheckedChanged(object sender,System.EventArgs e)
        {
            if(!_init) return;
            if(radDir.Checked)
            {
                _imageTool.EmulationType = EmulationType.Dir;
                _imageTool.Path = null;
                tbPath.Clear();
                _select = 1;
            }
        }

        /// <summary>
        /// 加载图像文件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void radFile_CheckedChanged(object sender,System.EventArgs e)
        {
            if(!_init) return;
            if(radFile.Checked)
            {
                _imageTool.EmulationType = EmulationType.File;
                _imageTool.Path = null;
                tbPath.Clear();
                _select = 2;
            }
        }

        /// <summary>
        /// 选择图像或者文件夹
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSelect_Click(object sender,System.EventArgs e)
        {
            if(!_init) return;
            if(_select == 1) //文件夹
            {
                FolderBrowserDialog dialog = new FolderBrowserDialog();
                dialog.SelectedPath = _imageTool.Path;
                dialog.Description = "请选择加载的文件夹";
                var f = dialog.ShowDialog();

                if(f == DialogResult.OK)
                {
                    _imageTool.Path = dialog.SelectedPath;
                }
            }
            else if(_select == 2)  //文件
            {
                OpenFileDialog dialog = new OpenFileDialog();
                dialog.Filter = "bmp文件;|*.bmp;";
                dialog.Title = "请选择加载的图片";
                dialog.Multiselect = false;
                var o = dialog.ShowDialog();
                if(o == DialogResult.OK)
                {
                    _imageTool.Path = dialog.FileName;
                }
            }
            tbPath.Text = _imageTool.Path;
        }

    }
}
