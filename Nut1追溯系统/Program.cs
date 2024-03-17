using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Configuration;

namespace 卓汇数据追溯系统
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            //Application.Run(new MainFrm());

            //Application.EnableVisualStyles();
            //Application.SetCompatibleTextRenderingDefault(false);
            //bool isload = Convert.ToBoolean(ConfigurationManager.AppSettings["IsLoad"]);//是否需要用户load界面
            //bool islogin = Convert.ToBoolean(ConfigurationManager.AppSettings["IsLogin"]);//是否需要登录界面
            //string language = Convert.ToString(ConfigurationManager.AppSettings["Language"]);

            //System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("zh");
            //LoadForm.Run(new MainFrm(), true, false);//调用Forms.dll的LoadForm的Run方法
            if (System.Diagnostics.Process.GetProcessesByName(System.Diagnostics.Process.GetCurrentProcess().ProcessName).Length > 1)
            {
                MessageBox.Show("程序运行中，不需要再次开启");
                return;
            }
            Application.Run(new MainFrm());
        }
    }
}
