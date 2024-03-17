using System;
using System.ComponentModel;
using System.Windows.Forms;
using Cognex.VisionPro.ToolBlock;
using Vision.Frm;
using Vision.Projects;
using Vision.Stations;
using Vision.Tools.ToolImpls;

namespace Vision.Tools
{
    [ToolboxItem(false)]
    public partial class UcNineCalibTool : UserControl
    {
        public UcNineCalibTool(Station station, NPointCalibTool tool)
        {
            InitializeComponent();
            _nTool = tool;
            _station = station;
            _toolblock = tool.ToolBlock;
            GetImageIn();
            ProjectManager.Instance.BeforeSaveProjectEvent += Instance_BeforeSaveProjectEvent;
        }

        private readonly NPointCalibTool _nTool;
        private readonly Station _station;
        private readonly CogToolBlock _toolblock;
        private bool _init;

        private void GetImageIn()
        {
            if (_station != null)
            {
                comboBox1.Items.Clear();
                comboBox1.Items.AddRange(_station.GetImageInToolNames(_nTool));
                if (_nTool.ImageInName != null)
                {
                    comboBox1.SelectedItem = _nTool.ImageInName.ToString();
                }
            }
        }

        private void cogToolBlockEditV21_Load(object sender, System.EventArgs e)
        {
            if (_toolblock != null)
            {
                cogToolBlockEditV21.Subject = _toolblock;
            }
            _init = true;
        }

        private void comboBox1_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            if (!_init) return;
            if (comboBox1.SelectedIndex != -1)
            {
                string imageToolName = comboBox1.Text;
                _nTool.ImageInName = imageToolName;
            }
            else
            {
                _nTool.ImageInName = null;
            }
        }

        private void Instance_BeforeSaveProjectEvent(object sender, EventArgs e)
        {
            comboBox1_SelectedIndexChanged(this, null);
        }

        private void Close()
        {
            ProjectManager.Instance.BeforeSaveProjectEvent -= Instance_BeforeSaveProjectEvent;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            FrmNPointCalib frmNPointCalib = new FrmNPointCalib(_nTool);
            frmNPointCalib.ShowDialog();
        }
    }
}
