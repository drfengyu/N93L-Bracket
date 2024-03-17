using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Cognex.VisionPro;
using Cognex.VisionPro.ToolBlock;
using Vision.Core;
using Vision.Projects;
using Vision.Tools.Interfaces;
using Vision.Tools.ToolImpls;

namespace Vision.Stations
{
    /// <summary>
    /// 工位类
    /// 每一个工位对应一个工位类
    /// </summary>
    [Serializable]
    public class Station
    {
        private string _name;

        [NonSerialized]
        private ManualResetEvent _manualResetEvent;

        [NonSerialized]
        private Thread _cycleThread;

        [NonSerialized]
        private bool _cycleFlag;

        /// <summary>
        /// 工位名称
        /// </summary>
        public string Name
        {
            get => _name;
            set
            {
                if (_name == value) return;
                _name = value;
                DisplayView?.SetTitle(value);
                OnStationNameChanged(new StationEventArgs(this));
            }
        }

        /// <summary>
        /// 工位显示record名称
        /// </summary>
        public string LastRecordName { get; set; }

        /// <summary>
        /// 工具集合
        /// </summary>
        public List<ToolBase> ToolList { get; set; }

        /// <summary>
        /// 工位路径
        /// </summary>
        public string StationPath { get; set; }

        [field: NonSerialized]
        public bool Living { get; set; }

        [field: NonSerialized]
        public UcDebug UcDebug { get; private set; }

        [field: NonSerialized]
        public event EventHandler<StationEventArgs> StationNameChangedEvent;

        [field: NonSerialized]
        public event EventHandler<ShowWindowEventArgs> StationRanEvent;

        [field: NonSerialized]
        public event EventHandler<StationShowChangedEventArgs> ShowDisplayChangedEvent;

        [field: NonSerialized]
        public bool Cycle { get; private set; }

        [field:NonSerialized]
        public CogDisplayView DisplayView { get; set; }

        [field: NonSerialized]
        public bool OK { get; set; }

        /// <summary>
        /// 机械手的示教位
        /// </summary>
        public PointA RobotOriginPosition { get; set; }

        /// <summary>
        /// 示教时模板的点位
        /// </summary>
        public PointA ModelPosition { get; set; }

        /// <summary>
        /// 示教时KK点位
        /// </summary>
        public PointD KKOriginPosition { get; set; }

        /// <summary>
        /// 旋转标定时机械手点位
        /// </summary>
        public PointD CenterCalibRobotPoint { get; set; }

        public ToolBase this[string name]
        {
            get
            {
                if (this.ToolList != null)
                {
                    return this.ToolList.FirstOrDefault(tool => tool.Name == name);
                }
                else
                {
                    return null;
                }
            }
        }

        public Station()
        {
            ToolList = new List<ToolBase>();
        }

        #region 运行相关
        /// <summary>
        /// 运行单次
        /// </summary>
        public void Run()
        {
            if (ToolList.Count > 0)
            {
                var result = true;
                bool nullImage = false;
                string errMsg = string.Empty;
                Stopwatch stopwatch = new Stopwatch();

                foreach (ToolBase tool in ToolList)
                {
                    try
                    {
                        //等待触发信号之后再进行 计时的开始
                        if (tool is TriggerTool )
                        {
                            tool.Run();
                            stopwatch.Start();
                        }
                        else if (tool is DetectTool || tool is CenterDetectTool)
                        {
                            tool.Run();
                            OK = true;
                        }
                        else
                        {
                            tool.Run();
                        }
                    }
                    catch (ToolException ex)
                    {
                        result = false;
                        nullImage = ex.ImageInNull;
                        errMsg = $"工具 [{tool.Name}] 运行失败！";
                        LogUI.AddLog($"{Name} {ex.Message}");
                        OK = false;
                        continue;
                    }
                }
                stopwatch.Stop();
                var time = stopwatch.Elapsed;
                ShowWindow(new ShowWindowEventArgs(result, time, nullImage, errMsg));
            }
        }

