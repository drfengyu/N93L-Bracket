using System;
using System.ComponentModel;
using System.Windows.Forms;
using Vision.Core;
using Vision.Projects;
using Vision.Tools.ToolImpls;

namespace Vision.Tools
{
    [ToolboxItem(false)]
    public partial class UcKkRobotTool : UserControl
    {
        public UcKkRobotTool(KkRobotCalibTool tool)
        {
            InitializeComponent();
            _tool = tool;
            ProjectManager.Instance.BeforeSaveProjectEvent += Instance_BeforeSaveProjectEvent;
        }

        private readonly KkRobotCalibTool _tool;
        private bool _init;

        /// <summary>
        /// 项目保存前置事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Instance_BeforeSaveProjectEvent(object sender, EventArgs e)
        {
            numInitX_ValueChanged(this, null);
            numInitY_ValueChanged(this, null);
            tbPLCX_TextChanged(this, null);
            tbPLCY_TextChanged(this, null);
        }

        private void Init()
        {
            if (_tool == null) return;

            if (_tool.KKOriginPosition == null)
            {
                _tool.KKOriginPosition = new PointD(0, 0);
            }

            //kk初始坐标
            numInitX.Value = (decimal)_tool.KKOriginPosition.X;
            numInitY.Value = (decimal)_tool.KKOriginPosition.Y;

            //plc地址初始化
            tbPLCX.Text = _tool.AddressX;
            tbPLCY.Text = _tool.AddressY;
        }

        private void Close()
        {
            ProjectManager.Instance.BeforeSaveProjectEvent -= Instance_BeforeSaveProjectEvent;
        }

        private void UcKRTool_Load(object sender, System.EventArgs e)
        {
            if (_tool != null)
            {
                cogCalibNPointToNPointEditV21.Subject = _tool.CalibTool;
                Init();
                _init = true;
            }
        }

        /// <summary>
        /// kk 初始x改变
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void numInitX_ValueChanged(object sender, System.EventArgs e)
        {
            if (_init)
            {
                _tool.KKOriginPosition.X = (double)numInitX.Value;
            }
        }

        /// <summary>
        /// kk 初始y改变
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void numInitY_ValueChanged(object sender, System.EventArgs e)
        {
            if (_init)
            {
                _tool.KKOriginPosition.Y = (double)numInitY.Value;
            }
        }

        /// <summary>
        /// kk现在x保持的plc地址
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tbPLCX_TextChanged(object sender, System.EventArgs e)
        {
            if (_init)
            {
                _tool.AddressX = tbPLCX.Text;
            }
        }

        /// <summary>
        /// kk现在y保持的plc地址
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tbPLCY_TextChanged(object sender, System.EventArgs e)
        {
            if (_init)
            {
                _tool.AddressY = tbPLCY.Text;
            }
        }

        private void btnCalc_Click(object sender, EventArgs e)
        {
            if (!_tool.CalibTool.Calibration.Calibrated)
            {
                "标定未完成，请先完成标定！".MsgBox();
                return;
            }

            var x = tbkk_x.Text;
            var y = tbkk_y.Text;

            if (string.IsNullOrEmpty(x) || string.IsNullOrEmpty(y))
            {
                "kk的坐标不正确，请检查！".MsgBox();
                return;
            }

            if (double.TryParse(x, out var kx) && double.TryParse(y, out var ky))
            {
                var cogtranform2DLinear = _tool.CalibTool.Calibration.GetComputedUncalibratedFromCalibratedTransform();
                cogtranform2DLinear.InvertBase().MapPoint(kx, ky, out var currentRx, out var currentRy);
                tbRX.Text = currentRx.ToString("f3");
                tbRY.Text = currentRy.ToString("f3");
            }
            else
            {
                "kk的坐标不正确，请检查！".MsgBox();
                return;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            tbRX.Clear();
            tbRY.Clear();
            tbkk_x.Clear();
            tbkk_y.Clear();
        }
    }
}
