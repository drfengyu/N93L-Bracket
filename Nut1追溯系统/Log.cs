using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;
using System.Diagnostics;
using System.Threading;

namespace 卓汇数据追溯系统
{
    public class Log
    {
        private static string logPath = string.Empty;
        private static string logPath2 = System.AppDomain.CurrentDomain.BaseDirectory + "操作记录\\";
        private static object _lock = new object();

        public static string LogPath
        {
            get
            {
                if (logPath == string.Empty)
                {
                    logPath = System.AppDomain.CurrentDomain.BaseDirectory + "日志文件\\";
                }
                return logPath;
            }
            set { Log.logPath = value; }
        }

        public static void WriteLog(string text)
        {
            System.IO.StreamWriter sw = null;
            if (!Directory.Exists(LogPath))
            {
                Directory.CreateDirectory(LogPath);
            }
            if (!Directory.Exists(logPath2))
            {
                Directory.CreateDirectory(logPath2);
            }
            string fileFullFileName = LogPath + DateTime.Now.ToString("yyyyMMdd") + ".Log";
            string fileFullFileName2 = logPath2 + DateTime.Now.ToString("yyyyMMdd") + ".csv";
            if (!File.Exists(fileFullFileName2))
            {
                using (sw = new StreamWriter(fileFullFileName2, true, Encoding.Default))
                {
                    string str = "Time,Data";
                    sw.WriteLine(str);
                }
            }
                lock (_lock)
            {
                try
                {
                    using (sw = new StreamWriter(fileFullFileName, true, Encoding.Default))
                    {
                        sw.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") +": "+ text);
                    }
                    using (sw = new StreamWriter(fileFullFileName2, true, Encoding.Default))
                    {
                        text = text.Replace(",", "|").Replace("\r\n", "").Replace("\r", "").Replace("\n", "").Replace("\t", "");
                        sw.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "," + text);
                    }
                    sw.Close();
                    sw.Dispose();
                }
                catch
                {}
            }
        }

