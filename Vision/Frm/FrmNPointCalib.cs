using System;
using System.Windows.Forms;
using Cognex.VisionPro;
using Cognex.VisionPro.CalibFix;
using Cognex.VisionPro.Caliper;
using Cognex.VisionPro.PMAlign;
using Vision.Core;
using Vision.Tools.ToolImpls;

namespace Vision.Frm
{
    public partial class FrmNPointCalib : Form
    {
        public FrmNPointCalib(NPointCalibTool nTool)
        {
            InitializeComponent();
            _nTool = nTool;
            this.WindowState = FormWindowState.Maximized;
            this.BringToFront();
        }

        private readonly NPointCalibTool _nTool;
        private ICogImage _image;
        private int _index = 0;

        private CogAcqFifoTool _acqTool;
        private CogPMAlignTool _pmaTool;
        private CogFindCircleTool _circleTool;
        private CogCalibNPointToNPointTool _nPointTool;

        /// <summary>
        /// 采集按钮颜色改变
        /// </summary>
        private void ActiveButtonEnable()
        {
            btnStopLive.Enabled = cogRecordDisplay1.LiveDisplayRunning;
            btnStartLive.Enabled = !cogRecordDisplay1.LiveDisplayRunning;
        }

        private void Calibration()
        {
            try
            {
                _nPointTool.Calibration.Calibrate();
            }
            catch (Exception ex)
            {
                ex.Message.MsgBox();
            }
        }