        /// <summary>
        /// 调试运行
        /// </summary>
        public void DebugRun()
        {
            if (ToolList.Count > 0)
            {
                var result = true;
                bool nullImage = false;
                string errMsg = string.Empty;
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();
                foreach (ToolBase tool in ToolList)
                {
                    try
                    {
                        if (tool is TriggerTool || tool is EndTool ||tool is ResultTool || tool is KkRobotCalibTool)
                        {
                            continue;
                        }
                        tool.Run();
                    }
                    catch (ToolException ex)
                    {
                        result = false;
                        nullImage = ex.ImageInNull;
                        errMsg = $"[{tool.Name}] 运行失败！";
                        LogUI.AddLog(ex.Message);
                        break;
                    }
                }
                stopwatch.Stop();
                var time = stopwatch.Elapsed;
                ShowDebugWindow(new ShowWindowEventArgs(result, time, nullImage, errMsg));
            }
        }

        /// <summary>
        /// 如果未开启线程 则开启线程
        /// 如果已经开启 则将线程继续
        /// </summary>
        public void StartCycle()
        {
            if (!Cycle)
            {
                if (_manualResetEvent == null)
                {
                    _manualResetEvent = new ManualResetEvent(true);
                }

                _cycleThread = new Thread(RunCycle)
                {
                    IsBackground = true
                };
                _cycleFlag = true;
                _cycleThread.Start();
            }
            else
            {
                _manualResetEvent?.Set();
            }
        }

        /// <summary>
        /// 循环运行
        /// </summary>
        private void RunCycle()
        {
            Cycle = true;
            while (_cycleFlag)
            {
                _manualResetEvent?.WaitOne();
                Run();
                Task.Run(SaveImage);
                Thread.Sleep(10);
            }
        }

        /// <summary>
        /// 停止循环运行
        /// </summary>
        public void StopCycle()
        {
            _manualResetEvent?.Reset();
        }

        /// <summary>
        /// 窗口显示
        /// </summary>
        /// <param name="args"></param>
        private void ShowWindow(ShowWindowEventArgs args)
        {
            //运行视图模式下显示
            if (DisplayView != null)
            {
                //先清除之前的显示
                DisplayView.ClearDisplay();

                CogToolBlock tb = null;
                if (ToolList != null && ToolList.Count > 0)
                {
                    foreach (ToolBase tool in ToolList)
                    {
                        if (tool is CenterDetectTool detectTool)
                        {
                            tb = detectTool.ToolBlock;
                            break;
                        }
                        else if (tool is DetectTool dTool)
                        {
                            tb = dTool.ToolBlock;
                            break;
                        }
                    }
                }
                if (tb != null)
                {
                    if (!args.IsNullImage)  //有图像
                    {
                        DisplayView.SetResultGraphicOnRecordDisplay(tb, LastRecordName);
                        DisplayView.SetTime(args.Time);
                    }
                }
            }
        }

        /// <summary>
        /// 调试模式显示
        /// </summary>
        /// <param name="args"></param>
        private void ShowDebugWindow(ShowWindowEventArgs args)
        {
            //debug模式下
            OnStationRan(args);
        }

        /// <summary>
        /// 保存图像
        /// </summary>
        /// <exception cref="System.Exception">当图像保存的路径未设置时 抛出路径未设置异常</exception>
        private void SaveImage()
        {
            try
            {
                var config = ProjectManager.Instance.ProjectData.ImageConfig;
                if (string.IsNullOrEmpty(config.SaveImageDir))
                {
                    string err = "未设置图像的保存路径！";
                    LogNetHelper.Log(err);
                    throw new System.Exception(err);
                }
                //按天进行保存 每张图像的名称就是当前的时间格式
                string fileName = DateTime.Now.ToString("HH_mm_ss_fff");

                //按配置进行OK,NG存储
                if (config.IsSaveOKImage && OK)    //OK
                {
                    DisplayView?.SaveOriginImage(config.SaveImageDir + $"\\{Name}\\OK\\", fileName);
                }
                if (config.IsSaveNGImage && !OK)   //NG
                {
                    //保存原图
                    DisplayView?.SaveOriginImage(config.SaveImageDir + $"\\{Name}\\NG\\", fileName);
                    //保存截屏
                    DisplayView?.SaveScreenImage(config.SaveImageDir + $"\\{Name}\\NG\\", fileName + "_");
                }
            }
            catch (Exception ex)
            {
                LogNetHelper.Log(ex.Message);
            }
        }

