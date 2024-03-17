using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using Vision.Core;
using Vision.Stations;
using Vision.Tools.Interfaces;
using Vision.Tools.ToolImpls;

namespace Vision.Projects
{
    public class ProjectManager
    {
        private static ProjectManager _instance;
        private Project _project;

        private ProjectManager()
        {
            if (!Directory.Exists(ProjectDir))
            {
                Directory.CreateDirectory(ProjectDir);
            }
            try
            {
                OpenProject();
            }
            catch (Exception ex)
            {
                ex.MsgBox();
            }
        }

        /// <summary>
        /// 更新UI事件
        /// </summary>
        public event EventHandler UpdateProjectUIEvent;

        /// <summary>
        /// 项目保存前置事件
        /// </summary>
        public event EventHandler BeforeSaveProjectEvent;

        public event EventHandler<StationShowChangedEventArgs> UcStationChangedEvent;

        public static ProjectManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new ProjectManager();
                }
                return _instance;
            }
        }

        /// <summary>
        /// 项目已经加载
        /// </summary>
        public bool IsLoaded { get; private set; }

        /// <summary>
        /// Project数据
        /// </summary>
        public Project ProjectData
        {
            get => _project;
            set => _project = value;
        }

        /// <summary>
        /// 项目文件路径
        /// </summary>
        public string ProjectPath => Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Project", "proj.vpr");

        /// <summary>
        /// 项目文件夹
        /// </summary>
        public string ProjectDir => Path.Combine(AppDomain.CurrentDomain.BaseDirectory + "Project");

        #region 【项目加载保存】

        /// <summary>
        /// 保存project 
        /// </summary>
        /// <exception cref="Exception">如果保存出现错误 返回exception</exception>
        public bool SaveProject()
        {
            if (!IsLoaded) return false;
            //项目保存前置事\

            try
            {
                OnBeforeSaveProject();
            }
            catch (Exception ex)
            {
                LogNetHelper.Log("项目保存失败！\r\n " + ex.Message);
                return false;
            }

            try
            {
                List<string> stationPaths = new List<string>();
                //遍历station
                foreach (var station in _project.StationList)
                {
                    //工程保存的文件夹
                    //没有则创建
                    if (!Directory.Exists(station.StationPath))
                    {
                        Directory.CreateDirectory(station.StationPath);
                    }

                    stationPaths.Add(station.StationPath);

                    //遍历工具 
                    //保存vpp
                    List<string> vppPaths = new List<string>();
                    foreach (var tool in station.ToolList)
                    {
                        if (tool is IVpp iTool)
                        {
                            //工位文件夹检查
                            if (!Directory.Exists(Directory.GetParent(iTool.ToolPath)?.FullName))
                            {
                                var fullName = Directory.GetParent(iTool.ToolPath)?.FullName;
                                if (fullName != null)
                                    Directory.CreateDirectory(fullName);
                            }

                            iTool.SaveVpp();
                            vppPaths.Add(iTool.ToolPath);
                        }
                    }

                    //遍历文件夹
                    //如果多了vpp 删除

                    var vpps = Directory.GetFiles(station.StationPath, "*.vpp");
                    foreach (var item in vpps)
                    {
                        if (!vppPaths.Contains(item))
                        {
                            File.Delete(Path.GetFullPath(item));
                        }
                    }

                }

                //遍历project下所有的文件夹
                //如果有station之外的 删除

                var dirs = Directory.GetDirectories(ProjectDir);
                foreach (var dir in dirs)
                {
                    if (!stationPaths.Contains(dir))
                    {
                        Directory.Delete(dir, true);
                    }
                }


                var res = SerializerHelper.SerializeToBinary(_project, ProjectPath);
                LogNetHelper.Log($"项目保存成功！");
                if (!res)
                {
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                ex.Message.MsgBox();
                return false;
            }
        }

        /// <summary>
        /// 打开项目
        /// </summary>
        public void OpenProject()
        {
            if (IsLoaded)
            {
                return;
            }

            if (!File.Exists(ProjectPath))
            {
                _project = new Project();
                IsLoaded = true;
                OnUpdateProjectUI();
                LogNetHelper.Log("项目创建成功！");
                return;
            }
            try
            {
                Project data = SerializerHelper.DeSerializeFromBinary<Project>(ProjectPath);
                if (data != null)
                {
                    _project = data;
                    //加载vpp
                    foreach (var station in _project.StationList)
                    {
                        foreach (var tool in station.ToolList)
                        {
                            switch (tool)
                            {
                                case null:
                                    return;
                                case IVpp aTool:
                                    if (!aTool.IsLoaded)
                                    {
                                        aTool.RegisterStation(station);
                                        aTool.LoadVpp();
                                    }
                                    break;
                                case IRegisterStation rTool:
                                    rTool.RegisterStation(station);
                                    break;
                            }

                        }
                        station.RegisterViewDisplay();
                        station.ShowDisplayChangedEvent += Station_ShowDisplayChangedEvent;
                    }
                    _project.RunThread();
                    IsLoaded = true;
                    OnUpdateProjectUI();
                    LogNetHelper.Log("项目载入成功！");
                }
            }
            catch
            {
                IsLoaded = false;
                var msg = "视觉项目载入失败!";
                LogNetHelper.Log(msg);
                LogUI.AddLog(msg);
                throw;
            }
        }

        /// <summary>
        /// 关闭项目
        /// </summary>
        public void CloseProject()
        {
            if (!IsLoaded) return;
            foreach (var station in _project.StationList)
            {
                station.ShowDisplayChangedEvent -= Station_ShowDisplayChangedEvent;
                station.Close();
            }
            _project.Close();
            IsLoaded = false;
        }
        #endregion

        #region 【工位工具】
        /// <summary>
        /// 添加工位
        /// </summary>
        /// <param name="station"></param>
        /// <returns></returns>
        public bool AddStation(Station station)
        {
            if (!IsLoaded) return false;
            if (_project == null)
                return false;

            if (!_project.AddStation(station))
            {
                return false;
            }
            try
            {
                station.RegisterViewDisplay();
                station.ShowDisplayChangedEvent += Station_ShowDisplayChangedEvent;
                OnUpdateProjectUI();
                return true;
            }
            catch (Exception ex)
            {
                LogUI.AddLog(ex.Message);
            }
            return false;
        }

        /// <summary>
        /// 删除工位
        /// </summary>
        /// <param name="station"></param>
        public void DeleteStation(Station station)
        {
            if (!IsLoaded) return;
            if (_project == null)
                return;

            if (MessageBox.Show("是否确定删除此工位，此过程可能导致程序无法正常运行", "重要提示",
                   MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.No)
                return;

            if (_project.DeleteStation(station))
            {
                OnUpdateProjectUI();
            }
        }

        /// <summary>
        /// 工位重命名
        /// </summary>
        /// <param name="station"></param>
        /// <param name="newName"></param>
        public void RenameStation(Station station, string newName)
        {
            if (!IsLoaded) return;
            if (_project == null)
                return;

            if (_project.RenameStation(station, newName))
            {
                OnUpdateProjectUI();
            }
        }

        /// <summary>
        /// 添加工具 
        /// </summary>
        /// <param name="station"></param>
        /// <param name="tool"></param>
        public void AddTool(Station station, ToolBase tool)
        {
            if (!IsLoaded) return;
            if (_project == null || tool == null)
                return;

            if (!_project.StationExist(station)) return;
            station.AddTool(tool);
            OnUpdateProjectUI();
        }

        /// <summary>
        /// 删除工具
        /// </summary>
        /// <param name="station"></param>
        /// <param name="tool"></param>
        /// <returns></returns>
        public void DeleteTool(Station station, ToolBase tool)
        {
            if (!IsLoaded) return;
            if (_project == null || tool == null)
                return;
            if (MessageBox.Show("是否确定删除当前工具，此过程可能导致程序无法正常运行", "重要提示",
                   MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.No)
                return;

            if (!_project.StationExist(station)) return;
            station.DeleteTool(tool);
            OnUpdateProjectUI();
        }

        /// <summary>
        /// 删除所有工具
        /// </summary>
        /// <param name="station"></param>
        public void DeleteAllTool(Station station)
        {
            if (!IsLoaded) return;
            if (_project == null || station == null)
                return;

            if (MessageBox.Show("是否确定删除所有工具，此过程可能导致程序无法正常运行", "重要提示",
                   MessageBoxButtons.YesNo, MessageBoxIcon.Warning)
               == DialogResult.No)
                return;

            station.RemoveAllTool();
            OnUpdateProjectUI();
        }

        /// <summary>
        /// 工具重命名
        /// </summary>
        /// <param name="station"></param>
        /// <param name="tool"></param>
        /// <param name="newName"></param>
        public void RenameTool(Station station, ToolBase tool, string newName)
        {
            if (!IsLoaded) return;
            if (_project == null || tool == null)
                return;

            if (!_project.StationExist(station)) return;
            station.RenameTool(tool, newName);
            OnUpdateProjectUI();
        }

        /// <summary>
        /// 工具上移
        /// </summary>
        /// <param name="station"></param>
        /// <param name="tool"></param>
        public void UpTool(Station station, ToolBase tool)
        {
            if (station == null || tool == null) return;
            var index = station.ToolList.FindIndex(x => x.Name == tool.Name);
            if (index <= 0) return;
            station.ToolList.RemoveAt(index);
            station.ToolList.Insert(index - 1, tool);
            OnUpdateProjectUI();
        }

        /// <summary>
        /// 工具下移
        /// </summary>
        /// <param name="station"></param>
        /// <param name="tool"></param>
        public void DownTool(Station station, ToolBase tool)
        {
            if (station == null || tool == null) return;
            var index = station.ToolList.FindIndex(x => x.Name == tool.Name);
            if (index >= station.ToolList.Count - 1) return;
            station.ToolList.RemoveAt(index);
            station.ToolList.Insert(index + 1, tool);
            OnUpdateProjectUI();
        }

        /// <summary>
        /// 工位上移
        /// </summary>
        /// <param name="station"></param>
        public void UpStation(Station station)
        {
            if (station == null) return;
            var index = _project.StationList.FindIndex(x => x.Name == station.Name);
            if (index <= 0) return;
            _project.StationList.RemoveAt(index);
            _project.StationList.Insert(index - 1, station);
            OnUpdateProjectUI();
        }

        /// <summary>
        /// 工位下移
        /// </summary>
        /// <param name="station"></param>
        public void DownStation(Station station)
        {
            if (station == null) return;
            var index = _project.StationList.FindIndex(x => x.Name == station.Name);
            if (index >= _project.StationList.Count - 1) return;
            _project.StationList.RemoveAt(index);
            _project.StationList.Insert(index + 1, station);
            OnUpdateProjectUI();
        }
        #endregion

        #region 【事件触发器】
        /// <summary>
        /// project改变事件
        /// </summary>
        public void OnUpdateProjectUI()
        {
            if (UpdateProjectUIEvent == null) return;
            var e = UpdateProjectUIEvent;
            e.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// 保存project前置事件
        /// </summary>
        private void OnBeforeSaveProject()
        {
            if (BeforeSaveProjectEvent == null)
                return;
            var e = BeforeSaveProjectEvent;
            e.Invoke(this, EventArgs.Empty);
        }

        private void Station_ShowDisplayChangedEvent(object sender, StationShowChangedEventArgs e)
        {
            if (UcStationChangedEvent != null)
            {
                UcStationChangedEvent(this, e);
            }
        }
        #endregion

    }
}