        private bool Grab()
        {
            if (cogRecordDisplay1.LiveDisplayRunning)
            {
                "请先停止连续相机取图".MsgBox();
                return false;
            }

            try
            {
                if (_acqTool != null)
                {
                    _acqTool.Run();
                    if (_acqTool.RunStatus.Result == CogToolResultConstants.Accept)
                    {
                        _image = _acqTool.OutputImage;
                        return true;
                    }
                    else
                    {
                        "取图失败".MsgBox();
                        return false;
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                ex.MsgBox();
                return false;
            }
        }

        /// <summary>
        /// 检查工具是否齐全
        /// </summary>
        /// <returns></returns>
        private bool CheckTools()
        {
            if (_nTool != null)
            {
                var tb = _nTool.ToolBlock;
                if (tb != null)
                {
                    try
                    {
                        _acqTool = tb.Tools["CogAcqFifoTool1"] as CogAcqFifoTool;
                        _pmaTool = tb.Tools["CogPMAlignTool1"] as CogPMAlignTool;
                        _circleTool = tb.Tools["CogFindCircleTool1"] as CogFindCircleTool;
                        _nPointTool = tb.Tools["CogCalibNPointToNPointTool1"] as CogCalibNPointToNPointTool;
                        return true;
                    }
                    catch
                    {
                        return false;
                    }
                }
                return false;
            }
            return false;
        }
        
        /// <summary>
        /// 如果已经标定 将标定的点位显示到表格中
        /// </summary>
        private void InitControl()
        {
            if (CheckTools())
            {
                try
                {
                    //如果已经标定完成，将结果显示在界面
                    if (_nPointTool.Calibration.Calibrated)
                    {
                        for (int i = 0; i < _nPointTool.Calibration.NumPoints; i++)
                        {
                            var x1 = _nPointTool.Calibration.GetUncalibratedPointX(i);
                            var y1 = _nPointTool.Calibration.GetUncalibratedPointY(i);
                            var x2 = _nPointTool.Calibration.GetRawCalibratedPointX(i);
                            var y2 = _nPointTool.Calibration.GetRawCalibratedPointY(i);
                            dgv.Rows.Add(i + 1, x1, y1, x2, y2);
                        }
                    }
                }
                catch (Exception ex)
                {
                    ex.Message.MsgBox();
                }
            }
            else
            {
                "标定工具不全，请检查！".MsgBox();
            }
        }

        /// <summary>
        /// 设置表格显示
        /// </summary>
        private void SetDgv()
        {
            for (int i = 0; i < 3; i++)
            {
                dgv.Columns[i].ReadOnly = true;
            }
            dgv.RowHeadersVisible = false;
            dgv.AllowUserToAddRows = false;
            dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }

        /// <summary>
        /// 传入点位
        /// </summary>
        private bool SetPoints()
        {
            if (dgv.Rows.Count != 9)
            {
                "标定的点位不足9个".MsgBox();
                return false;
            }
            try
            {
                //清除原有点位
                int pNumber = _nPointTool.Calibration.NumPoints;
                for (int i = 0; i < pNumber; i++)
                {
                    _nPointTool.Calibration.DeletePointPair(0);
                }

                //添加新的点位
                for (int i = 0; i < 9; i++)
                {
                    var cell1 = dgv.Rows[i].Cells[1].Value;
                    var cell2 = dgv.Rows[i].Cells[2].Value;
                    var cell3 = dgv.Rows[i].Cells[3].Value;
                    var cell4 = dgv.Rows[i].Cells[4].Value;

                    if (cell1 == null || cell2 == null || cell3 == null || cell4 == null)
                    {
                        "点位不全，请检查！".MsgBox();
                        return false;
                    }

                    double x1 = Convert.ToDouble(cell1.ToString().Trim());
                    double y1 = Convert.ToDouble(cell2.ToString().Trim());
                    double x2 = Convert.ToDouble(cell3.ToString().Trim());
                    double y2 = Convert.ToDouble(cell4.ToString().Trim());


                    _nPointTool.Calibration.AddPointPair(x1, y1, x2, y2);
                    //_nPointTool.Calibration.SetUncalibratedPoint(i, x1, y1);
                    //_nPointTool.Calibration.SetRawCalibratedPoint(i, x2, y2);
                    //_nPointTool.Calibration.NumPoints = 9;
                }
                return true;
            }
            catch (Exception ex)
            {
                $"点位设置失败，请检查\r\n{ex.Message}".MsgBox();
                return false;
            }
        }

        /// <summary>
        /// dgv添加数据
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        private void AddDgv(double x, double y)
        {
            dgv.Rows.Add(_index + 1, x, y);
        }

        /// <summary>
        /// 显示图像
        /// </summary>
        /// <param name="record"></param>
        private void ShowRecord(ICogRecord record)
        {
            //先清除图像
            cogRecordDisplay1.StaticGraphics.Clear();
            cogRecordDisplay1.InteractiveGraphics.Clear();
            cogRecordDisplay1.Image = null;

            if (record != null)
            {
                cogRecordDisplay1.Image = _image;
                cogRecordDisplay1.Record = record;
                cogRecordDisplay1.AutoFit = true;
            }
        }

        private void btnStartLive_Click(object sender, System.EventArgs e)
        {
            if (_acqTool != null && !cogRecordDisplay1.LiveDisplayRunning)
            {
                if (_acqTool.Operator != null)
                {
                    cogRecordDisplay1.StaticGraphics.Clear();
                    cogRecordDisplay1.InteractiveGraphics.Clear();
                    cogRecordDisplay1.StartLiveDisplay(_acqTool.Operator);
                    cogRecordDisplay1.AutoFit = true;
                    cogRecordDisplay1.Fit(true);
                }
                else
                {
                    "相机未设置".MsgBox();
                }
            }

            ActiveButtonEnable();
        }

        private void btnStopLive_Click(object sender, EventArgs e)
        {
            if (_acqTool != null && cogRecordDisplay1.LiveDisplayRunning)
            {
                if (_acqTool.Operator != null)
                    cogRecordDisplay1.StopLiveDisplay();
                else
                    "相机未设置".MsgBox();
            }

            ActiveButtonEnable();
        }

        private void FrmNCalib_Load(object sender, EventArgs e)
        {
            ActiveButtonEnable();
            SetDgv();
            InitControl();
        }

        private void btnSaveCalib_Click(object sender, EventArgs e)
        {
            _nTool?.SaveVpp();
        }

        private void btnRun_Click(object sender, EventArgs e)
        {
            bool r = Grab();
            try
            {
                if (_index >= 9)
                    return;
                if (r)
                {
                    //_pmaTool.InputImage = _image;
                    //_circleTool.InputImage = _image as CogImage8Grey;

                    _pmaTool.Run();
                    _circleTool.Run();


                    //判断结果
                    if (_pmaTool.RunStatus.Result == CogToolResultConstants.Accept &&
                        _circleTool.RunStatus.Result == CogToolResultConstants.Accept)
                    {
                        ICogRecord ir = _circleTool.CreateLastRunRecord();
                        ShowRecord(ir);
                        var x = _circleTool.Results.GetCircle().CenterX;
                        var y = _circleTool.Results.GetCircle().CenterY;
                        AddDgv(x, y);
                    }
                    else
                    {
                        ShowRecord(null);
                        cogRecordDisplay1.Image = _image;
                        cogRecordDisplay1.Record = null;
                        string err = "模板未找到！";
                        err.MsgBox();
                        return;
                    }
                    _index++;
                }
            }
            catch (Exception ex)
            {
                ex.MsgBox();
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            if (_nPointTool.Calibration.Calibrated)
            {
                int count = _nPointTool.Calibration.NumPoints;
                for (int i = 0; i < count; i++)
                {
                    _nPointTool.Calibration.DeletePointPair(0);
                }
            }
            dgv.Rows.Clear();
            _index = 0;
        }

        private void btnCalc_Click(object sender, EventArgs e)
        {
            if (SetPoints())
            {
                Calibration();
            }
        }

        private void FrmNPointCalib_FormClosing(object sender, FormClosingEventArgs e)
        {
            //if (_acqTool != null && cogRecordDisplay1.LiveDisplayRunning)
            //{
            //    if (_acqTool.Operator != null)
            //    {
            //        cogRecordDisplay1.StopLiveDisplay();
            //        if (_acqTool.Operator.FrameGrabber != null)
            //        {
            //            _acqTool.Operator.FrameGrabber.Disconnect(true);
            //            _acqTool.Dispose();
            //            _acqTool = null;
            //        }
            //    }
                    
            //}
        }
    }
}
