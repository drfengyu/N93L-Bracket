using System;
using System.ComponentModel;
using System.Windows.Forms;
using Vision.Core;

namespace Vision.Projects
{
    [ToolboxItem(false)]
    public partial class UcSet : UserControl
    {
        public UcSet()
        {
            InitializeComponent();
        }


        private bool _init;
        private ImageConfig _imageConfig;
        private SystemConfig _systemConfig;
        private PointD _offset;

        private void ControlInit()
        {
            try
            {
                if (_init)
                    return;
                //初始化读取参数
                _imageConfig = ProjectManager.Instance.ProjectData.ImageConfig;
                _systemConfig = ProjectManager.Instance.ProjectData.SystemConfig;
                _offset = ProjectManager.Instance.ProjectData.Offset;

                //给控件赋值
                cbNG.Checked = _imageConfig.IsSaveNGImage;
                cbOK.Checked = _imageConfig.IsSaveOKImage;
                cbTime.Checked = _imageConfig.IsDeleteByTime;
                cbSize.Checked = _imageConfig.IsDeleteBySize;
                numSize.Value = _imageConfig.DeleteSize;
                numTime.Value = _imageConfig.DeleteDayTime;

                cbAutoRun.Checked = _systemConfig.AutoRun;

                tbHeart.Text = _systemConfig.HeartAddress;
                tbOnline.Text = _systemConfig.OnlineAddress;

                //补偿
                numOffsetX.Value = (decimal)_offset.X;
                numOffsetY.Value = (decimal)_offset.Y;

                tbPath.Text = _imageConfig.SaveImageDir;


                _init = true;
            }
            catch (Exception ex)
            {
                LogNetHelper.Log(ex.ToString());
            }

        }

        private void UcSet_Load(object sender, System.EventArgs e)
        {
            ControlInit();
        }

        private void numTime_ValueChanged(object sender, System.EventArgs e)
        {
            if (_init)
                _imageConfig.DeleteDayTime = (int)numTime.Value;
        }

        private void numSize_ValueChanged(object sender, System.EventArgs e)
        {
            if (_init)
                _imageConfig.DeleteSize = (int)numSize.Value;
        }

        private void cbTime_CheckedChanged(object sender, System.EventArgs e)
        {
            if (_init)
                _imageConfig.IsDeleteByTime = cbTime.Checked;
        }

        private void cbSize_CheckedChanged(object sender, System.EventArgs e)
        {
            if (_init)
                _imageConfig.IsDeleteBySize = cbSize.Checked;
        }

        private void cbNG_CheckedChanged(object sender, System.EventArgs e)
        {
            if (_init)
                _imageConfig.IsSaveNGImage = cbNG.Checked;
        }

        private void cbOK_CheckedChanged(object sender, System.EventArgs e)
        {
            if (_init)
                _imageConfig.IsSaveOKImage = cbOK.Checked;
        }

        private void cbAutoRun_CheckedChanged(object sender, System.EventArgs e)
        {
            if (_init)
                _systemConfig.AutoRun = cbAutoRun.Checked;
        }

        private void btnSave_Click(object sender, System.EventArgs e)
        {
            ProjectManager.Instance.SaveProject();
        }

        private void tbHeart_TextChanged(object sender, System.EventArgs e)
        {
            if (_init)
                _systemConfig.HeartAddress = tbHeart.Text;
        }

        private void tbOnline_TextChanged(object sender, System.EventArgs e)
        {
            if (_init)
                _systemConfig.OnlineAddress = tbOnline.Text;
        }

        private void numOffsetX_ValueChanged(object sender, System.EventArgs e)
        {
            if (_init)
            {
                _offset.X = (double)numOffsetX.Value;
            }

        }

        private void numOffsetY_ValueChanged(object sender, System.EventArgs e)
        {
            if (_init)
            {
                _offset.Y = (double)numOffsetY.Value;
            }

        }

        private void btnPath_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            if(fbd.ShowDialog() == DialogResult.OK)
            {
                var path = fbd.SelectedPath + "Images";
                tbPath.Text = path;
                _imageConfig.SaveImageDir = path;
            }
        }
    }
}
