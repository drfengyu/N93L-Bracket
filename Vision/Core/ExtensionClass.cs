using System;
using System.ComponentModel;
using System.Reflection;
using System.Text;
using System.Windows.Forms;

namespace Vision.Core
{
    /// <summary>
    /// Exception的扩展方法
    /// </summary>
    public static class ExtensionClass
    {
        /// <summary>
        /// 将exception的信息显示在messagebox上
        /// </summary>
        /// <param name="ex"></param>
        public static void MsgBox(this Exception ex)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("**************************** [ Exception ] ****************************");
            sb.Append(Environment.NewLine);
            sb.Append(DateTime.Now.ToString("G"));
            sb.Append(Environment.NewLine);
            sb.Append(ex.Message);
            sb.Append(Environment.NewLine);
            sb.Append(ex.Source);
            sb.Append(Environment.NewLine);
            sb.Append(ex.StackTrace);
            sb.Append(Environment.NewLine);
            sb.Append("**************************** [ Exception ] ****************************");
            MessageBox.Show(sb.ToString(), "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        /// <summary>
        /// MessageBox扩展方法
        /// </summary>
        /// <param name="message"></param>
        public static void MsgBox(this string message)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(Environment.NewLine);
            sb.Append(message);
            sb.Append(Environment.NewLine);
            MessageBox.Show(sb.ToString(), "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        /// <summary>
        /// 获取Enum成员的Description的描述 </summary>
        /// <param name="emun"></param>
        /// <returns></returns>
        public static string GetEmunDescription(this Enum emun)
        {
            Type type = emun.GetType();
            MemberInfo[] member = type.GetMember(emun.ToString());
            if (member.Length != 0)
            {
                object[] customAttributes = member[0].GetCustomAttributes(typeof(DescriptionAttribute), inherit: false);
                if (customAttributes.Length != 0)
                {
                    return ((DescriptionAttribute)customAttributes[0]).Description;
                }
            }
            return emun.ToString();
        }
    }
}
