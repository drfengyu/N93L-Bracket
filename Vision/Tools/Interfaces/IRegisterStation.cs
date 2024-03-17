using Vision.Stations;

namespace Vision.Tools.Interfaces
{
    /// <summary>
    /// 注册station的接口
    /// 所有工具类中需要用到station类的可以继承此接口
    /// 它有一个station的注册方法
    /// </summary>
    public interface IRegisterStation
    {
        /// <summary>
        /// 将此矩阵注册到station
        /// </summary>
        /// <param name="station"></param>
        void RegisterStation(Station station);
    }
}
