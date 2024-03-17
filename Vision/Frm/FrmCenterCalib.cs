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
    public partial class FrmCenterCalib : Form
    {
        public FrmCenterCalib(CenterCalibTool tool)
        {
            InitializeComponent();
            _centerTool = tool;
            this.WindowState = FormWindowState.Maximized;
            this.BringToFront();
        }

        private ICogImage _image;
        private int _index = 0;

        private readonly CenterCalibTool _centerTool;
        private CogCalibNPointToNPointTool _nPointTool;
        private CogAcqFifoTool _acqTool;
        private CogFitCircleTool _fitCircleTool;
        private CogPMAlignTool _pmaTool;
        private CogFindCircleTool _findCircleTool;

        /// <summary>
        /// 计算标定
        /// </summary>
        private void Calibration()
        {
            try
            {
                if (_fitCircleTool != null)
                {
                    _fitCircleTool.InputImage = _image as CogImage8Grey;
                    if (_fitCircleTool.RunParams.NumPoints >= 3)
                    {
                        _fitCircleTool.Run();
                        if (_fitCircleTool.RunStatus.Result != CogToolResultConstants.Accept)
                        {
                            throw new Exception("标定失败！");
                        }
                        var x = _fitCircleTool.Result.GetX(0);
                        var y = _fitCircleTool.Result.GetY(0);
                        _centerTool.CenterPoint = new PointD(x, y);
                        _centerTool.IsCalibed = true;
                    }
                }
            }
            catch (Exception ex)
            {
                _centerTool.CenterPoint = null;
                _centerTool.IsCalibed = false;
                ex.MsgBox();
            }
        }

        /// <summary>
        /// 拍照一次 运行
        /// </summary>
        /// <returns></returns>
        private bool Grab()
        {
            if (cogRecordDisplay1.LiveDisplayRunning)
            {
                "请先停止连续相机取图".MsgBox();
                return false;
            }


            try
            {
                _image = null;
                //运行取像
                _acqTool.Run();
                _image = _acqTool.OutputImage;
                //运行9点标定
                _nPointTool.InputImage = _image;
                _nPointTool.Run();
                _image = _nPointTool.OutputImage;

                if (_image != null)
                {
                    return true;
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
        /// 初始化窗体
        /// </summary>
        private void InitControl()
        {
            if (!CheckTools()) return;
            try
            {
                //如果已经标定完成，将结果显示在界面
                if (_fitCircleTool == null) return;
                for (int i = 0; i < _fitCircleTool.RunParams.NumPoints; i++)
                {
                    var x1 = _fitCircleTool.RunParams.GetX(i);
                    var y1 = _fitCircleTool.RunParams.GetY(i);
                    dgv.Rows.Add(i + 1, x1, y1);
                }
            }
            catch (Exception ex)
            {
                ex.Message.MsgBox();
            }
        }

        /// <summary>
        /// 获取工具
        /// </summary>
        /// <returns></returns>
        private bool CheckTools()
        {
            try
            {
                if (_centerTool != null)
                {
                    _acqTool = _centerTool.ToolBlock.Tools["CogAcqFifoTool1"] as CogAcqFifoTool;
                    _nPointTool = _centerTool.ToolBlock.Tools["CogCalibNPointToNPointTool1"] as CogCalibNPointToNPointTool;
                    _pmaTool = _centerTool.ToolBlock.Tools["CogPMAlignTool1"] as CogPMAlignTool;
                    _findCircleTool = _centerTool.ToolBlock.Tools["CogFindCircleTool1"] as CogFindCircleTool;
                    _fitCircleTool = _centerTool.ToolBlock.Tools["CogFitCircleTool1"] as CogFitCircleTool;
                    return true;
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
        /// 设置datagridview
        /// </summary>
        private void InitDgv()
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
            if (dgv.Rows.Count < 3)
            {
                "标定的点位不足3个".MsgBox();
                return false;
            }
            try
            {
                int pNumber = _fitCircleTool.RunParams.NumPoints;
                for (int i = 0; i < pNumber; i++)
                {
                    _fitCircleTool.RunParams.DeletePoint(0);
                }

                for (int i = 0; i < dgv.RowCount; i++)
                {
                    var cell1 = dgv.Rows[i].Cells[1].Value;
                    var cell2 = dgv.Rows[i].Cells[2].Value;

                    if (cell1 == null || cell2 == null)
                    {
                        "点位不全，请检查！".MsgBox();
                        return false;
                    }

                    double x1 = Convert.ToDouble(cell1.ToString().Trim());
                    double y1 = Convert.ToDouble(cell2.ToString().Trim());

                    //_fitCircleTool.RunParams.SetPoint(i, x1, y1);
                    _fitCircleTool.RunParams.AddPoint(x1, y1);
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
                cogRecordDisplay1.Fit(true);
            }
        }

        /// <summary>
        /// 按钮背景色改变
        /// </summary>
        private void ActiveButtonEnable()
        {
            btnStopLive.Enabled = cogRecordDisplay1.LiveDisplayRunning;
            btnStartLive.Enabled = !cogRecordDisplay1.LiveDisplayRunning;
        }

        /// <summary>
        /// 窗体加载事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FrmCenterCalib_Load(object sender, System.EventArgs e)
        {
            ActiveButtonEnable();
            InitDgv();
            InitControl();
        }

        /// <summary>
        /// 运行一次 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnRun_Click(object sender, EventArgs e)
        {
            bool r = Grab();
            try
            {
                if (_index >= 5)
                    return;
                if (_fitCircleTool != null && r)
                {
                    //_pmaTool.InputImage = _image;

                    //_findCircleTool.InputImage = _image as CogImage8Grey;


                    _pmaTool.Run();

                    _findCircleTool.Run();

                    if (_pmaTool.RunStatus.Result == CogToolResultConstants.Accept &&
                       _findCircleTool.RunStatus.Result == CogToolResultConstants.Accept)
                    {

                        //判断结果
                        if (_findCircleTool.Results.Count > 0)
                        {
                            ICogRecord ir = _findCircleTool.CreateLastRunRecord();
                            ShowRecord(ir);
                            var x = _findCircleTool.Results.GetCircle().CenterX;
                            var y = _findCircleTool.Results.GetCircle().CenterY;
                            AddDgv(x, y);
                        }
                        else
                        {
                            ShowRecord(null);
                            cogRecordDisplay1.Image = _image;
                            cogRecordDisplay1.Record = null;
                            string err = "找圆未找到！";
                            err.MsgBox();
                            return;
                        }
                        _index++;
                    }
                }
            }
            catch (Exception ex)
            {
                ex.MsgBox();
            }
        }

        /// <summary>
        /// 清除拟合圆参数
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnClear_Click(object sender, EventArgs e)
        {
            if (_fitCircleTool != null)
            {
                var number = _fitCircleTool.RunParams.NumPoints;
                for (int i = 0; i < number; i++)
                {
                    _fitCircleTool.RunParams.DeletePoint(0);
                    dgv.Rows.RemoveAt(0);
                }
            }
        }

        /// <summary>
        /// 开始采集
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnStartLive_Click(object sender, EventArgs e)
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

        /// <summary>
        /// 停止采集
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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

        /// <summary>
        /// 标定按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCalc_Click(object sender, EventArgs e)
        {
            if (SetPoints())
            {
                Calibration();
            }
        }

        /// <summary>
        /// 保存标定按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSaveCalib_Click(object sender, EventArgs e)
        {
            _centerTool?.SaveVpp();
        }

        private void FrmCenterCalib_FormClosing(object sender, FormClosingEventArgs e)
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
