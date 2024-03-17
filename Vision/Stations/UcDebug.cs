using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Cognex.VisionPro;
using Cognex.VisionPro.Implementation.Internal;
using Cognex.VisionPro.ToolBlock;
using Vision.Core;
using Vision.Projects;
using Vision.Tools.ToolImpls;

namespace Vision.Stations
{
    [ToolboxItem(false)]
    public partial class UcDebug : UserControl
    {
        private readonly Station _station;

        private CogAcqFifoTool _acqTool;

        private bool _init;

        private bool _living;

        public UcDebug(Station station)
        {
            InitializeComponent();
            _station = station;
            _station.RegisterDebugShow(this);
            _station.StationRanEvent += _station_StationRan;
            InitCamera();
        }

        public void StartLive()
        {
            if (_acqTool == null || cogRecordDisplay1.LiveDisplayRunning) return;
            if (_acqTool.Operator != null)
            {
                cogRecordDisplay1.AutoFit = true;
                cogRecordDisplay1.StaticGraphics.Clear();
                cogRecordDisplay1.InteractiveGraphics.Clear();
                cogRecordDisplay1.StartLiveDisplay(_acqTool.Operator);
                _living = true;
            }
        }

        public void StopLive()
        {
            if (_acqTool == null || !cogRecordDisplay1.LiveDisplayRunning) return;
            if (_acqTool.Operator != null)
            {
                cogRecordDisplay1.StaticGraphics.Clear();
                cogRecordDisplay1.InteractiveGraphics.Clear();
                cogRecordDisplay1.StopLiveDisplay();
                _living = false;
            }
        }

        private void InitCamera()
        {
            if (_station == null) return;
            if (_acqTool == null)
            {
                foreach (ToolBase tool in _station.ToolList)
                {
                    if (tool is ImageAcqTool aTool)
                    {
                        _acqTool = aTool.AcqFifoTool;
                    }
                }
            }
        }

        private void Close()
        {
            if (_station != null)
            {
                _station.StationRanEvent -= _station_StationRan;
            }
        }

        private void _station_StationRan(object sender, ShowWindowEventArgs e)
        {
            if (_station == null) return;
            if (_station.LastRecordName != null)
            {
                //清楚之前的显示
                ClearDisplay();
                foreach (var tool in _station.ToolList)
                {
                    if (tool is CenterDetectTool cdTool)
                    {
                        if (!e.IsNullImage)  
                        {
                            SetResultGraphicOnRecordDisplay(cdTool.ToolBlock, _station.LastRecordName);
                        }
                        SetTitle(e.Result ? "运行成功" : $"{e.ErrorMsg}", e.Result ? Color.Green : Color.Red);
                        SetTime(e.Time);
                    }
                    else if(tool is DetectTool dTool)
                    {
                        if (!e.IsNullImage) 
                        {
                            SetResultGraphicOnRecordDisplay(dTool.ToolBlock, _station.LastRecordName);
                        }
                        SetTitle(e.Result ? "运行成功" : $"{e.ErrorMsg}", e.Result ? Color.Green : Color.Red);
                        SetTime(e.Time);
                    }
                }
            }
            else
            {
                LogUI.AddLog("工位未设置输出图像，请检查！");
            }
        }

        private void SetResultGraphicOnRecordDisplay(CogToolBlock toolBlock, string recordName)
        {
            if (InvokeRequired)
            {
                cogRecordDisplay1.Invoke(new Action<CogToolBlock, string>(SetResultGraphicOnRecordDisplay),
                    toolBlock, recordName);
                return;
            }

            toolBlock.LastRunRecordEnable = CogUserToolLastRunRecordConstants.CompositeSubToolRecords;
            var lastRecord = toolBlock.CreateLastRunRecord();
            if (lastRecord != null && lastRecord.SubRecords.ContainsKey(recordName))
            {
                cogRecordDisplay1.Record = lastRecord.SubRecords[recordName];
                cogRecordDisplay1.Fit();
            }
            else
            {
                cogRecordDisplay1.Record = null;
                LogUI.AddLog("输出图像设置出现错误，请检查！");
            }
        }


        /// <summary>
        /// 清除显示界面
        /// </summary>
        public void ClearDisplay()
        {
            if (InvokeRequired)
            {
                Invoke(new Action(ClearDisplay));
                return;
            }

            cogRecordDisplay1.StaticGraphics.Clear();
            cogRecordDisplay1.InteractiveGraphics.Clear();
            cogRecordDisplay1.Image = null;
        }


        private delegate void SetTitleDelegate(string title, Color color);

        public void SetTitle(string title, Color color)
        {
            if (InvokeRequired)
            {
                Invoke(new SetTitleDelegate(SetTitle), title, color);
                return;
            }

            labelStatu.Text = title;
            labelStatu.ForeColor = color;
        }

        private delegate void SetTimeDelegate(TimeSpan time);

        public void SetTime(TimeSpan time)
        {
            if (InvokeRequired)
            {
                Invoke(new SetTimeDelegate(SetTime), time);
                return;
            }

            labelTime.Text = $"运行时间:{time.TotalMilliseconds:f2} ms";
        }

        private void UcStationShow_Load(object sender, System.EventArgs e)
        {
            UpdateComBox();
            _init = true;
        }

        private void UpdateComBox()
        {
            var names = _station.GetLastRunRecordName();
            comboBox1.Items.Clear();
            foreach (var name in names)
            {
                comboBox1.Items.Add(name);
            }
            if (names.Contains(_station.LastRecordName))
            {
                comboBox1.SelectedItem = _station.LastRecordName;
            }
            else
            {
                comboBox1.SelectedIndex = -1;
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_init)
            {
                if (comboBox1.SelectedIndex == -1) return;
                _station.LastRecordName = comboBox1.Text;
                ProjectManager.Instance.SaveProject();
            }
        }

        private void btnRun_Click(object sender, EventArgs e)
        {
            _station.DebugRun();
        }

        private void btnLive_Click(object sender, EventArgs e)
        {
            if (btnLive.Text == "连续采集" && !_living)
            {
                StartLive();
                if (_living)
                    btnLive.Text = "停止采集";
            }
            else if (btnLive.Text == "停止采集" && _living)
            {
                StopLive();
                if (!_living)
                    btnLive.Text = "连续采集";
            }
        }
    }
}
