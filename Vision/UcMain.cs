using PlcComm;
using System;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Forms;
using Vision.Core;
using Vision.Projects;
using Vision.Stations;

namespace Vision
{
    [ToolboxItem(false)]
    public partial class UcMain : UserControl
    {
        public UcMain(Melsoft_PLC_TCP2 plc)
        {
            InitializeComponent();
            //加载plc
            if (ProjectManager.Instance.IsLoaded)
            {
                ProjectManager.Instance.ProjectData.RegisterPlc(plc);
            }
            _ucWindow = new UcWindowShow(ProjectManager.Instance.ProjectData);
            _ucProject = new UcProject();
            _ucSet = new UcSet();

            //Init();
            //ShowLog();
            if (ProjectManager.Instance.ProjectData.SystemConfig.AutoRun)
            {
                Run();
            }
            ProjectManager.Instance.UcStationChangedEvent += Instance_UcStationChangedEvent;
        }

        private UcWindowShow _ucWindow;
        private UcProject _ucProject;
        private UcSet _ucSet;
        private bool _cycle;             //检测循环
        private bool _logCycle = true;   //log循环flag

        /// <summary>
        /// 关闭窗体时调用
        /// </summary>
        public void Close()
        {
            _logCycle = false;
            if (_cycle)
            {
                Stop();
            }
            if (ProjectManager.Instance.IsLoaded)
            {
                ProjectManager.Instance.CloseProject();
            }
            ProjectManager.Instance.UcStationChangedEvent -= Instance_UcStationChangedEvent;
        }

        /// <summary>
        /// 初始化
        /// </summary>
        private void Init()
        {
            if (!ProjectManager.Instance.IsLoaded)
            {
                return;
            }
            AddControl(_ucWindow);
        }

        /// <summary>
        /// 显示log
        /// </summary>
        private void ShowLog()
        {
            Task.Run(async () =>
            {
                while (_logCycle)
                {
                    var s = LogUI.GetLog();
                    listBox1.BeginInvoke(new Action(() =>
                    {
                        if (s != null)
                        {
                            if (listBox1.Items.Count > 200)
                            {
                                listBox1.Items.RemoveAt(0);
                            }
                            listBox1.Items.Add(s);
                            listBox1.SelectedIndex = listBox1.Items.Count - 1;
                        }
                    }));
                    await Task.Delay(100);
                }
            });
        }

        /// <summary>
        /// 添加控件
        /// </summary>
        /// <param name="control"></param>
        private void AddControl(UserControl control)
        {
            panel1.Controls.Clear();
            control.Dock = DockStyle.Fill;
            panel1.Controls.Add(control);
        }

        /// <summary>
        /// 开始运行
        /// </summary>
        private void Run()
        {
            foreach (Station station in ProjectManager.Instance.ProjectData.StationList)
            {
                station.StartCycle();
                _cycle = true;
            }
            AddControl(_ucWindow);
            _ucWindow.ShowUnit();
        }

        /// <summary>
        /// 停止运行
        /// </summary>
        private void Stop()
        {
            foreach (Station station in ProjectManager.Instance.ProjectData.StationList)
            {
                station.StopCycle();
                _cycle = false;
            }
        }

        private void toolStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            var item = e.ClickedItem;
            var err = "请先停止运行";
            switch (item.Text)
            {
                case "视觉":
                    if (_cycle)
                    {
                        //err.MsgBox();
                        //return;
                        Stop();
                    }
                    AddControl(_ucProject);
                    break;
                case "设置":
                    if (_cycle)
                    {
                        // err.MsgBox();
                        //return;
                        Stop();
                    }
                    AddControl(_ucSet);
                    break;
                case "运行":
                    Run();
                    break;
                case "停止":
                    Stop();
                    break;
                case "保存":
                    ProjectManager.Instance.SaveProject();
                    break;
            }
        }

        private void Instance_UcStationChangedEvent(object sender, StationShowChangedEventArgs e)
        {
            if (e.ShowOne)
                _ucWindow.ShowUnit(e.StationName);
            else
                _ucWindow.ShowUnit();
        }

        private void FrmMain_Load(object sender, System.EventArgs e)
        {
            Init();
            ShowLog();
            //if (ProjectManager.Instance.ProjectData.SystemConfig.AutoRun)
            //{
            //    Run();
            //}
        }
    }
}
