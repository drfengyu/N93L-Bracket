using System;
using System.ComponentModel;
using System.Windows.Forms;
using Cognex.VisionPro;
using Vision.Core;
using Vision.Frm;
using Vision.Projects;
using Vision.Stations;
using Vision.Tools.ToolImpls;

namespace Vision.Tools
{
    [ToolboxItem(false)]
    public partial class UcCenterCalibTool : UserControl
    {
        public UcCenterCalibTool(Station station, CenterCalibTool tool)
        {
            InitializeComponent();
            _station = station;
            _cTool = tool;
            ProjectManager.Instance.BeforeSaveProjectEvent += Instance_BeforeSaveProjectEvent;
        }

        private readonly CenterCalibTool _cTool;
        private readonly Station _station;
        private bool _init;

        /// <summary>
        /// 初始化
        /// </summary>
        private void Init()
        {
            //ModelPoint combox
            var inputs = _station.GetModelPoint();
            cbModelInput.Items.Clear();
            if (inputs != null)
                // ReSharper disable once CoVariantArrayConversion
                cbModelInput.Items.AddRange(inputs.ToArray());

            //显示已经绑定的工具
            cbModelInput.Text = _cTool.InputModelDataToolName;

            //RobotDelta combox
            inputs = _station.GetRobotDeltaPoint();
            cbRobotDelta.Items.Clear();
            if (inputs != null)
                // ReSharper disable once CoVariantArrayConversion
                cbRobotDelta.Items.AddRange(inputs.ToArray());

            //显示已经绑定的工具
            cbRobotDelta.Text = _cTool.InputRobotDeltaToolName;

            //cam_robot combox
            inputs = _station.GetNPointDeltaPoint();
            cbCamNPoint.Items.Clear();
            if (inputs != null)
                // ReSharper disable once CoVariantArrayConversion
                cbCamNPoint.Items.AddRange(inputs.ToArray());

            //显示已经绑定的工具
            cbCamNPoint.Text = _cTool.InputNPointDeltaToolName;

            if(_station.CenterCalibRobotPoint == null)
            {
                _station.CenterCalibRobotPoint = new PointD();
            }
            if (_cTool.CenterPoint == null)
            {
                _cTool.CenterPoint = new PointD();
            }
            //点位
            numCX.Value = (decimal)_cTool.CenterPoint.X;
            numCY.Value = (decimal)_cTool.CenterPoint.Y;
            numRX.Value = (decimal)_station.CenterCalibRobotPoint.X;
            numRY.Value = (decimal)_station.CenterCalibRobotPoint.Y;
        }

        /// <summary>
        /// vpp绑定
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cogToolBlockEditV21_Load(object sender, System.EventArgs e)
        {
            if (_cTool.ToolBlock != null)
            {
                cogToolBlockEditV21.Subject = _cTool.ToolBlock;
            }
        }

        /// <summary>
        /// combox事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void comboBox1_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            if (!_init) return;
            if (cbCamNPoint.SelectedIndex != -1)
            {
                _cTool?.RemoveNPointCalibTool();
                NPointCalibTool tool = (NPointCalibTool)_station[cbCamNPoint.SelectedItem.ToString()];
                _cTool?.AddNPointCalibTool(tool);
                _cTool.InputNPointDeltaToolName = cbCamNPoint.SelectedItem.ToString();
            }
            else
            {
                _cTool.InputNPointDeltaToolName = null;
            }
        }

        /// <summary>
        /// 窗体加载事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UcCenterCalibTool_Load(object sender, System.EventArgs e)
        {
            Init();
            _init = true;
        }

        /// <summary>
        /// 输入参数选择事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbInput_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            if (_init)
            {
                if (cbModelInput.SelectedIndex != -1)
                {
                    _cTool.InputModelDataToolName = cbModelInput.SelectedItem.ToString();
                }
                else
                {
                    _cTool.InputModelDataToolName = null;
                }
            }
        }

        /// <summary>
        /// 保存项目前置事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Instance_BeforeSaveProjectEvent(object sender, EventArgs e)
        {
            //comboBox1_SelectedIndexChanged(this,null);
            //cbInput_SelectedIndexChanged(this,null);
            //cbRobotDelta_SelectedIndexChanged(this,null);
        }

        /// <summary>
        /// 关闭控件
        /// </summary>
        private void Close()
        {
            ProjectManager.Instance.BeforeSaveProjectEvent -= Instance_BeforeSaveProjectEvent;
        }

        /// <summary>
        /// 机械手偏移combox
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbRobotDelta_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_init)
            {
                if (cbRobotDelta.SelectedIndex != -1)
                {
                    _cTool.InputRobotDeltaToolName = cbRobotDelta.SelectedItem.ToString();
                }
                else
                {
                    _cTool.InputRobotDeltaToolName = null;
                }
            }
        }


        private void numCX_ValueChanged(object sender, EventArgs e)
        {
            _cTool.CenterPoint.X = (double)numCX.Value;
        }

        private void numCY_ValueChanged(object sender, EventArgs e)
        {
            _cTool.CenterPoint.Y = (double)numCY.Value;
        }

        private void numRX_ValueChanged(object sender, EventArgs e)
        {
            _station.CenterCalibRobotPoint.X = (double)numRX.Value;
        }

        private void numRY_ValueChanged(object sender, EventArgs e)
        {
            _station.CenterCalibRobotPoint.Y = (double)numRY.Value;
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            FrmCenterCalib frmCenterCalib = new FrmCenterCalib(_cTool);
            frmCenterCalib.ShowDialog();
        }
    }
}
