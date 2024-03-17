using Vision.Stations;

namespace Vision.Tools.Interfaces
{
    /// <summary>
    /// 康耐视vpp接口，所有需要保存和加载vpp的工具都要继承此接口
    /// </summary>
    public interface IVpp
    {
        /// <summary>
        /// vpp是否加载完成
        /// </summary>
        bool IsLoaded { get; }

        /// <summary>
        /// vpp的路径
        /// </summary>
        string ToolPath { get; set; }

        /// <summary>
        /// 加载vpp
        /// </summary>
        void LoadVpp();

        /// <summary>
        /// 保存vpp
        /// </summary>
        void SaveVpp();

        /// <summary>
        /// 创建新的vpp
        /// </summary>
        void CreateVpp();

        /// <summary>
        /// 将此vpp注册到station
        /// </summary>
        /// <param name="station"></param>
        void RegisterStation(Station station);
    }
}
