using System;
using HslCommunication.LogNet;

namespace Vision.Core
{
    /// <summary>
    /// 日志的静态类 
    /// </summary>
    public class LogNetHelper
    {
        private static readonly ILogNet logNetDay = new LogNetDateTime(AppDomain.CurrentDomain.BaseDirectory + "Logs\\", GenerateMode.ByEveryDay);
        private static readonly ILogNet logNetOne = new LogNetSingle(AppDomain.CurrentDomain.BaseDirectory + "Logs\\log.txt");

        /// <summary>
        /// 写入一条调试日志
        /// </summary>
        /// <param name="msg">日志内容</param>
        /// <param name="isOneFile">
        /// 是否写入到一个单一日志中 默认false
        /// <para>如果为true 则写入到每天的更新日志中</para>
        /// </param>
        public static void Log(string msg, bool isOneFile = false)
        {

            if (isOneFile)
                logNetOne.WriteDebug(msg);
            else
                logNetDay.WriteDebug(msg);
        }
    }
}