        public static void WriteOEELog(string text)
        {
            System.IO.StreamWriter sw = null;
            if (!Directory.Exists(LogPath))
            {
                Directory.CreateDirectory(LogPath);
            }
            string fileFullFileName = LogPath + "OEE-Demo" + ".Log";
            lock (_lock)
            {
                try
                {
                    using (sw = new StreamWriter(fileFullFileName, true, Encoding.Default))
                    {
                        sw.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss: ") + text);
                    }

                    sw.Close();
                    sw.Dispose();
                }
                catch
                {

                }
            }
        }
        public static void WriteLog(string text, string LogPath1)
        {
            System.IO.StreamWriter sw = null;
            if (!Directory.Exists(LogPath1))
            {
                Directory.CreateDirectory(LogPath1);
            }
            string fileFullFileName = LogPath1 + DateTime.Now.ToString("yyyyMMdd") + ".csv";
            if (!File.Exists(fileFullFileName))
            {
                using (sw = new StreamWriter(fileFullFileName, true, Encoding.Default))
                {
                    string str = "Time,Procuct,full_sn,Fixture_id,Start_Time,Weld_start_time,Weld_stop_time,Stop_time,Weld_wait_ct,Actual_weld_ct, power_ll, power_ul, pattern_type, frequency, linear_speed, spot_size, pulse_energy,power, filling_pattern, hatch,status,Result,正焊-焊前结果,正焊-获参结果,侧焊-焊前结果,侧焊-获参结果";
                    if (LogPath1.Contains("System_ini"))
                    {
                        str = "记录时间,异常代码,触发时间,模组代码,上传状态,运行状态,故障描述,时长";
                    }
                    if (LogPath1.Contains("完整数据"))
                    {
                        str = "Time,Procuct,full_sn,Fixture_id,Start_Time,Weld_start_time,Weld_stop_time,Stop_time,Weld_wait_ct,正焊-焊前结果,正焊-获参结果,侧焊-焊前结果,侧焊-获参结果";
                    }
                    if (LogPath1.Contains("OEE_抛料日志数据"))
                    {
                        str = "记录时间,SN,SystemType,uptime,JSONBody,Error,Frenquency,上传状态";
                    }
                    if (LogPath1.Contains("OEE_小料抛料计数数据"))
                    {
                        str = "时间,合计抛料数量,总共抛料数量,UA抛料数量,LA抛料数量,上传状态";
                    }
                    if (LogPath1.Contains("band 获取 HSG 结果"))
                    {
                        str = "time,Band,版本";
                    }
                    if (LogPath1.Contains("UpLoad_HeartBeat"))
                    {
                        str = "记录时间,故障状态,故障代码,故障信息,上传状态";
                    }
                    if (LogPath1.Contains("Trace_Logs_U_Bracket数据"))
                    {
                        str = "Time,U_Bracket,full_sn,fixture,uut_start,laser_start_time,laser_stop_time,uut_stop,tossing_item,上传结果,物料结果";
                    }
                    if (LogPath1.Contains("OEE_Default"))
                    {
                        str = "Time,Procuct,full_sn,Fixture_id,Start_Time,EndTime,Status,ActualCT,SwVersion,DefectCode,SendStatus";
                    }
                    if (LogPath1.Contains("OEE_MQTT_Default数据"))
                    {
                        str = "Time,Procuct,GUID,EMT,SerialNumber,BGBarcode,Fixture,StartTime,EndTime,Status,ActualCT,SwVersion,ScanCount,ErrorCode,PFErrorCode,Cavity,ClientPcName,MAC,IP,EventTime,SendResult";
                    }
                    if (LogPath1.Contains("OEE_MQTT_Default数据"))
                    {
                        str = "Time,Procuct,GUID,EMT,SerialNumber,BGBarcode,Fixture,StartTime,EndTime,Status,ActualCT,SwVersion,ScanCount,ErrorCode,PFErrorCode,Cavity,ClientPcName,MAC,IP,EventTime,SendResult";
                    }
                    if (LogPath1.Contains("日志文件"))
                    {
                        str = "Time,SystemInfo";
                    }
                    if (LogPath1.Contains("备件更换记录"))
                    {
                        str = "类别,品名,规格,标准寿命,实际使用次数,上次更换时间,当前时间,更换原因,处理生技";
                    }
                    if (LogPath1.Contains("OEE_demo"))
                    {
                        str = "记录时间,URL路径,Data";
                    }
                    if (LogPath1.Contains("产品出流道记录"))
                    {
                        str = "记录时间,产品SP码,流道";
                    }
                    if (LogPath1.Contains("产品各工站CT记录"))
                    {
                        str = "记录时间,CT,名称";
                    }
                    if (LogPath1.Contains("IFactory前站校验"))
                    {
                        str = "记录时间,SN,返回结果,异常信息";
                    }
                    if (LogPath1.Contains("JGP上传次数校验"))
                    {
                        str = "记录时间,SN,返回结果,异常信息";
                    }
                    if (LogPath1.Contains("参数修改记录"))
                    {
                        str = "用户名称,权限等级,登录时间,参数名称,原参数,现参数,地区";
                    }
                    if (LogPath1.Contains("焊接结果"))
                    {
                        str = "Time,SN,fixtrue,焊接总结果,正焊1,正焊2,侧焊1,侧焊2,侧焊3,正焊-焊前结果,正焊-获参结果,侧焊-焊前结果,侧焊-获参结果";
                    }
                    if (LogPath1.Contains("PLC上传记录"))
                    {
                        str = "Time,SN,Fixture,TP_空运行,TP_HSG扫码屏蔽,TP_HSG重码屏蔽,TP_HSG前站屏蔽,TP_TRACE上传屏蔽,TP_上料机-HSG屏蔽,TP_上料机-料盘屏蔽,TP_主机-治具屏蔽,TP_主机-小料感应屏蔽,TP_主机-治具CCD屏蔽,TP_主机-上小料动作屏蔽,TP_主机-小黄屏蔽,TP_主机-正面焊屏蔽,TP_主机-侧面焊屏蔽,TP_主机-蜂鸣器屏蔽,TP_主机-安全门屏蔽,TP_主机-小黄安全门屏蔽,TP_上料机 - 蜂鸣器屏蔽,TP_上料机-安全门屏蔽,状态字_ST5-盖板,状态字1_ST5-治具,状态字2_ST5-治具,状态字3_ST5-治具,ST5上传-正焊焊前结果,ST5上传-正焊焊点1,ST5上传-正焊焊点2,ST5上传-正焊获参结果,ST5上传-侧焊焊前结果,ST5上传-侧焊焊点1,ST5上传-侧焊焊点2,ST5上传-侧焊焊点3,ST5上传-侧焊获参结果,ST5上传-工站状态,ST5上传-TRACE结果,ST5上传-PDCA结果,ST5上传-PDCA图片结果,ST5上传-OEE过站结果,ST5上传-焊接参数结果,ST5上传-侧焊小黄ID";

                    }
                    if (LogPath1.Contains("超时CT记录"))
                    {
                        str = "SN,CT";
                    }
                    if (LogPath1.Contains("MQTT返回NG"))
                    {
                        str = "GUID,Result,Error_code";
                    }
                    if (LogPath1.Contains("手动上传OEE_DT"))
                    {
                        str = "guid,EMT,PoorNum,TotalNum,Status,ErrorCode,EventTime,ModuleCode,ClientPcName,Mac,IP";
                    }
                    sw.WriteLine(str);
                }
            }
            lock (_lock)
            {
                try
                {
                    using (sw = new StreamWriter(fileFullFileName, true, Encoding.Default))
                    {
                        sw.WriteLine(text);
                    }
                    sw.Close();
                    sw.Dispose();
                }
                catch
                {
                }
            }
        }
        public static void WriteCSV(string text, string LogPath1)
        {
            System.IO.StreamWriter sw = null;
            if (!Directory.Exists(LogPath1))
            {
                Directory.CreateDirectory(LogPath1);
            }
            string fileFullFileName = LogPath1 + DateTime.Now.ToString("yyyyMMdd") + ".csv";
            if (!File.Exists(fileFullFileName))
            {
                using (sw = new StreamWriter(fileFullFileName, true, Encoding.Default))
                {
                    string str = "Time,Procuct,full_sn,Fixture_id,Start_Time,Weld_start_time,Weld_stop_time,Stop_time,Weld_wait_ct,Actual_weld_ct, power_ll, power_ul, pattern_type, frequency, linear_speed, spot_size, pulse_energy,power, filling_pattern, hatch,status,Result";
                    if (LogPath1.Contains("System_ini"))
                    {
                        str = "记录时间,异常代码,触发时间,模组代码,上传状态,运行状态,故障描述,时长";
                    }
                    if (LogPath1.Contains("MQTT 返回已上传过的数据"))
                    {
                        str = "GUID,Result,ErrorCode";
                    }
                    if (LogPath1.Contains("OEE_抛料日志数据"))
                    {
                        str = "记录时间,SN,SystemType,uptime,JSONBody,Error,Frenquency,上传状态";
                    }
                      if (LogPath1.Contains("OEE_小料抛料计数数据"))
                    {
                        str = "时间,合计抛料数量,总共抛料数量,UA抛料数量,LA抛料数量,上传状态";
                    }
                    if (LogPath1.Contains("UpLoad_HeartBeat"))
                    {
                        str = "记录时间,故障状态,故障代码,故障信息,上传状态";
                    }
                    if (LogPath1.Contains("OEE_Default"))
                    {
                        str = "Time,Procuct,full_sn,Fixture_id,Start_Time,EndTime,Status,ActualCT,SwVersion,DefectCode,SendStatus";
                    }
                    if (LogPath1.Contains("日志文件"))
                    {
                        str = "Time,SystemInfo";
                    }
                    if (LogPath1.Contains("备件更换记录"))
                    {
                        str = "类别,品名,规格,标准寿命,实际使用次数,上次更换时间,当前时间,更换原因,处理生技";
                    }
                    if (LogPath1.Contains("OEE_demo"))
                    {
                        str = "记录时间,URL路径,Data";
                    }
                    if (LogPath1.Contains("产品出流道记录"))
                    {
                        str = "记录时间,产品SP码,流道";
                    }
                    if (LogPath1.Contains("产品各工站CT记录"))
                    {
                        str = "记录时间,CT,名称";
                    }
                    if (LogPath1.Contains("IFactory前站校验"))
                    {
                        str = "记录时间,SN,返回结果,异常信息";
                    }
                    if (LogPath1.Contains("Phase厂区校验"))
                    {
                        str = "记录时间,SN,返回结果,异常信息";
                    }
                    if (LogPath1.Contains("JGP上传次数校验"))
                    {
                        str = "记录时间,SN,返回结果,异常信息";
                    }
                    if (LogPath1.Contains("参数修改记录"))
                    {
                        str = "用户名称,权限等级,登录时间,参数名称,原参数,现参数";
                    }
                    if (LogPath1.Contains("焊接总结果与小料结果记录"))
                    {
                        str = "时间,SN,小料1,小料2,小料3,焊接总结果";
                    }
                    if (LogPath1.Contains("PLC_DT"))
                    {
                        str = "大状态,状态细节字,描述";
                    }
                    sw.WriteLine(str);
                }
            }
            lock (_lock)
            {
                try
                {
                    using (sw = new StreamWriter(fileFullFileName, true, Encoding.Default))
                    {
                        sw.WriteLine(text);
                    }
                    sw.Close();
                    sw.Dispose();
                }
                catch
                {
                    //foreach (Process process in System.Diagnostics.Process.GetProcesses())
                    //{
                    //    if (process.ProcessName.ToUpper().Equals("wps"))
                    //        process.Kill();    //杀进程
                    //}
                    //GC.Collect();
                    //Thread.Sleep(200);
                    //using (sw = System.IO.File.AppendText(fileFullFileName))
                    //{

                    //    sw.WriteLine(DateTime.Now.ToString("yyyy-MM-dd,HH:mm:ss:,") + text);

                    //}
                }
            }
        }