        #endregion

        #region 工具相关
        /// <summary>
        /// 新建工具
        /// </summary>
        /// <param name="tool"></param>
        public void AddTool(ToolBase tool)
        {
            string name = GenDefaultToolName(tool);
            tool.Name = name;
            try
            {
                switch (tool)
                {
                    case IVpp iTool:
                        iTool.ToolPath = Path.Combine(StationPath + $"\\{tool.Name}.vpp");
                        iTool.RegisterStation(this);
                        iTool.CreateVpp();
                        break;
                    case IRegisterStation rTool:
                        rTool.RegisterStation(this);
                        break;
                }
                LogUI.AddLog($"[{tool.Name}]新建成功");
            }
            catch (Exception ex)
            {
                LogUI.AddLog($"工具新建失败  " + ex.Message);
            }
            ToolList.Add(tool);
        }

        /// <summary>
        /// 删除工具
        /// </summary>
        /// <param name="tool"></param>
        public void DeleteTool(ToolBase tool)
        {
            //先判断存在
            if (this[tool.Name] != null)
            {
                ToolList.Remove(tool);
                LogUI.AddLog($"[{tool.Name}]移除成功");
            }
        }

        /// <summary>
        /// 工具重命名
        /// </summary>
        /// <param name="tool"></param>
        /// <param name="newName"></param>
        /// <exception cref="Exception"></exception>
        public void RenameTool(ToolBase tool, string newName)
        {
            if (tool == null) return;

            //判断新的名称工具是否已经存在
            if (this[newName] != null)
            {
                LogUI.AddLog("新的名称已经存在！");
            }

            string oldName = tool.Name;
            //判断旧的工具是否存在
            if (this[tool.Name] == null) return;
            foreach (var t in ToolList)
            {
                if (t.Name == tool.Name)
                {
                    t.Name = newName;
                    if (t is IVpp iTool)
                    {
                        var oldPath = iTool.ToolPath;
                        iTool.ToolPath = StationPath + $"\\{newName}.vpp";
                    }
                }
            }
            LogUI.AddLog($"[{oldName}]重命名成功");
        }

        /// <summary>
        /// 删除所有工具
        /// </summary>
        public void RemoveAllTool()
        {
            if (ToolList != null)
            {
                ToolList.Clear();
                LogUI.AddLog($"[{Name}]所有工具删除成功");
            }
        }

        /// <summary>
        /// 获取toolblock下所有的record名称
        /// </summary>
        /// <returns></returns>
        public List<string> GetLastRunRecordName()
        {
            List<string> lists = new List<string>();

            //遍历所有CenterDetectTool
            foreach (var tool in ToolList)
            {
                if (tool is CenterDetectTool dTool)
                {
                    var lastRecord = dTool.ToolBlock.CreateLastRunRecord();
                    if (lastRecord != null)
                    {
                        foreach (ICogRecord r in lastRecord.SubRecords)
                        {
                            lists.Add(r.RecordKey);
                        }
                    }
                }
            }
            //遍历所有CenterDetectTool
            foreach (var tool in ToolList)
            {
                if (tool is DetectTool dTool)
                {
                    var lastRecord = dTool.ToolBlock.CreateLastRunRecord();
                    if (lastRecord != null)
                    {
                        foreach (ICogRecord r in lastRecord.SubRecords)
                        {
                            lists.Add(r.RecordKey);
                        }
                    }
                }
            }

            return lists;
        }

        /// <summary>
        /// 注册debug窗口
        /// </summary>
        /// <param name="ucStation"></param>
        public void RegisterDebugShow(UcDebug ucStation)
        {
            UcDebug = ucStation;
        }

        /// <summary>
        /// 获取所有IModelPoint的Tool名称
        /// </summary>
        /// <returns></returns>
        public List<string> GetModelPoint()
        {
            List<string> inputs = new List<string>();
            foreach (var tool in ToolList)
            {
                if (tool is IModelPoint)
                {
                    //只添加之前的工具
                    if (tool.Name == Name)
                        break;
                    inputs.Add(tool.Name);
                }
            }
            return inputs;
        }

