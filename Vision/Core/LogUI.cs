using System;
using System.Collections.Generic;
using System.Text;

namespace Vision.Core
{
    /// <summary>
    /// UI界面Log
    /// </summary>
    public class LogUI
    {
        private static readonly Queue<string> msgs = new Queue<string>();
        private static readonly Queue<string> toolMsg = new Queue<string>();

        private static readonly object _lock = new object();

        /// <summary>
        /// 添加项目的异常log
        /// </summary>
        /// <param name="ex"></param>
        public static void AddException(Exception ex)
        {
            lock(_lock)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("**************************** [ Exception ] ****************************");
                sb.Append(Environment.NewLine);
                sb.Append(ex.Message);
                sb.Append(Environment.NewLine);
                sb.Append(ex.Source);
                sb.Append(Environment.NewLine);
                sb.Append(ex.StackTrace);
                sb.Append(Environment.NewLine);
                sb.Append("**************************** [ Exception ] ****************************");
                msgs.Enqueue(sb.ToString());
            }
        }

        /// <summary>
        /// 添加项目的log
        /// </summary>
        /// <param name="message"></param>
        public static void AddLog(string message)
        {
            lock(_lock)
            {
                msgs.Enqueue(DateTime.Now.ToString("HH:mm:ss") + " :  " + message + "\r\n");
            }
        }

        /// <summary>
        /// 添加工具栏的log
        /// </summary>
        /// <param name="message"></param>
        public static void AddToolLog(string message)
        {
            lock(_lock)
            {
                toolMsg.Enqueue(DateTime.Now.ToString("HH:mm:ss") + " :  " + message + "\r\n");
            }
        }

        /// <summary>
        /// 显示整个项目的log
        /// </summary>
        /// <returns></returns>
        public static string GetLog()
        {
            lock(_lock)
            {
                if(msgs.Count > 0)
                {
                    return msgs.Dequeue();
                }
                return null;
            }
        }

        /// <summary>
        /// 显示工具栏的log
        /// </summary>
        /// <returns></returns>
        public static string GetToolLog()
        {
            lock(_lock)
            {
                if(toolMsg.Count > 0)
                {
                    return toolMsg.Dequeue();
                }
                return null;
            }
        }
    }
}