        public static void WriteCSV_NG(string text, string LogPath1)
        {
            System.IO.StreamWriter sw = null;
            if (!Directory.Exists(LogPath1))
            {
                Directory.CreateDirectory(LogPath1);
            }
            string fileFullFileName = LogPath1 + "sendNG.csv";
            if (!File.Exists(fileFullFileName))
            {
                using (sw = new StreamWriter(fileFullFileName, true, Encoding.Default))
                {
                    string str = "Time,Procuct,full_sn,Fixture_id,Start_Time,Weld_start_time,Weld_stop_time,Stop_Time,Weld_wait_ct,Actual_weld_ct,Status";
                    sw.WriteLine(str);
                }
            }
            lock (_lock)
            {
                try
                {
                    using (sw = new StreamWriter(fileFullFileName, true, Encoding.Default))
                    {
                        sw.WriteLine(text);
                    }
                    sw.Close();
                    sw.Dispose();
                }
                catch
                {
                }
            }
        }

        public static void WriteCSV_NUM(string text)
        {
            System.IO.StreamWriter sw = null;
            if (!Directory.Exists(System.AppDomain.CurrentDomain.BaseDirectory + "\\月产量数据统计\\"))
            {
                Directory.CreateDirectory(System.AppDomain.CurrentDomain.BaseDirectory + "\\月产量数据统计\\");
            }
            string filePath = System.AppDomain.CurrentDomain.BaseDirectory + "\\月产量数据统计\\" + DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString() + ".csv";
            if (!File.Exists(filePath))
            {
                using (sw = new StreamWriter(filePath, true, Encoding.Default))
                {
                    //string str = "Time,OK产品数量,NG产品数量,良率,稼动率";
                    string str = "Time,OK产品数量,NG产品数量";
                    sw.WriteLine(str);
                }
            }
            lock (_lock)
            {
                try
                {
                    using (sw = new StreamWriter(filePath, true, Encoding.Default))
                    {
                        sw.WriteLine(text);
                    }
                    sw.Close();
                    sw.Dispose();
                }
                catch
                {
                }
            }
        }
        public static void WriteCSV_PDCA(string text)
        {
            System.IO.StreamWriter sw = null;
            if (!Directory.Exists(System.AppDomain.CurrentDomain.BaseDirectory + "\\PDCA上传成功数据统计\\"))
            {
                Directory.CreateDirectory(System.AppDomain.CurrentDomain.BaseDirectory + "\\PDCA上传成功数据统计\\");
            }
            string filePath = System.AppDomain.CurrentDomain.BaseDirectory + "\\PDCA上传成功数据统计\\" + DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString() + ".csv";
            if (!File.Exists(filePath))
            {
                using (sw = new StreamWriter(filePath, true, Encoding.Default))
                {
                    //string str = "Time,OK数量,NG数量,良率,稼动率";
                    string str = "Time,UA数量,LA数量";
                    sw.WriteLine(str);
                }
            }
            lock (_lock)
            {
                try
                {
                    using (sw = new StreamWriter(filePath, true, Encoding.Default))
                    {
                        sw.WriteLine(text);
                    }
                    sw.Close();
                    sw.Dispose();
                }
                catch
                {
                }
            }
        }

