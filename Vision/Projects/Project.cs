using PlcComm;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Vision.Core;
using Vision.Stations;

namespace Vision.Projects
{
    [Serializable]
    public class Project
    {
        private List<Station> _stations;    //工位集合
        private ImageConfig _imageConfig;   //图像保存配置
        private SystemConfig _systemConfig;  //系统配置
        private PointD _offset;

        [NonSerialized]
        private bool _imageThreadFlag;

        [NonSerialized]
        private bool _systemThreadFlag;

        public Project()
        {
            RunThread();
        }

        [field: NonSerialized]
        public MxPlc MxPlc { get; set; }

        /// <summary>
        /// 工位列表
        /// </summary>
        public List<Station> StationList
        {
            get => _stations ?? (_stations = new List<Station>());
            private set => _stations = value;
        }

        /// <summary>
        /// 图像配置
        /// </summary>
        public ImageConfig ImageConfig
        {
            get => _imageConfig ?? (_imageConfig = new ImageConfig());
            set => _imageConfig = value;
        }

        /// <summary>
        /// 系统配置
        /// </summary>
        public SystemConfig SystemConfig
        {
            get => _systemConfig ?? (_systemConfig = new SystemConfig());
            set => _systemConfig = value;
        }


        /// <summary>
        /// 系统补偿
        /// </summary>
        public PointD Offset
        {
            get => _offset ?? (_offset = new PointD());
            set => _offset = value;
        }


        public Station this[string name]
        {
            get
            {
                return StationList.FirstOrDefault(s => s.Name == name);
            }
        }

        public Station this[int index] => StationList[index];

        /// <summary>
        /// 注册plc
        /// </summary>
        /// <param name="plc"></param>
        public void RegisterPlc(Melsoft_PLC_TCP2 plc)
        {
            if (plc == null)
            {
                throw new Exception("plc加载失败！");
            }
            MxPlc = new MxPlc(plc);
        }

        /// <summary>
        /// 一些循环
        /// </summary>
        public void RunThread()
        {
            _imageThreadFlag = true;
            _systemThreadFlag = true;
            Task.Run(ImageDelete);
            Task.Run(HeartBeat);
        }

        /// <summary>
        /// 添加工位
        /// </summary>
        /// <param name="station"></param>
        /// <returns></returns>
        public bool AddStation(Station station)
        {
            if (station == null)
                return false;

            if (!StationExist(station))
            {
                StationList.Add(station);
                LogUI.AddLog($"[{station.Name}]新建成功");
                return true;
            }
            return false;
        }

        /// <summary>
        /// 重命名工位
        /// </summary>
        /// <param name="station"></param>
        /// <param name="newName"></param>
        /// <returns></returns>
        public bool RenameStation(Station station, string newName)
        {
            if (station == null)
                return false;

            if (StationExist(station))
            {
                string oldPath = station.StationPath;

                station.StationPath = ProjectManager.Instance.ProjectDir + $"\\{newName}";
                station.Name = newName;
                //Local.RenameDirectory(oldPath, station.StationPath);
                LogUI.AddLog($"[{station.Name}]重命名成功");
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 删除工位
        /// </summary>
        /// <param name="station"></param>
        /// <returns></returns>
        public bool DeleteStation(Station station)
        {
            if (station == null) return false;
            if (StationExist(station))
            {
                StationList.Remove(station);
                LogUI.AddLog($"[{station.Name}]移除成功");
                return true;
            }
            return false;
        }

        /// <summary>
        /// 生成默认工位名称
        /// </summary>
        /// <returns></returns>
        public string GenDefaultStationName()
        {
            int m = 1;
            string name = "工位" + m.ToString();
            foreach (Station s in StationList)
            {
                if (s.Name == name)
                {
                    m++;
                    name = "工位" + m.ToString();
                }
            }
            return "工位" + m.ToString();
        }

        /// <summary>
        /// 工位是否存在
        /// </summary>
        /// <param name="station"></param>
        /// <returns></returns>
        public bool StationExist(Station station)
        {
            if (StationList == null) { return false; }
            foreach (Station s in StationList)
            {
                if (s.Name == station.Name)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 工位是否存在（根据名称）
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool StationExsit(string name)
        {
            if (StationList == null) { return false; }
            var station = StationList.Find(s => s.Name == name);
            if (station != null)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 关闭项目
        /// </summary>
        public void Close()
        {
            if (StationList != null && StationList.Count > 0)
            {
                foreach (Station station in StationList)
                {
                    station.Close();
                }
                StationList.Clear();
            }
            _imageThreadFlag = false;
            _systemThreadFlag = false;
        }

        /// <summary>
        /// 异步循环删除图像文件
        /// </summary>
        private void ImageDelete()
        {
            var config = ImageConfig;
            while (_imageThreadFlag)
            {
                //按图像保存的时间进行删除
                if (config.IsDeleteByTime)
                {
                    DateTime now = DateTime.Now;
                    string rootDir = config.SaveImageDir;
                    //工位 文件夹
                    var dirs = Directory.GetDirectories(rootDir);
                    foreach (var x in dirs)
                    {
                        //OK NG
                        var stations = Directory.GetDirectories(x);
                        foreach (var station in stations)
                        {
                            //日期文件夹
                            var dates = Directory.GetDirectories(station);
                            foreach (var date in dates)
                            {
                                //每天文件夹路径 -- 最终判断路径
                                if ((now - Directory.GetCreationTime(date)).TotalDays >= config.DeleteDayTime)
                                {
                                    Directory.Delete(date, true);
                                }
                            }
                        }
                    }
                }

                //按图像文件夹的大小进行删除
                if (config.IsDeleteBySize)
                {
                    string rootDir = config.SaveImageDir;
                    //获取图像文件夹的大小
                    var size = Local.GetFolderSize(rootDir);
                    int msize = (int)(size / 1024 / 1024);
                    if (msize >= config.DeleteSize)
                    {
                        //工位文件夹
                        var dirs = Directory.GetDirectories(rootDir);

                        foreach (var x in dirs)
                        {
                            //OK NG 文件夹
                            var stations = Directory.GetDirectories(x);
                            foreach (var station in stations)
                            {
                                //日期文件夹
                                var dates = Directory.GetDirectories(station);
                                foreach (var date in dates)
                                {
                                    Directory.Delete(date, true);
                                }
                            }
                        }
                    }
                }

                Thread.Sleep(1000);
            }
        }

        /// <summary>
        /// 心跳
        /// </summary>
        /// <exception cref="NotImplementedException"></exception>
        private void HeartBeat()
        {
            var config = SystemConfig;
            short num = 0;
            //判断是否连接
            if (MxPlc != null && MxPlc.IsConnected)
            {
                while (_systemThreadFlag)
                {
                    //心跳 
                    MxPlc.WriteShort(config.HeartAddress, num);
                    if (num == 0)
                    {
                        num = 1;
                    }
                    else if (num == 1)
                    {
                        num = 0;
                    }

                    //联机状态
                    MxPlc.WriteShort(config.OnlineAddress, 1);

                    Thread.Sleep(1000);
                }
            }
        }
    }
}