        /// <summary>
        /// 获取所有继承KK机器人接口的工具名称
        /// </summary>
        /// <returns></returns>
        public List<string> GetRobotDeltaPoint()
        {
            List<string> inputs = new List<string>();
            foreach (var tool in ToolList)
            {
                if (tool is IRobotDeltaPoint)
                {
                    //只添加之前的工具
                    if (tool.Name == Name)
                        break;
                    inputs.Add(tool.Name);
                }
            }
            return inputs;
        }

        /// <summary>
        /// 获取所有继承INpointDelta的Tool名称
        /// </summary>
        /// <returns></returns>
        public List<string> GetNPointDeltaPoint()
        {
            List<string> inputs = new List<string>();
            foreach (var tool in ToolList)
            {
                if (tool is INPoint)
                {
                    //只添加之前的工具
                    if (tool.Name == Name)
                        break;
                    inputs.Add(tool.Name);
                }
            }
            return inputs;
        }

        /// <summary>
        /// 获取所有9点标定工具名称
        /// </summary>
        /// <returns></returns>
        public List<string> GetNPointToolName()
        {
            List<string> inputs = new List<string>();
            foreach (var item in ToolList)
            {
                if (item is NPointCalibTool nTool)
                {
                    inputs.Add(nTool.Name);
                }
            }
            return inputs;
        }

        /// <summary>
        /// 注册station主窗体显示界面
        /// </summary>
        /// <returns></returns>
        public CogDisplayView RegisterViewDisplay()
        {
            if (DisplayView == null)
            {
                DisplayView = new CogDisplayView();
                DisplayView.SetTitle(Name);
                DisplayView.ShowDisplay += _displayView_ShowDisplayOne;
            }
            return DisplayView;
        }

        /// <summary>
        /// 获取所有图像工具名称
        /// </summary>
        /// <param name="tool"></param>
        /// <returns></returns>
        public object[] GetImageInToolNames(ToolBase tool)
        {
            List<string> list = new List<string>();
            if (ToolList.Count > 0)
            {
                foreach (var toolBase in ToolList)
                {
                    if (toolBase == tool)
                    {
                        break;
                    }
                    if (toolBase is IImageOut)
                    {
                        if (toolBase.Enable)
                            list.Add(toolBase.Name);
                    }
                }
            }
            return list.ToArray();
        }

        public void Close()
        {
            foreach (var t in ToolList)
            {
                t.Close();
            }
            if (_cycleFlag)
            {
                _cycleFlag = false;
                _cycleThread.Abort();
                _cycleThread.Join();
            }
            if (DisplayView == null)
            {
                DisplayView.ShowDisplay -= _displayView_ShowDisplayOne;
            }
        }

        /// <summary>
        /// 生成默认的工具名称
        /// </summary>
        /// <param name="tool"></param>
        /// <returns></returns>
        private string GenDefaultToolName(ToolBase tool)
        {
            int m = 1;

            var defaultName = tool.GetType().GetCustomAttribute<ToolNameAttribute>()?.Name;
            string name = defaultName + m.ToString();
            while (ToolExsit(name))
            {
                m++;
                name = defaultName + m.ToString();
            }
            return defaultName + m.ToString();
        }

        /// <summary>
        /// 检查工具是否存在
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        private bool ToolExsit(string name)
        {
            return ToolList.Any(x => x.Name == name);
        }

        /// <summary>
        /// 工位名称改变事件触发器
        /// </summary>
        /// <param name="e"></param>
        private void OnStationNameChanged(StationEventArgs e)
        {
            if (StationNameChangedEvent != null)
            {
                var eventHandler = StationNameChangedEvent;
                eventHandler.Invoke(this, e);
            }
        }

        /// <summary>
        /// 工位运行事件触发器
        /// </summary>
        /// <param name="args"></param>
        private void OnStationRan(ShowWindowEventArgs args)
        {
            if (StationRanEvent != null)
            {
                var e = StationRanEvent;
                e.Invoke(this, args);
            }
        }

        private void _displayView_ShowDisplayOne(object sender, StationShowChangedEventArgs e)
        {
            if (ShowDisplayChangedEvent != null)
            {
                e.StationName = Name;
                ShowDisplayChangedEvent(sender, e);
            }
        }
        #endregion
    }
}