        public static void WriteCSV_DiscardLog(string text)
        {
            System.IO.StreamWriter sw = null;
            if (!Directory.Exists(@"E:\装机软件\系统配置\OEE_抛料日志数据\" + DateTime.Now.ToString("yyyyMM")))
            {
                Directory.CreateDirectory(@"E:\装机软件\系统配置\OEE_抛料日志数据\" + DateTime.Now.ToString("yyyyMM"));
            }
            string filePath = @"E:\装机软件\系统配置\OEE_抛料日志数据\" + DateTime.Now.ToString("yyyyMM") + "\\" + DateTime.Now.ToString("yyyyMMdd") + ".csv";
            if (!File.Exists(filePath))
            {
                using (sw = new StreamWriter(filePath, true, Encoding.Default))
                {
                    //string str = "Time,OK数量,NG数量,良率,稼动率";
                    string str = "系统类型,上传时间,工站,机台号,SN,上传数据,报错信息,上传次数";
                    sw.WriteLine(str);
                }
            }
            lock (_lock)
            {
                try
                {
                    using (sw = new StreamWriter(filePath, true, Encoding.Default))
                    {
                        sw.WriteLine(text);
                    }
                    sw.Close();
                    sw.Dispose();
                }
                catch
                {
                }
            }
        }
    }
}
